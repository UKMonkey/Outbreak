using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Outbreak.Items.Containers;
using Psy.Core.Logging;
using Psy.Core.Serialization;
using Vortex.Interface;
using Vortex.Interface.Serialisation;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Properties;
using Vortex.Interface.World.Chunks;
using Vortex.Interface.World.Entities;

namespace Outbreak.Server.Persistance.File.Entites
{
    /**
     * Because of the threading - the conversion to the byte[] when saving must be done here
     * The good news is that we can at least palm off the pushing to the file to 
     * another thread meaning that disk access won't slow down the game!
     */
    public class EntityHandler : 
        Base.FileHandler<EntityLoader, EntitySaver>, IEntityLoader, IEntitySaver
    {
        private const string DataDir = "Entities";
        private readonly List<ChunkKey> _entitiesToLoad;
        private readonly List<string> _entitiesToDelete;
        private readonly Dictionary<int, EntitySaveData> _entitiesToSave;

#pragma warning disable 67
        public event EntityChunkKeyCallback OnEntityGenerated;
        public event EntitiesCallback OnEntityUpdated;
        public event EntityIdCallback OnEntityDeleted;
#pragma warning restore 67

        public event EntityChunkKeyCallback OnEntityLoaded;
        public event ChunkKeyCallback OnEntitiesUnavailable;

        private readonly IServer _server;
        private readonly IGame _game;

        private readonly string _runInstanceGuid;
        private readonly IInventoryCache _inventoryCache;

        public EntityHandler(IGame game, IServer server, IInventoryCache cache)
            :base(game, "Entity")
        {
            _entitiesToLoad = new List<ChunkKey>();
            _entitiesToDelete = new List<string>();
            _entitiesToSave = new Dictionary<int, EntitySaveData>();

            _server = server;
            _game = game;

            // take advantage of the fact that entity ids are unique per instance
            // so to make an entity id more unique - we add the instance id to the save name
            // this way when restarted and we start re-using the entity ids, we don't have a collision
            _runInstanceGuid = System.Guid.NewGuid().ToString();

            _inventoryCache = cache;
        }

        protected override void PerformSave()
        {
            var entitiesToDelete = new List<string>();
            var entitiesToSave = new List<EntitySaveData>();
            lock (this)
            {
                entitiesToDelete.AddRange(_entitiesToDelete);
                entitiesToSave.AddRange(_entitiesToSave.Values);

                _entitiesToDelete.Clear();
                _entitiesToSave.Clear();
            }

            if (entitiesToSave.Count > 0)
            {
                Saver.SaveEntities(entitiesToSave);
            }

            if (entitiesToDelete.Count > 0)
            {
                Saver.DeleteEntities(entitiesToDelete);
            }
        }

        protected override void PerformLoad()
        {
            var entitiesToLoad = new List<ChunkKey>();
            lock (this)
            {
                entitiesToLoad.AddRange(_entitiesToLoad);
                _entitiesToLoad.Clear();
            }

            if (entitiesToLoad.Count != 0)
                Loader.LoadEntities(entitiesToLoad);
        }

        public override void Init()
        {
            base.Init();

            Loader.OnEntitiesUnavailable += EntitiesUnavailable;
            Loader.OnEntityDeleted += EntitiesDeleted;
            Loader.OnEntityLoaded += EntitiesLoaded;
            Loader.OnEntityUpdated += EntitiesUpdated;

            Loader.DataDir = DataDir;
            Loader.InventoryCache = _inventoryCache;
        }

        public void LoadEntities(ChunkKey area)
        {
            Logger.Write(string.Format("Loading entities in {0}", area), LoggerLevel.Info);
            lock (this)
            {
                _entitiesToLoad.Add(area);
                Monitor.PulseAll(this);
            }
        }

        public void LoadEntities(List<ChunkKey> area)
        {
            foreach (var item in area)
            {
                Logger.Write(string.Format("Loading entities in {0}", item), LoggerLevel.Info);
            }
            lock(this)
            {
                _entitiesToLoad.AddRange(area);
                Monitor.PulseAll(this);
            }
        }

        private string GetTargetSaveFileForPlayer(string playerName)
        {
            var name = Path.Combine(Utils.GetRootSaveDirectory(Game), "Players");
            Directory.CreateDirectory(name);
            return Path.Combine(name, playerName);
        }

        private string GetEntityTargetSaveFilename(Entity entity)
        {
            if (entity.GetPlayerId() != null)
            {
                var player = entity.GetPlayer(Game.Engine);
                if (player == null)
                    return null;
                return GetTargetSaveFileForPlayer(player.PlayerName);
            }

            var chunk = _server.GetChunkKeyForWorldVector(entity.GetPosition());
            var name = Utils.GetPathForChunk(_game, chunk);
            name = Path.Combine(name, DataDir);

            Directory.CreateDirectory(name);

            return Path.Combine(name, _runInstanceGuid + entity.EntityId.ToString(CultureInfo.InvariantCulture));
        }

        protected bool ShouldSaveEntity(Entity entity)
        {
            if (_entitiesToSave.ContainsKey(entity.EntityId))
                return false;

            if (entity.GetStatic())
                return true;

            if (entity.GetPlayerId() != null)
                return true;
            return false;
        }

        protected byte[] GetEntityData(Entity entity)
        {
            var stream = new MemoryStream();
            stream.Write(entity.EntityTypeId);
            var properties = entity.NonDefaultProperties.Where(item => item.IsPersistant).ToList();

            stream.Write(properties, properties.Count);
            return stream.ToArray();
        }

        public Entity LoadPlayerEntity(string playerName)
        {
            var targetFilename = GetTargetSaveFileForPlayer(playerName);

            lock(this)
            {
                if (_entitiesToDelete.Contains(targetFilename))
                    return null;
            }

            var entity = Loader.LoadEntityFromFile(targetFilename);

            if (entity == null)
                return null;
            var key = _server.GetChunkKeyForWorldVector(entity.GetPosition());

            OnEntityLoaded(new List<Entity> {entity}, key);
            return entity;
        }

        public void SaveEntities(ICollection<Entity> entities)
        {
            var toSave = new Dictionary<int, EntitySaveData>();
            
            foreach (var entity in entities)
            {
                if (!ShouldSaveEntity(entity))
                    continue;
                
                var saveData = new EntitySaveData
                                {
                                    Targetname = GetEntityTargetSaveFilename(entity),
                                    Data = GetEntityData(entity)
                                };

                if (saveData.Targetname == null)
                    continue;

                toSave[entity.EntityId] = saveData;
            }

            if (toSave.Count == 0)
                return;

            lock (this)
            {
                foreach (var item in toSave)
                {
                    _entitiesToSave[item.Key] = item.Value;
                }
            }
        }

        public void DeleteEntity(Entity entity)
        {
            var fileName = GetEntityTargetSaveFilename(entity);
            if (fileName == null)
                return;
            
            lock(this)
            {
                _entitiesToDelete.Add(fileName);
                Monitor.PulseAll(this);
            }
        }

        private void EntitiesUnavailable(List<ChunkKey> keys)
        {
            if (OnEntitiesUnavailable != null)
                OnEntitiesUnavailable(keys);
        }

        private void EntitiesLoaded(List<Entity> entities, ChunkKey key)
        {
            if (OnEntityLoaded != null)
                OnEntityLoaded(entities, key);

            SaveEntities(entities);
        }

        private void EntitiesUpdated(List<Entity> entities)
        {
            if (OnEntityUpdated != null)
                OnEntityUpdated(entities);
        }

        private void EntitiesDeleted(List<int> deleted)
        {
            if (OnEntityDeleted != null)
                OnEntityDeleted(deleted);
        }
    }
}

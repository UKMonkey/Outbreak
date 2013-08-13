using System;
using System.Collections.Generic;
using System.IO;
using Outbreak.Items.Containers;
using Psy.Core.Logging;
using Psy.Core.Serialization;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Properties;
using Vortex.Interface.Serialisation;
using Vortex.Interface.World.Chunks;
using Vortex.Interface.World.Entities;

namespace Outbreak.Server.Persistance.File.Entites
{
    public class EntityLoader : Base.FileLoader
    {
        public event EntityChunkKeyCallback OnEntityLoaded;
        public event ChunkKeyCallback OnEntitiesUnavailable;

        public IInventoryCache InventoryCache { get; set; }

#pragma warning disable 67
        public event EntityChunkKeyCallback OnEntityGenerated;
        public event EntitiesCallback OnEntityUpdated;
        public event EntityIdCallback OnEntityDeleted;
#pragma warning restore 67

        public string DataDir { get; set; }


        public override void Dispose()
        {
        }


        public Entity LoadEntityFromFile(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
                return null;

            var stream = new FileStream(fileName, FileMode.Open);
            var type = stream.ReadShort();
            var properties = stream.ReadTraits<EntityProperty>();

            stream.Close();
            stream.Dispose();

            var entity = Game.EntityFactory.Get(type);
            entity.SetProperties(properties);
            System.IO.File.Delete(fileName);

            return entity;
        }

        protected bool LoadEntities(ChunkKey area)
        {
            var name = Utils.GetPathForChunk(Game, area);
            name = Path.Combine(name, DataDir);

            if (!Directory.Exists(name))
                return false;

            var entities = new List<Entity>();
            foreach (var filename in Directory.EnumerateFiles(name))
            {
                try
                {
                    var entity = LoadEntityFromFile(filename);
                    entities.Add(entity);
                }
                catch (Exception e)
                {
                    Logger.Write(String.Format("Unable to load entity in file {0} ({1})- skipping", filename, e), LoggerLevel.Error);
                }
            }

            OnEntityLoaded(entities, area);

            return true;
        }

        public void LoadEntities(List<ChunkKey> area)
        {
            var unavailable = new List<ChunkKey>();
            foreach (var item in area)
            {
                var result = LoadEntities(item);
                if (!result)
                    unavailable.Add(item);
            }

            if (unavailable.Count != 0)
                OnEntitiesUnavailable(unavailable);
        }
    }
}

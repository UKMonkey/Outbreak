using System;
using System.Linq;
using System.Collections.Generic;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.World;
using Vortex.Interface.World.Chunks;
using Vortex.Interface.World.Entities;
using Vortex.Interface.World.Triggers;
using Vortex.Interface.World.Wrapper;
using Outbreak.Entities;
using Outbreak.Enums;
using Outbreak.Server.World.Triggers;
using Psy.Core;
using EntityTypeEnum = Outbreak.Entities.EntityTypeEnum;

namespace Outbreak.Server.World.Providers
{
    public class ConnectedRoomsWorldProvider : IWorldProvider
    {
        public event EntityChunkKeyCallback OnEntityGenerated;
        public event ChunkCallback OnChunksGenerated;
        public event TriggerCallback OnTriggerGenerated;

#pragma warning disable 67
        public event EntityChunkKeyCallback OnEntityLoaded;
        public event EntitiesCallback OnEntityUpdated;
        public event EntityIdCallback OnEntityDeleted;
        public event ChunkKeyCallback OnEntitiesUnavailable;

        public event ChunkCallback OnChunkLoad;
        public event ChunkKeyCallback OnChunksUnavailable;

        public event TriggerCallback OnTriggerLoaded;
        public event ChunkKeyCallback OnTriggersUnavailable;
#pragma warning restore 67

        private readonly IServer _engine;

        public ConnectedRoomsWorldProvider(IServer engine)
        {
            _engine = engine;
        }

        public void Dispose()
        {
        }

        public void LoadChunks(List<ChunkKey> keys)
        {
            var generated = new List<Chunk>();

            foreach (var key in keys)
            {
                var height = new Vector3(0, 0, -1.5f);
                var lights = new List<ILight>
                                 {
                                     new Light(new Vector3(2.0f, 2.0f, 0) + height, 8f,
                                               Colours.RandomSolid() * 3)
                                 };
                var chunkMesh = new ChunkMesh();
                chunkMesh.AddRectangle((int)MaterialType.Grassland, new Vector3(0, 0, 0), new Vector3(Chunk.ChunkWorldSize, Chunk.ChunkWorldSize, 0));

                var toAdd = new Chunk(key, chunkMesh, lights);

                generated.Add(toAdd);
            }
            OnChunksGenerated(generated);
        }

        public void LoadTriggers(ChunkKey location)
        {
            var data = new List<ITrigger>();

            var key = new TriggerKey(location, 0);

            var zst = new ZombieSpawnTrigger(_engine, key);
            data.Add(zst);

            OnTriggerGenerated(location, data);
        }

        private IEnumerable<Entity> MakeWall(ChunkKey area, float xmin, float xmax, float ymin, float ymax)
        {
            var ret = new List<Entity>();
            var start = new Vector3(xmin, ymin, 0);
            var end = new Vector3(xmax, ymax, 0);

            const float stepSize = 1;
            var steps = (end - start).Length/stepSize;
            var step = (start - end).NormalizeRet() * stepSize;
            var entityRotation = step;
            var rot = Math.Atan(entityRotation.Y/entityRotation.X) + (Math.PI/2);
            
            for (var i=0; i<steps; ++i)
            {
                var entity = _engine.EntityFactory.Get(EntityTypeEnum.BasicWall);
                entity.SetPosition(_engine.ChunkVectorToWorldVector(area, end + i * step));
                entity.SetRotation((float) rot);

                ret.Add(entity);
            }
            return ret;
        }

        public void LoadEntities(ChunkKey area)
        {
            var entities = new List<Entity>();

            entities.AddRange(MakeWall(area, 0, 1.5f, 0, 0));
            entities.AddRange(MakeWall(area, 10, 10, 0, 0));

            entities.AddRange(MakeWall(area, 0, 0, 0, 5));
            entities.AddRange(MakeWall(area, 0, 0, 10, 15));

            OnEntityGenerated(entities.Where(item => item != null).ToList(), area);
        }

        public void LoadEntities(List<ChunkKey> area)
        {
            foreach (var item in area)
                LoadEntities(item);
        }
    }
}
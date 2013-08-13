using System;
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
    class GrasslandWorldProvider : IWorldProvider
    {
        public event ChunkCallback OnChunksGenerated;
        public event EntityChunkKeyCallback OnEntityGenerated;
        public event TriggerCallback OnTriggerGenerated;

#pragma warning disable 67
        public event ChunkCallback OnChunkLoad;
        public event ChunkKeyCallback OnChunksUnavailable;

        public event EntityChunkKeyCallback OnEntityLoaded;
        public event EntitiesCallback OnEntityUpdated;
        public event EntityIdCallback OnEntityDeleted;
        public event ChunkKeyCallback OnEntitiesUnavailable;

        public event ChunkKeyCallback OnTriggersUnavailable;
        public event TriggerCallback OnTriggerLoaded;
#pragma warning restore 67

        private readonly IServer _engine;

        public GrasslandWorldProvider(IServer engine)
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
                var height = new Vector3(0, 0, -2.1f);
                var lights = new List<ILight>();

                if (key.X % 2 == 0 && key.Y % 2 == 0)
                {
                    lights.Add(new Light(new Vector3(3, 3, 0) + height, 5.5f,
                                         Colours.RandomSolid()*0.3f));
                }
                var chunkMesh = new ChunkMesh();

                chunkMesh.AddRectangle(key.X % 2 == 0 ? (int)MaterialType.Grassland : (int)MaterialType.Wall1, new Vector3(0, 0, 0), new Vector3(Chunk.ChunkWorldSize, Chunk.ChunkWorldSize, 0));

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

        public void LoadEntities(ChunkKey area)
        {
            var entities = new List<Entity>();

            var wall = _engine.EntityFactory.Get(EntityTypeEnum.BasicWall);
            wall.SetPosition(_engine.ChunkVectorToWorldVector(area, new Vector3(2.0f, 4.0f, 0)));
            wall.SetRotation((float)Math.PI/4);
            entities.Add(wall);

            for (int i = 0; i < 16; i++)
            {
                var walln = _engine.EntityFactory.Get(EntityTypeEnum.BasicWall);
                walln.SetPosition(_engine.ChunkVectorToWorldVector(area, new Vector3(i * 0.9f, i * 0.8f, 0)));
                walln.SetRotation((float)Math.PI / 2);
                entities.Add(walln);
            }

            OnEntityGenerated(entities, area);
        }

        public void LoadEntities(List<ChunkKey> area)
        {
            foreach (var item in area)
                LoadEntities(item);
        }

        /*
        private void MakeWall(List<Entity> entities, ChunkKey area, float xmin, float xmax, float ymin, float ymax)
        {
            var start = new Vector(xmin, ymin);
            var end = new Vector(xmax, ymax);

            const float stepSize = Tile.Size;
            var steps = (end - start).Length / stepSize;
            var step = (start - end).Normalize() * stepSize;
            var entityRotation = step;
            var rot = Math.Atan(entityRotation.Y / entityRotation.X) + (Math.PI / 2);

            for (var i = 0; i < steps; ++i)
            {
                var entity = _engine.EntityFactory.Get<BasicWall>();
                entity.Position = _engine.ChunkVectorToWorldVector(area, end + i * step);
                entity.Rotation = (float)rot;
                entities.Add(entity);
            }
        }
         */
    }
}

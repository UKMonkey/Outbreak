using System.Collections.Generic;
using System.Linq;
using Vortex.Interface.World.Entities;
using Vortex.Interface.EntityBase;
using Vortex.Interface.World.Chunks;

namespace EngineTests.Vortex.World.Observable
{
    class TestEntityCache : IEntityCache
    {
#pragma warning disable 67
        public event EntitiesCallback OnEntitiesLoaded;
        public event EntitiesCallback OnEntitiesUpdated;
        public event EntitiesCallback OnEntitiesDeleted;
#pragma warning restore 67

        public void SetObservedChunks(IEnumerable<ChunkKey> ignored)
        {
        }

        public ICollection<Entity> GetObservedEntities()
        {
            return new List<Entity>();
        }

        public IEnumerable<Entity> GetStaticEntities(ChunkKey area)
        {
            return new List<Entity>();
        }

        public IEnumerable<Entity> GetMobileEntities(ChunkKey area)
        {
            return new List<Entity>();
        }

        public IEnumerable<Entity> GetEntities(ChunkKey area)
        {
            return new List<IEnumerable<Entity>> { GetMobileEntities(area), GetStaticEntities(area) }.SelectMany(item => item).ToList();
        }

        public void SaveEntities(ICollection<Entity> area)
        {}

        public void DeleteEntity(int toDelete)
        {}

        public void AddEntities(List<Entity> toAdd)
        {}

        public void UpdateEntities(List<Entity> toAdd)
        {}

        public void ProcessLoadedData()
        {}
    }
}

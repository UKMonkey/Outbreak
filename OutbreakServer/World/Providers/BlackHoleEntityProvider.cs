using System.Collections.Generic;
using Vortex.Interface.World.Chunks;
using Vortex.Interface.World.Entities;


namespace Outbreak.Server.World.Providers
{
    public class BlackHoleEntityProvider : IEntityLoader
    {
        // these aren't used - that's expected - that's why it's called black hole
#pragma warning disable 67
        public event EntityChunkKeyCallback OnEntityLoaded;
        public event EntityChunkKeyCallback OnEntityGenerated;
        public event EntitiesCallback OnEntityUpdated;
        public event EntityIdCallback OnEntityDeleted;
        public event ChunkKeyCallback OnEntitiesUnavailable;
#pragma warning restore 67


        public void LoadEntities(ChunkKey area)
        {
        }

        public void LoadEntities(List<ChunkKey> area)
        {
        }

        public void Dispose()
        {
        }
    }
}



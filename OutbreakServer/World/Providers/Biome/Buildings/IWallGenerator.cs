using System.Collections.Generic;
using Vortex.Interface.EntityBase;
using Vortex.Interface.World.Chunks;

namespace Outbreak.Server.World.Providers.Biome.Buildings
{
    public interface IWallGenerator
    {
        IEnumerable<KeyValuePair<ChunkKey, List<Entity>>> GenerateWalls(BuildingData data);
    }
}

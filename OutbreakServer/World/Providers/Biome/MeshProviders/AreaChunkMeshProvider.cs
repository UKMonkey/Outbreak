using Vortex.Interface;
using Vortex.Interface.World.Chunks;
using Outbreak.Enums;
using Psy.Core;

namespace Outbreak.Server.World.Providers.Biome.MeshProviders
{
    public class AreaChunkMeshProvider : AreaMeshProvider
    {
        public AreaChunkMeshProvider(IEngine engine, ChunkKey area, MaterialType materialType)
            : base(area.GetWorldArea(engine), materialType)
        {
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Psy.Core;
using Vortex.Interface.World;

namespace Outbreak.Server.World.Providers.Biome.MeshProviders
{
    public class SimpleCombinedMeshProvider : IChunkMeshProvider
    {
        private readonly List<IChunkMeshProvider> _providers;

        public bool FullMeshProvider { get; private set; }

        public SimpleCombinedMeshProvider(IEnumerable<IChunkMeshProvider> partialProviders)
        {
            _providers = partialProviders.ToList();

            FullMeshProvider = false;
            foreach (var item in _providers)
                FullMeshProvider = FullMeshProvider || item.FullMeshProvider;
        }

        public void GetMeshesForArea(Rectangle area, ChunkMesh mesh, out Rectangle? meshedArea)
        {
            Rectangle? meshArea = null;

            foreach (var provider in _providers)
            {
                provider.GetMeshesForArea(area, mesh, out meshArea);
            }

            meshedArea = meshArea;
        }
    }
}

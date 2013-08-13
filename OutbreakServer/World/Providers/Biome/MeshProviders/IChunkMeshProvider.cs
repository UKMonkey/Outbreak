using System.Collections.Generic;
using Vortex.Interface.World;
using Psy.Core;

namespace Outbreak.Server.World.Providers.Biome.MeshProviders
{
    /// <summary>
    /// A mesh provider ... some will be guarenteed to provide the full mesh (if in multiple calls of the callback)
    /// some will only provide part of the area.  If only part of the area is returned, then *A SINGLE* area is to be returned
    /// (what position is arbitary)
    /// </summary>
    public interface IChunkMeshProvider
    {
        bool FullMeshProvider { get; }
        void GetMeshesForArea(Rectangle area, ChunkMesh mesh, out Rectangle? meshedArea);
    }
}

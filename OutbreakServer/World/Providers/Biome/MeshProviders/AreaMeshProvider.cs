using Vortex.Interface.World;
using Outbreak.Enums;
using Psy.Core;

namespace Outbreak.Server.World.Providers.Biome.MeshProviders
{
    public class AreaMeshProvider : IChunkMeshProvider
    {
        public bool FullMeshProvider { get { return false; } }

        private readonly Rectangle _area;
        private readonly int _material;

        public AreaMeshProvider(Rectangle area, MaterialType materialType)
        {
            _area = area;
            _material = (int)materialType;
        }

        public void GetMeshesForArea(Rectangle area, ChunkMesh mesh, out Rectangle? meshedArea)
        {
            meshedArea = area.IntersectingArea(_area);

            if (meshedArea != null && meshedArea.Value.Area == 0)
                meshedArea = null;

            if (meshedArea != null)
                mesh.AddRectangle(_material, meshedArea.Value.BottomLeft.AsVector3(), meshedArea.Value.TopRight.AsVector3());
        }
    }
}

using Outbreak.Enums;
using Psy.Core;
using Vortex.Interface.World;

namespace Outbreak.Server.World.Providers.Biome.MeshProviders
{
    public abstract class IntersectMeshProvider : IChunkMeshProvider
    {
            // these are more than likely not full providers
        public virtual bool FullMeshProvider { get { return false; } }

            // what sort of material is this mesh?
        protected abstract MaterialType GetMaterial();

            // what is the full amount that this mesh covers
        public abstract Rectangle GetFullArea();

            // intersect the full mesh with the requested mesh and return the intersection
        public void GetMeshesForArea(Rectangle area, ChunkMesh mesh, out Rectangle? meshedArea)
        {
            meshedArea = area.IntersectingArea(GetFullArea());

            var material = GetMaterial();

            if (meshedArea != null && meshedArea.Value.Area == 0)
                meshedArea = null;

            if (meshedArea != null)
                mesh.AddRectangle((int)material, meshedArea.Value.BottomLeft.AsVector3(), meshedArea.Value.TopRight.AsVector3());
        }
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using SlimMath;
using Vortex.Interface.World;
using Psy.Core;

namespace Outbreak.Server.World.Providers.Biome.MeshProviders
{
    public class CombinedMeshProvider : IChunkMeshProvider
    {
        private readonly List<IChunkMeshProvider> _providers;

        public bool FullMeshProvider { get; private set; }

        public CombinedMeshProvider(List<IChunkMeshProvider> partialProviders)
        {
            _providers = partialProviders;

            FullMeshProvider = false;
            foreach (var item in partialProviders)
                FullMeshProvider = FullMeshProvider || item.FullMeshProvider;
        }

        public void GetMeshesForArea(Rectangle area, ChunkMesh mesh, out Rectangle? meshedArea)
        {
            var toBeReprocessed = new List<Rectangle>();
            toBeReprocessed.Add(area);

            foreach (var prov in _providers)
                toBeReprocessed = ProcessAreas(toBeReprocessed, prov, mesh);

            meshedArea = area;
        }

        /// <summary>
        /// returns areas that were not processed
        /// </summary>
        private List<Rectangle> ProcessAreas(IEnumerable<Rectangle> toProcess, IChunkMeshProvider provider, ChunkMesh mesh)
        {
            var ret = new List<Rectangle>();

            foreach (var rect in toProcess)
                ret.AddRange(ProcessArea(rect, provider, mesh));

            return ret;
        }

        /// <summary>
        /// returns areas that were not processed
        /// </summary>
        private List<Rectangle> ProcessArea(Rectangle toProcess, IChunkMeshProvider provider, ChunkMesh mesh)
        {
            Rectangle? generated;

            provider.GetMeshesForArea(toProcess, mesh, out generated);
            if (generated != null)
            {
                var missing = GetMissingAreas(toProcess, generated.Value);
                var ret = new List<Rectangle>();
                foreach (var area in missing)
                    ret.AddRange(ProcessArea(area, provider, mesh));
            }
            return new List<Rectangle>{toProcess};
        }

        private List<Rectangle> GetMissingAreas(Rectangle expected, Rectangle provided)
        {
            var ret = new List<Rectangle>(4);
            Debug.Assert(expected.Intersects(provided));

            if (expected.BottomLeft.X < provided.BottomLeft.X)
                ret.Add(new Rectangle(expected.TopLeft, new Vector2(provided.BottomLeft.X, expected.BottomLeft.Y)));
            
            if (expected.TopRight.X > provided.TopRight.X)
                ret.Add(new Rectangle(new Vector2(provided.BottomRight.X, expected.TopRight.Y), expected.BottomRight));

            var left = expected.BottomLeft.X < provided.BottomLeft.X
                ? provided.BottomLeft.X
                : expected.BottomLeft.X;
            var right = expected.TopRight.X > provided.TopRight.X
                ? provided.BottomRight.Y
                : expected.BottomRight.Y;

            if (expected.TopLeft.Y > provided.TopLeft.Y)
                ret.Add(new Rectangle(new Vector2(left, expected.TopLeft.Y), new Vector2(right, expected.BottomRight.Y)));
            if (expected.BottomRight.Y < provided.BottomRight.Y)
                ret.Add(new Rectangle(new Vector2(left, provided.BottomRight.Y), new Vector2(right, expected.BottomRight.Y)));

            return ret;
        }
    }
}

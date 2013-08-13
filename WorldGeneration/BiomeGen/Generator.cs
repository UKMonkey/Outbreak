using System;
using System.Collections.Generic;
using BiomeGen.Maths.Geometry;

namespace BiomeGen
{
    public abstract class Generator
    {
#if DEBUG
        private const int Capacity = 0;
#else
        private const int Capacity = 100;
#endif

        public readonly float Width;
        public readonly float Height;
        public List<BiomeRect> Rects { get; set; }
        public readonly float BiomeSizeInChunks;
        public readonly float ChunkSizeInMetres;
        public float Resolution { get; set; }
        protected Random Rng { get; set; }

        public Generator(float chunkSizeInMetres, float biomeSizeInChunks, float resolution)
        {
            BiomeSizeInChunks = biomeSizeInChunks;
            ChunkSizeInMetres = chunkSizeInMetres;
            Resolution = resolution;
            var size =((biomeSizeInChunks * chunkSizeInMetres) / resolution);
            Width = Height = size;
            Rects = new List<BiomeRect>(Capacity);
        }

        protected abstract BiomeRect GetDefaultRectangle();

        public void Generate(int seed)
        {
            Rng = new Random(seed);
            Rects = new List<BiomeRect>(Capacity) { GetDefaultRectangle() };
            GenerateImpl();
            //Optimize();
        }

        protected abstract void GenerateImpl();

        protected bool CollidesWithMatchingType(BiomeRect buildingBiomeRect)
        {
            foreach (var rect in Rects)
            {
                if (rect.Intersects(buildingBiomeRect) && rect.Colour == buildingBiomeRect.Colour)
                {
                    return true;
                }
            }
            return false;
        }

        protected void AddRect(BiomeRect n)
        {
            var newRects = new List<BiomeRect>(20) { n };
            var victims = new List<BiomeRect>(20);

            foreach (var rect in Rects)
            {
                if (!n.Intersects(rect))
                    continue;

                victims.Add(rect);

                var clampn = rect.Intersect(n);

                if (clampn.BottomLeft.X > rect.BottomLeft.X)
                {
                    var colour = rect.Colour;
                    newRects.Add(new BiomeRect(rect.BottomLeft, new BiomeVector(clampn.BottomLeft.X, rect.TopRight.Y), colour));
                }

                if (clampn.TopRight.X < rect.TopRight.X)
                {
                    var colour = rect.Colour;
                    newRects.Add(new BiomeRect(new BiomeVector(clampn.TopRight.X, rect.BottomLeft.Y), rect.TopRight, colour));
                }

                if (clampn.BottomLeft.Y > rect.BottomLeft.Y)
                {
                    var colour = rect.Colour;
                    newRects.Add(new BiomeRect(new BiomeVector(clampn.BottomLeft.X, rect.BottomLeft.Y), new BiomeVector(clampn.TopRight.X, clampn.BottomLeft.Y), colour));
                }

                if (clampn.TopRight.Y < rect.TopRight.Y)
                {
                    var colour = rect.Colour;
                    newRects.Add(new BiomeRect(new BiomeVector(clampn.BottomLeft.X, clampn.TopRight.Y), new BiomeVector(clampn.TopRight.X, rect.TopRight.Y), colour));
                }
            }

            foreach (var victim in victims)
            {
                Rects.Remove(victim);
            }

            Rects.AddRange(newRects);
        }

        protected void Optimize()
        {
            var victims = new List<BiomeRect>(10);
            var newrects = new List<BiomeRect>(5);

            foreach (var rect in Rects)
            {
                // does this rect adjoin a similar height rect on its right hand side?

                if (victims.Contains(rect))
                    continue;

                foreach (var other in Rects)
                {
                    if (rect.Equals(other))
                        continue;

                    if (victims.Contains(other))
                        continue;

                    if ((rect.Colour == other.Colour) &&
                        (rect.Height == other.Height) &&
                        (rect.TopRight.X == other.BottomLeft.X) &&
                        (rect.BottomLeft.Y == other.BottomLeft.Y))
                    {
                        var colour = rect.Colour;
                        colour = rect.Colour;
                        newrects.Add(new BiomeRect(rect.BottomLeft, other.TopRight, colour));
                        victims.Add(rect);
                        break;
                    }
                }
            }

            foreach (var victim in victims)
            {
                Rects.Remove(victim);
            }

            Rects.AddRange(newrects);
        }
    }
}
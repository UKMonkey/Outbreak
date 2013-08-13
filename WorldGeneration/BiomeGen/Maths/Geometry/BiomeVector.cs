using System;
using SlimMath;

namespace BiomeGen.Maths.Geometry
{
    public struct BiomeVector
    {
        public readonly float X;
        public readonly float Y;

        public BiomeVector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Vector2(BiomeVector bv)
        {
            return new Vector2(bv.X, bv.Y);
        }

        private static BiomeVector Clamp(BiomeRect biomeRect, BiomeVector biomeVector)
        {
            var x = Math.Max(Math.Min(biomeRect.TopRight.X, biomeVector.X), biomeRect.BottomLeft.X);
            var y = Math.Max(Math.Min(biomeRect.TopRight.Y, biomeVector.Y), biomeRect.BottomLeft.Y);

            return new BiomeVector(x, y);

        }

        public BiomeVector Clamp(BiomeRect biomeRect)
        {
            return Clamp(biomeRect, this);
        }

        public new string ToString()
        {
            return String.Format("{0}, {1}", X, Y);
        }
    }
}
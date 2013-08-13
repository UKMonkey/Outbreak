using System;
using System.Diagnostics;
using System.Drawing;
using Psy.Core;
using SlimMath;
using Rectangle = Psy.Core.Rectangle;

namespace BiomeGen.Maths.Geometry
{
    public struct BiomeRect
    {
        public BiomeVector BottomLeft;
        public BiomeVector TopRight;
        public Color Colour;

        public BiomeVector TopLeft
        {
            get
            {
                return new BiomeVector(BottomLeft.X, TopRight.Y);
            }
        }

        public BiomeVector BottomRight
        {
            get
            {
                return new BiomeVector(TopRight.X, BottomLeft.Y);
            }
        }

        public BiomeRect(BiomeVector bottomLeft, BiomeVector topRight, Color colour)
        {
            BottomLeft = bottomLeft;
            TopRight = topRight;
            Colour = colour;

            Debug.Assert(Width > 0);
            Debug.Assert(Height > 0);
        }

        public float Width
        {
            get { return Math.Abs(TopRight.X - BottomLeft.X); }
        }

        public float Height
        {
            get { return Math.Abs(TopRight.Y - BottomLeft.Y); }
        }

        public BiomeRect(float bottomX, float bottomY, float topX, float topY, Color colour)
        {
            BottomLeft = new BiomeVector(bottomX, bottomY);
            TopRight = new BiomeVector(topX, topY);
            Colour = colour;

            Debug.Assert(Math.Abs(bottomX - topX) > 0);
            Debug.Assert(Math.Abs(bottomY - topY) > 0);

            Debug.Assert(topX > bottomX);
            Debug.Assert(topY > bottomY);
        }

        public bool Intersects(BiomeRect other)
        {
            return
                (TopRight.X > other.BottomLeft.X) &&
                (BottomLeft.X < other.TopRight.X) &&
                (TopRight.Y > other.BottomLeft.Y) &&
                (BottomLeft.Y < other.TopRight.Y);
        }

        public BiomeRect Intersect(BiomeRect other)
        {
            var bottomLeft = BottomLeft.Clamp(other);
            var topRight = TopRight.Clamp(other);
            return new BiomeRect(bottomLeft, topRight, other.Colour);
        }

        public static implicit operator Rectangle(BiomeRect br)
        {
            return new Rectangle(br.TopLeft, br.BottomRight);
        }

        public BiomeRect Translate(Vector3 biomeWorldCoordinateOffset)
        {
            return new BiomeRect(BottomLeft.X + biomeWorldCoordinateOffset.X, BottomLeft.Y + biomeWorldCoordinateOffset.Y,
                TopRight.X + biomeWorldCoordinateOffset.X, TopRight.Y + biomeWorldCoordinateOffset.Y, Colour); 
        }
    }
}
namespace Outbreak.Server.World.Providers.Biome
{
    public class BiomeKey
    {
        public readonly int X;
        public readonly int Y;

        public BiomeKey(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X*397) ^ Y;
            }
        }

        public static bool operator ==(BiomeKey a, BiomeKey b)
        {
            var aIsNull = ReferenceEquals(a, null);
            var bIsNull = ReferenceEquals(b, null);

            if (aIsNull || bIsNull)
            {
                return aIsNull && bIsNull;
            }

            return (a.X == b.X && a.Y == b.Y);
        }

        public static bool operator !=(BiomeKey a, BiomeKey b)
        {
            return !(a == b);
        }

        public bool Equals(BiomeKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.X == X && other.Y == Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BiomeKey)) return false;
            return Equals((BiomeKey) obj);
        }

        public override string ToString()
        {
            return string.Format("BiomeKey: {0},{1}", X, Y);
        }
    }
}

namespace BiomeGen.Maths
{
    public static class MathsUtil
    {
        public static int Clamp(int min, int max, int value)
        {
            return value > max ? max : (value < min ? min : value);
        }
    }
}
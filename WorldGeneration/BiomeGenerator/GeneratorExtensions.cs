using System.Drawing;
using BiomeGen;

namespace BiomeGenerator
{
    public static class GeneratorExtensions
    {
        public static Size Size(this Generator generator)
        {
            return new Size((int)generator.Width, (int)generator.Height);
        }
    }
}
using System;
using SlimMath;

namespace Outbreak
{
    public static class OutsideLightingCalculator
    {
        public static Color4 GetOutsideLighting(GameTime gameTime)
        {
            var a = 1.2f * (1 - ((Math.Abs((gameTime.Fractional - 12) / 24.0f)) / 0.5f));
            var b = a * a;
            var c = Math.Min(b, 1.0f);
            var d = Math.Max(c, 0.15f);

            return new Color4(1.0f, d, d, d);
        }
    }
}
using System.Drawing;
using System.Drawing.Drawing2D;
using BiomeGen;
using BiomeGen.Maths.Geometry;

namespace BiomeGenerator
{
    public class GeneratorRenderer
    {
        public void Render(Graphics graphics, Generator generator)
        {
            var clipbounds = graphics.ClipBounds.Height;

            foreach (var rect in generator.Rects)
            {
                DrawRect(graphics, rect);
            }

            var col = Color.FromArgb(32, 255, 255, 255);
            
            var gridPen = new Pen(col);

            var chunkSizeInMetres = generator.ChunkSizeInMetres / generator.Resolution;

            for (int x = 0; x < generator.BiomeSizeInChunks; x++)
            {
                graphics.DrawLine(gridPen, x * chunkSizeInMetres, clipbounds, x * chunkSizeInMetres, clipbounds - generator.Height);
            }

            for (int y = 0; y < generator.BiomeSizeInChunks; y++)
            {
                graphics.DrawLine(gridPen, 0, clipbounds - (y * chunkSizeInMetres), generator.Width, clipbounds - (y * chunkSizeInMetres));
            }
        }

        private static void DrawRect(Graphics graphics, BiomeRect biomeRect)
        {
            var clipbounds = graphics.ClipBounds.Height;

            var darkColour = Color.FromArgb(255, biomeRect.Colour.R / 3, biomeRect.Colour.G / 3, biomeRect.Colour.B / 3);

            var darkBrush = new LinearGradientBrush(new Point((int)biomeRect.TopLeft.X, (int)(clipbounds - biomeRect.TopLeft.Y)),
                                                new Point((int)biomeRect.BottomRight.X, (int)(clipbounds - biomeRect.BottomRight.Y)),
                                                biomeRect.Colour, darkColour);

            var medColour = Color.FromArgb(92, biomeRect.Colour.R/2, biomeRect.Colour.G/2, biomeRect.Colour.B/2);
            var medPen = new Pen(medColour) { Width = 1f };

            graphics.FillRectangle(darkBrush, biomeRect.TopLeft.X, clipbounds - biomeRect.TopLeft.Y, biomeRect.Width, biomeRect.Height);
            graphics.DrawRectangle(medPen,  biomeRect.TopLeft.X,  clipbounds - biomeRect.TopLeft.Y, biomeRect.Width, biomeRect.Height);
            graphics.DrawLine(medPen, biomeRect.TopLeft.X, clipbounds - biomeRect.TopLeft.Y,
                biomeRect.BottomRight.X, clipbounds - biomeRect.BottomRight.Y);
        }
    }
}
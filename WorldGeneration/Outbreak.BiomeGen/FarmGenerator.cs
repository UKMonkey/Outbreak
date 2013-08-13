using BiomeGen;
using BiomeGen.Maths.Geometry;
using Psy.Core;

namespace Outbreak.BiomeGen
{
    public class FarmGenerator : Generator
    {
        public static int BailSize = 2;
        public static float BailDensity = 0.0015f;
        public static int DrivewayWidth = 6;

        public static int MainRoadWidth = 15;

        public static int FarmGenerationMargin = MainRoadWidth;

        public FarmGenerator(int chunkSizeInMetres, int biomeSizeInChunks, float resolution) 
            : base(chunkSizeInMetres, biomeSizeInChunks, resolution) 
        { }

        protected override void GenerateImpl()
        {
            AddMainRoads();

            for (int i = 0; i < Rng.Next(4, 32); i++)
            {
                AddField();
            }

            AddHouse();
        }

        private void AddMainRoads()
        {
            // add vertical road.
            var mainRoadColor = Legend.MainRoadColor;
            var vertRoadRect = new BiomeRect(MainRoadWidth, 0, Width, MainRoadWidth, mainRoadColor);
            AddRect(vertRoadRect);

            // add horizontal road.
            var horizRoadRect = new BiomeRect(0, MainRoadWidth, MainRoadWidth, Height, mainRoadColor);
            AddRect(horizRoadRect);

            // add junction.0
            var junctionRect = new BiomeRect(0, 0, MainRoadWidth, MainRoadWidth, Legend.JunctionColor);
            AddRect(junctionRect);
        }


        private void AddHouse()
        {
            var w = Rng.Next(64, 80);
            var h = Rng.Next(64, 80);
            var x = Rng.Next(FarmGenerationMargin+1, (int)(Width - w));
            var y = Rng.Next(FarmGenerationMargin+1, (int)(Height - h));

            var drivewayColor = Legend.DrivewayColor;
            var parkingspace = new BiomeRect(x, y, x + w, y + h, drivewayColor);
            AddRect(parkingspace);

            var hh = Rng.Next(h / 2, h-1);
            var hw = Rng.Next(w / 2, w-1);

            var hx = Rng.NextFloat(x, x + (w - hw));
            var hy = Rng.NextFloat(y, y + (h - hh));

            var house = new BiomeRect(hx, hy, hx + hw, hy + hh, Legend.HouseColor);

            AddRect(house);

            var drivewayX = Rng.NextFloat(x, x + w - DrivewayWidth);
            var driveway = new BiomeRect(drivewayX, FarmGenerationMargin, drivewayX + DrivewayWidth, y, drivewayColor);
            AddRect(driveway);

            var stableStartX = x - 120 < 0 ? x + w + 20 : x - 100;
            var stableX = Rng.NextFloat(stableStartX, stableStartX + 60);
            var stableY = Rng.NextFloat(y, y + (h / 2.0f));

            var stableWidth = Rng.Next(35, 45);
            var stableHeight = Rng.Next(40, 50);

            var outhouse = new BiomeRect(stableX, stableY, stableX + stableWidth, stableY + stableHeight, Legend.Stable);

            //AddRect(outhouse);
        }

        private void AddField()
        {
            
            var w = Rng.NextFloat(Width / 6, Width / 3);
            var h = Rng.NextFloat(Width / 6, Height / 3);
            var x = Rng.NextFloat(FarmGenerationMargin, Width - w);
            var y = Rng.NextFloat(FarmGenerationMargin, Height - h);
            
            /*
            int h;
            int x;
            int y;
            var w = h = x = y = 10;
            */
            var field = new BiomeRect(x, y, x + w, y + h, Legend.FieldColor);

            if (CollidesWithMatchingType(field))
                return;

            AddRect(field);

            // bails
            /*
            var bailcount = (int)(BailDensity *  (((w * h) / ((float)BailSize * BailSize))));

            for (var i = 0; i < bailcount; i++)
            {
                var bx = Rng.Next(field.TopLeft.X + BailSize, field.BottomRight.X - BailSize);
                var by = Rng.Next(field.TopLeft.Y + BailSize, field.BottomRight.Y - BailSize);

                var bail = new Rect(bx, by, bx + BailSize, by + BailSize, BailColor);

                if (CollidesWithMatchingType(bail))
                    continue;

                AddRect(bail);
            }
             */
        }

        protected override BiomeRect GetDefaultRectangle()
        {
            return new BiomeRect(0, 0, Width, Height, Legend.Grass);
        }
    }
}
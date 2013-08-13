using System.Drawing;
using BiomeGen;
using BiomeGen.Maths.Geometry;
using Psy.Core;

namespace Outbreak.BiomeGen
{
    public class SuburbGenerator : Generator
    {
        public const float RoadWidth = 10.0f;

        public SuburbGenerator(float chunkSizeInMetres, float biomeSizeInChunks, float resolution) : base(chunkSizeInMetres, biomeSizeInChunks, resolution) {}

        protected override BiomeRect GetDefaultRectangle()
        {
            return new BiomeRect(0, 0, Width, Height, Legend.Grass);
        }

        protected override void GenerateImpl()
        {
            const int xcount = 6;
            const int ycount = 6;

            var xPer = Width / xcount;
            var yPer = Width / ycount;

            var cornerShopSquareX = Rng.Next(0, xcount - 1);
            var cornerShopSquareY = Rng.Next(0, xcount - 1);

            for (var x = 0; x < xcount; x++)
            {
                for (var y = 0; y < ycount; y++)
                {
                    var blx = x*xPer;
                    var bly = y*yPer;
                    var buildingColour = Legend.HouseColor;

                    AddMainRoad(blx, bly, xPer, yPer);
                    if (cornerShopSquareX == x && cornerShopSquareY == y)
                        buildingColour = Legend.CornerShopColor;

                    AddBuilding(blx + RoadWidth, bly + RoadWidth, xPer - RoadWidth, yPer - RoadWidth, buildingColour);
                }
            }
        }

        private void AddBuilding(float minX, float minY, float sizeX, float sizeY, Color buildingColour)
        {
            var houseWidth = (int)((sizeX) / Rng.NextFloat(1.4f, 1.7f));
            var houseHeight = (int)((sizeY) / Rng.NextFloat(1.4f, 1.7f));

            var houseOffsetX = (sizeX - houseWidth) / 2;
            var houseOffsetY = (sizeY - houseHeight) / 2;

            AddRect(new BiomeRect(minX + houseOffsetX, minY + houseOffsetY, minX + houseOffsetX + houseWidth, minY + houseOffsetY + houseHeight, buildingColour));

            AddRect(new BiomeRect(
                minX + houseOffsetX + houseWidth, 
                minY + houseOffsetY,
                minX + houseWidth + (houseOffsetX * 1.2f),
                minY + houseOffsetY + houseHeight, Legend.DrivewayColor));

            AddRect(new BiomeRect(
                minX + houseWidth + (houseOffsetX * 1.2f),
                minY + houseOffsetY + (houseHeight * 0.475f),
                minX + sizeX,
                minY + houseOffsetY + (houseHeight * 0.525f), Legend.DrivewayColor));
        }

        private void AddMainRoad(float minX, float minY, float sizeX, float sizeY)
        {
            AddRect(new BiomeRect(minX + RoadWidth, minY, minX + sizeX, minY + RoadWidth, Legend.MainRoadColor));
            AddRect(new BiomeRect(minX, minY, minX + RoadWidth, minY + RoadWidth, Legend.JunctionColor));
            AddRect(new BiomeRect(minX, minY + RoadWidth, minX + RoadWidth, minY + sizeY, Legend.MainRoadColor));
        }
    }
}
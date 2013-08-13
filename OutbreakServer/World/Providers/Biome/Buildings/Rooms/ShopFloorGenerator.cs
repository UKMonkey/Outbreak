using System;
using System.Collections.Generic;
using Outbreak.Entities;
using Outbreak.Entities.Properties;
using Outbreak.Items;
using Outbreak.Items.ItemGenerators;
using Psy.Core;
using Psy.Core.Logging;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using EntityTypeEnum = Outbreak.Entities.EntityTypeEnum;

namespace Outbreak.Server.World.Providers.Biome.Buildings.Rooms
{
    public class ShopFloorGenerator : BasicGenerator
    {
        public ShopFloorGenerator(ItemGeneratorDictionary generatorDict, IServer server) :
            base(1, 10, GetAllowedItems(), generatorDict, server)
        {
        }


        private class ShelfLineData
        {
            public Vector3 Start { get; set; }
            public Vector3 End { get; set; }
            public float ShelfDirection { get; set; }
        }


        private static List<ItemTypeEnum> GetAllowedItems()
        {
            return new List<ItemTypeEnum>
                       {
                           ItemTypeEnum.FirstAidPack,
                           ItemTypeEnum.CricketBat,
                           ItemTypeEnum.PistolAmmo,
                           ItemTypeEnum.ShotgunAmmo,
                           //ItemTypeEnum.BodyItem,
                           //ItemTypeEnum.LegItem,
                           //ItemTypeEnum.HeadItem,
                           //ItemTypeEnum.FootItem,
                           ItemTypeEnum.Food
                       };
        }

        private IEnumerable<ShelfLineData> GetInnerShelfLines(RoomData room, Random randomiser)
        {
            var exampleEntity = EntityFactory.Get(EntityTypeEnum.ShopShelf);
            var boundingBox = exampleEntity.Model.ModelInstance.GetBoundingBox();
            var boxWidth = GetBoxWidth(boundingBox.GetCorners());
            var boxLength = GetBoxLength(boundingBox.GetCorners());

            const float isleSize = 4f;

            // sub area that we can use for shelves
            var shelfArea = new Rectangle(room.Area.TopLeft + new Vector2(isleSize + boxWidth, -(isleSize + boxWidth)),
                                          room.Area.BottomRight + new Vector2(-(isleSize+boxWidth), isleSize+boxWidth));
            Vector3 majorStep;
            Vector3 minorStep;
            Vector2 rootStart;
            Vector2 rootEnd;
            int stepCount;
            float xGap;
            float yGap;

            // establish direction of shelves and gaps in the shelf area that will be created
            if (true)//randomiser.NextDouble() > 0.5f)
            {
                majorStep = new Vector3(1, 0, 0) * (isleSize + 2*boxWidth);
                minorStep = new Vector3(1, 0, 0) * boxWidth;
                stepCount = (int)Math.Floor(shelfArea.Width/(isleSize + 2*boxWidth));

                xGap = (float)(shelfArea.Width - stepCount * (isleSize + 2 * boxWidth)) / 2;
                yGap = (float)(shelfArea.Height - Math.Round(shelfArea.Height / (boxLength)) * boxLength) / 2;

                shelfArea = new Rectangle(shelfArea.TopLeft + new Vector2(xGap, -yGap),
                                          shelfArea.BottomRight + new Vector2(-xGap, yGap));

                rootStart = shelfArea.TopLeft;
                rootEnd = shelfArea.BottomLeft;
            }
            else
            {
                majorStep = new Vector3(0, 1, 0) * (isleSize + boxWidth);
                minorStep = new Vector3(0, 1, 0) * boxWidth;
                stepCount = (int)Math.Floor(shelfArea.Height / (isleSize + 2 * boxWidth));

                xGap = (float)(shelfArea.Width - Math.Round(shelfArea.Width / (boxLength)) * boxLength) / 2;
                yGap = (float)(shelfArea.Height - stepCount * (isleSize + 2 * boxWidth)) / 2;
            }

            // resize area to remove gaps
            var direction1 = majorStep.ZPlaneAngle() + (float) Math.PI/2;
            var direction2 = direction1 + (float) Math.PI;

            for (var i=0; i<stepCount; ++i)
            {
                var startPosition = new Vector3(rootStart, 0) + i*majorStep;
                var endPosition = new Vector3(rootEnd, 0) + i * majorStep;
                yield return new ShelfLineData{Start = startPosition, End = endPosition, ShelfDirection = direction1};

                startPosition = startPosition + minorStep;
                endPosition = endPosition + minorStep;
                yield return new ShelfLineData{ Start = startPosition, End = endPosition, ShelfDirection = direction2};
            }
        }

        private float GetBoxLength(Vector3[] corners)
        {
            var minx = corners[0].X;
            var maxx = minx;

            for (var i = 1; i < corners.Length; ++i)
            {
                var corner = corners[i];
                minx = Math.Min(minx, corner.X);
                maxx = Math.Max(maxx, corner.X);
            }

            return maxx - minx;
        }

        private float GetBoxWidth(Vector3[] corners)
        {
            var miny = corners[0].Y;
            var maxy = miny;

            for (var i = 1; i < corners.Length; ++i)
            {
                var corner = corners[i];
                miny = Math.Min(miny, corner.Y);
                maxy = Math.Max(maxy, corner.Y);
            }

            return maxy - miny;
        }

        private IEnumerable<Entity> GenerateShelfLines(ShelfLineData shelfLine)
        {
            var exampleEntity = EntityFactory.Get(EntityTypeEnum.ShopShelf);
            var boundingBox = exampleEntity.Model.ModelInstance.GetBoundingBox();

            var direction = shelfLine.End - shelfLine.Start;
            var directionAngle = shelfLine.ShelfDirection;
            var boxLength = GetBoxLength(boundingBox.GetCorners());

            var stepDistance = direction.NormalizeRet() * boxLength;
            var position = shelfLine.Start + stepDistance / 2;
            var steps = (int)Math.Round(direction.Length / boxLength);

            for (var i = 0; i < steps; ++i)
            {
                var entity = EntityFactory.Get(EntityTypeEnum.ShopShelf);
                entity.SetPosition(position + i * stepDistance);
                entity.SetRotation(directionAngle);
                entity.SetValidInventoryItems(GetAllowedItems());
                entity.SetMinInventoryItemCount(1);
                entity.SetMaxInventoryItemCount(3);
                yield return entity;
            }
        }
        
        public override IEnumerable<EntitySpawnData> GenerateRoomClutter(RoomData room, Random randomiser)
        {
            var lines = GetInnerShelfLines(room, randomiser);
            
            foreach (var item in lines)
            {
                var entities = GenerateShelfLines(item);

                var chairRequirements = new Dictionary<PositionRequirement, float>
                                            {
                                                {PositionRequirement.FloorLevel, 1},
                                                {PositionRequirement.PrePositioned, 1}
                                            };
                var groupRequirements = new Dictionary<GroupRequirement, float>
                                            {
                                            };

                yield return new EntitySpawnData(entities, chairRequirements, groupRequirements);
            }
        }

        public override bool CanGenerateForRoom(RoomType type)
        {
            switch (type)
            {
                case RoomType.ShopFloor:
                    return true;
                default:
                    return false;
            }
        }
    }
}

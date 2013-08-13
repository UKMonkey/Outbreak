using System;
using Outbreak.Items.ItemGenerators;
using Psy.Core;
using SlimMath;
using Vortex.Interface;

namespace Outbreak.Server.World.Providers.Biome.Buildings.Generators
{
    public class CornerShopGenerator : BuildingGeneratorBase
    {
        public CornerShopGenerator(IGameServer gameServer, ItemGeneratorDictionary itemGeneratorDictionary) 
            : base(gameServer, itemGeneratorDictionary)
        {
        }

        protected RoomData GenerateBathRoom()
        {
            var room = new RoomData(Server, RoomType.Bathroom);
            room.Area = new Rectangle();

            var sizeX = RandomNumberGenerator.Next((int) Math.Round(BuildingSizeX/10f),
                                                   (int) Math.Round(BuildingSizeX/9f));

            var sizeY = RandomNumberGenerator.Next((int) Math.Round(BuildingSizeY/10f),
                                                   (int) Math.Round(BuildingSizeY/9f));

            room.Area.TopLeft = new Vector2(BottomLeft.X, BottomLeft.Y + sizeY);
            room.Area.BottomRight = new Vector2(BottomLeft.X + sizeX, BottomLeft.Y);

            return room;
        }

        protected RoomData GenerateStoreRoom(Vector2 bottomRight)
        {
            var room = new RoomData(Server, RoomType.StoreRoom);
            room.Area = new Rectangle();
            room.Area.TopLeft = new Vector2(TopLeft.X, TopLeft.Y);
            room.Area.BottomRight = bottomRight;

            return room;
        }

        protected RoomData GenerateShopFloor(Vector2 bottomLeft)
        {
            var room = new RoomData(Server, RoomType.ShopFloor);
            room.Area = new Rectangle();
            room.Area.TopLeft = new Vector2(bottomLeft.X, TopRight.Y);
            room.Area.BottomRight = new Vector2(BottomRight.X, bottomLeft.Y);

            return room;
        }

        protected override BuildingData GenerateBuildingData()
        {
            var ret = new BuildingData(new WallGenerator(Server, RandomNumberGenerator, ChunksToGenerate));

            var bathRoom = GenerateBathRoom();
            var storeRoom = GenerateStoreRoom(bathRoom.Area.TopRight);
            var shopFloor = GenerateShopFloor(bathRoom.Area.BottomRight);

            ret.AddRoom(bathRoom);
            ret.AddRoom(storeRoom);
            ret.AddRoom(shopFloor);

            return ret;
        }

        public override string GetBuildingName()
        {
            return "Corner shop";
        }
    }
}

using System;
using System.Collections.Generic;
using Outbreak.Items.ItemGenerators;
using SlimMath;
using Vortex.Interface;
using Psy.Core;

namespace Outbreak.Server.World.Providers.Biome.Buildings.Generators
{
    public class SmallHouseGenerator : BuildingGeneratorBase
    {
        public SmallHouseGenerator(IGameServer gameServer, ItemGeneratorDictionary itemGeneratorDictionary) : 
            base(gameServer, itemGeneratorDictionary)
        {
        }

        protected virtual RoomData GenerateBathroom()
        {
            var room = new RoomData(Server, RoomType.Bathroom);
            var xSize = (int)Math.Round(BuildingSizeX * (0.25 + RandomNumberGenerator.NextDouble() * (0.33-0.25)));
            var ySize = (int)Math.Round(BuildingSizeY * (0.10 + RandomNumberGenerator.NextDouble() * (0.20-0.10)));

            room.Area = new Rectangle();
            room.Area.TopLeft = TopLeft;
            room.Area.BottomRight = TopLeft + new Vector2(xSize, -ySize);

            return room;
        }

        protected virtual RoomData GenerateLivingroom(Vector2 topRight)
        {
            var room = new RoomData(Server, RoomType.LivingRoom);
            room.Area = new Rectangle();
            room.Area.TopLeft = new Vector2(BottomLeft.X, topRight.Y);
            room.Area.BottomRight = new Vector2(topRight.X, BottomLeft.Y);

            return room;
        }

        protected virtual RoomData GenerateDiningRoom(Vector2 bottomLeft)
        {
            var sizey = (int)Math.Round((BuildingSizeY/2)*(0.8 + RandomNumberGenerator.NextDouble() * 0.4));
            var sizex = (int)Math.Round((BottomRight.X - bottomLeft.X)*(0.5 + RandomNumberGenerator.NextDouble()*0.2));

            var topLeft = new Vector2(bottomLeft.X, bottomLeft.Y + sizey);
            var bottomRight = new Vector2(bottomLeft.X + sizex, bottomLeft.Y);

            var room = new RoomData(Server, RoomType.DiningRoom);
            room.Area = new Rectangle();
            room.Area.TopLeft = topLeft;
            room.Area.BottomRight = bottomRight;

            return room;
        }

        protected virtual RoomData GenerateKitchen(Vector2 topLeft)
        {
            var room = new RoomData(Server, RoomType.Kitchen);
            room.Area = new Rectangle();
            room.Area.TopLeft = topLeft;
            room.Area.BottomRight = BottomRight;

            return room;
        }

        protected virtual RoomData GenerateHallway(Vector2 bottomLeft)
        {
            var room = new RoomData(Server, RoomType.Hallway);
            room.Area = new Rectangle();
            room.Area.TopLeft = bottomLeft + new Vector2(0, 4);
            room.Area.BottomRight = new Vector2(BottomRight.X, bottomLeft.Y);

            return room;
        }

        protected virtual RoomData GenerateBedroom(Vector2 topLeft, Vector2 bottomRight)
        {
            var room = new RoomData(Server, RoomType.Bedroom);
            room.Area = new Rectangle();
            room.Area.TopLeft = topLeft;
            room.Area.BottomRight = bottomRight;

            return room;
        }

        protected virtual IEnumerable<RoomData> GenerateBedrooms(Vector2 bottomLeft)
        {
            var sizeX = TopRight.X - bottomLeft.X;
            var maxRooms = (int)Math.Floor(sizeX/5);
            if (maxRooms == 0)
                maxRooms = 1;
            var roomCount = RandomNumberGenerator.Next(1, maxRooms);

            var ret = new List<RoomData>(roomCount);
            var roomSize = (int) Math.Round(sizeX / roomCount);

            for (int i=0; i<roomCount; ++i)
            {
                var topLeft = new Vector2(bottomLeft.X + i*roomSize, TopLeft.Y);
                var bottomRight = i == roomCount - 1 ? 
                    new Vector2(TopRight.X, bottomLeft.Y) : 
                    new Vector2(bottomLeft.X + (i+1)*roomSize, bottomLeft.Y);

                ret.Add(GenerateBedroom(topLeft, bottomRight));
            }

            return ret;
        }

        protected override BuildingData GenerateBuildingData()
        {
            var ret = new BuildingData(new WallGenerator(Server, RandomNumberGenerator, ChunksToGenerate));

            var bathroom = GenerateBathroom();
            var livingRoom = GenerateLivingroom(bathroom.Area.BottomRight);
            var diningRoom = GenerateDiningRoom(livingRoom.Area.BottomRight);
            var kitchen = GenerateKitchen(diningRoom.Area.TopRight);
            var hallway = GenerateHallway(diningRoom.Area.TopLeft);
            var bedrooms = GenerateBedrooms(hallway.Area.TopLeft);

            ret.AddRoom(hallway);
            ret.AddRoom(bathroom);
            ret.AddRoom(livingRoom);
            ret.AddRoom(diningRoom);
            ret.AddRoom(kitchen);

            foreach (var room in bedrooms)
                ret.AddRoom(room);

            return ret;
        }

        public override string GetBuildingName()
        {
            return "Small House";
        }
    }
}

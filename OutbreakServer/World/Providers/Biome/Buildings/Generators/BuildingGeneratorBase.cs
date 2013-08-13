using System;
using System.Collections.Generic;
using Outbreak.Items.ItemGenerators;
using Outbreak.Server.World.Providers.Biome.Buildings.Rooms;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.World.Chunks;

namespace Outbreak.Server.World.Providers.Biome.Buildings.Generators
{
    public abstract class BuildingGeneratorBase : IBuildingGenerator
    {
        public Random RandomNumberGenerator { get; set; }
        public int MinRoomSize { get; set; }
        public int MaxRoomSize { get; set; }
        public Vector2 BottomLeft { get; set; }
        public Vector2 TopRight { get; set; }
        public float MainEntranceDirection { get; set; }
        public List<ChunkKey> ChunksToGenerate { get; set; }

        protected readonly IServer Server;
        protected readonly ItemGeneratorDictionary ItemGeneratorDictionary;
        protected GroupGenerator ClutterGenerator { get; private set; }

        private BuildingData _data;

        protected Vector2 BottomRight
        {
            get { return new Vector2(TopRight.X, BottomLeft.Y); }
        }
        protected Vector2 TopLeft
        {
            get { return new Vector2(BottomLeft.X, TopRight.Y); }
        }

        protected float BuildingSizeX
        {
            get { return BottomRight.X - BottomLeft.X; }
        }

        protected float BuildingSizeY
        {
            get { return TopLeft.Y - BottomLeft.Y; }
        }


        protected BuildingGeneratorBase(IGameServer gameServer, ItemGeneratorDictionary itemGeneratorDictionary)
        {
            _data = null;
            Server = gameServer.Engine;
            ItemGeneratorDictionary = itemGeneratorDictionary;
            RegisterGenerators();
        }

        private void RegisterGenerators()
        {
            var generator = new GroupGenerator();
            generator.RegisterGenerator(new EmptyGenerator(ItemGeneratorDictionary, Server));
            generator.RegisterGenerator(new KitchenGenerator(ItemGeneratorDictionary, Server));
            generator.RegisterGenerator(new LivingroomGenerator(ItemGeneratorDictionary, Server));
            generator.RegisterGenerator(new DiningroomGenerator(ItemGeneratorDictionary, Server));
            generator.RegisterGenerator(new ShopFloorGenerator(ItemGeneratorDictionary, Server));

            ClutterGenerator = generator;
        }

        public BuildingData GetBuildingData()
        {
            if (_data == null)
            {
                _data = GenerateBuildingData();
                GenerateRandomClutter(_data.Rooms);
            }
            return _data;
        }

        private void GenerateRandomClutter(IEnumerable<RoomData> rooms)
        {
            foreach (var room in rooms)
            {
                if (!ClutterGenerator.CanGenerateForRoom(room.RoomType))
                    continue;

                room.AddClutter(ClutterGenerator.GenerateClutter(room, RandomNumberGenerator));
            }
        }

        protected abstract BuildingData GenerateBuildingData();
        public abstract string GetBuildingName();
    }
}

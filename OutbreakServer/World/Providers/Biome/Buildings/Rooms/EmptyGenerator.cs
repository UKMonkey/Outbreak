using System;
using System.Collections.Generic;
using Outbreak.Items;
using Outbreak.Items.ItemGenerators;
using Vortex.Interface;


namespace Outbreak.Server.World.Providers.Biome.Buildings.Rooms
{
    public class EmptyGenerator : BasicGenerator
    {
        public EmptyGenerator(ItemGeneratorDictionary generatorDict, IServer server) : 
            base(0, 0, new List<ItemTypeEnum>(), generatorDict, server)
        {
        }

        public override IEnumerable<EntitySpawnData> GenerateRoomClutter(RoomData room, Random randomiser)
        {
            return new List<EntitySpawnData>();
        }

        public override bool CanGenerateForRoom(RoomType type)
        {
            if (type == RoomType.Hallway)
                return true;
            return false;
        }
    }
}

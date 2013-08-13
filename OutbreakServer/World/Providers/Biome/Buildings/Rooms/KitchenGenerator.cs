using System;
using System.Collections.Generic;
using Outbreak.Entities;
using Outbreak.Items;
using Outbreak.Items.ItemGenerators;
using Vortex.Interface;
using Vortex.Interface.EntityBase;

namespace Outbreak.Server.World.Providers.Biome.Buildings.Rooms
{
    public class KitchenGenerator : BasicGenerator
    {
        public KitchenGenerator(ItemGeneratorDictionary generatorDict, IServer server) :
            base(0, 2, GetAllowedItems(), generatorDict, server)
        {
        }

        private static IEnumerable<ItemTypeEnum> GetAllowedItems()
        {
            return new List<ItemTypeEnum>
                       {
                           ItemTypeEnum.Food,
                           ItemTypeEnum.Food,
                           ItemTypeEnum.FirstAidPack
                       };
        }

        public override IEnumerable<EntitySpawnData> GenerateRoomClutter(RoomData room, Random randomiser)
        {
            var entities = new List<Entity>
                               {
                                   EntityFactory.Get(EntityTypeEnum.Chair),
                                   EntityFactory.Get(EntityTypeEnum.Chair),
                                   EntityFactory.Get(EntityTypeEnum.Chair),
                                   EntityFactory.Get(EntityTypeEnum.Chair)
                               };
            var chairRequirements = new Dictionary<PositionRequirement, float>
                                {
                                    {PositionRequirement.FloorLevel, 1}
                                };
            var groupRequirements = new Dictionary<GroupRequirement, float>
                                        {
                                        };

            return new List<EntitySpawnData>
                       {
                           new EntitySpawnData(entities, chairRequirements, groupRequirements)
                       };
        }

        public override bool CanGenerateForRoom(RoomType type)
        {
            if (type == RoomType.Kitchen)
                return true;
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Outbreak.Entities;
using Outbreak.Items;
using Outbreak.Items.ItemGenerators;
using Outbreak.Server.Entities;
using Vortex.Interface;
using Vortex.Interface.EntityBase;

namespace Outbreak.Server.World.Providers.Biome.Buildings.Rooms
{
    public abstract class BasicGenerator : IClutterGenerator
    {
        private readonly int _minCount;
        private readonly int _maxCount;
        private readonly List<ItemTypeEnum> _availableItems; 
        private readonly ItemGeneratorDictionary _itemGeneratorDictionary;
        protected IServer Server;
        protected IEntityFactory EntityFactory { get { return Server.EntityFactory; } }

        public abstract IEnumerable<EntitySpawnData> GenerateRoomClutter(RoomData room, Random randomiser);
        public abstract bool CanGenerateForRoom(RoomType type);


        protected BasicGenerator(int minCount, int maxCount, 
            IEnumerable<ItemTypeEnum> items, ItemGeneratorDictionary generatorDict,
            IServer server)
        {
            _minCount = minCount;
            _maxCount = maxCount;
            _availableItems = items.ToList();
            _itemGeneratorDictionary = generatorDict;
            Server = server;
        }


        protected Entity GenerateRandomItem(Random randomiser)
        {
            if (_availableItems.Count == 0)
                return null;

            var itemType =_availableItems[randomiser.Next(0, _availableItems.Count - 1)];
            var item = _itemGeneratorDictionary[itemType].Generate();
            var entity = EntityFactory.Get((short)EntityTypeEnum.InventoryItem);
            entity.SetInventoryItem(item);

            return entity;
        }

        protected IEnumerable<EntitySpawnData> GetItemEntities(Random randomiser)
        {
            var count = randomiser.Next(_minCount, _maxCount);
            var ret = new List<Entity>();

            for (var i = 0; i < count; ++i)
            {
                var item = GenerateRandomItem(randomiser);
                ret.Add(item);
            }

            var requirements = new Dictionary<PositionRequirement, float>
                                  {
                                      {PositionRequirement.FloorLevel, 1}
                                  };

            return ret.Select(item => new EntitySpawnData(item, requirements));
        }

        public IEnumerable<EntitySpawnData> GenerateClutter(RoomData room, Random randomiser)
        {
            return GenerateRoomClutter(room, randomiser).Concat(GetItemEntities(randomiser));
        }
    }
}

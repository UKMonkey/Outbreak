using System;
using System.Diagnostics;
using Outbreak.Entities;
using Outbreak.Items;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Psy.Core;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;

namespace Outbreak.Server.Entities.Behaviours.OnKilled
{
    public class SpawnItems : IEntityBehaviour
    {
        private readonly GameServer _server;
        private readonly ItemTypeEnum _itemType;
        private readonly short _minCount;
        private readonly short _maxCount;

        public SpawnItems(GameServer server, ItemTypeEnum itemType, short minCount, short maxCount)
        {
            _itemType = itemType;
            _minCount = minCount;
            _maxCount = maxCount;
            _server = server;
            if (minCount > maxCount)
                throw new Exception("Unable to have min > max");
        }


        public void PerformBehaviour(Entity target, Entity instigator)
        {
            var count = StaticRng.Random.Next(_minCount, _maxCount);
            if (count == 0)
                return;

            var distance = StaticRng.Random.NextDouble(0.2, 1);
            var rotation = StaticRng.Random.NextDouble(-Math.PI, Math.PI);
            var position = target.GetPosition();
            var addition = DirectionUtil.CalculateVector((float)rotation) * (float)distance;

            var item = _server.ItemGeneratorDictionary[_itemType].Generate();
            if (count > item.GetItemSpec().GetStackMax())
            {
                count = item.GetItemSpec().GetStackMax();
                Debug.Assert(true, "Unable to generate a stack as large as requested");
            }
            item.SetCount((short)count);

            var inventoryItem = _server.EntityFactory.Get((short)EntityTypeEnum.InventoryItem);
            inventoryItem.SetInventoryItem(item);
            inventoryItem.SetPosition(position + addition);

            _server.Engine.SpawnEntity(inventoryItem);
        }
    }
}

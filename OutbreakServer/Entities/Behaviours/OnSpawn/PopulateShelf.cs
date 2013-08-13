using System.Collections.Generic;
using System.Diagnostics;
using Outbreak.Entities.Properties;
using Outbreak.Items;
using Outbreak.Items.Containers;
using Outbreak.Resources;
using Psy.Core;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;

namespace Outbreak.Server.Entities.Behaviours.OnSpawn
{
    public class PopulateShelf : IEntityBehaviour
    {
        private readonly GameServer _gameServer;

        public PopulateShelf(GameServer gameServer)
        {
            _gameServer = gameServer;
        }


        private void AddItem(Inventory inv, List<ItemTypeEnum> validItems)
        {
            var number = StaticRng.Random.Next(0, validItems.Count - 1);
            var itemType = validItems[number];

            var generator = _gameServer.ItemGeneratorDictionary.Get(itemType);
            var item = generator.Generate();

            var result = inv.AddItem(item, true);
            Debug.Assert(result, "Unable to add item to shelf");
        }


        public void PerformBehaviour(Entity target, Entity instigator)
        {
            if (StaticRng.Random.Next(0, 99) > 10)
                return;

            var validItems = target.GetValidInventoryItems();
            var minCount = target.GetMinInventoryItemCount();
            var maxCount = target.GetMaxInventoryItemCount();

            Debug.Assert(validItems.Count > 0, "No valid items");

            var targetCount = StaticRng.Random.Next(minCount, maxCount);

            var inventory = StaticInventoryCache.Instance.CreateNewInventory(true);
            inventory.Initialise((byte)(StashSize.Small), InventoryType.Stash);

            for (var i=0; i<targetCount; ++i)
            {
                AddItem(inventory, validItems);
            }

            target.SetInventory(inventory);
            target.SetModel(Models.StockedShelf01);
        }
    }
}

using Outbreak.Entities.Properties;
using Outbreak.Items;
using Outbreak.Items.Containers;
using Outbreak.Server.Items;
using Psy.Core;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;

namespace Outbreak.Server.Entities.Behaviours.OnSpawn
{
    public class AddItemsToInventory : IEntityBehaviour
    {
        private readonly GameServer _server;
        private readonly ItemTypeEnum[] _items;

        public AddItemsToInventory(GameServer gameServer, ItemTypeEnum[] itemTypes)
        {
            _server = gameServer;
            _items = itemTypes;
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            var inv = target.GetInventory();
            var itemType = _items.RandomItem();

            var itemGenerator = _server.ItemGeneratorDictionary.Get(itemType);
            var item = itemGenerator.Generate();

            inv.AddItem(item);

            target.SetEquippedItemId((byte) InventorySpecialSlotEnum.PrimaryWeapon);
        }
    }
}

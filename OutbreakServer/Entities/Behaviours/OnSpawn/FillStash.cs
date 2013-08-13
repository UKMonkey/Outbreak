using Outbreak.Entities.Properties;
using Outbreak.Items;
using Outbreak.Items.Containers;
using Outbreak.Server.Items;
using Psy.Core;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;

namespace Outbreak.Server.Entities.Behaviours.OnSpawn
{
    public class FillStash : IEntityBehaviour
    {
        private const float HealthPackProbability = 0.2f;
        private readonly GameServer _gameServer;
        private readonly StashSize _stashSize;

        public FillStash(GameServer gameServer, StashSize stashSize)
        {
            _gameServer = gameServer;
            _stashSize = stashSize;
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            var inventory = StaticInventoryCache.Instance.CreateNewInventory(true);
            inventory.Initialise((byte)_stashSize, InventoryType.Stash);
            
            if (StaticRng.Random.NextDouble() <= HealthPackProbability)
            {
                AddHealthPackToInventory(inventory);
            }

            AddWeaponToInventory(inventory);
            AddWeaponToInventory(inventory);
            AddFoodToInventory(inventory);
            AddAmmoToInventory(inventory);

            target.SetInventory(inventory);
        }

        private void AddWeaponToInventory(Inventory inventory)
        {
            AddItemTypeToInventory(inventory, ItemTypeEnumConsts.Weapons);
        }

        private void AddItemTypeToInventory(Inventory inventory, ItemTypeEnum[] itemTypes)
        {
            var itemType = itemTypes.RandomItem();
            var itemGenerator = _gameServer.ItemGeneratorDictionary.Get(itemType);
            var item = itemGenerator.Generate();
            inventory.AddItem(item, true);
        }

        private void AddFoodToInventory(Inventory inventory)
        {
            AddItemTypeToInventory(inventory, ItemTypeEnumConsts.Food);
        }

        private void AddAmmoToInventory(Inventory inventory)
        {
            AddItemTypeToInventory(inventory, ItemTypeEnumConsts.Ammo);
        }

        private void AddHealthPackToInventory(Inventory inventory)
        {
            AddItemTypeToInventory(inventory, ItemTypeEnumConsts.HealthPacks);
        }
    }
}
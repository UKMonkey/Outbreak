using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers;

namespace Outbreak.Server.Entities.Behaviours.OnSpawn
{
    public class CreateInventory : IEntityBehaviour
    {
        private readonly byte _size;
        private readonly bool _persistent;

        public CreateInventory(byte size, bool persistent)
        {
            _size = size;
            _persistent = persistent;
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            var inventory = StaticInventoryCache.Instance.CreateNewInventory(_persistent);
            inventory.Initialise(_size, InventoryType.PlayerBackpack);

            target.SetInventory(inventory);
        }
    }
} 

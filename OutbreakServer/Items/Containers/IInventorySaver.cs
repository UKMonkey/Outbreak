using Outbreak.Items.Containers;

namespace Outbreak.Server.Items.Containers
{
    public interface IInventorySaver
    {
        void SaveInventory(Inventory item);
        void DeleteInventory(Inventory inventoryId);
    }
}

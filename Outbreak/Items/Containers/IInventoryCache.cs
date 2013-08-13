namespace Outbreak.Items.Containers
{
    public delegate void InventoryCallback(Inventory items);

    public interface IInventoryCache
    {
        long UnknownId { get; }

        Inventory GetInventory(long inventoryId);
        Inventory CreateNewInventory(bool keepPersisted);
        void RemoveInventory(long inventoryId);
    }
}

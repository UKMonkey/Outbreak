using Outbreak.Items.Containers.InventoryItems;

namespace Outbreak.Items
{
    public interface IItemGenerator
    {
        InventoryItem Generate();
    }
}

using Outbreak.Items.Containers.InventoryItems;

namespace Outbreak.Items.Modifications
{
    public interface IInventoryItemModification
    {
        void Apply(InventoryItem target);
    }
}

using Outbreak.Items.Containers.InventorySpecs;

namespace Outbreak.Items.Modifications
{
    public interface IItemSpecModification
    {
        void Apply(ItemSpec target);
    }
}

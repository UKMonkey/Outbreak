using System.Collections.Generic;

namespace Outbreak.Items.Containers.InventorySpecs
{
    public delegate void ItemSpecCallback(ItemSpec spec);
    public delegate bool ItemSpecChecker(ItemSpec spec);

    public interface IItemSpecCache
    {
        event ItemSpecCallback OnItemAdded;
        int EmptySpecId { get; }

        ItemSpec GetItemSpec(int id);
        ItemSpec AddSpec(ItemSpec spec);
        IEnumerable<ItemSpec> GetSpecsOfType(ItemSpecChecker checker);
    }
}

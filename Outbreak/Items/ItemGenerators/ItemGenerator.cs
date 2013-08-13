using System.Collections.Generic;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Psy.Core;

namespace Outbreak.Items.ItemGenerators
{
    public abstract class ItemGenerator : IItemGenerator
    {
        public InventoryItem Generate()
        {
            var spec = GetItemSpec();
            var item = new InventoryItem(spec.Id);

            PerformModifications(item);

            return item;
        }

        protected virtual int GetMaxCount()
        {
            return 1000;
        }

        private ItemSpec GetItemSpec()
        {
            // todo: refactor this.
            var existingSpecs = GetExistingSpecs();

            if (existingSpecs.Count == 0)
                return GenerateItemSpec();

            var maxCount = GetMaxCount();
            if (existingSpecs.Count == maxCount)
                return GetRandomExistingItemSpec(existingSpecs);

            if (StaticRng.Random.Next(0, maxCount) > existingSpecs.Count)
                return GenerateItemSpec();

            return GetRandomExistingItemSpec(existingSpecs);
        }

        private ItemSpec GenerateItemSpec()
        {
            var baseSpec = GetDefaultObject();
            PerformModifications(baseSpec);
            baseSpec = StaticItemSpecCache.Instance.AddSpec(baseSpec);
            return baseSpec;
        }

        private ItemSpec GetRandomExistingItemSpec(IList<ItemSpec> specs)
        {
            return specs[StaticRng.Random.Next(0, specs.Count)];
        }

            // Get all the specs that could be returned rather than creating a new one
        protected abstract List<ItemSpec> GetExistingSpecs();

            // returns a new copy of the item spec that will be used as a base
            // we will perform some tweeks on it before storing this spec as a perminant feature
        protected abstract ItemSpec GetDefaultObject();

            // performs the modifications to the item spec passed in according to any requirements
            // the net result will be registered as a new type.
        protected abstract void PerformModifications(ItemSpec target);

            // performs the modifications to the item
            // eg count, improvements etc etc
        protected abstract void PerformModifications(InventoryItem item);
    }
}

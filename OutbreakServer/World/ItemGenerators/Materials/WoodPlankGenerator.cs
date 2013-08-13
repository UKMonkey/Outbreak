using System.Collections.Generic;
using System.Linq;
using Outbreak.Items;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.ItemGenerators;
using Outbreak.Resources;

namespace Outbreak.Server.World.ItemGenerators.Materials
{
    public class WoodPlankGenerator : ItemGenerator
    {
        private WoodPlankGenerator()
        {
        }

        public static IItemGenerator GetGenerator()
        {
            return new WoodPlankGenerator();
        }

        protected override int GetMaxCount()
        {
            return 1;
        }

        protected override List<ItemSpec> GetExistingSpecs()
        {
            return StaticItemSpecCache.Instance.GetSpecsOfType(
                item => item.GetResourceType() == ResourceType.Wood
                ).ToList();
        }

        protected override ItemSpec GetDefaultObject()
        {
            var spec = new ItemSpec();

            spec.SetDescription("Wooden plank - used for building");
            spec.SetName("Wooden plank");
            spec.SetCost(10);
            spec.SetImageName(Icons.WoodenPlank01);
            spec.SetModelName(Models.WoodenPlank01);
            spec.SetResourceAmount(1);
            spec.SetResourceType(ResourceType.Wood);
            spec.SetStackMax(64);

            return spec;
        }

        protected override void PerformModifications(ItemSpec target)
        {
            // no mods to the spec
        }

        protected override void PerformModifications(InventoryItem item)
        {
            // just set the count to be 1 - something might change it later but at least it has a sane default
            item.SetCount(1);
        }
    }
}

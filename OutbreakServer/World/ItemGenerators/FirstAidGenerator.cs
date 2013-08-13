using System.Collections.Generic;
using System.Linq;
using Outbreak.Items;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.ItemGenerators;
using Outbreak.Resources;

namespace Outbreak.Server.World.ItemGenerators
{
    public class FirstAidGenerator : ItemGenerator
    {
        public static IItemGenerator GetGenerator()
        {
            return new FirstAidGenerator();
        }

        protected override List<ItemSpec> GetExistingSpecs()
        {
            return StaticItemSpecCache.Instance.GetSpecsOfType(
                item => item.IsHealthPack()).ToList();
        }

        protected override ItemSpec GetDefaultObject()
        {
            var ret = new ItemSpec();
            ret.SetDescription("Something for the pain\nRight click to use");
            ret.SetName("Health pack");
            ret.SetCost(5);
            ret.SetImageName(Icons.FirstAidKit);
            ret.SetModelName(Models.HealthPack01);

            ret.SetHealAmount(50);
            ret.SetStackMax(5);
            ret.SetBaseUsageTime(3000);

            return ret;
        }

        protected override void PerformModifications(ItemSpec target)
        {
            // for now, we'll not edit the healthpack...
        }

        protected override void PerformModifications(InventoryItem item)
        {
            item.SetCount(1);
        }
    }
}

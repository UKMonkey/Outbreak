using System.Collections.Generic;
using System.Linq;
using Outbreak.Items;
using Outbreak.Items.Containers;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.ItemGenerators;
using Outbreak.Items.Modifications;
using Outbreak.Resources;

namespace Outbreak.Server.World.ItemGenerators.Equipment
{
    public class HeadGenerator : ItemGenerator
    {
        private readonly MaterialModification _materialModification;

        private HeadGenerator()
        {
            _materialModification = new MaterialModification(InventorySpecialSlotEnum.HeadArmour);
        }

        public static IItemGenerator GetGenerator()
        {
            return new HeadGenerator();
        }

        protected override List<ItemSpec> GetExistingSpecs()
        {
            return StaticItemSpecCache.Instance.GetSpecsOfType(
                item => item.CanBeWornOnLocation(ArmourWearLocation.Head)
                ).ToList();
        }

        protected override ItemSpec GetDefaultObject()
        {
            var spec = new ItemSpec();

            spec.SetDescription("Headwear");
            spec.SetName("Headwear");
            spec.SetCost(10);
            spec.SetImageName(Icons.HeadItem);
            spec.SetModelName(Models.TopHat01);
            spec.AddArmourType(ArmourWearLocation.Head);
            spec.SetStackMax(1);

            return spec;
        }

        protected override void PerformModifications(ItemSpec target)
        {
            _materialModification.Apply(target);
        }

        protected override void PerformModifications(InventoryItem item)
        {
            var spec = item.GetItemSpec();
            item.SetDurability(spec.GetDurabilityMax());
            item.SetCount(1);
        }
    }
}

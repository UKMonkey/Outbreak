using Outbreak.Items.Containers;
using Outbreak.Items.Containers.InventorySpecs;
using Psy.Core;


namespace Outbreak.Items.Modifications
{
    /** 
     * Apply a material and its defences to an item...
     */
    public class MaterialModification : IItemSpecModification
    {
        private readonly InventorySpecialSlotEnum _slot;

        public MaterialModification(InventorySpecialSlotEnum slot)
        {
            _slot = slot;
        }

        private static string GetRandomMaterial()
        {
            return WearableItemStatsCache.GetMaterials()[StaticRng.Random.Next(0, WearableItemStatsCache.GetMaterials().Count)];
        }

        public void Apply(ItemSpec target)
        {
            var material = GetRandomMaterial();
            target.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.Material, material));

            var multipliers = WearableItemStatsCache.GetBaseSlotDefences(_slot);
            var defences = WearableItemStatsCache.GetBaseDamageDefences(material, multipliers);

            target.SetDefenceMultipliers(defences);
            target.SetDurabilityMax(WearableItemStatsCache.GetBaseMaxDurability(material));
        }
    }
}

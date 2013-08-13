using System.Collections.Generic;
using System.Linq;
using Outbreak.Items;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.Containers.InventorySpecs.Types;
using Outbreak.Items.ItemGenerators;
using Outbreak.Resources;

namespace Outbreak.Server.World.ItemGenerators.Weapons
{
    public class PistolGenerator: ItemGenerator
    {
        private PistolGenerator()
        {
        }


        public static IItemGenerator GetGenerator()
        {
            return new PistolGenerator();
        }


        protected override List<ItemSpec> GetExistingSpecs()
        {
            return StaticItemSpecCache.Instance.GetSpecsOfType(
                item => item.IsWeapon() &&
                    item.GetWeaponType() == WeaponTypes.Pistol
                ).ToList();
        }


        protected override ItemSpec GetDefaultObject()
        {
            var ret = new ItemSpec();
            ret.SetDescription("A single-action, semi-automatic,\nmagazine-fed, recoil-operated handgun.");
            ret.SetName("Pistol");
            ret.SetImageName(Icons.PistolM1911);
            ret.SetModelName(Models.Pistol01);
            ret.SetStackMax(1);
            ret.SetAmmoType(AmmoType.Bullet9Mm);
            ret.SetWeaponType(WeaponTypes.Pistol);
            ret.SetReloadClipSize(-1);
            ret.SetStartReloadTime(500);
            ret.SetReloadTime(2500);
            ret.SetStopReloadTime(1);
            ret.SetClipSize(12);
            ret.SetWeaponNoiseDistance(50f);
            ret.SetCost(10);

            return ret;
        }


        protected override void PerformModifications(ItemSpec target)
        {
        }


        protected override void PerformModifications(InventoryItem item)
        {
            item.SetCount(1);
        }
    }
}

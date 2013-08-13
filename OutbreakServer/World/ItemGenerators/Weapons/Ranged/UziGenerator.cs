using System.Collections.Generic;
using System.Linq;
using Outbreak.Items;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.Containers.InventorySpecs.Types;
using Outbreak.Items.ItemGenerators;
using Outbreak.Resources;

namespace Outbreak.Server.World.ItemGenerators.Weapons.Ranged
{
    public class UziGenerator : ItemGenerator
    {
        private UziGenerator()
        {
        }

        public static IItemGenerator GetGenerator()
        {
            return new UziGenerator();
        }


        protected override List<ItemSpec> GetExistingSpecs()
        {
            return StaticItemSpecCache.Instance.GetSpecsOfType(
                item => item.IsWeapon() &&
                    item.GetWeaponType() == WeaponTypes.Uzi
                ).ToList();
        }


        protected override ItemSpec GetDefaultObject()
        {
            var ret = new ItemSpec();
            ret.SetDescription("Spray happy");
            ret.SetName("Uzi");
            ret.SetImageName(Icons.PistolM1911);
            ret.SetModelName(Models.Pistol01);
            ret.SetStackMax(1);
            ret.SetAmmoType(AmmoType.Bullet9Mm);
            ret.SetWeaponType(WeaponTypes.Uzi);
            ret.SetReloadClipSize(-1);
            ret.SetStartReloadTime(500);
            ret.SetReloadTime(2500);
            ret.SetStopReloadTime(1);
            ret.SetClipSize(28);
            ret.SetWeaponNoiseDistance(50f);
            ret.SetCost(10);
            ret.SetWeaponDelay(3);

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
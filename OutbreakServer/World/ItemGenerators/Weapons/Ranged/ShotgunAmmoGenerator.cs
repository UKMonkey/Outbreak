using System.Collections.Generic;
using System.Linq;
using Outbreak.Items;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.Containers.InventorySpecs.Types;
using Outbreak.Items.ItemGenerators;
using Outbreak.Resources;
using Vortex.Interface;

namespace Outbreak.Server.World.ItemGenerators.Weapons.Ranged
{
    public class ShotgunAmmoGenerator : ItemGenerator
    {
        private ShotgunAmmoGenerator()
        {
        }


        public static IItemGenerator GetGenerator()
        {
            return new ShotgunAmmoGenerator();
        }

        protected override List<ItemSpec> GetExistingSpecs()
        {
            return StaticItemSpecCache.Instance.GetSpecsOfType(
                item => item.IsAmmo() &&
                    item.GetAmmoType() == AmmoType.Bullet9Mm
                ).ToList();
        }

        protected override ItemSpec GetDefaultObject()
        {
            var spec = new ItemSpec();
            spec.SetDescription("Shotgun Ammo");
            spec.SetName("Slugs");
            spec.SetCost(1);
            spec.SetImageName(Icons.ShotgunAmmo);
            spec.SetModelName(Models.AmmoCrate01);
            spec.SetStackMax(16);
            spec.SetAmmoType(AmmoType.ShotgunSlug);
            spec.SetDamageMin(2f);
            spec.SetDamageMax(3f);
            spec.SetDamageType(DamageTypeEnum.LowCaliberBullet);
            return spec;
        }

        protected override void PerformModifications(ItemSpec target)
        {
        }

        protected override void PerformModifications(InventoryItem item)
        {
            item.SetCount(16);
        }
    }
}
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
    public class ShotgunGenerator : ItemGenerator
    {
        private ShotgunGenerator()
        {}

        public static IItemGenerator GetGenerator()
        {
            return new ShotgunGenerator();
        }

        protected override List<ItemSpec> GetExistingSpecs()
        {
            return StaticItemSpecCache.Instance.GetSpecsOfType(
                item => item.IsWeapon() &&
                    item.GetAmmoType() == AmmoType.ShotgunSlug
                ).ToList();
        }

        protected override ItemSpec GetDefaultObject()
        {
            var spec = new ItemSpec();

            spec.SetDescription("A big fat shotgun");
            spec.SetName("Shotgun");
            spec.SetCost(35);
            spec.SetImageName(Icons.Shotgun);
            spec.SetModelName(Models.Pistol01);
            spec.SetStackMax(1);
            spec.SetAmmoType(AmmoType.ShotgunSlug);
            spec.SetWeaponType(WeaponTypes.Shotgun);
            spec.SetReloadClipSize(1);
            spec.SetStartReloadTime(0);
            spec.SetReloadTime(500);
            spec.SetStopReloadTime(50);
            spec.SetClipSize(8);
            spec.SetBulletSpread(0.4f);
            spec.SetWeaponNoiseDistance(80f);

            return spec;
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
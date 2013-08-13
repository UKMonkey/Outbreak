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
    public class PistolAmmoGenerator : ItemGenerator
    {
        private PistolAmmoGenerator()
        {
        }

        public static IItemGenerator GetGenerator()
        {
            return new PistolAmmoGenerator();
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

            spec.SetDescription("9×19mm Parabellum");
            spec.SetName("9mm");
            spec.SetCost(1);
            spec.SetImageName(Icons.GenericAmmo);
            spec.SetModelName(Models.AmmoCrate01);
            spec.SetStackMax(128);
            spec.SetAmmoType(AmmoType.Bullet9Mm);
            spec.SetDamageMin(4.0f);
            spec.SetDamageMax(5.0f);
            spec.SetDamageType(DamageTypeEnum.LowCaliberBullet);

            return spec;
        }

        protected override void PerformModifications(ItemSpec target)
        {
        }

        protected override void PerformModifications(InventoryItem item)
        {
            item.SetCount(128);
        }
    }
}
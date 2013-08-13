using System.Collections.Generic;
using System.Linq;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.Containers.InventorySpecs.Types;
using Outbreak.Items.ItemGenerators;
using Outbreak.Resources;
using Vortex.Interface;

namespace Outbreak.Server.World.ItemGenerators.Weapons.Melee
{
    public class ZombieMaulGenerator : ItemGenerator
    {
        protected override List<ItemSpec> GetExistingSpecs()
        {
            return StaticItemSpecCache.Instance.GetSpecsOfType(
                item => item.IsWeapon() &&
                    item.GetWeaponType() == WeaponTypes.ZombieMaul
                ).ToList();
        }

        protected override ItemSpec GetDefaultObject()
        {
            var ret = new ItemSpec();
            ret.SetDescription("The mauling of a zombie");
            ret.SetName("Grrg Arg");
            ret.SetImageName(Icons.PistolM1911);
            ret.SetModelName(Models.Pistol01);
            ret.SetStackMax(1);
            ret.SetWeaponType(WeaponTypes.ZombieMaul);
            ret.SetWeaponNoiseDistance(2f);

            ret.SetDamageMin(10);
            ret.SetDamageMax(20);
            ret.SetDamageType(DamageTypeEnum.Maul);
            ret.SetMeleeRange(0.5f);
            ret.SetWeaponDelay(15);

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

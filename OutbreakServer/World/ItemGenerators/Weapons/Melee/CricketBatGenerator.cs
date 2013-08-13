using System.Collections.Generic;
using System.Linq;
using Outbreak.Items;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.Containers.InventorySpecs.Types;
using Outbreak.Items.ItemGenerators;
using Outbreak.Resources;
using Vortex.Interface;

namespace Outbreak.Server.World.ItemGenerators.Weapons.Melee
{
    public class CricketBatGenerator : ItemGenerator
    {
        private CricketBatGenerator()
        {
        }


        public static IItemGenerator GetGenerator()
        {
            return new CricketBatGenerator();
        }


        protected override List<ItemSpec> GetExistingSpecs()
        {
            return StaticItemSpecCache.Instance.GetSpecsOfType(
                item => item.IsWeapon() &&
                    item.GetWeaponType() == WeaponTypes.CricketBat
                ).ToList();
        }


        protected override ItemSpec GetDefaultObject()
        {
            var ret = new ItemSpec();
            ret.SetDescription("A solid blank of wood usually used for hitting balls\nWill probably work quite well on zombies at close range");
            ret.SetName("Cricket bat");
            ret.SetImageName(Icons.CricketBat01);
            ret.SetModelName(Models.CricketBat01);
            ret.SetStackMax(1);
            ret.SetWeaponType(WeaponTypes.CricketBat);
            ret.SetWeaponNoiseDistance(1f);

            ret.SetDamageMin(10);
            ret.SetDamageMax(20);
            ret.SetDamageType(DamageTypeEnum.BluntMelee);
            ret.SetMeleeRange(1.3f);
            ret.SetWeaponDelay(50);
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

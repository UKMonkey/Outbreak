using Outbreak.Items;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.Containers.InventorySpecs.Types;
using Outbreak.Items.ItemGenerators;
using Outbreak.Items.Modifications;
using Outbreak.Resources;
using Vortex.Interface;

namespace Outbreak.Server.World.ItemGenerators
{
    public static class ShotgunAmmoGenerator
    {
         public static IItemGenerator GetGenerator()
         {
             var builder = new ConfigurableItemGeneratorBuilder();

             return builder.
                 AddDefaultSpec().
                     AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.Description, "Shotgun Ammo")).
                     AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.Name, "Slugs")).
                     AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.Cost, 3)).
                     AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.ImageName, Icons.ShotgunAmmo)).
                     AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.ModelName, Models.AmmoCrate01)).
                     AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.StackMax, 16)).
                     AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.AmmoType, (short)AmmoType.ShotgunSlug)).
                     AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.DamageMin, 2.0f)).
                     AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.DamageMax, 3.0f)).
                     AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.DamageType, (short)DamageType.LowCaliberBullet)).
                 AddSpecModification(new FloatModification(ItemSpecPropertyEnum.DamageMin, 2.0f, 3.0f)).
                 AddSpecModification(new FloatModification(ItemSpecPropertyEnum.DamageMax, 3.0f, 5.0f)).
                 AddItemModification(new ShortModification(InventoryItemPropertyEnum.Count, 10, 16)).
                 GetGenerator(0, 1, 1, 1);
         }
    }
}
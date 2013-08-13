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
    public static class PistolAmmoGenerator
    {
        public static IItemGenerator GetGenerator()
        {
            var builder = new ConfigurableItemGeneratorBuilder();

            return builder.
                AddDefaultSpec().
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.Description, "9×19mm Parabellum")).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.Name, "9mm")).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.Cost, 1)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.ImageName, Icons.GenericAmmo)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.ModelName, Models.AmmoCrate01)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.StackMax, 128)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.AmmoType, (short)AmmoType.Pistol9Mm)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.DamageMin, 4.0f)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.DamageMax, 5.0f)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.DamageType, (short)DamageType.LowCaliberBullet)).
                AddSpecModification(new FloatModification(ItemSpecPropertyEnum.DamageMin, 3.0f, 5.0f)).
                AddSpecModification(new FloatModification(ItemSpecPropertyEnum.DamageMax, 4.0f, 6.0f)).
                AddItemModification(new ShortModification(InventoryItemPropertyEnum.Count, 30, 128)).
                GetGenerator(0, 1, 1, 1);
        }
    }
}
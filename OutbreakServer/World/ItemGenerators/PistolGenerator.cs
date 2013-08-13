using Outbreak.Items;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.Containers.InventorySpecs.Types;
using Outbreak.Items.ItemGenerators;
using Outbreak.Items.Modifications;
using Outbreak.Resources;

namespace Outbreak.Server.World.ItemGenerators
{
    public static class PistolGenerator
    {
        public static IItemGenerator GetGenerator()
        {
            var builder = new ConfigurableItemGeneratorBuilder();

            return builder.
                AddDefaultSpec().
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.Description, "A single-action, semi-automatic,\n magazine-fed, recoil-operated handgun.")).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.Name, "Pistol")).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.Cost, 10)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.ImageName, Icons.PistolM1911)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.ModelName, Models.Pistol01)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.StackMax, 1)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.AmmoType, (short)AmmoType.Pistol9Mm)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.WeaponType, (short)WeaponTypes.Pistol)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.ReloadClipSize, (short)-1)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.StartReloadTime, 500)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.ReloadTime, 2500)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.StopReloadTime, 1)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.ClipSize, (short)12)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.NoiseRange, 50f)).
                AddSpecModification(new ShortModification(ItemSpecPropertyEnum.ClipSize, 8, 16)).
                AddSpecModification(new IntModification(ItemSpecPropertyEnum.Cost, 8, 20)).
                AddSpecModification(new IntModification(ItemSpecPropertyEnum.ReloadTime, 2000, 3000)).
                AddItemModification(new ShortModification(InventoryItemPropertyEnum.Count, 1, 1)).
                GetGenerator(1, 3, 1, 1);
        }
    }
}
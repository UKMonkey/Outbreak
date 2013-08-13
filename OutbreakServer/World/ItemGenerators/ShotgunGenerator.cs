using Outbreak.Items;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.Containers.InventorySpecs.Types;
using Outbreak.Items.ItemGenerators;
using Outbreak.Items.Modifications;
using Outbreak.Resources;

namespace Outbreak.Server.World.ItemGenerators
{
    public static class ShotgunGenerator
    {
        public static IItemGenerator GetGenerator()
        {
            var builder = new ConfigurableItemGeneratorBuilder();

            return builder.
                AddDefaultSpec().
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.Description, "A big fat shotgun")).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.Name, "Shotgun")).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.Cost, 35)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.ImageName, Icons.Shotgun)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.ModelName, Models.Pistol01)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.StackMax, 1)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.AmmoType, (short)AmmoType.ShotgunSlug)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.WeaponType, (short)WeaponTypes.Shotgun)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.ReloadClipSize, (short)1)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.StartReloadTime, 0)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.ReloadTime, 500)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.StopReloadTime, 0)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.ClipSize, (short)8)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.BulletSpread, 0.4f)).
                    AddDefaultSpecProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.NoiseRange, 80f)).
                AddSpecModification(new ShortModification(ItemSpecPropertyEnum.ClipSize, 6, 10)).
                AddSpecModification(new IntModification(ItemSpecPropertyEnum.Cost, 30, 40)).
                AddSpecModification(new IntModification(ItemSpecPropertyEnum.ReloadTime, 250, 500)).
                AddSpecModification(new FloatModification(ItemSpecPropertyEnum.BulletSpread, 0.3f, 5f)).
                AddItemModification(new ShortModification(InventoryItemPropertyEnum.Count, 1, 1)).
                GetGenerator(1, 3, 1, 1);
        }  
    }
}
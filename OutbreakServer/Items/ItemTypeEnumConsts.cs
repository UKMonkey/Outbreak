using Outbreak.Items;

namespace Outbreak.Server.Items
{
    public static class ItemTypeEnumConsts
    {
        public static readonly ItemTypeEnum[] HealthPacks = new[] { ItemTypeEnum.FirstAidPack };
        public static readonly ItemTypeEnum[] Ammo = new[] { ItemTypeEnum.ShotgunAmmo, ItemTypeEnum.PistolAmmo };
        public static readonly ItemTypeEnum[] Weapons = new[] { ItemTypeEnum.CricketBat, ItemTypeEnum.Pistol, ItemTypeEnum.Shotgun };
        public static readonly ItemTypeEnum[] Food = new[] { ItemTypeEnum.Food };
        public static readonly ItemTypeEnum[] ZombieWeapons = new[] { ItemTypeEnum.ZombieMaul };
    }
}
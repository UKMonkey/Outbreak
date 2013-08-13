using System.Collections.Generic;
using Outbreak.Items.Containers.InventorySpecs.Types;
using Vortex.Interface;

namespace Outbreak.Items.Containers.InventorySpecs
{
    public static class ItemSpecExtensions
    {
        public static string GetDescription(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.Description).StringValue;
        }

        public static void SetDescription(this ItemSpec item, string value)
        {
            var spec = new ItemSpecProperty((short) ItemSpecPropertyEnum.Description, value);
            item.SetProperty(spec);
        }

        /**/
        public static string GetName(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.Name).StringValue;
        }

        public static void SetName(this ItemSpec item, string value)
        {
            var spec = new ItemSpecProperty((short)ItemSpecPropertyEnum.Name, value);
            item.SetProperty(spec);
        }

        /**/
        public static string GetModelName(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.ModelName).StringValue;
        }

        public static void SetModelName(this ItemSpec item, string value)
        {
            var spec = new ItemSpecProperty((short)ItemSpecPropertyEnum.ModelName, value);
            item.SetProperty(spec);
        }

        /**/
        public static string GetImageName(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.ImageName).StringValue;
        }

        public static void SetImageName(this ItemSpec item, string value)
        {
            var spec = new ItemSpecProperty((short)ItemSpecPropertyEnum.ImageName, value);
            item.SetProperty(spec);
        }

        /**/
        public static int GetCost(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.Cost).IntValue;
        }

        public static void SetCost(this ItemSpec item, int value)
        {
            var spec = new ItemSpecProperty((short)ItemSpecPropertyEnum.Cost, value);
            item.SetProperty(spec);
        }

        /**/
        public static byte GetStackMax(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.StackMax).ByteValue;
        }

        public static void SetStackMax(this ItemSpec item, byte value)
        {
            var spec = new ItemSpecProperty((short)ItemSpecPropertyEnum.StackMax, value);
            item.SetProperty(spec);
        }

        /**/
        public static short GetDurabilityMax(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.DurabilityMax).ShortValue;
        }

        public static void SetDurabilityMax(this ItemSpec item, short value)
        {
            var spec = new ItemSpecProperty((short)ItemSpecPropertyEnum.DurabilityMax, value);
            item.SetProperty(spec);
        }

        public static bool HasDurability(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.DurabilityMax) != null;
        }

        /****/
        public static short GetBaseUsageTime(this ItemSpec item)
        {
            var prop = item.GetProperty(ItemSpecPropertyEnum.BaseUsageTime);
            if (prop == null)
                return -1;
            return prop.ShortValue;
        }

        public static void SetBaseUsageTime(this ItemSpec item, short value)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.BaseUsageTime, value));
        }

        /****/
        public static bool IsAmmo(this ItemSpec item)
        {
            if (item.IsWeapon())
                return false;

            return item.GetAmmoType() != null;
        }

        public static AmmoType? GetAmmoType(this ItemSpec item)
        {
            var prop = item.GetProperty(ItemSpecPropertyEnum.AmmoType);
            if (prop == null)
                return null;
            return (AmmoType) prop.ShortValue;
        }

        public static void SetAmmoType(this ItemSpec item, AmmoType value)
        {
            item.SetProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.AmmoType, (short)value));
        }

        /****/
        public static bool IsWeapon(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.WeaponType) != null;
        }

        public static WeaponTypes? GetWeaponType(this ItemSpec item)
        {
            var prop = item.GetProperty(ItemSpecPropertyEnum.WeaponType);
            if (prop == null)
                return null;
            return (WeaponTypes)prop.ShortValue;
        }

        public static void SetWeaponType(this ItemSpec item, WeaponTypes value)
        {
            item.SetProperty(new ItemSpecProperty((short)ItemSpecPropertyEnum.WeaponType, (short)value));
        }

        /****/

        public static void AddArmourType(this ItemSpec item, ArmourWearLocation type)
        {
            var prop = item.GetProperty(ItemSpecPropertyEnum.ArmourType);
            if (prop == null)
            {
                item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.ArmourType, (short) type));
            }
            else
            {
                var value = prop.ShortValue | (short) type;
                item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.ArmourType, value));
            }
        }

        public static bool CanBeWornOnLocation(this ItemSpec item, ArmourWearLocation target)
        {
            var prop = item.GetProperty(ItemSpecPropertyEnum.ArmourType);
            if (prop == null)
                return false;

            return (prop.ShortValue & (short)target) != 0;            
        }

        public static bool IsHeadwear(this ItemSpec item)
        {
            return item.CanBeWornOnLocation(ArmourWearLocation.Head);
        }

        public static bool IsBodywear(this ItemSpec item)
        {
            return item.CanBeWornOnLocation(ArmourWearLocation.Body);
        }

        public static bool IsLegwear(this ItemSpec item)
        {
            return item.CanBeWornOnLocation(ArmourWearLocation.Legs);
        }

        public static bool IsFootwear(this ItemSpec item)
        {
            return item.CanBeWornOnLocation(ArmourWearLocation.Feet);
        }

        /**/
        public static short GetStartReloadTime(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.StartReloadTime).ShortValue;
        }

        public static void SetStartReloadTime(this ItemSpec item, short value)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.StartReloadTime, value));
        }

        public static short GetReloadTime(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.ReloadTime).ShortValue;
        }

        public static void SetReloadTime(this ItemSpec item, short value)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.ReloadTime, value));
        }

        public static short GetStopReloadTime(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.StopReloadTime).ShortValue;
        }

        public static void SetStopReloadTime(this ItemSpec item, short value)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.StopReloadTime, value));
        }

        /****/
        public static uint GetWeaponDelay(this ItemSpec item)
        {
            var prop = item.GetProperty(ItemSpecPropertyEnum.UsageDelay);
            if (prop == null)
                return 0;
            return (uint)prop.IntValue;
        }

        public static void SetWeaponDelay(this ItemSpec item, uint value)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.UsageDelay, (int)value));
        }

        /****/
        public static float GetMeleeRange(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.MeleeRange).FloatValue;
        }

        public static void SetMeleeRange(this ItemSpec item, float value)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.MeleeRange, value));
        }

        /****/
        public static bool RequiresAmmo(this ItemSpec item)
        {
            if (!item.HasProperty(ItemSpecPropertyEnum.ClipSize))
                return false;

            return item.GetClipSize() != 0;
        }

        public static short GetClipSize(this ItemSpec item)
        {
            var prop = item.GetProperty(ItemSpecPropertyEnum.ClipSize);
            if (prop == null)
                return 0;
            return item.GetProperty(ItemSpecPropertyEnum.ClipSize).ShortValue;
        }

        public static void SetClipSize(this ItemSpec item, short value)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.ClipSize, value));
        }

        public static short GetReloadClipSize(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.ReloadClipSize).ShortValue;
        }

        public static void SetReloadClipSize(this ItemSpec item, short size)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.ReloadClipSize, size));
        }

        /****/
        public static float GetBulletSpread(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.BulletSpread).FloatValue;
        }

        public static void SetBulletSpread(this ItemSpec item, float value)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.BulletSpread, value));
        }

        /****/
        public static float GetDamageMin(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.DamageMin).FloatValue;
        }

        public static void SetDamageMin(this ItemSpec item, float value)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.DamageMin, value));
        }

        public static float GetDamageMax(this ItemSpec item)
        {
            return item.GetProperty(ItemSpecPropertyEnum.DamageMax).FloatValue;
        }

        public static void SetDamageMax(this ItemSpec item, float value)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.DamageMax, value));
        }

        public static DamageTypeEnum GetDamageType(this ItemSpec item)
        {
            return (DamageTypeEnum)(item.GetProperty(ItemSpecPropertyEnum.DamageType).ShortValue);
        }

        public static void SetDamageType(this ItemSpec item, DamageTypeEnum type)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.DamageType, (short)type));
        }

        /****/
        public static short GetHungerReduceAmount(this ItemSpec item)
        {
            var prop = item.GetProperty(ItemSpecPropertyEnum.HungerReduceAmount);
            if (prop == null)
                return 0;

            return prop.ShortValue;
        }

        public static void SetHungerReduceAmount(this ItemSpec item, short value)
        {
            var spec = new ItemSpecProperty((short)ItemSpecPropertyEnum.HungerReduceAmount, value);
            item.SetProperty(spec);
        }

        /****/
        public static short GetHealAmount(this ItemSpec item)
        {
            var prop = item.GetProperty(ItemSpecPropertyEnum.HealAmount);
            if (prop == null)
                return 0;

            return prop.ShortValue;
        }

        public static void SetHealAmount(this ItemSpec item, short value)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.HealAmount, value));
        }

        public static bool IsHealthPack(this ItemSpec item)
        {
            return item.GetHealAmount() > 0 &&
                   item.IsWeapon() == false;
        }

        /****/
        public static float GetWeaponNoiseDistance(this ItemSpec item)
        {
            var prop = item.GetProperty(ItemSpecPropertyEnum.NoiseRange);
            if (prop == null)
                return 0;

            return prop.FloatValue;
        }

        public static void SetWeaponNoiseDistance(this ItemSpec item, float value)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.NoiseRange, value));
        }

        public static void SetDefenceMultipliers(this ItemSpec item, Dictionary<DamageTypeEnum, float> multipliers)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.DefenceValues, multipliers));
        }

        /****/
        public static void SetDefenceMultiplier(this ItemSpec item, DamageTypeEnum damageType, float value)
        {
            var prop = item.GetProperty(ItemSpecPropertyEnum.DefenceValues);
            Dictionary<DamageTypeEnum, float> map;

            if (prop == null)
                map = new Dictionary<DamageTypeEnum, float>();
            else
                map = (Dictionary<DamageTypeEnum, float>)(prop.ObjectValue);

            map[damageType] = value;
            item.SetDefenceMultipliers(map);
        }

        public static float GetDefenceMultiplier(this ItemSpec item, DamageTypeEnum damageType)
        {
            var prop = item.GetProperty(ItemSpecPropertyEnum.DefenceValues);

            if (prop == null)
                return 0;

            var map = (Dictionary<DamageTypeEnum, float>)(prop.ObjectValue);
            if (!map.ContainsKey(damageType))
                return 0;

            return map[damageType];
        }

        /****/
        public static ResourceType GetResourceType(this ItemSpec item)
        {
            var prop = item.GetProperty(ItemSpecPropertyEnum.ResouceType);
            if (prop == null)
                return ResourceType.None;
            return (ResourceType) prop.ShortValue;
        }

        public static void SetResourceType(this ItemSpec item, ResourceType resource)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.ResouceType, (short)resource));
        }

        /****/
        public static byte GetResourceAmount(this ItemSpec item)
        {
            var prop = item.GetProperty(ItemSpecPropertyEnum.ResourceAmount);
            if (prop == null)
                return 0;
            return prop.ByteValue;
        }

        public static void SetResourceAmount(this ItemSpec item, byte amount)
        {
            item.SetProperty(new ItemSpecProperty(ItemSpecPropertyEnum.ResourceAmount, amount));
        }
    }
}

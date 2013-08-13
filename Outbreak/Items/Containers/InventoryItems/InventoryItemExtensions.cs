using System.Diagnostics;
using Outbreak.Items.Containers.InventorySpecs;
using Vortex.Interface;

namespace Outbreak.Items.Containers.InventoryItems
{
    public static class InventoryItemExtensions
    {
        public static short GetCount(this InventoryItem item)
        {
            return item.GetProperty(InventoryItemPropertyEnum.Count).ShortValue;
        }

        public static void SetCount(this InventoryItem item, short value)
        {
            item.SetProperty(new InventoryItemProperty((short)InventoryItemPropertyEnum.Count, value));
        }

        /****/
        public static bool HasDurability(this InventoryItem item)
        {
            return item.GetProperty(InventoryItemPropertyEnum.Durability) != null;
        }

        public static short GetDurability(this InventoryItem item)
        {
            return item.GetProperty(InventoryItemPropertyEnum.Durability).ShortValue;
        }

        public static void SetDurability(this InventoryItem item, short value)
        {
            item.SetProperty(new InventoryItemProperty((short)InventoryItemPropertyEnum.Durability, value));
        }

        /****/
        public static ItemSpec GetItemSpec(this InventoryItem item)
        {
            return StaticItemSpecCache.Instance.GetItemSpec(item.ItemSpecId);
        }

        /****/
        public static uint GetLastUsedTime(this InventoryItem item, IEngine engine)
        {
            var prop = item.GetProperty(InventoryItemPropertyEnum.LastUsedTime);
            if (prop == null)
                return 0;
            if ((uint)prop.IntValue > engine.CurrentFrameNumber)
                return 0;

            return (uint) prop.IntValue;
        }

        public static void SetLastUsedTime(this InventoryItem item, IEngine engine)
        {
            var value = engine.CurrentFrameNumber;
            item.SetProperty(new InventoryItemProperty((short)InventoryItemPropertyEnum.LastUsedTime, value));
        }

        /****/
        public static ItemSpec GetLoadedAmmoType(this InventoryItem item)
        {
            var prop = item.GetProperty(InventoryItemPropertyEnum.LoadedAmmoType);

            return prop == null ? 
                null : 
                StaticItemSpecCache.Instance.GetItemSpec(prop.IntValue);
        }

        public static void SetLoadedAmmoType(this InventoryItem item, ItemSpec ammoType)
        {
            Debug.Assert(ammoType.IsAmmo());
            item.SetProperty(new InventoryItemProperty((short)InventoryItemPropertyEnum.LoadedAmmoType, ammoType.Id));
        }

        /****/
        public static short GetLoadedAmmoCount(this InventoryItem item)
        {
            var prop = item.GetProperty(InventoryItemPropertyEnum.LoadedAmmoCount);

            return prop == null ?
                (short)0 :
                prop.ShortValue;
        }

        public static void SetLoadedAmmoCount(this InventoryItem item, short ammoCount)
        {
            item.SetProperty(new InventoryItemProperty((short)InventoryItemPropertyEnum.LoadedAmmoCount, ammoCount));
        }

        public static bool EditLoadedAmmoCount(this InventoryItem item, short amount)
        {
            var prop = item.GetProperty(InventoryItemPropertyEnum.LoadedAmmoCount);
            if (prop == null && amount < 0)
                return false;

            if (prop == null)
            {
                item.SetLoadedAmmoCount(amount);
                return true;
            }

            if (amount < 0 &&
                amount + prop.ShortValue < 0)
                return false;

            prop.ShortValue = (short)(prop.ShortValue + amount);
            return true;
        }

        /****/
        public static bool GetIsReloading(this InventoryItem item)
        {
            var prop = item.GetProperty(InventoryItemPropertyEnum.Reloading);
            return prop != null && prop.BoolValue;
        }

        public static void SetIsReloading(this InventoryItem item, bool value)
        {
            item.SetProperty(new InventoryItemProperty((short)InventoryItemPropertyEnum.Reloading, value));
        }

        /****/
        public static long GetNextReloadTime(this InventoryItem item)
        {
            var prop = item.GetProperty(InventoryItemPropertyEnum.NextReloadTime);
            return prop == null ?
                0 :
                prop.LongValue;
        }

        public static void SetNextReloadTime(this InventoryItem item, long value)
        {
            item.SetProperty(new InventoryItemProperty((short)InventoryItemPropertyEnum.NextReloadTime, value));
        }

        /****/
        public static bool GetIsFiring(this InventoryItem item)
        {
            var prop = item.GetProperty(InventoryItemPropertyEnum.Firing);
            return prop != null && prop.BoolValue;
        }

        public static void SetIsFiring(this InventoryItem item, bool value)
        {
            item.SetProperty(new InventoryItemProperty((short)InventoryItemPropertyEnum.Firing, value));
        }


        /****/
        public static bool GetFinalReloading(this InventoryItem item)
        {
            var prop = item.GetProperty(InventoryItemPropertyEnum.FinalReloading);
            return prop != null && prop.BoolValue;
        }

        public static void SetFinalReloading(this InventoryItem item, bool value)
        {
            item.SetProperty(new InventoryItemProperty((short)InventoryItemPropertyEnum.FinalReloading, value));
        }

        /****/
        public static float GetWeaponNoiseDistance(this InventoryItem item)
        {
            var spec = item.GetItemSpec();
            return spec.GetWeaponNoiseDistance();
        }

        /****/
        public static float GetItemStatePercent(this InventoryItem item)
        {
            var currentDurability = item.GetDurability();
            var maxDurability = item.GetItemSpec().GetDurabilityMax();

            if (maxDurability == 0)
                return 1;

            return ((float)currentDurability/(float)maxDurability);
        }


        /****/
        public static float GetDefenceMultiplier(this InventoryItem item, DamageTypeEnum damageType)
        {
            // the defence an item can provide is dependant on 
            // 1) the types defence
            // 2) the items upkeep, where the item's value drops at an increasing rate  
            // y = 1-((x-1) * (x-1))

            var state = item.GetItemStatePercent();
            var multiplier = 1 - ((state - 1)*(state - 1));

            var def = item.GetItemSpec().GetDefenceMultiplier(damageType);
            return def*multiplier;
        }
    }
}

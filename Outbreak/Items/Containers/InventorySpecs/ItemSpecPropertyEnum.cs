using System;

namespace Outbreak.Items.Containers.InventorySpecs
{
    public enum ItemSpecPropertyEnum : short
    {
            // generic attributes
        Description,
        Name,
        Cost,
        ModelName,
        ImageName,
        StackMax,
        DurabilityMax,

        AmmoType,
        WeaponType,

        BaseUsageTime,

            // weapon specific attributes
        StartReloadTime = 20,
        UsageDelay,
        MeleeRange,
        ReloadTime,
        StopReloadTime,
        ClipSize,
        ReloadClipSize,
        NoiseRange,

            // auto / shotgun specific attributes
        BulletSpread = 40,

            // ammo / weapon attributes
        DamageMin = 50,
        DamageMax,
        DamageType,
      
            // armour attributes
        Material = 100,
        ArmourType,
        DefenceValues,

            // resource attributes
        ResourceAmount = 150,
        ResouceType,

        HealAmount = 200,
        HungerReduceAmount
    }


    public enum ResourceType
    {
        None,
        Wood
    }

    /**  Enum used in ArmourType
     *   This enum is flags as something like socks could be worn on both the hands and feet...
     *   or a bin bag could be used as a wrapping for all items providing protection Vs blood.
     */
    [Flags]
    public enum ArmourWearLocation
    {
        Head = 1,
        Body = 2,
        Legs = 4,
        Feet = 8
    }
}

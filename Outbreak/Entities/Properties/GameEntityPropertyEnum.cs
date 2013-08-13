using Vortex.Interface.EntityBase.Properties;

namespace Outbreak.Entities.Properties
{
    public enum GameEntityPropertyEnum : short
    {
        InventoryId = EntityPropertyEnum.MaxEngineEnumProperty + 1,

        /// <summary>
        /// Has this entity been registered with an inventory
        /// </summary>
        EntityRegisteredWithInventory,

        /// <summary>
        /// 0 = not visible at all
        /// 0.5 = not been visible for a little while, can probably start fading out
        /// 1 = completely visible
        /// </summary>
        VisibleRating,

        /// <summary>
        /// Is this entity a human? (player or non-player controlled)
        /// </summary>
        IsHuman,

        /// <summary>
        /// Is this entity hungry for brains?
        /// </summary>
        IsZombie,
        
        HealAmount,
        Experience,
        Level,
        LevelExperience,
        ExplosionRange,
        ExplosionDamage,

        /// <summary>
        /// Only applicable for entities with a FloatingItemId. If this is set to true, the entity should
        /// be destroyed once it is picked up.
        /// </summary>
        DestroyOnCollected,

        /// <summary>
        /// This entity is a single item lying on the ground if it has a FloatingItemId property.
        /// </summary>
        FloatingItemId,

        /// <summary>
        /// Zombie moan timer
        /// </summary>
        NextZombieMoan = EntityPropertyEnum.MaxEngineEnumProperty + 201,

        /// <summary>
        /// Entity that the zombie is chasing
        /// </summary>
        ChaseTargetId,
        ChaseMinTime,
        ChaseStartTime,
        ChaseThinkCount,

        ChaseNoiseStartTime,
        ChaseNoiseMinTime,

        RunSpeed = EntityPropertyEnum.MaxEngineEnumProperty + 301,
        WalkSpeed,
        IsRunning,
        KeyboardMovementDir,

        AccelerationRate = EntityPropertyEnum.MaxEngineEnumProperty + 401,
        MaxSpeed,

        EquippedItem = EntityPropertyEnum.MaxEngineEnumProperty + 501,
        WeaponUseCount,

        /// <summary>
        /// for InventoryItem
        /// </summary>
        ItemSpecId,

        /// <summary>
        /// doors have an open and closed position ... this records what angles they are,
        /// and what status they they they're in (so that when you use the door it can go the other way)
        /// </summary>
        DoorClosedAngle,
        DoorOpenAngle,
        DoorCurrentlyOpen,


        /// <summary>
        /// a list of player ids that have caused aggro.
        /// </summary>
        AggroList,


        
        /// <summary>
        /// Is this player god?
        /// </summary>
        IsGod = EntityPropertyEnum.MaxEngineEnumProperty + 601,

        /// <summary>
        /// Hunger level. 0 = No hunger, 100 = Starvation
        /// </summary>
        Hunger,

        /// <summary>
        /// Counts time between each hunger tick.
        /// </summary>
        HungerTimer,

        /// <summary>
        /// Mood. 0 = Neutral, -100 = Depressed, 100 = I'M SUPER!, THANKS FOR ASKING
        /// </summary>
        Mood,

        ValidInventoryItems,
        InventoryItemMin,
        InventoryItemMax,

        /// <summary>
        /// what items the entity has in visible slots
        /// </summary>
        PrimaryWeaponItem,
        SecondaryWeaponItem,
        HeadSlotItem,
    }
}

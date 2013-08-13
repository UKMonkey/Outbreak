using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Outbreak.Entities.Behaviours;
using Outbreak.Items;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Properties;
using Outbreak.Items.Containers;
using Outbreak.Items.Containers.InventoryItems;
using Psy.Core;

namespace Outbreak.Entities.Properties
{
    public static class GameEntityPropertyExtensions
    {

        public static EntityProperty GetProperty(this Entity entity, GameEntityPropertyEnum target)
        {
            var realVal = (short) target;
            return entity.GetProperty(realVal);
        }

        public static bool HasProperty(this Entity entity, GameEntityPropertyEnum target)
        {
            var realVal = (short) target;
            return entity.HasProperty(realVal);
        }

        public static void ClearProperty(this Entity entity, GameEntityPropertyEnum target)
        {
            var realVal = (short) target;
            entity.ClearProperty(realVal);
        }

        /****/

        public static float GetVisibleRating(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.VisibleRating))
                return 0;
            return entity.GetProperty(GameEntityPropertyEnum.VisibleRating).FloatValue;
        }

        public static void SetVisibleRating(this Entity entity, float value)
        {
            value = Math.Max(Math.Min(1, value), 0);
            entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.VisibleRating, value) {IsDirtyable = false});
        }

        public static float DropVisibleRating(this Entity entity, float value)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.VisibleRating))
                return 0;

            var prop = entity.GetProperty(GameEntityPropertyEnum.VisibleRating);
            prop.FloatValue = Math.Max(0, prop.FloatValue - value);
            return prop.FloatValue;
        }

        public static float IncreaseVisibleRating(this Entity entity, float value)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.VisibleRating))
            {
                entity.SetVisibleRating(value);
                return value;
            }

            var prop = entity.GetProperty(GameEntityPropertyEnum.VisibleRating);
            prop.FloatValue = Math.Min(1, prop.FloatValue + value);
            return prop.FloatValue;
        }

        
        /****/

        private static bool HasRegisteredWithInventory(this Entity entity)
        {
            var hasProp = entity.HasProperty(GameEntityPropertyEnum.EntityRegisteredWithInventory);
            if (!hasProp)
                return false;

            return entity.GetProperty(GameEntityPropertyEnum.EntityRegisteredWithInventory).BoolValue;
        }

        public static bool HasInventory(this Entity entity)
        {
            return entity.GetInventoryId() != StaticInventoryCache.Instance.UnknownId;
        }

        public static long GetInventoryId(this Entity entity)
        {
            if (entity.HasProperty(GameEntityPropertyEnum.InventoryId))
                return entity.GetProperty(GameEntityPropertyEnum.InventoryId).LongValue;
            return StaticInventoryCache.Instance.UnknownId;
        }

        public static void SetInventoryId(this Entity entity, long value)
        {
            var prop = new EntityProperty((short) GameEntityPropertyEnum.InventoryId, value);
            entity.SetProperty(prop);
        }

        public static Inventory GetInventory(this Entity entity)
        {
            var inventoryId = entity.GetInventoryId();
            var inventory = StaticInventoryCache.Instance.GetInventory(inventoryId);

            if (inventory != null && inventory.Id != inventoryId)
                entity.SetInventoryId(inventory.Id);

            if (inventory != null && !entity.HasRegisteredWithInventory())
                entity.SetInventory(inventory);

            return inventory;
        }

        public static void RemoveInventory(this Entity entity)
        {
            if (!entity.HasInventory())
                return;
            
            var inv = entity.GetInventory();
            entity.SetInventoryId(StaticInventoryCache.Instance.UnknownId);
            entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.EntityRegisteredWithInventory, false) { IsPersistant = false, IsDirtyable = false, IsDirty = false });

            StaticInventoryCache.Instance.RemoveInventory(inv.Id);
        }

        public static void SetInventory(this Entity entity, Inventory value)
        {
            Debug.Assert(!entity.HasRegisteredWithInventory(), "Entity already has inventory");
            entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.EntityRegisteredWithInventory, true) { IsPersistant = false, IsDirtyable = false, IsDirty = false });
            entity.SetInventoryId(value.Id);

            value.OnSlotChanged +=
                (inv, slotIndex) =>
                    {
                        entity.PerformBehaviour((short) GameEntityBehaviourEnum.OnInventoryChange, null);
                    };

            value.OnSlotChanged +=
                (inv, slotIndex) =>
                    {
                        if (slotIndex == (byte)InventorySpecialSlotEnum.PrimaryWeapon)
                            entity.PerformBehaviour((short) GameEntityBehaviourEnum.OnPrimaryWeaponChange, null);
                    };

            value.OnSlotChanged +=
                (inv, slotIndex) =>
                {
                    if (slotIndex == (byte)InventorySpecialSlotEnum.SecondaryWeapon)
                        entity.PerformBehaviour((short)GameEntityBehaviourEnum.OnSecondaryWeaponChange, null);
                };

            value.OnSlotChanged +=
                (inv, slotIndex) =>
                {
                    if (slotIndex == (byte)InventorySpecialSlotEnum.HeadArmour)
                        entity.PerformBehaviour((short)GameEntityBehaviourEnum.OnHeadSlotChange, null);
                };
        }

        /**/

        public static bool HasHungerTimer(this Entity entity)
        {
            return entity.HasProperty(GameEntityPropertyEnum.HungerTimer);
        }

        public static double GetHungerTimer(this Entity entity)
        {
            return !entity.HasProperty(GameEntityPropertyEnum.HungerTimer)
                       ? 0
                       : entity.GetProperty(GameEntityPropertyEnum.HungerTimer).DoubleValue;
        }

        public static void SetHungerTimer(this Entity entity, double value)
        {
            entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.HungerTimer, value));
        }

        /**/

        public static short GetHunger(this Entity entity)
        {
            return !entity.HasProperty(GameEntityPropertyEnum.Hunger)
                       ? (short) 0
                       : entity.GetProperty(GameEntityPropertyEnum.Hunger).ShortValue;
        }

        public static void SetHunger(this Entity entity, short value)
        {
            entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.Hunger, value));
        }

        /**/

        public static bool GetIsHuman(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.IsHuman))
                return false;
            return entity.GetProperty(GameEntityPropertyEnum.IsHuman).BoolValue;
        }

        public static void SetIsHuman(this Entity entity, bool value)
        {
            entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.IsHuman, value));
        }

        /**/

        public static bool GetIsZombie(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.IsZombie))
                return false;
            return entity.GetProperty(GameEntityPropertyEnum.IsZombie).BoolValue;
        }

        public static void SetIsZombie(this Entity entity, bool value)
        {
            entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.IsZombie, value));
        }

        /**/

        public static int GetFloatingItemId(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.FloatingItemId).IntValue;
        }

        public static void SetFloatingItemId(this Entity entity, int value)
        {
            entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.FloatingItemId, value));
        }

        public static bool HasFloatingItemId(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.FloatingItemId))
                return false;
            return entity.GetFloatingItemId() != StaticInventoryCache.Instance.UnknownId;
        }

        /**/

        public static int GetHealAmount(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.HealAmount).IntValue;
        }

        public static void SetHealAmount(this Entity entity, int value)
        {
            entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.HealAmount, value));
        }

        /**/

        public static bool GetDestroyOnPickedUp(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.DestroyOnCollected).BoolValue;
        }

        public static void SetDestroyOnPickedUp(this Entity entity, bool value)
        {
            entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.DestroyOnCollected, value));
        }

        /**/

        public static float GetExplosionRange(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.ExplosionRange).FloatValue;
        }

        public static void SetExplosionRange(this Entity entity, float range)
        {
            entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.ExplosionRange, range));
        }

        /**/

        public static float GetExplosionDamage(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.ExplosionDamage).FloatValue;
        }

        public static void SetExplosionDamage(this Entity entity, float value)
        {
            entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.ExplosionDamage, value));
        }

        /**/

        public static int? GetChaseTargetId(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.ChaseTargetId))
                return null;

            return entity.GetProperty(GameEntityPropertyEnum.ChaseTargetId).IntValue;
        }

        public static void ClearChaseTarget(this Entity entity)
        {
            entity.ClearProperty(GameEntityPropertyEnum.ChaseTargetId);
        }

        public static void SetChaseTargetId(this Entity entity, int target)
        {
            entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.ChaseTargetId, target));
        }

        /**/

        public static int GetChaseThinkCount(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.ChaseThinkCount).IntValue;
        }

        public static void IncrementChaseThinkCount(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.ChaseThinkCount))
                entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.ChaseThinkCount, (int) 1));
            else
            {
                var prop = entity.GetProperty(GameEntityPropertyEnum.ChaseThinkCount);
                prop.IntValue = prop.IntValue + 1;
            }
        }

        /**/

        public static double GetChaseStartTime(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.ChaseStartTime))
                return 0;
            return entity.GetProperty(GameEntityPropertyEnum.ChaseStartTime).DoubleValue;
        }

        public static void SetChaseStartTime(this Entity entity, double time)
        {
            entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.ChaseStartTime, time));
        }

        /**/

        public static long GetChaseMinTime(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.ChaseMinTime).LongValue;
        }

        public static void SetChaseMinTime(this Entity entity, long value)
        {
            entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.ChaseMinTime, value));
        }

        /**/

        public static float GetAccelerationRate(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.AccelerationRate).FloatValue;
        }

        public static void SetAccelerationRate(this Entity entity, float value)
        {
            entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.AccelerationRate, value));
        }

        /**/

        public static float GetMaxSpeed(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.MaxSpeed).FloatValue;
        }

        public static void SetMaxSpeed(this Entity entity, float value)
        {
            entity.SetProperty(new EntityProperty((short) GameEntityPropertyEnum.MaxSpeed, value));
        }

        /****/

        public static float GetWalkSpeed(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.WalkSpeed).FloatValue;
        }

        public static void SetWalkSpeed(this Entity entity, float value)
        {
            entity.GetProperty(GameEntityPropertyEnum.WalkSpeed).FloatValue = value;
        }

        /****/

        public static float GetRunSpeed(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.RunSpeed).FloatValue;
        }

        public static void SetRunSpeed(this Entity entity, float value)
        {
            entity.GetProperty(GameEntityPropertyEnum.RunSpeed).FloatValue = value;
        }

        public static void SetToWalkingSpeed(this Entity entity)
        {
            entity.SetMovementSpeed(entity.GetWalkSpeed());
        }

        public static void SetToRunningSpeed(this Entity entity)
        {
            entity.SetMovementSpeed(entity.GetRunSpeed());
        }

        public static void SetMovementSpeed(this Entity entity, float speed)
        {
            var prop = entity.GetProperty(EntityPropertyEnum.MovementVector);
            var movementVector = prop.VectorValue.NormalizeRet()*speed;

            prop.VectorValue = movementVector;
        }

        /****/

        public static void SetIsRunning(this Entity entity, bool value)
        {
            entity.GetProperty(GameEntityPropertyEnum.IsRunning).BoolValue = value;
        }


        public static bool GetIsRunning(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.IsRunning).BoolValue;
        }

        /****/

        public static bool GetIsGod(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.IsGod))
                return false;

            return entity.GetProperty(GameEntityPropertyEnum.IsGod).BoolValue;
        }

        public static void SetIsGod(this Entity entity, bool value)
        {
            entity.GetProperty(GameEntityPropertyEnum.IsGod).BoolValue = value;
        }

        /****/
        public static Direction GetKeyboardDirection(this Entity entity)
        {
            return (Direction)entity.GetProperty(GameEntityPropertyEnum.KeyboardMovementDir).ByteValue;
        }

        public static void SetKeyboardDirection(this Entity entity, Direction dir)
        {
            entity.GetProperty(GameEntityPropertyEnum.KeyboardMovementDir).ByteValue = (byte) dir;

            switch (dir)
            {
                case Direction.None:
                    entity.SetMovementVector(new Vector3(0, 0, 0));
                    break;
                default:
                    var directionVector = DirectionUtil.CalculateVector(DirectionUtil.GetRotationFromDirection(dir));
                    var speed = entity.GetIsRunning() ? entity.GetRunSpeed() : entity.GetWalkSpeed();

                    entity.SetMovementVector(directionVector * speed);
                    break;
            }
        }

        /****/
        public static bool GetReloadingPrimary(this Entity entity)
        {
            var weapon = entity.GetInventory()[InventorySpecialSlotEnum.PrimaryWeapon];
            if (weapon == null)
                return false;
            return weapon.GetIsReloading();
        }

        public static void SetReloadingPrimary(this Entity entity, bool value)
        {
            var weapon = entity.GetInventory()[InventorySpecialSlotEnum.PrimaryWeapon];
            if (weapon == null)
                return;
            weapon.SetIsReloading(value);
        }

        /****/
        public static bool GetReloadingSecondary(this Entity entity)
        {
            var weapon = entity.GetInventory()[InventorySpecialSlotEnum.SecondaryWeapon];
            if (weapon == null)
                return false;
            return weapon.GetIsReloading();
        }

        public static void SetReloadingSecondary(this Entity entity, bool value)
        {
            var weapon = entity.GetInventory()[InventorySpecialSlotEnum.SecondaryWeapon];
            if (weapon == null)
                return;
            weapon.SetIsReloading(value);
        }

        /***/
        public static void SetReloadingStatus(this Entity entity, InventorySpecialSlotEnum weapon, bool value)
        {
            switch (weapon)
            {
                case InventorySpecialSlotEnum.PrimaryWeapon:
                    entity.SetReloadingPrimary(value);
                    break;
                case InventorySpecialSlotEnum.SecondaryWeapon:
                    entity.SetReloadingSecondary(value);
                    break;
            }
        }

        /****/
        public static long GetNextPrimaryReloadTime(this Entity entity)
        {
            var weapon = entity.GetInventory()[InventorySpecialSlotEnum.PrimaryWeapon];
            if (weapon == null)
                return 0;
            return weapon.GetNextReloadTime();
        }

        public static void SetNextPrimaryReloadTime(this Entity entity, long time)
        {
            var weapon = entity.GetInventory()[InventorySpecialSlotEnum.PrimaryWeapon];
            if (weapon == null)
                return;
            weapon.SetNextReloadTime(time);
        }

        public static long GetNextSecondaryReloadTime(this Entity entity)
        {
            var weapon = entity.GetInventory()[InventorySpecialSlotEnum.SecondaryWeapon];
            if (weapon == null)
                return 0;
            return weapon.GetNextReloadTime();
        }

        public static void SetNextSecondaryReloadTime(this Entity entity, long time)
        {
            var weapon = entity.GetInventory()[InventorySpecialSlotEnum.SecondaryWeapon];
            if (weapon == null)
                return;
            weapon.SetNextReloadTime(time);
        }

        public static byte GetEquippedItemId(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.EquippedItem).ByteValue;
        }

        public static void SetEquippedItemId(this Entity entity, byte slotId)
        {
            entity.GetProperty(GameEntityPropertyEnum.EquippedItem).ByteValue = slotId;
        }

        public static InventoryItem GetEquippedItem(this Entity entity)
        {
            var inventory = entity.GetInventory();
            if (inventory == null)
                return null;
            return inventory[entity.GetEquippedItemId()];
        }

        /****/
        public static float GetDoorOpenAngle(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.DoorOpenAngle).FloatValue;
        }

        public static void SetDoorOpenAngle(this Entity entity, float value)
        {
            entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.DoorOpenAngle, value));
        }

        /****/
        public static float GetDoorClosedAngle(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.DoorClosedAngle).FloatValue;
        }

        public static void SetDoorClosedAngle(this Entity entity, float value)
        {
            entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.DoorClosedAngle, value));
        }

        /****/
        public static bool GetDoorIsOpen(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.DoorCurrentlyOpen).BoolValue;
        }

        public static void SetDoorIsOpen(this Entity entity, bool value)
        {
            entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.DoorCurrentlyOpen, value));
        }

        /****/
        public static int GetLevel(this Entity entity)
        {
            return entity.GetProperty(GameEntityPropertyEnum.Level).IntValue;
        }

        /****/
        public static long GetMinNoiseChaseTime(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.ChaseNoiseMinTime))
            {
                var minChaseTime = entity.GetChaseMinTime();
                entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.ChaseNoiseMinTime, minChaseTime));
            }

            var prop = entity.GetProperty(GameEntityPropertyEnum.ChaseNoiseMinTime);
            return prop.LongValue;
        }

        public static double GetStartNoiseChaseTime(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.ChaseNoiseStartTime))
                return -1;
            var prop = entity.GetProperty(GameEntityPropertyEnum.ChaseNoiseStartTime);
            return prop.DoubleValue;
        }

        public static void SetStartNoiseChaseTime(this Entity entity, double value)
        {
            entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.ChaseNoiseStartTime, value));
        }

        /****/
        public static byte GetWeaponUseCount(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.WeaponUseCount))
                return 0;
            return entity.GetProperty(GameEntityPropertyEnum.WeaponUseCount).ByteValue;
        }

        public static void IncreaseWeaponUseCount(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.WeaponUseCount))
                entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.WeaponUseCount, 1));
            else
                entity.GetProperty(GameEntityPropertyEnum.WeaponUseCount).ByteValue += 1;
        }

        public static void DecreaseWeaponUseCount(this Entity entity)
        {
            if (entity.HasProperty(GameEntityPropertyEnum.WeaponUseCount))
                entity.GetProperty(GameEntityPropertyEnum.WeaponUseCount).ByteValue -= 1;
        }

        /****/
        public static void SetValidInventoryItems(this Entity entity, List<ItemTypeEnum> items)
        {
            entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.ValidInventoryItems, items){IsDirtyable = false});
        }

        public static List<ItemTypeEnum> GetValidInventoryItems(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.ValidInventoryItems))
                return new List<ItemTypeEnum>();

            var prop = (List<ItemTypeEnum>)entity.GetProperty(GameEntityPropertyEnum.ValidInventoryItems).ObjectValue;
            return prop;
        }

        /****/
        public static void SetMinInventoryItemCount(this Entity entity, short count)
        {
            entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.InventoryItemMin, count) { IsDirtyable = false });
        }

        public static short GetMinInventoryItemCount(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.InventoryItemMax))
                return 0;

            return entity.GetProperty(GameEntityPropertyEnum.InventoryItemMin).ShortValue;
        }

        /****/
        public static void SetMaxInventoryItemCount(this Entity entity, short count)
        {
            entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.InventoryItemMax, count) { IsDirtyable = false });
        }

        public static short GetMaxInventoryItemCount(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.InventoryItemMax))
                return 0;

            return entity.GetProperty(GameEntityPropertyEnum.InventoryItemMax).ShortValue;
        }

        /****/

        public static void SetPrimaryWeaponItem(this Entity entity, short count)
        {
            entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.PrimaryWeaponItem, count));
        }

        public static short GetPrimaryWeaponItem(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.PrimaryWeaponItem))
                return -1;

            return entity.GetProperty((int) GameEntityPropertyEnum.PrimaryWeaponItem).ShortValue;
        }

        /****/

        public static void SetSecondaryWeaponItem(this Entity entity, short count)
        {
            entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.SecondaryWeaponItem, count));
        }

        public static short GetSecondaryWeaponItem(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.SecondaryWeaponItem))
                return -1;

            return entity.GetProperty((int)GameEntityPropertyEnum.SecondaryWeaponItem).ShortValue;
        }

        /****/

        public static void SetHeadSlotItem(this Entity entity, short count)
        {
            entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.HeadSlotItem, count));
        }

        public static short GetHeadSlotItem(this Entity entity)
        {
            if (!entity.HasProperty(GameEntityPropertyEnum.HeadSlotItem))
                return -1;

            return entity.GetProperty((int)GameEntityPropertyEnum.HeadSlotItem).ShortValue;
        }



        // all of the equipment has a defence value between -1 and 1.
        // higher numbers is a better defence.
        // this number however is multiplied to the damage to provide a final damage value
        // so what we do is we get subtract from the sum of all the equipable slots,
        // the sum of all the defence of equiped items / sum of equipable slots
        // eg if there are 5 slots that can hold an item, with only 2 equipped
        // (5 - (0.1 + 0.3 + 0 + 0 + 0)) / 5
        // this will then give us a number between 0 and 1 where 1 will take the max damage, and 0 will take the least
        //
        // this function only has meaning for entities that have armour in their inventory
        // if the entity takes MORE damage than usual, then this value should be negative.
        public static float GetInventoryDefenceValue(this Entity entity, DamageTypeEnum dmgType)
        {
            var inventory = entity.GetInventory();

            // no inventory = no armour = no defence
            if (inventory == null)
                return 0;

            var armourTypes = new List<InventorySpecialSlotEnum>
                                  {
                                      InventorySpecialSlotEnum.BodyArmour,
                                      InventorySpecialSlotEnum.LegArmour,
                                      InventorySpecialSlotEnum.HeadArmour,
                                      InventorySpecialSlotEnum.FootArmour
                                  };

            var items = armourTypes.Select(armour => inventory[armour]).Where(item => item != null);
            var defence = items.Sum(item => item.GetDefenceMultiplier(dmgType));
            return defence/armourTypes.Count;
        }
    }
}

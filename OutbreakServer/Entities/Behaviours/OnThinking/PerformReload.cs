using System;
using System.Linq;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.Containers;
using Outbreak.Items.Containers.InventoryItems;
using Psy.Core;

namespace Outbreak.Server.Entities.Behaviours.OnThinking
{
    public class PerformReload : IEntityBehaviour
    {
        public void PerformBehaviour(Entity target, Entity instigator)
        {
            var now = Timer.GetTime();
            var weapon = target.GetEquippedItem();

            if (weapon == null)
                return;

            if (!weapon.GetIsReloading())
                return;

            var nextRun = weapon.GetNextReloadTime();
            if (now <= nextRun) 
                return;

            if (weapon.GetFinalReloading())
            {
                weapon.SetIsReloading(false);
                weapon.SetFinalReloading(false);
                return;
            }

            var nextReloadTime = DoReload(target, weapon);
            weapon.SetIsReloading(nextReloadTime > 0);
            weapon.SetNextReloadTime(nextReloadTime);
        }

        /** reloads the given weapon,
         * returns the time for the next reload
         * 0 = stop reloading
         */
        private static long DoReload(Entity target, InventoryItem weapon)
        {
            var inventory = target.GetInventory();
            var ammoType = weapon.GetItemSpec().GetAmmoType();

            if (!ammoType.HasValue)
                return 0;

            // if there's something already loaded, then we can only load that type of ammo
            var equippedAmmo = weapon.GetLoadedAmmoType();
            var available = weapon.GetLoadedAmmoCount();

            if (equippedAmmo != null && available > 0)
                return DoReload(target, weapon, equippedAmmo.Id);

            // get the first collection of ammo that could fit this gun
            var targetAmmoType = inventory.GetContent().
                FirstOrDefault(item =>
                               item.Key >= (byte) InventorySpecialSlotEnum.ContainerStart &&
                               item.Value != null &&
                               item.Value.GetItemSpec().IsAmmo() &&
                               item.Value.GetItemSpec().GetAmmoType() == ammoType);

            // no matching ammo - abandon reloading
            if (targetAmmoType.Value == null)
                return 0;

            return DoReload(target, weapon, targetAmmoType.Value.ItemSpecId);
        }

        /** Ram some ammo into the gun
         * assumes that the ammo is valid for the weapon
         */
        private static short PushAmmoToWeapon(InventoryItem weapon, InventoryItem ammo, short amoutToTransfer)
        {
            var availableAmount = ammo.GetCount();
            weapon.SetLoadedAmmoType(ammo.GetItemSpec());
            if (availableAmount < amoutToTransfer)
            {
                ammo.SetCount(0);
                weapon.EditLoadedAmmoCount(availableAmount);
                return availableAmount;
            }

            ammo.SetCount((short)(availableAmount - amoutToTransfer));
            weapon.EditLoadedAmmoCount(amoutToTransfer);
            return amoutToTransfer;
        }

        /** Set the next time for a reload process
         */
        private static long UpdateReloadTime(InventoryItem weapon)
        {
            var requiredExtraTime = weapon.GetItemSpec().GetReloadTime();
            return (long)Math.Round(Timer.GetTime() + requiredExtraTime);
        }

        private static long UpdateFinalReloadTime(InventoryItem weapon)
        {
            weapon.SetFinalReloading(true);

            var requiredExtraTime = weapon.GetItemSpec().GetStopReloadTime();
            if (requiredExtraTime == 0)
                return 0;

            return (long)Math.Round(Timer.GetTime() + requiredExtraTime);
        }

        /** reloads the given weapon with the given ammo type
         *  updates the time for the next reload
         *  it is not assumed that the ammo is available.
         *  it is assumed that the weapon is available.
         *  it is assumed that the ammo type is valid for the weapon
         */
        private static long DoReload(Entity target, InventoryItem weapon, int ammoItemSpecId)
        {
            var inventory = target.GetInventory();
            var ammo = inventory.GetContent().
                Where(item =>
                      item.Key >= (byte) InventorySpecialSlotEnum.ContainerStart &&
                      item.Value != null &&
                      item.Value.GetItemSpec().IsAmmo() &&
                      item.Value.ItemSpecId == ammoItemSpecId).Reverse();

            var targetAmmoAmount = weapon.GetItemSpec().GetClipSize() - weapon.GetLoadedAmmoCount();
            var maxTransferable = weapon.GetItemSpec().GetReloadClipSize() == -1
                                      ? targetAmmoAmount
                                      : Math.Min(targetAmmoAmount, weapon.GetItemSpec().GetReloadClipSize());
            var amountToTransfer = (short)Math.Min(targetAmmoAmount, maxTransferable);

            if (amountToTransfer == 0)
                return 0;

            foreach (var ammoItem in ammo)
            {
                var amountTransfered = PushAmmoToWeapon(weapon, ammoItem.Value, amountToTransfer);
                amountToTransfer -= amountTransfered;
            }

            var continueReload = false;
            if (amountToTransfer == 0)
            {
                continueReload = maxTransferable != targetAmmoAmount;
            }

            return continueReload ? 
                UpdateReloadTime(weapon) : 
                UpdateFinalReloadTime(weapon);
        }
    }
}

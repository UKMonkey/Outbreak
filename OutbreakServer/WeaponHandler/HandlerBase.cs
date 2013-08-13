using System.Collections.Generic;
using Outbreak.Entities.Behaviours;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers.InventorySpecs;
using Psy.Core;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs.Types;

namespace Outbreak.Server.WeaponHandler
{
    public abstract class HandlerBase : IWeaponUseHandler
    {
        private readonly HashSet<WeaponTypes> _types;
        private readonly HashSet<KeyValuePair<int, InventoryItem>> _knownShooters;

        // return true if the weapon can continue to fire
        protected abstract bool PerformUseImpl(Entity owner, InventoryItem weapon);
        protected abstract void StopPerformUse(Entity owner, InventoryItem weapon);

        protected readonly GameServer GameServer;

        protected HandlerBase(IEnumerable<WeaponTypes> types, GameServer gameServer)
        {
            _types = new HashSet<WeaponTypes>(types);
            _knownShooters = new HashSet<KeyValuePair<int, InventoryItem>>();
            GameServer = gameServer;
        }

        public HashSet<WeaponTypes> GetApplicableWeapons()
        {
            return _types;
        }

        private void TriggerNoise(Entity owner, InventoryItem weapon)
        {
            var distance = weapon.GetWeaponNoiseDistance();
            var listeners = GameServer.Engine.GetEntitiesWithinArea(owner.GetPosition(), distance);
            foreach (var listener in listeners)
            {
                listener.PerformBehaviour((short)GameEntityBehaviourEnum.OnHearWeaponNoise, owner);
            }
        }

        protected bool PerformUse(int entityId, InventoryItem weapon)
        {
            var owner = GameServer.Engine.GetEntity(entityId);
            return PerformUse(owner, weapon);
        }

        protected bool PerformUse(Entity owner, InventoryItem weapon)
        {
            if (owner == null)
                return false;
            TriggerNoise(owner, weapon);
            weapon.SetLastUsedTime(GameServer.Engine);
            return PerformUseImpl(owner, weapon);
        }

        public void StartUseWeapon(Entity owner, InventoryItem weapon)
        {
            var data = new KeyValuePair<int, InventoryItem>(owner.EntityId, weapon);
            if (_knownShooters.Contains(data))
                return;

            if (CanFire(weapon) && PerformUse(owner, weapon))
            {
                owner.IncreaseWeaponUseCount();
                _knownShooters.Add(data);
            }
        }
        
        public void StopUseWeapon(Entity owner, InventoryItem weapon)
        {
            StopUseWeapon(owner.EntityId, weapon);
        }

        public void StopUseWeapon(int id, InventoryItem weapon)
        {
            if (!_knownShooters.Contains(new KeyValuePair<int, InventoryItem>(id, weapon)))
                return;
            
            _knownShooters.Remove(new KeyValuePair<int, InventoryItem>(id, weapon));
            var owner = GameServer.Engine.GetEntity(id);
            if (owner == null)
                return;

            owner.DecreaseWeaponUseCount();
            StopPerformUse(owner, weapon);
        }

        protected bool CanFire(InventoryItem weapon)
        {
            if (!weapon.GetItemSpec().RequiresAmmo())
                return true;

            var count = weapon.GetLoadedAmmoCount();
            if (count == 0)
                return false;

            if (weapon.GetIsReloading())
                return false;

            return true;
        }

        protected float GetDamage(float minDamage, float maxDamage)
        {
            if (minDamage > maxDamage)
                return maxDamage;
            return (float)StaticRng.Random.NextDouble(minDamage, maxDamage);
        }

        protected bool CanUpdate(int entityId, InventoryItem weapon)
        {
            var lastUsage = weapon.GetLastUsedTime(GameServer.Engine);
            var now = GameServer.Engine.CurrentFrameNumber;
            var delay = weapon.GetItemSpec().GetWeaponDelay();

            if (delay < now - lastUsage)
                return true;
            return false;
        }

        public void Update()
        {
            var toRemove = new List<KeyValuePair<int, InventoryItem>>();

            foreach (var pair in _knownShooters)
            {
                var ownerId = pair.Key;
                var weapon = pair.Value;

                var remove = !CanFire(weapon);
                var canUpdate = CanUpdate(ownerId, weapon);

                if (canUpdate)
                    remove = !remove && !PerformUse(ownerId, weapon);

                if (remove)
                    toRemove.Add(pair);
            }

            foreach (var pair in toRemove)
            {
                var weapon = pair.Value;
                StopUseWeapon(pair.Key, weapon);
            }
        }

        protected void ApplyDamage(Entity owner, Entity hit, float damageAmount, DamageTypeEnum dmgType)
        {
            if (damageAmount > 0)
            {
                hit.TakeDamage(damageAmount, dmgType, owner);
                hit.PerformBehaviour((int)GameEntityBehaviourEnum.OnTakeDamage, owner);
            }
        }
    }
}

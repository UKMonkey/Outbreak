using System;
using System.Collections.Generic;
using System.Linq;
using Vortex.Interface.EntityBase;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.Containers.InventoryItems;

namespace Outbreak.Server.WeaponHandler
{
    public class WeaponHandlerFactory
    {
        private readonly List<IWeaponUseHandler> _handlers;


        public WeaponHandlerFactory()
        {
            _handlers = new List<IWeaponUseHandler>();
        }

        public void AddHandler(IWeaponUseHandler handler)
        {
            _handlers.Add(handler);
        }

        public bool StartWeaponShoot(Entity owner, InventoryItem weapon)
        {
            if (weapon == null)
                return false;

            var weaponType = weapon.GetItemSpec().GetWeaponType();
            if (weaponType == null)
                return false;

            var handlers = _handlers.Where(handler => handler.GetApplicableWeapons().Contains(weaponType.Value));
            var run = false;
            foreach (var handler in handlers)
            {
                if (run)
                    throw new Exception("Multiple handlers for a single weapon type!");

                handler.StartUseWeapon(owner, weapon);
                run = true;
            }

            if (!run)
                throw new Exception(string.Format("No weapon handler for weapon type {0}", weaponType));

            return false;
        }

        public void StopWeaponShoot(Entity owner, InventoryItem weapon)
        {
            var weaponType = weapon.GetItemSpec().GetWeaponType();
            if (weaponType == null)
                return;

            var handlers = _handlers.Where(handler => handler.GetApplicableWeapons().Contains(weaponType.Value));
            foreach (var handler in handlers)
            {
                handler.StopUseWeapon(owner, weapon);
            }
        }

        public void UpdateHandlers()
        {
            foreach (var weaponUseHandler in _handlers)
            {
                weaponUseHandler.Update();
            }
        }
    }
}

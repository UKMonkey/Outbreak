using System.Collections.Generic;
using Outbreak.Net.Messages.FireWeapon;
using Psy.Core;
using Vortex.Interface.EntityBase;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs.Types;

namespace Outbreak.Server.WeaponHandler.Ranged
{
    public class AutomaticHandler : BulletShooter
    {
        public AutomaticHandler(GameServer gameServer) :
            base(new List<WeaponTypes>{WeaponTypes.Uzi}, gameServer)
        {
        }

        protected override bool PerformUseImpl(Entity owner, InventoryItem weapon)
        {
            var bulletData = Fire(owner, weapon, owner.GetRotation());

            var count = weapon.GetLoadedAmmoCount();
            weapon.SetLoadedAmmoCount((short)(count - 1));

            var msg = new ServerFirePistolMessage();
            msg.StartPoint = GetBulletSource(owner);
            msg.BulletEffects.Add(bulletData);
            msg.EntityUser = owner.EntityId;

            GameServer.Engine.SendMessage(msg);

            return weapon.GetLoadedAmmoCount() != 0;
        }

        protected override void StopPerformUse(Entity owner, InventoryItem weapon)
        {
        }
    }
}

using System.Collections.Generic;
using Outbreak.Net.Messages.FireWeapon;
using Vortex.Interface.EntityBase;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs.Types;

namespace Outbreak.Server.WeaponHandler.Ranged
{
    public class SemiAutomaticHandler : BulletShooter
    {
        public SemiAutomaticHandler(GameServer gameServer) :
            base(new List<WeaponTypes>{WeaponTypes.Pistol}, gameServer)
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

            return false;
        }

        protected override void StopPerformUse(Entity owner, InventoryItem weapon)
        {
        }
    }
}

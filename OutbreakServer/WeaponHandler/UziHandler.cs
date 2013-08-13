using System.Collections.Generic;
using Outbreak.Net.Messages.FireWeapon;
using Psy.Core;
using Vortex.Interface.EntityBase;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs.Types;


namespace Outbreak.Server.WeaponHandler
{
    public class UziHandler : BulletShooter
    {
        public UziHandler(GameServer gameServer) :
            base(new List<WeaponTypes>{WeaponTypes.Uzi}, gameServer)
        {
        }

        protected override bool PerformUseImpl(Entity owner, InventoryItem weapon)
        {
            var bulletData = Fire(owner, weapon, owner.GetRotation());

            var count = weapon.GetLoadedAmmoCount();
            weapon.SetLoadedAmmoCount((short)(count - 1));

            var msg = new ServerFirePistolMessage();
            msg.StartPoint = owner.GetPosition() + DirectionUtil.CalculateVector(owner.GetRotation()) * 0.5f;
            msg.BulletEffects.Add(bulletData);

            GameServer.Engine.SendMessage(msg);

            return weapon.GetLoadedAmmoCount() != 0;
        }

        protected override bool UpdateFiringWeapon(int entityId, InventoryItem weapon)
        {
            return PerformUse(entityId, weapon);
        }

        protected override void StopPerformUse(Entity owner, InventoryItem weapon)
        {
        }
    }
}

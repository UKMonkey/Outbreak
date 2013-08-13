using System.Collections.Generic;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Net.Messages.FireWeapon;
using Vortex.Interface.EntityBase;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs.Types;
using Psy.Core;

namespace Outbreak.Server.WeaponHandler
{
    public class ShotgunHandler : BulletShooter
    {
        public ShotgunHandler(GameServer gameServer)
            : base(new List<WeaponTypes> {WeaponTypes.Shotgun}, gameServer)
        {
        }

        protected override bool PerformUseImpl(Entity owner, InventoryItem weapon)
        {
            var bulletCount = StaticRng.Random.Next(5, 11);
            var msg = new ServerFireShotgunMessage();

            msg.StartPoint = owner.GetPosition() + DirectionUtil.CalculateVector(owner.GetRotation())*0.5f;
            var count = weapon.GetLoadedAmmoCount();
            weapon.SetLoadedAmmoCount((short)(count - 1));
            var spread = weapon.GetItemSpec().GetBulletSpread();

            for (var i = 0; i < bulletCount; ++i)
            {
                var rotation = (float)(owner.GetRotation() + (StaticRng.Random.NextDouble() * spread) - (spread / 2));
                msg.BulletEffects.Add(Fire(owner, weapon, rotation));
            }

            GameServer.Engine.SendMessage(msg);

            return false;
        }

        protected override bool UpdateFiringWeapon(int entityId, InventoryItem weapon)
        {
            return false;
        }

        protected override void StopPerformUse(Entity owner, InventoryItem weapon)
        {
        }
    }
}

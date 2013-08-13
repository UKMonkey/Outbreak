using System;
using System.Collections.Generic;
using System.Linq;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.Containers.InventorySpecs.Types;
using Outbreak.Net.Messages.FireWeapon;
using Psy.Core;
using Vortex.Interface.EntityBase;

namespace Outbreak.Server.WeaponHandler.Melee
{
    class FrontArchRepeated : HandlerBase
    {
        public FrontArchRepeated(GameServer gameServer) :
            base(new List<WeaponTypes>
                     {
                         WeaponTypes.CricketBat
                     }, 
            gameServer)
        {
        }

        protected bool IsInArchRange(Entity owner, Entity target)
        {
            const float archRange = (float)(Math.PI/2);

            var ownerToitem = owner.GetPosition() - target.GetPosition();
            var angle = ownerToitem.ZPlaneAngle() - owner.GetRotation();

            while (angle < 0)
                angle += (float)Math.PI * 2;
            while (angle > (Math.PI * 2))
                angle -= (float)Math.PI * 2;

            angle -= (float)Math.PI;
            if (Math.Abs(angle) > archRange)
                return false;
            return true;
        }

        protected override bool PerformUseImpl(Entity owner, InventoryItem weapon)
        {
            var range = weapon.GetItemSpec().GetMeleeRange();
            var msg = new ServerFireSwingWeapon();
            msg.SwingDirection = owner.GetRotation() + (float)Math.PI;
            msg.StartPoint = owner.GetPosition();
            msg.EntityUser = owner.EntityId;

            var potentialTargets = GameServer.Engine.GetEntitiesWithinArea(owner.GetPosition(), range).Where(item => item.EntityId != owner.EntityId);
            var targets = potentialTargets.Where(target =>
                    IsInArchRange(owner, target)
                );

            foreach (var target in targets)
            {
                var weaponDamageMin = weapon.GetItemSpec().GetDamageMin();
                var weaponDamageMax = weapon.GetItemSpec().GetDamageMax();
                var weaponDamage = GetDamage(weaponDamageMin, weaponDamageMax);

                var damageType = weapon.GetItemSpec().GetDamageType();

                if (target.GetIsHuman())
                    msg.HitHumans.Add(target.EntityId);
                else if (target.GetIsZombie())
                    msg.HitZombies.Add(target.EntityId);
                else
                    msg.HitScenery.Add(target.EntityId);

                ApplyDamage(owner, target, weaponDamage, damageType);
            }

            GameServer.Engine.SendMessage(msg);

            return true;
        }

        protected override void StopPerformUse(Entity owner, InventoryItem weapon)
        {
        }
    }
}

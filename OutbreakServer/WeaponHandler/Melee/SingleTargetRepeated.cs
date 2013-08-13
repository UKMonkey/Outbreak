using System;
using System.Collections.Generic;
using System.Linq;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.Containers.InventorySpecs.Types;
using Psy.Core;
using Vortex.Interface.EntityBase;

namespace Outbreak.Server.WeaponHandler.Melee
{
    class SingleTargetRepeated : HandlerBase
    {
        public SingleTargetRepeated(GameServer gameServer) :
            base(new List<WeaponTypes>
                     {
                         WeaponTypes.ZombieMaul,
                     }, 
            gameServer)
        {
        }

        protected override bool PerformUseImpl(Entity owner, InventoryItem weapon)
        {
            // if the owner has a specific chase target, then use that target
            var targetId = owner.GetChaseTargetId();
            var range = weapon.GetItemSpec().GetMeleeRange();
            Entity target = null;

            // if not, then pick a target that is infront of the owner
            if (targetId == null)
            {
                var potentialTargets = GameServer.Engine.GetEntitiesWithinArea(owner.GetPosition(), range);
                var targetAngle = 1000f;
                foreach (var item in potentialTargets.Where(item => item.EntityId != owner.EntityId))
                {
                    var ownerToitem = owner.GetPosition() - item.GetPosition();
                    var angle = ownerToitem.ZPlaneAngle() - owner.GetRotation();

                    while (angle < 0)
                        angle += (float) Math.PI*2;
                    while (angle > (Math.PI * 2))
                        angle -= (float) Math.PI*2;

                    angle -= (float)Math.PI;
                    if (Math.Abs(angle) > Math.PI / 2 || targetAngle < angle)
                        continue;

                    target = item;
                    targetAngle = angle;
                }
            }
            else
            {
                target = GameServer.Engine.GetEntity(targetId.Value);
                if (target == null)
                    return true;

                var ownerToTarget = owner.GetPosition() - target.GetPosition();

                // we're too far away from the target - but it's ok, we can keep trying to attack
                if (ownerToTarget.Length - target.Radius - owner.Radius - range > 0)
                    return true;
            }

            // no target, so no damage to do, but we can keep trying to attack if we want...
            if (target == null)
                return true;

            var weaponDamageMin = weapon.GetItemSpec().GetDamageMin();
            var weaponDamageMax = weapon.GetItemSpec().GetDamageMax();
            var weaponDamage = GetDamage(weaponDamageMin, weaponDamageMax);

            var damageType = weapon.GetItemSpec().GetDamageType();
            ApplyDamage(owner, target, weaponDamage, damageType);

            return true;
        }

        protected override void StopPerformUse(Entity owner, InventoryItem weapon)
        {
        }
    }
}

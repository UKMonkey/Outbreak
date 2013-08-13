using System.Collections.Generic;
using Outbreak.Effects;
using Outbreak.Entities.Behaviours;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.Containers.InventorySpecs.Types;
using Outbreak.Net.Messages.FireWeapon;
using Psy.Core;
using Vortex.Interface.EntityBase;

namespace Outbreak.Server.WeaponHandler
{
    abstract public class BulletShooter : HandlerBase
    {
        protected BulletShooter(IEnumerable<WeaponTypes> types, GameServer gameServer) 
            : base(types, gameServer)
        {
        }

        protected void ApplyDamage(Entity owner, Entity hit, InventoryItem weapon)
        {
            var ammoSpec = weapon.GetLoadedAmmoType();
            var min = ammoSpec.GetDamageMin();
            var max = ammoSpec.GetDamageMax();

            if (min > max)
                min = max;

            var dmg = (StaticRng.Random.NextDouble()*(max - min)) + min;
            hit.TakeDamage((float)dmg, ammoSpec.GetDamageType(), owner);

            if (dmg > 0)
            {
                hit.PerformBehaviour((int)GameEntityBehaviourEnum.OnTakeDamage, owner);
            }
        }

        protected BulletEffect GetEffect(Entity hit)
        {
            if (hit == null)
                return BulletEffect.None;
            if (hit.GetIsZombie() || hit.GetIsHuman())
                return BulletEffect.Bloodsplatter;
            return BulletEffect.None;
        }

        protected BulletEffectData Fire(Entity owner, InventoryItem weapon, float rotation)
        {
            var collisionResult = GameServer.Engine.TraceRay(owner.GetPosition().Translate(0, 0, -0.5f) , rotation, x => true);
            var effect = BulletEffect.None;

            if (collisionResult.HasCollided && collisionResult.CollisionMesh != null)
            {
                var hit = GameServer.Engine.GetEntity(collisionResult.CollisionMesh.Id);
                ApplyDamage(owner, hit, weapon);
                effect = GetEffect(hit);
            }

            return new BulletEffectData
            {
                EndPoint = collisionResult.CollisionPoint,
                Rotation = rotation,
                Effect = (byte)effect
            };
        }
    }
}

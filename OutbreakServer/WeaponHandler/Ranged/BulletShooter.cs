using System.Collections.Generic;
using System.Linq;
using Outbreak.Effects;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Items.Containers.InventorySpecs.Types;
using Outbreak.Net.Messages.FireWeapon;
using SlimMath;
using Vortex.Interface.EntityBase;
using Psy.Core;

namespace Outbreak.Server.WeaponHandler.Ranged
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

            var dmg = GetDamage(min, max);
            ApplyDamage(owner, hit, dmg, ammoSpec.GetDamageType());
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
            var collisionResult = GameServer.Engine.TraceRay(owner.GetPosition(), rotation, x => true);
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

        private Vector3 DefaultShootSource(Entity owner)
        {
            return owner.GetPosition() + DirectionUtil.CalculateVector(owner.GetRotation()) * 0.5f;
        }

        protected Vector3 GetBulletSource(Entity owner)
        {
            if (!owner.Model.ModelInstance.HasAnchor("lefthand"))
            {
                return DefaultShootSource(owner);
            }

            var weapon = owner.Model.ModelInstance.GetAttachedSubModels("lefthand").FirstOrDefault();
            if (weapon == null)
            {
                return DefaultShootSource(owner);
            }

            if (!weapon.HasAnchor("muzzle"))
            {
                return DefaultShootSource(owner);
            }

            var anchorPosition = owner.Model.ModelInstance.GetAnchorPosition("lefthand");
            var muzzlePosition = weapon.GetAnchorPosition("muzzle");

            var absPosition = anchorPosition + muzzlePosition;

            var rotMatrix = Matrix.RotationZ(owner.GetRotation());

            var bulletSource = owner.GetPosition() + Vector3.Transform(absPosition, rotMatrix).AsVector3();

            return bulletSource;
        }
    }
}

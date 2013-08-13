using System;
using System.Linq;
using Psy.Core;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Outbreak.Entities.Properties;

namespace Outbreak.Server.Entities.Behaviours.OnCollision
{
    public class Explode : IEntityBehaviour
    {
        private readonly IEngine _engine;

        public Explode(IEngine engine)
        {
            _engine = engine;
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            var maxRange = target.GetExplosionRange();
            var maxDamage = target.GetExplosionDamage();
            var nearbyEntities = _engine.GetEntitiesWithinArea(target.GetPosition(), maxRange);

            foreach (var other in nearbyEntities.Where(item => item.EntityId != target.EntityId))
            {
                var distance = other.GetPosition().Distance(target.GetPosition());
                var dmg = maxRange - maxDamage*Math.Log(distance, maxRange);
                other.TakeDamage((float)dmg, DamageTypeEnum.HighExplosion, target);
            }

            target.Destroy();
        }
    }
}

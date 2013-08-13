using Outbreak.Entities.Properties;
using Psy.Core;
using SlimMath;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Vortex.Interface.EntityBase.Properties;


namespace Outbreak.Server.Entities.Behaviours.OnHearWeapon
{
    public class TurnToFace : IEntityBehaviour
    {
        protected bool ShouldReact(Entity target)
        {
            if (target.GetChaseTargetId() != null)
                return false;

            var startChase = target.GetStartNoiseChaseTime();
            if (startChase < 0)
                return true;

            var minChase = target.GetMinNoiseChaseTime();
            var now = Timer.GetTime();
            var chaseTime = now - startChase;

            if (chaseTime > minChase)
                return true;

            return false;
        }

        protected void React(Entity target, Vector3 origin)
        {
            target.LookAt(origin);
            var direction = DirectionUtil.CalculateVector(target.GetRotation());
            var speed = target.GetMovementVector().Length;

            target.SetMovementVector(direction * speed);
            var now = Timer.GetTime();

            target.SetStartNoiseChaseTime(now);
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            if (!ShouldReact(target))
                return;

            React(target, instigator.GetPosition());
        }
    }
}

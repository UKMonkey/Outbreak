using Psy.Core;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Vortex.Interface.EntityBase.Properties;
using Outbreak.Entities.Properties;

namespace Outbreak.Server.Entities.Behaviours.OnThinking
{
    public class Accelerating : IEntityBehaviour
    {
        public void PerformBehaviour(Entity target, Entity instigator)
        {
            var movementVector = target.GetMovementVector();
            var newMovementVector = movementVector*target.GetAccelerationRate();
            var maxSpeedSquared = target.GetMaxSpeed();
            maxSpeedSquared *= target.GetMaxSpeed();

            if (newMovementVector.LengthSquared >= maxSpeedSquared)
            {
                newMovementVector = newMovementVector.NormalizeRet()*target.GetMaxSpeed();
            }

            target.SetMovementVector(newMovementVector);
        }
    }
}

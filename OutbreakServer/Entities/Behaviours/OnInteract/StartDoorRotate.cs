using System;
using Outbreak.Entities.Properties;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Vortex.Interface.EntityBase.Properties;

namespace Outbreak.Server.Entities.Behaviours.OnInteract
{
    public class StartDoorRotate : IEntityBehaviour
    {
        private readonly float _rotationRate;


        public StartDoorRotate(float rotationRate)
        {
            _rotationRate = rotationRate;
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            var isOpen = target.GetDoorIsOpen();
            var targetAngle = isOpen ? target.GetDoorClosedAngle() : target.GetDoorOpenAngle();
            var currentAngle = target.GetRotation();
            var rotateSpeed = _rotationRate;

            var angleDifference = currentAngle - targetAngle;
            while (angleDifference > (float)(Math.PI * 2))
                angleDifference -= (float) (Math.PI*2);
            while (angleDifference < 0)
                angleDifference += (float) (Math.PI*2);

            if (angleDifference < Math.PI)
                rotateSpeed *= -1;

            target.SetRotationSpeed(rotateSpeed);
            target.SetRotationTarget(targetAngle);
            target.SetDoorIsOpen(!isOpen);
        }
    }
}

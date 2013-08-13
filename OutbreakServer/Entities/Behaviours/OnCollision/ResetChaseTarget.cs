using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Vortex.Interface.EntityBase.Properties;
using Outbreak.Entities.Properties;

namespace Outbreak.Server.Entities.Behaviours.OnCollision
{
    public class ResetChaseTarget : IEntityBehaviour
    {
        public void PerformBehaviour(Entity target, Entity instigator)
        {
            // if we hit a static solid entity then we can reset the target,
            // else don't  (or if there's no instigator then just clear it)
            if (instigator != null && (!instigator.GetStatic() || !instigator.GetSolid()))
                return;
                
            target.ClearChaseTarget();
        }
    }
}

using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Vortex.Interface.EntityBase.Properties;
using Outbreak.Entities.Properties;

namespace Outbreak.Server.Entities.Behaviours.OnCollision
{
    public class PerformHeal : IEntityBehaviour
    {
        public void PerformBehaviour(Entity target, Entity instigator)
        {
            if (instigator == null)
                return;

            if (!instigator.GetIsHuman())
                return;

            var amount = target.GetHealAmount();

            instigator.SetHealth(instigator.GetHealth() + amount);
            if (instigator.GetHealth() > instigator.GetMaxHealth())
            {
                instigator.SetHealth(instigator.GetMaxHealth());
            }
        }
    }
}

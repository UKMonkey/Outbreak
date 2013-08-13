using Psy.Core;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Outbreak.Entities.Properties;
using Vortex.Interface.EntityBase.Properties;

namespace Outbreak.Server.Entities.Behaviours.OnThinking
{
    public class GetHungry : IEntityBehaviour
    {
        public void PerformBehaviour(Entity target, Entity instigator)
        {
            if (!target.HasHungerTimer())
            {
                ResetHungerTimer(target);
                return;
            }

            if (!ShouldTick(target))
                return;

            GetMoreHungry(target);

            ResetHungerTimer(target);
        }

        private static void GetMoreHungry(Entity target)
        {
            var hunger = target.GetHunger();

            // can't be any more hungry!
            if (hunger < Consts.MaxHunger)
            {
                hunger++;
                target.SetHunger(hunger);                
            }

            if (hunger == Consts.MaxHunger)
            {
                var health = target.GetHealth();
                if (health > 0)
                {
                    health--;
                    target.SetHealth(health);
                }
            }

        }

        private static bool ShouldTick(Entity target)
        {
            return Timer.GetTime() > (target.GetHungerTimer() + Consts.HungerDelayMilliseconds);
        }

        private static void ResetHungerTimer(Entity target)
        {
            target.SetHungerTimer(Timer.GetTime() + Consts.HungerDelayMilliseconds);
        }
    }
}
using System;
using Psy.Core;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Damage;
using Outbreak.Entities.Properties;


namespace Outbreak.Server.Entities.Damage
{
    public class BasicDamageHandler : IEntityDamageHandler
    {
            // value at which we take a % off the damage rather than apply a % miss chance
        private const float HighDamageValue = 10;

        // @ret value between -1 & 1
        // negative values increase damage by an amount
        // positive values decrease damage by an amount
        private static float GetArmourValue(Entity target, DamageTypeEnum type)
        {
            return target.GetInventoryDefenceValue(type);
        }

        private static float ApplyArmour(float damage, float armourValue)
        {
            if (damage > HighDamageValue)
            {
                var editAmount = (armourValue)*damage;
                return Math.Max(0, damage - editAmount);
            }

            var applyModifier = StaticRng.Random.NextDouble() < Math.Abs(armourValue);
            if (!applyModifier)
                return damage;

            if (armourValue > 0)
                return 0;

            // armour value < 0 -> subtract the result to get larger number
            return damage - damage*(armourValue);
        }

        public int EstablishDamage(Entity target, float amount, DamageTypeEnum type, Entity dealer)
        {
            if (target.GetIsGod())
            {
                return 0;
            }

            var armourValue = GetArmourValue(target, type);
            return (int)Math.Floor(ApplyArmour(amount, armourValue));
        }
    }
}

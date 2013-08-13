using System;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Damage;

namespace Outbreak.Server.Entities.Damage
{
    public class WoodenDamageHandler : IEntityDamageHandler
    {
        public int EstablishDamage(Entity target, float amount, DamageTypeEnum type, Entity dealer)
        {
            switch (type)
            {
                case DamageTypeEnum.BluntMelee:
                    return (int)Math.Floor(amount * 0.1f);

                case DamageTypeEnum.SharpMelee:
                    return (int)Math.Round(amount*3);

                case DamageTypeEnum.Fire:
                    return (int) Math.Round(amount*2);

                case DamageTypeEnum.Biological:
                case DamageTypeEnum.Ice:
                    return 0;

                case DamageTypeEnum.Nuclear:
                    return (int)Math.Round(amount*0.5f);

                default:
                    return (int)Math.Floor(amount);
            }
        }
    }
}

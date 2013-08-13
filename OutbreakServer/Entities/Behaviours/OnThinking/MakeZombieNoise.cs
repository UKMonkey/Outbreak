using Outbreak.Audio;
using Outbreak.Resources;
using Psy.Core;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Vortex.Interface.EntityBase.Properties;
using GameEntityPropertyEnum = Outbreak.Entities.Properties.GameEntityPropertyEnum;

namespace Outbreak.Server.Entities.Behaviours.OnThinking
{
    public class MakeZombieNoise : IEntityBehaviour
    {
        private const int RandomDelayMultiplier = 32000;
        private const int BaseDelay = 16000;
        private double _lastSound = 0;
        private const double MinGroanDelay = 3000;

        private IServer Engine { get; set; }

        private readonly string[] _availableMoans = new[]
                                                        {
                                                            Sound.ZombieGroan2,
                                                            Sound.ZombieGroan3,
                                                            Sound.ZombieGroan4,
                                                        };

        public MakeZombieNoise(IServer engine)
        {
            Engine = engine;
        }

        private bool EnsureHasProperty(Entity target)
        {
            if (target.HasProperty((int)GameEntityPropertyEnum.NextZombieMoan))
                return true;

            var now = Timer.GetTime();
            SetNextMoan(target, now);

            return false;
        }

        private void SetNextMoan(Entity target, double now)
        {
            var nextMoan = now + (StaticRng.Random.NextDouble() * RandomDelayMultiplier) + BaseDelay;

            target.SetProperty(
                new EntityProperty((short)GameEntityPropertyEnum.NextZombieMoan, nextMoan) {IsDirtyable = false}
                );
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            if (!EnsureHasProperty(target))
                return;
            
            var now = Timer.GetTime();
            var nextMoan = target.GetProperty((short) GameEntityPropertyEnum.NextZombieMoan).DoubleValue;

            if (now < nextMoan)
                return;

            SetNextMoan(target, now);
            var soundId = StaticRng.Random.Next(0, _availableMoans.Length);

            if (_lastSound + MinGroanDelay > now)
                return;

            _lastSound = now;
            Engine.PlaySoundOnEntity(target, (byte)AudioChannel.Entity, _availableMoans[soundId]);
        }
    }
}

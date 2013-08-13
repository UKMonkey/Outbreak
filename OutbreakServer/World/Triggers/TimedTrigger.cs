using System.Collections.Generic;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.World.Triggers;

namespace Outbreak.Server.World.Triggers
{
    public abstract class TimedTrigger : TriggerBase
    {
        public override TriggerActivationMethod ActivationMethod 
            { get { return TriggerActivationMethod.Timer; }}

        /** Frequency this trigger should go at (per day)
         */
        private const int DefaultFrequency = 1;
        private const string FrequencyKey = "Frequency";
        private uint _frequency;
        public uint Frequency
        {
            get { return _frequency; }
            set
            {
                _frequency = value;
                Properties.Remove(FrequencyKey);
                Properties.Add(FrequencyKey, string.Format("{0}", value));
            }
        }

        protected uint LastActivated { get; private set; }
        protected uint NextActivation { get; private set; }

        protected TimedTrigger(IEngine engine)
            :base (engine)
        {
            Frequency = DefaultFrequency;
            LastActivated = 0;
            NextActivation = 0;
        }

        protected TimedTrigger(IEngine engine, TriggerKey uniqueKey, Vector3 location, uint frequency)
            :base (engine, uniqueKey, location)
        {
            Frequency = frequency;
            LastActivated = 0;
            NextActivation = 0;
        }

        public override void SetProperties(TriggerKey key, Vector3 location, IEnumerable<KeyValuePair<string, string>> properties)
        {
            base.SetProperties(key, location, properties);

            string freqValue;
            if (Properties.TryGetValue(FrequencyKey, out freqValue))
            {
                if (!uint.TryParse(freqValue, out _frequency))
                    _frequency = DefaultFrequency;                
            }
        }

        public override void Update()
        {
            if (Engine.TimeOfDayProvider.TimeOfDay >= NextActivation)
            {
                RegisterActivation();
                Activate();
            }
        }

        protected virtual void Activate()
        {
            OnActivated(this);
        }

        private void RegisterActivation()
        {
            LastActivated = Engine.TimeOfDayProvider.TimeOfDay;
            var delay = Engine.TimeOfDayProvider.TicksPerDay / Frequency;

            NextActivation = (Engine.TimeOfDayProvider.TimeOfDay + delay) & Engine.TimeOfDayProvider.TicksPerDay;
        }
    }
}

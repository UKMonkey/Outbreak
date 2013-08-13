using System.Linq;
using System.Collections.Generic;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.World.Triggers;

namespace Outbreak.Server.World.Triggers
{
    public abstract class TimedAndProximityTrigger : TimedTrigger
    {
        /**  Types that can set off the trigger
         */
        private readonly List<short> _proximityTypes;

        /** distance that a proximityType must be to the trigger to set it off
         */
        private const int DefaultRange = 0;
        private const string RangeKey = "Range";
        private float _range;
        public float Range
        {
            get { return _range; }
            set
            {
                _range = value;
                Properties.Remove(RangeKey);
                Properties.Add(RangeKey, string.Format("{0}", value));
            }
        }

        /**  Entity that set off the proximity
         */
        protected Entity ProximityTarget { get; private set; }

        protected TimedAndProximityTrigger(IEngine engine, IEnumerable<short> proximityTypes) : 
            base(engine)
        {
            _proximityTypes = new List<short>();
            _proximityTypes.AddRange(proximityTypes);
        }

        protected TimedAndProximityTrigger(IEngine engine, IEnumerable<short> proximityTypes,
                                           float range, TriggerKey uniqueKey, Vector3 location, uint frequency) : 
                                               base(engine, uniqueKey, location, frequency)
        {
            _proximityTypes = new List<short>();
            _proximityTypes.AddRange(proximityTypes);
            _range = range;
        }

        public override void SetProperties(TriggerKey key, Vector3 location, IEnumerable<KeyValuePair<string, string>> properties)
        {
            base.SetProperties(key, location, properties);

            string rngValue;
            if (Properties.TryGetValue(RangeKey, out rngValue))
            {
                if (!float.TryParse(rngValue, out _range))
                    _range = DefaultRange;
            }
        }

        private bool ProximitySatisfied()
        {
            var entities = Engine.GetEntitiesWithinArea(Location, _range);
            var applicableEntities = entities.Where(item => _proximityTypes.Contains(item.EntityTypeId));
            ProximityTarget = applicableEntities.FirstOrDefault();
            return ProximityTarget != null;
        }

        protected override void Activate()
        {
            if (ProximitySatisfied())
            {
                base.Activate();
            }
        }
    }
}
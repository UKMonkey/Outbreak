using System;
using Vortex.Interface.Traits;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Psy.Core;

namespace Outbreak.Items.Modifications
{
    public class FloatModification : IItemSpecModification, IInventoryItemModification
    {
        private readonly short _property;

        private readonly float ?_min;
        private readonly float ?_max;
        private readonly float ?_range;

        public FloatModification(ItemSpecPropertyEnum property,
                               float min,
                               float max)
        {
            _property = (short)property;
            _max = max;
            _min = min;
        }

        public FloatModification(ItemSpecPropertyEnum property,
                               float range)
        {
            _property = (short) property;
            _range = range;
        }

        public FloatModification(InventoryItemPropertyEnum property,
                               float min,
                               float max)
        {
            _property = (short)property;
            _max = max;
            _min = min;
        }

        private float GetExpected(Trait baseValue)
        {
            if (baseValue == null)
            {
                if (!_min.HasValue || !_max.HasValue)
                    throw new Exception("Unable to apply float mod as no base value, min or max has been defined");
                return (_min.Value + _max.Value) / 2f;                
            }

            return baseValue.FloatValue;
        }

        protected float GetNewValue(float expected)
        {
            float min;
            float max;

            if (_range.HasValue)
            {
                min = expected - _range.Value/2;
                max = expected + _range.Value/2;
            }
            else if (_min.HasValue && _max.HasValue)
            {
                min = _min.Value;
                max = _max.Value;
            }
            else
            {
                throw new Exception("Unable to get new value as min, max & range not specified");
            }
            return (float)StaticRng.Random.RandomBell(min, expected, max);
        }

        public void Apply(ItemSpec target)
        {
            var expected = GetExpected(target.GetProperty((ItemSpecPropertyEnum)_property));
            var newValue = GetNewValue(expected);

            target.SetProperty(new ItemSpecProperty(_property, newValue));
        }

        public void Apply(InventoryItem target)
        {
            var expected = GetExpected(target.GetProperty((InventoryItemPropertyEnum)_property));
            var newValue = GetNewValue(expected);

            target.SetProperty(new InventoryItemProperty(_property, newValue));
        }
    }
}

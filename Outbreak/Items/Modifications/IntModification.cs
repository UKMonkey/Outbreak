using System;
using Vortex.Interface.Traits;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Psy.Core;

namespace Outbreak.Items.Modifications
{
    public class IntModification : IItemSpecModification, IInventoryItemModification
    {
        private readonly short _property;
        private readonly int _min;
        private readonly int _max;

        public IntModification(ItemSpecPropertyEnum property,
                               int min,
                               int max)
        {
            _property = (short)property;
            _max = max;
            _min = min;
        }

        public IntModification(InventoryItemPropertyEnum property,
                               int min,
                               int max)
        {
            _property = (short)property;
            _max = max;
            _min = min;
        }

        private int GetExpected(Trait baseValue)
        {
            if (baseValue == null)
                return (int)Math.Round((_min + _max) / 2f);
            return baseValue.IntValue;
        }

        public void Apply(ItemSpec target)
        {
            var expected = GetExpected(target.GetProperty((ItemSpecPropertyEnum)_property));
            var newValue = (int)StaticRng.Random.RandomBell(_min, expected, _max);

            target.SetProperty(new ItemSpecProperty(_property, newValue));
        }

        public void Apply(InventoryItem target)
        {
            var expected = GetExpected(target.GetProperty((InventoryItemPropertyEnum)_property));
            var newValue = (int)StaticRng.Random.RandomBell(_min, expected, _max);

            target.SetProperty(new InventoryItemProperty(_property, newValue));
        }
    }
}

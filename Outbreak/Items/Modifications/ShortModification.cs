using System;
using Vortex.Interface.Traits;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Psy.Core;

namespace Outbreak.Items.Modifications
{
    public class ShortModification : IItemSpecModification, IInventoryItemModification
    {
        private readonly short _property;
        private readonly short _min;
        private readonly short _max;

        public ShortModification(ItemSpecPropertyEnum property,
                               short min,
                               short max)
        {
            _property = (short)property;
            _max = max;
            _min = min;
        }

        public ShortModification(InventoryItemPropertyEnum property,
                               short min,
                               short max)
        {
            _property = (short)property;
            _max = max;
            _min = min;
        }

        private short GetExpected(Trait baseValue)
        {
            if (baseValue == null)
                return (short) Math.Round((_min + _max)/2f);
            return baseValue.ShortValue;
        }

        public void Apply(ItemSpec target)
        {
            var expected = GetExpected(target.GetProperty((ItemSpecPropertyEnum) _property));
            var newValue = (short)StaticRng.Random.RandomBell(_min, expected, _max);

            target.SetProperty(new ItemSpecProperty(_property, newValue));
        }

        public void Apply(InventoryItem target)
        {
            var expected = GetExpected(target.GetProperty((InventoryItemPropertyEnum) _property));
            var newValue = (short)StaticRng.Random.RandomBell(_min, expected, _max);

            target.SetProperty(new InventoryItemProperty(_property, newValue));
        }
    }
}

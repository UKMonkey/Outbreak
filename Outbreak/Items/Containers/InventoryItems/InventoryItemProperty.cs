using SlimMath;
using Vortex.Interface.Traits;
using Psy.Core;

namespace Outbreak.Items.Containers.InventoryItems
{
    public class InventoryItemProperty : Trait
    {
        public InventoryItemProperty()
        {}

        public InventoryItemProperty(short id, byte[] data) : base(id, data)
        {
        }

        public InventoryItemProperty(Trait other) : base(other)
        {
        }

        public InventoryItemProperty(short id, bool data) : base(id, data)
        {
        }

        public InventoryItemProperty(short id, int data) : base(id, data)
        {
        }

        public InventoryItemProperty(short id, string data) : base(id, data)
        {
        }

        public InventoryItemProperty(short id, byte data): base(id, data)
        {
        }

        public InventoryItemProperty(short id, short data) : base(id, data)
        {
        }

        public InventoryItemProperty(short id, Vector3 data) : base(id, data)
        {
        }

        public InventoryItemProperty(short id, float data) : base(id, data)
        {
        }

        public InventoryItemProperty(short id, long data)
            : base(id, data)
        {
        }
    }
}

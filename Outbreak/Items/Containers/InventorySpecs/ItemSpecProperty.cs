using SlimMath;
using Vortex.Interface.Traits;

namespace Outbreak.Items.Containers.InventorySpecs
{
    public class ItemSpecProperty : Trait
    {
        public ItemSpecProperty()
        {}

        public ItemSpecProperty(short id, byte[] data) : base(id, data)
        {
        }

        public ItemSpecProperty(Trait other) : base(other)
        {
        }

        public ItemSpecProperty(short id, bool data) : base(id, data)
        {
        }

        public ItemSpecProperty(ItemSpecPropertyEnum id, bool data) : base((short)id, data)
        {
        }

        public ItemSpecProperty(short id, int data) : base(id, data)
        {
        }

        public ItemSpecProperty(ItemSpecPropertyEnum id, int data) : base((short)id, data)
        {
        }

        public ItemSpecProperty(short id, string data) : base(id, data)
        {
        }

        public ItemSpecProperty(ItemSpecPropertyEnum id, string data) : base((short)id, data)
        {
        }

        public ItemSpecProperty(short id, byte data): base(id, data)
        {
        }

        public ItemSpecProperty(ItemSpecPropertyEnum id, byte data) : base((short)id, data)
        {
        }

        public ItemSpecProperty(short id, short data) : base(id, data)
        {
        }

        public ItemSpecProperty(ItemSpecPropertyEnum id, short data) : base((short)id, data)
        {
        }

        public ItemSpecProperty(short id, Vector3 data) : base(id, data)
        {
        }

        public ItemSpecProperty(ItemSpecPropertyEnum id, Vector3 data) : base((short)id, data)
        {
        }

        public ItemSpecProperty(short id, float data) : base(id, data)
        {
        }

        public ItemSpecProperty(ItemSpecPropertyEnum id, float data) : base((short)id, data)
        {
        }

        public ItemSpecProperty(short id, object data): base(id, data)
        {
        }

        public ItemSpecProperty(ItemSpecPropertyEnum id, object data) : base ((short)id, data)
        {
        }
    }
}

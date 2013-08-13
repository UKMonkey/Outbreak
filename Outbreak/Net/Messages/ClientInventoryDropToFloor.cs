using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ClientInventoryDropToFloor : Message
    {
        public byte SourceSlot { get; set; }
        public long InventoryId { get; set; }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            SourceSlot = messageStream.ReadByte();
            InventoryId = messageStream.ReadInt64();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteByte(SourceSlot);
            messageStream.WriteInt64(InventoryId);
        }
    }
}
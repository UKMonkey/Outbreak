using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ClientUseItemMessage : Message
    {
        public long InventoryId { get; set; }
        public byte InventorySlotId { get; set; }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            InventoryId = messageStream.ReadInt64();
            InventorySlotId = messageStream.ReadByte();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteInt64(InventoryId);
            messageStream.WriteByte(InventorySlotId);
        }
    }
}

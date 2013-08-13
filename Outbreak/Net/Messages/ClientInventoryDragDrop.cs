using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ClientInventoryDragDrop : Message
    {
        public long SourceInventoryId { get; set; }
        public byte SourceSlot { get; set; }
        public long TargetInventoryId { get; set; }
        public byte TargetSlot { get; set; }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            SourceInventoryId = messageStream.ReadInt64();
            SourceSlot = messageStream.ReadByte();
            TargetInventoryId = messageStream.ReadInt64();
            TargetSlot = messageStream.ReadByte();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteInt64(SourceInventoryId);
            messageStream.WriteByte(SourceSlot);
            messageStream.WriteInt64(TargetInventoryId);
            messageStream.WriteByte(TargetSlot);
        }
    }
}
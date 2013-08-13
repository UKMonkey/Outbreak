using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ClientRequestInventoryUpdates : Message
    {
        public long InventoryId;

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            InventoryId = messageStream.ReadInt64();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteInt64(InventoryId);
        }
    }
}

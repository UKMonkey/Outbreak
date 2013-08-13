using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ServerStopItemUsage : Message
    {
        public int EntityId;
        public bool Success;

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            EntityId = messageStream.ReadEntityId();
            Success = messageStream.ReadBoolean();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteEntityId(EntityId);
            messageStream.WriteBool(Success);
        }
    }
}

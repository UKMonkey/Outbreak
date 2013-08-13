using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ServerStartItemUsage : Message
    {
        public int EntityId;
        public int SpecId;
        public int UsageTime;

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            EntityId = messageStream.ReadEntityId();
            SpecId = messageStream.ReadInt32();
            UsageTime = messageStream.ReadInt32();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteEntityId(EntityId);
            messageStream.WriteInt32(SpecId);
            messageStream.WriteInt32(UsageTime);
        }
    }
}

using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ServerShowContainerContentsMessage : Message
    {
        public int ContainerEntityId { get; set; }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            ContainerEntityId = messageStream.ReadInt32();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteInt32(ContainerEntityId);
        }
    }
}
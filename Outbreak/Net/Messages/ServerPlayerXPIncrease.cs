using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ServerPlayerXPIncrease : Message
    {
        public int Amount { get; set; }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            Amount = messageStream.ReadInt32();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteInt32(Amount);
        }
    }
}
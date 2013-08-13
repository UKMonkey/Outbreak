using System;
using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ClientItemSpecMessage : Message
    {
        public int ItemSpecId;

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            ItemSpecId = messageStream.ReadInt32();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteInt32(ItemSpecId);
        }
    }
}

using System;
using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ServerGameNameMessage : Message
    {
        public string Name { get; set; }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            Name = messageStream.ReadString();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.Write(Name);
        }
    }
}

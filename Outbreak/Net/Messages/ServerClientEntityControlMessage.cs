using System.Collections.Generic;
using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ServerClientEntityControlMessage : Message
    {
        public ushort ClientId { get; set; }
        public int EntityId { get; set; }

        public ServerClientEntityControlMessage()
        {
            ExpiryDelay = DoesNotExpire;
        }

        public override IEnumerable<int> EntityIds()
        {
            return new List<int>{EntityId};
        }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            ClientId = messageStream.ReadUint16();
            EntityId = messageStream.ReadEntityId();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteUInt16(ClientId);
            messageStream.WriteEntityId(EntityId);
        }
    }
}

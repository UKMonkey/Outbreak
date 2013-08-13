using Vortex.Interface.Net;
using Outbreak.Items;

namespace Outbreak.Net.Messages
{
    public class ServerClientChangeEquippedWeapon : Message
    {
        public ushort ClientId { get; private set; }
        public int ItemSpecId { get; private set; }

        public ServerClientChangeEquippedWeapon() {}

        public ServerClientChangeEquippedWeapon(ushort clientId, int itemSpecId)
        {
            ClientId = clientId;
            ItemSpecId = itemSpecId;
        }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            ItemSpecId = messageStream.ReadInt32();
            ClientId = messageStream.ReadUint16();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteInt32(ItemSpecId);
            messageStream.WriteUInt16(ClientId);
        }
    }
}
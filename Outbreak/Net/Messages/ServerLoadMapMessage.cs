using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ServerLoadMapMessage : Message
    {
        public ushort ClientId { get; set; }
        public byte GameTimeHour { get; set; }
        public byte GameTimeMinute { get; set; }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            ClientId = messageStream.ReadUint16();
            GameTimeHour = messageStream.ReadByte();
            GameTimeMinute = messageStream.ReadByte();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteUInt16(ClientId);
            messageStream.WriteByte(GameTimeHour);
            messageStream.WriteByte(GameTimeMinute);
        }
    }
}

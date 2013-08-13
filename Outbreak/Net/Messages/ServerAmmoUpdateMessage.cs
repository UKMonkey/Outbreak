using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ServerAmmoUpdateMessage : Message
    {
        public int LoadedAmmoTotal { get; set; }


        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            LoadedAmmoTotal = messageStream.ReadInt32();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteInt32(LoadedAmmoTotal);
        }
    }
}

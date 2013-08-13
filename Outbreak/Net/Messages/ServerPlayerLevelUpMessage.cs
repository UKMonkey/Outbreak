using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ServerPlayerLevelUpMessage : Message
    {
        public RemotePlayer RemotePlayer { get; set; }
        public int NewLevel { get; set; }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            RemotePlayer = messageStream.ReadRemotePlayer();
            NewLevel = messageStream.ReadInt32();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.Write(RemotePlayer);
            messageStream.WriteInt32(NewLevel);
        }
    }
}
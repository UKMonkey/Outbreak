using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ClientStartFireWeaponMessage : Message
    {
        public float Rotation { get; set; }
        
        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            Rotation = messageStream.ReadRotation();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteRotation(Rotation);
        }
    }
}

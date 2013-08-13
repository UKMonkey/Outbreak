using SlimMath;
using Vortex.Interface.Net;

namespace Outbreak.Net.Messages.FireWeapon
{
    public class ServerFireWeaponMessage : Message
    {
        public Vector3 StartPoint;

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            StartPoint = messageStream.ReadVector();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.Write(StartPoint);
        }
    }
}

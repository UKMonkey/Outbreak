using SlimMath;
using Vortex.Interface.Net;

namespace Outbreak.Net.Messages
{
    public class ClientMoveMessage : Message
    {
        public Vector3 Position { get; set; }
        public float Rotation { get; set; }
        public Vector3 MovementVector { get; set; }

        public ClientMoveMessage()
        {
            DeliveryMethod = DeliveryMethod.UnreliableSequenced;
            Channel = 2;
        }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            Position = messageStream.ReadVector();
            Rotation = messageStream.ReadFloat();
            MovementVector = messageStream.ReadVector();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.Write(Position);
            messageStream.WriteFloat(Rotation);
            messageStream.Write(MovementVector);
        }
    }
}

using Vortex.Interface.Net;

namespace Outbreak.Net.Messages.FireWeapon
{
    public class ServerFirePistolMessage : ServerFireBulletMessage
    {
        public int EntityUser { get; set; }

        public ServerFirePistolMessage()
            :base(1, 1)
        {
        }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            base.DeserializeImpl(messageStream);
            EntityUser = messageStream.ReadEntityId();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            base.SerializeImpl(messageStream);
            messageStream.WriteEntityId(EntityUser);
        }
    }
}

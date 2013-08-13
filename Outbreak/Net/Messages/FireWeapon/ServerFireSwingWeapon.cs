using System.Collections.Generic;
using SlimMath;
using Vortex.Interface.Net;

namespace Outbreak.Net.Messages.FireWeapon
{
    public class ServerFireSwingWeapon : Message
    {
        public float SwingDirection { get; set; }
        public int EntityUser { get; set; }
        public List<int> HitZombies { get; set; }
        public List<int> HitHumans { get; set; }
        public List<int> HitScenery { get; set; }
        public Vector3 StartPoint { get; set; }

        public ServerFireSwingWeapon()
        {
            HitZombies = new List<int>();
            HitScenery = new List<int>();
            HitHumans = new List<int>();
        }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            SwingDirection = messageStream.ReadFloat();
            HitZombies = messageStream.ReadEntityIds();
            HitHumans = messageStream.ReadEntityIds();
            HitScenery = messageStream.ReadEntityIds();
            StartPoint = messageStream.ReadVector();
            EntityUser = messageStream.ReadEntityId();
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteFloat(SwingDirection);
            messageStream.Write(HitZombies);
            messageStream.Write(HitHumans);
            messageStream.Write(HitScenery);
            messageStream.Write(StartPoint);
            messageStream.WriteEntityId(EntityUser);
        }
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using SlimMath;
using Vortex.Interface.Net;

namespace Outbreak.Net.Messages.FireWeapon
{
    public class BulletEffectData
    {
        public Vector3 EndPoint { get; set; }
        public float Rotation { get; set; }
        public byte Effect { get; set; }
    }

    public class ServerFireBulletMessage : ServerFireWeaponMessage
    {
        private readonly short _maxBulletCount;
        private readonly short _minBulletCount;

        public List<BulletEffectData> BulletEffects;


        protected ServerFireBulletMessage(short minBulletCount = 0, short maxBulletCount=256)
        {
            _minBulletCount = minBulletCount;
            _maxBulletCount = maxBulletCount;

            BulletEffects = new List<BulletEffectData>(maxBulletCount);
        }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            base.DeserializeImpl(messageStream);

            byte count = 1;

            if (_minBulletCount != 1 || _maxBulletCount != 1)
                count = messageStream.ReadByte();

            BulletEffects = ReadBulletEffects(messageStream, count);
        }

        private static List<BulletEffectData> ReadBulletEffects(IIncomingMessageStream messageStream, short count)
        {
            var ret = new List<BulletEffectData>(count);

            for (var i=0; i<count; ++i)
            {
                var item = new BulletEffectData();
                item.EndPoint = messageStream.ReadVector();
                item.Rotation = messageStream.ReadFloat();
                item.Effect = messageStream.ReadByte();

                ret.Add(item);
            }

            return ret;
        }

        private void WriteBulletEffects(IOutgoingMessageStream messageStream)
        {
            foreach (var item in BulletEffects)
            {
                messageStream.Write(item.EndPoint);
                messageStream.WriteFloat(item.Rotation);
                messageStream.WriteByte(item.Effect);
            }
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            base.SerializeImpl(messageStream);

            if (_maxBulletCount == 1 && _minBulletCount == 1)
            {
                Debug.Assert(BulletEffects.Count == 1);
            }
            else
            {
                messageStream.WriteByte((byte)BulletEffects.Count);
            }
            WriteBulletEffects(messageStream);
        }
    }
}

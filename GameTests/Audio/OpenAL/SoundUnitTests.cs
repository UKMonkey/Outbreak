using NUnit.Framework;
using Vortex.Client.Audio.OpenAL;

namespace UnitTests.Audio.OpenAL
{
    public class SoundUnitTests
    {
        //[Test]
        public void TestBuilder()
        {
            var engine = new OpenALAudioEngine();
            var channel = engine.CreateChannel(0);

            var sample = new OpenALAudioSample("D:\\Program Files (x86)\\Steam\\steamapps\\common\\Nuclear Dawn\\nucleardawn\\sound\\weapons\\grenades\\warning_beep.wav");
            channel.Play(sample);
        }
    }
}

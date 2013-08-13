using System;
using Outbreak.Audio;
using Vortex.Interface;
using Psy.Core.Configuration;

namespace Outbreak.Client.Audio
{
    public static class AudioChannels
    {
        private const string MasterVolumeChannelName = "Volume.Master";
        private const string UIChannelName = "Volume.UI";
        private const string MusicChannelName = "Volume.Music";
        private const string AmbientChannelName = "Volume.Ambient";
        private const string EntityChannelName = "Volume.Ambient";

        public static void Create(IClient engine)
        {
            var playerConfiguration = engine.Configuration.PlayerConfiguration;
            playerConfiguration.AddConfiguration(MasterVolumeChannelName, 0.5f);
            playerConfiguration.AddConfiguration(UIChannelName, 0.5f);
            playerConfiguration.AddConfiguration(MusicChannelName, 0.5f);
            playerConfiguration.AddConfiguration(AmbientChannelName, 0.5f);
            playerConfiguration.AddConfiguration(EntityChannelName, 0.5f);

            foreach (AudioChannel channelEnum in Enum.GetValues(typeof(AudioChannel)))
            {
                if (channelEnum == AudioChannel.Master)
                    continue;

                var audioChannel = engine.AudioEngine.CreateChannel((int)channelEnum);
                audioChannel.ChannelVolume = VolumeLevelFromConfiguration(channelEnum);
            }
        }

        public static float VolumeLevelFromConfiguration(AudioChannel audioChannel)
        {
            switch (audioChannel)
            {
                case AudioChannel.Master:
                    return StaticConfigurationManager.ConfigurationManager.GetFloat(MasterVolumeChannelName);

                case AudioChannel.Interface:
                    return StaticConfigurationManager.ConfigurationManager.GetFloat(UIChannelName);

                case AudioChannel.Music:
                    return StaticConfigurationManager.ConfigurationManager.GetFloat(MusicChannelName);

                case AudioChannel.Entity:
                    return StaticConfigurationManager.ConfigurationManager.GetFloat(EntityChannelName);

                case AudioChannel.Ambient:
                    return StaticConfigurationManager.ConfigurationManager.GetFloat(AmbientChannelName);

                default:
                    throw new ArgumentOutOfRangeException("audioChannel");
            }
        }

        public static string ChannelLevelConfigurationName(AudioChannel audioChannel)
        {
            switch (audioChannel)
            {
                case AudioChannel.Master:
                    return MasterVolumeChannelName;

                case AudioChannel.Interface:
                    return UIChannelName;

                case AudioChannel.Music:
                    return MusicChannelName;

                case AudioChannel.Entity:
                    return EntityChannelName;

                case AudioChannel.Ambient:
                    return AmbientChannelName;

                default:
                    throw new ArgumentOutOfRangeException("audioChannel");
            }
        }
    }
}
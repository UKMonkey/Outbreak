using System;

namespace Launcher.Updatables
{
    public class UpdateVersion
    {
        public Version Version { get; set; }
        public string Url { get; set; }

        public UpdateVersion(string version, string url)
        {
            Version = new Version(version);
            Url = url;
        }
    }
}
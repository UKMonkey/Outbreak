using System;
using System.IO;

namespace Launcher
{
    public class LauncherConfiguration
    {
        private LauncherConfiguration() {}

        public string PatchSource { get; private set; }
        public bool NoDownload { get; private set; }

        public static LauncherConfiguration ReadFromFile(string launcherFile)
        {

            var fileData = File.ReadAllLines(launcherFile);
            var patchSource = "";
            var noDownload = false;

            foreach (var line in fileData)
            {
                if (line.StartsWith(";"))
                    continue;

                var parts = line.Split('=');

                if (parts.Length < 2)
                    continue;

                if (parts[0].ToLower() == "patchsource")
                {
                    patchSource = parts[1];
                }
                else if (parts[0].ToLower() == "nodownload")
                {
                    noDownload = parts[1].ToLower() == "true";
                }
            }

            if (string.IsNullOrEmpty(patchSource))
            {
                throw new Exception("PatchSource not specified in launcher.cfg. Please re-install the launcher.");
            }

            return new LauncherConfiguration
            {
                PatchSource = patchSource,
                NoDownload = noDownload
            };
        }

        
    }
}
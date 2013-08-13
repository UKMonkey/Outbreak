using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Launcher.Updatables
{
    public static class UpdatableFactory
    {
        public static List<Updatable> Create(Dictionary<string, Version> currentVersions)
        {
            var result = new List<Updatable>();

            result.Add(GetEngine());
            result.AddRange(GetMods());
            ApplyCurrentVersions(result, currentVersions);

            return result;
        }

        private static void ApplyCurrentVersions(IEnumerable<Updatable> result, Dictionary<string, Version> currentVersions)
        {
            foreach (var updatable in result)
            {
                Version currentVersion;

                if (!currentVersions.TryGetValue(updatable.Name, out currentVersion))
                {
                    continue;
                }
                updatable.CurrentVersion = currentVersion;
            }
        }

        private static Updatable GetEngine()
        {
            if (!File.Exists("launcher.cfg"))
            {
                throw new Exception("Failed to find launcher.cfg. Please re-install the launcher.");
            }

            var launcherConfiguration = LauncherConfiguration.ReadFromFile("launcher.cfg");
            return ReadPatchFile(launcherConfiguration.PatchSource, "Engine");
        }

        private static IEnumerable<Updatable> GetMods()
        {
            var modDirectory = new DirectoryInfo("Mods");

            var result = new List<Updatable>();

            foreach (var directory in modDirectory.GetDirectories())
            {
                var modLauncherFile = Path.Combine(directory.FullName, "launcher.cfg");
                if (File.Exists(modLauncherFile))
                {
                    var launcherConfiguration = LauncherConfiguration.ReadFromFile(modLauncherFile);

                    result.Add(ReadPatchFile(launcherConfiguration.PatchSource, directory.Name));
                }
            }

            return result;
        }

        private static Updatable ReadPatchFile(string patchSource, string name)
        {
            string result;

            using (var client = new WebClient())
            {
                result = client.DownloadString(patchSource);
            }

            var lines = result.Replace("\r", "").Split('\n');

            Version stableVersion = null;
            var versions = new List<UpdateVersion>();

            foreach (var line in lines)
            {
                var parts = line.Split('=');

                if (parts.Length < 2)
                    continue;

                if (parts[0].ToLower() == "stableversion")
                {
                    stableVersion = new Version(parts[1]);
                }
                else if (parts[0].ToLower().StartsWith("version:"))
                {
                    var versionPart = parts[0].Split(':');
                    var versionNumber = versionPart[1];
                    var url = parts[1];

                    versions.Add(new UpdateVersion(versionNumber, url));
                }
            }

            if (stableVersion == null)
            {
                throw new Exception(string.Format("No stable version specified in update source {0}", patchSource));
            }

            return new Updatable
            {
                Name = name,
                AvailableVersions = versions,
                LatestVersion = stableVersion,
            };

        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Launcher.Updatables
{
    public static class VersionData
    {
        public static Dictionary<string, Version> Load()
        {
            var result = new Dictionary<string, Version>();

            if (!File.Exists("versions.dat"))
            {
                return result;
            }

            var versionData = File.ReadAllLines("versions.dat");

            foreach (var line in versionData)
            {
                var parts = line.Split('=');
                var name = parts[0];
                var version = parts[1];

                result[name] = new Version(version);
            }

            return result;
        }

        public static Dictionary<string, Version> GetFrom(IEnumerable<Updatable> updatables)
        {
            var result = new Dictionary<string, Version>();

            foreach (var updatable in updatables)
            {
                result[updatable.Name] = updatable.CurrentVersion;
            }

            return result;
        }

        public static void Save(Dictionary<string, Version> versionData)
        {
            File.WriteAllLines("versions.dat", versionData.Select(x => string.Format("{0}={1}", x.Key, x.Value)));
        }
    }
}
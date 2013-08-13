using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Launcher.Updatables
{
    public class Updatable
    {
        public const string EngineName = "Engine";

        public string Name { get; set; }
        public Version LatestVersion { get; set; }
        public List<UpdateVersion> AvailableVersions { get; set; }
        public Version CurrentVersion { get; set; }
        public bool InProgress { get; set; }

        public bool RequiresUpdating
        {
            get
            {
                if (CurrentVersion == null)
                    return true;

                return CurrentVersion < LatestVersion;
            }
        }

        public UpdateVersion LatestUpdate
        {
            get
            {
                return AvailableVersions.Single(x => x.Version == LatestVersion);
            }
        }

        public string ExtractLocation
        {
            get
            {
                return Name == EngineName ? "." : Path.Combine("Mods", Name);
            }
        }

        
    }
}
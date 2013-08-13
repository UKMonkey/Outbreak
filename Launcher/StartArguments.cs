using System;

namespace Launcher
{
    public class StartArguments
    {
        public readonly string ModName;

        private StartArguments(string modName)
        {
            ModName = modName;
        }

        public static StartArguments ParseFrom(string[] args)
        {
            var modName = "";

            for (int i = 0; i < args.Length; i++)
            {
                var atl = args[i].ToLower();

                if (atl == "-mod")
                {
                    modName = args[i + 1];
                }
            }

            if (modName == "")
            {
                throw new Exception("Mod name must be specified with -mod command line argument.");
            }

            return new StartArguments(modName);
        }
    }
}
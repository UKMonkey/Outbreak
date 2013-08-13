using System;
using System.Globalization;
using System.IO;
using Outbreak.Items.Containers.InventoryItems;
using Psy.Core.Logging;
using Vortex.Interface;
using Vortex.Interface.World.Chunks;

namespace Outbreak.Server.Persistance
{
    public class Utils
    {
        private static readonly string SavePath = EstablishSavePath();

        // root directory we'll save data in.  We'll add another directory of the
        // game name afterwards so that we can save several games in that directory
        // but this should just be something of the form ~/outbreak/saveData
        private static string EstablishSavePath()
        {
            var savePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(savePath, "Outbreak", "Saves");
        }

        public static string GetRootSaveDirectory(IGame game)
        {
            var saveDir = Path.Combine(SavePath, game.GameName);
            Directory.CreateDirectory(saveDir);
            return saveDir;
        }

        public static string GetPathForChunk(IGame game, ChunkKey key)
        {
            var saveDir = Path.Combine(GetRootSaveDirectory(game), key.X.ToString(CultureInfo.InvariantCulture));
            saveDir = Path.Combine(saveDir, key.Y.ToString(CultureInfo.InvariantCulture));

            Directory.CreateDirectory(saveDir);
            return saveDir;
        }

        public static string GetFloatingItemPath(IGame game)
        {
            var path = Path.Combine(GetRootSaveDirectory(game), "FloatingItems");
            Directory.CreateDirectory(path);

            return path;
        }

        public static string GetInventoryPath(IGame game)
        {
            var path = Path.Combine(GetRootSaveDirectory(game), "Inventories");
            Directory.CreateDirectory(path);

            return path;
        }

        public static string GetPathForFloatingItem(IGame game, int itemId)
        {
            return Path.Combine(GetFloatingItemPath(game), string.Format("Item_{0}", itemId));
        }
    }
}

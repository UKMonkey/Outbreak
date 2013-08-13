using System.Collections.Generic;
using System.IO;
using Outbreak.Items.Containers.FloatingItems;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Server.Persistance.File.Base;
using Psy.Core.Serialization;
using Vortex.Interface.Serialisation;

namespace Outbreak.Server.Persistance.File.FloatingItems
{
    class FloatingItemLoader : FileLoader
    {
        private class SavedData
        {
            public InventoryItem Item;
            public int Id;
        }

        public override void Dispose()
        {
        }

        private static SavedData DecodeSavedData(Stream stream)
        {
            var id = stream.ReadInt();
            var specId = stream.ReadInt();
            var properties = stream.ReadTraits<InventoryItemProperty>();

            var item = new InventoryItem(specId);
            item.SetProperties(properties);

            stream.Close();
            stream.Dispose();

            return new SavedData
            {
                Id = id,
                Item = item
            };
        }

        private IEnumerable<SavedData> GetSavedData()
        {
            var path = Utils.GetFloatingItemPath(Game);
            if (!Directory.Exists(path))
                yield break;

            foreach (var fileName in Directory.EnumerateFiles(path))
            {
                var stream = new FileStream(fileName, FileMode.Open);
                yield return DecodeSavedData(stream);
            }
        }

        public void LoadData(IFloatingItemCache instance)
        {
            foreach (var decodedData in GetSavedData())
            {
                instance.ForceAddItem(decodedData.Id, decodedData.Item);
            }
        }
    }
}

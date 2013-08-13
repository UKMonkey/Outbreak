using System.Collections.Generic;
using System.IO;
using Outbreak.Server.Persistance.File.Base;

namespace Outbreak.Server.Persistance.File.FloatingItems
{
    class FloatingItemSaver : FileSaver
    {
        public override void Dispose()
        {
        }

        public void SaveItems(List<InventorySaveData> itemsToSave)
        {
            foreach (var item in itemsToSave)
            {
                var stream = new FileStream(item.Targetname, FileMode.Create);
                stream.Write(item.Data, 0, item.Data.Length);
                stream.Close();
                stream.Dispose();
            }
        }

        public void DeleteItems(List<string> itemsToDelete)
        {
            foreach (var item in itemsToDelete)
                System.IO.File.Delete(item);
        }
    }
}

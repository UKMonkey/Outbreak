using System.Collections.Generic;
using System.IO;

namespace Outbreak.Server.Persistance.File.Inventories
{
    public class InventorySaver : Base.FileSaver
    {
        public override void Dispose()
        {
        }

        public void PerformSave(InventorySaveData data)
        {
            var stream = new FileStream(data.Targetname, FileMode.Create);
            stream.Write(data.Data, 0, data.Data.Length);
            stream.Close();
            stream.Dispose();
        }

        public void PerformSave(IEnumerable<InventorySaveData> data)
        {
            foreach (var item in data)
                PerformSave(item);
        }

        public void DeleteItems(IEnumerable<string> data)
        {
            foreach (var item in data)
                System.IO.File.Delete(item);
        }
    }
}

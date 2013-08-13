using System.Collections.Generic;
using System.IO;
using Outbreak.Items.Containers;
using Outbreak.Items.Containers.InventoryItems;
using Psy.Core.Serialization;
using Vortex.Interface.Serialisation;

namespace Outbreak.Server.Persistance.File.Inventories
{
    public class InventoryLoader : Base.FileLoader
    {
        public override void Dispose()
        {
        }

        public Inventory LoadInventory(string fileName, long id)
        {
            if (!System.IO.File.Exists(fileName))
                return null;

            var inventory = new Inventory(id, true);
            var stream = new FileStream(fileName, FileMode.Open);

            var size = (byte)stream.ReadByte();
            var type = (InventoryType)stream.ReadInt();
            inventory.Initialise(size, type);

            var count = stream.ReadInt();
            for (int i = 0; i < count; ++i)
            {
                var slot = (byte)stream.ReadByte();
                var specId = stream.ReadInt();

                var item = new InventoryItem(inventory, specId);
                item.SetProperties(stream.ReadTraits<InventoryItemProperty>());

                inventory[slot] = item;
            }

            stream.Close();
            stream.Dispose();

            return inventory;
        }

        public HashSet<long> GetUsedIds(string path)
        {
            var ret = new HashSet<long>();

            if (!Directory.Exists(path))
                return ret;

            foreach (var item in Directory.EnumerateFiles(path))
            {
                ret.Add(long.Parse(Path.GetFileName(item)));
            }
            return ret;
        }
    }
}

using System.IO;
using System.Linq;
using System.Collections.Generic;
using Outbreak.Items.Containers;
using Vortex.Interface;
using Vortex.Interface.Serialisation;
using Psy.Core.Serialization;
using Outbreak.Server.Items.Containers;

namespace Outbreak.Server.Persistance.File.Inventories
{
    /**
     * Because of the threading - the conversion to the byte[] when saving must be done here
     * The good news is that we can at least palm off the pushing to the file to 
     * another thread meaning that disk access won't slow down the game!
     */
    public class InventoryHandler : 
        Base.FileHandler<InventoryLoader, InventorySaver>,
        IInventoryLoader, IInventorySaver
    {
        private Dictionary<long, InventorySaveData> _saveInventories;
        private List<long> _deleteInventories;

        public InventoryHandler(IGame game)
            : base(game, "InventoryHandler")
        {
            _saveInventories = new Dictionary<long, InventorySaveData>();
            _deleteInventories = new List<long>();
        }

        private string GetFileForInventory(long id)
        {
            var path = Utils.GetInventoryPath(Game);
            return Path.Combine(path, string.Format("{0}", id));
        }

        protected override void PerformSave()
        {
            Dictionary<long, InventorySaveData> toSave;
            List<long> toDelete;

            lock (this)
            {
                toSave = _saveInventories;
                _saveInventories = new Dictionary<long, InventorySaveData>();

                toDelete = _deleteInventories;
                _deleteInventories = new List<long>();
            }

            Saver.PerformSave(toSave.Values);
            Saver.DeleteItems(toDelete.Select(item => GetFileForInventory(item)));
        }

        protected override void PerformLoad()
        {
            // loading must be done on the main thread 
        }

        protected byte[] GetInventoryData(Inventory inventory)
        {
            var stream = new MemoryStream();
            var content = inventory.GetContent()
                .Where(item => item.Value != null).
                ToDictionary(item => item.Key, item => item.Value);

            stream.WriteByte(inventory.GetInventorySize());
            stream.Write((int)inventory.InventoryType);

            stream.Write(content.Count);
            foreach (var item in content)
            {
                stream.WriteByte(item.Key);
                stream.Write(item.Value.ItemSpecId);
                stream.Write(item.Value.GetProperties(), item.Value.GetPropertyCount());
            }

            return stream.ToArray();
        }

        public Inventory LoadInventory(long id)
        {
            return Loader.LoadInventory(GetFileForInventory(id), id);
        }

        public HashSet<long> GetAvailableIds()
        {
            return Loader.GetUsedIds(Utils.GetInventoryPath(Game));
        }

        public void SaveInventory(Inventory item)
        {
            if (!item.KeepPersisted)
                return;

            var data = new InventorySaveData
            {
                Data = GetInventoryData(item),
                Targetname = GetFileForInventory(item.Id)
            };
            lock (this)
            {
                _saveInventories[item.Id] = data;
            }
        }

        public void DeleteInventory(Inventory inventory)
        {
            lock (this)
            {
                _deleteInventories.Add(inventory.Id);
            }
        }
    }
}

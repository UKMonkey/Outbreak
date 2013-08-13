using System.Collections.Generic;
using System.IO;
using Outbreak.Items.Containers.FloatingItems;
using Outbreak.Items.Containers.InventoryItems;
using Vortex.Interface;
using Psy.Core.Serialization;
using Vortex.Interface.Serialisation;

namespace Outbreak.Server.Persistance.File.FloatingItems
{
    class FloatingItemHandler
        : Base.FileHandler<FloatingItemLoader, FloatingItemSaver>
    {
        private List<InventorySaveData> _itemsToSave;
        private List<string> _itemsToDelete; 

        public FloatingItemHandler(IGame game) : base(game, "FloatingItems")
        {
            _itemsToDelete = new List<string>();
            _itemsToSave = new List<InventorySaveData>();
        }

        protected override void PerformSave()
        {
            List<InventorySaveData> itemsToSave;
            List<string> itemsToDelete;
            lock(this)
            {
                itemsToSave = _itemsToSave;
                _itemsToSave = new List<InventorySaveData>();

                itemsToDelete = _itemsToDelete;
                _itemsToDelete = new List<string>();
            }

            Saver.SaveItems(itemsToSave);
            Saver.DeleteItems(itemsToDelete);
        }

        protected override void PerformLoad()
        {
            // no need to load other than at the start
        }

        public void SaveItem(int id, InventoryItem item)
        {
            item.OnItemChanged += (x, y) => UpdateSavedData(id, y);
            UpdateSavedData(id, item);
        }

        private static byte[] GetSaveData(int id, InventoryItem item)
        {
            var stream = new MemoryStream();
            stream.Write(id);
            stream.Write(item.ItemSpecId);
            stream.Write(item.GetProperties(), item.GetPropertyCount());
            return stream.ToArray();
        }

        private void UpdateSavedData(int id, InventoryItem item)
        {
                var toAdd = new InventorySaveData
                                {
                                    Data = GetSaveData(id, item),
                                    Targetname = Utils.GetPathForFloatingItem(Game, id)
                                };
            lock (this)
            {
                _itemsToSave.Add(toAdd);
            }
        }

        public void DeleteItem(int id, InventoryItem item)
        {
            lock (this)
            {
                _itemsToDelete.Add(Utils.GetPathForFloatingItem(Game, id));
            }
        }

        public void LoadItems(IFloatingItemCache instance)
        {
            Loader.LoadData(instance);
        }
    }
}

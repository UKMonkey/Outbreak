using System.Collections.Generic;
using System.IO;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Server.Items.Containers;
using Vortex.Interface;

namespace Outbreak.Server.Persistance.File.ItemSpecs
{
    /** Specs don't change once created.  This means that we can 
     *  get the other thread to process the specs rather than convert to 
     *  the byte[] on the main thread.  Should save a little CPU time on that...
     */
    public class ItemSpecHandler :
        Base.FileHandler<ItemSpecLoader, ItemSpecSaver>, IItemSpecLoader
    {
        private readonly List<ItemSpec> _specsToSave;
        private const string DirName = "ItemSpecs";

        public ItemSpecHandler(IGame game)
            :base(game, "Item")
        {
            _specsToSave = new List<ItemSpec>();

            var targetFilename = Utils.GetRootSaveDirectory(game);
            targetFilename = Path.Combine(targetFilename, DirName);
            Directory.CreateDirectory(targetFilename);

            Saver.TargetFilename = targetFilename;
            Loader.TargetFilename = targetFilename;
        }

        protected override void PerformSave()
        {
            var specsToSave = new List<ItemSpec>();
            lock(this)
            {
                specsToSave.AddRange(_specsToSave);
                _specsToSave.Clear();
            }

            Saver.SaveSpecs(specsToSave);
        }

        protected override void PerformLoad()
        {
            // do nothing - there is no callback for when we've loaded specs as this is done on start only
        }

        public void SaveItemSpec(ItemSpec spec)
        {
            lock (this)
            {
                _specsToSave.Add(spec);
            }
        }

        public Dictionary<int, ItemSpec> LoadSpecs()
        {
            lock(this)
            {
                return Loader.LoadSpecs();
            }
        }
    }
}

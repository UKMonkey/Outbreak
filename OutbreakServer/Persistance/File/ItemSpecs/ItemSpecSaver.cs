using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Server.Persistance.File.Base;
using Vortex.Interface.Serialisation;
using Psy.Core.Serialization;

namespace Outbreak.Server.Persistance.File.ItemSpecs
{
    public class ItemSpecSaver : FileSaver
    {
        public string TargetFilename { get; set; }

        private void SaveSpec(ItemSpec spec)
        {
            if (!Directory.Exists(TargetFilename))
                Directory.CreateDirectory(TargetFilename);

            var fullFile = Path.Combine(TargetFilename, spec.Id.ToString(CultureInfo.InvariantCulture));
            var stream = new FileStream(fullFile, FileMode.Create);
            stream.Write(spec.Id);
            stream.Write(spec.GetProperties().ToList());
            stream.Close();
            stream.Dispose();
        }

        public void SaveSpecs(List<ItemSpec> specs)
        {
            foreach (var spec in specs)
            {
                SaveSpec(spec);
            }
        }

        public override void Dispose()
        {
        }
    }
}

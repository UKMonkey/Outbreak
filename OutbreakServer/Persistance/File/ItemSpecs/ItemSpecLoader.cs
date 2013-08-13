using System;
using System.Collections.Generic;
using Outbreak.Items.Containers.InventorySpecs;
using Psy.Core.Logging;
using Psy.Core.Serialization;
using Vortex.Interface.Serialisation;
using System.IO;

namespace Outbreak.Server.Persistance.File.ItemSpecs
{
    public class ItemSpecLoader : Base.FileLoader
    {
        public string TargetFilename { get; set; }

        public override void Dispose()
        {
        }

        private ItemSpec LoadSpec(string fileName)
        {
            var stream = new FileStream(fileName, FileMode.Open);
            var specId = stream.ReadInt();
            var specProperties = stream.ReadTraits<ItemSpecProperty>();
            var spec = new ItemSpec(specId);
            spec.SetProperties(specProperties);

            stream.Close();
            stream.Dispose();

            return spec;
        }


        public Dictionary<int, ItemSpec> LoadSpecs()
        {
            try
            {
                var ret = new Dictionary<int, ItemSpec>();
                foreach(var file in Directory.EnumerateFiles(TargetFilename))
                {
                    var spec = LoadSpec(file);
                    ret[spec.Id] = spec;
                }
                return ret;
            }
            catch (Exception)
            {
                Logger.Write(string.Format("Unable to process directory {0} - no process to load this corrupt game", TargetFilename), LoggerLevel.Critical);
                throw;
            }
        }
    }
}

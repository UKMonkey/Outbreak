using System.Collections.Generic;
using System.IO;
using Outbreak.Server.Persistance.File.Base;


namespace Outbreak.Server.Persistance.File.Entites
{
    public class EntitySaver : FileSaver
    {
        public override void Dispose()
        {
        }

        public void DeleteEntities(List<string> entitiesToDelete)
        {
            foreach (var filename in entitiesToDelete)
            {
                System.IO.File.Delete(filename);
            }
        }

        private void SaveEntity(EntitySaveData data)
        {
            if (data.Data != null)
            {
                var stream = new FileStream(data.Targetname, FileMode.Create);
                stream.Write(data.Data, 0, data.Data.Length);
                stream.Close();
                stream.Dispose();
            }
        }

        public void SaveEntities(List<EntitySaveData> entitiesToSave)
        {
            foreach (var item in entitiesToSave)
            {
                SaveEntity(item);
            }
        }
    }
}

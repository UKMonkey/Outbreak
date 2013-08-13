using System.Collections.Generic;
using System.IO;
using Outbreak.Server.Persistance.File.Base;
using Vortex.Interface.World.Chunks;

namespace Outbreak.Server.Persistance.File.Chunks
{
    public class ChunkSaver : FileSaver
    {
        public string TargetFilename { get; set; }


        protected void SaveChunk(ChunkKey chunkkey, byte[] data)
        {
            var path = Utils.GetPathForChunk(Game, chunkkey);
            var fullFilePath = Path.Combine(path, TargetFilename);

            var file = new FileStream(fullFilePath, FileMode.Create);
            file.Write(data, 0, data.Length);
            file.Close();
            file.Dispose();
        }

        public void SaveData(IDictionary<ChunkKey, byte[]> chunksToSave)
        {
            foreach (var chunk in chunksToSave)
            {
                SaveChunk(chunk.Key, chunk.Value);
            }
        }

        public override void Dispose()
        {
        }
    }
}

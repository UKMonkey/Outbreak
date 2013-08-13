using System;
using System.Collections.Generic;
using Psy.Core.Logging;
using Vortex.Interface.Serialisation;
using Vortex.Interface.World.Chunks;
using System.IO;

namespace Outbreak.Server.Persistance.File.Chunks
{
    public class ChunkLoader : Base.FileLoader
    {
        public event ChunkCallback OnChunkLoad;
        public event ChunkKeyCallback OnChunksUnavailable;

#pragma warning disable 67
        public event ChunkCallback OnChunksGenerated;
#pragma warning restore 67

        public string TargetFilename { get; set; }

        public override void Dispose()
        {
        }

        private Chunk LoadChunk(ChunkKey key)
        {
            var path = Utils.GetPathForChunk(Game, key);
            var fullFilePath = path + TargetFilename;

            if (!System.IO.File.Exists(fullFilePath))
                return null;

            try
            {
                var file = new FileStream(fullFilePath, FileMode.Open);
                var chunk = file.ReadChunk();
                file.Close();
                file.Dispose();
                return chunk;
            }
            catch (Exception e)
            {
                Logger.Write(string.Format("Unable to get {0} data ({1}) - assuming corrupt and will re-create", fullFilePath, e), LoggerLevel.Error);
                System.IO.File.Delete(fullFilePath);
                return null;
            }
        }

        private void LoadChunkSetFromDisk(List<ChunkKey> keys)
        {
            var chunks = new List<Chunk>(keys.Count);
            var unavailable = new List<ChunkKey>(keys.Count);
            foreach (var key in keys)
            {
                var chunk = LoadChunk(key);
                if (chunk == null)
                    unavailable.Add(key);
                else
                    chunks.Add(chunk);
            }

            if (chunks.Count > 0)
                OnChunkLoad(chunks);
            if (unavailable.Count > 0)
                OnChunksUnavailable(unavailable);
        }

        public void LoadChunks(List<ChunkKey> chunkKeys)
        {
            if (chunkKeys.Count == 0)
                return;

            LoadChunkSetFromDisk(chunkKeys);
        }
    }
}

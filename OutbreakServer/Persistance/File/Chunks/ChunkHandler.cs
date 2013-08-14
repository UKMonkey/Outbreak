using System.Collections.Generic;
using Vortex.Interface.Serialisation;
using System.IO;
using System.Linq;
using System.Threading;
using Vortex.Interface;
using Vortex.Interface.World.Chunks;

namespace Outbreak.Server.Persistance.File.Chunks
{
    /** Because the chunk data can change, we have to take care when saving
     * not to have a problem with threading.  So the main thread will convert
     * to a byte array, and the saver thread will perform the disk access.
     */
    public class ChunkHandler :
        Base.FileHandler<ChunkLoader, ChunkSaver>, IChunkLoader, IChunkSaver 
    {
        public event ChunkCallback OnChunkLoad;
        public event ChunkCallback OnChunksGenerated;
        public event ChunkKeyCallback OnChunksUnavailable;

        private readonly Dictionary<ChunkKey, byte[]> _chunksToSave;
        private readonly List<List<ChunkKey>> _chunksToLoad; 

        public ChunkHandler(IGame game)
            :base(game, "Chunk")
        {
            _chunksToLoad = new List<List<ChunkKey>>();
            _chunksToSave = new Dictionary<ChunkKey, byte[]>();

            const string filename = "chunkData.bin";
            Saver.TargetFilename = filename;
            Loader.TargetFilename = filename;
        }

        protected override void PerformSave()
        {
            Dictionary<ChunkKey, byte[]> chunksToSave; 
            lock(this)
            {
                chunksToSave = new Dictionary<ChunkKey, byte[]>(_chunksToSave);
                _chunksToSave.Clear();
            }

            Saver.SaveData(chunksToSave);
        }

        protected override void PerformLoad()
        {
            var chunksToLoad = new List<List<ChunkKey>>();
            lock (this)
            {
                chunksToLoad.AddRange(_chunksToLoad);
                _chunksToLoad.Clear();
            }

            var collection = new HashSet<ChunkKey>();
            foreach (var key in chunksToLoad.SelectMany(item => item))
            {
                collection.Add(key);
            }

            Loader.LoadChunks(collection.ToList());
        }

        public override void Init()
        {
            base.Init();

            Loader.OnChunkLoad += ChunkLoaded;
            Loader.OnChunksGenerated += ChunkGenerated;
            Loader.OnChunksUnavailable += ChunksUnavailable;
        }

        private void ChunkLoaded(List<Chunk> chunks)
        {
            if (OnChunkLoad != null)
                OnChunkLoad(chunks);
        }

        private void ChunkGenerated(List<Chunk> chunks)
        {
            if (OnChunksGenerated != null)
                OnChunksGenerated(chunks);
        }

        private void ChunksUnavailable(List<ChunkKey> keys)
        {
            if (OnChunksUnavailable != null)
                OnChunksUnavailable(keys);
        }

        public void LoadChunks(List<ChunkKey> chunkKeys)
        {
            lock (this)
            {
                _chunksToLoad.Add(chunkKeys);
                Monitor.PulseAll(this);
            }
        }

        public void SaveChunks(List<Chunk> chunksToSave)
        {
            var dataToSave = new Dictionary<ChunkKey, byte[]>();
            foreach (var chunk in chunksToSave)
            {
                var stream = new MemoryStream();
                stream.Write(chunk);
                var data = stream.ToArray();
                dataToSave[chunk.Key] = data;

                chunk.ChunkMeshUpdated -= ChunkMeshUpdated;
                chunk.ChunkMeshUpdated += ChunkMeshUpdated;

                chunk.ChunkBlockUpdated -= ChunkBlockUpdated;
                chunk.ChunkBlockUpdated += ChunkBlockUpdated;
            }

            lock (this)
            {
                foreach (var item in dataToSave)
                {
                    _chunksToSave[item.Key] = item.Value;
                }
            }
        }

        private void ChunkMeshUpdated(Chunk item)
        {
            SaveChunks(new List<Chunk>{item});
        }

        private void ChunkBlockUpdated(Chunk item, int x, int y)
        {
            SaveChunks(new List<Chunk>{item});
        }
    }
}

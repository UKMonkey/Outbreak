using System.Collections.Generic;
using System.Linq;
using Vortex.Interface.EntityBase;
using Vortex.Interface.World.Chunks;
using Vortex.Interface.World.Entities;
using Vortex.Interface.World.Triggers;
using Vortex.Interface.World.Wrapper;

namespace Outbreak.Server.World.Providers
{
    /************************************************************************/
    /* Has a list of providers and starts with the first, and then on       */
    /* failure picks the next etc etc until there are none left to try      */
    /************************************************************************/
    public class SerialWorldProvider : WorldProviderWrapper
    {
        private readonly Dictionary<ChunkKey, int> _chunksReported;
        private readonly Dictionary<ChunkKey, int> _entitiesReported;
        private readonly Dictionary<ChunkKey, int> _triggersReported;

        public SerialWorldProvider()
        {
            _chunksReported = new Dictionary<ChunkKey, int>();
            _entitiesReported = new Dictionary<ChunkKey, int>();
            _triggersReported = new Dictionary<ChunkKey, int>();
        }

        public SerialWorldProvider(IEnumerable<IChunkLoader> chunkProviders) : this()
        {
            RegisterChunkEvents(chunkProviders);
        }

        public SerialWorldProvider(IEnumerable<IEntityLoader> entityProviders) : this()
        {
            RegisterEntityEvents(entityProviders);
        }

        public SerialWorldProvider(IEnumerable<ITriggerLoader> providers) : this()
        {
            RegisterTriggerEvents(providers);
        }

        public SerialWorldProvider(IEnumerable<IWorldProvider> providers) : this()
        {
            var providerList = providers.ToList();
            RegisterChunkEvents(providerList);
            RegisterEntityEvents(providerList);
            RegisterTriggerEvents(providerList);
        }

        public void AddChunkLoader(IChunkLoader chunkLoader)
        {
            RegisterChunkEvents(chunkLoader);
        }

        public void AddEntityLoader(IEntityLoader entityLoader)
        {
            RegisterEntityEvents(entityLoader);
        }

        public void AddTriggerLoader(ITriggerLoader triggerLoader)
        {
            RegisterTriggerEvents(triggerLoader);
        }



#region Chunks
        public override void LoadChunks(List<ChunkKey> chunkKeys)
        {
            if (ChunkProviders.Count == 0)
            {
                base.ChunksUnavailable(chunkKeys);
                return;
            }

            foreach (var key in chunkKeys)
            {
                _chunksReported.Add(key, 0);
            }
            ChunkProviders[0].LoadChunks(chunkKeys);
        }

        private void ClearChunkKeys(IEnumerable<ChunkKey> keys)
        {
            foreach (var key in keys)
                _chunksReported.Remove(key);
        }

        protected override void ChunksLoaded(List<Chunk> chunks)
        {
            ClearChunkKeys(chunks.Select(item => item.Key));
            base.ChunksLoaded(chunks);
        }

        protected override void ChunksGenerated(List<Chunk> chunks)
        {
            ClearChunkKeys(chunks.Select(item => item.Key));
            base.ChunksGenerated(chunks);
        }

        protected override void ChunksUnavailable(List<ChunkKey> keys)
        {
            var chunkToNext = new Dictionary<int, List<ChunkKey>>();

            foreach (var key in keys)
            {
                var nextToTry = _chunksReported[key] + 1;
                _chunksReported[key] = nextToTry;

                if (!chunkToNext.ContainsKey(nextToTry))
                    chunkToNext[nextToTry] = new List<ChunkKey>();

                chunkToNext[nextToTry].Add(key);
            }

            foreach (var item in chunkToNext)
            {
                // failed
                if (ChunkProviders.Count == item.Key)
                {
                    ClearChunkKeys(item.Value);
                    base.ChunksUnavailable(item.Value);
                    return;
                }

                ChunkProviders[item.Key].LoadChunks(keys);
                
            }
        }
#endregion


#region Entities
        public override void LoadEntities(ChunkKey area)
        {
            lock (this)
            {
                if (EntityProviders.Count == 0)
                {
                    base.EntitiesUnavailable(new List<ChunkKey> { area });
                    return;
                }
                if (_entitiesReported.ContainsKey(area))
                    return;

                _entitiesReported.Add(area, 0);
                EntityProviders[0].LoadEntities(area);
            }
        }

        public override void LoadEntities(List<ChunkKey> area)
        {
            lock (this)
            {
                foreach (var item in area)
                    LoadEntities(item);
            }
        }

        private void ClearEntityKeys(ChunkKey key)
        {
            _entitiesReported.Remove(key);
        }

        protected override void EntityLoaded(List<Entity> loaded, ChunkKey key)
        {
            ClearEntityKeys(key);
            base.EntityLoaded(loaded, key);
        }

        protected override void EntitiesUnavailable(List<ChunkKey> keys)
        {
            var nextGenerators = new Dictionary<int, List<ChunkKey>>();

            foreach (var key in keys)
            {
                var nextToTry = _entitiesReported[key] + 1;

                // failed
                if (EntityProviders.Count == nextToTry)
                {
                    ClearEntityKeys(key);
                    base.EntitiesUnavailable(keys);
                    return;
                }
                _entitiesReported[key]++;
                if (!nextGenerators.ContainsKey(nextToTry))
                    nextGenerators[nextToTry] = new List<ChunkKey>();
                nextGenerators[nextToTry].Add(key);
            }

            foreach(var generator in nextGenerators)
            {
                EntityProviders[generator.Key].LoadEntities(generator.Value);
            }
        }
#endregion


#region triggers
        public override void LoadTriggers(ChunkKey area)
        {
            if (TriggerProviders.Count == 0)
            {
                base.TriggersUnavailable(new List<ChunkKey> { area });
                return;
            }

            // already loading the thing - so don't worry
            if (_triggersReported.ContainsKey(area))
                return;

            _triggersReported.Add(area, 0);
            TriggerProviders[0].LoadTriggers(area);
        }

        private void ClearTriggerKeys(ChunkKey key)
        {
            _triggersReported.Remove(key);
        }

        protected override void TriggerGenerated(ChunkKey key, List<ITrigger> triggers)
        {
            ClearTriggerKeys(key);
            base.TriggerGenerated(key, triggers);
        }

        protected override void TriggerLoaded(ChunkKey key, List<ITrigger> triggers)
        {
            ClearTriggerKeys(key);
            base.TriggerLoaded(key, triggers);
        }

        protected override void TriggersUnavailable(List<ChunkKey> keys)
        {
            foreach (var key in keys)
            {
                var nextToTry = _triggersReported[key] + 1;

                // failed
                if (TriggerProviders.Count == nextToTry)
                {
                    ClearTriggerKeys(key);
                    base.TriggersUnavailable(keys);
                    return;
                }
                _triggersReported[key]++;

                TriggerProviders[nextToTry].LoadTriggers(key);
            }
        }
#endregion
    }
}

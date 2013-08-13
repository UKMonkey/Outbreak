using System;
using System.Collections.Generic;
using Vortex.Interface.World.Chunks;
using Vortex.Interface.World.Wrapper;
using Psy.Core;

namespace Outbreak.Server.World.Providers
{
    /************************************************************************/
    /* Randomly picks a provider based on the seed and tries to generate    */
    /* chunks as required                                                   */
    /************************************************************************/
    class RandomWorldProvider : WorldProviderWrapper
    {
        private readonly int _seed;
        private readonly List<IWorldProvider> _providers;

        public RandomWorldProvider(List<IWorldProvider> providers, int seed=0)
        {
            _providers = providers;
            if (seed == 0)
                _seed = StaticRng.Random.Next();
            else
                _seed = seed;

            foreach(var item in providers)
            {
                RegisterEvents(item);
            }
        }

        private IWorldProvider GetProvider(ChunkKey key)
        {
            var rand = new Random(key.GetHashCode() * _seed);
            return _providers[rand.Next(0, _providers.Count)];
        }

        public override void LoadChunks(List<ChunkKey> chunkKeys)
        {
            foreach (var chunkKey in chunkKeys)
            {
                var provider = GetProvider(chunkKey);
                provider.LoadChunks(new List<ChunkKey>{chunkKey});
            }
        }

        public override void LoadTriggers(ChunkKey location)
        {
            var provider = GetProvider(location);
            provider.LoadTriggers(location);
        }

        public override void LoadEntities(ChunkKey area)
        {
            var provider = GetProvider(area);
            provider.LoadEntities(area);
        }

        public override void LoadEntities(List<ChunkKey> area)
        {
            foreach (var item in area)
                LoadEntities(item);
        }
    }
}

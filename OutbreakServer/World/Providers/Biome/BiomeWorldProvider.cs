using System;
using System.Collections.Generic;
using System.Linq;
using Outbreak.Items.ItemGenerators;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.World.Chunks;
using Vortex.Interface.World;
using Outbreak.Server.World.Providers.Biome.Buildings;
using Outbreak.Server.World.Providers.Biome.MeshProviders;
using Psy.Core;

namespace Outbreak.Server.World.Providers.Biome
{
    public abstract class BiomeWorldProvider : WorldProviderWrapper
    {
        protected ItemGeneratorDictionary ItemGeneratorDictionary { get; set; }

        /// <summary>
        /// Number of chunks / row & column in a Biome
        /// </summary>
        public virtual int BiomeChunkSize { get { return 20; } }

        /// <summary>
        /// World size of a Biome
        /// </summary>
        protected float BiomeWorldSize { get { return GameServer.Engine.ChunkWorldSize * BiomeChunkSize; } }

        /// <summary>
        /// Seed being used to generate the biome
        /// </summary>
        protected readonly int RandomSeed;

        /// <summary>
        /// Engine so that we can make entities
        /// </summary>
        protected readonly IGameServer GameServer;

        protected BiomeWorldProvider(int randomSeed, IGameServer gameServer, ItemGeneratorDictionary itemGeneratorDictionary)
        {
            ItemGeneratorDictionary = itemGeneratorDictionary;
            RandomSeed = randomSeed;
            GameServer = gameServer;
        }

#region Utils
        public Vector3 ChunkVectorToBiomeVector(BiomeKey biomeKey, ChunkKey key, Vector3 chunkExtra)
        {
            var availableChunks = GetChunksForBiome(biomeKey);
            var bottomLeftKey = availableChunks[0][0];

            var chunkModification = new Vector3((key.X - bottomLeftKey.X) * GameServer.Engine.ChunkWorldSize,
                                               (key.Y - bottomLeftKey.Y) * GameServer.Engine.ChunkWorldSize, 0);
            return chunkExtra + chunkModification;
        }

        public void BiomeVectorToChunkVector(BiomeKey key, Vector3 biomeExtra, out ChunkKey chunkKey, out Vector3 chunkExtra)
        {
            var extraX = Math.Floor((biomeExtra.X - GameServer.Engine.ChunkWorldSize) / GameServer.Engine.ChunkWorldSize) + 1;
            var extraY = Math.Floor((biomeExtra.Y - GameServer.Engine.ChunkWorldSize) / GameServer.Engine.ChunkWorldSize) + 1;

            chunkExtra = new Vector3((float)(biomeExtra.X - extraX * GameServer.Engine.ChunkWorldSize),
                                    (float)(biomeExtra.Y - extraY * GameServer.Engine.ChunkWorldSize),
                                    biomeExtra.Z);

            chunkKey = new ChunkKey((int)(key.X * BiomeChunkSize + extraX),
                                    (int)(key.Y * BiomeChunkSize + extraY));
        }

        public Vector3 BiomeVectorToWorldVector(BiomeKey key, Vector2 biomeExtra)
        {
            return BiomeVectorToWorldVector(key, new Vector3(biomeExtra, 0));
        }

        public Vector3 BiomeVectorToWorldVector(BiomeKey key, Vector3 biomeExtra)
        {
            var x = BiomeWorldSize * key.X + biomeExtra.X;
            var y = BiomeWorldSize * key.Y + biomeExtra.Y;
            return new Vector3(x, y, 0);
        }

        public BiomeKey GetBiomeForChunk(ChunkKey key)
        {
            return new BiomeKey((int)Math.Floor((key.X - BiomeChunkSize)/(float)BiomeChunkSize) + 1,
                                (int)Math.Floor((key.Y - BiomeChunkSize)/(float)BiomeChunkSize) + 1);
        }

        public List<List<ChunkKey>> GetChunksForBiome(BiomeKey key)
        {
            var ret = new List<List<ChunkKey>>();
            for (var y=0; y<BiomeChunkSize; ++y)
            {
                var item = new List<ChunkKey>();
                ret.Add(item);
                for (var x=0; x<BiomeChunkSize; ++x)
                {
                    item.Add(new ChunkKey(key.X * BiomeChunkSize + x, key.Y * BiomeChunkSize + y));
                }
            }

            return ret;
        }

        /**
         * Note that the provider should either create a new random generator for each chunk or avoid doing any
         * randomness, as the keys passed in may be in any order for any biome
         */
        protected IEnumerable<IChunk> GetChunks(BiomeKey biomeKey, IEnumerable<ChunkKey> keys, IEnumerable<BuildingData> buildings, IChunkMeshProvider chunkMeshProvider)
        {
            var ret = new List<MeshOnlyChunk>();
            var buildingList = buildings.ToList();
            foreach (var chunkKey in keys)
            {
                var mesh = GenerateChunkMesh(biomeKey, chunkKey, buildingList, chunkMeshProvider);
                var lights = buildingList.SelectMany(item => item.GetLightsForChunk(chunkKey)).ToList();
                var chunk = new MeshOnlyChunk(chunkKey, mesh, lights);

                ret.Add(chunk);
            }
            return ret;
        }

        protected ChunkMesh GenerateChunkMesh(BiomeKey biomeKey, ChunkKey key, 
            List<BuildingData> buildings, IChunkMeshProvider chunkMeshProvider)
        {
            Rectangle? result;
            var mesh = new ChunkMesh();
            var chunkArea = key.GetWorldArea(GameServer.Engine);

            var providers = new List<IChunkMeshProvider> (buildings);
            providers.Add(chunkMeshProvider);

            var fullProvider = new SimpleCombinedMeshProvider(providers);

            fullProvider.GetMeshesForArea(chunkArea, mesh, out result);

            mesh.Translate(-chunkArea.BottomLeft.AsVector3());

            return mesh;
        }
#endregion


#region Chunks
        protected abstract void GenerateBiomeChunks(BiomeKey biomeKey, List<ChunkKey> expectedChunks);

        public override void LoadChunks(List<ChunkKey> chunkKeys)
        {
            var keys = new Dictionary<BiomeKey, List<ChunkKey>>();
            foreach (var item in chunkKeys)
            {
                var biome = GetBiomeForChunk(item);
                if (!keys.ContainsKey(biome))
                    keys.Add(biome, new List<ChunkKey>());
                keys[biome].Add(item);
            }

            foreach(var item in keys)
                GenerateBiomeChunks(item.Key, item.Value);
        }
#endregion


#region Entities
        protected abstract void GenerateBiomeEntities(BiomeKey biomeKey, List<ChunkKey> expectedChunks);

        public override void LoadEntities(ChunkKey area)
        {
            var key = GetBiomeForChunk(area);
            GenerateBiomeEntities(key, new List<ChunkKey>{area});
        }

        public override void LoadEntities(List<ChunkKey> area)
        {
            var biomeToChunk = new Dictionary<BiomeKey, List<ChunkKey>>();
            foreach (var item in area)
            {
                var key = GetBiomeForChunk(item);
                if (!biomeToChunk.ContainsKey(key))
                    biomeToChunk[key] = new List<ChunkKey>();
                biomeToChunk[key].Add(item);
            }

            foreach (var entry in biomeToChunk)
            {
                GenerateBiomeEntities(entry.Key, entry.Value);
            }
        }
#endregion


#region Triggers
        protected abstract void GenerateBiomeTriggers(BiomeKey biomeKey, List<ChunkKey> expectedChunks);

        public override void LoadTriggers(ChunkKey area)
        {
            var key = GetBiomeForChunk(area);
            GenerateBiomeTriggers(key, new List<ChunkKey>{area});
        }
#endregion
    }
}

using System.Collections.Generic;
using NUnit.Framework;
using Outbreak.Items.ItemGenerators;
using Vortex.Interface;
using Vortex.Interface.World.Chunks;
using Outbreak.Server.World.Providers.Biome;

namespace UnitTests.Daybreak.Server.World.Providers.Biome
{
    public class TestBiomeWorld : BiomeWorldProvider
    {
        public List<ChunkKey> RequestedChunks { get; private set; }
        public List<ChunkKey> RequestedEntities { get; private set; }
        public List<ChunkKey> RequestedTriggers { get; private set; } 

        public TestBiomeWorld(int randomSeed, IGameServer gameServer, ItemGeneratorDictionary itemGeneratorDictionary) 
            : base(randomSeed, gameServer, itemGeneratorDictionary)
        {
            RequestedChunks = new List<ChunkKey>();
            RequestedEntities = new List<ChunkKey>();
            RequestedTriggers = new List<ChunkKey>();
        }

        protected override void GenerateBiomeChunks(BiomeKey biomeKey, List<ChunkKey> expectedChunks)
        {
            foreach (var item in expectedChunks)
                Assert.AreEqual(biomeKey, GetBiomeForChunk(item));

            RequestedChunks.AddRange(expectedChunks);
        }

        protected override void GenerateBiomeEntities(BiomeKey biomeKey, List<ChunkKey> expectedChunks)
        {
            foreach (var item in expectedChunks)
                Assert.AreEqual(biomeKey, GetBiomeForChunk(item));

            RequestedEntities.AddRange(expectedChunks);
        }

        protected override void GenerateBiomeTriggers(BiomeKey biomeKey, List<ChunkKey> expectedChunks)
        {
            foreach (var item in expectedChunks)
                Assert.AreEqual(biomeKey, GetBiomeForChunk(item));
            RequestedTriggers.AddRange(expectedChunks);
        }
    }
}

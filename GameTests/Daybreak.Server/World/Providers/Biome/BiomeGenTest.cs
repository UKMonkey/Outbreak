using System.Linq;
using NUnit.Framework;
using Outbreak.Items.ItemGenerators;
using Outbreak.Server.World.Providers.Biome;

namespace UnitTests.Daybreak.Server.World.Providers.Biome
{
    public class BiomeGenTest
    {
        private void TestBiomeToChunks(BiomeWorldProvider worldProvider, BiomeKey key)
        {
            var chunks = worldProvider.GetChunksForBiome(key);
            Assert.AreEqual(chunks.SelectMany(item => item).Count(), worldProvider.BiomeChunkSize * worldProvider.BiomeChunkSize);
            foreach (var item in chunks.SelectMany(item => item))
            {
                var biome = worldProvider.GetBiomeForChunk(item);
                Assert.AreEqual(key, biome);
            }
        }

        [Test]
        public void TestBiomeUtils()
        {
            var testObject = new TestBiomeWorld(0, new TestGameServer(), new ItemGeneratorDictionary());

            for (var i=-10; i<=10; ++i)
                for (var j=-10; j<=10; ++j)
                    TestBiomeToChunks(testObject, new BiomeKey(i, j));
        }
    }
}

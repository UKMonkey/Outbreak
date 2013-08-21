using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BiomeGen;
using Outbreak.BiomeGen;
using Outbreak.Items.ItemGenerators;
using Outbreak.Server.World.Providers.Biome.Buildings;
using Outbreak.Server.World.Providers.Biome.MeshProviders;
using Outbreak.Server.World.Triggers;
using Psy.Core.Logging;
using SlimMath;
using Vortex.Interface.EntityBase;
using Vortex.Interface.World.Chunks;
using Vortex.Interface.World.Triggers;
using Rectangle = Psy.Core.Rectangle;

namespace Outbreak.Server.World.Providers.Biome
{
    public class TranslatorBiomeWorldProvider : BiomeWorldProvider
    {
        public TranslatorBiomeWorldProvider(int randomSeed, GameServer gameServer, ItemGeneratorDictionary itemGeneratorDictionary)
            : base(randomSeed, gameServer, itemGeneratorDictionary)
        { 
        }

        private const int MinRoomSize = 5;
        private const int MaxRoomSize = 10;

        protected BuildingData GenerateBuilding(Rectangle area, BiomeKey biomeKey, List<ChunkKey> chunksToGenerate, IBuildingGenerator generator)
        {
            var bottomLeft = area.BottomLeft;
            var topRight = area.TopRight;

            Logger.Write(String.Format("Creating building {0} at {1}, {2}", 
                generator.GetBuildingName(), 
                biomeKey, 
                BiomeVectorToWorldVector(biomeKey, area.BottomLeft)),
                LoggerLevel.Debug);

            generator.BottomLeft = bottomLeft;
            generator.MinRoomSize = MinRoomSize;
            generator.MaxRoomSize = MaxRoomSize;
            generator.TopRight = topRight;
            generator.MainEntranceDirection = (float)(Math.PI);
            generator.ChunksToGenerate = chunksToGenerate;

            generator.RandomNumberGenerator = new Random(GetBiomeRandomSeed(biomeKey));

            return generator.GetBuildingData();
        }

        protected List<BuildingData> GetBuildingData(IEnumerable<TranslatorChunkMeshProvider> meshes, 
            BiomeKey biomeKey, List<ChunkKey> chunksToGenerate)
        {
            var ret = new List<BuildingData>();
            foreach (var mesh in meshes)
            {
                if (!mesh.IsBuildingMaterial())
                    continue;

                var buildingGenerator = mesh.GetBuildingGeneratorForMaterial(GameServer, ItemGeneratorDictionary);

                Debug.Assert(buildingGenerator != null);
                if (buildingGenerator == null)
                    continue;

                var buildingArea = mesh.GetFullArea();
                var building = GenerateBuilding(buildingArea, biomeKey, chunksToGenerate, buildingGenerator);
                ret.Add(building);
            }

            return ret;
        }

        protected int GetBiomeRandomSeed(BiomeKey key)
        {
            int a = (int)(2891211 * Math.Sin((RandomSeed + 1) + (key.X * 19) + (key.Y * 51)));
            int b = (int)(1791211 * Math.Cos((RandomSeed + 1) + (key.X * 7) + (key.Y * 11)));
            return a + b;
        }

        protected override void GenerateBiomeChunks(BiomeKey biomeKey, List<ChunkKey> expectedChunks)
        {
            var generator = GetGenerator(biomeKey);

            var chunkMeshProviders = GetChunkMeshProviders(biomeKey, generator).ToList();
            var chunkMeshProvider = GetGroupMeshProvider(chunkMeshProviders);
            var buildings = GetBuildingData(chunkMeshProviders, biomeKey, expectedChunks);

            var chunks = GetChunks(biomeKey, expectedChunks, buildings, chunkMeshProvider).ToList();

            ChunksGenerated(chunks);
        }

        private IChunkMeshProvider GetGroupMeshProvider(IEnumerable<TranslatorChunkMeshProvider> providers)
        {
            var nonBuildingMeshes = providers.Where(x => !x.IsBuildingMaterial());
            var combinedMeshProvider = new SimpleCombinedMeshProvider(nonBuildingMeshes);
            return combinedMeshProvider;
        }

        private IEnumerable<TranslatorChunkMeshProvider> GetChunkMeshProviders(BiomeKey biomeKey, Generator generator)
        {
            var biomeWorldCoordinateOffset = BiomeVectorToWorldVector(biomeKey, new Vector3());
            return generator.Rects.Select(item => new TranslatorChunkMeshProvider(item.Translate(biomeWorldCoordinateOffset)));
        }

        private Generator GetGenerator(BiomeKey biomeKey)
        {
            /* note: we should use the biomeKey in here to determine what generator to use.
             * probably using some perlin noise generator or some such.
            */
            Generator ret;
            var seed = GetBiomeRandomSeed(biomeKey);
            if (Math.Cos(seed) < 0)
                ret = new FarmGenerator(GameServer.Engine.ChunkWorldSize, BiomeChunkSize, 1.0f);
            else
                ret = new SuburbGenerator(GameServer.Engine.ChunkWorldSize, BiomeChunkSize, 1.0f);

            ret.Generate(seed);
            return ret;
        }

        protected override void GenerateBiomeEntities(BiomeKey biomeKey, List<ChunkKey> expectedChunks)
        {
            var generator = GetGenerator(biomeKey);

            var chunkMeshProviders = GetChunkMeshProviders(biomeKey, generator);
            var buildings = GetBuildingData(chunkMeshProviders, biomeKey, expectedChunks);

            var entities = new Dictionary<ChunkKey, List<Entity>>();

            foreach (var wall in buildings.SelectMany(building => building.WallEntities))
            {
                if (!entities.ContainsKey(wall.Key))
                    entities.Add(wall.Key, new List<Entity>());
                entities[wall.Key].AddRange(wall.Value);
            }

            foreach (var extra in buildings.SelectMany(building => building.Entities))
            {
                if (!entities.ContainsKey(extra.Key))
                    entities.Add(extra.Key, new List<Entity>());
                entities[extra.Key].AddRange(extra.Value);
            }

            var emptyEntityList = new List<Entity>();
            foreach (var item in expectedChunks)
            {
                var entitiesGenerated = entities.ContainsKey(item) ? entities[item] : emptyEntityList;
                EntityGenerated(entitiesGenerated, item);
            }
        }

        protected override void GenerateBiomeTriggers(BiomeKey biomeKey, List<ChunkKey> expectedChunks)
        {
            foreach (var chunk in expectedChunks)
            {
                if ((chunk.X + chunk.Y) % 2 != 0)
                    continue;

                var trigger = new ZombieSpawnTrigger(GameServer.Engine, new TriggerKey(chunk, 0));
                TriggerGenerated(chunk, new List<ITrigger> {trigger});
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Outbreak.Items.ItemGenerators;
using Outbreak.Server.World.Providers.Biome.Buildings.Generators;
using SlimMath;
using Vortex.Interface.EntityBase;
using Vortex.Interface.World.Chunks;
using Outbreak.Server.World.Providers.Biome.Buildings;
using Outbreak.Server.World.Providers.Biome.MeshProviders;
using Psy.Core.Logging;

namespace Outbreak.Server.World.Providers.Biome
{
    public class FarmBiomeWorldProvider : BiomeWorldProvider
    {
        private const int MinRoomSize = 5;
        private const int MaxRoomSize = 10;

        public FarmBiomeWorldProvider(int randomSeed, GameServer gameServer, ItemGeneratorDictionary itemGeneratorDictionary) 
            : base(randomSeed, gameServer, itemGeneratorDictionary) { }

        protected override void GenerateBiomeChunks(BiomeKey biomeKey, List<ChunkKey> expectedChunks)
        {
            var buildings = GetBuildingsForBiome(biomeKey);
            var tileProvider = GetTileAreaProvider(biomeKey, buildings);
            var chunks = GetChunks(biomeKey, expectedChunks, buildings, tileProvider);

            ChunksGenerated(chunks.ToList());
        }

        protected override void GenerateBiomeEntities(BiomeKey biomeKey, List<ChunkKey> expectedChunks)
        {
            var entities = new Dictionary<ChunkKey, List<Entity>>();
            var buildings = GetBuildingsForBiome(biomeKey);

            foreach (var wall in buildings.SelectMany(building => building.WallEntities))
            {
                if (!entities.ContainsKey(wall.Key))
                    entities.Add(wall.Key, new List<Entity>());
                entities[wall.Key].AddRange(wall.Value);
            }

            var emptyEntityList = new List<Entity>();
            foreach (var item in expectedChunks)
                EntityGenerated(entities.ContainsKey(item) ? entities[item] : emptyEntityList, item);
        }

        protected override void GenerateBiomeTriggers(BiomeKey biomeKey, List<ChunkKey> expectedChunks)
        {
            //var buildings = GetBuildingsForChunk(key);
            // For now, no triggers, let's keep it simple!
        }


        /**********************/

        protected IChunkMeshProvider GetTileAreaProvider(BiomeKey key, IEnumerable<BuildingData> buildings)
        {
            /*
            var randomisor = new Random((RandomSeed + key.X) * key.Y);
            var ret = new AreaChunkMeshProvider(DefaultTile);
            var paddockCount = randomisor.Next(4, 7);
            var paddockSize = BiomeWorldSize / paddockCount;

            foreach (var building in buildings)
            {
                var area = new Rectangle();
                area.BottomRight = building.Area.BottomRight + new Vector(2, -2);
                area.TopLeft = building.Area.TopLeft + new Vector(-2, 2);

                ret.AddRegion(area, BuildingTile);
            }

            for (var x = 0; x < paddockCount; ++x)
            {
                for (var y = 0; y < paddockCount; ++y)
                {
                    var tileType = randomisor.Next(0, PossibleTiles.Count());
                    var tile = PossibleTiles[tileType];
                    var topLeft     = BiomeVectorToWorldVector(key, new Vector(paddockSize * x, paddockSize * (y+1)));
                    var bottomRight = BiomeVectorToWorldVector(key, new Vector(paddockSize * (x+1), paddockSize * y));
                    ret.AddRegion(topLeft, bottomRight, tile);
                }
            }

            return ret;
            */
            return null;
        }

        protected bool IsBuildingCornerInBuilding(BuildingData buildingA, BuildingData buildingB)
        {
            var areaA = buildingA.Area;
            var areaB = buildingB.Area;
            var result = areaA.Intersects(areaB);
            return result;
        }

        // make sure that this building isn't in another building, and that another building isn't in it.
        // make sure that this building fits completely within the biome
        protected bool IsValidBuilding(BiomeKey key, IEnumerable<BuildingData> otherBuildings, BuildingData building)
        {
            foreach (var otherBuilding in otherBuildings)
            {
                if (IsBuildingCornerInBuilding(building, otherBuilding) ||
                    IsBuildingCornerInBuilding(otherBuilding, building))
                    return false;
            }

            var bottomLeftOfBiome = BiomeVectorToWorldVector(key, new Vector3(0, 0, 0));
            var topRightOfBiome = bottomLeftOfBiome + new Vector3(BiomeWorldSize, BiomeWorldSize, 0);

            if (topRightOfBiome.X < building.Area.BottomRight.X ||
                topRightOfBiome.Y < building.Area.TopLeft.Y ||
                bottomLeftOfBiome.X > building.Area.TopLeft.X ||
                bottomLeftOfBiome.Y > building.Area.BottomRight.Y)
                return false;

            return true;
        }

        protected BuildingData GetRandomBuilding(BiomeKey key, Random randomisor, float size, List<BuildingData> otherBuildings)
        {
            var bottomLeft = new Vector2(BiomeWorldSize * key.X, BiomeWorldSize * key.Y) +
                             new Vector2((float)(randomisor.NextDouble() * BiomeWorldSize - size),
                                        (float)(randomisor.NextDouble() * BiomeWorldSize - size));

            IBuildingGenerator houseGenerator = new SmallHouseGenerator(GameServer, ItemGeneratorDictionary);
            houseGenerator.BottomLeft = bottomLeft;
            houseGenerator.MinRoomSize = MinRoomSize;
            houseGenerator.MaxRoomSize = MaxRoomSize;
            houseGenerator.TopRight = bottomLeft + new Vector2(size, size);
            houseGenerator.RandomNumberGenerator = randomisor;
            houseGenerator.MainEntranceDirection = (float)(randomisor.NextDouble()*2*Math.PI);

            var building = houseGenerator.GetBuildingData();

            if (!IsValidBuilding(key, otherBuildings, building))
                return GetRandomBuilding(key, randomisor, size, otherBuildings);
            return building;
        }

        protected List<BuildingData> GetBuildingsForBiome(BiomeKey key)
        {
            var ret = new List<BuildingData>();
            var randomisor = new Random((RandomSeed + key.X) * key.Y);
            var barnCount = randomisor.Next(1, 4);

            var houseSize = (float)(32 * (1 + randomisor.NextDouble() / 4));
            ret.Add(GetRandomBuilding(key, randomisor, houseSize, ret));

            for (var i = 0; i < barnCount; ++i)
            {
                var barnSize = (float)(14 * (1 + randomisor.NextDouble() / 2));
                ret.Add(GetRandomBuilding(key, randomisor, barnSize, ret));
            }

            foreach (var buildingData in ret)
            {
                Logger.Write(string.Format("Building to be located at {0} - {1}", buildingData.Area.BottomLeft, buildingData.Area.TopRight));
            }

            return ret;
        }
    }
}

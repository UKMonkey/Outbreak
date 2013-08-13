using System;
using System.Collections.Generic;
using System.Linq;
using Outbreak.Items;
using Outbreak.Items.ItemGenerators;
using Outbreak.Server.Entities;
using Outbreak.Server.World.Providers.Biome.Buildings.Generators;
using SlimMath;
using Vortex.Interface.EntityBase;
using Vortex.Interface.World;
using Vortex.Interface.World.Chunks;
using Vortex.Interface.World.Triggers;
using Outbreak.Entities;
using Outbreak.Enums;
using Outbreak.Server.World.Providers.Biome.Buildings;
using Outbreak.Server.World.Providers.Biome.MeshProviders;
using Outbreak.Server.World.Triggers;
using Psy.Core;

namespace Outbreak.Server.World.Providers.Biome
{
    public class DemoBuildingBiomeWorld : BiomeWorldProvider
    {
        private const int MinRoomSize = 5;
        private const int MaxRoomSize = 10;

        public DemoBuildingBiomeWorld(int randomSeed, GameServer gameServer, ItemGeneratorDictionary itemGeneratorDictionary) 
            : base(randomSeed, gameServer, itemGeneratorDictionary ) { }

        protected override void GenerateBiomeChunks(BiomeKey biomeKey, List<ChunkKey> expectedChunks)
        {
            var buildings = GetBuildingsForBiome(biomeKey, expectedChunks);
            var tileProvider = GetTileAreaProvider(biomeKey);
            var chunks = GetChunks(biomeKey, expectedChunks, buildings, tileProvider).ToList();

            foreach (var chunk in chunks)
            {
                var position = new Vector3(Chunk.ChunkWorldSize / 2, Chunk.ChunkWorldSize / 2, -1.8f);
                var light = new Light(position, 2f,
                                 new Color4(0.4f, 0.4f, 0.4f, 0.4f));
                chunk.Lights.Add(light);
            }

            ChunksGenerated(chunks);
        }

        protected override void GenerateBiomeEntities(BiomeKey biomeKey, List<ChunkKey> expectedChunks)
        {
            var entities = new Dictionary<ChunkKey, List<Entity>>();
            var buildings = GetBuildingsForBiome(biomeKey, expectedChunks);

            // for debugging
            entities.Add(new ChunkKey(0, 0), new List<Entity>());
            entities.Add(new ChunkKey(-2, -2), new List<Entity>());

            foreach (var wall in buildings.SelectMany(building => building.WallEntities))
            {
                if (!entities.ContainsKey(wall.Key))
                    entities.Add(wall.Key, new List<Entity>());
                entities[wall.Key].AddRange(wall.Value);
            }

            // for debugging
            var shotgunItem = ItemGeneratorDictionary.Get(ItemTypeEnum.Shotgun).Generate();
            var weapon = GameServer.EntityFactory.Get((short)EntityTypeEnum.InventoryItem);
            weapon.SetInventoryItem(shotgunItem);
            weapon.SetPosition(GameServer.Engine.ChunkVectorToWorldVector(new ChunkKey(0,0), new Vector3(3f, 3f, 0)));
            entities[new ChunkKey(0, 0)].Add(weapon);

            // more debugging
            var shotgunAmmoItem = ItemGeneratorDictionary.Get(ItemTypeEnum.ShotgunAmmo).Generate();
            var ammo = GameServer.EntityFactory.Get((short)EntityTypeEnum.InventoryItem);
            ammo.SetInventoryItem(shotgunAmmoItem);
            ammo.SetPosition(GameServer.Engine.ChunkVectorToWorldVector(new ChunkKey(0, 0), new Vector3(3f, 5f, 0)));
            entities[new ChunkKey(0, 0)].Add(ammo);

            // for debugging
            var pistolItem = ItemGeneratorDictionary.Get(ItemTypeEnum.Pistol).Generate();
            weapon = GameServer.EntityFactory.Get((short)EntityTypeEnum.InventoryItem);
            weapon.SetInventoryItem(pistolItem);
            weapon.SetPosition(GameServer.Engine.ChunkVectorToWorldVector(new ChunkKey(0, 0), new Vector3(6f, 3f, 0)));
            entities[new ChunkKey(0, 0)].Add(weapon);

            // more debuggings
            var ammoItem = ItemGeneratorDictionary.Get(ItemTypeEnum.PistolAmmo).Generate();
            ammo = GameServer.EntityFactory.Get((short)EntityTypeEnum.InventoryItem);
            ammo.SetInventoryItem(ammoItem);
            ammo.SetPosition(GameServer.Engine.ChunkVectorToWorldVector(new ChunkKey(0, 0), new Vector3(6f, 5f, 0)));
            entities[new ChunkKey(0, 0)].Add(ammo);

            var firstAidItem = ItemGeneratorDictionary.Get(ItemTypeEnum.FirstAidPack).Generate();
            var firstAid = GameServer.EntityFactory.Get((short)EntityTypeEnum.InventoryItem);
            firstAid.SetInventoryItem(firstAidItem);
            firstAid.SetPosition(GameServer.Engine.ChunkVectorToWorldVector(new ChunkKey(0, 0), new Vector3(9f, 5f, 0)));
            entities[new ChunkKey(0, 0)].Add(firstAid);

            if (expectedChunks.Contains(new ChunkKey(-2, -2)))
            {
                var zombie = GameServer.EntityFactory.Get((short) EntityTypeEnum.Zombie);
                zombie.SetPosition(new Vector3(-1.6f*Chunk.ChunkWorldSize, -1.6f*Chunk.ChunkWorldSize, 0));
                entities[new ChunkKey(-2, -2)].Add(zombie);
            }

            foreach (var room in buildings.SelectMany(building => building.Rooms))
            {
                var entitiesToInclude = room.Entities.Where(item => expectedChunks.Contains(item.Key));
                foreach (var entityList in entitiesToInclude)
                {
                    if (!entities.ContainsKey(entityList.Key))
                        entities.Add(entityList.Key, new List<Entity>());
                    entities[entityList.Key].AddRange(entityList.Value);
                }
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
            foreach (var chunkKey in expectedChunks)
            {
                var trigger = new ZombieSpawnTrigger(GameServer.Engine, new TriggerKey(chunkKey, 0));
                TriggerGenerated(chunkKey, new List<ITrigger> { trigger });
            }
        }


        /***********/

        protected IChunkMeshProvider GetTileAreaProvider(BiomeKey key)
        {
            var chunks = GetChunksForBiome(key);
            var list = new List<IChunkMeshProvider>();
            foreach (var chunk in chunks.SelectMany(item => item))
            {
                var material = ((chunk.X + chunk.Y)%2 == 0)
                                   ? MaterialType.Grassland
                                   : MaterialType.Wall1;
                list.Add(new AreaChunkMeshProvider(GameServer.Engine, chunk, material));
            }

            return new CombinedMeshProvider(list);
        }

        protected bool IsBuildingCornerInBuilding(BuildingData buildingA, BuildingData buildingB)
        {
            var areaA = buildingA.Area;
            var areaB = buildingB.Area;
            var result = areaA.Intersects(areaB);
            return result;
        }

        protected bool IsValidBuilding(BiomeKey key, IEnumerable<BuildingData> otherBuildings, BuildingData building)
        {
            foreach (var otherBuilding in otherBuildings)
            {
                if (IsBuildingCornerInBuilding(building, otherBuilding) ||
                    IsBuildingCornerInBuilding(otherBuilding, building))
                    return false;
            }

            var bottomLeftOfBiome = new Vector3(0, 0, 0);
            var topRightOfBiome = bottomLeftOfBiome + new Vector3(BiomeWorldSize, BiomeWorldSize, 0);

            if (topRightOfBiome.X < building.Area.BottomRight.X ||
                topRightOfBiome.Y < building.Area.TopLeft.Y ||
                bottomLeftOfBiome.X > building.Area.TopLeft.X ||
                bottomLeftOfBiome.Y > building.Area.BottomRight.Y)
                return false;

            return true;
        }

        protected BuildingData GetRandomBuilding(BiomeKey key, Random randomisor, float size, List<BuildingData> otherBuildings, List<ChunkKey> expectedChunks)
        {
            // bottom left of the biome
            //var bottomLeft = new Vector(BiomeWorldSize*key.X, BiomeWorldSize*key.Y) + new Vector(Tile.Size*5, Tile.Size*5);
            
            // top right of the biome
            //var bottomLeft = new Vector(BiomeWorldSize, BiomeWorldSize) - new Vector(Tile.Size * 5 + size, Tile.Size * 5 + size);
            var bottomLeft = new Vector3(BiomeWorldSize, BiomeWorldSize, 0) - new Vector3(32, 32, 0);

            IBuildingGenerator houseGenerator = new SmallHouseGenerator(GameServer, ItemGeneratorDictionary);
            houseGenerator.BottomLeft = bottomLeft.AsVector2();
            houseGenerator.MinRoomSize = MinRoomSize;
            houseGenerator.MaxRoomSize = MaxRoomSize;
            houseGenerator.TopRight = (bottomLeft + new Vector3(size, size, 0)).AsVector2();
            houseGenerator.RandomNumberGenerator = randomisor;
            
            houseGenerator.MainEntranceDirection = (float)(randomisor.NextDouble() * 2 * Math.PI);
            houseGenerator.ChunksToGenerate = expectedChunks;

            var building = houseGenerator.GetBuildingData();

            if (!IsValidBuilding(key, otherBuildings, building))
                return GetRandomBuilding(key, randomisor, size, otherBuildings, expectedChunks);
            return building;
        }

        protected List<BuildingData> GetBuildingsForBiome(BiomeKey key, List<ChunkKey> expectedChunks)
        {
            var ret = new List<BuildingData>();

            var randomisor = new Random((RandomSeed + key.X) * key.Y);

            var houseSize = (float)(15 * (1 + randomisor.NextDouble() / 4));
            ret.Add(GetRandomBuilding(key, randomisor, houseSize, ret, expectedChunks));

            return ret;
        }
    }
}

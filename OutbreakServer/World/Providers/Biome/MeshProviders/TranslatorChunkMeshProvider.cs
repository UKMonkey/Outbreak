using BiomeGen.Maths.Geometry;
using Outbreak.BiomeGen;
using Outbreak.Enums;
using Outbreak.Items.ItemGenerators;
using Outbreak.Server.World.Providers.Biome.Buildings;
using Outbreak.Server.World.Providers.Biome.Buildings.Generators;
using Vortex.Interface;
using Rectangle = Psy.Core.Rectangle;

namespace Outbreak.Server.World.Providers.Biome.MeshProviders
{
    public class TranslatorChunkMeshProvider : IntersectMeshProvider
    {
        private readonly BiomeRect _biomeRect;
        public override bool FullMeshProvider { get { return false; } }

        public TranslatorChunkMeshProvider(BiomeRect biomeRect)
        {
            _biomeRect = biomeRect;
        }

        protected override MaterialType GetMaterial()
        {
            var colour = _biomeRect.Colour;

            if (colour == Legend.FieldColor)
                return MaterialType.Crops;
            if (colour == Legend.Grass)
                return MaterialType.Grassland;
            if (colour == Legend.DrivewayColor)
                return MaterialType.Wall1;
            if (colour == Legend.MainRoadColor)
                return MaterialType.Road;
            if (colour == Legend.JunctionColor)
                return MaterialType.Road;

            return MaterialType.Debug;
        }

        public bool IsBuildingMaterial()
        {
            var colour = _biomeRect.Colour;

            if (colour == Legend.HouseColor)
                return true;
            if (colour == Legend.CornerShopColor)
                return true;

            return false;
        }

        public IBuildingGenerator GetBuildingGeneratorForMaterial(IGameServer server, ItemGeneratorDictionary itemDict)
        {
            if (_biomeRect.Colour == Legend.HouseColor)
                return new SmallHouseGenerator(server, itemDict);
            if (_biomeRect.Colour == Legend.CornerShopColor)
                return new CornerShopGenerator(server, itemDict);

            return null;
        }

        public override Rectangle GetFullArea()
        {
            return _biomeRect;
        }
    }
}

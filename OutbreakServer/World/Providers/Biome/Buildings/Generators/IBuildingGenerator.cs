using System;
using System.Collections.Generic;
using SlimMath;
using Vortex.Interface.World.Chunks;

namespace Outbreak.Server.World.Providers.Biome.Buildings
{
    public interface IBuildingGenerator
    {
        // method of creating random numbers
        Random RandomNumberGenerator { get; set; }

        // min/max sizes of the rooms to be generated
        int MinRoomSize { get; set; }
        int MaxRoomSize { get; set; }

        //float MinPadding { get; set; }
        //float MaxPadding { get; set; }

        // area allowed to be used for the building
        Vector2 BottomLeft { get; set; }
        Vector2 TopRight { get; set; }

        // direction of the main entrace - 0 being up
        float MainEntranceDirection { get; set; }

        // chunks that should be generated in the building
        List<ChunkKey> ChunksToGenerate { get; set; }

        // returns a random name for the building based on the type
        // eg Fish Place, Joe's Goods or Some Palace
        String GetBuildingName();

        // generates the building
        BuildingData GetBuildingData();
    }
}

using System;
using System.Collections.Generic;
using Vortex.Interface.EntityBase;

namespace Outbreak.Server.World.Providers.Biome.Buildings.Rooms
{
    public interface IClutterGenerator
    {
        /** returns entities that will be good clutter for a room of the given type
         */
        IEnumerable<EntitySpawnData> GenerateClutter(RoomData room, Random randomiser);

        /** returns if this generator can generate data for the given room.
         */
        bool CanGenerateForRoom(RoomType type);
    }
}

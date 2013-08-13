using System;
using System.Collections.Generic;

namespace Outbreak.Server.World.Providers.Biome.Buildings.Rooms
{
    public class GroupGenerator: IClutterGenerator
    {
        private readonly Dictionary<RoomType, List<IClutterGenerator>> _generators;

        public GroupGenerator()
        {
            _generators = new Dictionary<RoomType, List<IClutterGenerator>>();
        }

        public IEnumerable<EntitySpawnData> GenerateClutter(RoomData room, Random randomiser)
        {
            var generators = _generators[room.RoomType];
            var id = randomiser.Next(0, generators.Count - 1);

            return generators[id].GenerateClutter(room, randomiser);
        }

        public bool CanGenerateForRoom(RoomType type)
        {
            return _generators.ContainsKey(type);
        }

        public void RegisterGenerator(IClutterGenerator generator)
        {
            var values = Enum.GetValues(typeof (RoomType));
            foreach (RoomType roomType in values)
            {
                if (!generator.CanGenerateForRoom(roomType))
                    continue;

                if (!_generators.ContainsKey(roomType))
                    _generators[roomType] = new List<IClutterGenerator>();

                _generators[roomType].Add(generator);
            }
        }
    }
}

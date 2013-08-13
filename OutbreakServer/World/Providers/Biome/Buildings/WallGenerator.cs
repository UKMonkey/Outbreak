using System;
using System.Linq;
using System.Collections.Generic;
using Outbreak.Entities.Properties;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.World.Chunks;
using Outbreak.Entities;
using Psy.Core;
using EntityTypeEnum = Outbreak.Entities.EntityTypeEnum;

namespace Outbreak.Server.World.Providers.Biome.Buildings
{
    /// <summary>
    /// This generator will create doors in a building so that all rooms are accessable
    /// All hallways will be connected to any rooms connected to them
    /// All rooms not connected to a hallway will be connected to a room next to them
    /// note that if there is a room that is more than 1 hop away from a hallway
    /// it will not be accessable, and may make the adjoining room also unaccessable
    /// </summary>
    public class WallGenerator : IWallGenerator
    {
        private readonly IServer _engine;
        private readonly Random _randomGenerator;
        private readonly HashSet<ChunkKey> _chunkKeys;
        private readonly EntityTypeEnum _wallType;
        private readonly EntityTypeEnum _doorType;
        private readonly HashSet<RoomType> _externalDoorRoom;

        public WallGenerator(IServer engine, Random generator, IEnumerable<ChunkKey> chunkKeys)
        {
            _engine = engine;
            _randomGenerator = generator;

            _wallType = EntityTypeEnum.BasicWall;
            _doorType = EntityTypeEnum.BasicDoor;

            _externalDoorRoom = new HashSet<RoomType>{RoomType.Hallway, RoomType.ShopFloor, RoomType.StoreRoom};

            _chunkKeys = new HashSet<ChunkKey>();
            foreach (var key in chunkKeys)
                _chunkKeys.Add(key);
        }

        public IEnumerable<KeyValuePair<ChunkKey, List<Entity>>> GenerateWalls(BuildingData data)
        {
            var walls = data.Walls;

            var ret = new Dictionary<ChunkKey, List<Entity>>();
            var wallsProcessed = new HashSet<int>();

            foreach (var wall in walls)
            {
                GetDoors(wall, data, wallsProcessed);
                AddItemsToDictionary(GenerateWall(wall), ret);
            }
            return ret;
        }

#region DoorGeneration
            // will generate doors for any number of rooms, but will ensure that doors for the wall
            // required are available.
        private void GetDoors(WallData wall, BuildingData data, ISet<int> wallsProcessed)
        {
            // skip if this wall has already been processed
            if (wallsProcessed.Contains(wall.Id))
                return;

            wallsProcessed.Add(wall.Id);

            // Add a door if
            // 1.  the room should have an outside door, and the wall is external (has no adjacent walls)
            if (_externalDoorRoom.Contains(wall.Room.RoomType) && wall.IsExternal)
            {
                // Only add the door though if it's the largest external wall for that room
                foreach (var otherWall in wall.Room.Walls.Where(item => item.IsExternal))
                {
                    if (otherWall.Id == wall.Id)
                        continue;

                    if (otherWall.Size > wall.Size)
                        return;

                    // pass, we use the other wall
                    if (otherWall.Size == wall.Size
                        && otherWall.Id > wall.Id)
                        return;
                }
                GenerateExternalDoor(wall);
                return;
            }

            // 2.  There are linked rooms, one of which is a hallway (or this is a hallway),
            //     add a door to the hallway and its linked rooms
            //     infact, we'll just generate all the doors for the hallway when we do this!
            var hallways = wall.AdjacentWalls.Where(item =>
                item.Room.RoomType == RoomType.Hallway && 
                !wallsProcessed.Contains(item.Id)).ToList();

            foreach (var hallway in hallways)
                wallsProcessed.Add(hallway.Id);

            if (wall.Room.RoomType == RoomType.Hallway && !wall.IsExternal)
            {
                GenerateHallwayDoors(new List<WallData> { wall });
                return;
            }


            if (hallways.Count > 0)
            {
                GenerateHallwayDoors(hallways);
                return;
            }

            // 3.  There are linked rooms, none are a hallway, add a door to wall with the largest overlap
            //     in the room - that may or may not be on this wall!
            var anyConnectingHallways = wall.Room.AjoiningRooms.Any(item => item.RoomType == RoomType.Hallway);
            if (anyConnectingHallways == false)
            {
                GenerateLinkingDoor(wall, data);
            }
        }

        protected void GenerateHallwayDoors(List<WallData> hallways)
        {
            foreach (var wall in hallways)
            {
                foreach (var matchingWall in wall.AdjacentWalls)
                {
                    var overlap = wall.GetOverlapSection(matchingWall);
                    GenerateInternalDoor(wall, overlap);
                }
            }
        }

        protected void GenerateHallwayDoors(WallData hallway)
        {
            foreach (var item in hallway.AdjacentWalls)
                GenerateInternalDoor(hallway, hallway.GetOverlapSection(item));
        }

        protected void GenerateLinkingDoor(WallData wall, BuildingData data)
        {
            var room = wall.Room;
            var targetData = wall.GetLargestOverlapSection();

            if (!targetData.HasValue)
                return;

            foreach (var otherWall in room.Walls.Where(item => item.Id != wall.Id))
            {
                var overlap = otherWall.GetLargestOverlapSection();
                if (!overlap.HasValue)
                    continue;

                if (targetData.Value.Size < overlap.Value.Size)
                    return;

                if (targetData.Value.Size == overlap.Value.Size &&
                    otherWall.Id < wall.Id)
                    return;
            }

            GenerateInternalDoor(wall, targetData.Value);
        }

        protected void GenerateExternalDoor(WallData wall)
        {
            var baseStep = _randomGenerator.Next(1, wall.Size - 2);
            wall.DoorPositions.Add(baseStep);
            wall.DoorPositions.Add(baseStep+1);
        }

        protected void GenerateInternalDoor(WallData wall, WallOverlapData bounds)
        {
            var isValid = true;
            int baseStep;

            do
            {
                if (bounds.Size < 2)
                    return;

                isValid = true;

                baseStep = _randomGenerator.Next(bounds.StartModifier + 1,
                    bounds.StartModifier + bounds.Size - 1);

                for (int i = 0; i < wall.AdjacentWalls.Count && isValid; ++i)
                {
                    var doorAPosition = GetDoorPosition(wall, baseStep, wall.AdjacentWalls[i]);
                    var doorBPosition = GetDoorPosition(wall, baseStep + 1, wall.AdjacentWalls[i]);

                    isValid &= IsValidDoor(wall.AdjacentWalls[i], doorAPosition);
                    isValid &= IsValidDoor(wall.AdjacentWalls[i], doorBPosition);
                }
            }
            while (!isValid);

            wall.DoorPositions.Add(baseStep);
            wall.DoorPositions.Add(baseStep + 1);

            foreach (var adjWall in wall.AdjacentWalls)
            {
                adjWall.DoorPositions.Add(GetDoorPosition(wall, baseStep, adjWall));
                adjWall.DoorPositions.Add(GetDoorPosition(wall, baseStep+1, adjWall));
            }
        }

        protected int GetDoorPosition(WallData wallWithDoor, int doorPosition, WallData otherWall)
        {
            var distance = (int)Math.Round(wallWithDoor.StartPosition.Distance(otherWall.StartPosition));
            return distance - doorPosition;
        }

        protected bool IsValidDoor(WallData wall, int doorPosition)
        {
            if (doorPosition > wall.Size)
                return true;
            if (doorPosition < 0)
                return true;

            if (wall.DoorPositions.Contains(doorPosition) ||
                wall.DoorPositions.Contains(doorPosition + 1) ||
                wall.DoorPositions.Contains(doorPosition - 1))
                return false;

            return true;
        }
#endregion

        protected void PerformWallSort(WallData wall, List<WallData> adjacent)
        {
            if (Math.Abs(wall.Direction.X) > 0.1)
            {
                adjacent.Sort((a, b) =>
                              (int)(wall.Direction.X * a.StartPosition.X.CompareTo(b.StartPosition.X)));
            }
            else
            {
                adjacent.Sort((a, b) =>
                              (int)(wall.Direction.Y * a.StartPosition.Y.CompareTo(b.StartPosition.Y)));
            }            
        }

        protected int GetUniqueSection(int startModifier, WallData baseWall, WallData adjacentWall, List<WallData> uniques)
        {
            bool invalidEnd;
            var start = baseWall.StartPosition + startModifier * baseWall.Direction;
            var endPoint = VectorExtensions.GetClosestEnd(start, adjacentWall.EndPosition, baseWall.EndPosition,
                                                baseWall.Direction, out invalidEnd);

            var size = (int)Math.Round(endPoint.Distance(start));

            if (!invalidEnd && size > 0)
            {
                var toAdd = new WallData(start, baseWall.Direction, size, baseWall.Room);
                foreach (var doorPos in baseWall.DoorPositions)
                    toAdd.DoorPositions.Add(doorPos - startModifier);

                uniques.Add(toAdd);
                return size;
            }

            return 0;
        }

        protected int GetDuplicateSection(int startModifier, WallData baseWall, WallData adjacentWall, List<WallData> duplicates)
        {
            bool invalidEnd;
            var start = baseWall.StartPosition + startModifier * baseWall.Direction;
            var endPoint = VectorExtensions.GetClosestEnd(start, adjacentWall.StartPosition, 
                                baseWall.EndPosition, baseWall.Direction, out invalidEnd);

            var size = (int)Math.Round(endPoint.Distance(start));

            if (size > 0)
            {
                var toAdd = new WallData(start, baseWall.Direction, size, baseWall.Room);

                foreach (var doorPos in baseWall.DoorPositions)
                    toAdd.DoorPositions.Add(doorPos - startModifier);

                duplicates.Add(toAdd);
                return size;
            }

            return 0;
        }

        protected void GetSegments(WallData wall, out List<WallData> uniqueSegs, out List<WallData> dupSegs)
        {
            uniqueSegs = new List<WallData>();
            dupSegs = new List<WallData>();

            var adjacent = new List<WallData>(wall.AdjacentWalls);

            if (adjacent.Count == 0)
            {
                uniqueSegs.Add(wall);
                return;
            }

            PerformWallSort(wall, adjacent);
           
            var startModifier = 0;
            
            for (int i=0; i<adjacent.Count; ++i)
            {
                var otherWall = adjacent[i];

                startModifier += GetUniqueSection(startModifier, wall, otherWall, uniqueSegs);
                startModifier += GetDuplicateSection(startModifier, wall, otherWall, dupSegs);
            }

            if (startModifier < wall.Size)
            {
                var toAdd = new WallData((wall.StartPosition + startModifier * wall.Direction), wall.Direction, wall.Size - startModifier, wall.Room);
                foreach (var doorPos in wall.DoorPositions)
                    toAdd.DoorPositions.Add(doorPos - startModifier);
                uniqueSegs.Add(toAdd);
            }
        }

        // split the wall into segments based on the adjacent walls
        // generate the segments based on their direction and if there's an adjacent wall
        protected IEnumerable<KeyValuePair<ChunkKey, List<Entity>>> GenerateWall(WallData wall)
        {
            List<WallData> uniqueSegments;
            List<WallData> duplicateSegments;

            GetSegments(wall, out uniqueSegments, out duplicateSegments);

            var result = new Dictionary<ChunkKey, List<Entity>>();
            var segments = new List<WallData>(uniqueSegments);

            if (wall.Direction.X > 0 ||
                (wall.Direction.X >= 0 && wall.Direction.Y > 0))
            {
                segments.AddRange(duplicateSegments);
            }

            foreach (var segment in segments)
            {
                AddItemsToDictionary(
                    MakeWall(segment.StartPosition, segment.Direction, segment.Size, segment.DoorPositions),
                    result);
            }

            return result;
        }

        // calls where count <= 0 have no effect are used...
        protected IEnumerable<KeyValuePair<ChunkKey, List<Entity>>> MakeWall(Vector3 start, Vector3 direction, int count, List<int> doors)
        {
            var ret = new Dictionary<ChunkKey, List<Entity>>();
            var dirZplanAngle = direction.ZPlaneAngle();
            var rot = (float)(dirZplanAngle + (Math.PI / 2));

            for (var i = 0; i < count; ++i)
            {
                if (doors.Contains(i))
                {
                    var position = start + i * direction;
                    var closedDirection = rot;

                    if (doors.Contains(i - 1))
                    {
                        position += direction;
                    }
                    else
                    {
                        closedDirection += (float)(Math.PI);
                    }

                    var door = AddEntityToDictionary(_doorType, position, dirZplanAngle, ret);
                    if (door != null)
                    {
                        door.SetDoorIsOpen(true);
                        var openDirection = door.GetRotation();
                        door.SetDoorOpenAngle(openDirection);
                        door.SetDoorClosedAngle(closedDirection);
                    }
                }
                else
                {
                    var position = start + (float)(i + 0.5) * direction;
                    AddEntityToDictionary(_wallType, position, rot, ret);   
                }
            }
            return ret;
        }

        // takes an item with its position in biome coords, and 
        // 1. puts the item into world coords
        // 2. adds the item into the dictionary
        private Entity AddEntityToDictionary(EntityTypeEnum type, Vector3 entityPosision, float entityRotation, IDictionary<ChunkKey, List<Entity>> data)
        {
            ChunkKey chunkKey;
            Vector3 chunkVector;

            _engine.GetChunkVectorForWorldVector(entityPosision, out chunkKey, out chunkVector);
            if (!_chunkKeys.Contains(chunkKey))
                return null;

            var item = _engine.EntityFactory.Get(type);
            item.SetPosition(entityPosision);
            item.SetRotation(entityRotation);

            if (!data.ContainsKey(chunkKey))
                data.Add(chunkKey, new List<Entity>());
            data[chunkKey].Add(item);
            return item;
        }

        private void AddItemsToDictionary(IEnumerable<KeyValuePair<ChunkKey, List<Entity>>> items, Dictionary<ChunkKey, List<Entity>> data)
        {
            foreach (var item in items)
            {
                if (!data.ContainsKey(item.Key))
                {
                    data.Add(item.Key, item.Value);
                }
                else
                {
                    data[item.Key].AddRange(item.Value);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Outbreak.Server.World.Providers.Biome.MeshProviders;
using SlimMath;
using Vortex.Interface.EntityBase;
using Outbreak.Enums;
using Psy.Core;
using Vortex.Interface.World.Chunks;
using Vortex.Interface.World;

namespace Outbreak.Server.World.Providers.Biome.Buildings
{
    /************************************************************************/
    /* The building data object contains information about a building in    */
    /* world coords.  It provides the ability to establish if a given tile  */
    /* in a given chunk is inside the building or not.                      */
    /* TODO - While it only provides wall information now, it may need to   */
    /* provide additional data eg cupboards, doors etc                      */
    /* May need to re-examine this if we have, since this expects all z     */
    /* values of the walls to be 0                                          */
    /************************************************************************/
    public class BuildingData : IChunkMeshProvider
    {
        public bool FullMeshProvider { get { return false; } }

        public Rectangle Area;

        private readonly List<RoomData> _rooms;
        public IEnumerable<RoomData> Rooms
        { get { return _rooms; } }

        private List<WallData> _walls;
        public List<WallData> Walls
        {
            get
            {
                if (_walls == null)
                    _walls = GenerateWalls();
                return _walls;
            }
        }

        public IEnumerable<KeyValuePair<ChunkKey, List<Entity>>> WallEntities
        { get { return _wallGenerator.GenerateWalls(this); } }

        public IEnumerable<KeyValuePair<ChunkKey, List<Entity>>> Entities
        { get { return CombineData(_rooms.SelectMany(item => item.Entities)); } }

        public IEnumerable<KeyValuePair<ChunkKey, List<ILight>>> Lights
        { get { return CombineData(_rooms.SelectMany(item => item.Lights)); } }

        private readonly IWallGenerator _wallGenerator;

        public BuildingData(IWallGenerator wallGenerator)
        {
            _rooms = new List<RoomData>();
            _wallGenerator = wallGenerator;
        }

        public void GetMeshesForArea(Rectangle area, ChunkMesh mesh, out Rectangle? meshedArea)
        {
            var provider = new SimpleCombinedMeshProvider(Rooms);
            provider.GetMeshesForArea(area, mesh, out meshedArea);
        }

        private static IEnumerable<KeyValuePair<TKeyType, List<ValueType>>>
            CombineData<TKeyType, ValueType>(IEnumerable<KeyValuePair<TKeyType, List<ValueType>>> data)
        {
            var ret = new Dictionary<TKeyType, List<ValueType>>();

            foreach (var item in data)
            {
                if (!ret.ContainsKey(item.Key))
                    ret.Add(item.Key, new List<ValueType>());
                ret[item.Key].AddRange(item.Value);
            }

            return ret;
        }

        public IEnumerable<ILight> GetLightsForChunk(ChunkKey key)
        {
            var data = Lights.FirstOrDefault(item => item.Key == key);
            if (data.Value == null)
                return new List<ILight>();
            return data.Value;
        }

        private void RecalulateBoundries(RoomData updatedRoom)
        {
            if (updatedRoom.Area.Height == 0 && 
                updatedRoom.Area.Width == 0)
                return;

            UpdateCorners(updatedRoom.Area.TopLeft,
                          updatedRoom.Area.BottomRight);
        }

        /** Update the area this building uses
         */
        private void UpdateCorners(Vector2 topLeft, Vector2 bottomRight)
        {
            if (Area.Width == 0 && Area.Height == 0)
            {
                Area.BottomRight = bottomRight;
                Area.TopLeft = topLeft;
            }
            else
            {
                Area.BottomRight = new Vector2(Math.Max(bottomRight.X, Area.BottomRight.X),
                                              Math.Min(bottomRight.Y, Area.BottomRight.Y));
                Area.TopLeft = new Vector2(Math.Min(topLeft.X, Area.TopLeft.X),
                                          Math.Max(topLeft.Y, Area.TopLeft.Y));
            }
        }

        public void AddRoom(RoomData room)
        {
            _rooms.Add(room);
            RecalulateBoundries(room);
            room.OnRoomChanged += RecalulateBoundries;
        }

        public bool IsPointInBoundry(Vector2 worldVector)
        {
            return Area.Contains(worldVector);
        }

        /** returns the vector of the collision point
         *  or null if there was none
         *  Expects lines to have start.X <= end.X
         */
        protected Vector3? LineHitsLine(Vector3 startA, Vector3 endA, Vector3 startB, Vector3 endB)
        {
            var directionA = endA - startA;
            var directionB = endB - startB;

            var dirANormal = directionA.NormalizeRet();
            var dirBNormal = directionB.NormalizeRet();

            // directions are parallel (ish)
            if (Math.Abs(dirANormal.Cross(dirBNormal).Length) < 0.0001)
                return null;

            var multipleA = (startA - startB).Cross(directionB).Length / directionB.Cross(directionA).Length;
            if (multipleA < 0 || multipleA > 1)
                return null;

            var multipleB = (startA - startB).Cross(directionA).Length / directionB.Cross(directionA).Length;
            if (multipleB < 0 || multipleB > 1)
                return null;

            var pointA = startA + multipleA * directionA;
            var pointB = startB + multipleB * directionB;

            // if they're not the same, then the lines hit, but behind the target
            if (pointA != pointB)
                return null;

            return pointA;
        }

        public RoomData GetRoomContainingPoint(Vector3 biomeVector)
        {
            foreach (var room in _rooms)
            {
                if (room.IsPointInRoom(biomeVector.AsVector2()))
                    return room;
            }
            return null;
        }

        public ChunkMesh GetChunkMesh()
        {
            var ret = new ChunkMesh();
            ret.AddRectangle((byte)MaterialType.Grassland, Area.BottomLeft.AsVector3(), Area.TopRight.AsVector3());
            return ret;
        }

        private List<WallData> GenerateWalls()
        {
            var ret = new List<WallData>();

            foreach (var room in Rooms)
            {
                Debug.Assert(room.Area.BottomLeft.X < room.Area.TopRight.X);
                Debug.Assert(room.Area.BottomLeft.Y < room.Area.TopRight.Y);

                var wallData1 = new WallData(room.Area.BottomLeft.AsVector3(),
                                             new Vector3(1, 0, 0),
                                             (int) Math.Round(room.Area.BottomRight.X - room.Area.BottomLeft.X),
                                             room);
                var wallData2 = new WallData(room.Area.BottomRight.AsVector3(),
                                             new Vector3(0, 1, 0),
                                             (int) Math.Round(room.Area.TopLeft.Y - room.Area.BottomLeft.Y),
                                             room);
                var wallData3 = new WallData(room.Area.TopRight.AsVector3(),
                                             new Vector3(-1, 0, 0),
                                             (int) Math.Round(room.Area.BottomRight.X - room.Area.BottomLeft.X),
                                             room);
                var wallData4 = new WallData(room.Area.TopLeft.AsVector3(),
                                             new Vector3(0, -1, 0),
                                             (int) Math.Round(room.Area.TopLeft.Y - room.Area.BottomLeft.Y),
                                             room);
                ret.Add(wallData1);
                ret.Add(wallData2);
                ret.Add(wallData3);
                ret.Add(wallData4);

                room.Walls.Add(wallData1);
                room.Walls.Add(wallData2);
                room.Walls.Add(wallData3);
                room.Walls.Add(wallData4);
            }

            //ret.Sort(CompareLength);
            EstablishAdjacentWalls(ret);

            return ret;
        }

        private const float ErrorFactor = 0.001f;
        private bool AreOppositeDirections(Vector3 a, Vector3 b)
        {
            if (Math.Abs(a.X + b.X) > ErrorFactor ||
                Math.Abs(a.Y + b.Y) > ErrorFactor ||
                Math.Abs(a.Z + b.Z) > ErrorFactor)
                return false;
            return true;
        }

        private void EstablishAdjacentWalls(List<WallData> walls)
        {
            for (var i=0; i<walls.Count; ++i)
            {
                var wallToInspect = walls[i];
                for (var j = i+1; j < walls.Count; ++j)
                {
                    var wallToTest = walls[j];

                    if (wallToTest.Room == wallToInspect.Room)
                        continue;

                    if (!(AreOppositeDirections(wallToInspect.Direction, wallToTest.Direction)))
                        continue;

                    if (wallToInspect.PointSitsOnWall(wallToTest.StartPosition) ||
                        wallToInspect.PointSitsOnWall(wallToTest.EndPosition) ||
                        wallToTest.PointSitsOnWall(wallToInspect.StartPosition) ||
                        wallToTest.PointSitsOnWall(wallToInspect.EndPosition))
                    {
                        wallToInspect.AdjacentWalls.Add(wallToTest);
                        wallToTest.AdjacentWalls.Add(wallToInspect);
                    }
                }
            }
        }
    }
}

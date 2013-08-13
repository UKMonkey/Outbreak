using System;
using System.Collections.Generic;
using System.Diagnostics;
using Psy.Core;
using SlimMath;

namespace Outbreak.Server.World.Providers.Biome.Buildings
{
    public struct WallOverlapData
    {
        public readonly int StartModifier;
        public readonly int Size;
        public bool HasOverlap { get {return Size > 0;} }

        public WallOverlapData(int start, int size)
        {
            StartModifier = start;
            Size = size;
        }
    }

    public class WallData : IEquatable<WallData>
    {
        private static int _nextId;

            // unique wall id for the building
        public readonly int Id;

            // one side of the wall
        public readonly Vector3 StartPosition;

        public Vector3 EndPosition
        { get { return StartPosition + Size*Direction; } }

            // direction the wall should be propagated
        public readonly Vector3 Direction;

            // direction of the wall that points away from the inside of the room
        public readonly Vector3 OutsideDirection;

            // size of the wall
        public readonly int Size;

            // the room this wall is associated with
        public readonly RoomData Room;

            // positions of the door
        public readonly List<int> DoorPositions; 

            // the walls that sit on the other side of this wall
        public readonly List<WallData> AdjacentWalls;

            // is this wall purely pointing outside
        public bool IsExternal { get { return AdjacentWalls.Count == 0; } }

        private readonly List<WallData> _externalSegments; 

        public WallData(Vector3 start, Vector3 direction, int size, RoomData room)
        {
            StartPosition = start;
            Direction = direction.NormalizeRet();
            OutsideDirection = direction.Rotate((float)(Math.PI/2));
            Size = size;
            Room = room;
            DoorPositions = new List<int>();
            AdjacentWalls = new List<WallData>();
            Id = ++_nextId;

            _externalSegments = null;
        }

        public WallOverlapData? GetLargestOverlapSection()
        {
            WallOverlapData? largestOverlapData = null;
            foreach (var otherWall in AdjacentWalls)
            {
                var result = GetOverlapSection(otherWall);
                if (!result.HasOverlap)
                {
                    Debug.Assert(true);
                    continue;
                }

                if (largestOverlapData == null ||
                    largestOverlapData.Value.Size < result.Size)
                    largestOverlapData = result;
            }

            return largestOverlapData;
        }

        /** returns null if there is no overlap section; although this should never happen
         * if the wall is adjacent!
         */
        public WallOverlapData GetOverlapSection(WallData other)
        {
            // don't forget that adjacent lines have opposite directions!
            // the start point must be either the begining or the end of ONE of the 2 lines...

            // if the wall is known to be adjacent, or we have just calculated it to be an adjacent wall
            // then it'll overlap and we'll have a non-null value to return
            // asserted at the end.
            if (!AdjacentWalls.Contains(other) && !Intersects(other))
                return new WallOverlapData();

            // if our start position is on the other wall
            if (other.PointSitsOnWall(StartPosition))
            {
                if (other.PointSitsOnWall(EndPosition))
                    return new WallOverlapData(0, Size);

                return new WallOverlapData(0, (int)Math.Round((StartPosition - other.StartPosition).Length));            }

            // if our end position is on the other wall
            if (other.PointSitsOnWall(EndPosition))
            {
                var startModifier = (int)Math.Round(other.EndPosition.Distance(StartPosition));
                var length = (int) Math.Round((other.EndPosition - EndPosition).Length);
                return new WallOverlapData(startModifier, length);
            }

            // so the other wall might start & stop inside this one...
            if (PointSitsOnWall(other.StartPosition) &&
                PointSitsOnWall(other.EndPosition))
            {
                var startModifier = (int)Math.Round(other.EndPosition.Distance(StartPosition));
                return new WallOverlapData(startModifier, other.Size);
            }

            // clearly a problem with my thinking.
            // the compiler seems to agree ... interesting
            Debug.Assert(false);
            return new WallOverlapData();
        }

        public List<WallData> GetExternalSections()
        {
            if (_externalSegments != null)
                return _externalSegments;

            // the areas that have no adjacent wall next to it ...
            // walk along the wall and see if the point is 
            return null;
        }

        public bool Equals(WallData other)
        {
            if (other == null)
                return false;
            return false;
        }

        public bool PointSitsOnWall(Vector3 point)
        {
            if (point.DistanceSquared(StartPosition) > Size * Size)
                return false;

            var direction = point - StartPosition;
            
            var xMultiplier = direction.X/Direction.X;
            var yMultiplier = direction.Y/Direction.Y;

            if (xMultiplier < 0 || yMultiplier < 0)
                return false;

            if (Math.Abs(xMultiplier - yMultiplier) > 0.01)
                return false;

            return true;
        }

        public bool Intersects(WallData other)
        {
                // check they go in opposite directions
            if (Math.Abs(Direction.X + other.Direction.X) > 0.01f ||
                Math.Abs(Direction.Y + other.Direction.Y) > 0.01f)
                return false;

                // check they're on the same line
            var startPointDiffence = other.StartPosition - StartPosition;

            if (Direction.X == 0 && Math.Abs(startPointDiffence.X) > 0.01f)
                return false;
            if (Direction.Y == 0 && Math.Abs(startPointDiffence.Y) > 0.01f)
                return false;

            if (startPointDiffence.X * Direction.X < 0 ||
                startPointDiffence.Y * Direction.Y < 0)
                return false;

            var xStep = startPointDiffence.X/Direction.X;
            var yStep = startPointDiffence.Y/Direction.Y;

            if (Math.Abs(xStep - yStep) < 0.001)
                return false;

                // check they're close enough
            if (startPointDiffence.Length > (other.Size + Size))
                return false;

            return true;
        }
    }
}

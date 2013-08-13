using System;
using System.Linq;
using System.Collections.Generic;
using Outbreak.Enums;
using Outbreak.Server.World.Providers.Biome.MeshProviders;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.World.Chunks;
using Vortex.Interface.World;
using Psy.Core;

namespace Outbreak.Server.World.Providers.Biome.Buildings
{
    public delegate void RoomChanged(RoomData item);

    public enum RoomType
    {
        Kitchen,
        DiningRoom,
        LivingRoom,
        Bathroom,
        Bedroom,
        Hallway,
        ShopFloor,
        StoreRoom
    }

    public class RoomData : IntersectMeshProvider
    {
        protected IServer Engine { get; private set; }

            // so that we can generate the right entities for each room
        public RoomType RoomType { get; private set; }

            // not allowing non-rectangular rooms ... may need to convert to a list of areas
        public Rectangle Area;

            // all the entities that make up the room, organised by chunk key
        private readonly Dictionary<ChunkKey, List<Entity>> _entities;
        public IEnumerable<KeyValuePair<ChunkKey, List<Entity>>> Entities { get { return _entities; } }

            // all the lights in the room, organised by chunk key
        private readonly Dictionary<ChunkKey, List<ILight>> _lights;
        public IEnumerable<KeyValuePair<ChunkKey, List<ILight>>> Lights { get { return _lights; } }

            // all the walls that make up this room
        public List<WallData> Walls { get; private set; }

        public IEnumerable<RoomData> AjoiningRooms
        {
            get { return Walls.SelectMany(item => item.AdjacentWalls).Select(item => item.Room).Distinct(); }
        }

        public event RoomChanged OnRoomChanged;

        public bool InternalRoom
        {
            get
            {
                switch (RoomType)
                {
                    default:
                        return true;
                }
            }
        }

        public RoomData(IServer server, RoomType type)
        {
            _entities = new Dictionary<ChunkKey, List<Entity>>();
            _lights = new Dictionary<ChunkKey, List<ILight>>();

            Engine = server;
            RoomType = type;
            Area = new Rectangle();
            Walls = new List<WallData>();
        }

        protected override MaterialType GetMaterial()
        {
            switch (RoomType)
            {
                case RoomType.ShopFloor:
                    return MaterialType.MarbleFloor;
                case RoomType.Bathroom:
                    return MaterialType.TiledFloor;
                default:
                    return MaterialType.Woodpanel;
            }
        }

        public override Rectangle GetFullArea()
        {
            return Area;
        }

        // light position in chunk coords
        public void AddLight(ChunkKey key, ILight light)
        {
            if (!_lights.ContainsKey(key))
                _lights.Add(key, new List<ILight>());
            _lights[key].Add(light);

            RoomChanged();
        }

        // make sure this entity doesn't spawn ontop of another
        private bool IsValidEntity(Entity item)
        {
            foreach (var entity in Entities.SelectMany(values => values.Value))
            {
                var maxDistance = entity.Radius + item.Radius;
                maxDistance *= maxDistance * 1.5f;

                if (entity.GetPosition().DistanceSquared(item.GetPosition()) < maxDistance)
                    return false;
            }
            return true;
        }

        private Vector3 GetSpotInRoomForEntity(Dictionary<PositionRequirement, float> individualReqs,
                                        Dictionary<GroupRequirement, float> groupReqs,
                                        Entity lastInGroup,
                                        Entity item)
        {
            if (individualReqs.ContainsKey(PositionRequirement.PrePositioned))
                return item.GetPosition();
            return RandomSpotInRoom();
        }

        private Vector3 RandomSpotInRoom()
        {
            var x = (float)StaticRng.Random.NextDouble(Area.TopLeft.X, Area.BottomRight.X);
            var y = (float)StaticRng.Random.NextDouble(Area.BottomRight.Y, Area.TopLeft.Y);

            return new Vector3(x, y, 0);
        }

        public void AddClutter(IEnumerable<EntitySpawnData> entities)
        {
            foreach (var entity in entities)
                AddClutter(entity);
        }

        // entity in world coords
        public void AddClutter(EntitySpawnData entityData)
        {
            Entity lastInGroup = null;

            foreach (var entity in entityData.Entities)
            {
                entity.SetPosition(
                    GetSpotInRoomForEntity(entityData.PositionRequirement, entityData.GroupRequirement, lastInGroup, entity)
                );

                bool isValid;
                if (entityData.PositionRequirement.ContainsKey(PositionRequirement.PrePositioned))
                    isValid = true;
                else
                    isValid = IsValidEntity(entity);

                while (!isValid)
                {
                    entity.SetPosition(
                        GetSpotInRoomForEntity(entityData.PositionRequirement, entityData.GroupRequirement, lastInGroup, entity)
                    );
                    isValid = IsValidEntity(entity);
                }

                var key = Engine.GetChunkKeyForWorldVector(entity.GetPosition());
                AddClutter(key, entity);
                lastInGroup = entity;
            }
        }

        private void AddClutter(ChunkKey key, Entity entity)
        {
            AddClutter(key, new List<Entity> { entity });
        }

        // entity in world coords
        private void AddClutter(ChunkKey key, IEnumerable<Entity> entities)
        {
            if (!_entities.ContainsKey(key))
                _entities.Add(key, new List<Entity>());
            _entities[key].AddRange(entities);

            RoomChanged();
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

        /** 1.  A tile is outside the building if it is not inside the bottomLeft / topRight
         *  2.  A tile is inside the building if any 3 of the N / S / E / W vectors hit an
         *      odd number of walls
         */
        public bool IsPointInRoom(Vector2 biomeVector)
        {
            return Area.Contains(biomeVector);
        }

        protected void RoomChanged()
        {
            if (OnRoomChanged != null)
                OnRoomChanged(this);
        }
    }
}

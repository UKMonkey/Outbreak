using System;
using System.Collections.Generic;
using System.Linq;
using Psy.Graphics.Models;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.Audio;
using Vortex.Interface.EntityBase;
using Vortex.Interface.World.Chunks;
using Psy.Core;
using Psy.Core.Collision;
using Psy.Core.Console;
using Vortex.Interface.Net;
using Vortex.Interface.World;
using Vortex.Interface.World.Entities;
using Vortex.World.Chunks;
using Psy.Core.ThreedMesh;

namespace UnitTests.Daybreak.Server.World
{
    public class TestServer : IServer
    {
        public float ObservedSize { get { return Chunk.ChunkWorldSize; } }
        public IConsoleCommandContext ConsoleCommandContext { get; private set; }
        public IServerConfiguration Configuration { get; set; }
        public int UpdateWorldFrequency { get { return 10; } }
        public int SynchronizeFrequency { get { return 10; } }

        public ITimeOfDayProvider TimeOfDayProvider
        {
            get { throw new NotImplementedException(); }
        }

        public uint CurrentFrameNumber
        {
            get { return 0; }
        }

        public IEnumerable<Entity> Entities { get { return _entities; } }
        public CompiledModelCache CompiledModelCache { get; private set; }
        public IMapGeometry MapGeometry { get { return null; } }

        public IAudioLookup AudioLookup
        { get { throw new NotImplementedException(); } }

        public bool IsRaining
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IEnumerable<RemotePlayer> ConnectedClients
        { get { throw new NotImplementedException(); } }

        public IConsole Console
        { get { throw new NotImplementedException(); } }

        private readonly List<Entity> _entities;


        public TestServer()
        {
            _entities = new List<Entity>();
        }

        public Entity GetEntity(int id)
        {
            return _entities.FirstOrDefault(item => item.EntityId == id);
        }

        public IEnumerable<Entity> GetEntities(IEnumerable<int> entityIds)
        {
            return entityIds.Select(GetEntity);
        }

        public Entity GetEntity(RemotePlayer remotePlayer)
        {
            throw new NotImplementedException();
        }

        public Entity GetEntity(string playerName)
        {
            throw new NotImplementedException();
        }

        public void Listen(int port)
        {
        }

        public void SendMessage(Message message, RemotePlayer except)
        {
        }

        public void SendMessageToClient(Message message, RemotePlayer destination)
        {
        }

        public void SetClientFocus(RemotePlayer player, Entity entity)
        {
        }

        public void SpawnEntity(Entity entity)
        {
            _entities.Add(entity);
        }

        public ChunkKey GetChunkKeyForWorldVector(Vector3 vector)
        {
            return Utils.GetChunkKeyForPosition(vector);
        }

        public void GetChunkVectorForWorldVector(Vector3 worldVector, out ChunkKey key, out Vector3 chunkVector)
        {
            Utils.GetChunkVectorFromWorldVector(worldVector, out key, out chunkVector);
        }

        public void PlaySoundOnEntity(Entity target, byte audioChannel, string soundFilename)
        {
        }

        public Cube GetModelContainer(Model target)
        {
            return new Cube(target.Vertices);
        }

        public IEntityFactory EntityFactory
        {
            get { throw new NotImplementedException(); }
        }

        public NetworkStatus NetworkStatus
        {
            get { throw new NotImplementedException(); }
        }

        public MaterialCache MaterialCache
        {
            get { throw new NotImplementedException(); }
        }

        public RemotePlayerCache RemotePlayers
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<Entity> GetEntitiesInChunk(ChunkKey chunkKey)
        {
            return Entities.Where(item => GetChunkKeyForWorldVector(item.GetPosition()) == chunkKey);
        }

        public IEnumerable<Entity> GetEntitiesWithinArea(Vector3 centre, float distance)
        {
            var distanceSquared = distance * distance;
            return Entities.Where(item => item.GetPosition().DistanceSquared(centre) <= distanceSquared);
        }

        public void ConsoleText(string text, Color4 colour)
        {
        }

        public void ConsoleText(string text)
        {
        }

        public void LoadMap()
        {
        }

        public Color4 OutsideLightingColour
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void LoadMapCompleted()
        {
        }

        public void SendMessage(Message message)
        {
        }

        public RemotePlayer GetRemoteClientById(ushort playerId)
        {
            return null;
        }

        public IEnumerable<Entity> SpawnEntityAtRandomObservedLocation(short type, int numberToSpawn)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Entity> SpawnEntityAtRandomObservedLocation(short type, ChunkKey chunkKey, int numberToSpawn)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Entity> SpawnEntityAtRandomObservedLocation(short type, Vector3 centre, float distance, int numberToSpawn)
        {
            throw new NotImplementedException();
        }

        public Direction GetCollisionAdjustedDirection(Entity entity, Direction dir, float f)
        {
            return Direction.None;
        }

        public void BroadcastSay(string messageString)
        {
        }

        public void SendRconCommand(string command, string password)
        {
        }

        public void FireBullet(Vector3 from, Vector3 to, Color4 colour, Entity hit, Entity shooter = null)
        {
        }

        public CollisionResult TraceRay(Vector3 @from, float angle, Func<Entity, bool> filter)
        {
            return new CollisionResult();
        }

        public CollisionResult TraceRay(Vector3 from, float angle, bool worldOnly)
        {
            return new CollisionResult();
        }

        public CollisionResult TraceRay(Vector3 source, Vector3 target)
        {
            return new CollisionResult();
        }

        public CollisionResult TraceRay(Vector3 source, Vector3 target, Func<Entity, bool> filter)
        {
            return new CollisionResult();
        }

        public CollisionResult TraceRay(Vector3 source, Vector3 target, IEnumerable<Entity> entities)
        {
            return new CollisionResult();
        }

        public CollisionResult TraceRay(Ray ray, Func<Entity, bool> filter)
        {
            throw new NotImplementedException();
        }

        public void CalculateVisibleArea(Vector3 position)
        {
        }

        public void RegisterMessageCallback(Type msgType, MessageHandler handler)
        {
        }

        public void UnregisterMessageCallback(Type msgType)
        {
        }

        public event EntitiesCallback OnEntitiesGenerated;

        public void RegisterRequirement(short type, SpawnTest test)
        {
        }

        public Vector3 ChunkVectorToWorldVector(ChunkKey chunkKey, Vector3 vector)
        {
            return Utils.GetChunkWorldVectorWithOffset(chunkKey, vector);
        }

        public void TrackEntity(Entity thingToTrack)
        {
        }

        public void StopTrackingEntity(Entity thingToStopTracking)
        {
        }

        public void Dispose()
        {
        }
    }
}

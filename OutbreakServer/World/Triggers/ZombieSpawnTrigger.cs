using System;
using System.Linq;
using System.Collections.Generic;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.EntityBase.Properties;
using Vortex.Interface.World.Chunks;
using Vortex.Interface.World.Triggers;
using Outbreak.Entities;
using Outbreak.Entities.Properties;
using Psy.Core;
using Psy.Core.Console;


namespace Outbreak.Server.World.Triggers
{
    [TriggerType(TypeName = "zombieSpawnTrigger")]
    public class ZombieSpawnTrigger : TimedTrigger
    {
        public override bool SendToClient { get { return false; } }

        /** zombie density to spawn
         */
        private const float DefaultDensity = 0.005f;
        private const string DensityKey = "Density";
        private float _density;
        public float Density
        {
            get { return _density; }
            set 
            {
                _density = value;
                Properties.Remove(DensityKey);
                Properties.Add(DensityKey, string.Format("{0}", value));
            }
        }

        private const bool DefaultCanSpawn = false;
        private const string CanSpawnKey = "CanSpawn";
        private bool _canSpawn;
        private bool CanSpawn
        {
            get { return _canSpawn; }
            set
            {
                _canSpawn = value;
                Properties.Remove(CanSpawnKey);
                Properties.Add(CanSpawnKey, string.Format("{0}", value));
            }
        }

        public ZombieSpawnTrigger(IEngine engine)
            : base(engine)
        {
            CanSpawn = DefaultCanSpawn;
            Density = DefaultDensity;
        }

        public ZombieSpawnTrigger(IEngine engine, TriggerKey uniqueKey)
            : base(engine, uniqueKey, 
                   engine.ChunkVectorToWorldVector(uniqueKey.ChunkLocation, new Vector3(1, 1, 0)), 
                   engine.TimeOfDayProvider.TicksPerDay / 100)
        {
            OnActivated += SpawnZombie;
            StaticConsole.Console.RegisterFloat("zombiespawn", () => CanSpawn ? 1 : 0, delegate(float f) { CanSpawn = f > 0; });

            CanSpawn = DefaultCanSpawn;
            Density = DefaultDensity;
        }

        public override void SetProperties(TriggerKey key, Vector3 location, IEnumerable<KeyValuePair<string, string>> properties)
        {
            base.SetProperties(key, location, properties);
            string value;

            CanSpawn = DefaultCanSpawn;
            Density = DefaultDensity;

            if (Properties.TryGetValue(DensityKey, out value))
            {
                if (!float.TryParse(value, out _density))
                    _density = DefaultDensity;
            }

            if (Properties.TryGetValue(CanSpawnKey, out value))
            {
                if (!bool.TryParse(value, out _canSpawn))
                    _canSpawn = true;
            }
        }

        private void SpawnZombie(ITrigger trigger)
        {
            if (!CanSpawn)
                return;

            var entities = Engine.GetEntitiesInChunk(UniqueKey.ChunkLocation);
            var countInArea = entities.Count(item => item.GetIsZombie());
            var countToSpawn = (int)Math.Floor((Chunk.ChunkWorldSize * Chunk.ChunkWorldSize * Density) - countInArea);

            if (countToSpawn <= 0)
                return;

            if (Engine.Entities.Count(x => x.GetIsZombie()) > 10)
                return;

            var zombies = Engine.SpawnEntityAtRandomObservedLocation((int)EntityTypeEnum.Zombie, UniqueKey.ChunkLocation, countToSpawn);
            
            foreach (var zombie in zombies)
            {
                var rotation = (float) (StaticRng.Random.NextDouble()*2*Math.PI);
                var speed = zombie.GetWalkSpeed();

                var movementDir = DirectionUtil.CalculateVector(rotation);
                zombie.SetRotation(rotation);
                zombie.SetMovementVector(movementDir * speed);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Outbreak.Entities.Properties;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Properties;

namespace Outbreak.Client
{
    public class EntityWatcher
    {
        private readonly Dictionary<int, Entity> _visibleEntities;
        private readonly IClient _engine;
        private const float TwoPi = (float)Math.PI*2;

        private bool _returnAllEntities;

        public EntityWatcher(IClient client)
        {
            _visibleEntities = new Dictionary<int, Entity>();
            _engine = client;

            client.SetEntityViewCollector(GetVisibleEntities);
            _returnAllEntities = true;
        }

        public EntityWatcher(IClient client, Entity target)
            :this(client)
        {
            _returnAllEntities = false;
            target.OnDeath += EntityKilled;

            _engine.RegisterViewSystemForEntity(target, IsVisible, VisibleEntitiesUpdated, NonVisibleEntitiesUpdated);
        }

        private void EntityKilled(Entity item)
        {
            _returnAllEntities = true;
        }

        public void OnEntityKilled(Entity item)
        {
            if (_visibleEntities.ContainsKey(item.EntityId))
                _visibleEntities.Remove(item.EntityId);
        }

        public IEnumerable<Entity> GetVisibleEntities()
        {
            if (_returnAllEntities)
                return _engine.Entities;
            return _visibleEntities.Values;
        }

        private void VisibleEntitiesUpdated(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                var newValue = entity.IncreaseVisibleRating(0.1f);
                if (newValue > 0.3f)
                {
                    _visibleEntities[entity.EntityId] = entity;
                    entity.OnDeath += OnEntityKilled;
                }
            }
        }

        private void NonVisibleEntitiesUpdated(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                var newValue = entity.DropVisibleRating(0.05f);
                if (newValue < 0.3f && _visibleEntities.ContainsKey(entity.EntityId))
                    _visibleEntities.Remove(entity.EntityId);
            }            
        }

        /**
         * Performance improvements on this are worth while ... if we can do any caching here
         * on - for example - requirements for something to be visible 
         */
        private bool IsVisible(Entity eyes, Entity target, IEnumerable<Entity> entities)
        {
            if (target.GetStatic())
                return true;

            // is the target we're following?
            if (eyes != null && eyes.EntityId == target.EntityId)
                return true;

            if (eyes == null)
                return false; // should never happen

            // is the target close enough?
            var maxDistance = eyes.GetViewRange();
            var maxDistanceSqrd = maxDistance*maxDistance;
            var distanceSqrd = (target.GetPosition() - eyes.GetPosition()).LengthSquared;

            if (distanceSqrd > maxDistanceSqrd)
                return false;

            // is the target too close?
            var direction = target.GetPosition() - eyes.GetPosition();

            var minDistance = eyes.GetMeleeViewRange();
            var minDistanceSqrd = minDistance*minDistance;

            if (distanceSqrd > minDistanceSqrd)
            {
                // is the target within the right angle?
                var angle = Math.Atan2(direction.Y, direction.X);
                var angleDifference = eyes.GetRotation() - angle;
                var angleRange = eyes.GetViewAngleRange();

                while (angleDifference > Math.PI*2)
                    angleDifference -= TwoPi;
                while (angleDifference < 0)
                    angleDifference += TwoPi;

                if (!(angleDifference < angleRange || (TwoPi - angleDifference) < angleRange))
                    return false;
            }

            var result = _engine.TraceRay(eyes.GetEyePosition(), target.GetEyePosition(), entities);
            if (!result.HasCollided) // how?
                return true;
            if (result.CollisionMesh.Id == target.EntityId)
                return true;

            return false;
        }
    }
}

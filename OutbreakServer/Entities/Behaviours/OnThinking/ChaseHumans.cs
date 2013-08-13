using System;
using System.Collections.Generic;
using System.Linq;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Vortex.Interface.EntityBase.Properties;
using Outbreak.Entities.Properties;
using Psy.Core;
using Psy.Core.Logging;

namespace Outbreak.Server.Entities.Behaviours.OnThinking
{
    public class ChaseHumans : IEntityBehaviour
    {
        private readonly IEngine _engine;
        private const int ThinkSkipsOnInactive = 5;

        public ChaseHumans(IEngine engine)
        {
            _engine = engine;
        }

        private IEnumerable<Entity> GetVisibleEntities(Entity target, IEnumerable<Entity> toCheck)
        {
            var targetRotation = target.GetRotation();
            var viewAngle = target.GetViewAngleRange()/2;

            return toCheck.Where(item =>
                                     {
                                         var angleToItem = target.GetPosition().AngleTo2DPoint(item.GetPosition());
                                         var angleDifference = (float)Math.PI - Math.Abs(Math.Abs(angleToItem - targetRotation) - (float)Math.PI);

                                         if (Math.Abs(angleDifference) > viewAngle)
                                             return false;

                                         var distanceSqrd = target.GetPosition().DistanceSquared(item.GetPosition());
                                         var position = target.GetPosition();
                                         var collisionResult = _engine.TraceRay(position, item.GetPosition(),
                                             x => IsPotentialBlockingTarget(distanceSqrd, position, x) &&
                                                  x.EntityId != target.EntityId &&
                                                  x.EntityId != item.EntityId);

                                         return !collisionResult.HasCollided;
                                     });
        }

        private bool IsPotentialBlockingTarget(float distanceSqrd, Vector3 position, Entity item)
        {
            if (!item.GetSolid())
                return false;

            if (distanceSqrd < item.GetPosition().DistanceSquared(position))
                return false;

            return true;
        }

        // if we're not chasing anything, then we don't need to check what to do
        // as often...
        private bool ShouldPerformBehaviour(Entity target)
        {
            if (target.GetChaseTargetId() != null)
                return true;

            if (target.GetChaseThinkCount() % ThinkSkipsOnInactive == 0)
                return true;

            return false;
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            target.IncrementChaseThinkCount();
            if (!ShouldPerformBehaviour(target))
                return;

            var targetKnownInSight = SelectTarget(target);

            if (GetTarget(target) == null)
                return;

            if (!targetKnownInSight)
            {
                var toChase = GetTarget(target);
                var visibleEntities = GetVisibleEntities(target, new List<Entity> {toChase});

                if (!visibleEntities.Any())
                    targetKnownInSight = true;
            }

            if (targetKnownInSight)
                SetDestination(target, GetTarget(target).GetPosition());
        }

        private Entity GetTarget(Entity beingProcessed)
        {
            var id = beingProcessed.GetChaseTargetId();
            return id == null ? null : _engine.GetEntity(id.Value);
        }

        private bool CanChangeTarget(Entity toProcess)
        {
            if (GetTarget(toProcess) == null)
                return true;

            if ((Timer.GetTime() - toProcess.GetChaseStartTime()) > toProcess.GetChaseMinTime())
                return true;

            return false;
        }

        private Entity EstablishTarget(Entity toProcess, IEnumerable<Entity> availablePlayers)
        {
            Entity closestPlayer = null;
            float distance = 0;

            foreach (var player in availablePlayers)
            {
                if (closestPlayer == null)
                {
                    distance = toProcess.GetPosition().Distance(player.GetPosition());
                    closestPlayer = player;
                }
                else
                {
                    var tmpDistance = toProcess.GetPosition().Distance(player.GetPosition());
                    if (tmpDistance < distance)
                    {
                        closestPlayer = player;
                        distance = tmpDistance;
                    }
                }
            }

            return closestPlayer;
        }

        private bool SelectTarget(Entity toProcess)
        {
            if (!CanChangeTarget(toProcess))
                return false;

            var maxDistanceSqrd = toProcess.GetViewRange() * toProcess.GetViewRange();
            var nearbyPotentialTargets = _engine.Entities.Where(item => 
                !item.GetIsGod() &&
                item.GetIsHuman() && 
                item.GetPosition().DistanceSquared(toProcess.GetPosition()) < maxDistanceSqrd);

            var currentTarget = GetTarget(toProcess);

            var availablePlayers = GetVisibleEntities(toProcess, nearbyPotentialTargets);
            var tmp = EstablishTarget(toProcess, availablePlayers);
            
            if (tmp == null)
            {
                if (currentTarget != null)
                {
                    Logger.Write(string.Format("Chaser {0} lost target", toProcess.EntityId), LoggerLevel.Trace);
                }
                toProcess.ClearChaseTarget();
                toProcess.SetToWalkingSpeed();
            }
            else if (currentTarget != tmp)
            {
                Logger.Write(string.Format("Chaser {0} found new target ({1})", toProcess.EntityId, tmp.EntityId), LoggerLevel.Trace);
                toProcess.SetChaseStartTime(Timer.GetTime());
                toProcess.SetChaseTargetId(tmp.EntityId);
                toProcess.SetToRunningSpeed();
            }

            return true;
        }

        private void SetDestination(Entity toProcess, Vector3 destination)
        {
            toProcess.LookAt(destination);
            var movementVector = toProcess.GetMovementVector();
            var newMovementVector = DirectionUtil.CalculateVector(toProcess.GetRotation()) * movementVector.Length;

            toProcess.SetMovementVector(newMovementVector);
        }
    }
}

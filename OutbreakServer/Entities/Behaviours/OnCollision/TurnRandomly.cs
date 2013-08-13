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

namespace Outbreak.Server.Entities.Behaviours.OnCollision
{
    public class TurnRandomly : IEntityBehaviour
    {
        private readonly IEngine _engine;

        private class AngleRange
        {
            public float Min { get; set; }
            public float Max { get; set; }
            public float Range { get { return Max - Min; } }

            private const float Pi2 = (float) (Math.PI*2);
            public static IEnumerable<AngleRange> GetRanges(float min, float max)
            {
                if (min >= 0 && max <= Pi2)
                    return new List<AngleRange>{new AngleRange{Min = min, Max = max}};
                if (min < 0)
                    return GetRanges(0, max).Concat(GetRanges(Pi2 + min, Pi2));
                if (max > 0)
                    return GetRanges(min, Pi2).Concat(GetRanges(0, max - Pi2));
                throw new Exception("Logic error");
            }
        }

        public TurnRandomly(IEngine engine)
        {
            _engine = engine;
        }

        private void SetDirection(Entity target, float newRotation)
        {
            var vector = target.GetMovementVector();
            var newVector = DirectionUtil.CalculateVector(newRotation)*vector.Length;
            target.SetRotation(newRotation);
            target.SetMovementVector(newVector);
        }

        private IEnumerable<AngleRange> GetInvalidRange(Entity from, Entity to)
        {
            var travelDirection = to.GetPosition() - from.GetPosition();
            var centralAngle = travelDirection.ZPlaneAngle();

            var angleRange = (float)(Math.Atan2(to.Radius, travelDirection.Length));
            var min = centralAngle - angleRange;
            var max = centralAngle + angleRange;

            return AngleRange.GetRanges(min, max);
        }

        private static IEnumerable<AngleRange> UpdateValidRanges(IEnumerable<AngleRange> validRanges, AngleRange invalidRange)
        {
            var ret = new List<AngleRange>();

            foreach (var item in validRanges)
            {
                if ((item.Min > invalidRange.Max) ||
                    (item.Max < invalidRange.Min))
                {
                    ret.Add(item);
                }
                else if (item.Min > invalidRange.Min)
                {
                    ret.Add(new AngleRange {Min = invalidRange.Max, Max = item.Max});
                }
                else if (item.Max < invalidRange.Max)
                {
                    ret.Add(new AngleRange { Min = item.Min, Max = invalidRange.Min });
                }
                else
                {
                    ret.Add(new AngleRange { Min = item.Min, Max = invalidRange.Min });
                    ret.Add(new AngleRange { Min = invalidRange.Max, Max = item.Max });
                }
            }

            return ret.Where(item => item.Range > 0);
        }

        private static float PickValidDirection(List<AngleRange> validRanges)
        {
            var index = StaticRng.Random.Next(0, validRanges.Count - 1);
            var validRange = validRanges[index];

            return (float)((StaticRng.Random.NextDouble()*(validRange.Range)) + validRange.Min);
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            if (!instigator.GetSolid())
                return;

            if (target.GetChaseTargetId() == instigator.EntityId)
                return;

            // only perform if the instigator is infront of us
            if (Math.Cos(target.GetRotation() - (instigator.GetPosition() - target.GetPosition()).ZPlaneAngle()) < 0)
                return;

            var modestDistance = target.GetMovementVector().Length*150;
            var entities = _engine.GetEntitiesWithinArea(target.GetPosition(), modestDistance)
                .Where(item => item.GetSolid())
                .Where(item => item.EntityId != target.EntityId);
            
            IEnumerable<AngleRange> validRanges = new List<AngleRange> {new AngleRange() {Min = 0, Max = (float)(2*Math.PI)}};
            var invalidRanges = new List<AngleRange>();

            foreach (var entity in entities)
                invalidRanges.AddRange(GetInvalidRange(target, entity));

            foreach (var invalidRange in invalidRanges)
                validRanges = UpdateValidRanges(validRanges, invalidRange);
            
            var ranges = validRanges.ToList();
            float newDirection;

            if (ranges.Count > 0)
                newDirection = PickValidDirection(ranges);
            else
                newDirection = target.GetRotation() + 0.1f;

            target.SetPosition(target.GetPosition() - target.GetMovementVector());
            SetDirection(target, newDirection);
        }
    }
}

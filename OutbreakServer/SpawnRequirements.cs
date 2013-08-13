using Outbreak.Entities.Properties;
using Psy.Core;
using SlimMath;
using Vortex.Interface;

namespace Outbreak.Server
{
    public static class SpawnRequirements
    {
        public static bool DistanceToOthersRequirement(
            double distanceSqrd, IEngine engine, float outdoorLightIntensity, Color4 bakedLight, float fieldOfFiew, Vector3 location)
        {
            var entities = engine.GetEntitiesWithinArea(location, 10);

            foreach (var entity in entities)
            {
                // can't spawn ontop of another entity
                if (entity.GetPosition().LengthSquared <
                    distanceSqrd)
                    return false;
            }

            return true;
        }

        public static bool HumanDistanceRequirement(IEngine engine, float outdoorLightIntensity, Color4 bakedLight, float fieldOfFiew, Vector3 location)
        {
            var entities = engine.GetEntitiesWithinArea(location, 30);

            foreach (var entity in entities)
            {
                if (entity.GetIsHuman())
                    return false;
            }

            return true;
        }

        public static bool ZombieLightRequirement(IEngine engine, float outdoorLightIntensity, Color4 bakedLight, float fieldOfFiew, Vector3 location)
        {
            return true;
            //var lightIntensity = bakedLight.Intensity() + outdoorLightIntensity;
            //return lightIntensity < 0.3 && fieldOfFiew < 0.01;
        }
    }
}

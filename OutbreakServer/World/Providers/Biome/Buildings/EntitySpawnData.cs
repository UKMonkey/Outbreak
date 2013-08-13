using System.Collections.Generic;
using Vortex.Interface.EntityBase;

namespace Outbreak.Server.World.Providers.Biome.Buildings
{
    public enum PositionRequirement
    {
        PrePositioned = 1,
        ByWall = 2,
        ParallelToWall = 4,
        FloorLevel = 8
    }

    public enum GroupRequirement
    {
    }


    public class EntitySpawnData
    {
        // entities to spawn
        public IEnumerable<Entity> Entities { get; private set; }

        // requirements that all entities must meet with respect to other entities & the room
        public Dictionary<PositionRequirement, float> PositionRequirement { get; private set; }

        // requirements on the group as a whole (eg must be within x units of first item)
        public Dictionary<GroupRequirement, float> GroupRequirement { get; private set; }

        public EntitySpawnData(Entity item, Dictionary<PositionRequirement, float> req)
        {
            Entities = new List<Entity>{item};
            PositionRequirement = req;
        }

        public EntitySpawnData(IEnumerable<Entity> items, Dictionary<PositionRequirement, float> req, Dictionary<GroupRequirement, float> grpReq)
        {
            Entities = items;
            PositionRequirement = req;
            GroupRequirement = grpReq;
        }
    }
}

using System.Collections.Generic;
using Outbreak.Entities.Properties;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Vortex.Interface.EntityBase.Properties;

namespace Outbreak.Server.Entities.Behaviours.OnTakeDamage
{
    public class GetAngry : IEntityBehaviour
    {
        private readonly GameServer _gameServer;

        public GetAngry(GameServer gameServer)
        {
            _gameServer = gameServer;
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            if (instigator == null)
            {
                // angry at nobody.
                return;
            }

            if (!instigator.HasProperty((int)EntityPropertyEnum.RemotePlayer))
            {
                // not angry at a player.
                return;
            }

            List<int> aggroList;

            if (target.HasProperty((int)GameEntityPropertyEnum.AggroList))
            {
                aggroList = (List<int>)target.GetProperty((int) GameEntityPropertyEnum.AggroList).ObjectValue;
            }
            else
            {
                aggroList = new List<int>(5);
                target.SetProperty(new EntityProperty((int)GameEntityPropertyEnum.AggroList, aggroList) {IsDirtyable = false});
            }

            if (!aggroList.Contains(instigator.EntityId))
            {
                aggroList.Add(instigator.EntityId);
            }
        }
    }
}
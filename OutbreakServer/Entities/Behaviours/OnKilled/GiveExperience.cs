using System.Collections.Generic;
using Outbreak.Entities.Properties;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Vortex.Interface.EntityBase.Properties;

namespace Outbreak.Server.Entities.Behaviours.OnKilled
{
    public class GiveExperience : IEntityBehaviour
    {
        private readonly int _xpAmount;
        private readonly GameServer _gameServer;
        private readonly Levelling _levelling;

        public GiveExperience(GameServer gameServer, int xpAmount)
        {
            _gameServer = gameServer;
            _xpAmount = xpAmount;
            _levelling = new Levelling(gameServer, gameServer.LevelExperienceCalculator);
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            if (!target.HasProperty((int)GameEntityPropertyEnum.AggroList))
            {
                // no aggro list, nobody to give experience to.
                return;
            }

            var aggroList = (List<int>)(target.GetProperty((int) GameEntityPropertyEnum.AggroList).ObjectValue);
            if (aggroList.Count == 0)
                return;

            var xpAmount = _xpAmount/aggroList.Count;

            foreach (var entityId in aggroList)
            {
                var entity = _gameServer.Engine.GetEntity(entityId);
                if (entity == null)
                {
                    continue;
                }

                var player = entity.GetPlayer(_gameServer.Engine);
                if (player == null)
                {
                    continue;
                }

                _levelling.GiveExperienceToPlayer(player, entity, xpAmount);
            }
        }
    }
}
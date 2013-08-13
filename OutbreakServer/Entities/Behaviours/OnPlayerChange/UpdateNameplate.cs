using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Vortex.Interface.EntityBase.Properties;

namespace Outbreak.Server.Entities.Behaviours.OnPlayerChange
{
    public class UpdateNameplate : IEntityBehaviour
    {
        private readonly IEngine _engine;

        public UpdateNameplate(IEngine engine)
        {
            _engine = engine;
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            var player = target.GetPlayer(_engine);

            if (player == null)
            {
                target.SetNameplate("");
            }
            else
            {
                target.SetNameplate(player.PlayerName);
            }
        }
    }
}

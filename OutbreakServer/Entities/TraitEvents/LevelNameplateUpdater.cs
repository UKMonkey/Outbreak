using Outbreak.Entities.Properties;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Properties;
using Vortex.Interface.Traits;

namespace Outbreak.Server.Entities.TraitEvents
{
    public class LevelNameplateUpdater
    {
        private readonly IEngine _engine;

        public LevelNameplateUpdater(IEngine engine)
        {
            _engine = engine;
        }

        public void UpdateNameplate(Entity changed, Trait changedTrait)
        {
            if (changedTrait.PropertyId != (short)GameEntityPropertyEnum.Level)
                return;

            UpdateNameplate(changed);
        }

        public void UpdateNameplate(Entity target)
        {
            var player = target.GetPlayer(_engine);
            if (player == null)
                return;

            var newNameplate = string.Format("{0} ({1})", player.PlayerName, target.GetLevel());
            target.SetNameplate(newNameplate);
        }
    }
}

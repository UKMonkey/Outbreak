using Outbreak.Entities.Properties;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;

namespace Outbreak.Server.Entities.Behaviours.OnInventoryChange
{
    public class ChangeModelIfEmpty : IEntityBehaviour
    {
        private readonly string _targetModel;

        public ChangeModelIfEmpty(string targetModel)
        {
            _targetModel = targetModel;
        }

        private static bool ShouldPerformBehaviour(Entity target)
        {
            if (!target.HasInventory())
                return true;

            var inv = target.GetInventory();
            if (inv.IsEmpty)
                return true;

            return false;
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            if (!ShouldPerformBehaviour(target))
                return;

            target.SetModel(_targetModel);
        }
    }
}

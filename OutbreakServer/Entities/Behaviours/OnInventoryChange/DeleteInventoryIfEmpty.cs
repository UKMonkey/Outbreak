using Outbreak.Entities.Properties;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;

namespace Outbreak.Server.Entities.Behaviours.OnInventoryChange
{
    public class DeleteInventoryIfEmpty : IEntityBehaviour
    {
        public void PerformBehaviour(Entity target, Entity instigator)
        {
            if (!target.HasInventory())
                return;

            var inv = target.GetInventory();

            if (!inv.IsEmpty)
                return;

            target.RemoveInventory();
        }
    }
}

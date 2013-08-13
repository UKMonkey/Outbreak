using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers.FloatingItems;

namespace Outbreak.Server.Entities.Behaviours.OnInteract
{
    public class AddToInventory : IEntityBehaviour
    {
        public void PerformBehaviour(Entity target, Entity instigator)
        {
            if (instigator == null)
                return;
            
            var inventory = instigator.GetInventory();
            if (inventory == null)
                return;

            if (!target.HasFloatingItemId())
                return;

            var floatingId = target.GetFloatingItemId();
            var item = StaticFloatingItemCache.Instance.GetItem(floatingId);

            var completelyAdded = inventory.AddItem(item);

            if (completelyAdded || target.GetDestroyOnPickedUp())
            {
                target.SetFloatingItemId(StaticFloatingItemCache.Instance.UnknownId);
                StaticFloatingItemCache.Instance.RemoveFloatingItem(floatingId);
                target.Destroy();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers;
using Outbreak.Items.Containers.InventorySpecs;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Vortex.Interface.EntityBase.Properties;

namespace Outbreak.Server.Entities.Behaviours.OnInventoryChange
{
    public class UpdateVisibleItems : IEntityBehaviour
    {
        private readonly InventorySpecialSlotEnum _slotId;
        private readonly GameEntityPropertyEnum _entityProperty;


        public UpdateVisibleItems(InventorySpecialSlotEnum targetFrom, GameEntityPropertyEnum targetTo)
        {
            _slotId = targetFrom;
            _entityProperty = targetTo;
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            if (!target.HasInventory())
                return;

            var inventory = target.GetInventory();
            var item = inventory[_slotId];

            var itemSpecId = StaticItemSpecCache.Instance.EmptySpecId;

            if (item != null)
                itemSpecId = item.ItemSpecId;

            target.SetProperty(new EntityProperty((short)_entityProperty, itemSpecId));
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Outbreak.Entities.Properties;
using Psy.Graphics.Models;
using Vortex.Interface.EntityBase;
using Outbreak.Items.Containers.InventorySpecs;

namespace Outbreak
{
    public class ModelEquipper
    {
        private class DelayedItemChange
        {
            public readonly Entity Entity;
            public readonly string AnchorName;
            public readonly short ItemSpecId;

            public DelayedItemChange(Entity entity, string anchorName, short itemSpecId)
            {
                Entity = entity;
                AnchorName = anchorName;
                ItemSpecId = itemSpecId;
            }
        }

        private readonly IItemSpecCache _itemSpecCache;
        private readonly CompiledModelCache _compiledModelCache;
        private readonly List<DelayedItemChange> _delayedItemChanges;

        public ModelEquipper(IItemSpecCache itemSpecCache, CompiledModelCache compiledModelCache)
        {
            _itemSpecCache = itemSpecCache;
            _compiledModelCache = compiledModelCache;
            _itemSpecCache.OnItemAdded += HandleAsyncItemUpdates;
            _delayedItemChanges = new List<DelayedItemChange>();
        }

        public void Equip(Entity entity)
        {
            EquipHeadSlotItem(entity);
            EquipPrimary(entity);
            EquipSecondary(entity);
        }

        public void EquipPrimary(Entity entity)
        {
            Equip(entity, entity.GetPrimaryWeaponItem(), "lefthand");
        }

        public void EquipSecondary(Entity entity)
        {
            Equip(entity, entity.GetSecondaryWeaponItem(), "righthand");
        }

        public void EquipHeadSlotItem(Entity entity)
        {
            Equip(entity, entity.GetHeadSlotItem(), "hat");
        }

        private void Equip(Entity entity, short itemSpecId, string anchorName)
        {
            if (!entity.Model.ModelInstance.HasAnchor(anchorName))
            {
                return;
            }

            if (itemSpecId == _itemSpecCache.EmptySpecId)
            {
                entity.Model.ModelInstance.RemoveSubModels(anchorName);
            }
            else
            {
                var subModel = entity.Model.ModelInstance.GetAttachedSubModels(anchorName).FirstOrDefault();
                var itemSpec = _itemSpecCache.GetItemSpec(itemSpecId);
                
                if (itemSpec == null)
                {
                    AddDelayedItemChange(entity, anchorName, itemSpecId);
                    return;
                }
                
                var newModelName = itemSpec.GetModelName();

                if (subModel != null && subModel.Model.Name.Equals(newModelName))
                {
                    // no need to change model, rejoice!
                    return;
                }

                entity.Model.ModelInstance.RemoveSubModels(anchorName);
                entity.Model.ModelInstance.AddSubModel(anchorName, _compiledModelCache.GetModel(itemSpec.GetModelName()));
            }
        }

        private void HandleAsyncItemUpdates(ItemSpec spec)
        {
            var delayedItemChanges = _delayedItemChanges.Where(x => x.ItemSpecId == spec.Id);

            foreach (var delayedItemChange in delayedItemChanges)
            {
                Equip(delayedItemChange.Entity, delayedItemChange.ItemSpecId, delayedItemChange.AnchorName);
            }

            _delayedItemChanges.RemoveAll(x => x.ItemSpecId == spec.Id);
        }

        private void AddDelayedItemChange(Entity entity, string anchorName, short itemSpecId)
        {
            _delayedItemChanges.Add(new DelayedItemChange(entity, anchorName, itemSpecId));
            entity.OnDeath += EntityOnOnDeath;
        }

        private void EntityOnOnDeath(Entity entity)
        {
            _delayedItemChanges.RemoveAll(x => x.Entity == entity);
        }
    }
}
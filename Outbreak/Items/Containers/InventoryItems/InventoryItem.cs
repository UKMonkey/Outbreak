using System.Collections.Generic;
using System.Diagnostics;
using Vortex.Interface.Traits;
using Outbreak.Items.Containers.InventorySpecs;

namespace Outbreak.Items.Containers.InventoryItems
{
    public delegate void InventoryItemCallback(Inventory inventory, InventoryItem item);

    public class InventoryItem
    {
        public readonly int ItemSpecId;
        public event InventoryItemCallback OnItemChanged;

        public readonly Inventory Inventory;
        private readonly Dictionary<short, InventoryItemProperty> _properties;

        public InventoryItem(int specId)
            :this(null, specId)
        {
        }

        public InventoryItem(Inventory inventory, int specId)
        {
            ItemSpecId = specId;
            Inventory = inventory;
            _properties = new Dictionary<short, InventoryItemProperty>();
        }

        public InventoryItem Clone(Inventory inventory)
        {
            var ret = new InventoryItem(inventory, ItemSpecId);
            foreach (var item in _properties)
                ret.SetProperty(item.Value);
            return ret;
        }

        public int GetPropertyCount()
        {
            return _properties.Count;
        }

        public IEnumerable<InventoryItemProperty> GetProperties()
        {
            return _properties.Values;
        }

        /// <summary>
        /// Add the source stack onto this.
        /// </summary>
        /// <param name="source">Source stack</param>
        public void AddStack(InventoryItem source)
        {
            if (ItemSpecId != source.ItemSpecId)
                return;
            if (this.HasDurability())
                return;

            var maxStackSize = StaticItemSpecCache.Instance.GetItemSpec(ItemSpecId).GetStackMax();
            var count = this.GetCount();
            var amountToAdd = source.GetCount();

            if (count + amountToAdd > maxStackSize)
            {
                this.SetCount(maxStackSize);
                source.SetCount((short) (amountToAdd + count - maxStackSize));
            }
            else
            {
                this.SetCount((short)(count + amountToAdd));
                source.SetCount(0);
            }
        }

        public InventoryItemProperty GetProperty(InventoryItemPropertyEnum prop)
        {
            return _properties.ContainsKey((short)prop) ? _properties[(short)prop] : null;
        }

        public void SetProperty(InventoryItemProperty prop)
        {
            if (_properties.ContainsKey(prop.PropertyId))
            {
                var property = _properties[prop.PropertyId];
                property.Value = prop.Value;
            }
            else
            {
                _properties.Add(prop.PropertyId, new InventoryItemProperty(prop));
                _properties[prop.PropertyId].OnTraitChanged += PropertyChanged;
            }

            if (OnItemChanged != null)
                OnItemChanged(Inventory, this);
        }

        public void SetProperties(List<InventoryItemProperty> properties)
        {
            foreach (var item in properties)
                SetProperty(item);
        }

        private void PropertyChanged(Trait changed)
        {
            if (Inventory != null)
            {
                Debug.Assert(OnItemChanged != null);
            }

            if (OnItemChanged != null)
                OnItemChanged(Inventory, this);
        }

        public bool IsDirty()
        {
            foreach (var item in _properties)
                if (item.Value.IsDirty)
                    return true;
            return false;
        }
    }
}

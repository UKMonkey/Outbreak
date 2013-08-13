using System.Collections.Generic;
using System.Linq;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;

namespace Outbreak.Items.Containers
{
    public delegate void SlotChangedEvent(Inventory inventory, byte slotIndex);

    public class Inventory
    {
        public long Id { get; private set; }
        public event SlotChangedEvent OnSlotChanged;
        public InventoryType InventoryType { get; private set; }
        public bool KeepPersisted { get; private set; }

        public bool IsEmpty
        {
            get
            {
                return _slots.All(item => item == null);
            }
        }

        public int OccupiedSlots
        {
            get
            {
                return _slots.Count(x => x == null);
            }
        }

        private InventoryItem[] _slots; 

        public Inventory(long id, bool keepPersisted)
        {
            Id = id;
            KeepPersisted = keepPersisted;
        }

        public byte GetInventorySize()
        {
            if (_slots == null)
                return 0;
            return (byte)_slots.Length;
        }

        public Dictionary<byte, InventoryItem> GetContent()
        {
            var ret = new Dictionary<byte, InventoryItem>();
            if (_slots == null)
                return ret;
            
            for (var i=0; i<_slots.Length; ++i)
                ret.Add((byte)i, _slots[i]);
            return ret;
        }

        public InventoryItem this [byte slotIndex]
        {
            get
            {
                if (slotIndex >= GetInventorySize())
                    return null;
                return _slots[slotIndex];
            }
            set
            {
                SetItem(slotIndex, value);
            }
        }

        public InventoryItem this [InventorySpecialSlotEnum item]
        {
            get { return this[(byte) item]; }
            set
            {
                this[(byte)item] = value; }
        }

        public void Initialise(byte size, InventoryType inventoryType)
        {
            _slots = new InventoryItem[size];
            InventoryType = inventoryType;
        }

        private void StackChanged(byte slotIndex)
        {
            if (_slots[slotIndex] != null && _slots[slotIndex].GetCount() == 0)
                _slots[slotIndex] = null;

            if (OnSlotChanged != null)
            {
                OnSlotChanged(this, slotIndex);
            }
        }

        private void StackChanged(Inventory inventory, InventoryItem item)
        {
            for (var i = 0; i < GetInventorySize(); ++i)
            {
                if (_slots[i] != item)
                    continue;

                if (item.GetCount() == 0)
                    _slots[i] = null;

                if (OnSlotChanged != null)
                {
                    OnSlotChanged(this, (byte) i);
                }
                return;
            }
        }

        public void CombineStacks(InventoryItem destination, InventoryItem source)
        {
            if (destination == null || source == null || destination.ItemSpecId != source.ItemSpecId)
                return;
            if (destination.HasDurability() || source.HasDurability())
                return;

            var maxStackSize = StaticItemSpecCache.Instance.GetItemSpec(destination.ItemSpecId).GetStackMax();
            var count = destination.GetCount();
            var amountToAdd = source.GetCount();

            if (count + amountToAdd > maxStackSize)
            {
                destination.SetCount(maxStackSize);
                source.SetCount((short)(amountToAdd + count - maxStackSize));
            }
            else
            {
                destination.SetCount((short)(count + amountToAdd));
                source.SetCount(0);
            }

            if (destination.Inventory != null)
            {
                destination.Inventory.StackChanged(destination.Inventory, destination);
            }

            if (source.Inventory != null)
            {
                source.Inventory.StackChanged(source.Inventory, source);
            }
        }

        public int TotalAmount(int itemSpecId)
        {
            return _slots
                .Where(i => i != null && i.ItemSpecId == itemSpecId)
                .Sum(c => c.GetCount());
        }

        /// <summary>
        /// Add an InventoryItem to Inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="forceAdd">Optional ignore CanInventoryItemBePutIntoSlot</param>
        /// <param name="targetSlotIndex">Optional target slot</param>
        /// <returns>Whether the inventory item was completely added or not</returns>
        public bool AddItem(InventoryItem item, bool forceAdd = false, byte? targetSlotIndex = null)
        {
            if (!CanAddItemToInventory() && !forceAdd)
                return false;

            var canStack = !item.HasDurability();

            if (canStack)
                MergeItem(item);

            if (item.GetCount() == 0)
                return true;

            if (targetSlotIndex.HasValue)
            {
                if (_slots[targetSlotIndex.Value] != null)
                {
                    CombineStacks(_slots[targetSlotIndex.Value], item);
                }
            }

            for (var slotIndex = 0; slotIndex < _slots.Length; ++slotIndex)
            {
                if (_slots[slotIndex] != null && _slots[slotIndex].GetCount() > 0)
                    continue;

                if (!CanSlotAccomodateItem(slotIndex, item))
                    continue;

                _slots[slotIndex] = item.Clone(this);
                _slots[slotIndex].OnItemChanged += StackChanged;

                if (item.Inventory != null)
                {
                    item.SetCount(0);    
                }

                StackChanged((byte)slotIndex);
                return true;
            }

            return false;
        }

        private void MergeItem(InventoryItem item)
        {
            foreach (var existingItem in _slots.Where(i => i != null))
            {
                if (item.GetCount() == 0)
                    return;

                CombineStacks(existingItem, item);
            }
        }

        public void ClearItems()
        {
            var count = _slots.Length;
            for(var i=0; i<count; ++i)
                SetItem((byte)i, null);
        }

        /// <summary>
        /// Copy an InventoryItem into a slot of this Inventory.
        /// </summary>
        /// <param name="slotIndex">Slot index</param>
        /// <param name="item">InventoryItem to use as a source</param>
        /// <returns>The cloned InventoryItem</returns>
        public InventoryItem SetItem(byte slotIndex, InventoryItem item)
        {
            if (item != null)
            {
                if (item.GetCount() == 0)
                {
                    _slots[slotIndex] = null;
                }
                else
                {
                    _slots[slotIndex] = item.Clone(this);
                    _slots[slotIndex].OnItemChanged += StackChanged;
                }
            }
            else
            {
                _slots[slotIndex] = null;    
            }
            
            StackChanged(slotIndex);

            return _slots[slotIndex];
        }

        public bool CanSlotAccomodateItem(int slotIndex, InventoryItem inventoryItem)
        {
            if (InventoryType != InventoryType.PlayerBackpack)
                return true;

            var itemSpec = StaticItemSpecCache.Instance.GetItemSpec(inventoryItem.ItemSpecId);            

            switch (slotIndex)
            {
                case (int)InventorySpecialSlotEnum.PrimaryWeapon:
                case (int)InventorySpecialSlotEnum.SecondaryWeapon:
                    return (itemSpec.IsWeapon());
                case (int)InventorySpecialSlotEnum.HeadArmour:
                    return (itemSpec.IsHeadwear());
                case (int)InventorySpecialSlotEnum.BodyArmour:
                    return (itemSpec.IsBodywear());
                case (int)InventorySpecialSlotEnum.LegArmour:
                    return (itemSpec.IsLegwear());
                case (int)InventorySpecialSlotEnum.FootArmour:
                    return (itemSpec.IsFootwear());
                default:
                    return slotIndex >= (short)InventorySpecialSlotEnum.ContainerStart;
            }
        }

        public bool CanAddItemToInventory()
        {
            switch (InventoryType)
            {
                case InventoryType.Stash:
                    return false;
                case InventoryType.PlayerBackpack:
                    return true;
                case InventoryType.Corpse:
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get a slot index that this inventory item can be put into. Partially
        /// filled slots of the same item are considered even if the specified
        /// item will be split.
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Slot index or null if no space is available</returns>
        public byte? GetFreeSlotForItem(InventoryItem item)
        {
            for (byte slotIndex = 0; slotIndex < _slots.Length; slotIndex++)
            {
                if (!CanSlotAccomodateItem(slotIndex, item))
                {
                    continue;
                }

                if (_slots[slotIndex] == null ||
                    _slots[slotIndex].ItemSpecId == item.ItemSpecId)
                {
                    return slotIndex;
                }
            }

            return null;
        }
    }
}

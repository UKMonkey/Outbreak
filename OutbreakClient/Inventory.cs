using System;
using System.Collections.Generic;
using System.Linq;

namespace Daybreak
{
    public class Inventory : Dictionary<byte, InventoryEntry>
    {
        public Inventory() : base(3) {}

        public new ushort this[byte index]
        {
            get
            {
                return base[index].Quantity;
            }
            set
            {
                if (ContainsKey(index))
                {
                    base[index].Quantity = value;
                }
                else
                {
                    base[index] = new InventoryEntry(value);

                    if (Count > byte.MaxValue)
                    {
                        // inventory is full. remove a dead entry.
                        var result = this.Where(x => x.Value.Quantity == 0 && !x.Value.IsDirty).ToList();
                        if (result.Count > 0)
                        {
                            foreach (var deadEntry in result)
                            {
                                Remove(deadEntry.Key);
                            }
                        }
                        else
                        {
                            throw new Exception("Inventory can only support byte.MaxValue entries");
                        }

                        
                    }
                }
            }
        }

        public byte DirtyEntryCount
        {
            get
            {
                return (byte)this.Count(x => x.Value.IsDirty);
            }
        }

        /// <summary>
        /// Increase the players inventory quantity of a particular item.
        /// </summary>
        /// <param name="itemTypeId"></param>
        /// <param name="itemQuantity"></param>
        /// <returns>False if the inventory is full.</returns>
        public bool Increase(byte itemTypeId, ushort itemQuantity)
        {
            if (ContainsKey(itemTypeId))
            {
                if (base[itemTypeId].Quantity == ushort.MaxValue)
                    return false;

                var currentQuantity = this[itemTypeId];
                var newQuantity = (ushort)(currentQuantity + itemQuantity);
                this[itemTypeId] = newQuantity;
            }
            else
            {
                this[itemTypeId] = itemQuantity;
            }

            return true;
        }

        /// <summary>
        /// Decrease the players inventory quantity of a particular item.
        /// </summary>
        /// <param name="itemTypeId"></param>
        /// <param name="itemQuantity"></param>
        /// <returns>False if the inventory contains an insufficient amount</returns>
        public bool Decrease(byte itemTypeId, ushort itemQuantity)
        {
            if (!ContainsKey(itemTypeId))
                return false;

            var currentAmount = base[itemTypeId].Quantity;

            if (currentAmount < itemQuantity)
                return false;

            var newAmount = (ushort)(currentAmount - itemQuantity);
            base[itemTypeId].Quantity = newAmount;

            return true;
        }
    }

}

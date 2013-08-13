namespace Daybreak
{
    public class InventoryEntry
    {
        private ushort _quantity;
        public ushort Quantity
        {
            get
            {
                return _quantity;
            }
            set {
                if (_quantity == value)
                    return;
                IsDirty = true;
                _quantity = value;
                Psy.Logging.Logger.WriteString(string.Format("Quantity updated to {0}", _quantity));
            }
        }

        public bool IsDirty { get; set; }

        public InventoryEntry(ushort quantity)
        {
            Quantity = quantity;
            IsDirty = true;
        }
    }
}
namespace Outbreak.Client.Gui
{
    public struct InventorySlotMetadata
    {
        public readonly long InventoryId;
        public readonly byte SlotId;

        public InventorySlotMetadata(long inventoryId, byte slotId)
        {
            InventoryId = inventoryId;
            SlotId = slotId;
        }
    }
}
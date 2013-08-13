using Vortex.Interface.EntityBase;
using Outbreak.Items.Containers.FloatingItems;
using Outbreak.Items.Containers.InventoryItems;

namespace Outbreak.Client.Items.Containers
{
    public class FloatingItemCache : IFloatingItemCache
    {
        public event FloatingItemCallback OnAdded;
        public event FloatingItemCallback OnRemoved;

        public int UnknownId
        {
            get { return -1; }
        }

        public InventoryItem GetItem(int id)
        {
            throw new System.NotImplementedException();
        }

        public void RegisterItem(InventoryItem item, Entity target)
        {
            throw new System.NotImplementedException();
        }

        public void ForceAddItem(int id, InventoryItem item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveFloatingItem(int getFloatingItemId)
        {
        }
    }
}

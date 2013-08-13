using Vortex.Interface.EntityBase;
using Outbreak.Items.Containers.InventoryItems;

namespace Outbreak.Items.Containers.FloatingItems
{
    public delegate void FloatingItemCallback(int id, InventoryItem item);

    /**  
     * keeps track of any items that are part of an entity
     * but not an inventory
     * ie an item that has been dropped on the floor
    */
    public interface IFloatingItemCache
    {
        event FloatingItemCallback OnAdded;
        event FloatingItemCallback OnRemoved;

        int UnknownId { get; }

        /// <summary>
        /// Does not delete the item from the cache!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        InventoryItem GetItem(int id);

        /// <summary>
        /// add an item to the cache
        /// </summary>
        /// <param name="item"></param>
        /// <param name="target"></param>
        void RegisterItem(InventoryItem item, Entity target);


        // used for restoring data that has been previously saved
        // not for use other than by persistance handlers!
        void ForceAddItem(int id, InventoryItem item);
        void RemoveFloatingItem(int itemId);
    }
}

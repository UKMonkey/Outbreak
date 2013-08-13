using System;
using System.Collections.Generic;
using Vortex.Interface.EntityBase;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers.FloatingItems;
using Outbreak.Items.Containers.InventoryItems;

namespace Outbreak.Server.Items.Containers
{
    public class FloatingItemCache : IFloatingItemCache
    {
        public event FloatingItemCallback OnAdded;
        public event FloatingItemCallback OnRemoved;

        private readonly Dictionary<int, InventoryItem> _cache;
        private int _next;

        private bool _allowForceAdds = true;

        public FloatingItemCache()
        {
            _cache = new Dictionary<int, InventoryItem>();
        }

        public int UnknownId
        {
            get { return -1; }
        }

        public InventoryItem GetItem(int id)
        {
            var item = _cache[id];
            return item;
        }

        public int RegisterItem(InventoryItem item)
        {
            _allowForceAdds = false;

            while (_cache.ContainsKey(_next))
            {
                ++_next;
                if (_next == UnknownId)
                    ++_next;
            }

            _cache.Add(_next, item);

            if (OnAdded != null)
                OnAdded(_next, item);

            return _next;
        }

        public void RegisterItem(InventoryItem item, Entity target)
        {
            var id = RegisterItem(item);
            target.SetFloatingItemId(id);
        }

        public void ForceAddItem(int id, InventoryItem item)
        {
            if (!_allowForceAdds)
                throw new Exception("Unable to safely force add items any more");
            
            _cache[id] = item;
            _next = Math.Max(_next, id);

            if (OnAdded != null)
                OnAdded(_next, item);
        }

        public void RemoveFloatingItem(int itemId)
        {
            if (!_cache.ContainsKey(itemId))
                return;

            var item = _cache[itemId];
            _cache.Remove(itemId);

            if (OnRemoved != null)
                OnRemoved(itemId, item);
        }
    }
}

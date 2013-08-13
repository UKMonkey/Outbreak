using System.Collections.Generic;
using System.Linq;
using Psy.Core.Logging;
using Vortex.Interface;
using Vortex.Interface.Net;
using Outbreak.Items.Containers;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Net.Messages;

namespace Outbreak.Server.Items.Containers
{
    public class InventoryCache : IInventoryCache
    {
        private readonly HashSet<long> _availableIds;  
        private readonly Dictionary<long, Inventory> _inventories;
        private readonly IServer _engine;

        // TODO - tie into when a client disconnects and remove them from this dic.
        private readonly Dictionary<long, HashSet<RemotePlayer>> _subscriptions;

        public long UnknownId {get{return -1;}}
        private long _nextInventoryId;

        private readonly IInventoryLoader _loader;
        private readonly IInventorySaver _saver;


        private class NullLoader : IInventoryLoader
        {
            public Inventory LoadInventory(long id)
            {
                return null;
            }

            public HashSet<long> GetAvailableIds()
            {
                return new HashSet<long>();
            }
        }

        private class NullSaver : IInventorySaver
        {
            public void SaveInventory(Inventory item)
            {
            }

            public void DeleteInventory(Inventory inventoryId)
            {
            }
        }


        public InventoryCache(IServer engine, IInventoryLoader loader, IInventorySaver saver)
        {
            _inventories = new Dictionary<long, Inventory>();
            _engine = engine;
            _subscriptions = new Dictionary<long, HashSet<RemotePlayer>>();

            _loader = loader ?? new NullLoader();
            _saver = saver ?? new NullSaver();

            _availableIds = _loader.GetAvailableIds();
            if (_availableIds.Count > 0)
                _nextInventoryId = _availableIds.Max() + 1;

            _engine.RegisterMessageCallback(typeof(ClientRequestInventoryUpdates), HandlePerformSubscription);
            _engine.RegisterMessageCallback(typeof(ClientCancelInventoryUpdates), HandleCancelSubscription);
        }

        public Inventory GetInventory(long inventoryId)
        {
            if (inventoryId == UnknownId)
                return null;

            if (_inventories.ContainsKey(inventoryId))
                return _inventories[inventoryId];

            if (_availableIds.Contains(inventoryId))
                return LoadInventory(inventoryId);

            return null;
        }

        private Inventory LoadInventory(long id)
        {
            var inventory = _loader.LoadInventory(id);
            _inventories[id] = inventory;
            inventory.OnSlotChanged += OnInventoryChanged;
            _availableIds.Remove(id);

            return inventory;
        }

        public Inventory CreateNewInventory(bool keepPersisted)
        {
            while (_inventories.ContainsKey(_nextInventoryId) ||
                _availableIds.Contains(_nextInventoryId))
                ++_nextInventoryId;

            var ret = new Inventory(_nextInventoryId++, keepPersisted);
            _inventories[ret.Id] = ret;
            ret.OnSlotChanged += OnInventoryChanged;

            _saver.SaveInventory(ret);

            return ret;
        }

        public void RemoveInventory(long inventoryId)
        {
            if (!_inventories.ContainsKey(inventoryId))
                return;

            var inventory = _inventories[inventoryId];
            _inventories.Remove(inventoryId);
            _saver.DeleteInventory(inventory);
        }

        private void HandlePerformSubscription(Message msg)
        {
            var message = (ClientRequestInventoryUpdates) msg;

            if (_availableIds.Contains(message.InventoryId))
                LoadInventory(message.InventoryId);

            if (!_inventories.ContainsKey(message.InventoryId))
                return;

            if (!_subscriptions.ContainsKey(message.InventoryId))
                _subscriptions.Add(message.InventoryId, new HashSet<RemotePlayer>());

            if (_subscriptions[message.InventoryId].Contains(message.Sender))
                return;

            _subscriptions[message.InventoryId].Add(message.Sender);
            SendFullInventory(message.Sender, GetInventory(message.InventoryId));
        }

        private void HandleCancelSubscription(Message msg)
        {
            var message = (ClientCancelInventoryUpdates) msg;
            if (!_subscriptions.ContainsKey(message.InventoryId))
                return;

            _subscriptions[message.InventoryId].Remove(msg.Sender);
        }

        private void SendFullInventory(RemotePlayer target, Inventory inventory)
        {
            var msg = new ServerInventoryStatusMessage 
                {
                    PartialUpdate = false, 
                    InventorySize = inventory.GetInventorySize(),
                    InventoryId = inventory.Id,
                    InventoryType = inventory.InventoryType,
                    InventoryContent = inventory.GetContent()
                };

            _engine.SendMessageToClient(msg, target);
        }

        private void OnInventoryChanged(Inventory inventory, byte item)
        {
            _saver.SaveInventory(inventory);

            if (!_subscriptions.ContainsKey(inventory.Id))
            {
                Logger.Write(string.Format("Inventory #{0} changed with no subscribers", inventory.Id));
                return;
            }
                
            var listeners = _subscriptions[inventory.Id];

            var msg = new ServerInventoryStatusMessage
                {
                    PartialUpdate = true,
                    InventorySize = inventory.GetInventorySize(),
                    InventoryId = inventory.Id,
                    InventoryType = inventory.InventoryType,
                    InventoryContent = new Dictionary<byte, InventoryItem>{{item, inventory[item]}}
                };

            Logger.Write(string.Format("Inventory #{0} updated, informing {1} subscribers", inventory.Id, listeners.Count));

            foreach (var listener in listeners)
            {
                _engine.SendMessageToClient(msg, listener);
            }
        }
    }
}

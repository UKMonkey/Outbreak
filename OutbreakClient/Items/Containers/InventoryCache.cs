using System;
using System.Collections.Generic;
using Psy.Core.Logging;
using Vortex.Interface;
using Vortex.Interface.Net;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers;
using Outbreak.Net.Messages;
using Psy.Core;

namespace Outbreak.Client.Items.Containers
{
    public class InventoryCache : IInventoryCache
    {
        private readonly Dictionary<long, Inventory> _inventories;
        private readonly Dictionary<long, double> _subscribeTime;
        private const long MaxSubscribeTime = 60*1000;
        private readonly IClient _engine;

        public long UnknownId
        {
            get { return -1; }
        }

        public InventoryCache(IClient engine)
        {
            _inventories = new Dictionary<long, Inventory>();
            _subscribeTime = new Dictionary<long, double>();
            _engine = engine;

            _engine.RegisterMessageCallback(typeof (ServerInventoryStatusMessage), HandleInventoryUpdated);
        }

        public Inventory GetInventory(long inventoryId)
        {
            if (inventoryId == UnknownId)
                return null;

            if (!_subscribeTime.ContainsKey(inventoryId))
                PerformInventorySubscription(inventoryId);

            _subscribeTime[inventoryId] = Timer.GetTime();

            if (_inventories.ContainsKey(inventoryId))
                return _inventories[inventoryId];

            var ret = new Inventory(inventoryId, false);
            _inventories[ret.Id] = ret;
            ret.OnSlotChanged += OnSlotChanged;

            return ret;
        }

        public Inventory CreateNewInventory(bool keepPersisted)
        {
            throw new InvalidOperationException();
        }

        public void RemoveInventory(long inventoryId)
        {
            _inventories.Remove(inventoryId);
        }

        private void PerformInventorySubscription(long id)
        {
            Logger.Write(string.Format("Requesting inventory updates for #{0}", id));

            var msg = new ClientRequestInventoryUpdates {InventoryId = id};
            _engine.SendMessage(msg);
        }

        private void HandleInventoryUpdated(Message msg)
        {
            var message = (ServerInventoryStatusMessage) msg;
            var inventory = GetInventory(message.InventoryId);

            Logger.Write(string.Format("Inventory update for #{0}. Partial = {1}. Item# = {2}", 
                message.InventoryId, message.PartialUpdate, message.InventoryContent.Count));

            if (!message.PartialUpdate)
            {
                inventory.Initialise(message.InventorySize, message.InventoryType);
            }

            foreach (var item in message.InventoryContent)
            {
                inventory.SetItem(item.Key, item.Value);
            }
        }

        private void OnSlotChanged(Inventory item, byte changed)
        {
            if (_engine.Me != null &&
                _engine.Me.GetInventoryId() == item.Id)
                return;

            if (!_subscribeTime.ContainsKey(item.Id))
                return;

            var time = Timer.GetTime() - MaxSubscribeTime;

            if (_subscribeTime[item.Id] >= time)
                return;

            _subscribeTime.Remove(item.Id);

            Logger.Write("Cancelling Inventory Updates");

            var msg = new ClientCancelInventoryUpdates {InventoryId = item.Id};
            _engine.SendMessage(msg);
        }
    }
}

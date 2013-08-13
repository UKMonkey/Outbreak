using System;
using System.Collections.Generic;
using System.Linq;
using Vortex.Interface;
using Vortex.Interface.Net;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Net.Messages;

namespace Outbreak.Server.Items.Containers
{
    public class ItemSpecCache : IItemSpecCache
    {
        public event ItemSpecCallback OnItemAdded;

        private readonly IServer _engine;
        private readonly Dictionary<int, ItemSpec> _specs;
        private int _nextSpec;

        public ItemSpecCache(IServer engine, IItemSpecLoader loader)
        {
            _engine = engine;
            _engine.RegisterMessageCallback(typeof(ClientItemSpecMessage), HandleSpecRequest);

            if (loader != null)
                _specs = loader.LoadSpecs();
            else
                _specs = new Dictionary<int, ItemSpec>();

            if (_specs.Count > 0)
                _nextSpec = _specs.Keys.Max() + 1;
            else
                _nextSpec = 1;
        }

        public int EmptySpecId
        {
            get { return -1; }
        }

        public ItemSpec GetItemSpec(int id)
        {
            return _specs[id];
        }


        // lock before calling this!
        private ItemSpec GetDuplicate(ItemSpec spec)
        {
            foreach (var otherSpec in _specs.Values)
            {
                if (otherSpec == spec)
                    return otherSpec;
            }
            return null;
        }

        public ItemSpec AddSpec(ItemSpec spec)
        {
            // todo: the cloning behaviour of the ItemSpec is a bit, confusing.

            lock (this)
            {
                // if the spec is already available, then return the old one and don't add this new one
                var duplicate = GetDuplicate(spec);
                if (duplicate != null)
                    return duplicate;

                var id = ++_nextSpec;
                spec = spec.Clone(id);
                _specs.Add(id, spec);
            }

            OnItemAdded(spec);
            return spec;
        }

        public IEnumerable<ItemSpec> GetSpecsOfType(ItemSpecChecker checker)
        {
            return _specs.Values.Where(item => checker(item));
        }

        private void HandleSpecRequest(Message msg)
        {
            var message = (ClientItemSpecMessage) msg;
            if (!_specs.ContainsKey(message.ItemSpecId))
                return;
            var spec = _specs[message.ItemSpecId];
            SendUpdate(spec, msg.Sender);
        }

        private void SendUpdate(ItemSpec spec, RemotePlayer target = null)
        {
            if (spec == null)
                return;

            var msg = new ServerItemSpecMessage {ItemSpec = spec};
            if (target == null)
                _engine.SendMessage(msg);
            else
                _engine.SendMessageToClient(msg, target);
        }
    }
}

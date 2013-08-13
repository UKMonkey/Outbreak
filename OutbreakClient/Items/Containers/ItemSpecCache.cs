using System;
using System.Collections.Generic;
using System.Linq;
using Vortex.Interface;
using Vortex.Interface.Net;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Net.Messages;

namespace Outbreak.Client.Items.Containers
{
    public class ItemSpecCache : IItemSpecCache
    {
        public event ItemSpecCallback OnItemAdded;

        private readonly Dictionary<int, ItemSpec> _specs;
        private readonly IEngine _engine;

        public ItemSpecCache(IEngine engine)
        {
            _specs = new Dictionary<int, ItemSpec>();
            _engine = engine;
            _engine.RegisterMessageCallback(typeof(ServerItemSpecMessage), HandleItemSpecMessage);
        }

        private void HandleItemSpecMessage(Message msg)
        {
            var message = (ServerItemSpecMessage) msg;
            _specs.Remove(message.ItemSpec.Id);
            _specs.Add(message.ItemSpec.Id, message.ItemSpec);

            if (OnItemAdded != null)
                OnItemAdded(message.ItemSpec);
        }

        public int EmptySpecId
        {
            get { return -1; }
        }

        /// <summary>
        /// Request an item spec.
        /// </summary>
        /// <param name="id">Item spec id</param>
        /// <returns>ItemSpec instance or null</returns>
        public ItemSpec GetItemSpec(int id)
        {
            // todo: only send one request.
            if (_specs.ContainsKey(id))
                return _specs[id];
            
            _engine.SendMessage(new ClientItemSpecMessage {ItemSpecId = id});
            return null;
        }

        public ItemSpec AddSpec(ItemSpec spec)
        {
            throw new InvalidOperationException("Cannot AddSpec on client.");
        }

        public IEnumerable<ItemSpec> GetSpecsOfType(ItemSpecChecker checker)
        {
            return _specs.Values.Where(item => checker(item));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Outbreak.Items.Containers;
using Psy.Core.Logging;
using Vortex.Interface.Net;
using Outbreak.Items.Containers.InventoryItems;

namespace Outbreak.Net.Messages
{
    public class ServerInventoryStatusMessage : Message
    {
        private const int EmptyInventorySlotSpecId = -1;

        public IDictionary<byte, InventoryItem> InventoryContent { get; set; }
        public long InventoryId { get; set; }
        public bool PartialUpdate { get; set; }

        // only used if partialUpdate is false
        public byte InventorySize { get; set; }

        public InventoryType InventoryType { get; set; }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            PartialUpdate = messageStream.ReadBoolean();

            if (!PartialUpdate)
                InventorySize = messageStream.ReadByte();

            InventoryId = messageStream.ReadInt64();
            InventoryType = (InventoryType)messageStream.ReadInt16();
            InventoryContent = ReadInventoryContent(messageStream, InventoryId);
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteBool(PartialUpdate);

            if (!PartialUpdate)
                messageStream.WriteByte(InventorySize);

            messageStream.WriteInt64(InventoryId);
            messageStream.WriteInt16((short)InventoryType);
            Write(messageStream, InventoryContent);
            
        }

        static void Write(IOutgoingMessageStream stream, IEnumerable<KeyValuePair<byte, InventoryItem>> data)
        {
            var toSend = data.ToList();

            var count = (short) toSend.Count;
            stream.WriteInt16(count);

            Logger.Write(string.Format("Writing {0} inventory items", count));

            var i = 0;
            foreach (var item in toSend)
            {
                Logger.Write(string.Format("Writing inventory item {0}", i));
                stream.WriteByte(item.Key);
                WriteInventoryItem(stream, item.Value);
                i++;
            }
        }

        static Dictionary<byte, InventoryItem> ReadInventoryContent(IIncomingMessageStream stream, long inventoryId)
        {
            var count = stream.ReadInt16();
            var ret = new Dictionary<byte, InventoryItem>(count);

            Logger.Write(string.Format("Reading {0} inventory slots", count));

            for (var i=0; i<count; ++i)
            {
                var key = stream.ReadByte();

                Logger.Write(string.Format("Reading inventory slot {0}", key));

                var value = ReadInventoryItem(stream, inventoryId);

                Logger.Write(string.Format("Finished inventory slot {0}.", key));

                ret[key] = value;
            }

            return ret;
        }

        static InventoryItem ReadInventoryItem(IIncomingMessageStream stream, long inventoryId)
        {
            var specId = stream.ReadInt32();

            if (specId == EmptyInventorySlotSpecId)
            {
                Logger.Write("Reading empty inventory item");
                return null;
            }

            Logger.Write(string.Format("Reading inventory item with item spec id {0}", specId));

            var properties = stream.ReadEntityProperties<InventoryItemProperty>();

            var ret = new InventoryItem(null, specId);
            foreach (var item in properties)
                ret.SetProperty(item);

            return ret;
        }

        static void WriteInventoryItem(IOutgoingMessageStream stream, InventoryItem inventoryItem)
        {
            if (inventoryItem == null)
            {
                Logger.Write("Writing empty inventory item");
                stream.WriteInt32(EmptyInventorySlotSpecId);
                return;
            }

            Logger.Write(string.Format("Writing inventory item with item spec id {0}", inventoryItem.ItemSpecId));

            if (inventoryItem.ItemSpecId == 0)
            {
                throw new Exception("ItemSpec has id of 0. Make sure you use correct StaticItemSpecCache ritual.");
            }

            stream.WriteInt32(inventoryItem.ItemSpecId);
            stream.Write(inventoryItem.GetProperties().ToList());
        }
    }
}

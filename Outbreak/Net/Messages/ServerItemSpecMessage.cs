using System.Linq;
using System.Threading;
using Vortex.Interface.Net;
using Outbreak.Items.Containers.InventorySpecs;

namespace Outbreak.Net.Messages
{
    public class ServerItemSpecMessage : Message
    {
        public ItemSpec ItemSpec { get; set; }

        protected override void DeserializeImpl(IIncomingMessageStream messageStream)
        {
            var id = messageStream.ReadInt32();
            var properties = messageStream.ReadEntityProperties<ItemSpecProperty>();

            ItemSpec = new ItemSpec(id);
            foreach (var prop in properties)
                ItemSpec.SetProperty(prop);
        }

        protected override void SerializeImpl(IOutgoingMessageStream messageStream)
        {
            messageStream.WriteInt32(ItemSpec.Id);
            messageStream.Write(ItemSpec.GetProperties().ToList());
        }
    }
}

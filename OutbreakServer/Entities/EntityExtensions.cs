using Outbreak.Entities.Properties;
using Outbreak.Items.Containers.FloatingItems;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Psy.Core;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Properties;

namespace Outbreak.Server.Entities
{
    public static class EntityExtensions
    {
        public static void SetInventoryItem(this Entity entity, InventoryItem item, ItemSpec itemSpec=null)
        {
            StaticFloatingItemCache.Instance.RegisterItem(item, entity);

            if (itemSpec == null)
                itemSpec = item.GetItemSpec();

            entity.SetProperty(new EntityProperty((short)GameEntityPropertyEnum.ItemSpecId, itemSpec.Id));
            entity.SetNameplate(itemSpec.GetName());
            entity.SetModel(itemSpec.GetModelName());
            entity.SetNameplateColour(Colours.Yellow);
        }
    }
}

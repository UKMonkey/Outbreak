using Outbreak.Items.Containers;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Psy.Core;
using Psy.Gui.Components;
using Vortex.Interface;

namespace Outbreak.Client.Gui
{
    public class InventoryWidgetTooltipFactory
    {
        private readonly IClient _client;

        public InventoryWidgetTooltipFactory(IClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Can be null if the item spec doesn't exist on the client.
        /// </summary>
        /// <param name="itemSpecId"></param>
        /// <returns></returns>
        private static ItemSpec GetItemSpec(int itemSpecId)
        {
            var specCache = StaticItemSpecCache.Instance;
            var spec = specCache.GetItemSpec(itemSpecId);
            return spec;
        }

        public Widget WidgetTooltipFactory(Widget parent)
        {
            var slotMetadata = ((InventorySlotMetadata) parent.Metadata);
            var slotId = slotMetadata.SlotId;
            var inventoryContent = StaticInventoryCache.Instance.GetInventory(slotMetadata.InventoryId).GetContent();
            var inventoryItem = slotId > inventoryContent.Count - 1 ? null : inventoryContent[slotId];

            if (inventoryItem == null)
                return null;

            var itemSpec = GetItemSpec(inventoryItem.ItemSpecId);
            if (itemSpec == null)
                return null;

            var tooltip = _client.GuiLoader.Load("tooltip.xml", null);

            var list = tooltip.FindWidgetByClass<TextList>("itemSpecDetails");

            list.AddLine(itemSpec.GetName(), Colours.Green, fontSize: 19);

            if (itemSpec.IsWeapon())
            {
                if (itemSpec.HasProperty(ItemSpecPropertyEnum.ClipSize))
                {
                    var clipSize = itemSpec.GetClipSize();
                    list.AddLine(string.Format("{0} rounds", clipSize), Colours.Orange);
                }

                if (itemSpec.HasProperty(ItemSpecPropertyEnum.AmmoType))
                {
                    var ammoType = itemSpec.GetAmmoType();
                    var ammoTypeName = ammoType.Value.ToString();
                    list.AddLine(string.Format("Ammo type: {0}", ammoTypeName), Colours.Orange, fontSize: 15);
                }

                if (itemSpec.HasProperty(ItemSpecPropertyEnum.ReloadTime))
                {
                    var reloadTime = itemSpec.GetReloadTime();
                    list.AddLine(string.Format("Reload time: {0:0.00}s", reloadTime / 1000f), Colours.White, fontSize: 15);
                }

                if (itemSpec.HasProperty(ItemSpecPropertyEnum.BulletSpread))
                {
                    var spread = itemSpec.GetBulletSpread();
                    list.AddLine(string.Format("Bullet spread: {0:0.00}", spread), Colours.White, fontSize: 15);
                }
            }
            else if (itemSpec.IsAmmo())
            {
                var dmgMin = itemSpec.GetDamageMin();
                var dmgMax = itemSpec.GetDamageMax();

                var damageString = string.Format("{0:0.00} to {1:0.00} damage", dmgMin, dmgMax);
                list.AddLine(damageString, Colours.Orange, fontSize: 15);

                var stackSize = inventoryItem.GetCount();

                list.AddLine(string.Format("{0}/{1} in this stack", stackSize, itemSpec.GetStackMax()), Colours.LightBlue);
            }
            else if (itemSpec.HasProperty(ItemSpecPropertyEnum.HealAmount))
            {
                var healAmount = itemSpec.GetHealAmount();

                list.AddLine(string.Format("Heal amount: {0}", healAmount), Colours.White, fontSize: 15);
            }

            if (itemSpec.HasProperty(ItemSpecPropertyEnum.Description))
            {
                var description = itemSpec.GetDescription().Split('\n');

                foreach (var line in description)
                {
                    list.AddLine(line, Colours.Grey, fontSize: 15, italic: true);
                }
            }

            return tooltip;
        }
    }
}
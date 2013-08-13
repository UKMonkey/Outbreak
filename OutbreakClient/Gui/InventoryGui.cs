using System;
using Outbreak.Audio;
using Psy.Core.Input;
using Vortex.Interface;
using Outbreak.Entities.Properties;
using Outbreak.Client.Gui.Widgets;
using Outbreak.Items.Containers;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Net.Messages;
using Psy.Gui.Components;
using Psy.Gui.Events;

namespace Outbreak.Client.Gui
{
    public class InventoryGui : IDisposable
    {
        private const string InventoryWindowName = "inventoryWindow";

        private readonly IClient _engine;
        private readonly InventoryWidgetTooltipFactory _tooltipFactory;
        private GuiWindow _inventoryWindow;

        public InventoryGui(IClient engine)
        {
            _engine = engine;
            _tooltipFactory = new InventoryWidgetTooltipFactory(_engine);
            CreateWindow();
        }

        public void Dispose()
        {
            _inventoryWindow.Delete();
            _engine.Gui.Desktop.DragDrop -= DesktopOnDragDrop;
        }

        private void CreateWindow()
        {
            _engine.GuiLoader.Load("playerInventory.xml", _engine.Gui.Desktop);
            _inventoryWindow = (GuiWindow)_engine.Gui.GetWidgetByName(InventoryWindowName);
            BindGuiEventHandlers();
        }

        private InventorySlot GetInventorySlotWidget(byte slotId)
        {
            var widget = (InventorySlot)_engine.Gui.GetWidgetByName(string.Format("inventorySlot{0}", slotId));
            return widget;
        }

        private InventoryItem GetInventoryItem(InventorySlotMetadata slotMetadata)
        {
            var inventory = StaticInventoryCache.Instance.GetInventory(slotMetadata.InventoryId);

            var inventoryContent = inventory.GetContent();
            return slotMetadata.SlotId > inventoryContent.Count - 1 ? null : inventoryContent[slotMetadata.SlotId];
        }

        /// <summary>
        /// Can be null if the item spec doesn't exist on the client.
        /// </summary>
        /// <param name="itemSpecId"></param>
        /// <returns></returns>
        private ItemSpec GetItemSpec(int itemSpecId)
        {
            var specCache = StaticItemSpecCache.Instance;
            var spec = specCache.GetItemSpec(itemSpecId);
            return spec;
        }

        private void BindGuiEventHandlers()
        {
            _engine.Gui.Desktop.DragDrop += DesktopOnDragDrop;

            byte j = 0;
            while (j < 32)
            {
                var widget = GetInventorySlotWidget(j);

                if (widget != null)
                {
                    widget.Visible = false;
                }

                j++;
            }

            for (byte i = 0; i < Consts.PlayerBackpackSize; i++)
            {
                var widget = GetInventorySlotWidget(i);

                if (widget == null)
                    continue;

                widget.Visible = true;
                widget.DragDrop += InventorySlotDragDrop;
                widget.MouseDown += InventorySlotUse;
                widget.MouseUp += CancelInventorySlotUse;
                widget.TooltipFactory = _tooltipFactory.WidgetTooltipFactory;
            }
        }

        private void CancelInventorySlotUse(object sender, MouseEventArguments mouseEventArguments)
        {
            if (mouseEventArguments.Button == MouseButton.Left)
                return;

            var msg = new ClientCancelUseItemMessage();
            _engine.SendMessage(msg);
        }

        private void InventorySlotUse(object sender, MouseEventArguments mouseEventArguments)
        {
            if (mouseEventArguments.Button == MouseButton.Left)
                return;

            var inventory = GetInventory();
            var slotId = ((InventorySlotMetadata) ((Widget) sender).Metadata).SlotId;

            var item = inventory[slotId];
            if (item == null)
                return;

            if (item.GetItemSpec().GetBaseUsageTime() < 0)
                return;

            var msg = new ClientUseItemMessage
            {
                InventoryId = GetInventory().Id,
                InventorySlotId = slotId
            };
            _engine.SendMessage(msg);
        }

        private void DesktopOnDragDrop(object sender, DragDropEventArgs args)
        {
            if (sender != args.Target) // Target should always be the Desktop
                return;

            var inventorySlotMetadata = ((InventorySlotMetadata) args.Dragged.Metadata);

            if (inventorySlotMetadata.InventoryId != GetInventory().Id)
                return;

            var inventoryDropToFloorRequest =
                new ClientInventoryDropToFloor
                {
                    SourceSlot = inventorySlotMetadata.SlotId,
                    InventoryId = inventorySlotMetadata.InventoryId
                };
            _engine.SendMessage(inventoryDropToFloorRequest);
        }

        private void InventorySlotDragDrop(object sender, DragDropEventArgs args)
        {
            if (sender != args.Target)
                return;

            var sourceMetadata = ((InventorySlotMetadata)args.Dragged.Metadata);
            var targetMetadata = ((InventorySlotMetadata)args.Target.Metadata);

            var inventoryItem = GetInventoryItem(sourceMetadata);
            if (!CanAddItemToInventory(targetMetadata, inventoryItem))
                return;

            _engine.AudioEngine.Play(Resources.Sound.UIClickSoundName, (int)AudioChannel.Interface);

            var inventoryDragDropRequest = 
                new ClientInventoryDragDrop
                {
                    SourceInventoryId = sourceMetadata.InventoryId,
                    SourceSlot = sourceMetadata.SlotId,
                    TargetInventoryId = targetMetadata.InventoryId,
                    TargetSlot = targetMetadata.SlotId
                };

            _engine.SendMessage(inventoryDragDropRequest);
        }

        private bool CanAddItemToInventory(InventorySlotMetadata targetMetadata, InventoryItem inventoryItem)
        {
            var inventory = GetInventory();

            return 
                inventory.CanSlotAccomodateItem(targetMetadata.SlotId, inventoryItem) && 
                inventory.CanAddItemToInventory();
        }

        public void Update()
        {
            if (GetInventory() == null)
                return;
            UpdateInventory();
        }

        private void UpdateInventory()
        {
            var inventory = GetInventory();
            var inventoryContent = inventory.GetContent();

            for (byte i = 0; i < Consts.PlayerBackpackSize; i++)
            {
                var widget = GetInventorySlotWidget(i);

                if (widget == null)
                    continue;

                if (inventoryContent.ContainsKey(i) && inventoryContent[i] != null)
                {
                    var spec = GetItemSpec(inventoryContent[i].ItemSpecId);
                    if (spec == null)
                        continue;

                    widget.ImageName = spec.GetProperty(ItemSpecPropertyEnum.ImageName).StringValue;
                    widget.StackSize = inventoryContent[i].GetCount();
                    widget.Draggable = true;
                    widget.Metadata = new InventorySlotMetadata(inventory.Id, i);
                }
                else
                {
                    widget.ImageName = "";
                    widget.StackSize = 0;
                    widget.Draggable = false;
                    widget.Metadata = new InventorySlotMetadata(inventory.Id, i);
                }
            }
        }

        private Inventory GetInventory()
        {
            var playerEntity = _engine.Me;
            var inventory = playerEntity == null ? null : playerEntity.GetInventory();

            if (inventory != null)
            {
                inventory.OnSlotChanged -= SlotChanged;
                inventory.OnSlotChanged += SlotChanged;
            }

            return inventory;
            
        }

        public void ToggleVisibility()
        {
            _inventoryWindow.Visible = !_inventoryWindow.Visible;

            if (_inventoryWindow.Visible)
            {
                UpdateInventory();
            }
        }

        /// <summary>
        /// Event handler
        /// </summary>
        private void SlotChanged(Inventory inventory, byte modified)
        {
            UpdateInventory();
        }

        public void Show()
        {
            _inventoryWindow.Visible = true;
            UpdateInventory();
        }

        public void Hide()
        {
            _inventoryWindow.Visible = false;
        }
    }
}
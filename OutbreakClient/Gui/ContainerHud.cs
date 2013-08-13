using System;
using Outbreak.Audio;
using Outbreak.Client.Gui.Widgets;
using Outbreak.Items.Containers;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Net.Messages;
using Psy.Core.Input;
using Psy.Gui.Components;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers.InventoryItems;
using Psy.Gui.Events;

namespace Outbreak.Client.Gui
{
    public class ContainerHud : IDisposable
    {
        private readonly GameClient _gameClient;
        private readonly InventoryWidgetTooltipFactory _tooltipFactory;
        private int? _containerEntityId;
        private Widget _window;

        public ContainerHud(GameClient gameClient)
        {
            _gameClient = gameClient;
            _tooltipFactory = new InventoryWidgetTooltipFactory(_gameClient.Engine);
        }

        public void Dispose()
        {
            if (_containerEntityId.HasValue)
            {
                GetInventory().OnSlotChanged -= SlotChanged;
                _containerEntityId = null;
            }

            DeleteExistingWindow();
            _gameClient.Engine.Gui.Desktop.DragDrop -= DesktopOnDragDrop;
        }

        public void ShowForContainer(int containerEntityId)
        {
            _containerEntityId = containerEntityId;

            GetInventory().OnSlotChanged += SlotChanged;

            DeleteExistingWindow();
            CreateWindow();

            _window.Visible = true;
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

        public Inventory GetInventory()
        {
            if (!_containerEntityId.HasValue)
                return null;

            var entity = _gameClient.Engine.GetEntity(_containerEntityId.Value);
            if (entity == null)
                return null;

            var inventory = entity.GetInventory();

            if (inventory == null)
                return null;

            return inventory;
        }

        /// <summary>
        /// Event handler
        /// </summary>
        private void SlotChanged(Inventory inventory, byte modified)
        {
            UpdateInventory();
        }

        private InventorySlot GetContainerSlotWidget(byte slotId)
        {
            var widget = (InventorySlot)_gameClient.Engine.Gui.GetWidgetByName(string.Format("chestSlot{0}", slotId));
            return widget;
        }

        private void CreateWindow()
        {
            _window = _gameClient.Engine.GuiLoader.Load("smallContainer.xml", _gameClient.Engine.Gui.Desktop);
            UpdateInventory();
        }

        private void BindGuiEventHandlers()
        {
            var inventory = GetInventory();
            if (inventory == null)
                return;

            var inventorySize = inventory.GetInventorySize();
            _gameClient.Engine.Gui.Desktop.DragDrop += DesktopOnDragDrop;

            for (byte i = 0; i < (byte)inventorySize; i++)
            {
                var widget = GetContainerSlotWidget(i);

                if (widget == null)
                {
                    continue;
                }

                widget.DragDrop += ContainerSlotDragDrop;
                widget.Click += ContainerSlotClick;
                widget.TooltipFactory = _tooltipFactory.WidgetTooltipFactory;
            }
        }

        private void ContainerSlotClick(object sender, ClickEventArgs args)
        {
            if (args.Button != MouseButton.Right)
            {
                return;
            }

            var metadata = ((InventorySlotMetadata)((Widget)sender).Metadata);
            var item = GetInventoryItem(metadata);
            
            if (item == null)
            {
                return;
            }

            var playerInventory = GetPlayerInventory();
            var slotId = playerInventory.GetFreeSlotForItem(item);
            if (slotId == null)
            {
                return;
            }

            var inventoryDragDropRequest =
                new ClientInventoryDragDrop
                {
                    SourceInventoryId = metadata.InventoryId,
                    SourceSlot = metadata.SlotId,
                    TargetInventoryId = playerInventory.Id,
                    TargetSlot = slotId.Value
                };

            _gameClient.Engine.SendMessage(inventoryDragDropRequest);

        }

        private Inventory GetPlayerInventory()
        {
            return _gameClient.Engine.Me.GetInventory();
        }

        private InventoryItem GetInventoryItem(InventorySlotMetadata slotMetadata)
        {
            var inventory = StaticInventoryCache.Instance.GetInventory(slotMetadata.InventoryId);

            var inventoryContent = inventory.GetContent();
            return slotMetadata.SlotId > inventoryContent.Count - 1 ? null : inventoryContent[slotMetadata.SlotId];
        }

        private void ContainerSlotDragDrop(object sender, DragDropEventArgs args)
        {
            if (sender != args.Target)
            {
                return;
            }

            var sourceMetadata = ((InventorySlotMetadata)args.Dragged.Metadata);
            var targetMetadata = ((InventorySlotMetadata)args.Target.Metadata);

            var inventoryItem = GetInventoryItem(sourceMetadata);
            if (!CanAddItemToInventory(targetMetadata, inventoryItem))
            {
                return;
            }

            _gameClient.Engine.AudioEngine.Play(Resources.Sound.UIClickSoundName, (int)AudioChannel.Interface);

            var inventoryDragDropRequest =
                new ClientInventoryDragDrop
                {
                    SourceInventoryId = sourceMetadata.InventoryId,
                    SourceSlot = sourceMetadata.SlotId,
                    TargetInventoryId = targetMetadata.InventoryId,
                    TargetSlot = targetMetadata.SlotId
                };

            _gameClient.Engine.SendMessage(inventoryDragDropRequest);
        }

        private bool CanAddItemToInventory(InventorySlotMetadata targetMetadata, InventoryItem inventoryItem)
        {
            return GetInventory().CanSlotAccomodateItem(targetMetadata.SlotId, inventoryItem) && GetInventory().CanAddItemToInventory();
        }

        private void DesktopOnDragDrop(object sender, DragDropEventArgs args)
        {
            if (sender != args.Target)
            {
                return;
            }

            var inventorySlotMetadata = ((InventorySlotMetadata) args.Dragged.Metadata);

            if (inventorySlotMetadata.InventoryId != GetInventory().Id)
            {
                return;
            }

            var inventoryDropToFloorRequest =
                new ClientInventoryDropToFloor
                {
                    SourceSlot = inventorySlotMetadata.SlotId,
                    InventoryId = inventorySlotMetadata.InventoryId
                };

            _gameClient.Engine.SendMessage(inventoryDropToFloorRequest);
        }

        private void UpdateInventory()
        {
            if (_containerEntityId == 0)
            {
                return;
            }

            BindGuiEventHandlers();
            var inventory = GetInventory();
            if (inventory == null)
            {
                Hide();
                return;
            }

            var inventoryContent = inventory.GetContent();
            var inventorySize = GetInventory().GetInventorySize();

            for (byte i = 0; i < (int)inventorySize; i++)
            {
                var widget = GetContainerSlotWidget(i);

                if (widget == null)
                {
                    continue;
                }

                if (inventoryContent.ContainsKey(i) && inventoryContent[i] != null)
                {
                    var spec = GetItemSpec(inventoryContent[i].ItemSpecId);
                    if (spec == null)
                    {
                        continue;
                    }

                    widget.ImageName = spec.GetImageName();
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

            if (inventory.InventoryType == InventoryType.Corpse || 
                inventory.InventoryType == InventoryType.Stash)
            {
                if (inventory.OccupiedSlots == 0)
                {
                    Hide();    
                }
            }
        }

        private void DeleteExistingWindow()
        {
            if (_window != null)
            {
                _window.Delete();
            }
        }

        public void Update()
        {
            UpdateInventory();
        }

        public void Hide()
        {
            if (_window != null && _window.Visible)
            {
                Dispose();
            }
        }
    }
}
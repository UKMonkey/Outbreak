using System;
using Vortex.Interface;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Psy.Gui.Components;

namespace Outbreak.Client.Gui
{
    public class StatusHud : IDisposable
    {
        private const string AmmoWindowName = "ammoWindow";
        private const string AmmoTextName = "ammoText";
        private const string HungerTextName = "hungerText";

        private readonly IClient _engine;
        private Label _ammoTextWidget;
        private Label _hungerTextWidget;
        private GuiWindow _window;

        public StatusHud(IClient engine)
        {
            _engine = engine;
            CreateWindow();
        }

        public void Dispose()
        {
            _window.Delete();
        }

        private void CreateWindow()
        {
            _engine.GuiLoader.Load("statusPane.xml", _engine.Gui.Desktop);
            _ammoTextWidget = _engine.Gui.GetWidgetByName<Label>(AmmoTextName);
            _hungerTextWidget = _engine.Gui.GetWidgetByName<Label>(HungerTextName);
            _window = _engine.Gui.GetWidgetByName<GuiWindow>(AmmoWindowName);
        }

        public void Update()
        {
            _window.Visible = true;

            UpdateAmmo();
            UpdateHunger();
        }

        private void UpdateHunger()
        {
            if (_engine.Me == null)
                return;

            var hunger = _engine.Me.GetHunger();
            _hungerTextWidget.Text = string.Format("Hunger: {0}/{1}", hunger, Consts.MaxHunger);
        }

        private void UpdateAmmo()
        {
            if (_engine.Me == null)
                return;

            var equippedItem = _engine.Me.GetEquippedItem();
            if (equippedItem == null)
            {
                _ammoTextWidget.Text = "No weapon";
                return;
            }
                
            var ammoLoaded = equippedItem.GetLoadedAmmoCount();
            var itemSpec = equippedItem.GetItemSpec();

            // a latency issue - don't worry, we'll get it soon... probably
            if (itemSpec == null)
            {
                _ammoTextWidget.Text = "";
                return;
            };

            var clipSize = itemSpec.GetClipSize();
            _window.Visible = true;

            var name = itemSpec.GetName();

            if (clipSize != 0)
                _ammoTextWidget.Text = string.Format("{0} {1}/{2}", name, ammoLoaded, clipSize);
            else
                _ammoTextWidget.Text = string.Format("{0} --/--", name);
        }
    }
}
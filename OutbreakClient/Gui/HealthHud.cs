using System;
using Vortex.Interface;
using Vortex.Interface.EntityBase.Properties;
using Outbreak.Client.Gui.Widgets;
using Psy.Gui.Components;

namespace Outbreak.Client.Gui
{
    public class HealthHud : IDisposable
    {
        private const string HealthWindowName = "healthWindow";
        private const string HealthBarName = "healthBar";

        private readonly IClient _engine;
        private GuiWindow _healthWindow;
        private HealthBar _healthBar;

        public HealthHud(IClient engine)
        {
            _engine = engine;
            CreateWindow();
        }

        public void Dispose()
        {
            _healthWindow.Delete();
        }

        private void CreateWindow()
        {
            _engine.GuiLoader.Load("health.xml", _engine.Gui.Desktop);
            _healthWindow = _engine.Gui.GetWidgetByName<GuiWindow>(HealthWindowName);
            _healthBar = _engine.Gui.GetWidgetByName<HealthBar>(HealthBarName);
        }

        public void Update()
        {
            if (_engine.Me == null)
            {
                _healthWindow.Visible = false;
                return;
            }

            _healthWindow.Visible = true;

            var healthAmount = _engine.Me.GetHealth();
            var maxHealth = _engine.Me.GetMaxHealth();

            _healthBar.Health = healthAmount;
            _healthBar.MaximumHealth = maxHealth;
        }
    }
}
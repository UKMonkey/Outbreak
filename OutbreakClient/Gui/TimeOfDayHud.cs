using System;
using Psy.Gui.Components;
using Vortex.Interface;

namespace Outbreak.Client.Gui
{
    public class TimeOfDayHud : IDisposable
    {
        private readonly GameClient _gameClient;
        private readonly IClient _engine;
        private readonly GuiWindow _timeOfDayWindow;
        private readonly Label _label;

        private const string LabelName = "timeOfDayLabel";

        public TimeOfDayHud(GameClient gameClient)
        {
            _gameClient = gameClient;
            _engine = gameClient.Engine;

            _timeOfDayWindow = (GuiWindow)(_engine.GuiLoader.Load("timeOfDay.xml", _engine.Gui.Desktop));
            _label = _engine.Gui.GetWidgetByName<Label>(LabelName);
        }

        public void Dispose()
        {
            _timeOfDayWindow.Delete();
        }
        
        public void Update()
        {
            _label.Text = GetTimeString();
        }

        private string GetTimeString()
        {
            return string.Format("{0:00}:{1:00}", _gameClient.GameTime.Hour, _gameClient.GameTime.Minute);
        }
    }
}
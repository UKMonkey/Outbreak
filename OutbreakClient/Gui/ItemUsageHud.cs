using System;
using Psy.Core;
using Vortex.Interface;
using Outbreak.Client.Gui.Widgets;
using Psy.Gui.Components;

namespace Outbreak.Client.Gui
{
    public class ItemUsageHud : IDisposable
    {
        private const string ProgressWindowName = "progressWindow";
        private const string ProgressBarName = "progressBar";

        private readonly IClient _engine;
        private GuiWindow _progressWindow;
        private ProgressBar _progressBar;

        private double _startTime;

        public ItemUsageHud(IClient engine)
        {
            _engine = engine;
            CreateWindow();
            _startTime = 0;
        }

        public void Dispose()
        {
            _progressWindow.Delete();
        }

        private void CreateWindow()
        {
            _engine.GuiLoader.Load("progress.xml", _engine.Gui.Desktop);
            _progressWindow = _engine.Gui.GetWidgetByName<GuiWindow>(ProgressWindowName);
            _progressBar = _engine.Gui.GetWidgetByName<ProgressBar>(ProgressBarName);

            _progressWindow.Visible = false;
        }

        public void Start(int workTime)
        {
            _progressBar.Complete = false;
            _progressBar.Canceled = false;
            _progressWindow.Visible = true;
            _progressBar.Visible = true;

            _progressBar.MaxValue = workTime;
            _progressBar.Alpha = 1f;
            _progressBar.Intensity = 1f;
            _startTime = Timer.GetTime();
        }

        public void Stop(bool success)
        {
            if (success)
                _progressBar.Complete = true;
            else
                _progressBar.Canceled = true;
        }

        public void Update()
        {
            if (!_progressWindow.Visible)
                return;
            
            if (_progressBar.Complete || _progressBar.Canceled)
            {
                _progressBar.Intensity -= 0.01f;
                _progressBar.Alpha = 0f;
                if (_progressBar.Intensity < 0.1)
                    _progressWindow.Visible = false;
            }
            else
            {
                var now = Timer.GetTime();
                var difference = now - _startTime;
                _progressBar.CurrentValue = (int)Math.Round(difference);
            }
        }
    }
}
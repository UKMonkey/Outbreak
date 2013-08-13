using System;
using System.Windows.Forms;

namespace Launcher
{
    public partial class MainForm : Form
    {
        private readonly GameLauncher _gameLauncher;

        public MainForm(GameLauncher gameLauncher)
        {
            _gameLauncher = gameLauncher;
            InitializeComponent();

            gameLauncher.SetView(new View(WebBrowser, PatchProgress, StatusLabel, StepProgress, 0, 10, 0));
            Poller.Enabled = true;
        }

        private void PollerTick(object sender, EventArgs e)
        {
            if (_gameLauncher.PerformUpdate())
            {
                PlayButton.Enabled = true;
            }
        }

        private void ExitButtonClick(object sender, EventArgs e)
        {
            _gameLauncher.Exit();
        }

        private void PlayButtonClick(object sender, EventArgs e)
        {
            _gameLauncher.Launch();
        }
    }
}

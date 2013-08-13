using System;
using System.Windows.Forms;

namespace Launcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            GameLauncher gameLauncher;

            try
            {
                var startArguments = StartArguments.ParseFrom(args);
                gameLauncher = new GameLauncher(startArguments);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Launcher error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(gameLauncher));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Launcher.Updatables;
using System.Linq;

namespace Launcher
{

    public class GameLauncher
    {
        private View _view;
        private readonly Updater _updater;
        private StartArguments _args;
        protected List<Updatable> Updatables { get; set; }
        protected Dictionary<string, Version> CurrentVersions { get; set; }

        public GameLauncher(StartArguments args)
        {
            CurrentVersions = VersionData.Load();
            Updatables = UpdatableFactory.Create(CurrentVersions);
            _updater = new Updater(Updatables);
            _args = args;

            _updater.UpdatableItemCompleted += UpdaterOnUpdatableItemCompleted;
            _updater.UpdatableItemStarted += UpdaterOnUpdatableItemStarted;
            _updater.UpdatableItemDownloadProgress += UpdaterOnUpdatableItemDownloadProgress;
            _updater.UpdatableItemFailed += UpdaterOnUpdatableItemFailed;
        }

        private void UpdaterOnUpdatableItemFailed(Updatable updatable, Exception exception)
        {
            MessageBox.Show(exception.Message, "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        private void UpdaterOnUpdatableItemDownloadProgress(Updatable updatable, int percentage)
        {
            _view.ProgressText2 = string.Format("Downloaded {0}%", percentage);
        }

        private void UpdaterOnUpdatableItemStarted(Updatable name)
        {
            _view.ProgressBarValue += 1;
        }

        private void UpdaterOnUpdatableItemCompleted(Updatable name)
        {
            _view.ProgressBarValue += 1;
        }

        public void SetView(View view)
        {
            _view = view;
            _view.ProgressBarMinimum = 0;
            _view.ProgressBarValue = 0;
            _view.ProgressBarMaxiumum = Updatables.Count(x => x.RequiresUpdating) * 2;
            _view.ProgressText1 = "Starting update.";
            _view.ProgressText2 = "";
            //_view.WebBrowserUrl = string.Format("http://outbreak-game.co.uk/launcher.html?i={0}", Guid.NewGuid());
        }

        public void Exit()
        {
            Application.Exit();
        }

        public void Launch()
        {
            try
            {
                Process.Start("Vortex.Client.Exe", string.Format("-mod {0}", _args.ModName));
                Application.Exit();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Failed to launch client.\nReason = '{0}'", e.Message), 
                    "Launcher Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }   
        }

        public bool PerformUpdate()
        {
            if (_updater.ExecutingThreads > 0)
                return false;

            var beingUpdated = _updater.Perform();
            if (beingUpdated != null)
            {
                _view.ProgressText1 = string.Format("Updating {0} to version {1}", beingUpdated.Name,
                                                    beingUpdated.LatestVersion);
                return false;
            }

            _view.ProgressText1 = "Updating complete.";

            _updater.DeleteTemporaryFiles();

            VersionData.Save(VersionData.GetFrom(Updatables));

            return true;
        }
    }
}
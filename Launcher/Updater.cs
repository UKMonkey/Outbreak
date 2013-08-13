using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Threading;
using Ionic.Zip;
using Launcher.Updatables;

namespace Launcher
{
    public delegate void UpdatableItemStarted(Updatable updatable);
    public delegate void UpdatableItemFailed(Updatable updatable, Exception e);
    public delegate void UpdatableItemCompleted(Updatable updatable);
    public delegate void UpdatableItemDownloadProgress(Updatable updatable, int percentage);

    public class Updater
    {
        private readonly IEnumerable<Updatable> _updatables;
        private readonly Dispatcher _dispatcher;
        private readonly List<string> _tempFiles;
        public int ExecutingThreads { get; private set; }

        public Updater(IEnumerable<Updatable> updatables)
        {
            ExecutingThreads = 0;
            _updatables = updatables;
            _dispatcher = Dispatcher.CurrentDispatcher;
            _tempFiles = new List<string>(5);
        }

        public event UpdatableItemCompleted UpdatableItemCompleted;

        private void OnUpdatableItemCompleted(Updatable updatable)
        {
            var handler = UpdatableItemCompleted;
            if (handler != null) handler(updatable);
        }

        public event UpdatableItemStarted UpdatableItemStarted;

        private void OnUpdatableItemStarted(Updatable updatable)
        {
            var handler = UpdatableItemStarted;
            if (handler != null) handler(updatable);
        }

        public event UpdatableItemDownloadProgress UpdatableItemDownloadProgress;

        private void OnUpdatableItemDownloadProgress(Updatable updatable, int percentage)
        {
            UpdatableItemDownloadProgress handler = UpdatableItemDownloadProgress;
            if (handler != null) handler(updatable, percentage);
        }

        public event UpdatableItemFailed UpdatableItemFailed;

        public void OnUpdatableItemFailed(Updatable updatable, Exception e)
        {
            UpdatableItemFailed handler = UpdatableItemFailed;
            if (handler != null) handler(updatable, e);
        }


        public Updatable Perform()
        {
            if (_updatables.Any(x => x.InProgress))
                return null;

            var nextUpdateTask = _updatables.FirstOrDefault(x => x.RequiresUpdating && !x.InProgress);

            if (nextUpdateTask == null)
                return null;

            Update(nextUpdateTask);

            return nextUpdateTask;
        }

        private void FileActionComplete(Updatable updatable, string filename)
        {
            ExecutingThreads--;
            updatable.InProgress = false;
            updatable.CurrentVersion = updatable.LatestVersion;
            OnUpdatableItemCompleted(updatable);
        }

        private void FileActionFailed(Updatable updatable, string filename, Exception e)
        {
            OnUpdatableItemFailed(updatable, e);
        }

        public void DeleteTemporaryFiles()
        {
            foreach (var tempFile in _tempFiles)
            {
                File.Delete(tempFile);
            }
        }

        private void Update(Updatable updatable)
        {
            var latestUpdate = updatable.LatestUpdate;
            if (latestUpdate == null)
            {
                return;
            }

            var filename = string.Format("patch-{0}.zip", updatable.Name);

            _tempFiles.Add(filename);

            SpawnDownloadFileThread(updatable, filename, latestUpdate);
        }

        private void SpawnDownloadFileThread(Updatable updatable, string fileName, UpdateVersion latestUpdate)
        {
            ExecutingThreads++;
            updatable.InProgress = true;
            OnUpdatableItemStarted(updatable);

            var downloadThread = new Thread(() =>
            {
                try
                {
                    using (var webClient = new WebClient())
                    {
                        webClient.DownloadProgressChanged += (sender, args) => 
                                                             _dispatcher.Invoke(new Action<Updatable, int>(OnUpdatableItemDownloadProgress), updatable, args.ProgressPercentage);

                        webClient.DownloadFileCompleted += (sender, args) =>
                                                           _dispatcher.Invoke(new Action<Updatable, int>(OnUpdatableItemDownloadProgress), updatable, 100);

                        webClient.DownloadFile(latestUpdate.Url, fileName);
                    }

                    using (var zipFile = new ZipFile(fileName))
                    {
                        zipFile.ExtractAll(updatable.ExtractLocation, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
                catch (Exception e)
                {
                    var ex = new Exception(string.Format("Failed to process update `{0}` to version {3}\nURL:{1}\n{2}", updatable.Name, updatable.LatestUpdate.Url, e.Message, updatable.LatestUpdate.Version),
                                           e.InnerException);

                    _dispatcher.Invoke(new Action<Updatable, string, Exception>(FileActionFailed), updatable, fileName, ex);
                }

                _dispatcher.Invoke(new Action<Updatable, string>(FileActionComplete), updatable, fileName);
            });

            downloadThread.Start();
        }
    }
}
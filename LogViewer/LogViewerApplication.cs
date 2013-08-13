using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LogViewer.GridView;
using LogViewer.LogFile;
using LogViewer.RowStyles.RowStylers;

namespace LogViewer
{
    public class LogViewerApplication
    {
        private readonly Queue<string> _readQueue;
        private readonly Dictionary<string, LogProcessor> _logProcessors;
        private readonly Dictionary<string, List<ILogPresenter>> _logPresenters;
        private readonly bool _isFileSystemWatcherShit;

        public ProcessMetrics ProcessMetrics { get; set; }

        public static string LogPath
        {
            get { return Psy.Core.Logging.Loggers.FileLogger.LogFolderPath; }
        }

        public static string LogFileFilter
        {
            get { return "*.log"; }
        }

        public LogViewerApplication(bool isFileSystemWatcherShit)
        {
            _readQueue = new Queue<string>();
            _logProcessors = new Dictionary<string, LogProcessor>();
            _logPresenters = new Dictionary<string, List<ILogPresenter>>();
            
            ProcessMetrics = new ProcessMetrics();
            _isFileSystemWatcherShit = isFileSystemWatcherShit;

            Psy.Core.Logging.Loggers.FileLogger.CreateLogFolder();
        }

        public void QueueFileForProcessing(string filename, IEnumerable<DataGridView> grids)
        {
            ProcessMetrics.Queues++;

            if (!_readQueue.Contains(filename))
                _readQueue.Enqueue(filename);

            if (!_logProcessors.ContainsKey(filename))
                _logProcessors.Add(filename, new LogProcessor(filename));

            if (!_logPresenters.ContainsKey(filename))
            {
                var presenters = new List<ILogPresenter>();
                _logPresenters.Add(filename, presenters);

                foreach (var grid in grids)
                {
                    var presenter = new GridLogPresenter(
                        grid,
                        new CombinedRowStyler(
                            new SourceStyler(),
                            new LevelStyler()),
                        new StyleApplicator(),
                        new DataGridViewRowFactory());
                    presenters.Add(presenter);
                }
            }
        }

        public void ProcessFileQueue()
        {
            var requeue = new List<string>();

            while (_readQueue.Count != 0)
            {
                try
                {
                    var item = _readQueue.Dequeue();

                    var presenters = _logPresenters[item];
                    var processor = _logProcessors[item];
                    var newLines = processor.GetNewLines();

                    foreach (var presenter in presenters)
                        presenter.Begin();

                    foreach (var line in newLines)
                    {
                        foreach (var presenter in presenters)
                            presenter.Add(line);
                        ++ProcessMetrics.Lines;
                    }

                    foreach (var presenter in presenters)
                        presenter.End();

                    ProcessMetrics.Batches++;

                    if (_isFileSystemWatcherShit)
                    {
                        requeue.Add(item);
                    }
                }
                catch (Exception)
                {
                    ProcessMetrics.FileLoadExceptions++;
                    break;
                }
            }

            foreach (var item in requeue)
            {
                _readQueue.Enqueue(item);
            }

            ProcessMetrics.ProcessCalls++;
        }


        public void ClearLog()
        {
        }
    }

}
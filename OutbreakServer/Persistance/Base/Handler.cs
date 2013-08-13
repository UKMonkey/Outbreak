using System;
using System.Threading;
using Psy.Core.Logging;

namespace Outbreak.Server.Persistance.Base
{
    public abstract class Handler<TLoader, TSaver> : IDisposable
        where TLoader : IDisposable, new()
        where TSaver : IDisposable, new()
    {
        private const int MaxLoadOnlyCount = 100;

        private volatile bool _runThread = true;
        private readonly Thread _worker;

        protected TLoader Loader;
        protected TSaver Saver;

        protected abstract void PerformSave();
        protected abstract void PerformLoad();

        protected Handler(string typeName)
        {
            Loader = new TLoader();
            Saver = new TSaver();

            _runThread = true;
            _worker = new Thread(WorkerThreadMain)
                          {
                              Priority = ThreadPriority.Lowest,
                              IsBackground = true,
                              Name = typeName + "Handler"
                          };
        }


        public virtual void Dispose()
        {
            _runThread = false;

            lock (this)
            {
                Monitor.PulseAll(this);
            }
            _worker.Join();

            Loader.Dispose();
            Saver.Dispose();
        }


        public virtual void Init()
        {
            _worker.Start();
        }


        protected virtual void PerformFinalSave()
        {
            PerformSave();
        }


        private void WorkerThreadMain()
        {
            var loadOnlyCount = 0;

//            try
//            {
                while (_runThread)
                {
                    bool loadOnly;
                    lock (this)
                    {
                        loadOnly = Monitor.Wait(this, 2000);
                    }

                    PerformLoad();
                    if (!loadOnly || loadOnlyCount > MaxLoadOnlyCount)
                    {
                        PerformSave();
                        loadOnlyCount = 0;
                    }
                    else
                    {
                        loadOnlyCount++;
                    }
                }

                PerformFinalSave();
//            }
//            catch (Exception e)
//            {
//                Logger.WriteException(e);
//                throw;
//            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vortex.Interface;
using Vortex.Interface.World.Chunks;
using Vortex.Interface.World.Triggers;

namespace Outbreak.Server.Persistance.File.Triggers
{
    public class TriggerHandler: Base.FileHandler<TriggerLoader, TriggerSaver>, ITriggerSaver, ITriggerLoader
    {
        private readonly List<ChunkKey> _triggersToLoad = new List<ChunkKey>();
        private List<List<ITrigger>> _triggersToSave = new List<List<ITrigger>>();

        public TriggerHandler(IGame game) 
            : base(game, "Trigger")
        {
        }

        protected override void PerformSave()
        {
        }

        protected override void PerformLoad()
        {
            var triggersToLoad = new List<ChunkKey>();
            lock (this)
            {
                triggersToLoad.AddRange(_triggersToLoad);
                _triggersToLoad.Clear();
            }

            if (triggersToLoad.Count > 0)
                OnTriggersUnavailable(triggersToLoad);
        }

        public override void Dispose()
        {
        }

        public void SaveTrigger(List<ITrigger> toSave)
        {
        }

        public event TriggerCallback OnTriggerGenerated;
        public event TriggerCallback OnTriggerLoaded;
        public event ChunkKeyCallback OnTriggersUnavailable;

        public void LoadTriggers(ChunkKey location)
        {
            lock(this)
            {
                _triggersToLoad.Add(location);
            }
        }
    }
}

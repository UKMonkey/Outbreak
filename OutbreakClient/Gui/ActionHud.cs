using System;
using Outbreak.Client.Gui.Actions;

namespace Outbreak.Client.Gui
{
    public class ActionHud : IDisposable
    {
        private readonly IAction[] _actions;
        
        public ActionHud()
        {
            _actions = new IAction[9];
        }
        
        public void Update()
        {
            for (var i=0; i<_actions.Length; ++i)
            {
                if (_actions[i] == null)
                    continue;

            }
        }

        public void Dispose()
        {
        }

        public IAction this[int actionSlot]
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}

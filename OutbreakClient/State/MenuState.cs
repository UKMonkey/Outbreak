using SlimMath;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.Net;
using Psy.Core.Input;

namespace Outbreak.Client.State
{
    public abstract class MenuState : IGameState
    {
        public Context Context { get; set; }

        public IClient Engine
        {
            get { return Context.GameClient.Engine; }
        }

        public abstract void OnTransitionOut();

        public abstract void OnTransitionIn();

        public virtual void OnScreenMouseButtonDown(MouseEventArguments args) { }

        public virtual void OnWorldMouseMove(Vector3 viewCoords) { }

        public virtual void OnClientJoin(RemotePlayer player) { }

        public virtual void OnClientLeave(RemotePlayer player) { }

        public virtual void UpdateWorld() { }

        public virtual void OnFocusChange(Entity entity) { }

        public void OnXPChange(int amount) { }

        public void OnLevelUp(RemotePlayer remotePlayer, int newLevel) { }
        
        public void OnContainerOpen(int containerEntityId) { }

        public void OnActionStarted(int entityId, int time, int itemSpec) { }

        public void OnActionEnded(int entityId, bool success) { }
    }
}
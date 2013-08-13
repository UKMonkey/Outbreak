using SlimMath;
using Vortex.Interface.EntityBase;
using Vortex.Interface.Net;
using Psy.Core.Input;
using Psy.Core.StateMachine;

namespace Outbreak.Client.State
{
    public interface IGameState : IState<Context>
    {
        void OnScreenMouseButtonDown(MouseEventArguments args);
        void OnWorldMouseMove(Vector3 viewCoords);
        void OnClientJoin(RemotePlayer player);
        void OnClientLeave(RemotePlayer player);
        void UpdateWorld();
        void OnFocusChange(Entity entity);
        void OnXPChange(int amount);
        void OnLevelUp(RemotePlayer remotePlayer, int newLevel);
        void OnContainerOpen(int containerEntityId);
        void OnActionStarted(int entityId, int time, int itemSpec);
        void OnActionEnded(int entityId, bool success);
    }
}
using Psy.Core.StateMachine;

namespace Outbreak.Client.State
{
    public static class StateMachineFactory
    {
        public static StateMachine<IGameState, Context> Create(Context context)
         {
             var stateMachine = new StateMachine<IGameState, Context>(context);
             stateMachine.RegisterState("menu", new StateFactory<TitleMenuState, Context>());
             stateMachine.RegisterState("ingame", new StateFactory<InGameState, Context>());
             stateMachine.RegisterState("serverBrowser", new StateFactory<ServerBrowserState, Context>());
             return stateMachine;
         }
    }
}
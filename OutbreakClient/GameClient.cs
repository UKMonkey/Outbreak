using System;
using System.Collections.Generic;
using System.Linq;
using Outbreak.Effects;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.Net;
using Outbreak.Client.Audio;
using Outbreak.Client.Gui.Widgets;
using Outbreak.Client.Items.Containers;
using Outbreak.Client.State;
using Outbreak.Items.Containers;
using Outbreak.Items.Containers.FloatingItems;
using Outbreak.Items.Containers.InventorySpecs;
using Psy.Core.Input;
using Psy.Core.Logging;
using Psy.Core.StateMachine;
using Vortex.Interface.World.Blocks;

namespace Outbreak.Client
{
    public class GameClient : Game, IGameClient
    {
        public int MyClientId { get; set; }
        public string PlayerName { get; set; }

        public new IClient Engine { get; private set; }
        private StartArguments Arguments { get; set; }

        private MessageHandler _clientMessageHandler;
        public StateMachine<IGameState, Context> StateMachine { get; private set; }
        private EntityWatcher _entityWatcher;

        public GameClient(StartArguments arguments)
        {
            Arguments = arguments;

            var context = new Context(this);
            StateMachine = StateMachineFactory.Create(context);
            GameTime = new GameTime();
        }

        public void OnAttach(IClient engine)
        {
            Engine = engine;
            Engine.SetEntityViewCollector(EntityProvider);
            _entityWatcher = new EntityWatcher(Engine);
            InitializeAudio();
            InitializeGui();

            base.OnAttach(engine);
        }

        // let the user see all the entities if they die ...
        private IEnumerable<Entity> EntityProvider()
        {
            return _entityWatcher.GetVisibleEntities();
        }

        private void InitializeGui()
        {
            ActionSlot.Register(Engine.GuiLoader);
            InventorySlot.Register(Engine.GuiLoader);
            HealthBar.Register(Engine.GuiLoader);
            ProgressBar.Register(Engine.GuiLoader);
        }

        private void InitializeAudio()
        {
            AudioChannels.Create(Engine);
        }

        protected override void RegisterStaticItemSpecCache()
        {
            StaticItemSpecCache.Instance = new ItemSpecCache(Engine);
        }

        protected override void RegisterStaticFloatingItemCache()
        {
            StaticFloatingItemCache.Instance = new FloatingItemCache();
        }

        protected override void RegisterStaticInventoryCache(IItemSpecCache cache)
        {
            StaticInventoryCache.Instance = new InventoryCache(Engine);
        }

        public void SetGameName(string name)
        {
            GameName = name;
        }

        public override void OnBegin()
        {
            _clientMessageHandler = new MessageHandler(this, Engine);
            _clientMessageHandler.RegisterCallbacks();
            
            var clientConsoleCommands = new ConsoleCommands(this);
            clientConsoleCommands.BindConsoleCommands();

            StateMachine.TransitionTo("menu");
        }

        /// <summary>
        /// Client - player clicks on the screen somewhere.
        /// </summary>
        /// <param name="args"></param>
        public void OnScreenMouseButtonDown(MouseEventArguments args)
        {
            StateMachine.CurrentState.OnScreenMouseButtonDown(args);
        }

        public override void OnNetworkStatusChanged(NetworkStatus networkStatus)
        {
            Engine.ConsoleText("Network state changed to " + networkStatus);
        }

        /// <summary>
        /// Client - respond to a message containing the list of already connected clients
        /// </summary>
        public void OnConnectedClientListRecieved()
        {
            foreach (var c in Engine.ConnectedClients)
            {
                Engine.ConsoleText(String.Format("{0} - {1}", c.ClientId, c.PlayerName));
            }
        }

        /// <summary>
        /// Client - player moved their mouse across the map.
        /// </summary>
        /// <param name="viewCoords"></param>
        public void OnWorldMouseMove(Vector3 viewCoords)
        {
            StateMachine.CurrentState.OnWorldMouseMove(viewCoords);
        }

        /// <summary>
        /// Client method - called when a client joins
        /// </summary>
        /// <param name="player"></param>
        public void OnClientJoin(RemotePlayer player)
        {
            StateMachine.CurrentState.OnClientJoin(player);
        }

        public void OnClientRejected(RejectionReasonEnum reason)
        {
            
        }

        public override void UpdateWorld()
        {
            StateMachine.CurrentState.UpdateWorld();
            GameTime.Tick();
            Engine.OutsideLightingColour = OutsideLightingCalculator.GetOutsideLighting(GameTime);
        }

        public void TraceBullet(Vector3 from, Vector3 to, BulletEffect effect, float rotation)
        {
            Engine.FireBullet(from, to, new Color4(1.0f, 0.0f, 1.0f, 0.0f));
            switch (effect)
            {
                case BulletEffect.None:
                    break;
                case BulletEffect.Bloodsplatter:
                    Engine.BloodSpray(to, rotation);
                    break;
            }
        }

        public void SetEntityOwnership(int entityId, int clientId)
        {
            var client = Engine.ConnectedClients.FirstOrDefault(c => c.ClientId == clientId);
            if (client == null)
            {
                return;
            }

            var entity = Engine.Entities.FirstOrDefault(e => e.EntityId == entityId);
            if (entity == null)
            {
                return;
            }

            if (clientId == MyClientId)
            {
                Logger.Write(string.Format("I control entity {0}", entityId));
                Engine.Me = entity;
            }

            var player = Engine.ConnectedClients.FirstOrDefault(c => c.ClientId == clientId);
            if (player == null)
            {
                return;
            }

            Logger.Write(string.Format("{0} controls entity", player.PlayerName));
        }

        /// <summary>
        /// Client method - called when a client leaves
        /// </summary>
        /// <param name="player"></param>
        public void OnClientLeave(RemotePlayer player)
        {
            StateMachine.CurrentState.OnClientLeave(player);
        }

        public void OnFocusChange(Entity entity)
        {
            StateMachine.CurrentState.OnFocusChange(entity);
            _entityWatcher = new EntityWatcher(Engine, entity);
        }

        public BlockProperties GetDefaultBlock()
        {
            return null;
        }
    }
}

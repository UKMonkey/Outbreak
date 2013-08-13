using System.IO;
using Outbreak.Audio;
using SlimMath;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Properties;
using Vortex.Interface.Net;
using Outbreak.Entities.Properties;
using Outbreak.Client.Gui;
using Outbreak.Client.KeyBindings;
using Outbreak.Items.Containers;
using Outbreak.Net.Messages;
using Psy.Core;
using Psy.Core.FileSystem;
using Psy.Core.Input;
using Psy.Core.Logging;
using Psy.Core.Tasks;

namespace Outbreak.Client.State
{
    public class InGameState : IGameState
    {
        private const float ZoomDistanceAdjust = 1.0f;
        private double _lastToggleTime;
        private const long MaxToggleTime = 666;
        private const string UIClickSoundName = "MenuOver.ogg";

        private InventoryGui _inventoryGui;
        private ITask _avatarUpdateTask;
        private bool _debugView;
        private StatusHud _statusHud;
        private HealthHud _healthHud;
        private ItemUsageHud _itemUsageHud;
        private TimeOfDayHud _timeOfDay;
        private ChatboxHud _chatboxHud;
        private ContainerHud _containerHud;
        private ActionHud _actionHud;
        public Context Context { get; set; }

        private IClient Engine
        {
            get { return Context.GameClient.Engine; }
        }

        public void OnTransitionOut()
        {
            StaticTaskQueue.TaskQueue.UnscheduleTask(_avatarUpdateTask);
            Engine.InputBinder.ResetHandlers();
            Engine.InputBinder.ResetBinds();

            DeleteHud();
            _debugView = false;
        }

        public void OnTransitionIn()
        {
            _avatarUpdateTask = StaticTaskQueue
                .TaskQueue
                .CreateRepeatingTask("SendState", SendServerMyAvatarState, Engine.UpdateWorldFrequency);

            CreateHud();

            Engine.Gui.Desktop.Visible = true;
            Engine.Gui.Desktop.Transparent = true;

            ConfigureKeyBinds();

            _inventoryGui = new InventoryGui(Engine);
            Engine.MaterialCache.LoadMaterials("materials.xml");
        }

        private void CreateHud()
        {
            _statusHud = new StatusHud(Engine);
            _healthHud = new HealthHud(Engine);
            _itemUsageHud = new ItemUsageHud(Engine);
            _timeOfDay = new TimeOfDayHud(Context.GameClient);
            _chatboxHud = new ChatboxHud(Context.GameClient);
            _containerHud = new ContainerHud(Context.GameClient);
            _actionHud = new ActionHud();
        }

        private void UpdateHud()
        {
            _statusHud.Update();
            _healthHud.Update();
            _itemUsageHud.Update();
            _timeOfDay.Update();
            _inventoryGui.Update();
            _containerHud.Update();
            _actionHud.Update();
        }

        private void DeleteHud()
        {
            _statusHud.Dispose();
            _healthHud.Dispose();
            _itemUsageHud.Dispose();
            _inventoryGui.Dispose();
            _timeOfDay.Dispose();
            _chatboxHud.Dispose();
            _actionHud.Dispose();
        }

        private void ConfigureKeyBinds()
        {
            Engine.InputBinder.ResetBinds();

            DefaultInputBindings.Populate(Engine.InputBinder);

            var keybindsFile = Lookup.GetFilePath("keybinds.cfg");

            var keybindsFileInfo = new FileInfo(keybindsFile);

            if (keybindsFileInfo.Exists)
            {
                if (!Engine.InputBinder.LoadBindings(
                    new FileStream(keybindsFile, FileMode.Open, FileAccess.Read)))
                {
                    Logger.Write("Unable to load keybindings - removing and re-creating");
                }
            }

            Engine.InputBinder.SaveBindings(
                new FileStream(keybindsFile, FileMode.OpenOrCreate, FileAccess.Write));

            Engine.InputBinder
                .Register(InputActions.MoveUp.ToString(), @event => MoveChange(Direction.North, @event))
                .Register(InputActions.MoveDown.ToString(), @event => MoveChange(Direction.South, @event))
                .Register(InputActions.MoveLeft.ToString(), @event => MoveChange(Direction.West, @event))
                .Register(InputActions.MoveRight.ToString(), @event => MoveChange(Direction.East, @event))
                .Register(InputActions.Reload.ToString(), Reload)
                .Register(InputActions.Walk.ToString(), AlterMovementSpeed)
                .Register(InputActions.ZoomIn.ToString(), ZoomIn)
                .Register(InputActions.Zoomout.ToString(), ZoomOut)
                .Register(InputActions.Fire.ToString(), Fire)
                .Register(InputActions.Interact.ToString(), Interact)
                .Register(InputActions.Inventory.ToString(), ToggleInventory)
                .Register(InputActions.Chat.ToString(), ShowChatWindow)
                .Register(InputActions.DebugView.ToString(), ToggleDebugView)
                .Register(InputActions.HotAction1.ToString(), @event => PerformHotAction(1, @event))
                .Register(InputActions.HotAction2.ToString(), @event => PerformHotAction(2, @event))
                .Register(InputActions.HotAction3.ToString(), @event => PerformHotAction(3, @event))
                .Register(InputActions.HotAction4.ToString(), @event => PerformHotAction(4, @event))
                .Register(InputActions.HotAction5.ToString(), @event => PerformHotAction(5, @event))
                .Register(InputActions.HotAction6.ToString(), @event => PerformHotAction(6, @event))
                .Register(InputActions.HotAction7.ToString(), @event => PerformHotAction(7, @event))
                .Register(InputActions.HotAction8.ToString(), @event => PerformHotAction(8, @event))
                .Register(InputActions.HotAction9.ToString(), @event => PerformHotAction(9, @event));
        }

        private void ShowChatWindow(InputEvent obj)
        {
            _chatboxHud.Focus();
        }

        private void ToggleDebugView(InputEvent obj)
        {
            if (obj.KeyAction != KeyAction.Up)
                return;

            _debugView = !_debugView;

            if (_debugView)
            {
                Engine.Console.Eval("cmesh 1");
                Engine.Console.Eval("perfInfo 1");
                Engine.Console.Eval("posInfo 1");
                Engine.Console.Eval("frameNo 1");
                Engine.Console.Eval("latencyInfo 1");
            }
            else
            {
                Engine.Console.Eval("cmesh 0");
                Engine.Console.Eval("perfInfo 0");
                Engine.Console.Eval("posInfo 0");
                Engine.Console.Eval("frameNo 0");
                Engine.Console.Eval("latencyInfo 0");
            }
        }

        private void ToggleInventory(InputEvent obj)
        {
            if (obj.KeyAction != KeyAction.Down)
                return;

            Engine.AudioEngine.Play(UIClickSoundName, (int)AudioChannel.Interface);

            var player = Engine.Me;
            if (player == null || player.GetInventoryId() == StaticInventoryCache.Instance.UnknownId)
                return;

            _inventoryGui.ToggleVisibility();
        }

        private void ZoomOut(InputEvent e)
        {
            Engine.ZoomLevel += ZoomDistanceAdjust;
        }

        private void ZoomIn(InputEvent e)
        {
            Engine.ZoomLevel -= ZoomDistanceAdjust;
        }

        private void Fire(InputEvent e)
        {
            var me = Engine.Me;
            if (me == null)
                return;

            Message msg;
            if (e.KeyAction == KeyAction.Up)
                msg = new ClientStopFireWeaponMessage();
            else
                msg = new ClientStartFireWeaponMessage { Rotation = me.GetRotation() };

            Engine.SendMessage(msg);
        }

        private void Reload(InputEvent e)
        {
            var me = Engine.Me;
            if (me == null)
                return;

            if (e.KeyAction == KeyAction.Up)
                return;

            var msg = new ClientStartEquippedReload();
            Engine.SendMessage(msg);
        }

        private Entity EstablishEntityInfront(Entity from)
        {
            var collisionResult = Engine.TraceRay(from.GetPosition(), from.GetRotation(), (x) => true);
            if (!collisionResult.HasCollided)
                return null;

            var potentialEntityId = (short) collisionResult.CollisionMesh.Id;
            var entity = Engine.GetEntity(potentialEntityId);
            if (entity == null)
                return null;

            if (entity.Mesh == collisionResult.CollisionMesh)
                return entity;

            return null;
        }

        private void Interact(InputEvent e)
        {
            Interact();
        }

        private void Interact()
        {
            var me = Engine.Me;
            if (me == null)
                return;

            var ray = Engine.CameraMouseRay;

            //var collisionResult = Engine.TraceRay(camV, wmV,
            var collisionResult = Engine.TraceRay(ray,
                ent => ent != me && 
                    ent.GetPosition().DistanceSquared(me.GetPosition()) < 3);

            if (!collisionResult.HasCollided) 
                return;

            var potentialEntityId = collisionResult.CollisionMesh.Id;
            var entity = Engine.GetEntity(potentialEntityId);
            if (entity == null)
                return;

            if (Timer.GetTime() - _lastToggleTime < MaxToggleTime) 
                return;

            _lastToggleTime = Timer.GetTime();

            var msg = new ClientInteractWithEntityMessage {EntityId = potentialEntityId};
            Engine.SendMessage(msg);
        }

        private void MoveChange(Direction dir, InputEvent e)
        {
            _containerHud.Hide();
            _inventoryGui.Hide();

            if (e.KeyAction == KeyAction.Up)
                RemoveMovement(dir);
            else
                AddMovement(dir);
        }

        private void AlterMovementSpeed(InputEvent e)
        {
            var player = Engine.Me;

            if (player == null)
                return;

            var isRunning = e.KeyAction == KeyAction.Up;
            player.SetIsRunning(isRunning);
            if (isRunning)
                player.SetToRunningSpeed();
            else
                player.SetToWalkingSpeed();
        }

        public void OnScreenMouseButtonDown(MouseEventArguments args)
        {
        }

        /// <summary>
        /// Client - send the server my current player state.
        /// </summary>
        private void SendServerMyAvatarState()
        {
            if (Context.GameClient.Engine.Me == null)
                return;

            if (!Engine.Me.GetProperty(EntityPropertyEnum.Position).IsDirty &&
                !Engine.Me.GetProperty(EntityPropertyEnum.Rotation).IsDirty)
                return;

            var message = new ClientMoveMessage
                          {
                              Position = Engine.Me.GetPosition(), 
                              Rotation = Engine.Me.GetRotation(),
                              MovementVector = Engine.Me.GetMovementVector()
                          };

            Engine.Me.GetProperty(EntityPropertyEnum.Position).ClearDirtyFlag();
            Engine.Me.GetProperty(EntityPropertyEnum.Rotation).ClearDirtyFlag();
            Engine.Me.GetProperty(EntityPropertyEnum.MovementVector).ClearDirtyFlag();

            Engine.SendMessage(message);
        }

        private void AddMovement(Direction dir)
        {
            var player = Engine.Me;

            if (player == null)
                return;

            var current = player.GetKeyboardDirection();
            player.SetKeyboardDirection(DirectionUtil.MergeDirections(dir, current));
        }

        private void RemoveMovement(Direction dir)
        {
            var player = Engine.Me;

            if (player == null)
                return;

            var current = player.GetKeyboardDirection();
            current &= ~dir;
            player.SetKeyboardDirection(current);
        }

        public void OnWorldMouseMove(Vector3 viewCoords)
        {
            var player = Engine.Me;

            if (player == null)
                return;

            player.LookAt(viewCoords);
        }

        public void OnClientJoin(RemotePlayer player)
        {
            Engine.ConsoleText(string.Format("{0} has joined the server", player.PlayerName));
        }

        public void OnClientLeave(RemotePlayer player)
        {
            Engine.ConsoleText(string.Format("{0} has left the server", player.PlayerName));
        }

        public void UpdateWorld()
        {
            UpdateHud();
        }

        public void OnFocusChange(Entity entity)
        {
            Engine.SetCameraTarget(entity);
            Engine.Me = entity;

            RequestOtherEntityControllers();
        }

        public void OnXPChange(int amount)
        {
            if (!Engine.Me.HasProperty((int)GameEntityPropertyEnum.Experience))
                return;

            _chatboxHud.AddXPNotificationText(amount);
        }

        public void OnLevelUp(RemotePlayer remotePlayer, int newLevel)
        {
            _chatboxHud.AddLevelUpNotificationText(remotePlayer.PlayerName, newLevel);
        }

        public void OnContainerOpen(int containerEntityId)
        {
            _containerHud.ShowForContainer(containerEntityId);
            _inventoryGui.Show();
        }

        public void OnActionStarted(int entityId, int time, int itemSpec)
        {
            var player = Engine.Me;

            if (player != null && player.EntityId == entityId)
                _itemUsageHud.Start(time);
        }

        public void OnActionEnded(int entityId, bool success)
        {
            var player = Engine.Me;

            if (player != null && player.EntityId == entityId)
                _itemUsageHud.Stop(success);
        }

        private void RequestOtherEntityControllers()
        {
            var msg = new ClientRequestEntityControlMessage();
            Engine.SendMessage(msg);
        }

        private void PerformHotAction(int actionSlot, InputEvent e)
        {
            var action = _actionHud[actionSlot];
            if (action == null)
                return;

            action.PerformAction();
        }
    }
}
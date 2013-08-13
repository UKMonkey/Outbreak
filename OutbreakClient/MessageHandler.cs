using System;
using System.Linq;
using Outbreak.Audio;
using Outbreak.Effects;
using Outbreak.Entities.Properties;
using Outbreak.Net.Messages.FireWeapon;
using Outbreak.Resources;
using Psy.Core;
using Psy.Core.EpicModel;
using Vortex.Interface;
using Vortex.Interface.EntityBase.Properties;
using Vortex.Interface.Net;
using Outbreak.Net.Messages;
using Psy.Core.Logging;

namespace Outbreak.Client
{
    class MessageHandler
    {
        private readonly GameClient _gameClient;
        private readonly IClient _engine;

        public MessageHandler(GameClient gameClient, IClient engine)
        {
            _gameClient = gameClient;
            _engine = engine;
        }

        public void RegisterCallbacks()
        {
            _engine.RegisterMessageCallback(typeof(ServerGameNameMessage), HandleGameName);
            _engine.RegisterMessageCallback(typeof(ServerLoadMapMessage), HandleServerLoadMap);
            _engine.RegisterMessageCallback(typeof(ServerFirePistolMessage), HandleServerFireWeaponPistol);
            _engine.RegisterMessageCallback(typeof(ServerFireShotgunMessage), HandleServerFireWeaponShotgun);
            _engine.RegisterMessageCallback(typeof(ServerFireSwingWeapon), HandleServerSwingMelee);
            _engine.RegisterMessageCallback(typeof(ServerClientEntityControlMessage), HandleServerClientEntityControl);
            _engine.RegisterMessageCallback(typeof(ServerAmmoUpdateMessage), HandleServerAmmoUpdate);
            _engine.RegisterMessageCallback(typeof(ServerReloadWeaponMessage), HandlerServerReload);
            _engine.RegisterMessageCallback(typeof(ServerReloadCompleteMessage), HandlerServerReloadComplete);
            _engine.RegisterMessageCallback(typeof(ServerClientChangeEquippedWeapon), HandleServerClientChangeEquippedWeapon);
            _engine.RegisterMessageCallback(typeof(ServerPlayerXPIncrease), HandlePlayerXPIncrease);
            _engine.RegisterMessageCallback(typeof(ServerPlayerLevelUpMessage), HandlePlayerLevelUp);
            _engine.RegisterMessageCallback(typeof(ServerShowContainerContentsMessage), HandleShowContainerContents);
            _engine.RegisterMessageCallback(typeof(ServerStartItemUsage), HandleStartItemUsage);
            _engine.RegisterMessageCallback(typeof(ServerStopItemUsage), HandleStopItemUsage);
        }

        private void HandleGameName(Message msg)
        {
            var message = (ServerGameNameMessage) msg;
            _gameClient.SetGameName(message.Name);
        }

        private void HandleShowContainerContents(Message msg)
        {
            var message = (ServerShowContainerContentsMessage) msg;
            _gameClient.StateMachine.CurrentState.OnContainerOpen(message.ContainerEntityId);
        }

        private void HandlePlayerLevelUp(Message msg)
        {
            var message = (ServerPlayerLevelUpMessage) msg;
            _gameClient.StateMachine.CurrentState.OnLevelUp(message.RemotePlayer, message.NewLevel);
        }

        private void HandlePlayerXPIncrease(Message msg)
        {
            var message = (ServerPlayerXPIncrease) msg;
            _gameClient.StateMachine.CurrentState.OnXPChange(message.Amount);
        }

        private void HandleServerClientChangeEquippedWeapon(Message msg)
        {
            var message = (ServerClientChangeEquippedWeapon) msg;

            // for now just ignore everybody else
            if (_gameClient.Engine.Me.GetPlayerId() != message.ClientId)
            {
                throw new Exception("I was expecting the message for me.");
            }

            var player = _engine.Me;

            if (player == null)
            {
                throw new Exception("Got equipped weapon change, but can't find the Player entity");
            }

            //player.EquipWeapon(message.ItemSpecId);
        }

        private void HandleServerLoadMap(Message msg)
        {
            var message = (ServerLoadMapMessage) msg;
            _gameClient.MyClientId = message.ClientId;
            _gameClient.GameTime.SetTime(message.GameTimeHour, message.GameTimeMinute);
        }

        private void HandleServerFireWeaponShotgun(Message msg)
        {
            var message = (ServerFireShotgunMessage)msg;
            HandlerServerFireBullets(message);

            var me = _engine.Me;
            if (me == null)
                return;

            _engine.AudioEngine.Play(Sound.GunFire1, message.StartPoint, (int)AudioChannel.Ambient);
        }

        private void HandleServerSwingMelee(Message msg)
        {
            var message = (ServerFireSwingWeapon) msg;

            // TODO - something with the swing direction & the hit entities like display blood splatters in that direction
            var entity = _engine.GetEntity(message.EntityUser);
            if (entity != null)
                entity.Model.ModelInstance.RunAnimation(AnimationType.MeleeWeaponSwing1, false, false);

            var me = _engine.Me;
            if (me == null)
                return;

            _engine.AudioEngine.Play(Sound.BatSwing1, message.StartPoint, (int)AudioChannel.Ambient);
            if (message.HitHumans.Count > 0 || message.HitZombies.Count > 0)
                _engine.AudioEngine.Play(Sound.BatHit1, message.StartPoint, (int)AudioChannel.Ambient);
        }

        private void HandlerServerFireBullets(ServerFireBulletMessage message)
        {
            foreach (var bullet in message.BulletEffects)
            {
                var end = bullet.EndPoint;
                var start = message.StartPoint;

                _gameClient.TraceBullet(start, end, (BulletEffect)bullet.Effect, bullet.Rotation);
            }
        }

        private void HandleServerFireWeaponPistol(Message msg)
        {
            var message = (ServerFirePistolMessage)msg;
            HandlerServerFireBullets(message);

            var me = _engine.Me;
            if (me == null)
                return;

            var entity = _engine.GetEntity(message.EntityUser);
            if (entity != null)
                entity.Model.ModelInstance.RunAnimation(AnimationType.Shooting1H, false, false);


            _engine.AudioEngine.Play(Sound.GunFire1, message.StartPoint, (int)AudioChannel.Ambient);
        }

        private void HandleServerClientEntityControl(Message msg)
        {
            var message = (ServerClientEntityControlMessage) msg;
            var entityId = message.EntityId;
            var clientId = message.ClientId;

            Logger.Write(string.Format("Client {0} controlling entity {1}", clientId, entityId));

            _gameClient.SetEntityOwnership(entityId, clientId);
        }

        private void HandleServerAmmoUpdate(Message msg)
        {
            var player = _engine.Me;
            var message = (ServerAmmoUpdateMessage) msg;
            if (player != null)
            {
                //player.HandleAmmoUpdate(message);
            }
        }

        private void HandlerServerReload(Message msg)
        {
            var message = (ServerReloadWeaponMessage) msg;
            var entityId = message.EntityId;
            var entity = _engine.Entities.FirstOrDefault(e => e.EntityId == entityId);

            if (entity != null)
            {
                // todo: reload based on type of weapon being reloaded.
                entity.Model.ModelInstance.RunAnimation(AnimationType.Reloading1H, false, false);
            }
        }

        private void HandlerServerReloadComplete(Message msg)
        {
            var message = (ServerReloadCompleteMessage) msg;
            var entityId = message.EntityId;

            var player = _engine.Me;
            
            if (player != null && player.EntityId == entityId)
            {
                //player.ReloadComplete();
            }
            else
            {
                // TODO - if it's not our own player, then stop playing the reload animation for that entity
                //var entity = _engine.Entities.FirstOrDefault(e => e.EntityId == entityId);
            }
        }

        private void HandleStartItemUsage(Message msg)
        {
            var message = (ServerStartItemUsage) msg;
            _gameClient.StateMachine.CurrentState.OnActionStarted(message.EntityId, message.UsageTime, message.SpecId);
        }

        private void HandleStopItemUsage(Message msg)
        {
            var message = (ServerStopItemUsage)msg;
            _gameClient.StateMachine.CurrentState.OnActionEnded(message.EntityId, message.Success);
        }
    }
}

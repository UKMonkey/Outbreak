using System;
using Outbreak.Entities;
using Outbreak.Items.Containers;
using Outbreak.Server.Entities;
using Vortex.Interface.Net;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Net.Messages;
using Outbreak.Server.WeaponHandler;
using Psy.Core;
using Psy.Core.Logging;
using EntityBehaviourEnum = Vortex.Interface.EntityBase.Behaviours.EntityBehaviourEnum;
using Vortex.Interface.EntityBase.Properties;

namespace Outbreak.Server
{
    class MessageHandler
    {
        private readonly GameServer _gameServer;
        private WeaponHandlerFactory WeaponHandlers {get { return _gameServer.WeaponHandlers; }}
        private readonly IInventoryCache _inventoryCache;

        public MessageHandler(GameServer gameServer, IInventoryCache inventoryCache)
        {
            _gameServer = gameServer;
            _inventoryCache = inventoryCache;
        }

        public void RegisterMessageCallbacks()
        {
            _gameServer.Engine.RegisterMessageCallback(typeof(ClientMoveMessage), HandleClientMove);
            _gameServer.Engine.RegisterMessageCallback(typeof(ClientStartEquippedReload), HandleToggleEquippedReload);
            _gameServer.Engine.RegisterMessageCallback(typeof(ClientStartFireWeaponMessage), HandleClientStartFireWeapon);
            _gameServer.Engine.RegisterMessageCallback(typeof(ClientStopFireWeaponMessage), HandleClientStopFireWeapon);
            _gameServer.Engine.RegisterMessageCallback(typeof(ClientSuicideMessage), HandleClientSuicide);
            _gameServer.Engine.RegisterMessageCallback(typeof(ClientInventoryDragDrop), HandleClientInventoryDragDrop);
            _gameServer.Engine.RegisterMessageCallback(typeof(ClientInventoryDropToFloor), HandleClientInventoryDropToFloor);
            _gameServer.Engine.RegisterMessageCallback(typeof(ClientInteractWithEntityMessage), HandleClientInteractMessage);
            _gameServer.Engine.RegisterMessageCallback(typeof(ClientUseItemMessage), HandleUseItem);
            _gameServer.Engine.RegisterMessageCallback(typeof(ClientCancelUseItemMessage), CancelHandleUseItem);
        }

        private const float MaxSquaredDistance = 1.5f * 1.5f;

        private void HandleClientInteractMessage(Message msg)
        {
            var message = (ClientInteractWithEntityMessage)msg;

            var entityId = message.EntityId;
            
            var toggleEntity = _gameServer.Engine.GetEntity(entityId);
            if (toggleEntity == null)
                return;

            var playerEntity = _gameServer.GetEntityForRemotePlayer(msg.Sender);
            if (playerEntity ==  null)
                return;
            
            if (toggleEntity.GetPosition().Distance(playerEntity.GetPosition()) > MaxSquaredDistance)
                return;

            toggleEntity.PerformBehaviour((short)EntityBehaviourEnum.OnInteract, playerEntity);
        }

        private void HandleClientInventoryDragDrop(Message msg)
        {
            var player = _gameServer.GetEntityForRemotePlayer(msg.Sender);
            if (player == null)
            {
                Logger.Write("HandleClientInventoryDragDrop: Client is not attached to an entity");
                return;
            }

            var message = (ClientInventoryDragDrop) msg;

            var sourceInventory = _inventoryCache.GetInventory(message.SourceInventoryId);
            var targetInventory = _inventoryCache.GetInventory(message.TargetInventoryId);

            var currentTargetItem = targetInventory[message.TargetSlot];
            var sourceItem = sourceInventory[message.SourceSlot];

            if (currentTargetItem != null)
            {
                currentTargetItem.Inventory.CombineStacks(currentTargetItem, sourceItem);
                targetInventory.AddItem(sourceItem, false, message.TargetSlot);
            }
            else
            {
                sourceInventory.SetItem(message.SourceSlot, currentTargetItem);
                targetInventory.SetItem(message.TargetSlot, sourceItem);
            }
        }

        private void HandleClientInventoryDropToFloor(Message msg)
        {
            var message = (ClientInventoryDropToFloor) msg;

            var player = _gameServer.GetEntityForRemotePlayer(msg.Sender);
            if (player == null)
                return;

            // todo: check that the player has access to the specified inventory. possible exploit.

            var inventory = _inventoryCache.GetInventory(message.InventoryId);
            var inventoryItem = inventory[message.SourceSlot];
            var entity = _gameServer.EntityFactory.Get((short)EntityTypeEnum.InventoryItem);

            entity.SetInventoryItem(inventoryItem);
            entity.SetPosition(player.GetPosition());

            _gameServer.Engine.SpawnEntity(entity);

            inventory.SetItem(message.SourceSlot, null); 
        }

        private void HandleClientSuicide(Message msg)
        {
            var player = _gameServer.GetEntityForRemotePlayer(msg.Sender);
            if (player == null)
            {
                Logger.Write("HandleClientSuicide: Client is not attached to an entity");
                return;
            }

            player.Destroy();

            _gameServer.Engine.BroadcastSay(string.Format("Player {0} killed themselves", msg.Sender.PlayerName));
        }

        /// <summary>
        /// Server - Tell all clients a weapon has been fired.
        /// </summary>
        /// <param name="msg"></param>
        private void HandleClientStartFireWeapon(Message msg)
        {
            var message = (ClientStartFireWeaponMessage) msg;

            var rotation = message.Rotation;
            var client = msg.Sender;

            var player = _gameServer.GetEntityForRemotePlayer(client);
            if (player == null)
            {
                Logger.Write("HandleClientFireWeapon: Client is not attached to a player entity");
                return;
            }

            // make sure the player has the ammo required to fire the weapon.
            // update the rotation incase they're doing something mad ....
            player.SetRotation(rotation);
            var weapon = player.GetEquippedItem();
            WeaponHandlers.StartWeaponShoot(player, weapon);
        }

        private void HandleClientStopFireWeapon(Message msg)
        {
            var client = msg.Sender;
            var player = _gameServer.GetEntityForRemotePlayer(client);

            if (player == null)
                return;

            var weapon = player.GetEquippedItem();
            if (weapon == null)
                return;

            WeaponHandlers.StopWeaponShoot(player, weapon);
        }

        /// <summary>
        /// Server - handle client saying they've moved themselves
        /// </summary>
        /// <param name="msg"></param>
        private void HandleClientMove(Message msg)
        {
            var message = (ClientMoveMessage) msg;
            var position = message.Position;
            var rotation = message.Rotation;
            var movementVector = message.MovementVector;

            // find the client's entity.
            var client = msg.Sender;

            var player = _gameServer.GetEntityForRemotePlayer(client);
            if (player == null)
            {
                // client is not attached to a player entity.
                Logger.Write("HandleClientMove: Client is not attached to a player entity");
                return;
            }

            player.SetMovementVector(movementVector);
            player.SetPosition(position);
            player.SetRotation(rotation);
        }

        private void HandleToggleEquippedReload(Message msg)
        {
            var player = _gameServer.GetEntityForRemotePlayer(msg.Sender);

            if (player == null)
                return;

            var weapon = player.GetEquippedItem();

            if (weapon == null)
                return;

            if (weapon.GetIsReloading() || weapon.GetIsFiring())
                return;

            var spec = weapon.GetItemSpec();
            if (spec.GetAmmoType() == null)
                return;

            var startReloadTime = spec.GetStartReloadTime();
            
            var message = new ServerReloadWeaponMessage {EntityId = player.EntityId};
            _gameServer.Engine.SendMessage(message);
            
            player.SetReloadingPrimary(true);
            player.SetNextPrimaryReloadTime((long)Math.Round(Timer.GetTime()) + startReloadTime);
        }

        private void HandleUseItem(Message msg)
        {
            var message = (ClientUseItemMessage) msg;

            var player = _gameServer.GetEntityForRemotePlayer(msg.Sender);
            if (player == null)
                return;

            var inventory = _inventoryCache.GetInventory(message.InventoryId);
            if (inventory == null)
                return;

            var item = inventory[message.InventorySlotId];
            if (item == null || item.GetCount() == 0)
                return;

            _gameServer.ItemUserManager.StartUsage(player, item);
        }

        private void CancelHandleUseItem(Message msg)
        {
            var player = _gameServer.GetEntityForRemotePlayer(msg.Sender);
            if (player == null)
                return;

            _gameServer.ItemUserManager.CancelUsage(player);
        }
    }
}

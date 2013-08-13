using Outbreak.Audio;
using Outbreak.Entities.Properties;
using Outbreak.Net.Messages;
using Outbreak.Resources;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;
using Psy.Core;
using Vortex.Interface.EntityBase.Properties;

namespace Outbreak.Server.Entities.Behaviours.OnInteract
{
    public class OpenInventory : IEntityBehaviour
    {
        private readonly GameServer _gameServer;

        public OpenInventory(GameServer gameServer)
        {
            _gameServer = gameServer;
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            var distanceBetween = target.GetPosition().DistanceSquared(instigator.GetPosition());

            if (distanceBetween > 3.0f)
                return;

            if (!target.HasInventory())
                return;

            var player = instigator.GetPlayer(_gameServer.Engine);
            var message = new ServerShowContainerContentsMessage { ContainerEntityId = target.EntityId };

            _gameServer.Engine.SendMessageToClient(message, player);
            _gameServer.Engine.PlaySoundOnEntity(target, (int)AudioChannel.Ambient, Sound.UIClickSoundName);
            
        }
    }
}
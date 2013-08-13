using System;
using Outbreak.Entities;
using Outbreak.Entities.Properties;
using Psy.Core;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;

namespace Outbreak.Server.Entities.Behaviours.OnKilled
{
    public class DropInventory : IEntityBehaviour
    {
        private readonly IServer _server;

        public DropInventory(IServer server)
        {
            _server = server;
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            if (!target.HasInventory())
                return;

            var inventory = target.GetInventory();

            foreach (var item in inventory.GetContent().Values)
            {
                if (item == null)
                    continue;

                var position = target.GetPosition();
                var distance = StaticRng.Random.NextDouble(0.2, 1);
                var rotation = StaticRng.Random.NextDouble(-Math.PI, Math.PI);

                var addition = DirectionUtil.CalculateVector((float)rotation) * (float)distance;

                var inventoryItem = _server.EntityFactory.Get((short) EntityTypeEnum.InventoryItem);
                inventoryItem.SetInventoryItem(item);
                inventoryItem.SetPosition(position + addition);

                _server.SpawnEntity(inventoryItem);
            }

            target.RemoveInventory();
        }
    }
}

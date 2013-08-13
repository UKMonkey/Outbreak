using Outbreak.Entities.Properties;
using Outbreak.Items.Containers;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Behaviours;

namespace Outbreak.Server.Entities.Behaviours.OnThinking
{
    public class AttackChaseTarget : IEntityBehaviour
    {
        private readonly GameServer _server;

        public AttackChaseTarget(GameServer server)
        {
            _server = server;
        }

        private void StopUsingWeapon(Entity target)
        {
            if (target.GetWeaponUseCount() == 0)
                return;

            var weapon = target.GetInventory()[InventorySpecialSlotEnum.PrimaryWeapon];
            _server.WeaponHandlers.StopWeaponShoot(target, weapon);
        }

        public void PerformBehaviour(Entity target, Entity instigator)
        {
            var chaseTargetId = target.GetChaseTargetId();
            if (chaseTargetId == null)
            {
                StopUsingWeapon(target);
                return;
            }

            var chaseTarget = _server.Engine.GetEntity(chaseTargetId.Value);
            if (chaseTarget == null)
            {
                StopUsingWeapon(target);
                return;
            }

            var weapon = target.GetInventory()[InventorySpecialSlotEnum.PrimaryWeapon];
            var range = weapon.GetItemSpec().GetMeleeRange();
            var chaseVector = chaseTarget.GetPosition() - target.GetPosition();

            if (chaseVector.Length - target.Radius - chaseTarget.Radius <= range)
                _server.WeaponHandlers.StartWeaponShoot(target, weapon);
            else
                StopUsingWeapon(target);
        }
    }
}

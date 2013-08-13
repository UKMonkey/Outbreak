using System.Collections.Generic;
using Vortex.Interface.EntityBase;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs.Types;

namespace Outbreak.Server.WeaponHandler
{
    public interface IWeaponUseHandler
    {
        HashSet<WeaponTypes> GetApplicableWeapons();

        void StartUseWeapon(Entity owner, InventoryItem weapon);
        void StopUseWeapon(Entity owner, InventoryItem weapon);

        void Update();
    }
}

using System.Collections.Generic;

namespace Daybreak
{
    public static class InventoryLookup
    {
        public static readonly Dictionary<byte, string> Lookup = new Dictionary<byte, string>
        {
            {InventoryTypeIds.Glock, "Glock 9mm"},
            {InventoryTypeIds.Ammo9Mm, "9mm ammunition"},
        }; 
    }
}

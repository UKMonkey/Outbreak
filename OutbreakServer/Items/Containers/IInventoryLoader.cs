using System.Collections.Generic;
using Outbreak.Items.Containers;

namespace Outbreak.Server.Items.Containers
{
    public interface IInventoryLoader
    {
        Inventory LoadInventory(long id);
        HashSet<long> GetAvailableIds();
    }
}

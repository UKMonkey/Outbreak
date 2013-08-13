using System.Collections.Generic;
using Outbreak.Items.Containers.InventorySpecs;

namespace Outbreak.Server.Items.Containers
{
    public interface IItemSpecLoader
    {
        Dictionary<int, ItemSpec> LoadSpecs();
    }
}

using System.Collections.Generic;

namespace Outbreak.Items.ItemGenerators
{
    public class ItemGeneratorDictionary : Dictionary<ItemTypeEnum, IItemGenerator>
    {
        public ItemGeneratorDictionary() : base(10) { }

        public void Register(ItemTypeEnum itemType, IItemGenerator generator)
        {
            this[itemType] = generator;
        }

        public IItemGenerator Get(ItemTypeEnum itemType)
        {
            return this[itemType];
        }
    }
}
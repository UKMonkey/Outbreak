using System;
using System.Collections.Generic;
using Outbreak.Items;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Resources;

namespace Outbreak.Server.World.ItemGenerators
{
    public enum FoodType
    {
        PastaTin
    }

    public class FoodGenerator : IItemGenerator
    {
        private readonly Dictionary<FoodType, ItemSpec> _itemSpec;

        public FoodGenerator()
        {
            _itemSpec = new Dictionary<FoodType, ItemSpec>();
        }

        public InventoryItem Generate()
        {
            var foodItem = GetRandomFoodItemType();
            var itemSpec = GenerateImpl(foodItem);
            var inventoryItem = new InventoryItem(itemSpec.Id);
            inventoryItem.SetCount(1);

            return inventoryItem;
        }

        private static FoodType GetRandomFoodItemType()
        {
            return FoodType.PastaTin;
        }

        private ItemSpec GenerateImpl(FoodType foodType)
        {
            if (_itemSpec.ContainsKey(foodType))
            {
                return _itemSpec[foodType];
            }

            var spec = new ItemSpec();

            spec.SetName(GetItemName(foodType));
            spec.SetImageName(GetImageName(foodType));
            spec.SetDescription(GetItemDescription(foodType));
            spec.SetModelName(Models.AmmoCrate01);
            spec.SetHungerReduceAmount(GetHungerReduceAmount(foodType));
            spec.SetBaseUsageTime(3000);
            spec.SetStackMax(1);

            spec = StaticItemSpecCache.Instance.AddSpec(spec);

            return spec;
        }

        private string GetImageName(FoodType foodType)
        {
            switch (foodType)
            {
                case FoodType.PastaTin:
                    return "pastaTin.png";
                default:
                    throw new ArgumentOutOfRangeException("foodType");
            }
        }

        private short GetHungerReduceAmount(FoodType foodType)
        {
            switch (foodType)
            {
                case FoodType.PastaTin:
                    return 30;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetItemDescription(FoodType foodType)
        {
            switch (foodType)
            {
                case FoodType.PastaTin:
                    return "A tin of pasta. Delicious.";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetItemName(FoodType foodType)
        {
            switch (foodType)
            {
                case FoodType.PastaTin:
                    return "Pasta Tin";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
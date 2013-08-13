using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Outbreak;
using Outbreak.Items.Containers;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;

namespace UnitTests.InventoryTests
{
    public class TestItemSpecCache : IItemSpecCache
    {
        public event ItemSpecCallback OnItemAdded;
        private readonly List<ItemSpec> _specs;

        public TestItemSpecCache()
        {
            _specs = new List<ItemSpec>();
        }

        public int EmptySpecId
        {
            get { return -1; }
        }

        public ItemSpec GetItemSpec(int id)
        {
            return _specs.Single(x => x.Id == id);
        }

        public ItemSpec AddSpec(ItemSpec spec)
        {
            _specs.Add(spec);
            return spec;
        }

        public IEnumerable<ItemSpec> GetSpecsOfType(ItemSpecChecker checker)
        {
            return _specs.Where(x => checker(x));
        }
    }

    [TestFixture]
    public class AddingItems
    {
        protected Inventory Inventory { get; set; }
        protected Inventory Inventory2 { get; set; }

        protected ItemSpec CreamCake { get; set; }

        protected ItemSpec YuleLog { get; set; }

        protected const int MintStackMax = 10;
        protected ItemSpec WaferThinMint { get; set; }

        [SetUp]
        public void SetUp()
        {
            StaticItemSpecCache.Instance = new TestItemSpecCache();

            Inventory = new Inventory(1, true);
            Inventory.Initialise(Consts.PlayerBackpackSize, InventoryType.PlayerBackpack);

            Inventory2 = new Inventory(2, true);
            Inventory2.Initialise(Consts.PlayerBackpackSize, InventoryType.PlayerBackpack);

            CreamCake = new ItemSpec(1);
            CreamCake.SetStackMax(1); // Can't stack cream cakes
            StaticItemSpecCache.Instance.AddSpec(CreamCake);

            YuleLog = new ItemSpec(2);
            YuleLog.SetStackMax(1); // Can't stack yule logs
            StaticItemSpecCache.Instance.AddSpec(YuleLog);

            WaferThinMint = new ItemSpec(3);
            WaferThinMint.SetStackMax(MintStackMax);
            StaticItemSpecCache.Instance.AddSpec(WaferThinMint);
        }

        

        public void FillInventory(int freeSlots)
        {
            int itemCount = Consts.PlayerBackpackSize - (((int)InventorySpecialSlotEnum.ContainerStart) + freeSlots);
            for (var i = 1; i <= itemCount; i++)
            {
                var item = GetCreamCake();
                Inventory.AddItem(item, false);
            }
        }

        [Test]
        public void CombineInventorySlotsInTheSameInventory()
        {
            var mints1 = GetWaferThinMint();
            mints1.SetCount(2);
            mints1 = Inventory.SetItem(12, mints1);

            var mints2 = GetWaferThinMint();
            mints2.SetCount(6);
            mints2 = Inventory.SetItem(13, mints2);

            Inventory.CombineStacks(mints1, mints2);

            Assert.That(Inventory[12].GetCount(), Is.EqualTo(8));
            Assert.That(Inventory[13], Is.Null);
        }

        [Test] 
        public void AddingMintsUntilItExplodes()
        {
            const int count = (Consts.PlayerBackpackSize - (short)InventorySpecialSlotEnum.ContainerStart)*MintStackMax;
            const int step = MintStackMax - 2;
            var added = 0;

            while (added <= count)
            {
                var mints = GetWaferThinMint();
                mints.SetCount(step);
                added += step;

                var result = Inventory.AddItem(mints);

                if (added <= count)
                    Assert.That(result, Is.EqualTo(true));
                else
                    Assert.That(result, Is.EqualTo(false));
            }
        }

        [Test]
        public void CombineInventorySlotsInTheSameInventoryWithTooManyForOneStack()
        {
            var mints1 = GetWaferThinMint();
            mints1.SetCount(8);
            mints1 = Inventory.SetItem(12, mints1);

            var mints2 = GetWaferThinMint();
            mints2.SetCount(8);
            mints2 = Inventory.SetItem(13, mints2);

            Inventory.CombineStacks(mints1, mints2);

            Assert.That(Inventory[12].GetCount(), Is.EqualTo(10));
            Assert.That(Inventory[13].GetCount(), Is.EqualTo(6));
        }

        [Test]
        public void CombineInventorySlotsFromDifferentInventories()
        {
            var mints1 = GetWaferThinMint();
            mints1.SetCount(8);
            mints1 = Inventory.SetItem(12, mints1);

            var mints2 = GetWaferThinMint();
            mints2.SetCount(8);
            mints2 = Inventory2.SetItem(12, mints2);

            Inventory.CombineStacks(mints1, mints2);

            Assert.That(Inventory[12].GetCount(), Is.EqualTo(10), "Destination stack");
            Assert.That(Inventory2[12].GetCount(), Is.EqualTo(6), "Source stack");
        }

        [Test]
        public void AddingStackOfMints()
        {
            var initialStack = GetWaferThinMint();
            initialStack.SetCount(8);
            var firstResult = Inventory.AddItem(initialStack);

            var newStack = GetWaferThinMint();
            newStack.SetCount(4);
            var secondResult = Inventory.AddItem(newStack);

            var addedStacks = Inventory
                .GetContent()
                .Where(x => (x.Value != null) && (x.Value.ItemSpecId == WaferThinMint.Id))
                .ToList();

            Assert.That(addedStacks[0].Value.GetCount(), Is.EqualTo(10));
            Assert.That(addedStacks[1].Value.GetCount(), Is.EqualTo(2));
            Assert.That(firstResult, Is.EqualTo(true));
            Assert.That(secondResult, Is.EqualTo(true));
        }

        [Test]
        public void AddingStackOfMintsWithNotEnoughRoom()
        {
            FillInventory(1);

            var initialStack = GetWaferThinMint();
            initialStack.SetCount(8);
            var firstResult = Inventory.AddItem(initialStack);

            var newStack = GetWaferThinMint();
            newStack.SetCount(4);
            var secondResult = Inventory.AddItem(newStack);

            var addedStacks = Inventory
                .GetContent()
                .Where(x => (x.Value != null) && (x.Value.ItemSpecId == WaferThinMint.Id))
                .ToList();

            Assert.That(addedStacks.Count, Is.EqualTo(1));
            Assert.That(addedStacks[0].Value.GetCount(), Is.EqualTo(10));
            Assert.That(firstResult, Is.EqualTo(true));
            Assert.That(secondResult, Is.EqualTo(false));
        }

        [Test]
        public void YuleLogsShouldNotStack()
        {
            var preSize = Inventory.OccupiedSlots;

            Inventory.AddItem(GetYuleLog());
            Assert.That(Inventory.OccupiedSlots, Is.EqualTo(preSize - 1));

            Inventory.AddItem(GetYuleLog());
            Assert.That(Inventory.OccupiedSlots, Is.EqualTo(preSize - 2));
        }

        [Test]
        public void WaferThinMintsShouldStack()
        {
            var preSize = Inventory.OccupiedSlots;

            for (int i = 0; i < 10; i++)
            {
                var item = GetWaferThinMint();
                Inventory.AddItem(item);
            }

            Assert.That(Inventory.OccupiedSlots, Is.EqualTo(preSize-1));
        }

        [Test]
        public void AddingASingleItem()
        {
            var item = GetCreamCake();

            var preOccupiedSlots = Inventory.OccupiedSlots;
            Inventory.AddItem(item, false);
            var postOccupiedSlots = Inventory.OccupiedSlots;

            Assert.That(postOccupiedSlots, Is.EqualTo(preOccupiedSlots-1));
        }

        [Test]
        public void AddingTwoItems()
        {
            var item1 = GetCreamCake();
            var item2 = GetYuleLog();

            Inventory.AddItem(item1, false);
            Inventory.AddItem(item2, false);
        }

        [Test]
        public void FillingInventory()
        {
            const int itemCount = Consts.PlayerBackpackSize - (int) InventorySpecialSlotEnum.ContainerStart;
            for (var i = 1; i <= itemCount; i++)
            {
                var item = GetCreamCake();
                Inventory.AddItem(item, false);
            }
        }

        [Test]
        public void OverFillingInventory()
        {
            const int itemCount = Consts.PlayerBackpackSize - (int)InventorySpecialSlotEnum.ContainerStart;
            for (var i = 1; i <= itemCount; i++)
            {
                var item = GetCreamCake();
                Inventory.AddItem(item, false);
            }

            var waferThinMint = GetWaferThinMint();
            Assert.That(Inventory.AddItem(waferThinMint), Is.False);
        }

        private InventoryItem GetWaferThinMint()
        {
            var item = new InventoryItem(WaferThinMint.Id);
            item.SetCount(1);
            return item;
        }

        private InventoryItem GetCreamCake()
        {
            var item = new InventoryItem(CreamCake.Id);
            item.SetCount(1);
            return item;
        }

        private InventoryItem GetYuleLog()
        {
            var item = new InventoryItem(YuleLog.Id);
            item.SetCount(1);
            return item;
        }
    }
}
using System;
using System.Collections.Generic;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Outbreak.Net.Messages;
using Psy.Core;
using Psy.Core.Logging;
using Vortex.Interface;
using Vortex.Interface.EntityBase;
using Vortex.Interface.EntityBase.Properties;
using Outbreak.Entities.Properties;

namespace Outbreak.Server.Items.Users
{
    public class InventoryItemUser
    {
        private class ItemBeingUsed
        {
            public double FinishTime;
            public Entity Entity;
            public InventoryItem Item;
        }

        // we could make this a lot faster for lots of items
        // eg - by keeping the list ordered by the time things would be finished etc
        //
        // but it's probably not needed as there won't be many items being used at any one time
        // so the list should stay very small

        private readonly List<ItemBeingUsed> _itemsInUse;
        private readonly IEngine _engine;


        public InventoryItemUser(IEngine engine)
        {
            _itemsInUse = new List<ItemBeingUsed>();
            _engine = engine;
        }

        private static bool ApplyHealAmount(Entity user, InventoryItem itemUsed, ItemSpec itemSpec)
        {
            var healAmount = itemSpec.GetHealAmount();
            if (healAmount == 0)
                return false;

            user.SetHealth(user.GetHealth() + healAmount);

            return true;
        }

        private static bool ApplyHungerAmount(Entity user, InventoryItem itemUsed, ItemSpec itemSpec)
        {
            var hungerReduceAmount = itemSpec.GetHungerReduceAmount();
            if (hungerReduceAmount == 0)
                return false;

            var hunger = user.GetHunger();

            if (hunger == 0)
                return false;

            hunger -= hungerReduceAmount;

            hunger = Math.Min(Math.Max(hunger, (short)0), (short)Consts.MaxHunger);

            if (hunger < 0)
                hunger = 0;

            user.SetHunger(hunger);

            return true;
        }

        private void PerformUsage(Entity user, InventoryItem itemUsed)
        {
            var consumed = false;

            var itemSpec = itemUsed.GetItemSpec();

            consumed |= ApplyHealAmount(user, itemUsed, itemSpec);
            consumed |= ApplyHungerAmount(user, itemUsed, itemSpec);

            if (consumed)
            {
                Logger.Write(string.Format("Item `{0}` used, and consumed.", itemUsed.GetItemSpec().GetName()));
                itemUsed.SetCount((short)(itemUsed.GetCount() - 1));
            }
            else
            {
                Logger.Write(string.Format("Item `{0}` used and was NOT consumed.", itemUsed.GetItemSpec().GetName()));
            }

            var msg = new ServerStopItemUsage
            {
                EntityId = user.EntityId,
                Success = true
            };
            _engine.SendMessage(msg);
        }

        public void Update()
        {
            var now = Timer.GetTime();
            for (var i=0; i<_itemsInUse.Count; ++i)
            {
                var item = _itemsInUse[i];
                if (item.FinishTime > now)
                    continue;

                PerformUsage(item.Entity, item.Item);
                _itemsInUse.RemoveAt(i);
                --i;
            }
        }

        public void StartUsage(Entity user, InventoryItem itemUsed)
        {
            var timeToUse = (double)itemUsed.GetItemSpec().GetBaseUsageTime();
            if (timeToUse < 0)
                return;

            // TODO - get users skill and adjust finish time
            var finishTime = timeToUse + Timer.GetTime();
            _itemsInUse.Add(new ItemBeingUsed{Entity = user, Item = itemUsed, FinishTime = finishTime});

            var msg = new ServerStartItemUsage
                          {
                              EntityId = user.EntityId,
                              SpecId = itemUsed.GetItemSpec().Id,
                              UsageTime = (int)Math.Round(timeToUse)
                          };
            _engine.SendMessage(msg);
        }

        public void CancelUsage(Entity user)
        {
            _itemsInUse.RemoveAll(item => item.Entity.EntityId == user.EntityId);
            var msg = new ServerStopItemUsage
                          {
                              EntityId = user.EntityId,
                              Success = false
                          };
            _engine.SendMessage(msg);
        }
    }
}

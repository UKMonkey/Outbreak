using System;
using Vortex.Interface.EntityBase.Properties;
using Outbreak.Entities.Properties;
using Outbreak.Entities;
using Outbreak.Items.Containers;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;
using Psy.Core.Console;

namespace Outbreak.Server
{
    public class ConsoleCommands
    {
        private GameServer Server { get; set; }

        public ConsoleCommands(GameServer server)
        {
            Server = server;
        }

        public void BindConsoleCommands()
        {
            StaticConsole.Console.CommandBindings.Bind("killall", "Kills all enemies", KillAllEnemies);
            StaticConsole.Console.CommandBindings.Bind("give", "Give item to a player", Give);
            StaticConsole.Console.CommandBindings.Bind("stash", "Drop a stash at the players feet", Stash);
            StaticConsole.Console.CommandBindings.Bind("bolt", "Make player run as fast as Usain Bolt", Bolt);
            StaticConsole.Console.CommandBindings.Bind("god", "Can't be killed, zombies don't attack.", God);
        }

        private void God(string[] parameters)
        {
            var remotePlayer = Server.Engine.ConsoleCommandContext.Sender;
            var entity = Server.GetEntityForRemotePlayer(remotePlayer);
            entity.SetIsGod(true);
        }

        private void Bolt(string[] parameters)
        {
            var remotePlayer = Server.Engine.ConsoleCommandContext.Sender;
            var entity = Server.GetEntityForRemotePlayer(remotePlayer);
            entity.SetRunSpeed(2.0f);
            Server.Engine.ConsoleText(string.Format("{0} can now run as fast as Usain Bolt!", remotePlayer.PlayerName));
        }

        private void Stash(string[] parameters)
        {
            var remotePlayer = Server.Engine.ConsoleCommandContext.Sender;
            var entity = Server.GetEntityForRemotePlayer(remotePlayer);
            if (entity == null)
            {
                Server.Engine.ConsoleText(string.Format("Unable to spawn stash when dead!"));
                return;
            }

            var position = entity.GetPosition();

            var stash = Server.EntityFactory.Get(EntityTypeEnum.SmallStash);
            stash.SetPosition(position);
            Server.Engine.SpawnEntity(stash);

            Server.Engine.ConsoleText(string.Format("{0} spawned a stash of cool stuff!", remotePlayer.PlayerName));
        }

        private void Give(string[] parameters)
        {
            // 0 = Player Name, 1 = Item id, 2 = Quantity.

            int itemSpecId;
            if (!int.TryParse(parameters[2], out itemSpecId))
            {
                Server.Engine.BroadcastSay("Give: Invalid item Id");
                return;
            }

            var itemSpec = StaticItemSpecCache.Instance.GetItemSpec(itemSpecId);
            if (itemSpec == null)
            {
                Server.Engine.BroadcastSay("Give: Could not find item id");
                return;
            }

            short quantity;
            if (!short.TryParse(parameters[3], out quantity) || quantity <= 0)
            {
                Server.Engine.BroadcastSay("Give: Invalid quantity");
                return;
            }

            var player = Server.Engine.GetEntity(parameters[1]);
            if (player == null)
            {
                Server.Engine.BroadcastSay("Give: Could not find player.");
                return;
            }

            var targetInventory = player.GetInventory();
            var item = new InventoryItem(targetInventory, itemSpecId);

            item.SetCount(Math.Min(quantity, itemSpec.GetStackMax()));
            if (itemSpec.HasDurability())
                item.SetDurability(itemSpec.GetDurabilityMax());

            player.GetInventory().AddItem(item);

            Server.Engine.BroadcastSay(
                string.Format("Gave {0} units of {1} to {2}", 
                    quantity, itemSpec.GetName(), player.GetPlayer(Server.Engine).PlayerName));
        }

        private void KillAllEnemies(string[] parameters)
        {
            foreach (var entity in Server.Engine.Entities)
            {
                if (entity.EntityTypeId == (short)EntityTypeEnum.Zombie)
                {
                    entity.Destroy();
                }
            }

            Server.Engine.BroadcastSay("All zombies killed.");
        }
    }
}
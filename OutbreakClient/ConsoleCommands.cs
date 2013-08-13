using System;
using Outbreak.Net.Messages;
using Psy.Core;
using Psy.Core.Console;
using Outbreak.Entities.Properties;
using Outbreak.Items.Containers.InventoryItems;
using Outbreak.Items.Containers.InventorySpecs;

namespace Outbreak.Client
{
    class ConsoleCommands
    {
        private GameClient GameClient { get; set; }
        public string RconPassword { get; set; }

        public ConsoleCommands(GameClient gameClient)
        {
            GameClient = gameClient;
            RconPassword = "";
        }

        public void BindConsoleCommands()
        {
            StaticConsole.Console.CommandBindings.Bind(
                "say", "say something to the server", ConsoleCommandSay);
            StaticConsole.Console.CommandBindings.Bind(
                "name", "change player name", ConsoleCommandName);
            StaticConsole.Console.CommandBindings.Bind(
                "rcon_password", "set rcon password", ConsoleCommandRconPassword);
            StaticConsole.Console.CommandBindings.Bind(
                "rcon", "issue rcon command", ConsoleCommandRcon);
            StaticConsole.Console.CommandBindings.Bind(
                "disconnect", "disconnect from the server", ConsoleDisconnect);
            StaticConsole.Console.CommandBindings.Bind(
                "kill", "kill yourself. stupid.", ConsoleKill);
            StaticConsole.Console.CommandBindings.Bind(
                "time", "Show game time", ConsoleGameTime);
            StaticConsole.Console.CommandBindings.Bind(
                "inv", "Show player inventory", ConsoleInv);
        }

        private void ConsoleInv(string[] parameters)
        {
            var player = GameClient.Engine.Me;
            if (player == null)
            {
                StaticConsole.Console.AddLine("No player entity", Colours.Red);
                return;
            }

            var inventory = player.GetInventory();

            foreach (var inventorySlot in inventory.GetContent())
            {
                var value = inventorySlot.Value;

                if (value == null)
                {
                    StaticConsole.Console.AddLine(string.Format("{0} = empty", inventorySlot.Key));
                    continue;
                }

                var itemSpec = value.GetItemSpec();
                var name = itemSpec.GetName();

                StaticConsole.Console.AddLine(string.Format("{0} = {1} x{2}", inventorySlot.Key, name, value.GetCount()));
            }
        }

        private void ConsoleGameTime(params string[] parameters)
        {
            StaticConsole.Console.AddLine(
                string.Format(
                    "The time is {0}:{1}", 
                    GameClient.GameTime.Hour, GameClient.GameTime.Minute));
        }

        private void ConsoleKill(string[] parameters)
        {
            var killMessage = new ClientSuicideMessage();
            GameClient.Engine.SendMessage(killMessage);
        }

        private void ConsoleDisconnect(string[] parameters)
        {
            GameClient.Engine.Disconnect();
            GameClient.StateMachine.TransitionTo("menu");
        }

        private void ConsoleCommandRcon(string[] parameters)
        {
            var command = string.Join(" ", parameters, 1, parameters.Length - 1);
            GameClient.Engine.SendRconCommand(command, RconPassword);
        }

        private void ConsoleCommandRconPassword(string[] parameters)
        {
            if (parameters.Length != 2)
                return;

            RconPassword = parameters[1];
            StaticConsole.Console.AddLine("Rcon password set");
        }

        private void ConsoleCommandName(string[] parameters)
        {
            if (parameters.Length != 2)
                return;

            GameClient.PlayerName = parameters[1];
            StaticConsole.Console.AddLine(String.Format("Name changed to {0}", GameClient.PlayerName));
        }

        private void ConsoleCommandSay(string[] parameters)
        {
            GameClient.Engine.BroadcastSay(string.Join(" ", parameters, 1, parameters.Length - 1));
        }
    }
}

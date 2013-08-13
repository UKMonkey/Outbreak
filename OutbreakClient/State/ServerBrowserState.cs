using System;
using System.Collections.Generic;
using Outbreak.Client.Net;
using Psy.Gui.Components;
using SlimMath;

namespace Outbreak.Client.State
{
    public class ServerBrowserState : MenuState
    {
        private Widget _backButton;
        private readonly List<int> _joinServerButtonIds;

        public ServerBrowserState()
        {
            _joinServerButtonIds = new List<int>();
        }

        public override void OnTransitionOut()
        {
            _backButton.Delete();
            DeleteButtons();
        }

        public override void OnTransitionIn()
        {
            Engine.Gui.Desktop.Transparent = false;
            Engine.Gui.Desktop.Opacity = 0.75f;

            _backButton = Engine.Gui.CreateButton("Back", new Vector2(20, 20), new Vector2(100, 30), Engine.Gui.Desktop);
            _backButton.Click += (sender, args) => Context.GameClient.StateMachine.TransitionTo("menu");

            CreateJoinServerButtons();
        }

        private void DeleteButtons()
        {
            foreach (var widgetId in _joinServerButtonIds)
            {
                var widget = Engine.Gui.Desktop.FindWidget(widgetId);
                if (widget != null)
                {
                    widget.Delete();
                }
            }
        }

        private void CreateJoinServerButtons()
        {
            var serverBrowser = new ServerBrowser();

            var y = 300;
            const int buttonHeight = 23;
            foreach (var host in serverBrowser.GetHosts())
            {
                var widget = Engine.Gui.CreateButton(
                    host.Hostname,
                    new Vector2(250, y),
                    new Vector2(300, buttonHeight)
                    );

                var buttonHost = host;
                widget.Click += (sender, args) => ConnectToServer(buttonHost.Hostname, buttonHost.Port);

                _joinServerButtonIds.Add(widget.Id);

                y += (int)(buttonHeight * 1.5f);
            }
        }

        private void ConnectToServer(string host, int port)
        {
            Engine.HideSplashImage();
            Engine.Gui.Desktop.Visible = false;
            Engine.Gui.Desktop.Transparent = false;

            Context.GameClient.PlayerName = String.Format("Player({0})", DateTime.UtcNow.Second);
            Engine.ConsoleText(string.Format("Connecting to {0}", host));
            Engine.ConnectToServer(host, port);
            Context.GameClient.StateMachine.TransitionTo("ingame");
        }
    }
}
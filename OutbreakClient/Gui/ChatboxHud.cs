using System;
using Psy.Core;
using Psy.Gui.Components;

namespace Outbreak.Client.Gui
{
    public class ChatboxHud : IDisposable
    {
        private readonly GameClient _client;
        private readonly Widget _chatWindow;
        private readonly TextList _textLines;
        private readonly Textbox _inputBox;

        public ChatboxHud(GameClient client)
        {
            _client = client;
            _chatWindow = _client.Engine.GuiLoader.Load("chatbox.xml", _client.Engine.Gui.Desktop);
            _textLines = _client.Engine.Gui.GetWidgetByName<TextList>("chatLines");
            _inputBox = _client.Engine.Gui.GetWidgetByName<Textbox>("chatInput");
        }

        public void Dispose()
        {
            _chatWindow.Delete();
        }

        public void AddXPNotificationText(int amount)
        {
            _textLines.AddLine(string.Format("You gained {0} XP", amount), Colours.Yellow);
        }

        public void AddLevelUpNotificationText(string playerName, int newLevel)
        {
            _textLines.AddLine(string.Format("{0} reached level {1}", playerName, newLevel), Colours.Orange);
        }

        public void Focus()
        {
            _inputBox.Visible = true;
            _inputBox.Focus();
        }
    }
}
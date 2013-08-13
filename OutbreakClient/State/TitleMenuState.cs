using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Outbreak.Audio;
using Outbreak.Client.Audio;
using Outbreak.Client.Net;
using Outbreak.Resources;
using Psy.Core.Configuration;
using Psy.Core.Logging;
using Psy.Gui.Components;
using Psy.Gui.Events;

namespace Outbreak.Client.State
{
    public class TitleMenuState : MenuState
    {
        private IList<Host> _hosts;
        private ServerBrowser _serverBrowser;
        private const string PlayButtonName = "playButton";
        private const string OptionsButtonName = "optionsButton";

        private const string JoinServerCancelButtonName = "cancelJoinButton";
        private const string DirectConnectButtonName = "directConnectButton";
        private const string DirectConnectHostnameField = "hostname";
        private const string DirectConnectPortField = "port";
        private const string ErrorMessageField = "joinErrorLabel";
        private const string ServerListboxName = "servers";
        private const string PlayerNameTextboxName = "playerName";
        private const string PlayLocallyButtonName = "playLocally";

        public override void OnTransitionOut()
        {
            UnloadUI();
        }

        public override void OnTransitionIn()
        {
            _serverBrowser = new ServerBrowser();

            Logger.Write("Entering TitleMenuState");
            Engine.ShowSplashImage(Textures.MenuBackground);
            Engine.Gui.Desktop.Visible = true;
            Engine.Gui.Desktop.Transparent = true;

            LoadUI();
        }

        private void UnloadUI()
        {
            Engine.Gui.Clear();
        }

        private void LoadUI()
        {
            Engine.Gui.Clear();
            Engine.GuiLoader.Load("titleScreen.xml", Engine.Gui.Desktop);

            var playButton = Engine.Gui.GetWidgetByName(PlayButtonName);
            var optionsButton = Engine.Gui.GetWidgetByName(OptionsButtonName);

            playButton.Click += PlayButtonOnClick;
            optionsButton.Click += OptionsButtonOnClick;
        }

        private void PlayButtonOnClick(object sender, ClickEventArgs args)
        {
            PlayInterfaceInteractionSound();
            Engine.Gui.Clear();
            Engine.GuiLoader.Load("joinServer.xml", Engine.Gui.Desktop);

            // todo: move this default port into a configuration or something.
            Engine.Gui.GetWidgetByName<Textbox>(DirectConnectPortField).Value = "9103";

            var playerNameTextbox = Engine.Gui.GetWidgetByName<Textbox>(PlayerNameTextboxName);
            playerNameTextbox.Value = StaticConfigurationManager.ConfigurationManager.GetString("PlayerName");

            var cancelJoinButton = Engine.Gui.GetWidgetByName(JoinServerCancelButtonName);
            cancelJoinButton.Click += (o, eventArgs) => LoadUI();

            var directConnectButton = Engine.Gui.GetWidgetByName(DirectConnectButtonName);
            directConnectButton.Click += (o, eventArgs) => ConnectToServer();

            var localPlayButton = Engine.Gui.GetWidgetByName(PlayLocallyButtonName);
            localPlayButton.Click += (o, eventArgs) => CreateServerAndPlayLocally();

            var listbox = Engine.Gui.GetWidgetByName<Listbox>(ServerListboxName);

            listbox.RowSelected += ListboxOnRowSelected;
            listbox.DoubleClick += (o, eventArgs) => ConnectToServer();

            _hosts = _serverBrowser.GetHosts().ToList();

            listbox.Populate(
                _hosts.Select(x => 
                        new List<RowData> { new RowData { Data = x.Hostname, Header = "Server Name" } }));
        }

        private void CreateServerAndPlayLocally()
        {
            var server = new ProcessStartInfo("Vortex.Server.exe")
            {
                WindowStyle = ProcessWindowStyle.Minimized
            };
            Process.Start(server);
            
            Engine.Gui.GetWidgetByName<Textbox>(DirectConnectHostnameField).Value = "localhost";
            Engine.Gui.GetWidgetByName<Textbox>(DirectConnectPortField).Value = "9103";
            ConnectToServer();
        }

        private void ListboxOnRowSelected(Listbox listbox, int rowNumber)
        {
            Engine.Gui.GetWidgetByName<Textbox>(DirectConnectHostnameField).Value = _hosts[rowNumber].Hostname;
            Engine.Gui.GetWidgetByName<Textbox>(DirectConnectPortField).Value = _hosts[rowNumber].Port.ToString(CultureInfo.InvariantCulture);
        }

        private void ConnectToServer()
        {
            var host = Engine.Gui.GetWidgetByName<Textbox>(DirectConnectHostnameField).Value;

            if (host == "")
                return;

            int port;
            if (!int.TryParse(Engine.Gui.GetWidgetByName<Textbox>(DirectConnectPortField).Value, out port))
                return;

            var playerName = PlayerName;

            if (playerName == "")
                return;

            StaticConfigurationManager.ConfigurationManager.SetString("PlayerName", PlayerName);

            try
            {
                Context.GameClient.PlayerName = playerName;
                Engine.ConsoleText(string.Format("Connecting to {0}:{1}", host, port));
                Engine.ConnectToServer(host, port);
            }
            catch(Exception e)
            {
                Engine.Gui.GetWidgetByName<Label>(ErrorMessageField).Text = "Error: " + e.Message;
                return;
            }

            Engine.HideSplashImage();
            Engine.Gui.Desktop.Visible = false;
            Engine.Gui.Desktop.Transparent = false; 
            Context.GameClient.StateMachine.TransitionTo("ingame");
        }

        private string PlayerName
        {
            get { return Engine.Gui.GetWidgetByName<Textbox>(PlayerNameTextboxName).Value; }
        }

        private void OptionsButtonOnClick(object sender, ClickEventArgs args)
        {
            Engine.GuiLoader.Load("options.xml", Engine.Gui.Desktop);
            Engine.Gui.GetWidgetByName("closeButton").Click += CloseOptionWindowOnClick;

            BindMasterVolumeSlider();
            BindVolumeSlider("uiVolume", AudioChannel.Interface);
            BindVolumeSlider("musicVolume", AudioChannel.Music);

            PlayInterfaceInteractionSound();
        }

        private void BindVolumeSlider(string sliderName, AudioChannel audioChannel)
        {
            var volumeSlider = Engine.Gui.GetWidgetByName<Slider>(sliderName);
            var volume = AudioChannels.VolumeLevelFromConfiguration(audioChannel);

            volumeSlider.Value = (int)(volume * 100.0f);
            volumeSlider.Change += o =>
            {
                var value = ((Slider) o).Value / 100.0f;
                Engine.AudioEngine.GetChannel((int) audioChannel, false).ChannelVolume = value;
                var channelName = AudioChannels.ChannelLevelConfigurationName(audioChannel);
                StaticConfigurationManager.ConfigurationManager.SetFloat(channelName, value);
            };
        }

        private void BindMasterVolumeSlider()
        {
            var volume = AudioChannels.VolumeLevelFromConfiguration(AudioChannel.Master);

            var masterVolumeSlider = Engine.Gui.GetWidgetByName<Slider>("masterVolume");
            masterVolumeSlider.Value = (int)(volume * 100.0f);
            masterVolumeSlider.Change += o =>
            {
                var value = ((Slider) o).Value / 100.0f;
                Engine.AudioEngine.MasterVolume = value;
                var channelName = AudioChannels.ChannelLevelConfigurationName(AudioChannel.Master);
                StaticConfigurationManager.ConfigurationManager.SetFloat(channelName, value);
            };
        }

        private void CloseOptionWindowOnClick(object sender, ClickEventArgs args)
        {
            Engine.Gui.DeleteWidgetByName("optionsWindow");
            PlayInterfaceInteractionSound();
        }

        private void PlayInterfaceInteractionSound()
        {
            Engine.AudioEngine.Play(Sound.UIClickSoundName, (int)AudioChannel.Interface);
        }
    }
}
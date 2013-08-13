using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using EpicEdit.UI.Widgets;
using Outbreak.Client.Gui.Widgets;
using Psy.Core;
using Psy.Core.FileSystem;
using Psy.Graphics.Text;
using SlimMath;
using Vortex.Renderer;
using Psy.Core.Console;
using Psy.Core.Input;
using Psy.Core.Logging;
using Psy.Core.Tasks;
using Psy.Gui;
using Psy.Gui.ColourScheme;
using Psy.Gui.Loader;
using Psy.Gui.Renderer;
using Psy.Windows;
using Keys = System.Windows.Forms.Keys;

namespace GuiSandbox
{
    class EditorWindow : Window
    {
        public EditorWindow(WindowAttributes windowAttributes) : base(windowAttributes)
        {
        }

        private GuiManager _gui;
        private ConsoleRenderer _consoleRenderer;
        private GuiRenderer _guiRenderer;
        private XmlLoader _guiLoader;
        private Vector2 _mousePosition;
        private IFont _font;
        private string _workingDirectory;
        private string _filename;

        protected override void OnInitialize()
        {
            _gui = new GuiManager(GraphicsContext.WindowSize)
                       {
                           Desktop = {Transparent = true}
                       };

            _font = GraphicsContext.GetFont();

            _guiLoader = new XmlLoader(_gui);

            InventorySlot.Register(_guiLoader);

            _guiRenderer = new GuiRenderer(GraphicsContext, new Faceless());
            _consoleRenderer = new ConsoleRenderer(GraphicsContext, StaticConsole.Console) { Visible = false };

            UVMapper.Register(_guiLoader);

            StaticTaskQueue.TaskQueue.CreateRepeatingTask("GUI Update", _gui.Update, 33);

            StaticConsole.Console.CommandBindings.Bind("load", "Load a GUI layout", LoadLayoutCommandHandler);
            StaticConsole.Console.CommandBindings.Bind("clear", "Clear the GUI", ClearLayoutCommandHandler);
            StaticConsole.Console.CommandBindings.Bind("loadw", "Load a GUI layout using file open dialog", LoadLayoutFileDialogCommandHandler);
            StaticConsole.Console.CommandBindings.Bind("ed", "Opens GUI xml in editor", OpenXmlForEditing);

            Logger.Add(new ConsoleLogger());

            base.OnInitialize();
        }

        private void OpenXmlForEditing(string[] parameters)
        {
            if (string.IsNullOrEmpty(_filename))
            {
                StaticConsole.Console.AddLine("No file open", Colours.Red);
                return;
            }

            if (File.Exists(@"e:\Program Files (x86)\Notepad++\Notepad++.exe"))
            {
                Process.Start(@"e:\Program Files (x86)\Notepad++\Notepad++.exe", _filename);
            }
            else if (File.Exists(@"c:\Program Files (x86)\Notepad++\Notepad++.exe"))
            {
                Process.Start(@"c:\Program Files (x86)\Notepad++\Notepad++.exe", _filename);
            }
            else
            {
                Process.Start(@"notepad.exe", _filename);    
            }
        }

        private void LoadLayoutFileDialogCommandHandler(string[] parameters)
        {
            var dialog = new OpenFileDialog();

            if (string.IsNullOrEmpty(_workingDirectory))
            {
                _workingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            }

            dialog.InitialDirectory = _workingDirectory;
            
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
                return;

            _workingDirectory = Path.GetDirectoryName(dialog.FileName);

            LoadGuiXml(dialog.FileName);
        }

        private void ClearLayoutCommandHandler(string[] parameters)
        {
            _gui.Desktop.RemoveAllChildren();
        }

        private void LoadLayoutCommandHandler(params string[] parameters)
        {
            if (parameters.Length < 2)
                return;

            var filename = parameters[1];

            LoadGuiXml(Lookup.GetAssetPath(filename));
        }

        private void LoadGuiXml(string filename)
        {
            try
            {
                _gui.Clear();
                _guiLoader.Load(filename, _gui.Desktop);
                _filename = filename;
            }
            catch (Exception e)
            {
                StaticConsole.Console.AddLine(string.Format("Failed to load `{0}`", filename));
                StaticConsole.Console.AddException(e);
            }
        }

        private void LoadTest2()
        {
            _guiLoader.Load(@"animationWindow.xml", _gui.Desktop);
        }

        protected override void OnRender()
        {
            base.OnRender();
            AlphaBlending(true);

            _guiRenderer.Render(_gui);
            _consoleRenderer.Render();

            if (_gui.HoverWidget != null)
            {
                var widgetPosition = new Vector2(_mousePosition.X, _mousePosition.Y) - _gui.HoverWidget.GetAbsolutePosition();
                var widgetName = _gui.HoverWidget.UniqueName;

                _font.DrawString("Screen: " + _mousePosition + "  Widget[" + widgetName + "]:" + widgetPosition, 0, 0, Colours.Yellow);
            }
            else
            {
                _font.DrawString("Screen: " + _mousePosition, 0, 0, Colours.Yellow);
            }


            
            
        }

        protected override void OnMouseDown(object sender, MouseEventArgs e)
        {
            base.OnMouseDown(sender, e);
            _gui.HandleMouseDown(new Vector2(e.X, e.Y), MouseButton.Left);
        }

        protected override void OnMouseUp(object sender, MouseEventArgs e)
        {
            base.OnMouseUp(sender, e);
            _gui.HandleMouseUp(new Vector2(e.X, e.Y), MouseButton.Left);
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(sender, e);
            _gui.HandleMouseMove(new Vector2(e.X, e.Y));

            _mousePosition = new Vector2(e.X, e.Y);
        }

        protected override void OnKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(sender, e);

            if (_consoleRenderer.Visible)
            {
                var parsedKey = InputParser.KeyPress(e.KeyCode);
                StaticConsole.Console.OnKeyDown(parsedKey);
                return;
            }
                
            if (e.KeyCode == Keys.F1)
            {
                _gui.Clear();
            }
            if (e.KeyCode == Keys.F5)
            {
                _gui.Clear();
                _guiLoader.Reload(_gui.Desktop);
            }
        }

        protected override void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(sender, e);

            if (e.KeyChar == '`')
            {
                _consoleRenderer.Visible = !_consoleRenderer.Visible;
                return;
            }

            if (_consoleRenderer.Visible)
            {
                StaticConsole.Console.OnKeyPress(new KeyPressEventArguments(e));
                return;
            }

            _gui.HandleKeyText(e.KeyChar);
        }
    }
}

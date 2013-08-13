using System.Drawing;
using System.Windows.Forms;
using EpicEdit.UI;
using EpicEdit.UI.Widgets;
using Psy.Core;
using Psy.Core.Console;
using Psy.Core.Input;
using Psy.Core.Logging;
using Psy.Core.Tasks;
using Psy.Graphics;
using Psy.Graphics.Effects;
using Psy.Graphics.Models.Renderers;
using Psy.Gui;
using Psy.Gui.ColourScheme;
using Psy.Gui.Loader;
using Psy.Gui.Renderer;
using Psy.Windows;
using SlimMath;
using Vortex.Renderer;
using Vortex.Renderer.Camera;

namespace EpicEdit
{
    public class EditorWindow : Window
    {
        private IEffect _effect;
        private Matrix _projection;
        private BasicCamera _camera;
        private ConsoleRenderer _consoleRenderer;
        private readonly Editor _editor;
        private readonly ConsoleCommands _consoleCommands;
        private EpicModelRenderer _modelRenderer;
        private Matrix _viewMat;
        private CoordinateMarkerRenderer _coordinateMarkerRenderer;
        private IEffectHandle _techniqueHandle;
        private Ray _ray;
        private GuiManager _guiManager;
        private GuiRenderer _guiRenderer;
        private XmlLoader _guiLoader;
        public ModelPartWindow ModelPartWindow { get; private set; }
        public ModelPartFaceWindow ModelPartFaceWindow { get; private set; }
        public ButtonPanelWindow ButtonPanelWindow { get; private set; }
        public MaterialsWindow MaterialsWindow { get; private set; }
        public AnchorWindow AnchorWindow { get; private set; }
        public AnimationWindow AnimationWindow { get; private set; }

        public EditorWindow(WindowAttributes windowAttributes)
            : base(windowAttributes)
        {
            Logger.Write("EpicEditor Loaded", LoggerLevel.Info);
            _editor = new Editor(this);
            _consoleCommands = new ConsoleCommands(_editor);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            InitializeUI();

            StaticTaskQueue.TaskQueue.CreateRepeatingTask("AccelRotate", ModelRotate, 15);
            StaticTaskQueue.TaskQueue.CreateRepeatingTask("Animate", ModelAnimate, 1000 / 30);

            _consoleCommands.Register(StaticConsole.Console);

            _camera = new FuncCamera(GraphicsContext, () => _editor.CameraPosition.AsVector()) {ZoomDistance = 4};

            _effect = GraphicsContext.CreateEffect("defaultAmbient.fx");
            _techniqueHandle = _effect.CreateHandle("TVertexAndPixelShader");

            _consoleRenderer = new ConsoleRenderer(GraphicsContext, StaticConsole.Console) { Visible = false };
            _modelRenderer = new EpicModelRenderer(GraphicsContext);
            _modelRenderer.SetMaterials(_editor.Materials);

            _coordinateMarkerRenderer = new CoordinateMarkerRenderer(GraphicsContext);

            GraphicsContext.ClearColour = Color.CornflowerBlue.ToColor4();
            GraphicsContext.ZBufferEnabled = true;
        }

        private void ModelAnimate()
        {
            _editor.ModelAnimate();
        }

        private void InitializeUI()
        {
            _guiManager = new GuiManager(GraphicsContext.WindowSize);
            _guiRenderer = new GuiRenderer(GraphicsContext, new Faceless());
            _guiLoader = new XmlLoader(_guiManager);
            UVMapper.Register(_guiLoader);
            MatImage.Register(_guiLoader);

            ModelPartWindow = new ModelPartWindow(_guiLoader.Load("modelEditorSelectedModelPart.xml", _guiManager.Desktop), _editor);
            ModelPartFaceWindow = new ModelPartFaceWindow(_guiLoader.Load("modelEditorSelectedModelPartFace.xml", _guiManager.Desktop), _editor);
            ButtonPanelWindow = new ButtonPanelWindow(_guiLoader.Load("epicEditButtons.xml", _guiManager.Desktop), _editor);
            MaterialsWindow = new MaterialsWindow(_guiLoader.Load("materialsWindow.xml", _guiManager.Desktop), _editor);
            AnchorWindow = new AnchorWindow(_guiLoader.Load("modelEditorSelectedAnchor.xml", _guiManager.Desktop), _editor);
            AnimationWindow = new AnimationWindow(_guiLoader.Load("animationWindow.xml", _guiManager.Desktop), _editor);

            _editor.EditMode = EditMode.Geometry;

            _guiManager.Desktop.Transparent = true;
            StaticTaskQueue.TaskQueue.CreateRepeatingTask("UpdateUI", _guiManager.Update, 20);
        }

        private void ModelRotate()
        {
            _editor.Update();
        }

        protected override void OnMouseMove(object sender, MouseEventArgs args)
        {
            base.OnMouseMove(sender, args);

            if (!_guiManager.HandleMouseMove(new Vector2(args.X, args.Y)))
            {
                _editor.OnMouseMove(args);
            }
        }

        protected override void OnMouseDown(object sender, MouseEventArgs e)
        {
            base.OnMouseDown(sender, e);

            if (!_guiManager.HandleMouseDown(new Vector2(e.X, e.Y), MouseEventUtility.TranslateMouseButton(e)))
            {
                _guiManager.SetFocus(null);
                _editor.OnMouseDown(e);
            }
        }

        protected override void OnMouseWheel(object sender, MouseEventArgs e)
        {
            base.OnMouseWheel(sender, e);
            _camera.ZoomDistance -= e.Delta/(120.0f*2);
        }

        protected override void OnMouseUp(object sender, MouseEventArgs e)
        {
            base.OnMouseUp(sender, e);

            if (!_guiManager.HandleMouseUp(new Vector2(e.X, e.Y), MouseEventUtility.TranslateMouseButton(e)))
            {
                _editor.OnMouseUp();
            }
        }

        protected override void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '`')
            {
                _consoleRenderer.Visible = !_consoleRenderer.Visible;
            }
            else
            {
                if (_consoleRenderer.Visible)
                {
                    var args = new KeyPressEventArguments(e);
                    StaticConsole.Console.OnKeyPress(args);
                }
                else if (_guiManager.HandleKeyText(e.KeyChar))
                {
                    base.OnKeyPress(sender, e);
                }
            }
        }

        protected override void OnKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(sender, e);

            var key = InputParser.KeyPress(e.KeyCode);
            if (_consoleRenderer.Visible)
            {
                StaticConsole.Console.OnKeyDown(key);
            }
            else
            {
                _editor.OnKeyDown(key);
            }
        }

        protected override void OnKeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyUp(sender, e);
            if (_consoleRenderer.Visible)
                return;

            var key = InputParser.KeyPress(e.KeyCode);
            _editor.OnKeyUp(key);
        }

        protected override void OnRender()
        {
            base.OnRender();

            AlphaBlending(true);

            SetTransforms();
            Render();
        }

        private void Render()
        {
            GraphicsContext.CullMode = CullMode.None;
            GraphicsContext.Clear(1.0f);

            SetFilterMode();

            _modelRenderer.SetEffect(_effect);
            _modelRenderer.SelectedAnchor = _editor.FocusAnchor;

            _effect.Technique = _techniqueHandle;
            _effect.Begin();

            _effect.BeginPass(0);

            _effect.SetMatrix("worldMat", Matrix.Identity);
            var wvpm = _viewMat * _projection;
            _effect.SetMatrix("worldViewProjMat", wvpm);

            _effect.CommitChanges();

            _ray = BasicCamera.ScreenToWorldRay(GraphicsContext, MousePosition.X, MousePosition.Y);

            RenderModel();

            _effect.SetMatrix("worldMat", Matrix.Identity);
            _effect.SetMatrix("worldViewProjMat", wvpm);
            _effect.CommitChanges();
            GraphicsContext.World = Matrix.Identity;
            GraphicsContext.View = _viewMat;
            GraphicsContext.Projection = _projection;

            _modelRenderer.Wireframe = false;
            _coordinateMarkerRenderer.Render(_effect);
            _modelRenderer.FloorPlane();

            _effect.EndPass();

            _effect.End();

            _guiRenderer.Render(_guiManager);

            _consoleRenderer.Render();
        }

        private void SetFilterMode()
        {
            if (_editor.LoFiTextureMode)
            {
                GraphicsContext.MagFilter = TextureFilter.Point;
                GraphicsContext.MinFilter = TextureFilter.Linear;
                GraphicsContext.MipFilter = TextureFilter.Linear;
            }
            else
            {
                GraphicsContext.MagFilter = TextureFilter.Linear;
                GraphicsContext.MinFilter = TextureFilter.Linear;
                GraphicsContext.MipFilter = TextureFilter.Linear;                
            }

        }

        private void SetTransforms()
        {
            _projection = MatrixUtils.GetPerspectiveFovLH(GraphicsContext);
            _viewMat = GetViewMatrix();

            GraphicsContext.Projection = _projection;
            GraphicsContext.View = _viewMat;
            GraphicsContext.World = Matrix.Identity;

            _modelRenderer.ResetWorldMatrix();
            _modelRenderer.SetMatrices(_viewMat, _projection);
        }

        private Matrix GetViewMatrix()
        {
            return GetCameraMatrix() * _camera.GetViewTransform() * Matrix.Translation(0, 0.2f, 0);
        }

        private void RenderModel()
        {
            _modelRenderer.Wireframe = _editor.WireframeMode || _editor.EditMode != EditMode.Geometry;
            _modelRenderer.ResetWorldMatrix();
            _modelRenderer.SetMatrices(_viewMat, _projection);

            _modelRenderer.RenderAnchors = _editor.EditMode == EditMode.Anchors;

            _modelRenderer.Render(_editor.Model);
        }

        private Matrix GetCameraMatrix()
        {
            var q = Quaternion.RotationYawPitchRoll(0, _editor.CameraRotation.Y, -_editor.CameraRotation.X * 1.5f);
            return Matrix.RotationQuaternion(q);
        }

        public Ray GetCameraWorldRay()
        {
            return _ray;
        }

        public void ErrorMessage(string message)
        {
            System.Windows.Forms.MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
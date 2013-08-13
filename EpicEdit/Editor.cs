using System;
using System.IO;
using System.Windows.Forms;
using EpicEdit.Model.Factories;
using Psy.Core;
using Psy.Core.EpicModel;
using Psy.Core.EpicModel.Serialization;
using Psy.Core.FileSystem;
using Psy.Core.Input;
using SlimMath;
using Plane = EpicEdit.Model.Factories.Plane;

namespace EpicEdit
{
    public class Editor
    {
        private readonly EditorWindow _editorWindow;
        public EpicModel Model;
        public bool WireframeMode;
        public readonly MaterialCache Materials;
        private bool Spinning { get; set; }
        public readonly EasedVector CameraRotation;
        public readonly EasedVector CameraPosition;
        private Vector3 _mouseSpinStart;
        private Operation Operation { get; set; }
        private Vector3 OperationOriginalPosition { get; set; }
        private Vector3 OperationOriginalSize { get; set; }
        private Vector3 LastMousePosition { get; set; }
        private Vector3 OperationOriginalRotation { get; set; }
        private Axis AxisMode { get; set; }
        public bool LoFiTextureMode { get; set; }
        public int FaceIndex { get; set; }

        public int SelectedKeyFrameIndex { get; private set; }

        public AnimationType SelectedAnimationType
        {
            get { return _selectedAnimationType; }
            set { 
                if (_selectedAnimationType != value)
                {
                    _selectedAnimationType = value;
                    if (_editorWindow.AnimationWindow != null)
                    {
                        _editorWindow.AnimationWindow.Refresh();
                    }
                }
                else
                {
                    _selectedAnimationType = value;    
                }
            }
        }

        private EditMode _editMode;
        public EditMode EditMode
        {
            get { return _editMode; }
            set 
            { 
                _editMode = value;
                _editorWindow.ButtonPanelWindow.SetEditMode(value);
            }
        }

        public Anchor FocusAnchor { get; private set; }
        public ModelPart FocusModelPart { get; set; }
        public SelectMode SelectMode { get; set; }

        public bool AnimationMode
        {
            get { return _animationMode; }
            set { _animationMode = value;
                _editorWindow.AnimationWindow.Visible = value;
            }
        }

        public bool AnimationPlaying
        {
            get { return _animationPlaying; }
            set { _animationPlaying = value; }
        }

        private Vector3 _modelOriginalSpin;
        private bool _anchorJoinMode;
        private string _workingDirectory;
        private bool _animationMode;
        private AnimationType _selectedAnimationType;
        private bool _animationPlaying;
        private Vector2[] _copyFaceCoordinates;
        public float AnimationTime { get; private set; }

        public Editor(EditorWindow editorWindow)
        {
            _editorWindow = editorWindow;
            
            Materials = new MaterialCache();

            CameraRotation = new EasedVector();
            CameraPosition = new EasedVector();
            LoadDefaultModel();

            SelectedAnimationType = AnimationType.Standing;
        }

        private void SpinModel(float x, float y)
        {
            CameraRotation.X = _modelOriginalSpin.X + x;
            CameraRotation.Y = _modelOriginalSpin.Y + y;
        }

        private void ToggleWireframeMode()
        {
            WireframeMode = !WireframeMode;
        }

        private void LoadDefaultModel()
        {
            Model = new EpicModel("workspace_model");
        }

        public void OnKeyUp(Key key)
        {
            switch (key)
            {
                case Key.M:
                    EndModelOperation();
                    break;
                case Key.S:
                    EndModelOperation();
                    break;
                case Key.R:
                    EndModelOperation();
                    break;
            }
        }

        private void EndModelOperation()
        {
            Operation = Operation.None;
        }

        private void BeginMoveOperation()
        {
            if (Operation != Operation.None)
                return;

            switch (EditMode)
            {
                case EditMode.Geometry:
                    if (FocusModelPart == null)
                        return;

                    Operation = Operation.Move;
                    OperationOriginalPosition = FocusModelPart.Position;
                    break;

                case EditMode.Anchors:
                    if (FocusAnchor == null)
                        return;

                    Operation = Operation.Move;
                    OperationOriginalPosition = FocusAnchor.Position;
                    break;
            }
        }

        private void BeginResizeOperation()
        {
            if (FocusModelPart == null)
                return;

            if (Operation != Operation.None)
                return;

            Operation = Operation.Resize;
            OperationOriginalSize = FocusModelPart.Size;
        }

        private void BeginRotateOperation()
        {
            if (FocusModelPart == null)
                return;

            if (Operation != Operation.None)
                return;

            Operation = Operation.Rotate;
            OperationOriginalRotation = FocusModelPart.Rotation;
        }

        public void OnKeyDown(Key key)
        {
            switch (key)
            {
                case Key.F12:
                    ToggleWireframeMode();
                    break;
                case Key.Up:
                    CameraPosition.Y += 0.2f;
                    break;
                case Key.Down:
                    CameraPosition.Y -= 0.2f;
                    break;
                case Key.Left:
                    CameraPosition.X -= 0.2f;
                    break;
                case Key.Right:
                    CameraPosition.X += 0.2f;
                    break;
                case Key.M:
                    BeginMoveOperation();
                    break;
                case Key.S:
                    BeginResizeOperation();
                    break;
                case Key.R:
                    BeginRotateOperation();
                    break;
                case Key.P:
                    TogglePixallatedMode();
                    break;
                case Key.F:
                    SetSelectMode(SelectMode.Face);
                    break;
                case Key.O:
                    SetSelectMode(SelectMode.Object);
                    break;

                case Key.X:
                case Key.Y:
                case Key.Z:
                    SetAxisMode(key);
                    break;
            }
        }

        private void SetSelectMode(SelectMode selectMode)
        {
            _editorWindow.ButtonPanelWindow.SetSelectMode(selectMode);
            SelectMode = selectMode;
        }

        private void TogglePixallatedMode()
        {
            LoFiTextureMode = !LoFiTextureMode;
            _editorWindow.ButtonPanelWindow.SetLoFiTextureMode(LoFiTextureMode);
        }

        private void SetAxisMode(Key key)
        {
            switch (key)
            {
                case Key.X:
                    SetAxisMode(Axis.X);
                    break;
                case Key.Y:
                    SetAxisMode(Axis.Y);
                    break;                    
                case Key.Z:
                    SetAxisMode(Axis.Z);
                    break;                    
            }
        }

        public void SetAxisMode(Axis axis)
        {
            AxisMode = axis;
            _editorWindow.ButtonPanelWindow.SelectAxis(axis);
        }

        public void Update()
        {
            CameraRotation.Update();
            CameraPosition.Update();
        }

        public void OnMouseMove(MouseEventArgs args)
        {
            switch (Operation)
            {
                case Operation.Move:
                    ApplyMovement(args);
                    break;
                case Operation.Resize:
                    ApplyResize(args);
                    break;
                case Operation.Rotate:
                    ApplyRotation(args);
                    break;
                default:
                    LastMousePosition = new Vector3(args.X, args.Y, 0);
                    if (Spinning)
                    {
                        SpinModel((_mouseSpinStart.X - args.X) / 100.0f, (_mouseSpinStart.Y - args.Y) / 100.0f);
                    }
                    break;
            }
        }

        private Vector3 GetAxisAdjustment(MouseEventArgs args)
        {
            var inPos = args.X + args.Y;
            var lastPos = LastMousePosition.X + LastMousePosition.Y;


            switch (AxisMode)
            {
                case Axis.None:
                    return new Vector3();
                case Axis.X:
                    return new Vector3(inPos, 0, 0) - new Vector3(lastPos, 0, 0);
                case Axis.Y:
                    return new Vector3(0, inPos, 0) - new Vector3(0, lastPos, 0);
                case Axis.Z:
                    return new Vector3(0, 0, inPos) - new Vector3(0, 0, lastPos);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ApplyRotation(MouseEventArgs args)
        {
            if (FocusAnchor != null)
            {
                var diff = GetAxisAdjustment(args);
                var diffVector = new Vector3(diff.X / 1000, -diff.Y / 1000, diff.Z / 1000);

                FocusAnchor.Rotate(diffVector);

                LastMousePosition = new Vector3(args.X, args.Y, 0);

                _editorWindow.AnchorWindow.RefreshUI();
            }
            else if (FocusModelPart != null)
            {
                var diff = GetAxisAdjustment(args);
                var diffVector = new Vector3(diff.X / 1000, -diff.Y / 1000, diff.Z / 1000);

                if (SelectMode == SelectMode.Object)
                {
                    FocusModelPart.Rotation = OperationOriginalRotation + diffVector;
                }
                else if (SelectMode == SelectMode.Face)
                {
                    var face = FocusModelPart.SelectedFace;
                    if (face != null)
                    {
                        face.Rotate(diffVector);
                        // hack: face operation handling doesn't fit perfectly with object handling so the last mouse position has to be reset
                        LastMousePosition = new Vector3(args.X, args.Y, 0);
                    }
                }

                _editorWindow.ModelPartWindow.RefreshUI();
            }
        }

        private void ApplyResize(MouseEventArgs args)
        {
            if (FocusModelPart == null)
                return;

            var diff = GetAxisAdjustment(args);
            var diffVector = new Vector3(diff.X / 1000, -diff.Y / 1000, diff.Z / 1000);

            if (SelectMode == SelectMode.Object)
            {
                FocusModelPart.Size = OperationOriginalSize + diffVector;
            }
            else if (SelectMode == SelectMode.Face)
            {
                var face = FocusModelPart.SelectedFace;
                if (face != null)
                {
                    face.Resize(diffVector);
                    // hack: face operation handling doesn't fit perfectly with object handling so the last mouse position has to be reset
                    LastMousePosition = new Vector3(args.X, args.Y, 0);
                }
            }

            _editorWindow.ModelPartWindow.RefreshUI();
        }

        private void ApplyMovement(MouseEventArgs args)
        {

            var diff = GetAxisAdjustment(args);
            var diffVector = new Vector3(diff.X/1000, -diff.Y/1000, diff.Z/1000);

            switch (EditMode)
            {
                case EditMode.Geometry:
                    switch (SelectMode)
                    {
                        case SelectMode.Object:
                        {
                            if (FocusModelPart == null)
                                return;

                            FocusModelPart.Position = OperationOriginalPosition + diffVector;
                            break;
                        }
                        case SelectMode.Face:
                        {
                            var face = FocusModelPart.SelectedFace;
                            if (face == null)
                                return;
                        
                            face.Translate(diffVector);
                            LastMousePosition = new Vector3(args.X, args.Y, 0);
                        
                            break;
                        }
                    }
                    break;

                case EditMode.Anchors:
                    if (FocusAnchor == null)
                        return;

                    FocusAnchor.Position = FocusAnchor.Position + diffVector;
                    LastMousePosition = new Vector3(args.X, args.Y, 0);
                    _editorWindow.AnchorWindow.RefreshUI();

                    break;
            }

            _editorWindow.ModelPartWindow.RefreshUI();
        }

        public void OnMouseDown(MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                if (!Spinning)
                {
                    Spinning = true;
                    _mouseSpinStart = new Vector3(mouseEventArgs.X, mouseEventArgs.Y, 0);
                    _modelOriginalSpin = CameraRotation.AsVector();
                }
            } 
            else if (mouseEventArgs.Button == MouseButtons.Right)
            {
                Select();
            }
        }

        private void Select()
        {
            switch (EditMode)
            {
                case EditMode.Geometry:
                    switch (SelectMode)
                    {
                        case SelectMode.None:
                            break;
                        case SelectMode.Face:
                            SelectFace();
                            break;
                        case SelectMode.Object:
                            SelectObject();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case EditMode.Anchors:
                    SelectAnchor();
                    break;
            }
        }

        private void SelectAnchor()
        {
            var cameraRay = _editorWindow.GetCameraWorldRay();
            var intersects = Model.PickAnchor(cameraRay);

            if (intersects != PickAnchorResult.Nothing)
            {
                if (_anchorJoinMode)
                {
                    if (intersects.Anchor != FocusAnchor)
                    {
                        JoinAnchors(intersects.Anchor, FocusAnchor);
                        _anchorJoinMode = false;
                    }
                }
                else
                {
                    FocusAnchor = intersects.Anchor;
                    FocusModelPart = intersects.Anchor.ModelPart;
                }
            }
            else
            {
                FocusAnchor = null;
                _anchorJoinMode = false;
            }

            RefreshUI();
        }

        private static void JoinAnchors(Anchor parent, Anchor child)
        {
            child.SetParent(parent);
        }

        private void SelectObject()
        {
            var cameraRay = _editorWindow.GetCameraWorldRay();
            
            var intersects = Model.PickModelPart(cameraRay);

            if (FocusModelPart != null)
            {
                FocusModelPart.ClearSelect();
            }

            if (intersects != PickModelResult.Nothing)
            {
                intersects.ModelPart.ClearSelect();

                FocusModelPart = intersects.ModelPart;
                FocusModelPart.Select();

                _editorWindow.ModelPartWindow.SelectModelPart(intersects.ModelPart);
            }
        }

        private void SelectFace(ModelPart modelPart, int faceIndex)
        {
            if (FocusModelPart != null)
            {
                FocusModelPart.ClearSelect();
            }

            FocusModelPart = modelPart;

            if (modelPart != null)
            {
                modelPart.SelectFace(faceIndex);

                FaceIndex = faceIndex;
                _editorWindow.ModelPartFaceWindow.SelectModelPartFace();
                _editorWindow.ModelPartWindow.SelectModelPart(modelPart);
            }
        }

        private void SelectFace()
        {
            var cameraRay = _editorWindow.GetCameraWorldRay();
            var intersects = Model.PickModelPart(cameraRay);

            if (intersects != PickModelResult.Nothing)
            {
                SelectFace(intersects.ModelPart, intersects.FaceIndex);    
            }
        }

        public void OnMouseUp()
        {
            Spinning = false;
        }

        public void CloneModelPart(ModelPart modelPart)
        {
            var newModelPart = modelPart.Clone();
            Model.ModelParts.Add(newModelPart);
        }

        public void DeleteModelPart(ModelPart modelPart)
        {
            Model.ModelParts.Remove(modelPart);
            Select();
        }

        public void RefreshUI()
        {
            if (FocusModelPart != null)
            {
                _editorWindow.ModelPartWindow.SelectModelPart(FocusModelPart);
            }

            _editorWindow.ModelPartFaceWindow.RefreshUI();
            _editorWindow.AnchorWindow.RefreshUI();
        }

        public void LoadModel()
        {
            var dialog = new OpenFileDialog {Filter = "Epic Model (*.emd)|*.emd"};

            if (string.IsNullOrEmpty(_workingDirectory))
            {
                _workingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            }

            dialog.InitialDirectory = _workingDirectory;

            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
                return;

            _workingDirectory = Path.GetDirectoryName(dialog.FileName);

            try
            {
                LoadModelFromFile(dialog.FileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error loading model", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadModelFromFile(string filename)
        {
            Lookup.AddPath(Path.GetDirectoryName(filename));

            using (var reader = new BinaryReader(new FileStream(filename, FileMode.Open)))
            {
                var materialTranslator = new MaterialTranslator(this);
                var epicModelReader = new EpicModelReader(materialTranslator, reader);

                var model = epicModelReader.Read(filename);

                Model = model;
            }
        }

        public void SaveModel()
        {
            var dialog = new SaveFileDialog {Filter = "Epic Model (*.emd)|*.emd"};

            if (string.IsNullOrEmpty(_workingDirectory))
            {
                _workingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            }

            dialog.InitialDirectory = _workingDirectory;

            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
                return;

            _workingDirectory = Path.GetDirectoryName(dialog.FileName);

            SaveModelToFile(dialog.FileName);
        }

        private void SaveModelToFile(string filename)
        {
            FixKeyframes();

            using (var writer = new BinaryWriter(new FileStream(filename, FileMode.Create)))
            {
                var materialTranslator = new MaterialTranslator(this);
                var epicModelWriter = new EpicModelWriter(materialTranslator);
                epicModelWriter.Write(writer, Model);
            }
        }

        private void FixKeyframes()
        {
            foreach (var animation in Model.Animations)
            {
                animation.MoveKeyFrame(0, 0);
            }
        }

        public void AddCubeToModel()
        {
            var cuboid = Cuboid.CreateCuboid("cube", 1);
            Model.ModelParts.Add(cuboid);
        }

        public void AddPlaneToModel()
        {
            var plane = Plane.CreatePlane("plane", 1);
            Model.ModelParts.Add(plane);
        }

        public void NewModel()
        {
            var result = MessageBox.Show("Create new?", "Create new", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
                return;

            LoadDefaultModel();
            Select();
        }

        public void ChooseMaterial()
        {
            _editorWindow.MaterialsWindow.Show(SelectMaterial);
        }

        private void SelectMaterial(string materialName)
        {
            if (FocusModelPart == null)
            {
                return;
            }

            var material = Materials.GetByName(materialName);

            if (material == null)
            {
                return;
            }

            FocusModelPart.MaterialId = material.Id;

            _editorWindow.ModelPartWindow.RefreshUI();
            _editorWindow.ModelPartFaceWindow.RefreshUI();
            
        }

        public void DeleteObject()
        {
            Model.RemoveModelPart(FocusModelPart);
        }

        public void CloneObject()
        {
            if (FocusModelPart == null)
                return;
            var clone = FocusModelPart.Clone();
            Model.ModelParts.Add(clone);
        }

        public void CreateAnchor()
        {
            if (FocusModelPart == null)
            {
                return;
            }

            FocusModelPart.AddAnchor();
        }

        public void DeleteSelectedAnchor()
        {
            if (FocusAnchor == null)
            {
                return;
            }

            FocusAnchor.ModelPart.RemoveAnchor(FocusAnchor);

            FocusAnchor = null;
            RefreshUI();
        }

        public void BeginAnchorJoin()
        {
            _anchorJoinMode = true;
        }

        public void CreateAnimationFrame()
        {
            var animation = Model.GetAnimation(SelectedAnimationType, autoCreate:true);
            animation.AddFrame();
            _editorWindow.AnimationWindow.Refresh();
        }

        public void Error(string message)
        {
            _editorWindow.ErrorMessage(message);
        }

        /// <summary>
        /// Apply keyframe to the scene
        /// </summary>
        /// <param name="index"></param>
        public void ApplyKeyFrame(int index)
        {
            var animation = Model.GetAnimation(SelectedAnimationType);
            if (animation == null)
                return;

            animation.ApplyKeyFrame(index);
        }

        public void SelectKeyFrame(int index)
        {
            SelectedKeyFrameIndex = index;
            _editorWindow.AnimationWindow.Refresh();
        }

        public void MoveKeyFrame(int index, float position)
        {
            var animation = Model.GetAnimation(SelectedAnimationType);
            if (animation == null)
                return;

            animation.MoveKeyFrame(index, position);
            _editorWindow.AnimationWindow.Refresh();
        }

        public void DeleteKeyFrame(int index)
        {
            var animation = Model.GetAnimation(SelectedAnimationType);
            if (animation == null)
                return;

            animation.DeleteKeyFrame(index);
            _editorWindow.AnimationWindow.Refresh();
        }

        public void ApplyAnimationAtTime(float f)
        {
            var animation = Model.GetAnimation(SelectedAnimationType);
            if (animation == null)
                return;

            animation.ApplyAtTime(f);
        }

        public void ModelAnimate()
        {
            var animation = Model.GetAnimation(SelectedAnimationType);
            if (animation == null)
                return;

            if (!_animationPlaying)
                return;

            AnimationTime += 0.03f;

            if (AnimationTime > animation.Duration)
            {
                AnimationTime = 0;
            }

            ApplyAnimationAtTime(AnimationTime);

            _editorWindow.AnimationWindow.Refresh();
        }

        public void PasteFaceCoordinates()
        {
            var face = GetCurrentModelPartFace();

            if (_copyFaceCoordinates == null)
            {
                return;
            }

            if (face.TextureCoordinates.Length != _copyFaceCoordinates.Length)
            {
                return;
            }

            _copyFaceCoordinates.CopyTo(face.TextureCoordinates, 0);
        }

        public ModelPartFace GetCurrentModelPartFace()
        {
            return FocusModelPart.Faces[FaceIndex];
        }

        public void CopyFaceCoordinates()
        {
            var src = GetCurrentModelPartFace().TextureCoordinates;
            _copyFaceCoordinates = new Vector2[src.Length];
            src.CopyTo(_copyFaceCoordinates, 0);
        }

        public Animation Animation
        {
            get
            {
                var animation = Model.GetAnimation(SelectedAnimationType, autoCreate: false);
                return animation;
            }
        }

        public void IncludeCurrentModelPartInAnimation(bool value)
        {
            if (Animation == null)
            {
                return;
            }

            if (!value)
            {
                Animation.Ignore(FocusModelPart);
            }
            else
            {
                Animation.Include(FocusModelPart);
            }
        }
    }
}
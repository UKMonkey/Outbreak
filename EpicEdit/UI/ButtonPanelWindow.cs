using System;
using Psy.Gui.Components;
using Psy.Gui.Events;

namespace EpicEdit.UI
{
    public class ButtonPanelWindow
    {
        private const string NewModelButtonName = "newModelButton";
        private const string LoadModelButtonName = "loadModelButton";
        private const string SaveModelButtonName = "saveModelButton";

        private const string AddCubeButtonName = "addCubeButton";
        private const string AddPlaneButtonName = "addPlaneButton";

        private const string SelModeFaceName = "selModeFace";
        private const string SelModeObjectName = "selModeObject";

        private const string AxisXButtonName = "axisX";
        private const string AxisYButtonName = "axisY";
        private const string AxisZButtonName = "axisZ";

        private const string SampleLinearButtonName = "sampleLinear";
        private const string SamplePointButtonName = "samplePoint";

        private const string CloneObjectButtonName = "cloneObjectButton";
        private const string DeleteObjectButtonName = "deleteObjectButton";

        private const string GeometryModeButtonName = "geometryModeButton";
        private const string AnchorModeButtonName = "anchorModeButton";
        private const string CollisionModeButtonName = "collisionModeButton";
        private const string AnimationModeButtonname = "animationModeButton";

        private const string CreateAnchorButtonName = "createAnchorButton";

        private readonly Widget _widget;
        private readonly Editor _editor;
        private Button _newModelButton;
        private Button _loadModelButton;
        private Button _saveModelButton;
        private Button _addCubeButton;
        private ToggleButton _axisXButton;
        private ToggleButton _axisYButton;
        private ToggleButton _axisZButton;
        private ToggleButton _selectModeFace;
        private ToggleButton _selectModeObject;
        private ToggleButton _sampleLinear;
        private ToggleButton _samplePoint;
        private Button _addPlaneButton;
        private Button _cloneObjectButton;
        private Button _deleteObjectButton;
        private ToggleButton _geometryModeButton;
        private ToggleButton _anchorModeButton;
        private ToggleButton _collisionModeButton;
        private Button _createAnchorButton;
        private ToggleButton _animationEditorButton;

        public ButtonPanelWindow(Widget widget, Editor editor)
        {
            _widget = widget;
            _editor = editor;

            HookupWidgets();
        }

        public void SelectAxis(Axis axis)
        {
            switch (axis)
            {
                case Axis.None:
                    break;
                case Axis.X:
                    _axisXButton.Value = true;
                    break;
                case Axis.Y:
                    _axisYButton.Value = true;
                    break;
                case Axis.Z:
                    _axisZButton.Value = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("axis");
            }
        }

        private void HookupWidgets()
        {
            _cloneObjectButton = _widget.FindWidgetByUniqueName<Button>(CloneObjectButtonName);
            _cloneObjectButton.Click += CloneObjectButtonOnClick;

            _deleteObjectButton = _widget.FindWidgetByUniqueName<Button>(DeleteObjectButtonName);
            _deleteObjectButton.Click += DeleteObjectButtonOnClick;

            _newModelButton = _widget.FindWidgetByUniqueName<Button>(NewModelButtonName);
            _newModelButton.Click += NewModelButtonOnClick;

            _loadModelButton = _widget.FindWidgetByUniqueName<Button>(LoadModelButtonName);
            _loadModelButton.Click += LoadModelButtonOnClick;

            _saveModelButton = _widget.FindWidgetByUniqueName<Button>(SaveModelButtonName);
            _saveModelButton.Click += SaveModelButtonOnClick;

            _addCubeButton = _widget.FindWidgetByUniqueName<Button>(AddCubeButtonName);
            _addCubeButton.Click += AddCubeButtonOnClick;

            _addPlaneButton = _widget.FindWidgetByUniqueName<Button>(AddPlaneButtonName);
            _addPlaneButton.Click += AddPlaneButtonOnClick;

            _axisXButton = _widget.FindWidgetByUniqueName<ToggleButton>(AxisXButtonName);
            _axisXButton.Click += AxisXButtonOnClick;

            _axisYButton = _widget.FindWidgetByUniqueName<ToggleButton>(AxisYButtonName);
            _axisYButton.Click += AxisYButtonOnClick;

            _axisZButton = _widget.FindWidgetByUniqueName<ToggleButton>(AxisZButtonName);
            _axisZButton.Click += AxisZButtonOnClick;

            _selectModeFace = _widget.FindWidgetByUniqueName<ToggleButton>(SelModeFaceName);
            _selectModeFace.Toggled += SelectModeFaceOnToggled;

            _selectModeObject = _widget.FindWidgetByUniqueName<ToggleButton>(SelModeObjectName);
            _selectModeObject.Toggled += SelectModeObjectOnToggled;

            _sampleLinear = _widget.FindWidgetByUniqueName<ToggleButton>(SampleLinearButtonName);
            _sampleLinear.Toggled += SampleModeToggled;

            _samplePoint = _widget.FindWidgetByUniqueName<ToggleButton>(SamplePointButtonName);
            _samplePoint.Toggled += SampleModeToggled;

            _geometryModeButton = _widget.FindWidgetByUniqueName<ToggleButton>(GeometryModeButtonName);
            _geometryModeButton.Toggled += EditEditorToggled;

            _anchorModeButton = _widget.FindWidgetByUniqueName<ToggleButton>(AnchorModeButtonName);
            _anchorModeButton.Toggled += EditEditorToggled;

            _collisionModeButton = _widget.FindWidgetByUniqueName<ToggleButton>(CollisionModeButtonName);
            _collisionModeButton.Toggled += EditEditorToggled;

            _animationEditorButton = _widget.FindWidgetByUniqueName<ToggleButton>(AnimationModeButtonname);
            _animationEditorButton.Toggled += AnimationEditorButtonOnToggled;

            _createAnchorButton = _widget.FindWidgetByUniqueName<Button>(CreateAnchorButtonName);
            _createAnchorButton.Click += CreateAnchorButtonOnClick;
        }

        private void AnimationEditorButtonOnToggled(object sender)
        {
            _editor.AnimationMode = _animationEditorButton.Value;
        }

        private void CreateAnchorButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.CreateAnchor();
        }

        private void EditEditorToggled(object sender)
        {
            if (_geometryModeButton.Value)
            {
                _editor.EditMode = EditMode.Geometry;
            }
            else if (_anchorModeButton.Value)
            {
                _editor.EditMode = EditMode.Anchors;
            }
            else if (_collisionModeButton.Value)
            {
                _editor.EditMode = EditMode.Collision;
            }
            else
            {
                _editor.EditMode = EditMode.None;
            }
        }

        private void DeleteObjectButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.DeleteObject();
        }

        private void CloneObjectButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.CloneObject();
        }

        private void AddPlaneButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.AddPlaneToModel();
        }

        private void SampleModeToggled(object sender)
        {
            _editor.LoFiTextureMode = _samplePoint.Value;
        }

        private void SelectModeObjectOnToggled(object sender)
        {
            _editor.SelectMode = _selectModeObject.Value ? SelectMode.Object : SelectMode.None;
        }

        private void SelectModeFaceOnToggled(object sender)
        {
            _editor.SelectMode = _selectModeFace.Value ? SelectMode.Face : SelectMode.None;
        }

        private void AxisZButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.SetAxisMode(Axis.Z);
        }

        private void AxisYButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.SetAxisMode(Axis.Y);
        }

        private void AxisXButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.SetAxisMode(Axis.X);
        }

        private void AddCubeButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.AddCubeToModel();
        }

        private void SaveModelButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.SaveModel();
        }

        private void LoadModelButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.LoadModel();
        }

        private void NewModelButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.NewModel();
        }

        public void SetLoFiTextureMode(bool loFiTextureMode)
        {
            _samplePoint.Value = loFiTextureMode;
            _sampleLinear.Value = !loFiTextureMode;
        }

        public void SetSelectMode(SelectMode selectMode)
        {
            _selectModeFace.Value = selectMode == SelectMode.Face;
            _selectModeObject.Value = selectMode == SelectMode.Object;
        }

        public void SetEditMode(EditMode editMode)
        {
            _geometryModeButton.Value = editMode == EditMode.Geometry;
            _collisionModeButton.Value = editMode == EditMode.Collision;
            _anchorModeButton.Value = editMode == EditMode.Anchors;

            _createAnchorButton.Visible = editMode == EditMode.Anchors;

            _deleteObjectButton.Visible = editMode == EditMode.Geometry;
            _cloneObjectButton.Visible = editMode == EditMode.Geometry;
            _addCubeButton.Visible = editMode == EditMode.Geometry;
            _addPlaneButton.Visible = editMode == EditMode.Geometry;

            _selectModeFace.Visible = editMode == EditMode.Geometry;
            _selectModeObject.Visible = editMode == EditMode.Geometry || editMode == EditMode.Anchors;
        }
    }
}
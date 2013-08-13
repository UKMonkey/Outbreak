using Psy.Core.EpicModel;
using Psy.Gui.Components;
using Psy.Gui.Events;
using SlimMath;
using Psy.Core;

namespace EpicEdit.UI
{
    public class ModelPartWindow
    {
        private const string NameFieldName = "modelPartName";
        private const string PositionXFieldName = "modelPartPositionX";
        private const string PositionYFieldName = "modelPartPositionY";
        private const string PositionZFieldName = "modelPartPositionZ";
        private const string SizeXFieldName = "modelPartSizeX";
        private const string SizeYFieldName = "modelPartSizeY";
        private const string SizeZFieldName = "modelPartSizeZ";
        private const string RotationXFieldName = "modelPartRotationX";
        private const string RotationYFieldName = "modelPartRotationY";
        private const string RotationZFieldName = "modelPartRotationZ";
        private const string MaterialFieldName = "modelPartMaterial";
        private const string DeleteButtonName = "modelPartDelete";
        private const string CloneButtonName = "modelPartClone";
        private const string MaterialSelectButtonName = "materialSelect";
        private const string IncludeInAnimationButtonName = "includeInAnimation";

        private readonly Widget _widget;
        private readonly Editor _editor;
        private Textbox _nameField;
        private Textbox _positionXField;
        private Textbox _positionYField;
        private Textbox _positionZField;
        private Textbox _rotationXField;
        private Textbox _rotationYField;
        private Textbox _rotationZField;
        private Textbox _sizeXField;
        private Textbox _sizeYField;
        private Textbox _sizeZField;
        private Textbox _materialField;
        private Button _cloneButton;
        private Button _deleteButton;
        private Button _materialSelectButton;
        private Checkbox _includeInAnimation;

        protected ModelPart ModelPart { get; set; }

        private bool ModelPartSelected
        {
            get { return ModelPart != null; }
        }

        public ModelPartWindow(Widget widget, Editor editor)
        {
            _widget = widget;
            _editor = editor;
            _widget.Visible = false;

            HookupWidgets();
        }

        private void HookupWidgets()
        {
            _nameField = _widget.FindWidgetByUniqueName<Textbox>(NameFieldName);
            _nameField.Change += NameFieldOnChange;

            /* position fields */

            _positionXField = _widget.FindWidgetByUniqueName<Textbox>(PositionXFieldName);
            _positionXField.Change += PositionXFieldOnChange;

            _positionYField = _widget.FindWidgetByUniqueName<Textbox>(PositionYFieldName);
            _positionYField.Change += PositionYFieldOnChange;

            _positionZField = _widget.FindWidgetByUniqueName<Textbox>(PositionZFieldName);
            _positionZField.Change += PositionZFieldOnChange;

            /* rotation fields */

            _rotationXField = _widget.FindWidgetByUniqueName<Textbox>(RotationXFieldName);
            _rotationXField.Change += RotationXFieldOnChange;

            _rotationYField = _widget.FindWidgetByUniqueName<Textbox>(RotationYFieldName);
            _rotationYField.Change += RotationYFieldOnChange;

            _rotationZField = _widget.FindWidgetByUniqueName<Textbox>(RotationZFieldName);
            _rotationZField.Change += RotationZFieldOnChange;

            /* size fields */

            _sizeXField = _widget.FindWidgetByUniqueName<Textbox>(SizeXFieldName);
            _sizeXField.Change += SizeXFieldOnChange;

            _sizeYField = _widget.FindWidgetByUniqueName<Textbox>(SizeYFieldName);
            _sizeYField.Change += SizeYFieldOnChange;

            _sizeZField = _widget.FindWidgetByUniqueName<Textbox>(SizeZFieldName);
            _sizeZField.Change += SizeZFieldOnChange;

            /* material field */

            _materialField = _widget.FindWidgetByUniqueName<Textbox>(MaterialFieldName);
            _materialField.Change += MaterialFieldOnChange;

            /* command buttons */

            _cloneButton = _widget.FindWidgetByUniqueName<Button>(CloneButtonName);
            _cloneButton.Click += CloneButtonOnClick;

            _deleteButton = _widget.FindWidgetByUniqueName<Button>(DeleteButtonName);
            _deleteButton.Click += DeleteButtonOnClick;

            _materialSelectButton = _widget.FindWidgetByUniqueName<Button>(MaterialSelectButtonName);
            _materialSelectButton.Click += MaterialSelectButtonOnClick;

            _includeInAnimation = _widget.FindWidgetByUniqueName<Checkbox>(IncludeInAnimationButtonName);
            _includeInAnimation.Change += IncludeInAnimationOnChange;
        }

        private void IncludeInAnimationOnChange(object sender)
        {
            _editor.IncludeCurrentModelPartInAnimation(_includeInAnimation.Value);
        }

        private void MaterialSelectButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.ChooseMaterial();
        }

        private void DeleteButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.DeleteModelPart(ModelPart);
        }

        private void CloneButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.CloneModelPart(ModelPart);
        }

        private void MaterialFieldOnChange(object sender)
        {
            var materialName = _materialField.Value;

            var material = _editor.Materials.GetByName(materialName);
            if (material != null)
            {
                _materialField.Value = material.Name;
                ModelPart.MaterialId = material.Id;
                _editor.RefreshUI();
                return;
            }

            // reset the material name, the user got it wrong
            _materialField.Value = GetMaterialName();
        }

        private void SizeZFieldOnChange(object sender)
        {
            float f;
            if (float.TryParse(_sizeZField.Value, out f))
            {
                ModelPart.Size = new Vector3(ModelPart.Size.X, ModelPart.Size.Y, f);
            }
        }

        private void SizeYFieldOnChange(object sender)
        {
            float f;
            if (float.TryParse(_sizeYField.Value, out f))
            {
                ModelPart.Size = new Vector3(ModelPart.Size.X, f, ModelPart.Size.Z);
            }
        }

        private void SizeXFieldOnChange(object sender)
        {
            float f;
            if (float.TryParse(_sizeXField.Value, out f))
            {
                ModelPart.Size = new Vector3(f, ModelPart.Size.Y, ModelPart.Size.Z);
            }
        }

        private void RotationZFieldOnChange(object sender)
        {
            float f;
            if (float.TryParse(_rotationZField.Value, out f))
            {
                ModelPart.Rotation = new Vector3(ModelPart.Rotation.X, ModelPart.Rotation.Y, f.ToRadians());
            }
        }

        private void RotationYFieldOnChange(object sender)
        {
            float f;
            if (float.TryParse(_rotationYField.Value, out f))
            {
                ModelPart.Rotation = new Vector3(ModelPart.Rotation.X, f.ToRadians(), ModelPart.Rotation.Z);
            }
        }

        private void RotationXFieldOnChange(object sender)
        {
            float f;
            if (float.TryParse(_rotationXField.Value, out f))
            {
                ModelPart.Rotation = new Vector3(f.ToRadians(), ModelPart.Rotation.Y, ModelPart.Rotation.Z);
            }
        }

        private void PositionZFieldOnChange(object sender)
        {
            float f;
            if (float.TryParse(_positionZField.Value, out f))
            {
                ModelPart.Position = new Vector3(ModelPart.Position.X, ModelPart.Position.Y, f);
            }
        }

        private void PositionYFieldOnChange(object sender)
        {
            float f;
            if (float.TryParse(_positionYField.Value, out f))
            {
                ModelPart.Position = new Vector3(ModelPart.Position.X, f, ModelPart.Position.Z);
            }
        }

        private void PositionXFieldOnChange(object sender)
        {
            float f;
            if (float.TryParse(_positionXField.Value, out f))
            {
                ModelPart.Position = new Vector3(f, ModelPart.Position.Y, ModelPart.Position.Z);
            }
        }

        private void NameFieldOnChange(object sender)
        {
            ModelPart.Name = _nameField.Value;
        }

        public void SelectModelPart(ModelPart modelPart)
        {
            ModelPart = modelPart;
            _widget.Visible = ModelPartSelected;

            if (ModelPartSelected)
            {
                RefreshUI();    
            }
        }

        public void RefreshUI()
        {
            // name

            if (ModelPart == null)
                return;

            _nameField.Value = ModelPart.Name;
            
            // position x, y, z
            _positionXField.Value = ModelPart.Position.X.ToString("0.000");
            _positionYField.Value = ModelPart.Position.Y.ToString("0.000");
            _positionZField.Value = ModelPart.Position.Z.ToString("0.000");

            // size x, y, z
            _sizeXField.Value = ModelPart.Size.X.ToString("0.000");
            _sizeYField.Value = ModelPart.Size.Y.ToString("0.000");
            _sizeZField.Value = ModelPart.Size.Z.ToString("0.000");

            // rotation x, y, z
            _rotationXField.Value = ModelPart.Rotation.X.ToDegrees().ToString("0.00");
            _rotationYField.Value = ModelPart.Rotation.Y.ToDegrees().ToString("0.00");
            _rotationZField.Value = ModelPart.Rotation.Z.ToDegrees().ToString("0.00");

            // material
            _materialField.Value = GetMaterialName();

            _includeInAnimation.Visible = _editor.Animation != null;

            if (_editor.Animation != null)
            {
                _includeInAnimation.Value = !_editor.Animation.Ignores(_editor.FocusModelPart);
            }
        }

        private string GetMaterialName()
        {
            return _editor.Materials.HasMaterial(ModelPart.MaterialId) 
                ? _editor.Materials[ModelPart.MaterialId].Name 
                : "";
        }
    }
}
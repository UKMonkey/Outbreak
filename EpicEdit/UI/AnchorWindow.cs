using System.Collections.Generic;
using System.Linq;
using EpicEdit.Model;
using Psy.Core.EpicModel;
using Psy.Gui.Components;
using Psy.Gui.Events;
using SlimMath;

namespace EpicEdit.UI
{
    public class AnchorWindow
    {
        private const string AnchorNameTextboxName = "anchorName";
        private const string AnchorXTextboxName = "anchorX";
        private const string AnchorYTextboxName = "anchorY";
        private const string AnchorZTextboxName = "anchorZ";
        private const string AnchorRotationXTextboxName = "anchorRotX";
        private const string AnchorRotationYTextboxName = "anchorRotY";
        private const string AnchorRotationZTextboxName = "anchorRotZ";
        private const string AnchorParentAnchorLabelName = "anchorParentAnchorName";
        private const string AttachToParentAnchorButtonName = "attachAnchorButton";
        private const string DetachFromParentAnchorButtonName = "detachAnchorButton";
        private const string ListboxName = "childAnchorsListbox";
        private const string DeleteAnchorButtonName = "deleteAnchorButton";
        private const string DetachChildAnchorButtonName = "detachChildAnchorButton";

        private readonly Widget _widget;
        private readonly Editor _editor;

        private Textbox _anchorNameTextBox;
        private Textbox _anchorXTextbox;
        private Textbox _anchorYTextbox;
        private Textbox _anchorZTextbox;
        private Label _anchorParentAnchorNameLabel;
        private Button _attachToParentAnchorButton;
        private Button _detachFromParentAnchorButton;
        private Listbox _childAnchorsListBox;
        private Button _deleteAnchorButton;
        private int _selectedRowIndex;
        private Button _detachChildAnchorButton;
        private Textbox _anchorRotXTextbox;
        private Textbox _anchorRotYTextbox;
        private Textbox _anchorRotZTextbox;

        public AnchorWindow(Widget widget, Editor editor)
        {
            _widget = widget;
            _widget.Visible = false;
            _editor = editor;

            HookupWidgets();
        }

        private void HookupWidgets()
        {
            _anchorNameTextBox = _widget.FindWidgetByUniqueName<Textbox>(AnchorNameTextboxName);
            _anchorNameTextBox.Change += AnchorNameTextBoxOnChange;

            _anchorXTextbox = _widget.FindWidgetByUniqueName<Textbox>(AnchorXTextboxName);
            _anchorXTextbox.Change += AnchorXYZChange;

            _anchorYTextbox = _widget.FindWidgetByUniqueName<Textbox>(AnchorYTextboxName);
            _anchorYTextbox.Change += AnchorXYZChange;

            _anchorZTextbox = _widget.FindWidgetByUniqueName<Textbox>(AnchorZTextboxName);
            _anchorZTextbox.Change += AnchorXYZChange;

            _anchorRotXTextbox = _widget.FindWidgetByUniqueName<Textbox>(AnchorRotationXTextboxName);
            _anchorRotXTextbox.Change += AnchorRotXYZChange;

            _anchorRotYTextbox = _widget.FindWidgetByUniqueName<Textbox>(AnchorRotationYTextboxName);
            _anchorRotYTextbox.Change += AnchorRotXYZChange;

            _anchorRotZTextbox = _widget.FindWidgetByUniqueName<Textbox>(AnchorRotationZTextboxName);
            _anchorRotZTextbox.Change += AnchorRotXYZChange;

            _anchorParentAnchorNameLabel = _widget.FindWidgetByUniqueName<Label>(AnchorParentAnchorLabelName);

            _attachToParentAnchorButton = _widget.FindWidgetByUniqueName<Button>(AttachToParentAnchorButtonName);
            _attachToParentAnchorButton.Click += AttachToParentAnchorButtonOnClick;

            _detachFromParentAnchorButton = _widget.FindWidgetByUniqueName<Button>(DetachFromParentAnchorButtonName);
            _detachFromParentAnchorButton.Click += DetachFromParentAnchorButtonOnClick;

            _childAnchorsListBox = _widget.FindWidgetByUniqueName<Listbox>(ListboxName);
            _childAnchorsListBox.RowSelected += ChildAnchorsListBoxOnRowSelected;

            _detachChildAnchorButton = _widget.FindWidgetByUniqueName<Button>(DetachChildAnchorButtonName);
            _detachChildAnchorButton.Click += DetachChildAnchorButtonOnClick;

            _deleteAnchorButton = _widget.FindWidgetByUniqueName<Button>(DeleteAnchorButtonName);
            _deleteAnchorButton.Click += DeleteAnchorButtonOnClick;
        }

        private void DetachChildAnchorButtonOnClick(object sender, ClickEventArgs args)
        {
            var modelPart = _editor.FocusAnchor.Children[_selectedRowIndex];
            _editor.FocusAnchor.RemoveChild(modelPart);
            _editor.RefreshUI();
        }

        private void ChildAnchorsListBoxOnRowSelected(Listbox listbox, int rowNumber)
        {
            _selectedRowIndex = rowNumber;
        }

        private void DetachFromParentAnchorButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.FocusAnchor.DetachFromParent();
            _editor.RefreshUI();
        }

        private void AttachToParentAnchorButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.BeginAnchorJoin();
        }

        private void DeleteAnchorButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.DeleteSelectedAnchor();
        }

        private void AnchorRotXYZChange(object sender)
        {
            float x;
            float y;
            float z;

            if (!float.TryParse(_anchorRotXTextbox.Value, out x))
            {
                x = _editor.FocusAnchor.Rotation.X;
            }    
        
            if (!float.TryParse(_anchorRotYTextbox.Value, out y))
            {
                y = _editor.FocusAnchor.Rotation.Y;
            }   
         
            if (!float.TryParse(_anchorRotZTextbox.Value, out z))
            {
                z = _editor.FocusAnchor.Rotation.Z;
            }

            _editor.FocusAnchor.Rotation = new Vector3(x, y, z);
        }

        private void AnchorXYZChange(object sender)
        {
            float x;
            float y;
            float z;

            if (!float.TryParse(_anchorXTextbox.Value, out x))
            {
                x = _editor.FocusAnchor.Position.X;
            }

            if (!float.TryParse(_anchorYTextbox.Value, out y))
            {
                y = _editor.FocusAnchor.Position.Y;
            }
            
            if (!float.TryParse(_anchorZTextbox.Value, out z))
            {
                z = _editor.FocusAnchor.Position.Z;
            }

            _editor.FocusAnchor.Position = new Vector3(x, y, z);
        }

        private void AnchorNameTextBoxOnChange(object sender)
        {
            _editor.FocusAnchor.Name = _anchorNameTextBox.Value;
        }

        public void RefreshUI()
        {
            var anchor = _editor.FocusAnchor;

            if (anchor == null)
            {
                _widget.Visible = false;
                return;
            }

            _widget.Visible = true;

            _anchorNameTextBox.Value = anchor.Name;
            _anchorNameTextBox.Enabled = !anchor.IsPivot;

            _anchorXTextbox.Value = anchor.Position.X.ToString("0.000");
            _anchorYTextbox.Value = anchor.Position.Y.ToString("0.000");
            _anchorZTextbox.Value = anchor.Position.Z.ToString("0.000");

            _anchorRotXTextbox.Value = anchor.Rotation.X.ToString("0.000");
            _anchorRotYTextbox.Value = anchor.Rotation.Y.ToString("0.000");
            _anchorRotZTextbox.Value = anchor.Rotation.Z.ToString("0.000");

            _anchorParentAnchorNameLabel.Text = anchor.ModelPart.Name;

            _attachToParentAnchorButton.Visible = anchor.Parent == null;
            _detachFromParentAnchorButton.Visible = !_attachToParentAnchorButton.Visible;

            _childAnchorsListBox.Populate(GetRowData(anchor));

            _deleteAnchorButton.Visible = !anchor.IsPivot;

        }

        private static IEnumerable<IEnumerable<RowData>> GetRowData(Anchor anchor)
        {
            return anchor.Children.Select(child => new List<RowData>
            {
                new RowData
                {
                    Header = "Child anchors",
                    Data = child.Name
                }
            });
        }
    }
}
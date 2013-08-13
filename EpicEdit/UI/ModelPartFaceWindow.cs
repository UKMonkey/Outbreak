using EpicEdit.UI.Widgets;
using Psy.Gui.Components;
using Psy.Gui.Events;
using SlimMath;

namespace EpicEdit.UI
{
    public class ModelPartFaceWindow
    {
        private const string TexturePreviewName = "texturePreview";

        private const string TopLeftXName = "topLeftX";
        private const string TopLeftYName = "topLeftY";

        private const string TopRightXName = "topRightX";
        private const string TopRightYName = "topRightY";

        private const string BottomLeftXName = "bottomLeftX";
        private const string BottomLeftYName = "bottomLeftY";

        private const string BottomRightXName = "bottomRightX";
        private const string BottomRightYName = "bottomRightY";

        private const string ColourAName = "colourA";
        private const string ColourRName = "colourR";
        private const string ColourGName = "colourG";
        private const string ColourBName = "colourB";

        private const string CopyButtonName = "copyCoordinates";
        private const string PasteButtonName = "pasteCoordinates";

        private const string RotateClockwiseName = "rotateCW";
        private const string RotateCounterClockwiseName = "rotateCCW";

        private readonly Widget _widget;
        private readonly Editor _editor;
        private UVMapper _texturePreview;
        private Textbox _topLeftX;
        private Textbox _topLeftY;
        private Textbox _bottomLeftX;
        private Textbox _bottomLeftY;
        private Textbox _topRightX;
        private Textbox _topRightY;
        private Textbox _bottomRightX;
        private Textbox _bottomRightY;
        private Textbox _colourA;
        private Textbox _colourR;
        private Textbox _colourG;
        private Textbox _colourB;
        private Button _copyCoordinates;
        private Button _pasteCoordinates;

        private Button _rotateCw;
        private Button _rotateCcw;

        private bool ModelPartFaceSelected
        {
            get { return _editor.FocusModelPart != null; }
        }

        public ModelPartFaceWindow(Widget widget, Editor editor)
        {
            _widget = widget;
            _editor = editor;
            _widget.Visible = false;
            
            HookupWidgets();
        }

        private void HookupWidgets()
        {
            _texturePreview = _widget.FindWidgetByUniqueName<UVMapper>(TexturePreviewName);

            _texturePreview.UVPointChanged += TexturePreviewOnUVPointChanged;

            _topLeftX = _widget.FindWidgetByUniqueName<Textbox>(TopLeftXName);
            _topLeftY = _widget.FindWidgetByUniqueName<Textbox>(TopLeftYName);
            _bottomLeftX = _widget.FindWidgetByUniqueName<Textbox>(BottomLeftXName);
            _bottomLeftY = _widget.FindWidgetByUniqueName<Textbox>(BottomLeftYName);
            _topRightX = _widget.FindWidgetByUniqueName<Textbox>(TopRightXName);
            _topRightY = _widget.FindWidgetByUniqueName<Textbox>(TopRightYName);
            _bottomRightX = _widget.FindWidgetByUniqueName<Textbox>(BottomRightXName);
            _bottomRightY = _widget.FindWidgetByUniqueName<Textbox>(BottomRightYName);
            _colourA = _widget.FindWidgetByUniqueName<Textbox>(ColourAName);
            _colourR = _widget.FindWidgetByUniqueName<Textbox>(ColourRName);
            _colourG = _widget.FindWidgetByUniqueName<Textbox>(ColourGName);
            _colourB = _widget.FindWidgetByUniqueName<Textbox>(ColourBName);

            _rotateCw = _widget.FindWidgetByUniqueName<Button>(RotateClockwiseName);
            _rotateCw.Click += RotateCwOnClick;

            _rotateCcw = _widget.FindWidgetByUniqueName<Button>(RotateCounterClockwiseName);
            _rotateCcw.Click += RotateCcwOnClick;

            _copyCoordinates = _widget.FindWidgetByUniqueName<Button>(CopyButtonName);
            _copyCoordinates.Click += CopyCoordinatesOnClick;
            _pasteCoordinates = _widget.FindWidgetByUniqueName<Button>(PasteButtonName);
            _pasteCoordinates.Click += PasteCoordinatesOnClick;
        }

        private void RotateCcwOnClick(object sender, ClickEventArgs args)
        {
            var modelPartFace = _editor.GetCurrentModelPartFace();
            modelPartFace.RotateTextureCoordinatesCounterClockwise();
            RefreshUI();
        }

        private void RotateCwOnClick(object sender, ClickEventArgs args)
        {
            var modelPartFace = _editor.GetCurrentModelPartFace();
            modelPartFace.RotateTextureCoordinatesClockwise();
            RefreshUI();
        }

        private void PasteCoordinatesOnClick(object sender, ClickEventArgs args)
        {
            _editor.PasteFaceCoordinates();
            RefreshUI();
        }

        private void CopyCoordinatesOnClick(object sender, ClickEventArgs args)
        {
            _editor.CopyFaceCoordinates();
        }

        private void TexturePreviewOnUVPointChanged(object sender, UVPointChangedEventArgs args)
        {
            var modelPartFace = _editor.GetCurrentModelPartFace();

            modelPartFace.TextureCoordinates[args.PointIndex] = args.Position;

            RefreshUI();
        }

        public void SelectModelPartFace()
        {
            _widget.Visible = ModelPartFaceSelected;

            if (ModelPartFaceSelected)
            {
                RefreshUI();
            }
        }

        public void RefreshUI()
        {
            if (_editor.FocusModelPart == null)
            {
                _widget.Visible = false;
                return;
            }

            var modelPartFace = _editor.GetCurrentModelPartFace();

            _texturePreview.ImageName = GetMaterialTextureFilename();

            _texturePreview.Points = new Vector2[modelPartFace.TextureCoordinates.Length];
            for (var i = 0; i < _texturePreview.Points.Length; i++)
            {
                _texturePreview.Points[i] = modelPartFace.TextureCoordinates[i];
            }
            
            _bottomLeftX.Value = modelPartFace.TextureCoordinates[0].X.ToString("0.000");
            _bottomLeftY.Value = modelPartFace.TextureCoordinates[0].Y.ToString("0.000");

            _topLeftX.Value = modelPartFace.TextureCoordinates[1].X.ToString("0.000");
            _topLeftY.Value = modelPartFace.TextureCoordinates[1].Y.ToString("0.000");

            _topRightX.Value = modelPartFace.TextureCoordinates[2].X.ToString("0.000");
            _topRightY.Value = modelPartFace.TextureCoordinates[2].Y.ToString("0.000");

            _bottomRightX.Value = modelPartFace.TextureCoordinates[3].X.ToString("0.000");
            _bottomRightY.Value = modelPartFace.TextureCoordinates[3].Y.ToString("0.000");
        }

        private string GetMaterialTextureFilename()
        {
            return _editor.Materials.HasMaterial(_editor.FocusModelPart.MaterialId)
                ? _editor.Materials[_editor.FocusModelPart.MaterialId].TextureName
                : "";
        }

    }
}
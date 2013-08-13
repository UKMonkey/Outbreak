using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using EpicEdit.UI.Widgets;
using Psy.Core;
using Psy.Gui.Components;
using Psy.Gui.Events;
using Button = Psy.Gui.Components.Button;

namespace EpicEdit.UI
{
    public class MaterialsWindow
    {
        class MaterialWidget
        {
            private readonly MaterialsWindow _window;
            private readonly MatImage _image;

            private MaterialWidget(MaterialsWindow window, MatImage image)
            {
                _window = window;
                _image = image;

                image.Click += ImageOnClick;
            }

            public string MaterialName { get; private set; }

            public static MaterialWidget Create(MaterialsWindow window, int index)
            {
                var image = window._widget.FindWidgetByUniqueName<MatImage>(string.Format("mat{0:00}", index));

                if ((image == null))
                {
                    return null;
                }

                var materialWidget = new MaterialWidget(window, image);

                image.Metadata = materialWidget;

                return materialWidget;
            }

            private void ImageOnClick(object sender, ClickEventArgs args)
            {
                _window.OnTextureSelected(this);
            }

            public void SetMaterial(Material material)
            {
                _image.ImageName = material.TextureName;
                _image.FriendlyName = material.Name;
                MaterialName = material.Name;
            }
        }

        private void OnTextureSelected(MaterialWidget materialWidget)
        {
            _callback(materialWidget.MaterialName);
            _widget.Visible = false;
        }

        private readonly Widget _widget;
        private readonly Editor _editor;
        private readonly List<MaterialWidget> _materialWidgets;
        private Action<string> _callback;
        private readonly Button _addMaterialButton;
        private readonly OpenFileDialog _addMaterialDialog;

        public MaterialsWindow(Widget widget, Editor editor)
        {
            _widget = widget;
            _editor = editor;
            _materialWidgets = new List<MaterialWidget>();

            _addMaterialDialog = new OpenFileDialog
            {
                Filter = "PNG (*.png)|*.PNG|JPEG (*.jpg)|*.JPG|All Files (*.*)|*.*"
            };

            _widget.Visible = false;

            _addMaterialButton = _widget.FindWidgetByUniqueName<Button>("addMaterial");
            _addMaterialButton.Click += AddMaterialButtonOnClick;

            CreateMaterialButtons();
            RefreshMaterialButtons();
        }

        private void CreateMaterialButtons()
        {
            var i = 0;
            MaterialWidget materialWidget;
            while ((materialWidget = MaterialWidget.Create(this, i)) != null)
            {
                if (!_editor.Materials.HasMaterial(i))
                {
                    i++;
                    continue;
                }

                _materialWidgets.Add(materialWidget);
                i++;
            }
        }

        public void Show(Action<string> callback)
        {
            RefreshMaterialButtons();

            _widget.Visible = true;
            _callback = callback;
        }

        private void RefreshMaterialButtons()
        {
            var i = 0;
            MaterialWidget materialWidget;
            while ((materialWidget = MaterialWidget.Create(this, i)) != null)
            {
                if (!_editor.Materials.HasMaterial(i))
                {
                    i++;
                    continue;
                }

                materialWidget.SetMaterial(_editor.Materials[i]);

                i++;
            }

        }

        private void AddMaterialButtonOnClick(object sender, ClickEventArgs args)
        {
            var result = _addMaterialDialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            var filename = _addMaterialDialog.FileName;

            if (!File.Exists(filename))
            {
                MessageBox.Show(
                    string.Format("Can't open {0}", filename), 
                    "Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            _editor.Materials.Add(filename);
            RefreshMaterialButtons();
        }
    }
}
using System.Xml;
using Psy.Core;
using Psy.Gui;
using Psy.Gui.Components;
using Psy.Gui.Loader;
using SlimMath;

namespace EpicEdit.UI.Widgets
{
    public class MatImage : Widget
    {
        private const string XmlNodeName = "matimage";

        public string ImageName { get; set; }
        public string FriendlyName { get; set; }

        private MatImage(GuiManager guiManager, Widget parent) : base(guiManager, parent) {}

        private static Widget Create(GuiManager guiManager, XmlElement xmlElement, Widget parent)
        {
            var widget = new MatImage(guiManager, parent)
            {
                ImageName = xmlElement.GetString("imageName"),
            };
            return widget;
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            if (!string.IsNullOrEmpty(ImageName))
            {
                guiRenderer.Image(ImageName, Size, margin: Margin);

                var imageSize = guiRenderer.GetImageSize(ImageName);
                var goodImageSize = IsGoodImageSize(imageSize);

                var textColour = goodImageSize ? Colours.White : Colours.Red;

                if (!goodImageSize)
                {
                    guiRenderer.Line(new Vector2(0, 0), new Vector2(Size.X, Size.Y), Colours.Red);
                    guiRenderer.Line(new Vector2(1, 1), new Vector2(Size.X+1, Size.Y+1), Colours.Red);
                    guiRenderer.Line(new Vector2(Size.X, 0), new Vector2(0, Size.Y), Colours.Red);
                    TooltipText = string.Format("Image is not a valid size ({0}, {1})", imageSize.X, imageSize.Y);
                }
                else
                {
                    TooltipText = string.Format("{2} ({0}, {1})", imageSize.X, imageSize.Y, ImageName);
                }

                guiRenderer.Text("Arial", 12, FriendlyName, textColour, new Vector2(0, Size.Y));
            }

            base.Render(guiRenderer);
        }

        private static bool IsGoodImageSize(Vector2 imageSize)
        {
            var x = imageSize.X;

            if (x != imageSize.Y)
            {
                return false;
            }

            var ix = (int) x;

            return ((ix == 2) || (ix == 4) || (ix == 8) || (ix == 16) || (ix == 32) || (ix == 64) || (ix == 128) ||
                    (ix == 256) || (ix == 512));

        }

        public static void Register(XmlLoader loader)
        {
            loader.RegisterCustomWidget(XmlNodeName, Create);
        }
    }
}
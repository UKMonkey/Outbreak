using System.Globalization;
using System.Xml;
using Psy.Gui;
using Psy.Gui.Components;
using Psy.Gui.Loader;
using SlimMath;

namespace Outbreak.Client.Gui.Widgets
{
    public class InventorySlot : Widget
    {
        private readonly NinePatchHandle _ninePatchHandle;
        private const string XmlNodeName = "invslot";

        public string ImageName { get; set; }
        public int StackSize { get; set; }

        private InventorySlot(GuiManager guiManager, Widget parent = null) : base(guiManager, parent)
        {
            _ninePatchHandle = NinePatchHandle.Create("button");
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            if (guiRenderer.RenderMode == RenderMode.Normal)
            {
                guiRenderer.NinePatch(Size, _ninePatchHandle);
            }

            if (!string.IsNullOrEmpty(ImageName))
            {
                guiRenderer.Image(ImageName, Size, margin: Margin);
            }

            if (StackSize > 1)
            {
                guiRenderer.Text(StackSize.ToString(CultureInfo.InvariantCulture), new Vector2());
            }

            base.Render(guiRenderer);
        }

        private static Widget Create(GuiManager guiManager, XmlElement xmlElement, Widget parent)
        {
            var widget = new InventorySlot(guiManager, parent)
            {
                ImageName = xmlElement.GetString("imageName"),
            };
            return widget;
        }

        public static void Register(XmlLoader loader)
        {
            loader.RegisterCustomWidget(XmlNodeName, Create);
        }
    }
}
using System;
using System.Xml;
using Psy.Gui;
using Psy.Gui.Components;
using Psy.Gui.Loader;
using SlimMath;

namespace Outbreak.Client.Gui.Widgets
{
    public class ProgressBar : Widget
    {
        private const string XmlNodeName = "progressBar";

        public float MaxValue { get; set; }
        public float CurrentValue { get; set; }
        public bool Complete { get; set; }
        public bool Canceled { get; set; }
        public float Intensity { get; set; }

        protected ProgressBar(GuiManager guiManager, Widget parent = null) : base(guiManager, parent)
        {
            Intensity = 1;
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            var pctComplete = Complete || Canceled ? 1f
                              : Math.Min(CurrentValue/MaxValue, 1);

            guiRenderer.Image("healthbar", Size, null, pctComplete, 1, default(Vector2), Alpha, Intensity);
            guiRenderer.Image("healthbar_frame", Size, null, 1, 1, default(Vector2), Alpha, Intensity);

            var text = Complete ? "Complete"
                       : Canceled ? "Canceled"
                       : string.Format("{0:0.00}%", pctComplete*100);

            guiRenderer.Text(text, new Vector2(Size.X/2, Size.Y/2), VerticalAlignment.Middle, HorizontalAlignment.Centre);

            base.Render(guiRenderer);
        }

        private static Widget Create(GuiManager guiManager, XmlElement xmlElement, Widget parent)
        {
            var widget = new ProgressBar(guiManager, parent)
            {
                MaxValue = 100,
                CurrentValue = 0
            };
            return widget;
        }

        public static void Register(XmlLoader loader)
        {
            loader.RegisterCustomWidget(XmlNodeName, Create);
        }
    }
}
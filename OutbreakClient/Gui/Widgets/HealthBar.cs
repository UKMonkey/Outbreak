using System.Xml;
using Psy.Gui;
using Psy.Gui.Components;
using Psy.Gui.Loader;
using SlimMath;

namespace Outbreak.Client.Gui.Widgets
{
    public class HealthBar : Widget
    {
        private const string XmlNodeName = "healthbar";

        public int MaximumHealth { get; set; }
        public int Health { get; set; }

        protected HealthBar(GuiManager guiManager, Widget parent = null) : base(guiManager, parent) {}

        protected override void Render(IGuiRenderer guiRenderer)
        {
            var healthPct = Health/(float) MaximumHealth;

            guiRenderer.Image("healthbar", Size, null, healthPct);
            guiRenderer.Image("healthbar_frame", Size);

            guiRenderer.Text(string.Format("{0}/{1}", Health, MaximumHealth),
                             new Vector2(Size.X/2, Size.Y/2), VerticalAlignment.Middle, HorizontalAlignment.Centre);

            base.Render(guiRenderer);
        }

        private static Widget Create(GuiManager guiManager, XmlElement xmlElement, Widget parent)
        {
            var widget = new HealthBar(guiManager, parent)
            {
                MaximumHealth = 100,
                Health = 0
            };
            return widget;
        }

        public static void Register(XmlLoader loader)
        {
            loader.RegisterCustomWidget(XmlNodeName, Create);
        }
    }
}
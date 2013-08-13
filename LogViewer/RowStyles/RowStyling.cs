using System.Drawing;

namespace LogViewer.RowStyles
{
    public class RowStyling
    {
        public Color? BackgroundColor;
        public Color? ForegroundColor;
        public bool? Bold;
        public bool? Italic;
        public bool? DarkenBackground;

        public void Combine(RowStyling styling)
        {
            if (styling.BackgroundColor.HasValue)
            {
                BackgroundColor = styling.BackgroundColor;
            }
            if (styling.Bold.HasValue)
            {
                Bold = styling.Bold;
            }
            if (styling.ForegroundColor.HasValue)
            {
                ForegroundColor = styling.ForegroundColor;
            }
            if (styling.Italic.HasValue)
            {
                Italic = styling.Italic;
            }
            if (styling.DarkenBackground.HasValue && styling.BackgroundColor.HasValue)
            {
                BackgroundColor = DarkenColour(styling.BackgroundColor.Value);
            }
        }

        private static Color DarkenColour(Color value)
        {
            return Color.FromArgb(value.R/2, value.G/2, value.B/2);
        }
    }
}
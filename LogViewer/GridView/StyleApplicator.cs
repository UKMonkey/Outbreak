using System.Drawing;
using System.Windows.Forms;
using LogViewer.RowStyles;

namespace LogViewer.GridView
{
    class StyleApplicator : IStyleApplicator 
    {
        public void ApplyStyle(DataGridView gridView, DataGridViewRow row, RowStyling style)
        {
            var isBold = false;
            var isItalic = false;

            if (style.BackgroundColor.HasValue)
            {
                row.DefaultCellStyle.BackColor = style.BackgroundColor.Value;
            }
            if (style.Bold.HasValue)
            {
                isBold = style.Bold.Value;
            }
            if (style.ForegroundColor.HasValue)
            {
                row.DefaultCellStyle.ForeColor = style.ForegroundColor.Value;
            }
            if (style.Italic.HasValue)
            {
                isItalic = style.Italic.Value;
            }

            var newFontStyle = 
                (isItalic ? FontStyle.Italic : FontStyle.Regular) |
                (isBold ? FontStyle.Bold : FontStyle.Regular);

            var defaultFont = row.DefaultCellStyle.Font ?? gridView.DefaultCellStyle.Font;

            row.DefaultCellStyle.Font = new Font(defaultFont, newFontStyle);
            row.Height = 16;
        }
    }
}
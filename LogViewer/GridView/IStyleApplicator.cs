using System.Windows.Forms;
using LogViewer.RowStyles;

namespace LogViewer.GridView
{
    public interface IStyleApplicator
    {
        void ApplyStyle(DataGridView gridView, DataGridViewRow row, RowStyling style);
    }
}
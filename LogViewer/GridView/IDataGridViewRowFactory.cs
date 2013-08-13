using System.Windows.Forms;
using LogViewer.LogFile;

namespace LogViewer.GridView
{
    public interface IDataGridViewRowFactory
    {
        DataGridViewRow Create(Row row);
    }
}
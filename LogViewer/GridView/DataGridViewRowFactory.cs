using System.Windows.Forms;
using LogViewer.LogFile;

namespace LogViewer.GridView
{
    class DataGridViewRowFactory : IDataGridViewRowFactory
    {
        public DataGridViewRow Create(Row row)
        {
            var result = new DataGridViewRow();

            var dateCell = CreateCell(row.Timestamp.ToString("MM/dd/yyyy HH:mm:ss.fff"));
            var sourceCell = CreateCell(row.SourceIdentifier);
            var levelCell = CreateCell(row.Level);
            var metaCell = CreateCell(row.Meta);
            var outputCell = CreateCell(row.Output);

            result.Cells.Add(dateCell);
            result.Cells.Add(sourceCell);
            result.Cells.Add(levelCell);
            result.Cells.Add(metaCell);
            result.Cells.Add(outputCell);

            return result;
        }

        private static DataGridViewCell CreateCell(string text)
        {
            var cell = new DataGridViewTextBoxCell {Value = text};
            return cell;
        }
    }
}
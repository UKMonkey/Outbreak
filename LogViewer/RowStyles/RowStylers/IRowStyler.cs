using LogViewer.LogFile;

namespace LogViewer.RowStyles.RowStylers
{
    public interface IRowStyler
    {
        RowStyling GetStyle(Row row);
    }
}
using System.Collections.Generic;
using LogViewer.LogFile;

namespace LogViewer.RowStyles.RowStylers
{
    class CombinedRowStyler : IRowStyler
    {
        private readonly List<IRowStyler> _stylers;

        public CombinedRowStyler(params IRowStyler[] styler)
        {
            _stylers = new List<IRowStyler>(styler);
        }

        public RowStyling GetStyle(Row row)
        {
            var rowStyle = new RowStyling();

            foreach (var styler in _stylers)
            {
                rowStyle.Combine(styler.GetStyle(row));
            }

            return rowStyle;
        }
    }
}
using System.Collections.Generic;
using System.Windows.Forms;
using LogViewer.LogFile;
using LogViewer.RowStyles.RowStylers;

namespace LogViewer.GridView
{
    public class GridLogPresenter : ILogPresenter
    {
        private readonly DataGridView _grid;
        private readonly IRowStyler _rowStyler;
        private readonly IStyleApplicator _styleApplicator;
        private readonly IDataGridViewRowFactory _dataGridViewRowFactory;
        private readonly List<Row> _rows;

        public GridLogPresenter(
            DataGridView grid, 
            IRowStyler rowStyler, 
            IStyleApplicator styleApplicator,
            IDataGridViewRowFactory dataGridViewRowFactory)
        {
            _grid = grid;
            _rowStyler = rowStyler;
            _styleApplicator = styleApplicator;
            _dataGridViewRowFactory = dataGridViewRowFactory;
            _rows = new List<Row>();
            _grid.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
        }

        public void Begin()
        {
            _rows.Clear();
        }

        public void Add(Row row)
        {
            _rows.Add(row);
        }

        public void End()
        {
            _rows.Sort();

            foreach (var row in _rows)
            {
                var gridRow = _dataGridViewRowFactory.Create(row);
                var style = _rowStyler.GetStyle(row);
                _styleApplicator.ApplyStyle(_grid, gridRow, style);
                _grid.Rows.Add(gridRow);
            }
        }
    }
}
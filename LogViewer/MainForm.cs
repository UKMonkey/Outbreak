using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Psy.Core.Configuration;

namespace LogViewer
{
    public partial class MainForm : Form
    {
        private readonly LogViewerApplication _application;

        public MainForm()
        {
            InitializeComponent();
            _application = new LogViewerApplication(true);

            FileSystemWatcher.Path = LogViewerApplication.LogPath;
            FileSystemWatcher.Filter = LogViewerApplication.LogFileFilter;

            SetDefaultWindowState();
        }

        private void SetDefaultWindowState()
        {
            var defaultScreenIndex = StaticConfigurationManager.ConfigurationManager.GetInt("DefaultScreen");
            var isMaximized = StaticConfigurationManager.ConfigurationManager.GetBool("Maximize");

            Location = Screen.AllScreens[defaultScreenIndex].WorkingArea.Location;
            WindowState = isMaximized ? FormWindowState.Maximized : FormWindowState.Normal;
        }

        private void AddNewTab(string tabName)
        {
            var tab = 
                new TabPage(tabName)
                {
                    Name = tabName, 
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };

            FilePicker.TabPages.Add(tab);

            var gridView = 
                new GridViewControl
                {
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };

            tab.Controls.Add(gridView);
        }

        private void FileSystemWatcherChanged(object sender, FileSystemEventArgs e)
        {
            if (!FilePicker.TabPages.ContainsKey(e.Name))
                AddNewTab(e.Name);

            var gridView = FilePicker.TabPages[e.Name].Controls[0] as GridViewControl;
            if (gridView != null)
                _application.QueueFileForProcessing(e.FullPath, new List<DataGridView> {gridView.Grid, CombinedGrid.Grid} );
        }

        private void PopulateTimerTick(object sender, EventArgs e)
        {
            if (PauseButton.Checked)
                return;

            _application.ProcessFileQueue();
        }

        /*
        private void GridRowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (LogGrid.Rows.Count == 0)
                return;

            var hasScrolledAway = Math.Abs(_lastSetPosition - _scrollIndex) > 2;

            _lastSetPosition = LogGrid.Rows.Count;
            if (hasScrolledAway && _scrollIndex != -1)
            {
                return;
            }

            LogGrid.FirstDisplayedCell = LogGrid.Rows[LogGrid.Rows.Count - 1].Cells[0];
        }
        */

        /*
        private void GridScroll(object sender, ScrollEventArgs e)
        {
            _scrollIndex = LastVisibleRow();
        }
        */

        /*
        private int LastVisibleRow()
        {
            return LogGrid.FirstDisplayedCell.RowIndex + LogGrid.DisplayedRowCount(true);
        }
        */

        private void ClearToolStripMenuItemClick(object sender, EventArgs e)
        {
            _application.ClearLog();
            //LogGrid.Rows.Clear();
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            PauseButton.Checked = !PauseButton.Checked;
            Text = PauseButton.Checked ? "LogViewer - PAUSED" : "LogViewer";
        }
    }
}

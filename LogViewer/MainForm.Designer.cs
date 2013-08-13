namespace LogViewer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.FileSystemWatcher = new System.IO.FileSystemWatcher();
            this.PopulateTimer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.resultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClearButton = new System.Windows.Forms.ToolStripMenuItem();
            this.PauseButton = new System.Windows.Forms.ToolStripMenuItem();
            this.FilePicker = new System.Windows.Forms.TabControl();
            this.Combined = new System.Windows.Forms.TabPage();
            this.CombinedGrid = new LogViewer.GridViewControl();
            ((System.ComponentModel.ISupportInitialize)(this.FileSystemWatcher)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.FilePicker.SuspendLayout();
            this.Combined.SuspendLayout();
            this.SuspendLayout();
            // 
            // FileSystemWatcher
            // 
            this.FileSystemWatcher.EnableRaisingEvents = true;
            this.FileSystemWatcher.NotifyFilter = System.IO.NotifyFilters.Size;
            this.FileSystemWatcher.SynchronizingObject = this;
            this.FileSystemWatcher.Changed += new System.IO.FileSystemEventHandler(this.FileSystemWatcherChanged);
            // 
            // PopulateTimer
            // 
            this.PopulateTimer.Enabled = true;
            this.PopulateTimer.Interval = 500;
            this.PopulateTimer.Tick += new System.EventHandler(this.PopulateTimerTick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resultsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(735, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // resultsToolStripMenuItem
            // 
            this.resultsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClearButton,
            this.PauseButton});
            this.resultsToolStripMenuItem.Name = "resultsToolStripMenuItem";
            this.resultsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.resultsToolStripMenuItem.Text = "Results";
            // 
            // ClearButton
            // 
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(105, 22);
            this.ClearButton.Text = "Clear";
            this.ClearButton.Click += new System.EventHandler(this.ClearToolStripMenuItemClick);
            // 
            // PauseButton
            // 
            this.PauseButton.Name = "PauseButton";
            this.PauseButton.Size = new System.Drawing.Size(105, 22);
            this.PauseButton.Text = "Pause";
            this.PauseButton.Click += new System.EventHandler(this.PauseButton_Click);
            // 
            // FilePicker
            // 
            this.FilePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilePicker.Controls.Add(this.Combined);
            this.FilePicker.Location = new System.Drawing.Point(12, 27);
            this.FilePicker.Name = "FilePicker";
            this.FilePicker.SelectedIndex = 0;
            this.FilePicker.Size = new System.Drawing.Size(711, 493);
            this.FilePicker.TabIndex = 3;
            // 
            // Combined
            // 
            this.Combined.Controls.Add(this.CombinedGrid);
            this.Combined.Location = new System.Drawing.Point(4, 22);
            this.Combined.Name = "Combined";
            this.Combined.Padding = new System.Windows.Forms.Padding(3);
            this.Combined.Size = new System.Drawing.Size(703, 467);
            this.Combined.TabIndex = 0;
            this.Combined.Text = "Combined";
            this.Combined.UseVisualStyleBackColor = true;
            // 
            // CombinedGrid
            // 
            this.CombinedGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CombinedGrid.Location = new System.Drawing.Point(0, 0);
            this.CombinedGrid.Name = "CombinedGrid";
            this.CombinedGrid.Size = new System.Drawing.Size(703, 467);
            this.CombinedGrid.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 529);
            this.Controls.Add(this.FilePicker);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "LogViewer";
            ((System.ComponentModel.ISupportInitialize)(this.FileSystemWatcher)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.FilePicker.ResumeLayout(false);
            this.Combined.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.FileSystemWatcher FileSystemWatcher;
        private System.Windows.Forms.Timer PopulateTimer;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem resultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ClearButton;
        private System.Windows.Forms.ToolStripMenuItem PauseButton;
        private System.Windows.Forms.TabControl FilePicker;
        private System.Windows.Forms.TabPage Combined;
        private GridViewControl CombinedGrid;
    }
}


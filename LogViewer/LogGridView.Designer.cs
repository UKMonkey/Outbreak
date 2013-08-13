namespace LogViewer
{
    partial class LogGridView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.LogGrid = new System.Windows.Forms.DataGridView();
            this.Timestamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Meta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Output = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.LogGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // LogGrid
            // 
            this.LogGrid.AllowUserToAddRows = false;
            this.LogGrid.AllowUserToDeleteRows = false;
            this.LogGrid.AllowUserToResizeRows = false;
            this.LogGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LogGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.LogGrid.ColumnHeadersHeight = 22;
            this.LogGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.LogGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Timestamp,
            this.Source,
            this.Level,
            this.Meta,
            this.Output});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.LogGrid.DefaultCellStyle = dataGridViewCellStyle1;
            this.LogGrid.GridColor = System.Drawing.SystemColors.Window;
            this.LogGrid.Location = new System.Drawing.Point(9, 9);
            this.LogGrid.Margin = new System.Windows.Forms.Padding(0);
            this.LogGrid.Name = "LogGrid";
            this.LogGrid.ReadOnly = true;
            this.LogGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.LogGrid.RowHeadersVisible = false;
            this.LogGrid.RowHeadersWidth = 40;
            this.LogGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.LogGrid.RowTemplate.Height = 10;
            this.LogGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.LogGrid.Size = new System.Drawing.Size(814, 462);
            this.LogGrid.TabIndex = 2;
            // 
            // Timestamp
            // 
            this.Timestamp.HeaderText = "Timestamp";
            this.Timestamp.Name = "Timestamp";
            this.Timestamp.ReadOnly = true;
            this.Timestamp.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Timestamp.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Timestamp.Width = 150;
            // 
            // Source
            // 
            this.Source.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Source.HeaderText = "Source";
            this.Source.Name = "Source";
            this.Source.ReadOnly = true;
            this.Source.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Source.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Source.Width = 47;
            // 
            // Level
            // 
            this.Level.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Level.HeaderText = "Level";
            this.Level.Name = "Level";
            this.Level.ReadOnly = true;
            this.Level.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Level.Width = 58;
            // 
            // Meta
            // 
            this.Meta.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Meta.HeaderText = "Meta";
            this.Meta.Name = "Meta";
            this.Meta.ReadOnly = true;
            this.Meta.Width = 56;
            // 
            // Output
            // 
            this.Output.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Output.HeaderText = "Output";
            this.Output.Name = "Output";
            this.Output.ReadOnly = true;
            this.Output.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Output.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // GridView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(832, 480);
            this.ControlBox = false;
            this.Controls.Add(this.LogGrid);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GridView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "GridView";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.LogGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView LogGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Timestamp;
        private System.Windows.Forms.DataGridViewTextBoxColumn Source;
        private System.Windows.Forms.DataGridViewTextBoxColumn Level;
        private System.Windows.Forms.DataGridViewTextBoxColumn Meta;
        private System.Windows.Forms.DataGridViewTextBoxColumn Output;
    }
}
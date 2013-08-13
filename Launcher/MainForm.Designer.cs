namespace Launcher
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
            this.PatchProgress = new System.Windows.Forms.ProgressBar();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.StepProgress = new System.Windows.Forms.Label();
            this.Poller = new System.Windows.Forms.Timer(this.components);
            this.PlayButton = new System.Windows.Forms.Button();
            this.WebBrowser = new System.Windows.Forms.WebBrowser();
            this.ExitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PatchProgress
            // 
            this.PatchProgress.Location = new System.Drawing.Point(12, 389);
            this.PatchProgress.MarqueeAnimationSpeed = 1;
            this.PatchProgress.Name = "PatchProgress";
            this.PatchProgress.Size = new System.Drawing.Size(484, 28);
            this.PatchProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.PatchProgress.TabIndex = 0;
            this.PatchProgress.Value = 40;
            // 
            // StatusLabel
            // 
            this.StatusLabel.BackColor = System.Drawing.Color.Transparent;
            this.StatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLabel.ForeColor = System.Drawing.Color.White;
            this.StatusLabel.Location = new System.Drawing.Point(12, 341);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(484, 23);
            this.StatusLabel.TabIndex = 1;
            // 
            // StepProgress
            // 
            this.StepProgress.BackColor = System.Drawing.Color.Transparent;
            this.StepProgress.ForeColor = System.Drawing.Color.White;
            this.StepProgress.Location = new System.Drawing.Point(12, 364);
            this.StepProgress.Name = "StepProgress";
            this.StepProgress.Size = new System.Drawing.Size(484, 22);
            this.StepProgress.TabIndex = 2;
            // 
            // Poller
            // 
            this.Poller.Interval = 500;
            this.Poller.Tick += new System.EventHandler(this.PollerTick);
            // 
            // PlayButton
            // 
            this.PlayButton.BackColor = System.Drawing.Color.Lime;
            this.PlayButton.Enabled = false;
            this.PlayButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PlayButton.ForeColor = System.Drawing.Color.Black;
            this.PlayButton.Location = new System.Drawing.Point(544, 370);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(101, 47);
            this.PlayButton.TabIndex = 3;
            this.PlayButton.Text = "Play";
            this.PlayButton.UseVisualStyleBackColor = false;
            this.PlayButton.Click += new System.EventHandler(this.PlayButtonClick);
            // 
            // WebBrowser
            // 
            this.WebBrowser.AllowWebBrowserDrop = false;
            this.WebBrowser.IsWebBrowserContextMenuEnabled = false;
            this.WebBrowser.Location = new System.Drawing.Point(12, 33);
            this.WebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.WebBrowser.Name = "WebBrowser";
            this.WebBrowser.ScriptErrorsSuppressed = true;
            this.WebBrowser.Size = new System.Drawing.Size(633, 305);
            this.WebBrowser.TabIndex = 4;
            this.WebBrowser.Url = new System.Uri("http://outbreak-game.co.uk/launcher.html", System.UriKind.Absolute);
            // 
            // ExitButton
            // 
            this.ExitButton.BackColor = System.Drawing.Color.Transparent;
            this.ExitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExitButton.ForeColor = System.Drawing.Color.Black;
            this.ExitButton.Location = new System.Drawing.Point(618, -1);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(27, 28);
            this.ExitButton.TabIndex = 5;
            this.ExitButton.Text = "X";
            this.ExitButton.UseVisualStyleBackColor = false;
            this.ExitButton.Click += new System.EventHandler(this.ExitButtonClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(657, 429);
            this.ControlBox = false;
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.WebBrowser);
            this.Controls.Add(this.PlayButton);
            this.Controls.Add(this.StepProgress);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.PatchProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar PatchProgress;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Label StepProgress;
        private System.Windows.Forms.Timer Poller;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.WebBrowser WebBrowser;
        private System.Windows.Forms.Button ExitButton;

    }
}


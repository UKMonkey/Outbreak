using System;
using System.Windows.Forms;

namespace Launcher
{
    public class View
    {
        private readonly WebBrowser _webBrowser;
        private readonly ProgressBar _progressBar;
        private readonly Label _progressText1;
        private readonly Label _progressText2;

        public View(WebBrowser webBrowser, ProgressBar progressBar, Label progressText1, 
            Label progressText2, int minimumValue, int maximumValue, int value)
        {
            _webBrowser = webBrowser;
            _progressBar = progressBar;
            _progressText1 = progressText1;
            _progressText2 = progressText2;
            progressBar.Maximum = maximumValue;
            progressBar.Minimum = minimumValue;
            progressBar.Value = value;
        }

        public string WebBrowserUrl
        {
            set
            {
                _webBrowser.Url =  new Uri(value);
            }
        }

        public int ProgressBarValue
        {
            get { return _progressBar.Value; }
            set { _progressBar.Value = value; }
        }

        public int ProgressBarMaxiumum
        {
            get { return _progressBar.Maximum; }
            set { _progressBar.Maximum = value; }
        }

        public int ProgressBarMinimum
        {
            get { return _progressBar.Minimum; }
            set { _progressBar.Minimum = value; }
        }

        public string ProgressText1
        {
            get { return _progressText1.Text; }
            set { _progressText1.Text = value; }
        }

        public string ProgressText2
        {
            get { return _progressText2.Text; }
            set { _progressText2.Text = value; }
        }
    }
}
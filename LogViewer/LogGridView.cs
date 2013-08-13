using System.Windows.Forms;

namespace LogViewer
{
    public partial class LogGridView : Form
    {
        public DataGridView Grid { get { return LogGrid; } }

        public LogGridView()
        {
            InitializeComponent();
        }
    }
}

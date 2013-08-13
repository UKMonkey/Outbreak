using System;
using System.Globalization;
using System.Windows.Forms;
using BiomeGen;

namespace BiomeGenerator
{
    public partial class MainForm : Form
    {
        private Generator Generator { get; set; }
        private int Seed { get; set; }
        private GeneratorRenderer Renderer { get; set; }
        
        public MainForm(Generator generator)
        {
            Generator = generator;
            Renderer = new GeneratorRenderer();
            InitializeComponent();
            Reset();
            Seed = 73;
            ClientSize = Generator.Size();
        }

        private void Reset()
        {
            Generator.Generate(Seed);
            Text = string.Format("Seed:{0}, Tris:{1}", Seed.ToString(CultureInfo.InvariantCulture), Generator.Rects.Count);
            Invalidate();
        }

        private void FormPaint(object sender, PaintEventArgs e)
        {
            Renderer.Render(e.Graphics, Generator);
        }

        private void Form1Click(object sender, EventArgs e)
        {
            Seed++;
            Reset();
        }

        private void Form1ResizeBegin(object sender, EventArgs e)
        {
            Reset();
        }
    }
}

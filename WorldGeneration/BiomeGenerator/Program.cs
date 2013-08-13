using System;
using System.Windows.Forms;
using Outbreak.BiomeGen;

namespace BiomeGenerator
{
    static class Program
    {
        private const int ChunkSizeInMetres = 16;
        private const int BiomeSizeInChunks = 10;
        private static float Resolution = 0.25f;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var generator = new FarmGenerator(ChunkSizeInMetres, BiomeSizeInChunks, Resolution);
            var generator = new SuburbGenerator(ChunkSizeInMetres, BiomeSizeInChunks, Resolution);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(generator));
        }

        
    }
}

using System;
using System.Windows.Forms;

namespace TetrisClone
{
    static class Tetris
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TetrisForm form = new TetrisForm();
            form.Show();
            form.gameLoop();
        }
    }
}

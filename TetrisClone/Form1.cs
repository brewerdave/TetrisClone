using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace TetrisClone
{
    public partial class TetrisForm : Form
    {
        private Stopwatch watch;
        private long lastTime;
        private Board board;
        private SolidBrush greyBrush;
        public int columns { get; set; }
        public int rows { get; set; }

        public TetrisForm()
        {
            columns = 10;
            rows = 22;
            InitializeComponent();
            newGame();
            panel1.Paint += new PaintEventHandler(panel1_Paint);
            watch = Stopwatch.StartNew();
            greyBrush = new SolidBrush(Color.Gray);
        }

        public void gameLoop()
        {
            
            while (this.Created)
            {
                long currentTime = watch.ElapsedMilliseconds;
                long elapsedTime = currentTime - lastTime;
                if (elapsedTime > 1000) //1 second
                {
                    lastTime = currentTime;
                    board.moveDown();
                    if (board.gameOver()) newGame();
                    panel1.Invalidate();
                }
                Application.DoEvents();
            }
        }

        public void newGame()
        {
            board = new Board(columns, rows);
            gameLoop();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            board.paintBoard(e);
            e.Graphics.FillRectangle(greyBrush, 0, 0, 211, 42);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case (Keys)40:
                    board.moveDown();
                    panel1.Invalidate();
                    break;

                case (Keys)39:
                    board.fallingBlock.moveRight();
                    panel1.Invalidate();
                    break;

                case (Keys)37:
                    board.fallingBlock.moveLeft();
                    panel1.Invalidate();
                    break;

                case (Keys)38:
                    board.fallingBlock.rotate();
                    panel1.Invalidate();
                    break;
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGame();
        }
    }
}

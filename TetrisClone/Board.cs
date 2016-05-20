using System;
using System.Drawing;
using System.Windows.Forms;

namespace TetrisClone
{
    /// <summary>
    /// This class handles the game board and blocks
    /// </summary>
    class Board
    {
        Rectangle[,] blockGrid;
        SolidBrush[,] colorGrid;
        public Tetromino fallingBlock;
        public int columns { get; }
        public int rows { get; }
        public int score { get; set; }
        public int level { get; set; }
        public int rowsCleared { get; set; }
        public Tetromino nextBlock;

        public Board(int columns, int rows) 
        {
            this.columns = columns;
            this.rows = rows;
            score = 0;
            level = 1;
            blockGrid = new Rectangle[columns, rows];
            colorGrid = new SolidBrush[columns, rows];
            fallingBlock = new Tetromino(this);
            fallingBlock.makeFallingBlock();
            nextBlock = new Tetromino(this);
        }
        
        public void paintBoard(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            fallingBlock.paintBlock(e);

            for (int i = 0; i < columns; ++i)
            {
                for (int j = 0; j < rows; ++j)
                {
                    if (!blockGrid[i, j].IsEmpty)
                    {
                        g.FillRectangle(colorGrid[i, j], blockGrid[i, j]);
                    }
                }
            }
        }

        public void paintNextBlock(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            nextBlock.paintBlock(e);
        }

        public bool isSpotEmpty(int x, int y)
        {
            if (blockGrid[x, y].IsEmpty)
                return true;
            else return false;
        }

        public void newFallingBlock()
        {
            fallingBlock = nextBlock.makeFallingBlock();            
            nextBlock = new Tetromino(this);
        }

        /// <summary>
        /// Drops block and adds a new one
        /// </summary>
        public void moveDown()
        {
            if (!fallingBlock.moveBlock(0, 1))
            {
                addBlock();
                newFallingBlock();
            }
        }

        /// <summary>
        /// Adds falling block to the placed blocks
        /// </summary>
        public void addBlock()
        {
            Rectangle[] rects = fallingBlock.shapeRects;
            for(int i=0; i < rects.Length; ++i)
            {
                int x = rects[i].X / 21;
                int y = rects[i].Y / 21;

                if (y < 2)
                {
                    gameOver();
                }

                blockGrid[x, y] = rects[i];
                colorGrid[x, y] = new SolidBrush(fallingBlock.color);
            }

            clearRows();
        }

        /// <summary>
        /// Checks and removes full rows.
        /// </summary>
        public void clearRows()
        {
            int rowsDone = 0;
            for (int r = rows -1; r >=0; --r)
            {
                bool rowFull = true;
                for (int c = 0; c < columns; ++c)
                {
                    if(isSpotEmpty(c,r))
                    {
                        rowFull = false;
                        break;
                    }
                }
                if (rowFull)
                {
                    rowsDone += 1;
                    for (int j = r; j > 0; --j)
                    {
                        for (int i = 0; i < columns; ++i)
                        {
                            if (!isSpotEmpty(i, j - 1))
                            {
                                blockGrid[i, j - 1].Y += 21; //adjust rectangle coords
                            }
                            blockGrid[i,j] = blockGrid[i,j - 1];  //move row down
                            colorGrid[i,j] = colorGrid[i,j - 1];
                        }
                    }
                    r++; //recheck this row
                }
            }
            addScore(rowsDone);
        }

        private void addScore(int rows)
        {
            score += 40 * rows * level;
            rowsCleared += rows;
            level = (rowsCleared / 20) + 1;
        }

        public bool gameOver()
        {
            for (int i = 0; i < columns; ++i)
            {
                for(int j=0; j< 2; ++j)
                {
                    if (!isSpotEmpty(i, j))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

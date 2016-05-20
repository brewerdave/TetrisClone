using System;
using System.Drawing;
using System.Windows.Forms;

namespace TetrisClone
{
    class Tetromino
    {
        private static readonly bool[,] S_BLOCK = {
            { false, true,  true },
            { true,  true,  false},
            { false, false, false } };

        private static readonly bool[,] Z_BLOCK = {
            { true,  true,  false },
            { false, true,  true  },
            { false, false, false } };

        private static readonly bool[,] T_BLOCK = {
            { false, true,  false },
            { true,  true,  true  },
            { false, false, false } };

        private static readonly bool[,] O_BLOCK = {
            { true,  true },
            { true,  true } };

        private static readonly bool[,] I_BLOCK = {
            { false, false, false, false },
            { true,  true,  true,  true  },
            { false, false, false, false },
            { false, false, false, false } };

        private static readonly bool[,] J_BLOCK = {
            { true, false,  false },
            { true,  true,  true  },
            { false, false, false } };

        private static readonly bool[,] L_BLOCK = {
            { false, false, true  },
            { true,  true,  true  },
            { false, false, false } };

        private static readonly bool[][,] BLOCK_TYPES = {S_BLOCK, Z_BLOCK, T_BLOCK,
            O_BLOCK, I_BLOCK, J_BLOCK, L_BLOCK };

        private static readonly Color[] BLOCK_COLORS = {Color.Green, Color.Red, Color.Purple,
                                                    Color.Yellow, Color.Cyan, Color.Blue, Color.Orange };

        private static readonly int BLOCK_WIDTH = 20;

        public Rectangle[] shapeRects { get; set; }
        public Color color { get; }
        private Board board;
        private int blockType, blockRow, blockColumn, totalColumns, totalRows, previewOffset;
        private bool[,] shapeBool;

        public Tetromino(Board b)
        {
            board = b;
            totalColumns = board.columns;
            totalRows = board.rows;
            Random rand = new Random();
            blockType = rand.Next(0, 7); // 0 to 6
            shapeBool = BLOCK_TYPES[blockType];
            color = BLOCK_COLORS[blockType];
            shapeRects = new Rectangle[4];

            blockRow = 0;
            blockColumn = 0;

            // offset for preview window
            if (shapeBool.GetLength(0) != 4) { previewOffset = 11; }

            shapeRects = getRectangles(shapeBool);
        }

        public Tetromino makeFallingBlock()
        {
            blockColumn = (totalColumns / 2) - (shapeBool.GetLength(0) / 2); //center falling block
            previewOffset = 0;
            shapeRects = getRectangles(shapeBool);
            return this;
        }

        private Rectangle[] getRectangles(bool[,] shapeArray)
        {
            Rectangle[] returnArray = new Rectangle[4];
            int rectNumber = 0;
            int iLength = shapeArray.GetLength(0);
            int jLength = shapeArray.GetLength(1);
            for (int i = 0; i < iLength; ++i) //rows
            {
                for (int j = 0; j < jLength; ++j) //columns
                {
                    if (shapeArray[i, j])
                    {
                        returnArray[rectNumber] = new Rectangle((blockColumn * (BLOCK_WIDTH + 1)) + ((BLOCK_WIDTH + 1) * j) + 1 + previewOffset, //x
                                                               (blockRow * (BLOCK_WIDTH + 1)) + ((BLOCK_WIDTH + 1) * i) + 1 + previewOffset, //y 
                                                                BLOCK_WIDTH, BLOCK_WIDTH);
                        ++rectNumber;
                    }
                }
            }
            return returnArray;
        }

        public void paintBlock(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush brush = new SolidBrush(color);

            for (int i = 0; i < shapeRects.Length; ++i)
            {
                g.FillRectangle(brush, shapeRects[i]);
            }
        }

        private bool isValidMove(Rectangle[] rectangles)
        {
            for (int i = 0; i < rectangles.Length; ++i)
            {
                //check right/left/bottom boundaries
                if (rectangles[i].X > (((BLOCK_WIDTH+1) * totalColumns) - BLOCK_WIDTH) || 
                        rectangles[i].X <  0 || 
                        rectangles[i].Y > (((BLOCK_WIDTH+1) * totalRows) - BLOCK_WIDTH))
                {
                    return false;
                }
            }

            //check for other blocks
            for (int i = 0; i < rectangles.Length; ++i)
            {
                int x = (rectangles[i].X / (BLOCK_WIDTH+1));
                int y = (rectangles[i].Y / (BLOCK_WIDTH+1)); // grid coordinates

                if (!board.isSpotEmpty(x, y))
                {
                    return false;
                }
            }

            return true;
        }

        public bool moveBlock(int x, int y)
        {
            // Move block
            Rectangle[] tempRects = new Rectangle[shapeRects.Length];
            Array.Copy(shapeRects, tempRects, shapeRects.Length);

            for (int i = 0; i < tempRects.Length; ++i)
            {   
                tempRects[i].X += ((BLOCK_WIDTH+1) * x);
                tempRects[i].Y += ((BLOCK_WIDTH+1) * y);
            }
            if (isValidMove(tempRects))
            {
                shapeRects = tempRects;
                blockColumn += x;
                blockRow += y;
                return true;
            }
            return false;
        }

        public void rotate()
        {
            int shapeLength = shapeBool.GetLength(0);
            bool[,] tempShape = new bool[shapeLength, shapeLength];

            //rotate shape matrix
            for (int i=shapeLength-1; i>=0; --i)
            {
                for(int j=0; j<=shapeLength-1; ++j)
                {
                    tempShape[j,(shapeLength-1) -i] = shapeBool[i, j];
                }
            }

            Rectangle[] tempRects = getRectangles(tempShape);

            if (isValidMove(tempRects))
            {
                shapeBool = tempShape;
                shapeRects = tempRects;
            }
        }
    }
}

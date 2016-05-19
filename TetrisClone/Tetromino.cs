using System;
using System.Drawing;
using System.Windows.Forms;

namespace TetrisClone
{
    class Tetromino
    {
        private static readonly bool[,] S_BLOCK = {
            { false, true, true },
            { true,  true, false} };

        private static readonly bool[,] Z_BLOCK = {
            {true,  true,  false },
            {false, true,  true  } };

        private static readonly bool[,] T_BLOCK = {
            {false, true,  false },
            {true,  true,  true  } };

        private static readonly bool[,] O_BLOCK = {
            {true,  true },
            {true,  true } };

        private static readonly bool[,] I_BLOCK = {
            {true,  true,  true,  true },
            {false, false, false, false} };

        private static readonly bool[,] J_BLOCK = {
            {true, false,  false },
            {true,  true,  true  } };

        private static readonly bool[,] L_BLOCK = {
            {false, false, true  },
            {true,  true,  true  } };

        private static readonly bool[][,] BLOCK_TYPES = {S_BLOCK, Z_BLOCK, T_BLOCK,
            O_BLOCK, I_BLOCK, J_BLOCK, L_BLOCK };

        private static readonly Color[] BLOCK_COLORS = {Color.Green, Color.Red, Color.Purple,
                                                    Color.Yellow, Color.Cyan, Color.Blue, Color.Orange };

        private static readonly int BLOCK_WIDTH = 20;

        public Rectangle[] shapeRects { get; set; }
        public Color color { get; }
        private Board board;
        private int blockType, blockRow, blockColumn, totalColumns;
        private bool[,] shape;

        public Tetromino(Board b)
        {
            board = b;
            totalColumns = board.columns;
            Random rand = new Random();
            blockType = rand.Next(0, 7); // 0 to 6
            shape = BLOCK_TYPES[blockType];
            color = BLOCK_COLORS[blockType];
            shapeRects = new Rectangle[4];

            int rectNumber = 0; //rectangle array index
            int startX = (BLOCK_WIDTH*4) + 5; //4 columns in and 5 column borders
            blockRow = 0;
            blockColumn = 5 - (shape.GetLength(0)/2);

            for(int i=0; i < shape.GetLength(0); ++i) //rows
            {
                for(int j=0; j<shape.GetLength(1); ++j) //columns
                {
                    if (shape[i, j])
                    {
                        shapeRects[rectNumber] = new Rectangle((blockColumn * (BLOCK_WIDTH + 1)) + ((BLOCK_WIDTH + 1) * j) +1, //x
                                                               (blockRow * (BLOCK_WIDTH + 1)) + ((BLOCK_WIDTH + 1) * i) +1, //y 
                                                                BLOCK_WIDTH, BLOCK_WIDTH);
                        ++rectNumber;
                    }
                }
            }
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

        public bool moveDown()
        {
            // Check for bottom of playing area
            for (int i = 0; i < shapeRects.Length; ++i){
                if(shapeRects[i].Y + 21 >= 443)
                {
                    return false;
                }
            }

            // Check for other blocks
            for (int i = 0; i < shapeRects.Length; ++i)
            {
                int x = (shapeRects[i].X / 21); 
                int y = (shapeRects[i].Y / 21) + 1; // grid coordinates

                if (!board.isSpotEmpty(x, y))
                {
                    return false;
                }
            }

            // Move block
            for (int i=0; i<shapeRects.Length; ++i){
                shapeRects[i].Y += 21;
            }
            blockRow += 1;
            return true;
        }

        public void moveRight()
        {
            // Check for right of playing area
            for (int i = 0; i < shapeRects.Length; ++i)
            {
                if (shapeRects[i].X + 21 >= 191)
                {
                    return;
                }
            }

            // Check for other blocks
            for (int i = 0; i < shapeRects.Length; ++i)
            {
                int x = (shapeRects[i].X / 21) +1;
                int y = (shapeRects[i].Y / 21); // grid coordinates

                if (!board.isSpotEmpty(x, y))
                {
                    return;
                }
            }

            // Move block
            for (int i = 0; i < shapeRects.Length; ++i)
            {
                
                shapeRects[i].X += 21;
            }
            blockColumn += 1;
        }

        public void moveLeft()
        {
            // Check for left of playing area
            for (int i = 0; i < shapeRects.Length; ++i)
            {
                if (shapeRects[i].X - 21 <= 0)
                {
                    return;
                }
            }

            // Check for other blocks
            for (int i = 0; i < shapeRects.Length; ++i)
            {
                int x = (shapeRects[i].X / 21) - 1;
                int y = (shapeRects[i].Y / 21); // grid coordinates

                if (!board.isSpotEmpty(x, y))
                {
                    return;
                }
            }

            // Move block
            for (int i = 0; i < shapeRects.Length; ++i)
            {
                
                shapeRects[i].X -= 21;
            }
            blockColumn -= 1;
        }

        public void rotate()
        {
            Rectangle[] tempRects = new Rectangle[4];
            bool[,] tempShape = new bool[shape.GetLength(1),shape.GetLength(0)];

            //rotate shape matrix
            int tempCol, tempRow = 0;
            for(int i=shape.GetLength(1)-1; i>=0; --i)
            {
                tempCol = 0;
                for(int j=0; j< shape.GetLength(0); ++j)
                {
                    tempShape[tempRow, tempCol] = shape[j, i];
                    tempCol++;
                }
                tempRow++;
            }

            int rectNumber = 0;
            for (int i = 0; i < tempShape.GetLength(0); ++i) //rows
            {
                for (int j = 0; j < tempShape.GetLength(1); ++j) //columns
                {
                    if (tempShape[i, j])
                    {
                        tempRects[rectNumber] = new Rectangle((blockColumn * (BLOCK_WIDTH + 1)) + ((BLOCK_WIDTH + 1) * j) + 1, //x
                                                              (blockRow * (BLOCK_WIDTH + 1)) + ((BLOCK_WIDTH + 1) * i) + 1, //y 
                                                                BLOCK_WIDTH, BLOCK_WIDTH);
                        ++rectNumber;
                    }
                }
            }

            for (int i = 0; i < tempRects.Length; ++i)
            {
                //check boundaries
                if (tempRects[i].X < 0 || tempRects[i].X >= 191 || tempRects[i].Y >= 443)
                {
                    return;
                }
                // check collisions
                int x = tempRects[i].X / 21;
                int y = tempRects[i].Y / 21;
                if (!board.isSpotEmpty(x, y))
                {
                    return;
                }
            }
            // move is valid, copy new rotation
            shape = tempShape;
            shapeRects = tempRects;
        }
    }
}

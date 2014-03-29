using System;


namespace AssemblyCSharp
{
    public struct BoundingBox
    {
        public int Left, Right, Top, Bottom;
        public BoundingBox(int left, int right, int top, int bottom)
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
        }

        public override string ToString()
        {
            return string.Format("Left: {0}; Right: {1}; Top: {2}; Bottom: {3}", Left, Right, Top, Bottom);
        }
    }

    public class Block
    {
        public static System.Random r = new System.Random(DateTime.Today.Millisecond);
        public const int MAX_COLORS = 4;
        public const int BLOCK_CENTER_X = 1;
        public const int BLOCK_CENTER_Y = 1;
        public const int BLOCK_SIZE = 4;

        private const string
            cI1 = "0000" + "1111" + "0000" + "0000",
            cI2 = "0100" + "0100" + "0100" + "0100",
            cT1 = "0000" + "1110" + "0100" + "0000",
            cT2 = "0100" + "1100" + "0100" + "0000",
            cT3 = "0100" + "1110" + "0000" + "0000",
            cT4 = "0100" + "0110" + "0100" + "0000",
            cL1 = "0000" + "1110" + "1000" + "0000",
            cL2 = "1100" + "0100" + "0100" + "0000",
            cL3 = "0010" + "1110" + "0000" + "0000",
            cL4 = "0100" + "0100" + "0110" + "0000",
            cJ1 = "1000" + "1110" + "0000" + "0000",
            cJ2 = "0110" + "0100" + "0100" + "0000",
            cJ3 = "0000" + "1110" + "0010" + "0000",
            cJ4 = "0100" + "0100" + "1100" + "0000",
            cZ1 = "0000" + "1100" + "0110" + "0000",
            cZ2 = "0010" + "0110" + "0100" + "0000",
            cS1 = "0000" + "0110" + "1100" + "0000",
            cS2 = "0100" + "0110" + "0010" + "0000",
            cO1 = "0110" + "0110" + "0000" + "0000";

        //Индекс вращения, столбец, строка
        public int[, ,] Matrices;
        public int ColorIndex;
        public int X, Y;
        public int RotateIndex;
        public Direction MoveDirection;
        public BoundingBox[] BlockBounds;

        private void FillMatrixFromString(int rotIndex, string aString)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Matrices[rotIndex, i, j] = int.Parse(aString[4 * j + i].ToString()) * ColorIndex;

        }

        private void CalcBounds()
        {
            BlockBounds = new BoundingBox[Matrices.GetLength(0)];
            for (int rot = 0; rot < Matrices.GetLength(0); rot++)
            {
                //top
                BlockBounds[rot].Top = -1;
                for (int j = 0; j < BLOCK_SIZE; j++)
                {
                    for (int i = 0; i < BLOCK_SIZE; i++)
                    {
                        if (Matrices[rot, i, j] != 0)
                        {
                            BlockBounds[rot].Top = j;
                            break;
                        }
                    }
                    if (BlockBounds[rot].Top != -1)
                        break;
                }

                //bottom
                BlockBounds[rot].Bottom = -1;
                for (int j = BLOCK_SIZE - 1; j >= 0; j--)
                {
                    for (int i = 0; i < BLOCK_SIZE; i++)
                    {
                        if (Matrices[rot, i, j] != 0)
                        {
                            BlockBounds[rot].Bottom = j;
                            break;
                        }
                    }
                    if (BlockBounds[rot].Bottom != -1)
                        break;
                }

                //left
                BlockBounds[rot].Left = -1;
                for (int i = 0; i < BLOCK_SIZE; i++)
                {
                    for (int j = 0; j < BLOCK_SIZE; j++)
                    {
                        if (Matrices[rot, i, j] != 0)
                        {
                            BlockBounds[rot].Left = i;
                            break;
                        }
                    }
                    if (BlockBounds[rot].Left != -1)
                        break;
                }

                //right
                BlockBounds[rot].Right = -1;
                for (int i = BLOCK_SIZE - 1; i >= 0; i--)
                {
                    for (int j = 0; j < BLOCK_SIZE; j++)
                    {
                        if (Matrices[rot, i, j] != 0)
                        {
                            BlockBounds[rot].Right = i;
                            break;
                        }
                    }
                    if (BlockBounds[rot].Right != -1)
                        break;
                }
            }
        }

        public static Block CreateRandomBlock()
        {
            Block block = new Block();
            int index = r.Next(7);
            block.ColorIndex = 1 + r.Next(MAX_COLORS);
            switch (index)
            {
                case 0:
                    //I
                    block.Matrices = new int[2, 4, 4];
                    block.FillMatrixFromString(0, cI1);
                    block.FillMatrixFromString(1, cI2);
                    break;
                case 1:
                    //T
                    block.Matrices = new int[4, 4, 4];
                    block.FillMatrixFromString(0, cT1);
                    block.FillMatrixFromString(1, cT2);
                    block.FillMatrixFromString(2, cT3);
                    block.FillMatrixFromString(3, cT4);
                    break;
                case 2:
                    //L
                    block.Matrices = new int[4, 4, 4];
                    block.FillMatrixFromString(0, cL1);
                    block.FillMatrixFromString(1, cL2);
                    block.FillMatrixFromString(2, cL3);
                    block.FillMatrixFromString(3, cL4);
                    break;
                case 3:
                    //J
                    block.Matrices = new int[4, 4, 4];
                    block.FillMatrixFromString(0, cJ1);
                    block.FillMatrixFromString(1, cJ2);
                    block.FillMatrixFromString(2, cJ3);
                    block.FillMatrixFromString(3, cJ4);
                    break;
                case 4:
                    //Z
                    block.Matrices = new int[2, 4, 4];
                    block.FillMatrixFromString(0, cZ1);
                    block.FillMatrixFromString(1, cZ2);
                    break;
                case 5:
                    //S
                    block.Matrices = new int[2, 4, 4];
                    block.FillMatrixFromString(0, cS1);
                    block.FillMatrixFromString(1, cS2);
                    break;
                case 6:
                    //O
                    block.Matrices = new int[1, 4, 4];
                    block.FillMatrixFromString(0, cO1);
                    break;
            }
            block.CalcBounds();
            block.RotateIndex = r.Next(block.Matrices.GetLength(0));
            return block;
        }

        public void Rotate()
        {
            RotateIndex = GetNextRotation();
        }
        public int GetNextRotation()
        {
            if (RotateIndex == Matrices.GetLength(0) - 1)
                return 0;
            else
                return (RotateIndex + 1);
        }

        public void Move()
        {
            switch (MoveDirection)
            {
                case Direction.Right: X += 1; break;
                case Direction.Left: X -= 1; break;
                case Direction.Top: Y -= 1; break;
                case Direction.Bottom: Y += 1; break;
            }
        }
    }
}

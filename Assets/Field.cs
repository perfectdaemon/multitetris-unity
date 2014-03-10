using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public enum Direction { Right, Left, Top, Bottom };

    public delegate void OnGameOver();

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
	}

	public class Block
	{
		public const int MAX_COLORS = 4;
        public const int BLOCK_CENTER_X = 1;
        public const int BLOCK_CENTER_Y = 1;

		private const string 
			cI1 = "0000"+"1111"+"0000"+"0000",
			cI2 = "0100"+"0100"+"0100"+"0100",
			cT1 = "0000"+"1110"+"0100"+"0000",
			cT2 = "0100"+"1100"+"0100"+"0000",
			cT3 = "0100"+"1110"+"0000"+"0000",
			cT4 = "0100"+"0110"+"0100"+"0000",
			cL1 = "0000"+"1110"+"1000"+"0000",
			cL2 = "1100"+"0100"+"0100"+"0000",
			cL3 = "0010"+"1110"+"0000"+"0000",
			cL4 = "0100"+"0100"+"0110"+"0000",
			cJ1 = "1000"+"1110"+"0000"+"0000",
			cJ2 = "0110"+"0100"+"0100"+"0000",
			cJ3 = "0000"+"1110"+"0010"+"0000",
			cJ4 = "0100"+"0100"+"1100"+"0000",
			cZ1 = "0000"+"1100"+"0110"+"0000",
			cZ2 = "0010"+"0110"+"0100"+"0000",
			cS1 = "0000"+"0110"+"1100"+"0000",
			cS2 = "0100"+"0110"+"0010"+"0000",
			cO1 = "0110"+"0110"+"0000"+"0000";

		public int[,,] Matrices;
		public int ColorIndex;
		public int X, Y;
		public int RotateIndex;
		public Direction MoveDirection;
		public BoundingBox[] BlockBounds;

		private void FillMatrixFromString(int rotIndex, string aString)		
		{
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Matrices[rotIndex, i, j] = int.Parse(aString[4 * i + j].ToString()) * ColorIndex;
			
		}	
		
		private void CalcBounds()
		{
            BlockBounds = new BoundingBox[Matrices.GetLength(0)];
			for (int rot = 0; rot < Matrices.GetLength(0); rot++)
			{
				//top
				BlockBounds[rot].Top = -1;
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						if (Matrices[rot, i, j] != 0)
						{
							BlockBounds[rot].Top = i;
							break;
						}
					}
					if (BlockBounds[rot].Top != -1)
						break;
				}

				//bottom
				BlockBounds[rot].Bottom = -1;
				for (int i = 3; i >= 0; i--)
				{
					for (int j = 0; j < 4; j++)
					{
						if (Matrices[rot, i, j] != 0)
						{
							BlockBounds[rot].Bottom = i;
							break;
						}
					}
					if (BlockBounds[rot].Bottom != -1)
						break;
				}
				
				//left
				BlockBounds[rot].Left = -1;
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						if (Matrices[rot, j, i] != 0)
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
				for (int i = 3; i <= 0; i--)
				{
					for (int j = 0; j < 4; j++)
					{
						if (Matrices[rot, j, i] != 0)
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
			Block block = new Block ();
			System.Random r = new System.Random ();
			int index = r.Next (7);
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
            if (RotateIndex == Matrices.GetLength(0))
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

	public class Field
	{
        private float timeToMove;

		public const int FIELD_X = 24;
		public const int FIELD_Y = 24;
        public const int CLEAN_PERIOD_START = 8;
        public const int CLEAN_BLOCK_THRESHOLD = 6;
        public const float SPEED_START = 1.0f;
        public const float SPEED_INC = 0.5f;

        public Block CurrentBlock, NextBlock;

        public int[,] F = new int[FIELD_X, FIELD_Y];

        public int Scores = 0;

        public float CurrentSpeed, timeToClean;

        public int BeforeCleanCounter, CurrentCleanPeriod;

        public OnGameOver gameOver;

        private int CheckCell(int c, int r, int value)
        {
            if (IsInBounds(c, r) && F[c, r] == value)                
            {
                int result = 1;
                F[c, r] = value + 100;
                result += CheckCell(c - 1, r, value);
                result += CheckCell(c + 1, r, value);
                result += CheckCell(c, r + 1, value);
                result += CheckCell(c, r - 1, value);
                return result;
            }
            else
                return 0;

        }  

        private void MinusBlocksToClean(bool onlyRevert, ref float totalTime)
        {
            float p = totalTime;
            for (int i = 0; i < FIELD_X; i++)
                for (int j = 0; j < FIELD_Y; j++)
                    if (F[i, j] > 100)
                    {
                        F[i, j] = F[i, j] - 100;
                        if (!onlyRevert)
                        {
                            F[i, j] = - F[i, j];
                            p += 0.05f;
                            //todo: tweener add tween for current block with pause P
                        }
                    }
            totalTime = p;
        }

		void AddBlock(Direction origin)
		{
            //if (CurrentBlock != null)
            //    CurrentBlock = null;
            if (NextBlock == null)
                NextBlock = Block.CreateRandomBlock();

            CurrentBlock = NextBlock;
            NextBlock = Block.CreateRandomBlock();

            switch (origin)
            {
                case Direction.Right:
                    CurrentBlock.X = FIELD_X - CurrentBlock.BlockBounds[CurrentBlock.RotateIndex].Right - 1;
                    CurrentBlock.Y = FIELD_Y / 2 - Block.BLOCK_CENTER_Y;
                    CurrentBlock.MoveDirection = Direction.Left;
                    //todo:
                    //alphaHorLine = LINE_INACTIVE_ALPHA;
                    //alphaVertLine = LINE_ACTIVE_ALPHA;
                    //next dir - from top
                    //nextblockdir rotation
                    break;

                case Direction.Left:
                    CurrentBlock.X = - CurrentBlock.BlockBounds[CurrentBlock.RotateIndex].Left;
                    CurrentBlock.Y = FIELD_Y / 2 - Block.BLOCK_CENTER_Y;
                    CurrentBlock.MoveDirection = Direction.Right;
                    break;

                case Direction.Top:
                    CurrentBlock.X = FIELD_X / 2 - 1 - Block.BLOCK_CENTER_X;
                    CurrentBlock.Y = - CurrentBlock.BlockBounds[CurrentBlock.RotateIndex].Top;
                    CurrentBlock.MoveDirection = Direction.Bottom;
                    break;

                case Direction.Bottom:
                    CurrentBlock.X = FIELD_X / 2 - 1 - Block.BLOCK_CENTER_X;
                    CurrentBlock.Y = FIELD_Y - CurrentBlock.BlockBounds[CurrentBlock.RotateIndex].Bottom - 1;
                    CurrentBlock.MoveDirection = Direction.Top;
                    break;
            }
        }
		bool IsInBounds(int x, int y)
		{
			return (x >= 0 && x < FIELD_X && y >= 0 && y < FIELD_Y);
		}
		bool CouldBlockMove(Direction direction)
		{
            switch (direction)
            {
                case Direction.Right:
                    for (int i = 3; i >= 0; i--)
                        for (int j = 0; j < 4; j++)
                        {
                            if (CurrentBlock.Matrices[CurrentBlock.RotateIndex, j, i] == 0)
                                continue;
                            bool stop = false || F[i + CurrentBlock.X + 1, j + CurrentBlock.Y] > 10;
                            if (direction == CurrentBlock.MoveDirection)
                                stop = stop || (i + CurrentBlock.X + 1 > FIELD_X / 2 - 1);
                            else
                                stop = stop || (i + CurrentBlock.X + 1 > FIELD_X - 1);
                            if (stop)
                                return false;
                        }
                    break;

                case Direction.Left:
                    for (int i = 0; i < 4; i++)
                        for (int j = 0; j < 4; j++)
                        {
                            if (CurrentBlock.Matrices[CurrentBlock.RotateIndex, j, i] == 0)
                                continue;
                            bool stop = false || F[i + CurrentBlock.X - 1, j + CurrentBlock.Y] > 10;
                            if (direction == CurrentBlock.MoveDirection)
                                stop = stop || (i + CurrentBlock.X - 1 < FIELD_X / 2);
                            else
                                stop = stop || (i + CurrentBlock.X - 1 < 0);
                            if (stop)
                                return false;
                        }
                    break;

                case Direction.Top:
                    for (int j = 0; j < 4; j++)
                        for (int i = 0; i < 4; i++)
                        {
                            if (CurrentBlock.Matrices[CurrentBlock.RotateIndex, j, i] == 0)
                                continue;
                            bool stop = false || F[i + CurrentBlock.X, j + CurrentBlock.Y - 1] > 10;
                            if (direction == CurrentBlock.MoveDirection)
                                stop = stop || (j + CurrentBlock.Y - 1 < FIELD_Y / 2);
                            else
                                stop = stop || (j + CurrentBlock.Y - 1 < 0);
                            if (stop)
                                return false;
                        }
                    break;

                case Direction.Bottom:
                    for (int j = 3; j >= 0; j--)
                        for (int i = 0; i < 4; i++)
                        {
                            if (CurrentBlock.Matrices[CurrentBlock.RotateIndex, j, i] == 0)
                                continue;
                            bool stop = false || F[i + CurrentBlock.X, j + CurrentBlock.Y + 1] > 10;
                            if (direction == CurrentBlock.MoveDirection)
                                stop = stop || (j + CurrentBlock.Y + 1 > FIELD_Y / 2 - 1);
                            else
                                stop = stop || (j + CurrentBlock.Y + 1 > FIELD_Y - 1);
                            if (stop)
                                return false;
                        }
                    break;
            }

            return true;
		}

		bool CouldBlockRotate()
		{
            int nextRotation = CurrentBlock.GetNextRotation();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (CurrentBlock.Matrices[nextRotation, j, i] > 0 && F[CurrentBlock.X + i, CurrentBlock.Y + j] > 10)
                        return false;
            return true;
		}

		bool CouldBlockSet()
		{
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (F[CurrentBlock.X + i, CurrentBlock.Y + j] > 10 && CurrentBlock.Matrices[CurrentBlock.RotateIndex, j, i] > 0)
                        return false;
            return true;
		}

		void FindBlocksToClean()
		{
            int countAll = 0;
            float totalTime = 0;
            for (int i = 0; i < FIELD_X; i++)
                for (int j = 0; j < FIELD_Y; j++)                
                    if (F[i, j] > 0 && F[i, j] < 100)
                    {
                        int count = CheckCell(i, j, F[i, j]);
                        if (count >= CLEAN_BLOCK_THRESHOLD)
                        {
                            MinusBlocksToClean(false, ref totalTime);
                            countAll += count;
                        }
                        else
                            MinusBlocksToClean(true, ref totalTime);
                    }

            Scores += countAll;
            timeToClean = 1.0f + totalTime + 0.2f;  
        }

		Vector2 BlockPosToScreenPos(int x, int y)
		{
            throw new NotImplementedException("Метод не реализован");
		}		

		public void Redraw (float dt)
		{
            //erase all dynamic
            for (int i = 0; i < FIELD_X; i++)
                for (int j = 0; j < FIELD_Y; j++)
                    if (F[i, j] < 10 && F[i, j] > 0)
                        F[i, j] = 0;

            if (CurrentBlock != null)
                //"draw" current block
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                        if (IsInBounds(CurrentBlock.X + i, CurrentBlock.Y + j) && CurrentBlock.Matrices[CurrentBlock.RotateIndex, j, i] > 0)
                            //!!!! У Block-ов транспонированная матрица
                            //Поле - столбец, строка
                            //Блок - строка, столбец
                            F[CurrentBlock.X + i, CurrentBlock.Y + j] = CurrentBlock.Matrices[CurrentBlock.RotateIndex, j, i];

            //todo: раскраска спрайтов поля
            //todo: раскраска спрайта следующего блока
		}

		public void CleanBlocks()
		{
            //смещаем по вертикали, потом по горизонтали
            bool hasMove = true;
            while (hasMove)
            {
                hasMove = false;
                for (int j = 1; j < FIELD_Y / 2; j++)
                    for (int i = 0; i < FIELD_X; i++)
                    { 
                        //bottom
                        if (F[i, FIELD_Y / 2 + j] > 0 && F[i, FIELD_Y / 2 + j - 1] <= 0)
                        { 
                            F[i, (FIELD_Y / 2) + j - 1] = F[i, (FIELD_Y / 2) + j];
                            F[i, (FIELD_Y / 2) + j] = 0;
                            hasMove = true;
                        }
                        //Top
                        if (F[i, (FIELD_Y / 2) - j - 1] > 0 && F[i, (FIELD_Y / 2) - j]     <= 0) 
                        {
                            F[i, (FIELD_Y / 2) - j]     = F[i, (FIELD_Y / 2) - j - 1];
                            F[i, (FIELD_Y / 2) - j - 1] = 0;
                            hasMove = true;
                        }
                    }

                //reright this!!!!!
                //left block index 12
                //right block index 11
                for (int i = 1; i < FIELD_X / 2; i++)
                    for (int j = 0; j < FIELD_Y; j++)
                    {
                        //Right
                        if (F[(FIELD_X / 2) + i, j] > 0 && F[(FIELD_X / 2) + i - 1, j] <= 0)
                        {
                            F[(FIELD_X / 2) + i - 1, j] = F[(FIELD_X / 2) + i, j];
                            F[(FIELD_X / 2) + i,     j] = 0;
                            hasMove = true;
                        }

                        //Left
                        if (F[(FIELD_X / 2) - i - 1, j] > 0 && F[(FIELD_X / 2) - i, j] <= 0)
                        {
                            F[(FIELD_X / 2) - i,     j] = F[(FIELD_X / 2) - i - 1, j];
                            F[(FIELD_X / 2) - i - 1, j] = 0;
                            hasMove = true;
                        }
                    }                
            }

            //удаляем все следы
            for (int i = 0; i < FIELD_X; i++)
                for (int j = 0; j < FIELD_Y; j++)
                    if (F[i, j] < 0)
                        F[i, j] = 0;
		}

		public void AddNextBlock()
		{
            switch (CurrentBlock.MoveDirection)
            {
                case Direction.Right:
                    AddBlock(Direction.Bottom);
                    break;
                case Direction.Left:
                    AddBlock(Direction.Top);
                    break;
                case Direction.Top:
                    AddBlock(Direction.Right);
                    break;
                case Direction.Bottom:
                    AddBlock(Direction.Left);
                    break;
            }
            if (CouldBlockSet())
                timeToMove = 1.0f / CurrentSpeed;
            else
                gameOver();               
		}

        private void BlockSet()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (IsInBounds(CurrentBlock.X + i, CurrentBlock.Y + j) && CurrentBlock.Matrices[CurrentBlock.RotateIndex, j, i] > 0)
                        F[CurrentBlock.X + i, CurrentBlock.Y + j] = CurrentBlock.Matrices[CurrentBlock.RotateIndex, j, i] + 10;
        }  

		public void MoveCurrentBlock (float dt)
		{
            if (CurrentBlock == null)
                return;
            if (CouldBlockMove(CurrentBlock.MoveDirection))
                CurrentBlock.Move();
            else
            {
                BlockSet();
                //todo: sound - play sample
                //todo: particles - block set
                BeforeCleanCounter -= 1;
                if (BeforeCleanCounter == 0)
                {
                    FindBlocksToClean();
                    CurrentCleanPeriod++;
                    BeforeCleanCounter = CurrentCleanPeriod;
                    CurrentSpeed += SPEED_INC;
                }
                else
                    AddNextBlock();
            }  
		}

		public Field ()
		{
            //todo: создание спрайтов
            
            //Start the game!
            CurrentSpeed = SPEED_START;
            CurrentCleanPeriod = CLEAN_PERIOD_START;
            timeToMove = 1.0f / CurrentSpeed;
            //timeToClean = 0;
            Scores = 0;
            CurrentBlock = null;
            AddBlock(Direction.Top);
            BeforeCleanCounter = CurrentCleanPeriod;
		}

        private void PlayerControl(float dt)
        { 

        }

        public void Update(float dt)
        {
            if (timeToClean > 0)
            {
                timeToClean -= dt;
                if (timeToClean <= 0)
                {
                    CleanBlocks();
                    AddNextBlock();
                }
            }

            else if (CurrentBlock != null)
            {
                PlayerControl(dt);
                Redraw(dt);
                timeToMove -= dt;
                if (timeToMove < 0)
                {
                    MoveCurrentBlock(dt);
                    timeToMove = 1 / CurrentSpeed;
                }
            }
        }
	}
}
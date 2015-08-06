using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public enum Direction { Right, Left, Top, Bottom };

    public delegate void OnGameOver();

    public class Field : MonoBehaviour
    {
        private float timeToMove;

        public Transform FieldTopLeft;
        public GameObject blockPrefab;
        public Material unused;
        public Material[] used;

        public const int FIELD_X = 24;
        public const int FIELD_Y = 24;
        public const int CLEAN_PERIOD_START = 8;
        public const int CLEAN_BLOCK_THRESHOLD = 6;
        public const float SPEED_START = 1.0f;
        public const float SPEED_INC = 0.5f;

        public const float RENDER_BLOCK_SIZE = 0.35f;
        public Block CurrentBlock, NextBlock;

        public int[,] F = new int[FIELD_X, FIELD_Y];
        public GameObject[,] Sprites = new GameObject[FIELD_X, FIELD_Y];

        public int Scores = 0;

        public float CurrentSpeed, timeToClean;

        public int BeforeCleanCounter, CurrentCleanPeriod;

        public OnGameOver gameOver;

        private int GetF(int x, int y)
        {
            if (x >= this.F.GetLength(0) || y >= this.F.GetLength(1) || x < 0 || y < 0)
            {
                print(string.Format("Попытка получить доступ к {0}, {1}. Максимум: {2}, {3}", x, y, this.F.GetLength(0), this.F.GetLength(1)));
                return 0;
            }
            else
                return this.F[x, y];
        }

        private void SetF(int x, int y, int v)
        {
            if (x >= this.F.GetLength(0) || y >= this.F.GetLength(1) || x < 0 || y < 0)
                print(string.Format("Попытка получить доступ к {0}, {1}. Максимум: {2}, {3}", x, y, this.F.GetLength(0), this.F.GetLength(1)));
            else
                this.F[x, y] = v;
        }

        private int CheckCell(int c, int r, int value)
        {
            if (IsInBounds(c, r) && GetF(c, r) == value)
            {
                int result = 1;
                SetF(c, r, value + 100);
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
                            F[i, j] = -F[i, j];
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

            BoundingBox b = CurrentBlock.BlockBounds[CurrentBlock.RotateIndex];
            switch (origin)
            {
                case Direction.Right:
                    CurrentBlock.X = FIELD_X - 1 - b.Right;
                    CurrentBlock.Y = FIELD_Y / 2 - Block.BLOCK_CENTER_Y;
                    CurrentBlock.MoveDirection = Direction.Left;
                    //todo:
                    //alphaHorLine = LINE_INACTIVE_ALPHA;
                    //alphaVertLine = LINE_ACTIVE_ALPHA;
                    //next dir - from top
                    //nextblockdir rotation
                    break;

                case Direction.Left:
                    CurrentBlock.X = -CurrentBlock.BlockBounds[CurrentBlock.RotateIndex].Left;
                    CurrentBlock.Y = FIELD_Y / 2 - Block.BLOCK_CENTER_Y;
                    CurrentBlock.MoveDirection = Direction.Right;
                    break;

                case Direction.Top:
                    CurrentBlock.X = FIELD_X / 2 - 1 - Block.BLOCK_CENTER_X;
                    CurrentBlock.Y = -CurrentBlock.BlockBounds[CurrentBlock.RotateIndex].Top;
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
            BoundingBox b = CurrentBlock.BlockBounds[CurrentBlock.RotateIndex];
            int x = CurrentBlock.X;
            int y = CurrentBlock.Y;
            int rot = CurrentBlock.RotateIndex;
            switch (direction)
            {
                case Direction.Right:
                    for (int i = Block.BLOCK_SIZE - 1; i >= 0; i--)
                        for (int j = 0; j < Block.BLOCK_SIZE; j++)
                        {
                            if (CurrentBlock.Matrices[rot, i, j] == 0)
                                continue;
                            bool stop = false || GetF(i + x + 1, j + y) > 10;
                            if (direction == CurrentBlock.MoveDirection)
                                stop = stop || (i + x + 1 > FIELD_X / 2 - 1);
                            else
                                stop = stop || (i + x + 1 > FIELD_X - 1);
                            if (stop)
                                return false;
                        }
                    break;

                case Direction.Left:
                    for (int i = 0; i < Block.BLOCK_SIZE; i++)
                        for (int j = 0; j < Block.BLOCK_SIZE; j++)
                        {
                            if (CurrentBlock.Matrices[rot, i, j] == 0)
                                continue;
                            bool stop = false || GetF(i + x - 1, j + y) > 10;
                            if (direction == CurrentBlock.MoveDirection)
                                stop = stop || (i + x - 1 < FIELD_X / 2);
                            else
                                stop = stop || (i + x - 1 < 0);
                            if (stop)
                                return false;
                        }
                    break;

                case Direction.Top:
                    for (int j = 0; j < Block.BLOCK_SIZE; j++)
                        for (int i = 0; i < Block.BLOCK_SIZE; i++)
                        {
                            if (CurrentBlock.Matrices[rot, i, j] == 0)
                                continue;
                            bool stop = false || GetF(i + x, j + y - 1) > 10;
                            if (direction == CurrentBlock.MoveDirection)
                                stop = stop || (j + y - 1 < FIELD_Y / 2);
                            else
                                stop = stop || (j + y - 1 < 0);
                            if (stop)
                                return false;
                        }
                    break;

                case Direction.Bottom:
                    for (int j = Block.BLOCK_SIZE - 1; j >= 0; j--)
                        for (int i = 0; i < Block.BLOCK_SIZE; i++)
                        {
                            if (CurrentBlock.Matrices[rot, i, j] == 0)
                                continue;
                            bool stop = false || GetF(i + x, j + y + 1) > 10;
                            if (direction == CurrentBlock.MoveDirection)
                                stop = stop || (j + y + 1 > FIELD_Y / 2 - 1);
                            else
                                stop = stop || (j + y + 1 > FIELD_Y - 1);
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
            for (int i = 0; i < Block.BLOCK_SIZE; i++)
                for (int j = 0; j < Block.BLOCK_SIZE; j++)
                    if (CurrentBlock.Matrices[nextRotation, i, j] > 0 && GetF(CurrentBlock.X + i, CurrentBlock.Y + j) > 10)
                        return false;
            return true;
        }

        bool CouldBlockSet()
        {
            BoundingBox b = CurrentBlock.BlockBounds[CurrentBlock.RotateIndex];
            for (int i = 0; i < Block.BLOCK_SIZE; i++)
                for (int j = 0; j < Block.BLOCK_SIZE; j++)
                    if (GetF(CurrentBlock.X + i, CurrentBlock.Y + j) > 10 && CurrentBlock.Matrices[CurrentBlock.RotateIndex, i, j] > 0)
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

        public void Redraw(float dt)
        {
            //erase all dynamic
            for (int i = 0; i < FIELD_X; i++)
                for (int j = 0; j < FIELD_Y; j++)
                    if (F[i, j] < 10 && F[i, j] > 0)
                        F[i, j] = 0;

            if (CurrentBlock != null)
                //"draw" current block
                for (int i = 0; i < Block.BLOCK_SIZE; i++)
                    for (int j = 0; j < Block.BLOCK_SIZE; j++)
                        if (IsInBounds(CurrentBlock.X + i, CurrentBlock.Y + j) && CurrentBlock.Matrices[CurrentBlock.RotateIndex, i, j] > 0)
                            SetF(CurrentBlock.X + i, CurrentBlock.Y + j, CurrentBlock.Matrices[CurrentBlock.RotateIndex, i, j]);

            //todo: раскраска спрайтов поля
            for (int i = 0; i < FIELD_X; i++)
                for (int j = 0; j < FIELD_Y; j++)
                    if (F[i, j] == 0)
                    {
                        Sprites[i, j].renderer.material = unused;
                    }
                    else if (F[i, j] > 0)
                        Sprites[i, j].renderer.material = used[F[i, j] % 10];

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
                        if (F[i, (FIELD_Y / 2) - j - 1] > 0 && F[i, (FIELD_Y / 2) - j] <= 0)
                        {
                            F[i, (FIELD_Y / 2) - j] = F[i, (FIELD_Y / 2) - j - 1];
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
                            F[(FIELD_X / 2) + i, j] = 0;
                            hasMove = true;
                        }

                        //Left
                        if (F[(FIELD_X / 2) - i - 1, j] > 0 && F[(FIELD_X / 2) - i, j] <= 0)
                        {
                            F[(FIELD_X / 2) - i, j] = F[(FIELD_X / 2) - i - 1, j];
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

            print(CurrentBlock.BlockBounds[CurrentBlock.RotateIndex]);

            //AddBlock(Direction.Bottom);
            if (CouldBlockSet())
                timeToMove = 1.0f / CurrentSpeed;
            else
                gameOver();
        }

        private void BlockSet()
        {
            for (int i = 0; i < Block.BLOCK_SIZE; i++)
                for (int j = 0; j < Block.BLOCK_SIZE; j++)
                    if (IsInBounds(CurrentBlock.X + i, CurrentBlock.Y + j) && CurrentBlock.Matrices[CurrentBlock.RotateIndex, i, j] > 0)
                        SetF(CurrentBlock.X + i, CurrentBlock.Y + j, CurrentBlock.Matrices[CurrentBlock.RotateIndex, i, j] + 10);
        }

        public void MoveCurrentBlock(float dt)
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

        public void Start()
        {

            Vector2 startPos = new Vector2(FieldTopLeft.position.x, FieldTopLeft.position.y);
            for (int i = 0; i < FIELD_X; i++)
                for (int j = 0; j < FIELD_Y; j++)
                {
                    Sprites[i, j] = (GameObject)Instantiate(blockPrefab, new Vector3(startPos.x + i * 0.35f, startPos.y - j * 0.35f, 0), Quaternion.identity);
                }


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
            if (CurrentBlock == null)
                return;

            //receive input info
            int vert = 0, hor = 0;
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                vert = -1;
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                vert = 1;
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                hor = -1;
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                hor = 1;
            bool rotate = Input.GetKeyDown(KeyCode.Space);

            //process info                    
            if (hor > 0 && CurrentBlock.MoveDirection != Direction.Left && CouldBlockMove(Direction.Right))
                CurrentBlock.X += 1;
            else if (hor < 0 && CurrentBlock.MoveDirection != Direction.Right && CouldBlockMove(Direction.Left))
                CurrentBlock.X -= 1;

            if (vert > 0 && CurrentBlock.MoveDirection != Direction.Top && CouldBlockMove(Direction.Bottom))
                CurrentBlock.Y += 1;
            else if (vert < 0 && CurrentBlock.MoveDirection != Direction.Bottom && CouldBlockMove(Direction.Top))
                CurrentBlock.Y -= 1;

            if (rotate)
                CurrentBlock.Rotate();


            /*
            if (hor != 0)
            {
                if (CurrentBlock.MoveDirection == Direction.Bottom || CurrentBlock.MoveDirection == Direction.Top)
                    CurrentBlock.X += Mathf.CeilToInt(hor);
            }
            else if (vert != 0)
            {
                if (CurrentBlock.MoveDirection == Direction.Left || CurrentBlock.MoveDirection == Direction.Right)
                    CurrentBlock.Y += Mathf.CeilToInt(vert);
            }
             */
        }

        public void DoUpdate(float dt)
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

        public void Update()
        {
            this.DoUpdate(Time.deltaTime);
        }
    }
}
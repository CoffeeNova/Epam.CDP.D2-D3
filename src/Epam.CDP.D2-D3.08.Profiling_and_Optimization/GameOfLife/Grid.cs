using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
    class Grid
    {
        private readonly int _sizeX;
        private readonly int _sizeY;
        private readonly Cell[,] _cells;
        private readonly Cell[,] _nextGenerationCells;
        private static Random _rnd;
        private readonly Canvas _drawCanvas;
        private readonly Ellipse[,] _cellsVisuals;
        private const double CellSideSize = 5d;

        public Grid(Canvas c)
        {
            _drawCanvas = c;
            _rnd = new Random();
            _sizeX = (int)(c.Width / CellSideSize);
            _sizeY = (int)(c.Height / CellSideSize);
            _cells = new Cell[_sizeX, _sizeY];
            _nextGenerationCells = new Cell[_sizeX, _sizeY];
            _cellsVisuals = new Ellipse[_sizeX, _sizeY];

            for (int i = 0; i < _sizeX; i++)
                for (int j = 0; j < _sizeY; j++)
                {
                    _cells[i, j] = new Cell(i, j, 0, false);
                    _nextGenerationCells[i, j] = new Cell(i, j, 0, false);
                }

            SetRandomPattern();
            InitCellsVisuals();
            UpdateGraphics();
        }

        public void Clear()
        {
            for (int i = 0; i < _sizeX; i++)
                for (int j = 0; j < _sizeY; j++)
                {
                    _cells[i, j].IsAlive = false;
                    _nextGenerationCells[i, j].IsAlive = false; // OPTIMIZED
                    _cellsVisuals[i, j].Fill = Brushes.Gray;
                }
        }

        void MouseMove(object sender, MouseEventArgs e) // OPTIMIZED A BIT
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Ellipse cellVisual)
                {
                    var i = (int)(cellVisual.Margin.Left / CellSideSize);
                    var j = (int)(cellVisual.Margin.Top / CellSideSize);

                    var cell = _cells[i, j];
                    if (!cell.IsAlive)
                    {
                        cell.IsAlive = true;
                        cell.Age = 0;
                        cellVisual.Fill = Brushes.White;
                    }
                }
            }
        }

        public void UpdateGraphics()
        {
            for (int i = 0; i < _sizeX; i++)
                for (int j = 0; j < _sizeY; j++)
                {
                    var cell = _cells[i, j];
                    _cellsVisuals[i, j].Fill = cell.IsAlive
                        ? (cell.Age < 2 ? Brushes.White : Brushes.DarkGray)
                        : Brushes.Gray;
                }
        }

        public void UpdateCellGraphics(int i, int j)
        {
            var cell = _cells[i, j];
            _cellsVisuals[i, j].Fill = cell.IsAlive
                                          ? (cell.Age < 2 ? Brushes.White : Brushes.DarkGray)
                                          : Brushes.Gray;
        }

        public void InitCellsVisuals()
        {
            for (var i = 0; i < _sizeX; i++)
                for (var j = 0; j < _sizeY; j++)
                {
                    var left = _cells[i, j].PositionX;
                    var top = _cells[i, j].PositionY;

                    var ellipse = new Ellipse
                    {
                        Width = 5,
                        Height = 5,
                        Margin = new Thickness(left, top, 0, 0),
                        Fill = Brushes.Gray
                    };
                    ellipse.MouseMove += MouseMove;
                    ellipse.MouseLeftButtonDown += MouseMove;
                    _drawCanvas.Children.Add(ellipse);
                    _cellsVisuals[i, j] = ellipse;
                    UpdateCellGraphics(i, j); // OPTIMIZED
                }
        }


        public static bool GetRandomBoolean()
        {
            return _rnd.NextDouble() > 0.8;
        }

        public void SetRandomPattern()
        {
            for (int i = 0; i < _sizeX; i++)
                for (int j = 0; j < _sizeY; j++)
                    _cells[i, j].IsAlive = GetRandomBoolean();
        }

        public void UpdateToNextGeneration()
        {
            for (int i = 0; i < _sizeX; i++)
                for (int j = 0; j < _sizeY; j++)
                {
                    var cell = _cells[i, j];
                    var nextGenerationCell = _nextGenerationCells[i, j];
                    cell.IsAlive = nextGenerationCell.IsAlive;
                    cell.Age = nextGenerationCell.Age;
                }
            UpdateGraphics();
        }

        public void UpdateCellToNextGeneration(int i, int j)
        {
            var cell = _cells[i, j];
            var nextGenerationCell = _nextGenerationCells[i, j];
            cell.IsAlive = nextGenerationCell.IsAlive;
            cell.Age = nextGenerationCell.Age;
        }

        public void Update()
        {
            bool alive = false;
            int age = 0;

            for (int i = 0; i < _sizeX; i++)
            {
                for (int j = 0; j < _sizeY; j++)
                {
                    //                    nextGenerationCells[i, j] = CalculateNextGeneration(i,j);          // UNOPTIMIZED
                    CalculateNextGeneration(i, j, ref alive, ref age);   // OPTIMIZED
                    _nextGenerationCells[i, j].IsAlive = alive;  // OPTIMIZED
                    _nextGenerationCells[i, j].Age = age;  // OPTIMIZED
                    UpdateCellToNextGeneration(i, j); // OPTIMIZED
                    UpdateCellGraphics(i, j);
                }
            }
            //UpdateToNextGeneration();// UNOPTIMIZED
        }

        public Cell CalculateNextGeneration(int row, int column)    // UNOPTIMIZED
        {
            bool alive;
            int count, age;

            alive = _cells[row, column].IsAlive;
            age = _cells[row, column].Age;
            count = CountNeighbors(row, column);

            if (alive && count < 2)
                return new Cell(row, column, 0, false);

            if (alive && (count == 2 || count == 3))
            {
                _cells[row, column].Age++;
                return new Cell(row, column, _cells[row, column].Age, true);
            }

            if (alive && count > 3)
                return new Cell(row, column, 0, false);

            if (!alive && count == 3)
                return new Cell(row, column, 0, true);

            return new Cell(row, column, 0, false);
        }

        public void CalculateNextGeneration(int row, int column, ref bool isAlive, ref int age)     // OPTIMIZED
        {
            var count = CountNeighbors(row, column);
            var cell = _cells[row, column];
            isAlive = cell.IsAlive;
            age = cell.Age;

            if (isAlive)
                switch (count)
                {
                    case 2:
                    case 3:
                        isAlive = true;
                        cell.Age++;
                        age = cell.Age;
                        return;
                    default:
                        isAlive = false;
                        age = 0;
                        return;
                }

            if (count == 3)
            {
                isAlive = true;
                age = 0;
            }
        }

        public int CountNeighbors(int i, int j)
        {
            var count = 0;

            NeighborsAliveRules.ForEach(x =>
            {
                if (x(i, j))
                    count++;
            });
            return count;
        }

        private int MaxX => _sizeX - 1;
        private int MaxY => _sizeY - 1;

        private bool IsEastNeighborAlive(int i, int j)
        {
            return i != MaxX && _cells[i + 1, j].IsAlive;
        }

        private bool IsSoutheastNeighborAlive(int i, int j)
        {
            return i != MaxX && j != MaxY && _cells[i+1, j+1].IsAlive;
        }

        private bool IsSouthNeighborAlive(int i, int j)
        {
            return j != MaxY && _cells[i, j + 1].IsAlive;
        }

        private bool IsSouthwestNeighborAlive(int i, int j)
        {
            return i != 0 && j != MaxY && _cells[i - 1, j + 1].IsAlive;
        }

        private bool IsWestNeighborAlive(int i, int j)
        {
            return i != 0 && _cells[i - 1, j].IsAlive;
        }

        private bool IsNorthwestNeighborAlive(int i, int j)
        {
            return i != 0 && j != 0 && _cells[i - 1, j - 1].IsAlive;
        }

        private bool IsNorthNeighborAlive(int i, int j)
        {
            return j != 0 && _cells[i, j - 1].IsAlive;
        }

        private bool IsNortheastNeighborAlive(int i, int j)
        {
            return i != MaxX && j != 0 && _cells[i + 1, j - 1].IsAlive;
        }

        private List<Func<int, int, bool>> NeighborsAliveRules => 
            new List<Func<int, int, bool>>
            {
                IsEastNeighborAlive,
                IsSoutheastNeighborAlive,
                IsSouthNeighborAlive,
                IsSouthwestNeighborAlive,
                IsWestNeighborAlive,
                IsNorthwestNeighborAlive,
                IsNorthNeighborAlive,
                IsNortheastNeighborAlive
            };
    }
}
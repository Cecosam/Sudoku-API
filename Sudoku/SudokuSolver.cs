using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sudoku
{
    public class SudokuSolver
    {
        private SudokuCell[,] sudoku;
        private bool[,] fixedSudokuNumbers;
        private bool isSolved;
        private const int minimumOfSudokuClues = 17;
        private static int index;
        private static int clues;

        public SudokuSolver()
        {
            this.sudoku = null;
            this.fixedSudokuNumbers = null;
        }

        public void GetSudoku(SudokuCell[,] sudokuToSolve)
        {
            if (!CheckIfSudokuIsValid(sudokuToSolve))
            {
                Console.WriteLine("Invalid sudkou!");
                return;
            }
            Console.WriteLine("Valid sudoku inserted!");
        }

        public void GenerateSudoku(int row, int col, int number, List<Tuple<int, int>> possiblePositions)
        {
            sudoku[row, col].Value = number;
            if (CheckIfNumberIsInSubBox(row, col, number) || CheckIfSameNumberIsInColumn(row, col, number) || CheckIfSameNumberIsInRow(row, col, number))
            {
                sudoku[row, col].Value = 0;
                return;
            }

            var tempSudoku = CloneSudoku(sudoku);

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (sudoku[i, j].Value != 0)
                    {
                        fixedSudokuNumbers[i, j] = true;
                    }
                }
            }
            this.SolveSudoku();

            if (!isSolved)
            {
                sudoku = CloneSudoku(tempSudoku);
                sudoku[row, col].Value = 0;
                return;
            }

            sudoku = CloneSudoku(tempSudoku);

            index++;

            if (index >= clues)
            {
                return;
            }
            RemoveTheRightPosition(possiblePositions, row, col);

            var rnd = new Random();
            while (true)
            {
                var currentPosition = possiblePositions[rnd.Next(0, possiblePositions.Count)];

                row = currentPosition.Item1;
                col = currentPosition.Item2;

                RemoveTheRightPosition(possiblePositions, row, col);
                var possibleNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                while (true)
                {
                    if (possibleNumbers.Count == 0)
                    {
                        //possiblePositions.Add(new Tuple<int, int>(row, col));
                        break;
                    }
                    var i = rnd.Next(0, possibleNumbers.Count);
                    number = possibleNumbers[i];

                    GenerateSudoku(row, col, number, possiblePositions);

                    if (index >= clues)
                    {
                        return;
                    }

                    possibleNumbers.RemoveAt(i);
                }
                possiblePositions.Add(new Tuple<int, int>(row, col));
            }
        }

        private void RemoveTheRightPosition(List<Tuple<int, int>> possiblePositions, int row, int col)
        {
            for (int i = 0; i < possiblePositions.Count; i++)
            {
                var tempTuple = possiblePositions[i];
                if (tempTuple.Item1 == row && tempTuple.Item2 == col)
                {
                    possiblePositions.RemoveAt(i);
                }
            }
        }

        public SudokuCell[,] GenerateSudoku(int cluesCount)
        {
            clues = cluesCount;
            index = 0;
            InitializeSudoku();
            fixedSudokuNumbers = new bool[9, 9];
            int row, col, number;
            var rnd = new Random();
            var possiblePositions = GetAllPositions();

            while (true)
            {
                row = rnd.Next(0, 9);
                col = rnd.Next(0, 9);
                number = rnd.Next(1, 10);
                if (index >= clues)
                {
                    break;
                }
                GenerateSudoku(row, col, number, possiblePositions);
            }

            return sudoku;
        }

        private int NumbersInSudoku()
        {
            var counter = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (sudoku[i, j].Value != 0)
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }

        private List<Tuple<int, int>> GetAllPositions()
        {
            var result = new List<Tuple<int, int>>();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var tempTuple = new Tuple<int, int>(i, j);
                    result.Add(tempTuple);
                }
            }
            return result;
        }

        private SudokuCell[,] CloneSudoku(SudokuCell[,] sudoku)
        {
            var result = new SudokuCell[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    result[i, j] = new SudokuCell(i, j);
                    result[i, j].Value = sudoku[i, j].Value;
                }
            }
            return result;
        }

        public void PrintSolvedSudoku()
        {
            if (this.sudoku == null)
            {
                Console.WriteLine("There is no sudoku to print!");
                return;
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (j == 3 || j == 6)
                    {
                        Console.Write("| ");
                    }
                    Console.Write("{0} ", this.sudoku[i, j]);
                }

                Console.WriteLine();
                if (i == 2 || i == 5)
                {
                    Console.WriteLine(new String('-', 22));
                }
            }
        }

        public void SolveSudoku(SudokuCell[,] sudokuToSolve)
        {
            InitializeSudoku();
            this.fixedSudokuNumbers = new bool[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    this.sudoku[row, col].Value = sudokuToSolve[row, col].Value;
                }
            }

            var sw = new Stopwatch();
            this.isSolved = false;

            for (int i = 1; i < 10; i++)
            {
                if (this.isSolved == true)
                {
                    break;
                }
                SolveSudoku(0, 0, i, sw);
            }

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    sudokuToSolve[row, col].Value = this.sudoku[row, col].Value;
                }
            }
        }
        public void SolveSudoku()
        {
            if (this.sudoku == null)
            {
                Console.WriteLine("Please first insert sudoku!");
                return;
            }

            var sw = new Stopwatch();
            sw.Start();
            this.isSolved = false;

            for (int i = 1; i < 10; i++)
            {
                if (this.isSolved == true)
                {
                    break;
                }
                SolveSudoku(0, 0, i, sw);
            }
        }

        public void SolveSudoku(int row, int col, int number, Stopwatch sw)
        {
            if (sw.Elapsed.Seconds >= 2)
            {
                return;
            }
            if (col == 9)
            {
                col = 0;
                row++;
                if (row == 9)
                {
                    this.isSolved = true;
                    return;
                }
            }

            if (this.fixedSudokuNumbers[row, col] == true)
            {
                SolveSudoku(row, col + 1, number, sw);
                return;
            }

            if (CheckIfNumberIsInSubBox(row, col, number) || CheckIfSameNumberIsInColumn(row, col, number) || CheckIfSameNumberIsInRow(row, col, number))
            {
                return;
            }

            this.sudoku[row, col].Value = number;

            if (row == 8 && col == 8)
            {
                this.isSolved = true;
                return;
            }

            for (int i = 1; i < 10; i++)
            {
                SolveSudoku(row, col + 1, i, sw);
                if (sw.Elapsed.Seconds >= 2)
                {
                    if (this.fixedSudokuNumbers[row, col] == false)
                    {
                        this.sudoku[row, col].Value = 0;
                    }
                    return;
                }
                if (this.isSolved == true)
                {
                    return;
                }
            }

            if (this.fixedSudokuNumbers[row, col] == false)
            {
                this.sudoku[row, col].Value = 0;
            }
        }
        public bool CheckIfSudokuIsValid(SudokuCell[,] sudokuToSolve)
        {
            if (sudokuToSolve.GetLength(0) != 9 || sudokuToSolve.GetLength(1) != 9)
            {
                return false;
            }

            int minimumOfSudokuClues = 17;
            int countOfClues = 0;

            InitializeSudoku();
            this.fixedSudokuNumbers = new bool[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (sudokuToSolve[i, j] == null)
                    {
                        sudokuToSolve[i, j] = new SudokuCell(i, j);
                    }
                    if (sudokuToSolve[i, j].Value != 0)
                    {
                        this.sudoku[i, j].Value = sudokuToSolve[i, j].Value;
                        this.fixedSudokuNumbers[i, j] = true;
                        countOfClues++;
                    }
                }
            }

            if (countOfClues < minimumOfSudokuClues)
            {
                this.sudoku = null;
                this.fixedSudokuNumbers = null;
                return false;
            }

            if (!ValidateSudokuNumbers())
            {
                this.sudoku = null;
                this.fixedSudokuNumbers = null;
                return false;
            }
            return true;
        }

        private bool ValidateSudokuNumbers()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (this.sudoku[i, j].Value == 0)
                    {
                        continue;
                    }
                    if (CheckIfNumberIsInSubBox(i, j, sudoku[i, j].Value) || CheckIfSameNumberIsInColumn(i, j, sudoku[i, j].Value) || CheckIfSameNumberIsInRow(i, j, sudoku[i, j].Value))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckIfNumberIsInSubBox(int row, int col, int number)
        {
            int startRow = 0;
            int startCol = 0;

            if (row > 5)
            {
                startRow = 6;
            }
            else if (row > 2)
            {
                startRow = 3;
            }

            if (col > 5)
            {
                startCol = 6;
            }
            else if (col > 2)
            {
                startCol = 3;
            }

            for (int i = startRow; i < startRow + 3; i++)
            {
                for (int j = startCol; j < startCol + 3; j++)
                {
                    if (i == row && j == col)
                    {
                        continue;
                    }
                    if (this.sudoku[i, j].Value == number)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckIfSameNumberIsInRow(int row, int col, int number)
        {
            for (int i = 0; i < 9; i++)
            {
                if (i == col)
                {
                    continue;
                }
                else if (this.sudoku[row, i].Value == number)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckIfSameNumberIsInColumn(int row, int col, int number)
        {
            for (int i = 0; i < 9; i++)
            {
                if (i == row)
                {
                    continue;
                }
                else if (this.sudoku[i, col].Value == number)
                {
                    return true;
                }
            }
            return false;
        }

        private void InitializeSudoku()
        {
            this.sudoku = new SudokuCell[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    this.sudoku[row, col] = new SudokuCell(row, col);
                }
            }
        }
    }
}

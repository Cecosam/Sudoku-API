using System;
using System.Collections.Generic;
using Sudoku.Techniques;
using System.IO;

namespace Sudoku
{
    public class SudokuDifficulty
    {
        public SudokuCell[,] sudoku;
        public int counter;

        public SudokuCell[,] SolveSudoku(SudokuCell[,] inputSudoku)
        {
            InitializeSudoku(inputSudoku);
            var easySudoku = new EasySudokuTechniques(this);
            var mediumSudoku = new MediumSudokuTechniques(this);
            var hardSudoku = new HardSudokuTechniques(this);

            counter = GetTheCountOfClues();

            while (true)
            {
                if (counter == 81)
                {
                    break;
                }

                if (!easySudoku.UseAllEasyTechniques())
                {
                    if (!mediumSudoku.UseNakedPairTechnique())
                    {
                        if (!mediumSudoku.UseNakedPairInBoxTechnique())
                        {
                            if (!mediumSudoku.UseCandidateLinesTechnique())
                            {

                                if (!mediumSudoku.UseX_WingTechnique())
                                {

                                    if (!mediumSudoku.UseUniqueRectangleTechnique())
                                    {
                                        if (!hardSudoku.SolveSudoku(easySudoku, mediumSudoku))
                                        {
                                            //Should NEVER enter this if statement!
                                            throw new ArgumentException("Problem with the sudoku!!!");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return this.sudoku;
        }
        public DifficultyType CheckSudokuDifficulty(SudokuCell[,] inputSudoku)
        {
            InitializeSudoku(inputSudoku);
            var easySudoku = new EasySudokuTechniques(this);
            var mediumSudoku = new MediumSudokuTechniques(this);
            var hardSudoku = new HardSudokuTechniques(this);

            counter = GetTheCountOfClues();

            var sudokuDifficulty = DifficultyType.Easy;
            while (true)
            {
                if (counter == 81)
                {
                    break;
                }

                if (!easySudoku.UseAllEasyTechniques())
                {
                    sudokuDifficulty = DifficultyType.Medium;
                    if (!mediumSudoku.UseNakedPairTechnique())
                    {
                        if (!mediumSudoku.UseNakedPairInBoxTechnique())
                        {
                            if (!mediumSudoku.UseCandidateLinesTechnique())
                            {

                                if (!mediumSudoku.UseX_WingTechnique())
                                {

                                    if (!mediumSudoku.UseUniqueRectangleTechnique())
                                    {

                                        sudokuDifficulty = DifficultyType.Hard;
                                        if (!hardSudoku.UseNishioTechnique(easySudoku, mediumSudoku))
                                        {
                                            sudokuDifficulty = DifficultyType.VeryHard;
                                            return sudokuDifficulty;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return sudokuDifficulty;
        }

        public SudokuCell GetAllPossibleNumbers(int inputRow, int inputCol, SudokuCell[,] inputSudoku)
        {
            InitializeSudoku(inputSudoku);

            SetAllPossibleValues(inputRow, inputCol);

            return sudoku[inputRow, inputCol];
        }
        public bool DEBUG_CheckIfThereIsValueZeroWithZeroPossibilities()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (this.sudoku[row, col].Value == 0 && this.sudoku[row, col].PossibleValues.Count == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private int DEBUG_GetCountOfAllNonZeroValues()
        {
            var counter = 0;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (this.sudoku[row, col].Value != 0)
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }
        public void PrintSudoku()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (j == 3 || j == 6)
                    {
                        Console.Write("| ");
                    }
                    Console.Write("{0} ", this.sudoku[i, j].Value);
                }

                Console.WriteLine();
                if (i == 2 || i == 5)
                {
                    Console.WriteLine(new String('-', 22));
                }
            }
        }
        public void SetAllPossibleValues(int row, int col)
        {
            var currentCellPossibleValues = this.sudoku[row, col].PossibleValues;

            var valuesOnRow = GetValuesOnRow(row);
            var valuesOnCol = GetValuesOnCol(col);

            var startRow = (row / 3) * 3;
            var startCol = (col / 3) * 3;

            var valuesOnBox = GetValuesInBox(startRow, startCol);

            foreach (var item in valuesOnBox)
            {
                currentCellPossibleValues.Remove(item);
            }

            foreach (var item in valuesOnCol)
            {
                currentCellPossibleValues.Remove(item);
            }

            foreach (var item in valuesOnRow)
            {
                currentCellPossibleValues.Remove(item);
            }
        }

        private HashSet<int> GetValuesInBox(int startRow, int startCol)
        {
            var result = new HashSet<int>();
            for (int row = startRow; row < startRow + 3; row++)
            {
                for (int col = startCol; col < startCol + 3; col++)
                {
                    if (this.sudoku[row, col].Value != 0)
                    {
                        result.Add(this.sudoku[row, col].Value);
                    }
                }
            }
            return result;
        }

        private HashSet<int> GetValuesOnCol(int col)
        {
            var result = new HashSet<int>();
            for (int row = 0; row < 9; row++)
            {
                if (this.sudoku[row, col].Value != 0)
                {
                    result.Add(this.sudoku[row, col].Value);
                }
            }
            return result;
        }
        private HashSet<int> GetValuesOnRow(int row)
        {
            var result = new HashSet<int>();
            for (int col = 0; col < 9; col++)
            {
                if (this.sudoku[row, col].Value != 0)
                {
                    result.Add(this.sudoku[row, col].Value);
                }
            }
            return result;
        }
        private int GetTheCountOfClues()
        {
            var counter = 0;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (this.sudoku[row, col].Value != 0)
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }

        private void DEBUG_GetCurrentStateOfSudoku()
        {
            if (!Directory.Exists(@"D:\Test"))
            {
                Directory.CreateDirectory(@"D:\Test");
            }

            string[] sudokuLines = new string[9];

            for (int row = 0; row < 9; row++)
            {
                var currentLine = string.Empty;
                currentLine += "{ ";
                for (int col = 0; col < 9; col++)
                {
                    if (col != 8)
                    {
                        currentLine += sudoku[row, col] + ",";
                        continue;
                    }
                    currentLine += sudoku[row, col] + "}";
                }
                if (row != 8)
                {
                    currentLine += ",";
                }

                sudokuLines[row] = currentLine;
            }

            File.WriteAllLines(@"D:\Test\Test.txt", sudokuLines);
        }

        private void InitializeSudoku(int[,] inputSudoku)
        {
            this.sudoku = new SudokuCell[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    this.sudoku[row, col] = new SudokuCell(row, col);
                    this.sudoku[row, col].Value = inputSudoku[row, col];
                }
            }
        }

        private void InitializeSudoku(SudokuCell[,] inputSudoku)
        {
            this.sudoku = new SudokuCell[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    this.sudoku[row, col] = new SudokuCell(row, col);
                    this.sudoku[row, col].Value = inputSudoku[row, col].Value;
                }
            }
        }

        private int[,] SudokuToArrayOfInts(SudokuCell[,] inputSudoku)
        {
            int[,] retsult = new int[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    retsult[row, col] = inputSudoku[row, col].Value;
                }
            }
            return retsult;
        }


    }
}


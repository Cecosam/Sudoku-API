using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Techniques
{
    public class HardSudokuTechniques
    {
        public const int BOTTOM = 3;
        private SudokuDifficulty sudokuDifficulty;
        public HardSudokuTechniques(SudokuDifficulty sudokuDifficulty)
        {
            this.sudokuDifficulty = sudokuDifficulty;
        }

        public bool UseNishioTechnique(EasySudokuTechniques easySudoku, MediumSudokuTechniques mediumSudoku, int counter = 0)
        {
            if (BOTTOM == counter)
            {
                return false;
            }

            var possibleValues = new List<int>();
            var row = -1;
            var col = -1;
            var isFound = false;

            for (int iRow = 0; iRow < 9; iRow++)
            {
                for (int iCol = 0; iCol < 9; iCol++)
                {
                    if (sudokuDifficulty.sudoku[iRow, iCol].Value == 0 && sudokuDifficulty.sudoku[iRow, iCol].PossibleValues.Count <= 3)
                    {
                        possibleValues = sudokuDifficulty.sudoku[iRow, iCol].PossibleValues.Where(x => true).ToList();
                        row = sudokuDifficulty.sudoku[iRow, iCol].Row;
                        col = sudokuDifficulty.sudoku[iRow, iCol].Col;
                        isFound = true;
                        break;
                    }
                }
                if (isFound)
                {
                    break;
                }
            }

            if (row == -1)
            {
                return false;
            }

            foreach (var number in possibleValues)
            {
                var currentCount = sudokuDifficulty.counter;
                var state = SaveSudokuState();
                sudokuDifficulty.sudoku[row, col].Value = number;
                sudokuDifficulty.counter++;


                while (true)
                {
                    if (sudokuDifficulty.counter == 81)
                    {
                        return true;
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
                                            if (!UseNishioTechnique(easySudoku, mediumSudoku, counter + 1))
                                            {
                                                sudokuDifficulty.counter = currentCount;
                                                sudokuDifficulty.sudoku = state;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool SolveSudoku(EasySudokuTechniques easySudoku, MediumSudokuTechniques mediumSudoku)
        {
            if (sudokuDifficulty.counter == 81)
            {
                return true;
            }

            var possibleValues = new List<int>();
            var row = -1;
            var col = -1;
            var isFound = false;

            for (int iRow = 0; iRow < 9; iRow++)
            {
                for (int iCol = 0; iCol < 9; iCol++)
                {
                    if (sudokuDifficulty.sudoku[iRow, iCol].Value == 0)
                    {
                        possibleValues = sudokuDifficulty.sudoku[iRow, iCol].PossibleValues.Where(x => true).ToList();
                        row = sudokuDifficulty.sudoku[iRow, iCol].Row;
                        col = sudokuDifficulty.sudoku[iRow, iCol].Col;
                        isFound = true;
                        break;
                    }
                }
                if (isFound)
                {
                    break;
                }
            }

            if (row == -1)
            {
                return false;
            }

            foreach (var number in possibleValues)
            {
                var currentCount = sudokuDifficulty.counter;
                var state = SaveSudokuState();
                sudokuDifficulty.sudoku[row, col].Value = number;
                sudokuDifficulty.counter++;

                while (true)
                {
                    if (sudokuDifficulty.counter == 81)
                    {
                        return true;
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
                                            if (!SolveSudoku(easySudoku, mediumSudoku))
                                            {
                                                sudokuDifficulty.counter = currentCount;
                                                sudokuDifficulty.sudoku = state;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private SudokuCell[,] SaveSudokuState()
        {
            var state = new SudokuCell[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    state[row, col] = new SudokuCell(row, col);
                    var list = sudokuDifficulty.sudoku[row, col].PossibleValues.ToList();
                    state[row, col].PossibleValues = list;
                    state[row, col].Value = sudokuDifficulty.sudoku[row, col].Value;
                }
            }
            return state;
        }
    }
}

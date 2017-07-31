

namespace Sudoku.Techniques
{
    public class EasySudokuTechniques
    {
        private SudokuDifficulty sudokuDifficulty;
        public EasySudokuTechniques(SudokuDifficulty sudokuDifficulty)
        {
            this.sudokuDifficulty = sudokuDifficulty;
        }

        public bool UseAllEasyTechniques()
        {
            var result = false;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (sudokuDifficulty.sudoku[row, col].Value == 0)
                    {
                        sudokuDifficulty.SetAllPossibleValues(row, col);

                        if (sudokuDifficulty.sudoku[row, col].PossibleValues.Count == 1)
                        {
                            sudokuDifficulty.sudoku[row, col].Value = sudokuDifficulty.sudoku[row, col].PossibleValues[0];
                            sudokuDifficulty.sudoku[row, col].PossibleValues.Clear();
                            sudokuDifficulty.counter++;
                            result = true;
                        }
                    }
                }
            }
            return result;
        }
    }
}

using System.Collections.Generic;

namespace Sudoku
{
    public class SudokuCell
    {
        private SudokuCell()
        {

        }
        public SudokuCell(int row, int col)
        {
            this.PossibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            this.Value = 0;
            this.Row = row;
            this.Col = col;
        }
        public List<int> PossibleValues { get; set; }
        public int Value { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
    }
}

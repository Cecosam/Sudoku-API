using System.Collections.Generic;
using System.Linq;


namespace Sudoku.Techniques
{
    public class MediumSudokuTechniques
    {
        private SudokuDifficulty sudokuDifficulty;
        public MediumSudokuTechniques(SudokuDifficulty sudokuDifficulty)
        {
            this.sudokuDifficulty = sudokuDifficulty;
        }

        public bool UseNakedPairInBoxTechnique()
        {
            var result = false;
            for (int row = 0; row < 9; row += 3)
            {
                for (int col = 0; col < 9; col += 3)
                {
                    if (result == false)
                    {
                        result = UseNakedPairInBox(row, col);
                    }
                    else
                    {
                        UseNakedPairInBox(row, col);
                    }
                }
            }
            return result;
        }
        public bool UseNakedPairTechnique()
        {
            var result = false;
            for (int rowAndCol = 0; rowAndCol < 9; rowAndCol++)
            {
                if (result == false)
                {
                    result = UseNakedPairOnRow(rowAndCol);
                }
                else
                {
                    UseNakedPairOnRow(rowAndCol);
                }

                if (result == false)
                {
                    result = UseNakedPairOnCol(rowAndCol);
                }
                else
                {
                    UseNakedPairOnCol(rowAndCol);
                }
            }
            return result;
        }
        public bool UseCandidateLinesTechnique()
        {
            var result = false;
            for (int row = 0; row < 9; row += 3)
            {
                for (int col = 0; col < 9; col += 3)
                {
                    if (result == false)
                    {
                        result = UseCandidateLines(row, col);
                    }
                    else
                    {
                        UseCandidateLines(row, col);
                    }
                }
            }
            return result;
        }

        public bool UseX_WingTechnique()
        {
            var result = false;

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (sudokuDifficulty.sudoku[row, col].Value == 0)
                    {
                        foreach (var number in sudokuDifficulty.sudoku[row, col].PossibleValues)
                        {
                            var cellWithNumberOnRow = CheckIfNumberIsFoundOnlyTwoTimesOnRow(number, sudokuDifficulty.sudoku[row, col]);
                            var cellWithNumberOnCol = CheckIfNumberIsFoundOnlyTwoTimesOnCol(number, sudokuDifficulty.sudoku[row, col]);

                            if (cellWithNumberOnRow != null)
                            {
                                var resultRow = FindIfThereIsRowWhereNumberIsOnlyOnSameCols(cellWithNumberOnRow, sudokuDifficulty.sudoku[row, col], number);
                                if (resultRow != -1)
                                {
                                    if (result == false)
                                    {
                                        result = RemoveNumberFromCols(sudokuDifficulty.sudoku[row, col], sudokuDifficulty.sudoku[resultRow, col], number);
                                    }
                                    else
                                    {
                                        RemoveNumberFromCols(sudokuDifficulty.sudoku[row, col], sudokuDifficulty.sudoku[resultRow, col], number);
                                    }
                                    if (result == false)
                                    {
                                        result = RemoveNumberFromCols(cellWithNumberOnRow, sudokuDifficulty.sudoku[resultRow, cellWithNumberOnRow.Col], number);
                                    }
                                    else
                                    {
                                        RemoveNumberFromCols(cellWithNumberOnRow, sudokuDifficulty.sudoku[resultRow, cellWithNumberOnRow.Col], number);
                                    }
                                }
                            }

                            if (cellWithNumberOnCol != null)
                            {
                                var resultCol = FindIfThereIsColWhereNumberIsOnlyOnSameRows(cellWithNumberOnCol, sudokuDifficulty.sudoku[row, col], number);
                                if (resultCol != -1)
                                {
                                    if (result == false)
                                    {
                                        result = RemoveNumberFromRows(sudokuDifficulty.sudoku[row, col], sudokuDifficulty.sudoku[row, resultCol], number);
                                    }
                                    else
                                    {
                                        RemoveNumberFromRows(sudokuDifficulty.sudoku[row, col], sudokuDifficulty.sudoku[row, resultCol], number);
                                    }
                                    if (result == false)
                                    {
                                        result = RemoveNumberFromRows(cellWithNumberOnCol, sudokuDifficulty.sudoku[cellWithNumberOnCol.Row, resultCol], number);
                                    }
                                    else
                                    {
                                        RemoveNumberFromRows(cellWithNumberOnCol, sudokuDifficulty.sudoku[cellWithNumberOnCol.Row, resultCol], number);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        private bool RemoveNumberFromRows(SudokuCell firstCell, SudokuCell secondCell, int number)
        {
            var result = false;
            var row = firstCell.Row;
            for (int col = 0; col < 9; col++)
            {
                if (sudokuDifficulty.sudoku[row, col].Equals(firstCell) || sudokuDifficulty.sudoku[row, col].Equals(secondCell))
                {
                    continue;
                }

                if (sudokuDifficulty.sudoku[row, col].Value == 0)
                {
                    sudokuDifficulty.sudoku[row, col].PossibleValues.Remove(number);
                    if (sudokuDifficulty.sudoku[row, col].PossibleValues.Count == 1)
                    {
                        sudokuDifficulty.sudoku[row, col].Value = sudokuDifficulty.sudoku[row, col].PossibleValues[0];
                        sudokuDifficulty.sudoku[row, col].PossibleValues.Clear();
                        sudokuDifficulty.counter++;
                        result = true;
                    }
                }
            }
            return result;
        }

        private bool RemoveNumberFromCols(SudokuCell firstCell, SudokuCell secondCell, int number)
        {
            var result = false;
            var col = firstCell.Col;
            for (int row = 0; row < 9; row++)
            {
                if (sudokuDifficulty.sudoku[row, col].Equals(firstCell) || sudokuDifficulty.sudoku[row, col].Equals(secondCell))
                {
                    continue;
                }

                if (sudokuDifficulty.sudoku[row, col].Value == 0)
                {
                    sudokuDifficulty.sudoku[row, col].PossibleValues.Remove(number);
                    if (sudokuDifficulty.sudoku[row, col].PossibleValues.Count == 1)
                    {
                        sudokuDifficulty.sudoku[row, col].Value = sudokuDifficulty.sudoku[row, col].PossibleValues[0];
                        sudokuDifficulty.sudoku[row, col].PossibleValues.Clear();
                        sudokuDifficulty.counter++;
                        result = true;
                    }
                }
            }
            return result;
        }

        private int FindIfThereIsColWhereNumberIsOnlyOnSameRows(SudokuCell cellWithNumberOnRow, SudokuCell sudokuCell, int number)
        {
            var firstValueFound = false;
            var secondValueFound = false;
            var colToAvoid = sudokuCell.Col;
            for (int col = 0; col < 9; col++)
            {
                if (col == colToAvoid)
                {
                    continue;
                }
                firstValueFound = false;
                secondValueFound = false;
                for (int row = 0; row < 9; row++)
                {
                    if (sudokuDifficulty.sudoku[row, col].Value == 0 && sudokuDifficulty.sudoku[row, col].PossibleValues.Contains(number))
                    {
                        if (row == cellWithNumberOnRow.Row)
                        {
                            firstValueFound = true;
                        }
                        else if (row == sudokuCell.Row)
                        {
                            secondValueFound = true;
                        }
                        else
                        {
                            firstValueFound = false;
                            secondValueFound = false;
                            break;
                        }
                    }
                }
                if (firstValueFound && secondValueFound)
                {
                    return col;
                }
            }
            return -1;
        }

        private int FindIfThereIsRowWhereNumberIsOnlyOnSameCols(SudokuCell cellWithNumberOnRow, SudokuCell sudokuCell, int number)
        {
            var firstValueFound = false;
            var secondValueFound = false;
            var rowToAvoid = sudokuCell.Row;
            for (int row = 0; row < 9; row++)
            {
                if (row == rowToAvoid)
                {
                    continue;
                }
                firstValueFound = false;
                secondValueFound = false;
                for (int col = 0; col < 9; col++)
                {
                    if (sudokuDifficulty.sudoku[row, col].Value == 0 && sudokuDifficulty.sudoku[row, col].PossibleValues.Contains(number))
                    {
                        if (col == cellWithNumberOnRow.Col)
                        {
                            firstValueFound = true;
                        }
                        else if (col == sudokuCell.Col)
                        {
                            secondValueFound = true;
                        }
                        else
                        {
                            firstValueFound = false;
                            secondValueFound = false;
                            break;
                        }
                    }
                }
                if (firstValueFound && secondValueFound)
                {
                    return row;
                }
            }
            return -1;
        }

        private SudokuCell CheckIfNumberIsFoundOnlyTwoTimesOnCol(int number, SudokuCell sudokuCell)
        {
            var listWithCells = new List<SudokuCell>();
            var col = sudokuCell.Col;
            for (int row = 0; row < 9; row++)
            {
                if (sudokuDifficulty.sudoku[row, col].Equals(sudokuCell))
                {
                    continue;
                }

                if (sudokuDifficulty.sudoku[row, col].Value == 0)
                {
                    if (sudokuDifficulty.sudoku[row, col].PossibleValues.Contains(number))
                    {
                        listWithCells.Add(sudokuDifficulty.sudoku[row, col]);
                    }
                }
            }
            if (listWithCells.Count == 1)
            {
                return listWithCells[0];
            }
            return null;
        }
        private SudokuCell CheckIfNumberIsFoundOnlyTwoTimesOnRow(int number, SudokuCell sudokuCell)
        {
            var listWithCells = new List<SudokuCell>();
            var row = sudokuCell.Row;
            for (int col = 0; col < 9; col++)
            {
                if (sudokuDifficulty.sudoku[row, col].Equals(sudokuCell))
                {
                    continue;
                }

                if (sudokuDifficulty.sudoku[row, col].Value == 0)
                {
                    if (sudokuDifficulty.sudoku[row, col].PossibleValues.Contains(number))
                    {
                        listWithCells.Add(sudokuDifficulty.sudoku[row, col]);
                    }
                }
            }
            if (listWithCells.Count == 1)
            {
                return listWithCells[0];
            }
            return null;
        }

        public bool UseUniqueRectangleTechnique()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (sudokuDifficulty.sudoku[row, col].Value == 0)
                    {
                        if (sudokuDifficulty.sudoku[row, col].PossibleValues.Count == 2)
                        {
                            var startRow = (row / 3) * 3;
                            var startCol = (col / 3) * 3;

                            var cellOnTheSameRow = CheckIfThereIsAnotherCellLikeTheFirstOnRowInBox(sudokuDifficulty.sudoku[row, col], startCol);
                            var cellOnTheSameCol = CheckIfThereIsAnotherCellLikeTheFirstOnColInBox(sudokuDifficulty.sudoku[row, col], startRow);

                            if (cellOnTheSameRow != null)
                            {
                                var innerRow = CheckIfThereIsUniqueRectangleOnCols(sudokuDifficulty.sudoku[row, col], cellOnTheSameRow);
                                if (innerRow != -1)
                                {
                                    DistributeValuesInTheUniqueRectangleForRows(innerRow, sudokuDifficulty.sudoku[row, col], cellOnTheSameRow);
                                    return true;
                                }
                            }
                            else if (cellOnTheSameCol != null)
                            {
                                var innerCol = CheckIfThereIsUniqueRectangleOnRows(sudokuDifficulty.sudoku[row, col], cellOnTheSameCol);
                                if (innerCol != -1)
                                {
                                    DistributeValuesInTheUniqueRectangleForCols(innerCol, sudokuDifficulty.sudoku[row, col], cellOnTheSameCol);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void DistributeValuesInTheUniqueRectangleForCols(int innerCol, SudokuCell sudokuCell, SudokuCell cellOnTheSameCol)
        {
            var firstNumber = sudokuCell.PossibleValues[0];
            var secondNumber = sudokuCell.PossibleValues[1];

            sudokuCell.Value = firstNumber;
            sudokuCell.PossibleValues.Clear();

            sudokuDifficulty.sudoku[cellOnTheSameCol.Row, innerCol].Value = firstNumber;
            sudokuDifficulty.sudoku[cellOnTheSameCol.Row, innerCol].PossibleValues.Clear();

            cellOnTheSameCol.Value = secondNumber;
            cellOnTheSameCol.PossibleValues.Clear();

            sudokuDifficulty.sudoku[sudokuCell.Row, innerCol].Value = secondNumber;
            sudokuDifficulty.sudoku[sudokuCell.Row, innerCol].PossibleValues.Clear();

            sudokuDifficulty.counter += 4;
        }

        private void DistributeValuesInTheUniqueRectangleForRows(int innerRow, SudokuCell sudokuCell, SudokuCell cellOnTheSameRow)
        {
            var firstNumber = sudokuCell.PossibleValues[0];
            var secondNumber = sudokuCell.PossibleValues[1];

            sudokuCell.Value = firstNumber;
            sudokuCell.PossibleValues.Clear();

            sudokuDifficulty.sudoku[innerRow, cellOnTheSameRow.Col].Value = firstNumber;
            sudokuDifficulty.sudoku[innerRow, cellOnTheSameRow.Col].PossibleValues.Clear();

            cellOnTheSameRow.Value = secondNumber;
            cellOnTheSameRow.PossibleValues.Clear();

            sudokuDifficulty.sudoku[innerRow, sudokuCell.Col].Value = secondNumber;
            sudokuDifficulty.sudoku[innerRow, sudokuCell.Col].PossibleValues.Clear();

            sudokuDifficulty.counter += 4;
        }

        private int CheckIfThereIsUniqueRectangleOnCols(SudokuCell firstCell, SudokuCell secondCell)
        {
            var col = firstCell.Col;
            var secondCol = secondCell.Col;
            for (int row = 0; row < 9; row++)
            {
                if (sudokuDifficulty.sudoku[row, col].Equals(firstCell) || sudokuDifficulty.sudoku[row, col].Value != 0 ||
                    sudokuDifficulty.sudoku[row, col].PossibleValues.Count != 2)
                {
                    continue;
                }

                var tempList = sudokuDifficulty.sudoku[row, col].PossibleValues.Where(x => firstCell.PossibleValues.Contains(x)).ToList();
                if (tempList.Count == 2)
                {
                    if (sudokuDifficulty.sudoku[row, secondCol].PossibleValues.Count == 2 && sudokuDifficulty.sudoku[row, secondCol].Value == 0)
                    {
                        tempList = sudokuDifficulty.sudoku[row, secondCol].PossibleValues.Where(x => firstCell.PossibleValues.Contains(x)).ToList();
                        if (tempList.Count == 2)
                        {
                            return row;
                        }
                    }
                }
            }
            return -1;
        }
        private int CheckIfThereIsUniqueRectangleOnRows(SudokuCell firstCell, SudokuCell secondCell)
        {
            var row = firstCell.Row;
            var secondRow = secondCell.Row;
            for (int col = 0; col < 9; col++)
            {
                if (sudokuDifficulty.sudoku[row, col].Equals(firstCell) || sudokuDifficulty.sudoku[row, col].Value != 0 ||
                    sudokuDifficulty.sudoku[row, col].PossibleValues.Count != 2)
                {
                    continue;
                }

                var tempList = sudokuDifficulty.sudoku[row, col].PossibleValues.Where(x => firstCell.PossibleValues.Contains(x)).ToList();
                if (tempList.Count == 2)
                {
                    if (sudokuDifficulty.sudoku[secondRow, col].PossibleValues.Count == 2 && sudokuDifficulty.sudoku[secondRow, col].Value == 0)
                    {
                        tempList = sudokuDifficulty.sudoku[secondRow, col].PossibleValues.Where(x => firstCell.PossibleValues.Contains(x)).ToList();
                        if (tempList.Count == 2)
                        {
                            return col;
                        }
                    }
                }
            }
            return -1;
        }

        private SudokuCell CheckIfThereIsAnotherCellLikeTheFirstOnColInBox(SudokuCell sudokuCell, int startRow)
        {
            var col = sudokuCell.Col;
            for (int row = startRow; row < startRow + 3; row++)
            {
                if (sudokuDifficulty.sudoku[row, col].Equals(sudokuCell) || sudokuDifficulty.sudoku[row, col].Value != 0 ||
                    sudokuDifficulty.sudoku[row, col].PossibleValues.Count != 2)
                {
                    continue;
                }

                var tempList = sudokuDifficulty.sudoku[row, col].PossibleValues.Where(x => sudokuCell.PossibleValues.Contains(x)).ToList();
                if (tempList.Count == 2)
                {
                    return sudokuDifficulty.sudoku[row, col];
                }
            }
            return null;
        }
        private SudokuCell CheckIfThereIsAnotherCellLikeTheFirstOnRowInBox(SudokuCell sudokuCell, int startCol)
        {
            var row = sudokuCell.Row;
            for (int col = startCol; col < startCol + 3; col++)
            {
                if (sudokuDifficulty.sudoku[row, col].Equals(sudokuCell) || sudokuDifficulty.sudoku[row, col].Value != 0 ||
                    sudokuDifficulty.sudoku[row, col].PossibleValues.Count != 2)
                {
                    continue;
                }

                var tempList = sudokuDifficulty.sudoku[row, col].PossibleValues.Where(x => sudokuCell.PossibleValues.Contains(x)).ToList();
                if (tempList.Count == 2)
                {
                    return sudokuDifficulty.sudoku[row, col];
                }
            }
            return null;
        }

        private bool UseCandidateLines(int startRow, int startCol)
        {
            var result = false;
            var listWithCells = new List<SudokuCell>();
            var listWithPossibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            for (int row = startRow; row < startRow + 3; row++)
            {
                for (int col = startCol; col < startCol + 3; col++)
                {
                    if (sudokuDifficulty.sudoku[row, col].Value == 0)
                    {
                        listWithCells.Add(sudokuDifficulty.sudoku[row, col]);
                    }
                    else
                    {
                        listWithPossibleValues.Remove(sudokuDifficulty.sudoku[row, col].Value);
                    }
                }
            }

            foreach (var number in listWithPossibleValues)
            {
                var listWithCellsThatContainThatNumber = new List<SudokuCell>();
                foreach (var cell in listWithCells)
                {
                    if (cell.PossibleValues.Contains(number))
                    {
                        listWithCellsThatContainThatNumber.Add(cell);
                    }
                }

                if (listWithCellsThatContainThatNumber.Count < 2)
                {
                    continue;
                }

                if (IsOnOneRow(listWithCellsThatContainThatNumber))
                {
                    if (result == false)
                    {
                        result = CheckIfThereIsSolutionOnThisRow(number, listWithCellsThatContainThatNumber);
                    }
                    else
                    {
                        CheckIfThereIsSolutionOnThisRow(number, listWithCellsThatContainThatNumber);
                    }
                }
                if (IsOnOneCol(listWithCellsThatContainThatNumber))
                {
                    if (result == false)
                    {
                        result = CheckIfThereIsSolutionOnThisCol(number, listWithCellsThatContainThatNumber);
                    }
                    else
                    {
                        CheckIfThereIsSolutionOnThisCol(number, listWithCellsThatContainThatNumber);
                    }
                }
            }
            return result;
        }

        private bool CheckIfThereIsSolutionOnThisCol(int number, List<SudokuCell> listWithCellsThatContainThatNumber)
        {
            var result = false;
            var col = listWithCellsThatContainThatNumber[0].Col;
            for (int row = 0; row < 9; row++)
            {
                if (listWithCellsThatContainThatNumber.Contains(sudokuDifficulty.sudoku[row, col]))
                {
                    continue;
                }

                if (sudokuDifficulty.sudoku[row, col].Value == 0)
                {
                    sudokuDifficulty.sudoku[row, col].PossibleValues.Remove(number);
                    if (sudokuDifficulty.sudoku[row, col].PossibleValues.Count == 1)
                    {
                        sudokuDifficulty.sudoku[row, col].Value = sudokuDifficulty.sudoku[row, col].PossibleValues[0];
                        sudokuDifficulty.sudoku[row, col].PossibleValues.Clear();
                        sudokuDifficulty.counter++;
                        result = true;
                    }
                }
            }
            return result;
        }

        private bool CheckIfThereIsSolutionOnThisRow(int number, List<SudokuCell> listWithCellsThatContainThatNumber)
        {
            var result = false;
            var row = listWithCellsThatContainThatNumber[0].Row;
            for (int col = 0; col < 9; col++)
            {
                if (listWithCellsThatContainThatNumber.Contains(sudokuDifficulty.sudoku[row, col]))
                {
                    continue;
                }

                if (sudokuDifficulty.sudoku[row, col].Value == 0)
                {
                    sudokuDifficulty.sudoku[row, col].PossibleValues.Remove(number);
                    if (sudokuDifficulty.sudoku[row, col].PossibleValues.Count == 1)
                    {
                        sudokuDifficulty.sudoku[row, col].Value = sudokuDifficulty.sudoku[row, col].PossibleValues[0];
                        sudokuDifficulty.sudoku[row, col].PossibleValues.Clear();
                        sudokuDifficulty.counter++;
                        result = true;
                    }
                }
            }
            return result;
        }

        private bool IsOnOneRow(List<SudokuCell> listWithCellsThatContainThatNumber)
        {
            for (int i = 1; i < listWithCellsThatContainThatNumber.Count; i++)
            {
                if (listWithCellsThatContainThatNumber[i - 1].Row != listWithCellsThatContainThatNumber[i].Row)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsOnOneCol(List<SudokuCell> listWithCellsThatContainThatNumber)
        {
            for (int i = 1; i < listWithCellsThatContainThatNumber.Count; i++)
            {
                if (listWithCellsThatContainThatNumber[i - 1].Col != listWithCellsThatContainThatNumber[i].Col)
                {
                    return false;
                }
            }
            return true;
        }
        private bool UseNakedPairInBox(int startRow, int startCol)
        {
            var result = false;
            var listWithEmptyCellsInBox = new List<SudokuCell>();
            for (int row = startRow; row < startRow + 3; row++)
            {
                for (int col = startCol; col < startCol + 3; col++)
                {
                    if (sudokuDifficulty.sudoku[row, col].Value == 0)
                    {
                        listWithEmptyCellsInBox.Add(sudokuDifficulty.sudoku[row, col]);
                    }
                }
            }

            foreach (var currentCell in listWithEmptyCellsInBox)
            {
                var listWithPositions = new List<SudokuCell>();
                listWithPositions.Add(currentCell);
                foreach (var cellToCheck in listWithEmptyCellsInBox)
                {
                    if (currentCell.Equals(cellToCheck) || cellToCheck.PossibleValues.Count != currentCell.PossibleValues.Count)
                    {
                        continue;
                    }

                    var tempList = currentCell.PossibleValues.Where(x => cellToCheck.PossibleValues.Contains(x)).ToList();

                    if (tempList.Count == currentCell.PossibleValues.Count)
                    {
                        listWithPositions.Add(cellToCheck);
                    }
                }

                if (listWithPositions.Count == currentCell.PossibleValues.Count)
                {
                    CheckIfThereIsCellToSetValueInIt(listWithEmptyCellsInBox, listWithPositions, currentCell);
                    foreach (var cell in listWithEmptyCellsInBox)
                    {
                        if (cell.PossibleValues.Count == 1)
                        {
                            cell.Value = cell.PossibleValues[0];
                            cell.PossibleValues.Clear();
                            sudokuDifficulty.counter++;
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        private bool UseNakedPairOnCol(int col)
        {
            var result = false;
            var listWithEmptyCellsOnCol = new List<SudokuCell>();

            for (int row = 0; row < 9; row++)
            {
                if (sudokuDifficulty.sudoku[row, col].Value == 0)
                {
                    listWithEmptyCellsOnCol.Add(sudokuDifficulty.sudoku[row, col]);
                }
            }

            foreach (var currentCell in listWithEmptyCellsOnCol)
            {
                var listWithPositions = new List<SudokuCell>();
                listWithPositions.Add(currentCell);
                foreach (var cellToCheck in listWithEmptyCellsOnCol)
                {
                    if (currentCell.Equals(cellToCheck) || cellToCheck.PossibleValues.Count != currentCell.PossibleValues.Count)
                    {
                        continue;
                    }


                    var tempList = currentCell.PossibleValues.Where(x => cellToCheck.PossibleValues.Contains(x)).ToList();

                    if (tempList.Count == currentCell.PossibleValues.Count)
                    {
                        listWithPositions.Add(cellToCheck);
                    }
                }

                if (listWithPositions.Count == currentCell.PossibleValues.Count)
                {
                    CheckIfThereIsCellToSetValueInIt(listWithEmptyCellsOnCol, listWithPositions, currentCell);
                    foreach (var cell in listWithEmptyCellsOnCol)
                    {
                        if (cell.PossibleValues.Count == 1)
                        {
                            cell.Value = cell.PossibleValues[0];
                            cell.PossibleValues.Clear();
                            sudokuDifficulty.counter++;
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        private bool UseNakedPairOnRow(int row)
        {
            var result = false;
            var listWithEmptyCellsOnRow = new List<SudokuCell>();

            for (int col = 0; col < 9; col++)
            {
                if (sudokuDifficulty.sudoku[row, col].Value == 0)
                {
                    listWithEmptyCellsOnRow.Add(sudokuDifficulty.sudoku[row, col]);
                }
            }

            foreach (var currentCell in listWithEmptyCellsOnRow)
            {
                var listWithPositions = new List<SudokuCell>();
                listWithPositions.Add(currentCell);
                foreach (var cellToCheck in listWithEmptyCellsOnRow)
                {
                    if (currentCell.Equals(cellToCheck) || cellToCheck.PossibleValues.Count != currentCell.PossibleValues.Count)
                    {
                        continue;
                    }

                    var tempList = currentCell.PossibleValues.Where(x => cellToCheck.PossibleValues.Contains(x)).ToList();

                    if (tempList.Count == currentCell.PossibleValues.Count)
                    {
                        listWithPositions.Add(cellToCheck);
                    }
                }

                if (listWithPositions.Count == currentCell.PossibleValues.Count)
                {
                    CheckIfThereIsCellToSetValueInIt(listWithEmptyCellsOnRow, listWithPositions, currentCell);
                    foreach (var cell in listWithEmptyCellsOnRow)
                    {
                        if (cell.PossibleValues.Count == 1)
                        {
                            cell.Value = cell.PossibleValues[0];
                            cell.PossibleValues.Clear();
                            sudokuDifficulty.counter++;
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        private void CheckIfThereIsCellToSetValueInIt(List<SudokuCell> listWithEmptyCells, List<SudokuCell> listWithPositions, SudokuCell currentCell)
        {
            foreach (var cell in listWithEmptyCells)
            {
                if (listWithPositions.Contains(cell))
                {
                    continue;
                }

                foreach (var item in currentCell.PossibleValues)
                {
                    cell.PossibleValues.Remove(item);
                }
            }
        }
    }
}

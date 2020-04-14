using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver
{
    public interface ISudokuSolvingLogic
    {
        // TODO Implement as strategies?
        int FindNakedSingle(Sudoku p_sudoku, SudokuCell p_cell);
        int FindHiddenSingle(Sudoku p_sudoku, SudokuCell p_cell);
        int FindNakedSubset(Sudoku p_sudoku, SudokuCell p_cell);
        int FindHiddenSubset(Sudoku p_sudoku, SudokuCell p_cell);
        Sudoku UpdateCandidates(Sudoku p_sudoku);
    }


    public class SudokuSolvingLogic : ISudokuSolvingLogic
    {
        public int FindNakedSingle(Sudoku p_sudoku, SudokuCell p_sudokuCell)
        {
            IList<SudokuCell> row = p_sudoku.GetRow(p_sudokuCell.Row);
            IList<SudokuCell> column = p_sudoku.GetColumn(p_sudokuCell.Column);
            IList<SudokuCell> square = p_sudoku.GetSquare(p_sudokuCell.Row, p_sudokuCell.Column);

            int missingValue = FindNakedSingleInternal(row, column, square);

            return missingValue;
        }

        private int FindNakedSingleInternal(IList<SudokuCell> p_row, IList<SudokuCell> p_column,
            IList<SudokuCell> p_square)
        {
            IList<int> candidates = new List<int>();
            for (int i = 0; i < p_row.Count; i++)
            {
                bool isMissingInRow = p_row.All(p_cell => p_cell.Value != i + 1);
                bool isMissingInColumn = p_column.All(p_cell => p_cell.Value != i + 1);
                bool isMissingInSquare = p_square.All(p_cell => p_cell.Value != i + 1);

                if (isMissingInRow && isMissingInColumn && isMissingInSquare)
                {
                    candidates.Add(i + 1);
                }
            }

            if (candidates.Count == 1)
            {
                return candidates.FirstOrDefault();
            }

            return 0;
        }

        public int FindHiddenSingle(Sudoku p_sudoku, SudokuCell p_sudokuCell)
        {
            int missingValue = 0;
            IList<int> validRowAndColNums = CalculateValidRowAndColNums(p_sudoku.Size);
            
            bool isPossibleRow = (validRowAndColNums.Contains(p_sudokuCell.Row)) &&
                                 (validRowAndColNums.Contains(p_sudokuCell.Column));

            if (isPossibleRow)
            {
                // TODO Refactor
                bool columnIsPossible = p_sudoku[p_sudokuCell.Row - 1, p_sudokuCell.Column] != 0 &&
                                     p_sudoku[p_sudokuCell.Row + 1, p_sudokuCell.Column] != 0;
                bool rowIsPossible = p_sudoku[p_sudokuCell.Row, p_sudokuCell.Column - 1] != 0 &&
                                  p_sudoku[p_sudokuCell.Row, p_sudokuCell.Column + 1] != 0;

                IList<SudokuCell> row = p_sudoku.GetRow(p_sudokuCell.Row);
                IList<SudokuCell> column = p_sudoku.GetColumn(p_sudokuCell.Column);
                IList<SudokuCell> square = p_sudoku.GetSquare(p_sudokuCell.Row, p_sudokuCell.Column);

                if (columnIsPossible)
                {
                    IList<SudokuCell> previousColumn = p_sudokuCell.Column == 0
                        ? new List<SudokuCell>()
                        : p_sudoku.GetColumn(p_sudokuCell.Column - 1);
                    IList<SudokuCell> followingColumn = p_sudokuCell.Column == p_sudoku.Size - 1
                        ? new List<SudokuCell>()
                        : p_sudoku.GetColumn(p_sudokuCell.Column + 1);

                    missingValue = FindHiddenSingleInternal(square, previousColumn, column, followingColumn, row);
                }

                if (rowIsPossible)
                {
                    if (missingValue == 0)
                    {
                        IList<SudokuCell> previousRow = p_sudokuCell.Row == 0
                            ? new List<SudokuCell>()
                            : p_sudoku.GetRow(p_sudokuCell.Row - 1);
                        IList<SudokuCell> followingRow = p_sudokuCell.Row == p_sudoku.Size - 1
                            ? new List<SudokuCell>()
                            : p_sudoku.GetRow(p_sudokuCell.Row + 1);

                        missingValue = FindHiddenSingleInternal(square, previousRow, row, followingRow, column);
                    }
                }
            }


            return missingValue;
        }

        private int FindHiddenSingleInternal(IList<SudokuCell> p_square, IList<SudokuCell> p_previous,
            IList<SudokuCell> p_current, IList<SudokuCell> p_following, IList<SudokuCell> p_toCompare)
        {
            IList<int> candidates = new List<int>();
            for (int i = 0; i < p_current.Count; i++)
            {
                bool isMissingInSquare = p_square.All(p_cell => p_cell.Value != i + 1);
                bool presentInPrevious = p_previous.Any(p_cell => p_cell.Value == i + 1);
                bool presentInFollowing = p_following.Any(p_cell => p_cell.Value == i + 1);
                bool presentInCurrent = p_current.Any(p_cell => p_cell.Value == i + 1);
                bool presentInToCompare = p_toCompare.Any(p_cell => p_cell.Value == i + 1);

                bool isHiddenSingle = isMissingInSquare && presentInFollowing && presentInPrevious &&
                                      !presentInCurrent && !presentInToCompare;
                if (isHiddenSingle)
                {
                    candidates.Add(i + 1);
                }
            }

            if (candidates.Count == 1)
            {
                return candidates.FirstOrDefault();
            }

            return 0;
        }
        
        public int FindNakedSubset(Sudoku p_sudoku, SudokuCell p_cell)
        {
            UpdateCandidates(p_sudoku);
            
            // find naked subset row
            // find naked subset col
            // find naked subset square
            
            

            return 0;
        }

        public int FindHiddenSubset(Sudoku p_sudoku, SudokuCell p_cell)
        {

            return 0;
        }

        public Sudoku UpdateCandidates(Sudoku p_sudoku)
        {
            // Iterate through all cells
            foreach (SudokuCell sudokuCell in p_sudoku.GetAll())
            {
                UpdateCandidatesInternal(p_sudoku, sudokuCell);
            }
            // find and save candidates to cell

            return p_sudoku;
        }

        private void UpdateCandidatesInternal(Sudoku p_sudoku, SudokuCell p_sudokuCell)
        {
            // find possible candidates row
            IList<int> findPossibleCandidatesRow = FindPossibleCandidatesRow(p_sudoku, p_sudokuCell);
            // find possible candidates column
            IList<int> findPossibleCandidatesColumn = FindPossibleCandidatesColumn(p_sudoku, p_sudokuCell);
            // find possible candidates square
            IList<int> findPossibleCandidatesSquare = FindPossibleCandidatesSquare(p_sudoku, p_sudokuCell);
            // merge
            IList<int> possibleValues = findPossibleCandidatesRow.Intersect(findPossibleCandidatesColumn).Intersect(findPossibleCandidatesSquare).ToList();
            // store
            p_sudokuCell.UpdateCandidates(possibleValues);
        }

        // TODO calculate automatically depending on sudoku size
        private IList<int> CalculateValidRowAndColNums(int p_sudokuSize)
        {
            return new List<int>()
            {
                1,4,7
            };
        }

        private IList<int> FindPossibleCandidatesRow(Sudoku p_sudoku, SudokuCell p_cell)
        {
            IList<int> possibleCandidates = new List<int>();
            
            IList<SudokuCell> row = p_sudoku.GetRow(p_cell.Row);
            List<int> alreadyUsedValues = row.Where(p_cell => p_cell.Value != 0).Select(p_cell => p_cell.Value).ToList();

            for (int i = 0; i < p_sudoku.Size; i++)
            {
                if (!alreadyUsedValues.Contains(i + 1))
                {
                    possibleCandidates.Add(i +1);
                }
            }
            
            return possibleCandidates;
        }

        private IList<int> FindPossibleCandidatesColumn(Sudoku p_sudoku, SudokuCell p_cell)
        {
            IList<int> possibleCandidates = new List<int>();
            
            IList<SudokuCell> column = p_sudoku.GetColumn(p_cell.Column);
            List<int> alreadyUsedValues = column.Where(p_cell => p_cell.Value != 0).Select(p_cell => p_cell.Value).ToList();

            for (int i = 0; i < p_sudoku.Size; i++)
            {
                if (!alreadyUsedValues.Contains(i + 1))
                {
                    possibleCandidates.Add(i +1);
                }
            }
            
            return possibleCandidates;
        }

        private IList<int> FindPossibleCandidatesSquare(Sudoku p_sudoku, SudokuCell p_cell)
        {
            IList<int> possibleCandidates = new List<int>();
            
            IList<SudokuCell> square = p_sudoku.GetSquare(p_cell.Row, p_cell.Column);
            List<int> alreadyUsedValues = square.Where(p_cell => p_cell.Value != 0).Select(p_cell => p_cell.Value).ToList();

            for (int i = 0; i < p_sudoku.Size; i++)
            {
                if (!alreadyUsedValues.Contains(i + 1))
                {
                    possibleCandidates.Add(i +1);
                }
            }
            
            return possibleCandidates;
        }
    }
}
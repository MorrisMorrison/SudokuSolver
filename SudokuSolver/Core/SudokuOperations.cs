using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace SudokuSolver
{
    public class SudokuOperations
    {
        public static int GetMissingValue(IList<int> p_values)
        {
            for (int i = 0; i < 9; i++)
            {
                if (!p_values.Contains(i + 1)) return i + 1;
            }

            return 0;
        }

        public static int GetUniqueCandidateValue(IList<SudokuCell> p_sudokuCells)
        {
            int[] count = new int[9];

            foreach (SudokuCell sudokuCell in p_sudokuCells)
            {
                if (sudokuCell.IsSolved) count[sudokuCell.Value - 1]++;

                foreach (int sudokuCellCandidate in sudokuCell.Candidates)
                {
                    count[sudokuCellCandidate - 1]++;
                }
            }

            return count.FirstOrDefault(p_value => p_value == 1);
        }

        public static Sudoku UpdateCandidates(Sudoku p_sudoku)
        {
            foreach (SudokuCell sudokuCell in p_sudoku.GetAll())
            {
                if (sudokuCell.Candidates.Contains(sudokuCell.Value)) sudokuCell.DeleteCandidate(sudokuCell.Value);
            }

            foreach (SudokuCell sudokuCell in p_sudoku.GetAll())
            {
                SudokuOperations.UpdateCandidatesInternal(p_sudoku, sudokuCell);
            }

            return p_sudoku;
        }

        private static void UpdateCandidatesInternal(Sudoku p_sudoku, SudokuCell p_sudokuCell)
        {
            IList<int> findPossibleCandidatesRow = FindPossibleCandidatesRow(p_sudoku, p_sudokuCell);
            IList<int> findPossibleCandidatesColumn = FindPossibleCandidatesColumn(p_sudoku, p_sudokuCell);
            IList<int> findPossibleCandidatesSquare = FindPossibleCandidatesSquare(p_sudoku, p_sudokuCell);
            IList<int> possibleValues = findPossibleCandidatesRow.Intersect(findPossibleCandidatesColumn)
                .Intersect(findPossibleCandidatesSquare).ToList();
            
            p_sudokuCell.UpdateCandidates(possibleValues);
        }

        private static IList<int> FindPossibleCandidatesRow(Sudoku p_sudoku, SudokuCell p_cell)
        {
            IList<int> possibleCandidates = new List<int>();

            IList<SudokuCell> row = p_sudoku.GetRow(p_cell.Row);
            List<int> alreadyUsedValues =
                row.Where(p_cell => p_cell.Value != 0).Select(p_cell => p_cell.Value).ToList();

            for (int i = 0; i < p_sudoku.Size; i++)
            {
                if (!alreadyUsedValues.Contains(i + 1))
                {
                    possibleCandidates.Add(i + 1);
                }
            }

            return possibleCandidates;
        }

        private static IList<int> FindPossibleCandidatesColumn(Sudoku p_sudoku, SudokuCell p_cell)
        {
            IList<int> possibleCandidates = new List<int>();

            IList<SudokuCell> column = p_sudoku.GetColumn(p_cell.Column);
            List<int> alreadyUsedValues =
                column.Where(p_cell => p_cell.Value != 0).Select(p_cell => p_cell.Value).ToList();

            for (int i = 0; i < p_sudoku.Size; i++)
            {
                if (!alreadyUsedValues.Contains(i + 1))
                {
                    possibleCandidates.Add(i + 1);
                }
            }

            return possibleCandidates;
        }

        private static IList<int> FindPossibleCandidatesSquare(Sudoku p_sudoku, SudokuCell p_cell)
        {
            IList<int> possibleCandidates = new List<int>();

            IList<SudokuCell> square = p_sudoku.GetSquare(p_cell.Row, p_cell.Column);
            List<int> alreadyUsedValues =
                square.Where(p_cell => p_cell.Value != 0).Select(p_cell => p_cell.Value).ToList();

            for (int i = 0; i < p_sudoku.Size; i++)
            {
                if (!alreadyUsedValues.Contains(i + 1))
                {
                    possibleCandidates.Add(i + 1);
                }
            }

            return possibleCandidates;
        }
    }
}
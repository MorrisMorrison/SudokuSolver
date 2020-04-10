
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualBasic;

namespace SudokuSolver
{
    public interface ISudokuSolver
    {
        public Sudoku Solve(Sudoku p_sudoku);
    }
    
    public class SudokuSolver:ISudokuSolver
    {
        public Sudoku Solve(Sudoku p_sudoku)
        {
            int[,] result = new int[p_sudoku.Size, p_sudoku.Size];
            bool isSolved = false;

            while (!isSolved)
            {
                // get all coordinates of unsolved cells
                IList<SudokuCell> unsolved = p_sudoku.GetUnsolved();
                if (unsolved.Count == 0)
                {
                    isSolved = true;
                }

                foreach (SudokuCell sudokuCell in unsolved)
                {
                    int nakedSingle = GetNakedSingle(p_sudoku, sudokuCell);
                    if (nakedSingle != 0)
                    {
                        sudokuCell.Value = nakedSingle;
                        p_sudoku.Update(sudokuCell);
                        continue;
                    }

                    int hiddenSingle = GetHiddenSingle(p_sudoku, sudokuCell);
                    if (hiddenSingle != 0)
                    {
                        sudokuCell.Value = hiddenSingle;
                        p_sudoku.Update(sudokuCell);
                        continue;
                    }

                    int lastMissingInRow = GetLastMissingInRow(p_sudoku, sudokuCell);
                    if (lastMissingInRow != 0)
                    {
                        sudokuCell.Value = lastMissingInRow;
                        p_sudoku.Update(sudokuCell);
                        continue;
                    }
                    
                    int lastMissingInColumn = GetLastMissingInColumn(p_sudoku, sudokuCell);
                    if (lastMissingInColumn != 0)
                    {
                        sudokuCell.Value = lastMissingInColumn;
                        p_sudoku.Update(sudokuCell);
                        continue;
                    }

                }
            }
            
            
            return p_sudoku;
        }

        
        private int GetLastMissingInRow(Sudoku p_sudoku, SudokuCell p_sudokuCell)
        {
            IList<SudokuCell> row = p_sudoku.GetRow(p_sudokuCell.Row);
            bool isLastMissingInRow = row.Where(p_cell => p_cell.Value == 0).Count() == 1;

            if (isLastMissingInRow)
            {
                for (int i = 0; i < p_sudoku.Size; i++)
                {
                    if (row.All(p_cell => p_cell.Value != i+1))
                    {
                        return i + 1;
                    }
                }    
            }

            return 0;
        }
        private int GetLastMissingInColumn(Sudoku p_sudoku, SudokuCell p_sudokuCell)
        {
            IList<SudokuCell> column = p_sudoku.GetColumn(p_sudokuCell.Column);
            bool isLastMissingInColumn = column.Where(p_cell => p_cell.Value == 0).Count() == 1;

            if (isLastMissingInColumn)
            {
                for (int i = 0; i < p_sudoku.Size; i++)
                {
                    if (column.All(p_cell => p_cell.Value != i+1))
                    {
                        return i + 1;
                    }
                }  
            }
            
            return 0;
        }



        private int GetNakedSingle(Sudoku p_sudoku, SudokuCell p_sudokuCell)
        {
            IList<SudokuCell> row = p_sudoku.GetRow(p_sudokuCell.Row);
            IList<SudokuCell> column = p_sudoku.GetColumn(p_sudokuCell.Column);
            IList<SudokuCell> square = p_sudoku.GetSquare(p_sudokuCell.Row, p_sudokuCell.Column);

            int missingValue = GetNakedSingleInternal(row, column, square);
            
            return missingValue;
        }

        private int GetNakedSingleInternal(IList<SudokuCell> p_row, IList<SudokuCell> p_column, IList<SudokuCell> p_square)
        {
            
            IList<int> candidates = new List<int>();
            for (int i = 0; i < p_row.Count; i++)
            {
                bool isMissingInRow = p_row.All(p_cell => p_cell.Value != i + 1);
                bool isMissingInColumn = p_column.All(p_cell => p_cell.Value != i + 1);
                bool isMissingInSquare = p_square.All(p_cell => p_cell.Value != i + 1);

                if (isMissingInRow && isMissingInColumn && isMissingInSquare)
                {
                    candidates.Add(i+1);
                }
            }

            if (candidates.Count == 1)
            {
                return candidates.FirstOrDefault();
            }
            
            return 0;
        }


        public int GetHiddenSingle(Sudoku p_sudoku, SudokuCell p_sudokuCell)
        {
            int missingValue = 0;
            IList<SudokuCell> square = p_sudoku.GetSquare(p_sudokuCell.Row, p_sudokuCell.Column);

            
            
            if ((p_sudokuCell.Row == 1 || p_sudokuCell.Row == 4 || p_sudokuCell.Row == 7) && (p_sudokuCell.Column == 1 || p_sudokuCell.Column == 4 || p_sudokuCell.Column == 7))
            {
                IList<SudokuCell> previousRow = p_sudokuCell.Row == 0 ? new List<SudokuCell>() : p_sudoku.GetRow(p_sudokuCell.Row - 1);
                IList<SudokuCell> row = p_sudoku.GetRow(p_sudokuCell.Row);
                IList<SudokuCell> column = p_sudoku.GetColumn(p_sudokuCell.Column);
                IList<SudokuCell> followingRow = p_sudokuCell.Row == p_sudoku.Size -1 ? new List<SudokuCell>() : p_sudoku.GetRow(p_sudokuCell.Row + 1);

                missingValue = GetHiddenSingleInternal( square,previousRow, row,  followingRow, column);

                if (missingValue == 0)
                {
                    IList<SudokuCell> previousColumn = p_sudokuCell.Column == 0 ? new List<SudokuCell>() : p_sudoku.GetColumn(p_sudokuCell.Column - 1);
                    IList<SudokuCell> followingColumn = p_sudokuCell.Column == p_sudoku.Size -1 ?  new List<SudokuCell>() : p_sudoku.GetColumn(p_sudokuCell.Column + 1);
                    missingValue = GetHiddenSingleInternal(square,previousColumn, column, followingColumn, row);
                }

            }
            
            
            return missingValue;
        }

        // following - nachfolger
        // previous - vorgänger
        
        private int GetHiddenSingleInternal(IList<SudokuCell> p_square, IList<SudokuCell> p_previous,  IList<SudokuCell> p_current, IList<SudokuCell> p_following, IList<SudokuCell> p_toCompare)
        {
            // hiddenSingle = number is missing in current square && number is present in both prependign and ascending rows or columns
            
            IList<int> candidates = new List<int>();
            for (int i = 0; i < p_current.Count; i++)
            {
                bool isMissingInSquare = p_square.All(p_cell => p_cell.Value != i + 1);
                
                bool presentInPrevious = p_previous.Any(p_cell => p_cell.Value == i +1);
                bool presentInFollowing = p_following.Any(p_cell => p_cell.Value == i + 1);
                bool presentInCurrent = p_current.Any(p_cell => p_cell.Value == i + 1);
                bool presentInToCompare = p_toCompare.Any(p_cell => p_cell.Value == i + 1);


                bool isHiddenSingle = isMissingInSquare && presentInFollowing && presentInPrevious &&
                                      !presentInCurrent && !presentInToCompare;
                if (isHiddenSingle)
                {
                    candidates.Add(i+1);
                }
            }

            if (candidates.Count == 1)
            {
                return candidates.FirstOrDefault();
            }

            return 0;
        }
        
        public class GetHiddenSingleParameters
        {
            public IList<SudokuCell> PreviousRow { get; set;}
            public IList<SudokuCell> CurrentRow { get; set;}
            public IList<SudokuCell> FollowingRow { get; set;}
            public IList<SudokuCell> PreviousColumn { get; set;}
            public IList<SudokuCell> CurrentColumn { get; set;}
            public IList<SudokuCell> FollowingColumn { get; set;}
            public IList<SudokuCell> Square { get; set;}
        }

    }
}
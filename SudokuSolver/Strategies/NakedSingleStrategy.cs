using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Strategies
{
    public class NakedSingleStrategy: ISolvingStrategy
    {
        public Sudoku Solve(Sudoku p_sudoku)
        {
            int lastUnsolvedCellsCount = 0;
            bool isFinished = false;

            while (!isFinished)
            {
                IList<SudokuCell> unsolvedCells = p_sudoku.GetUnsolved();
                if (lastUnsolvedCellsCount == unsolvedCells.Count ||unsolvedCells.Count == 0) isFinished = true;
                lastUnsolvedCellsCount = unsolvedCells.Count;
                
                foreach (SudokuCell sudokuCell in unsolvedCells)
                {
                    int findNakedSingle = FindNakedSingle(p_sudoku, sudokuCell);
                    if (findNakedSingle != 0)
                    {
                        sudokuCell.Value = findNakedSingle;
                        p_sudoku.Update(sudokuCell);
                    }
                }    
            }

            return p_sudoku;
        }

        private int FindNakedSingle(Sudoku p_sudoku, SudokuCell p_sudokuCell)
        {
            IList<SudokuCell> row = p_sudoku.GetRow(p_sudokuCell.Row);
            IList<SudokuCell> column = p_sudoku.GetColumn(p_sudokuCell.Column);
            IList<SudokuCell> square = p_sudoku.GetSquare(p_sudokuCell.Row, p_sudokuCell.Column);

            int missingValue = FindNakedSingleInternal(p_sudokuCell,row, column, square);

            return missingValue;
        }

        private int FindNakedSingleInternal(SudokuCell p_sudokuCell,IList<SudokuCell> p_row, IList<SudokuCell> p_column,
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

            if(candidates.Count == 1){
                return candidates.FirstOrDefault();
            }

            p_sudokuCell.AddCandidates(candidates);
            return 0;
        }
    }
}
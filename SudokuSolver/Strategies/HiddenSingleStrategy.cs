using System.Collections.Generic;

namespace SudokuSolver.Strategies
{
    public class HiddenSingleStrategy: ISolvingStrategy
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
                    int findHiddenSingle = FindHiddenSingle(p_sudoku, sudokuCell);
                    if (findHiddenSingle != 0)
                    {
                        sudokuCell.Value = findHiddenSingle;
                        p_sudoku.Update(sudokuCell);
                    }
                }    
                
                p_sudoku = SudokuOperations.UpdateCandidates(p_sudoku);
            }

            return p_sudoku;
        }
        
        private int FindHiddenSingle(Sudoku p_sudoku, SudokuCell p_sudokuCell)
        {
            IList<SudokuCell> row = p_sudoku.GetRow(p_sudokuCell.Row);
            IList<SudokuCell> column = p_sudoku.GetColumn(p_sudokuCell.Column);
            IList<SudokuCell> square = p_sudoku.GetSquare(p_sudokuCell.Row, p_sudokuCell.Column);

            int uniqueCandidateValueRow = SudokuOperations.GetUniqueCandidateValue(row);
            if (uniqueCandidateValueRow != 0) return uniqueCandidateValueRow;
            
            int uniqueCandidateValueColumn = SudokuOperations.GetUniqueCandidateValue(column);
            if (uniqueCandidateValueColumn != 0) return uniqueCandidateValueColumn;

            int uniqueCandidateValueSquare = SudokuOperations.GetUniqueCandidateValue(square);
            if (uniqueCandidateValueSquare != 0) return uniqueCandidateValueSquare;
            
            return 0;
        }
    }
}
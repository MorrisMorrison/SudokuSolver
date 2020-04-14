using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualBasic;

namespace SudokuSolver
{
    /// <summary>
    /// Workflow.
    /// </summary>
    public interface ISudokuSolver
    {
        ISudokuSolvingLogic SolvingLogic { get; set; }
        public Sudoku Solve(Sudoku p_sudoku);
    }

    /// <summary>
    /// Extensible workflow.
    /// </summary>
    public interface IExtensibleSudokuSolver
    {
        ISudokuSolvingLogic SolvingLogic { get; set; }
        public Sudoku Solve(Sudoku p_sudoku, Func<Sudoku, SudokuCell, int> p_additionalSolvingLogic);
    }

    /// <summary>
    /// Solve Sudoku by using the strategies naked single and hidden single
    /// </summary>
    public class SimpleSudokuSolver : IExtensibleSudokuSolver
    {
        public ISudokuSolvingLogic SolvingLogic { get; set; }

        public SimpleSudokuSolver(ISudokuSolvingLogic p_solvingLogic)
        {
            SolvingLogic = p_solvingLogic;
        }

        public Sudoku Solve(Sudoku p_sudoku, Func<Sudoku, SudokuCell, int> p_additionalSolvingLogic = null)
        {
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
                    int nakedSingle = SolvingLogic.FindNakedSingle(p_sudoku, sudokuCell);
                    if (nakedSingle != 0)
                    {
                        sudokuCell.Value = nakedSingle;
                        p_sudoku.Update(sudokuCell);
                        continue;
                    }

                    int hiddenSingle = SolvingLogic.FindHiddenSingle(p_sudoku, sudokuCell);
                    if (hiddenSingle != 0)
                    {
                        sudokuCell.Value = hiddenSingle;
                        p_sudoku.Update(sudokuCell);
                        continue;
                    }

                    if (p_additionalSolvingLogic != null)
                    {
                        int value = p_additionalSolvingLogic.Invoke(p_sudoku, sudokuCell);
                        sudokuCell.Value = value;
                        p_sudoku.Update(sudokuCell);
                        continue;
                    }
                    
                }
            }


            return p_sudoku;
        }



    }

    public class AdvancedSudokuSolver:ISudokuSolver
    {
        public ISudokuSolvingLogic SolvingLogic { get; set; }

        public AdvancedSudokuSolver(ISudokuSolvingLogic p_solvingLogic)
        {
            SolvingLogic = p_solvingLogic;
        }

        public Sudoku Solve(Sudoku p_sudoku)
        {
            SimpleSudokuSolver simpleSudokuSolver = new SimpleSudokuSolver(SolvingLogic);
            return simpleSudokuSolver.Solve(p_sudoku, (p_sudoku, p_cell) =>
            {
                
                
                
                return 0;
            });
        }

    }
    
    
}
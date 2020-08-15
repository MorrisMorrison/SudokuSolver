using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualBasic;
using SudokuSolver.Strategies;

namespace SudokuSolver
{
    /// <summary>
    /// Workflow.
    /// </summary>
    public interface ISudokuSolver
    {
        IList<ISolvingStrategy> SolvingStrategies { get; set; }
        public Sudoku Solve(Sudoku p_sudoku, Func<Sudoku, SudokuCell, int> p_additionalSolvingLogic = null);
    }

    /// <summary>
    /// Solve Sudoku by using the strategies naked single and hidden single
    /// </summary>
    public class SudokuSolver : ISudokuSolver
    {
        public IList<ISolvingStrategy> SolvingStrategies { get; set; }
        private ISolvingStrategy _currentStrategy { get; set; }
        
        public SudokuSolver(IList<ISolvingStrategy> p_solvingStrategies)
        {
            SolvingStrategies = p_solvingStrategies;
        }

        public Sudoku Solve(Sudoku p_sudoku, Func<Sudoku, SudokuCell, int> p_additionalSolvingLogic = null)
        {
            Queue<ISolvingStrategy> strategyQueue = new Queue<ISolvingStrategy>(SolvingStrategies);
            
            bool isSolved = false;

            while (!isSolved)
            {
                _currentStrategy = strategyQueue.Peek();
                
                Sudoku sudoku = _currentStrategy.Solve(p_sudoku);
                if (sudoku.IsSolved()) isSolved = true;

                strategyQueue.Dequeue();
                p_sudoku = SudokuOperations.UpdateCandidates(p_sudoku);
            }

            return p_sudoku;
        }
    }
    
}
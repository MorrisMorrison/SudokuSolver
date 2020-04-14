using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "/home/morrismorrison/Development/Projects/NET/SudokuSolver/SudokuSolver/Data/sudoku.csv";
            ISudokuAccess sudokuAccess = new NumPySudokuAccess(9);
            IList<Sudoku> readPuzzles = sudokuAccess.ReadPuzzles(path);
            IList<Sudoku> readSolutions = sudokuAccess.ReadSolutions(path);
            
            SudokuSolvingLogic logic = new SudokuSolvingLogic();
            SimpleSudokuSolver solver = new SimpleSudokuSolver(logic);


            for (int i = 0; i < readPuzzles.Count; i++)
            {
                Sudoku possibleSolution = solver.Solve(readPuzzles[i]);
                Sudoku actualSolution = readSolutions[i];

                bool solved = actualSolution.Equals(possibleSolution);

                // Console.WriteLine(possibleSolution);
                // Console.WriteLine(actualSolution);
                Console.WriteLine("Sudoku "+ i + ": " + (solved ? "SOLVE" : "FAIL"));
            }
            
            // Console.WriteLine(possibleSolution.ToString());
            // Console.WriteLine(readSolutions.FirstOrDefault().ToString());
        }
    }
}
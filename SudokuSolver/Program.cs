using System;
using System.Collections.Generic;
using System.Linq;
using SimpleConfigAccess.Config;
using SudokuSolver.Strategies;

namespace SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Config config = new Config("../../../appsettings.json");
            string path = (string) config["SudokuSolver:Path"];
            
            ISudokuAccess sudokuAccess = new NumPySudokuAccess(9);
            IList<Sudoku> readPuzzles = sudokuAccess.ReadPuzzles(path);
            IList<Sudoku> readSolutions = sudokuAccess.ReadSolutions(path);

            IList<string> strategies = (IList<string>) config["SudokuSolver:Strategies"];
            
            List<ISolvingStrategy> solvingStrategies = new List<ISolvingStrategy>();
            if (strategies.Count < 1)
            {
                solvingStrategies.Add(new HiddenSingleStrategy());
                solvingStrategies.Add(new NakedSingleStrategy());
            }
            
            if (strategies.Contains("NakedSingle"))
            {
                solvingStrategies.Add(new NakedSingleStrategy());
            }
            
            if (strategies.Contains("HiddenSingle"))
            {
                solvingStrategies.Add(new HiddenSingleStrategy());
            }

        

            SudokuSolver solver = new SudokuSolver(solvingStrategies);

            for (int i = 0; i < readPuzzles.Count; i++)
            {
                Sudoku possibleSolution = solver.Solve(readPuzzles[i]);
                Sudoku actualSolution = readSolutions[i];

                bool solved = actualSolution.Equals(possibleSolution);
                
                Console.WriteLine("Sudoku " + i + ": ");
                Console.WriteLine("Calculated Solution:");
                Console.WriteLine(possibleSolution.ToString());
                Console.WriteLine("Actual Solution:");
                Console.WriteLine(actualSolution.ToString());
                Console.WriteLine("Result: " + (solved ? "SOLVED" : "FAIL"));
                Console.WriteLine("-------");
            }
            
        }
    }
}
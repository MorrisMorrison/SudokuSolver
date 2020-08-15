# SudokuSolver
This is a simple console application and/or library that allows solving simple sudoku puzzles.

The internal core library could also be extracted, to provide an easy to use option to read and manipulate puzzles.

The Solver itself is implemented extensible, that means it is possible to add custom strategies.
Internally these strategies get queued up and executed one after another. If a strategy isn't successful the next strategy of the queue gets loaded and executed.

The interal library allows reading puzzle data into Sudoku objects that provide convenient methods to interact with it.
It also provides a class to interact with SudokuCells and to manage its candidates.

## Examples
### Solver
```csharp
Sudoku sudokuPuzzle = sudokuAccess.ReadPuzzles(path).FirstOrDefault();

IList<ISolvingStrategy> strategies = new List<ISolvingStrategy>(){
    new NakedSingleStrategy(),
    new HiddenSingleStrateg(),
};

SudokuSolver solver = new SudokuSolver(strategies);
Sudoku result = solver.Solve(sudokuPuzzle);
``` 
### CustomStrategy
```csharp
public class CustomStrategy:ISolvingStrategy{
    public Sudoku Solve(Sudoku p_sudoku){
     // TODO solve     
    }
}
``` 

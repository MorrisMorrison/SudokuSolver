using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace SudokuSolver
{

    public interface ISudokuAccess
    {
        public IList<Sudoku> ReadPuzzles(string p_path, int p_columnIndex = 0);
        public IList<Sudoku> ReadSolutions(string p_path);
    }
    
    // different implementations for different kinds of input data
    public class NumPySudokuAccess:ISudokuAccess
    {
        
        private int _size { get; set; }

        public NumPySudokuAccess(int p_size)
        {
            _size = p_size;
        }

        public IList<Sudoku> ReadPuzzles(string p_path, int p_columnIndex = 0)
        {
            IList<Sudoku> result = new List<Sudoku>();
            
            using (TextFieldParser parser = new TextFieldParser(p_path))
            {
                parser.CommentTokens = new string[] { "#" };
                parser.SetDelimiters(new string[] { "," });
                
                // Skip first row
                parser.ReadLine();

                while (parser.LineNumber < 1000)
                {
                    string[] fields = parser.ReadFields();
                    string puzzle = fields[p_columnIndex];
                    
                    int[,] puzzleData = new int[_size, _size];

                    for (int i = 0; i < _size; i++)
                    {
                        for (int j = 0; j < _size; j++)
                        {
                            // row 1 skip 9, also start 9
                            // row 2, also start 18
                            
                            // find starting index of where to parse from
                            int index = i == 0 ? j: i * _size + j;
                            
                            int number = int.Parse(puzzle[index].ToString());
                            puzzleData[i,j] = number;
                        }
                    }
                 
                    result.Add(new Sudoku(_size, puzzleData));
                }
            }

            return result;
        }

        public IList<Sudoku> ReadSolutions(string p_path)
        {
            return ReadPuzzles(p_path, 1);
        }
    }


    
    
}
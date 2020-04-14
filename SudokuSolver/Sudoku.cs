using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;

namespace SudokuSolver
{
    /// <summary>
    /// Represents a single Sudoku puzzle.
    /// </summary>
    public class Sudoku
    {
        public int Size { get; set; }
        private int[,] _puzzleData { get; set; }

        public Sudoku(int p_size, int[,] p_puzzleData)
        {
            Size = p_size;
            _puzzleData = p_puzzleData;
        }

        public int this[int p_rowIndex, int p_columnIndex]
        {
            get { return _puzzleData[p_rowIndex, p_columnIndex]; }
        }

        public int[] GetRowValues(int p_rowIndex)
        {
            int[] row = new int[Size];
            for (int i = 0; i < Size; i++)
            {
                row[i] = _puzzleData[p_rowIndex, i];
            }

            return row;
        }

        public int[] GetColumnValues(int p_columnIndex)
        {
            int[] column = new int[Size];

            for (int i = 0; i < Size; i++)
            {
                column[i] =_puzzleData[i, p_columnIndex];
            }

            return column;
        }

        public int[] GetSquareValues(int p_rowIndex, int p_columnIndex)
        {
            int[] square = new int[Size];
            int squareRow = p_rowIndex != 0 ? Size % p_rowIndex: p_rowIndex;
            int squareColumn = p_columnIndex != 0? Size % p_columnIndex: p_columnIndex;
            int index = 0;
            
            for (int i = 0 + squareRow * 3; i < 3 * (squareRow +1); i++)
            {
                for (int j = 0 + squareColumn * 3; j < 3 * (squareColumn+1); j++)
                {
                    square[index] =_puzzleData[i, j];
                    index++;
                }
            }

            return square;
        }

        public IList<List<SudokuCell>> GetRows()
        {
            IList<List<SudokuCell>> result = new List<List<SudokuCell>>();
            for (int i = 0; i < Size; i++)
            {
                result.Add(GetRow(i).ToList());
            }

            return result;
        }
        
        public IList<SudokuCell> GetRow(int p_rowIndex)
        {
            IList<SudokuCell> row = new List<SudokuCell>();
            
            for (int i = 0; i < Size; i++)
            {
                row.Add(new SudokuCell()
                {
                    Row = p_rowIndex,
                    Column = i,
                    Value = _puzzleData[p_rowIndex, i]
                });
            }

            return row;
        }
        
        public IList<SudokuCell> GetColumn(int p_columnIndex)
        {
            IList<SudokuCell> column = new List<SudokuCell>();

            for (int i = 0; i < Size; i++)
            {
                column.Add(new SudokuCell()
                {
                   Row = i,
                   Column =  p_columnIndex,
                   Value = _puzzleData[i, p_columnIndex]
                });
            }

            return column;
        }
        
        public IList<SudokuCell> GetSquare(int p_rowIndex, int p_columnIndex)
        {
            IList<SudokuCell> square = new List<SudokuCell>();
            int squareRow = p_rowIndex == 0 ? p_rowIndex: p_rowIndex / (Size / 3);
            int squareColumn = p_columnIndex == 0 ? p_columnIndex: p_columnIndex /(Size/3);
            int index = 0;
            
            for (int i = 0 + squareRow * 3; i < 3 * (squareRow +1); i++)
            {
                for (int j = 0 + squareColumn * 3; j < 3 * (squareColumn+1); j++)
                {
                    square.Add(new SudokuCell()
                    {
                        Row = i,
                        Column = j,
                        Value = _puzzleData[i, j]
                    });
                    index++;
                }
            }

            return square;
        }
        
        public IList<SudokuCell> GetAll()
        {
            IList<SudokuCell> cells = new List<SudokuCell>();

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    cells.Add(new SudokuCell()
                    {
                        Row = i,
                        Column = j,
                        Value = _puzzleData[i, j]
                    });
                }    
            }
            
            return cells;
        }

        public IList<SudokuCell> GetUnsolved()
        {
            return GetAll().Where(p_cell => p_cell.Value == 0).ToList();
        }
        
        public bool IsSolved(int p_rowIndex, int p_columnIndex)
        {
            return _puzzleData[p_rowIndex, p_columnIndex] != 0;
        }

        public void Update(SudokuCell p_sudokuCell)
        {
            _puzzleData[p_sudokuCell.Row, p_sudokuCell.Column] = p_sudokuCell.Value;
        }

        public bool Equals(Sudoku p_sudoku)
        {
            IList<SudokuCell> sudokuCells = GetAll();

            foreach (SudokuCell sudokuCell in p_sudoku.GetAll())
            {
                if (!sudokuCells.Any(p_cell => p_cell.Row == sudokuCell.Row && p_cell.Column == sudokuCell.Column && p_cell.Value == sudokuCell.Value))
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            
            IList<List<SudokuCell>> allRows = GetRows();
            foreach (List<SudokuCell> row in allRows)
            {
                foreach (SudokuCell sudokuCell in row)
                {
                    builder.Append(sudokuCell.Value + " | ");
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }
        
    }

    /// <summary>
    /// Represents a single cell in a sudoku puzzle.
    /// </summary>
    public class SudokuCell
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Value { get; set; }
        
        // work with array internally to ensure the values 1-9
        private int[] _candidates { get; set; } = new int[9];

        public IList<int> Candidates
        {
            get { return _candidates.Select(p_candidate => p_candidate +1).ToList(); }
        }

        public void AddCandidate(int p_candidate)
        {
                _candidates[p_candidate-1] = 1;
        }

        public void AddCandidates(IList<int> p_candidates)
        {
            foreach (int candidate in p_candidates)
            {
                _candidates[candidate] = 1;
            }
        }
        
        public void UpdateCandidates(IList<int> p_candidates)
        {
            foreach (int candidate in _candidates)
            {
                if (p_candidates.Contains(candidate))
                {
                    _candidates[candidate] = 1;
                }
                else
                {
                    _candidates[candidate] = 0;
                }
            }
        }

        public void DeleteCandidate(int p_candidate)
        {
                _candidates[p_candidate-1] = 0;
        }

        public void DeleteCandidates(IList<int> p_candidates)
        {
            foreach (int candidate in p_candidates)
            {
                _candidates[candidate] = 0;
            }
        }
        
        public bool IsSolved
        {
            get { return Value != 0; }
        }

        public bool Equals(SudokuCell p_cell)
        {
            return p_cell.Row == Row && p_cell.Column == Column && p_cell.Value == Value;
        }
        
    }
}
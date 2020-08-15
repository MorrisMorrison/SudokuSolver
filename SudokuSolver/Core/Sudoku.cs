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
        private int[,] _data { get; set; }
        
        public bool IsSolved() => GetAll().All(p_cell => p_cell.IsSolved);

        public int this[int p_rowIndex, int p_columnIndex]
        {
            get { return _data[p_rowIndex, p_columnIndex]; }
        }

        public Sudoku(int p_size, int[,] p_data)
        {
            Size = p_size;
            _data = p_data;
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
                    Value = _data[p_rowIndex, i]
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
                   Value = _data[i, p_columnIndex]
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
                        Value = _data[i, j]
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
                        Value = _data[i, j]
                    });
                }    
            }
            
            return cells;
        }

        public IList<SudokuCell> GetUnsolved()
        {
            return GetAll().Where(p_cell => p_cell.Value == 0).ToList();
        }

        public void Update(SudokuCell p_sudokuCell)
        {
            _data[p_sudokuCell.Row, p_sudokuCell.Column] = p_sudokuCell.Value;
        }

        public bool Equals(Sudoku p_sudoku)
        {
            IList<SudokuCell> sudokuCells = GetAll();

            foreach (SudokuCell sudokuCell in p_sudoku.GetAll())
            {
                if (!sudokuCells.Any(p_cell => p_cell.Row == sudokuCell.Row 
                                               && p_cell.Column == sudokuCell.Column 
                                               && p_cell.Value == sudokuCell.Value))
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
        
        private int[] _candidates { get; } = new int[9];

        public IList<int> Candidates
        {
            get { return _candidates.Where(p_value => p_value != 0).ToList(); }
        }
        
        public void AddCandidate(int p_candidate)
        {
                _candidates[p_candidate-1] = 1;
        }

        public void AddCandidates(IList<int> p_candidates)
        {
            foreach (int candidate in p_candidates)
            {
                _candidates[candidate-1] = 1;
            }
        }
        
        public void UpdateCandidates(IList<int> p_candidates)
        {
            for (int i= 0; i < 9; i++){
                if (p_candidates.Contains(i+1)) _candidates[i] =1;
                else{ _candidates[i] = 0;}
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
                _candidates[candidate-1] = 0;
            }
        }
        
        public bool IsSolved => Value != 0;

        public bool Equals(SudokuCell p_cell)
        {
            return p_cell.Row == Row && p_cell.Column == Column && p_cell.Value == Value;
        }
        
    }
}
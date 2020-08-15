using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace SudokuSolver
{
    public class Utils
    {
        public static int GetMissingValue(IList<int> p_values){
            for (int i = 0; i < 9; i++){
                if (!p_values.Contains(i+1)) return i+1;
            }
            return 0;
        }

        public static int GetUniqueCandidateValue(IList<SudokuCell> p_sudokuCells)
        {
            int[] count = new int[9];
            
            foreach (SudokuCell sudokuCell in p_sudokuCells)
            {
                if (sudokuCell.IsSolved) count[sudokuCell.Value - 1]++;
                
                foreach (int sudokuCellCandidate in sudokuCell.Candidates)
                {
                    count[sudokuCellCandidate - 1]++;
                }
            }

            return count.FirstOrDefault(p_value => p_value == 1);
        }
    }
}
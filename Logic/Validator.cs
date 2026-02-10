using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Logic
{
    public class Validator
    {
        private Grid grid;

        public Validator(Grid grid) {  this.grid = grid; }

        public bool IsValid(int row, int col, int value)
        {
            //verificam coloanele
            for (int c = 0; c < grid.Size; c++)
            {
                if (grid.GetValue(row, c) == value)
                    return false;
            }

            //verificam liniile
            for (int r = 0; r < grid.Size; r++)
            {
                if (grid.GetValue(r, col) == value)
                    return false;
            }


            //gasim patratul 3x3
            int sqrt = (int)Math.Sqrt(grid.Size);
            int boxRow = (row / sqrt) * sqrt;
            int boxCol = (col / sqrt) * sqrt;


            //verificam daca regasim valoarea in patratul 3x3
            for (int r = 0; r < sqrt; r++)
                for (int c = 0; c < sqrt; c++)
                    if (grid.GetValue(boxRow + r, boxCol + c) == value)
                        return false;

            return true;

        }
        
    }
}

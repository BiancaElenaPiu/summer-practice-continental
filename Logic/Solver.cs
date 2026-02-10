using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Logic
{
    public class Solver
    {
        public Grid grid;
        private Validator validator;
        public int size;

        public Solver(Grid grid, int size)
        {
            this.grid = grid;
            this.validator = new Validator(grid);
            this.size = size;
        }

        public bool Solve()
        {
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    //se cauta celula goala
                    if (grid.GetValue(row, col) == 0)
                    {
                        //se incearca valorile posibile
                        for (int num = 1; num <= size; num++)
                        {
                            //verificam daca respecta regulile
                            if (validator.IsValid(row, col, num))
                            {
                                grid.SetValue(row, col, num);

                                //rezolvam recursiv 
                                if (Solve())
                                    return true;
                                grid.SetValue(row, col, 0); // backtracking
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

    }
}

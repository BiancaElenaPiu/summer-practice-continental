using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuSolver.Logic
{
    public class Generator
    {
        private Random rnd = new Random();
        public int size;


        public Generator(int size)
        {
            this.size = size;
        }





        //---------------------------------------------------------
        // pentru N x N 
        //---------------------------------------------------------


        public Grid GeneratePuzzle(int clues, int n)
        {
            if (clues < n || clues > n * n)
                throw new Exception("Număr invalid de indicii");

            if (!IsPerfectSquare(n))
                throw new Exception("Dimensiunea grilei trebuie să fie pătratul unui număr întreg");

            //var grid = new Grid(n);

            int attempts = 0;
            const int maxAttempts = 5;

            while (attempts < maxAttempts)
            {
                Grid grid = new Grid(n);
                if (FillGridRandom(grid, n))
                {
                    RemoveRandomCells(grid, (n * n) - clues);
                    return grid;
                }
                attempts++;
                MessageBox.Show($"{attempts}");
            }
            throw new Exception("Nu s-a putut genera un puzzle valid după " + maxAttempts + " încercări.");
        }



        private bool FillGridRandom(Grid grid, int n)
        {
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    if (grid.GetValue(r, c) == 0)
                    {
                        foreach (int num in Enumerable.Range(1, n).OrderBy(_ => rnd.Next()))
                        {
                            if (new Validator(grid).IsValid(r, c, num))
                            {
                                grid.SetValue(r, c, num);
                                if (FillGridRandom(grid, n))
                                    return true;
                                grid.SetValue(r, c, 0);
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

       
        
        private void RemoveRandomCells(Grid grid, int count)
        {
            int n = grid.Size;

            //int n = 9;
            while (count > 0)
            {
                int r = rnd.Next(0, n);
                int c = rnd.Next(0, n);

                if (grid.GetValue(r, c) != 0)
                {
                    grid.SetValue(r, c, 0);
                    count--;
                }
            }
        }

        private bool IsPerfectSquare(int n)
        {
            int root = (int)Math.Sqrt(n);
            return root * root == n;
        }

    }
}


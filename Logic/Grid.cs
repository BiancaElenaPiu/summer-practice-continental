using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Logic
{
    public class Grid
    {
        public int[,] cells { get; private set; }
        public int Size { get; }


        public Grid(int size)
        {
            Size = size;
            cells = new int[size, size];
        }

        public int GetValue (int row, int col) => cells[row, col];

        public void SetValue (int row, int col, int value) => cells[row, col] = value;

        public Grid Clone()
        {
            Grid g = new Grid(this.Size);
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    g.SetValue(r, c, this.GetValue(r, c));
            return g;
        }


    }
}

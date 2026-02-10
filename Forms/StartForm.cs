using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuSolver
{
    public partial class StartForm : Form
    {
        public int level;
        public int items;
        public StartForm()
        {
            InitializeComponent();
            comboBoxLevel.SelectedIndex = 0;
            comboBoxItems.SelectedIndex = 0;

            level = 1;
            items = 9;
            
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            int level = this.level;
            int items = this.items;

            if (items != 9 && items != 16)
            {
                MessageBox.Show("Selectează un număr valid de elemente!");
                return;
            }

            SudokuForm sudokuForm = new SudokuForm(level, items);
            sudokuForm.Show();
            this.Hide();
        }

        private void comboBoxLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            //easy
            if(comboBoxLevel.SelectedIndex == 0)
            {
                level = 1;
            }
            else if(comboBoxLevel.SelectedIndex == 1)
            {
                level = 2;
            }
            else
            {
                level = 3;
            }
        }

        private void comboBoxItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBoxItems.SelectedIndex == 0)
            {
                items = 9;
            }
            else if(comboBoxItems.SelectedIndex == 1)
            {
                items = 16;
            }
        }

       
    }
}

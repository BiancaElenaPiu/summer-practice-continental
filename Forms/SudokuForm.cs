using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SudokuSolver.Logic;

namespace SudokuSolver
{
    public partial class SudokuForm : Form
    {
        TextBox[,] grid;
        private Logic.Grid logicalGrid;
        private Logic.Grid solvedGrid;
        private Label mistakesLabel = new Label();


        bool[,] countedMistakes;

        int size;

        int mistakes = 0;
        int level;


        private Panel gridPanel;





        private Timer gameTimer = new Timer();
        private int seconds = 0;
        private Label timerLabel = new Label();
        private int selectedDigit = -1;

        int clues = 30;

        public SudokuForm(int level, int items)
        {

            
            this.level = level;

                InitializeComponent();

                this.size = items;

           // this.AutoScroll = true;
                GenerateSudokuGrid();

                if (level == 1)
                {
                    easyToolStripMenuItem_Click(new object(), new EventArgs());

                }
                else if (level == 2)
                {
                    mediumToolStripMenuItem_Click(new object(), new EventArgs());
                }
                else
                {
                    hardToolStripMenuItem_Click(new object(), new EventArgs());
                }

           

        }

        
        private void SetupTimerLabel()
        {
            timerLabel.AutoSize = true;
            timerLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            timerLabel.Location = new Point(10, 30); // pozitie deasupra butoanelor
            timerLabel.Text = "Timp: 0s";
            this.Controls.Add(timerLabel);
        }

        private void SetupTimer()
        {
            gameTimer.Interval = 1000; // 1s
            gameTimer.Tick += GameTimer_Tick;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            seconds++;
            timerLabel.Text = $"Timp: {seconds}s";
        }

        private void GenerateSudokuGrid()
        {
            // curata controalele existente
            //this.Controls.Clear();

            int margin = 20;
            int gap = 4;
            int blockSize = (int)Math.Sqrt(size);
            int cellSize = size <= 9 ? 40 : size <= 16 ? 30 : 24;

            grid = new TextBox[size, size];

            int gridWidth = size * cellSize + (blockSize - 1) * gap;
            int gridHeight = size * cellSize + (blockSize - 1) * gap;

            int startX = margin;
            int startY = margin + 50; //pastram loc pt timer

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    TextBox tb = new TextBox
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Multiline = true,
                        Font = new Font("Arial", 16),
                        TextAlign = HorizontalAlignment.Center,
                        MaxLength = 2
                    };

                    tb.Left = startX + col * cellSize + (col / blockSize) * gap;
                    tb.Top = startY + row * cellSize + (row / blockSize) * gap;

                    //control pentru caractere
                    tb.KeyPress += (s, e) =>
                    {
                        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                            e.Handled = true;
                    };

                    int r = row, c = col;
                    tb.Click += (s, e) =>
                    {
                        HighlightCells(r, c);
                        if (!tb.ReadOnly && selectedDigit != -1)
                            tb.Text = selectedDigit.ToString();
                    };

                    grid[row, col] = tb;
                    this.Controls.Add(tb);

                    for (int i = 1; i < blockSize; i++)
                    {
                        // linie orizontala
                        Label hLine = new Label
                        {
                            BackColor = Color.Black,
                            Width = size * cellSize + (blockSize - 1) * gap,
                            Height = 2,
                            Left = startX,
                            Top = startY + i * blockSize * cellSize + (i - 1) * gap
                        };
                        this.Controls.Add(hLine);

                        // linie verticala
                        Label vLine = new Label
                        {
                            BackColor = Color.Black,
                            Width = 2,
                            Height = size * cellSize + (blockSize - 1) * gap,
                            Top = startY,
                            Left = startX + i * blockSize * cellSize + (i - 1) * gap
                        };
                        this.Controls.Add(vLine);
                    }
                }
            }

            //timer
            SetupTimerLabel();
            SetupTimer();


            int lastButton = GenerateButtons(startY + gridHeight + margin);


            PlaceButtons(lastButton + 10);

           
            this.ClientSize = new Size(gridWidth + 2 * margin, lastButton + 80);

           // MessageBox.Show("Am aj aici");
            buttonNew_Click(new object(), new EventArgs());
        }




        private int GenerateButtons(int topStart)
        {
            int buttonSize = 35;
            int spacing = 5;
            int buttonsPerRow = Math.Min(size, 9); // max 9 pe rând
            int numRows = (int)Math.Ceiling((double)size / buttonsPerRow);

            int panelWidth = buttonsPerRow * (buttonSize + spacing) - spacing;
            int leftStart = (this.ClientSize.Width - panelWidth) / 2;

            for (int i = 0; i < size; i++)
            {
                int row = i / buttonsPerRow;
                int col = i % buttonsPerRow;

                Button btn = new Button
                {
                    Text = (i + 1).ToString(),
                    Width = buttonSize,
                    Height = buttonSize,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    Left = leftStart + col * (buttonSize + spacing),
                    Top = topStart + row * (buttonSize + spacing),
                    BackColor = Color.LightBlue
                };

                int digit = i + 1;
                btn.Click += (s, e) => { selectedDigit = digit; };

                this.Controls.Add(btn);
            }

            return topStart + numRows * (buttonSize + spacing);
        }


        private void PlaceButtons(int topPosition)
        {
            int spacing = 10;
            int btnWidth = 70;
            int btnHeight = 25;

           
            int totalWidth = btnWidth * 3 + spacing * 2;
            int startLeft = (this.ClientSize.Width - totalWidth) / 2;

            buttonNew.Left = startLeft;
            buttonReset.Left = buttonNew.Right + spacing;
            buttonShow.Left = buttonReset.Right + spacing;

            buttonNew.Top = buttonReset.Top = buttonShow.Top = topPosition;

            this.Controls.Add(buttonNew);
            this.Controls.Add(buttonReset);
            this.Controls.Add(buttonShow);
        }



        ////---------------------------------------------------------------------------
        ////functii si functionalitati
        ////----------------------------------------------------------------------------




        private void buttonNew_Click(object sender, EventArgs e)
        {
            
            var generator = new Logic.Generator(size);
            logicalGrid = generator.GeneratePuzzle(clues,size);
            countedMistakes = new bool[size, size];

            solvedGrid = logicalGrid.Clone();
            new Logic.Solver(solvedGrid,size).Solve();

            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    int val = logicalGrid.GetValue(r, c);
                    grid[r, c].Text = val == 0 ? "" : val.ToString();
                    grid[r, c].ReadOnly = val != 0; // blocheaza indiciile
                    //grid[r, c].BackColor = val != 0 ? Color.LightGray : Color.White;
                    grid[r, c].ForeColor = Color.Black;


                    // incercare de avertizare utilizator

                    if (val == 0)
                    {
                        int rowCopy = r;
                        int colCopy = c;

                        grid[r, c].TextChanged += (s, ev) =>
                        {
                            TextBox currentBox = grid[rowCopy, colCopy];
                            string text = currentBox.Text;

                            if (string.IsNullOrEmpty(text))
                            {
                                currentBox.ForeColor = Color.Black;
                                return;
                            }

                            if (!int.TryParse(text, out int userVal))
                            {

                                return;
                            }

                            int correctVal = solvedGrid.GetValue(rowCopy, colCopy);
                            //currentBox.ForeColor = userVal == correctVal ? Color.Black : Color.Red;

                            if (userVal == correctVal)
                            {
                                currentBox.ForeColor = Color.Black;
                                countedMistakes[rowCopy, colCopy] = false;
                            }
                            else
                            {
                               
                                if (!countedMistakes[rowCopy, colCopy])
                                {
                                    countedMistakes[rowCopy, colCopy] = true;
                                    mistakes++;
                                    mistakesLabel.Text = $"Greșeli: {mistakes}";
                                    currentBox.ForeColor = Color.Red;
                                }


                            }
                        };
                    }

                }

            }

            //incepem cronometrarea 
            seconds = 0;
            timerLabel.Text = "Timp: 0s";
            gameTimer.Start();

            //setam butoanele
            buttonPause.Enabled = true;
            buttonResume.Enabled = false;

            mistakesLabel.AutoSize = true;
            mistakesLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            mistakesLabel.Location = new Point(10, 50); // in dreapta sus
            mistakesLabel.Text = "Greșeli: 0";
            this.Controls.Add(mistakesLabel);



        }


        private void buttonReset_Click(object sender, EventArgs e)
        {
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    if (!grid[r, c].ReadOnly)
                        grid[r, c].Text = "";

            seconds = 0;
            timerLabel.Text = "Timp: 0s";
            gameTimer.Stop();
            gameTimer.Start();

            buttonPause.Enabled = true;
            buttonResume.Enabled = false;
            mistakes = 0;
            mistakesLabel.Text = $"Greșeli: {mistakes}";
            


        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            if (logicalGrid == null) return;

            if (solvedGrid == null) return;




            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    int val = solvedGrid.GetValue(r, c);
                    grid[r, c].Text = val.ToString();

                }
            }


            seconds = 0;
            //timerLabel.Text = "Timp: 0s";
            gameTimer.Stop();

            buttonPause.Enabled = false;
            buttonResume.Enabled = false;


        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            gameTimer.Stop();
            buttonPause.Enabled = false;
            buttonResume.Enabled = true;




        }

        private void buttonResume_Click(object sender, EventArgs e)
        {
            gameTimer.Start();
            buttonPause.Enabled = true;
            buttonResume.Enabled = false;




        }

        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clues = size == 16 ? 160 : 30;
            buttonNew_Click(sender, e);


        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clues = size == 16 ? 155: 25;
            buttonNew_Click(sender, e);
        }

        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clues = size == 16 ? 150 : 20;
            buttonNew_Click(sender, e);
        }



        private void HighlightCells(int row, int col)
        {

            int value = -1;

            if (grid[row, col].Text != "")
            {
                value = int.Parse(grid[row, col].Text);
            }

            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {

                    grid[r, c].BackColor = Color.White;
                    if (grid[r, c].Text == value.ToString())
                    {
                        grid[r, c].BackColor = Color.FromArgb(135, 206, 250);
                    }
                }

            }


            for (int i = 0; i < size; i++)
            {
                if (!grid[row, i].BackColor.Equals(Color.DeepSkyBlue))
                    grid[row, i].BackColor = Color.FromArgb(230, 230, 250);
                if (!grid[i, col].BackColor.Equals(Color.DeepSkyBlue))
                    grid[i, col].BackColor = Color.FromArgb(230, 230, 250);
            }


            int sqrt = (int)Math.Sqrt(size);
            int startRow = (row / sqrt) * sqrt;
            int startCol = (col / sqrt) * sqrt;

            for (int r = startRow; r < startRow + sqrt; r++)
            {
                for (int c = startCol; c < startCol + sqrt; c++)
                {
                    if (!grid[r, c].BackColor.Equals(Color.DeepSkyBlue))
                        grid[r, c].BackColor = Color.FromArgb(230, 230, 250);
                }
            }



            grid[row, col].BackColor = Color.FromArgb(173, 216, 230);


        }

        private void nivelToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int level = this.level;
            SudokuForm sudokuForm = new SudokuForm(level,9 );
            sudokuForm.Show();
            this.Hide();
        }

        private void x4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int level = this.level;
            SudokuForm sudokuForm = new SudokuForm(level, 16);
            sudokuForm.Show();
            this.Hide();
        }

        
    }
}
//verificare
//Console.WriteLine("Puzzle generat:");
//for (int r = 0; r < 9; r++)
//{
//    for (int c = 0; c < 9; c++)
//    {
//        Console.Write(logicalGrid.GetValue(r, c) + " ");
//    }
//    Console.WriteLine();
//}
//Console.WriteLine("Solutia generat:");
//for (int r = 0; r < 9; r++)
//{
//    for (int c = 0; c < 9; c++)
//    {
//        Console.Write(solvedGrid.GetValue(r, c) + " ");
//    }
//    Console.WriteLine();
//}


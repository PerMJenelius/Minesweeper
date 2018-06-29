﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MineSweeper
{
    public partial class MineSweeper : Form
    {
        static int nrMines = 20;
        static int[,] mines = new int[10, 10];
        static int nrColumns = 12;
        static int nrRows = 12;
        static int nrResolved = 0;
        static List<Button> buttons;
        static List<MineButton> mineButtons;
        static Button startButton;
        static TextBox timeBox;
        static bool newGame = true;
        static bool running = false;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        static double time = 0;

        public MineSweeper()
        {
            InitializeComponent();
            InitializeTimer();
            Shown += InitializeButtons;
            ResizeWindow();
        }

        private void ResizeWindow()
        {
            Width = (nrColumns * 20) + 38;
            textBoxTime.Left = Width - textBoxTime.Width - 28;
            labelTime.Left = textBoxTime.Left - 2;
            buttonReset.Left = (Width / 2) - (buttonReset.Width / 2) - 8;

            Height = (nrRows * 20) + 142;
            panelInfo.Top = Bottom - panelInfo.Height - 38;
        }

        private void InitializeButtons(object sender, EventArgs e)
        {
            mineButtons = new List<MineButton>();

            int posX = textBoxNrMines.Left;
            int posY = textBoxNrMines.Bottom + 1;

            for (int y = 1; y < (nrRows+1); y++)
            {
                for (int x = 1; x < (nrColumns+1); x++)
                {
                    MineButton button = new MineButton(x,y);
                    button.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
                    button.Location = new Point(posX, posY);
                    button.Margin = new Padding(0);
                    button.Name = $"button{y}:{x}";
                    button.Size = new Size(20, 20);
                    button.TabIndex = 5;
                    button.UseVisualStyleBackColor = true;
                    button.MouseDown += new MouseEventHandler(MineButton_Click);
                    button.MouseEnter += new EventHandler(HoverOverButton);
                    Controls.Add(button);
                    mineButtons.Add(button);
                    posX += 20;
                }
                posX -= 20 * nrColumns;
                posY += 20;
            }

            //ToDo: Flytta dessa anrop
            RandomizeMines();
            CalculateProximityValues();
        }

        private void InitializeTimer()
        {
            textBoxNrMines.Text = nrMines.ToString();

            textBoxTime.Text = "0";
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 100;
        }

        private void NewGame()
        {
            RandomizeMines();
            CalculateProximityValues();
            ResetButtons();
            running = true;
            StartTimer();
        }

        private void StartTimer()
        {
            time = 0;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            time = Math.Round((time + 0.1), 1);
            timeBox.Text = time.ToString();
        }

        private void ResetButtons()
        {
            buttons = Controls.OfType<Button>()
                .Where(b => b.Name != "buttonReset")
                .ToList();

            timeBox = Controls.OfType<TextBox>()
                .FirstOrDefault(t => t.Name == "textBoxTime");

            foreach (var button in buttons)
            {
                if (button.Name == "buttonReset")
                {
                    break;
                }

                button.Text = string.Empty;
                button.Enabled = true;
            }

            textBoxNrMines.Text = nrMines.ToString();
            nrResolved = nrMines;
            buttonReset.Text = "Reset";
            textBoxTime.Text = "0";
        }

        private void CalculateProximityValues()
        {
            IEnumerable<MineButton> mineFreeButtons = mineButtons
                .Where(b => b.CellType != CellType.Mine);

            foreach (var button in mineFreeButtons)
            {
                for (int x = button.XPosition-1; x <= button.XPosition+1; x++)
                {
                    for (int y = button.YPosition-1; y <= button.YPosition+1; y++)
                    {
                        if (x > 0 && y > 0 && x <= nrColumns && y <= nrRows)
                        {
                            MineButton tempButton = mineButtons
                                .FirstOrDefault(b => b.XPosition == x && b.YPosition == y);

                            if (tempButton != button && tempButton.CellType == CellType.Mine)
                            {
                                button.AddOneToCellType();
                            }
                        }
                    }
                }
            }
        }

        private void RandomizeMines()
        {
            Random rng = new Random();
            int xPos;
            int yPos;

            for (int i = 0; i < nrMines; i++)
            {
                bool minefreePos = false;

                while (!minefreePos)
                {
                    xPos = rng.Next(1, (nrColumns+1));
                    yPos = rng.Next(1, (nrRows+1));

                    var button = mineButtons
                        .FirstOrDefault(b => b.XPosition == xPos && b.YPosition == yPos);

                    if (button.CellType != CellType.Mine)
                    {
                        button.CellType = CellType.Mine;
                        minefreePos = true;
                    }
                }
            }
        }

        private void CheckForWin()
        {
            int nrCleared = 0;

            if (running)
            {
                foreach (var button in buttons)
                {
                    if (button.Text != string.Empty)
                    {
                        try
                        {
                            int mineNr = Convert.ToInt32(button.Text);

                            if (mineNr > 0 && mineNr < 9)
                            {
                                ++nrCleared;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }

                    if (button.Enabled == false)
                    {
                        ++nrCleared;
                    }
                }

                if (nrCleared + nrMines >= nrColumns)
                {
                    Win();
                }
            }
        }

        private void Win()
        {
            buttonReset.Text = "You win!";
            running = false;
            timer.Stop();
        }

        private void Lose(Button btn)
        {
            Explode(btn);

            foreach (var button in buttons)
            {
                string btnId = button.Name.Split('n')[1];
                int posX = Convert.ToInt32(btnId.Split('y')[0].Remove(0, 1));
                int posY = Convert.ToInt32(btnId.Split('y')[1]);

                if (mines[posX, posY] == 9)
                {
                    if (button.Text == string.Empty)
                    {
                        button.ForeColor = Color.Black;
                        button.Text = "*";
                    }
                }
                else if (mines[posX, posY] != 9 && button.Text == "ì")
                {
                    button.Text = "X";
                }
            }

            running = false;
            timer.Stop();
        }

        private void Explode(Button btn)
        {
            //btn.BackColor = Color.Yellow;
            //Thread.Sleep(1000);
            //btn.BackColor = Color.Red;
            //Thread.Sleep(1000);
            //btn.BackColor = Color.Black;
        }

        private void DisplayNumber(Button button)
        {
            string btnId = button.Name.Split('n')[1];
            int posX = Convert.ToInt32(btnId.Split('y')[0].Remove(0, 1));
            int posY = Convert.ToInt32(btnId.Split('y')[1]);

            switch (mines[posX, posY])
            {
                case 0:
                    ClearEmptyCells(button);
                    break;
                case 1:
                    button.ForeColor = Color.Green;
                    button.Text = "1";
                    ++nrResolved;
                    break;
                case 2:
                    button.ForeColor = Color.Blue;
                    button.Text = "2";
                    ++nrResolved;
                    break;
                case 3:
                    button.ForeColor = Color.DarkOrange;
                    button.Text = "3";
                    ++nrResolved;
                    break;
                case 4:
                    button.ForeColor = Color.DarkRed;
                    button.Text = "4";
                    ++nrResolved;
                    break;
                case 5:
                    button.ForeColor = Color.Purple;
                    button.Text = "5";
                    ++nrResolved;
                    break;
                case 6:
                    button.ForeColor = Color.Brown;
                    button.Text = "6";
                    ++nrResolved;
                    break;
                case 9:
                    Lose(button);
                    break;
                default:
                    button.ForeColor = Color.Black;
                    button.Text = mines[posX, posY].ToString();
                    ++nrResolved;
                    break;
            }
        }

        private void ClearEmptyCells(Button button)
        {
            string btnId = button.Name.Split('n')[1];
            int posX = Convert.ToInt32(btnId.Split('y')[0].Remove(0, 1));
            int posY = Convert.ToInt32(btnId.Split('y')[1]);

            button.Text = string.Empty;
            button.Enabled = false;

            //top left
            if (posX > 0 && posY > 0)
            {
                string buttonName = $"buttonx{posX - 1}y{posY - 1}";

                Button b1 = Controls.OfType<Button>()
                .FirstOrDefault(b => b.Name == buttonName);

                UpdateCell(b1);
            }

            //top center
            if (posY > 0)
            {
                string buttonName = $"buttonx{posX}y{posY - 1}";

                Button b1 = Controls.OfType<Button>()
                .FirstOrDefault(b => b.Name == buttonName);

                UpdateCell(b1);
            }

            //top right
            if (posX < 9 && posY > 0)
            {
                string buttonName = $"buttonx{posX + 1}y{posY - 1}";

                Button b1 = Controls.OfType<Button>()
                .FirstOrDefault(b => b.Name == buttonName);

                UpdateCell(b1);
            }

            //left
            if (posX > 0)
            {
                string buttonName = $"buttonx{posX - 1}y{posY}";

                Button b1 = Controls.OfType<Button>()
                .FirstOrDefault(b => b.Name == buttonName);

                UpdateCell(b1);
            }

            //right
            if (posX < 9)
            {
                string buttonName = $"buttonx{posX + 1}y{posY}";

                Button b1 = Controls.OfType<Button>()
                .FirstOrDefault(b => b.Name == buttonName);

                UpdateCell(b1);
            }

            //bottom left
            if (posX > 0 && posY < 9)
            {
                string buttonName = $"buttonx{posX - 1}y{posY + 1}";

                Button b1 = Controls.OfType<Button>()
                .FirstOrDefault(b => b.Name == buttonName);

                UpdateCell(b1);
            }

            //bottom center
            if (posY < 9)
            {
                string buttonName = $"buttonx{posX}y{posY + 1}";

                Button b1 = Controls.OfType<Button>()
                .FirstOrDefault(b => b.Name == buttonName);

                UpdateCell(b1);
            }

            //bottom right
            if (posX < 9 && posY < 9)
            {
                string buttonName = $"buttonx{posX + 1}y{posY + 1}";

                Button b1 = Controls.OfType<Button>()
                .FirstOrDefault(b => b.Name == buttonName);

                UpdateCell(b1);
            }
        }

        private void UpdateCell(Button button)
        {
            if (button.Enabled == true)
            {
                string btnId = button.Name.Split('n')[1];
                int posX = Convert.ToInt32(btnId.Split('y')[0].Remove(0, 1));
                int posY = Convert.ToInt32(btnId.Split('y')[1]);

                if (mines[posX, posY] == 0)
                {
                    ++nrResolved;
                    ClearEmptyCells(button);
                }
                else
                {
                    if (button.Text == string.Empty)
                    {
                        DisplayNumber(button);
                    }
                }
            }
        }

        private void MineButton_Click(object sender, MouseEventArgs e)
        {
            if (newGame)
            {
                newGame = false;
                startButton = sender as Button;
                NewGame();
            }

            if (running)
            {
                var button = sender as Button;

                if (e.Button == MouseButtons.Left)
                {
                    if (button.Text == string.Empty)
                    {
                        DisplayNumber(button);
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (button.Text == string.Empty)
                    {
                        button.ForeColor = Color.Red;
                        button.Text = "ì";
                        textBoxNrMines.Text = (Convert.ToInt16(textBoxNrMines.Text) - 1).ToString();
                        //button.Image = Image.FromFile(@"C://images/flag.png");
                    }
                    else if (button.Text == "ì")
                    {
                        button.ForeColor = Color.Red;
                        button.Text = "?";
                        textBoxNrMines.Text = (Convert.ToInt16(textBoxNrMines.Text) + 1).ToString();
                    }
                    else if (button.Text == "?")
                    {
                        button.Text = string.Empty;
                    }
                }

                CheckForWin();
            }

            buttonReset.Focus();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            timer.Stop();
            textBoxTime.Text = "0";
            ResetButtons();
            newGame = true;
        }

        private void HoverOverButton(object sender, EventArgs e)
        {
            MineButton button = sender as MineButton;
            //labelInfo.Text = $"Thanks for stopping by {button.Name}.";
            labelInfo.Text = button.CellType == CellType.Mine ? "MINES!" : "You're OK";
        }
    }
}

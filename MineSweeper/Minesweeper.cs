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
        static int nrMines = 15;
        static int[,] mines = new int[10, 10];
        static int nrCells = 100;
        static int nrResolved = 0;
        static List<Button> buttons;
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
            for (int posX = 0; posX < 10; posX++)
            {
                for (int posY = 0; posY < 10; posY++)
                {
                    if (mines[posX, posY] != 9)
                    {
                        if (posX > 0 && posY > 0 && mines[posX - 1, posY - 1] == 9)
                        {
                            ++mines[posX, posY];
                        }
                        if (posY > 0 && mines[posX, posY - 1] == 9)
                        {
                            ++mines[posX, posY];
                        }
                        if (posX < 9 && posY > 0 && mines[posX + 1, posY - 1] == 9)
                        {
                            ++mines[posX, posY];
                        }
                        if (posX > 0 && mines[posX - 1, posY] == 9)
                        {
                            ++mines[posX, posY];
                        }
                        if (posX < 9 && mines[posX + 1, posY] == 9)
                        {
                            ++mines[posX, posY];
                        }
                        if (posX > 0 && posY < 9 && mines[posX - 1, posY + 1] == 9)
                        {
                            ++mines[posX, posY];
                        }
                        if (posY < 9 && mines[posX, posY + 1] == 9)
                        {
                            ++mines[posX, posY];
                        }
                        if (posX < 9 && posY < 9 && mines[posX + 1, posY + 1] == 9)
                        {
                            ++mines[posX, posY];
                        }
                    }
                }
            }
        }

        private void RandomizeMines()
        {
            mines = new int[10, 10];
            Random rng = new Random();
            int xPos;
            int yPos;

            //string btnId = startButton.Name.Split('n')[1];
            //int strX = Convert.ToInt32(btnId.Split('y')[0].Remove(0, 1));
            //int strY = Convert.ToInt32(btnId.Split('y')[1]);

            for (int i = 0; i < nrMines; i++)
            {
                bool minefreePos = false;

                while (!minefreePos)
                {
                    xPos = rng.Next(0, 10);
                    yPos = rng.Next(0, 10);

                    if (/*xPos != strX && yPos != strY &&*/ mines[xPos, yPos] != 9)
                    {
                        mines[xPos, yPos] = 9;
                        minefreePos = true;
                    }
                }
            }
        }

        private void HandleMouseClick(object sender, MouseEventArgs e)
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

                if (nrCleared + nrMines >= nrCells)
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

        private void buttonReset_Click(object sender, EventArgs e)
        {
            NewGame();
        }
    }
}
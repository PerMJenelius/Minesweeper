using System;
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
        static int nrMines = 500;
        static int nrColumns = 90;
        static int nrRows = 50;
        static List<MineButton> mineButtons;
        static Button startButton;
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

            for (int y = 1; y < (nrRows + 1); y++)
            {
                for (int x = 1; x < (nrColumns + 1); x++)
                {
                    MineButton button = new MineButton(x, y);
                    InitializeMineButton(button, posX, posY);
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
            timer.Interval = 1000;
        }

        private void NewGame()
        {
            ResetButtons();
            RandomizeMines();
            CalculateProximityValues();
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
            ++time;
            textBoxTime.Text = time.ToString();
        }

        private void ResetButtons()
        {
            foreach (var button in mineButtons)
            {
                button.Text = string.Empty;
                button.Enabled = true;
                button.CellType = CellType.Empty;
                button.FlagType = FlagType.None;
            }

            textBoxNrMines.Text = nrMines.ToString();
            buttonReset.Text = "Reset";
            textBoxTime.Text = "0";
        }

        private void CalculateProximityValues()
        {
            IEnumerable<MineButton> mineFreeButtons = mineButtons
                .Where(b => b.CellType != CellType.Mine);

            foreach (var button in mineFreeButtons)
            {
                for (int x = button.XPosition - 1; x <= button.XPosition + 1; x++)
                {
                    for (int y = button.YPosition - 1; y <= button.YPosition + 1; y++)
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
            int random;
            List<MineButton> myList = new List<MineButton>();

            foreach (var button in mineButtons)
            {
                myList.Add(button);
            }

            for (int i = 0; i < nrMines; i++)
            {
                random = rng.Next(0, myList.Count);
                mineButtons[random].CellType = CellType.Mine;
                myList.RemoveAt(random);
            }
        }

        private void CheckForWin()
        {
            int nrRevealedCells = 0;

            foreach (var button in mineButtons)
            {
                if (button.CellType != CellType.Mine && button.FlagType == FlagType.Number)
                {
                    ++nrRevealedCells;
                }
            }

            if ((nrMines + nrRevealedCells) >= mineButtons.Count)
            {
                Win();
            }
        }

        private void Win()
        {
            var mineCells = mineButtons
                .Where(b => b.CellType == CellType.Mine);

            foreach (var cell in mineCells)
            {
                cell.ForeColor = Color.Red;
                cell.Text = "í";
            }

            buttonReset.Text = "You win!";
            running = false;
            timer.Stop();
        }

        private void Lose(MineButton btn)
        {
            Explode(btn);

            foreach (var button in mineButtons)
            {
                if (button.CellType == CellType.Mine)
                {
                    if (button.Text == string.Empty)
                    {
                        button.ForeColor = Color.Black;
                        button.Text = "*";
                    }
                }
                //else if (mines[posX, posY] != 9 && button.Text == "ì")
                //{
                //    button.Text = "X";
                //}
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

        private void DisplayNumber(MineButton button)
        {
            switch (button.CellType)
            {
                case CellType.Empty:
                    ClearEmptyCells(button);
                    break;
                case CellType.One:
                    button.ForeColor = Color.Green;
                    button.Text = "1";
                    break;
                case CellType.Two:
                    button.ForeColor = Color.Blue;
                    button.Text = "2";
                    break;
                case CellType.Three:
                    button.ForeColor = Color.DarkOrange;
                    button.Text = "3";
                    break;
                case CellType.Four:
                    button.ForeColor = Color.DarkRed;
                    button.Text = "4";
                    break;
                case CellType.Five:
                    button.ForeColor = Color.Purple;
                    button.Text = "5";
                    break;
                case CellType.Six:
                    button.ForeColor = Color.Brown;
                    button.Text = "6";
                    break;
                case CellType.Seven:
                    button.ForeColor = Color.Brown;
                    button.Text = "7";
                    break;
                case CellType.Eight:
                    button.ForeColor = Color.Brown;
                    button.Text = "8";
                    break;
                case CellType.Mine:
                    Lose(button);
                    break;
                default:
                    break;
            }

            if (button.CellType != CellType.Mine)
            {
                button.FlagType = FlagType.Number;
            }
        }

        private void ClearEmptyCells(MineButton button)
        {
            button.Enabled = false;

            for (int x = button.XPosition - 1; x <= button.XPosition + 1; x++)
            {
                for (int y = button.YPosition - 1; y <= button.YPosition + 1; y++)
                {
                    if (x > 0 && y > 0 && x <= nrColumns && y <= nrRows)
                    {
                        MineButton tempButton = mineButtons
                            .FirstOrDefault(b => b.XPosition == x && b.YPosition == y);

                        if (tempButton != button && tempButton.Enabled && tempButton.CellType != CellType.Mine)
                        {
                            DisplayNumber(tempButton);
                        }
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
                var button = sender as MineButton;

                if (e.Button == MouseButtons.Left)
                {
                    if (button.FlagType == FlagType.None)
                    {
                        DisplayNumber(button);
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (button.FlagType == FlagType.None)
                    {
                        //button.Image = Image.FromFile(@"/Images/flag.png");
                        button.ForeColor = Color.Red;
                        button.Text = "í";
                        button.FlagType = FlagType.Flag;
                        textBoxNrMines.Text = (Convert.ToInt16(textBoxNrMines.Text) - 1).ToString();
                    }
                    else if (button.FlagType == FlagType.Flag)
                    {
                        button.ForeColor = Color.Red;
                        button.Text = "?";
                        button.FlagType = FlagType.Question;
                        textBoxNrMines.Text = (Convert.ToInt16(textBoxNrMines.Text) + 1).ToString();
                    }
                    else if (button.FlagType == FlagType.Question)
                    {
                        button.Text = string.Empty;
                        button.FlagType = FlagType.None;
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
            //labelInfo.Text = button.CellType == CellType.Mine ? "MINES!" : "You're OK";
            //labelInfo.Text = $"{button.XPosition} : {button.YPosition}";
            labelInfo.Text = $"{mineButtons.Where(b => b.FlagType == FlagType.Number).Count().ToString()} + {nrMines} / {mineButtons.Count}";
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonReset_Click(sender, e);
        }
    }
}

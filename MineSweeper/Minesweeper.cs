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
        static int nrMines = 50;
        static int nrColumns = 20; //max 95
        static int nrRows = 20; //max 52
        static List<MineButton> mineButtons;
        static List<MineButton> emptyButtons = new List<MineButton>();
        static List<MineButton> numberButtons = new List<MineButton>();
        static List<List<MineButton>> emptyClusters = new List<List<MineButton>>();
        static bool newGame = true;
        static bool running = false;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        static int time = 0;

        public MineSweeper()
        {
            InitializeComponent();
            InitializeTimer();
            ResetTextBoxes();
            Shown += InitializeButtons;
            ResizeWindow();
        }

        private void InitializeTimer()
        {
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1000;
        }

        private void ResetTextBoxes()
        {
            textBoxNrMines.Text = nrMines.ToString();
            textBoxTime.Text = "0";
        }

        private void InitializeButtons(object sender, EventArgs e)
        {
            mineButtons = new List<MineButton>();

            int posX = textBoxNrMines.Left;
            int posY = textBoxNrMines.Bottom + 5;

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
        }

        private void ResizeWindow()
        {
            Width = (nrColumns * 20) + 38;
            Height = (nrRows * 20) + 142;
            textBoxTime.Left = Width - textBoxTime.Width - 28;
            labelTime.Left = textBoxTime.Left - 2;
            buttonReset.Left = (Width / 2) - (buttonReset.Width / 2) - 8;
        }

        private void NewGame(MineButton button)
        {
            ResetTextBoxes();
            ResetButtons();
            RandomizeMines(button);
            CalculateProximityValues();
            running = true;
            StartTimer();
        }

        private void StartTimer()
        {
            time = 0;
            timer.Start();
        }

        private void ResetButtons()
        {
            foreach (var button in mineButtons)
            {
                button.Text = string.Empty;
                button.BackColor = SystemColors.ButtonFace;
                button.Enabled = true;
                button.CellType = CellType.Empty;
                button.FlagType = FlagType.None;
                button.Number = 0;
            }

            buttonReset.Text = "Reset";
        }

        private void CalculateProximityValues()
        {
            var mineFreeButtons = mineButtons
                .Where(b => b.CellType != CellType.Mine);

            foreach (var button in mineFreeButtons)
            {
                for (int x = button.XPosition - 1; x <= button.XPosition + 1; x++)
                {
                    for (int y = button.YPosition - 1; y <= button.YPosition + 1; y++)
                    {
                        if (x >= 1 && x <= nrColumns && y >= 1 && y <= nrRows)
                        {
                            MineButton tempButton = mineButtons
                                .FirstOrDefault(b => b.XPosition == x && b.YPosition == y);

                            if (tempButton != button && tempButton.CellType == CellType.Mine)
                            {
                                if (button.CellType != CellType.Number)
                                {
                                    button.CellType = CellType.Number;
                                }
                                ++button.Number;
                            }
                        }
                    }
                }
            }
        }

        private void RandomizeMines(MineButton button)
        {
            Random random = new Random();
            List<MineButton> rngList = new List<MineButton>();

            rngList.AddRange(mineButtons);

            var btnIndex = mineButtons.IndexOf(button);
            rngList.RemoveAt(btnIndex);

            for (int i = 0; i < nrMines; i++)
            {
                int randomIndex = random.Next(0, rngList.Count);
                var randomMine = rngList[randomIndex];
                var newButton = mineButtons.FirstOrDefault(b => b.XPosition == randomMine.XPosition && b.YPosition == randomMine.YPosition);
                newButton.CellType = CellType.Mine;
                rngList.Remove(randomMine);
            }
        }

        private void CheckForWin()
        {
            if (mineButtons.Count(b => b.FlagType == FlagType.Number) == mineButtons.Count(b => b.CellType != CellType.Mine))
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
                cell.ForeColor = Color.Black;
                cell.Text = "*";
            }

            buttonReset.Text = "You win!";
            running = false;
            timer.Stop();
        }

        private void Lose(MineButton btn)
        {
            foreach (var button in mineButtons)
            {
                if (button.CellType == CellType.Mine)
                {
                    button.BackColor = Color.Black;
                }
            }

            running = false;
            timer.Stop();
        }

        private void CheckCell(MineButton button)
        {
            switch (button.CellType)
            {
                case CellType.Mine:
                    Lose(button);
                    break;
                case CellType.Empty:
                    ClearEmptyCells(button);
                    break;
                case CellType.Number:
                    DisplayNumber(button);
                    break;
            }
        }

        private void DisplayNumber(MineButton button)
        {
            switch (button.Number)
            {
                case 1:
                    button.ForeColor = Color.Green;
                    break;
                case 2:
                    button.ForeColor = Color.Blue;
                    break;
                case 3:
                    button.ForeColor = Color.DarkOrange;
                    break;
                case 4:
                    button.ForeColor = Color.DarkRed;
                    break;
                case 5:
                    button.ForeColor = Color.Purple;
                    break;
                case 6:
                    button.ForeColor = Color.Brown;
                    break;
                case 7:
                case 8:
                    button.ForeColor = Color.Black;
                    break;
            }
            button.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button.Text = button.Number.ToString();
            button.FlagType = FlagType.Number;
        }

        private void ClearEmptyCells(MineButton button)
        {
            numberButtons.Clear();
            emptyButtons.Clear();

            List<MineButton> tempButtons = new List<MineButton>();
            List<MineButton> foundButtons = new List<MineButton>();

            emptyButtons.Add(button);
            tempButtons.Add(button);

            do
            {
                foundButtons.Clear();

                foreach (var btn in tempButtons)
                {
                    foundButtons.AddRange(SearchAroundCell(btn));
                }

                tempButtons.Clear();
                tempButtons.AddRange(foundButtons);
                emptyButtons.AddRange(foundButtons);

            } while (tempButtons.Count > 0);

            foreach (var btn in emptyButtons)
            {
                btn.Enabled = false;
                btn.FlagType = FlagType.Number;
            }

            foreach (var btn in numberButtons)
            {
                DisplayNumber(btn);
            }
        }

        private List<MineButton> SearchAroundCell(MineButton button)
        {
            List<MineButton> tempButtons = new List<MineButton>();
            List<MineButton> output = new List<MineButton>();

            //for (int y = (button.YPosition - 1); y <= (button.YPosition + 1); y++)
            //{
            //    for (int x = (button.XPosition - 1); x <= (button.XPosition + 1); x++)
            //    {
            //        if (x >= 1 && x <= nrColumns && y >= 1 && y <= nrRows)
            //        {
            //            tempButtons.Add(mineButtons.FirstOrDefault(b => b.XPosition == x && b.YPosition == y));
            //        }
            //    }
            //}

            tempButtons.AddRange(mineButtons
                .Where(b => b.XPosition >= (button.XPosition - 1) 
                && b.XPosition <= (button.XPosition + 1) 
                && b.YPosition >= (button.YPosition - 1)
                && b.YPosition <= (button.YPosition + 1)));

            foreach (var tempButton in tempButtons)
            {
                if (tempButton != button && tempButton.CellType == CellType.Empty && !emptyButtons.Contains(tempButton))
                {
                    output.Add(tempButton);
                }
                else if (tempButton.CellType == CellType.Number && !numberButtons.Contains(tempButton))
                {
                    numberButtons.Add(tempButton);
                }
            }

            return output;
        }

        private List<MineButton> SearchAroundCellNew(MineButton button)
        {
            List<MineButton> tempButtons = new List<MineButton>();
            List<MineButton> output = new List<MineButton>();
            emptyButtons.Add(button);

            int x = button.XPosition;
            int y = button.YPosition;
            int z = 0;
            bool top = true, right = true, bottom = true, left = true;
            int nrEmpty = 0;
            bool searching = true;

            do
            {
                ++z;

                tempButtons.Clear();

                if (top)
                {
                    var newButtons = mineButtons.Where(b => b.XPosition >= (x - z) && b.XPosition <= (x + z) && b.YPosition == (y - z));

                    if (newButtons.Count(b => b.CellType == CellType.Mine) == 0)
                    {
                        tempButtons.AddRange(newButtons);
                    }
                    else
                    {
                        top = false;
                    }
                }
                if (right)
                {
                    var newButtons = mineButtons.Where(b => b.XPosition == (x + z) && b.YPosition > (y - z) && b.YPosition < (y + z));

                    if (newButtons.Count(b => b.CellType == CellType.Mine) == 0)
                    {
                        tempButtons.AddRange(newButtons);
                    }
                    else
                    {
                        right = false;
                    }
                }
                if (bottom)
                {
                    var newButtons = mineButtons.Where(b => b.XPosition >= (x - z) && b.XPosition <= (x + z) && b.YPosition == (y + z));

                    if (newButtons.Count(b => b.CellType == CellType.Mine) == 0)
                    {
                        tempButtons.AddRange(newButtons);
                    }
                    else
                    {
                        bottom = false;
                    }
                }
                if (left)
                {
                    var newButtons = mineButtons.Where(b => b.XPosition == (x - z) && b.YPosition > (y - z) && b.YPosition < (y + z));

                    if (newButtons.Count(b => b.CellType == CellType.Mine) == 0)
                    {
                        tempButtons.AddRange(newButtons);
                    }
                    else
                    {
                        left = false;
                    }
                }

                nrEmpty = 0;

                foreach (var tempButton in tempButtons)
                {
                    if (tempButton.CellType == CellType.Empty && !emptyButtons.Contains(tempButton))
                    {
                        output.Add(tempButton);
                        ++nrEmpty;
                    }
                    else if (tempButton.CellType == CellType.Number && !numberButtons.Contains(tempButton))
                    {
                        numberButtons.Add(tempButton);
                    }
                }
                searching = nrEmpty > 0;

            } while (searching);

            return output;
        }

        private void MineButton_Click(object sender, MouseEventArgs e)
        {
            if (newGame)
            {
                newGame = false;
                NewGame(sender as MineButton);
            }

            if (running)
            {
                var button = sender as MineButton;

                if (e.Button == MouseButtons.Left)
                {
                    if (button.FlagType == FlagType.None)
                    {
                        CheckCell(button);
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (button.FlagType == FlagType.None)
                    {
                        //button.Image = Image.FromFile(@"/Images/flag.png");
                        button.ForeColor = Color.Black;
                        button.Text = "*";
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

        private void MineButton_Hover(object sender, EventArgs e)
        {
            labelInfo.Text = emptyClusters.Count.ToString();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            timer.Stop();
            textBoxTime.Text = "0";
            ResetButtons();
            ResetTextBoxes();
            newGame = true;
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonReset_Click(sender, e);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            ++time;
            textBoxTime.Text = time.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSweeper
{
    public partial class MineSweeper : Form
    {
        static int nrMines = 5;
        static int[,] mines = new int[10, 10];
        static int nrCells = 100;
        static int nrResolved = 0;

        public MineSweeper()
        {
            InitializeComponent();
            NewGame();
        }

        private void NewGame()
        {
            RandomizeMines();
            CalculateProximityValues();
            ResetButtons();
        }

        private void ResetButtons()
        {
            foreach (var button in Controls.OfType<Button>())
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

            for (int i = 0; i < nrMines; i++)
            {
                bool minefreePos = false;

                while (!minefreePos)
                {
                    xPos = rng.Next(0, 10);
                    yPos = rng.Next(0, 10);

                    if (mines[xPos, yPos] != 9)
                    {
                        mines[xPos, yPos] = 9;
                        minefreePos = true;
                    }
                }
            }
        }

        private void HandleMouseClick(object sender, MouseEventArgs e)
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
                    button.Text = "Г";
                    textBoxNrMines.Text = (Convert.ToInt16(textBoxNrMines.Text) - 1).ToString();
                }
                else if (button.Text == "Г")
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

            buttonReset.Focus();
        }

        private void CheckForWin()
        {
            int nrCleared = 0;

            foreach (var button in Controls.OfType<Button>())
            {
                if (button.Name == "buttonReset")
                {
                    break;
                }

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

        private void Win()
        {
            MessageBox.Show("You win!");
        }

        private void DisplayNumber(Button button)
        {
            string btnId = button.Name.Split('n')[1];
            int posX = Convert.ToInt32(btnId.Split('y')[0].Remove(0,1));
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
                    Death();
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

        private void Death()
        {
            foreach (var button in Controls.OfType<Button>())
            {
                if (button.Name == "buttonReset")
                {
                    break;
                }

                string btnId = button.Name.Split('n')[1];
                int posX = Convert.ToInt32(btnId.Split('y')[0].Remove(0, 1));
                int posY = Convert.ToInt32(btnId.Split('y')[1]);

                if (mines[posX, posY] == 9)
                {
                    if (button.Text == string.Empty)
                    {
                        button.Text = "*";
                    }
                }
                else if (mines[posX, posY] != 9 && button.Text == "Г")
                {
                    button.Text = "X";
                }

                button.Enabled = false;
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            NewGame();
        }
    }
}

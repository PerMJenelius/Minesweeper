using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSweeper
{
    public class MineButton : Button
    {
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public CellType CellType { get; set; }
        public FlagType FlagType { get; set; }

        public MineButton(int xPosition, int yPosition)
        {
            XPosition = xPosition;
            YPosition = yPosition;
            CellType = CellType.Empty;
            FlagType = FlagType.None;
        }

        public void AddOneToCellType()
        {
            switch (CellType)
            {
                case CellType.Empty:
                    CellType = CellType.One;
                    break;
                case CellType.One:
                    CellType = CellType.Two;
                    break;
                case CellType.Two:
                    CellType = CellType.Three;
                    break;
                case CellType.Three:
                    CellType = CellType.Four;
                    break;
                case CellType.Four:
                    CellType = CellType.Five;
                    break;
                case CellType.Five:
                    CellType = CellType.Six;
                    break;
                case CellType.Six:
                    CellType = CellType.Seven;
                    break;
                case CellType.Seven:
                case CellType.Eight:
                    CellType = CellType.Eight;
                    break;
                default:
                    CellType = CellType.Empty;
                    break;
            }
        }
    }

    public enum CellType
    {
        Empty,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Mine
    }

    public enum FlagType
    {
        None,
        Flag,
        Number,
        Question
    }
}

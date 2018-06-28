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

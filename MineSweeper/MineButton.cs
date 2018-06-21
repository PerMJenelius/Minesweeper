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
        public CellType Value { get; set; }
        public FlagType FlagType { get; set; }
    }

    public enum CellType
    {
        Empty = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Mine = 9
    }

    public enum FlagType
    {
        None = 0,
        Flag = 1,
        Question = 2
    }
}

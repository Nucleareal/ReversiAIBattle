using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.ReversiSystem.Logic
{
    /// <summary>
    /// 方向を表す列挙定数。
    /// </summary>
    enum Direction
    {
        Left  = 0x001,
        Up    = 0x002,
        Right = 0x004,
        Down  = 0x008,

        LeftUp    = Left  | Up  ,
        LeftDown  = Left  | Down,
        RightUp   = Right | Up  ,
        RightDown = Right | Down,
    }
}

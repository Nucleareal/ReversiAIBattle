using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.ReversiSystem.Logic
{
    /// <summary>
    /// 環境変数。
    /// </summary>
    class REnvironment
    {
        /// <summary>
        /// 水平方向の盤面サイズ。
        /// </summary>
        public static int BoardX
        {
            get { return 8; }
        }

        /// <summary>
        /// 垂直方向の盤面サイズ。
        /// </summary>
        public static int BoardY
        {
            get { return 8; }
        }
    }
}

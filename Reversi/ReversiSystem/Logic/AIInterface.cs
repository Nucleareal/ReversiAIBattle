using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.ReversiSystem.Logic
{
    interface AIInterface
    {
        /// <summary>
        /// AIが実装すべきメソッドです。
        /// </summary>
        /// <param name="board">現在の盤面のコピー</param>
        /// <returns>指す座標の{X, Y}</returns>
        int[] Solve(Board board);

        /// <summary>
        /// AIの名前。表示に使用されます。
        /// </summary>
        string Name
        {
            get;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.ReversiSystem.Logic;

namespace Reversi.AI1
{
    class ReversiAI1 : AIInterface
    {
        private static Random rand = new Random();

        public int[] Solve(Board board)
        {
            var placeable = new List<int[]>();

            for (int i = 0; i < REnvironment.BoardX; i++)
            {
                for (int j = 0; j < REnvironment.BoardY; j++)
                {
                    var res = board.GetPutParam(i, j, board.HandStone);
                    if (res.CanPlace)
                    {
                        placeable.Add(new int[]{ i, j });
                    }
                }
            }

            if(placeable.Count == 0) return new int[] { -1, -1 };

            return placeable[rand.Next(placeable.Count)];
        }

        public string Name
        {
            get { return "AI1"; }
        }
    }
}

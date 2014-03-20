using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reversi.ReversiSystem.Logic;
using Reversi.AI1;
using Reversi.AI2;
using System.Threading;

namespace Reversi.ReversiSystem.Launcher
{
    class Launcher
    {
        /// <summary>
        /// プログラムのエントリーポイント。
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var launcher = new Launcher();
            launcher.Run();
        }

        /// <summary>
        /// プログラムのメインスレッド。
        /// </summary>
        public void Run()
        {
            var rand = new Random();

            var board = new Board();
            board.Init();

            var cc = new ChessClock();
            cc.Init();

            var ai1 = new ReversiAI1();
            var ai2 = new ReversiAI2();
            int forestalling = rand.Next(2);

            var ai = forestalling == 0 ? new AIInterface[] { ai1, ai2 } : new AIInterface[] { ai2, ai1 };

            Console.WriteLine("Started battle: {0} VS {1}", ai[0].Name, ai[1].Name);

            Console.WriteLine("Forestalling is {0}", ai[0].Name);

            foreach (var a in ai) cc.Register(a);


            var i = 0;

            board.PutBoard();

            int passcount = 0;

            while (board.Living)
            {
                var point = cc.RunClock(ai[i%2], board);

                if (!board.Living) break;

                if (!board.IsNormalPosition(point[0], point[1]))
                {
                    Console.WriteLine(" {0} Passed", ai[i % 2].Name);

                    board.Pass();
                    passcount++;
                    if (passcount == 2) break;
                }
                else
                {
                    Console.WriteLine(" {0} Placed ({1}, {2}) ", ai[i % 2].Name, point[0], point[1]);

                    passcount = 0;
                    board.PutStoneWithReverse(point[0], point[1], board.HandStone);
                }

                board.PutBoard();
                i++;
            }

            var arr = board.CountStones();

            Console.WriteLine("Black:{0} White:{1}", arr[0], arr[1]);

            Console.WriteLine(string.Format("{0} ({1})", board.Winner, arr[0] == arr[1] ? "" : arr[0] > arr[1] ? ai[0].Name : ai[1].Name));
        }
    }
}

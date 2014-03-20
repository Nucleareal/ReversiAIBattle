using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Reversi.ReversiSystem.Debug;

namespace Reversi.ReversiSystem.Logic
{
    /// <summary>
    /// タイムの計測を行います。
    /// </summary>
    class ChessClock
    {
        private Dictionary<AIInterface, long> timers;

        /// <summary>
        /// 1手の持ち時間。デフォルトでは30秒。
        /// </summary>
        private static readonly long handSec = 30L;

        /// <summary>
        /// AIの持ち時間。
        /// </summary>
        private static readonly long playerSec = 300L;

        /// <summary>
        /// タイマーの初期化。
        /// </summary>
        public void Init()
        {
            timers = new Dictionary<AIInterface, long>();

            Logger.String(string.Format("Stopwatch.IsHighResolution: {0}", Stopwatch.IsHighResolution));
        }

        public void Register(AIInterface ai)
        {
            timers[ai] = playerSec;
        }

        /// <summary>
        /// 実際にAIを走らせてタイムを計測します。
        /// </summary>
        /// <param name="runner">AI</param>
        /// <param name="orig">盤面</param>
        /// <returns>AIの着手</returns>
        public int[] RunClock(AIInterface runner, Board orig)
        {
            //AI始動
            var res = RunProgram(runner, new Board(orig));

            if (timers[runner] < 0)
            {
                Logger.String(string.Format(" {0} Timed out.", runner.Name));
                orig.Shutdown();
            }

            return res.Result;
        }

        private async Task<int[]> RunProgram(AIInterface runner, Board orig)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var task = Task.FromResult<int[]>(runner.Solve(orig));

            var res = await task;

            sw.Stop();

            long time = sw.ElapsedMilliseconds / 1000;

            if (time > handSec)
            {
                timers[runner] -= (time - handSec);
            }

            Logger.String(string.Format(" {0} used {1} seconds({3} ms), Left {2} seconds", runner.Name, time, timers[runner], sw.ElapsedMilliseconds));

            return res;
        }
    }
}

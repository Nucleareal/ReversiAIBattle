using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.ReversiSystem.Logic
{
    class Board
    {
        /// <summary>
        /// 盤面を表す配列。Ref()にて参照可能。
        /// </summary>
        private Stone[,] _board;

        /// <summary>
        /// 方向の配列。
        /// </summary>
        private static readonly Direction[] Around = {  Direction.Left , Direction.LeftUp   , Direction.Up  , Direction.RightUp ,
                                                        Direction.Right, Direction.RightDown, Direction.Down, Direction.LeftDown, };

        /// <summary>
        /// デフォルトコンストラクタ。
        /// </summary>
        public Board()
        {
        }

        /// <summary>
        /// コピーコンストラクタ。
        /// </summary>
        /// <param name="original">元となる盤面</param>
        public Board(Board original)
        {
            this._board = original._board;
            this.Hand = original.Hand;
            this.Living = original.Living;
            this.ForceLoser = original.ForceLoser;
        }

        /// <summary>
        /// 初期化。
        /// </summary>
        public void Init()
        {
            _board = new Stone[REnvironment.BoardY, REnvironment.BoardX];
            for (int i = 0; i < REnvironment.BoardY; i++)
                for (int j = 0; j < REnvironment.BoardX; j++)
                    _board[i, j] = Stone.None;

            Hand = 1;

            _board[3, 3] = Stone.Black;
            _board[4, 4] = Stone.Black;
            _board[3, 4] = Stone.White;
            _board[4, 3] = Stone.White;

            Living = true;

            ForceLoser = Stone.None;
        }

        /// <summary>
        /// 盤面の石の参照。
        /// </summary>
        /// <param name="x">X座標(水平方向)。</param>
        /// <param name="y">Y座標(垂直方向)。</param>
        /// <returns>(x, y)地点の石。</returns>
        public Stone Ref(int x, int y)
        {
            return _board[y, x];
        }

        /// <summary>
        /// 現在の盤面の手数を表す。
        /// </summary>
        public int Hand
        {
            private set;
            get;
        }

        /// <summary>
        /// 現在の手番です。
        /// </summary>
        public Stone HandStone
        {
            get
            {
                return Hand % 2 != 0 ? Stone.Black : Stone.White;
            }
        }

        /// <summary>
        /// 盤面の出力。<br/>
        /// 何もない地点では "."、 <br/>
        /// 黒石は "o"、 <br/>
        /// 白石は "x"。
        /// </summary>
        public void PutBoard()
        {
            for (int i = 0; i < REnvironment.BoardY; i++)
            {
                for (int j = 0; j < REnvironment.BoardX; j++)
                    Console.Write(StoneToString(_board[i, j]));
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        /// <summary>
        /// 周囲の盤面を変更せずに石を置きます。
        /// </summary>
        /// <param name="x">X座標。</param>
        /// <param name="y">Y座標。</param>
        /// <param name="s">置く石。</param>
        private void PutStone(int x, int y, Stone s)
        {
            _board[y, x] = s;
        }

        /// <summary>
        /// 着手して周囲の盤面を変更します。
        /// </summary>
        /// <param name="x">X座標。</param>
        /// <param name="y">Y座標。</param>
        /// <param name="s">着手する石。</param>
        public ReverseParam PutStoneWithReverse(int x, int y, Stone s)
        {
            return GetPutParam(x, y, s, true);
        }

        /// <summary>
        /// 着手して盤面を変更する、または着手した際の情報を調査します。
        /// </summary>
        /// <param name="x">X座標。</param>
        /// <param name="y">Y座標。</param>
        /// <param name="me">着手する石。</param>
        /// <param name="IsPlace">実際に着手するかどうか。</param>
        /// <returns>着手した際の情報。</returns>
        public ReverseParam GetPutParam(int x, int y, Stone me, bool enablePlace = false)
        {
            var res = new ReverseParam();

            var enemy = Enemy(me);

            var canPlace = false;
            var placeableCount = 0;
            var reversedCount = 0;

            res.CanPlace = false;
            res.ReversedCount = 0;
            res.MyStoneCount = -1;
            res.EnemyStoneCount = -1;

            if (Ref(x, y) != Stone.None) return res;

            for (int i = 0; i < Around.Length; i++)
            {
                bool isEnemy = true;
                Direction dir = Around[i];
                var reversing = new List<int[]>();

                int ix = x, iy = y, count = 0;

                // d 方向に敵の石を見つける処理。
                while (isEnemy)
                {
                    int[] pos = GetPastPosition(ix, iy, dir);

                    ix = pos[0];
                    iy = pos[1];

                    //移動した先が盤外なら抜ける
                    if (!IsNormalPosition(ix, iy)) break;

                    //移動した方向に敵の石があるなら
                    if (Ref(ix, iy) == enemy)
                    {
                        //ひっくり返す候補として登録する
                        reversing.Add(new int[] { ix, iy });
                        count++;
                    }
                    else
                    {
                        isEnemy = false;
                    }
                }

                //敵の石がない、または盤の端まで敵の石の場合はひっくり返せないので
                //さっさと諦める
                if (count == 0 || isEnemy) continue;

                //敵の石が続いている、かつその先が空欄か自分の石なので自分の石かどうか調べる
                if (Ref(ix, iy) == me)
                {
                    //敵の石が続いている
                    canPlace |= true;

                    placeableCount++;
                    reversedCount += reversing.Count;

                    if (enablePlace)
                    {
                        foreach (int[] elem in reversing)
                            PutStone(elem[0], elem[1], me);
                    }
                }
            }

            res.CanPlace = canPlace;
            res.ReversedCount = reversedCount;

            if (enablePlace && canPlace)
            {
                Hand++;
                PutStone(x, y, me);
                int[] Counts = CountStones();
                switch (me)
                {
                    case Stone.Black: res.MyStoneCount = Counts[0]; res.EnemyStoneCount = Counts[1]; break;
                    case Stone.White: res.MyStoneCount = Counts[1]; res.EnemyStoneCount = Counts[0]; break;
                }
            }
            else
            {
                res.MyStoneCount = -1;
                res.EnemyStoneCount = -1;
            }

            return res;
        }

        /// <summary>
        /// 指定した石の数をカウントして返します。
        /// </summary>
        /// <param name="me">指定する石</param>
        /// <returns>盤上に存在する指定した石の数</returns>
        public int CountStone(Stone me)
        {
            int res = 0;

            for (int y = 0; y < REnvironment.BoardY; y++)
                for (int x = 0; x < REnvironment.BoardX; x++)
                    if (Ref(x, y) == me) res++;

            return res;
        }

        /// <summary>
        /// 石の数をカウントして返します。
        /// </summary>
        /// <returns>盤上に存在する{Black, White, Blank}の数</returns>
        public int[] CountStones()
        {
            int resBlack = 0;
            int resWhite = 0;
            int resBlank = 0;

            for (int y = 0; y < REnvironment.BoardY; y++)
                for (int x = 0; x < REnvironment.BoardX; x++)
                {
                    switch (Ref(x, y))
                    {
                        case Stone.Black: resBlack++; break;
                        case Stone.White: resWhite++; break;
                        default: resBlank++; break;
                    }
                }

            return new int[] { resBlack, resWhite, resBlank };
        }

        /// <summary>
        /// 指定した座標が盤面に入っているかを返します。
        /// </summary>
        /// <param name="x">X座標。</param>
        /// <param name="y">Y座標。</param>
        /// <returns></returns>
        public bool IsNormalPosition(int x, int y)
        {
            return 0 <= x && x < REnvironment.BoardX &&
                   0 <= y && y < REnvironment.BoardY;
        }

        /// <summary>
        /// Direction方向に移動した座標を返します。
        /// </summary>
        /// <param name="x">移動前のX座標。</param>
        /// <param name="y">移動前のY座標。</param>
        /// <param name="d">移動する方向。</param>
        /// <returns>移動後の{X, Y}。</returns>
        public int[] GetPastPosition(int x, int y, Direction d)
        {
            int ex = x + ((d & Direction.Left) != 0 ? -1 : (d & Direction.Right) != 0 ? +1 : 0);
            int ey = y + ((d & Direction.Up) != 0 ? -1 : (d & Direction.Down) != 0 ? +1 : 0);
            return new int[] { ex, ey };
        }

        /// <summary>
        /// 指定された石に対する敵の石を返します。
        /// </summary>
        /// <param name="s">条件となる石。</param>
        /// <returns>敵の石。</returns>
        public Stone Enemy(Stone s)
        {
            return (Stone)(((int)s) * -1);
        }

        /// <summary>
        /// 石から文字列への変換。
        /// </summary>
        /// <param name="stone">変換元の石。</param>
        /// <returns>変換後の文字列。</returns>
        private string StoneToString(Stone stone)
        {
            switch (stone)
            {
                case Stone.None: return ".";
                case Stone.Black: return "o";
                case Stone.White: return "x";
            }
            return "!";
        }

        /// <summary>
        /// 勝者表示の文字列です。
        /// </summary>
        public string Winner
        {
            get
            {
                if (ForceLoser != Stone.None)
                {
                    return string.Format("Winner is {0}", Enemy(ForceLoser));
                }
                var list = CountStones();
                if (list[0] == list[1]) return "Draw";
                return string.Format("Winner is {0}", list[0] > list[1] ? Stone.Black : Stone.White);
            }
        }

        /// <summary>
        /// 現在ゲームが進行しているかどうかを返します。
        /// </summary>
        public bool Living
        {
            private set;
            get;
        }

        /// <summary>
        /// 現在の手番をタイムアウトにして強制終了させます。
        /// </summary>
        public void Shutdown()
        {
            Living = false;
            ForceLoser = HandStone;
        }

        /// <summary>
        /// 時間切れで負けになった手番です。
        /// </summary>
        public Stone ForceLoser
        {
            private set;
           get;
        }

        /// <summary>
        /// 石を置く際に返される情報。
        /// </summary>
        public class ReverseParam
        {
            /// <summary>
            /// 着手可能だったかどうか。
            /// </summary>
            public bool CanPlace
            {
                set;
                get;
            }

            /// <summary>
            /// 手番の石の数。
            /// </summary>
            public int MyStoneCount
            {
                set;
                get;
            }

            /// <summary>
            /// 相手の石の数。
            /// </summary>
            public int EnemyStoneCount
            {
                set;
                get;
            }

            /// <summary>
            /// ひっくり返した石の数。
            /// </summary>
            public int ReversedCount
            {
                set;
                get;
            }
        }
    }
}

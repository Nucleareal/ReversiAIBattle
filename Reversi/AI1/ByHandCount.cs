using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.ReversiSystem.Logic;

namespace Reversi.AI1
{
    class ByHandCount : AIInterface
    {
        private static Random rand = new Random();
        private Queue<Node> _queue;
        Dictionary<string, int> dict;

        public int[] Solve(Board board)
        {
            _queue = new Queue<Node>();
            dict = new Dictionary<string, int>();

            List<int[]> list = board.PlaceablePoints;

            if (list.Count == 0) return new int[] { -1, -1 };
            
            foreach (var v in list)
            {
                dict.Add(string.Format("{0},{1}", v[0], v[1]), 0);
                _queue.Enqueue(new Node { Parent = v, Place = v, Board = new Board(board), Depth = 5, Value = 0 });
            }

            Search(board.HandStone);

            int max      = int.MinValue / 4;
            int[] result = new int[]{-1, -1};

            foreach (var v in dict)
            {
                if (v.Value > max)
                {
                    max = v.Value;
                    result = v.Key.Split(',').Select(int.Parse).ToArray();
                }
            }

            Console.WriteLine(string.Format("Selected ({0}, {1}) => {2}", result[0], result[1], max));

            GC.Collect(); //メモリがやばい

            return result;
        }

        public void Search(Stone me)
        {
            //Console.WriteLine("Thinking in {0} Counts and current deep is {1}", _queue.Count, _queue.Peek().Depth);

            while (_queue.Count != 0 && _queue.Peek().Depth > 0)
            {
                try
                {
                    var node = _queue.Dequeue();
                    Eval(node, me);
                }
                catch(OutOfMemoryException e)
                {
                    break;
                }
            }
        }

        public void Eval(Node node, Stone me)
        {
            Board.ReverseParam param = node.Board.PutStoneWithReverse(node.Place[0], node.Place[1], node.Board.HandStone);

            if (!param.CanPlace)
            {
                dict[string.Format("{0},{1}", node.Parent[0], node.Parent[1])] = int.MinValue / 2;
                return;
            }

            node.Depth -= 1;

            List<int[]> placeable = node.Board.PlaceablePoints;

            int placeableCount = placeable.Count;
            placeableCount *= placeableCount;

            //自身の評価

            if (node.Board.HandStone == me)
                node.Value += placeableCount;
            else
                node.Value -= placeableCount;

            dict[string.Format("{0},{1}", node.Parent[0], node.Parent[1])] += node.Value;

            //Console.WriteLine("{0},{1} Changed => {2}", node.Parent[0], node.Parent[1], node.Value);

            //次に置ける場所がない
            if (placeable.Count == 0)
            {
                if (node.Board.HandStone == me) //自分の番でパスするのは
                    dict[string.Format("{0},{1}", node.Parent[0], node.Parent[1])] = int.MinValue / 2; //最悪なのでその先は探索しない
                else
                {
                    //相手の番でパスする
                    Board past = new Board(node.Board);

                    past.Pass();

                    //かつ自分もパス
                    if (past.PlaceablePoints.Count == 0)
                    {
                        past.Pass();
                        int[] Counts = past.CountStones();
                        switch (me)
                        {
                            case Stone.Black:
                                {
                                    if (Counts[0] > Counts[1]) //両方共パスして勝っているなら
                                        dict[string.Format("{0},{1}", node.Parent[0], node.Parent[1])] = int.MaxValue / 2; //最高
                                    break;
                                }
                            case Stone.White:
                                {
                                    if(Counts[0] < Counts[1])
                                        dict[string.Format("{0},{1}", node.Parent[0], node.Parent[1])] = int.MaxValue / 2;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        //自分はパスしない(相手にだけパスさせる)なら
                        dict[string.Format("{0},{1}", node.Parent[0], node.Parent[1])] += 100; //評価を上げる
                    }
                }
            }

            //Console.WriteLine("{0},{1} Changed => {2}", node.Parent[0], node.Parent[1], node.Value);

            //次の世代を生成

            foreach (var n in placeable)
            {
                Node next = new Node() { Parent = node.Parent, Place = n, Board = new Board(node.Board), Depth = node.Depth, Value = node.Value };
                _queue.Enqueue(next);
            }
        }

        public string Name
        {
            get { return "NuclearealAI Marq"; }
        }

        public class Node
        {
            public int[] Parent { set; get; }
            public int[] Place { set; get; }
            public Board Board { set; get; }
            public int Depth;
            public int Value;
        }
    }
}

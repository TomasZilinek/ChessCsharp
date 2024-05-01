using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPB069
{
    public enum AIdifficulty
    {
        Easy, Medium, Hard
    }

    public class AIPlayer : Player
    {
        public AIdifficulty Difficulty { get; private set; }

        public AIPlayer(bool isWhite, AIdifficulty diffiulty) : base(isWhite)
        {
            Difficulty = diffiulty;
        }

        public override string GetNickname()
        {
            return Difficulty.ToString() + " AI";
        }

        public async void MakeMove()
        {
            Board board = ProgramLogic.Instance.Board;

            Task<(BoardPosition, BoardPosition, int)> task = new Task<(BoardPosition, BoardPosition, int)>(ComputeBestMove);
            task.Start();

            (BoardPosition, BoardPosition, int) result = await task;
            Console.WriteLine(string.Format("best move, from: {0} to {1}, score = {2}", result.Item1,
                                                                                        result.Item2,
                                                                                        result.Item3));

            board.TryMakeMove(result.Item1, result.Item2, FigureType.Queen, null);
        }

        private (BoardPosition, BoardPosition, int) ComputeBestMove()
        {
            return Minimax(ProgramLogic.Instance.Board.BoardRepresentation, (int)Difficulty + 2, int.MinValue, int.MaxValue, IsWhite);
        }

        private (BoardPosition, BoardPosition, int) Minimax(BoardRepresentation boardRepresentation, int depth, int alpha,
                                                            int beta, bool whiteIsMoving)
        {
            if (depth == 0)
                return (new BoardPosition(), new BoardPosition(), PositionEvaluation.EvaluatePosition(boardRepresentation));

            if (whiteIsMoving)
            {
                (BoardPosition, BoardPosition,  int) maxEval = (new BoardPosition(), new BoardPosition(), int.MinValue);

                foreach (Figure whiteFigure in boardRepresentation.WhiteAliveFigures)
                {
                    foreach (BoardPosition toPosition in whiteFigure.GeneratePossibleMovesPositions())
                    {
                        BoardPosition fromPosition = whiteFigure.BoardPosition;

                        if (fromPosition == toPosition)
                            continue;

                        BoardRepresentation copyRepr = new BoardRepresentation(boardRepresentation);

                        if (!copyRepr.TryMakeMove(fromPosition, toPosition, out _, FigureType.Queen, out _, out _, out _))
                        {
                            //Console.WriteLine(string.Format("move from {0} to {1} impossible", fromPosition.ToChessNotation(),
                            //                                                                   toPosition.ToChessNotation()));
                            continue;
                        }

                        (BoardPosition, BoardPosition, int) eval = Minimax(copyRepr, depth - 1, alpha, beta, !whiteIsMoving);
                        /*Console.WriteLine(string.Format("current eval, from: {0}, to: {1}, score: {2}, depth: {3}",
                                                             fromPosition.ToChessNotation(),
                                                             toPosition.ToChessNotation(), eval.Item3, depth));*/

                        if (eval.Item3 > maxEval.Item3)
                            maxEval = (fromPosition, toPosition, eval.Item3);

                        if (eval.Item3 > alpha)
                            alpha = eval.Item3;

                        if (beta <= alpha)
                        {
                            /*Console.WriteLine(string.Format("max in if, from: {0}, to: {1}, score: {2}, depth: {3}",
                                                             maxEval.Item1.ToChessNotation(),
                                                             maxEval.Item2.ToChessNotation(), maxEval.Item3, depth));*/
                            return maxEval;
                        }
                    }
                }

                /*Console.WriteLine(string.Format("max in ret, from: {0}, to: {1}, score: {2}, depth: {3}",
                                                maxEval.Item1.ToChessNotation(),
                                                maxEval.Item2.ToChessNotation(), maxEval.Item3, depth));*/

                return maxEval;
            }
            else
            {
                (BoardPosition, BoardPosition, int) minEval = (new BoardPosition(), new BoardPosition(), int.MaxValue);

                foreach (Figure blackFigure in boardRepresentation.BlackAliveFigures)
                {
                    foreach (BoardPosition toPosition in blackFigure.GeneratePossibleMovesPositions())
                    {
                        BoardPosition fromPosition = blackFigure.BoardPosition;

                        if (fromPosition == toPosition)
                            continue;

                        BoardRepresentation copyRepr = new BoardRepresentation(boardRepresentation);

                        if (!copyRepr.TryMakeMove(fromPosition, toPosition, out _, FigureType.Queen, out _, out _, out _))
                            continue;

                        (BoardPosition, BoardPosition, int) eval = Minimax(copyRepr, depth - 1, alpha, beta, !whiteIsMoving);
                        /*Console.WriteLine(string.Format("current eval, from: {0}, to: {1}, score: {2}, depth: {3}",
                                                             fromPosition.ToChessNotation(),
                                                             toPosition.ToChessNotation(), eval.Item3, depth));*/

                        if (eval.Item3 < minEval.Item3)
                            minEval = (fromPosition, toPosition, eval.Item3);

                        if (eval.Item3 < beta)
                            beta = eval.Item3;

                        if (beta <= alpha)
                        {
                            /*Console.WriteLine(string.Format("min in if, from: {0}, to: {1}, score: {2}",
                                                             minEval.Item1.ToChessNotation(),
                                                             minEval.Item2.ToChessNotation(), minEval.Item3));*/
                            return minEval;
                        }
                    }
                }

                /*Console.WriteLine(string.Format("min in ret, from: {0}, to: {1}, score: {2}",
                                                minEval.Item1.ToChessNotation(),
                                                minEval.Item2.ToChessNotation(), minEval.Item3));*/

                return minEval;
            }
        }
    }
}

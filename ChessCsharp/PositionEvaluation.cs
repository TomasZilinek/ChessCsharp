using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPB069
{
    public static class PositionEvaluation
    {
        private static int[] figureTypesValues = new int[] { 500, 320, 330, 900, 20000, 100 };

        private static int[,] pawnRewardFields = new int[8, 8] {
            { 0,  0,  0,   0,   0,   0,   0,  0 },
            { 50, 50, 50,  50,  50,  50,  50, 50 },
            { 10, 10, 20,  30,  30,  20,  10, 10 },
            { 5,  5,  10,  25,  25,  10,  5,  5 },
            { 0,  0,  0,   20,  20,  0,   0,  0 },
            { 5,  -5, -10, 0,   0,   -10, -5, 5 },
            { 5,  10, 10,  -20, -20, 10,  10, 5 },
            { 0,  0,  0,   0,   0,   0,   0,  0 }
        };

        private static int[,] queenRewardFields = new int[8, 8] {
            { -20,-10,-10, -5, -5,-10,-10,-20 },
            { -10,  0,  0,  0,  0,  0,  0,-10 },
            { -10,  0,  5,  5,  5,  5,  0,-10 },
            {  -5,  0,  5,  5,  5,  5,  0, -5 },
            {   0,  0,  5,  5,  5,  5,  0, -5 },
            { -10,  5,  5,  5,  5,  5,  0,-10 },
            { -10,  0,  5,  0,  0,  0,  0,-10 },
            { -20,-10,-10, -5, -5,-10,-10,-20 }
        };

        private static int[,] rookRewardFields = new int[8, 8] {
            { 0,  0,  0,  0,  0,  0,  0,  0 },
            { 5, 10, 10, 10, 10, 10, 10,  5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            {  0,  0,  0,  5,  5,  0,  0,  0 }
        };

        private static int[,] knightRewardFields = new int[8, 8] {
            { -50,-40,-30,-30,-30,-30,-40,-50 },
            { -40,-20,  0,  0,  0,  0,-20,-40 },
            { -30,  0, 10, 15, 15, 10,  0,-30 },
            { -30,  5, 15, 20, 20, 15,  5,-30 },
            { -30,  0, 15, 20, 20, 15,  0,-30 },
            { -30,  5, 10, 15, 15, 10,  5,-30 },
            { -40,-20,  0,  5,  5,  0,-20,-40 },
            { -50,-40,-30,-30,-30,-30,-40,-50 }
        };

        private static int[,] bishopRewardFields = new int[8, 8] {
            { -20,-10,-10,-10,-10,-10,-10,-20 },
            { -10,  0,  0,  0,  0,  0,  0,-10 },
            { -10,  0,  5, 10, 10,  5,  0,-10 },
            { -10,  5,  5, 10, 10,  5,  5,-10 },
            { -10,  0, 10, 10, 10, 10,  0,-10 },
            { -10, 10, 10, 10, 10, 10, 10,-10 },
            { -10,  5,  0,  0,  0,  0,  5,-10 },
            { -20,-10,-10,-10,-10,-10,-10,-20 }
        };

        private static int[,] kingRewardFields = new int[8, 8] {
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -20,-30,-30,-40,-40,-30,-30,-20 },
            { -10,-20,-20,-20,-20,-20,-20,-10 },
            {  20, 20,  0,  0,  0,  0, 20, 20 },
            {  20, 30, 10,  0,  0, 10, 30, 20 }
        };

        public static int EvaluatePositionOld(BoardRepresentation boardRepresentation)
        {
            int[] whiteFiguresCounts = new int[6];
            int[] blackFiguresCounts = new int[6];

            foreach (Figure figure in boardRepresentation.WhiteAliveFigures)
            {
                whiteFiguresCounts[(int)figure.FigureType]++;
            }

            foreach (Figure figure in boardRepresentation.BlackAliveFigures)
            {
                blackFiguresCounts[(int)figure.FigureType]++;
            }

            return 9 * (whiteFiguresCounts[(int)FigureType.Queen] - blackFiguresCounts[(int)FigureType.Queen]) +
                   5 * (whiteFiguresCounts[(int)FigureType.Rook] - blackFiguresCounts[(int)FigureType.Rook]) +
                   3 * (whiteFiguresCounts[(int)FigureType.Knight] - blackFiguresCounts[(int)FigureType.Knight]) +
                   3 * (whiteFiguresCounts[(int)FigureType.Bishop] - blackFiguresCounts[(int)FigureType.Bishop]) +
                   1 * (whiteFiguresCounts[(int)FigureType.Pawn] - blackFiguresCounts[(int)FigureType.Pawn]) +
                   1 * ((boardRepresentation.IsInCheck(false) ? 1 : 0) - (boardRepresentation.IsInCheck(true) ? 1 : 0)) +
                   int.MaxValue * ((boardRepresentation.IsInCheckMate(false) ? 1 : 0) - (boardRepresentation.IsInCheckMate(true) ? 1 : 0));
        }

        public static int EvaluatePosition(BoardRepresentation boardRepresentation)
        {
            int whitePlayerScore = 0;
            int blackPlayerScore = 0;

            foreach (Figure figure in boardRepresentation.WhiteAliveFigures)
                whitePlayerScore += EvaluateFigure(figure);

            foreach (Figure figure in boardRepresentation.BlackAliveFigures)
                blackPlayerScore += EvaluateFigure(figure);

            return whitePlayerScore - blackPlayerScore;
        }

        private static int EvaluateFigure(Figure figure)
        {
            BoardPosition arrayPosition = new BoardPosition(figure.BoardPosition.X,
                                                            figure.IsWhite ? figure.BoardPosition.Y : 7 - figure.BoardPosition.Y);

            switch (figure.FigureType)
            {
                case FigureType.Pawn:
                    return figureTypesValues[(int)figure.FigureType] + pawnRewardFields[arrayPosition.Y, arrayPosition.X];
                case FigureType.Queen:
                    return figureTypesValues[(int)figure.FigureType] + queenRewardFields[arrayPosition.Y, arrayPosition.X];
                case FigureType.Rook:
                    return figureTypesValues[(int)figure.FigureType] + rookRewardFields[arrayPosition.Y, arrayPosition.X];
                case FigureType.Knight:
                    return figureTypesValues[(int)figure.FigureType] + knightRewardFields[arrayPosition.Y, arrayPosition.X];
                case FigureType.Bishop:
                    return figureTypesValues[(int)figure.FigureType] + bishopRewardFields[arrayPosition.Y, arrayPosition.X];
                case FigureType.King:
                    return figureTypesValues[(int)figure.FigureType] + kingRewardFields[arrayPosition.Y, arrayPosition.X];
            }

            return 0;
        }
    }
}

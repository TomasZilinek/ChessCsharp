using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPB069
{
    public class Figure
    {
        public bool IsWhite { get; private set; }
        public FigureType FigureType { get; private set; }

        public BoardPosition BoardPosition { get; set; }
        public BoardPosition PreviousPosition { get; set; }
        public bool IsLeftSideFigure { get; private set; }
        public bool AlreadyMoved { get; private set; }

        public Figure(bool isWhite, FigureType _figureType, BoardPosition position, bool isLeftSidefigure)
        {
            IsWhite = isWhite;
            FigureType = _figureType;
            BoardPosition = position;
            PreviousPosition = position;
            IsLeftSideFigure = isLeftSidefigure;
            AlreadyMoved = false;
        }

        public Figure(Figure oldFigure)
        {
            IsWhite = oldFigure.IsWhite;
            FigureType = oldFigure.FigureType;
            BoardPosition = oldFigure.BoardPosition;
            PreviousPosition = oldFigure.PreviousPosition;
            IsLeftSideFigure = oldFigure.IsLeftSideFigure;
            AlreadyMoved = oldFigure.AlreadyMoved;
        }

        public void MoveTo(BoardPosition position)
        {
            BoardPosition = position;
            AlreadyMoved = true;
        }

        public List<BoardPosition> GeneratePossibleMovesPositions()
        {
            List<BoardPosition> result = new List<BoardPosition>();

            switch (FigureType)
            {
                case FigureType.Pawn:
                    int direction = IsWhite ? -1 : 1;
                    BoardPosition pos;

                    for (int y = 1; y <= 2; y++)
                    {
                        pos = BoardPosition + new BoardPosition(0, y * direction);

                        if (pos.IsValidBoardPosition())
                            result.Add(pos);
                    }

                    // possible anpassant positions
                    pos = BoardPosition + new BoardPosition(1, direction);

                    if (pos.IsValidBoardPosition())
                        result.Add(pos);

                    pos = BoardPosition + new BoardPosition(-1, direction);

                    if (pos.IsValidBoardPosition())
                        result.Add(pos);

                    return result;
                case FigureType.Queen:
                    result.AddRange(new Figure(IsWhite, FigureType.Bishop, BoardPosition, false).GeneratePossibleMovesPositions());
                    result.AddRange(new Figure(IsWhite, FigureType.Rook, BoardPosition, false).GeneratePossibleMovesPositions());
                    return result;
                case FigureType.Rook:
                    for (int x = 0; x < 7; x++)
                        result.Add(new BoardPosition(x, BoardPosition.Y));

                    for (int y = 0; y < 7; y++)
                        result.Add(new BoardPosition(BoardPosition.X, y));

                    return result;
                case FigureType.Knight:
                    for (int y = -2; y <= 2; y += 4)
                    {
                        pos = BoardPosition + new BoardPosition(1, y);

                        if (pos.IsValidBoardPosition())
                            result.Add(pos);

                        pos = BoardPosition + new BoardPosition(-1, y);

                        if (pos.IsValidBoardPosition())
                            result.Add(pos);
                    }

                    for (int x = -2; x <= 2; x += 4)
                    {
                        pos = BoardPosition + new BoardPosition(x, 1);

                        if (pos.IsValidBoardPosition())
                            result.Add(pos);

                        pos = BoardPosition + new BoardPosition(x, -1);

                        if (pos.IsValidBoardPosition())
                            result.Add(pos);
                    }

                    return result;
                case FigureType.Bishop:
                    int[,] directions = new int[4, 2] { { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };

                    for (int i = 0; i < 4; i++)
                    {
                        pos = new BoardPosition(0, 0);
                        int dist = 1;

                        while (true)
                        {
                            pos = BoardPosition + new BoardPosition(dist * directions[i, 0], dist * directions[i, 1]);

                            if (pos.IsValidBoardPosition())
                                result.Add(pos);
                            else
                                break;

                            dist++;
                        }
                    }

                    return result;
                case FigureType.King:
                    for (int y = BoardPosition.Y - 1; y <= BoardPosition.Y + 1; y++)
                    {
                        for (int x = BoardPosition.X - 1; x <= BoardPosition.X + 1; x++)
                        {
                            BoardPosition newBoardPosition = new BoardPosition(x, y);

                            if ((x == BoardPosition.X && y == BoardPosition.Y) || !newBoardPosition.IsValidBoardPosition())
                                continue;

                            result.Add(newBoardPosition);
                        }
                    }

                    return result;
            }

            return result;
        }

        public override string ToString()
        {
            return "Figure{ " + FigureType.ToString() + ", " + BoardPosition.ToString() + " }";
        }
    }
}

using System;
using System.Collections.Generic;

namespace ChessPB069
{
    public static class ChessRules
    {
        /// <summary>
        /// returns true only if the move is valid. Does not take check threat and other things into consideration
        /// </summary>
        public static bool IsValidMove(BoardRepresentation boardRepresentation, BoardPosition fromPosition, BoardPosition toPosition,
                                       out Figure figureTakenByEnpassant)
        {
            Figure figureAtFromPosition = boardRepresentation.FigureAtPosition(fromPosition);
            Figure figureAtToPosition = boardRepresentation.FigureAtPosition(toPosition);

            if (fromPosition == toPosition ||
                (figureAtFromPosition != null && figureAtToPosition != null && figureAtFromPosition.IsWhite == figureAtToPosition.IsWhite))
            {
                figureTakenByEnpassant = null;
                return false;
            }

            bool isWhite = figureAtFromPosition.IsWhite;

            if (figureAtFromPosition == null || !fromPosition.IsValidBoardPosition() || !toPosition.IsValidBoardPosition())
            {
                figureTakenByEnpassant = null;
                return false;
            }

            int positionsYdifference = toPosition.Y - fromPosition.Y;
            int positionsXdifference = toPosition.X - fromPosition.X;

            if (figureAtFromPosition.FigureType == FigureType.Pawn)
            {
                if (figureAtToPosition == null)  // move forward or enpassant
                {
                    if ((isWhite && positionsYdifference == -1) || (!isWhite && positionsYdifference == 1))  // jumps by 1 or enpassant
                    {
                        if (positionsXdifference == 0)  // just move forward
                        {
                            figureTakenByEnpassant = null;
                            return true;
                        }
                        else if (Math.Abs(positionsXdifference) == 1)  // enpassant
                        {
                            figureTakenByEnpassant = boardRepresentation.LastMovedFigure;

                            return boardRepresentation.LastMovedFigure != null &&
                                   boardRepresentation.LastMovedFigure.FigureType == FigureType.Pawn &&
                                   boardRepresentation.LastMoveFromPosition.Y == toPosition.Y + (isWhite ? -1 : 1) &&
                                   boardRepresentation.LastMoveToPosition.Y == toPosition.Y + (isWhite ? 1 : -1) &&
                                   Math.Abs(boardRepresentation.LastMoveToPosition.X - toPosition.X) == 0;
                        }
                    }
                    else if (((isWhite && positionsYdifference == -2) || (!isWhite && positionsYdifference == 2)) && positionsXdifference == 0)
                    {
                        // jump by 2

                        figureTakenByEnpassant = null;

                        BoardPosition betweenPosition = fromPosition + new BoardPosition(0, isWhite ? -1 : 1);

                        if (figureAtFromPosition.AlreadyMoved || boardRepresentation.FigureAtPosition(betweenPosition) != null)
                            return false;

                        return true;
                    }
                }
                else  // take figure
                {
                    figureTakenByEnpassant = null;

                    return figureAtToPosition.IsWhite != figureAtFromPosition.IsWhite &&
                           ((isWhite && positionsYdifference == -1) || (!isWhite && positionsYdifference == 1)) &&
                           Math.Abs(positionsXdifference) == 1;
                }
            }
            else if (figureAtFromPosition.FigureType == FigureType.Rook)
            {
                figureTakenByEnpassant = null;
                return IsValidRookMove(boardRepresentation, fromPosition, toPosition);
            }
            else if (figureAtFromPosition.FigureType == FigureType.Knight)
            {
                figureTakenByEnpassant = null;

                return (Math.Abs(positionsXdifference) == 1 && Math.Abs(positionsYdifference) == 2) ||
                       (Math.Abs(positionsXdifference) == 2 && Math.Abs(positionsYdifference) == 1);
            }
            else if (figureAtFromPosition.FigureType == FigureType.Bishop)
            {
                figureTakenByEnpassant = null;
                return IsValidBishopMoveMove(boardRepresentation, fromPosition, toPosition);
            }
            else if (figureAtFromPosition.FigureType == FigureType.Queen)
            {
                figureTakenByEnpassant = null;

                return IsValidRookMove(boardRepresentation, fromPosition, toPosition) ||
                       IsValidBishopMoveMove(boardRepresentation, fromPosition, toPosition);
            }
            else if (figureAtFromPosition.FigureType == FigureType.King)
            {
                figureTakenByEnpassant = null;

                return (Math.Abs(positionsXdifference) < 2 && Math.Abs(positionsYdifference) < 2) ||
                       KingCastling(boardRepresentation, fromPosition, toPosition);
            }

            figureTakenByEnpassant = null;
            return false;
        }

        private static bool KingCastling(BoardRepresentation repr, BoardPosition fromPosition, BoardPosition toPosition)
        {
            Figure kingFigure = repr.FigureAtPosition(fromPosition);

            if (kingFigure.FigureType != FigureType.King || kingFigure.AlreadyMoved)
                return false;

            int positionsYdifference = toPosition.Y - fromPosition.Y;
            int positionsXdifference = toPosition.X - fromPosition.X;

            if (positionsYdifference != 0)
                return false;

            if (Math.Abs(positionsXdifference) == 2)
            {
                BoardPosition rookPosition = toPosition + new BoardPosition(positionsXdifference == 2 ? 1 : -2, 0);

                if (!rookPosition.IsValidBoardPosition())
                    return false;

                Figure figureAtRookPosition = repr.FigureAtPosition(rookPosition);

                if (figureAtRookPosition == null || figureAtRookPosition.FigureType != FigureType.Rook || figureAtRookPosition.AlreadyMoved)
                    return false;

                return NoUnitBetweenPositionsHorizontal(repr, fromPosition, rookPosition);
            }
            else
                return false;
        }

        private static bool NoUnitBetweenPositionsHorizontal(BoardRepresentation repr, BoardPosition pos1, BoardPosition pos2)
        {
            if (Math.Abs(pos1.X - pos2.X) < 2)
                return true;

            BoardPosition smaller = pos1.X < pos2.X ? pos1 : pos2;
            BoardPosition larger = pos1 == smaller ? pos2 : pos1;

            for (int x = smaller.X + 1; x < larger.X; x++)
            {
                if (repr.FigureAtPosition(new BoardPosition(x, smaller.Y)) != null)
                    return false;
            }

            return true;
        }

        private static bool NoUnitBetweenPositionsVertical(BoardRepresentation repr, BoardPosition pos1, BoardPosition pos2)
        {
            if (Math.Abs(pos1.Y - pos2.Y) < 2)
                return true;

            BoardPosition smaller = pos1.Y < pos2.Y ? pos1 : pos2;
            BoardPosition larger = pos1 == smaller ? pos2 : pos1;

            for (int y = smaller.Y + 1; y < larger.Y; y++)
            {
                if (repr.FigureAtPosition(new BoardPosition(smaller.X, y)) != null)
                    return false;
            }

            return true;
        }

        private static bool NoUnitBetweenPositionsDiagonal(BoardRepresentation repr, BoardPosition pos1, BoardPosition pos2)
        {
            if (Math.Abs(pos1.Y - pos2.Y) < 2)  // or with X, does not matter
                return true;

            BoardPosition smallerX = pos1.X < pos2.X ? pos1 : pos2;
            BoardPosition largerX = pos1 == smallerX ? pos2 : pos1;

            int yIncrement = largerX.Y < smallerX.Y ? -1 : 1;
            int c = 1;

            for (int x = smallerX.X + 1; x < largerX.X; x++)
            {
                if (repr.FigureAtPosition(new BoardPosition(x, smallerX.Y + c * yIncrement)) != null)
                    return false;

                c++;
            }

            return true;
        }

        private static bool PositionsAtTheSameHorizontalLine(BoardPosition pos1, BoardPosition pos2)
        {
            return pos1.X != pos2.X && pos1.Y == pos2.Y;
        }

        private static bool PositionsAtTheSameVerticalLine(BoardPosition pos1, BoardPosition pos2)
        {
            return pos1.X == pos2.X && pos1.Y != pos2.Y;
        }

        private static bool PositionsAtTheSameDiagonal(BoardPosition pos1, BoardPosition pos2)
        {
            return Math.Abs(pos1.X - pos2.X) == Math.Abs(pos1.Y - pos2.Y);
        }

        private static bool IsValidRookMove(BoardRepresentation repr, BoardPosition fromPosition, BoardPosition toPosition)
        {
            bool atSameHorizontal = PositionsAtTheSameHorizontalLine(fromPosition, toPosition);
            bool atSameVertical = PositionsAtTheSameVerticalLine(fromPosition, toPosition);

            if (!(atSameHorizontal || atSameVertical))
            {
                return false;
            }

            return (atSameHorizontal && NoUnitBetweenPositionsHorizontal(repr, fromPosition, toPosition)) ||
                   (atSameVertical && NoUnitBetweenPositionsVertical(repr, fromPosition, toPosition));
        }

        private static bool IsValidBishopMoveMove(BoardRepresentation repr, BoardPosition fromPosition, BoardPosition toPosition)
        {
            return PositionsAtTheSameDiagonal(fromPosition, toPosition) &&
                   NoUnitBetweenPositionsDiagonal(repr, fromPosition, toPosition);
        }

        /// <summary>
        /// returns true if it is possible to make the move, takes check and threats into consideration
        /// </summary>
        public static bool IsPossibleMove(BoardRepresentation boardRepresentation, BoardPosition fromPosition, BoardPosition toPosition,
                                          out Figure figureTakenByEnpassant)
        {
            if (!IsValidMove(boardRepresentation, fromPosition, toPosition, out Figure _figureKilledByEmpassant))
            {
                figureTakenByEnpassant = _figureKilledByEmpassant;
                return false;
            }

            Figure figureAtFromPosition = boardRepresentation.FigureAtPosition(fromPosition);
            bool movingPlayerIsWhite = figureAtFromPosition.IsWhite;

            // if castling then check if king passes through a threatened field
            if (figureAtFromPosition.FigureType == FigureType.King && Math.Abs(fromPosition.X - toPosition.X) == 2)
            {
                for (int x = Math.Min(fromPosition.X, toPosition.X); x <= Math.Max(fromPosition.X, toPosition.X); x++)
                {
                    List<Figure> enemyFigures = movingPlayerIsWhite ?
                        boardRepresentation.BlackAliveFigures : boardRepresentation.WhiteAliveFigures;

                    foreach (Figure enemyFigure in enemyFigures)
                    {
                        if (IsValidMove(boardRepresentation, enemyFigure.BoardPosition, new BoardPosition(x, fromPosition.Y), out _))
                        {
                            figureTakenByEnpassant = null;
                            return false;
                        }
                    }
                }
            }

            figureTakenByEnpassant = _figureKilledByEmpassant;

            BoardRepresentation copyRepr = new BoardRepresentation(boardRepresentation);
            Figure movingFigure = copyRepr.FigureAtPosition(fromPosition);

            copyRepr.MakeMove(fromPosition, toPosition, FigureType.Queen);
            
            return !copyRepr.IsInCheck(movingFigure.IsWhite);
        }
    }
}

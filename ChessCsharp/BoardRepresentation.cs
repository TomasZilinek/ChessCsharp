using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPB069
{
    public class BoardRepresentation
    {
        public List<Figure> WhiteAliveFigures { get; private set; }
        public List<Figure> BlackAliveFigures { get; private set; }

        public Figure LastMovedFigure { get; private set; }
        public BoardPosition LastMoveFromPosition { get; private set; }
        public BoardPosition LastMoveToPosition { get; private set; }

        public BoardRepresentation()  // new chessboard
        {
            WhiteAliveFigures = new List<Figure>();
            BlackAliveFigures = new List<Figure>();

            for (int i = 0; i < 8; i++)
            {
                WhiteAliveFigures.Add(new Figure(isWhite: true, FigureType.Pawn, new BoardPosition(i, 6), i < 4));
                BlackAliveFigures.Add(new Figure(isWhite: false, FigureType.Pawn, new BoardPosition(i, 1), i < 4));
            }

            for (FigureType figureType = FigureType.Rook; figureType < FigureType.Pawn; figureType++)
            {
                WhiteAliveFigures.Add(new Figure(isWhite: true, figureType, new BoardPosition((int)figureType, 7), figureType <= FigureType.Queen));
                BlackAliveFigures.Add(new Figure(isWhite: false, figureType, new BoardPosition((int)figureType, 0), figureType <= FigureType.Queen));

                if (figureType < FigureType.Queen)
                {
                    WhiteAliveFigures.Add(new Figure(isWhite: true, figureType, new BoardPosition(7 - (int)figureType, 7), false));
                    BlackAliveFigures.Add(new Figure(isWhite: false, figureType, new BoardPosition(7 - (int)figureType, 0), false));
                }
            }
        }

        public BoardRepresentation(BoardRepresentation oldRepresentation)
        {
            WhiteAliveFigures = new List<Figure>(oldRepresentation.WhiteAliveFigures.Select(figure => new Figure(figure)));
            BlackAliveFigures = new List<Figure>(oldRepresentation.BlackAliveFigures.Select(figure => new Figure(figure)));
            LastMovedFigure = oldRepresentation.LastMovedFigure == null ? null : new Figure(oldRepresentation.LastMovedFigure);
            LastMoveFromPosition = oldRepresentation.LastMoveFromPosition;
            LastMoveToPosition = oldRepresentation.LastMoveToPosition;
        }

        public Figure FigureAtPosition(BoardPosition position)
        {
            if (!position.IsValidBoardPosition())
                return null;

            foreach (Figure whiteFigure in WhiteAliveFigures)
            {
                if (whiteFigure.BoardPosition == position)
                    return whiteFigure;
            }

            foreach (Figure blackFigure in BlackAliveFigures)
            {
                if (blackFigure.BoardPosition == position)
                    return blackFigure;
            }

            return null;
        }

        public bool TryMakeMove(BoardPosition fromPosition, BoardPosition toPosition, out Figure figureTakenByEnpassant,
                                FigureType figureTypeToPromote, out FigureType takenFigureType, out bool wasCheck, out bool wasCheckMate)
        {
            if (ChessRules.IsPossibleMove(this, fromPosition, toPosition, out Figure _figureTakenByEnpassant))
            {
                Figure figureAtFromPosition = FigureAtPosition(fromPosition);
                Figure figureAtToPosition = FigureAtPosition(toPosition);

                figureAtFromPosition.MoveTo(toPosition);

                LastMoveFromPosition = fromPosition;
                LastMoveToPosition = toPosition;
                LastMovedFigure = figureAtFromPosition;

                takenFigureType = FigureType.King;

                if (_figureTakenByEnpassant != null)
                {
                    takenFigureType = FigureType.Pawn;
                    RemoveFigure(_figureTakenByEnpassant);
                }

                if (figureAtToPosition != null)
                {
                    takenFigureType = figureAtToPosition.FigureType;
                    RemoveFigure(figureAtToPosition);
                }
                else if (_figureTakenByEnpassant == null)
                    takenFigureType = FigureType.King;

                if (figureAtFromPosition.FigureType == FigureType.King && Math.Abs(fromPosition.X - toPosition.X) == 2)
                {
                    if (toPosition.X < 4)
                    {
                        FigureAtPosition(toPosition - new BoardPosition(2, 0)).MoveTo(toPosition + new BoardPosition(1, 0));
                    }
                    else
                    {
                        FigureAtPosition(toPosition + new BoardPosition(1, 0)).MoveTo(toPosition - new BoardPosition(1, 0));
                    }
                }

                // promoting
                if (figureAtFromPosition.FigureType == FigureType.Pawn && (toPosition.Y == 0 || toPosition.Y == 7))
                {
                    Figure promotedFigure = new Figure(figureAtFromPosition.IsWhite, figureTypeToPromote, toPosition, false);

                    RemoveFigure(figureAtFromPosition);
                    AddFigure(promotedFigure);
                }

                figureTakenByEnpassant = _figureTakenByEnpassant;
                wasCheck = IsInCheck(!figureAtFromPosition.IsWhite);
                wasCheckMate = IsInCheckMate(!figureAtFromPosition.IsWhite);

                return true;
            }

            figureTakenByEnpassant = null;
            takenFigureType = FigureType.King;
            wasCheck = wasCheckMate = false;

            return false;
        }

        /// <summary>
        /// like TryMakeMove but does not check move validity by ChessRules.IsPossibleMove, only by ChessRules.IsValidMove
        /// </summary>
        public void MakeMove(BoardPosition fromPosition, BoardPosition toPosition, FigureType figureTypeToPromote)
        {
            Figure figureAtFromPosition = FigureAtPosition(fromPosition);

            if (ChessRules.IsValidMove(this, fromPosition, toPosition, out Figure _figureTakenByEnpassant))
            {
                Figure figureAtToPosition = FigureAtPosition(toPosition);

                figureAtFromPosition.MoveTo(toPosition);

                LastMoveFromPosition = fromPosition;
                LastMoveToPosition = toPosition;
                LastMovedFigure = figureAtFromPosition;

                if (_figureTakenByEnpassant != null)
                {
                    RemoveFigure(_figureTakenByEnpassant);
                }

                if (figureAtToPosition != null)
                {
                    RemoveFigure(figureAtToPosition);
                }

                if (figureAtFromPosition.FigureType == FigureType.King && Math.Abs(fromPosition.X - toPosition.X) == 2)
                {
                    if (toPosition.X < 4)
                    {
                        FigureAtPosition(toPosition - new BoardPosition(2, 0)).MoveTo(toPosition + new BoardPosition(1, 0));
                    }
                    else
                    {
                        FigureAtPosition(toPosition + new BoardPosition(1, 0)).MoveTo(toPosition - new BoardPosition(1, 0));
                    }
                }

                // promoting
                if (figureAtFromPosition.FigureType == FigureType.Pawn && (toPosition.Y == 0 || toPosition.Y == 7))
                {
                    Figure promotedFigure = new Figure(figureAtFromPosition.IsWhite, figureTypeToPromote, toPosition, false);

                    RemoveFigure(figureAtFromPosition);
                    AddFigure(promotedFigure);
                }
            }
        }

        public void RemoveFigure(Figure figure)
        {
            if (figure.IsWhite)
                WhiteAliveFigures.Remove(figure);
            else
                BlackAliveFigures.Remove(figure);
        }

        public void AddFigure(Figure figure)
        {
            if (figure.IsWhite)
                WhiteAliveFigures.Add(figure);
            else
                BlackAliveFigures.Add(figure);
        }

        public List<Figure> GetAliveFiguresByColor(bool isWhite)
        {
            if (isWhite)
                return WhiteAliveFigures;
            else
                return BlackAliveFigures;
        }

        public bool IsInCheck(bool isWhite)
        {
            List<Figure> opponentFigures = isWhite ? BlackAliveFigures : WhiteAliveFigures;
            Figure allyKing = (isWhite ? WhiteAliveFigures : BlackAliveFigures).Single(f => f.FigureType == FigureType.King);

            foreach (Figure opponentFigure in opponentFigures)
            {
                if (ChessRules.IsValidMove(this, opponentFigure.BoardPosition, allyKing.BoardPosition, out _))
                    return true;
            }

            return false;
        }

        public bool IsInCheckMate(bool isWhite)
        {
            if (!IsInCheck(isWhite))
                return false;

            foreach (Figure figure in GetAliveFiguresByColor(isWhite))
            {
                var positions = figure.GeneratePossibleMovesPositions();

                foreach (BoardPosition toPosition in positions)
                {
                    if (ChessRules.IsPossibleMove(this, figure.BoardPosition, toPosition, out _))
                        return false;
                }
            }

            return true;
        }

        public List<BoardPosition> GenerateAllPossibleMoves(bool isWhite)
        {
            return GetAliveFiguresByColor(isWhite).SelectMany(x => x.GeneratePossibleMovesPositions()).ToList();
        }
    }
}

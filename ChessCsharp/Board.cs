using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessPB069
{
    public class Board
    {
        public BoardField[,] Fields { get; private set; }
        public Label[] boardNumbersLabels { get; private set; }
        public Label[] boardLettersLabels { get; private set; }

        public BoardRepresentation BoardRepresentation { get; private set; }

        private Panel gameBoardFieldsPanel;
        private Panel gameBoardNumberPanel;

        public bool OrientedByWhite { get; private set; }

        private BoardField selectedField;

        public bool ShowsPossibleMoves { get; private set; }
        public bool ShowsThreats { get; private set; }

        public Board()
        {
            selectedField = null;
            OrientedByWhite = true;
            ShowsPossibleMoves = ShowsThreats = false;

            SetUpFieldsList();
        }

        public void Initialize(Panel _gameBoardFieldsPanel, Panel _gameBoardNumberPanel)
        {
            gameBoardFieldsPanel = _gameBoardFieldsPanel;
            gameBoardNumberPanel = _gameBoardNumberPanel;

            SetUpBoardNumbersAndLetters();
            PlaceBoardFields();
        }

        private void SetUpFieldsList()
        {
            Fields = new BoardField[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Fields[i, j] = new BoardField();
                }
            }
        }

        private void SetUpBoardNumbersAndLetters()
        {
            boardNumbersLabels = new Label[8];
            boardLettersLabels = new Label[8];

            Font font = new Font("Microsoft Sans Serif", 19.8f);
            Color color = Color.White;

            for (int i = 0; i < 8; i ++)
            {
                boardNumbersLabels[i] = new Label();
                boardLettersLabels[i] = new Label();

                gameBoardNumberPanel.Controls.Add(boardNumbersLabels[i]);
                gameBoardNumberPanel.Controls.Add(boardLettersLabels[i]);

                boardNumbersLabels[i].Font = font;
                boardLettersLabels[i].Font = font;

                boardNumbersLabels[i].ForeColor = color;
                boardLettersLabels[i].ForeColor = color;

                boardNumbersLabels[i].AutoSize = true;
                boardLettersLabels[i].AutoSize = true;

                boardNumbersLabels[i].Text = (i + 1).ToString();
                boardLettersLabels[i].Text = ((char)(i + 65)).ToString();
            }

            PlaceNumbersAndLetters();
        }

        private void PlaceNumbersAndLetters()
        {
            if (gameBoardFieldsPanel != null)
            {
                int numbersStartingPosition = gameBoardFieldsPanel.Location.Y + BoardField.PanelSize.Height / 4;
                int lettersStartingPosition = gameBoardFieldsPanel.Location.X + BoardField.PanelSize.Width / 4;

                for (int i = 0; i < 8; i++)
                {
                    int numbersPositionIndex = OrientedByWhite ? 7 - i : i;
                    int lettersPositionIndex = OrientedByWhite ? i : 7 - i;

                    boardNumbersLabels[i].Location = new Point(gameBoardFieldsPanel.Location.X / 5,
                                                               numbersStartingPosition + numbersPositionIndex * BoardField.PanelSize.Width);
                    boardLettersLabels[i].Location = new Point(lettersStartingPosition + lettersPositionIndex * BoardField.PanelSize.Width,
                                                               gameBoardFieldsPanel.Location.Y + gameBoardFieldsPanel.Size.Height);
                }
            }
        }

        public void ReceiveFieldsAsPanels(Panel[,] panels)
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Fields[y, x].ReceivePanel(panels[y, x], new BoardPosition(x, y));
                }
            }
        }

        public void ChessBoardFieldClicked(Panel fieldPanel)
        {
            string[] coordsPartSplit = fieldPanel.Name.Split(':')[1].Split(',');
            bool humanPlayerIsWhite = ProgramLogic.Instance.Match.GetPlayerByColor(true) is HumanPlayer;

            int y = int.Parse(coordsPartSplit[0]);
            int x = int.Parse(coordsPartSplit[1]);

            BoardField newSelectedField = Fields[y, x];
            BoardField oldSelectedField = selectedField;

            if (selectedField == null)
            {
                selectedField = newSelectedField;
                selectedField.Select(true);

                if (ShowsPossibleMoves)
                    MarkPossibleMovesFields();
            }
            else
            {
                selectedField.Select(false);
                selectedField = null;

                if (oldSelectedField.Figure == null)
                {
                    selectedField = newSelectedField;
                    selectedField.Select(true);

                    if (ShowsPossibleMoves)
                        MarkPossibleMovesFields();
                }
                else
                {
                    if (ProgramLogic.Instance.Match.IsHumanPlayerFigure(oldSelectedField.Figure))
                    {
                        if (ProgramLogic.Instance.Match.IsHumanPlayerFigure(newSelectedField.Figure))
                        {
                            oldSelectedField.Select(false);
                            newSelectedField.Select(true);
                            selectedField = newSelectedField;

                            if (ShowsPossibleMoves)
                                MarkPossibleMovesFields();
                        }
                        else
                        {
                            Figure figureAtOldSelectedField = oldSelectedField?.Figure;

                            FigureType promotedFigureType = FigureType.Pawn;

                            if (figureAtOldSelectedField.FigureType == FigureType.Pawn &&
                                (newSelectedField.BoardPosition.Y == 0 || newSelectedField.BoardPosition.Y == 7))
                            {
                                PromoteDialog promoteDialog = new PromoteDialog(figureAtOldSelectedField.IsWhite);
                                promoteDialog.StartPosition = FormStartPosition.CenterParent;

                                if (promoteDialog.ShowDialog() == DialogResult.OK)
                                {
                                    promotedFigureType = promoteDialog.ChosenFigureType;
                                }
                                else
                                    return;
                            }

                            if (ProgramLogic.Instance.Match.WhiteIsMoving && humanPlayerIsWhite)
                            {
                                TryMakeMove(oldSelectedField.BoardPosition, newSelectedField.BoardPosition,
                                        promotedFigureType, oldSelectedField);
                            }
                            
                            UnmarkAllFields();
                        }
                    }
                }
            }
        }

        public bool TryMakeMove(BoardPosition fromPosition, BoardPosition toPosition, FigureType figureTypeToPromote,
                                BoardField oldSelectedField)
        {
            bool humanPlayerIsWhite = ProgramLogic.Instance.Match.GetPlayerByColor(true) is HumanPlayer;
            Figure figureAtFromPosition = BoardRepresentation.FigureAtPosition(fromPosition);
            Figure figureAtToPosition = BoardRepresentation.FigureAtPosition(toPosition);

            if (!BoardRepresentation.TryMakeMove(fromPosition, toPosition, out Figure _figureTakenByEnpassant, figureTypeToPromote,
                                                 out FigureType takenFigureType, out bool _wasCheck, out bool _wasCheckMate))
            {
                return false;
            }

            if (humanPlayerIsWhite == figureAtFromPosition.IsWhite)
            {
                oldSelectedField?.Select(false);
                UnmarkAllFields();
            }

            ProgramLogic.MainForm.undoMoveButton.Enabled = humanPlayerIsWhite != figureAtFromPosition.IsWhite;
            ProgramLogic.MainForm.gameResignButton.Enabled = true;

            ProgramLogic.Instance.Match.MakeMove(figureAtFromPosition.IsWhite, playingSequence: false);
            PlaceFigures();

            bool figureTaken = figureAtToPosition != null || _figureTakenByEnpassant != null;

            MovesManager.Moved(figureAtFromPosition.FigureType, figureAtFromPosition.IsWhite, figureTaken, takenFigureType,
                               fromPosition, toPosition, figureTypeToPromote, _wasCheck, _wasCheckMate,
                               ProgramLogic.Instance.Match.MyTimer.TimeRemainingInMilliseconds(figureAtFromPosition.IsWhite));

            if (_wasCheckMate)
            {
                ProgramLogic.Instance.Match.PauseUnpause(true);

                bool winnerIsWhite = figureAtFromPosition.IsWhite;
                CheckMateDialog checkMateDialog = new CheckMateDialog(humanPlayerIsWhite == figureAtFromPosition.IsWhite,
                                                                      ProgramLogic.Instance.Match.GetPlayerByColor(winnerIsWhite).GetNickname());

                checkMateDialog.StartPosition = FormStartPosition.CenterParent;
                checkMateDialog.ShowDialog();

                ProgramLogic.Instance.QuitMatch();
                ProgramLogic.MainForm.LoadTabPage(TabPageEnum.MainTabPage);
            }

            return true;
        }

        public void ChangeFieldPanelSize(int newSize)
        {
            if (Fields != null)
            {
                foreach (BoardField field in Fields)
                {
                    field.ChangeSize(newSize);
                    field.SetPanelPosition(new Point(field.BoardPosition.X * newSize, field.BoardPosition.Y * newSize));
                }

                PlaceNumbersAndLetters();
                PlaceBoardFields();
            }
        }

        public void FlipBoard()
        {
            OrientedByWhite = !OrientedByWhite;

            PlaceNumbersAndLetters();
            PlaceBoardFields();
            FlipTimeLabelsContents();
            FlipNameLabelsContents();
        }

        public void PlaceFigures()
        {
            foreach (BoardField field in Fields)
            {
                field.RemoveFigure();
            }

            foreach (Figure whiteFigure in BoardRepresentation.WhiteAliveFigures)
            {
                Fields[whiteFigure.BoardPosition.Y, whiteFigure.BoardPosition.X].PlaceFigure(whiteFigure);
            }

            foreach (Figure blackFigure in BoardRepresentation.BlackAliveFigures)
            {
                Fields[blackFigure.BoardPosition.Y, blackFigure.BoardPosition.X].PlaceFigure(blackFigure);
            }
        }

        private void PlaceBoardFields()
        {
            foreach (BoardField field in Fields)
            {
                if (OrientedByWhite)
                {
                    field.SetPanelPosition(new Point(field.BoardPosition.X * BoardField.PanelSize.Width,
                                                     field.BoardPosition.Y * BoardField.PanelSize.Height));
                }
                else
                {
                    field.SetPanelPosition(new Point((7 - field.BoardPosition.X) * BoardField.PanelSize.Width,
                                                     (7 - field.BoardPosition.Y) * BoardField.PanelSize.Height));
                }
            }
        }

        public void ShowThreats(bool show)
        {
            ShowsThreats = show;

            if (ShowsThreats)
            {
                UnmarkAllFields();
                MarkSelectedField();
                MarkThreatenedFields();
            }
            else
            {
                UnmarkAllFields();

                if (ShowsPossibleMoves)
                    MarkPossibleMovesFields();

                MarkSelectedField();
            }
        }

        public void ShowPossibleMoves(bool show)
        {
            ShowsPossibleMoves = show;

            if (ShowsPossibleMoves)
            {
                if (selectedField != null)
                {
                    MarkPossibleMovesFields();
                    MarkSelectedField();
                }
            }
            else
            {
                UnmarkAllFields();

                if (ShowsThreats)
                    MarkThreatenedFields();

                MarkSelectedField();
            }
        }

        private void MarkPossibleMovesFields()
        {
            UnmarkAllFields();

            if (selectedField != null && selectedField.Figure != null)
            {
                foreach (BoardField field in Fields)
                {
                    if (ChessRules.IsPossibleMove(BoardRepresentation, selectedField.Figure.BoardPosition, field.BoardPosition, out _))
                        field.MarkAsPossibleMove();
                }
            }

            MarkSelectedField();
        }

        public void MarkThreatenedFields()
        {
            UnmarkAllFields();

            bool AIPlayerIsWhite = ProgramLogic.Instance.Match.GetPlayerByColor(true) is AIPlayer;
            bool HumanPlayerIsWhite = !AIPlayerIsWhite;

            List<Figure> AIPlayerFigures = AIPlayerIsWhite ? BoardRepresentation.WhiteAliveFigures : BoardRepresentation.BlackAliveFigures;
            List<Figure> HumanPlayerFigures = HumanPlayerIsWhite ? BoardRepresentation.WhiteAliveFigures : BoardRepresentation.BlackAliveFigures;

            foreach (Figure AIFigure in AIPlayerFigures)
            {
                foreach (Figure humanFigure in HumanPlayerFigures)
                {
                    if (ChessRules.IsPossibleMove(BoardRepresentation, AIFigure.BoardPosition, humanFigure.BoardPosition, out _))
                    {
                        Fields[humanFigure.BoardPosition.Y, humanFigure.BoardPosition.X].MarkAsThreatened();
                    }
                }
            }

            MarkSelectedField();
        }

        private void UnmarkAllFields()
        {
            foreach (BoardField field in Fields)
            {
                field.UnmarkColor();
            }
        }

        private void MarkSelectedField()
        {
            if (selectedField != null)
                selectedField.Select(true);
        }

        public void MatchStarted(bool humanPlayerIsWhite)
        {
            if ((humanPlayerIsWhite && !OrientedByWhite) || (!humanPlayerIsWhite && OrientedByWhite))
                FlipBoard();

            BoardRepresentation = new BoardRepresentation();

            PlaceFigures();
            UnmarkAllFields();

            selectedField = null;
        }

        public void FlipTimeLabelsContents()
        {
            string lowerPlayerTimeLabelText = ProgramLogic.MainForm.gameLowerPlayerTimeLabel.Text;

            ProgramLogic.MainForm.gameLowerPlayerTimeLabel.Text = ProgramLogic.MainForm.gameUpperPlayerTimeLabel.Text;
            ProgramLogic.MainForm.gameUpperPlayerTimeLabel.Text = lowerPlayerTimeLabelText;
        }

        public void FlipNameLabelsContents()
        {
            string lowerPlayerNameLabelText = ProgramLogic.MainForm.gameLowerPlayerNameLabel.Text;

            ProgramLogic.MainForm.gameLowerPlayerNameLabel.Text = ProgramLogic.MainForm.gameUpperPlayerNameLabel.Text;
            ProgramLogic.MainForm.gameUpperPlayerNameLabel.Text = lowerPlayerNameLabelText;
        }

        public void PlaySequenceOfMoves(List<Move> moves)
        {
            bool humanPlayerIsWhite = ProgramLogic.Instance.Match.GetPlayerByColor(true) is HumanPlayer;
            int i = 0;

            foreach (Move move in moves)
            {
                BoardRepresentation.MakeMove(move.fromPosition, move.toPosition, move.promotedFigureType);
                ProgramLogic.Instance.Match.MakeMove(i % 2 == 0, playingSequence: true);

                MovesManager.Moved(move.figureType, i % 2 == 0, move.figureWasTaken, move.takenFigureType,
                                   move.fromPosition, move.toPosition, move.promotedFigureType, move.wasCheck,
                                   move.wasCheckMate, move.millisecondsRemaining);

                i++;
            }

            PlaceFigures();
            ProgramLogic.MainForm.undoMoveButton.Enabled = moves.Count % 2 == 0 && humanPlayerIsWhite;
        }

        public void UndoLastTwoMoves(List<Move> moves)
        {
            UnmarkAllFields();
            BoardRepresentation = new BoardRepresentation();
            PlaySequenceOfMoves(moves);

            ProgramLogic.Instance.Match.SetTimerByPlayedMoves(moves);
        }
    }
}

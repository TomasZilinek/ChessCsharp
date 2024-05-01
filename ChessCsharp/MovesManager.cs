using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChessPB069
{
    public struct Move
    {
        public FigureType figureType;
        public bool figureWasTaken;
        public FigureType takenFigureType;
        public BoardPosition fromPosition;
        public BoardPosition toPosition;
        public FigureType promotedFigureType;
        public bool wasCheck;
        public bool wasCheckMate;
        public int millisecondsRemaining;

        public Move(FigureType _figuretype, bool _figureTaken, FigureType _takenFigureType, BoardPosition _fromPosition,
                    BoardPosition _toPosition, FigureType _promotedFigureType, bool _wasCheck, bool _wasCheckMate,
                    int _millisecondsRemaining)
        {
            figureType = _figuretype;
            figureWasTaken = _figureTaken;
            takenFigureType = _takenFigureType;
            fromPosition = _fromPosition;
            toPosition = _toPosition;
            promotedFigureType = _promotedFigureType;
            wasCheck = _wasCheck;
            wasCheckMate = _wasCheckMate;
            millisecondsRemaining = _millisecondsRemaining;
        }

        public override string ToString()
        {
            string result = MovesManager.FigureTypeToNotation(figureType);

            if (figureType == FigureType.Pawn && figureWasTaken)
                result += fromPosition.ToChessNotation().ElementAt(0);

            if (figureWasTaken)
                result += 'x';

            result += toPosition.ToChessNotation();

            if (wasCheckMate)
                result += '#';
            else if (wasCheck)
                result += '+';

            return result;
        }
    }


    public static class MovesManager
    {
        public static List<Move> Moves = new List<Move>();

        public static void Moved(FigureType figureType, bool figureIsWhite, bool figureWasTaken, FigureType takenFigureType,
                                 BoardPosition fromPosition, BoardPosition toPosition, FigureType promotedFigureType,
                                 bool wasCheck, bool wasCheckMate, int millisecondsRemaining)
        {
            Move move = new Move(figureType, figureWasTaken, takenFigureType, fromPosition, toPosition, promotedFigureType,
                                 wasCheck, wasCheckMate, millisecondsRemaining);
            Moves.Add(move);

            string moveStringRepresentation = move.ToString();

            if (figureIsWhite)
            {
                string numberAsString = ProgramLogic.MainForm.movesDataGridView.Rows.Count.ToString() + ".  ";
                ProgramLogic.MainForm.movesDataGridView.Rows.Add(new string[] { numberAsString, moveStringRepresentation, "" });
            }
            else
            {
                ProgramLogic.MainForm.movesDataGridView.Rows[ProgramLogic.MainForm.movesDataGridView.Rows.Count - 1]
                            .Cells["blackMoveColumn"].Value = moveStringRepresentation;
            }
        }

        public static List<Move> UndoNumberOfLastMoves(int n)
        {
            ProgramLogic.MainForm.movesDataGridView.Rows.Clear();

            for (int i = 0; i < n; i++)
                Moves.RemoveAt(Moves.Count - 1);

            return Moves;
        }

        public static string FigureTypeToNotation(FigureType figureType)
        {
            if (figureType == FigureType.Pawn)
                return "";
            if (figureType == FigureType.Knight)
                return "N";
            else
                return figureType.ToString().ElementAt(0).ToString();
        }

        public static void ClearMoves()
        {
            Moves.Clear();
            ProgramLogic.MainForm.movesDataGridView.Rows.Clear();
        }

        public static void SaveGame(string filePath)
        {
            Match match = ProgramLogic.Instance.Match;
            bool humanPlayerIsWhite = match.GetPlayerByHumanity(true).IsWhite;

            XmlDocument document = new XmlDocument();
            XmlElement root = document.CreateElement("ChessGame");
            document.AppendChild(root);

            // time limit
            XmlElement timeLimitElement = document.CreateElement("TimeLimitMinutes");
            timeLimitElement.InnerText = match.TimeLimitInMinutes.ToString();
            root.AppendChild(timeLimitElement);

            // human player
            XmlElement humanPlayerElement = document.CreateElement("HumanPlayer");
            humanPlayerElement.SetAttribute("nickname", (match.GetPlayerByHumanity(true) as HumanPlayer).GetNickname());
            humanPlayerElement.SetAttribute("color", humanPlayerIsWhite ? "white" : "black");
            root.AppendChild(humanPlayerElement);

            // AI player
            XmlElement AIPlayerElement = document.CreateElement("AIPlayer");
            AIPlayerElement.SetAttribute("difficulty", (match.GetPlayerByHumanity(false) as AIPlayer).Difficulty.ToString());
            AIPlayerElement.SetAttribute("color", humanPlayerIsWhite ? "black" : "white");
            root.AppendChild(AIPlayerElement);

            // moves
            foreach (Move move in Moves)
            {
                XmlElement moveElement = MoveToXmlElement(document, move);
                root.AppendChild(moveElement);
            }

            document.Save(filePath);
        }

        private static XmlElement MoveToXmlElement(XmlDocument document, Move move)
        {
            XmlElement moveElement = document.CreateElement("Move");

            XmlElement figureTypeElement = document.CreateElement("FigureType");
            figureTypeElement.InnerText = move.figureType.ToString();
            moveElement.AppendChild(figureTypeElement);

            XmlElement figureTakenElement = document.CreateElement("FigureWasTaken");
            figureTakenElement.InnerText = move.figureWasTaken.ToString();
            moveElement.AppendChild(figureTakenElement);

            XmlElement takenFigureTypeElement = document.CreateElement("TakenFigureType");
            takenFigureTypeElement.InnerText = move.figureType.ToString();
            moveElement.AppendChild(takenFigureTypeElement);

            XmlElement fromPositionElement = BoardPositionToXmlElement(document, "FromPosition", move.fromPosition);
            moveElement.AppendChild(fromPositionElement);

            XmlElement toPositionElement = BoardPositionToXmlElement(document, "ToPosition", move.toPosition);
            moveElement.AppendChild(toPositionElement);

            XmlElement promotedFigureTypeElement = document.CreateElement("PromotedFigureType");
            promotedFigureTypeElement.InnerText = move.promotedFigureType.ToString();
            moveElement.AppendChild(promotedFigureTypeElement);

            XmlElement wasCheckElement = document.CreateElement("WasCheck");
            wasCheckElement.InnerText = move.wasCheck.ToString();
            moveElement.AppendChild(wasCheckElement);

            XmlElement wasCheckMateElement = document.CreateElement("WasCheckMate");
            wasCheckMateElement.InnerText = move.wasCheckMate.ToString();
            moveElement.AppendChild(wasCheckMateElement);

            XmlElement millisecondsRemainingElement = document.CreateElement("MillisecondsRemaining");
            millisecondsRemainingElement.InnerText = move.millisecondsRemaining.ToString();
            moveElement.AppendChild(millisecondsRemainingElement);

            return moveElement;
        }

        private static XmlElement BoardPositionToXmlElement(XmlDocument document, string name, BoardPosition boardPosition)
        {
            XmlElement positionElement = document.CreateElement(name);

            XmlElement xElement = document.CreateElement("X");
            xElement.InnerText = boardPosition.X.ToString();
            positionElement.AppendChild(xElement);

            XmlElement yElement = document.CreateElement("Y");
            yElement.InnerText = boardPosition.Y.ToString();
            positionElement.AppendChild(yElement);

            return positionElement;
        }

        public static bool TryLoadGame(string filePath, out HumanPlayer humanPlayer, out AIPlayer AIPlayer, out List<Move> loadedMoves,
                                       out int timeLimitInMinutes)
        {
            XmlDocument doc = new XmlDocument();

            string fileContent = System.IO.File.ReadAllText(filePath);

            try
            {
                doc.LoadXml(fileContent);
            }
            catch
            {
                humanPlayer = null;
                AIPlayer = null;
                loadedMoves = null;
                timeLimitInMinutes = 0;

                return false;
            }

            humanPlayer = null;
            AIPlayer = null;
            loadedMoves = new List<Move>();
            timeLimitInMinutes = 0;

            foreach (XmlElement nodeElement in doc.GetElementsByTagName("ChessGame")[0].ChildNodes)
            {
                if (nodeElement.Name == "TimeLimitMinutes")
                {
                    timeLimitInMinutes = int.Parse(nodeElement.InnerText, CultureInfo.InvariantCulture); ;
                }
                else if (nodeElement.Name == "HumanPlayer")
                {
                    humanPlayer = new HumanPlayer(nodeElement.GetAttribute("color") == "white", nodeElement.GetAttribute("nickname"));
                }
                else if (nodeElement.Name == "AIPlayer")
                {
                    bool isWhite = nodeElement.GetAttribute("color") == "white";
                    AIdifficulty difficulty = (AIdifficulty)Enum.Parse(typeof(AIdifficulty), nodeElement.GetAttribute("difficulty"));

                    AIPlayer = new AIPlayer(isWhite, difficulty);
                }
                else if (nodeElement.Name == "Move")
                {
                    Move move = XmlElementToMove(nodeElement);
                    loadedMoves.Add(move);
                }
            }

            return true;
        }

        private static Move XmlElementToMove(XmlElement moveElement)
        {
            FigureType figureType = (FigureType)Enum.Parse(typeof(FigureType), moveElement.ChildNodes[0].InnerText);
            bool figureWasTaken = bool.Parse(moveElement.ChildNodes[1].InnerText);
            FigureType takenFigureType = (FigureType)Enum.Parse(typeof(FigureType), moveElement.ChildNodes[2].InnerText);
            BoardPosition fromPosition = XmlElementToBoardPosition(moveElement.ChildNodes[3] as XmlElement);
            BoardPosition toPosition = XmlElementToBoardPosition(moveElement.ChildNodes[4] as XmlElement);
            FigureType promotedFigureType = (FigureType)Enum.Parse(typeof(FigureType), moveElement.ChildNodes[5].InnerText);
            bool wasCheck = bool.Parse(moveElement.ChildNodes[6].InnerText);
            bool wasCheckMate = bool.Parse(moveElement.ChildNodes[7].InnerText);
            int millisecondsRemaining = int.Parse(moveElement.ChildNodes[8].InnerText, CultureInfo.InvariantCulture);

            return new Move(figureType, figureWasTaken, takenFigureType, fromPosition, toPosition, promotedFigureType,
                            wasCheck, wasCheckMate, millisecondsRemaining);
        }

        private static BoardPosition XmlElementToBoardPosition(XmlElement boardPositionElement)
        {
            int x = int.Parse(boardPositionElement.GetElementsByTagName("X")[0].InnerText, CultureInfo.InvariantCulture);
            int y = int.Parse(boardPositionElement.GetElementsByTagName("Y")[0].InnerText, CultureInfo.InvariantCulture);

            return new BoardPosition(x, y);
        }
    }
}

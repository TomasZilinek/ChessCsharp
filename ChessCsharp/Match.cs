using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessPB069
{
    public class Match
    {
        public bool Started { get; set; }
        public bool WhiteIsMoving { get; private set; }
        public bool gameSaved { get; private set; }
        public bool gamePaused { get; private set; }
        public Player WhitePlayer { get; private set; }
        public Player BlackPlayer { get; private set; }
        public MyTimer MyTimer { get; private set; }
        public int TimeLimitInMinutes { get; private set; }

        public Match(bool humanPlayerIsWhite, AIdifficulty difficulty, int timeInMinutes, string humanPlayerNickName)
        {
            if (humanPlayerIsWhite)
            {
                WhitePlayer = new HumanPlayer(humanPlayerIsWhite, humanPlayerNickName);
                BlackPlayer = new AIPlayer(!humanPlayerIsWhite, difficulty);
            }
            else
            {
                WhitePlayer = new AIPlayer(!humanPlayerIsWhite, difficulty);
                BlackPlayer = new HumanPlayer(humanPlayerIsWhite, humanPlayerNickName);
            }

            Started = false;
            WhiteIsMoving = true;
            gameSaved = false;
            gamePaused = true;
            TimeLimitInMinutes = timeInMinutes;

            MyTimer = new MyTimer(timeInMinutes);
        }

        /// <summary>
        /// Match constructor for loading match from file
        /// </summary>
        public Match(HumanPlayer humanPlayer, AIPlayer AIPlayer, int timeInMinutes)
        {
            if (humanPlayer.IsWhite)
            {
                WhitePlayer = humanPlayer;
                BlackPlayer = AIPlayer;
            }
            else
            {
                WhitePlayer = AIPlayer;
                BlackPlayer = humanPlayer;
            }

            Started = false;
            WhiteIsMoving = true;
            gameSaved = false;
            gamePaused = true;
            TimeLimitInMinutes = timeInMinutes;

            MyTimer = new MyTimer();
        }

        public Player GetPlayerByColor(bool isWhitePlayer)
        {
            if (isWhitePlayer)
                return WhitePlayer;
            else
                return BlackPlayer;
        }

        public Player GetPlayerByHumanity(bool isHumanPlayer)
        {
            if (isHumanPlayer)
                return WhitePlayer is HumanPlayer ? WhitePlayer : BlackPlayer;
            else
                return WhitePlayer is HumanPlayer ? BlackPlayer : WhitePlayer;
        }

        public bool IsHumanPlayerFigure(Figure figure)
        {
            return figure != null && GetPlayerByHumanity(true).IsWhite == figure.IsWhite;
        }

        public void MakeMove(bool isWhite, bool playingSequence)
        {
            WhiteIsMoving = !isWhite;
            gameSaved = false;
            Started = true;

            MyTimer.PlayerMoved(isWhite);

            if (ProgramLogic.Instance.Board.ShowsThreats)
                ProgramLogic.Instance.Board.MarkThreatenedFields();

            if (!playingSequence && (GetPlayerByHumanity(false).IsWhite != isWhite))
            {
                Console.WriteLine("moved");
                (GetPlayerByHumanity(false) as AIPlayer).MakeMove();
            }
        }

        public void PauseUnpause(bool paused)
        {
            gamePaused = paused;
            MyTimer.PauseUnpause(paused);
        }

        public void SetTimerByPlayedMoves(List<Move> moves)
        {
            int whitePlayerMillisecondsRemaining;
            int blackPlayerMillisecondsRemaining;

            if (moves.Count == 1)
            {
                blackPlayerMillisecondsRemaining = TimeLimitInMinutes;
                whitePlayerMillisecondsRemaining = moves[0].millisecondsRemaining;
            }
            else if (moves.Count == 0)
            {
                MyTimer.ResetTimer();
                return;
            }
            else
            {
                whitePlayerMillisecondsRemaining = (moves.Count % 2 == 0 ? moves[moves.Count - 2] : moves[moves.Count - 1]).millisecondsRemaining;
                blackPlayerMillisecondsRemaining = (moves.Count % 2 == 0 ? moves[moves.Count - 1] : moves[moves.Count - 2]).millisecondsRemaining;
            }

            MyTimer.ManuallySetRemainingTime(whitePlayerMillisecondsRemaining, blackPlayerMillisecondsRemaining);
            WhiteIsMoving = moves.Count % 2 == 0;
        }

        public void SavedGame()
        {
            gameSaved = true;
        }
    }
}

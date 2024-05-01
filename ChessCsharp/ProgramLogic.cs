using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessPB069
{
    public class ProgramLogic
    {
        public static ProgramLogic Instance;
        public Board Board { get; private set; }
        public Match Match { get; private set; }

        public static MainForm MainForm { get; private set; }

        public static bool MOVEAITOO = true;

        public ProgramLogic()
        {
            Instance = this;
            Board = new Board();
        }

        public void Initialize(MainForm mainForm)
        {
            MainForm = mainForm;
            Board.Initialize(mainForm.gameBoardFieldsPanel, mainForm.gameBoardNumberPanel);
        }

        public void StartMatch(bool humanPlayerIsWhite, AIdifficulty difficulty, int timeInMinutes, string humanPlayerNickName)
        {
            QuitMatch();

            if (humanPlayerNickName.Length == 0 || string.IsNullOrWhiteSpace(humanPlayerNickName))
                humanPlayerNickName = "Humaan";

            Match = new Match(humanPlayerIsWhite, difficulty, timeInMinutes, humanPlayerNickName);
            Board.MatchStarted(humanPlayerIsWhite);

            MovesManager.ClearMoves();

            MainForm.gameLowerPlayerNameLabel.Text = humanPlayerNickName;
            MainForm.gameUpperPlayerNameLabel.Text = difficulty.ToString() + " AI";

            MainForm.gameLowerPlayerTimeLabel.Text = timeInMinutes.ToString() + ":00";
            MainForm.gameUpperPlayerTimeLabel.Text = timeInMinutes.ToString() + ":00";

            MainForm.gameResignButton.Enabled = false;
            MainForm.undoMoveButton.Enabled = false;

            if (Match.GetPlayerByColor(true) is AIPlayer)
                (Match.GetPlayerByHumanity(false) as AIPlayer).MakeMove();
        }

        /// <summary>
        /// method overload for starting match loaded from file
        /// </summary>
        public void StartMatch(HumanPlayer humanPlayer, AIPlayer AIPlayer, int timeInMinutes, List<Move> moves)
        {
            QuitMatch();

            Match = new Match(humanPlayer, AIPlayer, timeInMinutes);

            Board.MatchStarted(humanPlayer.IsWhite);
            Board.PlaySequenceOfMoves(moves);
            Match.SetTimerByPlayedMoves(moves);

            MainForm.gameLowerPlayerNameLabel.Text = humanPlayer.GetNickname();
            MainForm.gameUpperPlayerNameLabel.Text = AIPlayer.GetNickname();

            Match.PauseUnpause(false);
        }

        public void QuitMatch()
        {
            Match = null;
        }

        public void GlobalTimerTick(int millisecondsPassed)
        {
            if (Match != null)
            {
                Match.MyTimer.GlobalTimerTick(millisecondsPassed);
            }
        }

        public bool MatchInProgress()
        {
            return Match != null && Match.Started;
        }
    }
}

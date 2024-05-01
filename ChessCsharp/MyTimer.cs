using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessPB069
{
    public class MyTimer
    {
        public int MaxTimeInMinutes { get; private set; }
        public int WhiteTimeReminingInMilliseconds { get; private set; }
        public int BlackTimeReminingInMilliseconds { get; private set; }
        public bool Enabled { get; set; }
        public bool Paused { get; private set; }

        private bool PlayerToMoveIsWhite;

        public MyTimer(int timeInMinutes = 10)
        {
            PlayerToMoveIsWhite = false;
            MaxTimeInMinutes = timeInMinutes;

            ResetTimer(timeInMinutes);

            Enabled = false;
        }

        public void ResetTimer(int timeInMinutes = -1)
        {
            if (timeInMinutes == -1)
                timeInMinutes = MaxTimeInMinutes;
            
            WhiteTimeReminingInMilliseconds = timeInMinutes * 60 * 1000;
            BlackTimeReminingInMilliseconds = timeInMinutes * 60 * 1000;

            PlayerToMoveIsWhite = true;
        }

        public void GlobalTimerTick(int millisecondsPassed)
        {
            if (Enabled && !Paused)
            {
                if (PlayerToMoveIsWhite)
                {
                    WhiteTimeReminingInMilliseconds -= millisecondsPassed;
                }
                else
                {
                    BlackTimeReminingInMilliseconds -= millisecondsPassed;
                }

                UpdatePlayersTimeLabels();
            }
        }

        public void ManuallySetRemainingTime(int whitePlayerMillisecondsremaining, int blackPlayerMillisecondsRamaining)
        {
            WhiteTimeReminingInMilliseconds = whitePlayerMillisecondsremaining;
            BlackTimeReminingInMilliseconds = blackPlayerMillisecondsRamaining;

            UpdatePlayersTimeLabels();
        }

        private void UpdatePlayersTimeLabels()
        {
            GetPlayerTimerLabel(true).Text = TimeInMillisecondsToClockString(WhiteTimeReminingInMilliseconds);
            GetPlayerTimerLabel(false).Text = TimeInMillisecondsToClockString(BlackTimeReminingInMilliseconds);
        }

        public void PlayerMoved(bool isWhite)
        {
            if (!Enabled)
                Enabled = true;

            if (!Paused)
                PlayerToMoveIsWhite = !isWhite;
        }


        private Label GetPlayerTimerLabel(bool isWhite)
        {
            Label lowerPlayerTimerLabel = ProgramLogic.MainForm.gameLowerPlayerTimeLabel;
            Label upperPlayerTimerLabel = ProgramLogic.MainForm.gameUpperPlayerTimeLabel;

            if (ProgramLogic.Instance.Board.OrientedByWhite)
            {
                return isWhite ? lowerPlayerTimerLabel : upperPlayerTimerLabel;
            }
            else
            {
                return isWhite ? upperPlayerTimerLabel : lowerPlayerTimerLabel;
            }
        }

        private string TimeInMillisecondsToClockString(int timeInMilliseconds)
        {
            int timeInSeconds = timeInMilliseconds / 1000;
            int remainingMilliseconds = timeInMilliseconds % 1000;

            int resultMinutes = timeInSeconds / 60;
            int resultSeconds = timeInSeconds % 60;
            int resultSecondsFraction = remainingMilliseconds / 100;

            string result = "";

            if (resultMinutes != 0)
                result += resultMinutes.ToString() + ":";

            result += (resultSeconds < 10 ? "0" : "") + resultSeconds;

            if (resultMinutes == 0)
                result += "." + resultSecondsFraction.ToString();

            return result;
        }

        public int TimeRemainingInMilliseconds(bool isWhite)
        {
            return isWhite ? WhiteTimeReminingInMilliseconds : BlackTimeReminingInMilliseconds;
        }

        public void PauseUnpause(bool paused)
        {
            Paused = paused;
        }
    }
}

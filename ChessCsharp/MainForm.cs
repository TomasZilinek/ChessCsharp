using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessPB069
{
    public enum TabPageEnum
    {
        MainTabPage, CreditsTabPage, GameSettingsTabPage, GameTabPage, SaveGameTabPage, SaveDecisionTabPage, CloseAppTabPage
    }

    public partial class MainForm : Form
    {
        public TabPageEnum CurrentTabPage { get; private set; }
        public TabPageEnum SaveGameSuccessfulSaveDestination { get; set; }
        public TabPageEnum SaveGameBackButtonDestination { get; set; }
        public TabPageEnum SaveDecisionAbandonGameButtonDestination { get; set; }

        public MainForm()
        {
            InitializeComponent();

            tabControl1.Appearance = TabAppearance.Buttons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;

            CurrentTabPage = TabPageEnum.MainTabPage;

            Button[] buttons = new Button[] { newGameButton, creditsButton, exitGameButton, creditsBackButton, settingsBackButton,
                                              settingsStartGameButton };

            foreach (Button button in buttons)
            {
                button.BackColor = Color.FromArgb(100, 255, 255, 255);
            }

            timeComboBox.SelectedIndex = 0;
            settingsDifficultyComboBox.SelectedIndex = 0;

            // gameBoardPanel.Location = new Point((Size.Width - gameRightPanel.Width) / 2 - gameBoardPanel.Size.Width / 2, 50);

            gameUpperPlayerNameLabel.Location = new Point(gameBoardNumberPanel.Location.X, gameUpperPlayerNameLabel.Location.Y);
            gameLowerPlayerNameLabel.Location = new Point(gameBoardNumberPanel.Location.X, gameLowerPlayerNameLabel.Location.Y);

            CreateFieldsPanels();
        }

        public void LoadTabPage(TabPageEnum newTabPage)
        {
            CurrentTabPage = newTabPage;

            tabControl1.SelectTab((int)newTabPage);
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            LoadTabPage(TabPageEnum.GameSettingsTabPage);
        }

        private void creditsButton_Click(object sender, EventArgs e)
        {
            LoadTabPage(TabPageEnum.CreditsTabPage);
        }

        private void exitGameButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void creditsBackButton_Click(object sender, EventArgs e)
        {
            LoadTabPage(TabPageEnum.MainTabPage);
        }

        private void gameSettingsBackButton_Click(object sender, EventArgs e)
        {
            LoadTabPage(TabPageEnum.MainTabPage);
        }

        private void settingsStartGameButton_Click(object sender, EventArgs e)
        {
            ProgramLogic.Instance.StartMatch(settingsRadioButtonWhite.Checked,
                                             (AIdifficulty)Enum.Parse(typeof(AIdifficulty), settingsDifficultyComboBox.Text),
                                             int.Parse(timeComboBox.Text.Split(' ')[0]),
                                             settingsNicknameTextbox.Text);

            globalTimer.Enabled = true;
            LoadTabPage(TabPageEnum.GameTabPage);
        }

        private void gameBoardPanel_SizeChanged(object sender, EventArgs e)
        {
            if (ProgramLogic.Instance != null)
            {
                gameBoardNumberPanel.Size = new Size(gameBoardNumberPanel.Size.Height, gameBoardNumberPanel.Size.Height);

                gameBoardNumberPanel.Location = new Point((gameMainPanel.Width - gameBoardNumberPanel.Width) / 2,
                                                    gameBoardNumberPanel.Location.Y);

                gameUpperPlayerNameLabel.Location = new Point(gameBoardNumberPanel.Location.X, gameUpperPlayerNameLabel.Location.Y);
                gameLowerPlayerNameLabel.Location = new Point(gameBoardNumberPanel.Location.X, gameLowerPlayerNameLabel.Location.Y);

                ProgramLogic.Instance.Board.ChangeFieldPanelSize(gameBoardFieldsPanel.Size.Width / 8);

                gameBoardFieldsPanel.Location = new Point((int)(gameBoardNumberPanel.Size.Width * 0.05f) + 1,
                                                          (int)(gameBoardNumberPanel.Size.Height * 0.05f) + 1);

                gameBoardFieldsPanel.Size = new Size((int)(gameBoardNumberPanel.Size.Width * 0.9f) - 1,
                                                     (int)(gameBoardNumberPanel.Size.Height * 0.9f) - 1);

                ProgramLogic.Instance.Board.ChangeFieldPanelSize(gameBoardFieldsPanel.Size.Width / 8);
            }
        }

        private void CreateFieldsPanels()
        {
            int tileSize = gameBoardFieldsPanel.Size.Width / 8;
            const int gridSize = 8;

            Color colorWhite = Color.FromArgb(245, 245, 220);
            Color colorBlack = Color.FromArgb(153, 115, 77);

            Panel[,] _chessBoardPanels = new Panel[gridSize, gridSize];

            for (var y = 0; y < gridSize; y++)
            {
                for (var x = 0; x < gridSize; x++)
                {
                    var newPanel = new Panel
                    {
                        Size = new Size(tileSize, tileSize)
                    };

                    if (y % 2 == 0)
                        newPanel.BackColor = x % 2 != 0 ? colorBlack : colorWhite;
                    else
                        newPanel.BackColor = x % 2 != 0 ? colorWhite : colorBlack;

                    newPanel.Click += ChessBoardPanel_Click;
                    newPanel.Name = "fieldPanel:" + y + ',' + x;
                    newPanel.BackgroundImageLayout = ImageLayout.Zoom;

                    gameBoardFieldsPanel.Controls.Add(newPanel);

                    _chessBoardPanels[y, x] = newPanel;
                }
            }

            ProgramLogic.Instance.Board.ReceiveFieldsAsPanels(_chessBoardPanels);
        }

        private void ChessBoardPanel_Click(object sender, EventArgs e)
        {
            ProgramLogic.Instance.Board.ChessBoardFieldClicked(sender as Panel);
        }

        private void gameFlipBoardButton_Click(object sender, EventArgs e)
        {
            ProgramLogic.Instance.Board.FlipBoard();
        }

        private void globalTimer_Tick(object sender, EventArgs e)
        {
            ProgramLogic.Instance.GlobalTimerTick(globalTimer.Interval);
        }

        private void saveGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgramLogic.Instance.Match.PauseUnpause(true);
            LoadTabPage(TabPageEnum.SaveGameTabPage);
            SaveGameBackButtonDestination = TabPageEnum.GameTabPage;
            SaveGameSuccessfulSaveDestination = TabPageEnum.GameTabPage;
        }

        private void loadGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProgramLogic.Instance.MatchInProgress() && !ProgramLogic.Instance.Match.gameSaved)
            {
                SaveGameBackButtonDestination = CurrentTabPage;
                SaveGameSuccessfulSaveDestination = CurrentTabPage;
                SaveDecisionAbandonGameButtonDestination = CurrentTabPage;

                if (CurrentTabPage == TabPageEnum.GameTabPage)
                    ProgramLogic.Instance.Match.PauseUnpause(true);

                LoadTabPage(TabPageEnum.SaveDecisionTabPage);
            }
            else
                LoadGame();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();  // closing event catches this
        }

        private void flipBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgramLogic.Instance.Board.FlipBoard();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            goToMainMenuToolStripMenuItem.Enabled = CurrentTabPage != TabPageEnum.MainTabPage &&
                                                    CurrentTabPage != TabPageEnum.SaveGameTabPage &&
                                                    CurrentTabPage != TabPageEnum.SaveDecisionTabPage &&
                                                    CurrentTabPage != TabPageEnum.CloseAppTabPage;

            saveGameToolStripMenuItem.Enabled = CurrentTabPage == TabPageEnum.GameTabPage && ProgramLogic.Instance.MatchInProgress();

            loadGameToolStripMenuItem.Enabled = CurrentTabPage != TabPageEnum.SaveGameTabPage &&
                                                CurrentTabPage != TabPageEnum.SaveDecisionTabPage &&
                                                CurrentTabPage != TabPageEnum.CloseAppTabPage;
        }

        private void saveGameBackButton_Click(object sender, EventArgs e)
        {
            LoadTabPage(SaveGameBackButtonDestination);

            if (SaveGameBackButtonDestination == TabPageEnum.GameTabPage)
                ProgramLogic.Instance.Match.PauseUnpause(false);
        }

        private void goToMainMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTabPage == TabPageEnum.GameTabPage)
                gameToMainMenuButton_Click(sender, e);
            else
                LoadTabPage(TabPageEnum.MainTabPage);
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flipBoardToolStripMenuItem.Enabled = CurrentTabPage == TabPageEnum.GameTabPage;
        }

        private void saveGameChooseLocationButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML-File | *.xml";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ProgramLogic.Instance.Match.SavedGame();
                MovesManager.SaveGame(saveFileDialog.FileName);
                saveGameNotSavedLabel.Visible = false;

                if (SaveGameSuccessfulSaveDestination == TabPageEnum.GameTabPage)
                    ProgramLogic.Instance.Match.PauseUnpause(false);

                LoadTabPage(SaveGameSuccessfulSaveDestination);
            }
            else
                saveGameNotSavedLabel.Visible = true;
        }

        public void LoadGame()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML-File | *.xml";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (MovesManager.TryLoadGame(openFileDialog.FileName, out HumanPlayer humanPlayer, out AIPlayer AIPlayer,
                                             out List<Move> moves, out int timeInMinutes))
                {
                    ProgramLogic.Instance.StartMatch(humanPlayer, AIPlayer, timeInMinutes, moves);

                    globalTimer.Enabled = true;
                    LoadTabPage(TabPageEnum.GameTabPage);
                }
            }
            else
                saveGameNotSavedLabel.Visible = true;
        }

        private void gameToMainMenuButton_Click(object sender, EventArgs e)
        {
            if (ProgramLogic.Instance.MatchInProgress())
            {
                LoadTabPage(TabPageEnum.SaveDecisionTabPage);
                SaveGameSuccessfulSaveDestination = TabPageEnum.MainTabPage;
                SaveGameBackButtonDestination = TabPageEnum.GameTabPage;
                SaveDecisionAbandonGameButtonDestination = TabPageEnum.MainTabPage;
            }
            else
            {
                LoadTabPage(TabPageEnum.MainTabPage);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CurrentTabPage != TabPageEnum.CloseAppTabPage && ProgramLogic.Instance.MatchInProgress())
            {
                e.Cancel = true;

                ProgramLogic.Instance.Match.PauseUnpause(true);
                LoadTabPage(TabPageEnum.SaveDecisionTabPage);
                SaveDecisionAbandonGameButtonDestination = TabPageEnum.CloseAppTabPage;
                SaveGameSuccessfulSaveDestination = TabPageEnum.CloseAppTabPage;
                SaveGameBackButtonDestination = TabPageEnum.GameTabPage;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // used to confirm application exit
            if (tabControl1.SelectedIndex == (int)TabPageEnum.CloseAppTabPage)
                Application.Exit();
        }

        private void saveDecisionSaveGameButton_Click(object sender, EventArgs e)
        {
            LoadTabPage(TabPageEnum.SaveGameTabPage);
        }

        private void saveDecisionAbandonGameButton_Click(object sender, EventArgs e)
        {
            LoadTabPage(SaveDecisionAbandonGameButtonDestination);

            ProgramLogic.Instance.QuitMatch();
            MovesManager.ClearMoves();
        }

        private void closeAppExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingLoadGameButton_Click(object sender, EventArgs e)
        {
            LoadGame();
        }

        private void gameShowPossibleMovesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ProgramLogic.Instance.Board.ShowPossibleMoves(gameShowPossibleMovesCheckBox.Checked);

            if (gameShowPossibleMovesCheckBox.Checked && gameShowThreatsCheckBox.Checked)
                gameShowThreatsCheckBox.Checked = false;
        }

        private void gameShowThreatsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ProgramLogic.Instance.Board.ShowThreats(gameShowThreatsCheckBox.Checked);

            if (gameShowThreatsCheckBox.Checked && gameShowPossibleMovesCheckBox.Checked)
                gameShowPossibleMovesCheckBox.Checked = false;
        }

        private void resultGoToMainMenuButton_Click(object sender, EventArgs e)
        {
            ProgramLogic.Instance.QuitMatch();

            LoadTabPage(TabPageEnum.MainTabPage);
        }

        private void undoMoveButton_Click(object sender, EventArgs e)
        {
            List<Move> moves = MovesManager.UndoNumberOfLastMoves(2);
            MovesManager.Moves = new List<Move>();

            ProgramLogic.Instance.Board.UndoLastTwoMoves(moves);

            if (MovesManager.Moves.Count == 0)
            {
                gameResignButton.Enabled = false;
                undoMoveButton.Enabled = false;
            }
        }

        private void gameResignButton_Click(object sender, EventArgs e)
        {
            string winnnerNickname = ProgramLogic.Instance.Match.GetPlayerByHumanity(false).GetNickname();

            ProgramLogic.Instance.QuitMatch();
            MovesManager.ClearMoves();

            CheckMateDialog checkMateDialog = new CheckMateDialog(false, winnnerNickname);

            checkMateDialog.StartPosition = FormStartPosition.CenterParent;
            checkMateDialog.ShowDialog();

            LoadTabPage(TabPageEnum.MainTabPage);
        }
    }
}

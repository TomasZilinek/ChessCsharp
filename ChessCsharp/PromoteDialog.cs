using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessPB069
{
    public partial class PromoteDialog : Form
    {
        public FigureType ChosenFigureType { get; private set; }
        private Color chosencolor = Color.FromArgb(183, 252, 164);
        private List<Button> figuresButtons = new List<Button>();

        public PromoteDialog(bool playerIsWhite)
        {
            InitializeComponent();

            figuresButtons.Add(queenButton);
            figuresButtons.Add(rookButton);
            figuresButtons.Add(knightButton);
            figuresButtons.Add(bishopButton);

             string colorString = playerIsWhite ? "White" : "Black";

            queenButton.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(colorString  + FigureType.Queen.ToString());
            rookButton.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(colorString + FigureType.Rook.ToString());
            knightButton.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(colorString + FigureType.Knight.ToString());
            bishopButton.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(colorString + FigureType.Bishop.ToString());

            ChooseFigure(FigureType.Queen);
        }

        private void queenButton_Click(object sender, EventArgs e)
        {
            ChooseFigure(FigureType.Queen);
        }

        private void rookButton_Click(object sender, EventArgs e)
        {
            ChooseFigure(FigureType.Rook);
        }

        private void knightButton_Click(object sender, EventArgs e)
        {
            ChooseFigure(FigureType.Knight);
        }

        private void bishopButton_Click(object sender, EventArgs e)
        {
            ChooseFigure(FigureType.Bishop);
        }

        private void ChooseFigure(FigureType figureType)
        {
            ChosenFigureType = figureType;

            queenButton.BackColor = Color.Transparent;
            rookButton.BackColor = Color.Transparent;
            knightButton.BackColor = Color.Transparent;
            bishopButton.BackColor = Color.Transparent;

            switch (figureType)
            {
                case FigureType.Queen:
                    queenButton.BackColor = chosencolor;
                    break;
                case FigureType.Rook:
                    rookButton.BackColor = chosencolor;
                    break;
                case FigureType.Knight:
                    knightButton.BackColor = chosencolor;
                    break;
                case FigureType.Bishop:
                    bishopButton.BackColor = chosencolor;
                    break;
            }
        }
    }
}

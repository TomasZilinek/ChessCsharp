using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessPB069
{
    public class BoardField
    {
        public Panel Panel { get; private set; }
        public BoardPosition BoardPosition { get; private set; }
        public static Size PanelSize { get; private set; }
        public Figure Figure { get; private set; }

        private static Color selectedColor = Color.FromArgb(247, 236, 91);
        private static Color whiteColor = Color.FromArgb(245, 245, 220);
        private static Color blackColor = Color.FromArgb(153, 115, 77);
        private static Color possibleMovesColorWhiteVersion = MixColors(whiteColor, Color.FromArgb(200, 154, 255, 99));
        private static Color possibleMovesColorBlackVersion = MixColors(blackColor, Color.FromArgb(200, 154, 255, 99));

        public bool IsWhite
        {
            get
            {
                if (BoardPosition.Y % 2 == 0)
                    return BoardPosition.X % 2 == 0;
                else
                    return BoardPosition.X % 2 == 1;
            }
        }

        private static Color MixColors(Color c1, Color c2)
        {
            return Color.FromArgb((byte)((c1.A + c2.A) / 2),
                                  (byte)((c1.R + c2.R) / 2),
                                  (byte)((c1.G + c2.G) / 2),
                                  (byte)((c1.B + c2.B) / 2));
        }

        public void ReceivePanel(Panel _panel, BoardPosition position)
        {
            Panel = _panel;

            BoardPosition = position;
        }

        public void ChangeSize(int newSize)
        {
            if (Panel != null)
            {
                Panel.Size = new Size(newSize, newSize);
                PanelSize = Panel.Size;
            }  
        }

        public void SetPanelPosition(Point newPosition)
        {
            if (Panel != null)
                Panel.Location = newPosition;
        }

        public void Select(bool selection)
        {
            if (selection)
            {
                Panel.BackColor = selectedColor;

                // Console.WriteLine("Field " + BoardPosition.ToChessNotation() + " selected");
            }
            else
                UnmarkColor();
        }

        public void MarkAsPossibleMove()
        {
            if (IsWhite)
                Panel.BackColor = possibleMovesColorWhiteVersion;
            else
                Panel.BackColor = possibleMovesColorBlackVersion;
        }

        public void MarkAsThreatened()
        {
            Panel.BackColor = Color.Red;
        }

        public void PlaceFigure(Figure figure)
        {
            Figure = figure;
            Bitmap image = (Bitmap)Properties.Resources.ResourceManager.GetObject((figure.IsWhite ? "White" : "Black")
                                                                                  + figure.FigureType.ToString());

            Panel.BackgroundImage = image;
        }

        public void RemoveFigure()
        {
            Panel.BackgroundImage = null;
            Figure = null;
        }

        public void UnmarkColor()
        {
            if (IsWhite)
                Panel.BackColor = whiteColor;
            else
                Panel.BackColor = blackColor;
        }
    }
}

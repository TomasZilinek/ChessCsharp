using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPB069
{
    public struct BoardPosition
    {
        public byte X;
        public byte Y;

        public BoardPosition(int x, int y)
        {
            X = (byte)x;
            Y = (byte)y;
        }

        public bool IsValidBoardPosition()
        {
            return X >= 0 && X <= 7 && Y >= 0 && Y <= 7;
        }

        public override string ToString()
        {
            return $"BoardPosition[{X.ToString()}, {Y.ToString()}][{ToChessNotation()}]";
        }

        public static bool operator ==(BoardPosition left, BoardPosition right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(BoardPosition left, BoardPosition right)
        {
            return !(left == right);
        }

        public static BoardPosition operator +(BoardPosition left, BoardPosition right)
        {
            return new BoardPosition(left.X + right.X, left.Y + right.Y);
        }

        public static BoardPosition operator -(BoardPosition left, BoardPosition right)
        {
            return new BoardPosition(left.X - right.X, left.Y - right.Y);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                BoardPosition p = (BoardPosition)obj;
                return (X == p.X) && (Y == p.Y);
            }
        }

        public override int GetHashCode()
        {
            return new { X, Y }.GetHashCode();
        }

        public string ToChessNotation()
        {
            return ((char)(X + 97)).ToString() + (Math.Abs(Y - 7) + 1).ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPB069
{
    public abstract class Player
    {
        public bool IsWhite { get; protected set; }

        public Player(bool isWhite)
        {
            IsWhite = isWhite;
        }

        public abstract string GetNickname();
    }
}

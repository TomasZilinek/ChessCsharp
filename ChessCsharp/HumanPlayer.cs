using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPB069
{
    public class HumanPlayer : Player
    {
        private string nickName;

        public HumanPlayer(bool isWhite, string _nickName) : base(isWhite)
        {
            nickName = _nickName;
        }

        public override string GetNickname()
        {
            return nickName;
        }
    }
}

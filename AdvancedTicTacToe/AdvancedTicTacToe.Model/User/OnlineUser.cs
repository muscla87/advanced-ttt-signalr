using System.Collections.Generic;

namespace AdvancedTicTacToe.Model.User
{
    public class OnlineUser
    {
        public string UserName { get; set; }

        public HashSet<string> ConnectionIds { get; set; }
    }
}

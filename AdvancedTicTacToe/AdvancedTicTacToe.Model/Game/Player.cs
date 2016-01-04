
using System.Collections.Generic;
namespace AdvancedTicTacToe.Model.Game
{
    public class Player
    {
        public string UserName { get; internal set; }

        public PlayerState State { get; internal set; }

        public string Color { get; set; }

        public string Icon { get; set; }

        public bool IsRoomOwner { get; set; }

        public bool IsReady { get; set; }
    }
}

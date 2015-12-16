
using System.Collections.Generic;
namespace AdvancedTicTacToe.Model.Game
{
    public class Player
    {
        public string UserName { get; internal set; }

        public PlayerState State { get; internal set; }

        public IEnumerable<PowerUp> PowerUps { get; internal set; }
    }
}

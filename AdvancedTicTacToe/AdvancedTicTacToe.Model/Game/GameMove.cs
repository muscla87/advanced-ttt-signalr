
namespace AdvancedTicTacToe.Model.Game
{
    public class GameMove
    {
        public string PlayerName { get; set; }

        public int OuterIndex { get; set; }

        public int InnerIndex { get; set; }

        public PowerUp? PowerUp { get; set; }

    }
}

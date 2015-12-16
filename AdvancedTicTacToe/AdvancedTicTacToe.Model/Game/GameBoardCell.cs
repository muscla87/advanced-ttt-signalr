
namespace AdvancedTicTacToe.Model.Game
{
    public class GameBoardCell
    {
        public int BigCellIndex { get; internal set; }

        public int LocalIndex { get; internal set; }

        public string OwnerName { get; internal set; }

        public PowerUp? PowerUp { get; internal set; }

    }
}


namespace AdvancedTicTacToe.Model.Game
{
    public class Game
    {
        public string UniqueId { get; set; }

        public string Player1 { get; set; }

        public string Player2 { get; set; }

        public string WinnerName { get; set; }

        public string StartPlayer { get; set; }

        public string ActivePlayer { get; set; }

        public int StartOuterIndex { get; set; }

        public int ActiveOuterIndex { get; set; }

        public int[,] Cells { get; set; }

        public GameState State { get; set; }

        public Game Clone()
        {
            var clonedGame = new Game()
            {
                UniqueId = this.UniqueId,
                Player1 = this.Player1,
                Player2 = this.Player2,
                StartPlayer = this.StartPlayer,
                WinnerName = this.WinnerName,
                ActivePlayer = this.ActivePlayer,
                StartOuterIndex = this.StartOuterIndex,
                ActiveOuterIndex = this.ActiveOuterIndex,
                Cells = new int[9, 9],
                State = this.State
            };

            for (var i = 0; i < this.Cells.GetLength(0); i++)
            {
                for (var j = 0; j < this.Cells.GetLength(1); j++)
                {
                    clonedGame.Cells[i, j] = this.Cells[i, j];
                }
            }


            return clonedGame;
        }

    }
}

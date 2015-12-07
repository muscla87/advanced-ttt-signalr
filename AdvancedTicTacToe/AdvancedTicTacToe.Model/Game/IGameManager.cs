
namespace AdvancedTicTacToe.Model.Game
{
    public interface IGameManager
    {
        Game CreateNewGame(string player1, string player2);

        GameMoveResult ApplyMove(string gameId, GameMove move, out Game newGameState); //Returns new game state without modifying the input one

        Game GetExistingRunningGame(string player1, string player2);

        Game GetExistingRunningGameById(string gameId);

        void NotifyUserDisconnection(string userName);

    }
}

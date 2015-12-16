
namespace AdvancedTicTacToe.Model.Game
{
    public interface IGameRoomsManager
    {
        /// <summary>
        /// Joins an existing room with another player waiting for opponent or creates an empty room with the given player waiting for other player
        /// </summary>
        /// <param name="userName">Name of the user who wants to join or create a room.</param>
        /// <returns>The selected room state</returns>
        GameRoomState JoinFreeRoomOrCreate(string userName);

        /// <summary>
        /// Creates the room for two players.
        /// </summary>
        /// <param name="p1UserName">Name of the player 1.</param>
        /// <param name="p2UserName">Name of the player 2.</param>
        /// <returns>The selected room state</returns>
        GameRoomState CreateRoomForTwoPlayers(string p1UserName, string p2UserName);

        /// <summary>
        /// Gets the state of the room.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <returns></returns>
        GameRoomState GetRoomState(int roomId);

        /// <summary>
        /// Sets the state of the player in given room.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="playerName">Name of the player.</param>
        /// <param name="newState">The new state.</param>
        void SetPlayerState(int roomId, string playerName, PlayerState newState);

        /// <summary>
        /// Starts a new game in the room. Both players must be in ready state and no game is currently active.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="playerName">Name of the player.</param>
        /// <param name="newState">The new state.</param>
        GameRoomState StartNewGame(int roomId);


        /// <summary>
        /// Applies the move to the current playing game in the given room.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="move">The move.</param>
        /// <returns></returns>
        GameRoomState ApplyMove(int roomId, GameMove move);


    }
}

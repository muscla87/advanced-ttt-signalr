
using System;
namespace AdvancedTicTacToe.Model.Game
{
    public class GameRoomState
    {

        private int gamesCount = 0;

        public GameRoomState(int uniqueId)
        {
            UniqueId = uniqueId;
        }

        public int UniqueId { get; private set; }

        public Player Player1 { get; internal set; }

        public Player Player2 { get; internal set; }

        public Game CurrentPlayingGame { get; internal set; }

        public int PlayersCount { get { return (Player1 == null ? 0 : 1) + (Player2 == null ? 0 : 1); } }

        public void SetPlayerState(string userName, PlayerState newState)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Cannot provide empty or null string", "userName");

            var player = GetPlayerByName(userName);

            if (player != null)
            {
                player.State = newState;
            }
            else
            {
                throw new InvalidOperationException("Player " + userName + " is not seated in the room");
            }

        }

        public void JoinFreeSeat(string userName)
        {
            if (PlayersCount == 2)
                throw new InvalidOperationException("Cannot join a full room");
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Cannot provide empty or null string", "userName");

            if (Player1 == null)
            {
                Player1 = CreatePlayer(userName);
            }
            else
            {
                Player2 = CreatePlayer(userName);
            }

        }

        public bool HasPlayer(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Cannot provide empty or null string", "userName");
            return (Player1 != null && Player1.UserName == userName) || (Player2 != null && Player2.UserName == userName);
        }

        public bool HasFreeSeat()
        {
            return Player1 == null || Player2 == null;
        }

        public void StartNewGame()
        {
            if (HasFreeSeat())
                throw new InvalidOperationException("Cannot start a new game because there are not enough players are sitting in this room");

            if (CurrentPlayingGame != null && CurrentPlayingGame.State == GameState.Running)
                throw new InvalidOperationException("Cannot start a new game because there's one already running");

            if (Player1.State != PlayerState.Ready)
                throw new InvalidOperationException("Cannot start a new game because " + Player1.UserName + " is not ready");

            if (Player2.State != PlayerState.Ready)
                throw new InvalidOperationException("Cannot start a new game because " + Player2.UserName + " is not ready");

            gamesCount++;
            CurrentPlayingGame = new Game(UniqueId + ":" + gamesCount, Player1.UserName, Player2.UserName);
        }

        private Player CreatePlayer(string userName)
        {
            return new Player()
            {
                UserName = userName,
                State = PlayerState.WaitingForStart,
                PowerUps = new PowerUp[0]
            };
        }

        private Player GetPlayerByName(string userName)
        {
            if (Player1 != null && Player1.UserName == userName)
                return Player1;

            if (Player2 != null && Player2.UserName == userName)
                return Player2;

            return null;
        }

    }
}

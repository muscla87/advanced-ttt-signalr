
using System;
namespace AdvancedTicTacToe.Model.Game
{
    public class GameRoomState
    {
        private object syncLock = new object();

        private int gamesCount = 0;

        public GameRoomState(int uniqueId)
        {
            UniqueId = uniqueId;
            TurnTimer = 15;
            UsePowerUps = true;
        }

        public int UniqueId { get; private set; }

        public Player Player1 { get; internal set; }

        public Player Player2 { get; internal set; }

        public Game CurrentPlayingGame { get; internal set; }

        public int TurnTimer { get; set; }

        public bool UsePowerUps { get; set; }

        public int PlayersCount { get { return (Player1 == null ? 0 : 1) + (Player2 == null ? 0 : 1); } }

        public void SetPlayerState(string userName, PlayerState newState)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Cannot provide empty or null string", "userName");
            lock (syncLock)
            {
                var player = GetPlayerByName(userName);

                if (player != null)
                {
                    player.State = newState;
                    if (CanStartGame())
                    {
                        StartNewGame();
                    }
                }
                else
                {
                    throw new InvalidOperationException("Player " + userName + " is not seated in the room");
                }
            }

        }

        public void SetPlayerIcon(string userName, string icon)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Cannot provide empty or null string", "userName");

            if (string.IsNullOrWhiteSpace(icon))
                throw new ArgumentException("Cannot provide empty or null string", "icon");

            lock (syncLock)
            {
                var player = GetPlayerByName(userName);

                if (player != null)
                {
                    player.Icon = icon;
                }
                else
                {
                    throw new InvalidOperationException("Player " + userName + " is not seated in the room");
                }
            }
        }

        public void SetPlayerColor(string userName, string color)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Cannot provide empty or null string", "userName");

            if (string.IsNullOrWhiteSpace(color))
                throw new ArgumentException("Cannot provide empty or null string", "color");
            lock (syncLock)
            {
                var player = GetPlayerByName(userName);

                if (player != null)
                {
                    player.Color = color;
                }
                else
                {
                    throw new InvalidOperationException("Player " + userName + " is not seated in the room");
                }
            }
        }

        public void SetGameTurnTimer(string userName, int timer)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Cannot provide empty or null string", "userName");

            //TODO: add proper validation of timer parameter in terms of range values
            lock (syncLock)
            {
                var player = GetPlayerByName(userName);

                if (player != null)
                {
                    if (player.IsRoomOwner)
                    {
                        TurnTimer = timer;
                    }
                    else
                    {
                        throw new InvalidOperationException("Player " + userName + " is not the owner of the room");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Player " + userName + " is not seated in the room");
                }
            }
        }

        public void SetGameUsePowerUps(string userName, bool usePowerUps)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Cannot provide empty or null string", "userName");

            lock (syncLock)
            {
                var player = GetPlayerByName(userName);

                if (player != null)
                {
                    if (player.IsRoomOwner)
                    {
                        UsePowerUps = usePowerUps;
                    }
                    else
                    {
                        throw new InvalidOperationException("Player " + userName + " is not the owner of the room");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Player " + userName + " is not seated in the room");
                }
            }
        }

        public void JoinFreeSeat(string userName)
        {
            lock (syncLock)
            {
                if (PlayersCount == 2)
                    throw new InvalidOperationException("Cannot join a full room");
                if (string.IsNullOrWhiteSpace(userName))
                    throw new ArgumentException("Cannot provide empty or null string", "userName");

                //TODO assign proper icon and color
                if (Player1 == null)
                {
                    Player1 = CreatePlayer(userName);
                    Player1.Icon = "Cross";
                    Player1.Color = "Yellow";
                    Player1.IsRoomOwner = true;
                }
                else
                {
                    Player2 = CreatePlayer(userName);
                    Player2.Icon = "Circle";
                    Player2.Color = "Red";
                }
            }
        }

        public bool HasPlayer(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Cannot provide empty or null string", "userName");
            lock (syncLock)
            {
                return (Player1 != null && Player1.UserName == userName) || (Player2 != null && Player2.UserName == userName);
            }
        }

        public bool HasFreeSeat()
        {
            lock (syncLock)
            {
                return Player1 == null || Player2 == null;
            }
        }

        public void LeaveRoom(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Cannot provide empty or null string", "userName");

            lock (syncLock)
            {
                var player = GetPlayerByName(userName);
                if (player != null)
                {
                    if (CurrentPlayingGame != null && CurrentPlayingGame.State == GameState.Running)
                    {
                        CurrentPlayingGame.PlayerAbandon(userName);
                    }

                    if (player == Player1)
                    {
                        Player1 = null;
                    }
                    else
                    {
                        Player2 = null;
                    }

                    if (PlayersCount == 0)
                    {
                        CurrentPlayingGame = null;
                    }

                }
                else
                {
                    throw new InvalidOperationException("Player " + userName + " is not seated in the room");
                }
            }
        }

        public void StartNewGame()
        {
            lock (syncLock)
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
                CurrentPlayingGame = new Game(UniqueId + ":" + gamesCount, Player1.UserName, Player2.UserName, UsePowerUps, TurnTimer);
            }
        }

        public void ResetForNewGame()
        {
            lock (syncLock)
            {
                if (CurrentPlayingGame != null && CurrentPlayingGame.State == GameState.Running)
                    throw new InvalidOperationException("Cannot reset the roombecause there's a game running");

                if (Player1 != null)
                    Player1.State = PlayerState.WaitingForStart;
                if (Player2 != null)
                    Player2.State = PlayerState.WaitingForStart;

                CurrentPlayingGame = null;
            }
        }

        public GameMoveResult ApplyMove(GameMove gameMove)
        {
            if (gameMove == null)
                throw new ArgumentException("Cannot provide empty or null string", "gameMove");
            if (CurrentPlayingGame == null || CurrentPlayingGame.State != GameState.Running)
                throw new InvalidOperationException("Move cannot be applied to game not in Running state");
            lock (syncLock)
            {
                return CurrentPlayingGame.ApplyMove(gameMove);
            }
        }

        private bool CanStartGame()
        {
            return Player1 != null && Player1.State == PlayerState.Ready && Player2 != null && Player2.State == PlayerState.Ready;
        }

        private Player CreatePlayer(string userName)
        {
            return new Player()
            {
                UserName = userName,
                State = PlayerState.WaitingForStart,
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

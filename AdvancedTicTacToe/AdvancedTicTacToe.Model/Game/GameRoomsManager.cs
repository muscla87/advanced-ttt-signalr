
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedTicTacToe.Model.Game
{
    public class GameRoomsManager : IGameRoomsManager
    {

        private object roomsLock = new object();
        private Dictionary<int, GameRoomState> rooms = new Dictionary<int, GameRoomState>();


        /// <summary>
        /// Joins an existing room with another player waiting for opponent or creates an empty room with the given player waiting for other player
        /// </summary>
        /// <param name="userName">Name of the user who wants to join or create a room.</param>
        /// <returns>
        /// The selected room state
        /// </returns>
        public GameRoomState JoinFreeRoomOrCreate(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Cannot provide empty or null string", "userName");

            lock (roomsLock)
            {
                //first look for room where this player has already joined -.-'
                GameRoomState room = rooms.Values.FirstOrDefault(r => r.HasPlayer(userName));
                if (room == null)
                {
                    //look for room with a free place
                    room = rooms.Values.Where(r => r.HasFreeSeat()).OrderByDescending(r => r.PlayersCount).FirstOrDefault();

                    //otherwise create a new room (another player will eventually fill the other free seat)
                    if (room == null)
                    {
                        room = new GameRoomState(rooms.Count + 1);
                        rooms.Add(room.UniqueId, room);
                    }

                    //JOIN ROOM
                    room.JoinFreeSeat(userName);
                }



                return room;
            }
        }

        /// <summary>
        /// Creates the room for two players.
        /// </summary>
        /// <param name="p1UserName">Name of the player 1.</param>
        /// <param name="p2UserName">Name of the player 2.</param>
        /// <returns>
        /// The selected room state
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public GameRoomState CreateRoomForTwoPlayers(string p1UserName, string p2UserName)
        {
            if (string.IsNullOrWhiteSpace(p1UserName))
                throw new ArgumentException("Cannot provide empty or null string", "p1UserName");

            if (string.IsNullOrWhiteSpace(p2UserName))
                throw new ArgumentException("Cannot provide empty or null string", "p2UserName");

            lock (roomsLock)
            {
                //let's check if these players are already in a room
                var existingRoom = rooms.Values.FirstOrDefault(r => r.HasPlayer(p1UserName) || r.HasPlayer(p2UserName));
                if (existingRoom != null)
                {
                    //if the room is already for both then let's return it
                    if (existingRoom.HasPlayer(p1UserName) && existingRoom.HasPlayer(p2UserName))
                    {
                        return existingRoom;
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("Cannot create a room for players {0} and {1} because one of the two is seated in another room", p1UserName, p2UserName));
                    }
                }
                else
                {
                    var room = new GameRoomState(rooms.Count + 1);
                    rooms.Add(room.UniqueId, room);
                    room.JoinFreeSeat(p1UserName);
                    room.JoinFreeSeat(p2UserName);
                    return room;
                }
            }
        }

        /// <summary>
        /// Gets the state of the room.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public GameRoomState GetRoomState(int roomId)
        {
            lock (roomsLock)
            {
                GameRoomState room = null;
                rooms.TryGetValue(roomId, out room);
                return room;
            }
        }

        /// <summary>
        /// Sets the state of the player in given room.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="playerName">Name of the player.</param>
        /// <param name="newState">The new state.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SetPlayerState(int roomId, string playerName, PlayerState newState)
        {
            lock (roomsLock)
            {
                GameRoomState room = null;
                if (rooms.TryGetValue(roomId, out room))
                {
                    room.SetPlayerState(playerName, newState);
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Cannot set player state because the room {0} does not exist", roomId));
                }
            }
        }

        /// <summary>
        /// Starts a new game in the room. Both players must be in ready state and no game is currently active.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public GameRoomState StartNewGame(int roomId)
        {
            lock (roomsLock)
            {
                GameRoomState room = null;
                if (rooms.TryGetValue(roomId, out room))
                {
                    room.StartNewGame();
                    return room;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Cannot start a new game state because the room {0} does not exist", roomId));
                }
            }
        }

        /// <summary>
        /// Applies the move to the current playing game in the given room.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="move">The move.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public GameRoomState ApplyMove(int roomId, GameMove move)
        {
            lock (roomsLock)
            {
                GameRoomState room = null;
                if (rooms.TryGetValue(roomId, out room))
                {
                    room.CurrentPlayingGame.ApplyMove(move);
                    return room;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Cannot apply game move because the room {0} does not exist", roomId));
                }
            }
        }
    }
}

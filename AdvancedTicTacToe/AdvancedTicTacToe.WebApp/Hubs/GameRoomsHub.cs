using AdvancedTicTacToe.Model.Game;
using AdvancedTicTacToe.WebApp.Models;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTicTacToe.WebApp.Hubs
{

    //[Authorize]
    public class GameRoomsHub : Hub
    {
        private static IGameRoomsManager GameRoomsManager = new GameRoomsManager();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region Server Side Methods

        public GameRoomState GetRoomState(int roomId)
        {
            try
            {
                return GameRoomsManager.GetRoomState(roomId);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in " + nameof(GetRoomState));
                throw;
            }
        }

        public GameRoomState JoinFreeRoomOrCreate()
        {
            try
            {
                string userName = Context.User.GetSafeUserName(Context.Request);
                var room = GameRoomsManager.JoinFreeRoomOrCreate(userName);

                this.SendRoomStateUpdated(room);

                return room;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in " + nameof(JoinFreeRoomOrCreate));
                throw;
            }
        }

        public void ClearRooms()
        {
            GameRoomsManager = new GameRoomsManager();
        }

        public void SetPlayerIcon(int roomId, string icon)
        {
            try
            {
                string userName = Context.User.GetSafeUserName(Context.Request);
                var room = GameRoomsManager.GetRoomState(roomId);
                room.SetPlayerIcon(userName, icon);
                this.SendRoomStateUpdated(room);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in " + nameof(SetPlayerIcon));
                throw;
            }
        }

        public void SetPlayerColor(int roomId, string color)
        {
            try
            {
                string userName = Context.User.GetSafeUserName(Context.Request);
                var room = GameRoomsManager.GetRoomState(roomId);
                room.SetPlayerColor(userName, color);
                this.SendRoomStateUpdated(room);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in " + nameof(SetPlayerColor));
                throw;
            }
        }

        public void SetPlayerReady(int roomId)
        {
            try
            {
                string userName = Context.User.GetSafeUserName(Context.Request);
                var room = GameRoomsManager.GetRoomState(roomId);
                room.SetPlayerState(userName, PlayerState.Ready);
                this.SendRoomStateUpdated(room);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in " + nameof(SetPlayerReady));
                throw;
            }
        }

        public void SetGameTurnTimer(int roomId, int timer)
        {
            try
            {
                string userName = Context.User.GetSafeUserName(Context.Request);
                var room = GameRoomsManager.GetRoomState(roomId);
                room.SetGameTurnTimer(userName, timer);
                this.SendRoomStateUpdated(room);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in " + nameof(SetGameTurnTimer));
                throw;
            }
        }

        public void SetGameUsePowerUps(int roomId, bool usePowerUps)
        {
            try
            {
                string userName = Context.User.GetSafeUserName(Context.Request);
                var room = GameRoomsManager.GetRoomState(roomId);
                room.SetGameUsePowerUps(userName, usePowerUps);
                this.SendRoomStateUpdated(room);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in " + nameof(SetGameUsePowerUps));
                throw;
            }
        }

        public void ApplyMove(int roomId, GameMove gameMove)
        {
            try
            {
                string userName = Context.User.GetSafeUserName(Context.Request);
                var room = GameRoomsManager.GetRoomState(roomId);
                gameMove.PlayerName = userName;
                if (room.ApplyMove(gameMove) == GameMoveResult.Valid)
                {
                    this.SendRoomStateUpdated(room);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in " + nameof(SetGameUsePowerUps));
                throw;
            }
        }

        public void LeaveRoom(int roomId)
        {
            try
            {
                string userName = Context.User.GetSafeUserName(Context.Request);
                var room = GameRoomsManager.GetRoomState(roomId);
                room.LeaveRoom(userName);
                this.SendRoomStateUpdated(room);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in " + nameof(SetGameUsePowerUps));
                throw;
            }
        }

        public void ResetRoomForNewGame(int roomId)
        {
            try
            {
                string userName = Context.User.GetSafeUserName(Context.Request);
                var room = GameRoomsManager.GetRoomState(roomId);
                room.ResetForNewGame();
                this.SendRoomStateUpdated(room);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in " + nameof(SetGameUsePowerUps));
                throw;
            }
        }

        #endregion

        #region Client Side Methods

        private void SendRoomStateUpdated(GameRoomState room)
        {
            var userNames = new List<string>();
            if (room.Player1 != null)
            {
                userNames.Add(room.Player1.UserName);
            }
            if (room.Player2 != null)
            {
                userNames.Add(room.Player2.UserName);
            }

            if (userNames.Any())
            {
                StringBuilder debugText = new StringBuilder();
                debugText.AppendLine("Sending room " + room.UniqueId + " update: " + JsonConvert.SerializeObject(room));

                foreach (var user in userNames)
                {
                    foreach (var cid in UsersHub.OnlineUsersStore.GetUserConnectionIds(user))
                    {
                        Clients.Client(cid).roomStateChanged(room);
                        debugText.AppendFormat("Sending state update to {0} : {1}\n", user, cid);
                    }
                }

                logger.Debug(debugText.ToString());
            }
        }

        #endregion

        public override Task OnConnected()
        {
            string userName = Context.User.GetSafeUserName(Context.Request);
            string connectionId = Context.ConnectionId;
            logger.Debug("GameHub OnConnected: {0}:{1}", userName, connectionId);

            if (!string.IsNullOrEmpty(userName))
            {
                bool firstConnection = UsersHub.OnlineUsersStore.RegisterUserConnection(userName, connectionId);

                if (firstConnection)
                {
                    Clients.All.usersListUpdated(new
                    {
                        currentUsersList = UsersHub.OnlineUsersStore.GetConnectedUserNames(),
                        connectedUserName = userName
                    });
                }
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = Context.User.GetSafeUserName(Context.Request);
            string connectionId = Context.ConnectionId;
            logger.Debug("GameHub OnDisconnected: {0}:{1}", userName, connectionId);

            if (!string.IsNullOrEmpty(userName))
            {
                bool allConnectionsClosed = UsersHub.OnlineUsersStore.RegisterUserDisconnection(userName, connectionId);

                if (allConnectionsClosed)
                {
                    Clients.All.usersListUpdated(new
                    {
                        currentUsersList = UsersHub.OnlineUsersStore.GetConnectedUserNames(),
                        disconnectedUserName = userName
                    });
                }
            }

            return base.OnDisconnected(stopCalled);
        }

    }
}
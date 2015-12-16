using AdvancedTicTacToe.Model.Game;
using AdvancedTicTacToe.WebApp.Models;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedTicTacToe.WebApp.Hubs
{

    //[Authorize]
    public class GameRoomsHub : Hub
    {

        private static IGameRoomsManager GameRoomsManager = new GameRoomsManager();


        public GameRoomState JoinFreeRoomOrCreate()
        {
            string userName = Context.User.GetSafeUserName(Context.Request);
            var room = GameRoomsManager.JoinFreeRoomOrCreate(userName);

            this.SendRoomStateUpdated(room);

            return room;
        }

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
                foreach (var user in userNames)
                {
                    foreach (var cid in UsersHub.OnlineUsersStore.GetUserConnectionIds(user))
                    {
                        Clients.Client(cid).roomStateChanged(room);
                    }
                }
            }
        }


        public GameRoomState GetRoomState(int roomId)
        {

            return GameRoomsManager.GetRoomState(roomId);
        }

        public void ClearRooms()
        {
            GameRoomsManager = new GameRoomsManager();
        }


        public override Task OnConnected()
        {
            string userName = Context.User.GetSafeUserName(Context.Request);
            string connectionId = Context.ConnectionId;
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {

            string userName = Context.User.GetSafeUserName(Context.Request);
            string connectionId = Context.ConnectionId;
            return base.OnDisconnected(stopCalled);
        }

    }
}
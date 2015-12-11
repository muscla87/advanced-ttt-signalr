using AdvancedTicTacToe.Model.User;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedTicTacToe.WebApp.Hubs
{

    //[Authorize]
    public class UsersHub : Hub
    {

        private static readonly IOnlineUsersStore OnlineUsersStore = new OnlineUsersStore();


        public void Send(string message)
        {

            string sender = Context.User.Identity.Name;

            // So, broadcast the sender, too.
            Clients.All.received(new { sender = sender, message = message, isPrivate = false });
        }

        public IEnumerable<string> GetConnectedUsers()
        {

            return OnlineUsersStore.GetConnectedUsers()
                /*.Where(x =>
            {

                lock (x.ConnectionIds)
                {

                    return !x.ConnectionIds.Contains(Context.ConnectionId, StringComparer.InvariantCultureIgnoreCase);
                }

            })*/
            .OrderBy(u => u.UserName)
            .Select(u => u.UserName);
        }

        public override Task OnConnected()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                string userName = Context.User.Identity.Name;
                string connectionId = Context.ConnectionId;

                bool firstConnection = OnlineUsersStore.RegisterUserConnection(userName, connectionId);

                if (firstConnection)
                {
                    Clients.All.usersListUpdated(new
                    {
                        currentUsersList = OnlineUsersStore.GetConnectedUserNames(),
                        connectedUserName = userName
                    });
                }
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {

            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            bool allConnectionsClosed = OnlineUsersStore.RegisterUserDisconnection(userName, connectionId);

            if (allConnectionsClosed)
            {
                Clients.All.usersListUpdated(new
                {
                    currentUsersList = OnlineUsersStore.GetConnectedUserNames(),
                    disconnectedUserName = userName
                });
            }

            return base.OnDisconnected(stopCalled);
        }

    }
}
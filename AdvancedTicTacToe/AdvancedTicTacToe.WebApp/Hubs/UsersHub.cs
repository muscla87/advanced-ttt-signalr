﻿using AdvancedTicTacToe.Model.User;
using AdvancedTicTacToe.WebApp.Models;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedTicTacToe.WebApp.Hubs
{

    //[Authorize]
    public class UsersHub : Hub
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        //TODO: use IoC
        public static readonly IOnlineUsersStore OnlineUsersStore = new OnlineUsersStore();


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
            string userName = Context.User.GetSafeUserName(Context.Request);
            string connectionId = Context.ConnectionId;
            logger.Debug("UsersHub OnConnected: {0}:{1}", userName, connectionId);

            if (!string.IsNullOrEmpty(userName))
            {
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
            string userName = Context.User.GetSafeUserName(Context.Request);
            string connectionId = Context.ConnectionId;
            logger.Debug("UsersHub OnDisconnected: {0}:{1}", userName, connectionId);
            if (!string.IsNullOrEmpty(userName))
            {
                bool allConnectionsClosed = OnlineUsersStore.RegisterUserDisconnection(userName, connectionId);

                if (allConnectionsClosed)
                {
                    Clients.All.usersListUpdated(new
                    {
                        currentUsersList = OnlineUsersStore.GetConnectedUserNames(),
                        disconnectedUserName = userName
                    });
                }
            }
            return base.OnDisconnected(stopCalled);
        }

    }
}
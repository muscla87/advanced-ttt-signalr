using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedTicTacToe.Model.User
{
    public class OnlineUsersStore : IOnlineUsersStore
    {
        private static readonly ConcurrentDictionary<string, OnlineUser> onlineUsers
            = new ConcurrentDictionary<string, OnlineUser>(StringComparer.InvariantCultureIgnoreCase);

        public IEnumerable<OnlineUser> GetConnectedUsers()
        {
            return onlineUsers.Values;
        }

        public bool RegisterUserConnection(string userName, string connectionId)
        {
            var user = onlineUsers.GetOrAdd(userName, _ => new OnlineUser
            {
                UserName = userName,
                ConnectionIds = new HashSet<string>()
            });


            bool isFirstConnection = false;

            lock (user.ConnectionIds)
            {

                user.ConnectionIds.Add(connectionId);

                isFirstConnection = user.ConnectionIds.Count == 1;
            }

            return isFirstConnection;
        }


        public bool RegisterUserDisconnection(string userName, string connectionId)
        {
            bool allConnectionsClosed = false;
            OnlineUser user;
            onlineUsers.TryGetValue(userName, out user);

            if (user != null)
            {

                lock (user.ConnectionIds)
                {

                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));

                    if (!user.ConnectionIds.Any())
                    {
                        allConnectionsClosed = true;
                        OnlineUser removedUser;
                        onlineUsers.TryRemove(userName, out removedUser);
                    }
                }
            }

            return allConnectionsClosed;
        }


        public IEnumerable<string> GetUserConnectionIds(string userName)
        {
            OnlineUser user;
            if (onlineUsers.TryGetValue(userName, out user))
            {
                lock (user.ConnectionIds)
                {
                    return user.ConnectionIds.ToList();
                }
            }
            else
            {
                return new string[0];
            }

        }


        public IEnumerable<string> GetConnectedUserNames()
        {
            return onlineUsers.Keys.OrderBy(x => x);
        }
    }
}

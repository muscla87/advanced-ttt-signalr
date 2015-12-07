
using System.Collections.Generic;
namespace AdvancedTicTacToe.Model.User
{
    public interface IOnlineUsersStore
    {

        bool RegisterUserConnection(string userName, string connectionId);

        bool RegisterUserDisconnection(string userName, string connectionId);

        IEnumerable<string> GetUserConnectionIds(string userName);

        IEnumerable<OnlineUser> GetConnectedUsers();

        IEnumerable<string> GetConnectedUserNames();

    }
}

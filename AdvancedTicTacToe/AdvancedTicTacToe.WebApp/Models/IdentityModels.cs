using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

namespace AdvancedTicTacToe.WebApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public static class IPrincipalExtension
    {

        public static string GetSafeUserName(this IPrincipal principal, HttpRequestBase request, HttpResponseBase response)
        {

            HttpCookie tmpUserNameCookie = request.Cookies["TempUserName"];
            string tmpUserNameInCookie = null;
            if (tmpUserNameCookie != null && !string.IsNullOrEmpty(tmpUserNameCookie.Value))
            {
                tmpUserNameInCookie = tmpUserNameCookie.Value;
            }

            return GetSafeUserName(principal, tmpUserNameInCookie, response);

        }

        public static string GetSafeUserName(this IPrincipal principal, IRequest request)
        {
            string tmpUserNameInCookie = null;
            if (request.Cookies.ContainsKey("TempUserName"))
            {
                Cookie tmpUserNameCookie = request.Cookies["TempUserName"];
                if (tmpUserNameCookie != null && !string.IsNullOrEmpty(tmpUserNameCookie.Value))
                {
                    tmpUserNameInCookie = tmpUserNameCookie.Value;
                }
            }
            return GetSafeUserName(principal, tmpUserNameInCookie);

        }


        private static string GetSafeUserName(this IPrincipal principal, string tmpUserNameFromCookie, HttpResponseBase response = null)
        {
            if (principal.Identity.IsAuthenticated)
            {
                return principal.Identity.Name;
            }
            else
            {
                if (tmpUserNameFromCookie != null && !string.IsNullOrEmpty(tmpUserNameFromCookie))
                {
                    return tmpUserNameFromCookie;
                }
                else if (response != null)
                {
                    string tmpUserName = "User-" + Guid.NewGuid().ToString().Substring(0, 5);
                    var tmpUserNameCookie = new HttpCookie("TempUserName", tmpUserName);
                    response.Cookies.Add(tmpUserNameCookie);
                    return tmpUserName;
                }
                else
                {
                    return null;
                }
            }

        }

    }
}
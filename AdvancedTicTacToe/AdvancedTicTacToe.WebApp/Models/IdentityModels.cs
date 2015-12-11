using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
            if (principal.Identity.IsAuthenticated)
            {
                return principal.Identity.Name;
            }
            else
            {
                HttpCookie tmpUserNameCookie = request.Cookies["TempUserName"];
                if (tmpUserNameCookie != null && !string.IsNullOrEmpty(tmpUserNameCookie.Value))
                {
                    return tmpUserNameCookie.Value;
                }
                else
                {
                    string tmpUserName = "User-" + Guid.NewGuid().ToString().Substring(0, 5);
                    tmpUserNameCookie = new HttpCookie("TempUserName", tmpUserName);
                    response.Cookies.Add(tmpUserNameCookie);
                    return tmpUserName;
                }
            }

        }

    }
}
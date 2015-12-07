using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AdvancedTicTacToe.WebApp.Startup))]
namespace AdvancedTicTacToe.WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}

using System.Web.Mvc;

namespace AdvancedTicTacToe.WebApp.Controllers
{
    public class GameController : Controller
    {
        public ActionResult Index()
        {
            System.Threading.Thread.Sleep(500);
            return View();
        }

        public ActionResult Play(string gameId)
        {
            return View();
        }

    }
}
using System.Web.Mvc;

namespace AdvancedTicTacToe.WebApp.Controllers
{
    public class GameController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Play(string gameId)
        {
            System.Threading.Thread.Sleep(500);
            return View();
        }

    }
}
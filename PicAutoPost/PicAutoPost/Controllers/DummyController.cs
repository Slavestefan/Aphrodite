

using Microsoft.AspNetCore.Mvc;

namespace Slavestefan.Aphrodite.Web.Controllers
{
    /// <summary>
    /// This is just a dummy controller that points to an empty page for the bot's keep alive.
    /// </summary>
    public class DummyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
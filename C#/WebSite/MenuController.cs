using System.Web.Mvc;

namespace Equipment.Controllers.WebSite
{
    [Authorize]
    public class MenuController : Controller
    {
        //
        // GET: /Menu/
        public ActionResult Index()
        {
            return View();
        }
    }
}
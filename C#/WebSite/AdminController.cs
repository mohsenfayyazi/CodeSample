using System.Web.Mvc;

namespace Equipment.Controllers.WebSite
{
    [Authorize]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

    }
}

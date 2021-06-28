using System.Web.Mvc;

namespace Equipment.Controllers.planning
{
    /// <summary>
    /// بلاغ پروژه
    /// </summary>
   [Authorize]
    public class impartProjectController : Controller
    {
        //
        // GET: /impartProject/
        public ActionResult Index()
        {
            return View();
        }
    }
}
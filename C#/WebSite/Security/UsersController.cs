using Equipment.Codes.Security;
using System.Web.Mvc;

namespace Equipment.Controllers.WebSite.Security
{
    [Authorize]
    public class UsersController : DbController
    {

        public ActionResult Index()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult Succession()
        {
            return View();
        }

    }
}
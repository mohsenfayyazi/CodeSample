using Equipment.Codes.Security;
using Kendo.Mvc.UI;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Report
{
    [Authorize]
    public partial class ReportController : Controller
    {
        [MenuAuthorize]
        public ActionResult GeoghLoc()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult OwenerComp()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult Dimand()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult CheakList()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult CheakListZamin()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult SignifickLet()
        {
            return View();
        }

        public ActionResult FndWorkOrdRead([DataSourceRequest] DataSourceRequest request, string wYear, string crpl)
        {
            var query = (from b in cntx.FND_WORK_ORD where b.W_YEAR == wYear && b.CRPL == crpl select b)
                            .AsEnumerable().Select(p => new
                            {
                                p.WR_SEQN,
                                p.W_YEAR,
                                p.WR_DESC
                            });
            var data = new DataSourceResult
            {
                Data = query.ToList()
            };
            return Json(data);
        }

    }

}
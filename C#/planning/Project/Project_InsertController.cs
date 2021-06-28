using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Planning.Project
{
    [Authorize]
    public partial class ProjectController : Controller
    {
        //
        // GET: /Project/
        public ActionResult Project_Insert(int? id)
        {
            ViewData["ESIL_ID"] = id;
            return View();
        }
        public ActionResult ViewForm(string id, decimal notId)
        {
            ViewData["ESIL_ID"] = id;
            ViewData["notId"] = notId;
            return View();
        }
        public ActionResult Project_Insert_line(int? id)
        {
            //ViewBag.Item = from b in db.PDF_STANDARD_SPECIFICATION select b;
            ViewData["ESIL_ID"] = id;
            return View();
        }

        public ActionResult ReadPlans()
        {
            var query = from b in db.CGT_PLAN select new { b.PLN_CODE, b.PLN_DESC };
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadProjects()
        {
            var query = from b in db.CGT_PRO select new { b.PRJ_CODE, b.PRJ_DESC };
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadProgramTime([DataSourceRequest] DataSourceRequest request)
        {
            var query = from b in db.PLN_SIGNIFIC_LETER select new { b.ESIL_NO, b.ESIL_TYPE, b.ESIL_GRANT, b.ESIL_FGRANT };
            return Json(query.ToDataSourceResult(request));
        }

        //public ActionResult SaveModel(Equipment.Models.PartialClass.MultiModelPrj MMM)
        //{
        //    MMM.PDF_PROJECT_SPECIFICATION.PDF_STANDARD_SPECIFICATION = MMM.PDF_STANDARD_SPECIFICATION;
        //    MMM.PLN_SIGNIFIC_LETER.
        //    return "";
        //}
    }
}

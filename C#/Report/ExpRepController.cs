using Equipment.Codes.Security;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Report
{
    [Authorize]
    public partial class ReportController : Controller
    {
        [MenuAuthorize]
        public ActionResult Report_Defect_Amari()
        {
            return View();
        }
        public ActionResult Report_Contract_Remain()
        {
            return View();
        }
        public ActionResult Report_Contract_Perf()
        {
            return View();
        }
        public ActionResult Report_Contract_Type()
        {
            return View();
        }
        [MenuAuthorize]
        public ActionResult ExpMotevasetN()
        {
            return View();
        }

        [EntityAuthorize("CHK_DOMAIN > select")]
        public ActionResult GetCHKDOMAIN()
        {
            var RetVal = from b in cntx.CHK_DOMAIN where (b.DMAN_DMAN_ID == 369) orderby b.DMAN_ID select new { b.DMAN_TITL, b.DMAN_ID };
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [MenuAuthorize]
        public ActionResult ExpMotevasetNE()
        {
            Session["Key"] = 10;
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpAmar()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpEnarge()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpEtelaattKhoroge()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpDefetPost()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpShakesDefect()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpDefectTaj()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpAmarDefct()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpDefectBargh()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpReqWorkRq()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpMontlyTrans()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpExitKhat()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpHavadesList()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpKoliHavades()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpGhatTrans()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpMizanTolid()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpComparePik()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpTahodHavades()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpAmalkarddarkhast()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpAmalkarddarkhastN()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpCompareAmar()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpAmarGhatTrans()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpAmarghat()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpAmalKhat()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpTajMaub()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ExpTajPost()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult jamehadese()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ghatekhat()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult dastebanditedad()
        {
            return View();
        }

    }

}
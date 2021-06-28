using System.Web.Mvc;
using Asr.Base;

namespace Equipment.Controllers.Report
{
    [Authorize]
    [Developer("H.Hamidi")]
    public partial class ReportController : Controller
    {

        public ActionResult SummaryReport_ExploitationPosts(string state) // گزارش شماره 1 - مشخصات کلی پست های درحال بهره برداری
        {
            ViewBag.fromStatistics = string.IsNullOrEmpty(state) ? "0" : "1";
            return View();
        }

        public ActionResult SummaryReport_ExploitationLines(string state) // گزارش شماره 2 - طول خطوط در حال بهره برداری
        {
            ViewBag.fromStatistics = string.IsNullOrEmpty(state) ? "0" : "1";
            return View();
        }

    }
}
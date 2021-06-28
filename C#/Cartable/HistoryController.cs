using Asr.Base;
using Asr.Cartable;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers
{
    [Developer("A.Saffari")]
    [Authorize]
    public class HistoryController : Controller
    {
        //
        // GET: /History/
        public ActionResult Index(int id)
        {
            if (id == 0)
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            AsrWorkFlowProcess wp = new AsrWorkFlowProcess(id);
            return PartialView(wp.History.OrderBy(x => x.NOT_ID));
        }

        public ActionResult IndexHistory(int id)
        {
            if (id == 0)
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            AsrWorkFlowProcess wp = new AsrWorkFlowProcess(id);
            return PartialView(wp.History.OrderBy(x => x.NOT_ID));
        }

        public ActionResult GetHistory(string itemType, string itemKey)
        {
            if (string.IsNullOrEmpty(itemType) || string.IsNullOrEmpty(itemKey))
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            using (Asr.Cartable.Models.OraOwfEngineConnStr cntx = new Asr.Cartable.Models.OraOwfEngineConnStr())
            {
                var model = cntx.WF_NOTE_V
                    .Where(x => x.MESSAGE_TYPE == itemType && x.ITEM_KEY == itemKey)
                    .OrderBy(x => x.NOT_ID)
                    .ToList();
                return PartialView("Index", model);
            }
        }

    }

}

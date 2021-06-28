using Equipment.Models;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Planning.Estimate
{
    [Authorize]
    public partial class EstimateController : Controller
    {
        public ActionResult Add_EST_OPERATION(EST_OPERATION NewOperation)
        {
            NewOperation.SaveToDataBase();
            return Json(new { Success = true });
        }

        public JsonResult GetEstMate(int EOCO_EOCO_ID)
        {
            using (var northwind = new BandarEntities())
            {
                var OwnerEST_ESTIMATE = from b in northwind.EST_ESTIMATE where b.EOCO_EOCO_ID == EOCO_EOCO_ID select b;

                //if (!string.IsNullOrEmpty(OwnerTypes))
                //{
                //    OwnerCompany = OwnerCompany.Where(p => p.EOTY_EOTY_ID.ToString() == OwnerTypes);
                //}
                //.ToString() + "-" + c.FINY_FINY_YEAR 
                var tt = from c in OwnerEST_ESTIMATE select new { ESMT_ID = c.ESMT_ID, ESMT_DIMAND = c.ESMT_DIMAND };
                //OwnerEST_ESTIMATE.Select(c => new { ESMT_ID = c.ESMT_ID, ESMT_DIMAND = c.ESMT_DIMAND.ToString() + "-" + c.FINY_FINY_YEAR })

                return Json(tt, JsonRequestBehavior.AllowGet);
            }
        }

    }

}

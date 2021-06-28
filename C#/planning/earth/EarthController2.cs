using Equipment.Codes.Security;
using Equipment.DAL;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Equipment.Reporting;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

//using Microsoft.AspNet.SignalR;

namespace Equipment.Controllers.Planning.Earth
{
    [Authorize]
    public partial class EarthController
    {

        public ActionResult T()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult EAR_EARTH()
        {
            //using (BandarEntities b = this.DB())
            //{
            EAR_EARTH ear = cntx.EAR_EARTH.Find(294);
            ViewBag.Mymodel = ear;
            return View();
            //}
        }

      
       public ActionResult EAR_EARTH_Check_Read([DataSourceRequest] DataSourceRequest request, int? ERTH_ERTH_ID)
        {
            var Query = (from a in cntx.EAR_EARTH_CHECK_LIST
                         where 
                         a.ERTH_ERTH_ID == ERTH_ERTH_ID
                         select new
                         {
                             a.ECHL_ID,
                             a.ECHL_DESC,
                             
                         }).ToList().Distinct();            

            return Json(Query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

       


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Earth_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<EAR_EARTH> earths)
        {
            EAR_EARTH[] earEarths = earths as EAR_EARTH[] ?? earths.ToArray();
            if (earths != null)
            {
                foreach (EAR_EARTH earth in earEarths)
                {
                    earth.Update();
                }
            }
            return Json(earEarths.ToDataSourceResult(request, ModelState));
        }

        public ActionResult GetCheckListes(int id)
        {
            var model = EarthRepository.Get_EAR_EARTH().FirstOrDefault(b => b.ERTH_ID == id);
            return View(model);
        }

        public ActionResult ShowCheckList(int id)
        {
            ViewData["ECHL_ID"] = id;

            return View();
        }
        public ActionResult Testreport()
        {
            return DatabeseReport.ViewReport(24, "AmirhosseinSaffari", 225, 2555000d);
        }

    }

}

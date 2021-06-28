using Asr.Base;
using Equipment.Codes.Security;
using Equipment.DAL;
using Equipment.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Planning.Appraisal
{
    /// <summary>
    /// برآورد بار
    /// </summary>
    [Authorize]
    [Developer("A.Saffari")]
    public partial class AppraisalController : Controller
    {
        // GET: /appraisal/
        BandarEntities Db;
        public AppraisalController()
        {
            Db = this.DB();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///فرم ثبت مرکز مصرف
        /// </summary>
        /// <returns></returns>
        public ActionResult AddUsinCenter()
        {
            return View();
        }

        /// <summary>
        /// ثبت مرکز مصرف
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public ActionResult SaveUsingCenter(cUsingCenter Obj)
        {
            //Save Code Place Hiere!!!!
            return View();
        }

        /// <summary>
        /// فرم ثبت مشترک
        /// </summary>
        /// <returns></returns>
        public ActionResult AddMoshtarek()
        {
            return View();
        }

        /// <summary>
        /// تعریف مراکز مصرف
        /// </summary>
        /// <returns></returns>
        [MenuAuthorize]
        public ActionResult BKP_GEOGH_LOC()
        {
            return View();
        }

        public ActionResult BKP_GEOGH_LOC_add(BKP_GEOGH_LOC NewItem)
        {
            try
            {
                NewItem.SaveToDatabase();
                return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                return Json(new { Success = "False" }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public ActionResult BKP_GEOGH_LOC_Read([DataSourceRequest]
                                               DataSourceRequest request)
        {
            Session["BkpGeoghLoc-Grid-Filters"] = request;
            var data = AppraisalRepository.cntx
                                          .BKP_GEOGH_LOC
                                          .AsQueryable()
                                          .ApplyBkpGeoghLocFiltering(request.Filters)
                                          .ApplyBkpGeoghLocSorting(request.Groups, request.Sorts)
                                          .OrderBy(o => o.G_DESC)
                                          .AsEnumerable()
                                          .Select(b => new
                                          {
                                              b.G_CODE,
                                              b.G_DESC,
                                              //numberOfCustomers = b.EXP_OWENER_COMPANY.Count
                                          })
                                          .ToDataSourceResult(request);
            
            return Json(data, JsonRequestBehavior.AllowGet);
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BKP_GEOGH_LOC_Update([DataSourceRequest]
                                                 DataSourceRequest request, BKP_GEOGH_LOC bkp_geogh_loc)
        {
            if (bkp_geogh_loc != null && ModelState.IsValid)
            {
                var q = (from b in Db.BKP_GEOGH_LOC where b.G_CODE == bkp_geogh_loc.G_CODE select b).FirstOrDefault();
                if (q != null)
                {
                    q.G_DESC = bkp_geogh_loc.G_DESC;
                    Db.SaveChanges();
                }
            }
            return Json(new[] { bkp_geogh_loc }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult DeleteSelectedRows(List<string> SelectedRows)
        {
            foreach (string item in SelectedRows)
            {
                Console.WriteLine(item);
            }
            return Json(new { Deleted = "Success" }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<EXP_OWENER_COMPANY> Get_Owner_Companys()
        {
            var query = from b in Db.EXP_OWENER_COMPANY select b;// join  c in Context.EXP_OWENER_TYPE on b.EOTY_EOTY_ID equals c.EOTY_ID select b ;
            return query.ToList();
        }
    }
}
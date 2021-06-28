using Equipment.Codes.Security;
using Equipment.DAL;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Planning.Appraisal
{
    [Authorize]
    public partial class AppraisalController : Controller
    {
        /// <summary>
        /// Fill Needed Data To ViewBag
        /// </summary>
        private void FillViewBag()
        {
            #region Fill Needed Data To ViewBag(Expand To See Content)
            
            var ownerTypesQuery = from itm in Db.EXP_OWENER_TYPE
                                  select new
                                  {
                                      itm.EOTY_ID,
                                      itm.EOTY_DESC
                                  };
            ViewBag.OwnerTypeLST = ownerTypesQuery.ToList();
            var geogh = from itms in Db.BKP_GEOGH_LOC
                        select new
                        {
                            G_CODE = itms.G_CODE,
                            G_DESC = itms.G_DESC
                        };
            ViewBag.GeoghLocLST = geogh.AsEnumerable();
            
            #endregion
        }
        
        /// <summary>
        /// Customers List View
        /// </summary>
        /// <returns></returns>
        [MenuAuthorize]
        public ActionResult Customer()
        {
            FillViewBag();
            return View().IfUserCanAccess(HttpContext);
        }
        
        /// <summary>
        /// فرم ثبت مشترک
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCustomer(int? id)
        {
            try
            {
                FillViewBag();
                if (id == null)
                {
                    return View().IfUserCanAccess(HttpContext);
                }
                else
                {
                    return View(PublicRepository.cntx.EXP_OWENER_COMPANY.Find(id)).IfUserCanAccess(HttpContext);
                }
            }
            catch (Exception ex)
            {
                HttpContext.AddError(ex);
                return View();
            }
        }
        
        /// <summary>
        /// ثبت مشترک
        /// </summary>
        [HttpPost]
        public ActionResult SaveNewCoustomer(EXP_OWENER_COMPANY obj)
        {
            try
            {
                if (PublicRepository.ExistModel("EXP_OWENER_COMPANY", "EOCO_DESC='{0}'", obj.EOCO_DESC))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}] تکراری است.", obj.EOCO_DESC) }.ToJson();
                }
                //obj.SaveToDataBase();
                Db.EXP_OWENER_COMPANY.Add(obj);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد", obj.EOCO_DESC) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}] ثیت نشد[{1}].", obj.EOCO_DESC, ex.PersianMessage()) }.ToJson();
            }
        }
        
        /// <summary>
        /// ویرایش مشترک
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult UpdateCoustomer(EXP_OWENER_COMPANY obj)
        {
            try
            {
                if (obj == null)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "ویرایش انجام نشد" }.ToJson();
                }
                
                if (PublicRepository.ExistModel("EXP_OWENER_COMPANY", "EOCO_DESC='{0}' AND EOCO_ID<>{1}", obj.EOCO_DESC, obj.EOCO_ID))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}] تکراری است.", obj.EOCO_DESC) }.ToJson();
                }
                
                var cntx = PublicRepository.GetNewDatabaseContext;
                var original = cntx.EXP_OWENER_COMPANY.Find(obj.EOCO_ID);
                if (original != null)
                {
                    original.EOCO_DESC = obj.EOCO_DESC;
                    original.ACTV_TYPE = obj.ACTV_TYPE;
                    original.EOTY_EOTY_ID = obj.EOTY_EOTY_ID;
                    original.GEOL_G_CODE = obj.GEOL_G_CODE;
                    cntx.SaveChanges();
                    return new ServerMessages(ServerOprationType.Success) { Message = "ویرایش شد" }.ToJson();
                }
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "ویرایش انجام نشد" }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}] ویرایش نشد[{1}].", obj.EOCO_DESC, ex.PersianMessage()) }.ToJson();
            }
        }
        
        [HttpPost]
        public ActionResult ExpOwenerCompanyRead([DataSourceRequest]
                                                 DataSourceRequest request)
        {
            Session["ExpOwenerCompany-Grid-Filters"] = request;
            var data = AppraisalRepository.Get_EXP_OWNER_COMPANY()
                                          .ApplyExpOwenerCompanyFiltering(request.Filters)
                                          .ApplyExpOwenerCompanySorting(request.Groups, request.Sorts)
                                          .Select(p => new
                                          {
                                              p.EOCO_ID,
                                              p.EOCO_DESC,
                                              p.EOTY_EOTY_ID,
                                              p.ACTV_TYPE,
                                              p.CRET_BY,
                                              p.CRET_DATE,
                                              p.GEOL_G_CODE
                                          })
                                          .ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
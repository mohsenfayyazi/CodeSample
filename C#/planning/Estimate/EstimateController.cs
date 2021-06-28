using Asr.Base;
using Equipment.Codes.Security;
using Equipment.DAL;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;

//Amihossein Saffari 1392/12/14 7:25 
namespace Equipment.Controllers.Planning.Estimate
{
    [Authorize]
    [Developer("A.Saffari")]
    public partial class EstimateController : Controller
    {

        /// <summary>
        /// ESTIMETE HOME PAGE
        /// </summary>
        /// <returns></returns>
        [MenuAuthorize]
        public ActionResult EST_ESTIMATE()
        {
            FillViewBagNeededData();
            return View();
        }

        /// <summary>
        /// Estimates Grid DataSource Read Method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EST_ESTIMATE_Read([DataSourceRequest] DataSourceRequest request)
        {
            Session["EstEstimate-Grid-Filters"] = request;
            var data = AppraisalRepository.Get_EST_ESTIMATE()
                                          .ApplyEstEstimateFiltering(request.Filters)
                                          .OrderBy(o => o.EXP_OWENER_COMPANY.EOCO_DESC)
                                          .ApplyEstEstimateSorting(request.Groups, request.Sorts)
                //.AsEnumerable()
                                          .Select(b => new
                                          {
                                              b.ESMT_ID,
                                              b.EOCO_EOCO_ID,
                                              b.FINY_FINY_YEAR,
                                              b.ESMT_DIMAND
                                          })
                                          .ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// add or edit Estimate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddEstimate(int? id)
        {
            try
            {
                if (id == null)
                {
                    FillViewBagNeededData();
                    return View().IfUserCanAccess(HttpContext);
                }
                else
                {
                    FillViewBagNeededData();
                    return View(PublicRepository.cntx.EST_ESTIMATE.Find(id)).IfUserCanAccess(HttpContext);
                }
            }
            catch (Exception ex)
            {
                HttpContext.AddError(ex);
                return View();
            }
        }

        // <summary>
        // ثبت دیماند قراردادی مشترک
        // </summary>
        [HttpPost]
        public ActionResult SaveNewEstimate(EST_ESTIMATE obj)
        {
            try
            {
                if (obj.FINY_FINY_YEAR == null || obj.EOCO_EOCO_ID == null || obj.ESMT_DIMAND == 0 || obj.ESMT_DIMAND == null)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات را کامل وارد کنید" }.ToJson();
                }
                var owner = PublicRepository.cntx.EXP_OWENER_COMPANY.Find(obj.EOCO_EOCO_ID).EOCO_DESC;
                if (PublicRepository.ExistModel("EST_ESTIMATE", "EOCO_EOCO_ID={0} AND FINY_FINY_YEAR='{1}'", obj.EOCO_EOCO_ID, obj.FINY_FINY_YEAR))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("دیماند قراردادی سال[{0}] برای [{1}] قبلا ثبت شده است.", obj.FINY_FINY_YEAR, owner) }.ToJson();
                }
                obj.SaveToDataBase();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("دیماند قراردادی سال[{0}] برای [{1}] ثبت شد.", obj.FINY_FINY_YEAR, owner) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}]", ex.PersianMessage()) }.ToJson();
            }
        }

        // <summary>
        //  ویرایش دیماند قراردادی مشترک 
        // </summary>
        // <param name="obj"></param>
        // <returns></returns>
        public ActionResult UpdateEstimate(EST_ESTIMATE obj)
        {
            try
            {
                if (obj == null)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "ویرایش انجام نشد" }.ToJson();
                }

                if (PublicRepository.ExistModel("EST_ESTIMATE", "EOCO_EOCO_ID={0} AND FINY_FINY_YEAR='{1}' AND ESMT_ID <> {2}", obj.EOCO_EOCO_ID, obj.FINY_FINY_YEAR, obj.ESMT_ID))
                {
                    var owner = PublicRepository.cntx.EXP_OWENER_COMPANY.Find(obj.EOCO_EOCO_ID).EOCO_DESC;
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("دیماند قراردادی سال[{0}] برای [{1}] قبلا ثبت شده است.", obj.FINY_FINY_YEAR, owner) }.ToJson();
                }

                using (var cntx = PublicRepository.GetNewDatabaseContext)
                {
                    var original = cntx.EST_ESTIMATE.Find(obj.ESMT_ID);
                    if (original != null)
                    {
                        original.EXP_OWENER_COMPANY = obj.EXP_OWENER_COMPANY;
                        original.FINY_FINY_YEAR = obj.FINY_FINY_YEAR;
                        original.ESMT_DIMAND = obj.ESMT_DIMAND;
                        cntx.SaveChanges();
                        return new ServerMessages(ServerOprationType.Success) { Message = "ویرایش شد" }.ToJson();
                    }
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "ویرایش انجام نشد" }.ToJson();
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}] ویرایش نشد", ex.PersianMessage()) }.ToJson();
            }
        }

        private void FillViewBagNeededData()
        {
            ViewBag.OwnerCompany = AppraisalRepository.Get_EXP_OWNER_COMPANY().Select(c => new { c.EOCO_ID, c.EOCO_DESC });
            ViewBag.FinanYear = PublicRepository.Get_BKP_FINANCIAL_YEAR().Select(c => new { FINY_YEAR = c.FINY_YEAR });
        }

    }

}

using Equipment.Codes.Security;
using Equipment.DAL;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Planning.Estimate
{
    [Authorize]
    public partial class EstimateController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MenuAuthorize]
        public ActionResult Operation()
        {
            FillNeedeViewBagDataForOperation();
            return View();
        }

        /// <summary>
        /// add or edit Operation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddOperation(int? id)
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
                    return View(PublicRepository.cntx.EST_OPERATION.Find(id)).IfUserCanAccess(HttpContext);
                }
            }
            catch (Exception ex)
            {
                HttpContext.AddError(ex);
                return View();
            }
        }

        [HttpPost]
        public ActionResult SaveNewOperation(EST_OPERATION obj)
        {
            try
            {
                if (PublicRepository.ExistModel("EST_OPERATION", "EOCO_EOCO_ID={0} AND FINY_FINY_YEAR='{1}' AND OPRN_PRIOD='{2}' AND OPRN_TYPE={3}", obj.EOCO_EOCO_ID, obj.FINY_FINY_YEAR, obj.OPRN_PRIOD, obj.OPRN_TYPE))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = " تکراری است." }.ToJson();
                }
                obj.SaveToDataBase();
                return new ServerMessages(ServerOprationType.Success) { Message = "ثبت شد" }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("ثیت نشد[{0}].", ex.PersianMessage()) }.ToJson();
            }
        }

        public ActionResult UpdateOperation(EST_OPERATION obj)
        {
            try
            {
                if (obj == null)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "ویرایش انجام نشد" }.ToJson();
                }

                if (PublicRepository.ExistModel("EST_OPERATION", "EOCO_EOCO_ID={0} AND FINY_FINY_YEAR='{1}' AND OPRN_PRIOD='{2}'  AND OPRN_ID<>{3} AND OPRN_TYPE={4}", obj.EOCO_EOCO_ID, obj.FINY_FINY_YEAR, obj.OPRN_PRIOD, obj.OPRN_ID, obj.OPRN_TYPE))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تکراری است." }.ToJson();
                }

                using (var cntx = PublicRepository.GetNewDatabaseContext)
                {
                    var original = cntx.EST_OPERATION.Find(obj.OPRN_ID);
                    if (original != null)
                    {
                        original.EOCO_EOCO_ID = obj.EOCO_EOCO_ID;
                        original.FINY_FINY_YEAR = obj.FINY_FINY_YEAR;
                        original.OPRN_PIC = obj.OPRN_PIC;
                        original.OPRN_ENERGI = obj.OPRN_ENERGI;
                        original.OPRN_PRIOD = obj.OPRN_PRIOD;
                        original.OPRN_TYPE = obj.OPRN_TYPE;
                        cntx.SaveChanges();
                        return new ServerMessages(ServerOprationType.Success) { Message = "ویرایش شد" }.ToJson();
                    }
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "ویرایش انجام نشد" }.ToJson();
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("ویرایش نشد[{0}].", ex.PersianMessage()) }.ToJson();
            }
        }

        [HttpPost]
        public ActionResult OperationRead([DataSourceRequest] DataSourceRequest request)
        {
            Session["EstOperation-Grid-Filters"] = request;
            var data = AppraisalRepository.GetEstOperation()
                                          .ApplyEstOperationFiltering(request.Filters)
                                          .ApplyEstOperationSorting(request.Groups, request.Sorts)
                                          .AsEnumerable()
                                          .Select(p => new
                                          {
                                              p.OPRN_ID,
                                              p.EOCO_EOCO_ID,
                                              p.FINY_FINY_YEAR,
                                              p.OPRN_PIC,
                                              p.OPRN_PRIOD,
                                              p.OPRN_ENERGI,
                                              p.OPRN_TYPE
                                          })
                                          .ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private void FillNeedeViewBagDataForOperation()
        {
            ViewBag.OwnerCompany = AppraisalRepository.Get_EXP_OWNER_COMPANY().AsEnumerable().Select(c => new { c.EOCO_ID, c.EOCO_DESC });
            ViewBag.FinanYear = PublicRepository.Get_BKP_FINANCIAL_YEAR().AsEnumerable().Select(c => new { c.FINY_YEAR });
            //ViewBag.EstEstmate = AppraisalRepository.Get_EST_ESTIMATE().AsEnumerable().Select(c => new { ESMT_ID = c.ESMT_ID, ESMT_DIMAND = c.ESMT_DIMAND.ToString() + "-" + c.FINY_FINY_YEAR });
        }
    }
}
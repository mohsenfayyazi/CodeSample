using Equipment.Codes.Security;
using Equipment.DAL;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Planning.Earth
{
    [Authorize]
    public partial class EarthController : Controller
    {
        //
        // GET: /EAR_CHECK_LIST_ROW_TMP/
        public ActionResult EAR_CHECK_LIST_ROW_TMP()
        {
            return View().IfUserCanAccess(HttpContext);
        }

        public ActionResult ECHT_ECHT_ID_Read()
        {
            try
            {
                var RetVal = from b in EarthRepository.Get_EAR_EARTH_CHK_TMP() select new { b.ECHT_ID, b.ECHT_DESC };
                return Json(RetVal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }
        }

        [HttpPost]
        public ActionResult EAR_CHECK_LIST_ROW_TMP_read([DataSourceRequest]
                                                        DataSourceRequest request,int? id)
        {
            var data = EarthRepository.Get_EAR_CHECK_LIST_ROW_TMP()
                                      .ApplyEAR_CHECK_LIST_ROW_TMPFiltering(request.Filters)
                                      .ApplyEAR_CHECK_LIST_ROW_TMPSorting(request.Groups, request.Sorts)
                                      .OrderBy(o => o.CHTR_ROW)
                                      .AsEnumerable()
                                      .Where(xx=>xx.ECHT_ECHT_ID==id)
                                      .Select(b => new
                                      {
                                          b.CHTR_ID,
                                          b.CHTR_ROW,
                                          b.CHTR_DESC,
                                          b.CHTR_WEIGHT,
                                          b.ECHT_ECHT_ID,
                                          //customFieldPrsn = string.Format("{0} {1}\n({2})", b.PAY_PERSONEL.FIRS_NAME, b.PAY_PERSONEL.FAML_NAME, b.PAY_ORGAN.ORGA_DESC)
                                          customFieldPrsn = (b.PAY_PERSONEL.FIRS_NAME != null ? b.PAY_PERSONEL.FIRS_NAME : b.PAY_PERSONEL.FIRS_NAME) + " " + (b.PAY_PERSONEL.FAML_NAME != null ? b.PAY_PERSONEL.FAML_NAME : b.PAY_PERSONEL.FAML_NAME)

                                      })
                                      .ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EAR_CHECK_LIST_ROW_TMP_Add(EAR_CHECK_LIST_ROW_TMP NewObject)
        {
            if (!cntx.EAR_CHECK_LIST_ROW_TMP.Where(xx => xx.ECHT_ECHT_ID == NewObject.ECHT_ECHT_ID && xx.CHTR_DESC == NewObject.CHTR_DESC).Any())
            {
                var organe = NewObject.ORGA_CODE.Split('|');
                NewObject.ORGA_CODE = organe[0].Trim();
                NewObject.ORGA_MANA_ASTA_CODE = organe[1].Trim();
                NewObject.ORGA_MANA_CODE = organe[2].Trim();
                NewObject.SaveToDataBase();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();

            }
            else {
                return new ServerMessages(ServerOprationType.Failure) { Message = string.Format("اطلاعات  وارد شده تکراری می باشد ") }.ToJson();
            }
            
            
               
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EAR_CHECK_LIST_ROW_TMP_Update([DataSourceRequest]
                                                          DataSourceRequest request, [Bind(Prefix = "models")]
                                                          IEnumerable<EAR_CHECK_LIST_ROW_TMP> earCheckListRowTMPs)
        {
            if (earCheckListRowTMPs != null)
            {
                using (BandarEntities cntx = new BandarEntities())
                {
                    foreach (EAR_CHECK_LIST_ROW_TMP earCheckListRowTm in earCheckListRowTMPs)
                    {
                        EAR_CHECK_LIST_ROW_TMP row = cntx.EAR_CHECK_LIST_ROW_TMP.Where(b => b.CHTR_ID == earCheckListRowTm.CHTR_ID).FirstOrDefault();
                        row.CHTR_DESC = earCheckListRowTm.CHTR_DESC;
                        row.CHTR_ROW = earCheckListRowTm.CHTR_ROW;
                        row.CHTR_WEIGHT = earCheckListRowTm.CHTR_WEIGHT;
                    }
                    try
                    {
                        cntx.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return Json(earCheckListRowTMPs.ToDataSourceResult(request, ModelState));
        }
        
        [HttpPost]
        public ActionResult delete_EAR_CHECK_LIST_ROW_TMP(int TargetId)
        {
            try
            {
                var itemTodelete = new EAR_CHECK_LIST_ROW_TMP();
                itemTodelete.CHTR_ID = TargetId;
                itemTodelete.DeleteFromDataBase();
            }
            catch
            {
            }
            return Json(new { Success = true });
        }

        [HttpGet]
        public ActionResult GetTotalInfo(int? TargetId)
        {
            if (TargetId != null)
            {
                var records = EarthRepository.Get_EAR_CHECK_LIST_ROW_TMP().Where(x => x.ECHT_ECHT_ID == TargetId);
                var totalCount = records.Count();

                int newRowNumber = 1;
                if (records.Any())
                {
                    newRowNumber = Convert.ToInt32(records.Max(x => x.CHTR_ROW) + 1);
                }

                return Json(new { NewRowNumber = newRowNumber, TotalRowsCount = totalCount }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { NewRowNumber = 0, TotalRowsCount = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}


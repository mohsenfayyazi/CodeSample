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
    public partial class EarthController
    {
        //
        // GET: /EAR_CHECK_LIST_ROW/
        public ActionResult EAR_CHECK_LIST_ROW()
        {
            return View();
        }

        //GetPartial_EAR_CHECK_LIST_ROW
        public ActionResult Get_EarthList()
        {
            return Json((from b in EarthRepository.Get_EAR_EARTH() select new { b.ERTH_ID, b.ERTH_NAME }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEarths()
        {
            var retVal = from b in EarthRepository.Get_EAR_EARTH() orderby b.ERTH_NAME select new { b.ERTH_ID, b.ERTH_NAME, b.ERTH_ADDRESS, b.ERTH_OWNERSHIP };
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

    
        [HttpPost]
        public ActionResult EAR_CHECK_LIST_ROW_read([DataSourceRequest] DataSourceRequest request)
        {
            if (Session["89765EQ_CurrentEARCHKLST"] != null)
            {
                int id = Convert.ToInt32(Session["89765EQ_CurrentEARCHKLST"]);
                var data = EarthRepository.Get_EAR_CHECK_LIST_ROW()
                                          .Where(w => w.ECHL_ECHL_ID == id)
                                          .OrderBy(o => o.CHLR_ROW)
                                          .ApplyEAR_CHECK_LIST_ROWFiltering(request.Filters)
                                          .ApplyEAR_CHECK_LIST_ROWSorting(request.Groups, request.Sorts)
                                          .AsEnumerable()
                                          .Select(b => new
                                          {
                                              b.CHLR_ID,
                                              b.CHLR_ROW,
                                              b.CHLR_DESC,
                                              b.CHLR_WEIGHT,
                                              b.ECHL_ECHL_ID,
                                              coustomFieldPrsn = string.Format("{0} {1}", b.PAY_PERSONEL.FIRS_NAME, b.PAY_PERSONEL.FAML_NAME),
                                              b.CHLR_VALUE
                                          })
                                          .ToDataSourceResult(request);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false });
        }

        [HttpPost]
        public ActionResult EAR_CHECK_LIST_ROW_read2([DataSourceRequest] DataSourceRequest request, int id)
        {
            var data = EarthRepository.Get_EAR_CHECK_LIST_ROW()
                                      .Where(w => w.ECHL_ECHL_ID == id)
                                      .OrderBy(o => o.CHLR_ROW)
                                      .ApplyEAR_CHECK_LIST_ROWFiltering(request.Filters)
                                      .ApplyEAR_CHECK_LIST_ROWSorting(request.Groups, request.Sorts)
                                      .AsEnumerable()
                                      .Select(b => new
                                      {
                                          b.CHLR_ID,
                                          b.CHLR_ROW,
                                          b.CHLR_DESC,
                                          b.CHLR_WEIGHT,
                                          b.ECHL_ECHL_ID,
                                          coustomFieldPrsn = string.Format("{0} {1}", b.PAY_PERSONEL.FIRS_NAME, b.PAY_PERSONEL.FAML_NAME),
                                          b.CHLR_VALUE
                                      })
                                      .ToDataSourceResult(request);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EAR_CHECK_LIST_ROW_Update([DataSourceRequest]
                                                      DataSourceRequest request, [Bind(Prefix = "models")]
                                                      IEnumerable<EAR_CHECK_LIST_ROW> earCheckListRows)
        {
            var earCheckListRowTms = earCheckListRows as EAR_CHECK_LIST_ROW[] ?? earCheckListRows.ToArray();
            if (earCheckListRows != null)
            {
                using (BandarEntities cntx = new BandarEntities())
                {
                    try
                    {
                        foreach (EAR_CHECK_LIST_ROW earCheckListRowTm in earCheckListRowTms)
                        {
                            EAR_CHECK_LIST_ROW row = cntx.EAR_CHECK_LIST_ROW.Find(earCheckListRowTm.CHLR_ID);
                            row.CHLR_DESC = earCheckListRowTm.CHLR_DESC;
                            row.CHLR_ROW = earCheckListRowTm.CHLR_ROW;
                            row.CHLR_WEIGHT = earCheckListRowTm.CHLR_WEIGHT;
                            if (row.CHLR_VALUE == 0)
                            {
                                row.CHLR_VALUE = earCheckListRowTm.CHLR_VALUE;
                            }
                        }

                        cntx.SaveChanges();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return Json(earCheckListRowTms.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult delete_EAR_CHECK_LIST_ROW(int targetId)
        {
            try
            {
                var itemTodelete = new EAR_CHECK_LIST_ROW();
                itemTodelete.CHLR_ID = targetId;
                itemTodelete.DeleteFromDataBase();
            }
            catch
            {
            }
            return Json(new { Success = true });
        }

        [HttpPost]
        public ActionResult addnewchecklistrow(int checklistid, string newrow, string newdesc, short newvazn, string orgaCode, short prsnEmpNumb)
        {
            try
            {
                if (PublicRepository.ExistModel("EAR_CHECK_LIST_ROW", "ECHL_ECHL_ID={0} AND CHLR_DESC='{1}'", checklistid, newdesc.Trim()))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}] تکراری است.", newdesc) }.ToJson();
                }
                var item = new EAR_CHECK_LIST_ROW()
                {
                    ECHL_ECHL_ID = checklistid,
                    CHLR_ROW = newrow,
                    CHLR_DESC = newdesc,
                    CHLR_WEIGHT = newvazn,
                    PRSN_EMP_NUMB = prsnEmpNumb,
                    CHLR_VALUE = 0
                };
                item.SaveToDataBase();

                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", newdesc) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}] ثبت نشد({1})", newdesc, ex.PersianMessage()) }.ToJson();
            }
        }

    }

}

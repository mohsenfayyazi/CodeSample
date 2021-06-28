using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data;
using System.Linq;

using System.Web.Mvc;

namespace Equipment.Controllers.Worksheet
{
    [Authorize]
    public class WorksheetController : DbController
    {
        string username = string.Empty;

        //سازنده کلاس

        public WorksheetController()
            : base()
        {

            username = this.UserInfo().Username;
        }



        /*اولین  اکشن در این کنتولر که برای باز شدن فرم برقدار شدن  استفاده شده است */
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Item(int? id)
        {
            ViewData["WOSH_ID"] = id;
            return View();
        }
        public ActionResult Instrument(int? id)
        {
            ViewData["WOSH_ID"] = id;
            return View();
        }

        public ActionResult AddWorksheet(int? id)
        {
            ViewData["EEDO_ID"] = id;
            return View();
        }
        public string Ajax_GetInstru(int? WOSH_WOSH_ID)
        {

            string date = Db.Database.SqlQuery<string>(string.Format("SELECT b.eins_desc||','||c.eerr_desc from exp_worksheet_instru a,exp_instrument b,exp_error_inst c " +
            " where a.wosh_wosh_id = {0} and a.eins_eins_id = b.eins_id and a.eerr_eerr_id = c.eerr_id ", WOSH_WOSH_ID)).FirstOrDefault();

            return (date);
        }
        public ActionResult DropDownWorksheet(int? WOSH_TYPE)
        {

            var RetVal = (from b in Db.EXP_WORKSHEET
                          where b.WOSH_TYPE == WOSH_TYPE
                          orderby b.WOSH_TITL
                          select new
                          {
                              b.WOSH_ID,
                              b.WOSH_TITL
                          }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DropDownItem(int? WOSH_ID, int? Type)
        {

            var RetVal = (from b in Db.EXP_WORKSHEET_ITEM
                          where b.WOSH_WOSH_ID == WOSH_ID && b.WOIT_TYPE == Type
                          orderby b.WOIT_TITL
                          select new
                          {
                              b.WOIT_ID,
                              b.WOIT_TITL
                          }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DropDownFunction(int? EPRO_EPRO_ID)
        {

            var RetVal = (from b in Db.EXP_PFUNCTION

                          where b.EPRO_EPRO_ID == EPRO_EPRO_ID && b.ETIP_ETIP_ID == 1

                          select new
                          {
                              b.EFUN_ID,
                              b.EFUN_DESC
                          }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DropDownInstru(int? WOSH_ID)
        {

            var RetVal = (from b in Db.EXP_WORKSHEET_INSTRU
                          join c in Db.EXP_INSTRUMENT on b.EINS_EINS_ID equals c.EINS_ID
                          where b.WOSH_WOSH_ID == WOSH_ID

                          select new
                          {
                              b.WOIN_ID,
                              EINS_DESC = c.EINS_DESC
                          }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DropDownSubInstru(int? WOSH_ID, int? WOIN_ID)
        {
            int? EINS_ID = Db.EXP_WORKSHEET_INSTRU.Where(xx => xx.WOIN_ID == WOIN_ID).Select(xx => xx.EINS_EINS_ID).FirstOrDefault();

            var RetVal = (from b in Db.EXP_WORKSHEET_INSTRU
                          join c in Db.EXP_ERROR_INST on b.EERR_EERR_ID equals c.EERR_ID
                          where b.WOSH_WOSH_ID == WOSH_ID && b.EINS_EINS_ID == EINS_ID

                          select new
                          {
                              b.WOIN_ID,
                              EINS_DESC = c.EERR_DESC
                          }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DropDownInstrumentDetails(int? EINS_ID)
        {

            var RetVal = (from b in Db.EXP_ERROR_INST
                          where (b.EINS_EINS_ID == EINS_ID && b.EERR_TYPE == 0)
                          orderby b.EERR_DESC
                          select new
                          {
                              b.EERR_DESC,
                              b.EERR_ID
                          }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GridWorksheet([DataSourceRequest] DataSourceRequest request)
        {
            var Query = (from a in Db.EXP_WORKSHEET

                         orderby a.WOSH_TITL descending
                         select new
                         {

                             a.WOSH_ID,
                             a.WOSH_TITL,
                             a.WOSH_DESC,
                             a.WOSH_DATE,
                             a.WOSH_VERS

                         }).ToList().Distinct();


            return Json(Query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public string GetParentItemTitle(int? WOIT_WOIT_ID)
        {
            if (WOIT_WOIT_ID == null)
            {
                WOIT_WOIT_ID = 0;
            }
            string sql = string.Format("SELECT nvl(WOIT_TITL,'') from EXP_WORKSHEET_ITEM where WOIT_ID={0}", WOIT_WOIT_ID);
            string RetVal = Db.Database.SqlQuery<string>(sql).FirstOrDefault();
            return RetVal;
        }

        public string GetInstrumentDesc(int? EINS_EINS_ID)
        {
            if (EINS_EINS_ID == null)
            {
                EINS_EINS_ID = 0;
            }
            string sql = string.Format("SELECT nvl(EINS_DESC,'') from exp_instrument where EINS_ID={0}", EINS_EINS_ID);
            string RetVal = Db.Database.SqlQuery<string>(sql).FirstOrDefault();
            return RetVal;
        }

        public ActionResult GridItem([DataSourceRequest] DataSourceRequest request, int? WOSH_ID)
        {
            var Query = (from a in Db.EXP_WORKSHEET_ITEM
                         where a.WOSH_WOSH_ID == WOSH_ID
                         orderby a.WOIT_TITL descending
                         select new
                         {

                             a.WOIT_ID,
                             a.WOIT_TITL,
                             a.WOIT_DESC,
                             a.WOIT_TYPE,
                             a.WOIT_WOIT_ID

                         }).ToList().Distinct();
            var RetQuery = Query.Select(a => new
            {
                a.WOIT_ID,
                a.WOIT_TITL,
                a.WOIT_DESC,
                a.WOIT_TYPE,
                PrentItemTitle = GetParentItemTitle(a.WOIT_WOIT_ID)

            }).ToList().OrderBy(xx => xx.WOIT_TITL);

            return Json(RetQuery.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GridItemExpi([DataSourceRequest] DataSourceRequest request, int? EEDO_ID)
        {
            var Query = (from a in Db.EXP_WORKSHEET_ITEM
                         join b in Db.EXP_WORKSHEET_ITEM_EXPI on a.WOIT_ID equals b.WOIT_WOIT_ID
                         where b.EEDO_EEDO_ID == EEDO_ID
                         orderby a.WOIT_TITL descending
                         select new
                         {

                             a.WOIT_ID,
                             a.WOIT_TITL,
                             a.WOIT_DESC,
                             a.WOIT_TYPE,
                             a.WOIT_WOIT_ID,
                             b.WSIE_ID

                         }).ToList().Distinct();
            var RetQuery = Query.Select(a => new
            {
                a.WOIT_ID,
                a.WOIT_TITL,
                a.WOIT_DESC,
                a.WOIT_TYPE,
                PrentItemTitle = GetParentItemTitle(a.WOIT_WOIT_ID),
                a.WSIE_ID


            }).ToList().OrderBy(xx => xx.WOIT_TITL);

            return Json(RetQuery.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GridInstrument([DataSourceRequest] DataSourceRequest request, int? WOSH_ID)
        {
            var Query = (from a in Db.EXP_WORKSHEET_INSTRU
                         where a.WOSH_WOSH_ID == WOSH_ID
                         orderby a.EXP_ERROR_INST.EERR_DESC descending
                         select new
                         {

                             a.WOIN_ID,
                             a.EXP_ERROR_INST.EERR_DESC,
                             a.EINS_EINS_ID,


                         }).ToList().Distinct();
            var RetQuery = Query.Select(a => new
            {
                a.WOIN_ID,
                a.EERR_DESC,
                EINS_DESC = GetInstrumentDesc(a.EINS_EINS_ID)

            }).ToList().OrderBy(xx => xx.EERR_DESC);

            return Json(RetQuery.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public ActionResult InsertWorksheet(EXP_WORKSHEET objecttemp)
        {

            if (Db.EXP_WORKSHEET.Where(xx => xx.WOSH_TITL == objecttemp.WOSH_TITL).Any())
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format(" عنوان وارد شده تکراری میباشد") }.ToJson();
            }

            try
            {
                Db.EXP_WORKSHEET.Add(objecttemp);
                Db.SaveChanges();

                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات : " + ex.ToString()) }.ToJson();
            }

            //return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
        }

        public ActionResult InsertWorksheetItem(EXP_WORKSHEET_ITEM objecttemp)
        {

            //int id = Db.PLN_SIGNIFIC_CONFILICT.Select(xx => xx.SICO_ID).FirstOrDefault();
            if (Db.EXP_WORKSHEET_ITEM.Where(xx => xx.WOIT_TITL == objecttemp.WOIT_TITL && xx.WOSH_WOSH_ID == objecttemp.WOSH_WOSH_ID).Any())
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format(" عنوان وارد شده تکراری میباشد") }.ToJson();
            }

            try
            {
                Db.EXP_WORKSHEET_ITEM.Add(objecttemp);
                Db.SaveChanges();

                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات : " + ex.ToString()) }.ToJson();
            }

            //return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
        }

        public ActionResult InsertWorksheetItemExpi(EXP_WORKSHEET_ITEM_EXPI objecttemp)
        {
            for (int i = 0; i < 8; i++)
            {
                if (!string.IsNullOrEmpty(Request.Form["WOIT_WOIT_ID_" + i]))
                {
                    objecttemp.WOIT_WOIT_ID = int.Parse(Request.Form["WOIT_WOIT_ID_" + i]);

                    //int id = Db.PLN_SIGNIFIC_CONFILICT.Select(xx => xx.SICO_ID).FirstOrDefault();
                    //if (Db.EXP_WORKSHEET_ITEM_EXPI.Where(xx => xx.EEDO_EEDO_ID == objecttemp.EEDO_EEDO_ID && xx.WOIT_WOIT_ID == objecttemp.WOIT_WOIT_ID).Any())
                    //{
                    // return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format(" عنوان وارد شده تکراری میباشد") }.ToJson();
                    //}

                    try
                    {
                        Db.EXP_WORKSHEET_ITEM_EXPI.Add(objecttemp);
                        Db.SaveChanges();


                    }
                    catch (Exception ex)
                    {
                        // return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات : " + ex.ToString()) }.ToJson();
                    }
                }
            }
            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            //return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
        }

        public ActionResult InsertWorksheetInstrument(EXP_WORKSHEET_INSTRU objecttemp)
        {

            try
            {
                Db.EXP_WORKSHEET_INSTRU.Add(objecttemp);
                Db.SaveChanges();

                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات : " + ex.ToString()) }.ToJson();
            }


        }



    }

}
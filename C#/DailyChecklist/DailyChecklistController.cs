using Equipment.DAL;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Exploitation.DailyChecklist
{
    public class DailyChecklistController : DbController
    {
        // BandarEntities Db;

        PersianCalendar pc = new PersianCalendar();
        DateTime thisDate = DateTime.Now;
        int? userid = 0; string username = string.Empty;

        ///////////////////////////////////////////////////////////////////VIEW
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //        Db.Dispose();
        //    base.Dispose(disposing);
        //}
        //public DailyChecklistController()
        //    : base()
        //{


        //    Db = this.DB();
        //    userid = this.UserInfo().UserId;


        //}
        //
        // GET: /DailyChecklist/
        [HttpPost]
        public string Ajax_get_defect(decimal? EPIU_EPIU_ID)
        {
            string doc_numb = "";
            var query = from b in Db.EXP_EDOC_INSTRU
                        where b.EPIU_EPIU_ID == EPIU_EPIU_ID
                        select b;

            //cntx.EXP_EDOC_INSTRU.Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID)
            //.Select(xx => xx.EEDO_EEDO_ID);
            foreach (var item in query)
            {
                doc_numb = item.EEDO_EEDO_ID.ToString() + "-" + doc_numb;
            }

            return doc_numb;
        }

        public short? Ajax_Get_Score(int? CHTI_CHTI_ID)
        {
            return Db.CHK_ITEM_TEMPLATE.Where(xx => xx.CHTI_ID == CHTI_CHTI_ID).Select(xx => xx.CHTI_VALU).FirstOrDefault();
        }

        public ActionResult EXP_DAILY_CHECKLIST_USER_Insert(int? id)
        {
            ViewData["EDCH_ID"] = id;
            return View();
        }

        public ActionResult EXP_DAILY_CHECKLIST_HEAD(int? id)
        {
            ViewData["EDCH_TYPE"] = id;
            return View();
        }

        public ActionResult EXP_DAILY_CHECKLIST_VAR(int? id)
        {
            ViewData["EDCH_ID"] = id;
            return View();
        }
        public ActionResult EXP_DAILY_CHECKLIST_VAR_LIST(int? id)
        {
            ViewData["EDCH_ID"] = id;
            return View();
        }

        public ActionResult EXP_MONTHLY_CHECKLIST_VAR(int? id)
        {
            ViewData["EDCH_ID"] = id;
            return View();
        }

        public ActionResult EXP_SCORING_CHECKLIST_VAR(int? id)
        {
            ViewData["EDCH_ID"] = id;
            return View();
        }

        public ActionResult EXP_Yearly_CHECKLIST_VAR(int? id)
        {
            ViewData["EDCH_ID"] = id;
            return View();
        }

        public ActionResult EXP_DAILY_CHECKLIST_USER(int? id)
        {
            ViewData["EEDO_ID"] = id;
            return View();
        }

        public ActionResult EXP_ITEM_DEFECT(int? id, int? EDCH_ID)
        {
            ViewData["EEDO_ID"] = id;
            ViewData["EDCH_ID"] = EDCH_ID;
            return View();
        }

        public ActionResult insert_daily_checklist_head(EXP_DAILY_CHECKLIST_HEAD objecttemp)
        {
            var q = from b in Db.EXP_DAILY_CHECKLIST_HEAD where b.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID && b.EDCH_DATE == objecttemp.EDCH_DATE && b.EDCH_TYPE == objecttemp.EDCH_TYPE select b;
            if (!q.Any())
            {
                //objecttemp.EDCH_TYPE = 0;
                Db.EXP_DAILY_CHECKLIST_HEAD.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            else
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("اطلاعات ثبت شده تکراری می باشد ") }.ToJson();
            }
        }

        public ActionResult insert_daily_checklist_user(EXP_DAILY_CHECKLIST_USER objecttemp)
        {
            try
            {
                Db.EXP_DAILY_CHECKLIST_USER.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("اطلاعات ثبت شده تکراری می باشد ") }.ToJson();
            }
        }

        public ActionResult Insert_Post_Score(EXP_DAILY_CHECKLIST_VAR objecttemp)
        {
            int CHLT_ID = int.Parse(Request.Form["CHLT_ID"]);
            var q = from b in Db.EXP_DAILY_CHECKLIST_VAR
                    where b.EDCH_EDCH_ID == objecttemp.EDCH_EDCH_ID &&
                    b.CHTI_CHTI_ID == objecttemp.CHTI_CHTI_ID
                    select b;

            if (!q.Any())
            {
                if (CHLT_ID != 582 && Db.EXP_DAILY_CHECKLIST_VAR.Where(xx => xx.CHK_ITEM_TEMPLATE.CHLT_CHLT_ID == CHLT_ID && xx.EDCH_EDCH_ID == objecttemp.EDCH_EDCH_ID).Any())
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("اطلاعات ثبت شده تکراری می باشد ") }.ToJson();
                }
                else if (CHLT_ID == 582 && Db.EXP_DAILY_CHECKLIST_VAR.Where(xx => xx.CHK_ITEM_TEMPLATE.CHLT_CHLT_ID == CHLT_ID && xx.EDCH_EDCH_ID == objecttemp.EDCH_EDCH_ID).Count() > 2)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("اطلاعات ثبت شده بیشتر از حد مجاز می باشد ") }.ToJson();
                }
                else
                {
                    Db.EXP_DAILY_CHECKLIST_VAR.Add(objecttemp);
                    Db.SaveChanges();
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
                }
            }
            else
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("اطلاعات ثبت شده تکراری می باشد ") }.ToJson();
            }
        }

        public ActionResult insert_daily_checklist_var(EXP_DAILY_CHECKLIST_VAR objecttemp)
        {
            var q = from b in Db.EXP_DAILY_CHECKLIST_VAR
                    where b.EDCH_EDCH_ID == objecttemp.EDCH_EDCH_ID && b.CHTI_CHTI_ID == objecttemp.CHTI_CHTI_ID && b.SCSU_ROW_NO == objecttemp.SCSU_ROW_NO
                    select b;

            if (!string.IsNullOrEmpty(Request.Form["POSTINSTTT"]))
            {
                objecttemp.EPIU_EPIU_ID = decimal.Parse(Request.Form["POSTINSTTT"]);
                q = from b in Db.EXP_DAILY_CHECKLIST_VAR
                    where b.EDCH_EDCH_ID == objecttemp.EDCH_EDCH_ID && b.CHTI_CHTI_ID == objecttemp.CHTI_CHTI_ID && b.EPIU_EPIU_ID == objecttemp.EPIU_EPIU_ID
                    select b;
            }

            if (objecttemp.SCSU_ROW_NO == null)
            {
                q = from b in Db.EXP_DAILY_CHECKLIST_VAR
                    where b.EDCH_EDCH_ID == objecttemp.EDCH_EDCH_ID && b.CHTI_CHTI_ID == objecttemp.CHTI_CHTI_ID && b.EPIU_EPIU_ID == objecttemp.EPIU_EPIU_ID
                    select b;
            }

            if (!q.Any())
            {
                if (q.Select(xx => xx.EXP_DAILY_CHECKLIST_HEAD.EDCH_TYPE).FirstOrDefault() == 2)
                    objecttemp.EPIU_EPIU_ID = null;
                // objecttemp.EPIU_EPIU_ID = Convert.ToDecimal(Request.Form["POSTINSTTT"]);
                Db.EXP_DAILY_CHECKLIST_VAR.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            else
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("اطلاعات ثبت شده تکراری می باشد ") }.ToJson();
            }
        }

        public ActionResult GetChecklist([DataSourceRequest] DataSourceRequest request, int? EEDO_ID, int EDCH_TYPE)
        {
            //int CHLT_TYPE = EDCH_TYPE == 1 ? 1 : 0;
            var query = (from a in Db.EXP_CHECKLIST_EXPI
                         where (
                         a.EEDO_EEDO_ID == EEDO_ID &&
                         a.EXP_DAILY_CHECKLIST_VAR.CHK_ITEM_TEMPLATE.CHK_CHECK_LIST_TEMPLATE.CHLT_TYPE == EDCH_TYPE &&
                         (a.EXP_DAILY_CHECKLIST_VAR.CHK_ITEM_TEMPLATE.CHTI_STATUS == 1 && a.EXP_DAILY_CHECKLIST_VAR.CHK_ITEM_TEMPLATE.CHK_CHECK_LIST_TEMPLATE.CHLT_STAT == 1)
                         // (!b.EXP_CHECKLIST_EXPI.Where(xx=>xx.EXP_DAILY_CHECKLIST_VAR.EXP_DAILY_CHECKLIST_HEAD.EDCH_TYPE==EDCH_TYPE).Any() || 
                         //  d.EXP_DAILY_CHECKLIST_VAR.CHK_ITEM_TEMPLATE.CHK_CHECK_LIST_TEMPLATE.CHLT_TYPE == EDCH_TYPE) &&
                         //&& d.EXP_DAILY_CHECKLIST_VAR.CHK_ITEM_TEMPLATE.CHK_CHECK_LIST_TEMPLATE.CHLT_STAT == 1                          &&
                         )
                         // && userquery.Contains(a.RECIPIENT_ROLE                       )
                         select new
                         {
                             CODE_NAME = a.EXP_DAILY_CHECKLIST_VAR.EXP_POST_LINE_INSTRU.CODE_NAME,
                             CHTI_CHTI_ID = a.EXP_DAILY_CHECKLIST_VAR.CHTI_CHTI_ID,
                             Item = a.EXP_DAILY_CHECKLIST_VAR.CHK_ITEM_TEMPLATE.CHTI_DESC,
                             Form = a.EXP_DAILY_CHECKLIST_VAR.CHK_ITEM_TEMPLATE.CHK_CHECK_LIST_TEMPLATE.CHLT_TITL,
                             EPIU_ID = a.EXP_DAILY_CHECKLIST_VAR.EPIU_EPIU_ID,
                             CHTI_STATUS = a.EXP_DAILY_CHECKLIST_VAR.CHK_ITEM_TEMPLATE.CHTI_STATUS,
                             CHLT_CHLT_STAT = a.EXP_DAILY_CHECKLIST_VAR.CHK_ITEM_TEMPLATE.CHK_CHECK_LIST_TEMPLATE.CHLT_STAT,
                             HEAD_EDCH_TYPE = a.EXP_DAILY_CHECKLIST_VAR.EXP_DAILY_CHECKLIST_HEAD.EDCH_TYPE,
                             CHTI_ID = a.EXP_DAILY_CHECKLIST_VAR.CHK_ITEM_TEMPLATE.CHK_CHECK_LIST_TEMPLATE.CHLT_ID,
                             EDCH_TYPE = a.EXP_DAILY_CHECKLIST_VAR.EXP_DAILY_CHECKLIST_HEAD.EDCH_TYPE
                         }).ToList().Distinct();

            var query2 = query
                              //.Where(xx => (xx.CHTI_STATUS == 1 || xx.CHTI_CHTI_ID == null) && (xx.CHLT_STAT == 1 || xx.CHTI_CHTI_ID == null))
                              .Select(a => new
                              {
                                  a.CODE_NAME,
                                  Item = a.Item,//GetItemDesc(a.CHTI_CHTI_ID),
                                  Form = a.Form,//GetFormDesc(a.CHTI_CHTI_ID)
                                  a.EPIU_ID,
                                  a.CHTI_STATUS,
                                  a.CHLT_CHLT_STAT,
                                  a.HEAD_EDCH_TYPE,
                                  a.CHTI_ID,
                                  a.CHTI_CHTI_ID,
                                  a.EDCH_TYPE

                              }).ToList().Distinct();

            return Json(query2.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult insert_Item_Defect(EXP_DAILY_CHECKLIST_VAR objecttemp)
        {
            var objecttempitem = new EXP_CHECKLIST_EXPI();
            objecttemp.EPIU_EPIU_ID = decimal.Parse(Request.Form["POSTINSTTT"]);
            decimal? EEDO_ID = decimal.Parse(Request.Form["EEDO_ID"]);

            var q = from b in Db.EXP_DAILY_CHECKLIST_VAR
                    join c in Db.EXP_CHECKLIST_EXPI on b.EDCV_ID equals c.EDCV_EDCV_ID
                    where b.EDCH_EDCH_ID == objecttemp.EDCH_EDCH_ID && b.CHTI_CHTI_ID == objecttemp.CHTI_CHTI_ID && b.EPIU_EPIU_ID == objecttemp.EPIU_EPIU_ID
                   && c.EEDO_EEDO_ID == EEDO_ID
                    select b;

            if (!q.Any())
            {
                objecttemp.EDCV_VAR = "Abnormal";
                objecttemp.EPIU_EPIU_ID = Convert.ToDecimal(Request.Form["POSTINSTTT"]);
                Db.EXP_DAILY_CHECKLIST_VAR.Add(objecttemp);
                Db.SaveChanges();
                objecttempitem.EEDO_EEDO_ID = EEDO_ID;
                objecttempitem.EDCV_EDCV_ID = objecttemp.EDCV_ID;
                Db.EXP_CHECKLIST_EXPI.Add(objecttempitem);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            else
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("اطلاعات ثبت شده تکراری می باشد ") }.ToJson();
            }
        }

        public ActionResult InsertItem(decimal? EEDO_ID, decimal? EDCH_EDCH_ID, int? CHTI_CHTI_ID)
        {
            var objecttemp = new EXP_DAILY_CHECKLIST_VAR();
            var objecttempitem = new EXP_CHECKLIST_EXPI();
            objecttemp.EPIU_EPIU_ID = Db.EXP_EXPI_DOC.Where(xx => xx.EEDO_ID == EEDO_ID).Select(xx => xx.EXP_EDOC_INSTRU.Select(yy => yy.EPIU_EPIU_ID).FirstOrDefault()).FirstOrDefault();
            objecttemp.CHTI_CHTI_ID = CHTI_CHTI_ID;// Db.EXP_CHECKLIST_EXPI.Where(xx => xx.EEDO_EEDO_ID == EEDO_ID).Select(xx => xx.EXP_DAILY_CHECKLIST_VAR.CHTI_CHTI_ID).FirstOrDefault();

            var q = from b in Db.EXP_DAILY_CHECKLIST_VAR
                    join c in Db.EXP_CHECKLIST_EXPI on b.EDCV_ID equals c.EDCV_EDCV_ID
                    where c.EEDO_EEDO_ID == EEDO_ID
                    select b;

            var q2 = from b in Db.EXP_DAILY_CHECKLIST_VAR
                     where b.EDCH_EDCH_ID == EDCH_EDCH_ID && b.CHTI_CHTI_ID == objecttemp.CHTI_CHTI_ID && b.EPIU_EPIU_ID == objecttemp.EPIU_EPIU_ID
                     select new { b.EDCV_ID };
            //var ccc=cntx.EXP_DAILY_CHECKLIST_VAR.Where(xx=>xx.EPIU_EPIU_ID==objecttemp.EPIU_EPIU_ID).Count();
            //int count = cntx.Database.SqlQuery<int>(
            //string.Format("select EDCV_ID from EXP_DAILY_CHECKLIST_VAR where EDCH_EDCH_ID='{0}' and CHTI_CHTI_ID={1} and EPIU_EPIU_ID={2}", objecttemp.EDCH_EDCH_ID, objecttemp.CHTI_CHTI_ID, objecttemp.EPIU_EPIU_ID)).FirstOrDefault();

            if (!q2.Any() && q.Any())
            {
                objecttemp.EDCH_EDCH_ID = EDCH_EDCH_ID;
                objecttemp.EDCV_VAR = "Abnormal";
                // objecttemp.EEDO_EEDO_ID = EEDO_ID;
                Db.EXP_DAILY_CHECKLIST_VAR.Add(objecttemp);
                Db.SaveChanges();
                objecttempitem.EEDO_EEDO_ID = EEDO_ID;
                objecttempitem.EDCV_EDCV_ID = objecttemp.EDCV_ID;
                Db.EXP_CHECKLIST_EXPI.Add(objecttempitem);
                Db.SaveChanges();

                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            else
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("اطلاعات ثبت شده تکراری می باشد یا برای این دیفکت هنوز آیتم بازدید ثبت نشده است ") }.ToJson();
            }
        }

        public int? CheckItem(decimal? EEDO_ID, decimal? EDCH_EDCH_ID)
        {
            //var Query=Db.EXP_CHECKLIST_EXPI.Where(xx=>xx.EEDO_EEDO_ID==EEDO_ID && xx.EXP_DAILY_CHECKLIST_VAR.EDCH_EDCH_ID==EDCH_EDCH_ID).Any()
            int? check = 0;
            var Query = from b in Db.EXP_CHECKLIST_EXPI
                            // join c in Db.EXP_DAILY_CHECKLIST_VAR on b.EDCV_EDCV_ID equals c.EDCV_ID
                        where b.EEDO_EEDO_ID == EEDO_ID //&& c.EDCH_EDCH_ID==EDCH_EDCH_ID
                        select new { b.CHEX_ID };

            if (Query.Any())
            {
                check = -1;
                //return new ServerMessages(ServerOprationType.Success) { Message = string.Format("آیتم ثبت شده است ") }.ToJson();
            }
            else
            {
                check = 0;
                //return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("اطلاعات ثبت نشده است ") }.ToJson();
            }

            return check;
        }

        public ActionResult Get_ChkTemplate(int? EINS_ID, int? EDCH_TYPE)
        {
            var RetVal = from b in PublicRepository.Get_ChkTemplate()
                         where (b.EINS_EINS_ID == EINS_ID || EINS_ID == null) && b.CHLT_STAT == 1 && (b.CHLT_TYPE == EDCH_TYPE)
                         orderby b.CHLT_TITL
                         select new { b.CHLT_ID, b.CHLT_TITL };
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_user(string text, int? EPOL_ID)
        {
            var RetVal = (from p in Db.SEC_USER_TYPE_POST
                          join c in Db.SEC_USERS on p.SCSU_ROW_NO equals c.ROW_NO
                          where p.EPOL_EPOL_ID == EPOL_ID && p.ETDO_ETDO_ID == 463 && p.EURP_TYPE == 2 && p.EURP_ACTV == 1
                          select new
                          {
                              c.ROW_NO,
                              FAML_NAME = c.FAML_NAME != null ? c.FAML_NAME : c.PAY_PERSONEL.FAML_NAME,
                              NAME = (c.FIRS_NAME != null ? c.FIRS_NAME : c.PAY_PERSONEL.FIRS_NAME) + " " + (c.FAML_NAME != null ? c.FAML_NAME : c.PAY_PERSONEL.FAML_NAME)
                          }).OrderBy(xx => xx.FAML_NAME).ToList();
            //var query = (from a in Db.PAY_PERSONEL
            //             join b in Db.SEC_USERS on a.EMP_NUMB equals b.PRSN_EMP_NUMB
            //             where b.USER_STATE == "1" && (a.ASTA_CODE == "7" || a.ASTA_CODE == "1" || a.ASTA_CODE == "5" || a.ASTA_CODE == "4" || a.ASTA_CODE == null) && b.USER_NAME != null && (a.FAML_NAME.ToUpper().Contains(text) || a.FIRS_NAME.ToUpper().Contains(text) || text == null)
            //             select new
            //             {
            //                 b.ROW_NO,
            //                 b.ORCL_NAME,
            //                 FAML_NAME = (b.FIRS_NAME != null ? b.FIRS_NAME : b.PAY_PERSONEL.FIRS_NAME) + " " + (b.FAML_NAME != null ? b.FAML_NAME : b.PAY_PERSONEL.FAML_NAME) + "-" + b.ORCL_NAME,
            //             })
            //             .Union(
            //             from b in Db.SEC_USERS
            //             where b.USER_STATE == "1" && b.PRSN_EMP_NUMB == null && (b.FAML_NAME != null) && (b.FAML_NAME.ToUpper().Contains(text) || b.FIRS_NAME.ToUpper().Contains(text) || text == null)
            //             orderby b.FAML_NAME
            //             select new
            //             {
            //                 b.ROW_NO,
            //                 b.ORCL_NAME,
            //                 FAML_NAME = (b.FIRS_NAME) + " " + (b.FAML_NAME) + "-" + b.ORCL_NAME,
            //             }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_ChkTemplate_Monthly(int? EINS_ID, string CHLT_CODE)
        {
            var RetVal = from b in PublicRepository.Get_ChkTemplate()
                         where (b.EINS_EINS_ID == EINS_ID || EINS_ID == null) && (b.CHLT_CODE == CHLT_CODE || CHLT_CODE == null)
                         orderby b.CHLT_TITL
                         select new { b.CHLT_ID, b.CHLT_TITL };
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_ChkTemplate_Item(int? CHLT_ID)
        {
            var RetVal = from b in Db.CHK_ITEM_TEMPLATE
                         where
                             (b.CHLT_CHLT_ID == CHLT_ID && b.CHTI_STATUS != 0)
                         orderby b.CHTI_ID
                         select new { b.CHTI_ID, b.CHTI_DESC };
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_Status(int? CHLT_ID)
        {
            var RetVal = from b in Db.CHK_DOMAIN
                         where
                             (b.DMAN_DMAN_ID_LIST == CHLT_ID)
                         orderby b.DMAN_ID
                         select new { b.DMAN_TITL };
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_ChkTemplate_Item2(int? CHLT_ID, int? EPOL_ID)
        {
            var RetVal = (from b in Db.CHK_ITEM_TEMPLATE
                          join c in Db.EXP_DAILY_CHECKLIST_VAR on b.CHTI_ID equals c.CHTI_CHTI_ID
                          join d in Db.EXP_DAILY_CHECKLIST_HEAD on c.EDCH_EDCH_ID equals d.EDCH_ID
                          where
                              (b.CHLT_CHLT_ID == CHLT_ID) && d.EPOL_EPOL_ID == EPOL_ID
                          orderby b.CHTI_ID
                          select new { b.CHTI_ID, b.CHTI_DESC }).Distinct();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_Head([DataSourceRequest] DataSourceRequest request, int? EPOL_ID, short? EDCH_TYPE)
        {
            var query = (from p in Db.EXP_DAILY_CHECKLIST_HEAD
                         join q in Db.SEC_USERS on p.CRET_BY equals q.ORCL_NAME
                         join v in Db.EXP_POST_LINE on p.EPOL_EPOL_ID equals v.EPOL_ID
                         join z in Db.PAY_ORGAN on v.ORGA_CODE equals z.CODE
                         //join x in cntx.EXP_DAILY_CHECKLIST_VAR on p.EDCH_ID equals x.EDCH_EDCH_ID
                         where (p.EPOL_EPOL_ID == EPOL_ID) && p.EDCH_TYPE == EDCH_TYPE && v.EPOL_STAT == "1" && v.ORGA_MANA_ASTA_CODE == z.MANA_ASTA_CODE && v.ORGA_MANA_CODE == z.MANA_CODE
                         orderby p.EDCH_DATE descending, p.EDCH_ID descending
                         select new
                         {
                             p.EDCH_ID,
                             p.EDCH_DATE,
                             p.EDCH_TYPE,
                             EPOL_NAME = v.EPOL_NAME,
                             ORGA_DESC = z.ORGA_DESC,
                             p.EDCH_DESC,
                             p.EDCH_TIME,
                             COUNT = Db.EXP_DAILY_CHECKLIST_VAR.Where(xx => xx.EDCH_EDCH_ID == p.EDCH_ID).Count(),
                             FAML_NAME = (q.FIRS_NAME != null ? q.FIRS_NAME : q.PAY_PERSONEL.FIRS_NAME) + " " + (q.FAML_NAME != null ? q.FAML_NAME : q.PAY_PERSONEL.FAML_NAME)
                         }).ToList();

            if (userid == 353 || userid == 372 || userid == 516)
            {
                query = (from p in Db.EXP_DAILY_CHECKLIST_HEAD
                         join q in Db.SEC_USERS on p.CRET_BY equals q.ORCL_NAME
                         join v in Db.EXP_POST_LINE on p.EPOL_EPOL_ID equals v.EPOL_ID
                         join z in Db.PAY_ORGAN on v.ORGA_CODE equals z.CODE
                         where (p.EPOL_EPOL_ID == EPOL_ID || EPOL_ID == null) && p.EDCH_TYPE == EDCH_TYPE && v.EPOL_STAT == "1"
                         orderby p.EDCH_DATE descending, p.EDCH_ID descending
                         select new
                         {
                             p.EDCH_ID,
                             p.EDCH_DATE,
                             p.EDCH_TYPE,
                             EPOL_NAME = v.EPOL_NAME,
                             ORGA_DESC = z.ORGA_DESC,
                             p.EDCH_DESC,
                             p.EDCH_TIME,
                             COUNT = Db.EXP_DAILY_CHECKLIST_VAR.Where(xx => xx.EDCH_EDCH_ID == p.EDCH_ID).Count(),
                             FAML_NAME = (q.FIRS_NAME != null ? q.FIRS_NAME : q.PAY_PERSONEL.FIRS_NAME) + " " + (q.FAML_NAME != null ? q.FAML_NAME : q.PAY_PERSONEL.FAML_NAME)
                         }).ToList();
            }

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Get_checklist_head_user([DataSourceRequest] DataSourceRequest request, int? edch_id)
        {
            var query = (from p in Db.EXP_DAILY_CHECKLIST_USER
                         join c in Db.SEC_USERS.AsEnumerable() on p.SCSU_ROW_NO equals c.ROW_NO
                         where p.EDCH_EDCH_ID == edch_id
                         select new
                         {
                             p.EDCU_ID,
                             FIRS_NAME = c.FIRS_NAME != null ? c.FIRS_NAME : c.PAY_PERSONEL.FIRS_NAME,
                             FAML_NAME = c.FAML_NAME != null ? c.FAML_NAME : c.PAY_PERSONEL.FAML_NAME,
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Get_checklist_user([DataSourceRequest] DataSourceRequest request, int? eedo_id)
        {
            var query = (from p in Db.EXP_DAILY_CHECKLIST_VAR
                         join v in Db.EXP_CHECKLIST_EXPI on p.EDCV_ID equals v.EDCV_EDCV_ID
                         join c in Db.SEC_USERS.AsEnumerable() on p.CRET_BY equals c.ORCL_NAME
                         where v.EEDO_EEDO_ID == eedo_id
                         orderby v.CHEX_ID
                         select new
                         {
                             USER = p.CRET_BY,
                             v.CHEX_ID,
                             FIRS_NAME = c.FIRS_NAME != null ? c.FIRS_NAME : c.PAY_PERSONEL.FIRS_NAME,
                             FAML_NAME = c.FAML_NAME != null ? c.FAML_NAME : c.PAY_PERSONEL.FAML_NAME,
                             DATE = p.EXP_DAILY_CHECKLIST_HEAD.EDCH_DATE
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        public string MakeText()
        {
            return "sampleText";
        }

        public ActionResult Get_Var([DataSourceRequest] DataSourceRequest request, int? EDCH_ID)
        {
            var query = (from p in Db.EXP_DAILY_CHECKLIST_VAR
                         join v in Db.CHK_ITEM_TEMPLATE on p.CHTI_CHTI_ID equals v.CHTI_ID
                         join z in Db.CHK_CHECK_LIST_TEMPLATE on v.CHLT_CHLT_ID equals z.CHLT_ID
                         //join w in cntx.EXP_EPIV_V on p.EPVH_ID equals w.EPVH_ID
                         where p.EDCH_EDCH_ID == EDCH_ID
                         //&& p.ROW_NO == userid && (v.ORGA_MANA_ASTA_CODE == z.MANA_ASTA_CODE && v.ORGA_MANA_CODE == z.MANA_CODE)
                         orderby v.CHTI_DESC descending
                         select new
                         {
                             p.EDCV_ID,
                             EDCH_DATE = p.EXP_DAILY_CHECKLIST_HEAD.EDCH_DATE,
                             CHTI_DESC = v.CHTI_DESC,
                             CHLT_DESC = z.CHLT_TITL,
                             p.EDCV_VAR,
                             p.EDCV_DESC,
                             NAME = (p.SEC_USERS.FIRS_NAME != null ? p.SEC_USERS.FIRS_NAME : p.SEC_USERS.PAY_PERSONEL.FIRS_NAME) + " " + (p.SEC_USERS.FAML_NAME != null ? p.SEC_USERS.FAML_NAME : p.SEC_USERS.PAY_PERSONEL.FAML_NAME)
                         }).ToList().Distinct();

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Getpost(string OrgaCode)
        {
            var query = (from b in Db.EXP_POST_LINE
                         orderby b.EPOL_NAME
                         where b.EPOL_TYPE == "0" && b.ORGA_CODE == OrgaCode
                         select new { b.EPOL_ID, EPOL_NAME = b.EPOL_NAME }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //~DailyChecklistController()
        //{
        //    //این دستور کانکشن را دیسکانکت میکند
        //    Db.Dispose();
        //}
    }

}

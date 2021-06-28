using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.GroupEnrgy
{
    public class GroupEnergyController : DbController
    {
        //BandarEntities Db;

        PersianCalendar pc = new PersianCalendar();
        DateTime thisDate = DateTime.Now;
        int? userid = 0;
        string username = string.Empty;

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //        Db.Dispose();
        //    base.Dispose(disposing);
        //}

        public GroupEnergyController()
            : base()
        {
            //Db = this.DB();
            userid = this.UserInfo().UserId;
        }


        //
        //
        // GET: /GroupEnergy/
        public ActionResult Search_Line_Trans()
        {
            return View();
        }

        [HttpPost]
        public void Ajax_Update_group(int GROP_ID)
        {
            string sql = string.Format("update exp_groups set grop_grop_id=null where grop_id={0} ", GROP_ID);
            Db.Database.ExecuteSqlCommand(sql);
        }

        public ActionResult ReadInstrument([DataSourceRequest] DataSourceRequest request, string fd)
        {
            bool filterDisable = string.IsNullOrEmpty(fd);
            string filter = string.IsNullOrEmpty(fd.ToUpper()) ? "" : fd.ToUpper();

            var query = (from b in Db.EXP_POST_LINE_INSTRU
                             // join post in cntx.EXP_POST_LINE on b.EPOL_EPOL_ID equals post.EPOL_ID
                         where (b.EINS_EINS_ID == 2 || b.EINS_EINS_ID == 1 || b.EINS_EINS_ID == 3 || b.EINS_EINS_ID == 23 || b.EINS_EINS_ID == 1401 || b.EINS_EINS_ID == 13) && b.CODE_NAME != null && (b.EXP_POST_LINE.EPOL_NAME.ToUpper().Contains(filter) || b.EXP_POST_LINE1.EPOL_NAME.ToUpper().Contains(filter) || b.EXP_POST_LINE2.EPOL_NAME.ToUpper().Contains(filter) || b.CODE_DISP.ToUpper().Contains(filter) || b.CODE_NAME.ToUpper().Contains(filter) || filterDisable)
                         orderby b.EXP_POST_LINE.EPOL_NAME, b.CODE_NAME descending
                         select new
                         {
                             EPOL_NAME = (b.EINS_EINS_ID == 2 || b.EINS_EINS_ID == 1401 || b.EINS_EINS_ID == 23 || b.EINS_EINS_ID == 13) ?
                             (b.EXP_POST_LINE1.EPOL_NAME != null ? b.EXP_POST_LINE1.EPOL_NAME : b.EXP_POST_LINE2.EPOL_NAME)
                             : b.EXP_POST_LINE.EPOL_NAME,
                             b.EPIU_ID,
                             b.EINS_EINS_ID,
                             b.CODE_DISP,
                             b.EUNL_EUNL_ID,
                             b.EARD_EARD_ID,
                             b.OUIN_TYPE,
                             b.EPIU_TYPE,
                             b.CODE_NAME,
                             b.ETEX_ETEX_ID,
                             b.PHAS_TYPE,
                             b.PHAS_STAT,
                             b.SERN_NO,
                             //EUNL_DESC=cntx.EXP_UNIT_LEVEL.Where(xx=>xx.EUNL_ID==b.EUNL_EUNL_ID).Select(xx=>xx.EUNL_DESC).FirstOrDefault()
                         }).ToList();

            return Json(query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            ViewData["viewdata"] = Db.STR_UNIT_MEASURMENT.Select(o => new { o.UNIT_CODE, o.UNIT_DESC }).AsEnumerable();
            return View();
        }

        public ActionResult Add_Post(int? id)
        {
            ViewData["GROP_ID"] = id;
            return View();
        }
        public ActionResult Setting(int? id)
        {
            ViewData["GROP_ID"] = id;
            return View();
        }

        public ActionResult Add_Item(int? id)
        {
            ViewData["POGR_ID"] = id;
            return View();
        }

        public ActionResult Add_Time(int? id)
        {
            return View();
        }

        public ActionResult Add_Group(int? id)
        {
            ViewData["GROP_ID"] = id;
            return View();
        }

        public ActionResult insert_subgroup(EXP_GROUPS objecttemp)
        {
            //string sql = string.Format("update exp_groups set grop_grop_id='{0}' where grop_id={1}", objecttemp.GROP_GROP_ID, objecttemp.GROP_ID);
            string sql = string.Format("insert into  exp_groups_groups (GROP_GROP_ID,GROP_GROP_ID_R) values({0},{1})", objecttemp.GROP_GROP_ID, objecttemp.GROP_ID);
            Db.Database.ExecuteSqlCommand(sql);
            //cntx.EXP_GROUPS.Add(objecttemp);
            // cntx.SaveChanges();
            //Db.Database.ExecuteSqlCommand(sql);
            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
        }

        public ActionResult insert_time(EXP_GOLOBAL_TIME objecttemp)
        {
            var check = from b in Db.EXP_GOLOBAL_TIME
                        where b.GLTI_DATE == objecttemp.GLTI_DATE
                        select b;

            if (!check.Any())
            {
                try
                {
                    Db.EXP_GOLOBAL_TIME.Add(objecttemp);
                    Db.SaveChanges();
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
                }
                catch (Exception Ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { Message = "خطا در ثبت اطلاعات :" + Ex.ToString() }.ToJson();
                }
            }
            else
            {
                return new ServerMessages(ServerOprationType.Failure) { Message = string.Format("اطلاعات وارده شده تکرار است ") }.ToJson();

            }
        }

        public ActionResult insert_group(EXP_GROUPS objecttemp)
        {
            //objecttemp.GROP_GROP_ID = 62;
            try
            {
                string Sql = string.Format("insert into  exp_groups (GROP_DESC,GROP_ORDR,GROP_CODE,GROP_STAT) values ('{0}',{1},{2},{3})", objecttemp.GROP_DESC, objecttemp.GROP_ORDR, objecttemp.GROP_CODE, objecttemp.GROP_STAT);
                Db.Database.ExecuteSqlCommand(Sql);
                // Db.EXP_GROUPS.Add(objecttemp);
                // Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات  ") }.ToJson();

            }
        }

        public ActionResult insert_Setting(EXP_GROUP_SETTING objecttemp)
        {
            if (!string.IsNullOrEmpty(objecttemp.ORGA_CODE))
            {
                objecttemp.ORGA_MANA_ASTA_CODE = "7";
                objecttemp.ORGA_MANA_CODE = "6";
            }
            objecttemp.GRSE_STAT = 0;

            Db.EXP_GROUP_SETTING.Add(objecttemp);
            Db.SaveChanges();
            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
        }

        public ActionResult insert_post_group(EXP_POST_GROUP objecttemp)
        {

            var EXP_POST_GROUP_INSTRU = new EXP_POST_GROUP_INSTRU();
            Db.EXP_POST_GROUP.Add(objecttemp);
            var check = from b in Db.EXP_POST_GROUP
                        where b.GROP_GROP_ID == objecttemp.GROP_GROP_ID && b.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID
                        select b;

            if (!check.Any())
            {
                Db.SaveChanges();
            }

            string L = Request.Form["L"];
            string T = Request.Form["T"];
            if (!string.IsNullOrEmpty(T))
            {
                var query = from b in Db.EXP_POST_LINE_INSTRU
                            where b.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID && b.EINS_EINS_ID == 2
                            select b;

                foreach (var item in query)
                {
                    EXP_POST_GROUP_INSTRU.POGI_STAT = 1;
                    EXP_POST_GROUP_INSTRU.POGR_POGR_ID = objecttemp.POGR_ID;
                    EXP_POST_GROUP_INSTRU.EPIU_EPIU_ID = item.EPIU_ID;
                    Db.EXP_POST_GROUP_INSTRU.Add(EXP_POST_GROUP_INSTRU);

                    var check2 = from b in Db.EXP_POST_GROUP_INSTRU
                                 where b.EPIU_EPIU_ID == EXP_POST_GROUP_INSTRU.EPIU_EPIU_ID
                                 && b.POGR_POGR_ID == EXP_POST_GROUP_INSTRU.POGR_POGR_ID
                                 select b;

                    if (!check2.Any() && EXP_POST_GROUP_INSTRU.POGR_POGR_ID != 0)
                    {
                        Db.SaveChanges();
                    }
                }
            }
            if (!string.IsNullOrEmpty(L))
            {
                var query = from b in Db.EXP_POST_LINE_INSTRU
                            where ((b.EPOL_EPOL_ID_LINE == objecttemp.EPOL_EPOL_ID || b.EPOL_EPOL_ID_INSLIN == objecttemp.EPOL_EPOL_ID))
                            select b;

                foreach (var item in query)
                {
                    EXP_POST_GROUP_INSTRU.POGI_STAT = 1;
                    EXP_POST_GROUP_INSTRU.POGR_POGR_ID = objecttemp.POGR_ID;
                    EXP_POST_GROUP_INSTRU.EPIU_EPIU_ID = item.EPIU_ID;
                    Db.EXP_POST_GROUP_INSTRU.Add(EXP_POST_GROUP_INSTRU);

                    var check2 = from b in Db.EXP_POST_GROUP_INSTRU
                                 where b.EPIU_EPIU_ID == EXP_POST_GROUP_INSTRU.EPIU_EPIU_ID
                                 && b.POGR_POGR_ID == EXP_POST_GROUP_INSTRU.POGR_POGR_ID
                                 select b;

                    if (!check2.Any() && EXP_POST_GROUP_INSTRU.POGR_POGR_ID != 0)
                    {
                        Db.SaveChanges();
                    }
                }
            }

            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
        }

        public ActionResult insert_post_group_instru(EXP_POST_GROUP_INSTRU objecttemp)
        {
            string EPIU_EPIU_ID = Request.Form["POSTINSTTT"];
            EXP_POST_GROUP objecttemp2 = new EXP_POST_GROUP();
            objecttemp.EPIU_EPIU_ID = decimal.Parse(EPIU_EPIU_ID);

            int? EINS_ID = Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_ID == objecttemp.EPIU_EPIU_ID).Select(xx => xx.EINS_EINS_ID).FirstOrDefault();

            int? EPOL_ID = 0;
            if (EINS_ID == 2 || EINS_ID == 3 || EINS_ID == 23 || EINS_ID == 1401 || EINS_ID == 13)
            {
                EPOL_ID = Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_ID == objecttemp.EPIU_EPIU_ID).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault();
            }
            else if (EINS_ID == 1)
            {
                EPOL_ID = Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_ID == objecttemp.EPIU_EPIU_ID).Select(xx => xx.EPOL_EPOL_ID_INSLIN).FirstOrDefault();
            }

            if (!Db.EXP_POST_GROUP.Where(xx => xx.EPOL_EPOL_ID == EPOL_ID).Where(xx => xx.GROP_GROP_ID == objecttemp.POGR_POGR_ID).Any())
            {
                objecttemp2.EPOL_EPOL_ID = EPOL_ID;
                objecttemp2.GROP_GROP_ID = objecttemp.POGR_POGR_ID;
                Db.EXP_POST_GROUP.Add(objecttemp2);
                Db.SaveChanges();
            }

            objecttemp.POGR_POGR_ID = Db.EXP_POST_GROUP.Where(xx => xx.GROP_GROP_ID == objecttemp.POGR_POGR_ID)
                                                       .Where(xx => xx.EPOL_EPOL_ID == EPOL_ID)
                                                       .Select(xx => xx.POGR_ID).FirstOrDefault();
            Db.EXP_POST_GROUP_INSTRU.Add(objecttemp);

            var check = from b in Db.EXP_POST_GROUP_INSTRU
                        where b.POGR_POGR_ID == objecttemp.POGR_POGR_ID && b.EPIU_EPIU_ID == objecttemp.EPIU_EPIU_ID
                        select b;

            if (!check.Any())
            {
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            else
            {
                return new ServerMessages(ServerOprationType.Failure) { Message = string.Format("اطلاعات  وارد شده تکراری می باشد ") }.ToJson();
            }
        }

        public ActionResult GetGroup(short? id)
        {
            var RetVal = (from p in Db.EXP_GROUPS
                              //where p..GROP_GROP_ID !=id || p.GROP_GROP_ID==null
                          select new { p.GROP_ID, GROP_DESC = p.GROP_DESC }).OrderBy(xx => xx.GROP_DESC);

            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGroup_DP(int? GROP_CODE)
        {
            string user_name = this.HttpContext.User.Identity.Name;
            var check = from p in Db.EXP_USERS_GROUPS
                        join d in Db.SEC_USERS on p.SCSU_ROW_NO equals d.ROW_NO
                        where d.USER_NAME == user_name
                        select p;

            //var query = (from b in cntx.EXP_GROUPS
            //             where (b.GROP_CODE == GROP_CODE || GROP_CODE==null)
            //             orderby b.GROP_DESC
            //             select new { b.GROP_DESC, b.GROP_ID }).OrderBy(xx=>xx.GROP_DESC).Distinct().ToList();

            var query = Db.Database.SqlQuery<EXP_GROUPS>("select * from EXP_GROUPS where GROP_CODE=:GROP_CODE or  :GROP_CODE is null order by GROP_DESC", GROP_CODE).Select(x => new
            {
                x.GROP_ID,
                x.GROP_DESC
            }).ToList();

            if (check.Any() && user_name != "s-khademi")
            {
                query = Db.Database.SqlQuery<EXP_GROUPS>("select a.* from EXP_GROUPS a,EXP_USERS_GROUPS b,SEC_USERS c where a.GROP_ID=b.GROP_GROP_ID and b.SCSU_ROW_NO=c.ROW_NO and USER_NAME=:user_name  and (a.GROP_CODE=:GROP_CODE or  :GROP_CODE is null) order by GROP_DESC", user_name, GROP_CODE).Select(x => new
                {
                    x.GROP_ID,
                    x.GROP_DESC
                }).Distinct().ToList();
                //    query = (from b in cntx.EXP_GROUPS
                //             join d in cntx.EXP_USERS_GROUPS on b.GROP_ID equals d.GROP_GROP_ID
                //             where d.SEC_USERS.USER_NAME == user_name && (b.GROP_CODE == GROP_CODE || GROP_CODE == null)
                //             orderby b.GROP_DESC
                //             select new { b.GROP_DESC, b.GROP_ID });
            }

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_Time([DataSourceRequest] DataSourceRequest request)
        {


            var query = (from p in Db.EXP_GOLOBAL_TIME

                         orderby p.GLTI_DATE descending
                         select new

                         {


                             p.GLTI_ID,
                             p.GLTI_DATE,
                             p.GLTI_YEAR_MW,// = Get_LastYearBar(p.GLTI_DATE),// p.GLTI_YEAR_MW,
                             p.GLTI_TOZI_MW,
                             p.GLTI_HOUR
                         }).ToList();
            var Query = query.Select(p => new
            {
                p.GLTI_ID,
                p.GLTI_DATE,
                GLTI_YEAR_MW = p.GLTI_YEAR_MW == null ? Get_LastYearBar(p.GLTI_DATE) : p.GLTI_YEAR_MW,// p.GLTI_YEAR_MW,
                GLTI_TOZI_MW = p.GLTI_TOZI_MW == null ? Get_LastYearBarTozi(p.GLTI_DATE) : p.GLTI_TOZI_MW,
                p.GLTI_HOUR

            });
            var jsonResult = Json(Query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

            //return Json(Query.ToDataSourceResult(request));
        }

        public ActionResult Get_SubGroup([DataSourceRequest] DataSourceRequest request, short? GROP_ID)
        {
            var query = (from p in Db.EXP_GROUPS_GROUPS
                         where p.GROP_GROP_ID == GROP_ID || GROP_ID == null && (p.GROP_GROP_ID != 62 && p.GROP_GROP_ID != 148 || this.HttpContext.User.Identity.Name == "s-khademi")
                         orderby p.EXP_GROUPS.GROP_DESC
                         select new
                         {
                             p.GRGR_ID,
                             p.EXP_GROUPS.GROP_DESC,
                             GROP_DESC1 = p.EXP_GROUPS1.GROP_DESC
                             //p.GROP_CODE,
                             //p.GROP_ORDR,
                             //p.GROP_STAT,
                             //p.GROP_GROP_ID,
                             //GROP_GROP_DESC = p.EXP_GROUPS2.GROP_DESC
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }


        public ActionResult Get_Group([DataSourceRequest] DataSourceRequest request, short? GROP_ID)
        {
            var query = (from p in Db.EXP_GROUPS
                         where p.GROP_GROP_ID == GROP_ID || GROP_ID == null && (p.GROP_GROP_ID != 62 && p.GROP_GROP_ID != 148 || this.HttpContext.User.Identity.Name == "s-khademi")
                         orderby p.GROP_DESC
                         select new
                         {
                             p.GROP_ID,
                             p.GROP_DESC,

                             p.GROP_CODE,
                             p.GROP_ORDR,
                             p.GROP_STAT,
                             p.GROP_GROP_ID,
                             GROP_GROP_DESC = p.EXP_GROUPS2.GROP_DESC
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }



        public ActionResult Get_Group_Setting([DataSourceRequest] DataSourceRequest request, short? GROP_ID)
        {
            var query = (from p in Db.EXP_GROUP_SETTING
                         where p.GROP_GROP_ID == GROP_ID
                         orderby p.GRSE_ID
                         select new
                         {
                             p.GRSE_ID,
                             EUNL_DESC = p.EXP_UNIT_LEVEL.EUNL_DESC,
                             p.GRSE_STAT,
                             EINS_DESC = p.EXP_INSTRUMENT.EINS_DESC,
                             ORGA_DESC = p.PAY_ORGAN.ORGA_DESC

                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        public decimal? Get_LastYearBar(string GLTI_DATE)
        {
            decimal? LastYearBar = 0;
            string LastYear = Get_LastYearDate(GLTI_DATE);
            //LastYearBar = Db.EXP_GROP_V.Where(xx => xx.GROP_ID == 62).Where(xx => xx.VAR_DATE == LastYear).OrderByDescending(xx => xx.MW).Select(xx => xx.MW.Value).FirstOrDefault();
            LastYearBar = Db.Database.SqlQuery<decimal?>(string.Format("select sum(abs(mw)) from EXP_GROP_V where grop_id={0} and var_date='{1}' and mw<0 ", 62, LastYear)).FirstOrDefault();
            return LastYearBar;
        }
        public decimal? Get_LastYearBarTozi(string GLTI_DATE)
        {
            decimal? LastYearBar = 0;
            string LastYear = Get_LastYearDate(GLTI_DATE);
            //LastYearBar = Db.EXP_GROP_V.Where(xx => xx.GROP_ID == 148).Where(xx => xx.VAR_DATE == LastYear).OrderByDescending(xx => xx.MW).Select(xx => xx.MW.Value).FirstOrDefault();
            LastYearBar = Db.Database.SqlQuery<decimal?>(string.Format("select sum(abs(mw)) from EXP_GROP_V where grop_id={0} and var_date='{1}' and mw<0 ", 148, LastYear)).FirstOrDefault();

            return LastYearBar;
        }

        public string Get_LastYearDate(string GLTI_DATE)
        {

            var dateParts = GLTI_DATE.Split('/');
            DateTime LastYearDate = new DateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]), int.Parse(dateParts[2]), new PersianCalendar()).AddYears(-1);
            string LastYear = Db.Database.SqlQuery<string>(string.Format("SELECT farsi_date_u(to_date('{0}','mm/dd/yyyy')) FROM DUAL", LastYearDate.ToShortDateString())).FirstOrDefault().ToString();
            return LastYear;
        }

        public ActionResult Get_Post_Group([DataSourceRequest] DataSourceRequest request, short? GROP_ID)
        {
            var query = (from p in Db.EXP_POST_GROUP
                         where p.GROP_GROP_ID == GROP_ID
                         orderby p.EXP_POST_LINE.EPOL_NAME
                         select new
                         {
                             p.POGR_ID,
                             EPOL_NAME = p.EXP_POST_LINE.EPOL_NAME,
                             p.EPOL_EPOL_ID
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Get_Post_Group_Instru([DataSourceRequest] DataSourceRequest request, short? POGR_ID)
        {
            var query = (from p in Db.EXP_POST_GROUP_INSTRU
                         join d in Db.EXP_POST_GROUP on p.POGR_POGR_ID equals d.POGR_ID
                         where d.GROP_GROP_ID == POGR_ID
                         //orderby p.EXP_POST_LINE_INSTRU.CODE_NAME
                         select new
                         {
                             p.EPIU_EPIU_ID,
                             p.POGI_ID,
                             EPOL_NAME = p.EXP_POST_GROUP.EXP_POST_LINE.EPOL_NAME,
                             CODE_NAME = p.EXP_POST_LINE_INSTRU.CODE_NAME,
                             ////   EPOL_EPOL_ID = p.EXP_POST_GROUP.EXP_POST_LINE.EPOL_ID,
                             CODE_DISP = p.EXP_POST_LINE_INSTRU.CODE_DISP,
                             p.POGI_STAT,
                             p.POGI_DATE,
                             p.POGI_TIME,
                             p.POGR_POGR_ID
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Update_Post_Group_Instru([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<EXP_POST_GROUP_INSTRU> EXP_POST_GROUP_INSTRU)
        {
            if (EXP_POST_GROUP_INSTRU != null)
            {
                foreach (EXP_POST_GROUP_INSTRU item in EXP_POST_GROUP_INSTRU)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }


            return Json(EXP_POST_GROUP_INSTRU.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Update_Group([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<EXP_GROUPS> EXP_GROUPS)
        {
            if (EXP_GROUPS != null)
            {
                foreach (EXP_GROUPS item in EXP_GROUPS)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            var query = (from p in Db.EXP_GROUPS
                         orderby p.GROP_ORDR, p.GROP_DESC
                         select new
                         {
                             p.GROP_ID,
                             p.GROP_DESC,
                             p.GROP_CODE,
                             p.GROP_ORDR,
                             p.GROP_STAT,
                             p.GROP_GROP_ID
                         }).ToList();

            // return Json(EXP_GROUPS.ToDataSourceResult(request, ModelState));
            return Json(query.ToDataSourceResult(request));
        }

        //~GroupEnergyController()
        //{
        //    //این دستور کانکشن را دیسکانکت میکند
        //    Db.Dispose();
        //}

    }

}

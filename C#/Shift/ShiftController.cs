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

namespace Equipment.Controllers.Exploitation.DailyChecklist
{
    public class ShiftController : DbController
    {
        //BandarEntities Db;

        PersianCalendar pc = new PersianCalendar();
        DateTime thisDate = DateTime.Now;
        int? userid = 0; string username = string.Empty;
        public ShiftController()
            : base()
        {
            // Db = this.DB();
            username = this.UserInfo().Username;
        }

        public ActionResult EXP_SHIFT_HEAD(int? id)
        {
            ViewData["EDCH_TYPE"] = id;
            return View();
        }
        public ActionResult EXP_LEARN_PRSN()
        {
            return View();
        }

        public ActionResult EXP_SHIFT_GROUPS()
        {
            return View();
        }
        public ActionResult Report_Shift_48Hour()
        {
            return View();
        }
        public ActionResult Report_Shift_Keshik()
        {
            return View();
        }
        public ActionResult EXP_SHIFT_GROUPS_PRSN()
        {
            return View();
        }

        public ActionResult EXP_SHIFT_PRSN(int? id)
        {
            ViewData["ESHH_ID"] = id;
            return View();
        }
        public ActionResult ShiftDashboard(int EPOL_ID, string Date)
        {
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["Date"] = Date;
            return View();
        }
        public ActionResult Copy(int? id)
        {
            ViewData["ESHH_ID"] = id;
            return View();
        }

        public ActionResult EXP_SHIFT_PRSN_ORGA(int? id)
        {
            ViewData["ESHH_ID"] = id;
            return View();
        }
        public ActionResult EXP_SHIFT_PRSN_KESHIK(int? id)
        {
            ViewData["ESHH_ID"] = id;
            return View();
        }

        public ActionResult EXP_SHIFT_PRSN_Report_Personel()
        {
            return View();
        }
        public ActionResult EXP_SHIFT_ADD_USER(int? id, int? EPOL_ID, int? tab, int? EPPS_ID, int? EPPS_STAT, int? ESTY_ESTY_ID)
        {
            ViewData["ESHH_ID"] = id;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["tab"] = tab;
            ViewData["EPPS_ID"] = EPPS_ID;
            ViewData["EPPS_STAT"] = EPPS_STAT;
            ViewData["ESTY_ESTY_ID"] = ESTY_ESTY_ID;
            return View();
        }

        public ActionResult EXP_SHIFT_PRSN_Report()
        {
            return View();
        }
        public ActionResult EXP_SHIFT_ADD_USER_OffDate()
        {
            return View();
        }
        public ActionResult EXP_SHIFT_PRSN_CARD()
        {
            return View();
        }

        public ActionResult EXP_SHIFT_POST_Report()
        {
            return View();
        }

        public ActionResult EXP_SHIFT_PRSN_Report_Details()
        {
            return View();
        }

        public ActionResult Getpost(int? grop_id)
        {
            var query = (from b in Db.EXP_POST_LINE
                         join c in Db.EXP_POST_GROUP on b.EPOL_ID equals c.EPOL_EPOL_ID
                         orderby b.EPOL_NAME
                         where b.EPOL_TYPE == "0" && b.EPOL_STAT == "1" && c.GROP_GROP_ID == grop_id
                         select new { b.EPOL_ID, EPOL_NAME = b.EPOL_NAME }).Distinct().ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUserPost(int? ROW_NO)
        {
            var query = (from b in Db.EXP_POST_LINE
                         join c in Db.SEC_USER_TYPE_POST on b.EPOL_ID equals c.EPOL_EPOL_ID
                         orderby b.EPOL_NAME
                         where b.EPOL_TYPE == "0" && b.EPOL_STAT == "1" && c.SCSU_ROW_NO == ROW_NO
                         select new { b.EPOL_ID, EPOL_NAME = b.EPOL_NAME }).Distinct().ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllPost()
        {
            var query = (from b in Db.EXP_POST_LINE

                         orderby b.EPOL_NAME
                         where b.EPOL_TYPE == "0" && b.EPOL_STAT == "1"
                         select new { b.EPOL_ID, EPOL_NAME = b.EPOL_NAME }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_Group([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from p in Db.EXP_SHIFT_GROUPS
                         orderby p.ESHG_DESC
                         select new
                         {
                             p.ESHG_ID,
                             p.ESHG_DESC,
                             p.ESHG_TIME,
                             p.ESGH_STAT,
                             TYPE_DESC = p.EXP_SHIFT_TYPE.ESTY_DESC,
                             p.ESTY_ESTY_ID
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult insert_shift_groups(EXP_SHIFT_GROUPS objecttemp)
        {
            try
            {
                Db.EXP_SHIFT_GROUPS.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات  ") }.ToJson();

            }
        }

        public ActionResult insert_exp_shift_groups_prsn(EXP_SHIFT_GROUP_PRSN objecttemp)
        {
            try
            {
                Db.EXP_SHIFT_GROUP_PRSN.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات  ") }.ToJson();
            }
        }
        public ActionResult Copy_Shift_Data(EXP_SHIFT_HEAD objecttemp)
        {
            try
            {
                decimal New_ESHH_ID = 0;
                var ShiftHeadQuery = from b in Db.EXP_SHIFT_HEAD where b.ESHH_ID == objecttemp.ESHH_ID select b;
                if (Db.EXP_SHIFT_HEAD.Where(xx => xx.ESHH_DATE == objecttemp.ESHH_DATE).Any())
                {

                    New_ESHH_ID = Db.EXP_SHIFT_HEAD.Where(xx => xx.ESHH_DATE == objecttemp.ESHH_DATE && xx.GROP_GROP_ID == ShiftHeadQuery.Select(yy => yy.GROP_GROP_ID).FirstOrDefault()
                    ).Select(xx => xx.ESHH_ID).FirstOrDefault();

                    string InsertQuery = string.Format("insert into EXP_SHIFT_PERS (ESHH_ESHH_ID,SCSU_ROW_NO,FIRS_DATE,EPPS_STAT,ESHG_ESHG_ID) " +
                                                      "select {0},SCSU_ROW_NO,'{1}',EPPS_STAT,ESHG_ESHG_ID from EXP_SHIFT_PERS where ESHH_ESHH_ID={2}", New_ESHH_ID, objecttemp.ESHH_DATE, objecttemp.ESHH_ID
                        );

                    Db.Database.ExecuteSqlCommand(InsertQuery);



                }
                else
                {
                    var New_ObjectTemp = new EXP_SHIFT_HEAD();
                    New_ObjectTemp.ESHH_DATE = objecttemp.ESHH_DATE;
                    New_ObjectTemp.GROP_GROP_ID = ShiftHeadQuery.Select(yy => yy.GROP_GROP_ID).FirstOrDefault();
                    Db.EXP_SHIFT_HEAD.Add(New_ObjectTemp);
                    Db.SaveChanges();
                    New_ESHH_ID = Db.EXP_SHIFT_HEAD.Where(xx => xx.ESHH_DATE == objecttemp.ESHH_DATE && xx.GROP_GROP_ID == ShiftHeadQuery.Select(yy => yy.GROP_GROP_ID).FirstOrDefault()
                   ).Select(xx => xx.ESHH_ID).FirstOrDefault();

                    string InsertQuery = string.Format("insert into EXP_SHIFT_PERS (ESHH_ESHH_ID,SCSU_ROW_NO,FIRS_DATE,EPPS_STAT,ESHG_ESHG_ID) " +
                                                      "select {0},SCSU_ROW_NO,'{1}',EPPS_STAT,ESHG_ESHG_ID from EXP_SHIFT_PERS where ESHH_ESHH_ID={2}", New_ESHH_ID, objecttemp.ESHH_DATE, objecttemp.ESHH_ID
                        );

                    Db.Database.ExecuteSqlCommand(InsertQuery);


                }

                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات  ") }.ToJson();
            }
        }

        public ActionResult insert_shift_head(EXP_SHIFT_HEAD objecttemp)
        {
            DateTime d = DateTime.Today;
            int Agonumday = 0, Nextnumday = 0;
            string day = "";
            string[] UserList = new string[] { "S-KHADEMI", "M-LASHKARI", "ME-TORABI", "M-SABAEI", "M-RANJBARIAN", "A-MANARI" };

            string today = Db.Database.SqlQuery<string>(string.Format("SELECT farsi_date_u(to_date('{0}','mm/dd/yyyy')) FROM DUAL", d.ToShortDateString())).FirstOrDefault().ToString();
            //if (today.CompareTo(objecttemp.ESHH_DATE) >= 0 || objecttemp.GROP_GROP_ID == 1341)
            //{
            //    day = string.Format("select FDAYS_BETWEEN_U('{0}','{1}') from dual ", objecttemp.ESHH_DATE, today);
            //}
            //else
            //{
            //    // day = string.Format("select FDAYS_BETWEEN_U('{0}','{1}') from dual ", today, objecttemp.ESHH_DATE);
            //    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("زمان وارد شده بزرگتر از  تاریخ  جاری  نمی تواند باشد  ") }.ToJson();
            //}
            day = string.Format("select FDAYS_BETWEEN_U('{0}','{1}') from dual ", objecttemp.ESHH_DATE, today);
            Agonumday = Db.Database.SqlQuery<int>(day).FirstOrDefault();
            Nextnumday = Db.Database.SqlQuery<int>(string.Format("select FDAYS_BETWEEN_U('{0}','{1}') from dual ", today, objecttemp.ESHH_DATE)).FirstOrDefault();

            if ((Agonumday <= 1 && Nextnumday == 0) || UserList.Contains(username) || objecttemp.GROP_GROP_ID == 1341)
            {
                Db.EXP_SHIFT_HEAD.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            else
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("زمان وارد شده بیشتر از محدوده تاریخ 24 ساعته است ") }.ToJson();
            }
        }

        public ActionResult insert_shift_pers(EXP_SHIFT_PERS objecttemp, int? EPOL_EPOL_ID)
        {
            List<string> errors = new List<string>();
            string sql, row = "";
            row = Request.Form["rownum"];
            int usercount = int.Parse(row); //Db.SEC_USER_TYPE_POST.Where(xx => xx.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID).Where(xx => xx.ETDO_ETDO_ID == 463).Count();

            for (int j = 1; j <= usercount; j++)
            {


                if (Db.EXP_SHIFT_PERS.Where(xx => xx.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID && xx.ESHH_ESHH_ID == objecttemp.ESHH_ESHH_ID && xx.ESTY_ESTY_ID == 121).Any() || Request.Form["ESTY_ESTY_ID_" + objecttemp.EPOL_EPOL_ID + "_" + j] == "121")
                {
                    try
                    {

                        if (!string.IsNullOrEmpty(Request.Form["ESTY_ESTY_ID_" + objecttemp.EPOL_EPOL_ID + "_" + j]))
                        {
                            string Sql = string.Format("select epps_id from exp_shift_pers where EPPS_TYPE=2 and (firs_date <= '{0}'  and end_date>='{0}') and SCSU_ROW_NO={1} and epol_epol_id={2} and (epps_stat=4 or epps_stat=14)", objecttemp.FIRS_DATE, Request.Form["SCSU_ROW_NO_" + objecttemp.EPOL_EPOL_ID + "_" + j], objecttemp.EPOL_EPOL_ID);
                            int Epps_Id = Db.Database.SqlQuery<int>(Sql).FirstOrDefault();
                            if (Epps_Id == 0 || Request.Form["EPPS_STAT_" + objecttemp.EPOL_EPOL_ID + "_" + j] == "4")
                            {
                                sql = string.Format("insert into EXP_SHIFT_PERS (EPOL_EPOL_ID,ESHH_ESHH_ID,SCSU_ROW_NO,EPPS_STAT,ESTY_ESTY_ID,FIRS_DATE,REPL_TYPE,EPPS_REQU,EPPS_DESC) values ({0},{1},{2},{3},{4},'{5}','{6}','{7}','{8}')",
                                    objecttemp.EPOL_EPOL_ID, objecttemp.ESHH_ESHH_ID, Request.Form["SCSU_ROW_NO_" + objecttemp.EPOL_EPOL_ID + "_" + j], Request.Form["EPPS_STAT_" + objecttemp.EPOL_EPOL_ID + "_" + j],
                                    Request.Form["ESTY_ESTY_ID_" + objecttemp.EPOL_EPOL_ID + "_" + j], objecttemp.FIRS_DATE, Request.Form["REPL_TYPE_" + objecttemp.EPOL_EPOL_ID + "_" + j],
                                    Request.Form["EPPS_REQU_" + objecttemp.EPOL_EPOL_ID + "_" + j], Request.Form["EPPS_DESC_" + objecttemp.EPOL_EPOL_ID + "_" + j]);
                                Db.Database.ExecuteSqlCommand(sql);
                            }
                            else { errors.Add(" اطلاعات شیفت کاری   " + Request.Form["FAML_NAME_" + j] + " به دلیل ثبت رکورد مرخصی ثبت نشد  "); }
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(" اطلاعات شیفت کاری   " + Request.Form["FAML_NAME_" + j] + "  ثبت نشد  ");
                    }
                }
                else { errors.Add("قبل از ثبت اطلاعات شب کار شما میبایست یک ردیف روزکار ثبت کرده باشین "); }
            }
            try
            {
                string DeleteSql = string.Format("delete from  EXP_SHIFT_POST_COMMENT where  EPOL_EPOL_ID={0} and ESHH_ESHH_ID={1}", objecttemp.EPOL_EPOL_ID, objecttemp.ESHH_ESHH_ID);
                Db.Database.ExecuteSqlCommand(DeleteSql);

            }
            catch (Exception ex)
            {
                errors.Add(" خطا در حذف اطلاعات ");

            }
            Db.Database.ExecuteSqlCommand(string.Format("insert into  EXP_SHIFT_POST_COMMENT (EPOL_EPOL_ID,ESHH_ESHH_ID,ESPC_COMMENT) values ({0} ,{1} , '{2}')", objecttemp.EPOL_EPOL_ID, objecttemp.ESHH_ESHH_ID, Request.Form["ESPC_COMMENT"]));


            if (errors.Count == 0)
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            else
            {
                string errorMessages = string.Join("<br />", errors.ToArray());
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errorMessages }.ToJson();
            }
        }
        public ActionResult insert_shift_pers_keshik(EXP_SHIFT_PERS objecttemp)
        {
            List<string> errors = new List<string>();
            string sql, row = "", EANA_ROW = "";
            row = Request.Form["rownum"];
            EANA_ROW = Request.Form["EANA_ROW"];
            int usercount = int.Parse(row); //Db.SEC_USER_TYPE_POST.Where(xx => xx.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID).Where(xx => xx.ETDO_ETDO_ID == 463).Count();
            for (int j = 1; j <= usercount; j++)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Request.Form["Check_" + EANA_ROW + "_" + j]))
                    {
                        sql = string.Format("insert into EXP_SHIFT_PERS (ESHH_ESHH_ID,SCSU_ROW_NO,EPPS_STAT,ESHG_ESHG_ID,FIRS_DATE) values ({0},{1},{2},{3},'{4}')",
                             objecttemp.ESHH_ESHH_ID,
                             Request.Form["SCSU_ROW_NO_" + EANA_ROW + "_" + j],
                             Request.Form["EPPS_STAT_" + EANA_ROW + "_" + j],
                             Request.Form["ESHG_ESHG_ID_" + EANA_ROW + "_" + j],
                             objecttemp.FIRS_DATE);
                        Db.Database.ExecuteSqlCommand(sql);
                    }



                }
                catch (Exception ex)
                {
                    errors.Add(" اطلاعات شیفت کاری   " + Request.Form["FAML_NAME_" + j] + "  ثبت نشد  ");
                }
            }

            if (errors.Count == 0)
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            else
            {
                string errorMessages = string.Join("<br />", errors.ToArray());
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errorMessages }.ToJson();
            }
        }
        public ActionResult insert_shift_pers_orga(EXP_SHIFT_PERS objecttemp)
        {
            List<string> errors = new List<string>();
            string sql, row = "";
            row = Request.Form["rownum"];
            int usercount = int.Parse(row); //Db.SEC_USER_TYPE_POST.Where(xx => xx.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID).Where(xx => xx.ETDO_ETDO_ID == 463).Count();
            for (int j = 1; j <= usercount; j++)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Request.Form["EPOL_ID" + "_" + j]))
                    {
                        sql = string.Format("insert into EXP_SHIFT_PERS (ESHH_ESHH_ID,SCSU_ROW_NO,EPPS_STAT,ESTY_ESTY_ID,FIRS_DATE,REPL_TYPE,EPPS_REQU,EPPS_PLAC,EPOL_EPOL_ID) values ({0},{1},{2},{3},'{4}','{5}','{6}','{7}','{8}')",
                             objecttemp.ESHH_ESHH_ID,
                             Request.Form["SCSU_ROW_NO" + "_" + j],
                             Request.Form["EPPS_STAT" + "_" + j],
                             Request.Form["ESTY_ESTY_ID" + "_" + j],
                             objecttemp.FIRS_DATE,
                             Request.Form["REPL_TYPE" + "_" + j],
                             Request.Form["EPPS_REQU" + "_" + j],
                             Request.Form["EPPS_PLAC" + "_" + j],
                             Request.Form["EPOL_ID" + "_" + j]);
                    }
                    else if (!string.IsNullOrEmpty(Request.Form["ORGA_CODE" + "_" + j]))
                    {
                        sql = string.Format("insert into EXP_SHIFT_PERS (ESHH_ESHH_ID,SCSU_ROW_NO,EPPS_STAT,ESTY_ESTY_ID,FIRS_DATE,REPL_TYPE,EPPS_REQU,EPPS_PLAC,ORGA_MANA_ASTA_CODE,ORGA_MANA_CODE,ORGA_CODE) values ({0},{1},{2},{3},'{4}','{5}','{6}','{7}',7,6,{8})",
                             objecttemp.ESHH_ESHH_ID,
                             Request.Form["SCSU_ROW_NO" + "_" + j],
                             Request.Form["EPPS_STAT" + "_" + j],
                             Request.Form["ESTY_ESTY_ID" + "_" + j],
                             objecttemp.FIRS_DATE,
                             Request.Form["REPL_TYPE" + "_" + j],
                             Request.Form["EPPS_REQU" + "_" + j],
                             Request.Form["EPPS_PLAC" + "_" + j],
                             Request.Form["ORGA_CODE" + "_" + j]);
                    }
                    else if (!string.IsNullOrEmpty(Request.Form["ESHG_ESHG_ID" + "_" + j]))
                    {
                        sql = string.Format("insert into EXP_SHIFT_PERS (ESHH_ESHH_ID,SCSU_ROW_NO,EPPS_STAT,ESTY_ESTY_ID,FIRS_DATE,REPL_TYPE,EPPS_REQU,EPPS_PLAC,ORGA_MANA_ASTA_CODE,ORGA_MANA_CODE,ORGA_CODE) values ({0},{1},{2},{3},'{4}','{5}','{6}','{7}',7,6,{8})",
                             objecttemp.ESHH_ESHH_ID,
                             Request.Form["SCSU_ROW_NO" + "_" + j],
                             Request.Form["EPPS_STAT" + "_" + j],
                             Request.Form["ESHG_ESHG_ID" + "_" + j],
                             objecttemp.FIRS_DATE)                             //Request.Form["REPL_TYPE" + "_" + j],
                                                                               // Request.Form["EPPS_REQU" + "_" + j],
                                                                               //Request.Form["EPPS_PLAC" + "_" + j],
                                                                               // Request.Form["ORGA_CODE" + "_" + j])
                            ;
                    }
                    else
                    {
                        sql = string.Format("insert into EXP_SHIFT_PERS (ESHH_ESHH_ID,SCSU_ROW_NO,EPPS_STAT,ESTY_ESTY_ID,FIRS_DATE,REPL_TYPE,EPPS_REQU,EPPS_PLAC) values ({0},{1},{2},{3},'{4}','{5}','{6}','{7}')",
                         objecttemp.ESHH_ESHH_ID,
                         Request.Form["SCSU_ROW_NO" + "_" + j],
                         Request.Form["EPPS_STAT" + "_" + j],
                         Request.Form["ESTY_ESTY_ID" + "_" + j],
                         objecttemp.FIRS_DATE,
                         Request.Form["REPL_TYPE" + "_" + j],
                         Request.Form["EPPS_REQU" + "_" + j],
                         Request.Form["EPPS_PLAC" + "_" + j]);
                    }

                    Db.Database.ExecuteSqlCommand(sql);
                }
                catch (Exception ex)
                {
                    errors.Add(" اطلاعات شیفت کاری   " + Request.Form["FAML_NAME_" + j] + "  ثبت نشد  ");
                }
            }

            if (errors.Count == 0)
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            else
            {
                string errorMessages = string.Join("<br />", errors.ToArray());
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errorMessages }.ToJson();
            }
        }

        public ActionResult insert_shift_add_pers(EXP_SHIFT_PERS objecttemp)
        {
            string sql = "";
            try
            {
                sql = string.Format("insert into EXP_SHIFT_PERS (EPOL_EPOL_ID,ESHH_ESHH_ID,SCSU_ROW_NO,EPPS_STAT,ESTY_ESTY_ID,FIRS_DATE,EPPS_EPPS_ID,EPPS_REQU,EPPS_DESC) values ({0},{1},{2},{3},{4},'{5}',{6},{7},'{8}')",
                    objecttemp.EPOL_EPOL_ID, objecttemp.ESHH_ESHH_ID, Request.Form["SCSU_ROW_NO"], Request.Form["EPPS_STAT"], Request.Form["ESTY_ESTY_ID"], objecttemp.FIRS_DATE, Request.Form["EPPS_EPPS_ID"], Request.Form["EPPS_REQU"], Request.Form["EPPS_DESC"]);
                Db.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات" }.ToJson();
            }

            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
        }


        public ActionResult insert_shift_add_pers_offdate(EXP_SHIFT_PERS objecttemp)
        {
            string sql = "";
            sql = string.Format("select EPPS_ID from EXP_SHIFT_PERS where scsu_row_no={0} and firs_date>='{1}' and firs_date<='{2}' and epps_stat!=4", Request.Form["SCSU_ROW_NO"], objecttemp.FIRS_DATE, objecttemp.END_DATE);

            if (!Db.Database.SqlQuery<int>(sql).Any())
            {
                try
                {
                    sql = string.Format("insert into EXP_SHIFT_PERS (EPOL_EPOL_ID,SCSU_ROW_NO,FIRS_DATE,EPPS_DESC,EPPS_TYPE,END_DATE,EPPS_STAT,ESTY_ESTY_ID) values ({0},{1},'{2}','{3}',2,'{4}','{5}',{6})",
                        objecttemp.EPOL_EPOL_ID, Request.Form["SCSU_ROW_NO"], objecttemp.FIRS_DATE, Request.Form["EPPS_DESC"], objecttemp.END_DATE, objecttemp.EPPS_STAT, objecttemp.ESTY_ESTY_ID);
                    Db.Database.ExecuteSqlCommand(sql);
                }
                catch (Exception ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات" }.ToJson();
                }

                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            else
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "!برای این کاربر در این بازه زمانی ردیف اطلاعاتی ثبت شده است" }.ToJson();

            }
        }

        public ActionResult insert_overtime(EXP_OVER_TIME objecttemp)
        {
            //string sql = "";

            try
            {
                var query = from b in Db.EXP_OVER_TIME
                            where b.SCSU_ROW_NO == objecttemp.SCSU_ROW_NO && b.EOTI_YEAR == objecttemp.EOTI_YEAR && b.EOTI_MONT == objecttemp.EOTI_MONT
                            select b;
                if (!query.Any())
                {
                    Db.EXP_OVER_TIME.Add(objecttemp);
                    Db.SaveChanges();
                }
                else
                {
                    query.FirstOrDefault().EOTI_OVER_TIME = objecttemp.EOTI_OVER_TIME;
                    query.FirstOrDefault().EOTI_OVER_TIME_PAYED = objecttemp.EOTI_OVER_TIME_PAYED;
                    Db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات" }.ToJson();
            }

            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
        }

        public ActionResult insert_learn_prsn(EXP_LEARN_PRSN objecttemp)
        {
            try
            {
                Db.EXP_LEARN_PRSN.Add(objecttemp);
                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات" }.ToJson();
            }

            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
        }

        public ActionResult Get_Head([DataSourceRequest] DataSourceRequest request, int? GROP_ID, short? EDCH_TYPE)
        {
            var query = (from p in Db.EXP_SHIFT_HEAD
                         join q in Db.SEC_USERS on p.CRET_BY equals q.ORCL_NAME
                         join v in Db.EXP_GROUPS on p.GROP_GROP_ID equals v.GROP_ID
                         //join z in cntx.PAY_ORGAN on v.ORGA_CODE equals z.CODE
                         ////join x in cntx.EXP_DAILY_CHECKLIST_VAR on p.EDCH_ID equals x.EDCH_EDCH_ID
                         where (p.GROP_GROP_ID == GROP_ID)
                         orderby p.ESHH_DATE descending, p.ESHH_ID descending
                         select new
                         {
                             p.ESHH_ID,
                             p.ESHH_DATE,
                             p.ESHH_TYPE,
                             GROP_NAME = v.GROP_DESC,
                             GROP_ID = v.GROP_ID,
                             p.ESHH_DESC,
                             COUNT = Db.EXP_SHIFT_PERS.Where(xx => xx.ESHH_ESHH_ID == p.ESHH_ID).Count(),
                             FAML_NAME = (q.FIRS_NAME != null ? q.FIRS_NAME : q.PAY_PERSONEL.FIRS_NAME) + " " + (q.FAML_NAME != null ? q.FAML_NAME : q.PAY_PERSONEL.FAML_NAME)

                         }).ToList();
            //if (userid == 353 || userid == 372 || userid == 516)
            //{
            //    query = (from p in cntx.EXP_SHIFT_HEAD
            //             join q in cntx.SEC_USERS on p.CRET_BY equals q.ORCL_NAME
            //             //join v in cntx.EXP_POST_LINE on p.EPOL_EPOL_ID equals v.EPOL_ID
            //             //join z in cntx.PAY_ORGAN on v.ORGA_CODE equals z.CODE
            //             //where (p.EPOL_EPOL_ID == EPOL_ID || EPOL_ID == null)
            //             orderby p.ESHH_DATE descending, p.ESHH_ID descending
            //             select new
            //             {
            //                 p.ESHH_ID,
            //                 p.ESHH_DATE,
            //                 p.ESHH_TYPE,
            //              //   EPOL_NAME = v.EPOL_NAME,
            //              //   ORGA_DESC = z.ORGA_DESC,
            //                 p.ESHH_DESC,
            //                 COUNT = cntx.EXP_SHIFT_PERS.Where(xx => xx.ESHH_ESHH_ID == p.ESHH_ID).Count(),
            //                 FAML_NAME = (q.FIRS_NAME != null ? q.FIRS_NAME : q.PAY_PERSONEL.FIRS_NAME) + " " + (q.FAML_NAME != null ? q.FAML_NAME : q.PAY_PERSONEL.FAML_NAME)
            //             }).ToList();
            //}

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Get_Learn([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from p in Db.EXP_LEARN_PRSN
                         join q in Db.SEC_USERS on p.SCSU_ROW_NO equals q.ROW_NO
                         orderby q.FAML_NAME
                         select new
                         {
                             p.ELPR_ID,
                             p.ELPR_EDATE,
                             p.ELPR_SDATE,
                             p.ELPR_NAME,
                             p.ELPR_CODE,
                             p.ELPR_HOURS,
                             FAML_NAME = (q.FIRS_NAME != null ? q.FIRS_NAME : q.PAY_PERSONEL.FIRS_NAME) + " " + (q.FAML_NAME != null ? q.FAML_NAME : q.PAY_PERSONEL.FAML_NAME),
                             ASTA_DESC = p.PAY_ASSISTANT.ASTA_DESC
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Get_Shift([DataSourceRequest] DataSourceRequest request, int? ESHH_ID, int? EPOL_ID)
        {
            var query = (from p in Db.EXP_SHIFT_HEAD
                         join x in Db.EXP_SHIFT_PERS on p.ESHH_ID equals x.ESHH_ESHH_ID
                         join q in Db.SEC_USERS on x.SCSU_ROW_NO equals q.ROW_NO
                         join z in Db.EXP_SHIFT_PERS on x.EPPS_ID equals z.EPPS_EPPS_ID into ps
                         from z in ps.DefaultIfEmpty()
                         join t in Db.EXP_SHIFT_GROUP_PRSN on q.ROW_NO equals t.SCSU_ROW_NO into sgp
                         from t in sgp.DefaultIfEmpty()
                         where (p.ESHH_ID == ESHH_ID && x.EPOL_EPOL_ID == EPOL_ID)
                         orderby q.FAML_NAME
                         select new
                         {
                             p.ESHH_ID,
                             p.ESHH_DATE,
                             p.ESHH_TYPE,
                             p.ESHH_DESC,
                             COUNT = Db.EXP_SHIFT_PERS.Where(xx => xx.ESHH_ESHH_ID == p.ESHH_ID).Count(),
                             FAML_NAME = (q.FIRS_NAME != null ? q.FIRS_NAME : q.PAY_PERSONEL.FIRS_NAME) + " " + (q.FAML_NAME != null ? q.FAML_NAME : q.PAY_PERSONEL.FAML_NAME),
                             x.EPPS_STAT,
                             x.EPPS_ID,
                             TYPE_DESC = x.EXP_SHIFT_TYPE.ESTY_DESC,
                             GROUP_DESC = t.EXP_SHIFT_GROUPS.ESHG_DESC,
                             x.EPOL_EPOL_ID,
                             FAML_NAME_R = (z.SEC_USERS.FIRS_NAME != null ? z.SEC_USERS.FIRS_NAME : z.SEC_USERS.PAY_PERSONEL.FIRS_NAME) + " " + (z.SEC_USERS.FAML_NAME != null ? z.SEC_USERS.FAML_NAME : z.SEC_USERS.PAY_PERSONEL.FAML_NAME),
                             x.ESTY_ESTY_ID,
                             x.EPPS_REQU,
                             x.CRET_BY,
                             x.CRET_DATE,
                             x.EPPS_DESC,
                             q.ORCL_NAME
                         }).ToList();

            var Query = query.Select(a => new
            {
                a.ESHH_ID,
                a.ESHH_DATE,
                a.ESHH_TYPE,
                a.ESHH_DESC,
                a.COUNT,
                a.FAML_NAME,
                a.EPPS_STAT,
                a.EPPS_ID,
                a.TYPE_DESC,
                a.GROUP_DESC,
                a.EPOL_EPOL_ID,
                a.FAML_NAME_R,
                a.ESTY_ESTY_ID,
                a.EPPS_REQU,
                a.CRET_BY,
                CRETDATE = GetDate(a.CRET_DATE),
                CretTime = a.CRET_DATE.ToShortTimeString(),
                a.EPPS_DESC,
                a.ORCL_NAME
            }).ToList();

            return Json(Query.ToDataSourceResult(request));
        }
        //to use in dashboard page
        public ActionResult Get_Shift_Post([DataSourceRequest] DataSourceRequest request, string Date, int? EPOL_ID)
        {
            var query = (from p in Db.EXP_SHIFT_HEAD
                         join x in Db.EXP_SHIFT_PERS on p.ESHH_ID equals x.ESHH_ESHH_ID
                         join q in Db.SEC_USERS on x.SCSU_ROW_NO equals q.ROW_NO
                         join z in Db.EXP_SHIFT_PERS on x.EPPS_ID equals z.EPPS_EPPS_ID into ps
                         from z in ps.DefaultIfEmpty()
                         join t in Db.EXP_SHIFT_GROUP_PRSN on q.ROW_NO equals t.SCSU_ROW_NO into sgp
                         from t in sgp.DefaultIfEmpty()
                         where (p.ESHH_DATE == Date && x.EPOL_EPOL_ID == EPOL_ID)
                         orderby q.FAML_NAME
                         select new
                         {
                             p.ESHH_ID,
                             p.ESHH_DATE,
                             p.ESHH_TYPE,
                             p.ESHH_DESC,
                             COUNT = Db.EXP_SHIFT_PERS.Where(xx => xx.ESHH_ESHH_ID == p.ESHH_ID).Count(),
                             FAML_NAME = (q.FIRS_NAME != null ? q.FIRS_NAME : q.PAY_PERSONEL.FIRS_NAME) + " " + (q.FAML_NAME != null ? q.FAML_NAME : q.PAY_PERSONEL.FAML_NAME),
                             x.EPPS_STAT,
                             x.EPPS_ID,
                             TYPE_DESC = x.EXP_SHIFT_TYPE.ESTY_DESC,
                             GROUP_DESC = t.EXP_SHIFT_GROUPS.ESHG_DESC,
                             x.EPOL_EPOL_ID,
                             FAML_NAME_R = (z.SEC_USERS.FIRS_NAME != null ? z.SEC_USERS.FIRS_NAME : z.SEC_USERS.PAY_PERSONEL.FIRS_NAME) + " " + (z.SEC_USERS.FAML_NAME != null ? z.SEC_USERS.FAML_NAME : z.SEC_USERS.PAY_PERSONEL.FAML_NAME),
                             x.ESTY_ESTY_ID,
                             x.EPPS_REQU,
                             x.CRET_BY,
                             x.CRET_DATE,
                             x.EPPS_DESC,
                             q.ORCL_NAME
                         }).ToList();

            var Query = query.Select(a => new
            {
                a.ESHH_ID,
                a.ESHH_DATE,
                a.ESHH_TYPE,
                a.ESHH_DESC,
                a.COUNT,
                a.FAML_NAME,
                a.EPPS_STAT,
                a.EPPS_ID,
                a.TYPE_DESC,
                a.GROUP_DESC,
                a.EPOL_EPOL_ID,
                a.FAML_NAME_R,
                a.ESTY_ESTY_ID,
                a.EPPS_REQU,
                a.CRET_BY,
                CRETDATE = GetDate(a.CRET_DATE),
                CretTime = a.CRET_DATE.ToShortTimeString(),
                a.EPPS_DESC,
                a.ORCL_NAME
            }).ToList();

            return Json(Query.ToDataSourceResult(request));
        }
        public ActionResult Get_Shift_OffDate([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from x in Db.EXP_SHIFT_PERS
                         join q in Db.SEC_USERS on x.SCSU_ROW_NO equals q.ROW_NO
                         where (x.EPPS_TYPE == "2")

                         select new
                         {
                             FAML_NAME = (q.FIRS_NAME != null ? q.FIRS_NAME : q.PAY_PERSONEL.FIRS_NAME) + " " + (q.FAML_NAME != null ? q.FAML_NAME : q.PAY_PERSONEL.FAML_NAME),
                             x.EPPS_ID,
                             x.EPOL_EPOL_ID,
                             EPOL_NAME = x.EXP_POST_LINE.EPOL_NAME,
                             GROP_DESC = x.EXP_POST_LINE.EXP_POST_GROUP.Where(xx => xx.EXP_GROUPS.GROP_CODE == 88).Select(xx => xx.EXP_GROUPS.GROP_DESC).FirstOrDefault(),
                             x.CRET_BY,
                             x.EPPS_DESC,
                             x.FIRS_DATE,
                             x.END_DATE,
                             x.EPPS_STAT,
                             TYPE_DESC = x.EXP_SHIFT_TYPE.ESTY_DESC
                             //TYPE_DESC =GetShiftType(x.ESTY_ESTY_ID),
                         }).OrderByDescending(xx => xx.EPPS_ID).ToList();


            return Json(query.ToDataSourceResult(request));
        }

        public string GetDate(DateTime CRET_DATE)
        {
            string Sql = string.Format("SELECT farsi_date_u(to_date('{0}','mm/dd/yyyy')) FROM DUAL", CRET_DATE.ToShortDateString());
            string date = Db.Database.SqlQuery<string>(Sql).FirstOrDefault().ToString();
            return date;
        }
        public string GetShiftType(int? ESTY_ESTY_ID)
        {

            return Db.EXP_SHIFT_TYPE.Where(xx => xx.ESTY_ID == ESTY_ESTY_ID).Select(xx => xx.ESTY_DESC).FirstOrDefault();
        }

        public ActionResult Get_Shift_Orga([DataSourceRequest] DataSourceRequest request, int? ESHH_ID, int? EPOL_ID)
        {
            var query = (from p in Db.EXP_SHIFT_HEAD
                         join x in Db.EXP_SHIFT_PERS on p.ESHH_ID equals x.ESHH_ESHH_ID
                         join q in Db.SEC_USERS on x.SCSU_ROW_NO equals q.ROW_NO
                         where (p.ESHH_ID == ESHH_ID)
                         orderby q.FAML_NAME
                         select new
                         {
                             p.ESHH_ID,
                             p.ESHH_DATE,
                             p.ESHH_TYPE,
                             p.ESHH_DESC,
                             COUNT = Db.EXP_SHIFT_PERS.Where(xx => xx.ESHH_ESHH_ID == p.ESHH_ID).Count(),
                             FAML_NAME = (q.FIRS_NAME != null ? q.FIRS_NAME : q.PAY_PERSONEL.FIRS_NAME) + " " + (q.FAML_NAME != null ? q.FAML_NAME : q.PAY_PERSONEL.FAML_NAME),
                             x.EPPS_STAT,
                             x.EPPS_ID,
                             TYPE_DESC = x.EXP_SHIFT_TYPE.ESTY_DESC,
                             x.EXP_POST_LINE.EPOL_NAME,
                             x.EPOL_EPOL_ID,
                             x.EPPS_PLAC,
                             ORGA_DESC = x.PAY_ORGAN.ORGA_DESC
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Get_Shift_Keshik([DataSourceRequest] DataSourceRequest request, int ESHH_ID)
        {
            var query = (from p in Db.EXP_SHIFT_HEAD
                         join x in Db.EXP_SHIFT_PERS on p.ESHH_ID equals x.ESHH_ESHH_ID
                         join q in Db.SEC_USERS on x.SCSU_ROW_NO equals q.ROW_NO
                         where (p.ESHH_ID == ESHH_ID)
                         orderby q.FAML_NAME
                         select new
                         {
                             p.ESHH_ID,
                             p.ESHH_DATE,
                             p.ESHH_TYPE,
                             p.ESHH_DESC,
                             //COUNT = Db.EXP_SHIFT_PERS.Where(xx => xx.ESHH_ESHH_ID == p.ESHH_ID).Count(),
                             FAML_NAME = (q.FIRS_NAME != null ? q.FIRS_NAME : q.PAY_PERSONEL.FIRS_NAME) + " " + (q.FAML_NAME != null ? q.FAML_NAME : q.PAY_PERSONEL.FAML_NAME),
                             x.EPPS_STAT,
                             x.EPPS_ID,
                             TYPE_DESC = x.EXP_SHIFT_GROUPS.ESHG_DESC,
                             x.EXP_POST_LINE.EPOL_NAME,
                             x.EPOL_EPOL_ID,
                             x.EPPS_PLAC,
                             q.PAY_PERSONEL.MOBIL
                             // ORGA_DESC = x.PAY_ORGAN.ORGA_DESC
                         }).OrderBy(xx => xx.TYPE_DESC).ToList();

            return Json(query.ToDataSourceResult(request));
        }


        public ActionResult Get_Shift_Group([DataSourceRequest] DataSourceRequest request, int? ESHH_ID, int? EPOL_ID)
        {
            var query = (from p in Db.EXP_SHIFT_GROUPS
                         join x in Db.EXP_SHIFT_GROUP_PRSN on p.ESHG_ID equals x.ESHG_ESHG_ID
                         join q in Db.SEC_USERS on x.SCSU_ROW_NO equals q.ROW_NO
                         where (x.EPOL_EPOL_ID == EPOL_ID)
                         orderby q.FAML_NAME
                         select new
                         {
                             x.ESGP_ID,
                             x.START_DATE,
                             x.END_DATE,
                             x.ESGP_STAT,
                             FAML_NAME = (q.FIRS_NAME != null ? q.FIRS_NAME : q.PAY_PERSONEL.FIRS_NAME) + " " + (q.FAML_NAME != null ? q.FAML_NAME : q.PAY_PERSONEL.FAML_NAME),
                             EPOL_NAME = x.EXP_POST_LINE.EPOL_NAME,
                             x.EPOL_EPOL_ID,
                             ESHG_DESC = p.ESHG_DESC,
                             USER_NAME = q.ORCL_NAME
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Get_type()
        {
            var RetVal = (from b in Db.EXP_SHIFT_TYPE
                          orderby b.ESTY_ID
                          select new { b.ESTY_ID, b.ESTY_DESC }).Distinct();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update_Group_shift([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<EXP_SHIFT_GROUPS> EXP_SHIFT_GROUPS)
        {
            if (EXP_SHIFT_GROUPS != null)
            {
                foreach (EXP_SHIFT_GROUPS item in EXP_SHIFT_GROUPS)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            var query = (from p in Db.EXP_SHIFT_GROUPS
                         orderby p.ESHG_DESC
                         select new
                         {
                             p.ESHG_ID,
                             p.ESHG_DESC,
                             p.ESHG_TIME,
                             p.ESGH_STAT,
                             TYPE_DESC = p.EXP_SHIFT_TYPE.ESTY_DESC,
                             p.ESTY_ESTY_ID
                         }).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_groups(int? ESHG_ID)
        {
            var RetVal = (from b in Db.EXP_SHIFT_GROUPS
                          orderby b.ESHG_DESC
                          where b.ESGH_STAT == 1 && (b.ESHG_ID == ESHG_ID || ESHG_ID == null)
                          select new { b.ESHG_ID, b.ESHG_DESC }).Distinct();


            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Get_Date()
        {
            //var Query=(from a in Db.hra)
            var query = (from a in Db.HRA_CALENDAR
                         where a.MONT_FINY_FINY_YEAR.CompareTo("1397") > 0
                         select new
                         {
                             a.MONT_FINY_FINY_YEAR,
                             a.MONT_MONT_CODE,
                             a.CLN_DAY,
                             DateFarsi = a.MONT_FINY_FINY_YEAR + "/" + a.MONT_MONT_CODE + "/" + a.CLN_DAY
                         }).ToList();


            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_Assistant()
        {
            var RetVal = from b in Db.PAY_ASSISTANT
                         orderby b.ASST_DESC
                         select new { b.CODE, b.ASTA_DESC };
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        //~ShiftController()
        //{
        //    //این دستور کانکشن را دیسکانکت میکند
        //    Db.Dispose();
        //}
    }
}

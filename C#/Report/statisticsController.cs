using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Asr.Text;

namespace Equipment.Controllers.Report
{
    public class statisticsController : DbController
    {
        //BandarEntities Db;
        //
        // GET: /statistics/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult Index3()
        {
            return View();
        }

        public ActionResult GetBehsadId()
        {
            return View();
        }

        public ActionResult SendStaticPostInfo()
        {
            return View();
        }

        public ActionResult GetSetBehsadId()
        {
            return View();
        }

        public ActionResult GetSetBehsadLineId()
        {
            return View();
        }

        public ActionResult SendStaticlineInfo()
        {
            return View();
        }

        public ActionResult Organlinebe(string code)
        {
            var RetVal = (from b in Db.EXP_BAHSAD_INFO_S
                          where b.EBAS_PARENT_ID == code
                          select new { b.BHSD_ID, b.EBAS_DESC });
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Organbe(int? code)
        {
            string t = string.Empty;

            if (code == 1) { t = "1513"; }
            else if (code == 62) { t = "1514"; }
            else if (code == 63) { t = "1515"; }
            else if (code == 64) { t = "1516"; }
            var RetVal = (from b in Db.EXP_BAHSAD_INFO_S
                          where b.EBAS_PARENT_ID == t
                          select new { b.BHSD_ID, b.EBAS_DESC });
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }


        public ActionResult linetype()
        {
            var RetVal = (from b in Db.EXP_BAHSAD_INFO_S
                          where b.EBAS_PARENT_ID == "11017"
                          select new { b.BHSD_ID, b.EBAS_DESC });
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getpostbe(string code)
        {
            var RetVal = (from b in Db.EXP_BAHSAD_INFO_S
                          where b.EBAS_PARENT_ID == code
                          select new { b.BHSD_ID, b.EBAS_DESC });
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetInfo([DataSourceRequest] DataSourceRequest request, int? postrow)
        // {    
        //     var query = (from b in Db.EXP_POST_LINE join f in Db.EXP_POST_LINE_INSTRU on b.EPOL_ID equals f.EPOL_EPOL_ID
        //                  where b.EBAS_EBAS_ROW == postrow &&( f.EINS_EINS_ID ==2 ||  f.EINS_EINS_ID ==3|| f.EINS_EINS_ID ==4)                      
        //                  select new
        //                  {
        //                      f.EPIU_ID,f.EBAS_EBAS_ROW,f.CODE_NAME,f.CODE_DISP
        //                  }).ToList();
        //     return Json(query.ToDataSourceResult(request));
        // }

        public ActionResult GetInfo([DataSourceRequest] DataSourceRequest request, int? postrow)
        {
            var query = (from b in Db.EXP_BEPT_V
                         where b.EBAS_ROW == postrow && b.P_EBAS_EBAS_ROW != null
                         select new
                         {
                             b.P_EBAS_EBAS_ROW,
                             b.P_CODE_DISP,
                             b.P_CODE_NAME,
                             b.P_EPIU_ID,
                             b.V1_V,
                             b.V2_V,
                             b.V3_V,
                             b.V4_V,
                             b.V5_V,
                             b.V6_V,
                             b.V7_V,
                             b.V8_V,
                             b.V9_V,
                             b.V10_V,
                             b.V3_PID,
                             b.V4_PID,
                             b.V5_PID,
                             z = b.V6_V + b.V7_V + b.V10_V,
                             dateb = b.V2_V + b.V8_V + b.V9_V
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult GetInfot([DataSourceRequest] DataSourceRequest request, int? postinst)
        {
            //if (postrow != null)
            //{
            //    Session["Id"] = aocrdc;
            //}
            //int id = Convert.ToInt32(Session["Id"]);

            var query = (from b in Db.EXP_BEIN_V
                         where b.EPIUID == postinst
                         select new
                         {
                             b.BHSDID,
                             b.EBASDESC,
                             b.EBPFDESC,
                             b.EBPIDESC,
                             b.EBPRDESC,
                             b.EBPRROW,
                             b.EBPUDESC,
                             b.EBPVVALUE,
                             b.EPIUID
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        private int insertattrbuteinst(int beinst, int epui_id, string type, string utype)
        {
            int check = 0;
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();

            string MOUNT = "00";
            if (pc.GetMonth(DateTime.Now) < 10)
            {
                MOUNT = "0" + pc.GetMonth(DateTime.Now).ToString();
            }
            else
            {
                MOUNT = pc.GetMonth(DateTime.Now).ToString();
            }

            string day = "00";
            if (pc.GetDayOfMonth(DateTime.Now) < 10)
            {
                day = "0" + pc.GetDayOfMonth(DateTime.Now).ToString();
            }
            else
            {
                day = pc.GetDayOfMonth(DateTime.Now).ToString();
            }

            var qattr = from b in Db.EXP_BAHSAD_PROPERTY where b.EBPR_FID == type select new { b.EBPR_ID, b.EBPR_ROW };
            foreach (var pid in qattr)
            {
                //decimal? valid = null;
                var qp = from b in Db.EXP_BAHSAD_PROPERTY where b.EBPR_ID == pid.EBPR_ID select b.QUERY_EQUIP;
                var val = Db.Database.SqlQuery<string>(qp.FirstOrDefault().ToString() + epui_id.ToString()).FirstOrDefault();

                //var val = Db.Database.SqlQuery<string>(pid.QUERY_EQUIP.FirstOrDefault().ToString() + epui_id.ToString()).FirstOrDefault();
                EXP_BAHSAD_PROPERTY_V probehsad = new EXP_BAHSAD_PROPERTY_V();
                probehsad.EBPR_EBPR_ROW = pid.EBPR_ROW;
                //689;
                probehsad.EBPV_VALUE = val;
                probehsad.EBAS_EBAS_ROW = beinst;
                probehsad.EBPV_DAY = day;
                probehsad.EBPV_MONT = MOUNT;
                probehsad.EBPV_YEAR = pc.GetYear(DateTime.Now).ToString();
                if (val != null)
                {
                    var qvalid = (from b in Db.EXP_BAHSAD_PITEM where b.EBPR_EBPR_ROW == pid.EBPR_ROW && b.EBPI_QUERY == val select b);
                    if (qvalid.FirstOrDefault() != null)
                    {
                        probehsad.EBPI_EBPI_ROW = qvalid.FirstOrDefault().EBPI_ROW;

                    }
                }
                probehsad.EBPV_STAT = "2";
                Db.EXP_BAHSAD_PROPERTY_V.Add(probehsad);
                Db.SaveChanges();
                check = 0;
            }

            return check;
        }

        private int insertattrbute(int behsad_row, int epol_id)
        {
            int check;
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();

            string MOUNT = "00";
            if (pc.GetMonth(DateTime.Now) < 10)
            {
                MOUNT = "0" + pc.GetMonth(DateTime.Now).ToString();
            }
            else
            {
                MOUNT = pc.GetMonth(DateTime.Now).ToString();
            }

            string day = "00";
            if (pc.GetDayOfMonth(DateTime.Now) < 10)
            {
                day = "0" + pc.GetDayOfMonth(DateTime.Now).ToString();
            }
            else
            {
                day = pc.GetDayOfMonth(DateTime.Now).ToString();
            }
            EXP_BAHSAD_PROPERTY_V probehsad = new EXP_BAHSAD_PROPERTY_V();

            probehsad.EBPR_EBPR_ROW = (from b in Db.EXP_BAHSAD_PROPERTY where b.EBPR_ID == "121" select b.EBPR_ROW).FirstOrDefault();
            //689;
            probehsad.EBPV_VALUE = "109";
            probehsad.EBAS_EBAS_ROW = behsad_row;
            probehsad.EBPV_DAY = day;
            probehsad.EBPV_MONT = MOUNT;
            probehsad.EBPV_YEAR = pc.GetYear(DateTime.Now).ToString();
            probehsad.EBPI_EBPI_ROW = (from b in Db.EXP_BAHSAD_PITEM where b.EBPI_ID == "109" select b.EBPI_ROW).FirstOrDefault();
            //1061;
            probehsad.EBPV_STAT = "2";
            Db.EXP_BAHSAD_PROPERTY_V.Add(probehsad);
            Db.SaveChanges();
            check = 0;

            string[] p = { "192", "381", "191", "343", "341", "221" };

            foreach (var pid in p)
            {
                decimal? valid = null;
                var qp = from b in Db.EXP_BAHSAD_PROPERTY where b.EBPR_ID == pid select b.QUERY_EQUIP;
                var val = Db.Database.SqlQuery<string>(qp.FirstOrDefault().ToString() + epol_id.ToString()).FirstOrDefault();
                EXP_BAHSAD_PROPERTY_V probehsad1 = new EXP_BAHSAD_PROPERTY_V();

                if (pid == "381")
                {
                    probehsad1.EBPU_EBPU_ROW = (from b in Db.EXP_BAHSAD_PUNIT
                                                join c in Db.EXP_BAHSAD_PROPERTY on b.EBPR_EBPR_ROW equals c.EBPR_ROW
                                                where b.EBPU_ID == "161" && c.EBPR_ID == "381"
                                                select b.EBPU_ROW).FirstOrDefault();
                }
                if (pid == "191")
                {
                    if (val == "4") { val = "721"; }
                    else if (val == "1") { val = "126"; }
                    else if (val == "2") { val = "127"; }
                    else if (val == "3") { val = "128"; }
                    else
                    { val = ""; }
                    if (val != "")
                    {
                        valid = (from b in Db.EXP_BAHSAD_PITEM where b.EBPI_ID == val select b.EBPI_ROW).FirstOrDefault();
                    }
                }
                if (pid == "192")
                {
                    var vald = val.Split('/');
                    string posttype = vald[0].ToString();
                    string statdymm = vald[1].ToString();

                    if ((posttype == "0" || posttype == "1" || posttype == "1") && (statdymm == "0"))
                        val = "129";
                    else if (statdymm == "1")
                        val = "130";
                    else if (posttype == "2")
                        val = "501";
                    else if ((posttype == "4") && (statdymm == "0"))
                        val = "131";
                    else if ((posttype == "4") && (statdymm == "1"))
                        val = "701";
                    else if (posttype == "5")
                        val = "801";
                    else
                        val = "";

                    if (val != "")
                    {
                        valid = (from b in Db.EXP_BAHSAD_PITEM where b.EBPI_ID == val select b.EBPI_ROW).FirstOrDefault();
                    }
                }

                int QEBPR_ROW = (from b in Db.EXP_BAHSAD_PROPERTY where b.EBPR_ID == pid select b.EBPR_ROW).FirstOrDefault();

                probehsad1.EBPR_EBPR_ROW = QEBPR_ROW;
                probehsad1.EBPV_VALUE = val;
                probehsad1.EBAS_EBAS_ROW = behsad_row;
                probehsad1.EBPV_DAY = day;
                probehsad1.EBPV_MONT = MOUNT;
                probehsad1.EBPV_YEAR = pc.GetYear(DateTime.Now).ToString();
                probehsad1.EBPI_EBPI_ROW = valid;
                probehsad1.EBPV_STAT = "2";
                Db.EXP_BAHSAD_PROPERTY_V.Add(probehsad1);
                Db.SaveChanges();
                check = 0;
            }
            return check;
        }

        public ActionResult insertbehsadlineid()
        {
            int epol_id = int.Parse(Request.Form["EPOL_EPOL_ID_requ"].ToString());
            string be = Request.Form["bpost"].ToString();
            var qbe = (from b in Db.EXP_BAHSAD_INFO_S where b.BHSD_ID == be select b.EBAS_ROW);
            if (qbe != null)
            {
                int behsad_row = int.Parse(qbe.FirstOrDefault().ToString());
                var q = from b in Db.EXP_POST_LINE where b.EPOL_ID == epol_id && b.EBAS_EBAS_ROW == null select b;
                if (q.FirstOrDefault() != null)
                {
                    q.FirstOrDefault().EBAS_EBAS_ROW = behsad_row;
                    Db.SaveChanges();
                    int statcheck = insertattrbuteinst(behsad_row, epol_id, "5", "1");
                }

                return new ServerMessages(ServerOprationType.Success) { Message = "ثبت شد", }.ToJson();
            }
            else
            {
                return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات بهساد درست در سیستم انتقال نیافته است", }.ToJson();
            }
        }

        public ActionResult insertbehsadid()
        {
            int epol_id = int.Parse(Request.Form["EPOL_EPOL_ID_requ"].ToString());
            string be = Request.Form["bpost"].ToString();
            var qbe = (from b in Db.EXP_BAHSAD_INFO_S where b.BHSD_ID == be select b.EBAS_ROW);
            if (qbe != null)
            {
                int behsad_row = int.Parse(qbe.FirstOrDefault().ToString());
                var q = from b in Db.EXP_POST_LINE where b.EPOL_ID == epol_id && b.EBAS_EBAS_ROW == null select b;
                if (q.FirstOrDefault() != null)
                {
                    q.FirstOrDefault().EBAS_EBAS_ROW = behsad_row;
                    Db.SaveChanges();
                    int statcheck = insertattrbute(behsad_row, epol_id);
                }
                int epui_id = int.Parse(Request.Form["inst"].ToString());
                int beinst = int.Parse(Request.Form["instbe"].ToString());
                string type = Request.Form["insttype"].ToString();

                var qinst = from b in Db.EXP_POST_LINE_INSTRU where b.EPIU_ID == epui_id && b.EBAS_EBAS_ROW == null select b;
                if (qinst.FirstOrDefault() != null)
                {
                    qinst.FirstOrDefault().EBAS_EBAS_ROW = beinst;
                    Db.SaveChanges();
                    int statcheck = insertattrbuteinst(beinst, epui_id, type, null);
                }

                return new ServerMessages(ServerOprationType.Success) { Message = "ثبت شد", }.ToJson();
            }
            else
            {
                return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات بهساد درست در سیستم انتقال نیافته است", }.ToJson();
            }
        }

        public ActionResult InfoPost_New(int? id)
        {
            Session["attid"] = id;
            if (id != 0)
            {
                EXP_BEHV_V type = (from b in Db.EXP_BEHV_V where b.V1_R == id select b).FirstOrDefault();
                return View(type);
            }

            return View();
        }

        public ActionResult ReadPost([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from b in Db.EXP_POBE_V
                         where b.EXP_POST_LINE_EBAS_EBAS_ROW == null && b.EXP_POST_LINE_EPOL_TYPE == "0"
                         select new
                         {
                             b.EXP_POST_LINE_EPOL_ID,
                             b.EXP_POST_LINE_EPOL_NAME,
                             b.EXP_UNIT_LEVEL_EUNL_DESC,
                             b.BKP_GEOGH_LOC_G_DESC,
                             b.EXP_POST_LINE_EXPB_DATE,
                             b.EXP_POST_LINE_EXPL_DATE,
                             b.EXP_POST_LINE_POST_TYPE,
                             b.EXP_POST_LINE_POST_XYKM,
                             b.EXP_OWENER_TYPE_EOTY_DESC,
                             city = "هرمزگان"

                         }).ToList();
            return Json(query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult getpost(int? Unitvolt)
        {
            var RetVal = (from p in Db.EXP_POST_LINE
                          where p.EPOL_TYPE == "0" && p.EPOL_STAT != "3" && p.EUNL_EUNL_ID == Unitvolt
                          orderby p.EPOL_NAME
                          select new { p.EPOL_NAME, p.EPOL_ID });

            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getline(int? groupp)
        {
            var RetVal = (from p in Db.EXP_POST_LINE
                          where p.EPOL_TYPE == "1" && p.ELGR_ELGR_ID == groupp
                          orderby p.EPOL_NAME
                          select new { p.EPOL_NAME, p.EPOL_ID });

            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getgline(int? Unitvolt)
        {
            var RetVal = (from b in Db.EXP_LINE_GROUP
                          join c in Db.EXP_POST_LINE on b.ELGR_ID equals c.ELGR_ELGR_ID
                          where c.EUNL_EUNL_ID == Unitvolt
                          select new { b.ELGR_ID, b.ELGR_DESC }).Distinct();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getinst(int? post, int? type)
        {
            var RetVal = (from b in Db.EXP_POST_LINE_INSTRU
                          where b.EPOL_EPOL_ID == post && b.EINS_EINS_ID == type && b.EBAS_EBAS_ROW == null
                          select new { b.EPIU_ID, b.CODE_NAME }).Distinct();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getinstbe(string code, int? type)
        {
            string insttype = string.Empty;
            if (type == 2) { insttype = "95"; }
            else if (type == 3) { insttype = "97"; }
            else if (type == 4) { insttype = "96"; }

            var RetVal = (from b in Db.EXP_BAHSAD_INFO_S
                          where b.EBAS_PARENT_ID == code && b.WTYPE_ID == insttype
                          select new { b.EBAS_ROW, b.EBAS_DESC });

            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUnitLevel()
        {
            var query = (from b in Db.EXP_UNIT_LEVEL
                         join p in Db.EXP_POST_LINE on b.EUNL_ID equals p.EUNL_EUNL_ID
                         orderby b.EUNL_NUM descending
                         where b.EUNL_NUM != 20
                         select new { b.EUNL_ID, b.EUNL_NUM }).Distinct().OrderByDescending(b => b.EUNL_NUM);

            return Json(query, JsonRequestBehavior.AllowGet);
        }


        private string FSetposttype(string p)
        {
            return (from b in Db.EXP_BAHSAD_PITEM where b.EBPI_ID == p select b.EBPI_DESC).FirstOrDefault();
        }


        public ActionResult ReadBEHV([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from b in Db.EXP_BEHV_V.AsEnumerable()
                         from c in Db.EXP_POST_LINE
                         where b.V1_R == c.EBAS_EBAS_ROW
                         from u in Db.EXP_UNIT_LEVEL
                         where c.EUNL_EUNL_ID == u.EUNL_ID
                         select new
                         {
                             b.V1_R,
                             b.V1_D,
                             b.V1_V,
                             b.V2_V,
                             b.V3_V,
                             b.V4_V,
                             b.V5_V,
                             setposttype = FSetposttype(b.V5_V),
                             setpostowner = FSetposttype(b.V7_V),
                             city = "هرمزگان",
                             b.V6_V,
                             b.V7_V,
                             namep = c.EPOL_NAME,
                             volt = u.EUNL_NUM
                         }).ToList();
            return Json(query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GEtInfoPost()
        {
            try
            {
                int[] p = { 121, 192, 381, 191, 343, 341, 221 };

                var q = (from f in Db.EXP_POST_LINE where f.EBAS_EBAS_ROW == null select new { f.EPOL_ID, f.EPOL_NAME });

                //var q = (from f in Db.EXP_POST_LINE
                //        join b in Db.EXP_BAHSAD_INFO_S on f.EBAS_EBAS_ROW equals b.EBAS_ROW
                //        select new{ b.BHSD_ID,b.EBAS_ROW });

                //foreach (var qi in q)
                //{ 
                //var qvb=from b in Db.EXP_BAHSAD_PROPERTY_V join f in Db.EXP_BAHSAD_INFO_S on b.EBAS_EBAS_ROW equals f.EBAS_ROW
                //        where f.BHSD_ID==qi.BHSD_ID
                //        select b;


                //}

            }

            catch (Exception ex)
            {
                string d = ex.Message;
            }

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadBSend([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from b in Db.EXP_BEHV_V.AsEnumerable()
                         from c in Db.EXP_POST_LINE
                         where b.V1_R == c.EBAS_EBAS_ROW
                         from u in Db.EXP_UNIT_LEVEL
                         where c.EUNL_EUNL_ID == u.EUNL_ID
                         where (b.V1_S == "4" && b.V2_S == "4" && b.V3_S == "4" && b.V4_S == "4" && b.V5_S == "4" && b.V6_S == "4" && b.V7_S == "4")
                         select new
                         {
                             b.V1_R,
                             b.V1_D,
                             b.V1_V,
                             b.V2_V,
                             b.V3_V,
                             b.V4_V,
                             b.V5_V,
                             setposttype = FSetposttype(b.V5_V),
                             setpostowner = FSetposttype(b.V7_V),
                             city = "هرمزگان",
                             b.V6_V,
                             b.V7_V,
                             namep = c.EPOL_NAME,
                             volt = u.EUNL_NUM
                         }).ToList();
            return Json(query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInfotb([DataSourceRequest] DataSourceRequest request, string fd)
        {
            bool filterDisable = string.IsNullOrEmpty(fd);
            string filter = string.IsNullOrEmpty(fd.ToUpper()) ? "" : fd.ToUpper().ToArabicUtf8();

            var query = (from b in Db.EXP_BEPT_V
                         where (b.V1_S == "2" || b.V2_S == "2" || b.V3_S == "2" || b.V4_S == "2" || b.V5_S == "2" || b.V6_S == "2" || b.V7_S == "2" || b.V8_S == "2"
                         || b.V9_S == "2" || b.V10_S == "2")
                              && (b.PO_EPOL_NAME.ToUpper().Contains(filter) || filterDisable)

                         select new
                         {
                             b.PO_EPOL_NAME,
                             b.PO_EPOL_ID,
                             b.PO_CODE_DISP,
                             b.P_EBAS_EBAS_ROW,
                             b.P_CODE_DISP,
                             b.P_CODE_NAME,
                             b.P_EPIU_ID,
                             b.V1_V,
                             b.V2_V,
                             b.V3_V,
                             b.V4_V,
                             b.V5_V,
                             b.V6_V,
                             b.V7_V,
                             b.V8_V,
                             b.V9_V,
                             b.V10_V,
                             b.V3_PID,
                             b.V4_PID,
                             b.V5_PID,
                             z = b.V6_V + b.V7_V + b.V10_V,
                             dateb = b.V2_V + b.V8_V + b.V9_V
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult GetInfoget([DataSourceRequest] DataSourceRequest request, string fd)
        {
            bool filterDisable = string.IsNullOrEmpty(fd);
            string filter = string.Empty;
            if (fd != null || fd != "")
            {
                filter = string.IsNullOrEmpty(fd.ToUpper()) ? "" : fd.ToUpper().ToArabicUtf8();
            }

            var query = (from b in Db.EXP_BEPT_V
                         where ((b.V1_S == "4" && b.V2_S == "4" && b.V3_S == "4" && b.V4_S == "4" && b.V5_S == "4" && b.V6_S == "4") || (b.V7_S == "4" && b.V8_S == "4")
                         || (b.V9_S == "4" && b.V10_S == "4"))
                         && (b.PO_EPOL_NAME.ToUpper().Contains(filter) || filterDisable)
                         select new
                         {
                             b.PO_EPOL_NAME,
                             b.PO_EPOL_ID,
                             b.PO_CODE_DISP,
                             b.P_EBAS_EBAS_ROW,
                             b.P_CODE_DISP,
                             b.P_CODE_NAME,
                             b.P_EPIU_ID,
                             b.V1_V,
                             b.V2_V,
                             b.V3_V,
                             b.V4_V,
                             b.V5_V,
                             b.V6_V,
                             b.V7_V,
                             b.V8_V,
                             b.V9_V,
                             b.V10_V,
                             b.V3_PID,
                             b.V4_PID,
                             b.V5_PID,
                             z = b.V6_V + b.V7_V + b.V10_V,
                             dateb = b.V2_V + b.V8_V + b.V9_V
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        private string findstat(string p)
        {
            if (p == null || p == "")
                return "جدید";
            else
                return "تغییر یافته";
        }

        public ActionResult GetInfotupdate([DataSourceRequest] DataSourceRequest request, string fd)
        {
            bool filterDisable = string.IsNullOrEmpty(fd);
            string filter = string.IsNullOrEmpty(fd.ToUpper()) ? "" : fd.ToUpper().ToArabicUtf8();

            var q4 = (from b in Db.EXP_BEPT_V
                      where ((b.V1_S == "4" && b.V2_S == "4" && b.V3_S == "4" && b.V4_S == "4" && b.V5_S == "4" && b.V6_S == "4") || (b.V7_S == "4"
                      && b.V8_S == "4")
                     || (b.V9_S == "4" && b.V10_S == "4"))
                      select b.P_EPIU_ID);

            var query = (from b in Db.EXP_BEPT_V
                         where
                        (((b.V1_S == "3" || b.V1_S == "4") && (b.V2_S == "3" || b.V2_S == "4")
                         && (b.V3_S == "3" || b.V3_S == "4")
                         && (b.V4_S == "3" || b.V4_S == "4")
                         && (b.V5_S == "3" || b.V5_S == "4")
                         && (b.V6_S == "3" || b.V6_S == "4")) ||
                       ((b.V7_S == "3" || b.V7_S == "4") && (b.V8_S == "3" || b.V8_S == "4")) ||
                       ((b.V9_S == "3" || b.V9_S == "4") && (b.V10_S == "3" || b.V10_S == "4"))
                       )
                         && !q4.Contains(b.P_EPIU_ID) && (b.PO_EPOL_NAME.ToUpper().Contains(filter) || filterDisable)
                         select new
                         {
                             b.PO_EPOL_NAME,
                             b.PO_EPOL_ID,
                             b.PO_CODE_DISP,
                             b.P_EBAS_EBAS_ROW,
                             b.P_CODE_DISP,
                             b.P_CODE_NAME,
                             b.P_EPIU_ID,
                             b.V1_V,
                             b.V2_V,
                             b.V3_V,
                             b.V4_V,
                             b.V5_V,
                             b.V6_V,
                             b.V7_V,
                             b.V8_V,
                             b.V9_V,
                             b.V10_V,
                             b.V3_PID,
                             b.V4_PID,
                             b.V5_PID,
                             z = b.V6_V + b.V7_V + b.V10_V,
                             dateb = b.V2_V + b.V8_V + b.V9_V
                         }).ToList();
            return Json(query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadBUpdate([DataSourceRequest] DataSourceRequest request)
        {
            var q4 = (from b in Db.EXP_BEHV_V.AsEnumerable()
                      where (b.V1_S == "4" && b.V2_S == "4" && b.V3_S == "4" && b.V4_S == "4" && b.V5_S == "4" && b.V6_S == "4" && b.V7_S == "4")
                      select b.V1_R);

            var query = (from b in Db.EXP_BEHV_V.AsEnumerable()
                         from c in Db.EXP_POST_LINE
                         where b.V1_R == c.EBAS_EBAS_ROW
                         from u in Db.EXP_UNIT_LEVEL
                         where c.EUNL_EUNL_ID == u.EUNL_ID
                         where (b.V1_S == "3" || b.V1_S == "4")
                         && (b.V2_S == "3" || b.V2_S == "4")
                         && (b.V3_S == "3" || b.V3_S == "4")
                         && (b.V4_S == "3" || b.V4_S == "4")
                         && (b.V5_S == "3" || b.V5_S == "4")
                         && (b.V6_S == "3" || b.V6_S == "4")
                         && (b.V7_S == "3" || b.V7_S == "4")
                         && !q4.Contains(b.V1_R)
                         select new
                         {
                             b.V1_BID,
                             stat = findstat(b.V1_BID),
                             b.V1_R,
                             b.V1_D,
                             b.V1_V,
                             b.V2_V,
                             b.V3_V,
                             b.V4_V,
                             b.V5_V,
                             setposttype = FSetposttype(b.V5_V),
                             setpostowner = FSetposttype(b.V7_V),
                             city = "هرمزگان",
                             b.V6_V,
                             b.V7_V,
                             namep = c.EPOL_NAME,
                             volt = u.EUNL_NUM
                         }).ToList();
            return Json(query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult sendpost(string id)
        {
            try
            {
                List<string> success = new List<string>();
                List<string> error = new List<string>();
                string userservice = "horm02";
                string token = "", AuthenticateMsg = "", UpdateMsg = "";
                long wsCount;
                string um = string.Empty;
                string Ug = string.Empty;
                string UMsg = string.Empty;
                string Mc = string.Empty;
                FieldData[] f;
                PropertyData[] p;
                WorkspaceTypeData[] wt;
                WorkspaceData[] w;
                CycleTypeData[] c;
                CycleData[] g;
                Array[] y;
                Value val = new Value();

                val.Authenticate(userservice, "123456", out AuthenticateMsg, out token);

                if (AuthenticateMsg == "Login Succeeded.")
                {
                    /*

                        c = val.GetCycleTypes(userservice, token, out Mc);
                        foreach (var ct1 in c)
                        {
                            EXP_BAHSAD_CYCLE_T ctype = new EXP_BAHSAD_CYCLE_T();
                            int instl = (from b in Db.EXP_BAHSAD_CYCLE_T
                                         select b.EBCT_ROW).Count();
                            if (instl == 0)
                            { instl = 1; }
                            else
                            {
                                instl = (from b in Db.EXP_BAHSAD_CYCLE_T
                                         select b.EBCT_ROW).Max();
                                instl++;
                            }
                            ctype.EBCT_ROW = instl;
                            ctype.EBCT_ID = ct1.Id.ToString();
                            ctype.EBCT_DESC = ct1.Title;
                            var q = (from b in Db.EXP_BAHSAD_CYCLE_T where b.EBCT_DESC == ctype.EBCT_DESC select b);
                            if (q.FirstOrDefault() == null)
                            {
                                Db.EXP_BAHSAD_CYCLE_T.Add(ctype);
                                Db.SaveChanges();
                            }

                        }
                   
                    g = val.GetCycles(out UMsg);
                    foreach (var g1 in g)
                    {
                        EXP_BAHSAD_CYCLE_D a = new EXP_BAHSAD_CYCLE_D();

                        int instl = (from b in Db.EXP_BAHSAD_CYCLE_D
                                     select b.EBCD_ROW).Count();
                        if (instl == 0)
                        { instl = 1; }
                        else
                        {
                            instl = (from b in Db.EXP_BAHSAD_CYCLE_D
                                     select b.EBCD_ROW).Max();
                            instl++;
                        }
                        a.EBCD_ROW = instl;

                        a.EBCD_ID = g1.Id.ToString();
                        a.EBCD_DESC = g1.Title.ToString();
                        a.EBCD_YEARS = g1.PersianStartDate.Year.ToString();
                        a.EBCD_YEARE = g1.PersianEndDate.Year.ToString();
                        a.EBCD_MONTS = g1.PersianStartDate.Month.ToString();
                        a.EBCD_MONTE = g1.PersianEndDate.Month.ToString();
                        a.EBCD_EBCT_ID = g1.Type.Id.ToString();
                        a.EBCD_EBCT_DESC = g1.Type.Title.ToString();
                        a.EBCD_DAYS = g1.PersianStartDate.Day.ToString();
                        a.EBCD_DAYE = g1.PersianEndDate.Day.ToString();
                        var q = (from b in Db.EXP_BAHSAD_CYCLE_D where b.EBCD_DESC == a.EBCD_DESC select b);
                        if (q.FirstOrDefault() == null)
                        {
                            Db.EXP_BAHSAD_CYCLE_D.Add(a);
                            Db.SaveChanges();

                        }
                        
                    }
                   
                  
                     
                     f= val.GetFieldsList(userservice, token, out UMsg);
                     foreach (var f1 in f)
                     {
                    
                        EXP_BAHSAD_FIELD_LIST fillist = new EXP_BAHSAD_FIELD_LIST();

                      
                         fillist.EBFL_ID = f1.Id.ToString();
                         fillist.EBFL_DESC = f1.Title;
                         fillist.EBFL_TYDESC = f1.Type.ToString();
                         fillist.EBFL_FCID= f1.FieldCategory.Id.ToString();
                         fillist.EBFL_FCDESC = f1.FieldCategory.Title.ToString();
                         var q = (from b in Db.EXP_BAHSAD_FIELD_LIST where b.EBFL_DESC == f1.Title select b);
                         if (q.FirstOrDefault() == null)
                         {

                             Db.EXP_BAHSAD_FIELD_LIST.Add(fillist);
                             Db.SaveChanges();
                             if (f1.FieldPropertyTypes != null)
                             {
                                 foreach (var pf in f1.FieldPropertyTypes)
                                 {
                                     EXP_BAHSAD_PFTYPE b = new EXP_BAHSAD_PFTYPE();
                                     b.EBPF_ID = pf.Id.ToString();
                                     //  b.EBPF_ITM = pf.Items.ToString();
                                     b.EBPF_TYPE = pf.Type.ToString();
                                     b.EBPF_DESC = pf.Title;
                                     b.EBFL_EBFL_ROW = fillist.EBFL_ROW;

                                     Db.EXP_BAHSAD_PFTYPE.Add(b);
                                     Db.SaveChanges();
                                     if (pf.Units != null)
                                     {
                                         foreach (var pfu in pf.Units)
                                         {
                                             EXP_BAHSAD_PUNIT cu = new EXP_BAHSAD_PUNIT();
                                             cu.EBPU_ID = pfu.Id.ToString();
                                             cu.EBPU_DESC = pfu.Title;
                                             cu.FACTORRATE = pfu.FactorRate.ToString();
                                             cu.EBFL_EBFL_ROW = fillist.EBFL_ROW;
                                             cu.EBPF_EBPF_ROW = b.EBPF_ROW;
                                             Db.EXP_BAHSAD_PUNIT.Add(cu);
                                             Db.SaveChanges();


                                         }
                                     }
                                     if (pf.Items != null)
                                     {
                                         foreach (var pi in pf.Items)
                                         {
                                             EXP_BAHSAD_PITEM bi = new EXP_BAHSAD_PITEM();

                                             bi.EBPI_ID = pi.Id.ToString();
                                             bi.EBPI_DESC = pi.Title;
                                             bi.EBFL_EBFL_ROW = fillist.EBFL_ROW;
                                             bi.EBPF_EBPF_ROW = b.EBPF_ROW;
                                             Db.EXP_BAHSAD_PITEM.Add(bi);
                                             Db.SaveChanges();

                                         }
                                     }
                                 }
                             }
                             if (f1.Items != null)
                             {
                                 foreach (var pi in f1.Items)
                                 {
                                     EXP_BAHSAD_PITEM bi = new EXP_BAHSAD_PITEM();

                                     bi.EBPI_ID = pi.Id.ToString();
                                     bi.EBPI_DESC = pi.Title;
                                     bi.EBFL_EBFL_ROW = fillist.EBFL_ROW;
                                     Db.EXP_BAHSAD_PITEM.Add(bi);
                                     Db.SaveChanges();

                                 }
                             }
                             if (f1.Units != null)
                             {
                                 foreach (var pu in f1.Units)
                                 {
                                     EXP_BAHSAD_PUNIT cu = new EXP_BAHSAD_PUNIT();

                                     cu.EBPU_ID = pu.Id.ToString();
                                     cu.EBPU_DESC = pu.Title;
                                     cu.FACTORRATE = pu.FactorRate.ToString();
                                     cu.EBFL_EBFL_ROW = fillist.EBFL_ROW;
                                     Db.EXP_BAHSAD_PUNIT.Add(cu);
                                     Db.SaveChanges();

                                 }
                             }

                         }

                     }

                  
                    p = val.GetPropertiesList(userservice, token, out UMsg);
                    foreach (var p1 in p)
                    {
                        EXP_BAHSAD_PROPERTY a = new EXP_BAHSAD_PROPERTY();
                      
                      
                        a.EBPR_ID = p1.Id.ToString();
                        a.EBPR_DESC = p1.Title;
                        a.EBPR_TYPE = p1.Type.ToString();
                        if (p1.PropertyCategory != null)
                        {
                        a.EBPR_PCID= p1.PropertyCategory.Id.ToString();
                        a.EBPR_PCDESC=p1.PropertyCategory.Title.ToString();
                        }   
              
                          var q = (from b in Db.EXP_BAHSAD_PROPERTY where b.EBPR_DESC == p1.Title select b);
                          if (q.FirstOrDefault() == null)
                          {

                              Db.EXP_BAHSAD_PROPERTY.Add(a);
                              Db.SaveChanges();
                              if (p1.FieldPropertyTypes != null)
                              {
                                  foreach (var pf in p1.FieldPropertyTypes)
                                  {
                                      EXP_BAHSAD_PFTYPE b = new EXP_BAHSAD_PFTYPE();

                                      b.EBPF_ID = pf.Id.ToString();
                                      // b.EBPF_ITM = pf.Items.ToString();
                                      b.EBPF_TYPE = pf.Type.ToString();
                                      b.EBPF_DESC = pf.Title;
                                      b.EBPR_EBPR_ROW = a.EBPR_ROW;

                                      Db.EXP_BAHSAD_PFTYPE.Add(b);
                                      Db.SaveChanges();

                                      if (pf.Items != null)
                                      {
                                          foreach (var pi in pf.Items)
                                          {
                                              EXP_BAHSAD_PITEM bi = new EXP_BAHSAD_PITEM();

                                              bi.EBPI_ID = pi.Id.ToString();
                                              bi.EBPI_DESC = pi.Title;
                                              bi.EBPF_EBPF_ROW = b.EBPF_ROW;
                                              bi.EBPR_EBPR_ROW = a.EBPR_ROW;
                                              Db.EXP_BAHSAD_PITEM.Add(bi);
                                              Db.SaveChanges();

                                          }
                                      }

                                      if (pf.Units != null)
                                      {
                                          foreach (var pfu in pf.Units)
                                          {
                                              EXP_BAHSAD_PUNIT cu = new EXP_BAHSAD_PUNIT();

                                              cu.EBPU_ID = pfu.Id.ToString();
                                              cu.EBPU_DESC = pfu.Title;
                                              cu.FACTORRATE = pfu.FactorRate.ToString();

                                              cu.EBPF_EBPF_ROW = b.EBPF_ROW;
                                              cu.EBPR_EBPR_ROW = a.EBPR_ROW;
                                              Db.EXP_BAHSAD_PUNIT.Add(cu);
                                              Db.SaveChanges();


                                          }
                                      }
                                  }
                              }

                              if (p1.PropertyItems != null)
                              {
                                  foreach (var pi in p1.PropertyItems)
                                  {
                                      EXP_BAHSAD_PITEM bi = new EXP_BAHSAD_PITEM();

                                      bi.EBPI_ID = pi.Id.ToString();
                                      bi.EBPI_DESC = pi.Title;
                                      bi.EBPR_EBPR_ROW = a.EBPR_ROW;
                                      Db.EXP_BAHSAD_PITEM.Add(bi);
                                      Db.SaveChanges();

                                  }
                              }
                              if (p1.Units != null)
                              {
                                  foreach (var pu in p1.Units)
                                  {
                                      EXP_BAHSAD_PUNIT cu = new EXP_BAHSAD_PUNIT();
                                      cu.EBPU_ID = pu.Id.ToString();
                                      cu.EBPU_DESC = pu.Title;
                                      cu.FACTORRATE = pu.FactorRate.ToString();
                                      cu.EBPR_EBPR_ROW = a.EBPR_ROW;
                                      Db.EXP_BAHSAD_PUNIT.Add(cu);
                                      Db.SaveChanges();

                                  }
                              }

                          }

                    }



                    //var instl = (from bb in Db.EXP_BAHSAD_INFO_S select new { bb.EBAS_ROW });
                    */

                    int i = 1;

                    w = val.GetWorkspaces(userservice, token, 0, i, null, out um, out wsCount);
                    while (w.Length != 0)
                    {
                        w = val.GetWorkspaces(userservice, token, 0, i, null, out um, out wsCount);

                        foreach (var w1 in w)
                        {
                            EXP_BAHSAD_INFO_S a = new EXP_BAHSAD_INFO_S();
                            //a.EBAS_ROW = instl;
                            a.EBAS_PARENT_ID = w1.ParentId.ToString();
                            a.EBAS_DESC = w1.Title.ToString();
                            a.BHSD_ID = w1.Id.ToString();
                            a.WTYPE_ID = w1.WorkspaceType.Id.ToString();
                            a.EBAS_PAGE_ID = i;
                            var q = (from b in Db.EXP_BAHSAD_INFO_S
                                     where b.EBAS_DESC == w1.Title && b.EBAS_PARENT_ID == a.EBAS_PARENT_ID && b.WTYPE_ID == a.WTYPE_ID
                                     select b);
                            if (q.FirstOrDefault() == null)
                            {
                                Db.EXP_BAHSAD_INFO_S.Add(a);
                                Db.SaveChanges();
                            }
                        }
                        i++;
                    }

                }

            }
            catch (Exception ex)
            {
                string d = ex.Message;
            }

            return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات ثبت شد", }.ToJson();
        }

        public ActionResult sendInfoPosttobehsad(int rowid)
        {
            //string userservice = "horm02";
            //string token = "", AuthenticateMsg = "", UpdateMsg = "";
            //long? itemtype;

            Value val = new Value();
            var q = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBAS_EBAS_ROW == rowid && b.EBPV_STAT == "3" && b.EBPV_EBPV_ROW == null select b;

            foreach (var qi in q)
            {
                /*  itemtype = long.Parse(qi.EBPI_EBPI_ROW.ToString());

                  var qcheck=(from b in Db.EXP_BAHSAD_INFO_S where b.EBAS_ROW==qi.EBAS_EBAS_ROW select b.BHSD_ID);
                  var qitem = (from b in Db.EXP_BAHSAD_PROPERTY where b.EBPR_ROW == qi.EBPR_EBPR_ROW select b.EBPR_ID);
            
                  
                  if(qcheck.FirstOrDefault()!=null)
                  {
                       val.Authenticate(userservice, "123456", out AuthenticateMsg, out token);   
                       if (AuthenticateMsg == "Login Succeeded.")
                       {
                           val.InsertPropertyValue(long.Parse(qcheck.FirstOrDefault()), long.Parse(qitem.FirstOrDefault()), qi.EBPV_VALUE, null, null, itemtype, userservice, token, out UpdateMsg);
                       }
                  }
                  else
                  {
                      var qbas=(from b in Db.EXP_BAHSAD_INFO_S where b.EBAS_ROW==qi.EBAS_EBAS_ROW select b);
                      if(qbas!=null)
                      {
                         val.Authenticate(userservice, "123456", out AuthenticateMsg, out token);
                         if (AuthenticateMsg == "Login Succeeded.")
                         {
                             long? BHSDID=val.InsertWorkspace(qbas.FirstOrDefault().EBAS_DESC, long.Parse(qbas.FirstOrDefault().EBAS_PARENT_ID), 94, "Send Whit Web Service", userservice, token, out UpdateMsg);
                           
                           val.Authenticate(userservice, "123456", out AuthenticateMsg, out token);
                           if (AuthenticateMsg == "Login Succeeded.")
                           {
                               if(BHSDID != null)
                              {
                                val.InsertPropertyValue(long.Parse(BHSDID.ToString()), long.Parse(qitem.FirstOrDefault()), qi.EBPV_VALUE, null, null, itemtype, userservice, token, out UpdateMsg);
                              }
                           }
                       }
               

                          
                      }
                  }*/
                qi.EBPV_STAT = "4";
                Db.SaveChanges();
            }

            return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات ارسال شد", }.ToJson();
        }

        public ActionResult sendInfoPosttobehsadall()
        {
            //string userservice = "horm02";
            //string token = "", AuthenticateMsg = "", UpdateMsg = "";
            //long? itemtype;

            Value val = new Value();
            var q = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_STAT == "3" && b.EBPV_EBPV_ROW == null select b;

            foreach (var qi in q)
            {
                /*   itemtype = long.Parse(qi.EBPI_EBPI_ROW.ToString());

                   var qcheck=(from b in Db.EXP_BAHSAD_INFO_S where b.EBAS_ROW==qi.EBAS_EBAS_ROW select b.BHSD_ID);
                   var qitem = (from b in Db.EXP_BAHSAD_PROPERTY where b.EBPR_ROW == qi.EBPR_EBPR_ROW select b.EBPR_ID);
            
                  
                   if(qcheck.FirstOrDefault()!=null)
                   {
                        val.Authenticate(userservice, "123456", out AuthenticateMsg, out token);   
                        if (AuthenticateMsg == "Login Succeeded.")
                        {
                            val.InsertPropertyValue(long.Parse(qcheck.FirstOrDefault()), long.Parse(qitem.FirstOrDefault()), qi.EBPV_VALUE, null, null, itemtype, userservice, token, out UpdateMsg);
                        }
                   }
                   else
                   {
                       var qbas=(from b in Db.EXP_BAHSAD_INFO_S where b.EBAS_ROW==qi.EBAS_EBAS_ROW select b);
                       if(qbas!=null)
                       {
                          val.Authenticate(userservice, "123456", out AuthenticateMsg, out token);
                          if (AuthenticateMsg == "Login Succeeded.")
                          {
                              long? BHSDID=val.InsertWorkspace(qbas.FirstOrDefault().EBAS_DESC, long.Parse(qbas.FirstOrDefault().EBAS_PARENT_ID), 94, "Send Whit Web Service", userservice, token, out UpdateMsg);
                           
                            val.Authenticate(userservice, "123456", out AuthenticateMsg, out token);
                            if (AuthenticateMsg == "Login Succeeded.")
                            {
                                if(BHSDID != null)
                               {
                                 val.InsertPropertyValue(long.Parse(BHSDID.ToString()), long.Parse(qitem.FirstOrDefault()), qi.EBPV_VALUE, null, null, itemtype, userservice, token, out UpdateMsg);
                               }
                            }
                        }
               

                          
                       }
                   }*/
                qi.EBPV_STAT = "4";
                Db.SaveChanges();
            }

            return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات ارسال شد", }.ToJson();
        }


        // public ActionResult sendInfoinsttobehsadtall()
        // {


        //     string userservice = "horm02";
        //     string token = "", AuthenticateMsg = "", UpdateMsg = "";

        //     Value val = new Value();
        //     long? itemtype;

        //     var q = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_STAT == "3" && b.EBPV_EBPV_ROW == null select b;

        //     foreach (var qi in q)
        //     {
        //          itemtype = long.Parse(qi.EBPI_EBPI_ROW.ToString());

        //          var qcheck=(from b in Db.EXP_BAHSAD_INFO_S where b.EBAS_ROW==qi.EBAS_EBAS_ROW select b.BHSD_ID);
        //          var qitem = (from b in Db.EXP_BAHSAD_PROPERTY where b.EBPR_ROW == qi.EBPR_EBPR_ROW select b.EBPR_ID);


        //          if(qcheck.FirstOrDefault()!=null)
        //          {
        //               val.Authenticate(userservice, "123456", out AuthenticateMsg, out token);   
        //               if (AuthenticateMsg == "Login Succeeded.")
        //               {
        //                   val.InsertPropertyValue(long.Parse(qcheck.FirstOrDefault()), long.Parse(qitem.FirstOrDefault()), qi.EBPV_VALUE, null, null, itemtype, userservice, token, out UpdateMsg);
        //               }
        //          }
        //          else
        //          {
        //              var qbas=(from b in Db.EXP_BAHSAD_INFO_S where b.EBAS_ROW==qi.EBAS_EBAS_ROW select b);
        //              if(qbas!=null)
        //              {
        //                 val.Authenticate(userservice, "123456", out AuthenticateMsg, out token);
        //                 if (AuthenticateMsg == "Login Succeeded.")
        //                 {
        //                     long? BHSDID=val.InsertWorkspace(qbas.FirstOrDefault().EBAS_DESC, long.Parse(qbas.FirstOrDefault().EBAS_PARENT_ID), 94, "Send Whit Web Service", userservice, token, out UpdateMsg);

        //                   val.Authenticate(userservice, "123456", out AuthenticateMsg, out token);
        //                   if (AuthenticateMsg == "Login Succeeded.")
        //                   {
        //                       if(BHSDID != null)
        //                      {
        //                       val.InsertPropertyValue(long.Parse(BHSDID.ToString()), long.Parse(qitem.FirstOrDefault()), qi.EBPV_VALUE, null, null, itemtype, userservice, token, out UpdateMsg);
        //                      }
        //                   }
        //               }



        //              }
        //          }
        //         qi.EBPV_STAT = "4";
        //         Db.SaveChanges();


        //     }

        //     /*   var q = (from b in Db.EXP_BEHV_V where b.V1_R == rowid select b);
        //        if(q!=null)
        //        {
        //            if (q.FirstOrDefault().V1_BID != null)
        //            {
        //                if (q.FirstOrDefault().V2_V!=null)
        //                {
        //                itemtype=long.Parse(q.FirstOrDefault().V2_EBPI_ROW.ToString());
        //                val.InsertPropertyValue(long.Parse(q.FirstOrDefault().V1_BID),long.Parse(q.FirstOrDefault().V2_PI), q.FirstOrDefault().V2_V, null, null,itemtype, userservice, token, out UpdateMsg);
        //                }
        //                if (q.FirstOrDefault().V3_V != null)
        //                {
        //                    itemtype = long.Parse(q.FirstOrDefault().V3_EBPI_ROW.ToString());
        //                    val.InsertPropertyValue(long.Parse(q.FirstOrDefault().V1_BID), long.Parse(q.FirstOrDefault().V3_PI), q.FirstOrDefault().V3_V, null, null, itemtype, userservice, token, out UpdateMsg);
        //                }
        //                if (q.FirstOrDefault().V4_V != null)
        //                {
        //                    itemtype = long.Parse(q.FirstOrDefault().V4_EBPI_ROW.ToString());
        //                    val.InsertPropertyValue(long.Parse(q.FirstOrDefault().V1_BID), long.Parse(q.FirstOrDefault().V4_PI), q.FirstOrDefault().V4_V, null, null, itemtype, userservice, token, out UpdateMsg);
        //                }
        //                if (q.FirstOrDefault().V5_V != null)
        //                {
        //                    itemtype = long.Parse(q.FirstOrDefault().V5_EBPI_ROW.ToString());
        //                    val.InsertPropertyValue(long.Parse(q.FirstOrDefault().V1_BID), long.Parse(q.FirstOrDefault().V5_PI), q.FirstOrDefault().V5_V, null, null, itemtype, userservice, token, out UpdateMsg);
        //                }
        //                if (q.FirstOrDefault().V6_V != null)
        //                {
        //                    itemtype = long.Parse(q.FirstOrDefault().V6_EBPI_ROW.ToString());
        //                    val.InsertPropertyValue(long.Parse(q.FirstOrDefault().V1_BID), long.Parse(q.FirstOrDefault().V6_PI), q.FirstOrDefault().V6_V, null, null, itemtype, userservice, token, out UpdateMsg);
        //                }
        //                if (q.FirstOrDefault().V7_V != null)
        //                {
        //                    itemtype = long.Parse(q.FirstOrDefault().V7_EBPI_ROW.ToString());
        //                    val.InsertPropertyValue(long.Parse(q.FirstOrDefault().V1_BID), long.Parse(q.FirstOrDefault().V7_PI), q.FirstOrDefault().V7_V, null, null, itemtype, userservice, token, out UpdateMsg);
        //                }

        //            }



        //        //val.InsertPropertyValue(
        //        }
        //    }


        //   // var q = (from b in Db.EXP_BEHV_V where b.V1_R == rowid select b).FirstOrDefault();

        //    var q1 = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_ROW == q.V1_EBPR select b;
        //    if (q1.FirstOrDefault() != null)
        //    {
        //        if (q1.FirstOrDefault().EBPV_STAT == "3")
        //        {
        //            q1.FirstOrDefault().EBPV_STAT = "4";
        //        }
        //    }

        //    var q2 = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_ROW == q.V2_EBPR select b;
        //    if (q2.FirstOrDefault() != null)
        //    {

        //        if (q2.FirstOrDefault().EBPV_STAT == "3")
        //        {
        //            q2.FirstOrDefault().EBPV_STAT = "4";
        //        }


        //    }

        //    var q3 = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_ROW == q.V3_EBPR select b;
        //    if (q3.FirstOrDefault() != null)
        //    {
        //        if (q3.FirstOrDefault().EBPV_STAT == "3")
        //        {
        //            q3.FirstOrDefault().EBPV_STAT = "4";
        //        }
        //    }

        //    var q4 = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_ROW == q.V4_EBPR select b;
        //    if (q4.FirstOrDefault() != null)
        //    {
        //        if (q4.FirstOrDefault().EBPV_STAT == "3")
        //        {
        //            q4.FirstOrDefault().EBPV_STAT = "4";
        //        }
        //    }

        //    var q5 = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_ROW == q.V5_EBPR select b;
        //    if (q5.FirstOrDefault() != null)
        //    {
        //        if (q5.FirstOrDefault().EBPV_STAT == "3")
        //        {
        //            q5.FirstOrDefault().EBPV_STAT = "4";
        //        }
        //    }

        //    var q6 = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_ROW == q.V6_EBPR select b;
        //    if (q6.FirstOrDefault() != null)
        //    {
        //        if (q6.FirstOrDefault().EBPV_STAT == "3")
        //        {
        //            q6.FirstOrDefault().EBPV_STAT = "4";
        //        }
        //    }

        //    var q7 = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_ROW == q.V7_EBPR select b;
        //    if (q7.FirstOrDefault() != null)
        //    {
        //        if (q7.FirstOrDefault().EBPV_STAT == "3")
        //        {
        //            q7.FirstOrDefault().EBPV_STAT = "4";
        //        }
        //    }



        //    Db.SaveChanges();


        //*/

        //     return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات ارسال شد", }.ToJson();
        // }


        // public ActionResult sendInfoinsttobehsad()
        // {


        //     string userservice = "horm02";
        //     string token = "", AuthenticateMsg = "", UpdateMsg = "";

        //     Value val = new Value();
        //     long? itemtype;

        //     var q = from b in Db.EXP_BAHSAD_PROPERTY_V where  b.EBPV_STAT == "3" && b.EBPV_EBPV_ROW == null select b;

        //     foreach (var qi in q)
        //     {
        //         /* itemtype = long.Parse(qi.EBPI_EBPI_ROW.ToString());

        //          var qcheck=(from b in Db.EXP_BAHSAD_INFO_S where b.EBAS_ROW==qi.EBAS_EBAS_ROW select b.BHSD_ID);
        //          var qitem = (from b in Db.EXP_BAHSAD_PROPERTY where b.EBPR_ROW == qi.EBPR_EBPR_ROW select b.EBPR_ID);


        //          if(qcheck.FirstOrDefault()!=null)
        //          {
        //               val.Authenticate(userservice, "123456", out AuthenticateMsg, out token);   
        //               if (AuthenticateMsg == "Login Succeeded.")
        //               {
        //                   val.InsertPropertyValue(long.Parse(qcheck.FirstOrDefault()), long.Parse(qitem.FirstOrDefault()), qi.EBPV_VALUE, null, null, itemtype, userservice, token, out UpdateMsg);
        //               }
        //          }
        //          else
        //          {
        //              var qbas=(from b in Db.EXP_BAHSAD_INFO_S where b.EBAS_ROW==qi.EBAS_EBAS_ROW select b);
        //              if(qbas!=null)
        //              {
        //                 val.Authenticate(userservice, "123456", out AuthenticateMsg, out token);
        //                 if (AuthenticateMsg == "Login Succeeded.")
        //                 {
        //                     long BHSDID=val.InsertWorkspace(qbas.FirstOrDefault().EBAS_DESC, long.Parse(qbas.FirstOrDefault().EBAS_PARENT_ID), 94, "Send Whit Web Service", userservice, token, out UpdateMsg);

        //                   val.Authenticate(userservice, "123456", out AuthenticateMsg, out token);
        //                   if (AuthenticateMsg == "Login Succeeded.")
        //                   {
        //                       if(BHSDID != null)
        //                      {
        //                        val.InsertPropertyValue(long.Parse(BHSDID, long.Parse(qitem.FirstOrDefault()), qi.EBPV_VALUE, null, null, itemtype, userservice, token, out UpdateMsg);
        //                      }
        //                   }
        //               }



        //              }
        //          }*/
        //         qi.EBPV_STAT = "4";
        //         Db.SaveChanges();


        //     }

        //     /*   var q = (from b in Db.EXP_BEHV_V where b.V1_R == rowid select b);
        //        if(q!=null)
        //        {
        //            if (q.FirstOrDefault().V1_BID != null)
        //            {
        //                if (q.FirstOrDefault().V2_V!=null)
        //                {
        //                itemtype=long.Parse(q.FirstOrDefault().V2_EBPI_ROW.ToString());
        //                val.InsertPropertyValue(long.Parse(q.FirstOrDefault().V1_BID),long.Parse(q.FirstOrDefault().V2_PI), q.FirstOrDefault().V2_V, null, null,itemtype, userservice, token, out UpdateMsg);
        //                }
        //                if (q.FirstOrDefault().V3_V != null)
        //                {
        //                    itemtype = long.Parse(q.FirstOrDefault().V3_EBPI_ROW.ToString());
        //                    val.InsertPropertyValue(long.Parse(q.FirstOrDefault().V1_BID), long.Parse(q.FirstOrDefault().V3_PI), q.FirstOrDefault().V3_V, null, null, itemtype, userservice, token, out UpdateMsg);
        //                }
        //                if (q.FirstOrDefault().V4_V != null)
        //                {
        //                    itemtype = long.Parse(q.FirstOrDefault().V4_EBPI_ROW.ToString());
        //                    val.InsertPropertyValue(long.Parse(q.FirstOrDefault().V1_BID), long.Parse(q.FirstOrDefault().V4_PI), q.FirstOrDefault().V4_V, null, null, itemtype, userservice, token, out UpdateMsg);
        //                }
        //                if (q.FirstOrDefault().V5_V != null)
        //                {
        //                    itemtype = long.Parse(q.FirstOrDefault().V5_EBPI_ROW.ToString());
        //                    val.InsertPropertyValue(long.Parse(q.FirstOrDefault().V1_BID), long.Parse(q.FirstOrDefault().V5_PI), q.FirstOrDefault().V5_V, null, null, itemtype, userservice, token, out UpdateMsg);
        //                }
        //                if (q.FirstOrDefault().V6_V != null)
        //                {
        //                    itemtype = long.Parse(q.FirstOrDefault().V6_EBPI_ROW.ToString());
        //                    val.InsertPropertyValue(long.Parse(q.FirstOrDefault().V1_BID), long.Parse(q.FirstOrDefault().V6_PI), q.FirstOrDefault().V6_V, null, null, itemtype, userservice, token, out UpdateMsg);
        //                }
        //                if (q.FirstOrDefault().V7_V != null)
        //                {
        //                    itemtype = long.Parse(q.FirstOrDefault().V7_EBPI_ROW.ToString());
        //                    val.InsertPropertyValue(long.Parse(q.FirstOrDefault().V1_BID), long.Parse(q.FirstOrDefault().V7_PI), q.FirstOrDefault().V7_V, null, null, itemtype, userservice, token, out UpdateMsg);
        //                }

        //            }



        //        //val.InsertPropertyValue(
        //        }
        //    }


        //   // var q = (from b in Db.EXP_BEHV_V where b.V1_R == rowid select b).FirstOrDefault();

        //    var q1 = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_ROW == q.V1_EBPR select b;
        //    if (q1.FirstOrDefault() != null)
        //    {
        //        if (q1.FirstOrDefault().EBPV_STAT == "3")
        //        {
        //            q1.FirstOrDefault().EBPV_STAT = "4";
        //        }
        //    }

        //    var q2 = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_ROW == q.V2_EBPR select b;
        //    if (q2.FirstOrDefault() != null)
        //    {

        //        if (q2.FirstOrDefault().EBPV_STAT == "3")
        //        {
        //            q2.FirstOrDefault().EBPV_STAT = "4";
        //        }


        //    }

        //    var q3 = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_ROW == q.V3_EBPR select b;
        //    if (q3.FirstOrDefault() != null)
        //    {
        //        if (q3.FirstOrDefault().EBPV_STAT == "3")
        //        {
        //            q3.FirstOrDefault().EBPV_STAT = "4";
        //        }
        //    }

        //    var q4 = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_ROW == q.V4_EBPR select b;
        //    if (q4.FirstOrDefault() != null)
        //    {
        //        if (q4.FirstOrDefault().EBPV_STAT == "3")
        //        {
        //            q4.FirstOrDefault().EBPV_STAT = "4";
        //        }
        //    }

        //    var q5 = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_ROW == q.V5_EBPR select b;
        //    if (q5.FirstOrDefault() != null)
        //    {
        //        if (q5.FirstOrDefault().EBPV_STAT == "3")
        //        {
        //            q5.FirstOrDefault().EBPV_STAT = "4";
        //        }
        //    }

        //    var q6 = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_ROW == q.V6_EBPR select b;
        //    if (q6.FirstOrDefault() != null)
        //    {
        //        if (q6.FirstOrDefault().EBPV_STAT == "3")
        //        {
        //            q6.FirstOrDefault().EBPV_STAT = "4";
        //        }
        //    }

        //    var q7 = from b in Db.EXP_BAHSAD_PROPERTY_V where b.EBPV_ROW == q.V7_EBPR select b;
        //    if (q7.FirstOrDefault() != null)
        //    {
        //        if (q7.FirstOrDefault().EBPV_STAT == "3")
        //        {
        //            q7.FirstOrDefault().EBPV_STAT = "4";
        //        }
        //    }



        //    Db.SaveChanges();


        //*/

        //     return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات ارسال شد", }.ToJson();
        // }


        //public ActionResult sendInfoPostnew(int rowid)
        //{



        //    int epol_id = rowid;
        //    int behsad_row;
        //    var q = from b in Db.EXP_POST_LINE where b.EPOL_ID == epol_id select b;
        //    //    q.FirstOrDefault().EBAS_EBAS_ROW = behsad_row;
        //    //    Db.SaveChanges();

        //    EXP_BAHSAD_INFO_S binfo = new EXP_BAHSAD_INFO_S();
        //    binfo.EBAS_DESC = q.FirstOrDefault().EPOL_NAME;
        //    binfo.WTYPE_ID = "94";
        //    var qunit = from b in q
        //                join u in Db.EXP_UNIT_LEVEL on q.FirstOrDefault().EUNL_EUNL_ID equals u.EUNL_ID
        //                select u;

        //    if (qunit.FirstOrDefault().EUNL_NUM == 400)
        //    { binfo.EBAS_PARENT_ID = "1513"; }
        //    else if (qunit.FirstOrDefault().EUNL_NUM == 230)
        //    { binfo.EBAS_PARENT_ID = "1514"; }
        //    else if (qunit.FirstOrDefault().EUNL_NUM == 132)
        //    { binfo.EBAS_PARENT_ID = "1515"; }
        //    else if (qunit.FirstOrDefault().EUNL_NUM == 66)
        //    { binfo.EBAS_PARENT_ID = "1516"; }
        //    else if (qunit.FirstOrDefault().EUNL_NUM == 63)
        //    { binfo.EBAS_PARENT_ID = "1516"; }
        //    Db.EXP_BAHSAD_INFO_S.Add(binfo);
        //    Db.SaveChanges();
        //    behsad_row = binfo.EBAS_ROW;
        //    q.FirstOrDefault().EBAS_EBAS_ROW = behsad_row;
        //    Db.SaveChanges();


        //    System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();

        //    string MOUNT = "00";
        //    if (pc.GetMonth(DateTime.Now) < 10)
        //    {
        //        MOUNT = "0" + pc.GetMonth(DateTime.Now).ToString();
        //    }
        //    else
        //    {
        //        MOUNT = pc.GetMonth(DateTime.Now).ToString();
        //    }

        //    string day = "00";
        //    if (pc.GetDayOfMonth(DateTime.Now) < 10)
        //    {
        //        day = "0" + pc.GetDayOfMonth(DateTime.Now).ToString();
        //    }
        //    else
        //    {
        //        day = pc.GetDayOfMonth(DateTime.Now).ToString();
        //    }

        //    EXP_BAHSAD_PROPERTY_V probehsad = new EXP_BAHSAD_PROPERTY_V();

        //    probehsad.EBPR_EBPR_ROW = (from b in Db.EXP_BAHSAD_PROPERTY where b.EBPR_ID == "121" select b.EBPR_ROW).FirstOrDefault();
        //    //689;
        //    probehsad.EBPV_VALUE = "109";
        //    probehsad.EBAS_EBAS_ROW = behsad_row;
        //    probehsad.EBPV_DAY = day;
        //    probehsad.EBPV_MONT = MOUNT;
        //    probehsad.EBPV_YEAR = pc.GetYear(DateTime.Now).ToString();
        //    probehsad.EBPI_EBPI_ROW = (from b in Db.EXP_BAHSAD_PITEM where b.EBPI_ID == "109" && b.EBPR_EBPR_ROW == 689 select b.EBPI_ROW).FirstOrDefault();
        //    //1061;
        //    probehsad.EBPV_STAT = "3";
        //    Db.EXP_BAHSAD_PROPERTY_V.Add(probehsad);
        //    Db.SaveChanges();


        //    string[] p = { "192", "381", "191", "343", "341", "221" };



        //    foreach (var pid in p)
        //    {
        //        decimal? valid = null;

        //        var qp = from b in Db.EXP_BAHSAD_PROPERTY where b.EBPR_ID == pid select b.QUERY_EQUIP;
        //        var val = Db.Database.SqlQuery<string>(qp.FirstOrDefault().ToString() + epol_id.ToString()).FirstOrDefault();
        //        EXP_BAHSAD_PROPERTY_V probehsad1 = new EXP_BAHSAD_PROPERTY_V();

        //        if (pid == "381")
        //        {
        //            probehsad1.EBPU_EBPU_ROW = (from b in Db.EXP_BAHSAD_PUNIT
        //                                        join c in Db.EXP_BAHSAD_PROPERTY on b.EBPR_EBPR_ROW equals c.EBPR_ROW
        //                                        where b.EBPU_ID == "161" && c.EBPR_ID == "381"
        //                                        select b.EBPU_ROW).FirstOrDefault();
        //        }
        //        if (pid == "191")
        //        {

        //            if (val == "4") { val = "721"; }
        //            else if (val == "1") { val = "126"; }
        //            else if (val == "2") { val = "127"; }
        //            else if (val == "3") { val = "128"; }
        //            else
        //            { val = ""; }
        //            if (val != "")
        //            {
        //                valid = (from b in Db.EXP_BAHSAD_PITEM where b.EBPI_ID == val select b.EBPI_ROW).FirstOrDefault();
        //            }
        //        }
        //        if (pid == "192")
        //        {

        //            var vald = val.Split('/');
        //            string posttype = vald[0].ToString();
        //            string statdymm = vald[1].ToString();



        //            if ((posttype == "0" || posttype == "1" || posttype == "1") && (statdymm == "0")) { val = "129"; }
        //            else if (statdymm == "1") { val = "130"; }
        //            else if (posttype == "2") { val = "501"; }

        //            else if ((posttype == "4") && (statdymm == "0")) { val = "131"; }

        //            else if ((posttype == "4") && (statdymm == "1")) { val = "701"; }

        //            else if (posttype == "5") { val = "801"; }
        //            else { val = ""; }
        //            if (val != "")
        //            {
        //                valid = (from b in Db.EXP_BAHSAD_PITEM where b.EBPI_ID == val select b.EBPI_ROW).FirstOrDefault();
        //            }

        //        }


        //        int QEBPR_ROW = (from b in Db.EXP_BAHSAD_PROPERTY where b.EBPR_ID == pid select b.EBPR_ROW).FirstOrDefault();

        //        probehsad1.EBPR_EBPR_ROW = QEBPR_ROW;
        //        probehsad1.EBPV_VALUE = val;
        //        probehsad1.EBAS_EBAS_ROW = behsad_row;
        //        probehsad1.EBPV_DAY = day;
        //        probehsad1.EBPV_MONT = MOUNT;
        //        probehsad1.EBPV_YEAR = pc.GetYear(DateTime.Now).ToString();
        //        probehsad1.EBPI_EBPI_ROW = valid;
        //        probehsad1.EBPV_STAT = "3";
        //        Db.EXP_BAHSAD_PROPERTY_V.Add(probehsad1);
        //        Db.SaveChanges();
        //    }



        //    Db.SaveChanges();
        //    return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات ارسال شد", }.ToJson();

        //}

        //    //public ActionResult read_postitem()
        //     {
        //         var query = (from b in Db.EXP_BAHSAD_PITEM
        //                      where b.EBPR_EBPR_ROW == 696
        //                     select new { b.EBPI_ROW, b.EBPI_DESC });

        //         return Json(query, JsonRequestBehavior.AllowGet);
        //     }

        //public ActionResult read_city()
        //     {
        //         var query = (from b in Db.BKP_GEOGH_LOC
        //                     select new { b.G_CODE, b.G_DESC }).Distinct().OrderByDescending(b => b.G_DESC);

        //         return Json(query, JsonRequestBehavior.AllowGet);
        //     }

        //public ActionResult read_owner()
        //     {
        //         var query = (from b in Db.EXP_OWENER_TYPE
        //                     select new { b.EOTY_ID, b.EOTY_DESC });

        //         return Json(query, JsonRequestBehavior.AllowGet);
        //     }
        //Insertpostinfo


    }

}

using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Net;

namespace Equipment.Controllers.POSTVAR
{

    public class POSTVARController : DbController
    {

        PersianCalendar pc = new PersianCalendar();
        DateTime thisDate = DateTime.Now;
        int? userid = 0; string username = string.Empty;

        public POSTVARController()
            : base()
        {
            userid = this.UserInfo().UserId;
        }

        ///////////////////////////////////////////////////////////////////VIEW

        [HttpPost]
        public ActionResult _varchart()
        {
            return Json(Db.EXP_POST_VAR_HEAD);
        }

        [HttpPost]
        public decimal Ajax_get_var_id(string VAR_DATE, int? EPOL_EPOL_ID)
        {
            return Db.EXP_POST_VAR_HEAD.Where(xx => xx.VAR_DATE == VAR_DATE).Where(xx => xx.EPOL_EPOL_ID == EPOL_EPOL_ID).Select(xx => xx.EPVH_ID).FirstOrDefault();
        }

        public int? Ajax_get_humi(string INFV_TIME, int? EPVH_EPVH_ID)
        {
            return Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EPVH_EPVH_ID).Where(xx => xx.INFV_TIME == INFV_TIME).Where(xx => xx.HUMI != null).Select(xx => xx.HUMI).FirstOrDefault();
        }

        public int? Ajax_get_temp(string INFV_TIME, int? EPVH_EPVH_ID)
        {
            return Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EPVH_EPVH_ID).Where(xx => xx.INFV_TIME == INFV_TIME).Where(xx => xx.TEMP != null).Select(xx => xx.TEMP).FirstOrDefault();
        }

        public string Ajax_get_aux_tg(string INFV_TIME, int? EPVH_EPVH_ID)
        {
            return Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EPVH_EPVH_ID).Where(xx => xx.INFV_TIME == INFV_TIME).Where(xx => xx.AUX_TG != null).Select(xx => xx.AUX_TG).FirstOrDefault();
        }

        public int? Ajax_get_aux_a(string INFV_TIME, int? EPVH_EPVH_ID)
        {
            return Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EPVH_EPVH_ID).Where(xx => xx.INFV_TIME == INFV_TIME).Where(xx => xx.AUX_A != null).Select(xx => xx.AUX_A).FirstOrDefault();
        }

        public int? Ajax_get_aux_v(string INFV_TIME, int? EPVH_EPVH_ID)
        {
            return Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EPVH_EPVH_ID).Where(xx => xx.INFV_TIME == INFV_TIME).Where(xx => xx.AUX_V != null).Select(xx => xx.AUX_V).FirstOrDefault();
        }

        public decimal Ajax_Get_Avg(string INFV_TIME, string SVAR_DATE, string EVAR_DATE, int EPIV_TYPE, int EPIU_EPIU_ID, string field)
        {

            return Db.Database.SqlQuery<decimal>(string.Format("SELECT nvl(round(AVG(ABS( {5} )),3),0) avg FROM EXP_POST_VAR_INSTRU a,  EXP_POST_VAR_head b WHERE EPIV_TYPE ={0} AND (EPIU_EPIU_ID ={1} or EPIU_EPIU_ID2={1} ) AND INFV_TIME ={2} AND a.EPVH_EPVH_ID=b.EPVH_ID AND b.VAR_DATE >= '{3}' AND b.var_date <'{4}'", EPIV_TYPE, EPIU_EPIU_ID, INFV_TIME, EVAR_DATE, SVAR_DATE, field)).FirstOrDefault();
        }
        public string Ajax_Get_VAR(int EPIU_ID, int? EPOL_ID, string VAR_DATE, short? EPIV_TYPE, string field)
        {
            var Query = (from d in Db.EXP_POST_VAR_INSTRU
                         where d.INFV_DATE == VAR_DATE && d.EPOL_EPOL_ID == EPOL_ID && d.EPIV_TYPE == EPIV_TYPE
                         select new
                         {

                             d.MW

                         }
                       ).ToList().ToJson();
            return Query.ToString();
        }
        public string Ajax_Get_Post(string Date)
        {
            //var Query = (from d in Db.EXP_POST_LINE
            //             where d.EPOL_TYPE == "0" && d.POST_LAT != null
            //             select new
            //             {
            //                 d.EPOL_NAME,
            //                 d.EPOL_ID,
            //                 d.POST_LAT,
            //                 d.POST_LNG

            //             }
            //             ).ToList();
            var Query = Db.Database.SqlQuery<EXP_POST_LINE>("select b.* from exp_post_line b where " +
                                                           " b.EPOL_TYPE ='0' and b.EPOL_STAT ='1' and b.POST_LAT is not null")
                                                           .Select(x => new
                                                           {
                                                               x.EPOL_ID,
                                                               x.EPOL_NAME,
                                                               x.POST_LAT,
                                                               x.POST_LNG

                                                           }).ToList();
            var ReturnQuery = Query.Select(d => new
            {
                d.EPOL_NAME,
                d.EPOL_ID,
                d.POST_LAT,
                d.POST_LNG,
                MW = GetMW(d.EPOL_ID, Date),
                MVAR = GetMVAR(d.EPOL_ID, Date),
                Cap = GetCap(d.EPOL_ID)
            }).ToList().ToJson();

            return ReturnQuery.ToString();
        }

        public string Ajax_Get_Line(string Date)
        {
            var ReturnQuery = (from b in Db.EXP_POST_LINE_INSTRU
                               where b.EINS_EINS_ID == 1 && b.EUNL_EUNL_ID != 201 && b.EPIU_ID == 137782
                               select new
                               {
                                   b.CODE_DISP,
                                   b.EPOL_EPOL_ID_INSLIN,
                                   b.EPOL_EPOL_ID_LINE,
                                   SrcPost = b.EXP_POST_LINE1.EPOL_NAME,
                                   DesPost = b.EXP_POST_LINE2.EPOL_NAME
                               }).ToList();



            return ReturnQuery.ToString();
        }
        public decimal? GetMW(int EPOL_ID, string Date)
        {
            return Db.Database.SqlQuery<decimal>(string.Format("select nvl(mw,0) from (select sum(mw) mw from exp_post_var_instru where epol_epol_id={0} and infv_date='{1}' and (epiv_type=6 or epiv_type=9) group by infv_time order by  to_number(infv_time) desc) where rownum=1", EPOL_ID, Date)).FirstOrDefault();
        }
        public decimal? GetMVAR(int EPOL_ID, string Date)
        {

            return Db.Database.SqlQuery<decimal>(string.Format("select nvl(mvar,0) from (select sum(mvar) mvar from exp_post_var_instru where epol_epol_id={0} and infv_date='{1}' and (epiv_type=6 or epiv_type=9) group by infv_time order by  to_number(infv_time) desc) where rownum=1", EPOL_ID, Date)).FirstOrDefault();

        }
        public decimal? GetCap(int EPOL_ID)
        {

            return Db.Database.SqlQuery<decimal?>(string.Format("select sum(nvl(tran_zarf,0)) cap from exp_post_line_instru where epol_epol_id={0} and eins_eins_id=2 and tran_zarf is not null", EPOL_ID)).FirstOrDefault();

        }
        public ActionResult EXP_CHART(int? id)
        {
            ViewData["epvh_id"] = id;
            return View();
        }

        public ActionResult EXP_TRANS_MONT(int? id)
        {
            ViewData["epvh_id"] = id;
            return View();
        }

        public ActionResult EXP_DIST_MONT(int? id)
        {
            ViewData["epvh_id"] = id;
            return View();
        }

        public ActionResult EXP_TRANSFER(int? id)
        {
            ViewData["EPVH_TYPE"] = id;
            return View();
        }

        public ActionResult Copy_TRANS()
        {
            return View();
        }

        public ActionResult Report1(string DATE)
        {
            ViewData["DATE"] = DATE;
            return View();
        }
        public ActionResult Report1_Mgr(string DATE)
        {
            ViewData["DATE"] = DATE;
            return View();
        }
        public ActionResult Report18(string DATE)
        {
            ViewData["DATE"] = DATE;
            return View();
        }
        public ActionResult Report2(string DATE, int? GROP_ID)
        {
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["DATE"] = DATE;
            return View();
        }
        public ActionResult Report1_Details(string DATE, int? GROP_CODE)
        {
            ViewData["GROP_CODE"] = GROP_CODE;
            ViewData["DATE"] = DATE;
            return View();
        }
        public ActionResult Report8(string DATE, string TIME, string EDATE, int? GROP_ID, int? EPOL_ID, int? sum, int? bar)
        {
            ViewData["DATE"] = DATE;
            ViewData["EDATE"] = EDATE;
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["sum"] = sum;
            ViewData["bar"] = bar;
            ViewData["TIME"] = TIME;
            return View();
        }

        public ActionResult Report3(string DATE, string EDATE, int? GROP_ID, int? EPOL_ID, int? sum, int? bar)
        {
            ViewData["DATE"] = DATE;
            ViewData["EDATE"] = EDATE;
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["sum"] = sum;
            ViewData["bar"] = bar;
            return View();
        }
        public ActionResult Report29(string DATE, string EDATE, int? GROP_ID, int? EPOL_ID, int? sum, int? bar)
        {
            ViewData["DATE"] = DATE;
            ViewData["EDATE"] = EDATE;
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["sum"] = sum;
            ViewData["bar"] = bar;
            return View();
        }

        public ActionResult Report3_2(string DATE, string EDATE, int? GROP_ID, int? EPOL_ID, int? sum, int? bar)
        {
            ViewData["DATE"] = DATE;
            ViewData["EDATE"] = EDATE;
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["sum"] = sum;
            ViewData["bar"] = bar;
            return View();
        }

        public ActionResult Report4(string DATE, string EDATE, int? GROP_ID, int? EPOL_ID, int? sum, int? bar)
        {
            ViewData["DATE"] = DATE;
            ViewData["EDATE"] = EDATE;
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["sum"] = sum;
            ViewData["bar"] = bar;
            return View();
        }
        public ActionResult Report4_Abst(string DATE, string EDATE, int? GROP_ID, int? EPOL_ID, int? sum, int? bar)
        {
            ViewData["DATE"] = DATE;
            ViewData["EDATE"] = EDATE;
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["sum"] = sum;
            ViewData["bar"] = bar;
            return View();
        }
        public ActionResult Report17(string DATE, string EDATE, int? GROP_ID, int? EPOL_ID, int? sum, int? bar)
        {
            ViewData["DATE"] = DATE;
            ViewData["EDATE"] = EDATE;
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["sum"] = sum;
            ViewData["bar"] = bar;
            return View();
        }
        public ActionResult Report30(string DATE, string EDATE, int? GROP_ID, int? EPOL_ID, int? sum, int? bar)
        {
            ViewData["DATE"] = DATE;
            ViewData["EDATE"] = EDATE;
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["sum"] = sum;
            ViewData["bar"] = bar;
            return View();
        }
        public ActionResult Report4_yearly(string DATE, string EDATE, int? GROP_ID, int? EPOL_ID, int? sum, int? bar)
        {
            ViewData["DATE"] = DATE;
            ViewData["EDATE"] = EDATE;
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["sum"] = sum;
            ViewData["bar"] = bar;
            return View();
        }
        public ActionResult Report4_kv(string DATE, string EDATE, int? GROP_ID, int? EPOL_ID, int? sum, int? bar)
        {
            ViewData["DATE"] = DATE;
            ViewData["EDATE"] = EDATE;
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["sum"] = sum;
            ViewData["bar"] = bar;
            return View();
        }

        public ActionResult Report4_Behsad(string DATE, string EDATE, int? GROP_ID, int? EPOL_ID, int? sum, int? bar)
        {
            ViewData["DATE"] = DATE;
            ViewData["EDATE"] = EDATE;
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["sum"] = sum;
            ViewData["bar"] = bar;
            return View();
        }

        public ActionResult Report5(string DATE, string EDATE, int? GROP_ID, int? EPOL_ID, int? sum, int? bar)
        {
            ViewData["DATE"] = DATE;
            ViewData["EDATE"] = EDATE;
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["sum"] = sum;
            ViewData["bar"] = bar;
            return View();
        }

        public ActionResult Report6(string DATE, string EDATE, int? GROP_ID, int? EPOL_ID, int? sum, int? bar)
        {
            ViewData["DATE"] = DATE;
            ViewData["EDATE"] = EDATE;
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["sum"] = sum;
            ViewData["bar"] = bar;
            return View();
        }
        public ActionResult Report28(string DATE, string EDATE, int? GROP_ID, int? EPOL_ID, int? sum, int? bar)
        {
            ViewData["DATE"] = DATE;
            ViewData["EDATE"] = EDATE;
            ViewData["GROP_ID"] = GROP_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["sum"] = sum;
            ViewData["bar"] = bar;
            return View();
        }

        public ActionResult Report7(string DATE, string TIME, int? GROP_ID)
        {
            ViewData["DATE"] = DATE;
            ViewData["TIME"] = TIME;
            ViewData["GROP_ID"] = GROP_ID;
            return View();
        }
        public IEnumerable<SelectListItem> GetGroup_DP(int? GROP_CODE)
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
            }).AsEnumerable(); ;

            if (check.Any() && user_name != "s-khademi")
            {
                query = Db.Database.SqlQuery<EXP_GROUPS>("select a.* from EXP_GROUPS a,EXP_USERS_GROUPS b,SEC_USERS c where a.GROP_ID=b.GROP_GROP_ID and b.SCSU_ROW_NO=c.ROW_NO and USER_NAME=:user_name  and (a.GROP_CODE=:GROP_CODE or  :GROP_CODE is null) order by GROP_DESC", user_name, GROP_CODE).Select(x => new
                {
                    x.GROP_ID,
                    x.GROP_DESC
                }).Distinct().AsEnumerable();
                //    query = (from b in cntx.EXP_GROUPS
                //             join d in cntx.EXP_USERS_GROUPS on b.GROP_ID equals d.GROP_GROP_ID
                //             where d.SEC_USERS.USER_NAME == user_name && (b.GROP_CODE == GROP_CODE || GROP_CODE == null)
                //             orderby b.GROP_DESC
                //             select new { b.GROP_DESC, b.GROP_ID });
            }
            return query.Select(d => new SelectListItem { Value = d.GROP_ID.ToString(), Text = d.GROP_DESC });
            //return Json(query, JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<SelectListItem> Getpost()
        {
            var query = (from b in Db.EXP_POST_LINE
                         orderby b.EPOL_NAME
                         where b.EPOL_TYPE == "0"
                         select new { b.EPOL_ID, EPOL_NAME = b.EPOL_NAME }).ToList();
            return query.Select(d => new SelectListItem { Value = d.EPOL_ID.ToString(), Text = d.EPOL_NAME });
        }
        public ActionResult Report()
        {

            ViewData["Groups"] = GetGroup_DP(null);//  Getlines(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0);
            ViewData["Posts"] = Getpost();
            return View();
        }
        public ActionResult ReportOld()
        {
            return View();
        }
        public ActionResult EXP_REACTOR_UPDATE(int? id, int? epiu_id, string VarDate)
        {
            ViewData["epvh_id"] = id;
            ViewData["epiu_id"] = epiu_id;
            ViewData["Lines"] = Getlines(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0, VarDate);
            ViewData["LinesOut"] = GetlinesOut(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0);
            return View();
        }

        public ActionResult EXP_TRANS_FORM_UPDATE(int? id, int? epiu_id, string VarDate)
        {
            ViewData["epvh_id"] = id;
            ViewData["epiu_id"] = epiu_id;
            ViewData["LinesOut"] = GetlinesOut(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0);
            return View();
        }

        public ActionResult EXP_TRANS_DIST_UPDATE(int? id, int? epiu_id, string VarDate)
        {
            ViewData["epvh_id"] = id;
            ViewData["epiu_id"] = epiu_id;
            ViewData["Lines"] = Getlines(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0, VarDate);
            ViewData["LinesOut"] = GetlinesOut(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0);
            return View();
        }
        public ActionResult Import_Scada()
        {

            return View();
        }
        //public PartialViewResult ImportScadaFile(HttpPostedFileBase file)
        public ActionResult ImportScadaFile(string Msg)
        {
            List<string> errors = new List<string>();
            List<string> Data_List = new List<string>();
            DataSet DataSetFile = new DataSet();
            int Epol_id = 0, Counter = 0; decimal Epiu_id = 0, Epvh_id = 0;
            string Scada_Key = "", Field = "", Var_Date = "", Epol_Name = "", Code_Disp = "", SQL = "", IPAddress = "";
            var watch = new Stopwatch();
            watch.Start();

            if (Request.Files["FileUpload"] != null)
            {

                if (Request.Files["FileUpload"].ContentLength > 0)
                {
                    string FileExtension = System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName);


                    if (FileExtension == ".xls" || FileExtension == ".xlsx")
                    {

                        var Host = Dns.GetHostEntry(Dns.GetHostName());
                        foreach (IPAddress IP in Host.AddressList)
                        {
                            if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                IPAddress = Convert.ToString(IP);
                            }
                        }
                        string TempName = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + "-" + User.Identity.Name + "-" + IPAddress;
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/ImportFile/Scada/" + TempName));
                        string FileLocation = Server.MapPath("~/ImportFile/Scada/" + TempName + "/") + Request.Files["FileUpload"].FileName;



                        //}
                        Request.Files["FileUpload"].SaveAs(FileLocation);

                        using (FileStream fs = System.IO.File.Create(FileLocation))
                        {

                            fs.Dispose();
                            fs.Close();

                            Request.Files["FileUpload"].SaveAs(FileLocation);

                            Excel.Application xlApp = new Excel.Application();
                            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(FileLocation);
                            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
                            Excel.Range xlRange = xlWorksheet.UsedRange;
                            int rowCount = xlRange.Rows.Count;
                            int colCount = xlRange.Columns.Count;
                            if (xlRange.Cells[2, 4].Value2 != "")
                            {
                                Var_Date = Convert.ToString(xlRange.Cells[2, 4].Value2);
                                string sqldelete = string.Format("delete from exp_post_var_instru where epvh_epvh_id in (select epvh_id from exp_post_var_head a,exp_post_var_instru b " +
                                    " where a.epvh_id=b.epvh_epvh_id and a.epvh_type=6 and var_date='{0}')", Var_Date);
                                Db.Database.ExecuteSqlCommand(sqldelete);
                                Db.Database.ExecuteSqlCommand(string.Format("delete from exp_post_var_head where epvh_type=6 and var_date='{0}'", Var_Date));

                            }
                            Epol_id = 0; Epiu_id = 0; Scada_Key = ""; Field = ""; Epvh_id = 0;
                            for (int i = 1; i <= rowCount; i++)
                            {

                                for (int j = 1; j <= 40; j++)
                                {


                                    //write the value to the console
                                    if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                                    {
                                        string Data = Convert.ToString(xlRange.Cells[i, j].Value2);
                                        if (j == 5 && Db.EXP_POST_LINE.Where(xx => xx.EPOL_NAME_EN == Data).Select(xx => xx.EPOL_ID).Any())
                                        {
                                            Epol_id = Db.EXP_POST_LINE.Where(xx => xx.EPOL_NAME_EN == Data).Select(xx => xx.EPOL_ID).FirstOrDefault();
                                            Epol_Name = Data;
                                            if (Var_Date != "" && Epol_id != 0)
                                            {
                                                SQL = string.Format("insert into EXP_POST_VAR_HEAD (VAR_DATE,EPOL_EPOL_ID,EPVH_TYPE,EPVH_DESC) values('{0}',{1},6,'SCADA')", Var_Date, Epol_id);
                                                Db.Database.ExecuteSqlCommand(SQL);
                                                Epvh_id = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_TYPE == 6 && xx.VAR_DATE == Var_Date && xx.EPOL_EPOL_ID == Epol_id).Select(xx => xx.EPVH_ID).FirstOrDefault();
                                            }
                                        }                                            //Data_List.Add(Data);

                                        if (Epol_id != 0 && j == 8)
                                        {
                                            Epiu_id = Db.EXP_POST_LINE_INSTRU.Where(xx => (xx.EPOL_EPOL_ID == Epol_id || xx.EPOL_EPOL_ID_INSLIN == Epol_id || xx.EPOL_EPOL_ID_LINE == Epol_id)
                                            && xx.CODE_DISP == Data).Select(xx => xx.EPIU_ID).FirstOrDefault();
                                            Code_Disp = Data;
                                            // Data_List.Add(Data);
                                        }
                                        if (Epol_id != 0 && Epiu_id != 0 && j == 11)
                                        {
                                            Scada_Key = Data;
                                        }
                                        if (Epol_id != 0 && Epiu_id != 0 && j == 12)
                                        {
                                            Field = Data;
                                        }
                                        Counter = Counter + 1;
                                        if (Epol_id != 0 && Epiu_id != 0 && Scada_Key != "" && Field != "" && Var_Date != "" && (j > 14 && j < 40) && Epvh_id != 0)
                                        {
                                            try
                                            {

                                                SQL = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,EPIU_EPIU_ID,{5},INFV_TIME,EPIV_TYPE,scada_key,line_colum) values" +
                                                                                                          "('{0}',{1},{2},{3},{4},{6},{7}) ",
                                                    Epvh_id,

                                                    Epiu_id,
                                                    Data,
                                                    j - 14,
                                                    Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_ID == Epiu_id).Select(xx => xx.EINS_EINS_ID == 1 ? 1 : 6).FirstOrDefault(),
                                                    Field,
                                                    Scada_Key,
                                                    Field == "MW" ? 1 : Field == "MVAR" ? 2 : 3
                                                   );
                                                Db.Database.ExecuteSqlCommand(SQL);
                                                if (j - 14 == 1)
                                                {
                                                    Data_List.Add(" اطلاعات " + Field + "  پست " + Epol_Name + " تجهیز " + Code_Disp + "با موفقیت ثبت شد ");
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                errors.Add("خطای " + ex.Message + " در ثبت اطلاعات ساعت  " + (j - 15) + " اطلاعات " + Field + "  پست " + Epol_Name + " تجهیز " + Code_Disp);
                                            }
                                        }

                                    }

                                }
                            }
                            fs.Dispose();
                            fs.Close();


                        }

                    }
                }


            }
            //}
            //  return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            watch.Stop();
            var elapsed = watch.ElapsedTicks;
            TimeSpan TimeElap = watch.Elapsed;

            if (errors.Count == 0)
            {
                string Data_list_st = string.Join("-", Data_List.ToArray());
                ViewBag.Msg = (Data_list_st);// Counter.ToString() +"--دقیقه"+ TimeElap.ToString("mm")+"ثانیه"+ TimeElap.ToString("ss\\.ff");
                return View("Import_Scada");

                // return Json(new { Success =Data_list_st }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                string errorMessages = string.Join("-", errors.ToArray());
                ViewBag.Msg = errorMessages;
                return View("Import_Scada");
                // return Json(new { Fail = errorMessages }, JsonRequestBehavior.DenyGet);
            }
            //  return View("Import_Scada");
        }
        public ActionResult EXP_TRANS(int? id)
        {
            ViewData["epvh_id"] = id;
            return View();
        }

        public ActionResult EXP_LINE(int? id, int? type, string VarDate)
        {

            ViewData["epvh_id"] = id;
            ViewData["epvh_id"] = id;
            ViewData["type"] = type;
            ViewData["Lines"] = Getlines(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0, VarDate);
            ViewData["LinesOut"] = GetlinesOut(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0);
            return View();
        }

        public ActionResult EXP_TRANS_POW(int? id, int? type, string VarDate)
        {
            ViewData["epvh_id"] = id;
            ViewData["type"] = type;
            ViewData["Lines"] = Getlines(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0, VarDate);
            ViewData["LinesOut"] = GetlinesOut(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0);
            return View();
        }

        public ActionResult EXP_TRANS_UPDATE(int? id, int? type, string VarDate)
        {
            ViewData["epvh_id"] = id;
            ViewData["type"] = type;
            ViewData["Lines"] = Getlines(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0, VarDate);
            ViewData["LinesOut"] = GetlinesOut(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0);
            return View();
        }
        public ActionResult EXP_Solar_Panel(int? id, string VarDate)
        {
            ViewData["epvh_id"] = id;

            ViewData["Lines"] = Getlines(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0, VarDate);
            ViewData["LinesOut"] = GetlinesOut(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0);
            return View();
        }
        public ActionResult EXP_DIST_UPDATE(int? id, int? type, string VarDate)
        {
            ViewData["epvh_id"] = id;
            ViewData["type"] = type;
            ViewData["Lines"] = Getlines(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0, VarDate);
            ViewData["LinesOut"] = GetlinesOut(Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == id).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), 0);
            return View();
        }

        public ActionResult EXP_TRANSFER2(int? id, int? EPIU_EPIU_ID, int? EPIU_EPIU_ID2)
        {
            ViewData["epvh_id"] = id;
            ViewData["EPIU_EPIU_ID"] = EPIU_EPIU_ID;
            ViewData["EPIU_EPIU_ID2"] = EPIU_EPIU_ID2;
            return View();
        }

        public ActionResult EXP_TRANSFER1(int? id, int? EPIU_EPIU_ID, int? EPIU_EPIU_ID2)
        {
            ViewData["epvh_id"] = id;
            ViewData["EPIU_EPIU_ID"] = EPIU_EPIU_ID;
            ViewData["EPIU_EPIU_ID2"] = EPIU_EPIU_ID2;
            return View();
        }
        public ActionResult Chart(int EPIU_ID, int? EPOL_ID, string VAR_DATE, short? EPIV_TYPE)
        {

            ViewData["EPIU_ID"] = EPIU_ID;
            ViewData["EPOL_ID"] = EPOL_ID;
            ViewData["VAR_DATE"] = VAR_DATE;
            ViewData["EPIV_TYPE"] = EPIV_TYPE;


            return View();
        }
        public ActionResult EXP_DIST()
        {
            return View();
        }

        public ActionResult EXP_TRANSFER1_REP(int? id)
        {
            ViewData["epvh_id"] = id;
            return View();
        }

        public ActionResult EXP_UNCOMPLETE(int? id)
        {
            ViewData["epvh_id"] = id;
            return View();
        }
        public ActionResult EXP_Internal_Var(int? id)
        {
            ViewData["epvh_id"] = id;
            return View();
        }

        public ActionResult Getinstrument(short? EPOL_ID, short? EINS_EINS_ID)
        {
            var query = (from d in Db.EXP_POST_LINE_INSTRU
                         orderby d.CODE_NAME
                         where d.EPOL_EPOL_ID == EPOL_ID && d.EINS_EINS_ID == EINS_EINS_ID && (d.PHAS_STAT == "0" || d.PHAS_STAT == null)
                         // && d.EPIU_EPIU_ID_SAVABEGH == null && d.EPIU_TYPE != "3"
                         select new { d.EPIU_ID, d.CODE_NAME, d.CODE_DISP }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getinstrumenttrans(short? EPOL_ID, short? EINS_EINS_ID)
        {
            var query = (from d in Db.EXP_POST_LINE_INSTRU
                         orderby d.CODE_NAME
                         where d.EPOL_EPOL_ID == EPOL_ID && d.EINS_EINS_ID == EINS_EINS_ID && d.TRAN_ZARF != null && (d.PHAS_STAT == "0" || d.PHAS_STAT == null)
                               && d.EPIU_EPIU_ID_SAVABEGH == null && (d.EPIU_TYPE != "3" || d.EPIU_TYPE == null)
                         select new { d.EPIU_ID, d.CODE_NAME, d.CODE_DISP }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getpost_Dist(int? epol_id)
        {
            var query = Db.Database.SqlQuery<EXP_POST_LINE>("select b.* from exp_post_line b , SEC_USER_TYPE_POST c where b.epol_id=c.epol_epol_id and" +
                                                            " b.EPOL_TYPE ='0' and b.EPOL_STAT ='1' and   (b.EUNL_EUNL_ID = 63 or b.EUNL_EUNL_ID = 161 or b.EPOL_ID = 914 or b.epol_id=6201) " +
                                                            " and c.SCSU_ROW_NO = :userid and c.ETDO_ETDO_ID = 303 order by farsi_order_u (b.epol_name) ", userid)
                                                            .Select(x => new
                                                            {
                                                                x.EPOL_ID,
                                                                x.EPOL_NAME
                                                            }).ToList();
            //query = (from b in Db.EXP_POST_LINE
            //             join c in Db.SEC_USER_TYPE_POST on b.EPOL_ID equals c.EPOL_EPOL_ID
            //             orderby b.EPOL_NAME
            //             where b.EPOL_TYPE == "0" && b.EPOL_STAT == "1" && (b.EPOL_ID == epol_id || epol_id == null) && (b.EUNL_EUNL_ID == 63 || b.EUNL_EUNL_ID == 161 || b.EPOL_ID == 914)
            //             && c.SCSU_ROW_NO == userid && c.ETDO_ETDO_ID == 303 
            //             select new { b.EPOL_ID, EPOL_NAME = b.EPOL_NAME }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult geterrorkhazan()
        {
            var RetVal = from b in Db.EXP_ERROR_INST
                         where b.EERR_TYPE == 3
                         select new { b.EERR_ID, b.EERR_DESC };
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getpost(int? epol_id)
        {
            var query = (from b in Db.EXP_POST_LINE
                         join c in Db.SEC_USER_TYPE_POST on b.EPOL_ID equals c.EPOL_EPOL_ID
                         where b.EPOL_TYPE == "0" && b.EPOL_STAT == "1" && (b.EPOL_ID == epol_id || epol_id == null)
                               && c.SCSU_ROW_NO == userid && c.ETDO_ETDO_ID == 303
                         orderby b.EPOL_NAME
                         select new { b.EPOL_ID, EPOL_NAME = b.EPOL_NAME }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getline(int? epol_id, int? eunl_id)
        {
            var linequery = ((from d in Db.EXP_PLIN_V
                              join e in Db.EXP_CONVERT_VOLT on d.EUNL_EUNL_ID equals e.EUNL_EUNL_ID
                              where ((d.EPOL_EPOL_ID_LINE == epol_id || d.EPOL_EPOL_ID_INSLIN == epol_id) && e.EPOL_EPOL_ID == epol_id)
                              select new { d.EPIU_ID, d.CODE_DISP, d.EPOL_NAME }));
            return Json(linequery, JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<SelectListItem> Getlines(int? epol_id, int? eunl_id, string VarDate)
        {
            var linequery = ((from d in Db.EXP_PLIN_V
                              join e in Db.EXP_CONVERT_VOLT on d.EUNL_EUNL_ID equals e.EUNL_EUNL_ID
                              where ((d.EPOL_EPOL_ID_LINE == epol_id || d.EPOL_EPOL_ID_INSLIN == epol_id) && e.EPOL_EPOL_ID == epol_id)
                               && (VarDate.CompareTo(d.EXPL_DATE) > 0 || VarDate.CompareTo(d.EXPL_DATE) == 0)
                               && (VarDate.CompareTo(d.EXPB_DATE) < 0 || VarDate.CompareTo(d.EXPB_DATE) == 0 || d.EXPB_DATE == null)
                              select new { Value = d.EPIU_ID, Text = d.CODE_DISP })).AsEnumerable();

            return linequery.Select(d => new SelectListItem { Value = d.Value.ToString(), Text = d.Text });
        }

        public IEnumerable<SelectListItem> GetlinesOut(int? epol_id, int? eunl_id)
        {
            var linequery = ((from d in Db.EXP_PLIN_V
                              join e in Db.EXP_CONVERT_VOLT on d.EUNL_EUNL_ID equals e.EUNL_EUNL_ID_R
                              where ((d.EPOL_EPOL_ID_LINE == epol_id || d.EPOL_EPOL_ID_INSLIN == epol_id) && e.EPOL_EPOL_ID == epol_id
                              )
                              select new { Value = d.EPIU_ID, Text = d.CODE_DISP })).AsEnumerable();

            return linequery.Select(d => new SelectListItem { Value = d.Value.ToString(), Text = d.Text });
        }

        public ActionResult GetlineOut(int? epol_id, int? eunl_id)
        {
            var linequery = ((from d in Db.EXP_PLIN_V
                              join e in Db.EXP_CONVERT_VOLT on d.EUNL_EUNL_ID equals e.EUNL_EUNL_ID_R
                              where ((d.EPOL_EPOL_ID_LINE == epol_id || d.EPOL_EPOL_ID_INSLIN == epol_id) && e.EPOL_EPOL_ID == epol_id)
                              select new { d.EPIU_ID, d.CODE_DISP, d.EPOL_NAME }));
            return Json(linequery, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getline20(int? epol_id, int? eunl_id)
        {
            var linequery = ((from d in Db.EXP_PLIN_V
                              where (d.EPOL_EPOL_ID_LINE == epol_id || d.EPOL_EPOL_ID_INSLIN == epol_id) && d.EUNL_EUNL_ID == 201
                              select new { d.EPIU_ID, d.CODE_DISP, d.EPOL_NAME }));
            return Json(linequery, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getpost_Trans(int? epol_id, int? EPVH_TYPE)
        {
            var query = Db.Database.SqlQuery<EXP_POST_LINE>("select b.* from exp_post_line b , SEC_USER_TYPE_POST c where b.epol_id=c.epol_epol_id and" +
                                                            " b.EPOL_TYPE ='0' and b.EPOL_STAT ='1' and   (b.EUNL_EUNL_ID != 63 and b.EUNL_EUNL_ID != 161) and  b.EPOL_ID not in (914,6201)" +
                                                            " and c.SCSU_ROW_NO = :userid and c.ETDO_ETDO_ID = 303 order by farsi_order_u (b.epol_name) ", userid)
                                                            .Select(x => new
                                                            {
                                                                x.EPOL_ID,
                                                                x.EPOL_NAME
                                                            }).ToList();

            if (EPVH_TYPE == 2)
            {
                query = Db.Database.SqlQuery<EXP_POST_LINE>("select b.* from exp_post_line b , SEC_USER_TYPE_POST c where b.epol_id=c.epol_epol_id and" +
                                                            " b.EPOL_TYPE ='0' and b.EPOL_STAT ='1' and   (b.EUNL_EUNL_ID != 63 and b.EUNL_EUNL_ID != 161) and (b.post_type=1 or b.post_type=3 ) and  b.EPOL_ID not in (914,6201)" +
                                                            " and c.SCSU_ROW_NO = :userid and c.ETDO_ETDO_ID = 303 order by farsi_order_u (b.epol_name) ", userid).Select(x => new
                                                            {
                                                                x.EPOL_ID,
                                                                x.EPOL_NAME
                                                            }).ToList();
            }

            //query = (from b in Db.EXP_POST_LINE
            //            join c in Db.SEC_USER_TYPE_POST on b.EPOL_ID equals c.EPOL_EPOL_ID
            //            orderby b.EPOL_NAME
            //            where b.EPOL_TYPE == "0" && b.EPOL_STAT == "1" && (b.EPOL_ID == epol_id || epol_id == null) && b.EUNL_EUNL_ID != 63 && b.EUNL_EUNL_ID != 161
            //            && c.SCSU_ROW_NO == userid && c.ETDO_ETDO_ID == 303
            //            select new { b.EPOL_ID, EPOL_NAME = b.EPOL_NAME  }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_Var_Head([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from p in Db.EXP_POST_VAR_HEAD
                         orderby p.EPVH_ID descending
                         select new
                         {
                             p.EPVH_ID,
                             post_name = p.EXP_POST_LINE.EPOL_NAME,
                             p.VAR_DATE
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Get_Postvar([DataSourceRequest] DataSourceRequest request, int EPVH_TYPE, int? EPOL_ID, string YEAR)
        {
            var query = (from p in Db.EXP_POSTVAR_V
                         join v in Db.EXP_POST_LINE on p.EPOL_NAME equals v.EPOL_NAME
                         join z in Db.PAY_ORGAN on v.ORGA_CODE equals z.CODE
                         //join w in Db.EXP_EPIV_V on p.EPVH_ID equals w.EPVH_ID
                         where p.EPVH_TYPE == EPVH_TYPE && v.EPOL_TYPE == "0" && v.EPOL_STAT == "1"
                               && p.ROW_NO == userid && (v.ORGA_MANA_ASTA_CODE == z.MANA_ASTA_CODE && v.ORGA_MANA_CODE == z.MANA_CODE)
                               && (v.EPOL_ID == EPOL_ID || EPOL_ID == null)
                               && p.VAR_DATE.Substring(0, 4) == YEAR
                         orderby p.VAR_DATE descending, p.EPVH_ID descending
                         select new
                         {
                             p.EPVH_ID,
                             p.VAR_DATE,
                             p.EPOL_NAME,
                             p.EPVH_TYPE,
                             p.FAML_NAME,
                             ORGA_DESC = z.ORGA_DESC,
                             COUNT = Db.EXP_EPIV_V.Where(xx => xx.EPVH_ID == p.EPVH_ID).Count(),
                             ROWCOUNT = Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == p.EPVH_ID).Count(),
                             p.EPVH_DESC
                         }).ToList();

            //IQueryable<EXP_POSTVAR_V> models = Db.EXP_POSTVAR_V;
            //return Json((models).ToDataSourceResult(request));
            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Get_Postvar_Uncomplete([DataSourceRequest] DataSourceRequest request, int? evid)
        {
            var query = (from p in Db.EXP_EPIV_V
                         where p.EPVH_ID == evid
                         orderby p.EPIV_TYPE descending
                         select new
                         {
                             p.EPVH_ID,
                             p.CODE_DISP,
                             p.CODE_NAME,
                             p.EPIV_DESC,
                             p.LINE_COLUM,
                             p.COUNT
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult copy_transfer(EXP_POST_VAR_HEAD objecttemp)
        {
            int EPOL_EPOL_ID2 = 0;
            string sql = "", var_date = Request.Form["VAR_DATE2"];
            var EPVH_ID = Db.EXP_POST_VAR_HEAD.Where(xx => xx.VAR_DATE == objecttemp.VAR_DATE)
                                              .Where(xx => xx.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID)
                                              .Select(xx => xx.EPVH_ID).FirstOrDefault();

            var q = from b in Db.EXP_POST_VAR_HEAD
                    where b.VAR_DATE == var_date && b.EPOL_EPOL_ID == EPOL_EPOL_ID2 && b.EPVH_TYPE == 1
                    select new { b.EPVH_ID };

            if (!string.IsNullOrEmpty(Request.Form["EPOL_EPOL_ID2"]))
            {
                // EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                EPOL_EPOL_ID2 = int.Parse(Request.Form["EPOL_EPOL_ID2"]);
            }

            if (!q.Any())
            {
                sql = string.Format("insert into EXP_POST_VAR_HEAD (VAR_DATE,EPOL_EPOL_ID,epvh_type,EPVT_EPVT_ID) values ('{0}','{1}',1,{2}) ", var_date, EPOL_EPOL_ID2, 1);
                Db.Database.ExecuteSqlCommand(sql);
            }

            int EPVH_EPVH_ID = Db.Database.SqlQuery<int>(string.Format("select EPVH_ID from EXP_POST_VAR_HEAD where VAR_DATE='{0}' and EPOL_EPOL_ID={1} and epvh_type=1", var_date, EPOL_EPOL_ID2)).FirstOrDefault();
            var instru = from b in Db.EXP_POST_VAR_INSTRU
                         where b.EPVH_EPVH_ID == EPVH_EPVH_ID
                         select b;

            if (EPVH_EPVH_ID != 0 && (!instru.Any()))
            {
                sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,INFV_TIME,MV,MVAR,MVA,KV,A,TAP,EPIU_EPIU_ID,MW,TEMP,HUMI,OIL,WIND,R,OKV,V,AUX_TG,THR,SEC,PRI,AUX_A,AUX_V,LINE_COLUM,EPIV_DESC,EPIU_EPIU_ID2,OIL_TR,OIL_TAP,OIL_LEVEL,EPIV_TYPE)" +
                                          " select  {0},INFV_TIME,MV,MVAR,MVA,KV,A,TAP,EPIU_EPIU_ID,MW,TEMP,HUMI,OIL,WIND,R,OKV,V,AUX_TG,THR,SEC,PRI,AUX_A,AUX_V,LINE_COLUM,EPIV_DESC,EPIU_EPIU_ID2,OIL_TR,OIL_TAP,OIL_LEVEL,EPIV_TYPE from EXP_POST_VAR_INSTRU" +
                                          " where EPVH_EPVH_ID={1}", EPVH_EPVH_ID, EPVH_ID);
                Db.Database.ExecuteSqlCommand(sql);
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            else
            {
                return new ServerMessages(ServerOprationType.Failure) { Message = string.Format("خطا در ثبت اطلاعات") }.ToJson();
            }
        }

        public ActionResult update_reactor(EXP_POST_VAR_HEAD objecttemp)
        {
            List<string> errors = new List<string>();
            int reactor_epiu_id = 0, EPVH_EPVH_ID = 0;
            int? EPOL_EPOL_ID = 0;
            string VarDate = "";
            string sql = "";
            var EXP_POST_VAR_INSTRU = new EXP_POST_VAR_INSTRU();
            var EXP_POST_LINE = new EXP_POST_LINE();
            var EXP_POST_LINE_INSTRU = new EXP_POST_LINE_INSTRU();

            if (!string.IsNullOrEmpty(Request.Form["EPIU_EPIU_ID"]))
            {
                // EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                reactor_epiu_id = int.Parse(Request.Form["EPIU_EPIU_ID"]);
            }

            if (!string.IsNullOrEmpty(Request.Form["EPVH_EPVH_ID"]))
            {
                // EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                EPVH_EPVH_ID = int.Parse(Request.Form["EPVH_EPVH_ID"]);
                EPOL_EPOL_ID = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault();
                VarDate = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.VAR_DATE).FirstOrDefault();

            }

            var r = Request.Form["R"];
            if (reactor_epiu_id != 0)
            {
                for (int i = 1; i < 25; i++)
                {
                    try
                    {
                        string reactor_mode = "new";
                        EXP_POST_VAR_INSTRU.INFV_TIME = Request.Form["INFV_TIME"] + i.ToString();

                        if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                   .Where(xx => xx.EPVH_EPVH_ID == EPVH_EPVH_ID)
                                                   .Where(xx => xx.EPIU_EPIU_ID == reactor_epiu_id)
                                                   .Select(xx => xx.EPIV_ID).Any())
                           )
                        {
                            reactor_mode = "update";
                        }

                        // EXP_POST_VAR_INSTRU.R = short.Parse(Request.Form["R"]);
                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,KV,MVAR,A,WIND,OIL,R,INFV_TIME,EPIV_DESC,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},{4},{5},{6},{7},{8},'{9}',2,'{8}',{11} ) ",
                                            reactor_epiu_id,
                                            EPVH_EPVH_ID,
                                            Request.Form["RKV_" + i.ToString()] == "" ? "0" : Request.Form["RKV_" + i.ToString()],
                                            Request.Form["RMVAR_" + i.ToString()] == "" ? "0" : Request.Form["RMVAR_" + i.ToString()],
                                            Request.Form["RA_" + i.ToString()] == "" ? "0" : Request.Form["RA_" + i.ToString()],
                                            Request.Form["RWIND_" + i.ToString()] == "" ? "0" : Request.Form["RWIND_" + i.ToString()],
                                            Request.Form["ROIL_" + i.ToString()] == "" ? "0" : Request.Form["ROIL_" + i.ToString()],
                                            Request.Form["R"] == "" ? "0" : Request.Form["R"],
                                            EXP_POST_VAR_INSTRU.INFV_TIME,
                                            "REACTOR", VarDate, EPOL_EPOL_ID);

                        if (reactor_mode == "update")
                        {
                            sql = string.Format("update EXP_POST_VAR_INSTRU set  KV={2},MVAR={3},A={4},WIND={5},OIL={6},R={7} where  EPIU_EPIU_ID={0} and EPVH_EPVH_ID={1} and INFV_TIME='{8}' and EPIV_TYPE=2 ",
                                                reactor_epiu_id,
                                                EPVH_EPVH_ID,
                                                Request.Form["RKV_" + i.ToString()] == "" ? "0" : Request.Form["RKV_" + i.ToString()],
                                                Request.Form["RMVAR_" + i.ToString()] == "" ? "0" : Request.Form["RMVAR_" + i.ToString()],
                                                Request.Form["RA_" + i.ToString()] == "" ? "0" : Request.Form["RA_" + i.ToString()],
                                                Request.Form["RWIND_" + i.ToString()] == "" ? "0" : Request.Form["RWIND_" + i.ToString()],
                                                Request.Form["ROIL_" + i.ToString()] == "" ? "0" : Request.Form["ROIL_" + i.ToString()],
                                                Request.Form["R"] == "" ? "0" : Request.Form["R"],
                                                EXP_POST_VAR_INSTRU.INFV_TIME);
                        }

                        if (Request.Form["RKV_" + i.ToString()] != "" || Request.Form["RMVAR_" + i.ToString()] != "" ||
                            Request.Form["RA_" + i.ToString()] != "" || Request.Form["RWIND_" + i.ToString()] != "" ||
                            Request.Form["ROIL_" + i.ToString()] != "")
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در راکتور ");
                    }
                }//for
            }

            //return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات با موفقیت ثبت شد" }.ToJson();

            if (errors.Count == 0)
            {
                return Json(new { Success = "اطلاعات با موفقیت ثبت شد" }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                string errorMessages = string.Join("-", errors.ToArray());
                return Json(new { Fail = errorMessages }, JsonRequestBehavior.DenyGet);
            }
        }

        public ActionResult insert_behsad_mw()
        {
            try
            {
                List<string> success = new List<string>();
                List<string> error = new List<string>();

                //Asr.WebService.WebServiceTavanir.ValueSoapClient ValueSoapClient = new Asr.WebService.WebServiceTavanir.ValueSoapClient();
                WebServiceTavanir.Value ValueSoapClient = new WebServiceTavanir.Value();
                string token = "", AuthenticateMsg = "", UpdateMsg = "";
                ValueSoapClient.Authenticate("horm02", "123456", out AuthenticateMsg, out token);
                if (AuthenticateMsg == "Login Succeeded.")
                {
                    decimal mw = 0, mvar = 0; int behs_id = 0, EPOL_ID = 0;
                    string YEAR = "";
                    int row = int.Parse(Request.Form["row"]);
                    for (int i = 1; i <= row; i++)
                    {
                        if (!string.IsNullOrEmpty(Request.Form["EPOL_ID_" + i]))
                        {
                            EPOL_ID = int.Parse(Request.Form["EPOL_ID_" + i]);
                        }
                        try
                        {
                            if (!string.IsNullOrEmpty(Request.Form["MW_" + i]) && !string.IsNullOrEmpty(Request.Form["BEHS_" + i]))
                            {
                                behs_id = int.Parse(Request.Form["BEHS_" + i]);
                                mw = decimal.Parse(Request.Form["MW_" + i]);
                                mvar = decimal.Parse(Request.Form["MVAR_" + i]);
                                EPOL_ID = int.Parse(Request.Form["EPOL_ID_" + i]);
                                YEAR = Request.Form["VAR_DATE_" + i].Substring(0, 4);
                                // ValueSoapClient.UpdateNumericValue(behs_id, (long)Db.BKP_FINANCIAL_YEAR.Where(xx => xx.FINY_YEAR == YEAR).Select(xx => xx.BEHS_ID).FirstOrDefault(), 17782, mw, null, "horm02", token, out UpdateMsg);
                                ValueSoapClient.InsertNumericValue(behs_id, (long)Db.BKP_FINANCIAL_YEAR.Where(xx => xx.FINY_YEAR == YEAR).Select(xx => xx.BEHS_ID).FirstOrDefault(), 17782, mw * 1000000, null, "horm02", token, out UpdateMsg);
                                ValueSoapClient.InsertNumericValue(behs_id, (long)Db.BKP_FINANCIAL_YEAR.Where(xx => xx.FINY_YEAR == YEAR).Select(xx => xx.BEHS_ID).FirstOrDefault(), 17783, mvar * 1000000, null, "horm02", token, out UpdateMsg);
                                //success.Add(" اطلاعات پیک بار پست   " + Db.EXP_POST_LINE.Where(xx => xx.BEHS_ID == behs_id).Select(xx => xx.EPOL_NAME).FirstOrDefault() + "  با موفقیت ارسال شد  ");
                                success.Add(UpdateMsg);
                            }
                        }
                        catch (Exception ex)
                        {
                            error.Add(" خطا در ثبت پیک بار پست   " + Db.EXP_POST_LINE.Where(xx => xx.EPOL_ID == EPOL_ID).Select(xx => xx.EPOL_NAME).FirstOrDefault());
                        }
                    }
                    if (error.Count == 0)
                    {
                        string successMessages = string.Join("<br />", success.ToArray());
                        return new ServerMessages(ServerOprationType.Success) { Message = successMessages }.ToJson();
                    }
                    else
                    {
                        string errorMessages = string.Join("<br />", error.ToArray());
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errorMessages }.ToJson();
                    }
                }
                else
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "نام کاربری یا کلمه عبور اشتباه است" }.ToJson();
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();
            }
        }

        public ActionResult update_trans(EXP_POST_VAR_HEAD objecttemp)
        {
            int EPIU_EPIU_ID = 0, EPVH_EPVH_ID = 0;
            int? EPOL_EPOL_ID = 0;
            string sql = "", VarDate = "";
            var EXP_POST_VAR_INSTRU = new EXP_POST_VAR_INSTRU();
            var EXP_POST_LINE = new EXP_POST_LINE();
            var EXP_POST_LINE_INSTRU = new EXP_POST_LINE_INSTRU();
            List<string> errors = new List<string>();

            if (!string.IsNullOrEmpty(Request.Form["EPIU_EPIU_ID"]))
            {
                // EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
            }
            if (!string.IsNullOrEmpty(Request.Form["EPVH_EPVH_ID"]))
            {
                // EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                EPVH_EPVH_ID = int.Parse(Request.Form["EPVH_EPVH_ID"]);
                Db.Database.ExecuteSqlCommand(string.Format("update EXP_POST_VAR_HEAD set epvh_desc='{0}' where epvh_id={1}", objecttemp.EPVH_DESC, EPVH_EPVH_ID));
                EPOL_EPOL_ID = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault();
                VarDate = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.VAR_DATE).FirstOrDefault();
            }

            for (int i = 1; i < 25; i++)
            {
                #region     /////////////////////INSERT TRANS LINE

                string mode = "new";
                EXP_POST_VAR_INSTRU.INFV_TIME = i.ToString();
                if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                           .Where(xx => xx.EPVH_EPVH_ID == EPVH_EPVH_ID)
                                           .Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID)
                                           .Select(xx => xx.EPIV_ID).Any())
                    )
                {
                    mode = "update";
                }

                for (int j = 1; j < 9; j++)
                {
                    EXP_POST_LINE.EPOL_TYPE = "1";
                    EXP_POST_LINE_INSTRU.CODE_DISP = Request.Form["tl" + j];
                    if (EXP_POST_LINE_INSTRU.CODE_DISP == "--")
                    {
                        string sqldelete = string.Format("DELETE FROM EXP_POST_VAR_INSTRU WHERE EPVH_EPVH_ID={0} and LINE_COLUM={1} and EPIV_TYPE=0 and EPIU_EPIU_ID2={2}   ", EPVH_EPVH_ID, j, EPIU_EPIU_ID);
                        //string sqldelete = string.Format("DELETE FROM EXP_POST_VAR_INSTRU WHERE EPVH_EPVH_ID={0} and LINE_COLUM={1} and EPIV_TYPE=0", EPVH_EPVH_ID, j);
                        Db.Database.ExecuteSqlCommand(sqldelete);
                    }

                    if (!string.IsNullOrEmpty(EXP_POST_LINE_INSTRU.CODE_DISP) && EXP_POST_LINE_INSTRU.CODE_DISP != "--")
                    {
                        try
                        {
                            //insert into  EXP_POST_VAR_INSTRU 
                            if (!string.IsNullOrEmpty(Request.Form["TMVAR" + j.ToString() + "_" + i.ToString()]))
                                EXP_POST_VAR_INSTRU.MVAR = decimal.Parse(Request.Form["TMVAR" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MVAR = 0;

                            if (!string.IsNullOrEmpty(Request.Form["TMW" + j.ToString() + "_" + i.ToString()]))
                                EXP_POST_VAR_INSTRU.MW = decimal.Parse(Request.Form["TMW" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MW = 0;

                            EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = Db.Database.SqlQuery<int>(string.Format("select EPIU_ID  from EXP_POST_LINE_INSTRU,EXP_POST_LINE where EXP_POST_LINE_INSTRU.EINS_EINS_ID in (1,2) and upper(EXP_POST_LINE_INSTRU.CODE_DISP)=upper('{0}')  and EXP_POST_LINE_INSTRU.EINS_EINS_ID=1 and EXP_POST_LINE_INSTRU.epol_epol_id=EXP_POST_LINE.EPOL_ID and EXP_POST_LINE_INSTRU.epol_epol_id=EXP_POST_LINE.EPOL_ID and EXP_POST_LINE.epol_type is not null ",
                                                                                                        EXP_POST_LINE_INSTRU.CODE_DISP)).FirstOrDefault();

                            if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID == 0)
                            {
                                throw new Exception("line");
                            }

                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 0)
                                                       .Where(xx => xx.LINE_COLUM != j)
                                                       .Where(xx => xx.EPIU_EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID)
                                                       .Select(xx => xx.EPIV_ID).Any()))
                            {
                                throw new Exception("duplicate");
                            }

                            //EPVH_EPVH_ID = Db.Database.SqlQuery<int>
                            //    (string.Format("select EPVH_ID  from EXP_POST_VAR_HEAD where VAR_DATE='{0}' and EPOL_EPOL_ID={1}",
                            //    objecttemp.VAR_DATE,
                            //    objecttemp.EPOL_EPOL_ID)).FirstOrDefault();

                            string linemode = "new";
                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                       .Where(xx => xx.EPVH_EPVH_ID == EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 0)
                                                       .Where(xx => xx.LINE_COLUM == j)
                                                       .Where(xx => xx.EPIU_EPIU_ID2 == EPIU_EPIU_ID)
                                                       .Select(xx => xx.EPIV_ID).Any())
                               )
                            {
                                linemode = "update";
                            }

                            sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,MW,MVAR,INFV_TIME,LINE_COLUM,EPIV_DESC,EPIU_EPIU_ID2,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},{4},{5},'{6}',{7},0,'{8}',{9}) ",
                                                 EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                 EPVH_EPVH_ID,
                                                 Request.Form["TMW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                 Request.Form["TMVAR" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                                 //EXP_POST_VAR_INSTRU.MW,
                                                 //EXP_POST_VAR_INSTRU.MVAR,
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 j,
                                                 "TRANS",
                                                 EPIU_EPIU_ID,
                                                 VarDate,
                                                 EPOL_EPOL_ID);

                            if (linemode == "update")
                            {
                                sql = string.Format("update EXP_POST_VAR_INSTRU set MW={2},MVAR={3}  where EPIU_EPIU_ID={7} and  EPIV_TYPE={0}  and EPVH_EPVH_ID={1} and INFV_TIME='{4}' and EPIU_EPIU_ID2={5} and LINE_COLUM={6}  ",
                                                     0,
                                                     EPVH_EPVH_ID,
                                                     Request.Form["TMW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                     Request.Form["TMVAR" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                                     //EXP_POST_VAR_INSTRU.MW,
                                                     // EXP_POST_VAR_INSTRU.MVAR,
                                                     EXP_POST_VAR_INSTRU.INFV_TIME,
                                                     EPIU_EPIU_ID,
                                                     j,
                                                     EXP_POST_VAR_INSTRU.EPIU_EPIU_ID);
                            }

                            //INSERT TRANS LINE
                            if ((!string.IsNullOrEmpty(Request.Form["TMVAR" + j.ToString() + "_" + i.ToString()])) || (!string.IsNullOrEmpty(Request.Form["TMW" + j.ToString() + "_" + i.ToString()])))
                                Db.Database.ExecuteSqlCommand(sql);
                            else if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID != null)
                                DeleteRow(EPVH_EPVH_ID, j.ToString(), 0, EXP_POST_VAR_INSTRU.INFV_TIME, (int)EXP_POST_VAR_INSTRU.EPIU_EPIU_ID);

                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "duplicate" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " تکراری میباشد ");
                            }
                            else if (ex.Message == "line" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " اشتباه است لطفا با مرکز پیام هماهنگ کنید ");
                            }
                            else if (ex.Message != "line" && ex.Message != "duplicate")
                            {
                                errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در ترانس ");
                            }
                        }
                    }//if
                }

                #endregion

                #region             ///////////////// insert incoming
                try
                {
                    if (EPIU_EPIU_ID != 0)
                    {
                        if (!string.IsNullOrEmpty(Request.Form["IMVAR_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.MVAR = decimal.Parse(Request.Form["IMVAR_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.MVAR = 0;

                        if (!string.IsNullOrEmpty(Request.Form["IMW_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.MW = decimal.Parse(Request.Form["IMW_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.MW = 0;

                        if (!string.IsNullOrEmpty(Request.Form["IA_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.A = decimal.Parse(Request.Form["IA_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.A = 0;

                        if (!string.IsNullOrEmpty(Request.Form["IKV_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.KV = decimal.Parse(Request.Form["IKV_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.KV = 0;

                        if (!string.IsNullOrEmpty(Request.Form["TOIL_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.OIL = int.Parse(Request.Form["TOIL_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.OIL = 0;

                        if (!string.IsNullOrEmpty(Request.Form["TTHR_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.THR = int.Parse(Request.Form["TTHR_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.THR = 0;

                        if (!string.IsNullOrEmpty(Request.Form["TSEC_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.SEC = int.Parse(Request.Form["TSEC_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.SEC = 0;

                        if (!string.IsNullOrEmpty(Request.Form["TPRI_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.PRI = int.Parse(Request.Form["TPRI_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.PRI = 0;

                        if (!string.IsNullOrEmpty(Request.Form["TPOS_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.TAP = int.Parse(Request.Form["TPOS_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.TAP = 0;

                        mode = "new";

                        if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                   .Where(xx => xx.EPVH_EPVH_ID == EPVH_EPVH_ID)
                                                   .Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID)
                                                   .Where(xx => xx.EPIV_TYPE == 6)
                                                   .Select(xx => xx.EPIV_ID).Any())
                            )
                        {
                            mode = "update";
                        }

                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,MW,MVAR,INFV_TIME,A,KV,OKV,EPIV_DESC,oil,thr,sec,pri,tap,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},{4},{5},{6},{7},'{8}',{9},{10},{11},{12},{13},6,'{14}',{15}) ",
                                             EPIU_EPIU_ID,
                                             EPVH_EPVH_ID,
                                             Request.Form["IMW_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                             Request.Form["IMVAR_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             Request.Form["IA_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.A.ToString(),
                                             Request.Form["IKV_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.KV.ToString(),
                                             Request.Form["OKV"] == "" ? "0" : Request.Form["OKV"],
                                             "TRNAS INCOMING",
                                             Request.Form["TOIL_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.OIL.ToString(),
                                             Request.Form["TTHR_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.THR.ToString(),
                                             Request.Form["TSEC_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.SEC.ToString(),
                                             Request.Form["TPRI_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.PRI.ToString(),
                                             Request.Form["TPOS_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.TAP.ToString(),
                                             VarDate, EPOL_EPOL_ID
                                             );

                        if (mode == "update")
                        {
                            sql = string.Format("update EXP_POST_VAR_INSTRU set   MW ={2} , MVAR={3},A={5},KV={6},OKV={7},oil={8},thr={9},sec={10},pri={11},tap={12} where INFV_TIME={4} and EPIU_EPIU_ID={0}  and EPVH_EPVH_ID= {1}  and EPIV_TYPE=6 ",
                                                 EPIU_EPIU_ID,
                                                 EPVH_EPVH_ID,
                                                 Request.Form["IMW_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                 Request.Form["IMVAR_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 Request.Form["IA_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.A.ToString(),
                                                 Request.Form["IKV_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.KV.ToString(),
                                                 Request.Form["OKV"] == "" ? "0" : Request.Form["OKV"],
                                                 Request.Form["TOIL_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.OIL.ToString(),
                                                 Request.Form["TTHR_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.THR.ToString(),
                                                 Request.Form["TSEC_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.SEC.ToString(),
                                                 Request.Form["TPRI_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.PRI.ToString(),
                                                 Request.Form["TPOS_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.TAP.ToString()
                                                );
                        }

                        ///INSERT TRANS INCOMING
                        ///
                        if (
                            //(!string.IsNullOrEmpty(Request.Form["TIMVAR_" + i.ToString()])) ||
                            (!string.IsNullOrEmpty(Request.Form["IMW_" + i.ToString()]))// ||
                                                                                        // (!string.IsNullOrEmpty(Request.Form["TIA_" + i.ToString()])) ||
                                                                                        //(!string.IsNullOrEmpty(Request.Form["TIKV_" + i.ToString()])) ||
                                                                                        //// (!string.IsNullOrEmpty(Request.Form["TOIL_" + i.ToString()])) ||
                                                                                        // (!string.IsNullOrEmpty(Request.Form["TTHR_" + i.ToString()])) ||
                                                                                        // (!string.IsNullOrEmpty(Request.Form["TSEC_" + i.ToString()])) ||
                                                                                        // (!string.IsNullOrEmpty(Request.Form["TPRI_" + i.ToString()]))
                            )
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }
                        else if (EPIU_EPIU_ID != null)
                        {
                            DeleteRow(EPVH_EPVH_ID, "null", 6, EXP_POST_VAR_INSTRU.INFV_TIME, EPIU_EPIU_ID);
                        }
                    }
                }
                #endregion

                catch (Exception ex)
                {
                    errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در INCOMING ");
                }
            }

            if (errors.Count == 0)
            {
                // return Json(new { Success = "اطلاعات با موفقیت ثبت شد" }, JsonRequestBehavior.DenyGet);
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            else
            {
                string errorMessages = string.Join("-", errors.ToArray());
                return Json(new { Fail = errorMessages }, JsonRequestBehavior.DenyGet);
            }
        }
        public ActionResult
            Update_Solar_Panel(EXP_POST_VAR_HEAD objecttemp)
        {
            #region ////Header
            string /*mode = "new",*/ sql = string.Empty;
            int EPVH_EPVH_ID = 0;
            int? EPOL_EPOL_ID = 0;
            string VarDate = "";

            List<string> errors = new List<string>();
            //insert into EXP_POST_VAR_HEAD
            //objecttemp.EPVT_EPVT_ID = Db.exp_type;

            var EXP_POST_VAR_INSTRU = new EXP_POST_VAR_INSTRU();
            var EXP_POST_LINE = new EXP_POST_LINE();
            var EXP_POST_LINE_INSTRU = new EXP_POST_LINE_INSTRU();
            if (!string.IsNullOrEmpty(Request.Form["EPVH_EPVH_ID"]))
            {
                // EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                EPVH_EPVH_ID = int.Parse(Request.Form["EPVH_EPVH_ID"]);
                EPOL_EPOL_ID = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault();
                VarDate = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.VAR_DATE).FirstOrDefault();

            }

            EXP_POST_VAR_INSTRU.EPVH_EPVH_ID = EPVH_EPVH_ID;
            // Db.Database.SqlQuery<int>(string.Format("select EPVH_ID from EXP_POST_VAR_HEAD where VAR_DATE='{0}' and EPOL_EPOL_ID={1} and epvh_type=1", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID)).FirstOrDefault();

            Db.Database.ExecuteSqlCommand(string.Format("update EXP_POST_VAR_HEAD set epvh_desc='{0}' where epvh_id={1}", objecttemp.EPVH_DESC, EXP_POST_VAR_INSTRU.EPVH_EPVH_ID));

            int EPIU_EPIU_ID = 0;






            #endregion
            for (int j = 1; j < 3; j++)
            {
                EPIU_EPIU_ID = 0;
                if (!string.IsNullOrEmpty(Request.Form["EPIU_ID" + "_" + j.ToString()]))
                {
                    // EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                    EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_ID" + "_" + j.ToString()]);
                }
                if (EPIU_EPIU_ID != 0)
                {
                    for (int i = 1; i < 25; i++)
                    {
                        EXP_POST_VAR_INSTRU.INFV_TIME = Request.Form["INFV_TIME"] + i.ToString();

                        #region//////////////////////////////////////////////InSERT LINE





                        try
                        {
                            ////insert into EXP_POST_VAR

                            if (Request.Form["MW" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.MW = decimal.Parse(Request.Form["MW" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MW = 0;

                            if (Request.Form["DAILY_MW" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.DAILY_MW = decimal.Parse(Request.Form["DAILY_MW" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.DAILY_MW = 0;

                            if (Request.Form["COUNTER" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.COUNTER = decimal.Parse(Request.Form["COUNTER" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.COUNTER = 0;







                            string Mode = "new";
                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                       .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 13)
                                                       .Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID)

                                                       .Select(xx => xx.EPIV_ID).Any())
                               )
                            {
                                Mode = "update";
                            }

                            sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,MW,DAILY_MW,COUNTER,INFV_TIME,LINE_COLUM,EPIV_DESC,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},{4},{5},{6},'{7}',13,'{8}',{9} ) ",
                                                 EPIU_EPIU_ID,
                                                 EPVH_EPVH_ID,
                                                 Request.Form["MW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                 Request.Form["DAILY_MW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.DAILY_MW.ToString(),
                                                 Request.Form["COUNTER" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.COUNTER.ToString(),
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 j,
                                                 "Solar Panel",
                                                  VarDate, EPOL_EPOL_ID
                                                 );

                            if (Mode == "update")
                            {
                                sql = string.Format("update EXP_POST_VAR_INSTRU set MW={2},DAILY_MW={3},COUNTER={4}   where   EPIU_EPIU_ID={0} and EPVH_EPVH_ID={1} and INFV_TIME='{5}' and LINE_COLUM={6} and EPIV_TYPE=13  ",
                                                     EPIU_EPIU_ID,
                                                     EPVH_EPVH_ID,
                                                     Request.Form["MW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                     Request.Form["DAILY_MW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.DAILY_MW.ToString(),
                                                     Request.Form["COUNTER" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.COUNTER.ToString(),
                                                     EXP_POST_VAR_INSTRU.INFV_TIME,

                                                     j);
                            }

                            if ((Request.Form["DAILY_MW" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["MW" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["COUNTER" + j.ToString() + "_" + i.ToString()] != "") || Mode == "update")
                            {
                                Db.Database.ExecuteSqlCommand(sql);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "duplicate" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " تکراری میباشد ");
                            }
                            else if (ex.Message == "line" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " اشتباه است لطفا با مرکز پیام هماهنگ کنید ");
                            }
                            else if (ex.Message != "line" && ex.Message != "duplicate")
                            {
                                errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در خط " + EXP_POST_LINE.CODE_DISP);
                            }
                        }

                    }
                    #endregion
                }
            }

            if (errors.Count == 0)
            {
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            else
            {
                string errorMessages = string.Join("<br />", errors.ToArray());
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errorMessages }.ToJson();
            }
        }

        public ActionResult update_line(EXP_POST_VAR_HEAD objecttemp)
        {
            #region ////Header
            string /*mode = "new",*/ sql = string.Empty;
            int EPVH_EPVH_ID = 0;
            int? EPOL_EPOL_ID = 0;
            string VarDate = "";
            List<string> errors = new List<string>();
            //insert into EXP_POST_VAR_HEAD
            //objecttemp.EPVT_EPVT_ID = Db.exp_type;

            var EXP_POST_VAR_INSTRU = new EXP_POST_VAR_INSTRU();
            var EXP_POST_LINE = new EXP_POST_LINE();
            var EXP_POST_LINE_INSTRU = new EXP_POST_LINE_INSTRU();
            if (!string.IsNullOrEmpty(Request.Form["EPVH_EPVH_ID"]))
            {
                // EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                EPVH_EPVH_ID = int.Parse(Request.Form["EPVH_EPVH_ID"]);
                EPOL_EPOL_ID = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault();
                VarDate = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.VAR_DATE).FirstOrDefault();

            }

            EXP_POST_VAR_INSTRU.EPVH_EPVH_ID = EPVH_EPVH_ID;
            // Db.Database.SqlQuery<int>(string.Format("select EPVH_ID from EXP_POST_VAR_HEAD where VAR_DATE='{0}' and EPOL_EPOL_ID={1} and epvh_type=1", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID)).FirstOrDefault();

            Db.Database.ExecuteSqlCommand(string.Format("update EXP_POST_VAR_HEAD set epvh_desc='{0}' where epvh_id={1}", objecttemp.EPVH_DESC, EXP_POST_VAR_INSTRU.EPVH_EPVH_ID));

            int EPIU_EPIU_ID = 0, EPIU_EPIU_ID2 = 0;

            if (!string.IsNullOrEmpty(Request.Form["EPIU_EPIU_ID"]))
            {
                // EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
            }

            decimal epiu_id = Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID).Where(xx => xx.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID).Select(xx => xx.EPIU_ID).FirstOrDefault();

            if (!string.IsNullOrEmpty(Request.Form["EPIU_EPIU_ID2"]))
            {
                //EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID2"]);
                EPIU_EPIU_ID2 = int.Parse(Request.Form["EPIU_EPIU_ID2"]);
            }

            decimal reactor_epiu_id = EPIU_EPIU_ID2;

            #endregion

            for (int i = 1; i < 25; i++)
            {
                EXP_POST_VAR_INSTRU.INFV_TIME = Request.Form["INFV_TIME"] + i.ToString();

                #region//////////////////////////////////////////////InSERT LINE

                for (int j = 1; j < 9; j++)
                {
                    EXP_POST_LINE.EPOL_TYPE = "1";
                    EXP_POST_LINE_INSTRU.CODE_DISP = Request.Form["l" + j];
                    if (EXP_POST_LINE_INSTRU.CODE_DISP == "--")
                    {
                        Db.Database.ExecuteSqlCommand(string.Format("DELETE FROM EXP_POST_VAR_INSTRU WHERE EPVH_EPVH_ID={0} and LINE_COLUM={1} and EPIV_TYPE=1", EXP_POST_VAR_INSTRU.EPVH_EPVH_ID, j));
                    }

                    if (!string.IsNullOrEmpty(EXP_POST_LINE_INSTRU.CODE_DISP) && EXP_POST_LINE_INSTRU.CODE_DISP != "--")
                    {
                        try
                        {
                            ////insert into EXP_POST_LINE
                            if (Request.Form["MVAR" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.MVAR = decimal.Parse(Request.Form["MVAR" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MVAR = null;

                            if (Request.Form["MW" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.MW = decimal.Parse(Request.Form["MW" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MW = 0;

                            if (Request.Form["KV" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.KV = decimal.Parse(Request.Form["KV" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.KV = 0;


                            EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = Db.Database.SqlQuery<int>(string.Format("select EPIU_ID  from EXP_POST_LINE_INSTRU,EXP_POST_LINE where EXP_POST_LINE_INSTRU.EINS_EINS_ID in (1,2) and  upper(EXP_POST_LINE_INSTRU.CODE_DISP)=upper('{0}') and EXP_POST_LINE_INSTRU.epol_epol_id=EXP_POST_LINE.EPOL_ID and EXP_POST_LINE.epol_type is not null ",
                                                                                                        EXP_POST_LINE_INSTRU.CODE_DISP)).FirstOrDefault();

                            if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID == 0)
                            {
                                throw new Exception("line");
                            }

                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 1)
                                                       .Where(xx => xx.LINE_COLUM != j)
                                                       .Where(xx => xx.EPIU_EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID)
                                                       .Select(xx => xx.EPIV_ID).Any()))
                            {
                                throw new Exception("duplicate");
                            }

                            if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID == 0)
                            {
                                sql = string.Format("insert into EXP_POST_LINE_INSTRU (EXP_POST_LINE_INSTRU.EPOL_EPOL_ID_LINE,EPOL_EPOL_ID) values ({0},{1}) ",
                                      EXP_POST_LINE_INSTRU.EPOL_EPOL_ID_LINE,
                                      EXP_POST_LINE_INSTRU.EPOL_EPOL_ID);

                                // Db.Database.ExecuteSqlCommand(sql);
                                EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = Db.Database.SqlQuery<int>(string.Format("select EPIU_ID  from EXP_POST_LINE_INSTRU,EXP_POST_LINE where EXP_POST_LINE_INSTRU.EINS_EINS_ID in (1,2) and  upper(EXP_POST_LINE_INSTRU.CODE_DISP)=upper('{0}') and EXP_POST_LINE_INSTRU.epol_epol_id=EXP_POST_LINE.EPOL_ID and EXP_POST_LINE.epol_type is not null ",
                                                                                                            EXP_POST_LINE_INSTRU.EPOL_EPOL_ID_LINE,
                                                                                                            objecttemp.EPOL_EPOL_ID)).FirstOrDefault();
                            }

                            string linemode = "new";
                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                       .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 1)
                                                       .Where(xx => xx.LINE_COLUM == j)
                                                       .Select(xx => xx.EPIV_ID).Any())
                               )
                            {
                                linemode = "update";
                            }

                            sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,MW,MVAR,INFV_TIME,LINE_COLUM,EPIV_DESC,KV,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},{4},{5},'{6}',{7},1,'{8}',{9}) ",
                                                 EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["MW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                 Request.Form["MVAR" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 j,
                                                 "LINE",
                                                 Request.Form["KV" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.KV.ToString(),
                                                 VarDate,
                                                 EPOL_EPOL_ID

                                                 );

                            if (linemode == "update")
                            {
                                sql = string.Format("update EXP_POST_VAR_INSTRU set MW={2},MVAR={3},KV={5},EPIU_EPIU_ID={0}   where    EPVH_EPVH_ID={1} and INFV_TIME='{4}' and LINE_COLUM={6} and EPIV_TYPE=1  ",
                                                     EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                     EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                     Request.Form["MW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                     Request.Form["MVAR" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                                     EXP_POST_VAR_INSTRU.INFV_TIME,
                                                     Request.Form["KV" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.KV.ToString(),
                                                     j);
                            }

                            if ((Request.Form["MVAR" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["MW" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["KV" + j.ToString() + "_" + i.ToString()] != "") || linemode == "update")
                            {
                                Db.Database.ExecuteSqlCommand(sql);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "duplicate" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " تکراری میباشد ");
                            }
                            else if (ex.Message == "line" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " اشتباه است لطفا با مرکز پیام هماهنگ کنید ");
                            }
                            else if (ex.Message != "line" && ex.Message != "duplicate")
                            {
                                errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در خط " + EXP_POST_LINE.CODE_DISP);
                            }
                        }
                    }//if
                }
                #endregion

            }

            if (errors.Count == 0)
            {
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            else
            {
                string errorMessages = string.Join("<br />", errors.ToArray());
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errorMessages }.ToJson();
            }
        }

        public ActionResult update_lvdc(EXP_POST_VAR_HEAD objecttemp)
        {
            #region ////Header

            string mode = "new", sql = string.Empty;
            int EPVH_EPVH_ID = 0;
            int? EPOL_EPOL_ID = 0;
            string VarDate = "";
            List<string> errors = new List<string>();
            //insert into EXP_POST_VAR_HEAD
            //objecttemp.EPVT_EPVT_ID = Db.exp_type;

            var EXP_POST_VAR_INSTRU = new EXP_POST_VAR_INSTRU();
            var EXP_POST_LINE = new EXP_POST_LINE();
            var EXP_POST_LINE_INSTRU = new EXP_POST_LINE_INSTRU();
            if (!string.IsNullOrEmpty(Request.Form["EPVH_EPVH_ID"]))
            {
                // EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                EPVH_EPVH_ID = int.Parse(Request.Form["EPVH_EPVH_ID"]);
                EPOL_EPOL_ID = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault();
                VarDate = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.VAR_DATE).FirstOrDefault();

            }

            EXP_POST_VAR_INSTRU.EPVH_EPVH_ID = EPVH_EPVH_ID;// Db.Database.SqlQuery<int>(string.Format("select EPVH_ID from EXP_POST_VAR_HEAD where VAR_DATE='{0}' and EPOL_EPOL_ID={1} and epvh_type=1", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID)).FirstOrDefault();
            sql = string.Format("update EXP_POST_VAR_HEAD set epvh_desc='{0}' where epvh_id={1}", objecttemp.EPVH_DESC, EXP_POST_VAR_INSTRU.EPVH_EPVH_ID);
            Db.Database.ExecuteSqlCommand(sql);

            int EPIU_EPIU_ID = 0, EPIU_EPIU_ID2 = 0;

            if (!string.IsNullOrEmpty(Request.Form["EPIU_EPIU_ID"]))
            {
                // EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
            }

            decimal epiu_id = Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID).Where(xx => xx.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID).Select(xx => xx.EPIU_ID).FirstOrDefault();

            if (!string.IsNullOrEmpty(Request.Form["EPIU_EPIU_ID2"]))
            {
                //EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID2"]);
                EPIU_EPIU_ID2 = int.Parse(Request.Form["EPIU_EPIU_ID2"]);
            }

            decimal reactor_epiu_id = EPIU_EPIU_ID2;

            #endregion

            for (int i = 1; i < 25; i++)
            {
                EXP_POST_VAR_INSTRU.INFV_TIME = Request.Form["INFV_TIME"] + i.ToString();

                #region  /////////////////////////insert AUX,TEMP,HUMI

                try
                {
                    decimal EPIU_ID = Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID).Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID).Select(xx => xx.EPIU_ID).FirstOrDefault();

                    mode = "new";

                    if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                               .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                               .Where(xx => xx.EPIV_TYPE == 7)
                                               .Select(xx => xx.EPIV_ID).Any())
                        )
                    {
                        mode = "update";
                    }

                    sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,INFV_TIME,TEMP,HUMI,AUX_A,AUX_V,AUX_TG,EPIV_DESC,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},'{2}','{3}','{4}','{5}','{6}','{7}',7,'{8}',{9} ) ",
                                         EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                         EXP_POST_VAR_INSTRU.INFV_TIME,
                                         Request.Form["TEMP_" + i.ToString()] == "" ? "" : Request.Form["TEMP_" + i.ToString()],
                                         Request.Form["HUMI_" + i.ToString()] == "" ? "" : Request.Form["HUMI_" + i.ToString()],
                                         Request.Form["AUXA_" + i.ToString()] == "" ? "" : Request.Form["AUXA_" + i.ToString()],
                                         Request.Form["AUXV_" + i.ToString()] == "" ? "" : Request.Form["AUXV_" + i.ToString()],
                                         Request.Form["AUXTG_" + i.ToString()] == "" ? "" : Request.Form["AUXTG_" + i.ToString()],
                                         "AUX,TEMP,HUMI",
                                          VarDate, EPOL_EPOL_ID
                                         );

                    if (mode == "update")
                    {
                        sql = string.Format("update EXP_POST_VAR_INSTRU set   TEMP ='{2}' ,HUMI='{3}',AUX_A='{4}' ,AUX_V='{5}' ,AUX_TG='{6}'  where INFV_TIME={1}  and EPVH_EPVH_ID= {0}  and EPIV_TYPE=7 ",
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             Request.Form["TEMP_" + i.ToString()] == "" ? "" : Request.Form["TEMP_" + i.ToString()],
                                             Request.Form["HUMI_" + i.ToString()] == "" ? "" : Request.Form["HUMI_" + i.ToString()],
                                             Request.Form["AUXA_" + i.ToString()] == "" ? "" : Request.Form["AUXA_" + i.ToString()],
                                             Request.Form["AUXV_" + i.ToString()] == "" ? "" : Request.Form["AUXV_" + i.ToString()],
                                             Request.Form["AUXTG_" + i.ToString()] == "" ? "" : Request.Form["AUXTG_" + i.ToString()]
                      );
                    }

                    if (Request.Form["TEMP_" + i.ToString()] != ""
                        || Request.Form["HUMI_" + i.ToString()] != ""
                        || Request.Form["AUXA_" + i.ToString()] != ""
                        || Request.Form["AUXV_" + i.ToString()] != ""
                        || Request.Form["AUXTG_" + i.ToString()] != ""
                        )
                    {
                        Db.Database.ExecuteSqlCommand(sql);
                    }
                }
                catch (Exception ex)
                {
                    errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در قسمت Temprature ");
                }

                #endregion

                #region/////////////////////////////INSERT L.V DC

                string mode_48 = "", mode_110 = "", mode_220 = "";
                for (int j = 1; j < 3; j++)
                {
                    try
                    {
                        mode_48 = ""; mode_110 = ""; mode_220 = "";
                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,A,V,INFV_TIME,EPIV_DESC,LINE_COLUM,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},'{1}','{2}',{3},'{4}',{5},3,'{6}',{7} ) ",
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             Request.Form["48A" + j.ToString() + "_" + i.ToString()] == "" ? "" : Request.Form["48A" + j.ToString() + "_" + i.ToString()],
                                             Request.Form["48V" + j.ToString() + "_" + i.ToString()] == "" ? "" : Request.Form["48V" + j.ToString() + "_" + i.ToString()],
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             "L.V_DC_48",
                                             j, VarDate, EPOL_EPOL_ID);

                        if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                   .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                   .Where(xx => xx.LINE_COLUM == j)
                                                   .Where(xx => xx.EPIV_DESC == "L.V_DC_48")
                                                   .Select(xx => xx.EPIV_ID).Any())
                             )
                        {
                            mode_48 = "update";
                        }

                        if (mode_48 == "update")
                        {
                            sql = string.Format("update  EXP_POST_VAR_INSTRU set A='{1}' ,V='{2}' WHERE   EPVH_EPVH_ID={0} AND INFV_TIME='{3}' AND EPIV_DESC='{4}' AND LINE_COLUM= {5} and EPIV_TYPE=3 ",
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["48A" + j.ToString() + "_" + i.ToString()] == "" ? "" : Request.Form["48A" + j.ToString() + "_" + i.ToString()],
                                                 Request.Form["48V" + j.ToString() + "_" + i.ToString()] == "" ? "" : Request.Form["48V" + j.ToString() + "_" + i.ToString()],
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 "L.V_DC_48",
                                                 j);
                        }

                        if (Request.Form["48A" + j.ToString() + "_" + i.ToString()] != "" || Request.Form["48V" + j.ToString() + "_" + i.ToString()] != "")
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }

                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,A,V,INFV_TIME,EPIV_DESC,LINE_COLUM,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},'{1}','{2}',{3},'{4}',{5},4,'{6}',{7} ) ",
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             Request.Form["110A" + j.ToString() + "_" + i.ToString()] == "" ? "" : Request.Form["110A" + j.ToString() + "_" + i.ToString()],
                                             Request.Form["110V" + j.ToString() + "_" + i.ToString()] == "" ? "" : Request.Form["110V" + j.ToString() + "_" + i.ToString()],
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             "L.V_DC_110",
                                             j, VarDate, EPOL_EPOL_ID);

                        if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                   .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                   .Where(xx => xx.LINE_COLUM == j)
                                                   .Where(xx => xx.EPIV_DESC == "L.V_DC_110")
                                                   .Select(xx => xx.EPIV_ID).Any())
                             )
                        {
                            mode_110 = "update";
                        }

                        if (mode_110 == "update")
                        {
                            sql = string.Format("update  EXP_POST_VAR_INSTRU set A='{1}' ,V='{2}' WHERE   EPVH_EPVH_ID={0} AND INFV_TIME='{3}' AND EPIV_DESC='{4}' AND LINE_COLUM= {5} and EPIV_TYPE=4 ",
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["110A" + j.ToString() + "_" + i.ToString()] == "" ? "" : Request.Form["110A" + j.ToString() + "_" + i.ToString()],
                                                 Request.Form["110V" + j.ToString() + "_" + i.ToString()] == "" ? "" : Request.Form["110V" + j.ToString() + "_" + i.ToString()],
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 "L.V_DC_110",
                                                 j);
                        }

                        if (Request.Form["110A" + j.ToString() + "_" + i.ToString()] != "" || Request.Form["110V" + j.ToString() + "_" + i.ToString()] != "")
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }

                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,A,V,INFV_TIME,EPIV_DESC,LINE_COLUM,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},'{1}','{2}',{3},'{4}',{5},5,'{6}',{7} ) ",
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             Request.Form["220A" + j.ToString() + "_" + i.ToString()] == "" ? "" : Request.Form["220A" + j.ToString() + "_" + i.ToString()],
                                             Request.Form["220V" + j.ToString() + "_" + i.ToString()] == "" ? "" : Request.Form["220V" + j.ToString() + "_" + i.ToString()],
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             "L.V_DC_220",
                                             j, VarDate, EPOL_EPOL_ID);

                        if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                   .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                   .Where(xx => xx.LINE_COLUM == j)
                                                   .Where(xx => xx.EPIV_DESC == "L.V_DC_220")
                                                   .Select(xx => xx.EPIV_ID).Any())
                             )
                        {
                            mode_220 = "update";
                        }

                        if (mode_220 == "update")
                        {
                            sql = string.Format("update  EXP_POST_VAR_INSTRU set A='{1}' ,V='{2}' WHERE   EPVH_EPVH_ID={0} AND INFV_TIME='{3}' AND EPIV_DESC='{4}' AND LINE_COLUM= {5} and EPIV_TYPE=5",
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["220A" + j.ToString() + "_" + i.ToString()] == "" ? "" : Request.Form["220A" + j.ToString() + "_" + i.ToString()],
                                                 Request.Form["220V" + j.ToString() + "_" + i.ToString()] == "" ? "" : Request.Form["220V" + j.ToString() + "_" + i.ToString()],
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 "L.V_DC_220",
                                                 j);
                        }

                        if (Request.Form["110A" + j.ToString() + "_" + i.ToString()] != "" || Request.Form["110V" + j.ToString() + "_" + i.ToString()] != "")
                        {
                            if (j == 1)
                            {
                                Db.Database.ExecuteSqlCommand(sql);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در قسمت L.V DC & AC AUX & TEMPERATURE & HUMIDITY ");
                    }
                }
                #endregion

            }

            if (errors.Count == 0)
            {
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            else
            {
                string errorMessages = string.Join("<br />", errors.ToArray());
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errorMessages }.ToJson();
            }
        }

        public ActionResult update_unit(EXP_POST_VAR_HEAD objecttemp)
        {
            string/* mode = "new",*/ sql = string.Empty;
            int? EPOL_EPOL_ID = 0;
            string VarDate = "";

            #region//insert into EXP_POST_VAR_HEAD

            //objecttemp.EPVT_EPVT_ID = Db.exp_type;
            var q = from b in Db.EXP_POST_VAR_HEAD
                    where b.VAR_DATE == objecttemp.VAR_DATE && b.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID && b.EPVH_TYPE == 2
                    select new { b.EPVH_ID };

            if (!q.Any())
            {
                sql = string.Format("insert into EXP_POST_VAR_HEAD (VAR_DATE,EPOL_EPOL_ID,epvh_type,EPVT_EPVT_ID,EPVH_DESC) values ('{0}','{1}',2,{2},'{3}') ", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID, 1, objecttemp.EPVH_DESC);
                Db.Database.ExecuteSqlCommand(sql);
            }

            var EXP_POST_VAR_INSTRU = new EXP_POST_VAR_INSTRU();
            var EXP_POST_LINE = new EXP_POST_LINE();
            var EXP_POST_LINE_INSTRU = new EXP_POST_LINE_INSTRU();

            EXP_POST_VAR_INSTRU.EPVH_EPVH_ID = Db.Database.SqlQuery<int>(string.Format("select EPVH_ID from EXP_POST_VAR_HEAD where VAR_DATE='{0}' and EPOL_EPOL_ID={1} and epvh_type=2", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID)).FirstOrDefault();
            sql = string.Format("update EXP_POST_VAR_HEAD set epvh_desc='{0}' where epvh_id={1}", objecttemp.EPVH_DESC, EXP_POST_VAR_INSTRU.EPVH_EPVH_ID);
            Db.Database.ExecuteSqlCommand(sql);

            EPOL_EPOL_ID = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault();
            VarDate = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID).Select(xx => xx.VAR_DATE).FirstOrDefault();

            #endregion

            List<string> errors = new List<string>();
            int EPIU_EPIU_ID = 0, EPIU_EPIU_ID2 = 0;

            if (!string.IsNullOrEmpty(Request.Form["EPIU_EPIU_ID"]))
            {
                //EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
            }

            decimal epiu_id = Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID).Where(xx => xx.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID).Select(xx => xx.EPIU_ID).FirstOrDefault();

            if (!string.IsNullOrEmpty(Request.Form["EPIU_EPIU_ID2"]))
            {
                //EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID2"]);
                EPIU_EPIU_ID2 = int.Parse(Request.Form["EPIU_EPIU_ID2"]);
            }

            decimal reactor_epiu_id = EPIU_EPIU_ID2;

            for (int i = 1; i < 25; i++)
            {
                EXP_POST_VAR_INSTRU.INFV_TIME = Request.Form["INFV_TIME"] + i.ToString();

                #region//////////////////////////////////////////////InSERT UNIT

                for (int j = 1; j < 9; j++)
                {
                    EXP_POST_LINE.EPOL_TYPE = "2";
                    EXP_POST_LINE_INSTRU.CODE_DISP = Request.Form["U" + j];
                    if (EXP_POST_LINE_INSTRU.CODE_DISP == "--")
                    {
                        string sqldelete = string.Format("DELETE FROM EXP_POST_VAR_INSTRU WHERE EPVH_EPVH_ID={0} and LINE_COLUM={1} and EPIV_TYPE=9", EXP_POST_VAR_INSTRU.EPVH_EPVH_ID, j);
                        Db.Database.ExecuteSqlCommand(sqldelete);
                    }

                    if (!string.IsNullOrEmpty(EXP_POST_LINE_INSTRU.CODE_DISP) && EXP_POST_LINE_INSTRU.CODE_DISP != "--")
                    {
                        try
                        {
                            if (Request.Form["uMVAR" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.MVAR = decimal.Parse(Request.Form["uMVAR" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MVAR = 0;

                            if (Request.Form["uMW" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.MW = decimal.Parse(Request.Form["uMW" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MW = 0;

                            if (Request.Form["uKV" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.KV = decimal.Parse(Request.Form["uKV" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.KV = 0;

                            EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = Db.Database.SqlQuery<int>(string.Format("select EPIU_ID  from EXP_POST_LINE_INSTRU,EXP_POST_LINE where EXP_POST_LINE_INSTRU.EINS_EINS_ID in (1,2) and  upper(EXP_POST_LINE_INSTRU.CODE_DISP)=upper('{0}') and EXP_POST_LINE_INSTRU.epol_epol_id=EXP_POST_LINE.EPOL_ID and EXP_POST_LINE.epol_type is not null ",
                                                                                                        EXP_POST_LINE_INSTRU.CODE_DISP, objecttemp.EPOL_EPOL_ID)).FirstOrDefault();

                            if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID == 0)
                            {
                                throw new Exception("line");
                            }

                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 9)
                                                       .Where(xx => xx.LINE_COLUM != j)
                                                       .Where(xx => xx.EPIU_EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID)
                                                       .Select(xx => xx.EPIV_ID).Any()))
                            {
                                throw new Exception("duplicate");
                            }

                            string linemode = "new";
                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                       .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 9)
                                                       .Where(xx => xx.LINE_COLUM == j)
                                                       .Select(xx => xx.EPIV_ID).Any())
                               )
                            {
                                linemode = "update";

                            }
                            sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,MW,MVAR,INFV_TIME,LINE_COLUM,EPIV_DESC,KV,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},{4},{5},'{6}',{7},9,'{8}',{9} ) ",
                                                 EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["uMW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                 Request.Form["uMVAR" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 j,
                                                 "UNIT",
                                                 Request.Form["uKV" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.KV.ToString(),
                                                  VarDate, EPOL_EPOL_ID
                                                 );

                            if (linemode == "update")
                            {
                                sql = string.Format("update EXP_POST_VAR_INSTRU set MW={2},MVAR={3},KV={5},EPIU_EPIU_ID={0}   where    EPVH_EPVH_ID={1} and INFV_TIME='{4}' and LINE_COLUM={6} and EPIV_TYPE=9  ",
                                                     EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                     EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                     Request.Form["uMW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                     Request.Form["uMVAR" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                                     EXP_POST_VAR_INSTRU.INFV_TIME,
                                                     Request.Form["uKV" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.KV.ToString(),
                                                     j);
                            }

                            if ((Request.Form["uMVAR" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["uMW" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["uMW" + j.ToString() + "_" + i.ToString()] != ""))
                            {
                                Db.Database.ExecuteSqlCommand(sql);
                            }
                            else if (EPIU_EPIU_ID != null)
                            {
                                DeleteRow(EXP_POST_VAR_INSTRU.EPVH_EPVH_ID, "", 9, EXP_POST_VAR_INSTRU.INFV_TIME, (int)EXP_POST_VAR_INSTRU.EPIU_EPIU_ID);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "duplicate" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " تکراری میباشد ");
                            }
                            else if (ex.Message == "line" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " اشتباه است لطفا با مرکز پیام هماهنگ کنید ");
                            }
                            else if (ex.Message != "line" && ex.Message != "duplicate")
                            {
                                errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در Unit " + EXP_POST_LINE_INSTRU.CODE_DISP);
                            }
                        }
                    }
                }

                #endregion

            }

            if (errors.Count == 0)
            {
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            else
            {
                string errorMessages = string.Join("<br />", errors.ToArray());
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errorMessages }.ToJson();
            }
        }

        public ActionResult update_dist_trans(EXP_POST_VAR_HEAD objecttemp)
        {
            List<string> errors = new List<string>();
            int EPIU_EPIU_ID = 0, EPVH_EPVH_ID = 0;
            int? EPOL_EPOL_ID = 0;
            string VarDate = "";

            string sql = "";
            var EXP_POST_VAR_INSTRU = new EXP_POST_VAR_INSTRU();
            var EXP_POST_LINE = new EXP_POST_LINE();
            var EXP_POST_LINE_INSTRU = new EXP_POST_LINE_INSTRU();
            if (!string.IsNullOrEmpty(Request.Form["EPVH_EPVH_ID"]))
            {
                // EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                EPVH_EPVH_ID = int.Parse(Request.Form["EPVH_EPVH_ID"]);
                EPOL_EPOL_ID = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault();
                VarDate = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.VAR_DATE).FirstOrDefault();

                Db.Database.ExecuteSqlCommand(string.Format("update EXP_POST_VAR_HEAD set epvh_desc='{0}' where epvh_id={1}", objecttemp.EPVH_DESC, EPVH_EPVH_ID));

            }
            for (int i = 1; i < 25; i++)
            {
                EXP_POST_VAR_INSTRU.INFV_TIME = Request.Form["INFV_TIME"] + i.ToString();

                #region    /////////////////////INSERT TRANS LINE

                string mode = "new";

                mode = "new";


                if (!string.IsNullOrEmpty(Request.Form["EPIU_EPIU_ID"]))
                {
                    EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                    EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);

                    var epiuquery = (from b in Db.EXP_POST_LINE_INSTRU
                                     where b.EPOL_EPOL_ID == Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault()
                                           && b.CODE_DISP == Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_ID == EPIU_EPIU_ID).Select(xx => xx.CODE_DISP).FirstOrDefault()
                                     select b.EPIU_ID).ToArray();

                    if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                               .Where(xx => xx.EPVH_EPVH_ID == EPVH_EPVH_ID)
                                               .Where(xx => xx.EPIV_TYPE == 8 && epiuquery.Contains((int)xx.EPIU_EPIU_ID))
                                               .Select(xx => xx.EPIV_ID).Any())
                      )
                    {
                        mode = "update";

                    }
                    sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,TAP,OIL_TR,OIL_TAP,OIL_LEVEL,WIND,INFV_TIME,EPIV_DESC,epiv_type,A,MVAR,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},{4},'{5}',{6},{7},'{8}',8,{9},{10},'{11}',{12} ) ",
                                         EPIU_EPIU_ID,
                                         EPVH_EPVH_ID,
                                         Request.Form["TAP_" + i.ToString()] == "" ? "null" : Request.Form["TAP_" + i.ToString()],
                                         Request.Form["OILTR_" + i.ToString()] == "" ? "null" : Request.Form["OILTR_" + i.ToString()],
                                         Request.Form["OILTAP_" + i.ToString()] == "" ? "null" : Request.Form["OILTAP_" + i.ToString()],
                                         Request.Form["OILLEVEL_" + i.ToString()] == "" ? "" : Request.Form["OILLEVEL_" + i.ToString()],
                                         Request.Form["WIND_" + i.ToString()] == "" ? "null" : Request.Form["WIND_" + i.ToString()],
                                         EXP_POST_VAR_INSTRU.INFV_TIME,
                                         Request.Form["EPIV_DESC_" + i.ToString()],
                                         Request.Form["KHA_" + i.ToString()] == "" ? "null" : Request.Form["KHA_" + i.ToString()],
                                         Request.Form["KHMVAR_" + i.ToString()] == "" ? "null" : Request.Form["KHMVAR_" + i.ToString()],
                                          VarDate, EPOL_EPOL_ID
                                         );

                    if (mode == "update")
                    {
                        sql = string.Format("update EXP_POST_VAR_INSTRU set  TAP={2},OIL_TR={3},OIL_TAP={4},OIL_LEVEL='{5}',WIND={6},A={8},MVAR={9},epiv_desc='{10}' where  EPIU_EPIU_ID in ({0}) and EPVH_EPVH_ID={1} and INFV_TIME='{7}' and epiv_type=8",
                                             string.Join(",", epiuquery),
                                             EPVH_EPVH_ID,
                                             Request.Form["TAP_" + i.ToString()] == "" ? "null" : Request.Form["TAP_" + i.ToString()],
                                             Request.Form["OILTR_" + i.ToString()] == "" ? "null" : Request.Form["OILTR_" + i.ToString()],
                                             Request.Form["OILTAP_" + i.ToString()] == "" ? "null" : Request.Form["OILTAP_" + i.ToString()],
                                             Request.Form["OILLEVEL_" + i.ToString()] == "" ? "" : Request.Form["OILLEVEL_" + i.ToString()],
                                             Request.Form["WIND_" + i.ToString()] == "" ? "null" : Request.Form["WIND_" + i.ToString()],
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             Request.Form["KHA_" + i.ToString()] == "" ? "null" : Request.Form["KHA_" + i.ToString()],
                                             Request.Form["KHMVAR_" + i.ToString()] == "" ? "null" : Request.Form["KHMVAR_" + i.ToString()],
                                             Request.Form["EPIV_DESC_" + i.ToString()]
                                             );
                    }

                    if (Request.Form["TAP_" + i.ToString()] != "" ||
                        Request.Form["OILTR_" + i.ToString()] != "" ||
                        Request.Form["OILTAP_" + i.ToString()] != "" ||
                        Request.Form["OILLEVEL_" + i.ToString()] != "" ||
                        Request.Form["WIND_" + i.ToString()] != "" ||
                        Request.Form["KHA_" + i.ToString()] != ""
                        )
                    {
                        try
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }
                        catch (Exception ex)
                        {
                            errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در خط " + EXP_POST_LINE.EPOL_NAME);
                        }
                    }
                    else if (EPIU_EPIU_ID != null)
                    {
                        DeleteRow(EPVH_EPVH_ID, "", 8, EXP_POST_VAR_INSTRU.INFV_TIME, (int)EPIU_EPIU_ID);
                    }

                    mode = "new";

                    try
                    {
                        if (EPIU_EPIU_ID != 0)
                        {
                            if (!string.IsNullOrEmpty(Request.Form["IMVAR_" + i.ToString()]))
                                EXP_POST_VAR_INSTRU.MVAR = decimal.Parse(Request.Form["IMVAR_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MVAR = 0;

                            if (!string.IsNullOrEmpty(Request.Form["IMW_" + i.ToString()]))
                                EXP_POST_VAR_INSTRU.MW = decimal.Parse(Request.Form["IMW_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MW = 0;

                            if (!string.IsNullOrEmpty(Request.Form["IA_" + i.ToString()]))
                                EXP_POST_VAR_INSTRU.A = decimal.Parse(Request.Form["IA_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.A = 0;

                            if (!string.IsNullOrEmpty(Request.Form["IKV_" + i.ToString()]))
                                EXP_POST_VAR_INSTRU.KV = decimal.Parse(Request.Form["IKV_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.KV = 0;

                            if (!string.IsNullOrEmpty(Request.Form["cos_" + i.ToString()]))
                                EXP_POST_VAR_INSTRU.COSINUS = decimal.Parse(Request.Form["cos_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.COSINUS = 0;


                            ///insert INCOMING
                            sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,MW,MVAR,INFV_TIME,A,KV,EPIV_DESC,EPIV_TYPE, COSINUS,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},{4},{5},{6},'{7}',6,{8},'{9}',{10} ) ",
                                                 EPIU_EPIU_ID,
                                                 EPVH_EPVH_ID,
                                                 Request.Form["IMW_" + i.ToString()] == "" ? "null" : Request.Form["IMW_" + i.ToString()],
                                                 Request.Form["IMVAR_" + i.ToString()] == "" ? "null" : Request.Form["IMVAR_" + i.ToString()],
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 Request.Form["IA_" + i.ToString()] == "" ? "null" : Request.Form["IA_" + i.ToString()],
                                                 Request.Form["IKV_" + i.ToString()] == "" ? "null" : Request.Form["IKV_" + i.ToString()],
                                                 "TRNAS INCOMING",
                                                 Request.Form["cos_" + i.ToString()] == "" ? "null" : Request.Form["cos_" + i.ToString()],
                                                 VarDate, EPOL_EPOL_ID
                                                 );

                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                       .Where(xx => xx.EPVH_EPVH_ID == EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 6)
                                                       .Select(xx => xx.EPIV_ID).Any())
                                )
                            {
                                sql = string.Format("update EXP_POST_VAR_INSTRU set   MW ={2} , MVAR={3},A={5},KV={6}, COSINUS={7} where INFV_TIME={4} and EPIU_EPIU_ID in ({0})  and EPVH_EPVH_ID= {1} and EPIV_TYPE=6  ",
                                                     string.Join(",", epiuquery),
                                                     EPVH_EPVH_ID,
                                                     Request.Form["IMW_" + i.ToString()] == "" ? "null" : Request.Form["IMW_" + i.ToString()],
                                                     Request.Form["IMVAR_" + i.ToString()] == "" ? "null" : Request.Form["IMVAR_" + i.ToString()],
                                                     EXP_POST_VAR_INSTRU.INFV_TIME,
                                                     Request.Form["IA_" + i.ToString()] == "" ? "null" : Request.Form["IA_" + i.ToString()],
                                                     Request.Form["IKV_" + i.ToString()] == "" ? "null" : Request.Form["IKV_" + i.ToString()],
                                                     Request.Form["cos_" + i.ToString()] == "" ? "null" : Request.Form["cos_" + i.ToString()]
                              );
                            }

                            if (//(!string.IsNullOrEmpty(Request.Form["IMVAR_" + i.ToString()])) ||
                                         (!string.IsNullOrEmpty(Request.Form["IMW_" + i.ToString()]))// ||
                                                                                                     // (!string.IsNullOrEmpty(Request.Form["IA_" + i.ToString()])) ||
                                                                                                     ///  (!string.IsNullOrEmpty(Request.Form["IKV_" + i.ToString()])) ||
                                //  (!string.IsNullOrEmpty(Request.Form["cos_" + i.ToString()]))
                                )
                            {
                                Db.Database.ExecuteSqlCommand(sql);
                            }
                            else if (EPIU_EPIU_ID != null)
                            {
                                DeleteRow(EPVH_EPVH_ID, "", 6, EXP_POST_VAR_INSTRU.INFV_TIME, (int)EPIU_EPIU_ID);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در TRNAS INCOMING ");
                    }
                }

                #endregion

                #region//insert into EXP_POST_LINE

                for (int j = 1; j < 11; j++)
                {
                    EXP_POST_LINE.EPOL_TYPE = "1";
                    EXP_POST_LINE.EPOL_NAME = Request.Form["Tl" + j];
                    if (EXP_POST_LINE.EPOL_NAME == "--")
                    {
                        Db.Database.ExecuteSqlCommand(string.Format("DELETE FROM EXP_POST_VAR_INSTRU WHERE EPVH_EPVH_ID={0} and LINE_COLUM={1} and EPIV_TYPE=0 and EPIU_EPIU_ID2={2}   ", EPVH_EPVH_ID, j, EPIU_EPIU_ID));
                    }

                    if (!string.IsNullOrEmpty(EXP_POST_LINE.EPOL_NAME) && EXP_POST_LINE.EPOL_NAME != "--")
                    {
                        try
                        {
                            var epiuquery = (from b in Db.EXP_POST_LINE_INSTRU
                                             where b.EPOL_EPOL_ID == Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault()
                                                   && b.CODE_DISP == Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_ID == EPIU_EPIU_ID).Select(xx => xx.CODE_DISP).FirstOrDefault()
                                             select b.EPIU_ID).ToArray();

                            //inser into EXP_POST_LINE_INSTRU
                            //مقدار دهی ردیف خط
                            EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = Db.Database.SqlQuery<int>(string.Format("select EPIU_ID  from EXP_POST_LINE_INSTRU,EXP_POST_LINE where upper(EXP_POST_LINE.EPOL_NAME)=upper('{0}')  and EXP_POST_LINE_INSTRU.EINS_EINS_ID=1 and EXP_POST_LINE_INSTRU.epol_epol_id=EXP_POST_LINE.EPOL_ID and EXP_POST_LINE.epol_type is not null ",
                                                                                                        EXP_POST_LINE.EPOL_NAME)).FirstOrDefault();

                            if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID == 0)
                            {
                                throw new Exception("line");
                            }

                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 0)
                                                       .Where(xx => xx.LINE_COLUM != j)
                                                       .Where(xx => xx.EPIU_EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID)
                                                       .Select(xx => xx.EPIV_ID).Any()))
                            {
                                throw new Exception("duplicate");
                            }

                            var post = from b in Db.EXP_POST_LINE where b.EPOL_TYPE == EXP_POST_LINE.EPOL_TYPE && b.EPOL_NAME == EXP_POST_LINE.EPOL_NAME select b;
                            if (!post.Any())
                            {
                                sql = string.Format("insert into EXP_POST_LINE (EPOL_TYPE,EPOL_NAME) values ({0},'{1}') ", EXP_POST_LINE.EPOL_TYPE, EXP_POST_LINE.EPOL_NAME);
                                //Db.Database.ExecuteSqlCommand(sql);
                            }

                            //inser into EXP_POST_LINE_INSTRU
                            EXP_POST_LINE_INSTRU.EPOL_EPOL_ID_LINE = Db.Database.SqlQuery<int>(string.Format("select EPOL_ID  from EXP_POST_LINE where EPOL_TYPE='1' and epol_name='{0}' ",
                                                                                                              EXP_POST_LINE.EPOL_NAME)).FirstOrDefault();


                            var instru = from b in Db.EXP_POST_LINE_INSTRU
                                         where b.EPOL_EPOL_ID_LINE == EXP_POST_LINE_INSTRU.EPOL_EPOL_ID_LINE && b.EPIU_EPIU_ID == EPIU_EPIU_ID
                                         select b;
                            if (!instru.Any())
                            {
                                Db.EXP_POST_LINE_INSTRU.Add(EXP_POST_LINE_INSTRU);
                                //Db.SaveChanges();
                            }

                            //insert into  EXP_POST_VAR_INSTRU 
                            if (!string.IsNullOrEmpty(Request.Form["A" + j.ToString() + "_" + i.ToString()]))
                                EXP_POST_VAR_INSTRU.A = decimal.Parse(Request.Form["A" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.A = 0;

                            EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = Db.Database.SqlQuery<int>(string.Format("select EPIU_ID  from EXP_POST_LINE_INSTRU,EXP_POST_LINE where upper(EXP_POST_LINE.EPOL_NAME)=upper('{0}') and EXP_POST_LINE_INSTRU.epol_epol_id=EXP_POST_LINE.EPOL_ID and EXP_POST_LINE.epol_type is not null ",
                                                                                                        EXP_POST_LINE.EPOL_NAME)).FirstOrDefault();

                            if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID == 0)
                            {
                                sql = string.Format("insert into EXP_POST_LINE_INSTRU (EPIU_EPIU_ID,EPOL_EPOL_ID_LINE) values ({0},{1}) ", EPIU_EPIU_ID, EXP_POST_LINE_INSTRU.EPOL_EPOL_ID_LINE);
                                // Db.Database.ExecuteSqlCommand(sql);

                                EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = Db.Database.SqlQuery<int>(string.Format("select EPIU_ID  from EXP_POST_LINE_INSTRU,EXP_POST_LINE where upper(EXP_POST_LINE_INSTRU.EPOL_NAME)=upper('{0}') and EXP_POST_LINE_INSTRU.epol_epol_id=EXP_POST_LINE.EPOL_ID and EXP_POST_LINE.epol_type is not null ",
                                                                                                            EXP_POST_LINE.EPOL_NAME)).FirstOrDefault();
                            }

                            string linemode = "new";
                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                       .Where(xx => xx.EPVH_EPVH_ID == EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 0)
                                                       .Where(xx => xx.LINE_COLUM == j)
                                                       .Where(xx => xx.EPIU_EPIU_ID2 == EPIU_EPIU_ID)
                                                       .Select(xx => xx.EPIV_ID).Any())
                                )
                            {
                                linemode = "update";
                            }

                            sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,A,INFV_TIME,LINE_COLUM,EPIV_DESC,EPIU_EPIU_ID2,epiv_type,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},{4},'{5}',{6},0,'{7}',{8} ) ",
                                                 EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                 EPVH_EPVH_ID,
                                                 Request.Form["A" + j.ToString() + "_" + i.ToString()] == "" ? "null" : Request.Form["A" + j.ToString() + "_" + i.ToString()],
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 j,
                                                 "TRANS", EPIU_EPIU_ID, VarDate, EPOL_EPOL_ID);

                            if (linemode == "update")
                            {
                                sql = string.Format("update EXP_POST_VAR_INSTRU set A={2} , EPIU_EPIU_ID={0}   where epiv_type=0 and   EPVH_EPVH_ID={1} and INFV_TIME='{3}' and EPIU_EPIU_ID2 in ({4}) and LINE_COLUM={5}   ",
                                                     EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                     EPVH_EPVH_ID,
                                                     Request.Form["A" + j.ToString() + "_" + i.ToString()] == "" ? "null" : Request.Form["A" + j.ToString() + "_" + i.ToString()],
                                                     EXP_POST_VAR_INSTRU.INFV_TIME,
                                                     string.Join(",", epiuquery),
                                                     j);
                            }

                            if (Request.Form["A" + j.ToString() + "_" + i.ToString()] != "")
                                Db.Database.ExecuteSqlCommand(sql);
                            else if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID != null)
                                DeleteRow(EPVH_EPVH_ID, "", 0, EXP_POST_VAR_INSTRU.INFV_TIME, (int)EXP_POST_VAR_INSTRU.EPIU_EPIU_ID);

                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "duplicate" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE.EPOL_NAME + " تکراری میباشد ");
                            }
                            else if (ex.Message == "line" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE.EPOL_NAME + " اشتباه است لطفا با مرکز پیام هماهنگ کنید ");
                            }
                            else if (ex.Message != "line" && ex.Message != "duplicate")
                            {
                                //errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در خط خروجی تب ترانس " + EXP_POST_LINE_INSTRU.CODE_DISP);
                                errors.Add(ex.Message + "<br />");
                            }
                        }
                    }//end if
                }

                #endregion

            }

            if (errors.Count == 0)
            {
                return Json(new { Success = "اطلاعات با موفقیت ثبت شد" }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                string errorMessages = string.Join("-", errors.ToArray());
                return Json(new { Fail = errorMessages }, JsonRequestBehavior.DenyGet);
            }
        }

        public ActionResult update_dist(EXP_POST_VAR_HEAD objecttemp)
        {
            List<string> errors = new List<string>();
            int EPVH_EPVH_ID = 0;
            int? EPOL_EPOL_ID = 0;
            string VarDate = "";
            if (!string.IsNullOrEmpty(Request.Form["EPVH_EPVH_ID"]))
            {
                // EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                EPVH_EPVH_ID = int.Parse(Request.Form["EPVH_EPVH_ID"]);
                EPOL_EPOL_ID = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault();
                VarDate = Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.VAR_DATE).FirstOrDefault();

            }

            string mode = "new", sql = string.Empty;
            //insert into EXP_POST_VAR_HEAD
            var q = from b in Db.EXP_POST_VAR_HEAD
                    where b.VAR_DATE == objecttemp.VAR_DATE && b.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID && b.EPVH_TYPE == 3
                    select new { b.EPVH_ID };

            if (!q.Any())
            {
                sql = string.Format("insert into EXP_POST_VAR_HEAD (VAR_DATE,EPOL_EPOL_ID,EPVH_TYPE,EPVT_EPVT_ID) values ('{0}','{1}',3,1) ", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID);
                Db.Database.ExecuteSqlCommand(sql);
            }

            var EXP_POST_VAR_INSTRU = new EXP_POST_VAR_INSTRU();
            var EXP_POST_LINE = new EXP_POST_LINE();
            var EXP_POST_LINE_INSTRU = new EXP_POST_LINE_INSTRU();
            EXP_POST_VAR_INSTRU.EPVH_EPVH_ID = EPVH_EPVH_ID;
            sql = string.Format("update EXP_POST_VAR_HEAD set epvh_desc='{0}' where epvh_id={1}", objecttemp.EPVH_DESC, EXP_POST_VAR_INSTRU.EPVH_EPVH_ID);
            Db.Database.ExecuteSqlCommand(sql);

            for (int i = 1; i < 25; i++)
            {
                mode = "new";
                EXP_POST_VAR_INSTRU.INFV_TIME = Request.Form["INFV_TIME"] + i.ToString();

                #region//insert into EXP_POST_LINE

                for (int j = 1; j < 9; j++)
                {
                    EXP_POST_LINE.EPOL_TYPE = "1";
                    EXP_POST_LINE_INSTRU.CODE_DISP = Request.Form["l" + j];
                    if (EXP_POST_LINE_INSTRU.CODE_DISP == "--")
                    {
                        string sqldelete = string.Format("DELETE FROM EXP_POST_VAR_INSTRU WHERE EPVH_EPVH_ID={0} and LINE_COLUM={1} and EPIV_TYPE=1", EXP_POST_VAR_INSTRU.EPVH_EPVH_ID, j);
                        Db.Database.ExecuteSqlCommand(sqldelete);
                    }

                    if (!string.IsNullOrEmpty(EXP_POST_LINE_INSTRU.CODE_DISP) && EXP_POST_LINE_INSTRU.CODE_DISP != "--")
                    {
                        try
                        {
                            string mvv = Request.Form["MVAR" + j.ToString() + i.ToString()];
                            if (Request.Form["MVAR" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.MVAR = decimal.Parse(Request.Form["MVAR" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MVAR = 0;

                            if (Request.Form["MW" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.MW = decimal.Parse(Request.Form["MW" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MW = 0;

                            if (Request.Form["KV" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.KV = decimal.Parse(Request.Form["KV" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.KV = 0;

                            EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = Db.Database.SqlQuery<int>(string.Format("select EPIU_ID  from EXP_POST_LINE_INSTRU,EXP_POST_LINE where EXP_POST_LINE_INSTRU.EINS_EINS_ID in (1,2) and upper(EXP_POST_LINE_INSTRU.CODE_DISP)=upper('{0}') and EXP_POST_LINE_INSTRU.epol_epol_id=EXP_POST_LINE.EPOL_ID and EXP_POST_LINE.epol_type is not null ",
                                                                                                        EXP_POST_LINE_INSTRU.CODE_DISP)).FirstOrDefault();

                            if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID == 0)
                            {
                                throw new Exception("line");
                            }

                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 1)
                                                       .Where(xx => xx.LINE_COLUM != j)
                                                       .Where(xx => xx.EPIU_EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID)
                                                       .Select(xx => xx.EPIV_ID).Any()))
                            {
                                throw new Exception("duplicate");
                            }

                            string linemode = "new";
                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                       .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 1)
                                                       .Where(xx => xx.LINE_COLUM == j)
                                                       .Select(xx => xx.EPIV_ID).Any())
                                )
                            {
                                linemode = "update";
                            }

                            sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,MW,MVAR,INFV_TIME,LINE_COLUM,EPIV_DESC,KV,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},{4},{5},'{6}',{7},1,'{8}',{9}) ",
                                                 EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["MW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                 Request.Form["MVAR" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 j,
                                                 "LINE",
                                                 Request.Form["KV" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.KV.ToString(),
                                                 VarDate,
                                                 EPOL_EPOL_ID
                                                 );

                            if (linemode == "update")
                            {
                                sql = string.Format("update EXP_POST_VAR_INSTRU set MW={2},MVAR={3},KV={5},EPIU_EPIU_ID={0}   where    EPVH_EPVH_ID={1} and INFV_TIME='{4}' and LINE_COLUM={6} and EPIV_TYPE=1  ",
                                                     EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                     EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                     Request.Form["MW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                     Request.Form["MVAR" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                                     EXP_POST_VAR_INSTRU.INFV_TIME,
                                                     Request.Form["KV" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.KV.ToString(),
                                                     j);
                            }

                            if ((Request.Form["MVAR" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["MW" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["KV" + j.ToString() + "_" + i.ToString()] != "") || linemode == "update")
                            {
                                Db.Database.ExecuteSqlCommand(sql);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "duplicate" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " تکراری میباشد ");
                            }
                            else if (ex.Message == "line" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " اشتباه است لطفا با مرکز پیام هماهنگ کنید ");
                            }
                            else if (ex.Message != "line" && ex.Message != "duplicate")
                            {
                                errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در خط " + EXP_POST_LINE.CODE_DISP);
                            }
                        }
                    }
                }//end for

                #endregion

                mode = "new";

                if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                           .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                           .Where(xx => xx.EPIV_TYPE == 7)
                                           .Select(xx => xx.EPIV_ID).Any())
                    )
                {
                    mode = "update";
                }

                sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,INFV_TIME,TEMP,HUMI,AUX_A,AUX_V,AUX_TG,EPIV_DESC,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},{4},{5},'{6}','{7}',7,'{8}',{9}) ",
                                     EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                     EXP_POST_VAR_INSTRU.INFV_TIME,
                                     Request.Form["TEMP_" + i.ToString()] == "" ? "0" : Request.Form["TEMP_" + i.ToString()],
                                     Request.Form["HUMI_" + i.ToString()] == "" ? "0" : Request.Form["HUMI_" + i.ToString()],
                                     Request.Form["AUXA_" + i.ToString()] == "" ? "0" : Request.Form["AUXA_" + i.ToString()],
                                     Request.Form["AUXV_" + i.ToString()] == "" ? "0" : Request.Form["AUXV_" + i.ToString()],
                                     Request.Form["AUXTG_" + i.ToString()] == "" ? "T1" : Request.Form["AUXTG_" + i.ToString()],
                                     "AUX,TEMP,HUMI",
                                     VarDate,
                                     EPOL_EPOL_ID
                                     );

                if (mode == "update")
                {
                    sql = string.Format("update EXP_POST_VAR_INSTRU set   TEMP ={2} ,HUMI={3} ,AUX_A={4} ,AUX_V={5} ,AUX_TG='{6}'  where INFV_TIME={1}  and EPVH_EPVH_ID= {0} and temp is not null and humi is not null ",
                                         EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                         EXP_POST_VAR_INSTRU.INFV_TIME,
                                         Request.Form["TEMP_" + i.ToString()] == "" ? "0" : Request.Form["TEMP_" + i.ToString()],
                                         Request.Form["HUMI_" + i.ToString()] == "" ? "0" : Request.Form["HUMI_" + i.ToString()],
                                         Request.Form["AUXA_" + i.ToString()] == "" ? "0" : Request.Form["AUXA_" + i.ToString()],
                                         Request.Form["AUXV_" + i.ToString()] == "" ? "0" : Request.Form["AUXV_" + i.ToString()],
                                         Request.Form["AUXTG_" + i.ToString()] == "" ? "T1" : Request.Form["AUXTG_" + i.ToString()]
                                         );
                }

                if (Request.Form["TEMP_" + i.ToString()] != "" ||
                    Request.Form["HUMI_" + i.ToString()] != "" ||
                    Request.Form["AUXA_" + i.ToString()] != "" ||
                    Request.Form["AUXV_" + i.ToString()] != ""
                    )
                {
                    try
                    {
                        Db.Database.ExecuteSqlCommand(sql);
                    }
                    catch (Exception ex)
                    {
                        errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در AUX,TEMP,HUMI ");
                    }
                }

                for (int j = 1; j < 3; j++)
                {
                    try
                    {
                        string mode_48 = "", mode_110 = "";

                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,A,V,INFV_TIME,EPIV_DESC,LINE_COLUM,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},'{4}',{5},3,'{6}',{7}) ",
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             Request.Form["48A" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["48A" + j.ToString() + "_" + i.ToString()],
                                             Request.Form["48V" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["48V" + j.ToString() + "_" + i.ToString()],
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             "L.V_DC_48",
                                             j,
                                             VarDate,
                                             EPOL_EPOL_ID);

                        if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                   .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                   .Where(xx => xx.LINE_COLUM == j)
                                                   .Where(xx => xx.EPIV_TYPE == 3)
                                                   .Select(xx => xx.EPIV_ID).Any())
                             )
                        {
                            mode_48 = "update";
                        }

                        if (mode_48 == "update")
                        {
                            sql = string.Format("update  EXP_POST_VAR_INSTRU set A={1} ,V={2} WHERE   EPVH_EPVH_ID={0} AND INFV_TIME='{3}' AND EPIV_DESC='{4}' AND LINE_COLUM= {5} ",
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["48A" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["48A" + j.ToString() + "_" + i.ToString()],
                                                 Request.Form["48V" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["48V" + j.ToString() + "_" + i.ToString()],
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 "L.V_DC_48",
                                                 j);
                        }

                        if (Request.Form["48A" + j.ToString() + "_" + i.ToString()] != "" || Request.Form["48V" + j.ToString() + "_" + i.ToString()] != "")
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }

                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,A,V,INFV_TIME,EPIV_DESC,LINE_COLUM,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},'{4}',{5},4,'{6}',{7}) ",
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             Request.Form["110A" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["110A" + j.ToString() + "_" + i.ToString()],
                                             Request.Form["110V" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["110V" + j.ToString() + "_" + i.ToString()],
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             "L.V_DC_110",
                                             j, VarDate,
                                                 EPOL_EPOL_ID);

                        if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                   .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                   .Where(xx => xx.LINE_COLUM == j)
                                                   .Where(xx => xx.EPIV_TYPE == 4)
                                                   .Select(xx => xx.EPIV_ID).Any())
                             )
                        {
                            mode_110 = "update";
                        }

                        if (mode_110 == "update")
                        {
                            sql = string.Format("update  EXP_POST_VAR_INSTRU set A={1} ,V={2} WHERE   EPVH_EPVH_ID={0} AND INFV_TIME='{3}' AND EPIV_DESC='{4}' AND LINE_COLUM= {5} ",
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["110A" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["110A" + j.ToString() + "_" + i.ToString()],
                                                 Request.Form["110V" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["110V" + j.ToString() + "_" + i.ToString()],
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 "L.V_DC_110",
                                                 j);
                        }

                        if (Request.Form["110A" + j.ToString() + "_" + i.ToString()] != "" || Request.Form["110V" + j.ToString() + "_" + i.ToString()] != "")
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در تب L.V DC ");
                    }
                }//end for
            }//for i

            if (errors.Count == 0)
            {
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            else
            {
                string errorMessages = string.Join("<br />", errors.ToArray());
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errorMessages }.ToJson();
            }
        }

        public ActionResult insert_pow(EXP_POST_VAR_HEAD objecttemp)
        {
            string /*mode = "new",*/ sql = string.Empty;


            #region//insert into EXP_POST_VAR_HEAD

            //objecttemp.EPVT_EPVT_ID = Db.exp_type;
            var q = from b in Db.EXP_POST_VAR_HEAD
                    where b.VAR_DATE == objecttemp.VAR_DATE && b.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID && b.EPVH_TYPE == 2
                    select new { b.EPVH_ID };

            if (!q.Any())
            {
                Db.Database.ExecuteSqlCommand(string.Format("insert into EXP_POST_VAR_HEAD (VAR_DATE,EPOL_EPOL_ID,epvh_type,EPVT_EPVT_ID,EPVH_DESC) values ('{0}','{1}',2,{2},'{3}') ", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID, 1, objecttemp.EPVH_DESC));
            }

            var EXP_POST_VAR_INSTRU = new EXP_POST_VAR_INSTRU();
            var EXP_POST_LINE = new EXP_POST_LINE();
            var EXP_POST_LINE_INSTRU = new EXP_POST_LINE_INSTRU();

            EXP_POST_VAR_INSTRU.EPVH_EPVH_ID = Db.Database.SqlQuery<int>(string.Format("select EPVH_ID from EXP_POST_VAR_HEAD where VAR_DATE='{0}' and EPOL_EPOL_ID={1} and epvh_type=2", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID)).FirstOrDefault();

            Db.Database.ExecuteSqlCommand(string.Format("update EXP_POST_VAR_HEAD set epvh_desc='{0}' where epvh_id={1}", objecttemp.EPVH_DESC, EXP_POST_VAR_INSTRU.EPVH_EPVH_ID));

            #endregion

            List<string> errors = new List<string>();
            int EPIU_EPIU_ID = 0, EPIU_EPIU_ID2 = 0;

            if (!string.IsNullOrEmpty(Request.Form["EPIU_EPIU_ID"]))
            {
                // EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
            }

            decimal epiu_id = Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID).Where(xx => xx.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID).Select(xx => xx.EPIU_ID).FirstOrDefault();

            if (!string.IsNullOrEmpty(Request.Form["EPIU_EPIU_ID2"]))
            {
                //EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID2"]);
                EPIU_EPIU_ID2 = int.Parse(Request.Form["EPIU_EPIU_ID2"]);
            }

            decimal reactor_epiu_id = EPIU_EPIU_ID2;

            for (int i = 1; i < 25; i++)
            {
                EXP_POST_VAR_INSTRU.INFV_TIME = Request.Form["INFV_TIME"] + i.ToString();

                #region//////////////////////////////////////////////InSERT UNIT

                for (int j = 1; j < 9; j++)
                {
                    EXP_POST_LINE.EPOL_TYPE = "2";
                    EXP_POST_LINE_INSTRU.CODE_DISP = Request.Form["U" + j];
                    if (EXP_POST_LINE_INSTRU.CODE_DISP == "")
                    {
                        Db.Database.ExecuteSqlCommand(string.Format("DELETE FROM EXP_POST_VAR_INSTRU WHERE EPVH_EPVH_ID={0} and LINE_COLUM={1} and EPIV_TYPE=9", EXP_POST_VAR_INSTRU.EPVH_EPVH_ID, j));
                    }

                    if (!string.IsNullOrEmpty(EXP_POST_LINE_INSTRU.CODE_DISP) && EXP_POST_LINE_INSTRU.CODE_DISP != "--")
                    {
                        try
                        {
                            if (Request.Form["uMVAR" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.MVAR = decimal.Parse(Request.Form["uMVAR" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MVAR = 0;

                            if (Request.Form["uMW" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.MW = decimal.Parse(Request.Form["uMW" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MW = 0;

                            if (Request.Form["uKV" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.KV = decimal.Parse(Request.Form["uKV" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.KV = 0;

                            EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = Db.Database.SqlQuery<int>(string.Format("select EPIU_ID  from EXP_POST_LINE_INSTRU,EXP_POST_LINE where upper(EXP_POST_LINE_INSTRU.EPIU_ID)=upper('{0}') and EXP_POST_LINE_INSTRU.epol_epol_id=EXP_POST_LINE.EPOL_ID and EXP_POST_LINE.epol_type is not null ",
                                                                                                        EXP_POST_LINE_INSTRU.CODE_DISP, objecttemp.EPOL_EPOL_ID)).FirstOrDefault();

                            if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID == 0)
                            {
                                throw new Exception("line");
                            }

                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 9)
                                                       .Where(xx => xx.LINE_COLUM != j)
                                                       .Where(xx => xx.EPIU_EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID)
                                                       .Select(xx => xx.EPIV_ID).Any()))
                            {
                                throw new Exception("duplicate");
                            }

                            string linemode = "new";
                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                       .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 9)
                                                       .Where(xx => xx.LINE_COLUM == j)
                                                       .Select(xx => xx.EPIV_ID).Any())
                               )
                            {
                                linemode = "update";
                            }

                            sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,MW,MVAR,INFV_TIME,LINE_COLUM,EPIV_DESC,KV,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},{4},{5},'{6}',{7},9,'{8}',9) ",
                                                 EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["uMW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                 Request.Form["uMVAR" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 j,
                                                 "UNIT",
                                                 Request.Form["uKV" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.KV.ToString(),
                                                 objecttemp.EPOL_EPOL_ID, objecttemp.VAR_DATE
                                                 );

                            if (linemode == "update")
                            {
                                sql = string.Format("update EXP_POST_VAR_INSTRU set MW={2},MVAR={3},KV={5},EPIU_EPIU_ID={0}   where    EPVH_EPVH_ID={1} and INFV_TIME='{4}' and LINE_COLUM={6} and EPIV_TYPE=9  ",
                                                     EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                     EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                     Request.Form["uMW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                     Request.Form["uMVAR" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                                     EXP_POST_VAR_INSTRU.INFV_TIME,
                                                     Request.Form["uKV" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.KV.ToString(),
                                                     j);
                            }

                            if ((Request.Form["uMVAR" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["uMW" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["uMW" + j.ToString() + "_" + i.ToString()] != ""))
                            {
                                Db.Database.ExecuteSqlCommand(sql);
                            }
                            else if (EPIU_EPIU_ID != null)
                            {
                                DeleteRow(EXP_POST_VAR_INSTRU.EPVH_EPVH_ID, "", 9, EXP_POST_VAR_INSTRU.INFV_TIME, (int)EXP_POST_VAR_INSTRU.EPIU_EPIU_ID);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "duplicate" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " تکراری میباشد ");
                            }
                            else if (ex.Message == "line" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " اشتباه است لطفا با مرکز پیام هماهنگ کنید ");
                            }
                            else if (ex.Message != "line" && ex.Message != "duplicate")
                            {
                                errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در Unit " + EXP_POST_LINE_INSTRU.CODE_DISP);
                            }
                        }
                    }
                }

                #endregion

            }


            if (errors.Count == 0)
            {
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            else
            {
                string errorMessages = string.Join("<br />", errors.ToArray());
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errorMessages }.ToJson();
            }
        }

        public ActionResult DeleteRow(int? EPVH_EPVH_ID, string LINE_COLUM, int EPIV_TYPE, string INFV_TIME, int EPIU_EPIU_ID)
        {
            Db.Database.ExecuteSqlCommand(string.Format("DELETE FROM EXP_POST_VAR_INSTRU WHERE  EPVH_EPVH_ID={0}  and EPIV_TYPE={1} and INFV_TIME={2} and EPIU_EPIU_ID={3}", EPVH_EPVH_ID, EPIV_TYPE, INFV_TIME, EPIU_EPIU_ID));
            return null;
        }
        public ActionResult DeleteMonthlyData(int EPIU_ID, int? EPVH_ID, string EPIV_TYPE, string Column, string Phase)
        {
            try
            {
                string Sql = "";
                if (string.IsNullOrEmpty(Column))
                {
                    Sql = string.Format("DELETE FROM EXP_POST_VAR_INSTRU WHERE  EPVH_EPVH_ID={0}  and EPIV_TYPE in ({1}) and EPIU_EPIU_ID={2} ", EPVH_ID, EPIV_TYPE, EPIU_ID);
                }
                else
                {
                    Sql = string.Format("DELETE FROM EXP_POST_VAR_INSTRU WHERE  EPVH_EPVH_ID={0}  and EPIV_TYPE in ({1}) and EPIU_EPIU_ID={2} and LINE_COLUM='{3}' and EPIV_PHAS='{4}'", EPVH_ID, EPIV_TYPE, EPIU_ID, Column, Phase);

                }
                Db.Database.ExecuteSqlCommand(Sql);
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت حذف شد") }.ToJson();
            }
            catch (Exception Ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در حذف اطلاعات" }.ToJson();


            }
        }

        public ActionResult insert_exp_transfer(EXP_POST_VAR_HEAD objecttemp)
        {
            string sql = string.Empty;
            var q = from b in Db.EXP_POST_VAR_HEAD
                    where b.VAR_DATE == objecttemp.VAR_DATE && b.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID && b.EPVH_TYPE == objecttemp.EPVH_TYPE
                    select new { b.EPVH_ID };

            if (!q.Any())
            {
                Db.Database.ExecuteSqlCommand(string.Format("insert into EXP_POST_VAR_HEAD (VAR_DATE,EPOL_EPOL_ID,epvh_type,EPVT_EPVT_ID,EPVH_DESC) values ('{0}','{1}',{4},{2},'{3}') ", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID, 1, objecttemp.EPVH_DESC, objecttemp.EPVH_TYPE));
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            else
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری است" }.ToJson();
            }
        }
        public ActionResult insert_exp_internal_var(EXP_POST_VAR_INSTRU objecttemp)
        {

            int EPVH_EPVH_ID = int.Parse(Request.Form["EPVH_EPVH_ID"]);
            string sql = string.Empty;
            var q = from b in Db.EXP_POST_VAR_INSTRU
                    where b.EPVH_EPVH_ID == EPVH_EPVH_ID && b.EPIV_TYPE == 14
                    select new { b.EPIV_ID };

            if (!q.Any())
            {
                string Sql = string.Format("insert into EXP_POST_VAR_INSTRU (INFV_DATE,INFV_TIME,EPOL_EPOL_ID,EPIV_TYPE,EPVH_EPVH_ID,EPIV_DESC,MW,MVAR) values ('{0}',24,'{1}',14,{2},'بار داخلی',{3},{4}) ", Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.VAR_DATE).FirstOrDefault()
                    , Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), EPVH_EPVH_ID, objecttemp.MW, objecttemp.MVAR);
                Db.Database.ExecuteSqlCommand(Sql);

            }
            else
            {
                string Sql = string.Format("delete from EXP_POST_VAR_INSTRU where epiv_type=14 and EPVH_EPVH_ID={0} ", EPVH_EPVH_ID);
                Db.Database.ExecuteSqlCommand(Sql);
                Sql = string.Format("insert into EXP_POST_VAR_INSTRU (INFV_DATE,INFV_TIME,EPOL_EPOL_ID,EPIV_TYPE,EPVH_EPVH_ID,EPIV_DESC,MW,MVAR) values ('{0}',24,'{1}',14,{2},'بار داخلی',{3},{4}) ", Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.VAR_DATE).FirstOrDefault()
                    , Db.EXP_POST_VAR_HEAD.Where(xx => xx.EPVH_ID == EPVH_EPVH_ID).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault(), EPVH_EPVH_ID, objecttemp.MW, objecttemp.MVAR);
                Db.Database.ExecuteSqlCommand(Sql);
            }
            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
        }
        public ActionResult insert_transfer(EXP_POST_VAR_HEAD objecttemp)
        {
            List<string> errors = new List<string>();
            string mode = "new", sql = string.Empty;
            //insert into EXP_POST_VAR_HEAD
            //objecttemp.EPVT_EPVT_ID = Db.exp_type;
            var q = from b in Db.EXP_POST_VAR_HEAD
                    where b.VAR_DATE == objecttemp.VAR_DATE && b.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID && b.EPVH_TYPE == 1
                    select new { b.EPVH_ID };

            if (!q.Any())
            {
                Db.Database.ExecuteSqlCommand(string.Format("insert into EXP_POST_VAR_HEAD (VAR_DATE,EPOL_EPOL_ID,epvh_type,EPVT_EPVT_ID,EPVH_DESC) values ('{0}','{1}',1,{2},'{3}') ", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID, 1, objecttemp.EPVH_DESC));
            }

            var EXP_POST_VAR_INSTRU = new EXP_POST_VAR_INSTRU();
            var EXP_POST_LINE = new EXP_POST_LINE();
            var EXP_POST_LINE_INSTRU = new EXP_POST_LINE_INSTRU();

            EXP_POST_VAR_INSTRU.EPVH_EPVH_ID = Db.Database.SqlQuery<int>(string.Format("select EPVH_ID from EXP_POST_VAR_HEAD where VAR_DATE='{0}' and EPOL_EPOL_ID={1} and epvh_type=1", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID)).FirstOrDefault();

            int EPIU_EPIU_ID = 0, EPIU_EPIU_ID2 = 0;

            if (!string.IsNullOrEmpty(Request.Form["EPIU_EPIU_ID"]))
            {
                //EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
            }

            decimal epiu_id = Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID).Where(xx => xx.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID).Select(xx => xx.EPIU_ID).FirstOrDefault();

            if (!string.IsNullOrEmpty(Request.Form["EPIU_EPIU_ID2"]))
            {
                //EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID2"]);
                EPIU_EPIU_ID2 = int.Parse(Request.Form["EPIU_EPIU_ID2"]);
            }

            decimal reactor_epiu_id = EPIU_EPIU_ID2;

            for (int i = 1; i < 25; i++)
            {

                #region /////////////////////INSERT TRANS LINE

                mode = "new";
                EXP_POST_VAR_INSTRU.INFV_TIME = Request.Form["INFV_TIME"] + i.ToString();

                if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                           .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                           .Where(xx => xx.EPIU_EPIU_ID == epiu_id)
                                           .Select(xx => xx.EPIV_ID).Any())
                    )
                {
                    mode = "update";
                }

                for (int j = 1; j < 9; j++)
                {
                    EXP_POST_LINE.EPOL_TYPE = "1";
                    EXP_POST_LINE_INSTRU.CODE_DISP = Request.Form["tl" + j];
                    if (EXP_POST_LINE_INSTRU.CODE_DISP == "--")
                    {
                        //string sqldelete = string.Format("DELETE FROM EXP_POST_VAR_INSTRU WHERE EPVH_EPVH_ID={0} and LINE_COLUM={1} and EPIV_TYPE=0", EXP_POST_VAR_INSTRU.EPVH_EPVH_ID, j);
                        Db.Database.ExecuteSqlCommand(string.Format("DELETE FROM EXP_POST_VAR_INSTRU WHERE EPVH_EPVH_ID={0} and LINE_COLUM={1} and EPIV_TYPE=0 and EPIU_EPIU_ID2={2}   ", EXP_POST_VAR_INSTRU.EPVH_EPVH_ID, j, EPIU_EPIU_ID));
                    }

                    if (!string.IsNullOrEmpty(EXP_POST_LINE_INSTRU.CODE_DISP) && EPIU_EPIU_ID != 0 && EXP_POST_LINE_INSTRU.CODE_DISP != "--")
                    {
                        //insert into EXP_POST_LINE
                        try
                        {
                            if (!string.IsNullOrEmpty(Request.Form["TMVAR" + j.ToString() + "_" + i.ToString()]))
                                EXP_POST_VAR_INSTRU.MVAR = decimal.Parse(Request.Form["TMVAR" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MVAR = 0;

                            if (!string.IsNullOrEmpty(Request.Form["TMW" + j.ToString() + "_" + i.ToString()]))
                                EXP_POST_VAR_INSTRU.MW = decimal.Parse(Request.Form["TMW" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MW = 0;

                            EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = Db.Database.SqlQuery<int>(string.Format("select EPIU_ID  from EXP_POST_LINE_INSTRU,EXP_POST_LINE where EXP_POST_LINE_INSTRU.EINS_EINS_ID in (1,2) and upper(EXP_POST_LINE_INSTRU.CODE_DISP)=upper('{0}') and EXP_POST_LINE_INSTRU.epol_epol_id=EXP_POST_LINE.EPOL_ID and EXP_POST_LINE.epol_type is not null ",
                                                                                                        EXP_POST_LINE_INSTRU.CODE_DISP)).FirstOrDefault();

                            if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID == 0)
                            {
                                throw new Exception("line");
                            }

                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 0)
                                                       .Where(xx => xx.LINE_COLUM != j)
                                                       .Where(xx => xx.EPIU_EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID)
                                                       .Select(xx => xx.EPIV_ID).Any()))
                            {
                                throw new Exception("duplicate");
                            }

                            string linemode = "new";
                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                       .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 0)
                                                       .Where(xx => xx.LINE_COLUM == j)
                                                       .Where(xx => xx.EPIU_EPIU_ID2 == EPIU_EPIU_ID)
                                                       .Select(xx => xx.EPIV_ID).Any())
                               )
                            {
                                linemode = "update";
                            }

                            sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,MW,MVAR,INFV_TIME,LINE_COLUM,EPIV_DESC,EPIU_EPIU_ID2,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},{4},{5},'{6}',{7},0,'{8}',9) ",
                                                 EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["TMW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                 Request.Form["TMVAR" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 j,
                                                 "TRANS",
                                                 EPIU_EPIU_ID, objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID);

                            if (linemode == "update")
                            {
                                sql = string.Format("update EXP_POST_VAR_INSTRU set MW={2},MVAR={3},EPIU_EPIU_ID={7}  where  EPIV_TYPE={0}  and EPVH_EPVH_ID={1} and INFV_TIME='{4}' and EPIU_EPIU_ID2={5} and LINE_COLUM={6}  ",
                                                     0,
                                                     EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                     Request.Form["TMW" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                     Request.Form["TMVAR" + j.ToString() + "_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                                     EXP_POST_VAR_INSTRU.INFV_TIME,
                                                     EPIU_EPIU_ID,
                                                     j,
                                                     EXP_POST_VAR_INSTRU.EPIU_EPIU_ID
                                                     );
                            }

                            //INSERT TRANS LINE
                            if ((Request.Form["TMVAR" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["TMW" + j.ToString() + "_" + i.ToString()] != ""))
                            {
                                Db.Database.ExecuteSqlCommand(sql);
                            }
                            else if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID != null)
                            {
                                DeleteRow(EXP_POST_VAR_INSTRU.EPVH_EPVH_ID, j.ToString(), 0, EXP_POST_VAR_INSTRU.INFV_TIME, EPIU_EPIU_ID);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "duplicate" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " تکراری میباشد ");
                            }
                            else if (ex.Message == "line" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " اشتباه است لطفا با مرکز پیام هماهنگ کنید ");
                            }
                            else if (ex.Message != "line" && ex.Message != "duplicate")
                            {
                                errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "   در خط ترانس " + EXP_POST_LINE_INSTRU.CODE_DISP);
                            }
                        }
                    }//end if
                }

                #endregion

                #region///////////////// insert incoming
                try
                {
                    if (EPIU_EPIU_ID != 0)
                    {
                        if (!string.IsNullOrEmpty(Request.Form["TIMVAR_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.MVAR = decimal.Parse(Request.Form["TIMVAR_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.MVAR = 0;

                        if (!string.IsNullOrEmpty(Request.Form["TIMW_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.MW = decimal.Parse(Request.Form["TIMW_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.MW = 0;

                        if (!string.IsNullOrEmpty(Request.Form["TIA_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.A = decimal.Parse(Request.Form["TIA_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.A = 0;

                        if (!string.IsNullOrEmpty(Request.Form["TIKV_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.KV = decimal.Parse(Request.Form["TIKV_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.KV = 0;

                        if (!string.IsNullOrEmpty(Request.Form["TOIL_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.OIL = int.Parse(Request.Form["TOIL_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.OIL = 0;

                        if (!string.IsNullOrEmpty(Request.Form["TTHR_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.THR = int.Parse(Request.Form["TTHR_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.THR = 0;

                        if (!string.IsNullOrEmpty(Request.Form["TSEC_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.SEC = int.Parse(Request.Form["TSEC_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.SEC = 0;

                        if (!string.IsNullOrEmpty(Request.Form["TPRI_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.PRI = int.Parse(Request.Form["TPRI_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.PRI = 0;

                        if (!string.IsNullOrEmpty(Request.Form["TPOS_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.TAP = int.Parse(Request.Form["TPOS_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.TAP = 0;

                        mode = "new";
                        if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                   .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                   .Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID)
                                                   .Where(xx => xx.EPIV_TYPE == 6)
                                                   .Select(xx => xx.EPIV_ID).Any())
                            )
                        {
                            mode = "update";
                        }

                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,MW,MVAR,INFV_TIME,A,KV,OKV,EPIV_DESC,oil,thr,sec,pri,tap,EPIV_TYPE) values ({0},{1},{2},{3},{4},{5},{6},{7},'{8}',{9},{10},{11},{12},{13},6) ",
                                             EPIU_EPIU_ID,
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             Request.Form["TIMW_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                             Request.Form["TIMVAR_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             Request.Form["TIA_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.A.ToString(),
                                             Request.Form["TIKV_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.KV.ToString(),
                                             Request.Form["OKV"] == "" ? "0" : Request.Form["OKV"],
                                             "TRNAS INCOMING",
                                             Request.Form["TOIL_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.OIL.ToString(),
                                             Request.Form["TTHR_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.THR.ToString(),
                                             Request.Form["TSEC_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.SEC.ToString(),
                                             Request.Form["TPRI_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.PRI.ToString(),
                                             Request.Form["TPOS_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.TAP.ToString()
                                             );

                        if (mode == "update")
                        {
                            sql = string.Format("update EXP_POST_VAR_INSTRU set   MW ={2} , MVAR={3},A={5},KV={6},OKV={7},oil={8},thr={9},sec={10},pri={11},tap={12} where INFV_TIME={4} and EPIU_EPIU_ID={0}  and EPVH_EPVH_ID= {1}  and EPIV_TYPE=6 ",
                                                 EPIU_EPIU_ID,
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["TIMW_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MW.ToString(),
                                                 Request.Form["TIMVAR_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.MVAR.ToString(),
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 Request.Form["TIA_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.A.ToString(),
                                                 Request.Form["TIKV_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.KV.ToString(),
                                                 Request.Form["OKV"] == "" ? "0" : Request.Form["OKV"],
                                                 Request.Form["TOIL_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.OIL.ToString(),
                                                 Request.Form["TTHR_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.THR.ToString(),
                                                 Request.Form["TSEC_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.SEC.ToString(),
                                                 Request.Form["TPRI_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.PRI.ToString(),
                                                 Request.Form["TPOS_" + i.ToString()] == "" ? "null" : EXP_POST_VAR_INSTRU.TAP.ToString()
                                                );
                        }

                        ///INSERT TRANS INCOMING
                        ///
                        if ((!string.IsNullOrEmpty(Request.Form["TIMVAR_" + i.ToString()])) ||
                            (!string.IsNullOrEmpty(Request.Form["TIMW_" + i.ToString()])) ||
                            (!string.IsNullOrEmpty(Request.Form["TIA_" + i.ToString()])) ||
                            (!string.IsNullOrEmpty(Request.Form["TIKV_" + i.ToString()])) ||
                            (!string.IsNullOrEmpty(Request.Form["TOIL_" + i.ToString()])) ||
                            (!string.IsNullOrEmpty(Request.Form["TTHR_" + i.ToString()])) ||
                            (!string.IsNullOrEmpty(Request.Form["TSEC_" + i.ToString()])) ||
                            (!string.IsNullOrEmpty(Request.Form["TPRI_" + i.ToString()]))
                           )
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }
                        else if (EPIU_EPIU_ID != null)
                        {
                            DeleteRow(EXP_POST_VAR_INSTRU.EPVH_EPVH_ID, "null", 6, EXP_POST_VAR_INSTRU.INFV_TIME, EPIU_EPIU_ID);
                        }
                    }//end if
                }
                catch (Exception ex)
                {
                    errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در TRNAS INCOMING ");
                }

                #endregion

                #region/////////////////////////insert AUX,TEMP,HUMI

                decimal EPIU_ID = Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID).Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID).Select(xx => xx.EPIU_ID).FirstOrDefault();

                mode = "new";
                if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                           .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                           .Where(xx => xx.EPIV_TYPE == 7)
                                           .Select(xx => xx.EPIV_ID).Any())
                    )
                {
                    mode = "update";
                }

                sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,INFV_TIME,TEMP,HUMI,AUX_A,AUX_V,AUX_TG,EPIV_DESC,EPIV_TYPE) values ({0},{1},{2},{3},{4},{5},'{6}','{7}',7) ",
                                     EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                     EXP_POST_VAR_INSTRU.INFV_TIME,
                                     Request.Form["TEMP_" + i.ToString()] == "" ? "null" : Request.Form["TEMP_" + i.ToString()],
                                     Request.Form["HUMI_" + i.ToString()] == "" ? "null" : Request.Form["HUMI_" + i.ToString()],
                                     Request.Form["AUXA_" + i.ToString()] == "" ? "null" : Request.Form["AUXA_" + i.ToString()],
                                     Request.Form["AUXV_" + i.ToString()] == "" ? "null" : Request.Form["AUXV_" + i.ToString()],
                                     Request.Form["AUXTG_" + i.ToString()] == "" ? "T" : Request.Form["AUXTG_" + i.ToString()],
                                     "AUX,TEMP,HUMI"
                      );

                if (mode == "update")
                {
                    sql = string.Format("update EXP_POST_VAR_INSTRU set   TEMP ={2} ,HUMI={3} ,AUX_A={4} ,AUX_V={5} ,AUX_TG='{6}'  where INFV_TIME={1}  and EPVH_EPVH_ID= {0} and temp is not null and humi is not null and EPIV_TYPE=7 ",
                                         EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                         EXP_POST_VAR_INSTRU.INFV_TIME,
                                         Request.Form["TEMP_" + i.ToString()] == "" ? "null" : Request.Form["TEMP_" + i.ToString()],
                                         Request.Form["HUMI_" + i.ToString()] == "" ? "null" : Request.Form["HUMI_" + i.ToString()],
                                         Request.Form["AUXA_" + i.ToString()] == "" ? "null" : Request.Form["AUXA_" + i.ToString()],
                                         Request.Form["AUXV_" + i.ToString()] == "" ? "null" : Request.Form["AUXV_" + i.ToString()],
                                         Request.Form["AUXTG_" + i.ToString()] == "" ? "T" : Request.Form["AUXTG_" + i.ToString()]
                                         );
                }

                if (Request.Form["TEMP_" + i.ToString()] != ""
                     || Request.Form["HUMI_" + i.ToString()] != ""
                     || Request.Form["AUXA_" + i.ToString()] != ""
                     || Request.Form["AUXV_" + i.ToString()] != ""
                     || Request.Form["AUXTG_" + i.ToString()] != ""
                    )
                {
                    try
                    {
                        Db.Database.ExecuteSqlCommand(sql);
                    }
                    catch (Exception ex)
                    {
                        errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در Temp,Humidity ");
                    }
                }

                #endregion

                #region   //////////////////////////////////////////////insert reactor

                var r = Request.Form["R"];
                if (reactor_epiu_id != 0)
                {
                    string reactor_mode = "new";

                    if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                               .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                               .Where(xx => xx.EPIU_EPIU_ID == reactor_epiu_id)
                                               .Select(xx => xx.EPIV_ID).Any())
                        )
                    {
                        reactor_mode = "update";
                    }

                    // EXP_POST_VAR_INSTRU.R = short.Parse(Request.Form["R"]);
                    sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,KV,MVAR,A,WIND,OIL,R,INFV_TIME,EPIV_DESC,EPIV_TYPE) values ({0},{1},{2},{3},{4},{5},{6},{7},{8},'{9}',2) ",
                                         reactor_epiu_id,
                                         EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                         Request.Form["RKV_" + i.ToString()] == "" ? "0" : Request.Form["RKV_" + i.ToString()],
                                         Request.Form["RMVAR_" + i.ToString()] == "" ? "0" : Request.Form["RMVAR_" + i.ToString()],
                                         Request.Form["RA_" + i.ToString()] == "" ? "0" : Request.Form["RA_" + i.ToString()],
                                         Request.Form["RWIND_" + i.ToString()] == "" ? "0" : Request.Form["RWIND_" + i.ToString()],
                                         Request.Form["ROIL_" + i.ToString()] == "" ? "0" : Request.Form["ROIL_" + i.ToString()],
                                         Request.Form["R"] == "" ? "0" : Request.Form["R"],
                                         EXP_POST_VAR_INSTRU.INFV_TIME,
                                         "REACTOR"
                                         );

                    if (reactor_mode == "update")
                    {
                        sql = string.Format("update EXP_POST_VAR_INSTRU set  KV={2},MVAR={3},A={4},WIND={5},OIL={6},R={7} where  EPIU_EPIU_ID={0} and EPVH_EPVH_ID={1} and INFV_TIME='{8}' and EPIV_TYPE=2 ",
                                             reactor_epiu_id,
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             Request.Form["RKV_" + i.ToString()] == "" ? "0" : Request.Form["RKV_" + i.ToString()],
                                             Request.Form["RMVAR_" + i.ToString()] == "" ? "0" : Request.Form["RMVAR_" + i.ToString()],
                                             Request.Form["RA_" + i.ToString()] == "" ? "0" : Request.Form["RA_" + i.ToString()],
                                             Request.Form["RWIND_" + i.ToString()] == "" ? "0" : Request.Form["RWIND_" + i.ToString()],
                                             Request.Form["ROIL_" + i.ToString()] == "" ? "0" : Request.Form["ROIL_" + i.ToString()],
                                             Request.Form["R"] == "" ? "0" : Request.Form["R"],
                                             EXP_POST_VAR_INSTRU.INFV_TIME);
                    }

                    if (Request.Form["RKV_" + i.ToString()] != "" || Request.Form["RMVAR_" + i.ToString()] != "" ||
                        Request.Form["RA_" + i.ToString()] != "" || Request.Form["RWIND_" + i.ToString()] != "" ||
                        Request.Form["ROIL_" + i.ToString()] != "")
                    {
                        try
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }
                        catch (Exception ex)
                        {
                            errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در راکتور ");
                        }
                    }
                }

                #endregion

                #region //////////////////////////////////////////////InSERT LINE

                for (int j = 1; j < 9; j++)
                {
                    EXP_POST_LINE.EPOL_TYPE = "1";
                    EXP_POST_LINE_INSTRU.CODE_DISP = Request.Form["l" + j];
                    if (!string.IsNullOrEmpty(EXP_POST_LINE_INSTRU.CODE_DISP) && EXP_POST_LINE_INSTRU.CODE_DISP != "--")
                    {
                        try
                        {
                            //insert into EXP_POST_LINE
                            if (Request.Form["MVAR" + j.ToString() + "_" + i.ToString()] != "")
                            {
                                EXP_POST_VAR_INSTRU.MVAR = decimal.Parse(Request.Form["MVAR" + j.ToString() + "_" + i.ToString()]);
                            }
                            // else { EXP_POST_VAR_INSTRU.MVAR = 0; }

                            if (Request.Form["MW" + j.ToString() + "_" + i.ToString()] != "")
                            {
                                EXP_POST_VAR_INSTRU.MW = decimal.Parse(Request.Form["MW" + j.ToString() + "_" + i.ToString()]);
                            }
                            //  else { EXP_POST_VAR_INSTRU.MW = 0; }

                            if (Request.Form["KV" + j.ToString() + "_" + i.ToString()] != "")
                            {
                                EXP_POST_VAR_INSTRU.KV = decimal.Parse(Request.Form["KV" + j.ToString() + "_" + i.ToString()]);
                            }
                            //  else { EXP_POST_VAR_INSTRU.KV = 0; }

                            EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = Db.Database.SqlQuery<int>(string.Format("select EPIU_ID  from EXP_POST_LINE_INSTRU,EXP_POST_LINE where EXP_POST_LINE_INSTRU.EINS_EINS_ID in (1,2) and upper(EXP_POST_LINE_INSTRU.CODE_DISP)=upper('{0}') and EXP_POST_LINE_INSTRU.epol_epol_id=EXP_POST_LINE.EPOL_ID and EXP_POST_LINE.epol_type is not null ",
                                                                                                        EXP_POST_LINE_INSTRU.CODE_DISP)).FirstOrDefault();

                            if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID == 0)
                            {
                                throw new Exception("line");
                            }

                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 1)
                                                       .Where(xx => xx.LINE_COLUM != j)
                                                       .Where(xx => xx.EPIU_EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID)
                                                       .Select(xx => xx.EPIV_ID).Any())
                              )
                            {
                                throw new Exception("duplicate");
                            }

                            string linemode = "new";
                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                       .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 1)
                                                       .Where(xx => xx.LINE_COLUM == j)
                                                       .Select(xx => xx.EPIV_ID).Any())
                               )
                            {
                                linemode = "update";
                            }

                            sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,MW,MVAR,INFV_TIME,LINE_COLUM,EPIV_DESC,KV,EPIV_TYPE) values ({0},{1},{2},{3},{4},{5},'{6}',{7},1) ",
                                                 EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 EXP_POST_VAR_INSTRU.MW,
                                                 EXP_POST_VAR_INSTRU.MVAR,
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 j,
                                                 "LINE", EXP_POST_VAR_INSTRU.KV);

                            if (linemode == "update")
                            {
                                sql = string.Format("update EXP_POST_VAR_INSTRU set MW={2},MVAR={3},KV={5},EPIU_EPIU_ID={0}   where    EPVH_EPVH_ID={1} and INFV_TIME='{4}' and LINE_COLUM={6} and EPIV_TYPE=1  ",
                                                     EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                     EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                     EXP_POST_VAR_INSTRU.MW,
                                                     EXP_POST_VAR_INSTRU.MVAR,
                                                     EXP_POST_VAR_INSTRU.INFV_TIME,
                                                     EXP_POST_VAR_INSTRU.KV,
                                                     j);
                            }

                            if ((Request.Form["MVAR" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["MW" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["KV" + j.ToString() + "_" + i.ToString()] != ""))
                            {
                                Db.Database.ExecuteSqlCommand(sql);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "duplicate" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " تکراری میباشد ");
                            }
                            else if (ex.Message == "line" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " اشتباه است لطفا با مرکز پیام هماهنگ کنید ");
                            }
                            else if (ex.Message != "line" && ex.Message != "duplicate")
                            {
                                errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در خط " + EXP_POST_LINE_INSTRU.CODE_DISP);
                            }
                        }
                    }//end if
                }
                #endregion

                #region/////////////////////////////INSERT L.V DC

                string mode_48 = "", mode_110 = "", mode_220 = "";
                for (int j = 1; j < 3; j++)
                {
                    try
                    {

                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,A,V,INFV_TIME,EPIV_DESC,LINE_COLUM,EPIV_TYPE) values ({0},{1},{2},{3},'{4}',{5},3) ",
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             Request.Form["48A" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["48A" + j.ToString() + "_" + i.ToString()],
                                             Request.Form["48V" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["48V" + j.ToString() + "_" + i.ToString()],
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             "L.V_DC_48",
                                             j);

                        if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                   .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                   .Where(xx => xx.LINE_COLUM == j)
                                                   .Where(xx => xx.EPIV_DESC == "L.V_DC_48")
                                                   .Select(xx => xx.EPIV_ID).Any())
                             )
                        {
                            mode_48 = "update";
                        }

                        if (mode_48 == "update")
                        {
                            sql = string.Format("update EXP_POST_VAR_INSTRU set A={1} ,V={2} WHERE   EPVH_EPVH_ID={0} AND INFV_TIME='{3}' AND EPIV_DESC='{4}' AND LINE_COLUM= {5} ",
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["48A" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["48A" + j.ToString() + "_" + i.ToString()],
                                                 Request.Form["48V" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["48V" + j.ToString() + "_" + i.ToString()],
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 "L.V_DC_48",
                                                 j);
                        }

                        if (Request.Form["48A" + j.ToString() + "_" + i.ToString()] != "" || Request.Form["48V" + j.ToString() + "_" + i.ToString()] != "")
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }

                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,A,V,INFV_TIME,EPIV_DESC,LINE_COLUM,EPIV_TYPE) values ({0},{1},{2},{3},'{4}',{5},4) ",
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             Request.Form["110A" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["110A" + j.ToString() + "_" + i.ToString()],
                                             Request.Form["110V" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["110V" + j.ToString() + "_" + i.ToString()],
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             "L.V_DC_110",
                                             j);

                        if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                   .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                   .Where(xx => xx.LINE_COLUM == j)
                                                   .Where(xx => xx.EPIV_DESC == "L.V_DC_110")
                                                   .Select(xx => xx.EPIV_ID).Any())
                                                   )
                        {
                            mode_110 = "update";
                        }

                        if (mode_110 == "update")
                        {
                            sql = string.Format("update  EXP_POST_VAR_INSTRU set A={1} ,V={2} WHERE   EPVH_EPVH_ID={0} AND INFV_TIME='{3}' AND EPIV_DESC='{4}' AND LINE_COLUM= {5} ",
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["110A" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["110A" + j.ToString() + "_" + i.ToString()],
                                                 Request.Form["110V" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["110V" + j.ToString() + "_" + i.ToString()],
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 "L.V_DC_110",
                                                 j);
                        }

                        if (Request.Form["110A" + j.ToString() + "_" + i.ToString()] != "" || Request.Form["110V" + j.ToString() + "_" + i.ToString()] != "")
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }

                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,A,V,INFV_TIME,EPIV_DESC,LINE_COLUM,EPIV_TYPE) values ({0},{1},{2},{3},'{4}',{5},5) ",
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             Request.Form["220A" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["220A" + i.ToString() + "_" + i.ToString()],
                                             Request.Form["220V" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["220V" + i.ToString() + "_" + i.ToString()],
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             "L.V_DC_220",
                                             j);

                        if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                   .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                   .Where(xx => xx.LINE_COLUM == j)
                                                   .Where(xx => xx.EPIV_DESC == "L.V_DC_220")
                                                   .Select(xx => xx.EPIV_ID).Any())
                             )
                        {
                            mode_220 = "update";
                        }

                        if (mode_220 == "update")
                        {
                            sql = string.Format("update  EXP_POST_VAR_INSTRU set A={1} ,V={2} WHERE   EPVH_EPVH_ID={0} AND INFV_TIME='{3}' AND EPIV_DESC='{4}' AND LINE_COLUM= {5}",
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["220A" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["220A" + j.ToString() + "_" + i.ToString()],
                                                 Request.Form["220V" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["220V" + j.ToString() + "_" + i.ToString()],
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 "L.V_DC_220",
                                                 j);
                        }

                        if (Request.Form["220A" + j.ToString() + "_" + i.ToString()] != "" || Request.Form["220V" + j.ToString() + "_" + i.ToString()] != "")
                        {
                            if (j == 1)
                            {
                                Db.Database.ExecuteSqlCommand(sql);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در خط " + EXP_POST_LINE.EPOL_NAME);
                    }
                }//end for

                #endregion

            }

            if (errors.Count == 0)
            {
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            else
            {
                string errorMessages = string.Join("<br />", errors.ToArray());
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errorMessages }.ToJson();
            }
        }

        public ActionResult insert_dist(EXP_POST_VAR_HEAD objecttemp)
        {
            #region //Header

            List<string> errors = new List<string>();
            int EPIU_EPIU_ID = 0;
            //decimal epiu_id = 0;
            string mode = "new", sql = string.Empty;

            //insert into EXP_POST_VAR_HEAD
            var q = from b in Db.EXP_POST_VAR_HEAD
                    where b.VAR_DATE == objecttemp.VAR_DATE && b.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID && b.EPVH_TYPE == 3
                    select new { b.EPVH_ID };

            if (!q.Any())
            {
                sql = string.Format("insert into EXP_POST_VAR_HEAD (VAR_DATE,EPOL_EPOL_ID,epvh_type,EPVT_EPVT_ID,EPVH_DESC) values ('{0}','{1}',3,{2},'{3}') ", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID, 1, objecttemp.EPVH_DESC);
                Db.Database.ExecuteSqlCommand(sql);
            }

            var EXP_POST_VAR_INSTRU = new EXP_POST_VAR_INSTRU();
            var EXP_POST_LINE = new EXP_POST_LINE();
            var EXP_POST_LINE_INSTRU = new EXP_POST_LINE_INSTRU();

            EXP_POST_VAR_INSTRU.EPVH_EPVH_ID = Db.Database.SqlQuery<int>(string.Format("select EPVH_ID from EXP_POST_VAR_HEAD where VAR_DATE='{0}' and EPOL_EPOL_ID={1} and EPVH_TYPE=3", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID)).FirstOrDefault();

            #endregion

            for (int i = 1; i < 25; i++)
            {
                #region //insert into EXP_POST_LINE

                mode = "new";
                EXP_POST_VAR_INSTRU.INFV_TIME = Request.Form["INFV_TIME"] + i.ToString();

                for (int j = 1; j < 9; j++)
                {
                    EXP_POST_LINE.EPOL_TYPE = "1";
                    EXP_POST_LINE_INSTRU.CODE_DISP = Request.Form["l" + j];
                    if (!string.IsNullOrEmpty(EXP_POST_LINE_INSTRU.CODE_DISP) && EXP_POST_LINE_INSTRU.CODE_DISP != "--")
                    {
                        try
                        {
                            string mvv = Request.Form["MVAR" + j.ToString() + i.ToString()];
                            if (Request.Form["MVAR" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.MVAR = decimal.Parse(Request.Form["MVAR" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MVAR = 0;

                            if (Request.Form["MW" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.MW = decimal.Parse(Request.Form["MW" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.MW = 0;

                            if (Request.Form["KV" + j.ToString() + "_" + i.ToString()] != "")
                                EXP_POST_VAR_INSTRU.KV = decimal.Parse(Request.Form["KV" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.KV = 0;

                            EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = Db.Database.SqlQuery<int>(string.Format("select EPIU_ID  from EXP_POST_LINE_INSTRU,EXP_POST_LINE where EXP_POST_LINE_INSTRU.EINS_EINS_ID in (1,2) and upper(EXP_POST_LINE_INSTRU.CODE_DISP)=upper('{0}') and EXP_POST_LINE_INSTRU.epol_epol_id=EXP_POST_LINE.EPOL_ID and EXP_POST_LINE.epol_type is not null ",
                                                                                                        EXP_POST_LINE_INSTRU.CODE_DISP)).FirstOrDefault();

                            if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID == 0)
                            {
                                throw new Exception("line");
                            }

                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 1)
                                                       .Where(xx => xx.LINE_COLUM != j)
                                                       .Where(xx => xx.EPIU_EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID)
                                                       .Select(xx => xx.EPIV_ID).Any()))
                            {
                                throw new Exception("duplicate");
                            }

                            string linemode = "new";
                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                       .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 1)
                                                       .Where(xx => xx.LINE_COLUM == j)
                                                       .Select(xx => xx.EPIV_ID).Any())
                                )
                            {
                                linemode = "update";
                            }

                            sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,MW,MVAR,INFV_TIME,LINE_COLUM,EPIV_DESC,KV,EPIV_TYPE) values ({0},{1},{2},{3},{4},{5},'{6}',{7},1) ",
                                                 EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 EXP_POST_VAR_INSTRU.MW,
                                                 EXP_POST_VAR_INSTRU.MVAR,
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 j,
                                                 "LINE", EXP_POST_VAR_INSTRU.KV);

                            if (linemode == "update")
                            {
                                sql = string.Format("update EXP_POST_VAR_INSTRU set MW={2},MVAR={3},KV={5}  ,EPIU_EPIU_ID={0}   where    EPVH_EPVH_ID={1} and INFV_TIME='{4}' and LINE_COLUM={6} and EPIV_TYPE=1  ",
                                                     EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                     EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                     EXP_POST_VAR_INSTRU.MW,
                                                     EXP_POST_VAR_INSTRU.MVAR,
                                                     EXP_POST_VAR_INSTRU.INFV_TIME,
                                                     EXP_POST_VAR_INSTRU.KV,
                                                     j);
                            }

                            if ((Request.Form["MVAR" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["MW" + j.ToString() + "_" + i.ToString()] != "") || (Request.Form["KV" + j.ToString() + "_" + i.ToString()] != ""))
                            {
                                Db.Database.ExecuteSqlCommand(sql);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "duplicate" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " تکراری میباشد ");
                            }
                            else if (ex.Message == "line" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE_INSTRU.CODE_DISP + " اشتباه است لطفا با مرکز پیام هماهنگ کنید ");
                            }
                            else if (ex.Message != "line" && ex.Message != "duplicate")
                            {
                                errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در خط " + EXP_POST_LINE.CODE_DISP);
                            }
                        }
                    }//if
                }//end for

                #endregion

                #region  //insert TRANS TAP

                mode = "new";
                if (!string.IsNullOrEmpty(Request.Form["EPIU_EPIU_ID"]))
                {
                    EXP_POST_LINE_INSTRU.EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);
                    EPIU_EPIU_ID = int.Parse(Request.Form["EPIU_EPIU_ID"]);

                    //epiu_id = Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID).Where(xx => xx.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID).Select(xx => xx.EPIU_ID).FirstOrDefault();

                    if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                               .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                               .Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID)
                                               .Where(xx => xx.EPIV_TYPE == 8)
                                               .Select(xx => xx.EPIV_ID).Any())
                        )
                    {
                        mode = "update";
                    }

                    sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,TAP,OIL_TR,OIL_TAP,OIL_LEVEL,WIND,INFV_TIME,EPIV_DESC,epiv_type,A,MVAR) values ({0},{1},{2},{3},{4},'{5}',{6},{7},'{8}',8,{9},{10}) ",
                                         EPIU_EPIU_ID,
                                         EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                         Request.Form["TAP_" + i.ToString()] == "" ? "null" : Request.Form["TAP_" + i.ToString()],
                                         Request.Form["OILTR_" + i.ToString()] == "" ? "null" : Request.Form["OILTR_" + i.ToString()],
                                         Request.Form["OILTAP_" + i.ToString()] == "" ? "null" : Request.Form["OILTAP_" + i.ToString()],
                                         Request.Form["OILLEVEL_" + i.ToString()] == "" ? "" : Request.Form["OILLEVEL_" + i.ToString()],
                                         Request.Form["WIND_" + i.ToString()] == "" ? "null" : Request.Form["WIND_" + i.ToString()],
                                         EXP_POST_VAR_INSTRU.INFV_TIME,
                                         " ",
                                         Request.Form["KHA_" + i.ToString()] == "" ? "null" : Request.Form["KHA_" + i.ToString()],
                                         Request.Form["KHMVAR_" + i.ToString()] == "" ? "null" : Request.Form["KHMVAR_" + i.ToString()]
                                         );

                    if (mode == "update")
                    {
                        sql = string.Format("update EXP_POST_VAR_INSTRU set  TAP={2},OIL_TR={3},OIL_TAP={4},OIL_LEVEL='{5}',WIND={6},A={8},MVAR={9} where  EPIU_EPIU_ID={0} and EPVH_EPVH_ID={1} and INFV_TIME='{7}' and epiv_type=8",
                                             EPIU_EPIU_ID,
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             Request.Form["TAP_" + i.ToString()] == "" ? "null" : Request.Form["TAP_" + i.ToString()],
                                             Request.Form["OILTR_" + i.ToString()] == "" ? "null" : Request.Form["OILTR_" + i.ToString()],
                                             Request.Form["OILTAP_" + i.ToString()] == "" ? "null" : Request.Form["OILTAP_" + i.ToString()],
                                             Request.Form["OILLEVEL_" + i.ToString()] == "" ? "" : Request.Form["OILLEVEL_" + i.ToString()],
                                             Request.Form["WIND_" + i.ToString()] == "" ? "null" : Request.Form["WIND_" + i.ToString()],
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             Request.Form["KHA_" + i.ToString()] == "" ? "null" : Request.Form["KHA_" + i.ToString()],
                                             Request.Form["KHMVAR_" + i.ToString()] == "" ? "null" : Request.Form["KHMVAR_" + i.ToString()]
                                             );
                    }

                    try
                    {
                        if (Request.Form["TAP_" + i.ToString()] != "" ||
                            Request.Form["OILTR_" + i.ToString()] != "" ||
                            Request.Form["OILTAP_" + i.ToString()] != "" ||
                            Request.Form["OILLEVEL_" + i.ToString()] != "" ||
                            Request.Form["WIND_" + i.ToString()] != "" ||
                            Request.Form["KHA_" + i.ToString()] != ""
                            )
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }
                        else if (EPIU_EPIU_ID != null)
                        {
                            DeleteRow(EXP_POST_VAR_INSTRU.EPVH_EPVH_ID, "", 8, EXP_POST_VAR_INSTRU.INFV_TIME, (int)EPIU_EPIU_ID);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add("خطا در ثبت اطلاعات   " + ex.Message);
                    }

                    #endregion

                    #region //TRNAS INCOMING

                    mode = "new";
                    if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                               .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                               .Where(xx => xx.EPIU_EPIU_ID == EPIU_EPIU_ID)
                                               .Where(xx => xx.EPIV_TYPE == 6)
                                               .Select(xx => xx.EPIV_ID).Any())
                       )
                    {
                        mode = "update";
                    }

                    try
                    {
                        if (!string.IsNullOrEmpty(Request.Form["IMVAR_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.MVAR = decimal.Parse(Request.Form["IMVAR_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.MVAR = 0;

                        if (!string.IsNullOrEmpty(Request.Form["IMW_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.MW = decimal.Parse(Request.Form["IMW_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.MW = 0;

                        if (!string.IsNullOrEmpty(Request.Form["IA_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.A = decimal.Parse(Request.Form["IA_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.A = 0;

                        if (!string.IsNullOrEmpty(Request.Form["IKV_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.KV = decimal.Parse(Request.Form["IKV_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.KV = 0;

                        if (!string.IsNullOrEmpty(Request.Form["cos_" + i.ToString()]))
                            EXP_POST_VAR_INSTRU.COSINUS = decimal.Parse(Request.Form["cos_" + i.ToString()]);
                        else
                            EXP_POST_VAR_INSTRU.COSINUS = 0;


                        ///insert INCOMING
                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,MW,MVAR,INFV_TIME,A,KV,EPIV_DESC,EPIV_TYPE, COSINUS) values ({0},{1},{2},{3},{4},{5},{6},'{7}',6,{8}) ",
                                             EPIU_EPIU_ID,
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             Request.Form["IMW_" + i.ToString()] == "" ? "null" : Request.Form["IMW_" + i.ToString()],
                                             Request.Form["IMVAR_" + i.ToString()] == "" ? "null" : Request.Form["IMVAR_" + i.ToString()],
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             Request.Form["IA_" + i.ToString()] == "" ? "null" : Request.Form["IA_" + i.ToString()],
                                             Request.Form["IKV_" + i.ToString()] == "" ? "null" : Request.Form["IKV_" + i.ToString()],
                                             "TRNAS INCOMING",
                                             Request.Form["cos_" + i.ToString()] == "" ? "null" : Request.Form["cos_" + i.ToString()]
                                             );

                        if (mode == "update")
                        {
                            sql = string.Format("update EXP_POST_VAR_INSTRU set   MW ={2} , MVAR={3},A={5},KV={6}, COSINUS={7} where INFV_TIME={4} and EPIU_EPIU_ID={0}  and EPVH_EPVH_ID= {1} and EPIV_TYPE=6  ",
                                                 EPIU_EPIU_ID,
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["IMW_" + i.ToString()] == "" ? "null" : Request.Form["IMW_" + i.ToString()],
                                                 Request.Form["IMVAR_" + i.ToString()] == "" ? "null" : Request.Form["IMVAR_" + i.ToString()],
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 Request.Form["IA_" + i.ToString()] == "" ? "null" : Request.Form["IA_" + i.ToString()],
                                                 Request.Form["IKV_" + i.ToString()] == "" ? "null" : Request.Form["IKV_" + i.ToString()],
                                                 Request.Form["cos_" + i.ToString()] == "" ? "null" : Request.Form["cos_" + i.ToString()]
                                                 );
                        }

                        if ((!string.IsNullOrEmpty(Request.Form["IMVAR_" + i.ToString()])) ||
                            (!string.IsNullOrEmpty(Request.Form["IMW_" + i.ToString()])) ||
                            (!string.IsNullOrEmpty(Request.Form["IA_" + i.ToString()])) ||
                            (!string.IsNullOrEmpty(Request.Form["IKV_" + i.ToString()])) ||
                            (!string.IsNullOrEmpty(Request.Form["cos_" + i.ToString()]))
                           )
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }
                        else if (EPIU_EPIU_ID != null)
                        {
                            DeleteRow(EXP_POST_VAR_INSTRU.EPVH_EPVH_ID, "", 6, EXP_POST_VAR_INSTRU.INFV_TIME, (int)EPIU_EPIU_ID);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add("خطا در ثبت اطلاعات ساعت  " + ex.Message);
                    }
                }//end if

                #endregion

                #region //TRANS

                for (int j = 1; j < 9; j++)
                {
                    EXP_POST_LINE.EPOL_TYPE = "1";
                    EXP_POST_LINE.EPOL_NAME = Request.Form["Tl" + j];
                    if (EXP_POST_LINE.EPOL_NAME == "")
                    {
                        Db.Database.ExecuteSqlCommand(string.Format("DELETE FROM EXP_POST_VAR_INSTRU WHERE EPVH_EPVH_ID={0} and LINE_COLUM={1} and EPIV_TYPE=0 and EPIU_EPIU_ID2={2}", EXP_POST_VAR_INSTRU.EPVH_EPVH_ID, j, EPIU_EPIU_ID));
                    }

                    if (!string.IsNullOrEmpty(EXP_POST_LINE.EPOL_NAME) && EXP_POST_LINE.EPOL_NAME != "--")
                    {
                        try
                        {
                            //inser into EXP_POST_LINE_INSTRU
                            //مقدار دهی ردیف خط
                            EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = Db.Database.SqlQuery<int>(string.Format("select EPIU_ID  from EXP_POST_LINE_INSTRU,EXP_POST_LINE where EXP_POST_LINE_INSTRU.EINS_EINS_ID in (1,2) and upper(EXP_POST_LINE_INSTRU.CODE_DISP)=upper('{0}') and EXP_POST_LINE_INSTRU.epol_epol_id=EXP_POST_LINE.EPOL_ID and EXP_POST_LINE.epol_type is not null ",
                                                                                                        EXP_POST_LINE.EPOL_NAME)).FirstOrDefault();

                            if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID == 0)
                            {
                                throw new Exception("line");
                            }

                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIV_TYPE == 0)
                                                       .Where(xx => xx.LINE_COLUM != j)
                                                       .Where(xx => xx.EPIU_EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID)
                                                       .Select(xx => xx.EPIV_ID).Any()))
                            {
                                throw new Exception("duplicate");
                            }

                            //insert into  EXP_POST_VAR_INSTRU 
                            if (!string.IsNullOrEmpty(Request.Form["A" + j.ToString() + "_" + i.ToString()]))
                                EXP_POST_VAR_INSTRU.A = decimal.Parse(Request.Form["A" + j.ToString() + "_" + i.ToString()]);
                            else
                                EXP_POST_VAR_INSTRU.A = 0;

                            EXP_POST_VAR_INSTRU.EPVH_EPVH_ID = Db.Database.SqlQuery<int>(string.Format("select EPVH_ID  from EXP_POST_VAR_HEAD where VAR_DATE='{0}' and EPOL_EPOL_ID={1}",
                                                                                                        objecttemp.VAR_DATE,
                                                                                                        objecttemp.EPOL_EPOL_ID)).FirstOrDefault();

                            string linemode = "new";
                            if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                       .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                       .Where(xx => xx.EPIU_EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID)
                                                       .Select(xx => xx.EPIV_ID).Any())
                               )
                            {
                                linemode = "update";
                            }

                            sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,A,INFV_TIME,LINE_COLUM,EPIV_DESC,EPIU_EPIU_ID2,epiv_type) values ({0},{1},{2},{3},{4},'{5}',{6},0) ",
                                                 EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["A" + j.ToString() + "_" + i.ToString()] == "" ? "null" : Request.Form["A" + j.ToString() + "_" + i.ToString()],
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 j,
                                                 "TRANS", EPIU_EPIU_ID);

                            if (linemode == "update")
                            {
                                sql = string.Format("update EXP_POST_VAR_INSTRU set A={2} , EPIU_EPIU_ID={0}   where epiv_type=0 and   EPVH_EPVH_ID={1} and INFV_TIME='{3}' and EPIU_EPIU_ID2={4} and LINE_COLUM={5}   ",
                                                     EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                     EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                     Request.Form["A" + j.ToString() + "_" + i.ToString()] == "" ? "null" : Request.Form["A" + j.ToString() + "_" + i.ToString()],
                                                     EXP_POST_VAR_INSTRU.INFV_TIME,
                                                     EPIU_EPIU_ID,
                                                     j);
                            }

                            if (EXP_POST_VAR_INSTRU.A != 0)
                            {
                                Db.Database.ExecuteSqlCommand(sql);
                            }
                            else if (EXP_POST_VAR_INSTRU.EPIU_EPIU_ID != null)
                            {
                                DeleteRow(EXP_POST_VAR_INSTRU.EPVH_EPVH_ID, "", 0, EXP_POST_VAR_INSTRU.INFV_TIME, (int)EXP_POST_VAR_INSTRU.EPIU_EPIU_ID);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "duplicate" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE.EPOL_NAME + " تکراری میباشد ");
                            }
                            else if (ex.Message == "line" && i == j)
                            {
                                errors.Add(" کد دیسپاچینگی خط " + EXP_POST_LINE.EPOL_NAME + " اشتباه است لطفا با مرکز پیام هماهنگ کنید ");
                            }
                            else if (ex.Message != "line" && ex.Message != "duplicate")
                            {
                                errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در خط خروجی تب ترانس " + EXP_POST_LINE_INSTRU.CODE_DISP);
                            }
                        }
                    }//if
                }//end for

                #endregion

                #region

                try
                {
                    if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                               .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                               .Where(xx => xx.EPIV_TYPE == 7)
                                               .Select(xx => xx.EPIV_ID).Any())
                       )
                    {
                        mode = "update";
                    }

                    sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,INFV_TIME,TEMP,HUMI,AUX_A,AUX_V,AUX_TG,EPIV_DESC,EPIV_TYPE) values ({0},{1},{2},{3},{4},{5},'{6}','{7}',7) ",
                                         EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                         EXP_POST_VAR_INSTRU.INFV_TIME,
                                         Request.Form["TEMP_" + i.ToString()] == "" ? "0" : Request.Form["TEMP_" + i.ToString()],
                                         Request.Form["HUMI_" + i.ToString()] == "" ? "0" : Request.Form["HUMI_" + i.ToString()],
                                         Request.Form["AUXA_" + i.ToString()] == "" ? "0" : Request.Form["AUXA_" + i.ToString()],
                                         Request.Form["AUXV_" + i.ToString()] == "" ? "0" : Request.Form["AUXV_" + i.ToString()],
                                         Request.Form["AUXTG_" + i.ToString()] == "" ? "T1" : Request.Form["AUXTG_" + i.ToString()],
                                         "AUX,TEMP,HUMI"
                                         );

                    if (mode == "update")
                    {
                        sql = string.Format("update EXP_POST_VAR_INSTRU set   TEMP ={2} ,HUMI={3} ,AUX_A={4} ,AUX_V={5} ,AUX_TG='{6}'  where INFV_TIME={1}  and EPVH_EPVH_ID= {0} and temp is not null and humi is not null ",
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             Request.Form["TEMP_" + i.ToString()] == "" ? "0" : Request.Form["TEMP_" + i.ToString()],
                                             Request.Form["HUMI_" + i.ToString()] == "" ? "0" : Request.Form["HUMI_" + i.ToString()],
                                             Request.Form["AUXA_" + i.ToString()] == "" ? "0" : Request.Form["AUXA_" + i.ToString()],
                                             Request.Form["AUXV_" + i.ToString()] == "" ? "0" : Request.Form["AUXV_" + i.ToString()],
                                             Request.Form["AUXTG_" + i.ToString()] == "" ? "T1" : Request.Form["AUXTG_" + i.ToString()]
                                             );
                    }

                    if (Request.Form["TEMP_" + i.ToString()] != "" ||
                        Request.Form["HUMI_" + i.ToString()] != "" ||
                        Request.Form["AUXA_" + i.ToString()] != "" ||
                        Request.Form["AUXV_" + i.ToString()] != ""
                        )
                    {
                        Db.Database.ExecuteSqlCommand(sql);
                    }

                    for (int j = 1; j < 3; j++)
                    {
                        string mode_48 = "", mode_110 = "";
                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,A,V,INFV_TIME,EPIV_DESC,LINE_COLUM,EPIV_TYPE) values ({0},{1},{2},{3},'{4}',{5},3) ",
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             Request.Form["48A" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["48A" + j.ToString() + "_" + i.ToString()],
                                             Request.Form["48V" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["48V" + j.ToString() + "_" + i.ToString()],
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             "L.V_DC_48",
                                             j);

                        if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                   .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                   .Where(xx => xx.LINE_COLUM == j)
                                                   .Where(xx => xx.EPIV_TYPE == 3)
                                                   .Select(xx => xx.EPIV_ID).Any())
                             )
                        {
                            mode_48 = "update";
                        }

                        if (mode_48 == "update")
                        {
                            sql = string.Format("update  EXP_POST_VAR_INSTRU set A={1} ,V={2} WHERE   EPVH_EPVH_ID={0} AND INFV_TIME='{3}' AND EPIV_DESC='{4}' AND LINE_COLUM= {5} ",
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["48A" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["48A" + j.ToString() + "_" + i.ToString()],
                                                 Request.Form["48V" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["48V" + j.ToString() + "_" + i.ToString()],
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 "L.V_DC_48",
                                                 j);
                        }

                        if (Request.Form["48A" + j.ToString() + "_" + i.ToString()] != "" || Request.Form["48V" + j.ToString() + "_" + i.ToString()] != "")
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }

                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPVH_EPVH_ID,A,V,INFV_TIME,EPIV_DESC,LINE_COLUM,EPIV_TYPE) values ({0},{1},{2},{3},'{4}',{5},4) ",
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             Request.Form["110A" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["110A" + j.ToString() + "_" + i.ToString()],
                                             Request.Form["110V" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["110V" + j.ToString() + "_" + i.ToString()],
                                             EXP_POST_VAR_INSTRU.INFV_TIME,
                                             "L.V_DC_110",
                                             j);

                        if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.INFV_TIME == EXP_POST_VAR_INSTRU.INFV_TIME)
                                                   .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                   .Where(xx => xx.LINE_COLUM == j)
                                                   .Where(xx => xx.EPIV_TYPE == 4)
                                                   .Select(xx => xx.EPIV_ID).Any())
                            )
                        {
                            mode_110 = "update";
                        }

                        if (mode_110 == "update")
                        {
                            sql = string.Format("update  EXP_POST_VAR_INSTRU set A={1} ,V={2} WHERE   EPVH_EPVH_ID={0} AND INFV_TIME='{3}' AND EPIV_DESC='{4}' AND LINE_COLUM= {5} ",
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 Request.Form["110A" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["110A" + j.ToString() + "_" + i.ToString()],
                                                 Request.Form["110V" + j.ToString() + "_" + i.ToString()] == "" ? "0" : Request.Form["110V" + j.ToString() + "_" + i.ToString()],
                                                 EXP_POST_VAR_INSTRU.INFV_TIME,
                                                 "L.V_DC_110",
                                                 j);
                        }

                        if (Request.Form["110A" + j.ToString() + "_" + i.ToString()] != "" || Request.Form["110V" + j.ToString() + "_" + i.ToString()] != "")
                        {
                            Db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    #endregion

                }
                catch (Exception ex)
                {
                    errors.Add("خطا در ثبت اطلاعات ساعت  " + EXP_POST_VAR_INSTRU.INFV_TIME + "  در تب L.V DC & AC AUX & TEMPERATURE & HUMIDITY ");
                }
            }//for i

            if (errors.Count == 0)
            {
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            else
            {
                string errorMessages = string.Join("<br />", errors.ToArray());
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errorMessages }.ToJson();
            }
        }

        public ActionResult insert_trans_mont(EXP_POST_VAR_HEAD objecttemp)
        {
            string mode = "new", sql = string.Empty;

            #region//insert into EXP_POST_VAR_HEAD

            //objecttemp.EPVT_EPVT_ID = Db.exp_type;
            if (objecttemp.EPOL_EPOL_ID == null)
            {
                objecttemp.EPOL_EPOL_ID = int.Parse(Request.Form["EPOL_EPOL_ID2"]);
            }

            var q = from b in Db.EXP_POST_VAR_HEAD
                    where b.VAR_DATE == objecttemp.VAR_DATE && b.EPOL_EPOL_ID == objecttemp.EPOL_EPOL_ID && b.EPVH_TYPE == objecttemp.EPVH_TYPE
                    select new { b.EPVH_ID };

            if (!q.Any())
            {
                sql = string.Format("insert into EXP_POST_VAR_HEAD (VAR_DATE,EPOL_EPOL_ID,epvh_type,EPVT_EPVT_ID) values ('{0}','{1}',{2},{3}) ", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID, objecttemp.EPVH_TYPE, 1);
                Db.Database.ExecuteSqlCommand(sql);
            }

            var EXP_POST_VAR_INSTRU = new EXP_POST_VAR_INSTRU();
            var EXP_POST_LINE = new EXP_POST_LINE();
            var EXP_POST_LINE_INSTRU = new EXP_POST_LINE_INSTRU();

            EXP_POST_VAR_INSTRU.EPVH_EPVH_ID = Db.Database.SqlQuery<int>(string.Format("select EPVH_ID from EXP_POST_VAR_HEAD where VAR_DATE='{0}' and EPOL_EPOL_ID={1} and epvh_type={2}", objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID, objecttemp.EPVH_TYPE)).FirstOrDefault();

            #endregion

            List<string> errors = new List<string>();
            int /*EPIU_EPIU_ID = 0,*/ row = 0;

            if (Request.Form["i"] != "")
            {
                row = int.Parse(Request.Form["i"]);
            }

            for (int i = 1; i <= row; i++)
            {
                #region//counter

                for (int j = 1; j < 3; j++)
                {
                    for (int x = 1; x <= 4; x++)
                    {
                        string counter = Request.Form["counter" + i + "_" + j + "_" + x];
                        string sf6 = Request.Form["sf6" + i + "_" + j + "_" + x];
                        if (counter != "" && counter != null)
                        {
                            try
                            {
                                EXP_POST_VAR_INSTRU.EPIV_PHAS = x.ToString();
                                EXP_POST_VAR_INSTRU.LINE_COLUM = short.Parse(j.ToString()); ;
                                EXP_POST_VAR_INSTRU.EPIV_TYPE = 10;
                                EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = decimal.Parse(Request.Form["EPIU_EPIU_ID_" + i]);
                                EXP_POST_VAR_INSTRU.COUNTER = decimal.Parse(counter);

                                if (!string.IsNullOrEmpty(sf6))
                                    EXP_POST_VAR_INSTRU.SF6 = decimal.Parse(sf6);
                                else
                                    EXP_POST_VAR_INSTRU.SF6 = 0;

                                mode = "new";
                                string phas = x.ToString();
                                if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPIV_PHAS == phas)
                                                           .Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                                           .Where(xx => xx.EPIV_TYPE == 10)
                                                           .Where(xx => xx.LINE_COLUM == j)
                                                           .Where(xx => xx.EPIU_EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID)
                                                           .Select(xx => xx.EPIV_ID).Any())
                                    )
                                {
                                    mode = "update";
                                }

                                sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,LINE_COLUM,EPIV_DESC,EPIV_TYPE,COUNTER,EPIV_PHAS,SF6,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},'COUNTER',10,{3},'{4}',{5},'{6}',7) ",
                                                     EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                     EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                     j,
                                                     EXP_POST_VAR_INSTRU.COUNTER,
                                                     x, EXP_POST_VAR_INSTRU.SF6, objecttemp.VAR_DATE, objecttemp.EPOL_EPOL_ID);

                                if (mode == "update")
                                {
                                    sql = string.Format("update EXP_POST_VAR_INSTRU set  counter={3} ,sf6={5} where EPIU_EPIU_ID={0} and EPVH_EPVH_ID={1} and LINE_COLUM={2} and EPIV_TYPE=10 and EPIV_PHAS={4}  ",
                                                         EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                         EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                         j,
                                                         EXP_POST_VAR_INSTRU.COUNTER,
                                                         x,


                                                         EXP_POST_VAR_INSTRU.SF6);
                                }

                                Db.Database.ExecuteSqlCommand(sql);
                            }
                            catch (Exception)
                            {
                                errors.Add("  خطا در ثبت اطلاعات    " + Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID).Select(xx => xx.CODE_NAME).FirstOrDefault() + "فاز :" + EXP_POST_VAR_INSTRU.EPIV_PHAS);
                            }
                        }
                    }
                }

                #endregion

                #region

                string mvarimp = "", mvarexp = "", mwimp = "", mwexp = "";
                try
                {
                    EXP_POST_VAR_INSTRU.EPIU_EPIU_ID = decimal.Parse(Request.Form["EPIU_EPIU_ID_" + i]);
                    mvarimp = Request.Form["mvarimp" + i];
                    mvarexp = Request.Form["mvarexp" + i];
                    mwimp = Request.Form["mwimp" + i];
                    mwexp = Request.Form["mwexp" + i];
                    mode = "new";

                    if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                               .Where(xx => xx.EPIV_TYPE == 11)
                                               .Where(xx => xx.EPIU_EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID)
                                               .Select(xx => xx.EPIV_ID).Any())
                       )
                    {
                        mode = "update";
                    }

                    if (!string.IsNullOrEmpty(mvarimp) || !string.IsNullOrEmpty(mvarexp))
                    {
                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,IMP,EXP,EPIV_DESC,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},'IMP,EXP MVAR',11,'{4}',{5}) ",
                                             EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             mvarimp == "" ? "0" : mvarimp,
                                             mvarexp == "" ? "0" : mvarexp,
                                             objecttemp.VAR_DATE,
                                             objecttemp.EPOL_EPOL_ID
                                             );

                        if (mode == "update")
                        {
                            sql = string.Format("update EXP_POST_VAR_INSTRU set  IMP={2} ,EXP={3}     where   EPIU_EPIU_ID={0} and EPVH_EPVH_ID={1} and  EPIV_TYPE=11   ",
                                                 EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 mvarimp == "" ? "0" : mvarimp,
                                                 mvarexp == "" ? "0" : mvarexp);
                        }

                        Db.Database.ExecuteSqlCommand(sql);
                    }
                }
                catch (Exception)
                {
                    errors.Add(" : خطا در ثبت اطلاعات    " + Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID).Select(xx => xx.CODE_NAME).FirstOrDefault().ToString());
                }

                try
                {
                    mode = "new";
                    if ((Db.EXP_POST_VAR_INSTRU.Where(xx => xx.EPVH_EPVH_ID == EXP_POST_VAR_INSTRU.EPVH_EPVH_ID)
                                               .Where(xx => xx.EPIV_TYPE == 12)
                                               .Where(xx => xx.EPIU_EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID)
                                               .Select(xx => xx.EPIV_ID).Any())
                       )
                    {
                        mode = "update";
                    }

                    if (!string.IsNullOrEmpty(mwimp) || !string.IsNullOrEmpty(mwexp))
                    {
                        sql = string.Format("insert into EXP_POST_VAR_INSTRU (EPIU_EPIU_ID,EPVH_EPVH_ID,IMP,EXP,EPIV_DESC,EPIV_TYPE,INFV_DATE,EPOL_EPOL_ID) values ({0},{1},{2},{3},'IMP,EXP MW',12,'{4}',{5}) ",
                                             EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                             EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                             mwimp == "" ? "0" : mwimp,
                                             mwexp == "" ? "0" : mwexp,
                                             objecttemp.VAR_DATE,
                                             objecttemp.EPOL_EPOL_ID
                                             );

                        if (mode == "update")
                        {
                            sql = string.Format("update EXP_POST_VAR_INSTRU set  IMP={2} ,EXP={3}     where   EPIU_EPIU_ID={0} and EPVH_EPVH_ID={1} and  EPIV_TYPE=12   ",
                                                 EXP_POST_VAR_INSTRU.EPIU_EPIU_ID,
                                                 EXP_POST_VAR_INSTRU.EPVH_EPVH_ID,
                                                 mwimp == "" ? "0" : mwimp,
                                                 mwexp == "" ? "0" : mwexp);
                        }

                        Db.Database.ExecuteSqlCommand(sql);
                    }
                }
                catch (Exception)
                {
                    errors.Add(" : خطا در ثبت اطلاعات    " + Db.EXP_POST_LINE_INSTRU.Where(xx => xx.EPIU_ID == EXP_POST_VAR_INSTRU.EPIU_EPIU_ID).Select(xx => xx.CODE_NAME).FirstOrDefault().ToString());
                }

                #endregion

            }

            if (errors.Count == 0)
            {
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            else
            {
                string errorMessages = string.Join("<br />", errors.ToArray());
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errorMessages }.ToJson();
            }

        }

    }

}

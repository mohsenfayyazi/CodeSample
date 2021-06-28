using Asr.Cartable;
using Asr.Security;
using Equipment.DAL;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Equipment.Controllers.Planning.Project
{
    [Authorize]
    public partial class ProjectController : Controller
    {
        BandarEntities db;

        public ProjectController()
        {
            db = this.DB();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //
        //ابلاغ پروژه
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Project_Add()
        {
            return View();
        }
        public ActionResult AddConflict(int? id)
        {
            ViewData["ESIL_ID"] = id;
            return View();
        }
        public ActionResult Add_Fider(int? id)
        {
            ViewData["ESIL_ID"] = id;
            return View();
        }
        public ActionResult ShowBudget(string id, int? ESIL_ID)
        {
            ViewData["FINY_YEAR"] = id;
            ViewData["ESIL_ID"] = ESIL_ID;
            return View();
        }
        public ActionResult AddSignificPlanUser(int? id)
        {
            ViewData["SIPL_ID"] = id;
            return View();
        }
        public ActionResult ShowSignificPlan(int? id)
        {
            ViewData["SIPL_ID"] = id;
            return View();
        }
        public ActionResult Select_Path(int? id)
        {
            ViewData["ESIL_ID"] = id;
            return View();
        }

        public ActionResult ViewFormCheckList(string id, decimal notId)
        {
            ViewData["ID"] = id;
            ViewData["notId"] = notId;
            return View();
        }
        public ActionResult Insert_Fider(PLN_SIGNIFIC_FIDER ObjTemp)
        {

            try
            {
                db.PLN_SIGNIFIC_FIDER.Add(ObjTemp);
                db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات  ") }.ToJson();

            }
        }
        public ActionResult InsertPrjDocument(PLN_SIGNIFIC_DOCUMENT ObjTemp)
        {

            try
            {
                db.PLN_SIGNIFIC_DOCUMENT.Add(ObjTemp);
                db.SaveChanges();




                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات  ") }.ToJson();

            }
        }


        public ActionResult InsertPrjConfilict(PLN_SIGNIFIC_CONFILICT ObjTemp)
        {

            try
            {
                db.PLN_SIGNIFIC_CONFILICT.Add(ObjTemp);
                db.SaveChanges();




                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات  ") }.ToJson();

            }
        }
        public ActionResult InsertPrjPlan(PLN_SIGNIFIC_PLAN ObjTemp)
        {

            try
            {
                db.PLN_SIGNIFIC_PLAN.Add(ObjTemp);
                db.SaveChanges();




                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات  ") }.ToJson();

            }
        }
        public ActionResult InsertPrjPlanUser(PLN_SIGNIFIC_PLAN_USER ObjTemp)
        {

            try
            {
                db.PLN_SIGNIFIC_PLAN_USER.Add(ObjTemp);
                db.SaveChanges();
                AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
                wp.StartProcess(this.HttpContext.User.Identity.Name, new string[] { this.HttpContext.User.Identity.Name },
                           db.EXP_TYPE_DOC.Where(xx => xx.ETDO_ID == 663).Select(xx => xx.ETDO_DESC).FirstOrDefault(),
                           "چک لیست انتخاب مسیر با شماره ردیف   " + ObjTemp.SIPL_SIPL_ID + "  ثبت شد ", 663,
                           ObjTemp.SIPL_SIPL_ID + "-" + ObjTemp.SIPU_ID);
                wp = new AsrWorkFlowProcess(return_noteid(ObjTemp.SIPL_SIPL_ID + "-" + ObjTemp.SIPU_ID, "FLW_PRCH.FLW_PRCH"));
                wp.Approve(new string[] { db.SEC_USERS.Where(xx => xx.ROW_NO == ObjTemp.SCSU_ROW_NO).Select(xx => xx.ORCL_NAME).FirstOrDefault() }, "چک لیست انتخاب مسیر جهت امتیاز دهی ارسال شد");
                var ObjectCheck = new PLN_SIGNIFIC_PLAN_CHECK_ITEM
                {
                    SIPL_SIPL_ID = ObjTemp.SIPL_SIPL_ID,
                    CHLT_CHLT_ID = db.PLN_SIGNIFIC_PLAN.Where(xx => xx.SIPL_ID == ObjTemp.SIPL_SIPL_ID).Select(xx => xx.CHLT_CHLT_ID).FirstOrDefault(),
                    SIPU_SIPU_ID = ObjTemp.SIPU_ID,
                    SPCI_ANSW = ""
                };
                var QueryItem = from b in db.CHK_ITEM_TEMPLATE where b.CHLT_CHLT_ID == ObjectCheck.CHLT_CHLT_ID select new { b.CHTI_ID };
                foreach (var Row in QueryItem)
                {
                    ObjectCheck.CHTI_CHTI_ID = Row.CHTI_ID;
                    db.PLN_SIGNIFIC_PLAN_CHECK_ITEM.Add(ObjectCheck);
                    db.SaveChanges();

                }
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات  " + ex.ToString()) }.ToJson();

            }
        }
        public ActionResult Insert_Key(PLN_SIGNIFIC_POWER_KEY ObjTemp)
        {

            try
            {
                db.PLN_SIGNIFIC_POWER_KEY.Add(ObjTemp);
                db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات  ") }.ToJson();

            }
        }
        public ActionResult Get_Fider([DataSourceRequest] DataSourceRequest request, short? ESIL_ID)
        {
            var query = (from p in db.PLN_SIGNIFIC_FIDER
                         join d in db.CHK_DOMAIN on p.DMAN_DMAN_ID equals d.DMAN_ID
                         join a in db.EXP_UNIT_LEVEL on p.EUNL_EUNL_ID equals a.EUNL_ID
                         where p.ESIL_ESIL_ID == ESIL_ID

                         select new
                         {
                             p.SIFI_ID,
                             p.ESIL_ESIL_ID,
                             DMAN_TITL = d.DMAN_TITL,
                             EUNL_DESC = a.EUNL_DESC,
                             p.SIFI_UNIT,
                             p.KA
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }
        public ActionResult Get_Key([DataSourceRequest] DataSourceRequest request, short? ESIL_ID)
        {
            var query = (from p in db.PLN_SIGNIFIC_POWER_KEY
                         join d in db.EXP_UNIT_LEVEL on p.EUNL_EUNL_ID equals d.EUNL_ID
                         where p.ESIL_ESIL_ID == ESIL_ID

                         select new
                         {
                             p.SIPK_ID,
                             p.ESIL_ESIL_ID,
                             EUNL_DESC = d.EUNL_DESC,

                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        public FileStreamResult SomeImage(int id)
        {
            MemoryStream MemorySt = new MemoryStream(db.PDF_PROJECT_SPECIFICATION.Where(xx => xx.ESIL_ESIL_ID == id).Select(xx => xx.PRSP_FILE).FirstOrDefault());
            //MemoryStream MemorySt = new MemoryStream(db.SCN_ATTACHE.Where(xx => xx.ID == 54781).Select(xx => xx.PDF_FILE).FirstOrDefault());
            var File = new FileStreamResult(MemorySt, "image/jpeg");
            File.FileDownloadName = db.PLN_SIGNIFIC_LETER.Where(xx => xx.ESIL_ID == id).Select(xx => xx.ESIL_NO).FirstOrDefault() + ".jpg";
            //byte[] bytes = db.PDF_PROJECT_SPECIFICATION.Where(xx => xx.ESIL_ESIL_ID == id).Select(xx => xx.PRSP_FILE).FirstOrDefault();
            return File;
        }
        public ActionResult insert_prj(IEnumerable<HttpPostedFileBase> PRSP_FILES)
        {
            // if (PublicRepository.check_col_u("TRH_WORK_TYPE", "WORK_DESC", objecttemp.WORK_DESC))
            // if (PublicRepository.ExistModel("TRH_WORK_TYPE", "DEL_DUMP_U(WORK_DESC) = DEL_DUMP_U('{0}') or DEL_DUMP_U(WORK_CODE) = DEL_DUMP_U('{1}')", objecttemp.WORK_DESC, objecttemp.WORK_CODE))
            //  {
            //      return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
            //  }
            //  else
            //  {
            try
            {
                int ESIL_ID = 0;
                if (!string.IsNullOrEmpty(Request.Form["ESIL_ID"]))
                {


                    ESIL_ID = int.Parse(Request.Form["ESIL_ID"]);


                }
                var objecttemp = new PDF_PROJECT_SPECIFICATION();
                if (ESIL_ID != 0)
                {
                    objecttemp = db.PDF_PROJECT_SPECIFICATION.Find(db.PDF_PROJECT_SPECIFICATION.Where(xx => xx.ESIL_ESIL_ID == ESIL_ID).Select(xx => xx.PRSP_ROW).FirstOrDefault());
                }
                string PLAN_PROJ_INPUT = Request.Form["PLAN_PROJ_INPUT"]; var val = PLAN_PROJ_INPUT.Split('^');
                var objecttemp2 = new PLN_SIGNIFIC_LETER();
                if (ESIL_ID != 0)
                {
                    objecttemp2 = db.PLN_SIGNIFIC_LETER.Find(ESIL_ID);
                }
                objecttemp2.CPRO_CPLA_PLN_CODE = short.Parse(val[0]);
                objecttemp2.CPRO_PRJ_CODE = short.Parse(val[1]);
                objecttemp2.END_DATE = Request.Form["END_DATE"];
                objecttemp2.ESIL_COMMENT = Request.Form["ESIL_COMMENT"];
                int MaxRow = 0;
                if (db.PLN_SIGNIFIC_LETER.Select(xx => xx.ESIL_ID).Any())
                {
                    MaxRow = db.PLN_SIGNIFIC_LETER.Select(xx => xx.ESIL_ID).Max();
                }

                objecttemp2.ESIL_TYPE = Request.Form["ESIL_TYPE"];
                objecttemp2.ESIL_DATE = Request.Form["ESIL_DATE"];
                objecttemp2.STAR_DATE = Request.Form["STAR_DATE"];
                //objecttemp2.ESIL_FGRANT = long.Parse(Request.Form["ESIL_FGRANT"]);
                //objecttemp2.ESIL_GRANT = long.Parse(Request.Form["ESIL_GRANT"]);
                objecttemp2.ESIL_PROJ_TYPE = Request.Form["ESIL_PROJ_TYPE"];
                objecttemp2.ESIL_NO = objecttemp2.ESIL_PROJ_TYPE == "1" ? "S_" + (MaxRow + 1).ToString() + "_" + Request.Form["ESIL_DATE"].Substring(0, 4) : "L_" + (MaxRow + 1).ToString() + "_" + Request.Form["ESIL_DATE"].Substring(0, 4);


                if (ESIL_ID != 0)
                {
                    UpdateModel(objecttemp2);
                }
                else
                {
                    db.PLN_SIGNIFIC_LETER.Add(objecttemp2);
                }

                db.SaveChanges();
                var objecttemp3 = new PLN_SIGNIFIC_LETTER_DETAILS();

                for (int i = 0; i < 4; ++i)
                {

                    if (!string.IsNullOrEmpty(Request.Form["DMAN_DMAN_ID_" + i]))
                    {
                        objecttemp3.DMAN_DMAN_ID = int.Parse(Request.Form["DMAN_DMAN_ID_" + i]);
                        objecttemp3.ESIL_ESIL_ID = objecttemp2.ESIL_ID;
                        if (ESIL_ID != 0)
                        {
                            UpdateModel(objecttemp3);
                        }
                        else
                        {
                            db.PLN_SIGNIFIC_LETTER_DETAILS.Add(objecttemp3);
                        }


                        db.SaveChanges();
                    }

                }
                objecttemp.ESIL_ESIL_ID = objecttemp2.ESIL_ID;
                objecttemp.CPRO_CPLA_PLN_CODE = short.Parse(val[0]);
                objecttemp.CPRO_PRJ_CODE = short.Parse(val[1]);
                objecttemp.PRSP_PLACE = Request.Form["PRSP_PLACE"];
                objecttemp.PRSP_GPS = Request.Form["PRSP_GPS"];
                objecttemp.PRSP_POST_STAT = Request.Form["PRSP_POST_STAT"];
                objecttemp.PRSP_POST_TYPE = Request.Form["PRSP_POST_TYPE"];
                objecttemp.PRSP_FIRS_SORT = Request.Form["PRSP_FIRS_SORT"];
                objecttemp.PRSP_SECN_SORT = Request.Form["PRSP_SECN_SORT"];
                objecttemp.PRSP_CNTR = Request.Form["PRSP_CNTR"];
                objecttemp.PRSP_KHZN_BANK = Request.Form["PRSP_KHZN_BANK"];
                objecttemp.PRSP_KHZN_BANK_AMNT = Request.Form["PRSP_KHZN_BANK_AMNT"];
                objecttemp.PRSP_KHZN_VOLT = Request.Form["PRSP_KHZN_VOLT"];
                objecttemp.PRSP_REAC = Request.Form["PRSP_REAC"];
                objecttemp.PRSP_REAC_AMNT = Request.Form["PRSP_REAC_AMNT"];
                objecttemp.PRSP_REAC_VOLT = Request.Form["PRSP_REAC_VOLT"];
                objecttemp.PRSP_NETW_VIEW = Request.Form["PRSP_NETW_VIEW"];
                objecttemp.PRSP_MVA_AMNT = Request.Form["PRSP_MVA_AMNT"];
                objecttemp.PRSP_TERN_AMNT = Request.Form["PRSP_TERN_AMNT"];
                objecttemp.PRSP_CHNG_RELA = Request.Form["PRSP_CHNG_RELA"];
                objecttemp.PRSP_CHNG_VOL1 = short.Parse(Request.Form["PRSP_CHNG_VOL1"]);
                objecttemp.PRSP_CHNG_VOL2 = short.Parse(Request.Form["PRSP_CHNG_VOL2"]);
                objecttemp.PRSP_CHNG_VOL3 = short.Parse(Request.Form["PRSP_CHNG_VOL3"]);
                if (objecttemp2.ESIL_PROJ_TYPE == "2")
                {
                    objecttemp.EPOL_EPOL_ID = int.Parse(Request.Form["EPOL_EPOL_ID"]);
                    objecttemp.EPOL_EPOL_ID_R = int.Parse(Request.Form["EPOL_EPOL_ID_R"]);
                    objecttemp.PRSP_LINE_LENG = Request.Form["PRSP_LINE_LENG"];

                    objecttemp.PRSP_VOLT = Request.Form["PRSP_VOLT"];
                    objecttemp.PRSP_IMPL = Request.Form["PRSP_IMPL"];
                    objecttemp.PRSP_MAGH_TYPE = Request.Form["PRSP_MAGH_TYPE"];
                    objecttemp.PRSP_WIRE_COND = Request.Form["PRSP_WIRE_COND"];
                    objecttemp.PRSP_SHIL_WIRE = Request.Form["PRSP_SHIL_WIRE"];
                    objecttemp.PRSP_CIRC_AMNT = Request.Form["PRSP_CIRC_AMNT"];
                    objecttemp.PRSP_BAND_AMNT = Request.Form["PRSP_BAND_AMNT"];
                    objecttemp.PRSP_EART_WIRE = Request.Form["PRSP_EART_WIRE"];
                    objecttemp.PRSP_FIBR = Request.Form["PRSP_FIBR"];
                    objecttemp.PRSP_FIBR_PROP = Request.Form["PRSP_FIBR_PROP"];
                }



                foreach (var PRSP_FILE in PRSP_FILES)
                {
                    if (PRSP_FILE != null)
                    {
                        byte[] buffer = new byte[PRSP_FILE.ContentLength];
                        PRSP_FILE.InputStream.Read(buffer, 0, PRSP_FILE.ContentLength - 1);
                        var fileName = Path.GetFileName(PRSP_FILE.FileName);
                        objecttemp.PRSP_FILE = buffer;
                    }

                }
                if (ESIL_ID != 0)
                {
                    UpdateModel(objecttemp);
                }
                else
                {
                    db.PDF_PROJECT_SPECIFICATION.Add(objecttemp);
                }


                db.SaveChanges();

                try
                {

                    AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
                    if (ESIL_ID == 0)
                    {
                        wp.StartProcess(this.HttpContext.User.Identity.Name, new string[] { this.HttpContext.User.Identity.Name }, db.EXP_TYPE_DOC.Where(xx => xx.ETDO_ID == 103).Select(xx => xx.ETDO_DESC).FirstOrDefault(), "نامه ابلاغ با شماره   " + objecttemp2.ESIL_NO + "  ثبت شد ", 103, objecttemp2.ESIL_ID);
                    }
                    int not_id = wp.NoteId;

                }
                catch (Exception ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
                }
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("نامه ابلاغ با شماره   [{0}]  ثبت شد ", objecttemp2.ESIL_NO) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();


            }
            //return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
            // }
        }
        //public ActionResult Getpost_Dist(int? epol_id)
        //{
        //    var query = Db.Database.SqlQuery<pay_cis>("select * from pay_city")
        //                                                    .Select(x => new
        //                                                    {
        //                                                        x.EPOL_ID,
        //                                                        x.EPOL_NAME
        //                                                    }).ToList();
        //    //query = (from b in Db.EXP_POST_LINE
        //    //             join c in Db.SEC_USER_TYPE_POST on b.EPOL_ID equals c.EPOL_EPOL_ID
        //    //             orderby b.EPOL_NAME
        //    //             where b.EPOL_TYPE == "0" && b.EPOL_STAT == "1" && (b.EPOL_ID == epol_id || epol_id == null) && (b.EUNL_EUNL_ID == 63 || b.EUNL_EUNL_ID == 161 || b.EPOL_ID == 914)
        //    //             && c.SCSU_ROW_NO == userid && c.ETDO_ETDO_ID == 303 
        //    //             select new { b.EPOL_ID, EPOL_NAME = b.EPOL_NAME }).ToList();
        //    return Json(query, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult SendStartNode(int notId)
        {

            AsrWorkFlowProcess wp = new AsrWorkFlowProcess(notId);


            AsrJobProvider jp = new AsrJobProvider("CONFIRM_MODIR", "FLW_PROJ");

            return this.Json(new { Success = true, data = jp.AllUsers }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SendStartNodeModir(int notId, string NextStat)
        {

            AsrWorkFlowProcess wp = new AsrWorkFlowProcess(notId);


            AsrJobProvider jp = new AsrJobProvider(NextStat, "FLW_PROJ");

            return this.Json(new { Success = true, data = jp.AllUsers }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InsertPrjGrant(PLN_SIGNIFIC_LETER ObjectTemp)
        {
            try
            {
                int ESIL_ID = 0;
                if (!string.IsNullOrEmpty(Request.Form["ESIL_ID"]))
                {


                    ESIL_ID = int.Parse(Request.Form["ESIL_ID"]);


                }
                string SqlUpdate = string.Format("update PLN_SIGNIFIC_LETER set ESIL_FGRANT='{0}' , ESIL_GRANT ='{1}' where ESIL_ID={2}", ObjectTemp.ESIL_FGRANT, ObjectTemp.ESIL_GRANT, ESIL_ID);
                db.Database.ExecuteSqlCommand(SqlUpdate);
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();


            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات" + "</br>" + ex.ToString() }.ToJson();
            }



        }
        public ActionResult InsertNewProject(CGT_PRO ObjectTemp)
        {
            try
            {
                ObjectTemp.SPRJ_YEAR = Request.Form["SPRJ_DATE"].Substring(0, 4);
                ObjectTemp.SPRJ_MONT = Request.Form["SPRJ_DATE"].Substring(5, 2);
                ObjectTemp.SPRJ_DAY = Request.Form["SPRJ_DATE"].Substring(8, 2);
                ObjectTemp.EPRJ_YEAR = Request.Form["EPRJ_DATE"].Substring(0, 4);
                ObjectTemp.EPRJ_MONT = Request.Form["EPRJ_DATE"].Substring(5, 2);
                ObjectTemp.EPRJ_DAY = Request.Form["EPRJ_DATE"].Substring(8, 2);
                ObjectTemp.PRJ_CODE = (short)(db.CGT_PRO.Where(xx => xx.CPLA_PLN_CODE == ObjectTemp.CPLA_PLN_CODE).Select(xx => xx.PRJ_CODE).Max() + 1);
                ObjectTemp.PRJ_STAT = "1";
                ObjectTemp.SAVE_STAT = "1";
                ObjectTemp.CRET_BY = this.UserInfo().Username.ToUpper();
                ObjectTemp.CRET_DATE = DateTime.Now;
                db.CGT_PRO.Add(ObjectTemp);

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات" }.ToJson();
            }

            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();

        }
        [HttpPost]
        public ActionResult ReadProjects([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from b in db.PLN_SIGNIFIC_LETER
                         orderby b.ESIL_ID descending
                         select new
                         {
                             b.ESIL_ID,
                             b.ESIL_NO,
                             b.ESIL_TYPE,
                             b.ESIL_GRANT,
                             b.ESIL_FGRANT,
                             PLN_DESC = b.CGT_PRO.CGT_PLAN.PLN_DESC,
                             PRJ_DESC = b.CGT_PRO.PRJ_DESC,
                             b.ESIL_DATE,
                             b.ESIL_PROJ_TYPE
                         }).ToList().AsEnumerable()
                         ;
            var FinalQuery = query.Select(b => new
            {
                b.ESIL_ID,
                b.ESIL_NO,
                b.ESIL_TYPE,
                b.ESIL_GRANT,
                b.ESIL_FGRANT,
                b.PLN_DESC,
                b.PRJ_DESC,
                b.ESIL_DATE,
                b.ESIL_PROJ_TYPE,
                Not_id = return_noteid(b.ESIL_ID.ToString(), "FLW_PROJ.FLW_PROJ")
            }).ToList();
            return Json(FinalQuery.ToDataSourceResult(request));
        }
        public int return_noteid(string id, string Flow)
        {

            string sql = "SELECT WF_NOTE_V.NOT_ID as m FROM WF_NOTE_V where stat='OPEN' and MESSAGE_NAME='CREATOR' and upper(RECIPIENT_ROLE)='" + this.User.Identity.Name.ToUpper() + "' and  WF_NOTE_V.ITEM_KEY='" + Flow + "^" + id + "'";
            //and stat='OPEN'

            int not_id = db.Database.SqlQuery<int>(sql).FirstOrDefault();
            return not_id;
        }
        public int GetNotId(int ESIL_ID)
        {
            int notId = db.Database.SqlQuery<int>("SELECT WF_NOTE_V.NOT_ID as m FROM WF_NOTE_V where stat='OPEN' and upper(RECIPIENT_ROLE)='" + this.User.Identity.Name.ToUpper() + "' and  WF_NOTE_V.ITEM_KEY='FLW_PROJ.FLW_PROJ^" + ESIL_ID + "'").FirstOrDefault();
            return notId;


        }
        public ActionResult RemoveProject(EXP_POST_LINE model)
        {
            //var db = new BandarEntities();
            //var myobj = db.EXP_POST_LINE.FirstOrDefault(b => b.EPOL_ID == model.EPOL_ID);
            //db.EXP_POST_LINE.Remove(myobj);
            //db.SaveChanges();         
            return View("Index");
        }

        //---------------------------------------------------------------------------add by shahamiri
        public ActionResult StandardSp()
        {
            return View("");
        }

        public ActionResult StandardSpRead([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from b in db.PDF_STANDARD_SPECIFICATION.AsEnumerable() orderby b.STSP_CODE, b.PRTP_PRTP_ROW select b)
                         //where (b.PRTP_PRTP_ROW == 1)
                         .AsEnumerable().Select(p => new
                         {
                             p.STSP_CODE,
                             p.STSP_DESC,
                             p.PRTP_PRTP_ROW,
                             p.DMAN_DMAN_ID,
                             p.STSP_TYPE,
                             p.STSP_OPTI
                         });
            var data = new DataSourceResult
            {
                Data = query.ToList()
            };
            return Json(data);
        }

        public ActionResult insertStandardSpe(short? id)
        {
            try
            {
                //FillViewBag();
                if (id == null)
                {
                    return View();
                    //.IfUserCanAccess(HttpContext);
                }
                else
                {
                    short key = id.GetValueOrDefault();
                    var model = db.PDF_STANDARD_SPECIFICATION.FirstOrDefault(x => x.STSP_CODE == key);
                    return View(model);
                    //.IfUserCanAccess(HttpContext);
                }
            }
            catch (Exception ex)
            {
                HttpContext.AddError(ex);
                return View();
            }
        }
        public ActionResult GetUnitLevel()
        {
            var query = (from b in db.EXP_UNIT_LEVEL

                         orderby b.EUNL_NUM descending
                         where b.ACTV_TYPE == "1"
                         select new { b.EUNL_ID, b.EUNL_DESC, b.EUNL_NUM }).Distinct().OrderByDescending(b => b.EUNL_NUM);

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetInstrument(int? EINS_ID, int? ESIN_ID)
        {
            var query = (from b in db.EXP_INSTRUMENT

                         orderby b.EINS_DESC descending
                         where b.EINS_EINS_ID == 1 && b.ESIN_ESIN_ID == 1
                         select new { b.EINS_ID, b.EINS_DESC }).Distinct().OrderByDescending(b => b.EINS_DESC);

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPlanSignific([DataSourceRequest] DataSourceRequest request, int? ESIL_ESIL_ID)
        {
            var Query = (from a in db.PLN_SIGNIFIC_PLAN

                         where a.ESIL_ESIL_ID == ESIL_ESIL_ID

                         select new
                         {
                             a.SIPL_ID,
                             a.SIPL_TITLE
                         }).ToList().Distinct();



            return Json(Query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult UsersPlanGridRead([DataSourceRequest] DataSourceRequest request, int? SIPL_ID)
        {
            var query = (from p in db.PLN_SIGNIFIC_PLAN_USER
                         join d in db.SEC_USERS on p.SCSU_ROW_NO equals d.ROW_NO
                         where (p.SIPL_SIPL_ID == SIPL_ID)

                         select new
                         {
                             p.SIPU_ID,
                             FAML_NAME = p.SEC_USERS.PAY_PERSONEL.FAML_NAME != null ? p.SEC_USERS.PAY_PERSONEL.FAML_NAME : p.SEC_USERS.FAML_NAME,
                             FIRS_NAME = p.SEC_USERS.PAY_PERSONEL.FIRS_NAME != null ? p.SEC_USERS.PAY_PERSONEL.FIRS_NAME : p.SEC_USERS.FIRS_NAME,

                             USER_NAME = d.USER_NAME
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult UsersPlanGridReadChecklist([DataSourceRequest] DataSourceRequest request, int? SIPU_ID, int? SIPL_ID)
        {
            var query = (from p in db.PLN_SIGNIFIC_PLAN_CHECK_ITEM

                         where (p.SIPU_SIPU_ID == SIPU_ID && (p.SIPL_SIPL_ID == SIPL_ID || SIPL_ID == null))

                         select new
                         {
                             p.SPCI_ID,
                             Checklist = p.CHK_CHECK_LIST_TEMPLATE.CHLT_TITL,
                             ChecklistItem = p.CHK_ITEM_TEMPLATE.CHTI_DESC,
                             p.SPCI_ANSW,
                             p.SIPL_SIPL_ID,
                             p.SIPU_SIPU_ID,
                             p.CHLT_CHLT_ID,
                             p.CHTI_CHTI_ID,
                             p.SPCI_DESC
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }


        public ActionResult GetShineType()
        {
            var query = (from b in db.EXP_SHINE_TYPE
                         orderby b.ESHT_ID descending
                         where b.ACTV_TYPE == "1"
                         select new
                         {
                             b.ESHT_ID,
                             b.ESHT_DESC
                         }).ToList().OrderBy(xx => xx.ESHT_DESC);
            //var query = (from b in Db.EXP_POST_LINE
            //             join c in Db.SEC_USER_TYPE_POST on b.EPOL_ID equals c.EPOL_EPOL_ID
            //             where b.EPOL_TYPE == "0" && b.EPOL_STAT == "1" && (b.EPOL_ID == epol_id || epol_id == null)
            //                   && c.SCSU_ROW_NO == userid && c.ETDO_ETDO_ID == 303
            //             orderby b.EPOL_NAME
            //             select new { b.EPOL_ID, EPOL_NAME = b.EPOL_NAME }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        //-------------------------------insert & update---------------------------------------------
        [HttpPost]
        public ActionResult SaveNewStandardSpe(PDF_STANDARD_SPECIFICATION obj)
        {
            try
            {
                if (PublicRepository.ExistModel("PDF_STANDARD_SPECIFICATION", "STSP_DESC='{0}'", obj.STSP_DESC))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}] تکراری است.", obj.STSP_DESC) }.ToJson();
                }
                using (BandarEntities db = GlobalConst.DB())
                {
                    db.PDF_STANDARD_SPECIFICATION.Add(obj);
                    db.SaveChanges();
                }
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد", obj.STSP_DESC) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}] ثیت نشد[{1}].", obj.STSP_DESC, ex.PersianMessage()) }.ToJson();
            }
        }


        public ActionResult UpdateStandardSpe(PDF_STANDARD_SPECIFICATION obj)
        {
            try
            {
                if (obj == null)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "ویرایش انجام نشد" }.ToJson();
                }

                if (PublicRepository.ExistModel("PDF_STANDARD_SPECIFICATION", "STSP_DESC='{0}' AND STSP_CODE<>{1}", obj.STSP_DESC, obj.STSP_CODE))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}] تکراری است.", obj.STSP_DESC) }.ToJson();
                }

                var cntx = PublicRepository.GetNewDatabaseContext;
                var original = cntx.PDF_STANDARD_SPECIFICATION.Find(obj.STSP_CODE);
                if (original != null)
                {
                    original.STSP_DESC = obj.STSP_DESC;
                    original.PRTP_PRTP_ROW = obj.PRTP_PRTP_ROW;
                    original.DMAN_DMAN_ID = obj.DMAN_DMAN_ID;
                    original.STSP_OPTI = obj.STSP_OPTI;
                    original.STSP_TYPE = obj.STSP_TYPE;
                    cntx.SaveChanges();
                    return new ServerMessages(ServerOprationType.Success) { Message = "ویرایش شد" }.ToJson();
                }
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "ویرایش انجام نشد" }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}] ویرایش نشد[{1}].", obj.STSP_DESC, ex.PersianMessage()) }.ToJson();
            }
        }

        public ActionResult UpdatePlanGridReadChecklist([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PLN_SIGNIFIC_PLAN_CHECK_ITEM> PLN_SIGNIFIC_PLAN_CHECK_ITEM)
        {
            if (PLN_SIGNIFIC_PLAN_CHECK_ITEM != null)
            {
                foreach (PLN_SIGNIFIC_PLAN_CHECK_ITEM item in PLN_SIGNIFIC_PLAN_CHECK_ITEM)
                {
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(PLN_SIGNIFIC_PLAN_CHECK_ITEM.ToDataSourceResult(request, ModelState));
        }

        //-------------------------------insert&update----------------------------------------------



        //------------------------------------------------------------------------------add by shahamiri
    }
}

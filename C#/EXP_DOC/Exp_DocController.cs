using Asr.Base;
using Asr.Cartable;
using Asr.Security;
using Equipment.Codes.Security;
using Equipment.DAL;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers
{
    [Authorize]
    [Developer("E.Bahmani"), Developer("M.Eizedi")]
    public class Exp_DocController : DbController
    {
        //BandarEntities Db;

        ////سازنده کلاس
        //public Exp_DocController()
        //{
        //    Db = GlobalConst.DB();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        Db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        public ActionResult InsertRequest(string userName, decimal eedo_id, int epol_id, decimal efun_id)
        {
            try
            {
                var query = from b in Db.EXP_EXPI_DOC where b.ETDO_ETDO_ID == 21 select b;
                string max = query.Max(c => c.DOC_NUMB);
                System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
                EXP_EXPI_DOC exp_doc = new EXP_EXPI_DOC();
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

                string hh = "00";
                if (DateTime.Now.Hour < 10)
                {
                    hh = "0" + DateTime.Now.Hour.ToString();
                }
                else
                {
                    hh = DateTime.Now.Hour.ToString();
                }
                string ss = "00";
                if (DateTime.Now.Minute < 10)
                {
                    ss = "0" + DateTime.Now.Minute.ToString();
                }
                else
                {
                    ss = DateTime.Now.Minute.ToString();
                }

                exp_doc.EEDO_DATE = pc.GetYear(DateTime.Now).ToString() + "/" + MOUNT + "/" + day;
                exp_doc.EEDO_TIME = hh + ":" + ss;
                //  exp_doc.DOC_NUMB = (Convert.ToInt32(max) + 1).ToString();
                exp_doc.EPOL_EPOL_ID = epol_id;
                exp_doc.EFUN_EFUN_ID = efun_id;
                exp_doc.ETDO_ETDO_ID = 21;
                string qmax = string.Empty;
                int s1 = 0;
                string qdocnum = string.Empty;
                int m = 0;
                var q = (from b in Db.EXP_EXPI_DOC where b.ETDO_ETDO_ID == 21 && b.EANA_EANA_ROW == null select b).FirstOrDefault();
                ;
                if (q != null)
                {
                    qmax = (from b in Db.EXP_EXPI_DOC where b.ETDO_ETDO_ID == 21 && b.EANA_EANA_ROW == null select b).Max(c => c.EEDO_ID).ToString();
                    s1 = Convert.ToInt32(qmax);
                    qdocnum = (from b in Db.EXP_EXPI_DOC where b.EEDO_ID == s1 select b.DOC_NUMB).FirstOrDefault().ToString();

                    m = Convert.ToInt16(qdocnum) + 1;
                }
                else
                {
                    m = 1;
                }

                exp_doc.DOC_NUMB = m.ToString();

                Db.EXP_EXPI_DOC.Add(exp_doc);
                Db.SaveChanges();
                string Qjdti = (from b in Db.SEC_JOB_TYPE_DOC where b.ETDO_ETDO_ID == 21 && b.ROWI_ORDE == 1 select b.JDTY_ID).FirstOrDefault().ToString();
                decimal QCURENT = Convert.ToDecimal(Qjdti);

                var rel1 = (from b in Db.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == eedo_id select b).FirstOrDefault();
                if (rel1 != null)
                {
                    var rel2 = new EXP_EDOC_INSTRU();
                    rel2.EEDO_EEDO_ID = exp_doc.EEDO_ID;
                    rel2.EPIU_EPIU_ID = rel1.EPIU_EPIU_ID;
                    rel2.EPOL_EPOL_ID = rel1.EPOL_EPOL_ID;
                    rel2.JDTY_JDTY_ID = Convert.ToDecimal(QCURENT.ToString());
                    rel2.ETDO_ETDO_ID = 21;
                    rel2.CUT_STAT = "1";
                    Db.EXP_EDOC_INSTRU.Add(rel2);
                    Db.SaveChanges();
                }

                EXP_RELATION_DOC relationDoc = new EXP_RELATION_DOC();
                relationDoc.EEDO_EEDO_ID = exp_doc.EEDO_ID;
                relationDoc.EEDO_EEDO_ID_R = eedo_id;
                Db.EXP_RELATION_DOC.Add(relationDoc);
                Db.SaveChanges();

                string postn = (from b in Db.EXP_POST_LINE where b.EPOL_ID == epol_id select b.EPOL_NAME).FirstOrDefault().ToString();
                string doc_name = (from b in Db.EXP_TYPE_DOC where b.ETDO_ID == 21 select b.ETDO_DESC).FirstOrDefault();
                string smessage = " درخواست " + "به شماره" + exp_doc.DOC_NUMB + " در تاریخ " + exp_doc.EEDO_DATE + "  مربوط به دیفکت " + eedo_id + " پست " + postn + " میباشد ";

                AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
                wp.StartProcess(userName, new string[] { userName }, doc_name, smessage, 21, exp_doc.EEDO_ID);

                return new ServerMessages(ServerOprationType.Success) { Message = "ثبت شد.", CoustomData = exp_doc.EEDO_ID.ToString() }.ToJson();
            }
            catch
            {
                return this.Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult checkCurrentDate(string date)
        {
            try
            {
                Codes.Globalization.PersianDateTime dateTime = new Codes.Globalization.PersianDateTime();
                DateTime miladiDate = dateTime.GetMiladiDateTime(date);
                DateTime currentDate = DateTime.Now;
                if (miladiDate > currentDate)
                {
                    return this.Json(new { Success = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return this.Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return this.Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult checkElamDate(string elamDate, string elamExecuteDate)
        {
            try
            {
                Codes.Globalization.PersianDateTime dateTime = new Codes.Globalization.PersianDateTime();
                DateTime miladiDate1 = dateTime.GetMiladiDateTime(elamDate);
                DateTime miladiDate2 = dateTime.GetMiladiDateTime(elamExecuteDate);
                if (miladiDate2 < miladiDate1)
                {
                    return this.Json(new { Success = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return this.Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return this.Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult checkMorajeDate(string morajeDate, string elamExecuteDateInput, string elamExecuteDateDisplay)
        {
            try
            {
                string elamDate = string.Empty;
                if (elamExecuteDateInput != null)
                {
                    elamDate = elamExecuteDateInput;
                }
                else if (elamExecuteDateDisplay != null)
                {
                    elamDate = elamExecuteDateDisplay;
                }

                Codes.Globalization.PersianDateTime dateTime = new Codes.Globalization.PersianDateTime();
                DateTime miladiDate1 = dateTime.GetMiladiDateTime(morajeDate);
                DateTime miladiDate2 = dateTime.GetMiladiDateTime(elamDate);
                if (miladiDate2 > miladiDate1)
                {
                    return this.Json(new { Success = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return this.Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return this.Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult checkRafDate(string morajeDate, string rafDate)
        {
            try
            {
                Codes.Globalization.PersianDateTime dateTime = new Codes.Globalization.PersianDateTime();
                DateTime miladiDate1 = dateTime.GetMiladiDateTime(morajeDate);
                DateTime miladiDate2 = dateTime.GetMiladiDateTime(rafDate);
                if (miladiDate2 < miladiDate1)
                {
                    return this.Json(new { Success = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return this.Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return this.Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Exp_Doc()
        {
            return View("Exp_Doc_F");
        }

        public ActionResult Exp_Edoc_Instr(int? id)
        {
            ViewData["id"] = id;
            return View("Exp_Edoc_Instr_F");
        }

        public ActionResult Get_Type_Doc([DataSourceRequest] DataSourceRequest request)
        {
            var query = from dman in Db.EXP_TYPE_DOC orderby dman.ETDO_ID select dman;
            var d = new DataSourceResult
            {
                Data = query.Select(p => new { p.ETDO_ID, p.ENTY_KEY, p.ETDO_DESC, p.ACTV_TYPE }),
            };
            return Json(d);
        }

        public ActionResult Insert_Type_Doc(Equipment.Models.EXP_TYPE_DOC objecttemp)
        {
            objecttemp.ACTV_TYPE = Request.Form["TYPE"];
            Db.EXP_TYPE_DOC.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Update_Type_Doc([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<Equipment.Models.EXP_TYPE_DOC> type_doc)
        {
            if (type_doc != null)
            {
                foreach (Equipment.Models.EXP_TYPE_DOC typedoc in type_doc)
                {
                    Db.Entry(typedoc).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(type_doc.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Exp_Expi_Update_Doc_F(string id, string ETDO_ID)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                Session["eedo_id"] = id;
                Session["etdo_id"] = ETDO_ID;
            }
            ViewData["post_line"] = Db.EXP_POST_LINE.Select(o => new { o.EPOL_ID, o.EPOL_NAME }).AsEnumerable();
            ViewData["project"] = Db.CGT_PRO
                                    .Where(o => o.CPRO_PRJ_CODE == Db.EXP_EXPI_DOC.Select(p => p.CPRO_PRJ_CODE).FirstOrDefault())
                                    .Where(o => o.CPLA_PLN_CODE == Db.EXP_EXPI_DOC.Select(p => p.CPRO_CPLA_PLN_CODE).FirstOrDefault())
                                    .Select(o => new { o.PRJ_CODE, o.PRJ_DESC })
                                    .AsEnumerable();
            ViewData["Organ"] = Db.PAY_ORGAN.Select(o => new { o.CODE, o.ORGA_DESC }).AsEnumerable();
            //ViewData["Type_Doc"] = cntx.EXP_TYPE_DOC.Select(o => new { o.ETDO_ID, o.ETDO_DESC }).Where(o => o.ETDO_ID == (cntx.EXP_TYPE_DOC.Select(o => new { o.ETDO_ID }))).AsEnumerable();
            ViewData["Type_Doc"] = Db.EXP_EXPI_DOC.Select(o => new { o.EEDO_ID, o.EXP_TYPE_DOC, o.ETDO_ETDO_ID, o.EXP_TYPE_DOC.ETDO_DESC }).Where(o => o.ETDO_ETDO_ID == o.EXP_TYPE_DOC.ETDO_ID).AsEnumerable();
            ViewData["Doc_val"] = Db.EXP_EITEM_DOC_VALUE.Select(o => new { o.EIDR_ID, o.EIDR_VALUE });
            ViewData["post_instr"] = Db.EXP_POST_LINE_INSTRU.Select(o => new { o.EINS_EINS_ID, o.CODE_DISP });
            ViewBag.exp_doc = Db.EXP_EXPI_DOC;
            return View("Exp_Expi_Update_Doc_F");
        }

        //Document Item 
        public ActionResult Exp_Item(string id)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                Session["fid"] = id;
            }
            return View("Exp_Item_F");
        }

        public ActionResult Insert_Item_Doc(EXP_ITEM_TYPE_DOC objecttemp)
        {
            if (string.IsNullOrEmpty(Request.Form["EITY_DESC"]))
            {
                objecttemp.EITY_DESC = Request.Form["DESC"];
            }

            objecttemp.ACTV_TYPE = Request.Form["ACTV"];
            objecttemp.OPTIO_MANE = Request.Form["OPTI_MANE"];
            objecttemp.EITY_TYPE = Request.Form["TYPE"];

            if ((objecttemp.EITY_TYPE == "170") || ((objecttemp.EITY_TYPE == "181")))
            {
                var dman = new CHK_DOMAIN();
                dman.ACTV_TYPE = "0";
                dman.DMAN_TYPE = "0";
                dman.DMAN_TITL = objecttemp.EITY_DESC;
                Db.CHK_DOMAIN.Add(dman);
                int dman_id = dman.DMAN_ID;
                Db.SaveChanges();
                objecttemp.DMAN_DMAN_ID = dman.DMAN_ID;
                Db.EXP_ITEM_TYPE_DOC.Add(objecttemp);
                Db.SaveChanges();
            }
            else
            {
                Db.EXP_ITEM_TYPE_DOC.Add(objecttemp);
                Db.SaveChanges();
            }

            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Get_Item_Doc([DataSourceRequest] DataSourceRequest request, int ETDO_ETDO_ID)
        {
            var query = from q in Db.EXP_ITEM_TYPE_DOC orderby q.EITY_ORDE where q.ETDO_ETDO_ID == ETDO_ETDO_ID select q;
            var d = new DataSourceResult
            {
                Data = query.Select(p => new
                {
                    p.EITY_ID,
                    p.EITY_DESC,
                    p.ACTV_TYPE,
                    p.EITY_LENT,
                    p.EITY_ORDE,
                    p.OPTIO_MANE,
                    p.EITY_TYPE,
                    p.DMAN_DMAN_ID,
                    p.ETDO_ETDO_ID
                }),
            };
            return Json(d);
        }

        public ActionResult Update_Item_Doc([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<EXP_ITEM_TYPE_DOC> itemtype_doc)
        {
            if (itemtype_doc != null)
            {
                foreach (EXP_ITEM_TYPE_DOC itemtypedoc in itemtype_doc)
                {
                    Db.Entry(itemtypedoc).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(itemtype_doc.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update_Edoc_Instru([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<EXP_EDOC_INSTRU> itemtype_doc)
        {
            if (itemtype_doc != null)
            {
                foreach (EXP_EDOC_INSTRU itemtypedoc in itemtype_doc)
                {
                    Db.Entry(itemtypedoc).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }
            return Json(itemtype_doc.ToDataSourceResult(request, ModelState));
        }

        ///////////////////////////////EXPI Relation
        public ActionResult Exp_Rel_Doc(string id)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                Session["id"] = id;
            }
            ViewData["Rel_Doc"] = Db.EXP_EXPI_DOC.Select(o => new { o.EEDO_ID, o.EEDO_DESC }).AsEnumerable();

            return View("Exp_Rel_Doc_F");
        }

        public ActionResult insert_Rel_doc(EXP_RELATION_DOC objecttemp)
        {
            var val = Request.Form["EEDO_EEDO_ID_R"].Split(',');

            int EEDO_EEDO_ID = Convert.ToInt16(Request.Form["eedo_id"]);
            objecttemp.EEDO_EEDO_ID = EEDO_EEDO_ID;
            foreach (var v in val)
            {
                int EEDO_EEDO_ID_R = int.Parse(v);
                objecttemp.EEDO_EEDO_ID_R = EEDO_EEDO_ID_R;
                var query = from q in Db.EXP_RELATION_DOC
                            where (q.EEDO_EEDO_ID == EEDO_EEDO_ID && q.EEDO_EEDO_ID_R == EEDO_EEDO_ID_R)
                            select q;
                if (!query.Any())
                {
                    Db.EXP_RELATION_DOC.Add(objecttemp);
                    Db.SaveChanges();
                }
            }

            // cntx.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Get_Rel_Doc([DataSourceRequest] DataSourceRequest request, int id)
        {
            var query = from q in Db.EXP_RELATION_DOC where q.EEDO_EEDO_ID == id select q;
            var d = new DataSourceResult
            {
                Data = query.Select(p => new { p.ERED_ID, p.EEDO_EEDO_ID, p.EEDO_EEDO_ID_R }),
            };
            return Json(d);
        }

        public ActionResult get_rel_doc_DP(int id)
        {
            var RetVal = (from b in PublicRepository.Get_Doc() where (b.ETDO_ETDO_ID == id) orderby b.DOC_NUMB select b).AsEnumerable().Select(b => new { b.EEDO_ID, b.DOC_NUMB });
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        ///////////////////////////////EXPI DOC
        [MenuAuthorize]
        public ActionResult Exp_Expi_Doc(string etdo_id, string id)
        {
            // var i=this.HttpContext.User.Identity.Name.DecryptFromAes("dslihgfkwjhg52398842309478@345987rtdsklfjhsd");
            //  FormsAuthentication.SetAuthCookie(;
            // FormsAuthentication.
            Session["ETDO_ID"] = etdo_id;
            Session["id"] = id;
            Session["eedo_id"] = null;
            ViewData["post_line"] = Db.EXP_POST_LINE.Select(o => new { o.EPOL_ID, o.EPOL_NAME }).AsEnumerable();
            ViewData["project"] = Db.CGT_PRO
                                    .Where(o => o.CPRO_PRJ_CODE == Db.EXP_EXPI_DOC.Select(p => p.CPRO_PRJ_CODE).FirstOrDefault())
                                    .Where(o => o.CPLA_PLN_CODE == Db.EXP_EXPI_DOC.Select(p => p.CPRO_CPLA_PLN_CODE).FirstOrDefault())
                                    .Select(o => new { o.PRJ_CODE, o.PRJ_DESC })
                                    .AsEnumerable();
            ViewData["Organ"] = Db.PAY_ORGAN.Select(o => new { o.CODE, o.ORGA_DESC }).AsEnumerable();
            //ViewData["Type_Doc"] = cntx.EXP_TYPE_DOC.Select(o => new { o.ETDO_ID, o.ETDO_DESC }).Where(o => o.ETDO_ID == (cntx.EXP_TYPE_DOC.Select(o => new { o.ETDO_ID }))).AsEnumerable();
            ViewData["Type_Doc"] = Db.EXP_EXPI_DOC.Select(o => new { o.EEDO_ID, o.EXP_TYPE_DOC, o.ETDO_ETDO_ID, o.EXP_TYPE_DOC.ETDO_DESC }).Where(o => o.ETDO_ETDO_ID == o.EXP_TYPE_DOC.ETDO_ID).AsEnumerable();
            ViewData["Doc_val"] = Db.EXP_EITEM_DOC_VALUE.Select(o => new { o.EIDR_ID, o.EIDR_VALUE });
            ViewData["post_instr"] = Db.EXP_POST_LINE_INSTRU.Select(o => new { o.EINS_EINS_ID, o.CODE_DISP });

            ViewBag.Instrument = Db.EXP_INSTRUMENT.Where(c => c.EINS_EINS_ID != null).Select(c => new { c.EINS_ID, c.EINS_DESC });
            ViewBag.Bay = Db.EXP_TYPE_BAY.Select(c => new { c.ETBY_ID, c.ETBY_DESC });
            ViewBag.UniteV = Db.EXP_UNIT_LEVEL.Select(c => new { c.EUNL_ID, c.EUNL_DESC });
            ViewBag.post = Db.EXP_POST_LINE.Select(c => new { c.EPOL_ID, c.EPOL_NAME });
            ViewBag.PostInstrument = Db.EXP_POST_LINE_INSTRU.Select(c => new { c.EPIU_ID, c.CODE_NAME });
            ViewBag.Offstat = Db.EXP_OFF_STAT.Select(c => new { c.EOFS_ID, c.EOFS_DESC });
            ViewBag.PFUNCTION = Db.EXP_PFUNCTION.Select(c => new { c.EFUN_ID, c.EFUN_DESC });

            ViewBag.exp_doc = Db.EXP_EXPI_DOC;
            return View("Exp_Expi_Doc_F");
        }

        [HttpPost]
        public string insert_exp(EXP_EXPI_DOC objecttemp)
        {
            if (!PublicRepository.ExistModel("EXP_EXPI_DOC", "DEL_DUMP_U(DOC_NUMB) = DEL_DUMP_U('{0}') and ETDO_ETDO_ID = '{1}'", objecttemp.DOC_NUMB, objecttemp.ETDO_ETDO_ID))
            {
                EXP_EXPI_DOC exp_doc = new EXP_EXPI_DOC();
                exp_doc.DOC_NUMB = objecttemp.DOC_NUMB;
                exp_doc.ETDO_ETDO_ID = objecttemp.ETDO_ETDO_ID;
                Db.EXP_EXPI_DOC.Add(objecttemp);
                //cntx.SaveChanges();
                Session["EEDO_ID"] = objecttemp.EEDO_ID;
                return (objecttemp.EEDO_ID.ToString());
            }
            else
            {
                return ("0");
            }
        }

        [HttpPost]
        public string insert_Edoc(EXP_EDOC_INSTRU objecttemp, string seedo_id)
        {
            string sql = string.Format("DELETE FROM EXP_EDOC_INSTRU WHERE EEDO_EEDO_ID={0} and EDIN_EDIN_ID is not null", objecttemp.EEDO_EEDO_ID);
            Db.Database.ExecuteSqlCommand(sql);
            EXP_EDOC_INSTRU exp_doc = new EXP_EDOC_INSTRU();
            int eedo_id = Convert.ToInt32(seedo_id);
            var querys = from b in Db.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == eedo_id select b;
            foreach (EXP_EDOC_INSTRU query in querys)
            {
                exp_doc.EEDO_EEDO_ID = objecttemp.EEDO_EEDO_ID;
                exp_doc.EDIN_EDIN_ID = query.EDIN_ID;
                exp_doc.ON_TIME = query.ON_TIME;
                exp_doc.ON_DATE = query.ON_DATE;
                exp_doc.OFF_TIME = query.OFF_TIME;
                exp_doc.OFF_DATE = query.OFF_DATE;

                Db.EXP_EDOC_INSTRU.Add(exp_doc);
                Db.SaveChanges();
            }

            return (eedo_id.ToString());
        }

        public ActionResult Insert_Edoc_Instr(EXP_EDOC_INSTRU objecttemp)
        {
            objecttemp.OFF_DATE = Request.Form["OFF_DATE"];
            objecttemp.EEDO_EEDO_ID = Convert.ToInt32(Request.Form["EEDO_EEDO_ID"]);

            Db.EXP_EDOC_INSTRU.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Insert_Instr_Tree(EXP_EDOC_INSTRU objecttemp)
        {
            int row = Convert.ToInt32(Request.Form["row"]);
            objecttemp.EEDO_EEDO_ID = Convert.ToInt32(Request.Form["eedo_id"]);
            for (int i = 0; i <= row; i++)
            {
                string value = Request.Form[i.ToString()];
                if (!string.IsNullOrEmpty(value))
                {
                    objecttemp.EINS_EINS_ID = Convert.ToInt32(value);
                    Db.EXP_EDOC_INSTRU.Add(objecttemp);
                    Db.SaveChanges();
                }
            }
            //objecttemp.OFF_DATE = Request.Form["OFF_DATE"];
            //objecttemp.EEDO_EEDO_ID = Convert.ToInt32(Request.Form["EEDO_EEDO_ID"]);
            //cntx.EXP_EDOC_INSTRU.Add(objecttemp);
            //cntx.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        public string confirm_exp(int? eedo_id)
        {
            //string postn 
            //var q  = (from b in cntx.EXP_POST_LINE join x in cntx.EXP_EXPI_DOC on b.EPOL_ID equals x.EPOL_EPOL_ID
            //          where x.EEDO_ID == eedo_id
            //          select new {b.EPOL_NAME,x.DOC_NUMB,x.DEFC_DESC,x.EEDO_DATE});//.FirstOrDefault().ToString();
            //string instn = (from b in cntx.EXP_POST_LINE_INSTRU  join p in cntx.EXP_EDOC_INSTRU on b.EPIU_ID  equals p.EPIU_EPIU_ID
            //                where p.EEDO_EEDO_ID == eedo_id
            //                select b.CODE_NAME).FirstOrDefault().ToString();
            //string post = q.FirstOrDefault().EPOL_NAME.ToString();
            //string defcdesc = q.FirstOrDefault().DEFC_DESC.ToString();
            //string date = q.FirstOrDefault().EEDO_DATE.ToString();
            //string doc = q.FirstOrDefault().DOC_NUMB.ToString();
            //string smessage = " دیفکت " + defcdesc + "به شماره" + doc + " در تاریخ " + date + " مربوط به پست" + post + "  و تجهیز " + instn;
            //string s = smessage + "ارسال شد";
            //AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
            //wp.StartProcess(this.HttpContext.User.Identity.Name, new string[] { }, "دیفکت", smessage, 2, eedo_id);
            //return (s);    
            return (null);
        }

        public ActionResult Insert_Expi_Doc(EXP_EXPI_DOC objecttemp)
        {
            string check = Request.Form["eedoId"];
            int etdo_id = 0;
            string time = string.Empty;
            if (!String.IsNullOrEmpty(Request.Form["EXP_EXPI_DOC.ETDO_ETDO_ID"]))
                etdo_id = Convert.ToInt32(Request.Form["EXP_EXPI_DOC.ETDO_ETDO_ID"]);

            string on_TIME = string.Empty;
            string off_TIME = string.Empty;
            string Qjdti = (from b in Db.SEC_JOB_TYPE_DOC where b.ETDO_ETDO_ID == etdo_id && b.ROWI_ORDE == 1 select b.JDTY_ID).FirstOrDefault().ToString();
            decimal QCURENT = Convert.ToDecimal(Qjdti);

            if (etdo_id == 21)
            {
                if ((Request.Form["EXP_EXPI_DOC.EEDO_DATE"] == "" || Request.Form["function"] == "" || Request.Form["EPOL_EPOL_ID_requ"] == "" ||
                     Request.Form["ORGA_CODE_request"] == "" ||
                     ((Request.Form["POSTINST"] == "" && Request.Form["etby1"] == "" && Request.Form["einsrequest"] == "" && Request.Form["typeinstREQUEST"] != "0") ||
                      (Request.Form["typeinstREQUEST"] == "0" && Request.Form["EPOL_EPOL_ID_requ"] == "")) ||
                     Request.Form["ofDATEreq"] == "" || Request.Form["off_TIME"] == "" ||
                     Request.Form["onDATEreq"] == "" || Request.Form["on_TIME"] == "" || Request.Form["EPEX_EPEX_ID"] == "" ||
                     Request.Form["type_doc"] == ""))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات برای ثبت کامل نیست" }.ToJson();
                }

                string offdate = Request.Form["ofDATEreq"];
                string onDATE = Request.Form["onDATEreq"];
                if (offdate == null || onDATE == null)
                {
                    int eid = Convert.ToInt16(check);
                    var f = (from b in Db.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == eid && b.CUT_STAT == "1" && b.JDTY_JDTY_ID == QCURENT select b).FirstOrDefault();
                    offdate = f.OFF_DATE;
                    onDATE = f.ON_DATE;
                }

                // string off_TIME = Request.Form["off_TIME"];
                // string on_TIME = Request.Form["on_TIME"];
                //off_TIME = off_TIME1.Substring(0, 5);
                //string off_TIMEap = off_TIME1.Substring(6, 1);
                //if (off_TIMEap == "P" || off_TIMEap == "A")
                //    off_TIMEap = off_TIMEap + "M";
                //if (off_TIMEap == "PM" || off_TIMEap == "م")
                //{
                //    string hhoff = off_TIME.Substring(0, 2);
                //    if (hhoff != "12")
                //    {
                //        string ssoff = off_TIME.Substring(3, 2);
                //        int hhoffi = Convert.ToInt16(hhoff) + 12;

                //        off_TIME = hhoffi + ":" + ssoff;
                //    }

                //}
                //if (off_TIMEap == "AM" || off_TIMEap == "ص")
                //{
                //    string hhoff = off_TIME.Substring(0, 2);
                //    if (hhoff == "12")
                //    {
                //        string ssoff = off_TIME.Substring(3, 2);
                //        hhoff = "00";

                //        off_TIME = hhoff + ":" + ssoff;
                //    }

                //}
                //on_TIME = on_TIME1.Substring(0, 5);
                //string on_TIMEap = on_TIME1.Substring(6, 1);

                //if (on_TIMEap == "P" || on_TIMEap == "A")
                //    on_TIMEap = on_TIMEap + "M";

                //if (on_TIMEap == "PM" || on_TIMEap == "م")
                //{
                //    string hhoff = on_TIME.Substring(0, 2);
                //    if (hhoff != "12")
                //    {
                //        string ssoff = on_TIME.Substring(3, 2);
                //        int hhoffi = Convert.ToInt16(hhoff) + 12;

                //        on_TIME = hhoffi + ":" + ssoff;
                //    }
                //}
                //if (on_TIMEap == "AM" || on_TIMEap == "ص")
                //{
                //    string hhoff = on_TIME.Substring(0, 2);
                //    if (hhoff == "12")
                //    {
                //        string ssoff = on_TIME.Substring(3, 2);
                //        hhoff = "00";

                //        on_TIME = hhoff + ":" + ssoff;
                //    }
                //}

                string off_TIME1 = Request.Form["off_TIME"];
                string on_TIME1 = Request.Form["on_TIME"];

                if (Request.Form["stattime"].ToString() == "0")
                {
                    time = (Db.Database.SqlQuery<string>("SELECT TIME_BETWEEN_U('" + offdate + "','" + off_TIME1 + "','" + onDATE + "','" + on_TIME1 + "')  FROM dual").FirstOrDefault());
                }
                else
                {
                    int day = (Db.Database.SqlQuery<int>("SELECT FDAYS_BETWEEN_U('" + offdate + "','" + onDATE + "')  FROM dual").FirstOrDefault());

                    string time1 = (Db.Database.SqlQuery<string>("SELECT TIME_BETWEEN_U('" + offdate + "','" + off_TIME1 + "','" + offdate + "','" + on_TIME1 + "')  FROM dual").FirstOrDefault());
                    if (day == 0)
                    {
                        time = time1;
                    }
                    else
                    {
                        var val = time1.Split(':');
                        string hhoff = val[0].ToString();
                        string ssoff = val[1].ToString();
                        int hh = Convert.ToInt16(hhoff) * (day + 1);
                        int ss = Convert.ToInt16(ssoff) * (day + 1);
                        if (ss >= 60)
                        {
                            hh = hh + ss / 60;
                            ss = ss % 60;
                        }
                        time = hh + ":" + ss;
                    }
                }
                if (Request.Form["stattime"].ToString() == "0")
                {
                    if (offdate.CompareTo(onDATE) > 0)
                    {
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                    }
                    else
                    {
                        if (offdate.CompareTo(onDATE) == 0)
                        {
                            if (off_TIME1.CompareTo(on_TIME1) > 0)
                            {
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                            }
                        }
                    }
                }
                else
                {
                    if (offdate.CompareTo(onDATE) > 0)
                    {
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                    }
                    else
                    {
                        if (off_TIME1.CompareTo(on_TIME1) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                        }
                    }
                }

                if (Request.Form["type_doc"] != "" && Request.Form["type_doc"] != null)
                {
                    int typeo = Convert.ToInt32(Request.Form["type_doc"].ToString());
                    if (typeo == 2)
                    {
                        if (Request.Form["defectnumber"] == "")
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "شماره دیفکت را انتخاب نمایید" }.ToJson();
                        }
                        else
                        {
                            int defectid = Convert.ToInt32(Request.Form["defectnumber"].ToString());
                            var qdefect = (from b in Db.EXP_EXPI_DOC
                                           join k in Db.EXP_EDOC_INSTRU on b.EEDO_ID equals k.EEDO_EEDO_ID
                                           where b.EEDO_ID == defectid
                                           select new { b.EFUN_EFUN_ID, b.ORGA_CODE, b.EPOL_EPOL_ID, k.EINS_EINS_ID, k.EPIU_EPIU_ID, k.ETBY_ETBY_ID, k.OFF_DATE, k.ON_DATE }).FirstOrDefault();

                            int post = Convert.ToInt32(Request.Form["EPOL_EPOL_ID_requ"]);
                            int function = Convert.ToInt32(Request.Form["function"]);

                            int bay = 0;
                            int inst = 0;
                            int tinst = 0;

                            if (Request.Form["etby1"] != "")
                            {
                                bay = Convert.ToInt32(Request.Form["etby1"]);
                                inst = Convert.ToInt32(Request.Form["POSTINST1"]);
                                if (post != qdefect.EPOL_EPOL_ID ||
                                    function != qdefect.EFUN_EFUN_ID || inst != qdefect.EPIU_EPIU_ID ||
                                    bay != qdefect.ETBY_ETBY_ID)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات درخواست با دیفکت انتخاب شده فرق نباید کند" }.ToJson();
                                }
                            }
                            if (Request.Form["POSTINST"] != "")
                            {
                                inst = Convert.ToInt32(Request.Form["POSTINST"]);
                                if (post != qdefect.EPOL_EPOL_ID ||
                                    function != qdefect.EFUN_EFUN_ID || inst != qdefect.EPIU_EPIU_ID)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات درخواست با دیفکت انتخاب شده فرق نباید کند" }.ToJson();
                                }
                            }
                            if (Request.Form["einsrequest"] != "")
                            {
                                tinst = Convert.ToInt32(Request.Form["einsrequest"]);
                                if (post != qdefect.EPOL_EPOL_ID || function != qdefect.EFUN_EFUN_ID || tinst != qdefect.EINS_EINS_ID
                                )
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات درخواست با دیفکت انتخاب شده فرق نباید کند" }.ToJson();
                                }
                            }
                        }
                    }

                    if (typeo == 21 && (Request.Form["defectnumber"] != "" || Request.Form["prognumber"] != ""))
                    {
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = " دیفکت و برنامه نباید انتخاب شود" }.ToJson();
                    }

                    if (typeo == 101)
                    {
                        if (Request.Form["prognumber"] == "")
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "شماره برنامه را انتخاب نمایید" }.ToJson();
                        }
                        else
                        {
                            int progid = Convert.ToInt32(Request.Form["prognumber"].ToString());
                            var qdefect = (from k in Db.EXP_EDOC_INSTRU
                                           where k.EDIN_ID == progid
                                           select new { k.EFUN_EFUN_ID, k.ORGA_CODE, k.EPOL_EPOL_ID, k.EINS_EINS_ID, k.EPIU_EPIU_ID, k.ETBY_ETBY_ID, k.ON_DATE, k.OFF_DATE }).FirstOrDefault();
                            int post = Convert.ToInt32(Request.Form["EPOL_EPOL_ID_requ"]);
                            int function = Convert.ToInt32(Request.Form["function"]);
                            int bay = 0;
                            int inst = 0;
                            int tinst = 0;
                            if (Request.Form["etby1"] != "")
                            {
                                bay = Convert.ToInt32(Request.Form["etby1"]);
                                inst = Convert.ToInt32(Request.Form["POSTINST1"]);
                                if (post != qdefect.EPOL_EPOL_ID || function != qdefect.EFUN_EFUN_ID || inst != qdefect.EPIU_EPIU_ID ||
                                    bay != qdefect.ETBY_ETBY_ID || qdefect.OFF_DATE != offdate || onDATE != qdefect.ON_DATE)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات درخواست با برنامه انتخاب شده فرق نباید کند" }.ToJson();
                                }
                            }
                            if (Request.Form["POSTINST"] != "")
                            {
                                inst = Convert.ToInt32(Request.Form["POSTINST"]);
                                if (post != qdefect.EPOL_EPOL_ID || function != qdefect.EFUN_EFUN_ID || inst != qdefect.EPIU_EPIU_ID || qdefect.OFF_DATE != offdate || onDATE != qdefect.ON_DATE)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات درخواست با برنامه انتخاب شده فرق نباید کند" }.ToJson();
                                }
                            }
                            if (Request.Form["einsrequest"] != "")
                            {
                                tinst = Convert.ToInt32(Request.Form["einsrequest"]);
                                if (post != qdefect.EPOL_EPOL_ID || function != qdefect.EFUN_EFUN_ID || tinst != qdefect.EINS_EINS_ID || qdefect.OFF_DATE != offdate || onDATE != qdefect.ON_DATE)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات درخواست با برنامه انتخاب شده فرق نباید کند" }.ToJson();
                                }
                            }
                        }
                    }
                }
            }

            if (check == "" || check == null)
            {
                int not_id = 0;
                objecttemp.EEDO_ID = objecttemp.EEDO_ID;

                objecttemp.ACTI_NACT = Request.Form["ACTI_NACT"];
                if (!String.IsNullOrEmpty(Request.Form["ANPT_ANPT_ID"]))
                    objecttemp.ANPT_ANPT_ID = Convert.ToInt16(Request.Form["ANPT_ANPT_ID"]);

                objecttemp.CCNT_CNTR_NO = Request.Form["CCNT_CNTR_NO"];
                objecttemp.CONF_TYPE = Request.Form["EXP_EXPI_DOC.CONF_TYPE"];
                if (!String.IsNullOrEmpty(Request.Form["CPRO_CPLA_PLN_CODE"]))
                    objecttemp.CPRO_CPLA_PLN_CODE = Convert.ToInt16(Request.Form["CPRO_CPLA_PLN_CODE"]);

                if (!String.IsNullOrEmpty(Request.Form["CPRO_PRJ_CODE"]))
                    objecttemp.CPRO_PRJ_CODE = Convert.ToInt16(Request.Form["CPRO_PRJ_CODE"]);

                objecttemp.DEFC_DESC = Request.Form["EXP_EXPI_DOC.DEFC_DESC"];
                if (!String.IsNullOrEmpty(Request.Form["DMAN_DMAN_ID"]))
                    objecttemp.DMAN_DMAN_ID = Convert.ToInt16(Request.Form["DMAN_DMAN_ID"]);


                //if (Request.Form["EXP_EXPI_DOC.DOC_NUMB"] == "")
                // {
                string qdocnum = string.Empty;

                int countq = (from b in Db.EXP_EXPI_DOC where b.ETDO_ETDO_ID == etdo_id && b.EANA_EANA_ROW == null select b).Count();
                if (countq == 0)
                {
                    qdocnum = "1";
                }
                else
                {
                    string qmax = (from b in Db.EXP_EXPI_DOC where b.ETDO_ETDO_ID == etdo_id && b.EANA_EANA_ROW == null select b).Max(c => c.EEDO_ID).ToString();
                    int s1 = Convert.ToInt32(qmax);
                    qdocnum = (from b in Db.EXP_EXPI_DOC where b.EEDO_ID == s1 select b.DOC_NUMB).FirstOrDefault().ToString();
                }

                int m = Convert.ToInt16(qdocnum) + 1;

                objecttemp.DOC_NUMB = m.ToString();

                //    }
                //  else
                //   {
                //       objecttemp.DOC_NUMB = Request.Form["EXP_EXPI_DOC.DOC_NUMB"];
                //   }

                objecttemp.EEDO_DATE = Request.Form["EXP_EXPI_DOC.EEDO_DATE"];
                objecttemp.EEDO_TIME = Request.Form["EEDO_TIME"];
                objecttemp.EEDO_DESC = Request.Form["EXP_EXPI_DOC.EEDO_DESC"];
                objecttemp.DEFC_DESC = Request.Form["EXP_EXPI_DOC.DEFC_DESC"];
                if (!String.IsNullOrEmpty(Request.Form["EXP_EXPI_DOC.ETDO_ETDO_ID"]))
                    objecttemp.ETDO_ETDO_ID = Convert.ToInt16(Request.Form["EXP_EXPI_DOC.ETDO_ETDO_ID"]);

                if (!String.IsNullOrEmpty(Request.Form["function"]))
                    objecttemp.EFUN_EFUN_ID = Convert.ToInt16(Request.Form["function"]);

                if (etdo_id == 2)
                {
                    if (!String.IsNullOrEmpty(Request.Form["EPOL_EPOL_ID"]))
                        objecttemp.EPOL_EPOL_ID = Convert.ToInt16(Request.Form["EPOL_EPOL_ID"]);
                    objecttemp.ORGA_CODE = Request.Form["ORGA_CODE"];
                    objecttemp.ORGA_MANA_ASTA_CODE = Request.Form["ORGA_MANA_ASTA_CODE"];
                    objecttemp.ORGA_MANA_CODE = Request.Form["ORGA_MANA_CODE"];
                }
                else
                {
                    if (!String.IsNullOrEmpty(Request.Form["EPOL_EPOL_ID_requ"]))
                        objecttemp.EPOL_EPOL_ID = Convert.ToInt16(Request.Form["EPOL_EPOL_ID_requ"]);
                    objecttemp.ORGA_CODE = Request.Form["ORGA_CODE_request"];
                    objecttemp.ORGA_MANA_ASTA_CODE = "7";
                    objecttemp.ORGA_MANA_CODE = "6";
                }

                objecttemp.FIRS_DATE = Request.Form["FDATE"];
                objecttemp.FIRS_TIME = Request.Form["EXP_EXPI_DOC.FIRS_TIME"];
                // objecttemp.END_DATE = Request.Form["END_DATE"];
                objecttemp.END_DATE = Request.Form["EDATE"];
                objecttemp.END_TIME = Request.Form["END_TIME"];
                objecttemp.EEDO_YEAR = Request.Form["EXP_EXPI_DOC.EEDO_YEAR"];
                if (!String.IsNullOrEmpty(Request.Form["EEOD_EEOD_ID"]))
                    objecttemp.EEOD_EEOD_ID = Convert.ToInt16(Request.Form["EEOD_EEOD_ID"]);
                if (!String.IsNullOrEmpty(Request.Form["EEVO_EEVO_ID"]))
                    objecttemp.EEVO_EEVO_ID = Convert.ToInt16(Request.Form["EEVO_EEVO_ID"]);

                objecttemp.EVEN_TYPE = Request.Form["EXP_EXPI_DOC.EVEN_TYPE"];
                objecttemp.INFL_TYPE = Request.Form["EXP_EXPI_DOC.INFL_TYPE"];
                objecttemp.LETT_TYPE = Request.Form["EXP_EXPI_DOC.LETT_TYPE"];
                objecttemp.OUT_FUNC = Request.Form["DESC"];
                objecttemp.SEND_TYPE = Request.Form["EXP_EXPI_DOC.SEND_TYPE"];
                objecttemp.TIME_STAT = Request.Form["EXP_EXPI_DOC.TIME_STAT"];
                objecttemp.TWRQ_YEAR = Request.Form["EXP_EXPI_DOC.TWRQ_YEAR"];

                Db.EXP_EXPI_DOC.Add(objecttemp);
                Db.SaveChanges();

                int idinst = 0;
                int idetby1 = 0;
                int idinstt = 0;
                int postcode = 0;
                if (etdo_id == 2)
                {
                    var rel1 = new EXP_EDOC_INSTRU();
                    string POSTINST = Request.Form["POSTINST"];

                    if (!string.IsNullOrEmpty(POSTINST))
                    {
                        idinst = Convert.ToInt32(POSTINST);
                        int id = (from b in Db.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == objecttemp.EEDO_ID && b.EPIU_EPIU_ID == idinst select b.EDIN_ID).Count();
                        if (id == 0)
                        {
                            rel1.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                            rel1.EPIU_EPIU_ID = Convert.ToInt32(POSTINST);
                            rel1.EPOL_EPOL_ID = Convert.ToInt32(Request.Form["EPOL_EPOL_ID"]);
                            rel1.ETDO_ETDO_ID = objecttemp.ETDO_ETDO_ID;
                            Db.EXP_EDOC_INSTRU.Add(rel1);
                            Db.SaveChanges();
                        }
                    }

                    string Org1 = string.Empty;
                    short Ope1 = 0;
                    int per = 0;

                    if (Request.Form["ORGA_CODEconf1"] != "")
                        Org1 = Request.Form["ORGA_CODEconf1"].ToString();
                    if (Request.Form["PRSN_EMP_NUMB1"] != "")
                        Ope1 = short.Parse(Request.Form["PRSN_EMP_NUMB1"].ToString());
                    if (Request.Form["PRSN_EMP_out"] != "")
                        per = Convert.ToInt16(Request.Form["PRSN_EMP_out"].ToString());

                    var rel2 = new EXP_SUPL_DOC();

                    if (Ope1 != 0)
                    {
                        int idm = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == objecttemp.EEDO_ID && b.PRSN_EMP_NUMB == Ope1 select b.ESUD_ID).Count();
                        if (idm == 0)
                        {
                            rel2.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                            rel2.ORGA_CODE = Org1;
                            rel2.ORGA_MANA_CODE = "6";
                            rel2.ORGA_MANA_ASTA_CODE = "7";
                            rel2.POSI_TYEP = "0";
                            rel2.PRSN_EMP_NUMB = Ope1;

                            Db.EXP_SUPL_DOC.Add(rel2);
                            Db.SaveChanges();
                        }
                    }
                    else
                    {
                        if (per != 0)
                        {
                            int idm = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == objecttemp.EEDO_ID && b.OUTP_OUTP_ID == per select b.ESUD_ID).Count();
                            if (idm == 0)
                            {
                                rel2.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                                rel2.ORGA_CODE = Org1;
                                rel2.ORGA_MANA_CODE = "6";
                                rel2.ORGA_MANA_ASTA_CODE = "7";
                                rel2.POSI_TYEP = "0";
                                rel2.OUTP_OUTP_ID = per;

                                Db.EXP_SUPL_DOC.Add(rel2);
                                Db.SaveChanges();
                            }
                        }
                    }
                }
                else
                {
                    var rel1 = new EXP_EDOC_INSTRU();
                    string POSTINST = string.Empty;
                    string etby1 = string.Empty;
                    string eins = string.Empty;

                    if (Request.Form["POSTINST"] != "")
                    {
                        POSTINST = Request.Form["POSTINST"];
                        idinst = Convert.ToInt32(POSTINST);
                    }

                    if (Request.Form["etby1"] != "")
                    {
                        POSTINST = Request.Form["POSTINST1"];
                        idinst = Convert.ToInt32(POSTINST);
                        etby1 = Request.Form["etby1"];
                        idetby1 = Convert.ToInt32(etby1);
                    }
                    if (Request.Form["einsrequest"] != "")
                    {
                        eins = Request.Form["einsrequest"];
                        idinstt = Convert.ToInt32(eins);
                    }
                    if (Request.Form["EPOL_EPOL_ID_requ"] != "")
                    {
                        postcode = Convert.ToInt32(Request.Form["EPOL_EPOL_ID_requ"]);
                    }

                    string tyepinst = Request.Form["typeinstREQUEST"].ToString();
                    if (((idinstt != 0 || idinst != 0) && tyepinst != "0") || (postcode != 0 && tyepinst == "0"))
                    {
                        int id = (from b in Db.EXP_EDOC_INSTRU
                                  where b.EEDO_EEDO_ID == objecttemp.EEDO_ID && b.EPOL_EPOL_ID == postcode &&
                                        (b.EPIU_EPIU_ID == idinst || b.EINS_EINS_ID == idinstt || b.ETBY_ETBY_ID == idetby1)
                                  select b.EDIN_ID).Count();
                        if (id == 0)
                        {
                            rel1.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                            if (idinst != 0)
                            {
                                rel1.EPIU_EPIU_ID = idinst;
                            }
                            if (idinstt != 0)
                            {
                                rel1.EINS_EINS_ID = idinstt;
                            }
                            if (idetby1 != 0)
                            {
                                rel1.ETBY_ETBY_ID = idetby1;
                            }

                            rel1.EPOL_EPOL_ID = postcode;
                            rel1.JDTY_JDTY_ID = Convert.ToDecimal(QCURENT.ToString());
                            rel1.OFF_DATE = Request.Form["ofDATEreq"];
                            rel1.OFF_TIME = Request.Form["off_TIME"];
                            rel1.ON_DATE = Request.Form["onDATEreq"];
                            rel1.ON_TIME = Request.Form["on_TIME"];
                            rel1.TIME_ISTA = Request.Form["stattime"];
                            rel1.INST_STAT = Request.Form["INST_STAT"];
                            rel1.EART_STAT = Request.Form["EART_STAT"];
                            rel1.CUST_STAT = Request.Form["CUST_STAT"];
                            rel1.ETDO_ETDO_ID = objecttemp.ETDO_ETDO_ID;
                            rel1.CUT_STAT = "1";
                            rel1.CONT_FUN = time;
                            if (Request.Form["Unitvolt"] != "")
                            {
                                rel1.EUNL_EUNL_ID = Convert.ToInt16(Request.Form["Unitvolt"].ToString());
                            }
                            if (Request.Form["offstat"] != "")
                            {
                                rel1.EOFS_EOFS_ID = Convert.ToInt32(Request.Form["offstat"]);
                            }

                            Db.EXP_EDOC_INSTRU.Add(rel1);
                            Db.SaveChanges();
                        }
                    }

                    int sup = 0;
                    int sup1 = 0;

                    var rel2 = new EXP_SUPL_DOC();
                    var rel3 = new EXP_SUPL_DOC();
                    if (Request.Form["EPEX_EPEX_ID"] != "")
                        sup = Convert.ToInt32(Request.Form["EPEX_EPEX_ID"].ToString());
                    if (Request.Form["EPEX_EPEX_ID1"] != "")
                        sup1 = Convert.ToInt32(Request.Form["EPEX_EPEX_ID1"].ToString());

                    if (sup != 0)
                    {
                        int idm = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == objecttemp.EEDO_ID && b.EPEX_EPEX_ID == sup select b.ESUD_ID).Count();
                        if (idm == 0)
                        {
                            rel2.EEDO_EEDO_ID = objecttemp.EEDO_ID;

                            rel2.POSI_TYEP = "2";
                            rel2.EPEX_EPEX_ID = sup;
                            rel2.ESUD_DESC = Request.Form["group"];
                            Db.EXP_SUPL_DOC.Add(rel2);
                            Db.SaveChanges();
                        }
                        int idm1 = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == objecttemp.EEDO_ID && b.EPEX_EPEX_ID == sup1 select b.ESUD_ID).Count();
                        if (idm1 == 0)
                        {
                            rel3.EEDO_EEDO_ID = objecttemp.EEDO_ID;

                            rel3.POSI_TYEP = "5";
                            rel3.EPEX_EPEX_ID = sup1;

                            Db.EXP_SUPL_DOC.Add(rel3);
                            Db.SaveChanges();
                        }
                    }
                }

                var rel = new EXP_RELATION_DOC();
                string EEDO_EEDO_ID_R = Request.Form["EEDO_EEDO_ID_R"];
                if (!string.IsNullOrEmpty(EEDO_EEDO_ID_R))
                {
                    rel.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                    rel.EEDO_EEDO_ID_R = Convert.ToInt32(EEDO_EEDO_ID_R);
                    Db.EXP_RELATION_DOC.Add(rel);
                    Db.SaveChanges();
                }
                if (!string.IsNullOrEmpty(Request.Form["master_doc"]))
                {
                    rel.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                    rel.EEDO_EEDO_ID_R = Convert.ToInt32(Request.Form["master_doc"]);
                    Db.EXP_RELATION_DOC.Add(rel);
                    Db.SaveChanges();
                }

                if (!string.IsNullOrEmpty(Request.Form["prognumber"]))
                {
                    rel.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                    rel.EDIN_EDIN_ID = Convert.ToInt32(Request.Form["prognumber"]);
                    Db.EXP_RELATION_DOC.Add(rel);
                    Db.SaveChanges();
                }

                if (!string.IsNullOrEmpty(Request.Form["defectnumber"]))
                {
                    rel.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                    rel.EEDO_EEDO_ID_R = Convert.ToInt32(Request.Form["defectnumber"]);
                    Db.EXP_RELATION_DOC.Add(rel);
                    Db.SaveChanges();
                }

                string srow = Request.Form["count"];
                int row = Convert.ToInt16(srow);
                var value = new EXP_EITEM_DOC_VALUE();
                for (int i = 0; i <= row; i++)
                {
                    string val = Request.Form[i.ToString()];
                    string EITY_ID = Request.Form["EITY_ID" + i.ToString()];
                    if (string.IsNullOrEmpty(val))
                    {
                        val = Request.Form["dman" + i.ToString()];
                    }
                    if (!string.IsNullOrEmpty(val))
                    {
                        value.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                        value.EITY_EITY_ID = Convert.ToInt16(EITY_ID);
                        value.EIDR_VALUE = val;
                        Db.EXP_EITEM_DOC_VALUE.Add(value);
                        Db.SaveChanges();
                    }
                }

                string bodymessage = string.Empty;
                string doc_name = (from b in Db.EXP_TYPE_DOC where b.ETDO_ID == etdo_id select b.ETDO_DESC).FirstOrDefault();

                switch (etdo_id)
                {
                    case (2):
                        {
                            string postn = (from b in Db.EXP_POST_LINE where b.EPOL_ID == objecttemp.EPOL_EPOL_ID select b.EPOL_NAME).FirstOrDefault().ToString();
                            var w = (from b in Db.EXP_POST_LINE_INSTRU where b.EPIU_ID == idinst select b);
                            string instn = w.FirstOrDefault().CODE_NAME.ToString();
                            string insttype = w.FirstOrDefault().EINS_EINS_ID.ToString();
                            if (insttype == "1")
                                instn = " خط " + instn;
                            else
                                instn = " تجهیز " + instn;
                            string smessage = " دیفکت " + "به شماره" + objecttemp.DOC_NUMB + " در تاریخ " + objecttemp.EEDO_DATE + " مربوط به " + postn + " و " + instn + " میباشد ";
                            // objecttemp.DEFC_DESC;//+ "به شماره" + objecttemp.DOC_NUMB + " در تاریخ " + objecttemp.EEDO_DATE + " مربوط به " + postn + " و " + instn+" میباشد ";
                            bodymessage = smessage;
                            break;
                        }
                    case (21):
                        {
                            string postn = (from b in Db.EXP_POST_LINE where b.EPOL_ID == objecttemp.EPOL_EPOL_ID select b.EPOL_NAME).FirstOrDefault().ToString();

                            string instrdoc = string.Empty;
                            if (idetby1 != 0)
                            {
                                string instn = string.Empty;
                                var bay = (from b in Db.EXP_TYPE_BAY where b.ETBY_ID == idetby1 select b);
                                instn = bay.FirstOrDefault().ETBY_DESC.ToString();
                                instrdoc = instn;
                            }
                            if (idinstt != 0)
                            {
                                string instn = string.Empty;
                                var inst = (from b in Db.EXP_INSTRUMENT where b.EINS_ID == idinstt select b);
                                string insttype = inst.FirstOrDefault().EINS_ID.ToString();
                                instn = inst.FirstOrDefault().EINS_DESC.ToString();
                                instrdoc = instn + " " + instrdoc;
                            }
                            if (idinst != 0)
                            {
                                var w = (from b in Db.EXP_POST_LINE_INSTRU where b.EPIU_ID == idinst select b);
                                string instn = string.Empty;
                                instn = w.FirstOrDefault().CODE_NAME.ToString();
                                string insttype = w.FirstOrDefault().EINS_EINS_ID.ToString();
                                if (insttype == "1")
                                    instn = " خط " + instn;
                                else
                                    instn = " تجهیز " + instn;
                                instrdoc = instn + " " + instrdoc;
                            }

                            string smessage = " درخواست " + "به شماره" + objecttemp.DOC_NUMB + " در تاریخ " + objecttemp.EEDO_DATE + " مربوط به " + postn + " و " + instrdoc + " میباشد ";
                            // objecttemp.DEFC_DESC;//+ "به شماره" + objecttemp.DOC_NUMB + " در تاریخ " + objecttemp.EEDO_DATE + " مربوط به " + postn + " و " + instn+" میباشد ";
                            bodymessage = smessage;
                            break;
                        }
                }

                try
                {
                    AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
                    // wp.StartProcess(this.HttpContext.User.Identity.Name, new string[] { this.HttpContext.User.Identity.Name }, doc_name, bodymessage, etdo_id, objecttemp.EEDO_ID);
                    // Asr.Cartable.AsrWorkFlowProcess wf = new Asr.Cartable.AsrWorkFlowProcess();
                    wp.StartProcess(this.HttpContext.User.Identity.Name, new string[] { this.HttpContext.User.Identity.Name }, doc_name, bodymessage, etdo_id, objecttemp.EEDO_ID);
                    // wp.StartProcess(this.HttpContext.User.Identity.Name, new string[] { this.HttpContext.User.Identity.Name }, doc_name, bodymessage, etdo_id, objecttemp.EEDO_ID);
                    not_id = wp.NoteId;
                    //var typedoc = (from x in cntx.EXP_TYPE_DOC where x.ETDO_ID == etdo_id select x).FirstOrDefault();

                    //string s = typedoc.FLOW_TYPE.ToString() + "." + typedoc.PRCC_NAME.ToString() + "^" + objecttemp.EEDO_ID.ToString();
                    //not_id = Asr.Security.WF_LOCAL_USER_ROLES.searchnotid(s);

                    //var row1 = (from b in cntx.EXP_EXPI_DOC
                    //            where b.EEDO_ID == objecttemp.EEDO_ID
                    //            select b).FirstOrDefault();

                    //row1.MADR_NUM = not_id.ToString();

                    //cntx.SaveChanges();
                    if (etdo_id == 2)
                    {
                        return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", objecttemp.DOC_NUMB), CoustomData = not_id.ToString() }.ToJson();
                    }
                    else
                    {
                        return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", objecttemp.DOC_NUMB), CoustomData = objecttemp.EEDO_ID.ToString() + "%" + not_id.ToString() }.ToJson();
                    }
                }
                catch (Exception ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
                }
            }
            else
            {
                decimal edin = 0;
                decimal eprog = 0;
                if (Request.Form["defectnumber"] != "" && Request.Form["defectnumber"] != null)
                {
                    edin = Convert.ToDecimal(Request.Form["defectnumber"]);
                }

                if (Request.Form["prognumber"] != "" && Request.Form["prognumber"] != null)
                {
                    eprog = Convert.ToDecimal(Request.Form["prognumber"]);
                }

                var rel = new EXP_RELATION_DOC();
                int eedoid = Convert.ToInt32(check);
                if (eprog != 0)
                {
                    string cou = (from b in Db.EXP_RELATION_DOC where b.EEDO_EEDO_ID == eedoid && b.EDIN_EDIN_ID == eprog select b).Count().ToString();
                    int rc = Convert.ToInt32(cou);
                    if (rc == 0)
                    {
                        rel.EEDO_EEDO_ID = eedoid;
                        rel.EDIN_EDIN_ID = eprog;
                        Db.EXP_RELATION_DOC.Add(rel);
                        Db.SaveChanges();
                    }
                }

                if (edin != 0)
                {
                    string cou = (from b in Db.EXP_RELATION_DOC where b.EEDO_EEDO_ID == eedoid && b.EEDO_EEDO_ID_R == edin select b).Count().ToString();
                    int rrc = Convert.ToInt32(cou);
                    if (rrc == 0)
                    {
                        rel.EEDO_EEDO_ID = eedoid;
                        rel.EEDO_EEDO_ID_R = edin;
                        Db.EXP_RELATION_DOC.Add(rel);
                        Db.SaveChanges();
                    }
                }
                //string ic = (from b in cntx.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == eedoid && b.CUT_STAT == "1" select b).Count().ToString();
                //int coi = Convert.ToInt32(ic);
                //if (coi !=0 )
                //{
                //    var instrument = (from b in cntx.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == eedoid && b.CUT_STAT == "1" select b).FirstOrDefault();
                //    cntx.EXP_EDOC_INSTRU.Remove(instrument);
                //    cntx.SaveChanges();
                //}

                var rel1 = new EXP_EDOC_INSTRU();

                rel1 = (from b in Db.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == eedoid && b.CUT_STAT == "1" select b).FirstOrDefault();

                string POSTINST = string.Empty;
                string etby1 = string.Empty;
                string eins = string.Empty;
                decimal idinst = 0;
                int idetby1 = 0;
                int idinstt = 0;
                int postcode = 0;

                if (Request.Form["POSTINST"] != "" && Request.Form["POSTINST"] != null)
                {
                    POSTINST = Request.Form["POSTINST"];
                    idinst = Convert.ToInt32(POSTINST);
                    rel1.EPIU_EPIU_ID = idinst;
                }

                if (Request.Form["etby1"] != "" && Request.Form["etby1"] != null)
                {
                    POSTINST = Request.Form["POSTINST1"];
                    idinst = Convert.ToInt32(POSTINST);
                    etby1 = Request.Form["etby1"];
                    idetby1 = Convert.ToInt32(etby1);
                    rel1.EPIU_EPIU_ID = idinst;
                    rel1.ETBY_ETBY_ID = idetby1;
                }
                if (Request.Form["einsrequest"] != "" && Request.Form["einsrequest"] != null)
                {
                    eins = Request.Form["einsrequest"];
                    idinstt = Convert.ToInt32(eins);
                    rel1.EINS_EINS_ID = idinstt;
                }
                if (Request.Form["EPOL_EPOL_ID_requ"] != "" && Request.Form["EPOL_EPOL_ID_requ"] != null)
                {
                    postcode = Convert.ToInt32(Request.Form["EPOL_EPOL_ID_requ"]);
                    rel1.EPOL_EPOL_ID = postcode;
                }

                if (Request.Form["Unitvolt"] != "" && Request.Form["Unitvolt"] != null)
                {
                    rel1.EUNL_EUNL_ID = Convert.ToInt16(Request.Form["Unitvolt"].ToString());
                }
                if (Request.Form["offstat"] != "" && Request.Form["offstat"] != null)
                {
                    rel1.EOFS_EOFS_ID = Convert.ToInt32(Request.Form["offstat"]);
                }

                if (Request.Form["ofDATEreq"] != "" && Request.Form["ofDATEreq"] != null)
                {
                    rel1.OFF_DATE = Request.Form["ofDATEreq"];
                }
                if (Request.Form["off_TIME"] != "" && Request.Form["off_TIME"] != null)
                {
                    rel1.OFF_TIME = Request.Form["off_TIME"];
                }
                if (Request.Form["onDATEreq"] != "" && Request.Form["onDATEreq"] != null)
                {
                    rel1.ON_DATE = Request.Form["onDATEreq"];
                }

                if (Request.Form["on_TIME"] != "" && Request.Form["on_TIME"] != null)
                {
                    rel1.ON_TIME = Request.Form["on_TIME"];
                }

                if (Request.Form["stattime"] != "" && Request.Form["stattime"] != null)
                {
                    rel1.TIME_ISTA = Request.Form["stattime"];
                    rel1.CONT_FUN = time;
                }
                if (Request.Form["INST_STAT"] != "" && Request.Form["INST_STAT"] != null)
                {
                    rel1.INST_STAT = Request.Form["INST_STAT"];
                }
                if (Request.Form["EART_STAT"] != "" && Request.Form["EART_STAT"] != null)
                {
                    rel1.EART_STAT = Request.Form["EART_STAT"];
                }
                if (Request.Form["CUST_STAT"] != "" && Request.Form["CUST_STAT"] != null)
                {
                    rel1.CUST_STAT = Request.Form["CUST_STAT"];
                }

                if (Request.Form["ofDATEprog"] != "" && Request.Form["ofDATEprog"] != null)
                {
                    rel1.OFF_DATE = Request.Form["ofDATEprog"];
                }
                if (Request.Form["off_TIMEprog"] != "" && Request.Form["off_TIMEprog"] != null)
                {
                    rel1.PROF_TIME = Request.Form["off_TIMEprog"];
                }
                if (Request.Form["onDATEprog"] != "" && Request.Form["onDATEprog"] != null)
                {
                    rel1.ON_DATE = Request.Form["onDATEprog"];
                }

                if (Request.Form["on_TIMEprog"] != "" && Request.Form["on_TIMEprog"] != null)
                {
                    rel1.PROE_TIME = Request.Form["on_TIMEprog"];
                }
                Db.SaveChanges();

                //if (idinstt != 0 || idinst != 0)
                //{

                //    int id = (from b in cntx.EXP_EDOC_INSTRU
                //              where b.EEDO_EEDO_ID == eedoid && b.EPOL_EPOL_ID == postcode &&
                //                  (b.EPIU_EPIU_ID == idinst || b.EINS_EINS_ID == idinstt || b.ETBY_ETBY_ID == idetby1)
                //              select b.EDIN_ID).Count();
                //    if (id == 0)
                //    {
                //        rel1.EEDO_EEDO_ID = eedoid;
                //        if (idinst != 0)
                //        {
                //            rel1.EPIU_EPIU_ID = idinst;
                //        }
                //        if (idinstt != 0)
                //        {
                //            rel1.EINS_EINS_ID = idinstt;
                //        }
                //        if (idetby1 != 0)
                //        {
                //            rel1.ETBY_ETBY_ID = idetby1;
                //        }

                //        rel1.EPOL_EPOL_ID = postcode;
                //        rel1.OFF_DATE = Request.Form["ofDATE"];
                //        rel1.OFF_TIME = Request.Form["off_TIME"];
                //        rel1.ON_DATE = Request.Form["onDATE"];
                //        rel1.ON_TIME = Request.Form["on_TIME"];
                //        rel1.TIME_ISTA = Request.Form["stattime"];
                //        rel1.INST_STAT = Request.Form["INST_STAT"];
                //        rel1.EART_STAT = Request.Form["EART_STAT"];
                //        rel1.CUST_STAT = Request.Form["CUST_STAT"];
                //        rel1.CUT_STAT = "1";
                //        if (Request.Form["Unitvolt"] != "")
                //        {
                //            rel1.EUNL_EUNL_ID = Convert.ToInt16(Request.Form["Unitvolt"].ToString());
                //        }
                //        if (Request.Form["offstat"] != "")
                //        {
                //            rel1.EOFS_EOFS_ID = Convert.ToInt32(Request.Form["offstat"]);
                //        }
                //        cntx.EXP_EDOC_INSTRU.Add(rel1);

                var suplier = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == eedoid && b.POSI_TYEP == "2" select b).FirstOrDefault();
                //cntx.EXP_SUPL_DOC.Remove(suplier);
                //cntx.SaveChanges();
                var suplierj = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == eedoid && b.POSI_TYEP == "5" select b).FirstOrDefault();
                //cntx.EXP_SUPL_DOC.Remove(suplierj);
                //cntx.SaveChanges();

                //int sup = 0;
                //int sup1 = 0;

                ////var rel2 = new EXP_SUPL_DOC();
                ////var rel3 = new EXP_SUPL_DOC();
                if (suplier != null)
                {
                    if (Request.Form["EPEX_EPEX_ID"] != "" && Request.Form["EPEX_EPEX_ID"] != null)
                        suplier.EPEX_EPEX_ID = Convert.ToInt32(Request.Form["EPEX_EPEX_ID"].ToString());

                    if (Request.Form["group"] != "" && Request.Form["group"] != null)
                        suplier.ESUD_DESC = Request.Form["group"];
                }
                else
                {
                    if (Request.Form["EPEX_EPEX_ID"] != "")
                    {
                        int sup = Convert.ToInt32(Request.Form["EPEX_EPEX_ID"].ToString());

                        var rel2 = new EXP_SUPL_DOC();
                        rel2.EEDO_EEDO_ID = eedoid;

                        rel2.POSI_TYEP = "2";
                        rel2.EPEX_EPEX_ID = sup;
                        rel2.ESUD_DESC = Request.Form["group"];
                        Db.EXP_SUPL_DOC.Add(rel2);
                        Db.SaveChanges();
                    }
                }
                if (suplierj != null)
                {
                    if (Request.Form["EPEX_EPEX_ID1"] != "" && Request.Form["EPEX_EPEX_ID1"] != null)
                        suplierj.EPEX_EPEX_ID = Convert.ToInt32(Request.Form["EPEX_EPEX_ID1"].ToString());
                }
                else
                {
                    if (Request.Form["EPEX_EPEX_ID1"] != "")
                    {
                        int sup1 = Convert.ToInt32(Request.Form["EPEX_EPEX_ID1"].ToString());
                        var rel3 = new EXP_SUPL_DOC();
                        rel3.EEDO_EEDO_ID = eedoid;

                        rel3.POSI_TYEP = "5";
                        rel3.EPEX_EPEX_ID = sup1;

                        Db.EXP_SUPL_DOC.Add(rel3);
                        Db.SaveChanges();
                    }
                }

                Db.SaveChanges();
                //if (sup != 0)
                //{
                //    int idm = (from b in cntx.EXP_SUPL_DOC where b.EEDO_EEDO_ID == eedoid && b.EPEX_EPEX_ID == sup select b.ESUD_ID).Count();
                //    if (idm == 0)
                //    {
                //        rel2.EEDO_EEDO_ID = eedoid;

                //        rel2.POSI_TYEP = "2";
                //        rel2.EPEX_EPEX_ID = sup;
                //        rel2.ESUD_DESC = Request.Form["group"];
                //        cntx.EXP_SUPL_DOC.Add(rel2);
                //        cntx.SaveChanges();
                //    }
                //    int idm1 = (from b in cntx.EXP_SUPL_DOC where b.EEDO_EEDO_ID == eedoid && b.EPEX_EPEX_ID == sup1 select b.ESUD_ID).Count();
                //    if (idm1 == 0)
                //    {
                //        rel3.EEDO_EEDO_ID = eedoid;

                //        rel3.POSI_TYEP = "5";
                //        rel3.EPEX_EPEX_ID = sup1;

                //        cntx.EXP_SUPL_DOC.Add(rel3);
                //        cntx.SaveChanges();
                //    }

                //}
                var chkl = (from b in Db.EXP_EXPI_DOC where b.EEDO_ID == eedoid select b).FirstOrDefault();
                if (Request.Form["EXP_EXPI_DOC.EEDO_DESC"] != "" && Request.Form["EXP_EXPI_DOC.EEDO_DESC"] != null)
                    chkl.EEDO_DESC = Request.Form["EXP_EXPI_DOC.EEDO_DESC"];

                if (Request.Form["EXP_EXPI_DOC.EEDO_DATE"] != "" && Request.Form["EXP_EXPI_DOC.EEDO_DATE"] != null)
                    chkl.EEDO_DATE = Request.Form["EXP_EXPI_DOC.EEDO_DATE"];

                if (Request.Form["ORGA_CODE_request"] != "" && Request.Form["ORGA_CODE_request"] != null)
                    chkl.ORGA_CODE = Request.Form["ORGA_CODE_request"];

                if (Request.Form["EPOL_EPOL_ID_requ"] != "" && Request.Form["EPOL_EPOL_ID_requ"] != null)
                    chkl.EPOL_EPOL_ID = Convert.ToInt32(Request.Form["EPOL_EPOL_ID_requ"]);

                if (Request.Form["function"] != "" && Request.Form["function"] != null)
                    chkl.EFUN_EFUN_ID = Convert.ToInt32(Request.Form["function"]);

                if (Request.Form["DESC"] != "" && Request.Form["DESC"] != null)
                    chkl.OUT_FUNC = Request.Form["DESC"];

                Db.SaveChanges();

                string relation = (from b in Db.EXP_RELATION_DOC
                                   join x in Db.EXP_EXPI_DOC on b.EEDO_EEDO_ID_R equals x.EEDO_ID
                                   where b.EEDO_EEDO_ID == eedoid && x.ETDO_ETDO_ID == 21
                                   select b.ERED_ID).FirstOrDefault().ToString();

                //  cntx.EXP_RELATION_DOC.Remove(relation);
                int rccc = Convert.ToInt32(relation);
                if (rccc == 0)
                {
                    if (!string.IsNullOrEmpty(Request.Form["master_doc"]))
                    {
                        rel.EEDO_EEDO_ID = eedoid;
                        rel.EEDO_EEDO_ID_R = Convert.ToInt32(Request.Form["master_doc"]);
                        Db.EXP_RELATION_DOC.Add(rel);
                        Db.SaveChanges();
                    }
                }
                else
                {
                    var qrelattd = (from b in Db.EXP_RELATION_DOC where b.EEDO_EEDO_ID == eedoid && b.ERED_ID == rccc select b).FirstOrDefault();
                    if (Request.Form["master_doc"] != "" && Request.Form["master_doc"] != null)
                        qrelattd.EEDO_EEDO_ID_R = Convert.ToInt32(Request.Form["master_doc"]);
                    Db.SaveChanges();
                }

                string nod = Request.Form["notId"];

                //  var rel = new EXP_RELATION_DOC();
                //if (!string.IsNullOrEmpty(Request.Form["master_doc"]))
                //{
                //    rel.EEDO_EEDO_ID = eedoid;
                //    rel.EEDO_EEDO_ID_R = Convert.ToInt32(Request.Form["master_doc"]);
                //    cntx.EXP_RELATION_DOC.Add(rel);
                //    cntx.SaveChanges();

                //}

                //  }
                //  }
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", objecttemp.DOC_NUMB), CoustomData = eedoid.ToString() + "%" + nod.ToString() }.ToJson();
                // return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", eedoid), CoustomData = eedoid.ToString() }.ToJson();
            }
        }

        public ActionResult Update_Expi_Doc(EXP_EXPI_DOC objecttemp)
        {
            int notid = 0;

            if (Request.Form["notId"] != null)
                notid = Convert.ToInt32(Request.Form["notId"].ToString());
            else
                notid = Convert.ToInt32(Session["notid"]);

            AsrWorkFlowProcess p1 = new AsrWorkFlowProcess(Convert.ToInt32(notid));

            string curent = p1.CurrentStat;
            int idtype = 0;
            string flows = p1.FlowName;
            if (flows == "FLW_REHA")
            {
                idtype = 183;
            }
            else
            {
                idtype = Convert.ToInt32(Session["etdo_id"]);
            }

            if (curent == "?")
            {
                if (idtype == 2)
                {
                    curent = "CRATEOR";
                }
                if (idtype == 21)
                {
                    curent = "CREATOR";
                }
            }

            string EEDO_ID = Request.Form["EXP_EXPI_DOC.EEDO_ID"];
            objecttemp.EEDO_ID = Convert.ToInt32(EEDO_ID);
            int ideedo = Convert.ToInt32(EEDO_ID);
            //  int idtype = Convert.ToInt32(Session["etdo_id"]);
            string time = string.Empty;
            if (idtype == 21)
            {
                if ((Request.Form["EXP_EXPI_DOC.EEDO_DATE"] == "" || Request.Form["function"] == "" || Request.Form["EPOL_EPOL_ID_requ"] == "" ||
                     Request.Form["ORGA_CODE_request"] == "" ||
                     ((Request.Form["POSTINST"] == "" && Request.Form["etby1"] == "" && Request.Form["einsrequest"] == "" && Request.Form["typeinstREQUEST"] != "0") ||
                      (Request.Form["typeinstREQUEST"] == "0" && Request.Form["EPOL_EPOL_ID_requ"] == "")) ||
                     Request.Form["ofDATEreq"] == "" || Request.Form["off_TIME"] == "" ||
                     Request.Form["onDATEreq"] == "" || Request.Form["on_TIME"] == "" || Request.Form["EPEX_EPEX_ID"] == "" ||
                     Request.Form["type_doc"] == ""))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات برای ثبت کامل نیست" }.ToJson();
                }
                string offdate = string.Empty;
                string onDATE = string.Empty;
                string on_TIME = string.Empty;
                string off_TIME = string.Empty;
                if (Request.Form["ofDATEreq"] == null || Request.Form["onDATEreq"] == null)
                {
                    int eid = Convert.ToInt16(EEDO_ID);
                    var f = (from b in Db.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == eid && b.CUT_STAT == "1" select b).FirstOrDefault();
                    offdate = f.OFF_DATE;
                    onDATE = f.ON_DATE;
                }
                else
                {
                    offdate = Request.Form["ofDATEreq"];
                    onDATE = Request.Form["onDATEreq"];
                }

                if (Request.Form["off_TIME"] != null && Request.Form["on_TIME"] != null)
                {
                    string off_TIME1 = Request.Form["off_TIME"];
                    string on_TIME1 = Request.Form["on_TIME"];

                    if (Request.Form["stattime"].ToString() == "0")
                    {
                        time = (Db.Database.SqlQuery<string>("SELECT TIME_BETWEEN_U('" + offdate + "','" + off_TIME1 + "','" + onDATE + "','" + on_TIME1 + "')  FROM dual").FirstOrDefault());
                    }
                    else
                    {
                        int day = (Db.Database.SqlQuery<int>("SELECT FDAYS_BETWEEN_U('" + offdate + "','" + onDATE + "')  FROM dual").FirstOrDefault());

                        string time1 = (Db.Database.SqlQuery<string>("SELECT TIME_BETWEEN_U('" + offdate + "','" + off_TIME1 + "','" + offdate + "','" + on_TIME1 + "')  FROM dual").FirstOrDefault());
                        if (day == 0)
                        {
                            time = time1;
                        }
                        else
                        {
                            var val = time1.Split(':');
                            string hhoff = val[0].ToString();
                            string ssoff = val[1].ToString();

                            int hh = Convert.ToInt16(hhoff) * (day + 1);
                            int ss = Convert.ToInt16(ssoff) * (day + 1);
                            if (ss >= 60)
                            {
                                hh = hh + ss / 60;
                                ss = ss % 60;
                            }
                            time = hh + ":" + ss;
                        }
                    }
                    if (Request.Form["stattime"].ToString() == "0")
                    {
                        if (offdate.CompareTo(onDATE) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                        }
                        else
                        {
                            if (offdate.CompareTo(onDATE) == 0)
                            {
                                if (off_TIME1.CompareTo(on_TIME1) > 0)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (offdate.CompareTo(onDATE) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                        }
                        else
                        {
                            if (off_TIME1.CompareTo(on_TIME1) > 0)
                            {
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                            }
                        }
                    }
                }

                if (Request.Form["off_TIMEprog"] != null && Request.Form["on_TIMEprog"] != null)
                {
                    string offdate2 = Request.Form["ofDATEprog"];
                    string onDATE2 = Request.Form["onDATEprog"];

                    string off_TIME2 = Request.Form["off_TIMEprog"];
                    string on_TIME2 = Request.Form["on_TIMEprog"];

                    var einst = (from p in Db.EXP_EDOC_INSTRU
                                 join b in Db.SEC_JOB_TYPE_DOC on p.JDTY_JDTY_ID equals b.JDTY_ID
                                 where p.EEDO_EEDO_ID == ideedo && p.CUT_STAT == "1" && b.ETDO_ETDO_ID == 21 && b.ACTIV_NAME == "CREATOR"
                                 select p).FirstOrDefault();

                    string stime = einst.TIME_ISTA.ToString();
                    //if (Request.Form["stattime"].ToString() == "0")
                    //{
                    //    time = (cntx.Database.SqlQuery<string>("SELECT TIME_BETWEEN_U('" + offdate + "','" + off_TIME1 + "','" + onDATE + "','" + on_TIME1 + "')  FROM dual").FirstOrDefault());
                    //}
                    //else
                    //{

                    //    int day = (cntx.Database.SqlQuery<int>("SELECT FDAYS_BETWEEN_U('" + offdate + "','" + onDATE + "')  FROM dual").FirstOrDefault());

                    //    string time1 = (cntx.Database.SqlQuery<string>("SELECT TIME_BETWEEN_U('" + offdate + "','" + off_TIME1 + "','" + offdate + "','" + on_TIME1 + "')  FROM dual").FirstOrDefault());
                    //    if (day == 0)
                    //    { time = time1; }
                    //    else
                    //    {
                    //        var val = time1.Split(':');
                    //        string hhoff = val[0].ToString();
                    //        string ssoff = val[1].ToString();

                    //        int hh = Convert.ToInt16(hhoff) * (day + 1);
                    //        int ss = Convert.ToInt16(ssoff) * (day + 1);
                    //        if (ss >= 60)
                    //        {
                    //            hh = hh + ss / 60;
                    //            ss = ss % 60;
                    //        }
                    //        time = hh + ":" + ss;
                    //    }

                    //}
                    if (stime == "0")
                    {
                        if (offdate2.CompareTo(onDATE2) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                        }
                        else
                        {
                            if (offdate2.CompareTo(onDATE2) == 0)
                            {
                                if (off_TIME2.CompareTo(on_TIME2) > 0)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (offdate2.CompareTo(onDATE2) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                        }
                        else
                        {
                            if (off_TIME2.CompareTo(on_TIME2) > 0)
                            {
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                            }
                        }
                    }
                }
                if (Request.Form["off_TIMEh"] != null && Request.Form["on_TIMEh"] != null)
                {
                    string offdate2 = Request.Form["offDATEh"];
                    string onDATE2 = Request.Form["onDATEh"];

                    string off_TIME2 = Request.Form["off_TIMEh"];
                    string on_TIME2 = Request.Form["on_TIMEh"];

                    var einst2 = (from p in Db.EXP_EDOC_INSTRU
                                  join b in Db.SEC_JOB_TYPE_DOC on p.JDTY_JDTY_ID equals b.JDTY_ID
                                  where p.EEDO_EEDO_ID == ideedo && p.CUT_STAT == "1" && b.ETDO_ETDO_ID == 21 && b.ACTIV_NAME == "CREATOR"
                                  select p).FirstOrDefault();

                    string stime = einst2.TIME_ISTA.ToString();

                    if (stime == "0")
                    {
                        if (offdate2.CompareTo(onDATE2) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                        }
                        else
                        {
                            if (offdate2.CompareTo(onDATE2) == 0)
                            {
                                if (off_TIME2.CompareTo(on_TIME2) > 0)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (offdate2.CompareTo(onDATE2) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                        }
                        else
                        {
                            if (off_TIME2.CompareTo(on_TIME2) > 0)
                            {
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                            }
                        }
                    }
                }

                if (Request.Form["type_doc"] != "" && Request.Form["type_doc"] != null)
                {
                    int typeo = Convert.ToInt32(Request.Form["type_doc"].ToString());
                    if (typeo == 2)
                    {
                        if (Request.Form["defectnumber"] == "")
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "شماره دیفکت را انتخاب نمایید" }.ToJson();
                        }
                        else
                        {
                            int defectid = Convert.ToInt32(Request.Form["defectnumber"].ToString());
                            var qdefect = (from b in Db.EXP_EXPI_DOC
                                           join k in Db.EXP_EDOC_INSTRU on b.EEDO_ID equals k.EEDO_EEDO_ID
                                           where b.EEDO_ID == defectid
                                           select new { b.EFUN_EFUN_ID, b.ORGA_CODE, b.EPOL_EPOL_ID, k.EINS_EINS_ID, k.EPIU_EPIU_ID, k.ETBY_ETBY_ID, k.OFF_DATE, k.ON_DATE }).FirstOrDefault();

                            int post = Convert.ToInt32(Request.Form["EPOL_EPOL_ID_requ"]);
                            int function = Convert.ToInt32(Request.Form["function"]);

                            int bay = 0;
                            int inst = 0;
                            int tinst = 0;

                            if (Request.Form["etby1"] != "")
                            {
                                bay = Convert.ToInt32(Request.Form["etby1"]);
                                inst = Convert.ToInt32(Request.Form["POSTINST1"]);
                                if (post != qdefect.EPOL_EPOL_ID ||
                                    function != qdefect.EFUN_EFUN_ID || inst != qdefect.EPIU_EPIU_ID ||
                                    bay != qdefect.ETBY_ETBY_ID)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات درخواست با دیفکت انتخاب شده فرق نباید کند" }.ToJson();
                                }
                            }
                            if (Request.Form["POSTINST"] != "")
                            {
                                inst = Convert.ToInt32(Request.Form["POSTINST"]);
                                if (post != qdefect.EPOL_EPOL_ID ||
                                    function != qdefect.EFUN_EFUN_ID || inst != qdefect.EPIU_EPIU_ID)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات درخواست با دیفکت انتخاب شده فرق نباید کند" }.ToJson();
                                }
                            }
                            if (Request.Form["einsrequest"] != "")
                            {
                                tinst = Convert.ToInt32(Request.Form["einsrequest"]);
                                if (post != qdefect.EPOL_EPOL_ID || function != qdefect.EFUN_EFUN_ID || tinst != qdefect.EINS_EINS_ID
                                )
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات درخواست با دیفکت انتخاب شده فرق نباید کند" }.ToJson();
                                }
                            }
                        }
                    }
                    if (typeo == 21 && (Request.Form["defectnumber"] != "" || Request.Form["prognumber"] != ""))
                    {
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = " دیفکت و برنامه نباید انتخاب شود" }.ToJson();
                    }

                    if (typeo == 101)
                    {
                        if (Request.Form["prognumber"] == "")
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "شماره برنامه را انتخاب نمایید" }.ToJson();
                        }
                        else
                        {
                            int progid = Convert.ToInt32(Request.Form["prognumber"].ToString());
                            var qdefect = (from k in Db.EXP_EDOC_INSTRU
                                           where k.EDIN_ID == progid
                                           select new { k.EFUN_EFUN_ID, k.ORGA_CODE, k.EPOL_EPOL_ID, k.EINS_EINS_ID, k.EPIU_EPIU_ID, k.ETBY_ETBY_ID, k.ON_DATE, k.OFF_DATE }).FirstOrDefault();
                            int post = Convert.ToInt32(Request.Form["EPOL_EPOL_ID_requ"]);
                            int function = Convert.ToInt32(Request.Form["function"]);
                            int bay = 0;
                            int inst = 0;
                            int tinst = 0;
                            if (Request.Form["etby1"] != "")
                            {
                                bay = Convert.ToInt32(Request.Form["etby1"]);
                                inst = Convert.ToInt32(Request.Form["POSTINST1"]);
                                if (post != qdefect.EPOL_EPOL_ID || function != qdefect.EFUN_EFUN_ID || inst != qdefect.EPIU_EPIU_ID ||
                                    bay != qdefect.ETBY_ETBY_ID || qdefect.OFF_DATE != offdate || onDATE != qdefect.ON_DATE)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات درخواست با برنامه انتخاب شده فرق نباید کند" }.ToJson();
                                }
                            }
                            if (Request.Form["POSTINST"] != "")
                            {
                                inst = Convert.ToInt32(Request.Form["POSTINST"]);
                                if (post != qdefect.EPOL_EPOL_ID || function != qdefect.EFUN_EFUN_ID || inst != qdefect.EPIU_EPIU_ID || qdefect.OFF_DATE != offdate || onDATE != qdefect.ON_DATE)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات درخواست با برنامه انتخاب شده فرق نباید کند" }.ToJson();
                                }
                            }
                            if (Request.Form["einsrequest"] != "")
                            {
                                tinst = Convert.ToInt32(Request.Form["einsrequest"]);
                                if (post != qdefect.EPOL_EPOL_ID || function != qdefect.EFUN_EFUN_ID || tinst != qdefect.EINS_EINS_ID || qdefect.OFF_DATE != offdate || onDATE != qdefect.ON_DATE)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات درخواست با برنامه انتخاب شده فرق نباید کند" }.ToJson();
                                }
                            }
                        }
                    }
                }

                if (curent == "CONTROLCENTER")
                {
                    string offdatep = Request.Form["offdatep"];

                    string offtimep = Request.Form["offtimep"];
                    string ondatep = Request.Form["ondatep"];
                    string ontimep = Request.Form["ontimep"];
                    string offDATEc = string.Empty;
                    string off_TIMEcontrol = string.Empty;
                    string onDATEc = string.Empty;
                    string on_TIMEcontrol = string.Empty;

                    if (Request.Form["offDATEc"] != "" && Request.Form["offDATEc"] != null)
                    {
                        offDATEc = Request.Form["offDATEc"];
                    }
                    else
                    {
                        if (Request.Form["datecheck"] != "" && Request.Form["datecheck"] != null)
                        {
                            offDATEc = Request.Form["datecheck"];
                        }
                    }
                    if (Request.Form["off_TIMEcontrol"] != "" && Request.Form["off_TIMEcontrol"] != null)
                    {
                        off_TIMEcontrol = Request.Form["off_TIMEcontrol"];
                    }
                    if (Request.Form["onDATEc"] != "" && Request.Form["onDATEc"] != null)
                    {
                        onDATEc = Request.Form["onDATEc"];
                    }
                    else
                    {
                        if (Request.Form["datecheck"] != "" && Request.Form["datecheck"] != null)
                        {
                            onDATEc = Request.Form["datecheck"];
                        }
                    }
                    if (Request.Form["on_TIMEcontrol"] != "" && Request.Form["on_TIMEcontrol"] != null)
                    {
                        on_TIMEcontrol = Request.Form["on_TIMEcontrol"];
                    }

                    if (Request.Form["stime1"] == "0")
                    {
                        if (offDATEc.CompareTo(offdatep) < 0 || offDATEc.CompareTo(ondatep) > 0 || onDATEc.CompareTo(offdatep) < 0 || onDATEc.CompareTo(ondatep) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع و پایان اجرا بایتس مساوی یا فی بین تاریخ های شروع و پایان برنامه ریزی باشد " }.ToJson();
                        }
                        if (offDATEc.CompareTo(offdatep) == 0)
                        {
                            if (off_TIMEcontrol.CompareTo(offtimep) < 0)
                            {
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع اجرا از زمان شروع برنامه بزرگتر و مساوی باید باشد" }.ToJson();
                            }
                        }
                        if (offDATEc.CompareTo(ondatep) == 0)
                        {
                            if (off_TIMEcontrol.CompareTo(ontimep) > 0)
                            {
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع اجرا از زمان پایان برنامه کوچکتر و مساوی باید باشد" }.ToJson();
                            }
                        }
                        if (onDATEc.CompareTo(offdatep) == 0)
                        {
                            if (on_TIMEcontrol.CompareTo(offtimep) < 0)
                            {
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان پایان اجرا از زمان شروع برنامه یزرگتر و مساوی باید باشد" }.ToJson();
                            }
                        }

                        if (onDATEc.CompareTo(ondatep) == 0)
                        {
                            if (on_TIMEcontrol.CompareTo(ontimep) > 0)
                            {
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان پایان اجرا از زمان پایان برنامه کوچکتر و مساوی باید باشد" }.ToJson();
                            }
                        }

                        if (offDATEc.CompareTo(onDATEc) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع اجرا باید مساوی یاکوچکتراز پایان اجرا  باشد " }.ToJson();
                        }
                        if (offDATEc.CompareTo(onDATEc) == 0)
                        {
                            if (off_TIMEcontrol.CompareTo(on_TIMEcontrol) > 0)
                            {
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان پایان اجرا از زمان شروع اجرا یزرگتر و مساوی باید باشد" }.ToJson();
                            }
                        }
                    }
                    else
                    {
                        if (off_TIMEcontrol.CompareTo(on_TIMEcontrol) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان پایان اجرا از زمان شروع اجرا یزرگتر و مساوی باید باشد" }.ToJson();
                        }

                        if (off_TIMEcontrol.CompareTo(offtimep) < 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع اجرا از زمان شروع برنامه یزرگتر و مساوی باید باشد" }.ToJson();
                        }
                        if (off_TIMEcontrol.CompareTo(ontimep) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع اجرا از زمان پایان برنامه کوچکتر و مساوی باید باشد" }.ToJson();
                        }
                        if (on_TIMEcontrol.CompareTo(offtimep) < 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان پایان اجرا از زمان شروع برنامه یزرگتر و مساوی باید باشد" }.ToJson();
                        }
                        if (on_TIMEcontrol.CompareTo(ontimep) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان پایان اجرا از زمان پایان برنامه کوچکتر و مساوی باید باشد" }.ToJson();
                        }
                    }
                }
                //AsrWorkFlowProcess p1 = new AsrWorkFlowProcess(Convert.ToInt32(notid));
                //string curent = p1.CurrentStat;
                //if (curent == "?")
                //{
                //    if (idtype == 2)
                //    { curent = "CRATEOR"; }
                //    if (idtype == 21)
                //    { curent = "CREATOR"; }
                //}
            }
            var item_type_doc = from b in Db.SEC_JOB_TYPE_DOC
                                join c in Db.SEC_JOB_TYPE_ITEM on b.JDTY_ID equals c.JDTY_JDTY_ID
                                join k in Db.EXP_ITEM_TYPE_DOC on c.EITY_EITY_ID equals k.EITY_ID
                                orderby k.EITY_ORDE
                                where k.ETDO_ETDO_ID == idtype && k.ACTV_TYPE == "1" && b.ACTIV_NAME == curent && c.ACTI_TYPE == 0 && k.EITY_TYPE == "248"
                                select k;

            var chkl = (from b in Db.EXP_EXPI_DOC where b.EEDO_ID == ideedo select b).FirstOrDefault();

            foreach (EXP_ITEM_TYPE_DOC IType in item_type_doc)
            {
                switch (IType.EITY_DESC)
                {
                    case "EEDO_DESC":
                        chkl.EEDO_DESC = Request.Form["EXP_EXPI_DOC.EEDO_DESC"];
                        break;
                    case "DEFC_DESC":
                        chkl.DEFC_DESC = Request.Form["EXP_EXPI_DOC.DEFC_DESC"];

                        break;
                    case "DOC_NUMB":
                        chkl.DOC_NUMB = Request.Form["EXP_EXPI_DOC.DOC_NUMB"];
                        break;
                    case "FIRS_DATE":
                        chkl.FIRS_DATE = Request.Form["FDATE"];
                        break;
                    case "FIRS_TIME":
                        chkl.FIRS_TIME = Request.Form["FIRS_TIME"];
                        break;
                    case "END_DATE":
                        chkl.END_DATE = Request.Form["EDATE"];
                        break;
                    case "END_TIME":
                        chkl.END_TIME = Request.Form["END_TIME"];
                        break;
                    case "EEDO_DATE":
                        chkl.EEDO_DATE = Request.Form["EXP_EXPI_DOC.EEDO_DATE"];
                        break;
                    case "EEDO_TIME":
                        chkl.EEDO_TIME = Request.Form["EEDO_TIME"];
                        break;
                    case "EEDO_YEAR":
                        chkl.EEDO_YEAR = Request.Form["EEDO_YEAR"];
                        break;
                    case "ACTI_NACT":
                        chkl.ACTI_NACT = Request.Form["EXP_EXPI_DOC.ACTI_NACT"];
                        break;
                    case "LETT_TYPE":
                        chkl.LETT_TYPE = Request.Form["EXP_EXPI_DOC.LETT_TYPE"];
                        break;
                    case "EPOL_EPOL_ID":
                        if (idtype == 21 && Request.Form["EPOL_EPOL_ID_requ"] != null)
                        {
                            chkl.EPOL_EPOL_ID = Convert.ToInt32(Request.Form["EPOL_EPOL_ID_requ"]);
                        }
                        else
                        {
                            if (idtype == 2)
                            {
                                chkl.EPOL_EPOL_ID = Convert.ToInt32(Request.Form["post_name"]);
                            }
                        }
                        break;
                    case "EFUN_EFUN_ID":
                        if (idtype == 21 && Request.Form["function"] != null)
                        {
                            chkl.EFUN_EFUN_ID = Convert.ToInt32(Request.Form["function"]);
                        }
                        else
                        {
                            if (idtype == 2)
                            {
                                chkl.EFUN_EFUN_ID = Convert.ToInt32(Request.Form["EFUN_EFUN_ID1"]);
                            }
                        }
                        break;
                    case "ORGAN_CODE":
                        if (idtype == 21 && Request.Form["ORGA_CODE_request"] != null)
                        {
                            chkl.ORGA_MANA_ASTA_CODE = "7";
                            chkl.ORGA_MANA_CODE = "6";
                            chkl.ORGA_CODE = Request.Form["ORGA_CODE_request"];
                        }
                        else
                        {
                            if (idtype == 2)
                            {
                                chkl.ORGA_MANA_ASTA_CODE = Request.Form["EXP_EXPI_DOC.ORGA_MANA_ASTA_CODE"];
                                chkl.ORGA_MANA_CODE = Request.Form["EXP_EXPI_DOC.ORGA_MANA_CODE"];
                                chkl.ORGA_CODE = Request.Form["EXP_EXPI_DOC.ORGA_CODE"];
                            }
                        }
                        break;
                    case "CPRO_PRJ_CODE":
                        chkl.CPRO_CPLA_PLN_CODE = Convert.ToInt16(Request.Form["EXP_EXPI_DOC.CPRO_CPLA_PLN_CODE"]);
                        chkl.CPRO_PRJ_CODE = Convert.ToInt16(Request.Form["EXP_EXPI_DOC.CPRO_PRJ_CODE"]);

                        break;
                    case "ANPT_ANPT_ID":
                        chkl.ANPT_ANPT_ID = Convert.ToInt32(Request.Form["EXP_EXPI_DOC.ANPT_ANPT_ID"]);
                        break;
                    case "OUT_FUNC":

                        chkl.OUT_FUNC = Request.Form["DESC"];
                        break;
                    //case "descprogram":
                    //    chkl.PROG_DESC = Request.Form["EXP_EXPI_DOC.PROG_DESC"];
                    //    break;
                    case "descpost":
                        chkl.WORK_DESC = Request.Form["EXP_EXPI_DOC.WORK_DESC"];
                        break;
                }
            }

            Db.SaveChanges();

            int idinst = 0;
            //cntx.Entry(objecttemp).State = EntityState.Modified;
            //cntx.SaveChanges();

            if (idtype == 2)
            {
                var rel1 = new EXP_EDOC_INSTRU();
                string POSTINST = Request.Form["POSTINSTRUMEN"];

                if (!string.IsNullOrEmpty(POSTINST))
                {
                    string sql = string.Format("delete from EXP_EDOC_INSTRU  where  EEDO_EEDO_ID={0}", objecttemp.EEDO_ID);
                    Db.Database.ExecuteSqlCommand(sql);

                    idinst = Convert.ToInt32(POSTINST);

                    int id = (from b in Db.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == objecttemp.EEDO_ID && b.EPIU_EPIU_ID == idinst select b.EDIN_ID).Count();
                    if (id == 0)
                    {
                        rel1.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                        rel1.EPIU_EPIU_ID = Convert.ToInt32(POSTINST);
                        rel1.EPOL_EPOL_ID = Convert.ToInt32(Request.Form["post_name"]);
                        rel1.ETDO_ETDO_ID = objecttemp.ETDO_ETDO_ID;
                        Db.EXP_EDOC_INSTRU.Add(rel1);
                        Db.SaveChanges();
                    }
                }

                string Org1 = string.Empty;
                short Ope1 = 0;
                int per = 0;
                string organn = Request.Form["ORGA_CODEconf1"];
                if (!string.IsNullOrEmpty(organn))
                {
                    if (Request.Form["ORGA_CODEconf1"] != "")
                        Org1 = Request.Form["ORGA_CODEconf1"].ToString();
                    if (Request.Form["PRSN_EMP_NUMB1"] != "")
                        Ope1 = short.Parse(Request.Form["PRSN_EMP_NUMB1"].ToString());
                    if (Request.Form["PRSN_EMP_out"] != "")
                        per = Convert.ToInt16(Request.Form["PRSN_EMP_out"].ToString());

                    var rel2 = new EXP_SUPL_DOC();

                    string sqlsupl = string.Format("delete from EXP_SUPL_DOC  where POSI_TYEP='0' and  EEDO_EEDO_ID={0}", objecttemp.EEDO_ID);
                    Db.Database.ExecuteSqlCommand(sqlsupl);

                    if (Ope1 != 0)
                    {
                        int idm = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == objecttemp.EEDO_ID && b.PRSN_EMP_NUMB == Ope1 select b.ESUD_ID).Count();
                        if (idm == 0)
                        {
                            rel2.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                            rel2.ORGA_CODE = Org1;
                            rel2.ORGA_MANA_CODE = "6";
                            rel2.ORGA_MANA_ASTA_CODE = "7";
                            rel2.POSI_TYEP = "0";
                            rel2.PRSN_EMP_NUMB = Ope1;

                            Db.EXP_SUPL_DOC.Add(rel2);
                            Db.SaveChanges();
                        }
                    }
                    else
                    {
                        if (per != 0)
                        {
                            int idm = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == objecttemp.EEDO_ID && b.OUTP_OUTP_ID == per select b.ESUD_ID).Count();
                            if (idm == 0)
                            {
                                rel2.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                                rel2.ORGA_CODE = Org1;
                                rel2.ORGA_MANA_CODE = "6";
                                rel2.ORGA_MANA_ASTA_CODE = "7";
                                rel2.POSI_TYEP = "0";
                                rel2.OUTP_OUTP_ID = per;

                                Db.EXP_SUPL_DOC.Add(rel2);
                                Db.SaveChanges();
                            }
                        }
                    }
                }
                var rel3 = new EXP_RELATION_DOC();
                string EEDO_EEDO_ID_R = Request.Form["EEDO_EEDO_ID_R"];
                if (!string.IsNullOrEmpty(EEDO_EEDO_ID_R))
                {
                    string sql = string.Format("delete from EXP_RELATION_DOC  where EEDO_EEDO_ID_R in (select eedo_id from EXP_EXPI_DOC where etdo_etdo_id!=21) and  EEDO_EEDO_ID={0}", EEDO_ID);
                    Db.Database.ExecuteSqlCommand(sql);
                    rel3.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                    rel3.EEDO_EEDO_ID_R = Convert.ToInt32(EEDO_EEDO_ID_R);
                    Db.EXP_RELATION_DOC.Add(rel3);
                    Db.SaveChanges();
                }
            }
            else
            {
                decimal edin = 0;
                decimal eprog = 0;
                if (Request.Form["defectnumber"] != "" && Request.Form["defectnumber"] != null)
                {
                    edin = Convert.ToDecimal(Request.Form["defectnumber"]);
                }

                if (Request.Form["prognumber"] != "" && Request.Form["prognumber"] != null)
                {
                    eprog = Convert.ToDecimal(Request.Form["prognumber"]);
                }

                var rel = new EXP_RELATION_DOC();
                int eedoid = Convert.ToInt32(EEDO_ID);
                if (eprog != 0)
                {
                    string cou = (from b in Db.EXP_RELATION_DOC where b.EEDO_EEDO_ID == eedoid && b.EDIN_EDIN_ID == eprog select b).Count().ToString();
                    int rc = Convert.ToInt32(cou);
                    if (rc == 0)
                    {
                        rel.EEDO_EEDO_ID = eedoid;
                        rel.EDIN_EDIN_ID = eprog;
                        Db.EXP_RELATION_DOC.Add(rel);
                        Db.SaveChanges();
                    }
                }

                if (edin != 0)
                {
                    string cou = (from b in Db.EXP_RELATION_DOC where b.EEDO_EEDO_ID == eedoid && b.EEDO_EEDO_ID_R == edin select b).Count().ToString();
                    int rrc = Convert.ToInt32(cou);
                    if (rrc == 0)
                    {
                        rel.EEDO_EEDO_ID = eedoid;
                        rel.EEDO_EEDO_ID_R = edin;
                        Db.EXP_RELATION_DOC.Add(rel);
                        Db.SaveChanges();
                    }
                }

                string sqc = (from b in Db.SEC_JOB_TYPE_DOC where b.ACTIV_NAME == curent && b.ETDO_ETDO_ID == idtype select b.JDTY_ID).FirstOrDefault().ToString();
                int isgc = Convert.ToInt16(sqc);
                int icheck = 0;

                int ccount = (from b in Db.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == ideedo && b.CUT_STAT == "1" && b.ATTG_STATT == null && b.JDTY_JDTY_ID == isgc select b).Count();
                var rel1 = new EXP_EDOC_INSTRU();
                if (curent == "CONTROLCENTER")
                {
                    if (Request.Form["stime1"] == "0")
                    {
                        icheck = 0;
                    }
                    else
                    {
                        var sqprog = (from b in Db.EXP_EDOC_INSTRU
                                      where b.EEDO_EEDO_ID == ideedo &&
                                            b.CUT_STAT == "1" && b.JDTY_JDTY_ID == 97
                                      select b).FirstOrDefault();

                        if (sqprog.OFF_DATE == sqprog.ON_DATE)
                            icheck = 0;
                        else
                        {
                            string currentday = Request.Form["datecheck"];
                            icheck = 1;
                            ccount = (from b in Db.EXP_EDOC_INSTRU
                                      where b.EEDO_EEDO_ID == ideedo &&
                                            b.CUT_STAT == "1" && b.ATTG_STATT == null && b.JDTY_JDTY_ID == isgc && b.OFF_DATE == currentday
                                      select b).Count();
                        }
                    }
                }

                if (ccount == 0 && (curent == "DISKERMAN" || curent == "DISBANDAR" || curent == "DISHORMOZGAN" ||
                                    curent == "SECCONTROL" || curent == "LINEORGAN" || curent == "ENTEGHAL" || curent == "FIXORGANE" || curent == "FIXORGANE"))
                {
                    var qrel = (from b in Db.EXP_EDOC_INSTRU
                                join j in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals j.JDTY_ID
                                where b.EEDO_EEDO_ID == ideedo && j.ACTIV_NAME == "CREATOR" && j.ETDO_ETDO_ID == 21
                                select b);
                    foreach (EXP_EDOC_INSTRU inst in qrel)
                    {
                        inst.JDTY_JDTY_ID = isgc;
                        inst.ETDO_ETDO_ID = 21;
                        decimal eedin = inst.EDIN_ID;

                        if (Request.Form["accept"] != "" && Request.Form["accept"] != null)
                        {
                            inst.EDIN_STIN = "0";
                        }
                        if (Request.Form["diac"] != "" && Request.Form["diac"] != null)
                        {
                            inst.EDIN_STIN = "1";
                        }

                        if (Request.Form["PROGDESC"] != "" && Request.Form["PROGDESC"] != null)
                        {
                            inst.ATTG_SPIC = Request.Form["PROGDESC"];
                        }

                        if (Request.Form["ofDATEprog"] != "" && Request.Form["ofDATEprog"] != null)
                        {
                            inst.OFF_DATE = Request.Form["ofDATEprog"];
                        }
                        if (Request.Form["off_TIMEprog"] != "" && Request.Form["off_TIMEprog"] != null)
                        {
                            inst.OFF_TIME = Request.Form["off_TIMEprog"];
                        }
                        if (Request.Form["onDATEprog"] != "" && Request.Form["onDATEprog"] != null)
                        {
                            inst.ON_DATE = Request.Form["onDATEprog"];
                        }
                        if (Request.Form["on_TIMEprog"] != "" && Request.Form["on_TIMEprog"] != null)
                        {
                            inst.ON_TIME = Request.Form["on_TIMEprog"];
                        }

                        if (Request.Form["offDATEh"] != "" && Request.Form["offDATEh"] != null)
                        {
                            inst.OFF_DATE = Request.Form["offDATEh"];
                        }
                        if (Request.Form["off_TIMEh"] != "" && Request.Form["off_TIMEh"] != null)
                        {
                            inst.OFF_TIME = Request.Form["off_TIMEh"];
                        }
                        if (Request.Form["onDATEh"] != "" && Request.Form["onDATEh"] != null)
                        {
                            inst.ON_DATE = Request.Form["onDATEh"];
                        }
                        if (Request.Form["on_TIMEh"] != "" && Request.Form["on_TIMEh"] != null)
                        {
                            inst.ON_TIME = Request.Form["on_TIMEh"];
                        }

                        if (Request.Form["cancel"] != "" && Request.Form["cancel"] != null)
                        {
                            inst.EREJ_EREJ_ID = Convert.ToInt16(Request.Form["cancel"].ToString());
                        }
                        if (Request.Form["HDESC"] != "" && Request.Form["HDESC"] != null)
                        {
                            inst.ATTG_SPIC = Request.Form["HDESC"];
                        }
                        if (Request.Form["hcancel"] != "" && Request.Form["hcancel"] != null)
                        {
                            inst.EREJ_EREJ_ID = Convert.ToInt16(Request.Form["hcancel"].ToString());
                        }

                        Db.EXP_EDOC_INSTRU.Add(inst);
                        Db.SaveChanges();
                        var q = (from b in Db.EXP_EDOC_INSTRU where b.EDIN_ID == inst.EDIN_ID select b).FirstOrDefault();
                        q.EDIN_EDIN_ID = eedin;
                        Db.SaveChanges();
                    }
                }
                else if (ccount == 0 && curent == "PROGRAMER")
                {
                    int m = 0;

                    m = Db.Database.SqlQuery<int>("select EXP_EDIN_Q.nextval from dual").FirstOrDefault();

                    string sql = "insert into EXP_EDOC_INSTRU(EDIN_ID, CRET_BY, CRET_DATE, OFF_DATE, OFF_TIME, ON_DATE, ON_TIME, TIME_ISTA, " +
                                 "EDIN_STIN, INST_STAT, EART_STAT, CUST_STAT, MDFY_BY, MDFY_DATE, EEDO_EEDO_ID, EFUN_EFUN_ID, EINS_EINS_ID, " +
                                 "EOFS_EOFS_ID, EPIU_EPIU_ID, EPOL_EPOL_ID, EUNL_EUNL_ID, CONT_FUN, CUT_STAT, OPER_STAT, FUNC_TYPE, ETDO_ETDO_ID,EDIN_EDIN_ID,JDTY_JDTY_ID)" +
                                 " select " + m + "+rownum, user,sysdate, OFF_DATE, OFF_TIME, ON_DATE, ON_TIME, TIME_ISTA, EDIN_STIN, INST_STAT, EART_STAT, CUST_STAT, " +
                                 "MDFY_BY, MDFY_DATE, EEDO_EEDO_ID, EFUN_EFUN_ID, EINS_EINS_ID, EOFS_EOFS_ID, EPIU_EPIU_ID, EPOL_EPOL_ID, EUNL_EUNL_ID, CONT_FUN, CUT_STAT, " +
                                 "OPER_STAT, FUNC_TYPE, ETDO_ETDO_ID,EDIN_ID,97  from (select user,sysdate,OFF_DATE, OFF_TIME, ON_DATE, ON_TIME, TIME_ISTA, " +
                                 "EDIN_STIN, INST_STAT, EART_STAT, CUST_STAT, MDFY_BY, MDFY_DATE, EEDO_EEDO_ID, EFUN_EFUN_ID, EINS_EINS_ID, EOFS_EOFS_ID, EPIU_EPIU_ID, " +
                                 "EPOL_EPOL_ID, EUNL_EUNL_ID, CONT_FUN, CUT_STAT, OPER_STAT, FUNC_TYPE, ETDO_ETDO_ID,EDIN_ID from EXP_EDOC_INSTRU where EEDO_EEDO_ID=" + ideedo + " and JDTY_JDTY_ID=91 and ETDO_ETDO_ID=21)";
                    Db.Database.ExecuteSqlCommand(sql);

                    var q1 = (from b in Db.EXP_EDOC_INSTRU
                              where b.EEDO_EEDO_ID == ideedo && b.JDTY_JDTY_ID == 97
                              select b);
                    foreach (EXP_EDOC_INSTRU q in q1)
                    {
                        m = Db.Database.SqlQuery<int>("select EXP_EDIN_Q.nextval from dual").FirstOrDefault();

                        //decimal eedint = instt.EDIN_ID;

                        ////instt.ATTG_SPIC = null;
                        ////instt.OFF_DATE = null;
                        ////instt.OFF_TIME = null;
                        ////instt.ON_DATE = null;
                        ////instt.ON_TIME = null;
                        ////instt.EREJ_EREJ_ID = null;

                        //cntx.EXP_EDOC_INSTRU.Add(instt);
                        //cntx.SaveChanges();

                        // var q = (from b in cntx.EXP_EDOC_INSTRU where b.EDIN_ID == instt.EDIN_ID select b).FirstOrDefault();
                        q.JDTY_JDTY_ID = isgc;
                        //   q.ETDO_ETDO_ID = 21;
                        //  q.EDIN_EDIN_ID = eedint;
                        if (Request.Form["PROGDESC"] != "" && Request.Form["PROGDESC"] != null)
                        {
                            q.ATTG_SPIC = Request.Form["PROGDESC"];
                        }

                        if (Request.Form["ofDATEprog"] != "" && Request.Form["ofDATEprog"] != null)
                        {
                            q.OFF_DATE = Request.Form["ofDATEprog"];
                        }
                        if (Request.Form["off_TIMEprog"] != "" && Request.Form["off_TIMEprog"] != null)
                        {
                            q.OFF_TIME = Request.Form["off_TIMEprog"];
                        }
                        if (Request.Form["onDATEprog"] != "" && Request.Form["onDATEprog"] != null)
                        {
                            q.ON_DATE = Request.Form["onDATEprog"];
                        }
                        if (Request.Form["on_TIMEprog"] != "" && Request.Form["on_TIMEprog"] != null)
                        {
                            q.ON_TIME = Request.Form["on_TIMEprog"];
                        }

                        if (Request.Form["cancel"] != "" && Request.Form["cancel"] != null)
                        {
                            q.EREJ_EREJ_ID = Convert.ToInt16(Request.Form["cancel"].ToString());
                        }

                        Db.SaveChanges();
                    }
                }
                else if (curent == "CONTROLCENTER" && ccount == 0)
                {
                    int m = 0;

                    m = Db.Database.SqlQuery<int>("select EXP_EDIN_Q.nextval from dual").FirstOrDefault();

                    string sql = "insert into EXP_EDOC_INSTRU(EDIN_ID, CRET_BY, CRET_DATE,  TIME_ISTA, " +
                                 "EDIN_STIN, INST_STAT, EART_STAT, CUST_STAT, MDFY_BY, MDFY_DATE, EEDO_EEDO_ID, EFUN_EFUN_ID, EINS_EINS_ID, " +
                                 "EOFS_EOFS_ID, EPIU_EPIU_ID, EPOL_EPOL_ID, EUNL_EUNL_ID, CONT_FUN, CUT_STAT, OPER_STAT, FUNC_TYPE, ETDO_ETDO_ID,EDIN_EDIN_ID,JDTY_JDTY_ID)" +
                                 " select " + m + "+rownum, user,sysdate,  TIME_ISTA, EDIN_STIN, INST_STAT, EART_STAT, CUST_STAT, " +
                                 "MDFY_BY, MDFY_DATE, EEDO_EEDO_ID, EFUN_EFUN_ID, EINS_EINS_ID, EOFS_EOFS_ID, EPIU_EPIU_ID, EPOL_EPOL_ID, EUNL_EUNL_ID, CONT_FUN, CUT_STAT, " +
                                 "OPER_STAT, FUNC_TYPE, ETDO_ETDO_ID,EDIN_ID,101  from (select user,sysdate, TIME_ISTA, " +
                                 "EDIN_STIN, INST_STAT, EART_STAT, CUST_STAT, MDFY_BY, MDFY_DATE, EEDO_EEDO_ID, EFUN_EFUN_ID, EINS_EINS_ID, EOFS_EOFS_ID, EPIU_EPIU_ID, " +
                                 "EPOL_EPOL_ID, EUNL_EUNL_ID, CONT_FUN, CUT_STAT, OPER_STAT, FUNC_TYPE, ETDO_ETDO_ID,EDIN_ID from EXP_EDOC_INSTRU where EEDO_EEDO_ID=" + ideedo +
                                 " and JDTY_JDTY_ID=91 and ETDO_ETDO_ID=21)";
                    Db.Database.ExecuteSqlCommand(sql);

                    var q1 = (from b in Db.EXP_EDOC_INSTRU
                              where b.EEDO_EEDO_ID == ideedo && b.JDTY_JDTY_ID == 101 && b.OFF_DATE == null && b.ON_DATE == null
                              select b);
                    foreach (EXP_EDOC_INSTRU q in q1)
                    {
                        m = Db.Database.SqlQuery<int>("select EXP_EDIN_Q.nextval from dual").FirstOrDefault();

                        q.JDTY_JDTY_ID = isgc;

                        if (Request.Form["stop"] != "" && Request.Form["stop"] != null)
                        {
                            q.EDIN_STIN = "1";
                            if (Request.Form["ccancel"] != "" && Request.Form["ccancel"] != null)
                            {
                                q.EREJ_EREJ_ID = Convert.ToInt16(Request.Form["ccancel"].ToString());
                            }
                            else
                            {
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "دلیل لغو را مضخص نمایید" }.ToJson();
                            }
                        }

                        if (Request.Form["run"] != "" && Request.Form["run"] != null)
                        {
                            q.EDIN_STIN = "0";

                            if (Request.Form["offDATEc"] != "" && Request.Form["offDATEc"] != null)
                            {
                                q.OFF_DATE = Request.Form["offDATEc"];
                            }
                            else
                            {
                                if (Request.Form["datecheck"] != "" && Request.Form["datecheck"] != null)
                                {
                                    q.OFF_DATE = Request.Form["datecheck"];
                                }
                            }
                            if (Request.Form["off_TIMEcontrol"] != "" && Request.Form["off_TIMEcontrol"] != null)
                            {
                                q.OFF_TIME = Request.Form["off_TIMEcontrol"];
                            }
                            if (Request.Form["onDATEc"] != "" && Request.Form["onDATEc"] != null)
                            {
                                q.ON_DATE = Request.Form["onDATEc"];
                            }
                            else
                            {
                                if (Request.Form["datecheck"] != "" && Request.Form["datecheck"] != null)
                                {
                                    q.ON_DATE = Request.Form["datecheck"];
                                }
                            }
                            if (Request.Form["on_TIMEcontrol"] != "" && Request.Form["on_TIMEcontrol"] != null)
                            {
                                q.ON_TIME = Request.Form["on_TIMEcontrol"];
                            }

                            if (Request.Form["EDIN_MW"] != "" && Request.Form["EDIN_MW"] != null)
                            {
                                q.EDIN_MW = Request.Form["EDIN_MW"].ToString();
                            }
                            if (Request.Form["EDIN_OFF"] != "" && Request.Form["EDIN_OFF"] != null)
                            {
                                q.EDIN_OFF = Request.Form["EDIN_OFF"].ToString();
                            }
                            if (Request.Form["EDIN_MVH"] != "" && Request.Form["EDIN_MVH"] != null)
                            {
                                q.EDIN_MVH = Request.Form["EDIN_MVH"].ToString();
                            }
                        }
                        if (Request.Form["cDESC"] != "" && Request.Form["cDESC"] != null)
                        {
                            q.ATTG_SPIC = Request.Form["cDESC"];
                        }

                        Db.SaveChanges();
                    }
                }
                else
                {
                    rel1 = (from b in Db.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == ideedo && b.CUT_STAT == "1" && b.JDTY_JDTY_ID == isgc select b).FirstOrDefault();

                    string POSTINST = string.Empty;
                    string etby1 = string.Empty;
                    string eins = string.Empty;

                    int idetby1 = 0;
                    int idinstt = 0;
                    int postcode = 0;

                    if (Request.Form["POSTINST"] != "" && Request.Form["POSTINST"] != null)
                    {
                        POSTINST = Request.Form["POSTINST"];
                        idinst = Convert.ToInt32(POSTINST);
                        rel1.EPIU_EPIU_ID = idinst;
                    }

                    if (Request.Form["etby1"] != "" && Request.Form["etby1"] != null)
                    {
                        POSTINST = Request.Form["POSTINST1"];
                        idinst = Convert.ToInt32(POSTINST);
                        etby1 = Request.Form["etby1"];
                        idetby1 = Convert.ToInt32(etby1);
                        rel1.EPIU_EPIU_ID = idinst;
                        rel1.ETBY_ETBY_ID = idetby1;
                    }
                    if (Request.Form["einsrequest"] != "" && Request.Form["einsrequest"] != null)
                    {
                        eins = Request.Form["einsrequest"];
                        idinstt = Convert.ToInt32(eins);
                        rel1.EINS_EINS_ID = idinstt;
                    }
                    if (Request.Form["EPOL_EPOL_ID_requ"] != "" && Request.Form["EPOL_EPOL_ID_requ"] != null)
                    {
                        postcode = Convert.ToInt32(Request.Form["EPOL_EPOL_ID_requ"]);
                        rel1.EPOL_EPOL_ID = postcode;
                    }

                    if (Request.Form["Unitvolt"] != "" && Request.Form["Unitvolt"] != null)
                    {
                        rel1.EUNL_EUNL_ID = Convert.ToInt16(Request.Form["Unitvolt"].ToString());
                    }
                    if (Request.Form["offstat"] != "" && Request.Form["offstat"] != null)
                    {
                        rel1.EOFS_EOFS_ID = Convert.ToInt32(Request.Form["offstat"]);
                    }

                    if (Request.Form["ofDATEreq"] != "" && Request.Form["ofDATEreq"] != null)
                    {
                        rel1.OFF_DATE = Request.Form["ofDATEreq"];
                    }
                    if (Request.Form["off_TIME"] != "" && Request.Form["off_TIME"] != null)
                    {
                        rel1.OFF_TIME = Request.Form["off_TIME"];
                    }
                    if (Request.Form["onDATEreq"] != "" && Request.Form["onDATEreq"] != null)
                    {
                        rel1.ON_DATE = Request.Form["onDATEreq"];
                    }

                    if (Request.Form["on_TIME"] != "" && Request.Form["on_TIME"] != null)
                    {
                        rel1.ON_TIME = Request.Form["on_TIME"];
                    }

                    if (Request.Form["stattime"] != "" && Request.Form["stattime"] != null)
                    {
                        rel1.TIME_ISTA = Request.Form["stattime"];
                        rel1.CONT_FUN = time;
                    }
                    if (Request.Form["INST_STAT"] != "" && Request.Form["INST_STAT"] != null)
                    {
                        rel1.INST_STAT = Request.Form["INST_STAT"];
                    }
                    if (Request.Form["EART_STAT"] != "" && Request.Form["EART_STAT"] != null)
                    {
                        rel1.EART_STAT = Request.Form["EART_STAT"];
                    }
                    if (Request.Form["CUST_STAT"] != "" && Request.Form["CUST_STAT"] != null)
                    {
                        rel1.CUST_STAT = Request.Form["CUST_STAT"];
                    }

                    if (Request.Form["ofDATEprog"] != "" && Request.Form["ofDATEprog"] != null)
                    {
                        rel1.OFF_DATE = Request.Form["ofDATEprog"];
                    }
                    if (Request.Form["off_TIMEprog"] != "" && Request.Form["off_TIMEprog"] != null)
                    {
                        rel1.OFF_TIME = Request.Form["off_TIMEprog"];
                    }
                    if (Request.Form["onDATEprog"] != "" && Request.Form["onDATEprog"] != null)
                    {
                        rel1.ON_DATE = Request.Form["onDATEprog"];
                    }

                    if (Request.Form["on_TIMEprog"] != "" && Request.Form["on_TIMEprog"] != null)
                    {
                        rel1.ON_TIME = Request.Form["on_TIMEprog"];
                    }
                    if (Request.Form["PROGDESC"] != "" && Request.Form["PROGDESC"] != null)
                    {
                        rel1.ATTG_SPIC = Request.Form["PROGDESC"];
                    }
                    if (Request.Form["cancel"] != "" && Request.Form["cancel"] != null)
                    {
                        rel1.EREJ_EREJ_ID = Convert.ToInt16(Request.Form["cancel"].ToString());
                    }
                    else
                    {
                        if (Request.Form["cancel"] == "")
                        {
                            rel1.EREJ_EREJ_ID = null;
                        }
                    }

                    if (Request.Form["accept"] != "" && Request.Form["accept"] != null)
                    {
                        rel1.EDIN_STIN = "0";
                        rel1.OFF_DATE = null;
                        rel1.OFF_TIME = null;
                        rel1.ON_DATE = null;
                        rel1.ON_TIME = null;
                        rel1.EREJ_EREJ_ID = null;
                    }
                    if (Request.Form["diac"] != "" && Request.Form["diac"] != null)
                    {
                        rel1.EDIN_STIN = "1";

                        if (Request.Form["offDATEh"] != "" && Request.Form["offDATEh"] != null)
                        {
                            rel1.OFF_DATE = Request.Form["offDATEh"];
                        }
                        if (Request.Form["off_TIMEh"] != "" && Request.Form["off_TIMEh"] != null)
                        {
                            rel1.OFF_TIME = Request.Form["off_TIMEh"];
                        }
                        if (Request.Form["onDATEh"] != "" && Request.Form["onDATEh"] != null)
                        {
                            rel1.ON_DATE = Request.Form["onDATEh"];
                        }
                        if (Request.Form["on_TIMEh"] != "" && Request.Form["on_TIMEh"] != null)
                        {
                            rel1.ON_TIME = Request.Form["on_TIMEh"];
                        }
                        if (Request.Form["hcancel"] != "" && Request.Form["hcancel"] != null)
                        {
                            rel1.EREJ_EREJ_ID = Convert.ToInt16(Request.Form["hcancel"].ToString());
                        }
                        else
                        {
                            if (Request.Form["hcancel"] == "")
                            {
                                rel1.EREJ_EREJ_ID = null;
                            }
                        }
                    }

                    if (Request.Form["HDESC"] != "" && Request.Form["HDESC"] != null)
                    {
                        rel1.ATTG_SPIC = Request.Form["HDESC"];
                    }

                    if (Request.Form["stop"] != "" && Request.Form["stop"] != null)
                    {
                        rel1.EDIN_STIN = "1";
                        rel1.OFF_DATE = null;
                        rel1.OFF_TIME = null;
                        rel1.ON_DATE = null;
                        rel1.ON_TIME = null;
                        rel1.EDIN_MW = null;
                        rel1.EDIN_MVH = null;
                        rel1.EDIN_OFF = null;

                        if (Request.Form["ccancel"] != "" && Request.Form["ccancel"] != null)
                        {
                            rel1.EREJ_EREJ_ID = Convert.ToInt16(Request.Form["ccancel"].ToString());
                        }
                        else
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "دلیل لغو را مضخص نمایید" }.ToJson();
                        }
                    }

                    if (Request.Form["run"] != "" && Request.Form["run"] != null)
                    {
                        rel1.EDIN_STIN = "0";
                        rel1.EREJ_EREJ_ID = null;

                        if (Request.Form["offDATEc"] != "" && Request.Form["offDATEc"] != null)
                        {
                            rel1.OFF_DATE = Request.Form["offDATEc"];
                        }
                        else
                        {
                            if (Request.Form["datecheck"] != "" && Request.Form["datecheck"] != null)
                            {
                                rel1.OFF_DATE = Request.Form["datecheck"];
                            }
                        }
                        if (Request.Form["off_TIMEcontrol"] != "" && Request.Form["off_TIMEcontrol"] != null)
                        {
                            rel1.OFF_TIME = Request.Form["off_TIMEcontrol"];
                        }
                        if (Request.Form["onDATEc"] != "" && Request.Form["onDATEc"] != null)
                        {
                            rel1.ON_DATE = Request.Form["onDATEc"];
                        }
                        else
                        {
                            if (Request.Form["datecheck"] != "" && Request.Form["datecheck"] != null)
                            {
                                rel1.ON_DATE = Request.Form["datecheck"];
                            }
                        }
                        if (Request.Form["on_TIMEcontrol"] != "" && Request.Form["on_TIMEcontrol"] != null)
                        {
                            rel1.ON_TIME = Request.Form["on_TIMEcontrol"];
                        }

                        if (Request.Form["EDIN_MW"] != "" && Request.Form["EDIN_MW"] != null)
                        {
                            rel1.EDIN_MW = Request.Form["EDIN_MW"].ToString();
                        }
                        if (Request.Form["EDIN_OFF"] != "" && Request.Form["EDIN_OFF"] != null)
                        {
                            rel1.EDIN_OFF = Request.Form["EDIN_OFF"].ToString();
                        }
                        if (Request.Form["EDIN_MVH"] != "" && Request.Form["EDIN_MVH"] != null)
                        {
                            rel1.EDIN_MVH = Request.Form["EDIN_MVH"].ToString();
                        }
                    }

                    if (Request.Form["cDESC"] != "" && Request.Form["cDESC"] != null)
                    {
                        rel1.ATTG_SPIC = Request.Form["cDESC"];
                    }

                    Db.SaveChanges();
                }

                var suplier = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == ideedo && b.POSI_TYEP == "2" select b).FirstOrDefault();
                var suplierj = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == ideedo && b.POSI_TYEP == "5" select b).FirstOrDefault();

                if (suplier != null)
                {
                    if (Request.Form["EPEX_EPEX_ID"] != "" && Request.Form["EPEX_EPEX_ID"] != null)
                        suplier.EPEX_EPEX_ID = Convert.ToInt32(Request.Form["EPEX_EPEX_ID"].ToString());

                    if (Request.Form["group"] != "" && Request.Form["group"] != null)
                        suplier.ESUD_DESC = Request.Form["group"];
                }
                else
                {
                    if (Request.Form["EPEX_EPEX_ID"] != "" && Request.Form["EPEX_EPEX_ID"] != null)
                    {
                        int sup = Convert.ToInt32(Request.Form["EPEX_EPEX_ID"].ToString());

                        var rel2 = new EXP_SUPL_DOC();
                        rel2.EEDO_EEDO_ID = ideedo;

                        rel2.POSI_TYEP = "2";
                        rel2.EPEX_EPEX_ID = sup;
                        rel2.ESUD_DESC = Request.Form["group"];
                        Db.EXP_SUPL_DOC.Add(rel2);
                        Db.SaveChanges();
                    }
                }
                if (suplierj != null)
                {
                    if (Request.Form["EPEX_EPEX_ID1"] != "" && Request.Form["EPEX_EPEX_ID1"] != null)
                        suplierj.EPEX_EPEX_ID = Convert.ToInt32(Request.Form["EPEX_EPEX_ID1"].ToString());
                }
                else
                {
                    if (Request.Form["EPEX_EPEX_ID1"] != "" && Request.Form["EPEX_EPEX_ID1"] != null)
                    {
                        int sup1 = Convert.ToInt32(Request.Form["EPEX_EPEX_ID1"].ToString());
                        var rel3 = new EXP_SUPL_DOC();
                        rel3.EEDO_EEDO_ID = ideedo;

                        rel3.POSI_TYEP = "5";
                        rel3.EPEX_EPEX_ID = sup1;

                        Db.EXP_SUPL_DOC.Add(rel3);
                        Db.SaveChanges();
                    }
                }

                Db.SaveChanges();

                if (curent == "CREATOR")
                {
                    string relation = (from b in Db.EXP_RELATION_DOC
                                       join x in Db.EXP_EXPI_DOC on b.EEDO_EEDO_ID_R equals x.EEDO_ID
                                       where b.EEDO_EEDO_ID == ideedo && x.ETDO_ETDO_ID == 21
                                       select b.ERED_ID).FirstOrDefault().ToString();
                    int rccc = Convert.ToInt32(relation);

                    if (Request.Form["DESC"] == "0")
                    {
                        var rel4 = new EXP_RELATION_DOC();
                        //  cntx.EXP_RELATION_DOC.Remove(relation);

                        if (rccc == 0)
                        {
                            if (!string.IsNullOrEmpty(Request.Form["master_doc"]))
                            {
                                rel4.EEDO_EEDO_ID = ideedo;
                                rel4.EEDO_EEDO_ID_R = Convert.ToInt32(Request.Form["master_doc"]);
                                Db.EXP_RELATION_DOC.Add(rel4);
                                Db.SaveChanges();
                            }
                        }
                        else
                        {
                            var qrelattd = (from b in Db.EXP_RELATION_DOC where b.EEDO_EEDO_ID == ideedo && b.ERED_ID == rccc select b).FirstOrDefault();
                            if (Request.Form["master_doc"] != "" && Request.Form["master_doc"] != null)
                                qrelattd.EEDO_EEDO_ID_R = Convert.ToInt32(Request.Form["master_doc"]);
                            Db.SaveChanges();
                        }
                    }
                    else
                    {
                        if (rccc != 0)
                        {
                            var r = (from b in Db.EXP_RELATION_DOC where b.ERED_ID == rccc select b).FirstOrDefault();

                            Db.EXP_RELATION_DOC.Remove(r);
                            Db.SaveChanges();
                        }
                    }
                }

                string Org1 = string.Empty;
                short Ope1 = 0;
                short Open1 = 0;

                if (Request.Form["ORGA_CODEconf1"] != null)
                {
                    string organn = Request.Form["ORGA_CODEconf1"];
                    if (!string.IsNullOrEmpty(organn))
                    {
                        if (Request.Form["ORGA_CODEconf1"] != "")
                            Org1 = Request.Form["ORGA_CODEconf1"].ToString();
                        if (Request.Form["PRSN_EMP_NUMB"] != "")
                            Ope1 = short.Parse(Request.Form["PRSN_EMP_NUMB"].ToString());
                        if (Request.Form["PRSN_EMP_NUMB_j"] != "")
                            Open1 = short.Parse(Request.Form["PRSN_EMP_NUMB_j"].ToString());

                        var rel2 = new EXP_SUPL_DOC();

                        if (Ope1 != 0)
                        {
                            int idm = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == objecttemp.EEDO_ID && b.PRSN_EMP_NUMB == Ope1 && b.POSI_TYEP == "3" select b.ESUD_ID).Count();
                            if (idm == 0)
                            {
                                rel2.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                                rel2.ORGA_CODE = Org1;
                                rel2.ORGA_MANA_CODE = "6";
                                rel2.ORGA_MANA_ASTA_CODE = "7";
                                rel2.POSI_TYEP = "3";
                                rel2.PRSN_EMP_NUMB = Ope1;

                                Db.EXP_SUPL_DOC.Add(rel2);
                                Db.SaveChanges();
                            }
                            else
                            {
                                var qidm = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == objecttemp.EEDO_ID && b.POSI_TYEP == "3" select b).FirstOrDefault();
                                qidm.PRSN_EMP_NUMB = Ope1;
                                qidm.ORGA_CODE = Org1;
                                Db.SaveChanges();
                            }
                        }
                        if (Open1 != 0)
                        {
                            int idm = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == objecttemp.EEDO_ID && b.OUTP_OUTP_ID == Open1 && b.POSI_TYEP == "6" select b.ESUD_ID).Count();
                            if (idm == 0)
                            {
                                rel2.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                                rel2.ORGA_CODE = Org1;
                                rel2.ORGA_MANA_CODE = "6";
                                rel2.ORGA_MANA_ASTA_CODE = "7";
                                rel2.POSI_TYEP = "6";
                                rel2.PRSN_EMP_NUMB = Open1;

                                Db.EXP_SUPL_DOC.Add(rel2);
                                Db.SaveChanges();
                            }
                            else
                            {
                                var qidm = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == objecttemp.EEDO_ID && b.POSI_TYEP == "6" select b).FirstOrDefault();
                                qidm.PRSN_EMP_NUMB = Open1;
                                qidm.ORGA_CODE = Org1;
                                Db.SaveChanges();
                            }
                        }
                    }
                }
            }

            string srow = Request.Form["count"];
            int row = Convert.ToInt16(srow);
            var value = new EXP_EITEM_DOC_VALUE();
            for (int i = 0; i <= row; i++)
            {
                string val = Request.Form[i.ToString()];
                if (string.IsNullOrEmpty(val))
                {
                    val = Request.Form["dman" + i.ToString()];
                }

                string EITY_ID = Request.Form["EITY_ID" + i.ToString()];
                if (!string.IsNullOrEmpty(EITY_ID))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        string sql = string.Format("DELETE FROM EXP_EITEM_DOC_VALUE WHERE EEDO_EEDO_ID={0} and EITY_EITY_ID={1}", EEDO_ID, EITY_ID);
                        Db.Database.ExecuteSqlCommand(sql);
                    }
                }

                if (!string.IsNullOrEmpty(val))
                {
                    value.EEDO_EEDO_ID = objecttemp.EEDO_ID;
                    value.EITY_EITY_ID = Convert.ToInt16(EITY_ID);
                    value.EIDR_VALUE = val;
                    Db.EXP_EITEM_DOC_VALUE.Add(value);
                    Db.SaveChanges();
                }
            }

            int etdo_id = Convert.ToInt32(objecttemp.ETDO_ETDO_ID);
            string bodymessage = string.Empty;
            string doc_name = (from b in Db.EXP_TYPE_DOC where b.ETDO_ID == etdo_id select b.ETDO_DESC).FirstOrDefault();

            switch (etdo_id)
            {
                case (2):
                    {
                        string postn = (from b in Db.EXP_POST_LINE where b.EPOL_ID == objecttemp.EPOL_EPOL_ID select b.EPOL_NAME).FirstOrDefault().ToString();
                        var w = (from b in Db.EXP_POST_LINE_INSTRU where b.EPIU_ID == idinst select b);
                        string instn = w.FirstOrDefault().CODE_NAME.ToString();
                        string insttype = w.FirstOrDefault().EINS_EINS_ID.ToString();
                        if (insttype == "1")
                            instn = " خط " + instn;
                        else
                            instn = " تجهیز " + instn;
                        bodymessage = " دیفکت " + "به شماره" + objecttemp.DOC_NUMB + " در تاریخ " + objecttemp.EEDO_DATE + " مربوط به " + postn + " و " + instn + " میباشد ";
                        // objecttemp.DEFC_DESC;//+ "به شماره" + objecttemp.DOC_NUMB + " در تاریخ " + objecttemp.EEDO_DATE + " مربوط به " + postn + " و " + instn+" میباشد ";
                        // string  bodymessage = smessage;
                        break;
                    }
            }

            //  int nId = Convert.ToInt32(Session["notid"]);
            AsrWorkFlowProcess fp = new AsrWorkFlowProcess(notid);
            fp.SetKeyValue("BODY", bodymessage);
            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] بروز رسانی شد.", ideedo), CoustomData = ideedo.ToString() }.ToJson();
            // return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", ideedo) }.ToJson();
            // return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Get_Type_Doc_DP()
        {
            var RetVal = from q in Db.EXP_TYPE_DOC where q.ETDO_ID == 2 || q.ETDO_ID == 21 || q.ETDO_ID == 101 select new { q.ETDO_DESC, q.ETDO_ID };

            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EXP_DOC_new(int? EEDO_ID)
        {
            return new ServerMessages(ServerOprationType.Failure) { Message = string.Format("[{0}] اطلاعات نوع مرکز وتاریخ کامل نیست ") }.ToJson();
            Session["EXP_DOC_id"] = EEDO_ID;

            if (EEDO_ID != 0)
            {
                EXP_EXPI_DOC EXPNEW = (from b in Db.EXP_EXPI_DOC where b.EEDO_ID == EEDO_ID select b).FirstOrDefault();
                return View(EXPNEW);
            }

            return View();
        }

        public ActionResult getperson()
        {
            var RetVal = from b in Db.EXP_PERSON_EXPLI select new { b.EPEX_ID, b.EPEX_NAME };

            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult getinstline()
        //{

        //    var Context = new Equipment.Models.BandarEntities();
        //    //Session["eedo_id"] = multicm.EXP_EXPI_DOC.EEDO_ID;
        //    int u = Convert.ToInt32(Session["eedo_id"]);
        //    string postid1 = (from b in Context.EXP_EXPI_DOC where b.EEDO_ID == u select b.EPOL_EPOL_ID).FirstOrDefault().ToString();
        //    if (postid1 != "")
        //    {
        //        int postid = Convert.ToInt16(postid1);
        //        // Convert.ToInt16((from b in Context.EXP_EXPI_DOC where b.EEDO_ID == u select b.EPOL_EPOL_ID).FirstOrDefault().ToString());
        //        var RetVal = from b in Context.EXP_POST_LINE_INSTRU
        //                     join p in Context.EXP_POST_LINE on b.EPOL_EPOL_ID equals p.EPOL_ID
        //                     where b.EPIU_EPIU_ID_SAVABEGH == null && ((p.EPOL_TYPE == "1" && (b.EPOL_EPOL_ID_INSLIN == postid || b.EPOL_EPOL_ID_LINE == postid)
        //                     && (b.EPOL_EPOL_ID_INSLIN != null && b.EPOL_EPOL_ID_LINE != null)) || (p.EPOL_TYPE == "0" && b.EPOL_EPOL_ID == postid))

        //                     select new { b.EPIU_ID, b.CODE_NAME };
        //        return Json(RetVal, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        // int postid = Convert.ToInt16(postid1);
        //        // Convert.ToInt16((from b in Context.EXP_EXPI_DOC where b.EEDO_ID == u select b.EPOL_EPOL_ID).FirstOrDefault().ToString());
        //        var RetVal = from b in Context.EXP_POST_LINE_INSTRU
        //                     select new { b.EPIU_ID, b.CODE_NAME };
        //        return Json(RetVal, JsonRequestBehavior.AllowGet);
        //    }

        //}
        //public ActionResult getinstline()
        //{

        //    var Context = new Equipment.Models.BandarEntities();

        //    int postid = Convert.ToInt32(Request.Form["post_name"]);

        //    var RetVal = from b in Context.EXP_POST_LINE_INSTRU
        //                 join p in Context.EXP_POST_LINE on b.EPOL_EPOL_ID equals p.EPOL_ID
        //                 where ((p.EPOL_TYPE == "1" && (b.EPOL_EPOL_ID_INSLIN == postid || b.EPOL_EPOL_ID_LINE == postid)
        //                 && (b.EPOL_EPOL_ID_INSLIN != null && b.EPOL_EPOL_ID_LINE != null)) || (p.EPOL_TYPE == "0" && b.EPOL_EPOL_ID == postid))

        //                 select new { b.EPIU_ID, b.CODE_NAME };

        //    return Json(RetVal, JsonRequestBehavior.AllowGet);

        //}

        public ActionResult getinstline(short postid)
        {
            var RetVal = from b in Db.EXP_POST_LINE_INSTRU
                         join p in Db.EXP_POST_LINE on b.EPOL_EPOL_ID equals p.EPOL_ID
                         where ((p.EPOL_TYPE == "1" && (b.EPOL_EPOL_ID_INSLIN == postid || b.EPOL_EPOL_ID_LINE == postid) &&
                                 (b.EPOL_EPOL_ID_INSLIN != null && b.EPOL_EPOL_ID_LINE != null)) || (p.EPOL_TYPE == "0" && b.EPOL_EPOL_ID == postid))
                         select new
                         {
                             b.EPIU_ID,
                             b.CODE_NAME,
                             b.PHAS_TYPE,
                             b.PHAS_STAT,
                             cexecdesc = b.EXP_TYPE_EQUIP.ETEX_DESC
                         };

            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getpostdefectre()
        {
            var RetVal = from p in Db.EXP_POST_LINE
                         where p.EPOL_TYPE == "0" && p.EPOL_EPOL_ID == null
                         select new { p.EPOL_NAME, p.EPOL_ID };

            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getpostrequest(int code, string oragan)
        {
            var i = this.HttpContext.User.Identity.Name;
            var q = from b in Db.SEC_USER_TYPE_POST
                    join s in Db.SEC_USERS on b.SCSU_ROW_NO equals s.ROW_NO
                    where b.ETDO_ETDO_ID == code && s.ORCL_NAME == i
                    select b.EPOL_EPOL_ID;

            var RetVal = from p in Db.EXP_POST_LINE
                         where q.Contains(p.EPOL_ID) && p.ORGA_CODE == oragan && p.ORGA_MANA_ASTA_CODE == "7" && p.ORGA_MANA_CODE == "6"
                         select new { p.EPOL_ID, p.EPOL_NAME };

            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getpost(int code)
        {
            var i = this.HttpContext.User.Identity.Name;
            var q = from b in Db.SEC_USER_TYPE_POST
                    join s in Db.SEC_USERS on b.SCSU_ROW_NO equals s.ROW_NO
                    where b.ETDO_ETDO_ID == code && s.ORCL_NAME == i
                    select b.EPOL_EPOL_ID;

            var RetVal = from p in Db.EXP_POST_LINE
                         where q.Contains(p.EPOL_ID)
                         select new { p.EPOL_NAME, p.EPOL_ID };

            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_Expi_Doc([DataSourceRequest] DataSourceRequest request, int ETDO_ID)
        {
            var query = from p in Db.EXP_EXPI_DOC.OrderByDescending(b => b.EEDO_ID)
                        where (p.ETDO_ETDO_ID == ETDO_ID)
                        select new
                        {
                            EEDO_ID = p.EEDO_ID,
                            p.DOC_NUMB,
                            p.EEDO_DESC,
                            p.ACTI_NACT,
                            p.EPOL_EPOL_ID,
                            p.CPRO_CPLA_PLN_CODE,
                            p.CPRO_PRJ_CODE,
                            p.ORGA_CODE,
                            ETDO_DESC = p.EXP_TYPE_DOC.ETDO_DESC,
                            ETDO_ETDO_ID = p.ETDO_ETDO_ID,
                            p.EEDO_DATE
                        };

            return Json(query.ToDataSourceResult(request));
        }

        private string time_reques(decimal? p, int i)
        {
            string curent = string.Empty;
            var q = from j in Db.EXP_EDOC_INSTRU
                    where j.EDIN_ID == p
                    select j;

            if (q.FirstOrDefault() != null)
            {
                if (i == 1)
                {
                    curent = q.FirstOrDefault().OFF_DATE + " " + q.FirstOrDefault().OFF_TIME;
                }
                if (i == 2)
                {
                    curent = q.FirstOrDefault().ON_DATE + " " + q.FirstOrDefault().ON_TIME;
                }
            }
            return curent;
        }

        private string time_prog(decimal? p, int i)
        {
            string curent = string.Empty;
            var q = from j in Db.EXP_EDOC_INSTRU
                    join k in Db.SEC_JOB_TYPE_DOC on j.JDTY_JDTY_ID equals k.JDTY_ID
                    where j.EDIN_EDIN_ID == p && k.ACTIV_NAME == "PROGRAMER"
                    select j;

            if (q.FirstOrDefault() != null)
            {
                if (i == 1)
                {
                    curent = q.FirstOrDefault().OFF_DATE + " " + q.FirstOrDefault().OFF_TIME;
                }
                if (i == 2)
                {
                    curent = q.FirstOrDefault().ON_DATE + " " + q.FirstOrDefault().ON_TIME;
                }
            }
            return curent;
        }

        private string time_progout(decimal? p, int i)
        {
            string curent = string.Empty;
            var q = from j in Db.EXP_EDOC_INSTRU
                    join k in Db.SEC_JOB_TYPE_DOC on j.JDTY_JDTY_ID equals k.JDTY_ID
                    where j.EDIN_EDIN_ID == p && k.ACTIV_NAME == "PROGRAMER"
                    select j;
            if (q.FirstOrDefault() == null)
            {
                var q1 = from j in Db.EXP_EDOC_INSTRU
                         where j.EDIN_ID == p
                         select j;

                if (q1.FirstOrDefault() != null)
                {
                    if (i == 1)
                    {
                        curent = q1.FirstOrDefault().OFF_DATE + " " + q1.FirstOrDefault().OFF_TIME;
                    }
                    if (i == 2)
                    {
                        curent = q1.FirstOrDefault().ON_DATE + " " + q1.FirstOrDefault().ON_TIME;
                    }
                }
                return curent;
            }
            else
            {
                if (q.FirstOrDefault() != null)
                {
                    if (i == 1)
                    {
                        curent = q.FirstOrDefault().OFF_DATE + " " + q.FirstOrDefault().OFF_TIME;
                    }
                    if (i == 2)
                    {
                        curent = q.FirstOrDefault().ON_DATE + " " + q.FirstOrDefault().ON_TIME;
                    }
                }
                return curent;
            }
        }

        private string time_null()
        {
            return null;
        }

        private string timcenter(decimal? p, int i, string Date)
        {
            string curent = string.Empty;
            var q = from j in Db.EXP_EDOC_INSTRU
                    join k in Db.SEC_JOB_TYPE_DOC on j.JDTY_JDTY_ID equals k.JDTY_ID
                    where j.EEDO_EEDO_ID == p && j.CUT_STAT == "1" && k.ACTIV_NAME == "CONTROLCENTER"
                    select j;

            if (q.FirstOrDefault() != null)
            {
                string timei = q.FirstOrDefault().TIME_ISTA.ToString();
                if (timei == "0")
                {
                    if (i == 1)
                    {
                        curent = q.FirstOrDefault().OFF_DATE + " " + q.FirstOrDefault().OFF_TIME;
                    }
                    if (i == 2)
                    {
                        curent = q.FirstOrDefault().ON_DATE + " " + q.FirstOrDefault().ON_TIME;
                    }
                }
                else
                {
                    var q1 = from j in Db.EXP_EDOC_INSTRU
                             join k in Db.SEC_JOB_TYPE_DOC on j.JDTY_JDTY_ID equals k.JDTY_ID
                             where j.EEDO_EEDO_ID == p && j.CUT_STAT == "1" && k.ACTIV_NAME == "CONTROLCENTER" && j.OFF_DATE == Date
                             select j;
                    if (q1.FirstOrDefault() != null)
                    {
                        if (i == 1)
                        {
                            curent = q1.FirstOrDefault().OFF_DATE + " " + q1.FirstOrDefault().OFF_TIME;
                        }
                        if (i == 2)
                        {
                            curent = q1.FirstOrDefault().ON_DATE + " " + q1.FirstOrDefault().ON_TIME;
                        }
                    }
                }
            }

            return curent;
        }

        private string returninst(decimal? p)
        {
            string curentinst = string.Empty;
            var qp = (from j in Db.EXP_EDOC_INSTRU
                      where j.EDIN_ID == p
                      select j);

            if (qp != null)
            {
                var q = qp.FirstOrDefault();

                if (q != null)
                {
                    if (q.ETBY_ETBY_ID != null)
                    {
                        int idetby = Convert.ToInt16(q.ETBY_ETBY_ID);
                        string bayid = (from b in Db.EXP_TYPE_BAY where b.ETBY_ID == idetby select b.ETBY_DESC).FirstOrDefault().ToString();
                        int idepiu = Convert.ToInt16(q.EPIU_EPIU_ID);

                        string postinst = (from b in Db.EXP_POST_LINE_INSTRU where b.EPIU_ID == idepiu select b.CODE_NAME).FirstOrDefault().ToString();
                        curentinst = bayid + " " + postinst;
                    }
                    else
                    {
                        if (q.EPIU_EPIU_ID != null)
                        {
                            int idepiu = Convert.ToInt16(q.EPIU_EPIU_ID);

                            curentinst = (from b in Db.EXP_POST_LINE_INSTRU where b.EPIU_ID == idepiu select b.CODE_NAME).FirstOrDefault().ToString();
                        }
                        else
                        {
                            if (q.EINS_EINS_ID != null)
                            {
                                int ideins = Convert.ToInt16(q.EINS_EINS_ID);

                                curentinst = (from b in Db.EXP_INSTRUMENT where b.EINS_ID == ideins select b.EINS_DESC).FirstOrDefault().ToString();
                            }
                            else
                            {
                                curentinst = "کل پست";
                            }
                        }
                    }
                }
            }

            return curentinst;
        }


        public ActionResult Get_EXP_EDOC_INSTRUcenter([DataSourceRequest] DataSourceRequest request, string Date)
        {
            var query = (from b in Db.EXP_EDOC_INSTRU.AsEnumerable()
                         join j in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals j.JDTY_ID
                         where b.CUT_STAT == "1" && j.ACTIV_NAME == "PROGRAMER" && (b.OFF_DATE.CompareTo(Date) == 0 || b.ON_DATE.CompareTo(Date) == 0)
                         orderby b.EDIN_ID
                         select b).Select(p => new
                         {
                             p.EDIN_ID,
                             inst = returninst(p.EDIN_ID),
                             p.EDIN_OFF,
                             off = p.OFF_DATE + " " + p.OFF_TIME,
                             on = p.ON_DATE + " " + p.ON_TIME,
                             offreq = time_reques(p.EDIN_EDIN_ID, 1),
                             onreq = time_reques(p.EDIN_EDIN_ID, 2),
                             offprog = time_progout(p.EDIN_EDIN_ID, 1),
                             onprog = time_progout(p.EDIN_EDIN_ID, 2),
                             p.OFF_DATE,
                             p.OFF_TIME,
                             p.ON_DATE,
                             p.ON_TIME,
                             p.EINS_EINS_ID,
                             d = p.CONT_FUN,
                             p.EPIU_EPIU_ID,
                             p.EDIN_STIN,
                             p.JDTY_JDTY_ID,
                             p.EPOL_EPOL_ID,
                             p.EUNL_EUNL_ID,
                             p.ETBY_ETBY_ID,
                             p.EOFS_EOFS_ID,
                             p.TIME_ISTA,
                             p.INST_STAT,
                             p.EART_STAT,
                             p.CUST_STAT,
                             p.CUT_STAT
                         });

            var d = new DataSourceResult
            {
                Data = query
            };
            return Json(d);
        }

        public ActionResult Get_EXP_EDOC_INSTRUout([DataSourceRequest] DataSourceRequest request, int? eedo_id)
        {
            var query = (from b in Db.EXP_EDOC_INSTRU.AsEnumerable()
                         join j in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals j.JDTY_ID
                         where b.EEDO_EEDO_ID == eedo_id && eedo_id.HasValue && b.CUT_STAT == "1" && b.ATTG_STATT == "1" && (j.ACTIV_NAME == "DISKERMAN" || j.ACTIV_NAME == "DISBANDAR" ||
                                                                                                                             j.ACTIV_NAME == "DISHORMOZGAN" ||
                                                                                                                             j.ACTIV_NAME == "SECCONTROL" || j.ACTIV_NAME == "LINEORGAN" || j.ACTIV_NAME == "ENTEGHAL" || j.ACTIV_NAME == "FIXORGANE")
                         orderby b.EDIN_ID
                         select b).Select(p => new
                         {
                             p.EDIN_ID,
                             inst = returninst(p.EDIN_ID),
                             p.EDIN_OFF,
                             off = p.OFF_DATE + " " + p.OFF_TIME,
                             on = p.ON_DATE + " " + p.ON_TIME,
                             offreq = time_reques(p.EDIN_EDIN_ID, 1),
                             onreq = time_reques(p.EDIN_EDIN_ID, 2),
                             offprog = time_progout(p.EDIN_EDIN_ID, 1),
                             onprog = time_progout(p.EDIN_EDIN_ID, 2),
                             p.OFF_DATE,
                             p.OFF_TIME,
                             p.ON_DATE,
                             p.ON_TIME,
                             p.EINS_EINS_ID,
                             d = p.CONT_FUN,
                             p.EPIU_EPIU_ID,
                             p.EDIN_STIN,
                             p.JDTY_JDTY_ID,
                             p.EPOL_EPOL_ID,
                             p.EUNL_EUNL_ID,
                             p.ETBY_ETBY_ID,
                             p.EOFS_EOFS_ID,
                             p.TIME_ISTA,
                             p.INST_STAT,
                             p.EART_STAT,
                             p.CUST_STAT,
                             p.CUT_STAT
                         });

            var d = new DataSourceResult
            {
                Data = query
            };
            return Json(d);
        }

        public ActionResult Get_EXP_EDOC_INSTRUoutall([DataSourceRequest] DataSourceRequest request, int? eedo_id, string currentcheck)
        {
            var query = (from b in Db.EXP_EDOC_INSTRU.AsEnumerable()
                         join j in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals j.JDTY_ID
                         where b.EEDO_EEDO_ID == eedo_id && eedo_id.HasValue &&
                               (
                                (currentcheck == "PROGRAMER" &&
                                 (j.ACTIV_NAME == "DISKERMAN" || j.ACTIV_NAME == "DISBANDAR" ||
                                  j.ACTIV_NAME == "DISHORMOZGAN" || j.ACTIV_NAME == "SECCONTROL" || j.ACTIV_NAME == "LINEORGAN" || j.ACTIV_NAME == "ENTEGHAL" ||
                                  j.ACTIV_NAME == "FIXORGANE")) ||
                                (j.ACTIV_NAME == currentcheck && currentcheck != "PROGRAMER"))
                         orderby b.EDIN_ID
                         select b).Select(p => new
                         {
                             p.EDIN_ID,
                             inst = returninst(p.EDIN_ID),
                             p.EDIN_OFF,
                             off = p.OFF_DATE + " " + p.OFF_TIME,
                             on = p.ON_DATE + " " + p.ON_TIME,
                             offreq = time_reques(p.EDIN_EDIN_ID, 1),
                             onreq = time_reques(p.EDIN_EDIN_ID, 2),
                             offprog = time_progout(p.EDIN_EDIN_ID, 1),
                             onprog = time_progout(p.EDIN_EDIN_ID, 2),
                             p.OFF_DATE,
                             p.OFF_TIME,
                             p.ON_DATE,
                             p.ON_TIME,
                             p.EINS_EINS_ID,
                             d = p.CONT_FUN,
                             p.EPIU_EPIU_ID,
                             p.EDIN_STIN,
                             p.JDTY_JDTY_ID,
                             p.EPOL_EPOL_ID,
                             p.EUNL_EUNL_ID,
                             p.ETBY_ETBY_ID,
                             p.EOFS_EOFS_ID,
                             p.TIME_ISTA,
                             p.INST_STAT,
                             p.EART_STAT,
                             p.CUST_STAT,
                             p.CUT_STAT,
                             p.ATTG_STATT
                         });

            var d = new DataSourceResult
            {
                Data = query
            };
            return Json(d);
        }

        public ActionResult Get_EXP_EDOC_INSTRU([DataSourceRequest] DataSourceRequest request, int? eedo_id, string curents, string showgrid, string date)
        {
            int flag = 0;
            if (curents == "MANEGERCR" || curents == "ENTEGHALC" || curents == "FANIC" || curents == "MANEGCONFIRM" || curents == "CREATOR" || curents == null)
            {
                curents = "CREATOR";
                flag = 1;
            }
            else if (curents == "DISPACHMANEG" || curents == "PROGRAMER")
            {
                curents = "PROGRAMER";
            }
            //else
            //    if (curents == "DISKERMAN" || curents == "DISBANDAR" || curents == "DISHORMOZGAN"
            //        || curents == "SECCONTROL" || curents == "LINEORGAN" || curents == "ENTEGHAL"|| curents == "FIXORGANE" )
            //        { curents = "PROGRAMER"; }

            int cquery = (from b in Db.EXP_EDOC_INSTRU
                          join j in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals j.JDTY_ID
                          where b.EEDO_EEDO_ID == eedo_id && b.ATTG_STATT == null && eedo_id.HasValue && j.ACTIV_NAME == curents
                          orderby b.EDIN_ID
                          select b).Count();

            if (cquery == 0 && curents == "PROGRAMER")
            {
                curents = "CREATOR";
                flag = 1;
            }

            if (cquery == 0 && curents == "CONTROLCENTER")
            {
                curents = "PROGRAMER";
                flag = 1;
            }

            if (cquery == 0 && (curents == "DISKERMAN" || curents == "DISBANDAR" || curents == "DISHORMOZGAN" || curents == "STARTPROC" ||
                                curents == "SECCONTROL" || curents == "LINEORGAN" || curents == "ENTEGHAL" || curents == "FIXORGANE"))
            {
                curents = "PROGRAMER";
                flag = 1;

                cquery = (from b in Db.EXP_EDOC_INSTRU
                          join j in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals j.JDTY_ID
                          where b.EEDO_EEDO_ID == eedo_id && b.ATTG_STATT == null && eedo_id.HasValue && j.ACTIV_NAME == curents
                          orderby b.EDIN_ID
                          select b).Count();

                if (cquery == 0 && curents == "PROGRAMER")
                {
                    curents = "CREATOR";
                    flag = 0;
                }
            }

            if (cquery != 0 && curents == "CONTROLCENTER" && showgrid == "0")
            {
                cquery = (from b in Db.EXP_EDOC_INSTRU
                          join j in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals j.JDTY_ID
                          where b.EEDO_EEDO_ID == eedo_id && b.ATTG_STATT == null && eedo_id.HasValue && j.ACTIV_NAME == curents && b.OFF_DATE == date
                          orderby b.EDIN_ID
                          select b).Count();

                if (cquery == 0)
                {
                    curents = "PROGRAMER";
                    flag = 1;
                }
                if (cquery != 0)
                {
                    curents = "CONTROLCENTER";
                    flag = 1;
                }
            }

            if (curents == "WORKER")
            {
                curents = "CONTROLCENTER";
            }

            if (curents == "CREATOR" && flag == 1)
            {
                var query = (from b in Db.EXP_EDOC_INSTRU.AsEnumerable()
                             join j in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals j.JDTY_ID
                             where b.EEDO_EEDO_ID == eedo_id && eedo_id.HasValue && b.ATTG_STATT == null && j.ACTIV_NAME == curents
                             orderby b.EDIN_ID
                             select b).Select(p => new
                             {
                                 p.EDIN_ID,
                                 inst = returninst(p.EDIN_ID),
                                 p.EDIN_OFF,
                                 off = p.OFF_DATE + " " + p.OFF_TIME,
                                 on = p.ON_DATE + " " + p.ON_TIME,
                                 offreq = p.OFF_DATE + " " + p.OFF_TIME,
                                 onreq = p.ON_DATE + " " + p.ON_TIME,
                                 p.EINS_EINS_ID,
                                 d = p.CONT_FUN,
                                 p.EPIU_EPIU_ID,
                                 p.JDTY_JDTY_ID,
                                 p.EPOL_EPOL_ID,
                                 p.EUNL_EUNL_ID,
                                 p.ETBY_ETBY_ID,
                                 p.EOFS_EOFS_ID,
                                 p.TIME_ISTA,
                                 p.INST_STAT,
                                 p.EART_STAT,
                                 p.CUST_STAT,
                                 p.CUT_STAT
                             });

                var d = new DataSourceResult
                {
                    Data = query
                };
                return Json(d);
            }
            else if (curents == "CREATOR" && flag == 0)
            {
                var query = (from b in Db.EXP_EDOC_INSTRU.AsEnumerable()
                             join j in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals j.JDTY_ID
                             where b.EEDO_EEDO_ID == eedo_id && eedo_id.HasValue && b.ATTG_STATT == null && j.ACTIV_NAME == curents
                             orderby b.EDIN_ID
                             select b).Select(p => new
                             {
                                 p.EDIN_ID,
                                 inst = returninst(p.EDIN_ID),
                                 p.EDIN_OFF,
                                 off = time_prog(p.EDIN_EDIN_ID, 1),
                                 on = time_prog(p.EDIN_EDIN_ID, 2),
                                 offprog = p.OFF_DATE + " " + p.OFF_TIME,
                                 onprog = p.ON_DATE + " " + p.ON_TIME,
                                 p.EINS_EINS_ID,
                                 d = p.CONT_FUN,
                                 p.EPIU_EPIU_ID,
                                 p.JDTY_JDTY_ID,
                                 p.EPOL_EPOL_ID,
                                 p.EUNL_EUNL_ID,
                                 p.ETBY_ETBY_ID,
                                 p.EOFS_EOFS_ID,
                                 p.TIME_ISTA,
                                 p.INST_STAT,
                                 p.EART_STAT,
                                 p.CUST_STAT,
                                 p.CUT_STAT
                             });

                var d = new DataSourceResult
                {
                    Data = query
                };
                return Json(d);
            }
            else if (curents == "PROGRAMER" && flag == 1)
            {
                var query = (from b in Db.EXP_EDOC_INSTRU.AsEnumerable()
                             join j in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals j.JDTY_ID
                             where b.EEDO_EEDO_ID == eedo_id && eedo_id.HasValue && b.ATTG_STATT == null && j.ACTIV_NAME == curents
                             orderby b.EDIN_ID
                             select b).Select(p => new
                             {
                                 p.EDIN_ID,
                                 inst = returninst(p.EDIN_ID),
                                 p.EDIN_OFF,
                                 off = time_null(),
                                 on = time_null(),
                                 offreq = time_reques(p.EDIN_EDIN_ID, 1),
                                 onreq = time_reques(p.EDIN_EDIN_ID, 2),
                                 offprog = p.OFF_DATE + " " + p.OFF_TIME,
                                 onprog = p.ON_DATE + " " + p.ON_TIME,
                                 p.EINS_EINS_ID,
                                 d = p.CONT_FUN,
                                 p.EPIU_EPIU_ID,
                                 p.JDTY_JDTY_ID,
                                 p.EPOL_EPOL_ID,
                                 p.EUNL_EUNL_ID,
                                 p.ETBY_ETBY_ID,
                                 p.EOFS_EOFS_ID,
                                 p.TIME_ISTA,
                                 p.INST_STAT,
                                 p.EART_STAT,
                                 p.CUST_STAT,
                                 p.CUT_STAT
                             });

                var d = new DataSourceResult
                {
                    Data = query
                };
                return Json(d);
            }
            else if (curents == "CONTROLCENTER" && flag == 1)
            {
                var query = (from b in Db.EXP_EDOC_INSTRU.AsEnumerable()
                             join j in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals j.JDTY_ID
                             where b.EEDO_EEDO_ID == eedo_id && eedo_id.HasValue && b.ATTG_STATT == null && j.ACTIV_NAME == curents && b.OFF_DATE == date
                             orderby b.EDIN_ID
                             select b).Select(p => new
                             {
                                 p.EDIN_ID,
                                 inst = returninst(p.EDIN_ID),
                                 p.EDIN_OFF,
                                 off = p.OFF_DATE + " " + p.OFF_TIME,
                                 on = p.ON_DATE + " " + p.ON_TIME,
                                 offreq = time_reques(p.EDIN_EDIN_ID, 1),
                                 onreq = time_reques(p.EDIN_EDIN_ID, 2),
                                 offprog = time_progout(p.EDIN_EDIN_ID, 1),
                                 onprog = time_progout(p.EDIN_EDIN_ID, 2),
                                 p.EINS_EINS_ID,
                                 d = p.CONT_FUN,
                                 p.EPIU_EPIU_ID,
                                 p.EDIN_STIN,
                                 p.JDTY_JDTY_ID,
                                 p.EPOL_EPOL_ID,
                                 p.EUNL_EUNL_ID,
                                 p.ETBY_ETBY_ID,
                                 p.EOFS_EOFS_ID,
                                 p.TIME_ISTA,
                                 p.INST_STAT,
                                 p.EART_STAT,
                                 p.CUST_STAT,
                                 p.CUT_STAT
                             });

                var d = new DataSourceResult
                {
                    Data = query
                };
                return Json(d);
            }
            else
            {
                var query = (from b in Db.EXP_EDOC_INSTRU.AsEnumerable()
                             join j in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals j.JDTY_ID
                             where b.EEDO_EEDO_ID == eedo_id && eedo_id.HasValue && b.ATTG_STATT == null && j.ACTIV_NAME == curents
                             orderby b.EDIN_ID
                             select b).Select(p => new
                             {
                                 p.EDIN_ID,
                                 inst = returninst(p.EDIN_ID),
                                 p.EDIN_OFF,
                                 off = p.OFF_DATE + " " + p.OFF_TIME,
                                 on = p.ON_DATE + " " + p.ON_TIME,
                                 offreq = time_reques(p.EDIN_EDIN_ID, 1),
                                 onreq = time_reques(p.EDIN_EDIN_ID, 2),
                                 offprog = time_progout(p.EDIN_EDIN_ID, 1),
                                 onprog = time_progout(p.EDIN_EDIN_ID, 2),
                                 p.EINS_EINS_ID,
                                 d = p.CONT_FUN,
                                 p.EPIU_EPIU_ID,
                                 p.EDIN_STIN,
                                 p.JDTY_JDTY_ID,
                                 p.EPOL_EPOL_ID,
                                 p.EUNL_EUNL_ID,
                                 p.ETBY_ETBY_ID,
                                 p.EOFS_EOFS_ID,
                                 p.TIME_ISTA,
                                 p.INST_STAT,
                                 p.EART_STAT,
                                 p.CUST_STAT,
                                 p.CUT_STAT
                             });

                var d = new DataSourceResult
                {
                    Data = query
                };
                return Json(d);
            }
        }

        public ActionResult Get_Inst([DataSourceRequest] DataSourceRequest request, int? EEDO_ID)
        {
            var query1 = from b in Db.EXP_EXPI_DOC where (b.ETDO_ETDO_ID == 21) select b;

            var query = from q in Db.EXP_EDOC_INSTRU
                        where (!Db.EXP_EDOC_INSTRU.Any(p => p.EDIN_EDIN_ID == q.EDIN_ID))
                        select q;

            //  var q=cntx.EXP_EDOC_INSTRU.Except(query1);

            var d = new DataSourceResult
            {
                Data = query.Select(b => new
                {
                    b.EDIN_ID,
                    inst = returninst(b.EDIN_ID),
                    b.EPIU_EPIU_ID,
                    b.EPOL_EPOL_ID,
                    b.EUNL_EUNL_ID,
                    b.EFUN_EFUN_ID,
                    b.OFF_DATE,
                    b.ON_DATE,
                    b.EOFS_EOFS_ID,
                    b.EEDO_EEDO_ID
                }).Where(p => p.EEDO_EEDO_ID == EEDO_ID && EEDO_ID.HasValue)
            };
            return Json(d);
        }

        private string Returninstrument(decimal p)
        {
            int c = Convert.ToInt16(p);
            string curent = string.Empty;
            var q = (from b in Db.EXP_EDOC_INSTRU
                     join i in Db.EXP_POST_LINE_INSTRU on b.EPIU_EPIU_ID equals i.EPIU_ID
                     where b.EEDO_EEDO_ID == c
                     select i.CODE_NAME);
            if (q.FirstOrDefault() != null)
                curent = q.FirstOrDefault().ToString();
            return curent;
        }

        private int Returninstrumentid(decimal p)
        {
            int c = Convert.ToInt16(p);
            int curent = 0;
            var q = (from b in Db.EXP_EDOC_INSTRU
                     join i in Db.EXP_POST_LINE_INSTRU on b.EPIU_EPIU_ID equals i.EPIU_ID
                     where b.EEDO_EEDO_ID == c
                     select i.EPIU_ID);
            if (q.FirstOrDefault() != null)
                curent = Convert.ToInt16(q.FirstOrDefault().ToString());
            return curent;
        }

        public class yearSearchModel
        {
            public int EEDO_YEAR2 { get; set; }

            public int post { get; set; }

            public int line { get; set; }

            public int EINS_ID2 { get; set; }

            public int ETBY_ID2 { get; set; }

            public int EUNL_ID2 { get; set; }

            public int EOFS_ID2 { get; set; }

            public int EXP_PROGRAM1 { get; set; }

            public int efun { get; set; }

            public int linepost { get; set; }

            public string azDate1 { get; set; }

            public string taDate1 { get; set; }
        }

        private string organpost(decimal? p)
        {
            int c = Convert.ToInt16(p);
            string curent = string.Empty;
            var q = from d in Db.EXP_POST_LINE
                    join j in Db.EXP_EDOC_INSTRU on d.EPOL_ID equals j.EPOL_EPOL_ID
                    where j.EDIN_ID == c
                    select d.ORGA_CODE;

            if (q.FirstOrDefault() != null)
                curent = q.FirstOrDefault().ToString();
            return curent;
        }

        private string programid(decimal? p)
        {
            string curent = string.Empty;
            var q = from d in Db.EXP_PFUNCTION
                    where d.EFUN_ID == p
                    select d.EPRO_EPRO_ID;

            if (q.FirstOrDefault() != null)
                curent = q.FirstOrDefault().ToString();
            return curent;
        }

        public ActionResult Exp_TimeProg_R([DataSourceRequest] DataSourceRequest request, yearSearchModel yearSearchModel1)
        {
            //  string EEDO_YEAR1Param = yearSearchModel1.EEDO_YEAR1;
            string EEDO_YEAR2Param = yearSearchModel1.EEDO_YEAR2.ToString();
            int postparam = yearSearchModel1.post;
            int lineParam = yearSearchModel1.line;
            int einsparam = yearSearchModel1.EINS_ID2;
            int eunlparam = yearSearchModel1.EUNL_ID2;
            int etbyparam = yearSearchModel1.ETBY_ID2;
            int eofsparam = yearSearchModel1.EOFS_ID2;
            int progparam = yearSearchModel1.EXP_PROGRAM1;
            int efunparam = yearSearchModel1.efun;
            int elinepost = yearSearchModel1.linepost;
            string azDate = yearSearchModel1.azDate1;
            string taDate = yearSearchModel1.taDate1;
            if (azDate == null)
                azDate = "";

            if (taDate == null)
                taDate = "";

            var qnotinst = (from x in Db.EXP_RELATION_DOC where x.EDIN_EDIN_ID != null select x.EDIN_EDIN_ID);

            /** برنامه های  پست های که در درخواست انجام کار این کاربر گرفته است قابل مشاهده است */

            var i = this.HttpContext.User.Identity.Name;
            var qprogset = from b in Db.SEC_USER_TYPE_POST
                           join s in Db.SEC_USERS on b.SCSU_ROW_NO equals s.ROW_NO
                           where b.ETDO_ETDO_ID == 21 && s.ORCL_NAME == i
                           select b.EPOL_EPOL_ID;

            /***/

            if (postparam != 0)
            {
                if (einsparam == 0)
                {
                    if (elinepost == 0)
                    {
                        if (taDate != "" && azDate != "")
                        {
                            var query = from k in Db.EXP_EXPI_DOC.AsEnumerable()
                                        join b in Db.EXP_EDOC_INSTRU.AsEnumerable() on k.EEDO_ID equals b.EEDO_EEDO_ID
                                        where (k.EEDO_YEAR == EEDO_YEAR2Param && k.EEDO_YEAR != null) &&
                                              (b.EPOL_EPOL_ID == postparam || postparam == 0) &&
                                              (b.EUNL_EUNL_ID == eunlparam || eunlparam == 0) &&
                                              (b.ETBY_ETBY_ID == etbyparam || etbyparam == 0) &&
                                              (b.EOFS_EOFS_ID == eofsparam || eofsparam == 0) &&
                                              (b.EFUN_EFUN_ID == efunparam || efunparam == 0) &&
                                              ((azDate.CompareTo(b.OFF_DATE) <= 0 && b.OFF_DATE.CompareTo(taDate) <= 0)) &&
                                              !qnotinst.Contains(b.EDIN_ID) &&
                                              qprogset.Contains(b.EPOL_EPOL_ID)
                                        select new { b.EDIN_ID, b.EPOL_EPOL_ID, organprog = organpost(b.EDIN_ID), b.ETBY_ETBY_ID, b.EPIU_EPIU_ID, b.EUNL_EUNL_ID, b.EFUN_EFUN_ID, prog = programid(b.EFUN_EFUN_ID), b.OFF_DATE, b.ON_DATE, b.EOFS_EOFS_ID };
                            return Json(query.ToDataSourceResult(request));
                        }
                        else
                        {
                            var query = from k in Db.EXP_EXPI_DOC.AsEnumerable()
                                        join b in Db.EXP_EDOC_INSTRU.AsEnumerable() on k.EEDO_ID equals b.EEDO_EEDO_ID
                                        where (k.EEDO_YEAR == EEDO_YEAR2Param && k.EEDO_YEAR != null) &&
                                              (b.EPOL_EPOL_ID == postparam || postparam == 0) &&
                                              (b.EUNL_EUNL_ID == eunlparam || eunlparam == 0) &&
                                              (b.ETBY_ETBY_ID == etbyparam || etbyparam == 0) &&
                                              (b.EOFS_EOFS_ID == eofsparam || eofsparam == 0) &&
                                              (b.EFUN_EFUN_ID == efunparam || efunparam == 0)
                                            // && ((azDate.CompareTo(b.OFF_DATE) <= 0 && b.OFF_DATE.CompareTo(taDate) <= 0) || azDate == "" || taDate == "")
                                                                                                                                                                      &&
                                              !qnotinst.Contains(b.EDIN_ID) &&
                                              qprogset.Contains(b.EPOL_EPOL_ID)
                                        select new { b.EDIN_ID, b.EPOL_EPOL_ID, organprog = organpost(b.EDIN_ID), b.ETBY_ETBY_ID, b.EPIU_EPIU_ID, b.EUNL_EUNL_ID, b.EFUN_EFUN_ID, prog = programid(b.EFUN_EFUN_ID), b.OFF_DATE, b.ON_DATE, b.EOFS_EOFS_ID };
                            return Json(query.ToDataSourceResult(request));
                        }
                    }
                    else
                    {
                        /*مثل خط و فرقی ندارد*/
                        var q = from k in Db.EXP_POST_LINE_INSTRU
                                where k.EPOL_EPOL_ID_INSLIN != null && k.EPOL_EPOL_ID_LINE != null && k.EPOL_EPOL_ID == elinepost
                                select new { k.EPIU_ID };
                        elinepost = int.Parse(q.FirstOrDefault().EPIU_ID.ToString());
                        if (taDate != "" && azDate != "")
                        {
                            var query = from k in Db.EXP_EXPI_DOC.AsEnumerable()
                                        join b in Db.EXP_EDOC_INSTRU.AsEnumerable() on k.EEDO_ID equals b.EEDO_EEDO_ID
                                        where (k.EEDO_YEAR == EEDO_YEAR2Param && k.EEDO_YEAR != null) &&
                                              (b.EPOL_EPOL_ID == postparam || postparam == 0) &&
                                              (b.EUNL_EUNL_ID == eunlparam || eunlparam == 0) &&
                                              (b.ETBY_ETBY_ID == etbyparam || etbyparam == 0) &&
                                              (b.EOFS_EOFS_ID == eofsparam || eofsparam == 0) &&
                                              (b.EFUN_EFUN_ID == efunparam || efunparam == 0) &&
                                              (b.EPIU_EPIU_ID == elinepost || elinepost == 0) &&
                                              ((azDate.CompareTo(b.OFF_DATE) <= 0 && b.OFF_DATE.CompareTo(taDate) <= 0)) &&
                                              !qnotinst.Contains(b.EDIN_ID) &&
                                              qprogset.Contains(b.EPOL_EPOL_ID)
                                        select new { b.EDIN_ID, b.EPOL_EPOL_ID, organprog = organpost(b.EDIN_ID), b.ETBY_ETBY_ID, b.EPIU_EPIU_ID, b.EUNL_EUNL_ID, b.EFUN_EFUN_ID, prog = programid(b.EFUN_EFUN_ID), b.OFF_DATE, b.ON_DATE, b.EOFS_EOFS_ID };
                            return Json(query.ToDataSourceResult(request));
                        }
                        else
                        {
                            var query = from k in Db.EXP_EXPI_DOC.AsEnumerable()
                                        join b in Db.EXP_EDOC_INSTRU.AsEnumerable() on k.EEDO_ID equals b.EEDO_EEDO_ID
                                        where (k.EEDO_YEAR == EEDO_YEAR2Param && k.EEDO_YEAR != null) &&
                                              (b.EPOL_EPOL_ID == postparam || postparam == 0) &&
                                              (b.EUNL_EUNL_ID == eunlparam || eunlparam == 0) &&
                                              (b.ETBY_ETBY_ID == etbyparam || etbyparam == 0) &&
                                              (b.EOFS_EOFS_ID == eofsparam || eofsparam == 0) &&
                                              (b.EFUN_EFUN_ID == efunparam || efunparam == 0) &&
                                              (b.EPIU_EPIU_ID == elinepost || elinepost == 0) &&
                                              !qnotinst.Contains(b.EDIN_ID) &&
                                              qprogset.Contains(b.EPOL_EPOL_ID)
                                        //     && ((azDate.CompareTo(b.OFF_DATE) <= 0 && b.OFF_DATE.CompareTo(taDate) <= 0) || azDate == "" || taDate == "")
                                        select new { b.EDIN_ID, b.EPOL_EPOL_ID, organprog = organpost(b.EDIN_ID), b.ETBY_ETBY_ID, b.EPIU_EPIU_ID, b.EUNL_EUNL_ID, b.EFUN_EFUN_ID, prog = programid(b.EFUN_EFUN_ID), b.OFF_DATE, b.ON_DATE, b.EOFS_EOFS_ID };
                            return Json(query.ToDataSourceResult(request));
                        }
                    }
                }
                else
                {
                    var q = (from h in Db.EXP_POST_LINE_INSTRU where h.EINS_EINS_ID == einsparam && h.EPOL_EPOL_ID == postparam select new { h.EPIU_ID });

                    if (taDate != "" && azDate != "")
                    {
                        var query = from k in Db.EXP_EXPI_DOC.AsEnumerable()
                                    join b in Db.EXP_EDOC_INSTRU.AsEnumerable() on k.EEDO_ID equals b.EEDO_EEDO_ID
                                    join f in q on b.EPIU_EPIU_ID equals f.EPIU_ID
                                    where (k.EEDO_YEAR == EEDO_YEAR2Param || EEDO_YEAR2Param == null) &&
                                          (b.EPOL_EPOL_ID == postparam || postparam == 0) &&
                                          (b.EUNL_EUNL_ID == eunlparam || eunlparam == 0) &&
                                          (b.ETBY_ETBY_ID == etbyparam || etbyparam == 0) &&
                                          (b.EFUN_EFUN_ID == efunparam || efunparam == 0) &&
                                          (b.EOFS_EOFS_ID == eofsparam || eofsparam == 0) &&
                                          ((azDate.CompareTo(b.OFF_DATE) <= 0 && b.OFF_DATE.CompareTo(taDate) <= 0)) &&
                                          !qnotinst.Contains(b.EDIN_ID) &&
                                          qprogset.Contains(b.EPOL_EPOL_ID)
                                    select new { b.EDIN_ID, b.EPOL_EPOL_ID, organprog = organpost(b.EDIN_ID), b.ETBY_ETBY_ID, b.EPIU_EPIU_ID, b.EUNL_EUNL_ID, b.EFUN_EFUN_ID, prog = programid(b.EFUN_EFUN_ID), b.OFF_DATE, b.ON_DATE, b.EOFS_EOFS_ID };

                        return Json(query.ToDataSourceResult(request));
                    }
                    else
                    {
                        var query = from k in Db.EXP_EXPI_DOC.AsEnumerable()
                                    join b in Db.EXP_EDOC_INSTRU.AsEnumerable() on k.EEDO_ID equals b.EEDO_EEDO_ID
                                    join f in q on b.EPIU_EPIU_ID equals f.EPIU_ID
                                    where (k.EEDO_YEAR == EEDO_YEAR2Param || EEDO_YEAR2Param == null) &&
                                          (b.EPOL_EPOL_ID == postparam || postparam == 0) &&
                                          (b.EUNL_EUNL_ID == eunlparam || eunlparam == 0) &&
                                          (b.ETBY_ETBY_ID == etbyparam || etbyparam == 0) &&
                                          (b.EFUN_EFUN_ID == efunparam || efunparam == 0) &&
                                          (b.EOFS_EOFS_ID == eofsparam || eofsparam == 0) &&
                                          !qnotinst.Contains(b.EDIN_ID) &&
                                          qprogset.Contains(b.EPOL_EPOL_ID)
                                    select new { b.EDIN_ID, b.EPOL_EPOL_ID, organprog = organpost(b.EDIN_ID), b.ETBY_ETBY_ID, b.EPIU_EPIU_ID, b.EUNL_EUNL_ID, b.EFUN_EFUN_ID, prog = programid(b.EFUN_EFUN_ID), b.OFF_DATE, b.ON_DATE, b.EOFS_EOFS_ID };

                        return Json(query.ToDataSourceResult(request));
                    }
                }
            }
            else
            {
                if (lineParam != 0)
                {
                    var q = from k in Db.EXP_POST_LINE_INSTRU
                            where k.EPOL_EPOL_ID_INSLIN != null && k.EPOL_EPOL_ID_LINE != null && k.EPOL_EPOL_ID == lineParam
                            select new { k.EPIU_ID };
                    lineParam = int.Parse(q.FirstOrDefault().EPIU_ID.ToString());

                    if (taDate != "" && azDate != "")
                    {
                        var query = from k in Db.EXP_EXPI_DOC.AsEnumerable()
                                    join b in Db.EXP_EDOC_INSTRU.AsEnumerable() on k.EEDO_ID equals b.EEDO_EEDO_ID
                                    where (k.EEDO_YEAR == EEDO_YEAR2Param && k.EEDO_YEAR != null) &&
                                          (b.EPOL_EPOL_ID == postparam || postparam == 0) &&
                                          (b.EUNL_EUNL_ID == eunlparam || eunlparam == 0) &&
                                          (b.ETBY_ETBY_ID == etbyparam || etbyparam == 0) &&
                                          (b.EOFS_EOFS_ID == eofsparam || eofsparam == 0) &&
                                          (b.EFUN_EFUN_ID == efunparam || efunparam == 0) &&
                                          (b.EPIU_EPIU_ID == lineParam || lineParam == 0) &&
                                          ((azDate.CompareTo(b.OFF_DATE) <= 0 && b.OFF_DATE.CompareTo(taDate) <= 0)) &&
                                          !qnotinst.Contains(b.EDIN_ID) &&
                                          qprogset.Contains(b.EPOL_EPOL_ID)
                                    select new { b.EDIN_ID, b.EPOL_EPOL_ID, organprog = organpost(b.EDIN_ID), b.ETBY_ETBY_ID, b.EPIU_EPIU_ID, b.EUNL_EUNL_ID, b.EFUN_EFUN_ID, prog = programid(b.EFUN_EFUN_ID), b.OFF_DATE, b.ON_DATE, b.EOFS_EOFS_ID };
                        return Json(query.ToDataSourceResult(request));
                    }
                    else
                    {
                        var query = from k in Db.EXP_EXPI_DOC.AsEnumerable()
                                    join b in Db.EXP_EDOC_INSTRU.AsEnumerable() on k.EEDO_ID equals b.EEDO_EEDO_ID
                                    where (k.EEDO_YEAR == EEDO_YEAR2Param && k.EEDO_YEAR != null) &&
                                          (b.EPOL_EPOL_ID == postparam || postparam == 0) &&
                                          (b.EUNL_EUNL_ID == eunlparam || eunlparam == 0) &&
                                          (b.ETBY_ETBY_ID == etbyparam || etbyparam == 0) &&
                                          (b.EOFS_EOFS_ID == eofsparam || eofsparam == 0) &&
                                          (b.EFUN_EFUN_ID == efunparam || efunparam == 0) &&
                                          (b.EPIU_EPIU_ID == lineParam || lineParam == 0)
                                        //         && ((azDate.CompareTo(b.OFF_DATE) <= 0 && b.OFF_DATE.CompareTo(taDate) <= 0) || azDate == "" || taDate == "")
                                                                                                                                                                  &&
                                          !qnotinst.Contains(b.EDIN_ID) &&
                                          qprogset.Contains(b.EPOL_EPOL_ID)
                                    select new { b.EDIN_ID, b.EPOL_EPOL_ID, organprog = organpost(b.EDIN_ID), b.ETBY_ETBY_ID, b.EPIU_EPIU_ID, b.EUNL_EUNL_ID, b.EFUN_EFUN_ID, prog = programid(b.EFUN_EFUN_ID), b.OFF_DATE, b.ON_DATE, b.EOFS_EOFS_ID };
                        return Json(query.ToDataSourceResult(request));
                    }
                }
                else
                {
                    if (taDate != "" && azDate != "")
                    {
                        var query = from k in Db.EXP_EXPI_DOC.AsEnumerable()
                                    join b in Db.EXP_EDOC_INSTRU.AsEnumerable() on k.EEDO_ID equals b.EEDO_EEDO_ID
                                    where (k.EEDO_YEAR == EEDO_YEAR2Param || EEDO_YEAR2Param == null) &&
                                          (b.EPOL_EPOL_ID == postparam || postparam == 0) &&
                                          (b.EUNL_EUNL_ID == eunlparam || eunlparam == 0) &&
                                          (b.ETBY_ETBY_ID == etbyparam || etbyparam == 0) &&
                                          (b.EOFS_EOFS_ID == eofsparam || eofsparam == 0) &&
                                          (b.EFUN_EFUN_ID == efunparam || efunparam == 0) &&
                                          ((azDate.CompareTo(b.OFF_DATE) <= 0 && b.OFF_DATE.CompareTo(taDate) <= 0)) &&
                                          !qnotinst.Contains(b.EDIN_ID) &&
                                          qprogset.Contains(b.EPOL_EPOL_ID)
                                    select new { b.EDIN_ID, b.EPOL_EPOL_ID, organprog = organpost(b.EDIN_ID), b.ETBY_ETBY_ID, b.EPIU_EPIU_ID, b.EUNL_EUNL_ID, b.EFUN_EFUN_ID, prog = programid(b.EFUN_EFUN_ID), b.OFF_DATE, b.ON_DATE, b.EOFS_EOFS_ID };

                        return Json(query.ToDataSourceResult(request));
                    }
                    else
                    {
                        var query = from k in Db.EXP_EXPI_DOC.AsEnumerable()
                                    join b in Db.EXP_EDOC_INSTRU.AsEnumerable() on k.EEDO_ID equals b.EEDO_EEDO_ID
                                    where (k.EEDO_YEAR == EEDO_YEAR2Param || EEDO_YEAR2Param == null) &&
                                          (b.EPOL_EPOL_ID == postparam || postparam == 0) &&
                                          (b.EUNL_EUNL_ID == eunlparam || eunlparam == 0) &&
                                          (b.ETBY_ETBY_ID == etbyparam || etbyparam == 0) &&
                                          (b.EOFS_EOFS_ID == eofsparam || eofsparam == 0) &&
                                          (b.EFUN_EFUN_ID == efunparam || efunparam == 0)
                                        //  && ((azDate.CompareTo(b.OFF_DATE) <= 0 && b.OFF_DATE.CompareTo(taDate) <= 0) || azDate == "" || taDate == "")
                                                                                                                                                                  &&
                                          !qnotinst.Contains(b.EDIN_ID) &&
                                          qprogset.Contains(b.EPOL_EPOL_ID)
                                    select new { b.EDIN_ID, b.EPOL_EPOL_ID, organprog = organpost(b.EDIN_ID), b.ETBY_ETBY_ID, b.EPIU_EPIU_ID, b.EUNL_EUNL_ID, b.EFUN_EFUN_ID, prog = programid(b.EFUN_EFUN_ID), b.OFF_DATE, b.ON_DATE, b.EOFS_EOFS_ID };

                        return Json(query.ToDataSourceResult(request));
                    }
                }
            }
        }

        private string organpostexpi(decimal? p)
        {
            int c = Convert.ToInt16(p);
            string curent = string.Empty;
            var q = from d in Db.EXP_POST_LINE
                    join j in Db.EXP_EXPI_DOC on d.EPOL_ID equals j.EPOL_EPOL_ID
                    where j.EEDO_ID == c
                    select d.ORGA_CODE;

            if (q.FirstOrDefault() != null)
                curent = q.FirstOrDefault().ToString();
            return curent;
        }

        public class defectreqModel
        {
            public int post { get; set; }

            public int EINS_Inst { get; set; }

            public int EXP_PROGRAM1 { get; set; }

            public int efun1 { get; set; }

            public string azdate { get; set; }

            public string tadate { get; set; }
        }

        public ActionResult defect_form([DataSourceRequest]
                                        DataSourceRequest request, defectreqModel defectreqModel1)
        {
            int postparam = defectreqModel1.post;
            int einsparam = defectreqModel1.EINS_Inst;
            int progparam = defectreqModel1.EXP_PROGRAM1;
            int efunparam = defectreqModel1.efun1;
            string azdate = defectreqModel1.azdate;
            string tadate = defectreqModel1.tadate;
            if (azdate == null)
                azdate = "";

            if (tadate == null)
                tadate = "";
            var qnotinst = (from x in Db.EXP_RELATION_DOC where x.EEDO_EEDO_ID_R != null select x.EEDO_EEDO_ID_R);
            /** دیفکت های  پست های که در درخواست انجام کار این کاربر گرفته است قابل مشاهده است */

            var i = this.HttpContext.User.Identity.Name;
            var q = from b in Db.SEC_USER_TYPE_POST
                    join s in Db.SEC_USERS on b.SCSU_ROW_NO equals s.ROW_NO
                    where b.ETDO_ETDO_ID == 21 && s.ORCL_NAME == i
                    select b.EPOL_EPOL_ID;

            /***/

            var query = (from b in Db.EXP_EXPI_DOC.AsEnumerable()
                         where b.ETDO_ETDO_ID == 2 && (b.EPOL_EPOL_ID == postparam || postparam == 0) && (b.EFUN_EFUN_ID == efunparam || efunparam == 0) &&
                               (Returninstrumentid(b.EEDO_ID) == einsparam || einsparam == 0) &&
                               ((azdate.CompareTo(b.EEDO_DATE) <= 0 && b.EEDO_DATE.CompareTo(tadate) <= 0) || azdate == "" || tadate == "") &&
                               !qnotinst.Contains(b.EEDO_ID) &&
                               q.Contains(b.EPOL_EPOL_ID)
                         orderby b.DOC_NUMB
                         select b).Select(p => new
                         {
                             p.EEDO_ID,
                             p.DOC_NUMB,
                             off = p.EEDO_DATE + " " + p.EEDO_TIME,
                             p.EPOL_EPOL_ID,
                             inst = Returninstrument(p.EEDO_ID),
                             instid = Returninstrumentid(p.EEDO_ID),
                             p.EFUN_EFUN_ID,
                             p.DEFC_DESC,
                             organprog = organpostexpi(p.EEDO_ID),
                             prog = programid(p.EFUN_EFUN_ID)
                         });
            var d = new DataSourceResult
            {
                Data = query
            };
            return Json(d);
        }

        //public ActionResult Get_Expi_Doc_DP(int etdo_id)
        //{
        //    var RetVal = from b in PublicRepository.Get_ExpDoc() where (b.ETDO_ETDO_ID == etdo_id || b.ETDO_ETDO_ID == null) orderby b.EEDO_DESC select new { b.EEDO_ID, b.EEDO_DESC };
        //    return Json(RetVal, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Update_Expi_Doc([DataSourceRequest]
        //                                 DataSourceRequest request, [Bind(Prefix = "models")]
        //                                 IEnumerable<EXP_EXPI_DOC> expi_doc)
        //{
        //    if (expi_doc != null)
        //    {
        //        foreach (EXP_EXPI_DOC expidoc in expi_doc)
        //        {

        //            cntx.Entry(expidoc).State = EntityState.Modified;
        //            cntx.SaveChanges();

        //        }
        //    }

        //    return Json(expi_doc.ToDataSourceResult(request, ModelState));
        //}

        public ActionResult Get_Value_Doc([DataSourceRequest] DataSourceRequest request, int id)
        {
            var query = from dman in Db.EXP_EITEM_DOC_VALUE where (dman.EEDO_EEDO_ID == id) select dman;
            var d = new DataSourceResult
            {
                Data = query.Select(p => new
                {
                    p.EIDR_ID,
                    p.EIDR_VALUE,
                    EITY_DESC = p.EXP_ITEM_TYPE_DOC.EITY_DESC
                }),
            };
            return Json(d);
        }

        public ActionResult select_all(int? id, string current)
        {
            ViewData["post_line"] = Db.EXP_POST_LINE.Select(o => new { o.EPOL_ID, o.EPOL_NAME }).AsEnumerable();
            ViewData["project"] = Db.CGT_PRO
                                    .Where(o => o.CPRO_PRJ_CODE == Db.EXP_EXPI_DOC.Select(p => p.CPRO_PRJ_CODE).FirstOrDefault())
                                    .Where(o => o.CPLA_PLN_CODE == Db.EXP_EXPI_DOC.Select(p => p.CPRO_CPLA_PLN_CODE).FirstOrDefault())
                                    .Select(o => new { o.PRJ_CODE, o.PRJ_DESC })
                                    .AsEnumerable();
            ViewData["Organ"] = Db.PAY_ORGAN.Select(o => new { o.CODE, o.ORGA_DESC }).AsEnumerable();
            //ViewData["Type_Doc"] = cntx.EXP_TYPE_DOC.Select(o => new { o.ETDO_ID, o.ETDO_DESC }).Where(o => o.ETDO_ID == (cntx.EXP_TYPE_DOC.Select(o => new { o.ETDO_ID }))).AsEnumerable();
            ViewData["Type_Doc"] = Db.EXP_EXPI_DOC.Select(o => new { o.EEDO_ID, o.EXP_TYPE_DOC, o.ETDO_ETDO_ID, o.EXP_TYPE_DOC.ETDO_DESC }).Where(o => o.ETDO_ETDO_ID == o.EXP_TYPE_DOC.ETDO_ID).AsEnumerable();
            ViewData["Doc_val"] = Db.EXP_EITEM_DOC_VALUE.Select(o => new { o.EIDR_ID, o.EIDR_VALUE });
            ViewData["post_instr"] = Db.EXP_POST_LINE_INSTRU.Select(o => new { o.EINS_EINS_ID, o.CODE_DISP });
            ViewData["JDTYid"] = Db.SEC_JOB_TYPE_DOC.Select(o => new { o.JDTY_ID, o.ACTIV_FNAM });

            ViewBag.Instrument = Db.EXP_INSTRUMENT.Where(c => c.EINS_EINS_ID != null).Select(c => new { c.EINS_ID, c.EINS_DESC });
            ViewBag.Bay = Db.EXP_TYPE_BAY.Select(c => new { c.ETBY_ID, c.ETBY_DESC });
            ViewBag.UniteV = Db.EXP_UNIT_LEVEL.Select(c => new { c.EUNL_ID, c.EUNL_DESC });
            ViewBag.post = Db.EXP_POST_LINE.Select(c => new { c.EPOL_ID, c.EPOL_NAME });
            ViewBag.PostInstrument = Db.EXP_POST_LINE_INSTRU.Select(c => new { c.EPIU_ID, c.CODE_NAME });
            ViewBag.Offstat = Db.EXP_OFF_STAT.Select(c => new { c.EOFS_ID, c.EOFS_DESC });
            ViewBag.PFUNCTION = Db.EXP_PFUNCTION.Select(c => new { c.EFUN_ID, c.EFUN_DESC });

            //  int ei = Convert.ToInt16(eedo_id);
            Session["eedo_id"] = id;
            ViewBag.current = current;

            return View();
        }

        public ActionResult ViewForm(string id, string notId, string date)
        {
            ViewData["post_line"] = Db.EXP_POST_LINE.Select(o => new { o.EPOL_ID, o.EPOL_NAME }).AsEnumerable();
            ViewData["project"] = Db.CGT_PRO
                                    .Where(o => o.CPRO_PRJ_CODE == Db.EXP_EXPI_DOC.Select(p => p.CPRO_PRJ_CODE).FirstOrDefault())
                                    .Where(o => o.CPLA_PLN_CODE == Db.EXP_EXPI_DOC.Select(p => p.CPRO_CPLA_PLN_CODE).FirstOrDefault())
                                    .Select(o => new { o.PRJ_CODE, o.PRJ_DESC })
                                    .AsEnumerable();
            ViewData["Organ"] = Db.PAY_ORGAN.Select(o => new { o.CODE, o.ORGA_DESC }).AsEnumerable();

            //ViewData["Type_Doc"] = cntx.EXP_TYPE_DOC.Select(o => new { o.ETDO_ID, o.ETDO_DESC }).Where(o => o.ETDO_ID == (cntx.EXP_TYPE_DOC.Select(o => new { o.ETDO_ID }))).AsEnumerable();
            ViewData["Type_Doc"] = Db.EXP_EXPI_DOC.Select(o => new { o.EEDO_ID, o.EXP_TYPE_DOC, o.ETDO_ETDO_ID, o.EXP_TYPE_DOC.ETDO_DESC }).Where(o => o.ETDO_ETDO_ID == o.EXP_TYPE_DOC.ETDO_ID).AsEnumerable();
            ViewData["Doc_val"] = Db.EXP_EITEM_DOC_VALUE.Select(o => new { o.EIDR_ID, o.EIDR_VALUE });
            ViewData["post_instr"] = Db.EXP_POST_LINE_INSTRU.Select(o => new { o.EINS_EINS_ID, o.CODE_DISP });
            ViewData["JDTYid"] = Db.SEC_JOB_TYPE_DOC.Select(o => new { o.JDTY_ID, o.ACTIV_FNAM });

            ViewBag.Instrument = Db.EXP_INSTRUMENT.Where(c => c.EINS_EINS_ID != null).Select(c => new { c.EINS_ID, c.EINS_DESC });
            ViewBag.Bay = Db.EXP_TYPE_BAY.Select(c => new { c.ETBY_ID, c.ETBY_DESC });
            ViewBag.UniteV = Db.EXP_UNIT_LEVEL.Select(c => new { c.EUNL_ID, c.EUNL_DESC });
            ViewBag.post = Db.EXP_POST_LINE.Select(c => new { c.EPOL_ID, c.EPOL_NAME });
            ViewBag.PostInstrument = Db.EXP_POST_LINE_INSTRU.Select(c => new { c.EPIU_ID, c.CODE_NAME });
            ViewBag.Offstat = Db.EXP_OFF_STAT.Select(c => new { c.EOFS_ID, c.EOFS_DESC });
            ViewBag.PFUNCTION = Db.EXP_PFUNCTION.Select(c => new { c.EFUN_ID, c.EFUN_DESC });

            ViewBag.exp_doc = Db.EXP_EXPI_DOC;

            EXP_EXPI_DOC cm = new EXP_EXPI_DOC();
            EXP_ITEM_TYPE_DOC cmitem = new EXP_ITEM_TYPE_DOC();
            EXP_EDOC_INSTRU cminstru = new EXP_EDOC_INSTRU();
            Equipment.Models.PartialClass.MultiModelPrj multicm = new Equipment.Models.PartialClass.MultiModelPrj();

            string ssend = string.Empty;
            //  string sdate = string.Empty;

            if (id != "0")
            {
                cm = Db.Database.SqlQuery<EXP_EXPI_DOC>("select * from EXP_EXPI_DOC where {0}".FormatWith(id)).FirstOrDefault();
                multicm.EXP_EXPI_DOC = cm;
                int i = 0;

                while (i <= 6)
                {
                    string ci = "FLW_REHA.PFLW_REHA^" + multicm.EXP_EXPI_DOC.EEDO_ID + "-" + i;

                    int si = Db.Database.SqlQuery<int>("select count(NOT_ID) from WF_NOTE_V where ITEM_KEY='" + ci + "'").FirstOrDefault();

                    if (si > 0)
                    {
                        if (i == 0)

                            ssend = ssend + " -دیسپاچینگ کرمان";
                        if (i == 1)

                            ssend = ssend + " -دیسپاچینگ هرمزگان";
                        if (i == 2)

                            ssend = ssend + " -دیسپاچینگ بندر";
                        if (i == 3)

                            ssend = ssend + " -حفاظت و کنترل";
                        if (i == 4)

                            ssend = ssend + " - اداره تعمیرات";
                        if (i == 5)

                            ssend = ssend + " -اداره خط ";
                        if (i == 6)

                            ssend = ssend + " -انتقال ";
                    }
                    i = i + 1;
                }

                Session["notid"] = notId;
                ViewBag.key = notId;
                Session["etdo_id"] = multicm.EXP_EXPI_DOC.ETDO_ETDO_ID;

                Session["eedo_id"] = multicm.EXP_EXPI_DOC.EEDO_ID;
                ViewBag.stringsend = ssend;
                ViewBag.stringdate = date;
                return View("PartialViewForm", multicm);
            }
            else
            {
                return View("ErrorNotFound");
            }
        }

        [MenuAuthorize]
        public ActionResult FormCentercontrol()
        {
            ViewData["post_line"] = Db.EXP_POST_LINE.Select(o => new { o.EPOL_ID, o.EPOL_NAME }).AsEnumerable();
            ViewData["project"] = Db.CGT_PRO
                                    .Where(o => o.CPRO_PRJ_CODE == Db.EXP_EXPI_DOC.Select(p => p.CPRO_PRJ_CODE).FirstOrDefault())
                                    .Where(o => o.CPLA_PLN_CODE == Db.EXP_EXPI_DOC.Select(p => p.CPRO_CPLA_PLN_CODE).FirstOrDefault())
                                    .Select(o => new { o.PRJ_CODE, o.PRJ_DESC })
                                    .AsEnumerable();
            ViewData["Organ"] = Db.PAY_ORGAN.Select(o => new { o.CODE, o.ORGA_DESC }).AsEnumerable();
            //ViewData["Type_Doc"] = cntx.EXP_TYPE_DOC.Select(o => new { o.ETDO_ID, o.ETDO_DESC }).Where(o => o.ETDO_ID == (cntx.EXP_TYPE_DOC.Select(o => new { o.ETDO_ID }))).AsEnumerable();
            ViewData["Type_Doc"] = Db.EXP_EXPI_DOC.Select(o => new { o.EEDO_ID, o.EXP_TYPE_DOC, o.ETDO_ETDO_ID, o.EXP_TYPE_DOC.ETDO_DESC }).Where(o => o.ETDO_ETDO_ID == o.EXP_TYPE_DOC.ETDO_ID).AsEnumerable();
            ViewData["Doc_val"] = Db.EXP_EITEM_DOC_VALUE.Select(o => new { o.EIDR_ID, o.EIDR_VALUE });
            ViewData["post_instr"] = Db.EXP_POST_LINE_INSTRU.Select(o => new { o.EINS_EINS_ID, o.CODE_DISP });
            ViewData["JDTYid"] = Db.SEC_JOB_TYPE_DOC.Select(o => new { o.JDTY_ID, o.ACTIV_FNAM });
            ViewData["epix"] = Db.EXP_PERSON_EXPLI.Select(o => new { o.EPEX_ID, o.EPEX_NAME });

            ViewBag.Instrument = Db.EXP_INSTRUMENT.Where(c => c.EINS_EINS_ID != null).Select(c => new { c.EINS_ID, c.EINS_DESC });
            ViewBag.Bay = Db.EXP_TYPE_BAY.Select(c => new { c.ETBY_ID, c.ETBY_DESC });
            ViewBag.UniteV = Db.EXP_UNIT_LEVEL.Select(c => new { c.EUNL_ID, c.EUNL_DESC });
            ViewBag.post = Db.EXP_POST_LINE.Select(c => new { c.EPOL_ID, c.EPOL_NAME });
            ViewBag.PostInstrument = Db.EXP_POST_LINE_INSTRU.Select(c => new { c.EPIU_ID, c.CODE_NAME });
            ViewBag.Offstat = Db.EXP_OFF_STAT.Select(c => new { c.EOFS_ID, c.EOFS_DESC });
            ViewBag.PFUNCTION = Db.EXP_PFUNCTION.Select(c => new { c.EFUN_ID, c.EFUN_DESC });

            return View();
        }

        public ActionResult ViewFormRequest(string id, string notId)
        {
            ViewData["post_line"] = Db.EXP_POST_LINE.Select(o => new { o.EPOL_ID, o.EPOL_NAME }).AsEnumerable();
            ViewData["project"] = Db.CGT_PRO
                                    .Where(o => o.CPRO_PRJ_CODE == Db.EXP_EXPI_DOC.Select(p => p.CPRO_PRJ_CODE).FirstOrDefault())
                                    .Where(o => o.CPLA_PLN_CODE == Db.EXP_EXPI_DOC.Select(p => p.CPRO_CPLA_PLN_CODE).FirstOrDefault())
                                    .Select(o => new { o.PRJ_CODE, o.PRJ_DESC })
                                    .AsEnumerable();
            ViewData["Organ"] = Db.PAY_ORGAN.Select(o => new { o.CODE, o.ORGA_DESC }).AsEnumerable();
            //ViewData["Type_Doc"] = cntx.EXP_TYPE_DOC.Select(o => new { o.ETDO_ID, o.ETDO_DESC }).Where(o => o.ETDO_ID == (cntx.EXP_TYPE_DOC.Select(o => new { o.ETDO_ID }))).AsEnumerable();
            ViewData["Type_Doc"] = Db.EXP_EXPI_DOC.Select(o => new { o.EEDO_ID, o.EXP_TYPE_DOC, o.ETDO_ETDO_ID, o.EXP_TYPE_DOC.ETDO_DESC }).Where(o => o.ETDO_ETDO_ID == o.EXP_TYPE_DOC.ETDO_ID).AsEnumerable();
            ViewData["Doc_val"] = Db.EXP_EITEM_DOC_VALUE.Select(o => new { o.EIDR_ID, o.EIDR_VALUE });
            ViewData["post_instr"] = Db.EXP_POST_LINE_INSTRU.Select(o => new { o.EINS_EINS_ID, o.CODE_DISP });
            ViewData["JDTYid"] = Db.SEC_JOB_TYPE_DOC.Select(o => new { o.JDTY_ID, o.ACTIV_FNAM });

            ViewBag.Instrument = Db.EXP_INSTRUMENT.Where(c => c.EINS_EINS_ID != null).Select(c => new { c.EINS_ID, c.EINS_DESC });
            ViewBag.Bay = Db.EXP_TYPE_BAY.Select(c => new { c.ETBY_ID, c.ETBY_DESC });
            ViewBag.UniteV = Db.EXP_UNIT_LEVEL.Select(c => new { c.EUNL_ID, c.EUNL_DESC });
            ViewBag.post = Db.EXP_POST_LINE.Select(c => new { c.EPOL_ID, c.EPOL_NAME });
            ViewBag.PostInstrument = Db.EXP_POST_LINE_INSTRU.Select(c => new { c.EPIU_ID, c.CODE_NAME });
            ViewBag.Offstat = Db.EXP_OFF_STAT.Select(c => new { c.EOFS_ID, c.EOFS_DESC });
            ViewBag.PFUNCTION = Db.EXP_PFUNCTION.Select(c => new { c.EFUN_ID, c.EFUN_DESC });

            ViewBag.exp_doc = Db.EXP_EXPI_DOC;

            EXP_EXPI_DOC cm = new EXP_EXPI_DOC();
            EXP_ITEM_TYPE_DOC cmitem = new EXP_ITEM_TYPE_DOC();
            EXP_EDOC_INSTRU cminstru = new EXP_EDOC_INSTRU();
            Equipment.Models.PartialClass.MultiModelPrj multicm = new Equipment.Models.PartialClass.MultiModelPrj();

            //    AsrWorkFlowProcess p1i = new AsrWorkFlowProcess(Convert.ToInt32(notId));
            //  cm = p.GetEntity<EXP_EXPI_DOC>();
            // p.GetKeyValue("Body");
            //  string s=p1i.CurrentStat;

            var ui = id.Split('A');
            string idd = ui[0].ToString();

            cm = Db.Database.SqlQuery<EXP_EXPI_DOC>("select * from EXP_EXPI_DOC where {0}".FormatWith(idd)).FirstOrDefault();
            multicm.EXP_EXPI_DOC = cm;

            Session["notid"] = notId;
            ViewBag.key = notId;
            Session["etdo_id"] = multicm.EXP_EXPI_DOC.ETDO_ETDO_ID;

            Session["eedo_id"] = multicm.EXP_EXPI_DOC.EEDO_ID;
            ViewBag.stedokey = 183;

            return View("PartialViewForm", multicm);
        }

        public ActionResult programgete(string etdo_id)
        {
            int i = Convert.ToInt32(etdo_id);
            var query = from k in Db.EXP_PROGRAM
                        where k.ETDO_ETDO_ID == i
                        select new { k.EPRO_ID, k.EPRO_DESC };
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getbay(int post)
        {
            var q = from b in Db.EXP_POST_LINE join j in Db.EXP_POST_LINE_INSTRU on b.EPOL_ID equals j.EPOL_EPOL_ID select j;
            var q1 = from b in q join j in Db.EXP_INSTRU_BAY on b.EPIU_ID equals j.EPIU_EPIU_ID select j.ECBA_ECBA_ID;

            var query = (from b in Db.EXP_TYPE_BAY
                         join j in Db.EXP_CORE_BAY on b.ETBY_ID equals j.ETBY_ETBY_ID
                         where q1.Contains(j.ECBA_ID)
                         select new { b.ETBY_ID, b.ETBY_DESC }).Distinct();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public class CustomerSearchModel
        {
            public string Type_Instrument { get; set; }

            public int etby { get; set; }

            public int post3 { get; set; }
        }

        public ActionResult GetPostInstrument(CustomerSearchModel customerSearchModel)
        {
            string Type_Instrument = customerSearchModel.Type_Instrument;
            int etby = customerSearchModel.etby;
            int postpo = customerSearchModel.post3;

            //if (Type_Instrument == "1")
            //{

            //    var RetVal = from b in Context.EXP_POST_LINE_INSTRU
            //                 join p in Context.EXP_POST_LINE on b.EPOL_EPOL_ID equals p.EPOL_ID
            //                 where p.EPOL_TYPE == "1" && (b.EPOL_EPOL_ID_INSLIN == postpo || b.EPOL_EPOL_ID_LINE == postpo)
            //                 && (b.EPOL_EPOL_ID_INSLIN != null && b.EPOL_EPOL_ID_LINE != null) && b.EPIU_EPIU_ID_SAVABEGH == null
            //                 select new { b.EPIU_ID, b.CODE_NAME };

            //    return Json(RetVal, JsonRequestBehavior.AllowGet);

            //}
            //else
            //{
            if (etby == 0)
            {
                //var RetVal = from b in Context.EXP_POST_LINE_INSTRU
                //             where b.EPOL_EPOL_ID == postpo && b.EPIU_EPIU_ID_SAVABEGH == null
                //             select new { b.EPIU_ID, b.CODE_NAME };
                var RetVal = from b in Db.EXP_POST_LINE_INSTRU
                             join p in Db.EXP_POST_LINE on b.EPOL_EPOL_ID equals p.EPOL_ID
                             where (p.EPOL_TYPE == "1" && (b.EPOL_EPOL_ID_INSLIN == postpo || b.EPOL_EPOL_ID_LINE == postpo) &&
                                    (b.EPOL_EPOL_ID_INSLIN != null && b.EPOL_EPOL_ID_LINE != null) && b.EPIU_EPIU_ID_SAVABEGH == null) ||
                                   (p.EPOL_TYPE == "0" && b.EPOL_EPOL_ID == postpo && b.EPIU_EPIU_ID_SAVABEGH == null)
                             select new
                             {
                                 b.EPIU_ID,
                                 b.CODE_NAME,
                                 b.PHAS_TYPE,
                                 b.PHAS_STAT,
                                 cexecdesc = b.EXP_TYPE_EQUIP.ETEX_DESC
                             };

                return Json(RetVal, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = from b in Db.EXP_TYPE_BAY
                            join j in Db.EXP_CORE_BAY on b.ETBY_ID equals j.ETBY_ETBY_ID
                            where b.ETBY_ID == etby
                            select j.EPIU_EPIU_ID;

                var q = from b in Db.EXP_POST_LINE
                        join j in Db.EXP_POST_LINE_INSTRU on b.EPOL_ID equals j.EPOL_EPOL_ID
                        where b.EPOL_ID == postpo && query.Contains(j.EPIU_ID) && j.EPIU_EPIU_ID_SAVABEGH == null
                        select new
                        {
                            j.EPIU_ID,
                            j.CODE_NAME,
                            j.PHAS_TYPE,
                            j.PHAS_STAT,
                            cexecdesc = j.EXP_TYPE_EQUIP.ETEX_DESC
                        };

                return Json(q, JsonRequestBehavior.AllowGet);
            }
            //  }
        }

        public ActionResult getcancelresn(string curents)
        {
            var query1 = from b in Db.EXP_CANCEL_RESN
                         join j in Db.EXP_CRESN_JOB on b.ECRE_ID equals j.ECRE_ECRE_ID
                         join k in Db.SEC_JOB_TYPE_DOC on j.JDTY_JDTY_ID equals k.JDTY_ID
                         where k.ACTIV_NAME == curents
                         select new { j.EREJ_ID, b.ECRE_DESC };

            return Json(query1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getinstrument(int post)
        {
            var query1 = (from b in Db.EXP_INSTRUMENT
                          join y in Db.EXP_POST_LINE_INSTRU on b.EINS_ID equals y.EINS_EINS_ID
                          where b.EINS_EINS_ID != null && y.EPOL_EPOL_ID == post
                          select new { b.EINS_ID, b.EINS_DESC }).Distinct();

            //var query = from b in Context.EXP_INSTRUMENT where b.EINS_ID == 1 select new { b.EINS_ID, b.EINS_DESC };

            return Json(query1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult functiongete(string epro_id)
        {
            if (epro_id == "")
                epro_id = "0";
            int epro_epro_id = Convert.ToInt32(epro_id);
            var RetVal = (from b in PublicRepository.get_function() where (b.EPRO_EPRO_ID == epro_epro_id) orderby b.EFUN_DESC select b).AsEnumerable().Select(b => new { b.EFUN_ID, b.EFUN_DESC });
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getprogram1()
        {
            var query = from k in Db.EXP_PROGRAM
                        select new { k.EPRO_ID, k.EPRO_DESC };
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult insertinstru_new(int? id, int type)
        {
            Session["typeidinstrument"] = type;
            if (id != 0)
            {
                if (type == 0)
                {
                    Session["requid"] = id;
                    Session["idinstrument"] = 0;
                    return View();
                }
                else
                {
                    string q = (from b in Db.EXP_EDOC_INSTRU where b.EDIN_ID == id select b.EEDO_EEDO_ID).FirstOrDefault().ToString();
                    Session["requid"] = Convert.ToInt16(q);
                    Session["idinstrument"] = id;
                    EXP_EDOC_INSTRU EXPNEW = (from b in Db.EXP_EDOC_INSTRU where b.EDIN_ID == id select b).FirstOrDefault();
                    return View(EXPNEW);
                }
            }
            else
            {
                return View("ErrorNotFound");
            }
        }

        public ActionResult Inserinstrumentnew(EXP_EDOC_INSTRU objecttemp)
        {
            try
            {
                int requid = Convert.ToInt16(Session["requid"].ToString());
                int idinstrument = Convert.ToInt16(Session["idinstrument"].ToString());
                int idinst = 0;
                int idetby1 = 0;
                int idinstt = 0;
                int postcode = 0;

                var rel1 = new EXP_EDOC_INSTRU();
                string POSTINST = string.Empty;
                string etby1 = string.Empty;
                string eins = string.Empty;
                string ETDO = (from b in Db.EXP_EXPI_DOC where b.EEDO_ID == requid select b.ETDO_ETDO_ID).FirstOrDefault().ToString();
                if (idinstrument != 0)
                {
                    rel1 = (from b in Db.EXP_EDOC_INSTRU where b.EDIN_ID == idinstrument select b).FirstOrDefault();
                }
                var einst = (from p in Db.EXP_EDOC_INSTRU where p.EEDO_EEDO_ID == requid && p.CUT_STAT == "1" select p).FirstOrDefault();
                string offdate = einst.OFF_DATE.ToString();
                string onDATE = einst.ON_DATE.ToString();

                string time = string.Empty;

                //string offdate = Request.Form["ofDATEreq"];
                //string onDATE = Request.Form["onDATEreq"];
                if (Request.Form["off_TIME"] != null && Request.Form["on_TIME"] != null)
                {
                    string off_TIME1 = Request.Form["off_TIME"];
                    string on_TIME1 = Request.Form["on_TIME"];

                    if (Request.Form["stattime"].ToString() == "0")
                    {
                        time = (Db.Database.SqlQuery<string>("SELECT TIME_BETWEEN_U('" + offdate + "','" + off_TIME1 + "','" + onDATE + "','" + on_TIME1 + "')  FROM dual").FirstOrDefault());
                    }
                    else
                    {
                        int day = (Db.Database.SqlQuery<int>("SELECT FDAYS_BETWEEN_U('" + offdate + "','" + onDATE + "')  FROM dual").FirstOrDefault());

                        string time1 = (Db.Database.SqlQuery<string>("SELECT TIME_BETWEEN_U('" + offdate + "','" + off_TIME1 + "','" + offdate + "','" + on_TIME1 + "')  FROM dual").FirstOrDefault());
                        if (day == 0)
                        {
                            time = time1;
                        }
                        else
                        {
                            var val = time1.Split(':');
                            string hhoff = val[0].ToString();
                            string ssoff = val[1].ToString();

                            int hh = Convert.ToInt16(hhoff) * (day + 1);
                            int ss = Convert.ToInt16(ssoff) * (day + 1);
                            if (ss >= 60)
                            {
                                hh = hh + ss / 60;
                                ss = ss % 60;
                            }
                            time = hh + ":" + ss;
                        }
                    }
                    rel1.CONT_FUN = time;
                    if (Request.Form["stattime"].ToString() == "0")
                    {
                        if (offdate.CompareTo(onDATE) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                        }
                        else
                        {
                            if (offdate.CompareTo(onDATE) == 0)
                            {
                                if (off_TIME1.CompareTo(on_TIME1) > 0)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (offdate.CompareTo(onDATE) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                        }
                        else
                        {
                            if (off_TIME1.CompareTo(on_TIME1) > 0)
                            {
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                            }
                        }
                    }
                }

                if (Request.Form["off_TIMEprog"] != null && Request.Form["on_TIMEprog"] != null)
                {
                    string offdate2 = Request.Form["ofDATEprog"];
                    string onDATE2 = Request.Form["onDATEprog"];

                    string off_TIME2 = Request.Form["off_TIMEprog"];
                    string on_TIME2 = Request.Form["on_TIMEprog"];

                    var einst1 = (from p in Db.EXP_EDOC_INSTRU
                                  join b in Db.SEC_JOB_TYPE_DOC on p.JDTY_JDTY_ID equals b.JDTY_ID
                                  where p.EEDO_EEDO_ID == requid && p.CUT_STAT == "1" && b.ETDO_ETDO_ID == 21 && b.ACTIV_NAME == "CREATOR"
                                  select p).FirstOrDefault();

                    string stime = einst1.TIME_ISTA.ToString();

                    if (stime == "0")
                    {
                        if (offdate2.CompareTo(onDATE2) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                        }
                        else
                        {
                            if (offdate2.CompareTo(onDATE2) == 0)
                            {
                                if (off_TIME2.CompareTo(on_TIME2) > 0)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (offdate2.CompareTo(onDATE2) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                        }
                        else
                        {
                            if (off_TIME2.CompareTo(on_TIME2) > 0)
                            {
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                            }
                        }
                    }
                }

                if (Request.Form["off_TIMEh"] != null && Request.Form["on_TIMEh"] != null)
                {
                    string offdate2 = Request.Form["offDATEh"];
                    string onDATE2 = Request.Form["onDATEh"];

                    string off_TIME2 = Request.Form["off_TIMEh"];
                    string on_TIME2 = Request.Form["on_TIMEh"];

                    var einst2 = (from p in Db.EXP_EDOC_INSTRU
                                  join b in Db.SEC_JOB_TYPE_DOC on p.JDTY_JDTY_ID equals b.JDTY_ID
                                  where p.EEDO_EEDO_ID == requid && p.CUT_STAT == "1" && b.ETDO_ETDO_ID == 21 && b.ACTIV_NAME == "CREATOR"
                                  select p).FirstOrDefault();

                    string stime = einst2.TIME_ISTA.ToString();

                    if (stime == "0")
                    {
                        if (offdate2.CompareTo(onDATE2) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                        }
                        else
                        {
                            if (offdate2.CompareTo(onDATE2) == 0)
                            {
                                if (off_TIME2.CompareTo(on_TIME2) > 0)
                                {
                                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (offdate2.CompareTo(onDATE2) > 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "تاریخ شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                        }
                        else
                        {
                            if (off_TIME2.CompareTo(on_TIME2) > 0)
                            {
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان شروع از پایان بزرگتر نمی تواند باشد" }.ToJson();
                            }
                        }
                    }
                }
                //if (idinstrument != 0)
                //{

                //    rel1 = (from b in cntx.EXP_EDOC_INSTRU where b.EDIN_ID == idinstrument select b).FirstOrDefault();
                //    rel1.EPIU_EPIU_ID = null;
                //    rel1.EINS_EINS_ID = null;
                //    rel1.ETBY_ETBY_ID = null;          
                //}

                if (Request.Form["POSTINST"] != "" && Request.Form["POSTINST"] != null)
                {
                    POSTINST = Request.Form["POSTINST"];
                    idinst = Convert.ToInt32(POSTINST);
                    rel1.EPIU_EPIU_ID = idinst;
                }
                if (Request.Form["etby1"] != "" && Request.Form["etby1"] != null)
                {
                    POSTINST = Request.Form["POSTINST1"];
                    idinst = Convert.ToInt32(POSTINST);
                    etby1 = Request.Form["etby1"];
                    idetby1 = Convert.ToInt32(etby1);
                    rel1.EPIU_EPIU_ID = idinst;
                    rel1.ETBY_ETBY_ID = idetby1;
                }
                if (Request.Form["einsrequest"] != "" && Request.Form["einsrequest"] != null)
                {
                    eins = Request.Form["einsrequest"];
                    idinstt = Convert.ToInt32(eins);
                    rel1.EINS_EINS_ID = idinstt;
                }

                if (Request.Form["EPOL_EPOL_ID_requ"] != "" && Request.Form["EPOL_EPOL_ID_requ"] != null)
                {
                    postcode = Convert.ToInt32(Request.Form["EPOL_EPOL_ID_requ"]);
                    rel1.EPOL_EPOL_ID = postcode;
                }
                rel1.EEDO_EEDO_ID = requid;
                rel1.OFF_DATE = offdate;
                rel1.ON_DATE = onDATE;

                if (Request.Form["on_TIME"] != "" && Request.Form["on_TIME"] != null)
                {
                    rel1.ON_TIME = Request.Form["on_TIME"];
                }

                if (Request.Form["off_TIME"] != "" && Request.Form["off_TIME"] != null)
                {
                    rel1.OFF_TIME = Request.Form["off_TIME"];
                }

                if (Request.Form["stattime"] != "" && Request.Form["stattime"] != null)
                {
                    rel1.TIME_ISTA = Request.Form["stattime"];
                }
                if (Request.Form["INST_STAT"] != "" && Request.Form["INST_STAT"] != null)
                {
                    rel1.INST_STAT = Request.Form["INST_STAT"];
                }
                if (Request.Form["EART_STAT"] != "" && Request.Form["EART_STAT"] != null)
                {
                    rel1.EART_STAT = Request.Form["EART_STAT"];
                }
                if (Request.Form["CUST_STAT"] != "" && Request.Form["CUST_STAT"] != null)
                {
                    rel1.CUST_STAT = Request.Form["CUST_STAT"];
                }

                if (Request.Form["ofDATEprog"] != "" && Request.Form["ofDATEprog"] != null)
                {
                    rel1.OFF_DATE = Request.Form["ofDATEprog"];
                }
                if (Request.Form["off_TIMEprog"] != "" && Request.Form["off_TIMEprog"] != null)
                {
                    rel1.OFF_TIME = Request.Form["off_TIMEprog"];
                }
                if (Request.Form["onDATEprog"] != "" && Request.Form["onDATEprog"] != null)
                {
                    rel1.ON_DATE = Request.Form["onDATEprog"];
                }

                if (Request.Form["on_TIMEprog"] != "" && Request.Form["on_TIMEprog"] != null)
                {
                    rel1.ON_TIME = Request.Form["on_TIMEprog"];
                }

                if (Request.Form["offDATEh"] != "" && Request.Form["offDATEh"] != null)
                {
                    rel1.OFF_DATE = Request.Form["offDATEh"];
                }
                if (Request.Form["off_TIMEh"] != "" && Request.Form["off_TIMEh"] != null)
                {
                    rel1.OFF_TIME = Request.Form["off_TIMEh"];
                }
                if (Request.Form["onDATEh"] != "" && Request.Form["onDATEh"] != null)
                {
                    rel1.ON_DATE = Request.Form["onDATEh"];
                }

                if (Request.Form["on_TIMEh"] != "" && Request.Form["on_TIMEh"] != null)
                {
                    rel1.ON_TIME = Request.Form["on_TIMEh"];
                }

                if (Request.Form["Unitvolt"] != "" && Request.Form["Unitvolt"] != null)
                {
                    rel1.EUNL_EUNL_ID = Convert.ToInt16(Request.Form["Unitvolt"].ToString());
                }
                if (Request.Form["offstat"] != "" && Request.Form["offstat"] != null)
                {
                    rel1.EOFS_EOFS_ID = Convert.ToInt32(Request.Form["offstat"]);
                }

                if (Request.Form["EDIN_MW"] != "" && Request.Form["EDIN_MW"] != null)
                {
                    rel1.EDIN_MW = Request.Form["EDIN_MW"];
                }

                if (Request.Form["EDIN_OFF"] != "" && Request.Form["EDIN_OFF"] != null)
                {
                    rel1.EDIN_OFF = Request.Form["EDIN_OFF"];
                }

                if (Request.Form["EDIN_MVH"] != "" && Request.Form["EDIN_MVH"] != null)
                {
                    rel1.EDIN_MVH = Request.Form["EDIN_MVH"];
                }

                if (idinstrument == 0)
                {
                    int ETDOi = Convert.ToInt16(ETDO);
                    string Qjdti = (from b in Db.SEC_JOB_TYPE_DOC where b.ETDO_ETDO_ID == ETDOi && b.ROWI_ORDE == 1 select b.JDTY_ID).FirstOrDefault().ToString();
                    decimal QCURENT = Convert.ToDecimal(Qjdti);
                    rel1.JDTY_JDTY_ID = Convert.ToDecimal(QCURENT.ToString());
                    rel1.ETDO_ETDO_ID = ETDOi;

                    Db.EXP_EDOC_INSTRU.Add(rel1);
                    Db.SaveChanges();
                }
                else
                {
                    Db.SaveChanges();
                }

                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", objecttemp.EDIN_ID) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }
        }

        public ActionResult deletedoc(decimal? eedo_id, decimal? edin, decimal? eprog)
        {
            if (eprog != 0)
            {
                var relation = (from b in Db.EXP_RELATION_DOC where b.EEDO_EEDO_ID == eedo_id && b.EDIN_EDIN_ID == eprog select b).FirstOrDefault();

                Db.EXP_RELATION_DOC.Remove(relation);
                //var instrument = (from b in cntx.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == eedo_id && b.CUT_STAT == "1" select b).FirstOrDefault();
                //cntx.EXP_EDOC_INSTRU.Remove(instrument);
            }

            if (edin != 0)
            {
                var relation = (from b in Db.EXP_RELATION_DOC where b.EEDO_EEDO_ID == eedo_id && b.EEDO_EEDO_ID_R == edin select b).FirstOrDefault();

                Db.EXP_RELATION_DOC.Remove(relation);
                //var instrument = (from b in cntx.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == eedo_id && b.CUT_STAT == "1" select b).FirstOrDefault();
                //cntx.EXP_EDOC_INSTRU.Remove(instrument);
            }
            Db.SaveChanges();

            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] حذف شد.", eedo_id) }.ToJson();
        }

        public ActionResult updatedoc(decimal? eedo_id, decimal? edin, decimal? eprog)
        {
            int eedoid = Convert.ToInt32(eedo_id);
            string offdate = Request.Form["ofDATEreq"];
            string onDATE = Request.Form["onDATEreq"];
            string time = string.Empty;
            string off_TIME = Request.Form["off_TIME"];
            string on_TIME = Request.Form["on_TIME"];
            //string off_TIME = off_TIME1.Substring(0, 5);
            //string off_TIMEap = off_TIME1.Substring(6, 2);
            //if (off_TIMEap == "PM")
            //{
            //    string hhoff = off_TIME.Substring(0, 2);
            //    if (hhoff != "12")
            //    {
            //        string ssoff = off_TIME.Substring(3, 2);
            //        int hhoffi = Convert.ToInt16(hhoff) + 12;

            //        off_TIME = hhoffi + ":" + ssoff;
            //    }

            //}
            //if (off_TIMEap == "AM")
            //{
            //    string hhoff = off_TIME.Substring(0, 2);
            //    if (hhoff == "12")
            //    {
            //        string ssoff = off_TIME.Substring(3, 2);
            //        hhoff = "00";

            //        off_TIME = hhoff + ":" + ssoff;
            //    }

            //}
            //string on_TIME = on_TIME1.Substring(0, 5);
            //string on_TIMEap = on_TIME1.Substring(6, 2);

            //if (on_TIMEap == "PM")
            //{
            //    string hhoff = on_TIME.Substring(0, 2);
            //    if (hhoff != "12")
            //    {
            //        string ssoff = on_TIME.Substring(3, 2);
            //        int hhoffi = Convert.ToInt16(hhoff) + 12;

            //        on_TIME = hhoffi + ":" + ssoff;
            //    }
            //}
            //if (on_TIMEap == "AM")
            //{
            //    string hhoff = on_TIME.Substring(0, 2);
            //    if (hhoff == "12")
            //    {
            //        string ssoff = on_TIME.Substring(3, 2);
            //        hhoff = "00";

            //        on_TIME = hhoff + ":" + ssoff;
            //    }
            //}

            if (Request.Form["stattime"].ToString() == "0")
            {
                time = (Db.Database.SqlQuery<string>("SELECT TIME_BETWEEN_U('" + offdate + "','" + off_TIME + "','" + onDATE + "','" + on_TIME + "')  FROM dual").FirstOrDefault());
            }
            else
            {
                int day = (Db.Database.SqlQuery<int>("SELECT FDAYS_BETWEEN_U('" + offdate + "','" + onDATE + "')  FROM dual").FirstOrDefault());

                string time1 = (Db.Database.SqlQuery<string>("SELECT TIME_BETWEEN_U('" + offdate + "','" + off_TIME + "','" + offdate + "','" + on_TIME + "')  FROM dual").FirstOrDefault());
                if (day == 0)
                {
                    time = time1;
                }
                else
                {
                    var val = time1.Split(':');
                    string hhoff = val[0].ToString();
                    string ssoff = val[1].ToString();

                    int hh = Convert.ToInt16(hhoff) * (day + 1);
                    int ss = Convert.ToInt16(ssoff) * (day + 1);
                    if (ss >= 60)
                    {
                        hh = hh + ss / 60;
                        ss = ss % 60;
                    }
                    time = hh + ":" + ss;
                }
            }

            //   string time = (cntx.Database.SqlQuery<string>("SELECT TIME_BETWEEN_U('" + offdate + "','" + off_TIME + "','" + onDATE + "','" + on_TIME + "')  FROM dual").FirstOrDefault());

            if (eprog != 0)
            {
                string cou = (from b in Db.EXP_RELATION_DOC where b.EEDO_EEDO_ID == eedo_id && b.EDIN_EDIN_ID == eprog select b).Count().ToString();
                int c = Convert.ToInt32(cou);
                if (c == 0)
                {
                    var rel = new EXP_RELATION_DOC();
                    rel.EEDO_EEDO_ID = eedoid;
                    rel.EDIN_EDIN_ID = eprog;
                    Db.EXP_RELATION_DOC.Add(rel);
                    Db.SaveChanges();
                }
            }

            if (edin != 0)
            {
                string cou = (from b in Db.EXP_RELATION_DOC where b.EEDO_EEDO_ID == eedo_id && b.EEDO_EEDO_ID_R == edin select b).Count().ToString();
                int c = Convert.ToInt32(cou);
                if (c == 0)
                {
                    var rel = new EXP_RELATION_DOC();
                    rel.EEDO_EEDO_ID = eedoid;
                    rel.EEDO_EEDO_ID_R = edin;
                    Db.EXP_RELATION_DOC.Add(rel);
                    Db.SaveChanges();
                }
            }
            string ic = (from b in Db.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == eedoid && b.CUT_STAT == "1" select b).Count().ToString();
            int coi = Convert.ToInt32(ic);
            if (coi != 0)
            {
                var instrument = (from b in Db.EXP_EDOC_INSTRU where b.EEDO_EEDO_ID == eedoid && b.CUT_STAT == "1" select b).FirstOrDefault();
                Db.EXP_EDOC_INSTRU.Remove(instrument);
                Db.SaveChanges();
            }
            var rel1 = new EXP_EDOC_INSTRU();
            string POSTINST = string.Empty;
            string etby1 = string.Empty;
            string eins = string.Empty;
            int idinst = 0;
            int idetby1 = 0;
            int idinstt = 0;
            int postcode = 0;

            if (Request.Form["POSTINST"] != "")
            {
                POSTINST = Request.Form["POSTINST"];
                idinst = Convert.ToInt32(POSTINST);
            }

            if (Request.Form["etby1"] != "")
            {
                POSTINST = Request.Form["POSTINST1"];
                idinst = Convert.ToInt32(POSTINST);
                etby1 = Request.Form["etby1"];
                idetby1 = Convert.ToInt32(etby1);
            }
            if (Request.Form["einsrequest"] != "")
            {
                eins = Request.Form["einsrequest"];
                idinstt = Convert.ToInt32(eins);
            }
            if (Request.Form["EPOL_EPOL_ID_requ"] != "")
            {
                postcode = Convert.ToInt32(Request.Form["EPOL_EPOL_ID_requ"]);
            }

            if (idinstt != 0 || idinst != 0)
            {
                int id = (from b in Db.EXP_EDOC_INSTRU
                          where b.EEDO_EEDO_ID == eedoid && b.EPOL_EPOL_ID == postcode &&
                                (b.EPIU_EPIU_ID == idinst || b.EINS_EINS_ID == idinstt || b.ETBY_ETBY_ID == idetby1)
                          select b.EDIN_ID).Count();
                if (id == 0)
                {
                    rel1.EEDO_EEDO_ID = eedoid;
                    if (idinst != 0)
                    {
                        rel1.EPIU_EPIU_ID = idinst;
                    }
                    if (idinstt != 0)
                    {
                        rel1.EINS_EINS_ID = idinstt;
                    }
                    if (idetby1 != 0)
                    {
                        rel1.ETBY_ETBY_ID = idetby1;
                    }

                    rel1.EPOL_EPOL_ID = postcode;
                    rel1.OFF_DATE = Request.Form["ofDATEreq"];
                    rel1.OFF_TIME = Request.Form["off_TIME"];
                    rel1.ON_DATE = Request.Form["onDATEreq"];
                    rel1.ON_TIME = Request.Form["on_TIME"];
                    rel1.TIME_ISTA = Request.Form["stattime"];
                    rel1.INST_STAT = Request.Form["INST_STAT"];
                    rel1.EART_STAT = Request.Form["EART_STAT"];
                    rel1.CUST_STAT = Request.Form["CUST_STAT"];
                    rel1.CONT_FUN = time;
                    rel1.CUT_STAT = "1";

                    if (Request.Form["Unitvolt"] != "")
                    {
                        rel1.EUNL_EUNL_ID = Convert.ToInt16(Request.Form["Unitvolt"].ToString());
                    }
                    if (Request.Form["offstat"] != "")
                    {
                        rel1.EOFS_EOFS_ID = Convert.ToInt32(Request.Form["offstat"]);
                    }
                    Db.EXP_EDOC_INSTRU.Add(rel1);
                    Db.SaveChanges();

                    var suplier = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == eedoid && b.POSI_TYEP == "2" select b).FirstOrDefault();
                    Db.EXP_SUPL_DOC.Remove(suplier);
                    Db.SaveChanges();
                    var suplierj = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == eedoid && b.POSI_TYEP == "5" select b).FirstOrDefault();
                    Db.EXP_SUPL_DOC.Remove(suplierj);
                    Db.SaveChanges();

                    int sup = 0;
                    int sup1 = 0;

                    var rel2 = new EXP_SUPL_DOC();
                    var rel3 = new EXP_SUPL_DOC();
                    if (Request.Form["EPEX_EPEX_ID"] != "")
                        sup = Convert.ToInt32(Request.Form["EPEX_EPEX_ID"].ToString());
                    if (Request.Form["EPEX_EPEX_ID1"] != "")
                        sup1 = Convert.ToInt32(Request.Form["EPEX_EPEX_ID1"].ToString());

                    if (sup != 0)
                    {
                        int idm = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == eedoid && b.EPEX_EPEX_ID == sup select b.ESUD_ID).Count();
                        if (idm == 0)
                        {
                            rel2.EEDO_EEDO_ID = eedoid;

                            rel2.POSI_TYEP = "2";
                            rel2.EPEX_EPEX_ID = sup;
                            rel2.ESUD_DESC = Request.Form["group"];
                            Db.EXP_SUPL_DOC.Add(rel2);
                            Db.SaveChanges();
                        }
                        int idm1 = (from b in Db.EXP_SUPL_DOC where b.EEDO_EEDO_ID == eedoid && b.EPEX_EPEX_ID == sup1 select b.ESUD_ID).Count();
                        if (idm1 == 0)
                        {
                            rel3.EEDO_EEDO_ID = eedoid;

                            rel3.POSI_TYEP = "5";
                            rel3.EPEX_EPEX_ID = sup1;

                            Db.EXP_SUPL_DOC.Add(rel3);
                            Db.SaveChanges();
                        }
                    }
                    var chkl = (from b in Db.EXP_EXPI_DOC where b.EEDO_ID == eedoid select b).FirstOrDefault();

                    chkl.EEDO_DESC = Request.Form["EXP_EXPI_DOC.EEDO_DESC"];
                    chkl.EEDO_DATE = Request.Form["EXP_EXPI_DOC.EEDO_DATE"];
                    chkl.ORGA_CODE = Request.Form["ORGA_CODE_request"];

                    chkl.EPOL_EPOL_ID = Convert.ToInt32(Request.Form["EPOL_EPOL_ID_requ"]);

                    chkl.EFUN_EFUN_ID = Convert.ToInt32(Request.Form["function"]);

                    chkl.OUT_FUNC = Request.Form["EXP_EXPI_DOC.OUT_FUNC"];

                    Db.SaveChanges();

                    var relation = (from b in Db.EXP_RELATION_DOC
                                    join c in Db.EXP_EXPI_DOC on b.EEDO_EEDO_ID_R equals c.EEDO_ID
                                    where b.EEDO_EEDO_ID == eedo_id && c.ETDO_ETDO_ID == 21
                                    select b).FirstOrDefault();

                    Db.EXP_RELATION_DOC.Remove(relation);

                    var rel = new EXP_RELATION_DOC();
                    if (!string.IsNullOrEmpty(Request.Form["master_doc"]))
                    {
                        rel.EEDO_EEDO_ID = eedoid;
                        rel.EEDO_EEDO_ID_R = Convert.ToInt32(Request.Form["master_doc"]);
                        Db.EXP_RELATION_DOC.Add(rel);
                        Db.SaveChanges();
                    }
                }
            }

            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] حذف شد.", eedo_id) }.ToJson();
        }

        public ActionResult Getcutoff()
        {
            var query = from b in Db.EXP_OFF_STAT
                        select new
                        {
                            b.EOFS_ID,
                            b.EOFS_DESC
                        };
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //      public JsonResult Approvereq(int notId, string eedoid, string sid)

        public JsonResult Approvereq(int notId, string eedoid, string sid, int stat, int Ersalstat)
        {
            int eedoid1 = Convert.ToInt16(eedoid);

            AsrWorkFlowProcess wp = new AsrWorkFlowProcess(notId);
            //   AsrJobProvider jpv1 = new AsrJobProvider(wp.PreviousState, wp.FlowName);
            string TEDO = string.Empty;
            string prog = string.Empty;
            string idrole = string.Empty;
            string flowid = string.Empty;
            if (sid == "MANEGERCR")
            {
                var qflag = (from b in Db.EXP_EXPI_DOC
                             join p in Db.EXP_PFUNCTION on b.EFUN_EFUN_ID equals p.EFUN_ID
                             join t in Db.EXP_PROGRAM on p.EPRO_EPRO_ID equals t.EPRO_ID
                             where b.EEDO_ID == eedoid1
                             select t).FirstOrDefault();

                if (qflag == null)
                {
                    prog = "20";
                }
                else
                {
                    prog = qflag.EPRO_ID.ToString();
                    TEDO = qflag.ETDO_ETDO_ID.ToString();
                }

                if (prog == "25") /* اضطراري*/
                {
                    wp.SetKeyValue("MY_FLAG", "EMARJENCY");
                    idrole = "CONTROLCENTER";
                }
                else
                {
                    if (prog == "11")  /*برنامه ادراه نظارت */
                    {
                        wp.SetKeyValue("MY_FLAG", "NEZARATTYPE");
                        idrole = "CONTROLCENTER";
                        idrole = "PROGRAMER";
                    }
                    else
                    {
                        if (prog == "17" || TEDO == "101") /* برنامه دفتر فني */
                        {
                            wp.SetKeyValue("MY_FLAG", "FANIP");
                            idrole = "FANIC";
                        }
                        else
                        {
                            wp.SetKeyValue("MY_FLAG", "OTHER");
                            idrole = "ENTEGHALC";
                        }
                    }
                }
            }
            if (sid == "FANIC")
            {
                decimal ee = eedoid1;

                var qflag = (from b in Db.EXP_EXPI_DOC
                             join p in Db.EXP_EDOC_INSTRU on b.EEDO_ID equals p.EEDO_EEDO_ID
                             where b.EEDO_ID == ee && p.CUT_STAT == "1"
                             select p).FirstOrDefault();

                string flagcheck = "0";
                if (qflag.EUNL_EUNL_ID != null)
                {
                    int uid = Convert.ToInt16(qflag.EUNL_EUNL_ID.ToString());
                    var qunitiddesc = (from b in Db.EXP_UNIT_LEVEL
                                       where b.EUNL_ID == uid
                                       select b).FirstOrDefault();

                    int eunlnum = qunitiddesc.EUNL_NUM;
                    if (eunlnum > 132)
                    {
                        int nfeinst = (from p in Db.EXP_EDOC_INSTRU
                                       join c in Db.EXP_POST_LINE_INSTRU on p.EPIU_EPIU_ID equals c.EPIU_ID
                                       where p.EEDO_EEDO_ID == ee && c.EINS_EINS_ID == 5 && c.EUNL_EUNL_ID == 27
                                       select p).Count();
                        if (nfeinst == 0)
                        {
                            flagcheck = "0";
                        }
                        else
                        {
                            flagcheck = "1";
                        }
                    }
                    else
                    {
                        flagcheck = "0";
                        var qfeinst = (from p in Db.EXP_EDOC_INSTRU
                                       where p.EEDO_EEDO_ID == ee && p.CUST_STAT == "0"
                                       select p);
                        foreach (var qn in qfeinst)
                        {
                            string mod = qn.CONT_FUN;
                            if (mod != null)
                            {
                                var m = mod.Split(':');
                                int im = Convert.ToInt16(m[0].ToString());
                                if (im > 2)
                                {
                                    flagcheck = "1";
                                }
                            }
                        }
                    }
                }
                if (flagcheck == "0")
                {
                    wp.SetKeyValue("MY_FLAG", "0");
                    idrole = "PROGRAMER";
                }
                else
                {
                    wp.SetKeyValue("MY_FLAG", "1");
                    idrole = "MANEGCONFIRM";
                }
            }
            if (sid == "PROGRAMER" && stat == 1 && Ersalstat == -1)
            {
                wp.SetKeyValue("MY_FLAG", "1");
                idrole = "ARCHEIVE";
            }

            if (sid == "PROGRAMER" && stat == 0 && Ersalstat == -1)
            {
                wp.SetKeyValue("MY_FLAG", "0");
                idrole = "CREATOR";
            }
            //if (sid == "PROGRAMER" && Ersalstat > -1)
            //{
            //    if (Ersalstat == 0)
            //    { wp.SetKeyValue("MY_FLAG", "DISKERMAN"); idrole = "DISKERMAN"; }
            //    if (Ersalstat == 1)
            //    { wp.SetKeyValue("MY_FLAG", "DISTRBUTH"); idrole = "DISHORMOZGAN"; }
            //    if (Ersalstat == 2)
            //    { wp.SetKeyValue("MY_FLAG", "DISTRBUTEB"); idrole = "DISBANDAR"; }
            //    if (Ersalstat == 3)
            //    { wp.SetKeyValue("MY_FLAG", "SECCONTROL"); idrole = "SECCONTROL"; }
            //    if (Ersalstat == 4)
            //    { wp.SetKeyValue("MY_FLAG", "FIXORGAN"); idrole = "FIXORGAN"; }
            //    if (Ersalstat == 5)
            //    { wp.SetKeyValue("MY_FLAG", "LINEORGAN"); idrole = "LINEORGAN"; }
            //  /* if (Ersalstat == 6)
            //       wp.SetKeyValue("MY_FLAG", "ENTGHAL"); idrole = "DISKERMAN";*/

            //}

            AsrJobProvider jp = new AsrJobProvider(idrole, wp.FlowName);
            return this.Json(new { Success = true, data = jp.AllUsers }, JsonRequestBehavior.AllowGet);
            // return this.Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult USERCREATEREQUEST()
        {
            string idrole = "CREATOR";
            AsrJobProvider jp = new AsrJobProvider(idrole, "FLW_REQU");
            return this.Json(new { Success = true, data = jp.AllUsers }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Approvesendreqinreq(string eedoid, int notId)
        {
            AsrWorkFlowProcess wp = new AsrWorkFlowProcess(notId);

            string cm = string.Empty;

            cm = Db.Database.SqlQuery<string>("select ITEM_KEY from WF_NOTE_V where NOT_ID=" + notId).FirstOrDefault();

            var string1 = cm.Split('^');
            string s1 = string1[1].ToString();
            var string2 = s1.Split('-');
            string Ersalstat = string2[1].ToString();

            string idrole = string.Empty;

            if (Ersalstat == "0")
            {
                wp.SetKeyValue("MY_FLAG", "DISKERMAN");
                idrole = "DISKERMAN";
            }
            if (Ersalstat == "1")
            {
                wp.SetKeyValue("MY_FLAG", "DISTRBUTH");
                idrole = "DISHORMOZGAN";
            }
            if (Ersalstat == "2")
            {
                wp.SetKeyValue("MY_FLAG", "DISTRBUTEB");
                idrole = "DISBANDAR";
            }
            if (Ersalstat == "3")
            {
                wp.SetKeyValue("MY_FLAG", "SECCONTROL");
                idrole = "SECCONTROL";
            }
            if (Ersalstat == "4")
            {
                wp.SetKeyValue("MY_FLAG", "FIXORGAN");
                idrole = "FIXORGAN";
            }
            if (Ersalstat == "5")
            {
                wp.SetKeyValue("MY_FLAG", "LINEORGAN");
                idrole = "LINEORGAN";
            }
            if (Ersalstat == "6")
            {
                wp.SetKeyValue("MY_FLAG", "ENTGHAL");
                idrole = "ENTEGHAL";
            }

            AsrJobProvider jp = new AsrJobProvider(idrole, "FLW_REHA");
            return this.Json(new { Success = true, data = jp.AllUsers }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Approvesendreq(string eedoid, int Ersalstat, int notId)
        {
            AsrWorkFlowProcess wp = new AsrWorkFlowProcess(notId);

            string idrole = string.Empty;
            if (Ersalstat == 0)
            {
                wp.SetKeyValue("MY_FLAG", "DISKERMAN");
                idrole = "DISKERMAN";
            }
            if (Ersalstat == 1)
            {
                wp.SetKeyValue("MY_FLAG", "DISTRBUTH");
                idrole = "DISHORMOZGAN";
            }
            if (Ersalstat == 2)
            {
                wp.SetKeyValue("MY_FLAG", "DISTRBUTEB");
                idrole = "DISBANDAR";
            }
            if (Ersalstat == 3)
            {
                wp.SetKeyValue("MY_FLAG", "SECCONTROL");
                idrole = "SECCONTROL";
            }
            if (Ersalstat == 4)
            {
                wp.SetKeyValue("MY_FLAG", "FIXORGAN");
                idrole = "FIXORGAN";
            }
            if (Ersalstat == 5)
            {
                wp.SetKeyValue("MY_FLAG", "LINEORGAN");
                idrole = "LINEORGAN";
            }
            if (Ersalstat == 6)
            {
                wp.SetKeyValue("MY_FLAG", "ENTGHAL");
                idrole = "ENTEGHAL";
            }

            AsrJobProvider jp = new AsrJobProvider(idrole, "FLW_REHA");
            return this.Json(new { Success = true, data = jp.AllUsers }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Checkconfirm(int? eedoid, string curent)
        {
            decimal ei = Convert.ToDecimal(eedoid.ToString());

            int countq = (from b in Db.EXP_EDOC_INSTRU
                          join k in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals k.JDTY_ID
                          //join x in cntx.EXP_EXPI_DOC on b.EEDO_EEDO_ID equals x.EEDO_ID
                          where b.EEDO_EEDO_ID == ei && k.ACTIV_NAME == curent
                          select b).Count();

            if (countq == 0)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات برای ارسال بررسی نشده است " }.ToJson();
            }
            else
            {
                var q = from b in Db.EXP_EDOC_INSTRU
                        join k in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals k.JDTY_ID
                        //join x in cntx.EXP_EXPI_DOC on b.EEDO_EEDO_ID equals x.EEDO_ID
                        where b.EEDO_EEDO_ID == ei && k.ACTIV_NAME == curent
                        select b;
                foreach (EXP_EDOC_INSTRU qe in q)
                {
                    if ((qe.OFF_TIME == null || qe.ON_TIME == null) && qe.EDIN_STIN == "0")
                    {
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "زمان احرا برنامه مشخص نیست " }.ToJson();
                    }
                    else
                    {
                        if (qe.EREJ_EREJ_ID == null && qe.EDIN_STIN == "1")
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "نحوه لغو برنامه مشخص نیست  " }.ToJson();
                        }
                        else if (qe.EDIN_STIN == null)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "وضعیت اجرا برنامه مشخص نیست" }.ToJson();
                        }
                    }
                }

                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ارسال شد.", 1) }.ToJson();
            }
        }

        public JsonResult confinststat(int? eedoid, string curent)
        {
            decimal ei = Convert.ToDecimal(eedoid.ToString());
            var q = from b in Db.EXP_EDOC_INSTRU
                    join k in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals k.JDTY_ID
                    where b.EEDO_EEDO_ID == ei && k.ACTIV_NAME == curent && b.ATTG_STATT == null
                    select b;
            foreach (EXP_EDOC_INSTRU qe in q)
            {
                qe.ATTG_STATT = "1";
            }
            Db.SaveChanges();
            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ارسال شد.", 1) }.ToJson();
        }

        public JsonResult findnotidsend(string eedoid, string eedin, int JDTY)
        {
            int edi = Convert.ToInt16(eedin);
            int id = Convert.ToInt16(eedoid);
            string sq = (from b in Db.EXP_EDOC_INSTRU
                         join j in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals j.JDTY_ID
                         where b.EDIN_ID == edi
                         select j.ACTIV_NAME).FirstOrDefault().ToString();
            int Ersalstat = -1;
            if (sq == "DISKERMAN")
                Ersalstat = 0;
            if (sq == "DISHORMOZGAN")
                Ersalstat = 1;
            if (sq == "DISBANDAR")
                Ersalstat = 2;
            if (sq == "SECCONTROL")
                Ersalstat = 3;
            if (sq == "FIXORGAN")
                Ersalstat = 4;
            if (sq == "LINEORGAN")
                Ersalstat = 5;
            if (sq == "ENTEGHAL")
                Ersalstat = 6;

            string ci = "FLW_REHA.PFLW_REHA^" + eedoid + "-" + Ersalstat;
            int cm = 0;

            cm = Db.Database.SqlQuery<int>("select max(NOT_ID) from WF_NOTE_V where ITEM_KEY='" + ci.ToString() + "' and STAT='OPEN'").FirstOrDefault();

            int not_id = cm;
            var q = (from b in Db.EXP_EDOC_INSTRU join j in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals j.JDTY_ID where b.EEDO_EEDO_ID == id && j.ACTIV_NAME == sq select b);
            foreach (EXP_EDOC_INSTRU iq in q)
            {
                iq.ATTG_STATT = "2";
                Db.SaveChanges();
            }

            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ارسال شد.", 1), CoustomData = not_id + "%" + Ersalstat }.ToJson();
        }

        public JsonResult SendRequestfh(string eedoid, int Ersalstat)
        {
            AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
            int etdo_id = 183;

            decimal eedoidi = Convert.ToDecimal(eedoid);
            var q = (from b in Db.EXP_EXPI_DOC where b.EEDO_ID == eedoidi select b).FirstOrDefault();
            int epol = Convert.ToInt16(q.EPOL_EPOL_ID.ToString());
            string doc_name = (from b in Db.EXP_TYPE_DOC where b.ETDO_ID == etdo_id select b.ETDO_DESC).FirstOrDefault();

            string postn = (from b in Db.EXP_POST_LINE where b.EPOL_ID == epol select b.EPOL_NAME).FirstOrDefault().ToString();

            string smessage = " درخواست " + "به شماره" + q.DOC_NUMB + " در تاریخ " + q.EEDO_DATE + " مربوط به " + postn + " میباشد ";

            string ci = "FLW_REHA.PFLW_REHA^" + eedoid + "-" + Ersalstat;
            int cm = 0;

            cm = Db.Database.SqlQuery<int>("select count(NOT_ID) from WF_NOTE_V where ITEM_KEY='" + ci.ToString() + "'").FirstOrDefault();

            if (cm > 0)
            {
                if (cm == 1)
                {
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] قبلا ارسال شده است", 1), CoustomData = -1 }.ToJson();
                }
                else
                {
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] قبلا ارسال شده است", 1), CoustomData = -2 }.ToJson();
                }
            }
            else
            {
                //  eedoid = eedoid + Ersalstat;
                wp.StartProcess(this.HttpContext.User.Identity.Name, new string[] { this.HttpContext.User.Identity.Name }, doc_name, smessage, etdo_id, eedoid, Ersalstat);
                int not_id = wp.NoteId;

                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ارسال شد.", 1), CoustomData = not_id }.ToJson();
            }
        }

        public ActionResult Insertrepdate(EXP_EXPI_DOC objecttemp)
        {
            if (Request.Form["ofdaterep"] != "" && Request.Form["ofdaterep"] != null)
            {
                string dateoff = Request.Form["ofdaterep"].ToString();
                int id = Convert.ToInt16(Session["eedo_id"].ToString());
                var qetdo = (from b in Db.EXP_EXPI_DOC where b.EEDO_ID == id select b).FirstOrDefault();
                string eedonumb = qetdo.DOC_NUMB;
                string userersal = qetdo.CRET_BY.ToString();
                int eeto_id = qetdo.ETDO_ETDO_ID.Value;

                objecttemp = qetdo;

                string qdocnum = string.Empty;

                int countq = (from b in Db.EXP_EXPI_DOC where b.ETDO_ETDO_ID == eeto_id && b.EANA_EANA_ROW == null select b).Count();
                if (countq == 0)
                {
                    qdocnum = "1";
                }
                else
                {
                    string qmax = (from b in Db.EXP_EXPI_DOC where b.ETDO_ETDO_ID == eeto_id && b.EANA_EANA_ROW == null select b).Max(c => c.EEDO_ID).ToString();
                    int s1 = Convert.ToInt32(qmax);
                    qdocnum = (from b in Db.EXP_EXPI_DOC where b.EEDO_ID == s1 select b.DOC_NUMB).FirstOrDefault().ToString();
                }
                int m = Convert.ToInt16(qdocnum) + 1;

                objecttemp.DOC_NUMB = m.ToString();

                Db.EXP_EXPI_DOC.Add(objecttemp);
                Db.SaveChanges();
                decimal eedoidnew = objecttemp.EEDO_ID;
                var qinst = (from b in Db.EXP_EDOC_INSTRU join c in Db.SEC_JOB_TYPE_DOC on b.JDTY_JDTY_ID equals c.JDTY_ID where b.EEDO_EEDO_ID == id && c.ACTIV_NAME == "CREATOR" select b);
                foreach (EXP_EDOC_INSTRU iqinst in qinst)
                {
                    iqinst.OFF_DATE = dateoff;
                    iqinst.ON_DATE = dateoff;
                    iqinst.EEDO_EEDO_ID = eedoidnew;
                    Db.EXP_EDOC_INSTRU.Add(iqinst);
                }
                Db.SaveChanges();

                var qrelation = (from b in Db.EXP_RELATION_DOC where b.EEDO_EEDO_ID == id select b);
                foreach (EXP_RELATION_DOC irelation in qrelation)
                {
                    irelation.EEDO_EEDO_ID = eedoidnew;
                    Db.EXP_RELATION_DOC.Add(irelation);
                }
                Db.SaveChanges();

                var rel = new EXP_RELATION_DOC();
                rel.EEDO_EEDO_ID = eedoidnew;
                rel.EEDO_EEDO_ID_R = id;
                Db.EXP_RELATION_DOC.Add(rel);
                Db.SaveChanges();
                string postn = (from b in Db.EXP_POST_LINE where b.EPOL_ID == objecttemp.EPOL_EPOL_ID select b.EPOL_NAME).FirstOrDefault().ToString();

                //  string instrdoc = string.Empty;
                //if (idetby1 != 0)
                //{
                //    string instn = string.Empty;
                //    var bay = (from b in cntx.EXP_TYPE_BAY where b.ETBY_ID == idetby1 select b);
                //    instn = bay.FirstOrDefault().ETBY_DESC.ToString();
                //    instrdoc = instn;

                //}
                //if (idinstt != 0)
                //{
                //    string instn = string.Empty;
                //    var inst = (from b in cntx.EXP_INSTRUMENT where b.EINS_ID == idinstt select b);
                //    string insttype = inst.FirstOrDefault().EINS_ID.ToString();
                //    instn = inst.FirstOrDefault().EINS_DESC.ToString();
                //    instrdoc = instn + " " + instrdoc;

                //}
                //if (idinst != 0)
                //{
                //    var w = (from b in cntx.EXP_POST_LINE_INSTRU where b.EPIU_ID == idinst select b);
                //    string instn = string.Empty;
                //    instn = w.FirstOrDefault().CODE_NAME.ToString();
                //    string insttype = w.FirstOrDefault().EINS_EINS_ID.ToString();
                //    if (insttype == "1") instn = " خط " + instn;
                //    else instn = " تجهیز " + instn;
                //    instrdoc = instn + " " + instrdoc;
                //}
                string doc_name = "درخواست تجدید شده درخواست " + eedonumb;
                string smessage = " درخواست " + "به شماره" + objecttemp.DOC_NUMB + " در تاریخ " + objecttemp.EEDO_DATE + " مربوط به " + postn + " میباشد ";

                AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
                wp.StartProcess(userersal, new string[] { userersal }, doc_name, smessage, 21, eedoidnew);
                int not_id = wp.NoteId;
                Session["eedotools"] = objecttemp.EEDO_ID;
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", objecttemp.DOC_NUMB), CoustomData = objecttemp.EEDO_ID }.ToJson();
            }
            else
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات برای ثبت کامل نیست" }.ToJson();
            }
        }

        public ActionResult Add_Repdate(int? id, int? NOTID)
        {
            Session["NOTID"] = NOTID;

            Session["eedo_id"] = id;

            if (id != 0)
            {
                return View();
            }
            else
            {
                return View("ErrorNotFound");
            }
        }

        public ActionResult viewneedtools(string id, string notId)
        {
            ViewBag.Instrument = Db.EXP_INSTRUMENT.Select(c => new { c.EINS_ID, c.EINS_DESC });
            ViewBag.typeinstrument = Db.EXP_TYPE_EQUIP.Select(c => new { c.ETEX_ID, c.ETEX_DESC });
            ViewBag.good = Db.STR_GOODS.Select(c => new { c.GOOD_ID, c.GOOD_DESC });
            ViewBag.exp_doc = Db.EXP_EXPI_DOC;

            EXP_EXPI_DOC cm = new EXP_EXPI_DOC();

            cm = Db.Database.SqlQuery<EXP_EXPI_DOC>("select * from EXP_EXPI_DOC where {0}".FormatWith(id)).FirstOrDefault();

            decimal did = cm.EEDO_ID;
            string sqeedo = (from b in Db.EXP_RELATION_DOC
                             join c in Db.EXP_EXPI_DOC on b.EEDO_EEDO_ID equals c.EEDO_ID
                             where b.EEDO_EEDO_ID_R == did && c.ETDO_ETDO_ID == 2
                             select b.EEDO_EEDO_ID).FirstOrDefault().ToString();

            Session["eedo_id"] = sqeedo;
            Session["eedotools"] = did;

            return View("Add_needtools", cm);
        }

        public ActionResult Add_needtools(int? id)
        {
            ViewBag.Instrument = Db.EXP_INSTRUMENT.Select(c => new { c.EINS_ID, c.EINS_DESC });
            ViewBag.typeinstrument = Db.EXP_TYPE_EQUIP.Select(c => new { c.ETEX_ID, c.ETEX_DESC });
            ViewBag.good = Db.STR_GOODS.Select(c => new { c.GOOD_ID, c.GOOD_DESC });

            Session["eedo_id"] = id;
            Session["eedotools"] = null;

            if (id != 0)
            {
                int counti = (from b in Db.EXP_RELATION_DOC
                              join k in Db.EXP_EXPI_DOC on b.EEDO_EEDO_ID_R equals k.EEDO_ID
                              where b.EEDO_EEDO_ID == id && k.ETDO_ETDO_ID == 164
                              select k).Count();
                if (counti != 0)
                {
                    EXP_EXPI_DOC EXPNEW = (from b in Db.EXP_RELATION_DOC
                                           join k in Db.EXP_EXPI_DOC on b.EEDO_EEDO_ID_R equals k.EEDO_ID
                                           where b.EEDO_EEDO_ID == id && k.ETDO_ETDO_ID == 164
                                           select k).FirstOrDefault();
                    Session["eedotools"] = EXPNEW.EEDO_ID.ToString();

                    return View(EXPNEW);
                }
                else
                    return View();
            }
            else
            {
                return View("ErrorNotFound");
            }
        }

        public JsonResult Userneedtools()
        {
            //BandarEntities cntxb = this.DB();
            string idrole = "COFANI";
            AsrJobProvider jp = new AsrJobProvider(idrole, "FLW_TOOL");
            return this.Json(new { Success = true, data = jp.AllUsers }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Inserttools(EXP_EXPI_DOC objecttemp)
        {
            //var Db = this.DB();
            int id = Convert.ToInt16(Session["eedo_id"].ToString());
            string qetdo = (from b in Db.EXP_EXPI_DOC where b.EEDO_ID == id select b.ETDO_ETDO_ID).FirstOrDefault().ToString();

            int idd = 0;
            if (Session["eedotools"] != null)
            {
                idd = Convert.ToInt16(Session["eedotools"].ToString());
            }

            if (idd == 0)
            {
                objecttemp.ETDO_ETDO_ID = 164;
                Db.EXP_EXPI_DOC.Add(objecttemp);
                Db.SaveChanges();
                var rel = new EXP_RELATION_DOC();
                rel.EEDO_EEDO_ID = id;
                rel.EEDO_EEDO_ID_R = objecttemp.EEDO_ID;
                Db.EXP_RELATION_DOC.Add(rel);
                Db.SaveChanges();
                string typedoc = string.Empty;
                if (qetdo == "2")
                    typedoc = "دیفکت به شماره " + id;
                if (qetdo == "21")
                    typedoc = "درخواست انجام کار به شماره " + id;

                string smessage = " فرم لوازم یدکی " + "به شماره" + objecttemp.DOC_NUMB + " مربوط به فرم " + typedoc + " میباشد ";
                string doc_name = (from b in Db.EXP_TYPE_DOC where b.ETDO_ID == 164 select b.ETDO_DESC).FirstOrDefault();

                AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
                wp.StartProcess(this.HttpContext.User.Identity.Name, new string[] { this.HttpContext.User.Identity.Name }, doc_name, smessage, 164, objecttemp.EEDO_ID);
                int not_id = wp.NoteId;
                Session["eedotools"] = objecttemp.EEDO_ID;
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", objecttemp.DOC_NUMB), CoustomData = not_id.ToString() + "%" + objecttemp.EEDO_ID }.ToJson();
            }
            else
            {
                var q = (from b in Db.EXP_EXPI_DOC where b.EEDO_ID == idd select b).FirstOrDefault();
                q.EEDO_DESC = objecttemp.EEDO_DESC;
                q.EEDO_DATE = objecttemp.EEDO_DATE;
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] بروز رسانی شد.", objecttemp.DOC_NUMB) }.ToJson();
            }
        }

        public ActionResult tool_New(int? id)
        {
            //var Db = this.DB();
            //var db = this.DB();

            if (id == 0)
            {
                Session["tollinst"] = null;
                return View();
            }
            else
            {
                string q = (from b in Db.EXP_EDOC_INSTRU where b.EDIN_ID == id select b.EEDO_EEDO_ID).FirstOrDefault().ToString();
                Session["tollinst"] = id;//Convert.ToInt16(q);
                EXP_EDOC_INSTRU EXPNEW = (from b in Db.EXP_EDOC_INSTRU where b.EDIN_ID == id select b).FirstOrDefault();
                return View(EXPNEW);
            }
        }

        public ActionResult EINS_EINS_ID_R()
        {
            //var Db = this.DB();
            var query = (from b in Db.EXP_INSTRUMENT
                         orderby b.EINS_DESC
                         where b.EINS_EINS_ID != null
                         select new { b.EINS_ID, b.EINS_DESC });
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SGOD_GOOD_ID_R()
        {
            //var Db = this.DB();
            var query = (from b in Db.STR_GOODS
                         where b.GOOD_CODE_DESC != "M"
                         select new { b.GOOD_ID, b.GOOD_DESC });
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Inserttoolsgood(EXP_EDOC_INSTRU objecttemp)
        {
            //var Db = this.DB();
            int id = Convert.ToInt16(Session["eedotools"].ToString());
            int idd = 0;
            if (Session["tollinst"] != null)
            {
                idd = Convert.ToInt16(Session["tollinst"].ToString());
            }

            if (idd == 0)
            {
                objecttemp.ETDO_ETDO_ID = 164;
                objecttemp.EEDO_EEDO_ID = id;
                Db.EXP_EDOC_INSTRU.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", objecttemp.EDIN_ID) }.ToJson();
            }
            else
            {
                var q = (from b in Db.EXP_EDOC_INSTRU where b.EDIN_ID == idd select b).FirstOrDefault();
                if (objecttemp.SGOD_GOOD_ID != null)
                {
                    q.SGOD_GOOD_ID = objecttemp.SGOD_GOOD_ID;
                }
                if (objecttemp.ETEX_ETEX_ID != null)
                {
                    q.ETEX_ETEX_ID = objecttemp.ETEX_ETEX_ID;
                }
                if (objecttemp.EINS_EINS_ID != null)
                {
                    q.EINS_EINS_ID = objecttemp.EINS_EINS_ID;
                }
                q.CONT_FUN = objecttemp.CONT_FUN;
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] بروز رسانی شد.", objecttemp.EDIN_ID) }.ToJson();
            }
        }

        public ActionResult Get_needtool([DataSourceRequest] DataSourceRequest request, int? eedo_id)
        {
            var query = from p in Db.EXP_EDOC_INSTRU.OrderByDescending(b => b.EDIN_ID)
                        where (p.EEDO_EEDO_ID == eedo_id && eedo_id.HasValue)
                        select new
                        {
                            p.EDIN_ID,
                            p.EINS_EINS_ID,
                            p.SGOD_GOOD_ID,
                            p.ETEX_ETEX_ID,
                            p.CONT_FUN
                        };
            return Json(query.ToDataSourceResult(request));
        }

    }

}
using Asr.Cartable;
using Asr.Text;
using Equipment.Codes.Security;
using Equipment.DAL;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Asr.Security;
using Oracle.ManagedDataAccess.Client;

namespace Equipment.Controllers.Development
{
    [Authorize]
    //[Attributes.HandleOraNetworkException]
    public class TarhController : DbController
    {
        //BandarEntities Db;

        PersianCalendar pc = new PersianCalendar();
        DateTime thisDate = DateTime.Now;
        AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
        string orclname = string.Empty, username = string.Empty;


        ///////////////////////////////////////////////////////////////////VIEW

        public TarhController()
            : base()
        {
            //Db = this.DB();
            orclname = this.UserInfo().Username.ToUpper();
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        Db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        public ActionResult ViewFormRequestGoods(string id, decimal notId)
        {
            ViewData["ID"] = id;
            ViewData["notId"] = notId;

            return View();
        }
        public ActionResult PDRF_WORK(int id, string form)
        {
            //ViewData["cntr"] = cntx.CNT_CONTRACT_V.Select(o => new { o.TITL,o.CNTR_NO }).AsEnumerable();
            ViewData["stmt_id"] = id;
            ViewData["form"] = form;
            return View();
        }

        public ActionResult PDRF_WORK_LETTER(int id, string item_key)
        {
            //ViewData["cntr"] = cntx.CNT_CONTRACT_V.Select(o => new { o.TITL,o.CNTR_NO }).AsEnumerable();
            ViewData["let_code"] = id;
            ViewData["item_key"] = item_key;
            return View();
        }
        public ActionResult PDRF_WORK_LETTER_CURRENT(int id, string item_key)
        {
            //ViewData["cntr"] = cntx.CNT_CONTRACT_V.Select(o => new { o.TITL,o.CNTR_NO }).AsEnumerable();
            ViewData["let_code"] = id;
            //ViewData["FIN_LETT_NO"] = FIN_LETT_NO;
            ViewData["item_key"] = item_key;
            return View();
        }
        public ActionResult PRN_PREF()
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = DateTime.Now;
            return View();
        }

        public ActionResult OPER_STANDARD()
        {
            return View();
        }

        public ActionResult PRN_TRANSCRIPT(int? id)
        {
            ViewData["id"] = id.GetValueOrDefault();
            return View();
        }
        public ActionResult UpdateLetter(int? id)
        {
            ViewData["id"] = id.GetValueOrDefault();
            return View();
        }
        public ActionResult InsertTrans(int? id)
        {
            ViewData["id"] = id.GetValueOrDefault();
            return View();
        }
        public ActionResult InsertAttCurren(int? id)
        {
            ViewData["id"] = id.GetValueOrDefault();
            return View();
        }
        public ActionResult CGT_LETTER(string id)
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = DateTime.Now;
            ViewData["let_type"] = id;
            return View();
        }
        public ActionResult CONTRACTOR(int? ESIL_ID)
        {
            ViewData["ESIL_ID"] = ESIL_ID;
            return View();
        }
        public ActionResult CGT_LETTER_CURRENT(string id)
        {

            return View();
        }

        public ActionResult CGT_LETTER_EXTEN()
        {
            return View();
        }

        public ActionResult Report_Prepayment()
        {
            return View();
        }

        public ActionResult ShowStatement(string id, int notId)
        {
            Asr.Cartable.AsrWorkFlowProcess wp = new Asr.Cartable.AsrWorkFlowProcess(notId);
            int key = Convert.ToInt32(wp.GetKeyValue("STMT_ID"));
            ViewData["STMT_ID"] = key;
            //        using(BandarEntities cntx=this.DB())
            //{
            //    ViewData["STMT_ID"] = key;
            //     var model=cntx.TRH_STATEMENT.FirstOrDefault(x=>x.STMT_ID==key);
            //            return View(model);
            //}

            return View();
        }

        public ActionResult ShowLetter(string id, int notId)
        {
            Asr.Cartable.AsrWorkFlowProcess wp = new Asr.Cartable.AsrWorkFlowProcess(notId);
            int key = Convert.ToInt32(wp.GetKeyValue("LET_CODE"));
            ViewData["LET_CODE"] = key;
            //        using(BandarEntities cntx=this.DB())
            //{
            //    ViewData["STMT_ID"] = key;
            //     var model=cntx.TRH_STATEMENT.FirstOrDefault(x=>x.STMT_ID==key);
            //            return View(model);
            //}

            return View();
        }

        public ActionResult PDF_CTRL()
        {
            return View();
        }

        public ActionResult PDF_CTRL_PRJ(short? id, short? CPRO_PRJ_CODE, short? CTRL_ID)
        {
            ViewData["PLN_CODE"] = id;
            ViewData["CTRL_ID"] = CTRL_ID;
            ViewData["PRJ_CODE"] = CPRO_PRJ_CODE;
            return View();
        }

        public ActionResult PDF_CTRL_PRJ_ROW(short? id)
        {
            ViewData["CTRL_ID"] = id;
            return View();
        }

        public ActionResult PDF_CTRL_CREATE_FILE()
        {
            return View();
        }

        public ActionResult TRH_MINUTE_DELIVER(string id)
        {
            ViewData["mide_type"] = id;
            return View();
        }

        public ActionResult PDF_DRTY()
        {
            return View();
        }

        public ActionResult PRN_REFERENCE(string id)
        {
            ViewData["FIN_LETT_NO"] = id;
            return View();
        }

        public ActionResult PDF_STANDARD_ACTIVITY()
        {
            return View();
        }

        public ActionResult PDF_STANDARD_ACTIVITY_01(string id)
        {
            ViewData["st_id"] = id;
            return View();
        }

        public ActionResult AddGoods(string id, short? TRAN_TYPE, short? TRAN_YEAR, short? SSTO_STOR_CODE)
        {
            ViewData["TRAN_NO"] = id;
            ViewData["TRAN_TYPE"] = TRAN_TYPE;
            ViewData["TRAN_YEAR"] = TRAN_YEAR;
            ViewData["SSTO_STOR_CODE"] = SSTO_STOR_CODE;
            return View();
        }

        [EntityAuthorize("PDF_STANDARD_ACTIVITY > select")]
        public ActionResult PDF_DRTY_ITEM(string id)
        {
            ViewData["temp_type"] = Db.PDF_STANDARD_ACTIVITY.Select(o => new { o.ST_ID, o.STND_DESC }).AsEnumerable();
            ViewData["id"] = id;
            return View();
        }

        [EntityAuthorize("PDF_STANDARD_ACTIVITY > select")]
        public string find_rec_f(string sst_id)
        {
            string std_code = string.Empty;
            short st_id = short.Parse(sst_id);
            var query = Db.PDF_STANDARD_ACTIVITY.Find(st_id);
            if (query.ST_ID != null)
            {
                std_code = string.Format("{0}", query.STND_ST_ID);//-{1}", query.STND_ST_ID, find_rec_f(Convert.ToString(query.ST_ID)));// query.Select(xx => xx.STND_ST_ID).FirstOrDefault().ToString() + "-" + find_rec_f(query.Select(xx => xx.ST_ID).FirstOrDefault().ToString());
            }
            return std_code;
        }

        [EntityAuthorize("PDF_CTRL_PRO > select | PDF_CTRL_PRO_ROW > select,insert | PDF_DAILY_REPORT_ITEMS > select | PDF_STANDARD_ACTIVITY > select")]
        public ActionResult PDF_CTRL_ROW(short? id)
        {
            var pro_row = new PDF_CTRL_PRO_ROW();
            short? PDRT_DRPT_ROW = Db.PDF_CTRL_PRO.Where(xx => xx.CTRL_ID == id).Select(xx => xx.PDRT_DRPT_ROW).FirstOrDefault();
            var query = from b in Db.PDF_DAILY_REPORT_ITEMS where b.PDRT_DRPT_ROW == PDRT_DRPT_ROW select b;

            foreach (var item in query)
            {
                var query2 = from b in Db.PDF_CTRL_PRO_ROW
                             where b.CTRL_CTRL_ID == id && b.PDRI_DPRI_ROW == item.DPRI_ROW
                             select b;

                if (!query2.Any())
                {
                    pro_row.PDRI_DPRI_ROW = item.DPRI_ROW;
                    pro_row.CTRL_CTRL_ID = id;
                    Db.PDF_CTRL_PRO_ROW.Add(pro_row);
                    Db.SaveChanges();
                }
            }

            ViewData["temp_type"] = Db.PDF_STANDARD_ACTIVITY.Select(o => new { o.ST_ID, o.STND_DESC }).AsEnumerable();
            ViewData["id"] = id;
            return View();
        }

        public ActionResult CGT_LETTER_END()
        {
            return View();
        }

        public ActionResult WORK_ORD(string id)
        {
            ViewData["let_code"] = id;
            return View();
        }

        public ActionResult OPER(string id)
        {
            ViewData["let_code"] = id;
            return View();
        }

        public ActionResult TRH_STMR_TCDR()
        {
            return View();
        }

        public ActionResult PRN_FACTORS()
        {
            return View();
        }

        public ActionResult PRN_STMR_SUPR(string id)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                Session["stmr_id"] = id;
            }
            return View();
        }

        public ActionResult PRN_LETTER()
        {
            return View();
        }

        public ActionResult PRN_SUPR_CONTRACT(string id, string CSPR_ID)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                ViewData["CNOR_ID"] = id;
                ViewData["CSPR_ID"] = CSPR_ID;
            }
            return View();
        }

        public ActionResult PRN_WAGE(string id)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                ViewData["LETR_ID"] = id;

            }
            return View();
        }

        public ActionResult PRN_WAGE2(string id)
        {

            return View();
        }

        public ActionResult PRN_JOB()
        {
            return View();
        }

        [EntityAuthorize("CNT_CONTRACTOR > select | PRN_JOB > select")]
        public ActionResult PRN_CONTRACTOR_SUPERVISION()
        {
            ViewData["temp_type"] = Db.CNT_CONTRACTOR.Select(o => new { o.CNOR_ID, o.COMP_NAME }).AsEnumerable();
            ViewData["temp_job"] = Db.PRN_JOB.Select(o => new { o.JOB_ROW, o.JOB_DESC }).AsEnumerable();
            return View();
        }

        public ActionResult PRN_PRIVATE_FACT()
        {
            return View();
        }

        //[MenuAuthorize]
        public ActionResult PRN_INQUIRY()
        {
            return View();
        }

        public ActionResult PRN_INQUIRY_ROW(string id)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                ViewData["INQY_ID"] = id;
            }
            return View();
        }

        public ActionResult PRN_COMMENT(string id)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                ViewData["INQY_ID"] = id;
            }
            return View();
        }

        [EntityAuthorize("PRN_INQUIRY_CONTRACTORS > select | PRN_INQUIRY_ROWS > select | PRN_INQUIRY_RESPONDS > select,insert | CNT_CONTRACTOR > select")]
        public ActionResult PRN_INQUIRY_RESPOND(short id)
        {
            var respond = new PRN_INQUIRY_RESPONDS();
            var query = from b in Db.PRN_INQUIRY_CONTRACTORS where (b.INQY_ID == id) select b;
            foreach (var item in query)
            {
                var query2 = from b in Db.PRN_INQUIRY_ROWS where (b.INQY_ID == id) select b;

                foreach (var item2 in query2)
                {
                    var query3 = from b in Db.PRN_INQUIRY_RESPONDS
                                 where (b.INQY_ID == id && b.INCR_ID == item.ID && b.INRW_ID == item2.ID)
                                 select b;

                    if (!query3.Any())
                    {
                        respond.INCR_ID = item.ID;
                        respond.INQY_ID = id;
                        respond.INRW_ID = item2.ID;
                        Db.PRN_INQUIRY_RESPONDS.Add(respond);
                        Db.SaveChanges();
                    }
                }
            }

            ViewData["temp_type"] = Db.CNT_CONTRACTOR.Select(o => new { o.CNOR_ID, o.COMP_NAME }).AsEnumerable();
            if ((id != null) && (!id.Equals("0")))
            {
                ViewData["INQY_ID"] = id;
            }
            return View();
        }

        //[EntityAuthorize("CNT_CONTRACTOR > select")]
        public ActionResult PRN_INQUIRY_CONTRACTOR(string id)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                ViewData["INQY_ID"] = id;
            }
            ViewData["temp_type"] = Db.CNT_CONTRACTOR.Select(o => new { o.CNOR_ID, o.COMP_NAME }).AsEnumerable();
            return View();
        }

        public ActionResult PRN_INQUIRY_PRICE(string id, string inqy_id)
        {
            ViewData["inqy_id"] = inqy_id;
            ViewData["incr_id"] = id;
            return View();
        }

        public ActionResult CGT_LETTER_INC()
        {
            return View();
        }

        public ActionResult TRH_SEARCHWORKORDER()
        {
            return View();
        }

        public ActionResult TRH_SEARCHCNTWORKORDER(string id)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                ViewData["cntr_no"] = id;

            }
            return View();
        }

        public ActionResult PDF_TECH_DOC_ROW_SHOW(string id)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                Session["tcdc_code"] = id;
            }
            return View();
        }

        public ActionResult PDF_TECH_DOC_ROW_SHOW2(string id)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                Session["tcdc_code"] = id;
            }
            return View();
        }

        public ActionResult TRH_SEARCHPURCH()
        {
            return View();
        }

        public ActionResult TRH_SEARCHREQ()
        {
            return View();
        }

        public ActionResult TRH_SEARCHWORK()
        {
            return View();
        }

        public ActionResult PRN_CATEGORY()
        {
            return View();
        }

        [EntityAuthorize("STR_UNIT_MEASURMENT > select")]
        public ActionResult prn_select_category(string id)
        {
            ViewData["unit"] = Db.STR_UNIT_MEASURMENT.Select(o => new { o.UNIT_CODE, o.UNIT_DESC }).AsEnumerable();
            if ((id != null) && (!id.Equals("0")))
            {
                Session["temp"] = id;
            }

            return View();
        }

        [EntityAuthorize("STR_UNIT_MEASURMENT > select")]
        public ActionResult PRN_CATEGORY_ROW(string id)
        {
            ViewData["unit"] = Db.STR_UNIT_MEASURMENT.Select(o => new { o.UNIT_CODE, o.UNIT_DESC }).AsEnumerable();
            if ((id != null) && (!id.Equals("0")))
            {
                ViewData["PC_ID"] = id;
            }
            return View();
        }

        [EntityAuthorize("STR_UNIT_MEASURMENT > select")]
        public ActionResult TRH_MINUTE_DELIVER_ROW(string id)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                ViewData["MIDE_ID"] = id;
            }
            return View();
        }

        [EntityAuthorize("CHK_DOMAIN > select")]
        public ActionResult TRH_WORK_TYPE()
        {
            ViewData["temp_type"] = Db.CHK_DOMAIN.Select(o => new { o.DMAN_ID, o.DMAN_TITL, o.DMAN_DMAN_ID }).Where(o => o.DMAN_DMAN_ID == 378).AsEnumerable();
            return View();
        }

        [EntityAuthorize("TRH_WORK_TYPE > select")]
        public ActionResult TRH_CHAPTER()
        {
            ViewData["temp_type"] = Db.TRH_WORK_TYPE.Select(o => new { o.WORK_ID, o.WORK_DESC }).AsEnumerable();
            return View();
        }

        [EntityAuthorize("TRH_CHAPTER > select | STR_UNIT_MEASURMENT> select")]
        public ActionResult TRH_ITEM()
        {
            ViewData["temp_type"] = Db.TRH_CHAPTER.Select(o => new { o.CHAP_ID, o.CHAP_DESC }).AsEnumerable();
            ViewData["viewdata"] = Db.STR_UNIT_MEASURMENT.Select(o => new { o.UNIT_CODE, o.UNIT_DESC }).AsEnumerable();
            return View();
        }

        public ActionResult Search_Item()
        {
            return View();
        }

        [EntityAuthorize("TRH_CHAPTER > select | BKP_FINANCIAL_YEAR > select")]
        public ActionResult TRH_INDEX()
        {
            ViewData["temp_type"] = Db.TRH_CHAPTER.Select(o => new { o.CHAP_ID, o.CHAP_DESC }).AsEnumerable();
            ViewData["viewdata"] = Db.BKP_FINANCIAL_YEAR.Select(o => new { o.FINY_YEAR }).AsEnumerable();
            return View();
        }

        [EntityAuthorize("TRH_CHAPTER > select | BKP_FINANCIAL_YEAR > select")]
        public ActionResult TRH_PRICE()
        {
            //var query = from b in cntx.TRH_PRICE
            //            join p in cntx.PDF_TECH_DOC_ROW on b.PRLS_IT_ID equals p.PRLS_IT_ID
            //            where b.PRLS_CHPT_FINY_FINY_YEAR == p.PRLS_CHPT_FINY_FINY_YEAR
            //            && b.PRLS_CHPT_WTYP_ACTI_CODE == p.PRLS_CHPT_WTYP_ACTI_CODE
            //            && b.PRLS_CHPT_CH_NO == p.PRLS_CHPT_CH_NO
            //            && b.PRLS_IT_ID == p.PRLS_IT_ID
            //            select b
            //            ;
            //var tcdr = new PDF_TECH_DOC_ROW();
            //string qq = string.Empty;
            //foreach (var item in query)
            //{
            //    string q =string.Format("update pdf_tech_doc_row set prcl_prcl_id ={0} where PRLS_CHPT_FINY_FINY_YEAR='{1}' and PRLS_CHPT_WTYP_ACTI_CODE={2} and PRLS_CHPT_CH_NO='{3}' and PRLS_IT_ID={4}",item.PRCL_ID,item.PRLS_CHPT_FINY_FINY_YEAR, item.PRLS_CHPT_WTYP_ACTI_CODE,item.PRLS_CHPT_CH_NO,item.PRLS_IT_ID);
            //    qq = q + ";" + qq;
            //}
            //cntx.SaveChanges();

            ViewData["viewdata1"] = Db.TRH_ITEM.Select(o => new { o.ITEM_ID, o.ITEM_DESC }).AsEnumerable();
            ViewData["viewdata"] = Db.BKP_FINANCIAL_YEAR.Select(o => new { o.FINY_YEAR }).AsEnumerable();
            return View();
        }

        [EntityAuthorize("TRH_ITEM > select | BKP_FINANCIAL_YEAR> select")]
        public ActionResult TRH_PRICE_Insert(string id)
        {
            ViewData["viewdata1"] = Db.TRH_ITEM.Select(o => new { o.ITEM_ID, o.ITEM_DESC }).AsEnumerable();
            ViewData["viewdata"] = Db.BKP_FINANCIAL_YEAR.Select(o => new { o.FINY_YEAR }).AsEnumerable();
            ViewData["id"] = id;
            return View();
        }

        public ActionResult PDF_TECH_DOC_UPDATE(string id)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                Session["tcdc_code"] = id;
            }

            return View();
        }

        public ActionResult PDF_TECH_DOC_ROW(string id)
        {
            if ((id != null) && (!id.Equals("0")))
            {
                Session["tcdc_code"] = id;
            }

            return View();
        }

        [EntityAuthorize("TRH_ITEM > select | BKP_FINANCIAL_YEAR > select")]
        public ActionResult TRH_STATEMENT()
        {
            ViewBag.contract = new SelectList(Db.CNT_CONTRACT, "CNTR_NO", "TITL");
            ViewData["viewdata1"] = Db.TRH_ITEM.Select(o => new { o.ITEM_ID, o.ITEM_DESC }).AsEnumerable();
            ViewData["viewdata"] = Db.BKP_FINANCIAL_YEAR.Select(o => new { o.FINY_YEAR }).AsEnumerable();
            return View();
        }

        public ActionResult PDF_TECH_DOC(int? ProjId, int? PlanId)
        {
            ViewData["ProjId"] = ProjId;
            ViewData["PlanId"] = PlanId;
            return View();
        }
        public ActionResult RequestGoods()
        {
            return View();
        }
        public ActionResult RequestGoodsWorkOrder(int? id)
        {
            ViewData["FORM_TYPE"] = id;
            return View();
        }

        [EntityAuthorize("TRH_PRICE > select")]
        public ActionResult TRH_STATEMENT_ROW(int id)
        {
            //ViewData["work"] = from b in cntx.TRH_STMR_V
            //                   orderby b.STMR_ID 
            //            where b.STMT_STMT_ID == id
            //            select new
            //            {
            //                b.WORK_DESC,
            //                b.STMR_ID,
            //                b.STMT_STMT_ID,
            //                b.STMR_AMNT,
            //                b.CHAP_DESC,
            //                b.ITEM_DESC,
            //                b.TCDR_AMNT,
            //                b.TCDR_PRICE,
            //                b.TCDR_ROW,
            //                b.ITEM_CODE
            //            }
            //            ;

            // ViewData["work"] = query;
            //ViewData["work"] = from w in cntx.TRH_WORK_TYPE
            //                   join c in cntx.TRH_CHAPTER on w.WORK_ID equals c.WORK_WORK_ID
            //                   join i in cntx.TRH_ITEM on c.CHAP_ID equals i.CHAP_CHAP_ID
            //                   join tdr in cntx.PDF_TECH_DOC_ROW on i.ITEM_ID equals tdr.ITEM_ITEM_ID
            //                   join str in cntx.TRH_STATEMENT_ROW on tdr.TCDR_ROW equals str.TCDR_TCDR_ROW
            //                   where
            //                   ((c.WORK_WORK_ID == w.WORK_ID)
            //                   && (i.CHAP_CHAP_ID == c.CHAP_ID)
            //                   && (tdr.ITEM_ITEM_ID == i.ITEM_ID)
            //                   && (tdr.TCDR_ROW == str.TCDR_TCDR_ROW)
            //                   && (str.TCDR_TCDC_TCDC_CODE == tdr.TCDC_TCDC_CODE)
            //                   && (str.STMT_STMT_ID == id))
            //                   select new
            //                   {
            //                       w.WORK_DESC,
            //                       str.STMR_ID,
            //                       str.STMT_STMT_ID,
            //                       str.STMR_AMNT,
            //                       c.CHAP_DESC,
            //                       i.ITEM_DESC,
            //                       tdr.TCDR_AMNT,
            //                       tdr.TCDR_PRICE,
            //                       tdr.TCDR_ROW,
            //                       i.ITEM_CODE
            //                   };

            ViewData["viewdata1"] = Db.TRH_PRICE.Select(o => new { o.PRCL_ID, o.PRICE }).AsEnumerable();
            if ((id != null) && (!id.Equals("0")))
            {
                Session["stmt_id"] = id.ToString();
            }
            return View();
        }
        ////////////////////////////////////////////////////////////////////INSERT

        [EntityAuthorize("TRH_WORK_TYPE > insert")]
        public ActionResult insert_work(TRH_WORK_TYPE objecttemp)
        {
            // if (PublicRepository.check_col_u("TRH_WORK_TYPE", "WORK_DESC", objecttemp.WORK_DESC))
            if (PublicRepository.ExistModel("TRH_WORK_TYPE", "DEL_DUMP_U(WORK_DESC) = DEL_DUMP_U('{0}') or DEL_DUMP_U(WORK_CODE) = DEL_DUMP_U('{1}')", objecttemp.WORK_DESC, objecttemp.WORK_CODE))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
            }
            else
            {
                objecttemp.WORK_TYPE = Request.Form["DMAN_DMAN_ID"];
                objecttemp.WORK_STAT = "1";
                Db.TRH_WORK_TYPE.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}]  ثبت شد ", objecttemp.WORK_DESC) }.ToJson();
                //return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
            }
        }

        public ActionResult insert_ctrl_row(PDF_CTRL_PRO_ROW objecttemp)
        {
            int count = 0;
            count = int.Parse(Request.Form["count"]);
            List<string> errors = new List<string>();
            for (int i = 0; i <= count; i++)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Request.Form[i.ToString()]))
                    {
                        objecttemp.CTRR_AMNT = (decimal?)decimal.Parse(Request.Form[i.ToString()]);
                        string sql = string.Format("update PDF_CTRL_PRO_ROW set CTRR_AMNT={0} where CTRR_ROW={1}", objecttemp.CTRR_AMNT, Request.Form["CTRR_ROW" + i.ToString()]);
                        if (objecttemp.CTRR_AMNT > 100)
                        { throw new Exception("error"); }
                        Db.Database.ExecuteSqlCommand(sql);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "error")
                    {
                        errors.Add(" مقدار وارد شده در ردیف " + i + " بیشتر از 100% می باشد ");
                    }
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


        [EntityAuthorize("PRN_JOB > insert")]
        public ActionResult insert_job(PRN_JOB objecttemp)
        {
            // if (PublicRepository.check_col_u("TRH_WORK_TYPE", "WORK_DESC", objecttemp.WORK_DESC))
            if (PublicRepository.ExistModel("PRN_JOB", "DEL_DUMP_U(JOB_DESC) = DEL_DUMP_U('{0}') ", objecttemp.JOB_DESC))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
            }
            else
            {
                Db.PRN_JOB.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}]  ثبت شد ", objecttemp.JOB_DESC) }.ToJson();
                //return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
            }
        }

        public ActionResult Insert_Work_ORD(FND_WORK_RQ ObjectTemp)
        {
            try
            {
                var val = Request.Form["WQ_SEQ"].Split('^');
                int wr_seqn = Convert.ToInt32(val[0]);
                string w_year = val[1].ToString();
                string Sql = string.Format("insert into CNT_WORK_CONTRACT (CCNT_CNTR_NO,TWRQ_YEAR,TWRQ_WQ_SEQ) values('{0}',{1},{2})",
                 Request.Form["CCNT_CNTR_NO"],
                 w_year, wr_seqn);
                Db.Database.ExecuteSqlCommand(Sql);
                return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات با موفقیت ثبت شد" }.ToJson();
            }
            catch (Exception Ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات" + Ex.ToString() }.ToJson();

            }
        }
        public ActionResult Insert_Pur_ORD(CNT_PUR_CONTRACT ObjectTemp)
        {
            try
            {
                var val = Request.Form["NO"].Split('^');
                short no = short.Parse(val[0]);
                string year = val[1].ToString();
                ObjectTemp.SPUR_PURE_NO = no;
                ObjectTemp.SPUR_PURE_YEAR = year;
                Db.CNT_PUR_CONTRACT.Add(ObjectTemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات با موفقیت ثبت شد" }.ToJson();
            }
            catch (Exception Ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات" + Ex.ToString() }.ToJson();

            }
        }

        [EntityAuthorize("PRN_PRIVATE_FACTS > insert")]
        public ActionResult insert_private_fact(PRN_PRIVATE_FACTS objecttemp)
        {
            // if (PublicRepository.check_col_u("TRH_WORK_TYPE", "WORK_DESC", objecttemp.WORK_DESC))
            if (PublicRepository.ExistModel("PRN_PRIVATE_FACTS", "DEL_DUMP_U(PRVT_DESC) = DEL_DUMP_U('{0}') ", objecttemp.PRVT_DESC))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
            }
            else
            {
                Db.PRN_PRIVATE_FACTS.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}]  ثبت شد ", objecttemp.PRVT_DESC) }.ToJson();
                //return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
            }
        }

        [EntityAuthorize("TRH_STATEMENT > select | TRH_STATEMENT_ROW > select | PRN_PDRF_WORK > select,insert | PRN_PDRF_OPER > select,insert")]
        public ActionResult insert_prn_work(PRN_PDRF_WORK objecttemp)
        {
            int wr_seqn = 0;
            string w_year = "";
            if (!string.IsNullOrEmpty(Request.Form["work_ord"]))
            {
                var val = Request.Form["work_ord"].Split('-');
                wr_seqn = Convert.ToInt32(val[0]);
                w_year = val[1].ToString();
            }
            objecttemp.TWRO_WR_SEQN = wr_seqn;
            objecttemp.TWRO_W_YEAR = w_year;
            objecttemp.STMT_STMT_ID = Convert.ToInt32(Request.Form["STMT_ID"]);
            objecttemp.STST_CCNT_CNTR_NO = Request.Form["CNTR_NO"];
            objecttemp.STST_STST_TYPE = Db.TRH_STATEMENT.Where(oo => oo.STMT_ID == objecttemp.STMT_STMT_ID).Select(oo => oo.STMT_TYPE).FirstOrDefault();
            objecttemp.ID = Db.PRN_PDRF_WORK.Select(oo => oo.ID).Max() + 1;
            Db.PRN_PDRF_WORK.Add(objecttemp);

            decimal? sum_price = Db.Database.SqlQuery<decimal>(string.Format("SELECT prn_current_stmr_u('{0}') FROM DUAL", objecttemp.STMT_STMT_ID)).FirstOrDefault();

            decimal amnt = Convert.ToDecimal((from b in Db.PRN_PDRF_WORK
                                              join o in Db.PRN_PDRF_OPER on b.ID equals o.PDRW_ID
                                              where (b.STST_CCNT_CNTR_NO == objecttemp.STST_CCNT_CNTR_NO && b.STMT_STMT_ID == objecttemp.STMT_STMT_ID && o.PDRW_ID == b.ID)
                                              orderby o.ID
                                              select o.AMNT).Sum());
            if (amnt == null) amnt = 0;

            if (decimal.Parse(Request.Form["amnt"]) <= sum_price)
            {
                if (decimal.Parse(Request.Form["amnt"]) + amnt <= sum_price)
                {
                    Db.SaveChanges();

                    PRN_PDRF_OPER objecttemp2 = new PRN_PDRF_OPER();
                    objecttemp2.PDRW_ID = objecttemp.ID;
                    objecttemp2.AMNT = long.Parse(Request.Form["amnt"]);
                    objecttemp2.TWRO_WR_SEQN = wr_seqn;
                    objecttemp2.TWRO_W_YEAR = w_year;
                    objecttemp2.SUBU_TYPE = objecttemp.SUBU_TYPE;
                    objecttemp2.COPE_OPRN_CODE = Convert.ToInt16(Request.Form["COPE_OPRN_CODE"]);
                    string subu = Db.PRN_PDRF_OPER.Where(xx => xx.TWRO_W_YEAR == w_year)
                        .Where(xx => xx.TWRO_WR_SEQN == wr_seqn)
                        .Where(xx => xx.COPE_OPRN_CODE == objecttemp2.COPE_OPRN_CODE)
                        .Select(xx => xx.SUBU_TYPE).FirstOrDefault();
                    //if (subu != objecttemp2.SUBU_TYPE)
                    //{
                    //    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "محل تامین منابع انتخاب شده با محل تامین منابع ثبت شده قبلی یکسان نمی باشد " }.ToJson();

                    //}

                    Db.PRN_PDRF_OPER.Add(objecttemp2);
                    if (!string.IsNullOrEmpty(Request.Form["ADVN_AMNT"]))
                    {
                        objecttemp2.ADVN_AMNT = long.Parse(Request.Form["ADVN_AMNT"]);
                    }
                    Db.SaveChanges();
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
                }
            }

            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مبلغ ثبت شده بیشتر از مبلغ کارکرد جاری میباشد " }.ToJson();
        }

        public ActionResult insert_CONT_LETT(PLN_CONT_LETT objecttemp)
        {
            try
            {
                Db.PLN_CONT_LETT.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            catch (Exception Ex)
            {

                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = Ex.ToString() }.ToJson();
            }
        }

        [EntityAuthorize("PRN_PDRF_WORK > insert")]
        public ActionResult insert_letter_work(PRN_PDRF_WORK objecttemp)
        {
            int? wr_seqn = null;
            string w_year = "", MAIN_CODE = null, ASS_CODE = null, EXPF_CODE = null;
            if (!string.IsNullOrEmpty(Request.Form["work_ord"]))
            {
                var val = Request.Form["work_ord"].Split('-');
                wr_seqn = Convert.ToInt32(val[0]);
                w_year = val[1].ToString();
                objecttemp.TWRO_WR_SEQN = wr_seqn;
                objecttemp.TWRO_W_YEAR = w_year;
            }


            long remain_amnt = Db.Database.SqlQuery<long>(string.Format("SELECT PRN_REMAIN_AMNT_U('{0}') FROM DUAL", Request.Form["CNTR_NO"])).FirstOrDefault();

            objecttemp.CLET_LET_CODE = Convert.ToInt32(Request.Form["let_code"]);
            objecttemp.ID = Db.PRN_PDRF_WORK.Select(oo => oo.ID).Max() + 1;
            decimal amnt = Convert.ToDecimal((from b in Db.PRN_PDRF_WORK
                                              join o in Db.PRN_PDRF_OPER on b.ID equals o.PDRW_ID
                                              where (b.CLET_LET_CODE == objecttemp.CLET_LET_CODE && o.PDRW_ID == b.ID)
                                              orderby o.ID
                                              select o.AMNT).Sum());
            if (amnt == null) amnt = 0;

            string let_type = Db.CGT_LETTER.Where(xx => xx.LET_CODE == objecttemp.CLET_LET_CODE).Select(xx => xx.LET_TYPE).FirstOrDefault();

            //ثبت اطلاعات گواهی پرداخت جاری
            if (!string.IsNullOrEmpty(Request.Form["AscrID"]))
            {
                var val = Request.Form["AscrID"].Split('^');
                ASS_CODE = val[0];
                MAIN_CODE = val[1];
                EXPF_CODE = val[2];

                try
                {
                    PRN_PDRF_OPER objecttemp2 = new PRN_PDRF_OPER();

                    objecttemp2.AMNT = long.Parse(Request.Form["amnt"]);
                    objecttemp2.TWRO_WR_SEQN = wr_seqn;
                    objecttemp2.TWRO_W_YEAR = w_year;
                    objecttemp2.ASST_ASS_CODE = ASS_CODE;
                    objecttemp2.ASST_ASS_CODE_R = ASS_CODE;
                    objecttemp2.ASST_MAIN_MAIN_CODE = MAIN_CODE;
                    objecttemp2.ASST_MAIN_MAIN_CODE_R = MAIN_CODE;
                    objecttemp2.BEXF_EXPF_CODE = EXPF_CODE;
                    objecttemp2.BEXF_EXPF_CODE_R = EXPF_CODE;
                    objecttemp2.GEOL_G_CODE = Request.Form["G_CODE"];
                    objecttemp2.CLET_LET_CODE = objecttemp.CLET_LET_CODE;
                    objecttemp2.COPE_OPRN_CODE = 63;
                    Db.PRN_PDRF_OPER.Add(objecttemp2);
                    Db.SaveChanges();
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();


                }
                catch (Exception Ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { Message = Ex.ToString() }.ToJson();
                }

            }

            //ثبت اطلاعات گواهی پرداخت جاری


            if (let_type != "19" && let_type != "12" && orclname != "JAMALZADEH")
            {
                if (decimal.Parse(Request.Form["amnt"]) <= remain_amnt)
                {
                    if (decimal.Parse(Request.Form["amnt"]) + amnt <= remain_amnt)
                    {
                        Db.PRN_PDRF_WORK.Add(objecttemp);
                        Db.SaveChanges();
                        PRN_PDRF_OPER objecttemp2 = new PRN_PDRF_OPER();
                        objecttemp2.CLET_LET_CODE = objecttemp.CLET_LET_CODE;
                        objecttemp2.PDRW_ID = objecttemp.ID;
                        objecttemp2.AMNT = long.Parse(Request.Form["amnt"]);
                        objecttemp2.TWRO_WR_SEQN = wr_seqn;
                        objecttemp2.TWRO_W_YEAR = w_year;
                        objecttemp2.SUBU_TYPE = objecttemp.SUBU_TYPE;
                        objecttemp2.COPE_OPRN_CODE = Convert.ToInt16(Request.Form["COPE_OPRN_CODE"]);
                        if (!string.IsNullOrEmpty(Request.Form["ADVN_AMNT"]))
                        {
                            objecttemp2.ADVN_AMNT = long.Parse(Request.Form["ADVN_AMNT"]);
                        }
                        string subu = Db.PRN_PDRF_OPER.Where(xx => xx.TWRO_W_YEAR == w_year)
                                                      .Where(xx => xx.TWRO_WR_SEQN == wr_seqn)
                                                      .Where(xx => xx.COPE_OPRN_CODE == objecttemp2.COPE_OPRN_CODE && xx.CLET_LET_CODE == objecttemp.CLET_LET_CODE)
                                                      .Select(xx => xx.SUBU_TYPE).FirstOrDefault();

                        if (subu != objecttemp2.SUBU_TYPE && subu != null)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "محل تامین منابع انتخاب شده با محل تامین منابع ثبت شده قبلی یکسان نمی باشد " }.ToJson();
                        }

                        Db.PRN_PDRF_OPER.Add(objecttemp2);
                        Db.SaveChanges();
                        return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
                    }
                }
            }
            else
            {
                Db.PRN_PDRF_WORK.Add(objecttemp);
                Db.SaveChanges();
                PRN_PDRF_OPER objecttemp2 = new PRN_PDRF_OPER();
                objecttemp2.PDRW_ID = objecttemp.ID;
                objecttemp2.AMNT = long.Parse(Request.Form["amnt"]);
                objecttemp2.TWRO_WR_SEQN = wr_seqn;
                objecttemp2.TWRO_W_YEAR = w_year;
                objecttemp2.SUBU_TYPE = objecttemp.SUBU_TYPE;
                objecttemp2.COPE_OPRN_CODE = Convert.ToInt16(Request.Form["COPE_OPRN_CODE"]);
                Db.PRN_PDRF_OPER.Add(objecttemp2);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }

            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مبلغ ثبت شده بیشتر از مانده قرارداد میباشد " }.ToJson();
        }

        [EntityAuthorize("PDF_CTRL_PRO > insert")]
        public ActionResult insert_ctrl(PDF_CTRL_PRO objecttemp)
        {
            Int16 max_id = 0;
            bool check = false;
            string msg = "در ثبت اطلاعات خطا بوجود آمده است";
            //var q = from p in cntx.PDF_CTRL_PRO_ROW where p.CTRL_CTRL_ID == objecttemp.CTRL_CTRL_ID  select p;

            var q1 = from p in Db.PDF_CTRL_PRO
                     orderby p.CTRL_ID descending
                     where p.CTRL_CTRL_ID == objecttemp.CTRL_CTRL_ID && p.PDRT_DRPT_ROW == objecttemp.PDRT_DRPT_ROW
                     select p;

            if (q1.Any())
            {
                max_id = Db.Database.SqlQuery<Int16>(string.Format("SELECT nvl( max (ctrl_id),0) from pdf_ctrl_pro where ctrl_ctrl_id='{0}' and PDRT_DRPT_ROW='{1}'", objecttemp.CTRL_CTRL_ID, objecttemp.PDRT_DRPT_ROW)).FirstOrDefault();
                objecttemp.CPRO_CPLA_PLN_CODE = q1.Select(xx => xx.CPRO_CPLA_PLN_CODE).FirstOrDefault();
                objecttemp.CPRO_PRJ_CODE = q1.Select(xx => xx.CPRO_PRJ_CODE).FirstOrDefault();
                objecttemp.CTRL_CODE = q1.Select(xx => xx.CTRL_CODE).FirstOrDefault();
                objecttemp.CTRL_DESC = Db.PDF_CTRL_PRO.Where(xx => xx.CTRL_ID == objecttemp.CTRL_CTRL_ID).Select(xx => xx.CTRL_DESC).FirstOrDefault() + "-" + objecttemp.CTRL_DATE;
                objecttemp.CTRL_LEVL = q1.Select(xx => xx.CTRL_LEVL).FirstOrDefault();
                objecttemp.CTRL_TYPE = q1.Select(xx => xx.CTRL_TYPE).FirstOrDefault();
                Db.PDF_CTRL_PRO.Add(objecttemp);
                Db.SaveChanges();
                check = true;

                var pro_row = new PDF_CTRL_PRO_ROW();

                if (max_id == 0)
                {
                    //record in pdf_ctrl_pro not found
                    var query = from b in Db.PDF_DAILY_REPORT_ITEMS

                                where b.PDRT_DRPT_ROW == objecttemp.PDRT_DRPT_ROW
                                select b;

                    foreach (var item in query)
                    {
                        pro_row.PDRI_DPRI_ROW = item.DPRI_ROW;
                        pro_row.CTRL_CTRL_ID = objecttemp.CTRL_ID;
                        pro_row.CTRR_AMNT = 0;
                        Db.PDF_CTRL_PRO_ROW.Add(pro_row);
                        Db.SaveChanges();
                        check = true;
                    }
                }
                else
                {
                    //record in pdf_ctrl_pro  found
                    var query = from b in Db.PDF_CTRL_PRO_ROW where b.CTRL_CTRL_ID == max_id select b;

                    foreach (var item in query)
                    {
                        pro_row.PDRI_DPRI_ROW = item.PDRI_DPRI_ROW;
                        pro_row.CTRL_CTRL_ID = objecttemp.CTRL_ID;
                        pro_row.CTRR_AMNT = item.CTRR_AMNT;
                        Db.PDF_CTRL_PRO_ROW.Add(pro_row);
                        Db.SaveChanges();
                        check = true;
                    }
                }
            }
            else
            {
                objecttemp.CPRO_CPLA_PLN_CODE = Db.PDF_CTRL_PRO.Where(xx => xx.CTRL_ID == objecttemp.CTRL_CTRL_ID).Select(xx => xx.CPRO_CPLA_PLN_CODE).FirstOrDefault();
                objecttemp.CPRO_PRJ_CODE = Db.PDF_CTRL_PRO.Where(xx => xx.CTRL_ID == objecttemp.CTRL_CTRL_ID).Select(xx => xx.CPRO_PRJ_CODE).FirstOrDefault();
                objecttemp.CTRL_LEVL = Db.PDF_CTRL_PRO.Where(xx => xx.CTRL_ID == objecttemp.CTRL_CTRL_ID).Select(xx => xx.CTRL_LEVL).FirstOrDefault();
                objecttemp.CTRL_DESC = Db.PDF_CTRL_PRO.Where(xx => xx.CTRL_ID == objecttemp.CTRL_CTRL_ID).Select(xx => xx.CTRL_DESC).FirstOrDefault() + "-" + objecttemp.CTRL_DATE;
                objecttemp.CTRL_CODE = Db.PDF_CTRL_PRO.Where(xx => xx.CTRL_ID == objecttemp.CTRL_CTRL_ID).Select(xx => xx.CTRL_CODE).FirstOrDefault() + "-" + objecttemp.CTRL_DATE;
                Db.PDF_CTRL_PRO.Add(objecttemp);
                Db.SaveChanges();
                check = true;

                var pro_row = new PDF_CTRL_PRO_ROW();
                var query = from b in Db.PDF_DAILY_REPORT_ITEMS where b.PDRT_DRPT_ROW == objecttemp.PDRT_DRPT_ROW select b;

                foreach (var item in query)
                {
                    pro_row.PDRI_DPRI_ROW = item.DPRI_ROW;
                    pro_row.CTRL_CTRL_ID = objecttemp.CTRL_ID;
                    pro_row.CTRR_AMNT = 0;
                    Db.PDF_CTRL_PRO_ROW.Add(pro_row);
                    Db.SaveChanges();
                    check = true;
                }
            }

            if (check == true)
            {
                return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات با موفقیت ثبت شد" }.ToJson();
            }
            else
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = msg }.ToJson();
            }
        }

        public ActionResult get_financial_year()
        {
            var RetVal = from b in Db.BKP_FINANCIAL_YEAR
                         orderby b.FINY_YEAR descending
                         select new { b.FINY_YEAR };
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("PDF_CTRL_PRO > insert")]
        public ActionResult insert_ctrl_file(PDF_CTRL_PRO objecttemp)
        {
            decimal? level = objecttemp.CTRL_LEVL;
            string PLAN_PROJ_INPUT = Request.Form["PLAN_PROJ_INPUT"];
            var val = PLAN_PROJ_INPUT.Split('^');
            objecttemp.CPRO_CPLA_PLN_CODE = short.Parse(val[0]);
            objecttemp.CPRO_PRJ_CODE = short.Parse(val[1]);
            //bool check = false;
            //انتخاب نوع گزارش هایی که نوع پروژه آنها با نوع پروژه این پروژه یکسان هستند
            var q = from b in Db.PDF_DAILY_REPORT_TYPES
                    join p in Db.CGT_PRO on b.CKPR_KPRJ_ROW equals p.CKPR_KPRJ_ROW
                    where (p.PRJ_CODE == objecttemp.CPRO_PRJ_CODE && p.CPLA_PLN_CODE == objecttemp.CPRO_CPLA_PLN_CODE)
                    select b;

            //انتخاب سطوح استاندار که با این نوع پروژه و سطح اعمال انتخابی یکسان هستند

            var q1 = from b in Db.PDF_STANDARD_ACTIVITY
                     join p in Db.CGT_PRO on b.CKPR_KPRJ_ROW equals p.CKPR_KPRJ_ROW
                     where (p.PRJ_CODE == objecttemp.CPRO_PRJ_CODE && p.CPLA_PLN_CODE == objecttemp.CPRO_CPLA_PLN_CODE && b.STND_LEVEL == level)
                     select b;

            var kpro_desc = Db.CGT_KPRO.Where(xx => xx.KPRJ_ROW == q.Select(yy => yy.CKPR_KPRJ_ROW).FirstOrDefault()).Select(xx => xx.KPRJ_DESC).FirstOrDefault();
            var sum = q1.Select(xx => xx.WGHT).Sum();

            objecttemp.CTRL_DESC = Db.CGT_PRO.Where(xx => xx.PRJ_CODE == objecttemp.CPRO_PRJ_CODE).Select(xx => xx.PRJ_DESC).FirstOrDefault() + "-" + objecttemp.CTRL_DESC;
            objecttemp.CTRL_CODE = objecttemp.CPRO_CPLA_PLN_CODE + "-" + objecttemp.CPRO_PRJ_CODE;
            Db.PDF_CTRL_PRO.Add(objecttemp);
            Db.SaveChanges();
            return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات با موفقیت ثبت شد" }.ToJson();

            //اگر مجموع وزن های آن سطح به 100 رسید امکان ثبت باشد

            //if (sum == 100)
            // {
            //ثبت گزارش روزانه
            //if (objecttemp.CTRL_TYPE == "1")
            //{
            //    foreach (var item in q)
            //    {
            //        sum2 = q.Where(xx => xx.DRPT_ROW == item.DRPT_ROW)
            //            .Select(xx => xx.PDF_DAILY_REPORT_ITEMS.Select(yy => yy.WGHT).Sum()).Sum();
            //        var q3 = from b in cntx.PDF_CTRL_PRO 
            //                 where b.PDRT_DRPT_ROW == item.DRPT_ROW && b.CTRL_TYPE == "1" select b;
            //        //if (sum2 == 100 && !q3.Any())
            //        if (!q3.Any())

            //        {
            //            objecttemp.CTRL_DESC = cntx.PDF_DAILY_REPORT_TYPES
            //            .Where(xx => xx.DRPT_ROW == item.DRPT_ROW)
            //            .Select(xx => xx.DRPT_DESC).FirstOrDefault();
            //            objecttemp.CTRL_CODE = objecttemp.CPRO_CPLA_PLN_CODE + "-"
            //                + objecttemp.CPRO_PRJ_CODE + "-"
            //                + item.DRPT_ROW + "-"
            //                + level + "-" +
            //                objecttemp.CTRL_TYPE;

            //            objecttemp.PDRT_DRPT_ROW = item.DRPT_ROW;
            //            cntx.PDF_CTRL_PRO.Add(objecttemp);
            //            cntx.SaveChanges();
            //            var pro_row = new PDF_CTRL_PRO_ROW();

            //            var query = from b in cntx.PDF_DAILY_REPORT_ITEMS
            //                        where b.PDRT_DRPT_ROW == objecttemp.PDRT_DRPT_ROW
            //                        select b;

            //            foreach (var row_item in query)
            //            {
            //                pro_row.PDRI_DPRI_ROW = row_item.DPRI_ROW;
            //                pro_row.CTRL_CTRL_ID = objecttemp.CTRL_ID;
            //                pro_row.CTRR_AMNT = 0;
            //                cntx.PDF_CTRL_PRO_ROW.Add(pro_row);
            //                cntx.SaveChanges();
            //                check = true;
            //            }
            // check = true;
            // return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات با موفقیت ثبت شد" }.ToJson();
            //  }
            //}

            // if (check == true)
            //  {
            //  }

            // }
            //else if (sum2 != 100 && objecttemp.CTRL_TYPE == "1")
            //{
            //    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "وزن آیتم های گزارش به 100 نرسیده است" }.ToJson();
            //}
            //if (objecttemp.CTRL_TYPE == "0")
            //{
            //    foreach (var item in q)
            //    {
            //        var q3 = from b in cntx.PDF_CTRL_PRO where b.PDRT_DRPT_ROW == item.DRPT_ROW && b.CTRL_TYPE == "0" select b;
            //        if (!q3.Any())
            //        {
            //            //var query = from r in cntx.PDF_DAILY_REPORT_TYPES
            //            //join p in cntx.PDF_CTRL_PRO on r.DRPT_ROW equals p.PDRT_DRPT_ROW
            //            //where (p.PDRT_DRPT_ROW == r.DRPT_ROW && p.CTRL_TYPE=="0" && r.CKPR_KPRJ_ROW == q.Select(xx=>xx.CKPR_KPRJ_ROW).FirstOrDefault())
            //            //orderby p.CTRL_ID
            //            //select p;
            //            //  foreach (var item2 in query)
            //            //{
            //            //   var count = cntx.PDF_CTRL_PRO_ROW.Where(xx => xx.CTRL_CTRL_ID == item2.CTRL_ID).Count();
            //            //   if (count == 0)
            //            //{
            //            //string sql = string.Format(" insert into pdf_ctrl_pro_row (CTRR_AMNT,CTRL_CTRL_ID)  select 0,{0}    from pdf_standard_activity     where CKPR_KPRJ_ROW = {1}      and STND_LEVEL = {2}", item2.CTRL_ID,q.Select(xx=>xx.CKPR_KPRJ_ROW).FirstOrDefault() , level);
            //            //cntx.Database.ExecuteSqlCommand(sql);
            //            //cntx.SaveChanges();
            //            objecttemp.CTRL_DESC = cntx.PDF_DAILY_REPORT_TYPES
            //            .Where(xx => xx.DRPT_ROW == item.DRPT_ROW)
            //            .Select(xx => xx.DRPT_DESC).FirstOrDefault();
            //            objecttemp.CTRL_CODE = objecttemp.CPRO_CPLA_PLN_CODE + "-"
            //                + objecttemp.CPRO_PRJ_CODE + "-"
            //                + item.DRPT_ROW + "-"
            //                + level + "-" +
            //                objecttemp.CTRL_TYPE;
            //            objecttemp.PDRT_DRPT_ROW = item.DRPT_ROW;
            //            cntx.PDF_CTRL_PRO.Add(objecttemp);
            //            cntx.SaveChanges();
            //            string sql = string.Format("begin PDF_CREATE_MSP_P('{0}','{1}','{2}','{3}','{4}','{5}'  ); end; ",
            //            item.PDF_CTRL_PRO.Select(xx => xx.CPRO_CPLA_PLN_CODE).FirstOrDefault(),
            //            item.PDF_CTRL_PRO.Select(xx => xx.CPRO_PRJ_CODE).FirstOrDefault(),
            //             objecttemp.CTRL_ID.ToString() + "Project.mpp",
            //            objecttemp.CTRL_ID,
            //            q.Select(xx => xx.CKPR_KPRJ_ROW).FirstOrDefault(),
            //            level);
            //            cntx.Database.ExecuteSqlCommand(sql);
            //            // cntx.Database.SqlQuery<int>(sql);
            //            // var DBdata = cntx.Database.SqlQuery<>(sql).ToList<Functions>();
            //            // var query = cntx.Database.SqlQuery<string>(sql);
            //            //    var result = query.ToList();

            //            //using (Oracle.ManagedDataAccess.Client.OracleConnection cmn = new Oracle.ManagedDataAccess.Client.OracleConnection(GlobalConst.ConnectionString))
            //            //{

            //            //    using (var cmd = cmn.CreateCommand())
            //            //    {
            //            //        //    cmd.CommandType = CommandType.StoredProcedure;
            //            //        //    cmd.CommandText = "PDF_CREATE_MSP_P";
            //            //        //   cmd.Parameters.Add("P_PLAN_CODE", OracleDbType.Int64).Value = 292;   //item.PDF_CTRL_PRO.Select(xx => xx.CPRO_CPLA_PLN_CODE).FirstOrDefault();
            //            //        //   cmd.Parameters.Add("P_PROJ_CODE", OracleDbType.Int32).Value = 1;    //item.PDF_CTRL_PRO.Select(xx => xx.CPRO_PRJ_CODE).FirstOrDefault();
            //            //        //    cmd.Parameters.Add(":P_PROJ_NAME", OracleDbType.Varchar2).Value =prjanme;  //item.PDF_CTRL_PRO.Select(xx => xx.PROJ_DESC).FirstOrDefault();
            //            //        //   cmd.Parameters.Add("P_CTRL_ID", OracleDbType.Int32).Value = 150;       //objecttemp.CTRL_ID;
            //            //        //   cmd.Parameters.Add("P_KPRJ_ROW", OracleDbType.Int32).Value = 25;     //q.Select(xx => xx.CKPR_KPRJ_ROW).FirstOrDefault();
            //            //        //   cmd.Parameters.Add("P_STND_LEVEL", OracleDbType.Int32).Value = 1;   //level;
            //            //        //  
            //            //        cmn.Open();
            //            //        cmd.ExecuteNonQuery();
            //            //    }
            //            //}
            //            //    PDF_CREATE_MSP_U(:CPRO_CPLA_PLN_CODE,:CPRO_PRJ_CODE,:L_CPRO_PRJ_DESC,:CTRL_ID , :L_CKPR_CKPR_KPRJ_ROW )
            //            check = true;
            //        }
            //        //else
            //        //{
            //        //    string sql = string.Format("delete from pdf_ctrl_pro_row where ctrl_ctrl_id = {0}", item2.CTRL_ID);
            //        //    cntx.Database.ExecuteSqlCommand(sql);
            //        //    cntx.SaveChanges();
            //        //    string sql2 = string.Format(" insert into pdf_ctrl_pro_row (CTRR_AMNT,CTRL_CTRL_ID)  select 0,{0}    from pdf_standard_activity     where CKPR_KPRJ_ROW = {1}      and STND_LEVEL = {2}", item2.CTRL_ID, q.Select(xx => xx.CKPR_KPRJ_ROW).FirstOrDefault(), level);
            //        //    cntx.Database.ExecuteSqlCommand(sql2);
            //        //    cntx.SaveChanges();

            //        //}

            //        // }
            //        // }
            //    }

            //}
            //  if (check == true)
            //   {
            //       return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات با موفقیت ثبت شد" }.ToJson();
            //   }
            //}

            // else
            // {

            //     return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "فعالیت های استاندارد نوع پروژه " + kpro_desc + " در این سطح کامل نیست" }.ToJson();
            // }

            //return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = " اطلاعاتی جهت ثبت موجود نمی باشد" }.ToJson();
        }

        [EntityAuthorize("PRN_PDRF_WORK > select | PRN_PDRF_OPER > insert")]
        public ActionResult insert_letter_oper(PRN_PDRF_OPER objecttemp)
        {
            int wr_seqn = 0;
            string w_year = "";
            string code = Request.Form["WR_SEQN"];
            long let_code = long.Parse(Request.Form["CLET_LET_CODE"]);
            if (!string.IsNullOrEmpty(code))
            {
                var val = code.Split('-');
                wr_seqn = Convert.ToInt32(val[0]);
                w_year = val[1].ToString();
                objecttemp.PDRW_ID = Db.PRN_PDRF_WORK
                    .Where(xx => xx.TWRO_WR_SEQN == wr_seqn)
                    .Where(xx => xx.TWRO_W_YEAR == w_year)
                    .Where(xx => xx.CLET_LET_CODE == let_code)
                    .Select(xx => xx.ID).FirstOrDefault();
            }

            Db.PRN_PDRF_OPER.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("PRN_INQUIRY > insert")]
        public ActionResult insert_prn_inquiry(PRN_INQUIRY objecttemp)
        {
            objecttemp.INQY_STAT = "1";
            objecttemp.INQY_KIND = "1";
            Db.PRN_INQUIRY.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("PRN_LETTER > insert")]
        public ActionResult insert_letter(PRN_LETTER objecttemp)
        {
            Db.PRN_LETTER.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("PDF_DAILY_REPORT_TYPES > insert")]
        public ActionResult insert_drty(PDF_DAILY_REPORT_TYPES objecttemp)
        {
            if (PublicRepository.ExistModel("PDF_DAILY_REPORT_TYPES", "DEL_DUMP_U(DRPT_DESC) = DEL_DUMP_U('{0}') ", objecttemp.DRPT_DESC))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
            }
            else
            {
                objecttemp.TYPE_DESC = Request.Form["TYPE_DESC"];
                Db.PDF_DAILY_REPORT_TYPES.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات با موفقیت ثبت شد" }.ToJson();
            }
        }

        [EntityAuthorize("PDF_DAILY_REPORT_ITEMS > insert")]
        public ActionResult insert_drty_item(PDF_DAILY_REPORT_ITEMS objecttemp)
        {
            //if (PublicRepository.ExistModel("PDF_DAILY_REPORT_ITEMS", "DEL_DUMP_U(ITEM_DESC) = DEL_DUMP_U('{0}') ", objecttemp.ITEM_DESC))
            //{
            //    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
            //}
            //else
            //{
            if (!objecttemp.WGHT.HasValue)
                objecttemp.WGHT = Db.PDF_STANDARD_ACTIVITY.Where(xx => xx.ST_ID == objecttemp.STND_ST_ID)
                   .Select(xx => xx.WGHT).FirstOrDefault();
            Db.PDF_DAILY_REPORT_ITEMS.Add(objecttemp);
            Db.SaveChanges();
            return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات با موفقیت ثبت شد" }.ToJson();
            //return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات" }.ToJson();
            // }
        }

        [EntityAuthorize("PRN_TRANSCRIPT > insert")]
        public ActionResult insert_transcript(PRN_TRANSCRIPT objecttemp)
        {
            Db.PRN_TRANSCRIPT.Add(objecttemp);
            Db.SaveChanges();
            return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات با موفقیت ثبت شد" }.ToJson();
        }

        //public ActionResult insert_standard_operation(PDF_STANDARD_OPERATIONS objecttemp)
        //{
        //    cntx.PDF_STANDARD_OPERATIONS.Add(objecttemp);
        //    cntx.SaveChanges();
        //    return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات با موفقیت ثبت شد" }.ToJson();
        //}

        [EntityAuthorize("PRN_WAGES > insert")]
        public ActionResult insert_wage(PRN_WAGES objecttemp)
        {
            Db.PRN_WAGES.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }
        public ActionResult InsertTransaction(HSB_TRAN_PDRFS ObjectTemp)
        {
            try
            {
                string val = Request.Form["Trans"];
                ObjectTemp.STRN_TRAN_NO = short.Parse(val.Split('-')[0]);
                ObjectTemp.STRN_TRAN_TYPE = val.Split('-')[1];
                ObjectTemp.STRN_SSTO_STOR_CODE = short.Parse(val.Split('-')[2]);
                ObjectTemp.STRN_TRAN_YEAR = val.Split('-')[3];


                Db.HSB_TRAN_PDRFS.Add(ObjectTemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مشکل در ثبت اطلاعات" }.ToJson();


            }
        }

        public ActionResult InsertTransactionRequestGoods(STR_TRANSACTION ObjectTemp)
        {
            try
            {
                string G_FINY_YEAR = Request.Form["G_FINY_YEAR"];
                string val = Request.Form["radPayOrgan"];
                ObjectTemp.ORGA_CODE = val.Split('^')[2];
                ObjectTemp.ORGA_MANA_CODE = val.Split('^')[1];
                ObjectTemp.ORGA_MANA_ASTA_CODE = val.Split('^')[0];
                ObjectTemp.PRSN_EMP_NUMB = short.Parse(Request.Form["EmpNumbReceiv"]);





                ObjectTemp.AOUT_ATRS_ATRS_ROW = short.Parse(Request.Form["CollectorVal"]);
                ObjectTemp.SSTO_STOR_CODE = short.Parse(Request.Form["goodsval"]);
                ObjectTemp.TWRO_WR_SEQN = int.Parse(Request.Form["workorderlov"].Split('^')[0]);
                ObjectTemp.TWRO_W_YEAR = Request.Form["workorderlov"].Split('^')[1];
                ObjectTemp.CCNT_CNTR_NO = Request.Form["contractlov"];


                //تصویب کننده
                ObjectTemp.SCON_PRSN_EMP_NUMB = short.Parse(Request.Form["EmpNumbTasvib"]);
                var PersonleMosavab = from b in Db.STR_CONFIRM
                                      where b.PRSN_EMP_NUMB == ObjectTemp.SCON_PRSN_EMP_NUMB && b.ORGA_MANA_ASTA_CODE == ObjectTemp.ORGA_MANA_ASTA_CODE
                                      select b;
                ObjectTemp.SCON_ORGA_MANA_ASTA_CODE = ObjectTemp.ORGA_MANA_ASTA_CODE;
                ObjectTemp.SCON_ORGA_MANA_CODE = PersonleMosavab.Select(xx => xx.ORGA_MANA_CODE).FirstOrDefault();
                ObjectTemp.SCON_ORGA_CODE = PersonleMosavab.Select(xx => xx.ORGA_CODE).FirstOrDefault();



                //ثبت اطلاعات تایید کننده
                ObjectTemp.SCON_PRSN_EMP_NUMB_R = short.Parse(Request.Form["EmpNumbConfirm"]);
                var PersonleConfirm = from b in Db.STR_CONFIRM
                                      where b.PRSN_EMP_NUMB == ObjectTemp.SCON_PRSN_EMP_NUMB_R && b.ORGA_MANA_ASTA_CODE == ObjectTemp.ORGA_MANA_ASTA_CODE
                                      select b;
                ObjectTemp.SCON_ORGA_MANA_ASTA_CODE_R = ObjectTemp.ORGA_MANA_ASTA_CODE;
                ObjectTemp.SCON_ORGA_MANA_CODE_R = PersonleConfirm.Select(xx => xx.ORGA_MANA_CODE).FirstOrDefault();
                ObjectTemp.SCON_ORGA_CODE_R = PersonleConfirm.Select(xx => xx.ORGA_CODE).FirstOrDefault();
                ObjectTemp.SCON_SSTO_STOR_CODE_RELATED_TO = PersonleConfirm.Select(xx => xx.SSTO_STOR_CODE).FirstOrDefault();




                ObjectTemp.TRAN_TYPE = "2";
                ObjectTemp.TRAN_STAT = "1";
                ObjectTemp.ST_TRAN = "39";
                ObjectTemp.OUTF_STAT = "0";
                ObjectTemp.TECI_STAT = "0";
                ObjectTemp.MEET_STAT = "0";
                ObjectTemp.AJST_STAT = "1";
                ObjectTemp.TRAN_NO = (short)(Db.STR_TRANSACTION.Where(xx => xx.SSTO_STOR_CODE == ObjectTemp.SSTO_STOR_CODE && xx.TRAN_YEAR == ObjectTemp.TRAN_YEAR && xx.TRAN_TYPE == "2").Select(xx => xx.TRAN_NO).Max() + 1);

                int CheckStore = Db.Database.SqlQuery<int>(string.Format("SELECT NVL(COUNT(STR_GOODS_HISTORIES.SSTG_SGOD_GOOD_ID),0) FROM STR_GOODS_HISTORIES WHERE STR_GOODS_HISTORIES.SSTG_SSTO_STOR_CODE={0}    AND STR_GOODS_HISTORIES.GOOH_YEAR=TO_CHAR({1}+1)", ObjectTemp.SSTO_STOR_CODE, G_FINY_YEAR)).FirstOrDefault();
                if (CheckStore != 0)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "در این سال با این انبار شما نمی توانید عملیات انجام دهید" }.ToJson();
                }
                int CheckStoreCurrentYear = Db.Database.SqlQuery<int>(string.Format("SELECT NVL(COUNT(STR_GOODS_HISTORIES.SSTG_SGOD_GOOD_ID),0) FROM STR_GOODS_HISTORIES WHERE STR_GOODS_HISTORIES.SSTG_SSTO_STOR_CODE={0}    AND STR_GOODS_HISTORIES.GOOH_YEAR={1}", ObjectTemp.SSTO_STOR_CODE, G_FINY_YEAR)).FirstOrDefault();
                if (CheckStoreCurrentYear == 0)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "موجودی های این انبار انتقال نیافته است" }.ToJson();
                }
                ObjectTemp.CRET_BY = this.HttpContext.User.Identity.Name;


                Db.STR_TRANSACTION.Add(ObjectTemp);
                Db.SaveChanges();
                wp.StartProcess(this.HttpContext.User.Identity.Name, new string[] { this.HttpContext.User.Identity.Name },
                           Db.EXP_TYPE_DOC.Where(xx => xx.ETDO_ID == 623).Select(xx => xx.ETDO_DESC).FirstOrDefault(),
                           "درخواست کالا با شماره ردیف   " + ObjectTemp.TRAN_NO + " سال " + ObjectTemp.TRAN_YEAR + " انبار" + ObjectTemp.SSTO_STOR_CODE + "  ثبت شد ", 623,
                           ObjectTemp.TRAN_NO + "-" + ObjectTemp.TRAN_YEAR + "-" + ObjectTemp.SSTO_STOR_CODE + "-" + ObjectTemp.TRAN_TYPE);
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مشکل در ثبت اطلاعات" }.ToJson();


            }
        }

        public decimal return_noteid(short TRAN_NO, string TRAN_TYPE, string TRAN_YEAR, short SSTO_STOR_CODE)
        {

            string sql = "SELECT WF_NOTE_V.NOT_ID as m FROM WF_NOTE_V where stat='OPEN' and MESSAGE_NAME='MCREATOR' and upper(RECIPIENT_ROLE)='" + this.User.Identity.Name.ToUpper() + "' and  WF_NOTE_V.ITEM_KEY='PFLW_REGO^" + TRAN_NO + "-" + TRAN_YEAR + "-" + SSTO_STOR_CODE + "-" + TRAN_TYPE + " '";
            //and stat='OPEN'

            decimal not_id = Db.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            return not_id;
        }
        public ActionResult Get_RequestGoods([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from p in Db.STR_TRANSACTION
                         where p.TRAN_TYPE == "2"

                         select new
                         {
                             p.TRAN_NO,
                             p.TRAN_TYPE,
                             p.TRAN_YEAR,
                             p.TRAN_MONT,
                             p.TRAN_DAY,
                             TRAN_DATE = p.TRAN_YEAR + "/" + p.TRAN_MONT + "/" + p.TRAN_DAY,
                             p.SSTO_STOR_CODE,
                             p.TRAN_DESC,
                             p.ST_TRAN,
                             p.ORGA_CODE,
                             p.AOUT_ATRS_ATRS_ROW,
                             Store = p.STR_STORE.STOR_DESC,
                             Contract = p.CNT_CONTRACT.TITL,
                             p.CCNT_CNTR_NO
                         }).OrderByDescending(xx => xx.TRAN_NO).ThenByDescending(xx => xx.TRAN_YEAR).ThenByDescending(xx => xx.TRAN_MONT).ThenByDescending(xx => xx.TRAN_DAY).ToList();
            var FinalQuery = query.Select(p => new
            {
                p.TRAN_NO,
                p.TRAN_TYPE,
                p.TRAN_YEAR,
                p.TRAN_MONT,
                p.TRAN_DAY,
                p.TRAN_DATE,
                p.SSTO_STOR_CODE,
                p.TRAN_DESC,
                p.ST_TRAN,
                p.ORGA_CODE,
                p.AOUT_ATRS_ATRS_ROW,
                p.Store,
                p.Contract,
                p.CCNT_CNTR_NO
                //Not_id = return_noteid(p.TRAN_NO, p.TRAN_TYPE, p.TRAN_YEAR, p.SSTO_STOR_CODE)
            }).ToList();
            return Json(FinalQuery.ToDataSourceResult(request));
        }

        public JsonResult GetNoteId(string TRAN_NO, string TRAN_YEAR, string SSTO_STOR_CODE, string TRAN_TYPE)
        {

            string sql = string.Format("SELECT WF_NOTE_V.NOT_ID as m FROM WF_NOTE_V where stat='OPEN' and MESSAGE_NAME='MCREATOR' and upper(RECIPIENT_ROLE)='" + this.User.Identity.Name.ToUpper() + "' and  WF_NOTE_V.ITEM_KEY='FLW_REGO.PFLW_REGO^{0}-{1}-{2}-{3}'", TRAN_NO, TRAN_YEAR, SSTO_STOR_CODE, TRAN_TYPE);


            decimal not_id = Db.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            //return not_id;
            return this.Json(new { Success = true, data = not_id }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRecipient(string TRAN_NO, string TRAN_YEAR, string SSTO_STOR_CODE, string TRAN_TYPE, string Field)
        {


            AsrJobProvider jp = new AsrJobProvider();

            return this.Json(new { Success = true, data = jp.GetRecipient(TRAN_NO, TRAN_YEAR, SSTO_STOR_CODE, TRAN_TYPE, Field) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Get_RequestGoods_Row([DataSourceRequest] DataSourceRequest request, short TRAN_NO, short SSTO_STOR_CODE, string TRAN_TYPE, string TRAN_YEAR)
        {
            var query = (from p in Db.STR_TRANSACTION_ROW
                         where p.STRN_TRAN_TYPE == TRAN_TYPE && p.STRN_TRAN_NO == TRAN_NO
                         && p.STRN_TRAN_YEAR == TRAN_YEAR
                         && p.STRN_SSTO_STOR_CODE == SSTO_STOR_CODE

                         select new
                         {
                             p.STRN_TRAN_NO,
                             p.STRN_TRAN_TYPE,
                             p.STRN_TRAN_YEAR,
                             p.STRN_SSTO_STOR_CODE,
                             p.PURE_PRICE,
                             p.TRAR_QUAN_RECE,
                             GOOD_DESC = p.STR_STORE_GOODS.STR_GOODS.GOOD_DESC
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }
        public ActionResult insert_minute_deliver(TRH_MINUTE_DELIVER objecttemp)
        {
            string code = Request.Form["WR_SEQN"];
            if (!string.IsNullOrEmpty(code))
            {
                var val = code.Split('-');
                objecttemp.CCNR_TWRO_WR_SEQN = Convert.ToInt32(val[0]);
                objecttemp.CCNR_TWRO_W_YEAR = val[1].ToString();
            }

            var q = from b in Db.TRH_MINUTE_DELIVER
                    where b.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO && b.MIDE_TYPE == 2
                    select b;

            if (q.Any())
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "برای این قرارداد تحویل دائم ثبت شده است و امکان ثبت  نمی باشد" }.ToJson();
            }
            else
            {
                Db.TRH_MINUTE_DELIVER.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}]  ثبت شد ", objecttemp.MIDE_DESC) }.ToJson();
                //return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
            }
        }

        public ActionResult insert_minute_deliver_row(TRH_MINUTE_DELIVER_ROW objecttemp)
        {
            var q = from b in Db.TRH_MINUTE_DELIVER_ROW
                    where b.MIDE_ID == objecttemp.MIDE_ID && b.MIDR_DESC == objecttemp.MIDR_DESC
                    select b;

            if (q.Any())
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "رکورد ثبت شده تکراری است" }.ToJson();
            }
            else
            {
                Db.TRH_MINUTE_DELIVER_ROW.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}]  ثبت شد ", objecttemp.MIDR_DESC) }.ToJson();
                //return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
            }
        }

        [EntityAuthorize("PRN_INQUIRY_CONTRACTORS > insert")]
        public ActionResult insert_prn_inquiry_contractor(PRN_INQUIRY_CONTRACTORS objecttemp)
        {
            Db.PRN_INQUIRY_CONTRACTORS.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("PRN_INQUIRY_ROWS > insert")]
        public ActionResult insert_prn_inquiry_row(PRN_INQUIRY_ROWS objecttemp)
        {
            Db.PRN_INQUIRY_ROWS.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("PDF_CTRL_PRO_ROW > update")]
        public ActionResult Update_CTRL_ROW([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PDF_CTRL_PRO_ROW> PDF_CTRL_PRO_ROW)
        {
            if (PDF_CTRL_PRO_ROW != null)
            {
                foreach (PDF_CTRL_PRO_ROW item in PDF_CTRL_PRO_ROW)
                {
                    if (item.CTRR_AMNT <= 100)
                    {
                        Db.Entry(item).State = EntityState.Modified;
                        Db.SaveChanges();
                    }
                }
            }

            return Json(PDF_CTRL_PRO_ROW.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("PDF_CTRL_PRO > update")]
        public ActionResult Update_CTRL_PRO([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PDF_CTRL_PRO> PDF_CTRL_PRO)
        {
            if (PDF_CTRL_PRO != null)
            {
                foreach (PDF_CTRL_PRO item in PDF_CTRL_PRO)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(PDF_CTRL_PRO.ToDataSourceResult(request, ModelState));
        }
        public ActionResult UpdateCgtLetter(CGT_LETTER ObjectTemp)
        {
            try
            {
                string Sql = string.Format("update cgt_letter set LET_NUMBER='{0}',LET_DAY='{1}',LET_MNT='{2}',LET_YEAR='{3}',LET_KIND='{4}',LET_DESC='{5}',LET_AMNT={6} where let_code={7}",
                    ObjectTemp.LET_NUMBER,
                    ObjectTemp.LET_DAY,
                    ObjectTemp.LET_MNT,
                    ObjectTemp.LET_YEAR,
                    ObjectTemp.LET_KIND,
                    ObjectTemp.LET_DESC,
                    ObjectTemp.LET_AMNT,
                    ObjectTemp.LET_CODE

                    );
                Db.Database.ExecuteSqlCommand(Sql);
                Sql = string.Format("update prn_pay_draft set  ADST_NUM={0} where FIN_LETT_NO={1}", Request.Form["ADST_NUM"], Request.Form["PDRF_FIN_LETT_NO"]);

                Db.Database.ExecuteSqlCommand(Sql);
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "در زمان ثبت اطلاعات خطایی رخ داد" }.ToJson();

            }

            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();


        }

        [EntityAuthorize("PRN_TRANSCRIPT > update")]
        public ActionResult Update_transcript([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PRN_TRANSCRIPT> PRN_TRANSCRIPT)
        {
            if (PRN_TRANSCRIPT != null)
            {
                foreach (PRN_TRANSCRIPT item in PRN_TRANSCRIPT)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(PRN_TRANSCRIPT.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("PDF_DAILY_REPORT_TYPES > update")]
        public ActionResult Update_DRTY([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PDF_DAILY_REPORT_TYPES> PDF_DAILY_REPORT_TYPES)
        {
            if (PDF_DAILY_REPORT_TYPES != null)
            {
                foreach (PDF_DAILY_REPORT_TYPES item in PDF_DAILY_REPORT_TYPES)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(PDF_DAILY_REPORT_TYPES.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("PRN_INQUIRY > update")]
        public ActionResult update_prn_inquiry(PRN_INQUIRY objecttemp)
        {
            //objecttemp.RESP_DAY=Request.Form[""];
            string sql = string.Format("update prn_inquiry set RESP_DAY='{0}',RESP_MONT='{1}',RESP_YEAR='{2}',OPEN_DAY='{3}'" +
                " ,OPEN_MONT='{4}',OPEN_YEAR='{5}',OPEN_TIME='{6}' where ID='{7}' ",
                objecttemp.RESP_DAY = Request.Form["RESP_DAY"],
                objecttemp.RESP_MONT = Request.Form["RESP_MONT"],
                objecttemp.RESP_YEAR = Request.Form["RESP_YEAR"],
                objecttemp.OPEN_DAY = Request.Form["OPEN_DAY"],
                objecttemp.OPEN_MONT = Request.Form["OPEN_MONT"],
                objecttemp.OPEN_YEAR = Request.Form["OPEN_YEAR"],
                objecttemp.OPEN_TIME = Request.Form["OPEN_TIME"],
                Request.Form["ID"]
                );

            Db.Database.ExecuteSqlCommand(sql);
            Db.SaveChanges();

            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("TRH_STATEMENT_ROW > select | PRN_PAY_DRAFT > insert,select | PRN_PDRF_WORK > select,update | PRN_PDRF_OPER > select,update")]
        public ActionResult insert_pay_draft_letter(PRN_PAY_DRAFT objecttemp)
        {
            try
            {
                objecttemp.CLET_LET_CODE = Convert.ToInt32(Request.Form["let_code"]);

                long? let_amnt = Db.CGT_LETTER.Where(xx => xx.LET_CODE == objecttemp.CLET_LET_CODE).Select(xx => xx.LET_AMNT).FirstOrDefault();
                objecttemp.FIN_LETT_NO = ((Db.Database.SqlQuery<int>("select max(to_number(FIN_LETT_NO)) FIN_LETT_NO from prn_pay_draft").FirstOrDefault()) + 1).ToString();
                objecttemp.ADPY_STAT = "1";
                objecttemp.CCNT_CNTR_NO = Request.Form["CNTR_NO"];
                if (Request.Form["YEAR"] != "سال انجام کار")
                {
                    objecttemp.FINY_FINY_YEAR = Request.Form["YEAR"];
                    objecttemp.FINY_FINY_YEAR_R = Request.Form["YEAR"];
                }
                objecttemp.EXTR_TAX = Request.Form["EXTR_TAX"];
                string month = string.Empty, day = string.Empty;
                int intmonth = pc.GetMonth(thisDate);
                int intday = pc.GetDayOfMonth(thisDate);
                if (intmonth < 10 && intmonth > 0)
                {
                    month = "0" + intmonth.ToString();
                }
                else { month = Convert.ToString(intmonth); };
                if (intday < 10 && intday > 0)
                {
                    day = "0" + intday.ToString();
                }
                else { day = Convert.ToString(intday); };
                objecttemp.FIN_LETT_DAY = day;
                objecttemp.FIN_LETT_MONT = month;
                objecttemp.FIN_LETT_YEAR = Convert.ToString(pc.GetYear(thisDate));
                if
                    (string.Compare(objecttemp.FIN_LETT_DAY, "28") > 0 && string.Compare(objecttemp.FIN_LETT_MONT, "12") == 0 && string.Compare(objecttemp.FIN_LETT_YEAR, "1399") == 0)

                {
                    objecttemp.FIN_LETT_DAY = "28";
                }
                objecttemp.CRET_BY = orclname;
                objecttemp.CRET_DATE = DateTime.Now;


                objecttemp.PDRF_AMNT = let_amnt;
                var remain_amnt = Db.Database.SqlQuery<decimal>(string.Format("SELECT PRN_REMAIN_AMNT_U('{0}') FROM DUAL", objecttemp.CCNT_CNTR_NO)).FirstOrDefault();

                var query2 = from b in Db.PRN_PDRF_WORK
                             join c in Db.PRN_PDRF_OPER on b.ID equals c.PDRW_ID
                             join w in Db.CGT_KPRO_OPER on c.COPE_OPRN_CODE equals w.COPE_OPRN_CODE
                             where (b.CLET_LET_CODE == objecttemp.CLET_LET_CODE && b.ID == c.PDRW_ID && c.COPE_OPRN_CODE == w.COPE_OPRN_CODE)
                             select w;

                string let_type = Db.CGT_LETTER.Where(xx => xx.LET_CODE == objecttemp.CLET_LET_CODE).Select(xx => xx.LET_TYPE).FirstOrDefault();
                var query = from b in Db.PRN_PDRF_WORK
                            join c in Db.PRN_PDRF_OPER on b.ID equals c.PDRW_ID
                            where (b.CLET_LET_CODE == objecttemp.CLET_LET_CODE && b.ID == c.PDRW_ID)
                            select c;
                if (let_type == "15")
                {
                    query = from c in Db.PRN_PDRF_OPER
                            where (c.CLET_LET_CODE == objecttemp.CLET_LET_CODE)
                            select c;
                }
                switch (let_type)
                {
                    case ("5"):
                        {
                            objecttemp.PDRF_TYPE = "4";
                        }
                        break;
                    case ("4"):
                        {
                            objecttemp.PDRF_TYPE = "5";
                        }
                        break;
                    case ("12"):
                        {
                            objecttemp.PDRF_TYPE = "2";
                        }
                        break;
                    case ("13"):
                        {
                            objecttemp.PDRF_TYPE = "3";
                        }
                        break;
                    case ("14"):
                        {
                            objecttemp.PDRF_TYPE = "7";
                        }
                        break;
                    case ("15"):
                        {
                            objecttemp.PDRF_TYPE = "1";
                        }
                        break;
                    case ("19"):
                        {
                            objecttemp.PDRF_TYPE = "8";
                        }
                        break;
                }

                query2 = from b in Db.PRN_PDRF_WORK
                         join c in Db.PRN_PDRF_OPER on b.ID equals c.PDRW_ID
                         join w in Db.CGT_KPRO_OPER on c.COPE_OPRN_CODE equals w.COPE_OPRN_CODE
                         where (b.CLET_LET_CODE == objecttemp.CLET_LET_CODE && b.ID == c.PDRW_ID && c.COPE_OPRN_CODE == w.COPE_OPRN_CODE)
                         select w;

                string sql2 = string.Format("SELECT count(*) from prn_pdrf_oper where  prn_get_remain_u(TWRO_WR_SEQN ,TWRO_W_YEAR ,'{0}',COPE_OPRN_CODE,SUBU_TYPE)<nvl(prn_pdrf_OPER.AMNT,0)-nvl(prn_pdrf_OPER.ADVN_AMNT,0) and PDRW_ID in (select id from prn_pdrf_work  where clet_let_code='{1}')  ", objecttemp.FINY_FINY_YEAR, objecttemp.CLET_LET_CODE);

                int pdrf_check = Db.Database.SqlQuery<int>(sql2).FirstOrDefault();

                string SPLY_TYPE = Db.CNT_CONTRACT.Where(xx => xx.CNTR_NO == objecttemp.CCNT_CNTR_NO).Select(xx => xx.SPLY_TYPE).FirstOrDefault();

                if (SPLY_TYPE != "1")
                {
                    if (pdrf_check >= 1)
                    {
                        string oper_desc = Db.Database.SqlQuery<string>(string.Format("SELECT concatall(b.OPRN_DESC||' دستور کار'||TWRO_WR_SEQN||'-' || TWRO_W_YEAR ||' مانده '||prn_get_remain_u(TWRO_WR_SEQN ,TWRO_W_YEAR ,'1395',COPE_OPRN_CODE,SUBU_TYPE)||',' )  FROM prn_pdrf_oper a,PCT_OPERATION b where  a.COPE_OPRN_CODE=b.OPRN_CODE and  prn_get_remain_u(TWRO_WR_SEQN ,TWRO_W_YEAR ,'{0}',COPE_OPRN_CODE,SUBU_TYPE)<a.AMNT and PDRW_ID in (select id from prn_pdrf_work  where clet_let_code='{1}')  ", objecttemp.FINY_FINY_YEAR, objecttemp.CLET_LET_CODE)).FirstOrDefault();
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مانده تعهد دستور کار در این عملیات " + oper_desc + " مبلغ مورد نظر را پوشش نمی دهد" }.ToJson();
                    }
                }

                objecttemp.STST_PRIC = objecttemp.PDRF_AMNT +
                                       (Db.PRN_PAY_DRAFT.Where(xx => xx.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Where(xx => xx.PDRF_TYPE == objecttemp.PDRF_TYPE).Select(xx => xx.PDRF_AMNT).Sum() == null ?
                                        0 :
                                        Db.PRN_PAY_DRAFT.Where(xx => xx.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Where(xx => xx.PDRF_TYPE == objecttemp.PDRF_TYPE).Select(xx => xx.PDRF_AMNT).Sum());

                objecttemp.ADST_NUM = Convert.ToInt16(Convert.ToInt16((Db.PRN_PAY_DRAFT.Where(xx => xx.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Where(xx => xx.PDRF_TYPE == objecttemp.PDRF_TYPE).Max(xx => xx.ADST_NUM))) + 1);

                if ((let_amnt == query.Select(xx => xx.AMNT).Sum() && let_amnt <= remain_amnt && (let_type != "19" || let_type != "12")) || (orclname == "JAMALZADEH"))
                {
                    Db.PRN_PAY_DRAFT.Add(objecttemp);
                    Db.SaveChanges();
                    //objecttemp.FIN_LETT_MONT
                    //objecttemp.FIN_LETT_YEAR
                    string sql = string.Format("update prn_pdrf_work set PDRF_FIN_LETT_NO='{0}' where CLET_LET_CODE='{1}'", objecttemp.FIN_LETT_NO, objecttemp.CLET_LET_CODE);
                    Db.Database.ExecuteSqlCommand(sql);
                    if (Db.Database.SqlQuery<int>(string.Format("select id from prn_pdrf_work  where PDRF_FIN_LETT_NO='{0}' and CLET_LET_CODE='{1}' ", objecttemp.FIN_LETT_NO, objecttemp.CLET_LET_CODE)).Any())
                    {
                        sql = string.Format("update prn_pdrf_oper set PDRF_FIN_LETT_NO='{0}',UNDE_CUNI_UNIT_CODE='{2}',UNDE_UNIT_ROW='{3}' where PDRW_ID in (select id from prn_pdrf_work  where CLET_LET_CODE='{1}')",
                        objecttemp.FIN_LETT_NO,
                        objecttemp.CLET_LET_CODE,
                        query2.Select(xx => xx.UNDE_CUNI_UNIT_CODE).FirstOrDefault(),
                        query2.Select(xx => xx.UNDE_UNIT_ROW).FirstOrDefault());
                        Db.Database.ExecuteSqlCommand(sql);
                    }
                    else
                    {
                        sql = string.Format("update prn_pdrf_oper set PDRF_FIN_LETT_NO='{0}',UNDE_CUNI_UNIT_CODE='{2}',UNDE_UNIT_ROW='{3}' where  CLET_LET_CODE='{1}')",
                        objecttemp.FIN_LETT_NO,
                        objecttemp.CLET_LET_CODE,
                        query2.Select(xx => xx.UNDE_CUNI_UNIT_CODE).FirstOrDefault(),
                        query2.Select(xx => xx.UNDE_UNIT_ROW).FirstOrDefault());
                        Db.Database.ExecuteSqlCommand(sql);

                    }
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
                    // return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
                }
                else if (let_amnt == query.Select(xx => xx.AMNT).Sum() && (let_type == "19" || let_type == "12"))
                {
                    Db.PRN_PAY_DRAFT.Add(objecttemp);
                    Db.SaveChanges();
                    //objecttemp.FIN_LETT_MONT
                    //objecttemp.FIN_LETT_YEAR
                    string sql = string.Format("update prn_pdrf_work set PDRF_FIN_LETT_NO='{0}' where CLET_LET_CODE='{1}'", objecttemp.FIN_LETT_NO, objecttemp.CLET_LET_CODE);
                    Db.Database.ExecuteSqlCommand(sql);
                    sql = string.Format("update prn_pdrf_oper set PDRF_FIN_LETT_NO='{0}',UNDE_CUNI_UNIT_CODE='{2}',UNDE_UNIT_ROW='{3}' where PDRW_ID in (select id from prn_pdrf_work  where CLET_LET_CODE='{1}')",
                    objecttemp.FIN_LETT_NO,
                    objecttemp.CLET_LET_CODE,
                    query2.Select(xx => xx.UNDE_CUNI_UNIT_CODE).FirstOrDefault(),
                    query2.Select(xx => xx.UNDE_UNIT_ROW).FirstOrDefault());
                    Db.Database.ExecuteSqlCommand(sql);
                    //AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
                    //wp.StartProcess(HttpContext.User.Identity.Name, null, string.Format("برسی درخواست شماره {0}", objecttemp.TCDC_CODE), objecttemp.EQPD_DESC, 63, objecttemp.TCDC_CODE);

                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
                }
                //} 
                else
                {

                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مانده تعهد دستور کار مبلغ مورد نظر را پوشش نمی دهد" }.ToJson();
                }
                // return Json(new { Success = "False" }, JsonRequestBehavior.DenyGet);
                //return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مبلغ ثبت شده بایستی با مبلغ کارکرد جاری یکسان باشد " }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات" }.ToJson();

            }
        }

        public decimal? FND_PRICE_JARI(short? COPE_OPRN_CODE, int? TWRO_WR_SEQN, string TWRO_W_YEAR, string FINY_FINY_YEAR)
        {
            decimal? RetVal = 0;
            decimal? PrnPrice = Db.Database.SqlQuery<decimal?>(string.Format("select  sum(decode(nvl(r_amnt,0),0,amnt,r_amnt)) from prn_price	where wr_seq={0} and w_year='{1}'	and nvl(pdrf_type,0) <> 4", TWRO_WR_SEQN, TWRO_W_YEAR)).FirstOrDefault();
            decimal? CRED_NEW = Db.Database.SqlQuery<decimal?>(string.Format("SELECT NVL(SUM(NVL(CRED_NEW,0)),0)       FROM cgt_cred_alloc      where cgt_cred_alloc.TWRO_WR_SEQN={0}      and  cgt_cred_alloc.TWRO_W_YEAR ='{1}'" +
                                                                   " AND  cgt_cred_alloc.COPE_OPRN_CODE = {2}      AND  cgt_cred_alloc.TWRO_STAT = '3'      AND  cgt_cred_alloc.CRED_STAT = '1'      AND  cgt_cred_alloc.CRED_KIND = '0'      AND  cgt_cred_alloc.CRED_YEAR = '{3}'", TWRO_WR_SEQN, TWRO_W_YEAR, COPE_OPRN_CODE, FINY_FINY_YEAR)).FirstOrDefault();

            RetVal = (CRED_NEW == null ? 0 : CRED_NEW) - (PrnPrice == null ? 0 : PrnPrice);

            return RetVal;


        }

        [EntityAuthorize("TRH_STATEMENT_ROW > select | PRN_PAY_DRAFT > insert,select | PRN_PDRF_WORK > select,update | PRN_PDRF_OPER > select,update")]
        public ActionResult insert_pay_draft_letter_current(PRN_PAY_DRAFT objecttemp)
        {
            try
            {
                objecttemp.CLET_LET_CODE = Convert.ToInt32(Request.Form["let_code"]);

                long? let_amnt = Db.CGT_LETTER.Where(xx => xx.LET_CODE == objecttemp.CLET_LET_CODE).Select(xx => xx.LET_AMNT).FirstOrDefault();
                objecttemp.FIN_LETT_NO = ((Db.Database.SqlQuery<int>("select max(to_number(FIN_LETT_NO)) FIN_LETT_NO from prn_pay_draft").FirstOrDefault()) + 1).ToString();
                int OperCount = Db.Database.SqlQuery<int>(string.Format("select count(*) from prn_pdrf_oper where CLET_LET_CODE ={0} and TWRO_WR_SEQN is not null", objecttemp.CLET_LET_CODE)).FirstOrDefault();
                objecttemp.ADPY_STAT = "1";
                objecttemp.CCNT_CNTR_NO = Request.Form["CNTR_NO"];
                if (Request.Form["YEAR"] != "سال انجام کار")
                {
                    objecttemp.FINY_FINY_YEAR = Request.Form["YEAR"];
                    objecttemp.FINY_FINY_YEAR_R = Request.Form["YEAR"];
                }

                int v_count = Db.PRN_PDRF_OPER.Where(xx => xx.CLET_LET_CODE == objecttemp.CLET_LET_CODE && xx.TWRO_WR_SEQN == null).Count();

                if (v_count != 0)
                {
                    v_count = Db.PRN_PDRF_OPER.Where(xx => xx.CLET_LET_CODE == objecttemp.CLET_LET_CODE && xx.TWRO_WR_SEQN != null).Count();
                    if (v_count != 0)
                    {
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "کليه رديف ها بايد داراي دستور کار باشد" }.ToJson();
                    }
                    else
                    {
                        int v_count_work = Db.CNT_WORK_CONTRACT.Where(xx => xx.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Count();
                        int v_count_PUR = Db.CNT_PUR_CONTRACT.Where(xx => xx.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Count();
                        if (v_count_work + v_count_PUR <= 0)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مدرک تعهد شده پيوست قراداد نمي باشد" }.ToJson();
                        }
                    }

                }

                if (OperCount == 0)//چک کردن مانده اعتبار گواهی پرداخت جاری با دستورکار
                {
                    long? v_w_remn = Db.Database.SqlQuery<long?>(string.Format("SELECT nvl(sum(nvl(CGT_CRED_ALLOC.CRED_AMNT,0)),0) FROM CGT_CRED_ALLOC, CNT_WORK_CONTRACT  WHERE(CNT_WORK_CONTRACT.CCNT_CNTR_NO ='{0}' )" +
                     " AND((CGT_CRED_ALLOC.TWQR_TWRQ_WQ_SEQ = CNT_WORK_CONTRACT.TWRQ_WQ_SEQ) AND(CGT_CRED_ALLOC.TWQR_TWRQ_YEAR = CNT_WORK_CONTRACT.TWRQ_YEAR))" +
                     " and CRED_YEAR = '{1}' and cred_stat = 1 ",
                     objecttemp.CCNT_CNTR_NO,
                     objecttemp.FINY_FINY_YEAR
                     )).FirstOrDefault();
                    //مانده مدارک
                    long? v_p_remn = Db.Database.SqlQuery<long?>(string.Format("SELECT nvl(sum(nvl(CGT_CRED_ALLOC.CRED_AMNT,0)),0) FROM CGT_CRED_ALLOC, cnt_pur_contract WHERE ( cnt_pur_contract.CCNT_CNTR_NO='{0}' )" +
                    " AND  ((CGT_CRED_ALLOC.SPUR_PURE_YEAR=cnt_pur_contract.SPUR_PURE_YEAR) AND (CGT_CRED_ALLOC.SPUR_PURE_NO=cnt_pur_contract.SPUR_PURE_NO)) " +
                    " and CRED_YEAR = '{1}' and cred_stat = 1  ",
                    objecttemp.CCNT_CNTR_NO,
                    objecttemp.FINY_FINY_YEAR
                    )).FirstOrDefault();

                    //
                    long? v_cntr = Db.Database.SqlQuery<long?>(string.Format("select nvl(sum(nvl(pdrf_amnt,0)-nvl(advn_amnt,0)),0) from prn_pay_draft o " +
                    " where  not exists (select 1 from cgt_cred_alloc where cred_year = '{1}' and cred_stat = 1 and PDRF_FIN_LETT_NO=o.FIN_LETT_NO) " +
                    " and ccnt_cntr_no ='{0}' and o.finy_finy_year= '{1}'  and pdrf_type not in (3,4)  ",
                    objecttemp.CCNT_CNTR_NO,
                    objecttemp.FINY_FINY_YEAR
                    )).FirstOrDefault();

                    if ((let_amnt == null ? 0 : let_amnt) - (objecttemp.ADVN_AMNT == null ? 0 : objecttemp.ADVN_AMNT) > (v_w_remn == null ? 0 : v_w_remn) + (v_p_remn == null ? 0 : v_p_remn) - (v_cntr == null ? 0 : v_cntr))
                    {
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مانده تعهد مدارک مبلغ صورت وضعيت را پوشش نميدهد" }.ToJson();

                    }


                }
                else
                {
                    var QueryOper = from b in Db.PRN_PDRF_OPER
                                    where b.CLET_LET_CODE == objecttemp.CLET_LET_CODE
                                    select new
                                    {
                                        b.COPE_OPRN_CODE,
                                        b.TWRO_WR_SEQN,
                                        b.TWRO_W_YEAR,
                                        b.AMNT
                                    };
                    foreach (var Row in QueryOper)
                    {
                        decimal? RetVal = FND_PRICE_JARI(Row.COPE_OPRN_CODE, Row.TWRO_WR_SEQN, Row.TWRO_W_YEAR, objecttemp.FINY_FINY_YEAR);
                        if ((Row.AMNT == null ? 0 : Row.AMNT) > RetVal)
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("مانده تعهد مدارک مبلغ صورت وضعيت را پوشش نميدهد.مبلغ مجاز {0} از دستور کار {1} است", RetVal, Row.TWRO_WR_SEQN) }.ToJson();

                        }


                    }



                }


                objecttemp.EXTR_TAX = Request.Form["EXTR_TAX"];
                string month = string.Empty, day = string.Empty;
                int intmonth = pc.GetMonth(thisDate);
                int intday = pc.GetDayOfMonth(thisDate);
                if (intmonth < 10 && intmonth > 0)
                {
                    month = "0" + intmonth.ToString();
                }
                else { month = Convert.ToString(intmonth); };
                if (intday < 10 && intday > 0)
                {
                    day = "0" + intday.ToString();
                }
                else { day = Convert.ToString(intday); };
                objecttemp.FIN_LETT_DAY = day;
                objecttemp.FIN_LETT_MONT = month;
                objecttemp.FIN_LETT_YEAR = Convert.ToString(pc.GetYear(thisDate));
                if
                    (string.Compare(objecttemp.FIN_LETT_DAY, "24") > 0 && string.Compare(objecttemp.FIN_LETT_MONT, "12") == 0 && string.Compare(objecttemp.FIN_LETT_YEAR, "1399") == 0)

                {
                    objecttemp.FIN_LETT_DAY = "24";
                }
                objecttemp.CRET_BY = orclname;
                objecttemp.CRET_DATE = DateTime.Now;


                objecttemp.PDRF_AMNT = let_amnt;
                var remain_amnt = Db.Database.SqlQuery<decimal>(string.Format("SELECT PRN_REMAIN_AMNT_U('{0}') FROM DUAL", objecttemp.CCNT_CNTR_NO)).FirstOrDefault();



                string let_type = Db.CGT_LETTER.Where(xx => xx.LET_CODE == objecttemp.CLET_LET_CODE).Select(xx => xx.LET_TYPE).FirstOrDefault();



                var query = from c in Db.PRN_PDRF_OPER
                            where (c.CLET_LET_CODE == objecttemp.CLET_LET_CODE)
                            select c;

                switch (let_type)
                {
                    case ("5"):
                        {
                            objecttemp.PDRF_TYPE = "4";
                        }
                        break;
                    case ("4"):
                        {
                            objecttemp.PDRF_TYPE = "5";
                        }
                        break;
                    case ("12"):
                        {
                            objecttemp.PDRF_TYPE = "2";
                        }
                        break;
                    case ("13"):
                        {
                            objecttemp.PDRF_TYPE = "3";
                        }
                        break;
                    case ("14"):
                        {
                            objecttemp.PDRF_TYPE = "7";
                        }
                        break;
                    case ("15"):
                        {
                            objecttemp.PDRF_TYPE = "1";
                        }
                        break;
                    case ("19"):
                        {
                            objecttemp.PDRF_TYPE = "8";
                        }
                        break;
                }

                //query2 = from b in Db.PRN_PDRF_WORK
                //         join c in Db.PRN_PDRF_OPER on b.ID equals c.PDRW_ID
                //         join w in Db.CGT_KPRO_OPER on c.COPE_OPRN_CODE equals w.COPE_OPRN_CODE
                //         where (b.CLET_LET_CODE == objecttemp.CLET_LET_CODE && b.ID == c.PDRW_ID && c.COPE_OPRN_CODE == w.COPE_OPRN_CODE)
                //         select w;

                string sql2 = string.Format("SELECT count(*) from prn_pdrf_oper where  prn_get_remain_u(TWRO_WR_SEQN ,TWRO_W_YEAR ,'{0}',COPE_OPRN_CODE,SUBU_TYPE)<nvl(prn_pdrf_OPER.AMNT,0)-nvl(prn_pdrf_OPER.ADVN_AMNT,0) and PDRW_ID in (select id from prn_pdrf_work  where clet_let_code='{1}')  ", objecttemp.FINY_FINY_YEAR, objecttemp.CLET_LET_CODE);

                int pdrf_check = Db.Database.SqlQuery<int>(sql2).FirstOrDefault();

                string SPLY_TYPE = Db.CNT_CONTRACT.Where(xx => xx.CNTR_NO == objecttemp.CCNT_CNTR_NO).Select(xx => xx.SPLY_TYPE).FirstOrDefault();

                if (SPLY_TYPE != "1")
                {
                    if (pdrf_check >= 1)
                    {
                        string oper_desc = Db.Database.SqlQuery<string>(string.Format("SELECT concatall(b.OPRN_DESC||' دستور کار'||TWRO_WR_SEQN||'-' || TWRO_W_YEAR ||' مانده '||prn_get_remain_u(TWRO_WR_SEQN ,TWRO_W_YEAR ,'1395',COPE_OPRN_CODE,SUBU_TYPE)||',' )  FROM prn_pdrf_oper a,PCT_OPERATION b where  a.COPE_OPRN_CODE=b.OPRN_CODE and  prn_get_remain_u(TWRO_WR_SEQN ,TWRO_W_YEAR ,'{0}',COPE_OPRN_CODE,SUBU_TYPE)<a.AMNT and PDRW_ID in (select id from prn_pdrf_work  where clet_let_code='{1}')  ", objecttemp.FINY_FINY_YEAR, objecttemp.CLET_LET_CODE)).FirstOrDefault();
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مانده تعهد دستور کار در این عملیات " + oper_desc + " مبلغ مورد نظر را پوشش نمی دهد" }.ToJson();
                    }
                }

                objecttemp.STST_PRIC = objecttemp.PDRF_AMNT +
                                       (Db.PRN_PAY_DRAFT.Where(xx => xx.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Where(xx => xx.PDRF_TYPE == objecttemp.PDRF_TYPE).Select(xx => xx.PDRF_AMNT).Sum() == null ?
                                        0 :
                                        Db.PRN_PAY_DRAFT.Where(xx => xx.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Where(xx => xx.PDRF_TYPE == objecttemp.PDRF_TYPE).Select(xx => xx.PDRF_AMNT).Sum());

                objecttemp.ADST_NUM = Convert.ToInt16(Convert.ToInt16((Db.PRN_PAY_DRAFT.Where(xx => xx.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Where(xx => xx.PDRF_TYPE == objecttemp.PDRF_TYPE).Max(xx => xx.ADST_NUM))) + 1);

                if ((let_amnt == query.Select(xx => xx.AMNT).Sum() && let_amnt <= remain_amnt && (let_type != "19" || let_type != "12")) || objecttemp.PDRF_TYPE == "2" || (orclname == "JAMALZADEH"))
                {
                    Db.PRN_PAY_DRAFT.Add(objecttemp);
                    Db.SaveChanges();
                    //objecttemp.FIN_LETT_MONT
                    //objecttemp.FIN_LETT_YEAR
                    string sql = string.Format("update prn_pdrf_work set PDRF_FIN_LETT_NO='{0}' where CLET_LET_CODE='{1}'", objecttemp.FIN_LETT_NO, objecttemp.CLET_LET_CODE);
                    Db.Database.ExecuteSqlCommand(sql);

                    sql = string.Format("update prn_pdrf_oper set PDRF_FIN_LETT_NO='{0}' where  CLET_LET_CODE='{1}'",
                    objecttemp.FIN_LETT_NO,
                    objecttemp.CLET_LET_CODE);
                    Db.Database.ExecuteSqlCommand(sql);

                    sql = string.Format("update cgt_letter set let_stat='4' where  LET_CODE='{0}'",

                     objecttemp.CLET_LET_CODE);
                    Db.Database.ExecuteSqlCommand(sql);

                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();

                }

                //} 
                else
                {

                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مانده قرارداد مبلغ مورد نظر را پوشش نمی دهد" }.ToJson();
                }
                // return Json(new { Success = "False" }, JsonRequestBehavior.DenyGet);
                //return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مبلغ ثبت شده بایستی با مبلغ کارکرد جاری یکسان باشد " }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات" }.ToJson();

            }
        }

        [EntityAuthorize("TRH_STATEMENT_ROW > select | PRN_PAY_DRAFT > insert,select | PRN_PDRF_WORK > select,update | PRN_PDRF_OPER > select,update")]
        public ActionResult insert_pay_draft(PRN_PAY_DRAFT objecttemp)
        {
            try
            {
                string msg = string.Empty;
                objecttemp.STMT_STMT_ID = Convert.ToInt32(Request.Form["STMT_ID"]);
                objecttemp.ADST_NUM = Db.TRH_STATEMENT.Where(xx => xx.STMT_ID == objecttemp.STMT_STMT_ID).Select(xx => xx.STMT_NO).FirstOrDefault();
                objecttemp.FIN_LETT_NO = ((Db.Database.SqlQuery<int>("select max(to_number(FIN_LETT_NO)) FIN_LETT_NO from prn_pay_draft").FirstOrDefault()) + 1).ToString();
                objecttemp.ADPY_STAT = "1";
                objecttemp.PDRF_TYPE = "1";
                objecttemp.CCNT_CNTR_NO = Request.Form["CNTR_NO"];
                if (Request.Form["YEAR"] != "سال انجام کار")
                {
                    objecttemp.FINY_FINY_YEAR = Request.Form["YEAR"];
                    objecttemp.FINY_FINY_YEAR_R = Request.Form["YEAR"];
                }
                objecttemp.EXTR_TAX = Request.Form["EXTR_TAX"];
                string month = string.Empty, day = string.Empty;
                int intmonth = pc.GetMonth(thisDate);
                int intday = pc.GetDayOfMonth(thisDate);
                if (intmonth < 10 && intmonth > 0)
                {
                    month = "0" + intmonth.ToString();
                }
                else { month = Convert.ToString(intmonth); };
                if (intday < 10 && intday > 0)
                {
                    day = "0" + intday.ToString();
                }
                else { day = Convert.ToString(intday); };
                objecttemp.FIN_LETT_DAY = day;
                objecttemp.FIN_LETT_MONT = month;
                objecttemp.FIN_LETT_YEAR = Convert.ToString(pc.GetYear(thisDate));
                if
                    (string.Compare(objecttemp.FIN_LETT_DAY, "28") > 0 && string.Compare(objecttemp.FIN_LETT_MONT, "12") == 0 && string.Compare(objecttemp.FIN_LETT_YEAR, "1399") == 0)

                {
                    objecttemp.FIN_LETT_DAY = "28";
                }
                string LastDay = Db.Database.SqlQuery<string>(string.Format("SELECT END_DAY from BKP_BOOK_STAT where FINY_FINY_YEAR={0} and BKTP_BK_CODE=decode('{1}',0,2,1,1,2,3)", objecttemp.FINY_FINY_YEAR, Db.CNT_CONTRACT.Where(xx => xx.CNTR_NO == objecttemp.CCNT_CNTR_NO).Select(xx => xx.SPLY_TYPE).FirstOrDefault())).FirstOrDefault();
                //if (string.Compare(objecttemp.FIN_LETT_YEAR, objecttemp.FINY_FINY_YEAR)>0)
                //{
                //    objecttemp.FIN_LETT_DAY = LastDay;
                //    objecttemp.FIN_LETT_MONT = "12";
                //    objecttemp.FIN_LETT_YEAR = objecttemp.FINY_FINY_YEAR;

                //}

                objecttemp.CRET_BY = orclname;
                objecttemp.CRET_DATE = DateTime.Now;

                var query = from b in Db.PRN_PDRF_WORK
                            join c in Db.PRN_PDRF_OPER on b.ID equals c.PDRW_ID
                            where (b.STMT_STMT_ID == objecttemp.STMT_STMT_ID && b.ID == c.PDRW_ID)
                            select c;

                long? price = Db.Database.SqlQuery<long>(string.Format("SELECT nvl(prn_current_stmr_u('{0}'),0) FROM DUAL", objecttemp.STMT_STMT_ID)).FirstOrDefault();
                objecttemp.PDRF_AMNT = price;
                var end_amnt = Db.Database.SqlQuery<decimal>(string.Format("SELECT nvl(PRN_INC_U('{0}'),0)+nvl(CNT_FIRST_AMNT_U('{0}'),0) FROM DUAL", objecttemp.CCNT_CNTR_NO)).FirstOrDefault();
                var STST_PRIC = Db.Database.SqlQuery<decimal>(string.Format("SELECT STMR_PRICE from TRH_STMR_V where STMT_ID ='{0}'", objecttemp.STMT_STMT_ID)).FirstOrDefault();

                objecttemp.STST_PRIC = STST_PRIC;

                var query2 = from b in Db.PRN_PDRF_WORK
                             join c in Db.PRN_PDRF_OPER on b.ID equals c.PDRW_ID
                             join w in Db.CGT_KPRO_OPER on c.COPE_OPRN_CODE equals w.COPE_OPRN_CODE
                             where (b.STMT_STMT_ID == objecttemp.STMT_STMT_ID && b.ID == c.PDRW_ID && c.COPE_OPRN_CODE == w.COPE_OPRN_CODE)
                             select w;

                string sql2 = string.Format("SELECT count(*) from prn_pdrf_oper where  prn_get_remain_u(TWRO_WR_SEQN ,TWRO_W_YEAR ,'{0}',COPE_OPRN_CODE,SUBU_TYPE)<nvl(prn_pdrf_OPER.AMNT,0)-nvl(prn_pdrf_OPER.ADVN_AMNT,0) and PDRW_ID in (select id from prn_pdrf_work  where stmt_stmt_id='{1}')  ", objecttemp.FINY_FINY_YEAR, objecttemp.STMT_STMT_ID);

                int pdrf_check = Db.Database.SqlQuery<int>(sql2).FirstOrDefault();

                string SPLY_TYPE = Db.CNT_CONTRACT.Where(xx => xx.CNTR_NO == objecttemp.CCNT_CNTR_NO).Select(xx => xx.SPLY_TYPE).FirstOrDefault();

                if (SPLY_TYPE != "1")
                {
                    if (pdrf_check >= 1)
                    {
                        string oper_desc = Db.Database.SqlQuery<string>(string.Format("SELECT concatall(b.OPRN_DESC||' دستور کار'||TWRO_WR_SEQN||'-' || TWRO_W_YEAR ||' مانده '||prn_get_remain_u(TWRO_WR_SEQN ,TWRO_W_YEAR ,'1395',COPE_OPRN_CODE,SUBU_TYPE)||',' )  FROM prn_pdrf_oper a,PCT_OPERATION b where  a.COPE_OPRN_CODE=b.OPRN_CODE and  prn_get_remain_u(TWRO_WR_SEQN ,TWRO_W_YEAR ,'{0}',COPE_OPRN_CODE,SUBU_TYPE)<a.AMNT and PDRW_ID in (select id from prn_pdrf_work  where clet_let_code='{1}')  ", objecttemp.FINY_FINY_YEAR, objecttemp.CLET_LET_CODE)).FirstOrDefault();
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مانده تعهد دستور کار در این عملیات " + oper_desc + " مبلغ مورد نظر را پوشش نمی دهد" }.ToJson();
                    }
                }

                if (Db.PRN_PAY_DRAFT.Where(xx => xx.STMT_STMT_ID == objecttemp.STMT_STMT_ID).Select(xx => xx.FIN_LETT_NO).Any())
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "گواهی پرداخت تکراری می باشد" }.ToJson();
                }

                if (price == query.Select(xx => xx.AMNT).Sum() && price <= end_amnt)
                {
                    Db.PRN_PAY_DRAFT.Add(objecttemp);
                    Db.SaveChanges();
                    //objecttemp.FIN_LETT_MONT
                    //objecttemp.FIN_LETT_YEAR
                    string sql = string.Format("update prn_pdrf_work set PDRF_FIN_LETT_NO='{0}' where STMT_STMT_ID='{1}'", objecttemp.FIN_LETT_NO, objecttemp.STMT_STMT_ID);
                    Db.Database.ExecuteSqlCommand(sql);
                    sql = string.Format("update prn_pdrf_oper set PDRF_FIN_LETT_NO='{0}',UNDE_CUNI_UNIT_CODE='{2}',UNDE_UNIT_ROW='{3}' where PDRW_ID in (select id from prn_pdrf_work  where stmt_stmt_id='{1}')",
                                         objecttemp.FIN_LETT_NO,
                                         objecttemp.STMT_STMT_ID,
                                         query2.Select(xx => xx.UNDE_CUNI_UNIT_CODE).FirstOrDefault(),
                                         query2.Select(xx => xx.UNDE_UNIT_ROW).FirstOrDefault());

                    Db.Database.ExecuteSqlCommand(sql);
                    sql = string.Format("update trh_statement set STMT_STAT='3' where stmt_id = {0}", objecttemp.STMT_STMT_ID);
                    Db.Database.ExecuteSqlCommand(sql);

                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
                }
                else
                {
                    //objecttemp.FINY_FINY_YEAR = "سال";
                    //cntx.PRN_PAY_DRAFT.Add(objecttemp);
                    //cntx.SaveChanges();
                    //return Json(new { Success = false, ErrorMessage = "Login Failed" }, JsonRequestBehavior.DenyGet);
                    if (price > end_amnt) { msg = "مبلغ وارد شده از مبلغ نهایی قرارداد بیشتر است"; };
                    if (price != query.Select(xx => xx.AMNT).Sum()) { msg = "مبلغ ثبت شده بایستی با مبلغ کارکرد جاری یکسان باشد "; };
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = msg }.ToJson();
                }
                // return Json(new { Success = "False" }, JsonRequestBehavior.DenyGet);
                //return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مبلغ ثبت شده بایستی با مبلغ کارکرد جاری یکسان باشد " }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات" }.ToJson();
            }
        }

        [EntityAuthorize("PRN_PAY_DRAFT > select,insert")]
        public ActionResult insert_pref(PRN_PAY_DRAFT objecttemp)
        {
            objecttemp.FIN_LETT_NO = ((Db.Database.SqlQuery<int>("select max(to_number(FIN_LETT_NO)) FIN_LETT_NO from prn_pay_draft").FirstOrDefault()) + 1).ToString();
            objecttemp.CRET_BY = orclname;
            objecttemp.CRET_DATE = DateTime.Today;
            objecttemp.FINY_FINY_YEAR_R = Request.Form["FINY_FINY_YEAR_R"];
            objecttemp.EXTR_TAX = Request.Form["EXTR_TAX"];
            objecttemp.HAVA_TYPE = Request.Form["HAVA_TYPE"];
            Db.PRN_PAY_DRAFT.Add(objecttemp);
            Db.SaveChanges();
            this.StartProcess("PFLW_PREF", "حواله 21 قرارداد", "ثبت حواله 21 قرارداد", objecttemp.FIN_LETT_NO);
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }


        [EntityAuthorize("TRH_STATEMENT > select,insert | TRH_STATEMENT_ROW > select,insert | PDF_TECH_DOC_ROW > select")]
        public ActionResult insert_statement(TRH_STATEMENT objecttemp)
        {
            var maxValue = Db.TRH_STATEMENT.Where(x => x.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Max(x => x.STMT_NO);
            var maxtype = Db.TRH_STATEMENT.Where(x => x.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO)
                                          .Where(x => x.STMT_NO == maxValue)
                                          .Select(x => x.STMT_TYPE).FirstOrDefault();
            var draft = Db.PRN_PAY_DRAFT.Where(xx => xx.STMT_STMT_ID == Db.TRH_STATEMENT.Where(x => x.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO)
                                        .Where(x => x.STMT_NO == maxValue)
                                        .Select(x => x.STMT_ID).FirstOrDefault()).Select(xx => xx.FIN_LETT_NO).FirstOrDefault();
            objecttemp.STMT_STAT = "0";
            objecttemp.STMT_TYPE = Request.Form["STMT_TYPE"];
            //objecttemp.STMT_TYPE = Request.Form["STMT_TYPE"];
            var end_lett = Db.CGT_LETTER.Where(x => x.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO)
                                        .Where(x => x.LET_TYPE == "17")
                                        .Select(x => x.LET_CODE).FirstOrDefault();
            if (maxtype == null) maxtype = "0";
            if ((maxtype == "0") && (end_lett == 0) && ((draft != null || maxValue == null)))
            {
                var q = from b in Db.PDF_TECH_DOC_ROW
                        where (b.TCDC_TCDC_CODE == b.PDF_TECH_DOC.TCDC_CODE &&
                               b.PDF_TECH_DOC.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO)
                        select b;

                var stst_row = new TRH_STATEMENT_ROW();
                if (maxValue == null)
                {
                    objecttemp.STMT_NO = 1;
                    Db.TRH_STATEMENT.Add(objecttemp);
                    Db.SaveChanges();
                    //this.StartProcess("PFLW_LEST", "صورت وضعیت", "گردش صورت وضعیت", objecttemp.STMT_ID);
                    foreach (var item in q)
                    {
                        stst_row.TCDR_TCDR_ROW = item.TCDR_ROW;
                        stst_row.STMT_STMT_ID = objecttemp.STMT_ID;
                        stst_row.TCDR_TCDC_TCDC_CODE = item.PDF_TECH_DOC.TCDC_CODE;
                        stst_row.STMR_PRICE = 0;
                        stst_row.STMR_AMNT = 0;
                        Db.TRH_STATEMENT_ROW.Add(stst_row);
                        Db.SaveChanges();
                        //stst_row.PRCL_PRCL_ID=item.ite
                        // item.PDF_TECH_DOC
                    }
                }
                else
                {
                    var query = from b in Db.TRH_STATEMENT
                                join c in Db.TRH_STATEMENT_ROW on b.STMT_ID equals c.STMT_STMT_ID
                                where (b.STMT_ID == c.STMT_STMT_ID && b.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO && b.STMT_NO == maxValue)
                                select c;

                    objecttemp.STMT_NO = Convert.ToInt16(Convert.ToInt16(maxValue) + 1);
                    var STMT_NO = from b in Db.TRH_STATEMENT
                                  where (b.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO && b.STMT_NO == objecttemp.STMT_NO)
                                  select b;

                    if (!STMT_NO.Any())
                    {
                        Db.TRH_STATEMENT.Add(objecttemp);
                        Db.SaveChanges();

                        this.StartProcess("PFLW_LEST", "صورت وضعیت", "گردش صورت وضعیت", objecttemp.STMT_ID);

                        foreach (var item in query)
                        {
                            stst_row.TCDR_TCDR_ROW = item.TCDR_TCDR_ROW;
                            stst_row.STMT_STMT_ID = objecttemp.STMT_ID;
                            stst_row.TCDR_TCDC_TCDC_CODE = item.TCDR_TCDC_TCDC_CODE;
                            stst_row.STMR_AMNT = item.STMR_AMNT;
                            stst_row.PRCL_PRCL_ID = item.PRCL_PRCL_ID;
                            stst_row.STMR_PRICE = 0;
                            stst_row.STMR_AMNT = 0;
                            Db.TRH_STATEMENT_ROW.Add(stst_row);
                            Db.SaveChanges();
                            //stst_row.PRCL_PRCL_ID=item.ite
                            // item.PDF_TECH_DOC
                        }
                    }
                }

                return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
            }
            //return Json(false, JsonRequestBehavior.AllowGet);
            //return Json(new { Success = "False" });
            var result = new { Success = "False", Message = "Error Message" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //STMR_ID                
        //STMT_STMT_ID           
        //PRCL_PRCL_ID           
        //TCDR_TCDC_TCDC_CODE    
        //TCDR_TCDR_ROW          
        //STMR_AMNT              
        //STMR_PRICE    

        [EntityAuthorize("TRH_CHAPTER > insert")]
        public ActionResult insert_chapter(TRH_CHAPTER objecttemp)
        {
            objecttemp.CHAP_STAT = "1";
            objecttemp.WORK_WORK_ID = Convert.ToInt16(Request.Form["WORK_ID"]);
            if (PublicRepository.ExistModel("TRH_CHAPTER", "(DEL_DUMP_U(CHAP_DESC) = DEL_DUMP_U('{0}') or DEL_DUMP_U(CHAP_CODE) = DEL_DUMP_U('{1}')) and work_work_id={2}", objecttemp.CHAP_DESC, objecttemp.CHAP_CODE, objecttemp.WORK_WORK_ID))
            {
                return new ServerMessages(ServerOprationType.Failure)
                {
                    ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد"
                }.ToJson();
            }
            else
            {
                Db.TRH_CHAPTER.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}]  ثبت شد ", objecttemp.CHAP_DESC) }.ToJson();
                //return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
            }

        }

        [EntityAuthorize("PRN_PDRF_REFERENCE > insert")]
        public ActionResult insert_prn_reference(PRN_PDRF_REFERENCE objecttemp)
        {
            //int SSTO_STOR_CODE, TRAN_NO, TRAN_TYPE, TRNA_YEAR;
            string code = Request.Form["STRN_TRAN_NO"];
            if (code != null)
            {
                var val = code.Split('-');
                objecttemp.STRN_SSTO_STOR_CODE = short.Parse(val[0].ToString());
                objecttemp.STRN_TRAN_NO = short.Parse(val[1].ToString());
                objecttemp.STRN_TRAN_YEAR = val[2].ToString();
                objecttemp.STRN_TRAN_TYPE = val[3].ToString();
            }
            Db.PRN_PDRF_REFERENCE.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("TRH_ITEM > insert")]
        public ActionResult insert_item(TRH_ITEM objecttemp)
        {
            objecttemp.ITEM_STAT = "1";
            objecttemp.CHAP_CHAP_ID = Convert.ToInt16(Request.Form["CHAP_ID"]);
            int work_id = Convert.ToInt32(Request.Form["WORK_ID"]);
            string SUNM_ID = Request.Form["SUNM_ID"];

            if (SUNM_ID == "")
            {
                objecttemp.SUNM_UNIT_CODE = null;
            }
            else
            {
                objecttemp.SUNM_UNIT_CODE = Convert.ToInt16(SUNM_ID);
            }

            if (PublicRepository.ExistModel("TRH_ITEM", "chap_chap_id  in (select chap_id from trh_chapter  where work_work_id={2} and TRH_ITEM.CHAP_CHAP_ID=CHAP_ID) and ITEM_DESC is not null and ITEM_CODE is not null and (DEL_DUMP_U(ITEM_DESC) = DEL_DUMP_U('{0}') or DEL_DUMP_U(ITEM_CODE) = DEL_DUMP_U('{1}'))", objecttemp.ITEM_DESC, objecttemp.ITEM_CODE, work_id))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
            }
            else
            {
                Db.TRH_ITEM.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}]  ثبت شد ", objecttemp.ITEM_DESC) }.ToJson();
                //return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
            }
        }

        [EntityAuthorize("TRH_INDEX > insert")]
        public ActionResult insert_index(TRH_INDEX objecttemp)
        {
            objecttemp.CHAP_CHAP_ID = Convert.ToInt16(Request.Form["CHAP_ID"]);
            objecttemp.FINY_FINY_YEAR = Request.Form["year"];
            Db.TRH_INDEX.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("PRN_COMMENT > insert")]
        public ActionResult insert_prn_comment(PRN_COMMENT objecttemp)
        {
            Db.PRN_COMMENT.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }


        [EntityAuthorize("PRN_FACTORS > select,insert")]
        public ActionResult insert_factors(PRN_FACTORS objecttemp)
        {
            objecttemp.FACT_CODE = Convert.ToInt16((Db.PRN_FACTORS.Select(oo => oo.FACT_CODE).Max()) + 1);
            //objecttemp.FACT_AMNT = Convert.ToInt16(objecttemp.FACT_AMNT);
            // cntx.PRN_FACTORS.Add(objecttemp);
            // cntx.SaveChanges();
            if (PublicRepository.ExistModel("PRN_FACTORS", "FACT_DESC is not null and DEL_DUMP_U(FACT_DESC) = DEL_DUMP_U('{0}')", objecttemp.FACT_DESC))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
            }
            else
            {
                objecttemp.FACT_SOURCE = Request.Form["FACT_SOURCE2"];
                objecttemp.FACT_BLNC = Request.Form["FACT_BLNC2"];
                string sql = string.Format("insert into PRN_FACTORS (FACT_CODE,FACT_BLNC,fact_source,FACT_DESC,fact_amnt) values ({0},'{1}','{2}','{3}',{4}) ", objecttemp.FACT_CODE, objecttemp.FACT_BLNC, objecttemp.FACT_SOURCE, objecttemp.FACT_DESC, objecttemp.FACT_AMNT);
                Db.Database.ExecuteSqlCommand(sql);
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}]  ثبت شد ", objecttemp.FACT_DESC) }.ToJson();
                //return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
            }
        }

        [EntityAuthorize("PRN_CATEGORIES > insert")]
        public ActionResult insert_prn_category(PRN_CATEGORIES objecttemp)
        {
            objecttemp.CATG_STAT = "1";
            objecttemp.CATG_TYPE = Request.Form["CATG_TYPE"];
            if (PublicRepository.ExistModel("PRN_CATEGORIES", "CATG_NAME is not null and (DEL_DUMP_U(CATG_NAME) = DEL_DUMP_U('{0}') )", objecttemp.CATG_NAME))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
            }
            else
            {
                Db.PRN_CATEGORIES.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}]  ثبت شد ", objecttemp.CATG_NAME) }.ToJson();
                //return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
            }
        }

        [EntityAuthorize("PRN_CONTRACTOR_SUPERVISION > insert")]
        public ActionResult insert_contractor_supervision(PRN_CONTRACTOR_SUPERVISION objecttemp)
        {
            Db.PRN_CONTRACTOR_SUPERVISION.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("PRN_SUPR_CONTRACTS > insert")]
        public ActionResult insert_supr_contract(PRN_SUPR_CONTRACTS objecttemp)
        {
            Db.PRN_SUPR_CONTRACTS.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("PRN_CATEGORY_ROWS > insert")]
        public ActionResult insert_prn_category_row(PRN_CATEGORY_ROWS objecttemp)
        {
            if (PublicRepository.ExistModel("PRN_CATEGORY_ROWS", "PC_ID={1} and   ITEM_ITEM_ID is not null and DEL_DUMP_U(ITEM_ITEM_ID) = DEL_DUMP_U('{0}')", objecttemp.ITEM_ITEM_ID, objecttemp.PC_ID))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
            }
            else
            {
                Db.PRN_CATEGORY_ROWS.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ", objecttemp.CATR_DESC) }.ToJson();
                //return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
            }
        }

        [EntityAuthorize("PRN_INQUIRY_ROWS > insert")]
        public ActionResult insert_category_inquiry_row(PRN_INQUIRY_ROWS objecttemp)
        {
            int pc_id = Convert.ToInt32(Request.Form["pc_id"]);
            short inqy_id = Convert.ToInt16(Request.Form["inqy_id"]);
            var rows = (from b in Db.PRN_CATEGORY_ROWS where b.PC_ID == pc_id select b);
            foreach (var row in rows)
            {
                objecttemp.INRW_SEQ = Convert.ToInt16((Db.PRN_INQUIRY_ROWS.Where(oo => oo.INQY_ID == inqy_id).Select(oo => oo.INRW_SEQ).Max()) + 1);
                objecttemp.INRW_AMNT = row.CATG_AMNT;
                objecttemp.INRW_FEE = Convert.ToInt64(row.CATG_FEE);
                objecttemp.ITEM_ITEM_ID = row.ITEM_ITEM_ID;
                objecttemp.SUNM_UNIT_CODE = row.SUNM_UNIT_CODE;
                objecttemp.INQY_ID = inqy_id;
                Db.PRN_INQUIRY_ROWS.Add(objecttemp);
                Db.SaveChanges();
            }

            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("TRH_PRICE > select,insert")]
        public ActionResult insert_price(TRH_PRICE objecttemp)
        {
            int work_id = Convert.ToInt32(Request.Form["work_id"]);
            string year = Request.Form["year"];
            var chapter = (from b in Db.TRH_CHAPTER where b.WORK_WORK_ID == work_id select b);
            foreach (var row in chapter)
            {
                var item = (from b in Db.TRH_ITEM where b.CHAP_CHAP_ID == row.CHAP_ID select b);

                foreach (var item_row in item)
                {
                    objecttemp.PRCL_STAT = 1;
                    objecttemp.FINY_FINY_YEAR = year;
                    objecttemp.ITEM_ITEM_ID = item_row.ITEM_ID;
                    bool doesExistAlready = Db.TRH_PRICE.Where(o => o.ITEM_ITEM_ID == item_row.ITEM_ID)
                        .Where(o => o.FINY_FINY_YEAR == year)
                        .Any(o => o.ITEM_ITEM_ID == item_row.ITEM_ID);
                    if (!doesExistAlready)
                    {
                        Db.TRH_PRICE.Add(objecttemp);
                        Db.SaveChanges();
                    }
                }
            }

            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        //        public ActionResult insert_cgt_letter(CGT_LETTER objecttemp)
        //        {
        //            int wr_seqn = 0;
        //            string w_year = "";
        //            if (!string.IsNullOrEmpty(Request.Form["work_ord"]))
        //            {
        //                var val = Request.Form["work_ord"].Split('-');
        //                wr_seqn = Convert.ToInt32(val[0]);
        //                w_year = val[1].ToString();
        //            }
        //            objecttemp.CCNR_TWRO_WR_SEQN = wr_seqn;
        //            objecttemp.CCNR_TWRO_W_YEAR = w_year;
        //            objecttemp.LET_CODE= (cntx.CGT_LETTER.Select(oo => oo.LET_CODE).Max()) + 1;
        //            objecttemp.CRET_BY = "GHARB";
        //            objecttemp.LET_STAT = "5";
        //            objecttemp.CRET_DATE = DateTime.Now;


        //            cntx.CGT_LETTER.Add(objecttemp);
        //            cntx.SaveChanges();

        //            PRN_PDRF_WORK work = new PRN_PDRF_WORK();
        //            work.CLET_LET_CODE = objecttemp.LET_CODE;
        //            cntx.PRN_PDRF_WORK.Add(work);
        //            cntx.SaveChanges();

        ////select nvl(sum(nvl(AMNT,0)),0)
        ////  into v_sum_oper
        ////  from PRN_PDRF_work,PRN_PDRF_OPER
        ////  where PRN_PDRF_work.ID=PRN_PDRF_OPER.PDRW_ID
        ////  and PRN_PDRF_work.CLET_LET_CODE = :LET_CODE ;

        //            var v_amnt = from b in cntx.PRN_PDRF_OPER
        //                         join w in cntx.PRN_PDRF_WORK on b.PDRW_ID equals w.ID
        //                         where b.PDRW_ID == w.ID && w.CLET_LET_CODE==objecttemp.LET_CODE
        //                         select (b.AMNT);
        //            if (objecttemp.LET_AMNT != v_amnt.Sum())
        //            { 
        //               ///جمع مبالغ مربوط به عملیات با صورت وضعیت همخوانی ندارد

        //            }



        //            var v_count = cntx.PRN_PDRF_WORK.Where(xx => xx.CLET_LET_CODE == objecttemp.LET_CODE)
        //                .Select(xx => xx.SUBU_TYPE).Distinct().Count();
        //            if (v_count ==0) {
        //                PRN_PAY_DRAFT draft = new PRN_PAY_DRAFT();
        //                draft.FIN_LETT_NO =(( Convert.ToInt32(cntx.PRN_PAY_DRAFT.Select(oo => oo.FIN_LETT_NO).Max()) )+ 1).ToString();
        //                draft.STST_PRIC = objecttemp.LET_AMNT;
        //                draft.PDRF_AMNT = objecttemp.LET_AMNT;
        //                draft.ADPY_STAT = "1";
        //                draft.PDRF_TYPE = objecttemp.LET_TYPE;
        //                draft.CCNT_CNTR_NO = objecttemp.CCNT_CNTR_NO;
        //                draft.ADST_NUM =Convert.ToInt16( cntx.PRN_PAY_DRAFT.Where(oo=>oo.CCNT_CNTR_NO==objecttemp.CCNT_CNTR_NO)
        //                    //.Where()
        //                    .Select(oo => oo.ADST_NUM).Max() + 1);
        //                draft.EXTR_TAX = "1";
        //                draft.CLET_LET_CODE = objecttemp.LET_CODE;
        //                draft.ADST_DAY = objecttemp.LET_DAY;
        //                draft.ADST_MONT = objecttemp.LET_MNT;
        //                draft.ADST_YEAR = objecttemp.LET_YEAR;
        //                draft.FINY_FINY_YEAR = Request.Form["YEAR"];
        //                draft.CRET_BY = "gharb";
        //                draft.CRET_DATE = DateTime.Now;
        //                draft.TWRO_WR_SEQN = objecttemp.CCNR_TWRO_WR_SEQN;
        //                draft.TWRO_W_YEAR = objecttemp.CCNR_TWRO_W_YEAR;
        //                cntx.PRN_PAY_DRAFT.Add(draft);
        //                cntx.SaveChanges();
        //            }
        //            return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
        //        }


        [EntityAuthorize("CGT_LETTER > select,insert")]
        public ActionResult insert_cgt_letter(CGT_LETTER objecttemp)
        {
            // objecttemp.LET_CODE = (cntx.CGT_LETTER.Select(oo => oo.LET_CODE).Max()) + 1;
            // objecttemp.CRET_BY = "BANDAR";
            objecttemp.LET_STAT = "3";
            // objecttemp.CRET_DATE = DateTime.Now;
            objecttemp.LET_KIND = Request.Form["LET_KIND"];
            objecttemp.LET_TYPE = Request.Form["LET_TYPE"];

            if (PublicRepository.ExistModel("CGT_LETTER", "DEL_DUMP_U(LET_TYPE) = DEL_DUMP_U('{0}') and DEL_DUMP_U(LET_NUMBER) = DEL_DUMP_U('{1}') and CCNT_CNTR_NO='{2}'", objecttemp.LET_TYPE, objecttemp.LET_NUMBER, objecttemp.CCNT_CNTR_NO))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
            }
            else
            {
                Db.CGT_LETTER.Add(objecttemp);
                Db.SaveChanges();
                //switch (objecttemp.LET_TYPE)
                //{
                //    case ("19"):
                //        {
                //            this.StartProcess("PFLW_LETA", "تعلیق", "ثبت نامه تعلیق", objecttemp.LET_CODE);
                //        }
                //        break;
                //    case ("4"):
                //        {
                //            this.StartProcess("PFLW_LEAL", "علی الحساب", "ثبت نامه علی الحساب", objecttemp.LET_CODE);
                //        }
                //        break;
                //    case ("5"):
                //        {
                //            this.StartProcess("PFLW_LEPA", "پیش پرداخت-"+objecttemp.LET_DESC, "پیش پرداخت", objecttemp.LET_CODE);
                //        }
                //        break;

                //    case ("12"):
                //        {
                //            this.StartProcess("PFLW_LEBA", "تعدیل", "تعدیل", objecttemp.LET_CODE);
                //        }
                //        break;

                //}
                //  this.StartProcess("PFLW_LEBA", "نامه تعدیل/علی الحساب/پیش پرداخت", "ثبت نامه  تعدیل/علی الحساب/پیش پرداخت", objecttemp.LET_CODE);
                //return Json(new { Success = "True" }, JsonRequestBehavior.AllowGet);
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();

            }

            //return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
        }

        [EntityAuthorize("CGT_LETTER > select,insert")]
        public ActionResult insert_cgt_letter_extn(CGT_LETTER objecttemp)
        {
            try
            {
                // objecttemp.LET_CODE = (cntx.CGT_LETTER.Select(oo => oo.LET_CODE).Max()) + 1;
                //  objecttemp.CRET_BY = "GHARB";
                objecttemp.LET_STAT = "5";
                //   objecttemp.CRET_DATE = DateTime.Now;
                objecttemp.LET_KIND = "1";
                objecttemp.LET_TYPE = "18";
                objecttemp.LET_AMNT = 0;

                if (PublicRepository.ExistModel("CGT_LETTER", "DEL_DUMP_U(LET_TYPE) = DEL_DUMP_U('{0}') and DEL_DUMP_U(LET_NUMBER) = DEL_DUMP_U('{1}')", objecttemp.LET_TYPE, objecttemp.LET_NUMBER))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
                }
                else
                {
                    Db.CGT_LETTER.Add(objecttemp);
                    Db.SaveChanges();
                    this.StartProcess("PFLW_LEEX", "نامه تمدید", "ثبت نامه تمدید", objecttemp.LET_CODE);
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
                }
            }
            catch (DbEntityValidationException ex)
            {
                string errMsg = string.Empty;
                foreach (var item in ex.EntityValidationErrors)
                {
                    foreach (var i in item.ValidationErrors)
                    {
                        errMsg += (i.ErrorMessage + "\n");
                    }
                }
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errMsg }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        [EntityAuthorize("CGT_LETTER > select,insert")]
        public ActionResult insert_cgt_letter_inc(CGT_LETTER objecttemp)
        {
            try
            {
                objecttemp.LET_STAT = "3";
                objecttemp.LET_KIND = "1";
                objecttemp.LET_DESC = "افزایش و کاهش";
                objecttemp.LET_TYPE = "16";
                string month = string.Empty, day = string.Empty;
                int intmonth = pc.GetMonth(thisDate);
                int intday = pc.GetDayOfMonth(thisDate);
                if (intmonth < 10 && intmonth > 0)
                {
                    month = "0" + intmonth.ToString();
                }
                else { month = Convert.ToString(intmonth); };
                if (intday < 10 && intday > 0)
                {
                    day = "0" + intday.ToString();
                }
                else { day = Convert.ToString(intday); };
                objecttemp.LET_DAY = day;
                objecttemp.LET_MNT = month;
                objecttemp.LET_YEAR = Convert.ToString(pc.GetYear(thisDate));
                int type = Convert.ToInt32(Request.Form["TYPE"]);
                objecttemp.LET_AMNT = type * objecttemp.LET_AMNT;

                if (PublicRepository.ExistModel("CGT_LETTER", "DEL_DUMP_U(LET_TYPE) = DEL_DUMP_U('{0}') and DEL_DUMP_U(LET_NUMBER) = DEL_DUMP_U('{1}')", objecttemp.LET_TYPE, objecttemp.LET_NUMBER))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
                }
                else
                {
                    Db.CGT_LETTER.Add(objecttemp);
                    Db.SaveChanges();
                    this.StartProcess("PFLW_LEIN", "نامه افزایش و کاهش", "ثبت نامه افزایش و کاهش", objecttemp.LET_CODE);
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
                }
            }
            catch (DbEntityValidationException ex)
            {
                string errMsg = string.Empty;
                foreach (var item in ex.EntityValidationErrors)
                {
                    foreach (var i in item.ValidationErrors)
                    {
                        errMsg += (i.ErrorMessage + "\n");
                    }
                }
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errMsg }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        [EntityAuthorize("CGT_LETTER > select,insert")]
        public ActionResult insert_cgt_letter_end(CGT_LETTER objecttemp)
        {
            try
            {
                string month = string.Empty, day = string.Empty;
                int intmonth = pc.GetMonth(thisDate);
                int intday = pc.GetDayOfMonth(thisDate);
                if (intmonth < 10 && intmonth > 0)
                {
                    month = "0" + intmonth.ToString();
                }
                else { month = Convert.ToString(intmonth); };
                if (intday < 10 && intday > 0)
                {
                    day = "0" + intday.ToString();
                }
                else { day = Convert.ToString(intday); };

                //  objecttemp.LET_CODE = (cntx.CGT_LETTER.Select(oo => oo.LET_CODE).Max()) + 1;
                //  objecttemp.CRET_BY = "GHARB";
                objecttemp.LET_STAT = "5";
                //  objecttemp.CRET_DATE = DateTime.Now;
                objecttemp.LET_KIND = "1";
                objecttemp.LET_TYPE = "17";
                objecttemp.LET_DAY = day;
                objecttemp.LET_MNT = month;
                objecttemp.LET_YEAR = Convert.ToString(pc.GetYear(thisDate));

                var cnt = Db.CGT_LETTER.Where(x => x.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Where(x => x.LET_TYPE == "17").Count();
                if (cnt == null) cnt = 0;
                if (PublicRepository.ExistModel("CGT_LETTER", "DEL_DUMP_U(LET_TYPE) = DEL_DUMP_U('{0}') and DEL_DUMP_U(LET_NUMBER) = DEL_DUMP_U('{1}')", objecttemp.LET_TYPE, objecttemp.LET_NUMBER))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
                }
                else
                {
                    Db.CGT_LETTER.Add(objecttemp);
                    Db.SaveChanges();
                    this.StartProcess("PFLW_LETT", "نامه خاتمه", "ثبت نامه خاتمه ", objecttemp.LET_CODE);
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
                }
            }
            catch (DbEntityValidationException ex)
            {
                string errMsg = string.Empty;
                foreach (var item in ex.EntityValidationErrors)
                {
                    foreach (var i in item.ValidationErrors)
                    {
                        errMsg += (i.ErrorMessage + "\n");
                    }
                }
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errMsg }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        [EntityAuthorize("PDF_TECH_DOC > insert")]
        public ActionResult insert_tech_doc(PDF_TECH_DOC objecttemp)
        {
            objecttemp.TCDC_STAT = "0";
            Db.PDF_TECH_DOC.Add(objecttemp);
            Db.SaveChanges();
            AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
            wp.StartProcess(HttpContext.User.Identity.Name, null, string.Format("برسی درخواست شماره {0}", objecttemp.TCDC_CODE), objecttemp.EQPD_DESC, 63, objecttemp.TCDC_CODE);
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("TRH_PRICE > select | TRH_STATEMENT > select | TRH_STATEMENT_ROW > select,insert")]
        public ActionResult insert_statement_price(TRH_STATEMENT_ROW objecttemp)
        {
            objecttemp.STMT_STMT_ID = Convert.ToInt32(Request.Form["STMT_STMT_ID"]);
            int item_id = Convert.ToInt32(Request.Form["ITEM_ID"]);
            decimal? ditem_id = Convert.ToDecimal(Request.Form["ITEM_ID"]);

            int price = Convert.ToInt32(Db.TRH_PRICE.Where(x => x.PRCL_ID == objecttemp.PRCL_PRCL_ID).Select(x => x.PRICE).FirstOrDefault());
            string cntr_no = Db.TRH_STATEMENT.Where(xx => xx.STMT_ID == objecttemp.STMT_STMT_ID).Select(xx => xx.CCNT_CNTR_NO).FirstOrDefault();
            decimal end_amnt = Db.Database.SqlQuery<decimal>(string.Format("SELECT nvl(PRN_REMAIN_AMNT_U('{0}'),0) FROM DUAL", cntr_no)).FirstOrDefault();

            objecttemp.STMR_PRICE = objecttemp.STMR_AMNT * price;
            Db.TRH_STATEMENT_ROW.Add(objecttemp);
            var query = from b in Db.TRH_STMR_V where (b.STMT_ID == objecttemp.STMT_STMT_ID && b.ITEM_ID == ditem_id) select b;
            if (query.Any())
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
            }
            else
            {
                decimal? sum_price = Db.TRH_STATEMENT_ROW.Where(xx => xx.STMT_STMT_ID == objecttemp.STMT_STMT_ID).Select(xx => xx.STMR_PRICE).Sum();
                if (objecttemp.STMR_PRICE <= end_amnt)
                {
                    //      if (sum_price + objecttemp.STMR_PRICE <= end_amnt)
                    //     {
                    Db.SaveChanges();
                    //                       
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
                    //   }
                }

                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مبلغ وارد شده بیشتر از سقف قرارداد می باشد" }.ToJson();
            }
            // return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("PRN_CUR_FACT_AMNT > insert")]
        public ActionResult insert_fact(PRN_CUR_FACT_AMNT objecttemp)
        {
            string work_id = Request.Form["WORK_WORK_ID"];
            if (!string.IsNullOrEmpty(work_id))
                objecttemp.WORK_WORK_ID = Convert.ToInt16(Request.Form["WORK_WORK_ID"]);
            if (objecttemp.SCOP != "5")
                objecttemp.WORK_WORK_ID = null;
            Db.PRN_CUR_FACT_AMNT.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("PRN_WAGES > select | PRN_SUPERVISION > insert")]
        public ActionResult insert_stmr_supr(PRN_SUPERVISION objecttemp)
        {
            objecttemp.AQNT = Db.PRN_WAGES.Where(xx => xx.WAGE_ROW == objecttemp.WAGE_WAGE_ROW).Select(xx => xx.AQNT).FirstOrDefault();
            Db.PRN_SUPERVISION.Add(objecttemp);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("PDF_STANDARD_ACTIVITY > insert,select")]
        public ActionResult insert_standard(PDF_STANDARD_ACTIVITY objecttemp)
        {
            decimal? wight = 0;
            decimal? masterwight = 0;
            bool check = false;
            string msg = "";

            if (PublicRepository.ExistModel("PDF_STANDARD_ACTIVITY", "DEL_DUMP_U(STND_DESC) = DEL_DUMP_U('{0}') ", objecttemp.STND_DESC))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
            }
            else
            {
                if (objecttemp.STND_LEVEL == 1)
                {
                    wight = Db.PDF_STANDARD_ACTIVITY.Where(xx => xx.CKPR_KPRJ_ROW == objecttemp.CKPR_KPRJ_ROW)
                                                    .Where(xx => xx.STND_LEVEL == 1)
                                                    .Select(xx => xx.WGHT).Sum();

                    if ((wight == null ? 0 : wight) + (objecttemp.WGHT == null ? 0 : objecttemp.WGHT) <= 100)
                    {
                        objecttemp.STND_ST_ID = null;
                        Db.PDF_STANDARD_ACTIVITY.Add(objecttemp);
                        Db.SaveChanges();
                        msg = "اطلاعات با موفقیت ثبت شد";
                        check = true;
                    }
                }
                else
                {
                    masterwight = Db.PDF_STANDARD_ACTIVITY.Where(xx => xx.ST_ID == objecttemp.STND_ST_ID).Select(xx => xx.WGHT).FirstOrDefault();

                    wight = Db.PDF_STANDARD_ACTIVITY.Where(xx => xx.STND_ST_ID == objecttemp.STND_ST_ID).Select(xx => xx.WGHT).Sum() == null ?
                            0 :
                            Db.PDF_STANDARD_ACTIVITY.Where(xx => xx.STND_ST_ID == objecttemp.STND_ST_ID).Select(xx => xx.WGHT).Sum();
                    if (masterwight >= wight + objecttemp.WGHT)
                    {
                        Db.PDF_STANDARD_ACTIVITY.Add(objecttemp);
                        Db.SaveChanges();
                        check = true;
                        msg = "اطلاعات با موفقیت ثبت شد";
                    }
                }

                if (check == true)
                    return new ServerMessages(ServerOprationType.Success) { Message = msg }.ToJson();
                else if (objecttemp.STND_LEVEL == 1)
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "  خطا! مجموع وزن های وارد شده بیشتر از 100 است  " }.ToJson();
                else
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "  خطا! مجموع وزن های وارد شده بیشتر از وزن های سطح قبل شده است  " }.ToJson();
            }
        }

        [EntityAuthorize("PDF_TECH_DOC_ROW > insert,select | TRH_PRICE > select")]
        public ActionResult insert_tech_doc_row(PDF_TECH_DOC_ROW objecttemp)
        {
            int save = 0;
            if (objecttemp.FACT == null) objecttemp.FACT = 1;
            objecttemp.TCDC_TCDC_CODE = Convert.ToInt32(Request.Form["TCDC_CODE"]);
            objecttemp.TCDR_TYPE = "0";
            string TCDR_AMNT2 = Request.Form["TCDR_AMNT2"];
            if (string.IsNullOrEmpty(TCDR_AMNT2))
            {
                long? price = Db.TRH_PRICE.Where(xx => xx.PRCL_ID == objecttemp.PRCL_PRCL_ID).Select(xx => xx.PRICE).FirstOrDefault();
                objecttemp.TCDR_PRICE = objecttemp.TCDR_AMNT * price * objecttemp.FACT;
                objecttemp.TCDR_FEE = price;
                objecttemp.TCDR_TYPE = "0";
                objecttemp.TCDR_TYPE = "0";
                objecttemp.ITEM_ITEM_ID = Convert.ToInt32(Request.Form["ITEM_ID"]);
                objecttemp.TCDR_CODE = Request.Form["CHAP_ID"] + "-" + Request.Form["ITEM_ID"];
                Db.PDF_TECH_DOC_ROW.Add(objecttemp);

                var query = from b in Db.PDF_TECH_DOC_ROW where (b.PRCL_PRCL_ID == objecttemp.PRCL_PRCL_ID && b.TCDC_TCDC_CODE == objecttemp.TCDC_TCDC_CODE) select b;
                if (!query.Any())
                {
                    save = Db.SaveChanges();
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
                }
                else
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
                }
            }
            else
            {
                objecttemp.TCDR_CODE = Request.Form["TCDR_CODE"];
                objecttemp.ITEM_ITEM_ID = Convert.ToInt32(Request.Form["ITEM_ID2"]);
                objecttemp.TCDR_AMNT = Convert.ToDecimal(Request.Form["TCDR_AMNT2"]);
                if (!string.IsNullOrEmpty(Request.Form["FACT2"]))
                    objecttemp.FACT = Convert.ToDecimal(Request.Form["FACT2"]);

                if (objecttemp.FACT == null)
                    objecttemp.FACT = 1;
                objecttemp.TCDR_PRICE = objecttemp.TCDR_AMNT * objecttemp.TCDR_FEE * objecttemp.FACT;
                Db.PDF_TECH_DOC_ROW.Add(objecttemp);

                var query = from b in Db.PDF_TECH_DOC_ROW
                            join p in Db.TRH_PRICE on b.PRCL_PRCL_ID equals p.PRCL_ID
                            where (p.ITEM_ITEM_ID == objecttemp.ITEM_ITEM_ID && b.TCDC_TCDC_CODE == objecttemp.TCDC_TCDC_CODE)
                            select b;

                var query2 = from b in Db.PDF_TECH_DOC_ROW
                             where (b.ITEM_ITEM_ID == objecttemp.ITEM_ITEM_ID && b.TCDC_TCDC_CODE == objecttemp.TCDC_TCDC_CODE)
                             select b;

                if (!query.Any() && !query2.Any())
                {
                    save = Db.SaveChanges();
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
                }
            }

            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات ثبت شده تکراری می باشد" }.ToJson();
        }

        ////////////////////////////////////////////////////////////////////GRID
        [EntityAuthorize("TRH_INDEX > select")]
        public ActionResult Get_TRH_INDEX([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from b in Db.TRH_INDEX orderby b.INDX_ID select b).Select(p => new
            {
                p.INDX_ID,
                p.INDX_LEVEL,
                p.INDX_VALUE,
                p.FINY_FINY_YEAR,
                p.CHAP_CHAP_ID
            });

            var d = new DataSourceResult
            {
                Data = query
            };
            return Json(d);
        }

        [EntityAuthorize("PRN_SUPERVISION > select")]
        public ActionResult Get_stmr_supr([DataSourceRequest] DataSourceRequest request)
        {
            // q = Request.Form["q"];
            var query = (from o in Db.PRN_SUPERVISION
                             //where (o.STMR_STMR_ID == stmr_id)
                         orderby o.SUPV_ROW
                         select o).Select(o => new
                         {
                             o.SUPV_ROW,
                             o.HOUR,
                             o.DAY,
                             o.AQNT,
                             JOB_DESC = o.PRN_JOB.JOB_DESC,
                             AMNT = o.PRN_WAGES.AMNT
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_PDRF_WORK > select | PRN_PDRF_OPER > select")]
        public ActionResult get_pdrf_oper([DataSourceRequest] DataSourceRequest request, string cntr_no, int stmt_id)
        {
            // q = Request.Form["q"];
            if (cntr_no == null)
                cntr_no = "0";

            var query = (from b in Db.PRN_PDRF_WORK
                         join o in Db.PRN_PDRF_OPER on b.ID equals o.PDRW_ID
                         where (b.STST_CCNT_CNTR_NO == cntr_no && b.STMT_STMT_ID == stmt_id && o.PDRW_ID == b.ID)
                         orderby o.ID
                         select o).Select(o => new
                         {
                             o.ID,
                             o.TWRO_W_YEAR,
                             o.TWRO_WR_SEQN,
                             o.SUBU_TYPE,
                             o.AMNT,
                             o.COPE_OPRN_CODE,
                             oprn_desc = o.PCT_OPERATION.OPRN_DESC,
                             o.PDRW_ID
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }
        public string GetExpfDesc(string ASST_ASS_CODE, string ASST_MAIN_MAIN_CODE, string BEXF_EXPF_CODE)
        {
            return (Db.CGT_ASCR_V.Where(xx => xx.ASS_CODE == ASST_ASS_CODE && xx.MAIN_CODE == ASST_MAIN_MAIN_CODE && xx.EXPF_CODE == BEXF_EXPF_CODE).Select(xx => xx.EXPF_DESC).FirstOrDefault());


        }

        public string GetLocation(string GEOL_G_CODE)
        {
            return (Db.BKP_GEOGH_LOC.Where(xx => xx.G_CODE == GEOL_G_CODE).Select(xx => xx.G_DESC).FirstOrDefault());


        }
        [EntityAuthorize("PRN_PDRF_WORK > select | PRN_PDRF_OPER > select")]
        public ActionResult get_pdrf_oper_all([DataSourceRequest] DataSourceRequest request, string cntr_no)
        {
            // q = Request.Form["q"];
            if (cntr_no == null)
                cntr_no = "0";

            var query = (from b in Db.PRN_PDRF_WORK
                         join o in Db.PRN_PDRF_OPER on b.ID equals o.PDRW_ID
                         where (b.STST_CCNT_CNTR_NO == cntr_no && o.PDRW_ID == b.ID)
                         orderby o.ID
                         select o).Select(o => new
                         {
                             o.ID,
                             o.TWRO_W_YEAR,
                             o.TWRO_WR_SEQN,
                             o.SUBU_TYPE,
                             o.AMNT,
                             o.COPE_OPRN_CODE,
                             oprn_desc = o.PCT_OPERATION.OPRN_DESC,
                             o.PDRW_ID
                         });
            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult get_oper(string KPRJ_ROW)
        {
            var RetVal = (from b in Db.PCT_OPERATION
                          join c in Db.CGT_KPRO_OPER on b.OPRN_CODE equals c.COPE_OPRN_CODE
                          where c.KPOP_TYPE == KPRJ_ROW
                          orderby b.OPRN_DESC
                          select new { b.OPRN_DESC, b.OPRN_CODE }).ToList();

            // var RetVal = (from b in cntx.CGT_KPRO orderby b.KPRJ_DESC select new { b.KPRJ_ROW, b.KPRJ_DESC }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_standard_master_dp(int? KPRJ_ROW)
        {
            var RetVal = (from b in Db.PDF_STANDARD_ACTIVITY where b.CKPR_KPRJ_ROW == KPRJ_ROW && b.STND_ST_ID == null orderby b.STND_DESC select new { b.ST_ID, b.STND_DESC }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("PRN_TRANSCRIPT > select ")]
        public ActionResult get_transcript([DataSourceRequest] DataSourceRequest request, string PDRF_FIN_LETT_NO)
        {
            // q = Request.Form["q"];
            if (PDRF_FIN_LETT_NO == null) PDRF_FIN_LETT_NO = "0";
            var query = (from b in Db.PRN_TRANSCRIPT
                         where (b.PDRF_FIN_LETT_NO == PDRF_FIN_LETT_NO)
                         orderby b.TRNS_ROW
                         select b).Select(b => new
                         {
                             b.TRNS_ROW,
                             b.TRNS_DESC,
                             b.PDRF_FIN_LETT_NO
                         });
            return Json(query.ToDataSourceResult(request));
        }
        public ActionResult get_trans([DataSourceRequest] DataSourceRequest request, string PDRF_FIN_LETT_NO)
        {
            // q = Request.Form["q"];
            if (PDRF_FIN_LETT_NO == null) PDRF_FIN_LETT_NO = "0";
            var query = (from b in Db.HSB_TRAN_PDRFS

                         where (b.PDRF_FIN_LETT_NO == PDRF_FIN_LETT_NO)

                         select b).Select(b => new
                         {
                             b.STRN_TRAN_YEAR,
                             b.STRN_TRAN_NO,
                             b.STRN_TRAN_TYPE,
                             b.STRN_SSTO_STOR_CODE,
                             b.PDRF_FIN_LETT_NO,
                             b.HTPD_ID


                         });
            return Json(query.ToDataSourceResult(request));
        }
        public ActionResult Get_Trans_DP(string PDRF_FIN_LETT_NO)
        {
            //var query = (from a in Db.STR_PUR_INF
            //             join b in Db.STR_TRANSACTION on a.PUIN_NO equals b.SPIF_PUIN_NO
            //             join c in Db.PRN_PAY_DRAFT on a.PUIN_NO equals c.CCNT_CNTR_NO
            //             where c.FIN_LETT_NO == PDRF_FIN_LETT_NO && a.PUIN_TYPE=="4"
            //             orderby b.TRAN_DESC
            //             select b).Select(b => new
            //             {
            //                 b.TRAN_NO,
            //                 TRAN_DESC=b.TRAN_NO+"-"+b.TRAN_TYPE+"-"+b.SSTO_STOR_CODE+"-"+b.TRAN_YEAR,
            //                 b.CCNT_CNTR_NO,
            //                 TRAN_DATE = b.TRAN_DAY + "/" + b.TRAN_MONT + "/" + b.TRAN_YEAR,
            //                 b.TWRO_WR_SEQN,
            //                 b.TWRO_W_YEAR,
            //                 b.END_DAY,
            //                 b.END_MONT,
            //                 b.END_YEAR,
            //                 b.DCMT_DOC_SEQ,
            //                 b.SUM_PRIC,
            //                 b.SSTO_STOR_CODE,
            //                 b.TRAN_TYPE,


            //             });

            //var query = Db.Database.SqlQuery<STR_TRANSACTION>("select SSTO_STOR_CODE,b.TRAN_NO||'-'||b.TRAN_TYPE||'-'||b.SSTO_STOR_CODE||'-'||b.TRAN_YEAR||'--'||  b.TRAN_YEAR  || '/' || b.TRAN_MONT || '/' || b.TRAN_DAY tran_Desc,b.TRAN_NO||'-'||b.TRAN_TYPE||'-'||b.SSTO_STOR_CODE||'-'||b.TRAN_YEAR TRAN_NO" +
            //                                                " from str_pur_inf a,str_transaction b where PUIN_NO=':CNTR_NO'" +
            //                                                " and a.puin_no=b.SPIF_PUIN_NO and a.puin_type=b.SPIF_puin_type ", Db.PRN_PAY_DRAFT.Where(xx=>xx.FIN_LETT_NO==PDRF_FIN_LETT_NO).Select(xx=>xx.CCNT_CNTR_NO).FirstOrDefault())
            //                                                .Select(x => new
            //                                                {
            //                                                    x.TRAN_NO,
            //                                                    x.TRAN_DESC,
            //                                                    x.SSTO_STOR_CODE,
            //                                                }).ToList();

            var query = (from a in Db.HSB_TRAN_V

                         where a.ID == Db.PRN_PAY_DRAFT.Where(xx => xx.FIN_LETT_NO == PDRF_FIN_LETT_NO).Select(xx => xx.CCNT_CNTR_NO).FirstOrDefault()
                         orderby a.TRAN_DESC
                         select a).Select(a => new
                         {
                             a.TRAN_NO,
                             a.TRAN_DESC


                         });
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("TRH_STATEMENT > select | TRH_STATEMENT_ROW > select")]
        public ActionResult get_statement([DataSourceRequest] DataSourceRequest request, string cntr_no, string stat)
        {
            // q = Request.Form["q"];
            //int price1 = 0, price2 = 0;
            if (cntr_no == null) cntr_no = "0";
            if (stat == null) stat = "0";
            var query = (from b in Db.TRH_STATEMENT
                         where (b.CCNT_CNTR_NO == cntr_no && stat == "1")
                         orderby b.STMT_NO descending
                         select b).AsEnumerable().Select(b => new
                         {
                             b.STMT_NO,
                             b.STMT_ID,
                             b.STMT_TYPE,
                             b.STMT_STAT,
                             b.STMT_DATE,
                             FIN_LETT_NO = b.PRN_PAY_DRAFT.Select(xx => xx.FIN_LETT_NO).FirstOrDefault(),
                             //
                             //price1 = cntx.PDF_TECH_DOC.Where(x=>x.CCNT_CNTR_NO==cntr_no).Select(x => x.PDF_TECH_DOC_ROW.Select(xx=>xx.TCDR_PRICE).Sum()).FirstOrDefault(),
                             //price2=cntx.TRH_STATEMENT_ROW.Where(x => x.STMT_STMT_ID == b.STMT_ID).Select(x => x.STMR_PRICE).Sum(),
                             //price = (cntx.PDF_TECH_DOC.Where(x => x.CCNT_CNTR_NO == cntr_no).Select(x => x.PDF_TECH_DOC_ROW.Select(xx => xx.TCDR_PRICE).Sum()).FirstOrDefault() == null ? 0 : cntx.PDF_TECH_DOC.Where(x => x.CCNT_CNTR_NO == cntr_no).Select(x => x.PDF_TECH_DOC_ROW.Select(xx => xx.TCDR_PRICE).Sum()).FirstOrDefault()) +
                             //(cntx.TRH_STATEMENT_ROW.Where(x => x.STMT_STMT_ID == b.STMT_ID).Select(x => x.STMR_PRICE).Sum() == null ? 0 : cntx.TRH_STATEMENT_ROW.Where(x => x.STMT_STMT_ID == b.STMT_ID).Select(x => x.STMR_PRICE).Sum()),
                             //b.CCNT_CNTR_NO
                             price = Db.TRH_STATEMENT_ROW.Where(x => x.STMT_STMT_ID == b.STMT_ID).Select(x => x.STMR_PRICE).Sum()
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("FND_WORK_ORD > select")]
        public ActionResult get_work_ord([DataSourceRequest] DataSourceRequest request, int wr_seqn, string year, string wr_desc)
        {
            // q = Request.Form["q"];
            if (wr_seqn == null) wr_seqn = 0;
            var query = (from b in Db.FND_WORK_ORD
                         where (b.WR_SEQN == wr_seqn && b.W_YEAR.Contains(year) && b.WR_DESC.Contains(wr_desc))
                         orderby b.WR_SEQN
                         select b).Select(b => new
                         {
                             b.WR_SEQN,
                             b.W_YEAR,
                             b.WR_DESC,
                         });

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_SUPR_CONTRACTS > select")]
        public ActionResult Get_SUPR_CONTRACT([DataSourceRequest] DataSourceRequest request, int CSPR_CCOR_CNOR_ID, int CSPR_ID)
        {
            var query = (from b in Db.PRN_SUPR_CONTRACTS
                         where (b.CSPR_ID == CSPR_ID && b.CSPR_CCOR_CNOR_ID == CSPR_CCOR_CNOR_ID)
                         select b).Select(b => new
                         {
                             b.CCNT_CNTR_NO,
                             b.CSPR_ID,
                             b.SUCT_STAT,
                             b.CSPR_CCOR_CNOR_ID,
                             //titl = b.CNT_CONTRACT.TITL,
                             b.ID
                         });

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("FND_WORK_RQ > select")]
        public ActionResult get_work([DataSourceRequest] DataSourceRequest request, int wq_seq, string year, string wq_desc)
        {
            // q = Request.Form["q"];
            if (wq_seq == null) wq_seq = 0;
            var query = (from b in Db.FND_WORK_RQ
                         where (b.WQ_SEQ == wq_seq && b.YEAR.Contains(year) && b.WQ_DESC.Contains(wq_desc))
                         orderby b.WQ_SEQ
                         select b).Select(b => new
                         {
                             b.WQ_SEQ,
                             b.YEAR,
                             b.WQ_DESC,
                         });

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_PAY_DRAFT > select")]
        public ActionResult Get_Pay_Draft([DataSourceRequest] DataSourceRequest request, string pdrf_type, string cntr_no)
        {
            // q = Request.Form["q"];
            if (string.IsNullOrEmpty(pdrf_type)) { pdrf_type = "0"; };
            if (string.IsNullOrEmpty(cntr_no)) { cntr_no = "0"; };

            var query = (from b in Db.PRN_PAY_DRAFT
                         where (b.CCNT_CNTR_NO == cntr_no && b.PDRF_TYPE == pdrf_type)
                         orderby b.FIN_LETT_NO
                         select b).Select(b => new
                         {
                             b.FIN_LETT_NO,
                             b.PDRF_AMNT,
                             adst_date = b.ADST_YEAR + "/" + b.ADST_MONT + "/" + b.ADST_DAY,
                             b.EXTR_TAX,
                             b.FINY_FINY_YEAR_R,
                             b.HAVA_TYPE
                         });
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_PAY_DRAFT > select")]
        public ActionResult Get_Pay_Draft_stmt([DataSourceRequest] DataSourceRequest request, string pdrf_type, string cntr_no)
        {
            // q = Request.Form["q"];
            if (string.IsNullOrEmpty(cntr_no)) { cntr_no = "0"; };
            var query = (from b in Db.PRN_PAY_DRAFT
                         where (b.CCNT_CNTR_NO == cntr_no)
                         orderby b.ADST_NUM descending
                         select b).Select(b => new
                         {
                             b.ADST_NUM,
                             b.STMT_STMT_ID,
                             b.FIN_LETT_NO,
                             b.PDRF_AMNT,
                             b.STST_PRIC,
                             bkp_code = b.DCMT_DOC_SEQ, //b.DCMT_FINY_FINY_YEAR + b.DCMT_BKTP_BK_CODE,// + SqlFunctions.StringConvert((double?)b.DCMT_DOC_SEQ),
                             b.PDRF_TYPE,
                             b.EXTR_TAX,
                             b.FINY_FINY_YEAR_R,
                             let_date = b.FIN_LETT_YEAR + "/" + b.FIN_LETT_MONT + "/" + b.FIN_LETT_DAY
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }
        public ActionResult Get_Contract(string text, short? PRJ_CODE, short? PLN_CODE)
        {

            var RetVal = (from b in Db.CNT_CONTRACT
                          join c in Db.CNT_CONTRACT_ORDER on b.CNTR_NO equals c.CCNT_CNTR_NO
                          join d in Db.FND_WORK_ORD on c.TWRO_WR_SEQN equals d.WR_SEQN


                          where
                            c.TWRO_W_YEAR == d.W_YEAR
                            && (b.TITL.Contains(text) || b.CNTR_NO.Contains(text) || text == null)
                            && (d.CPRO_PRJ_CODE == PRJ_CODE || PRJ_CODE == null)
                            && (d.CPRO_CPLA_PLN_CODE == PLN_CODE || PLN_CODE == null)

                          select new { b.CNTR_NO, TITL = b.TITL + "-" + b.CNTR_NO }).OrderBy(b => b.CNTR_NO);

            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("PRN_PAY_DRAFT > select")]
        public ActionResult Get_Pay_Draft_stmt2([DataSourceRequest] DataSourceRequest request, string pdrf_type, int? stmt_id)
        {
            // q = Request.Form["q"];

            var query = (from b in Db.PRN_PAY_DRAFT
                         where (b.PDRF_TYPE == pdrf_type && b.STMT_STMT_ID == stmt_id)
                         orderby b.FIN_LETT_NO
                         select b).Select(b => new
                         {
                             b.STMT_STMT_ID,
                             b.FIN_LETT_NO,
                             b.PDRF_AMNT,
                             b.STST_PRIC,
                             b.EXTR_TAX,
                             b.FINY_FINY_YEAR_R,
                             b.DCMT_DOC_SEQ
                         });
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_PAY_DRAFT > select")]
        public ActionResult Get_Pay_Draft_stmt_letter([DataSourceRequest] DataSourceRequest request, string pdrf_type, int? let_code)
        {
            // q = Request.Form["q"];
            if (string.IsNullOrEmpty(pdrf_type)) { pdrf_type = "0"; };
            var query = (from b in Db.PRN_PAY_DRAFT
                         where (b.PDRF_TYPE == pdrf_type && b.CLET_LET_CODE == let_code)
                         orderby b.FIN_LETT_NO
                         select b).Select(b => new
                         {
                             b.STMT_STMT_ID,
                             b.FIN_LETT_NO,
                             b.PDRF_AMNT,
                             b.STST_PRIC,
                             b.EXTR_TAX,
                             b.FINY_FINY_YEAR_R,
                             b.DCMT_DOC_SEQ
                         });
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("TRH_STMR_V > select")]
        public ActionResult Get_statement_row([DataSourceRequest] DataSourceRequest request, int id)
        {
            //var query = from b in cntx.PDF_TECH_DOC_ROW
            //            join p in cntx.TRH_STATEMENT_ROW on b.TCDR_ROW equals p.TCDR_TCDR_ROW
            //            where (p.STMT_STMT_ID == id && b.TCDR_ROW == p.TCDR_TCDR_ROW && p.TCDR_TCDC_TCDC_CODE == b.TCDC_TCDC_CODE)
            //            select b;

            var query = from b in Db.TRH_STMR_V where (b.STMT_ID == id && b.TCDC_CODE == null && b.TCDR_ROW == null) select b;

            var d = new DataSourceResult
            {
                Data = query.Select(p => new
                {
                    p.STMR_ID,
                    p.WORK_DESC,
                    p.CHAP_DESC,
                    p.ITEM_DESC,
                    p.STMR_PRICE,
                    p.STMR_AMNT,
                    p.STMT_TYPE,
                    p.STMT_ID,
                    p.CNTR_NO,
                    p.TCDC_CODE,
                    p.TCDR_ROW,
                    PRCL_PRCL_ID = p.PRCL_ID,
                    STMT_STMT_ID = p.STMT_ID,
                    p.EX_STMR_PRICE,
                    PRICE = p.STMR_PRICE - p.EX_STMR_PRICE,
                    p.PRCL_PRICE,
                    p.TCDR_AMNT,
                    p.TCDR_FEE,
                    p.TCDC_TCDC_CODE,
                    TCDR_TCDR_ROW = p.TCDR_ROW,
                }),
            };
            return Json(d);
        }

        [EntityAuthorize("TRH_STMR_V > select")]
        public ActionResult Get_stmr_tcdr_v([DataSourceRequest] DataSourceRequest request, int id)
        {
            //var query = from b in cntx.PDF_TECH_DOC_ROW
            //            join p in cntx.TRH_STATEMENT_ROW on b.TCDR_ROW equals p.TCDR_TCDR_ROW
            //            where (p.STMT_STMT_ID == id && b.TCDR_ROW == p.TCDR_TCDR_ROW && p.TCDR_TCDC_TCDC_CODE == b.TCDC_TCDC_CODE)
            //            select b;

            //    var query = from b in cntx.TRH_STMR_TCDR_V where b.STMT_STMT_ID == id select b;

            int count = 0;

            var query = from p in Db.TRH_STMR_V
                        where p.STMT_ID == id && p.TCDC_CODE != null
                        orderby p.STMR_ID descending
                        select new
                        {
                            p.STMR_ID,
                            TCDR_TCDR_ROW = p.TCDR_ROW,
                            TCDR_TCDC_TCDC_CODE = p.TCDC_TCDC_CODE,
                            p.TCDR_AMNT,
                            zarib = "1",
                            p.ITEM_DESC,
                            p.ITEM_ID,
                            p.CHAP_DESC,
                            p.WORK_DESC,
                            p.TCDR_FEE,
                            p.STMT_ID,
                            PRCL_PRCL_ID = p.PRCL_ID,
                            p.TCDC_CODE,
                            STMT_STMT_ID = p.STMT_ID,
                            p.STMR_AMNT,
                            p.STMR_PRICE,
                            p.EX_STMR_PRICE,
                            PRICE = p.STMR_PRICE + p.EX_STMR_PRICE,
                            p.CNTR_NO,
                            count = (count + 1)
                        };

            return Json(query.ToDataSourceResult(request));

        }

        //[EntityAuthorize("TRH_TCDR_V > select")]
        public ActionResult Get_tech_doc_row([DataSourceRequest] DataSourceRequest request, int id)
        {
            var query = (from p in Db.TRH_TCDR_V.AsEnumerable()
                         where (p.TCDC_CODE == id && p.FINY_FINY_YEAR == null)
                         orderby p.TCDR_ROW
                         select new
                         {
                             p.TCDR_ROW,
                             p.TCDR_PRICE,
                             p.TCDC_CODE,
                             p.TCDR_AMNT,
                             zarib = "1",
                             p.ITEM_DESC,
                             p.ITEM_ID,
                             p.CHAP_DESC,
                             p.WORK_DESC,
                             p.FACT,
                             p.PRICE,
                             p.TCDR_FEE,
                             p.TCDR_CODE
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        //[EntityAuthorize("TRH_TCDR_V > select")]
        public ActionResult Get_tech_doc_row2([DataSourceRequest] DataSourceRequest request, int id)
        {
            var query = (from p in Db.TRH_TCDR_V.AsEnumerable()
                         where (p.TCDC_CODE == id && p.FINY_FINY_YEAR != null)
                         orderby p.TCDR_ROW
                         select new
                         {
                             p.TCDR_ROW,
                             p.TCDR_PRICE,
                             p.TCDC_CODE,
                             p.TCDR_AMNT,
                             zarib = "1",
                             p.ITEM_DESC,
                             p.ITEM_ID,
                             p.CHAP_DESC,
                             p.WORK_DESC,
                             p.FACT,
                             p.PRICE,
                             p.TCDR_FEE,
                             p.TCDR_CODE
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_PDRF_REFERENCE > select")]
        public ActionResult Get_reference([DataSourceRequest] DataSourceRequest request, string PDRF_FIN_LETT_NO)
        {
            var query = from p in Db.PRN_PDRF_REFERENCE
                        orderby p.PREF_ID descending
                        where p.PDRF_FIN_LETT_NO == PDRF_FIN_LETT_NO
                        select new
                        {
                            p.PREF_ID,
                            p.PDRF_FIN_LETT_NO,
                            p.STRN_SSTO_STOR_CODE,
                            p.STRN_TRAN_NO,
                            p.STRN_TRAN_TYPE,
                            p.STRN_TRAN_YEAR,
                            //strn_date = p.STR_TRANSACTION.TRAN_YEAR + "/" + p.STR_TRANSACTION.TRAN_MONT + "/" + p.STR_TRANSACTION.TRAN_DAY,
                            //stor_desc = p.STR_TRANSACTION.STR_STORE.STOR_DESC
                        };

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_JOB > select")]
        public ActionResult Get_PRN_JOB([DataSourceRequest] DataSourceRequest request)
        {
            var query = from p in Db.PRN_JOB
                            //where p.CRET_BY==orclname
                        orderby p.JOB_ROW descending
                        select new
                        {
                            p.JOB_ROW,
                            p.JOB_DESC
                        };

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_PRIVATE_FACTS > select")]
        public ActionResult Get_PRN_PRIVATE_FACT([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from p in Db.PRN_PRIVATE_FACTS
                         orderby p.PRVT_ROW descending
                         // where p.CRET_BY==orclname
                         select new
                         {
                             p.PRVT_ROW,
                             p.PRVT_DESC
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_CONTRACTOR_SUPERVISION > select")]
        public ActionResult Get_PRN_CONTRACTOR_SUPERVISION([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from p in Db.PRN_CONTRACTOR_SUPERVISION.AsEnumerable()
                         orderby p.CRET_DATE descending
                         // where p.CRET_BY==orclname
                         select new
                         {
                             p.ID,
                             CCOR_CNOR_ID_2 = p.CCOR_CNOR_ID,
                             name = p.CSPR_NAME + " " + p.CSPR_FAMIL,
                             brth_date = p.BRTH_YEAR + "/" + p.BRTH_MONT + "/" + p.BRTH_DAY,
                             firs_date = p.FIRS_YEAR + "/" + p.FIRS_MONT + "/" + p.FIRS_DAY,
                             p.CSPR_MDRK,
                             p.CSPR_MRTH,
                             p.CSPR_POST,
                             CSPR_STAT2 = p.CSPR_STAT,
                             p.CSPR_TEL,
                             PJOB_JOB_ROW2 = p.PJOB_JOB_ROW,
                             p.CSPR_ADRS,
                             p.CSPR_DESC
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_LETTER > select")]
        public ActionResult Get_PRN_LETTER([DataSourceRequest] DataSourceRequest request)
        {
            var query = from p in Db.PRN_LETTER
                            // where p.CRET_BY==orclname
                        orderby p.LETR_ID descending
                        select new
                        {
                            p.LETR_ID,
                            p.LETTER,
                            p.LETTER_DATE
                        };

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_WAGES > select")]
        public ActionResult Get_WAGE([DataSourceRequest] DataSourceRequest request, int LETR_ID)
        {
            var query = from p in Db.PRN_WAGES
                        orderby p.WAGE_ROW descending
                        where (p.PLET_LETR_ID == LETR_ID)
                        select new
                        {
                            p.PLET_LETR_ID,
                            p.PJOB_JOB_ROW,
                            p.AMNT,
                            p.AQNT,
                            p.WAGE_ROW,
                            letr = p.PRN_LETTER.LETTER,
                            semat = p.PRN_JOB.JOB_DESC
                        };

            return Json(query.ToDataSourceResult(request));
        }
        public ActionResult Get_WORK_WORK([DataSourceRequest] DataSourceRequest request, string CCNT_CNTR_NO)
        {
            var query = from p in Db.CNT_WORK_CONTRACT
                        orderby p.TWRQ_WQ_SEQ descending
                        where (p.CCNT_CNTR_NO == CCNT_CNTR_NO)
                        select new
                        {
                            p.CCNT_CNTR_NO,
                            p.TWRQ_WQ_SEQ,
                            p.TWRQ_YEAR,
                            WQ_DESC = p.FND_WORK_RQ.WQ_DESC
                        };

            return Json(query.ToDataSourceResult(request));
        }
        public ActionResult GridContractor([DataSourceRequest] DataSourceRequest request, int ESIL_ID)
        {
            var query = from p in Db.CNT_CONTRACTOR
                        join q in Db.PLN_CONT_LETT on p.CNOR_ID equals q.CCOR_CNOR_ID

                        orderby p.COMP_NAME descending
                        where (q.ESIL_ESIL_ID == ESIL_ID)
                        select new
                        {
                            p.COMP_NAME,
                            PCOL_ID = q.ID
                        };

            return Json(query.ToDataSourceResult(request));
        }
        public ActionResult GridDocument([DataSourceRequest] DataSourceRequest request, int ESIL_ID)
        {
            var query = from p in Db.PLN_SIGNIFIC_DOCUMENT

                        orderby p.SIDO_ID descending
                        where (p.ESIL_ESIL_ID == ESIL_ID)
                        select new
                        {
                            p.SIDO_ID,
                            p.SIDO_DATE,
                            p.SIDO_TITL,
                            p.SIDO_COMM
                        };

            return Json(query.ToDataSourceResult(request));
        }
        public ActionResult GridConfilict([DataSourceRequest] DataSourceRequest request, int ESIL_ID)
        {
            var query = from p in Db.PLN_SIGNIFIC_CONFILICT

                        orderby p.SICO_ID descending
                        where (p.ESIL_ESIL_ID == ESIL_ID)
                        select new
                        {
                            p.SICO_ID,
                            p.SICO_SATE,
                            p.SICO_EDAT,
                            p.SICO_TITL,
                            p.SICO_COMM
                        };

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Get_Pur_WORK([DataSourceRequest] DataSourceRequest request, string CCNT_CNTR_NO)
        {
            var query = from p in Db.CNT_PUR_CONTRACT
                        orderby p.SPUR_PURE_NO descending
                        where (p.CCNT_CNTR_NO == CCNT_CNTR_NO)
                        select new
                        {
                            p.CCNT_CNTR_NO,
                            p.SPUR_PURE_NO,
                            p.SPUR_PURE_YEAR
                        };

            return Json(query.ToDataSourceResult(request));
        }


        public ActionResult ReadAscr([DataSourceRequest] DataSourceRequest request, string FINY_YEAR, string fdModal, int? WR_SEQN)
        {
            bool filterDisable = string.IsNullOrEmpty(fdModal);
            string filter = string.IsNullOrEmpty(fdModal.ToUpper()) ? "" : fdModal.ToUpper().ToArabicUtf8();

            if (WR_SEQN != null)//دستور کاری
            {
                var query = from p in Db.CGT_ASCR_V
                            join q in Db.FND_EXPF_DETL on p.EXPF_CODE equals q.BEXF_EXPF_CODE
                            join m in Db.FND_WORK_ORD on p.ASS_CODE equals m.ASST_ASS_CODE

                            where (p.EXPF_DESC.ToUpper().Contains(filter)
                            || p.ASS_CODE.ToUpper().Contains(filter)
                            || p.MAIN_CODE.ToUpper().Contains(filter)
                             || p.EXPF_CODE.ToUpper().Contains(filter)
                            || filterDisable)
                            && p.FINY_YEAR == FINY_YEAR
                            && q.TWRO_WR_SEQN == WR_SEQN
                            && q.TWRO_W_YEAR == FINY_YEAR
                            && m.WR_SEQN == WR_SEQN
                            && m.W_YEAR == FINY_YEAR
                            && p.MAIN_CODE == m.ASST_MAIN_MAIN_CODE

                            select new
                            {
                                p.ASS_CODE,
                                p.MAIN_CODE,
                                p.EXPF_DESC,
                                p.FINY_YEAR,
                                p.EXPF_CODE

                            };
                return Json(query.ToDataSourceResult(request));
            }
            else//انجام کاری
            {
                var query = from p in Db.CGT_ASCR_V


                            where (p.EXPF_DESC.ToUpper().Contains(filter)
                            || p.ASS_CODE.ToUpper().Contains(filter)
                            || p.MAIN_CODE.ToUpper().Contains(filter)
                             || p.EXPF_CODE.ToUpper().Contains(filter)
                            || filterDisable)
                            && p.FINY_YEAR == FINY_YEAR


                            select new
                            {
                                p.ASS_CODE,
                                p.MAIN_CODE,
                                p.EXPF_DESC,
                                p.FINY_YEAR,
                                p.EXPF_CODE

                            };
                return Json(query.ToDataSourceResult(request));
            }


        }
        [EntityAuthorize("TRH_WORK_TYPE > select")]
        public ActionResult Get_TRH_WORK_TYPE([DataSourceRequest] DataSourceRequest request)
        {
            string cret_by = string.Empty;
            var query = (from p in Db.TRH_WORK_TYPE
                             //join u in cntx.SEC_USERS on p.CRET_BY equals u.ORCL_NAME.ToUpper()
                         orderby p.WORK_ID descending
                         select new
                         {
                             p.WORK_ID,
                             p.WORK_CODE,
                             p.WORK_DESC,
                             p.WORK_TYPE,
                             p.WORK_STAT,
                             p.CRET_BY,
                             // name=u.FAML_NAME
                             // name=cntx.SEC_USERS.Where(xx=>xx.ORCL_NAME==cret_by.ToLower()).Select(xx=>xx.FAML_NAME).FirstOrDefault()
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_FACTORS > select")]
        public ActionResult Get_PRN_FACTORS([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from p in Db.PRN_FACTORS
                         orderby p.FACT_CODE descending
                         select new
                         {
                             p.FACT_AMNT,
                             p.FACT_CODE,
                             p.FACT_DESC,
                             p.FACT_SOURCE,
                             p.FACT_BLNC
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("CGT_LETTER > select")]
        public ActionResult Get_cgt_letter([DataSourceRequest] DataSourceRequest request, string cntr_no, string let_type)
        {
            if (string.IsNullOrEmpty(cntr_no))
                cntr_no = "0";

            // string orcl_name=new UserInfo().Username;
            //long let_code = 0;

            var query = Db.CGT_LETTER.Where(b => (b.CCNT_CNTR_NO == cntr_no && (b.LET_TYPE == let_type || let_type == null))).AsEnumerable()
                          .Select(b => new
                          {
                              b.LET_CODE,
                              //let_code = b.LET_CODE,
                              b.LET_NUMBER,
                              b.LET_DESC,
                              b.LET_AMNT,
                              let_date = b.LET_YEAR + "/" + b.LET_MNT + "/" + b.LET_DAY,
                              b.LET_TYPE,
                              b.LET_KIND,
                              b.LET_STAT,
                              ext_date = (b.EXTD_YEAR == null ? "0" : b.EXTD_YEAR) + " سال " + (b.EXTD_MONT == null ? "0" : b.EXTD_MONT) + " ماه " + (b.EXTD_DAY == null ? "0" : b.EXTD_DAY) + " روز ",
                              FIN_LETT_NO = getLetNumByLetCode(b.LET_CODE),
                              ADST_NUM = getAdstNum(getLetNumByLetCode(b.LET_CODE)),
                              DOC_SEQ = getDocSeq(getLetNumByLetCode(b.LET_CODE))
                          }).ToList().OrderByDescending(xx => xx.LET_CODE);

            return Json(query.ToDataSourceResult(request));
        }

        private string getLetNumByLetCode(long? letCode)
        {
            //     if (letCode.HasValue)
            //     {
            var obj = Db.PRN_PAY_DRAFT.Where(xx => xx.CLET_LET_CODE == letCode).FirstOrDefault();
            if (obj != null)
                return obj.FIN_LETT_NO;
            else
                return string.Empty;
            //return cntx.PRN_PAY_DRAFT.Where(xx => xx.CLET_LET_CODE == letCode).FIN_LETT_NO;
            //   }
            //   else
            //   {
            //       return string.Empty;
            //   }
        }
        private int? getDocSeq(string FIN_LETT_NO)
        {

            var obj = Db.PRN_PAY_DRAFT.Where(xx => xx.FIN_LETT_NO == FIN_LETT_NO).FirstOrDefault();
            if (obj != null)
                return obj.DCMT_DOC_SEQ;
            else
                return 0;

        }
        private int? getAdstNum(string FIN_LETT_NO)
        {

            var obj = Db.PRN_PAY_DRAFT.Where(xx => xx.FIN_LETT_NO == FIN_LETT_NO).FirstOrDefault();
            if (obj != null)
                return obj.ADST_NUM;
            else
                return 0;

        }

        [EntityAuthorize("CGT_LETTER > select")]
        public ActionResult Get_cgt_show_letter([DataSourceRequest] DataSourceRequest request, long let_code)
        {
            var query = (from b in Db.CGT_LETTER
                         where (b.LET_CODE == let_code || let_code == null)
                         orderby b.LET_CODE descending
                         select new
                         {
                             b.LET_CODE,
                             b.LET_NUMBER,
                             b.LET_DESC,
                             b.LET_AMNT,
                             let_date = b.LET_YEAR + "/" + b.LET_MNT + "/" + b.LET_DAY,
                             b.LET_TYPE,
                             b.LET_KIND,
                             b.LET_STAT,
                             ext_date = (b.EXTD_YEAR == null ? "0" : b.EXTD_YEAR) + " سال " + (b.EXTD_MONT == null ? "0" : b.EXTD_MONT) + " ماه " + (b.EXTD_DAY == null ? "0" : b.EXTD_DAY) + " روز "
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("CGT_LETTER > select")]
        public ActionResult Get_cgt_letter_all([DataSourceRequest] DataSourceRequest request, string cntr_no)
        {
            if (string.IsNullOrEmpty(cntr_no)) { cntr_no = "0"; };
            var query = (from b in Db.CGT_LETTER
                         where (b.CCNT_CNTR_NO == cntr_no && (b.LET_TYPE == "5" || b.LET_TYPE == "4" || b.LET_TYPE == "12" || b.LET_TYPE == "14"))
                         orderby b.LET_CODE descending
                         select new
                         {
                             b.LET_CODE,
                             b.LET_NUMBER,
                             b.LET_DESC,
                             b.LET_AMNT,
                             let_date = b.LET_YEAR + "/" + b.LET_MNT + "/" + b.LET_DAY,
                             b.LET_TYPE,
                             b.LET_KIND,
                             b.LET_STAT,
                             ext_date = b.EXTD_YEAR + " سال " + b.EXTD_MONT + " ماه " + b.EXTD_DAY + " روز "
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("CGT_LETTER > select")]
        public ActionResult Get_minute_deliver([DataSourceRequest] DataSourceRequest request, string cntr_no, short mide_type)
        {
            if (string.IsNullOrEmpty(cntr_no)) { cntr_no = "0"; };
            var query = (from b in Db.TRH_MINUTE_DELIVER
                         where (b.CCNT_CNTR_NO == cntr_no && b.MIDE_TYPE == mide_type)
                         orderby b.ID descending
                         select new
                         {
                             b.ID,
                             b.MIDE_DESC,
                             b.MIDE_DATE,
                             b.MIDE_TYPE,
                             b.CCNT_CNTR_NO,
                             b.CCNR_CCNT_CNTR_NO,
                             b.CCNR_TWRO_W_YEAR,
                             b.CCNR_TWRO_WR_SEQN,
                             b.CCOR_CNOR_ID
                             // wr_desc=b.CNT_CONTRACT_ORDER.TWRO_WR_SEQN
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("CGT_LETTER > select")]
        public ActionResult Get_minute_deliver_ROW([DataSourceRequest] DataSourceRequest request, short? mide_id)
        {
            var query = (from b in Db.TRH_MINUTE_DELIVER_ROW
                         where (b.MIDE_ID == mide_id)
                         orderby b.ID descending
                         select new
                         {
                             b.ID,
                             b.MIDR_DESC,
                             b.MIDE_ID,
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("CGT_LETTER > select")]
        public ActionResult Get_cgt_letter_inc([DataSourceRequest] DataSourceRequest request, string cntr_no)
        {
            if (string.IsNullOrEmpty(cntr_no))
                cntr_no = "0";

            var query = (from b in Db.CGT_LETTER
                         where (b.CCNT_CNTR_NO == cntr_no && b.LET_TYPE == "16")
                         orderby b.LET_CODE descending
                         select new
                         {
                             b.LET_CODE,
                             b.LET_NUMBER,
                             b.LET_DESC,
                             b.LET_AMNT,
                             let_date = b.LET_YEAR + "/" + b.LET_MNT + "/" + b.LET_DAY,
                             b.LET_TYPE,
                             b.LET_KIND,
                             b.LET_STAT,
                             ext_date = b.EXTD_YEAR + " سال " + b.EXTD_MONT + " ماه " + b.EXTD_DAY + " روز "
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("FND_WORK_ORD > select")]
        public ActionResult get_work_order([DataSourceRequest] DataSourceRequest request, int WR_SEQN, string WR_DESC, string W_YEAR)
        {
            var query = from b in Db.FND_WORK_ORD
                        where (b.WR_DESC.Contains(WR_DESC) && b.W_YEAR.Contains(W_YEAR) && b.WR_SEQN == WR_SEQN)
                        orderby b.WR_SEQN, b.W_YEAR
                        select new
                        {
                            b.WR_SEQN,
                            b.WR_DESC,
                            b.W_YEAR,
                            b.TPRT_PR_CODE,
                            b.SUPL,
                            b.INDC,
                            b.TWRO_TYPE
                        };
            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult get_cnt_work_order([DataSourceRequest] DataSourceRequest request, string cntr_no, string WR_DESC, string W_YEAR, string WR_SEQN, string INDC, string TYPE)
        {
            // if (string.IsNullOrEmpty(WR_SEQN)) WR_SEQN=
            int i = int.Parse(string.IsNullOrEmpty(WR_SEQN) ? "0" : WR_SEQN);
            if (string.IsNullOrEmpty(cntr_no)) { cntr_no = "0"; };
            List<int> WR_SEQN_ARR = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, i };
            var query = from b in Db.FND_WORK_ORD
                        join c in Db.CNT_CONTRACT_ORDER on b.WR_SEQN equals c.TWRO_WR_SEQN
                        where (c.CCNT_CNTR_NO.Contains(cntr_no) && b.WR_DESC.Contains(WR_DESC) && b.W_YEAR.Contains(W_YEAR)
                              && b.WR_SEQN == c.TWRO_WR_SEQN && b.W_YEAR == c.TWRO_W_YEAR)
                        orderby b.WR_SEQN, b.W_YEAR
                        select new
                        {
                            b.WR_SEQN,
                            b.WR_DESC,
                            b.W_YEAR,
                            b.TPRT_PR_CODE,
                            b.SUPL,
                            b.INDC,
                            b.TWRO_TYPE
                        };
            return Json(query.ToDataSourceResult(request));
        }
        public ActionResult get_pdrf_oper_letter2([DataSourceRequest] DataSourceRequest request, long let_code)
        {
            string LetType = Db.CGT_LETTER.Where(xx => xx.LET_CODE == let_code).Select(xx => xx.LET_TYPE).FirstOrDefault();

            var query = (from o in Db.PRN_PDRF_OPER
                         where o.CLET_LET_CODE == let_code
                         orderby o.ID
                         select o).Select(o => new
                         {
                             o.ID,
                             o.TWRO_W_YEAR,
                             o.TWRO_WR_SEQN,
                             //o.SUBU_TYPE,
                             o.AMNT,
                             //o.COPE_OPRN_CODE,
                             // oprn_desc = o.PCT_OPERATION.OPRN_DESC,
                             o.PDRW_ID,
                             o.ASST_ASS_CODE,
                             o.ASST_MAIN_MAIN_CODE,
                             o.BEXF_EXPF_CODE,
                             o.GEOL_G_CODE


                         }).ToList();
            var query2 = query.Select(o => new
            {
                o.ID,
                o.AMNT,
                o.TWRO_WR_SEQN,
                o.TWRO_W_YEAR,
                ASS_CODE = o.ASST_ASS_CODE,
                MAIN_CODE = o.ASST_MAIN_MAIN_CODE,
                EXPF_CODE = o.BEXF_EXPF_CODE,
                o.GEOL_G_CODE,
                EXPF_DESC = GetExpfDesc(o.ASST_ASS_CODE, o.ASST_MAIN_MAIN_CODE, o.BEXF_EXPF_CODE),
                G_DESC = GetLocation(o.GEOL_G_CODE)

            }).ToList().Distinct();
            return Json(query2.ToDataSourceResult(request));


        }



        [EntityAuthorize("PRN_OPER_V > select | PRN_PDRF_OPER > select")]
        public ActionResult get_pdrf_oper_letter([DataSourceRequest] DataSourceRequest request, long let_code)
        {
            var query = from b in Db.PRN_PDRF_OPER
                        join c in Db.PRN_PDRF_WORK on b.PDRW_ID equals c.ID
                        where (c.CLET_LET_CODE == let_code)
                        orderby b.TWRO_WR_SEQN, b.TWRO_WR_SEQN
                        select new
                        {
                            c.TWRO_WR_SEQN,
                            c.TWRO_W_YEAR,
                            b.PDRW_ID,
                            b.ID,
                            b.SUBU_TYPE,
                            b.AMNT,
                            oprn_desc = b.PCT_OPERATION.OPRN_DESC,

                        };
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("FND_WORK_ORD > select | PRN_PDRF_WORK > select")]
        public ActionResult get_work_letter([DataSourceRequest] DataSourceRequest request, long? let_code)
        {
            var query = from b in Db.FND_WORK_ORD
                        join c in Db.PRN_PDRF_WORK on b.WR_SEQN equals c.TWRO_WR_SEQN
                        where (b.WR_SEQN == c.TWRO_WR_SEQN && b.W_YEAR == c.TWRO_W_YEAR && c.CLET_LET_CODE == let_code)
                        orderby b.WR_SEQN, b.W_YEAR
                        select new
                        {
                            b.WR_SEQN,
                            b.WR_DESC,
                            b.W_YEAR,
                            b.TPRT_PR_CODE,
                            b.SUPL,
                            b.INDC,
                            b.TWRO_TYPE
                        };
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("STR_PURCHASE_REQUEST => select ")]
        public ActionResult get_purch([DataSourceRequest] DataSourceRequest request, int PURE_NO, string PRCH_DESC, string PURE_YEAR)
        {
            var query = (from b in Db.STR_PURCHASE_REQUEST
                         where (b.PRCH_DESC.Contains(PRCH_DESC) && b.PURE_YEAR.Contains(PURE_YEAR) && b.PURE_NO == PURE_NO)
                         orderby b.PURE_NO descending
                         select new
                         {
                             b.PURE_NO,
                             b.PRCH_DESC,
                             b.PURE_YEAR
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_INQUIRY > select")]
        public ActionResult get_INQUIRY([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from b in Db.PRN_INQUIRY
                         orderby b.ID descending
                         select new
                         {
                             b.ID,
                             b.INQY_SUBJ,
                             b.AMNT,
                             b.INQY_PIVS
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_INQUIRY_ROWS > select | TRH_ITEM > select")]
        public ActionResult get_inquiry_row([DataSourceRequest] DataSourceRequest request, int? inqy_id)
        {
            var query = (from b in Db.PRN_INQUIRY_ROWS
                         join i in Db.TRH_ITEM on b.ITEM_ITEM_ID equals i.ITEM_ID
                         where b.INQY_ID == inqy_id
                         orderby b.ID descending
                         select new
                         {
                             b.ID,
                             b.INRW_DESC,
                             b.INRW_AMNT,
                             b.INRW_FEE,
                             b.INRW_SEQ,
                             desc = i.ITEM_DESC
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_INQUIRY_RESPONDS > select | PRN_INQUIRY_ROWS > select | TRH_ITEM > select")]
        public ActionResult get_inquiry_responds([DataSourceRequest] DataSourceRequest request, int? inqy_id, int? incr_id)
        {
            var query = (from b in Db.PRN_INQUIRY_RESPONDS
                         join r in Db.PRN_INQUIRY_ROWS on b.INRW_ID equals r.ID
                         join i in Db.TRH_ITEM on r.ITEM_ITEM_ID equals i.ITEM_ID
                         where (b.INCR_ID == incr_id && b.INQY_ID == inqy_id)
                         orderby b.ID descending
                         select new
                         {
                             b.ID,
                             b.INRS_PRICE,
                             fee = r.INRW_FEE,
                             amnt = r.INRW_AMNT,
                             desc = i.ITEM_DESC,
                             b.INRW_ID,
                             b.INCR_ID,
                             b.INQY_ID
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PDF_STANDARD_ACTIVITY > select | STR_UNIT_MEASURMENT > select")]
        public ActionResult get_standard_master([DataSourceRequest] DataSourceRequest request, int? KPRJ_ROW)
        {
            var query = (from b in Db.PDF_STANDARD_ACTIVITY
                         where (b.CKPR_KPRJ_ROW == KPRJ_ROW && b.STND_ST_ID == null)
                         orderby b.ST_ID descending
                         select new
                         {
                             b.ST_ID,
                             b.STND_DESC,
                             b.STND_LEVEL,
                             b.SUNM_UNIT_CODE,
                             b.WGHT,
                             b.STAT,
                             b.FINY_FINY_YEAR,
                             b.CODE,
                             unit_desc = b.STR_UNIT_MEASURMENT.UNIT_DESC
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        //public ActionResult get_standard_operation([DataSourceRequest] DataSourceRequest request, int? KPRJ_ROW)
        //{
        //    var query = (from b in cntx.PDF_STANDARD_OPERATIONS
        //                 where (b.PDF_STANDARD_ACTIVITY.CKPR_KPRJ_ROW == KPRJ_ROW )
        //                 orderby b.STND_ST_ID descending
        //                 select new
        //                 {

        //                     STND_DESC=b.PDF_STANDARD_ACTIVITY.STND_DESC,
        //                     OPRN_DESC=cntx.PCT_OPERATION.Where(xx=>xx.OPRN_CODE==b.COPE_OPRN_CODE).Select(xx=>xx.OPRN_DESC).FirstOrDefault()



        //                 }).ToList();
        //    return Json(query.ToDataSourceResult(request));
        //}

        [EntityAuthorize("PDF_STANDARD_ACTIVITY > select | STR_UNIT_MEASURMENT > select")]
        public ActionResult get_standard([DataSourceRequest] DataSourceRequest request, int KPRJ_ROW, int st_id)
        {
            var query = (from b in Db.PDF_STANDARD_ACTIVITY
                         where (b.CKPR_KPRJ_ROW == KPRJ_ROW && (b.STND_ST_ID == st_id || st_id == null))
                         orderby b.ST_ID descending
                         select new
                         {
                             b.ST_ID,
                             b.STND_DESC,
                             b.STND_LEVEL,
                             b.SUNM_UNIT_CODE,
                             b.WGHT,
                             b.STAT,
                             b.FINY_FINY_YEAR,
                             b.CODE,
                             unit_desc = b.STR_UNIT_MEASURMENT.UNIT_DESC
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_CATEGORIES > select")]
        public ActionResult get_prn_category([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from b in Db.PRN_CATEGORIES
                             //where b.CRET_BY==orclname
                         orderby b.ID descending
                         select new
                         {
                             b.ID,
                             b.CARG_DESC,
                             b.CATG_STAT,
                             b.CATG_TYPE,
                             b.CATG_NAME
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_INQUIRY_CONTRACTORS > select")]
        public ActionResult get_inquiry_contractor([DataSourceRequest] DataSourceRequest request, int? inqy_id)
        {
            var query = (from b in Db.PRN_INQUIRY_CONTRACTORS
                         where b.INQY_ID == inqy_id
                         orderby b.ID descending
                         select new
                         {
                             b.ID,
                             b.CCOR_CNOR_ID,
                             b.INCR_PRIC,
                             b.INCR_FACT,
                             b.INCR_FAEM,
                             b.INSR_STAT,
                             b.INQY_ID
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_COMMENT > select")]
        public ActionResult get_prn_comment([DataSourceRequest] DataSourceRequest request, int? inqy_id)
        {
            var query = (from b in Db.PRN_COMMENT
                         where b.PI_ID == inqy_id
                         orderby b.ID descending
                         select new
                         {
                             b.ID,
                             b.CMNT_DESC,
                             b.CMNT_ROW,
                             b.PI_ID
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_CATEGORY_ROWS > select | TRH_ITEM > select")]
        public ActionResult get_category_row([DataSourceRequest] DataSourceRequest request, int pc_id)
        {
            var query = (from b in Db.PRN_CATEGORY_ROWS
                         join i in Db.TRH_ITEM on b.ITEM_ITEM_ID equals i.ITEM_ID
                         where b.PC_ID == pc_id
                         orderby b.CATR_ROW descending
                         select new
                         {
                             b.CATR_ROW,
                             b.ITEM_ITEM_ID,
                             b.SUNM_UNIT_CODE,
                             b.CATG_FEE,
                             b.CATG_AMNT,
                             b.CATR_DESC,
                             desc = i.ITEM_DESC
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult ReadItem([DataSourceRequest] DataSourceRequest request, string desc)
        {
            var query = (from b in Db.TRH_ITEM
                         where b.ITEM_DESC != "-" && (b.TRH_CHAPTER.CHAP_DESC.ToUpper().Contains(desc.ToUpper())
                         || b.ITEM_DESC.ToUpper().Contains(desc.ToUpper())
                         || b.TRH_CHAPTER.TRH_WORK_TYPE.WORK_DESC.ToUpper().Contains(desc.ToUpper())
                         || desc == null)
                         orderby b.ITEM_DESC descending
                         select new
                         {
                             b.ITEM_ID,
                             b.ITEM_CODE,
                             b.ITEM_DESC,
                             CHAP_DESC = b.TRH_CHAPTER.CHAP_DESC,
                             //WORK_DESC = b.TRH_CHAPTER.TRH_WORK_TYPE.WORK_DESC
                         }).ToList().OrderBy(xx => xx.ITEM_DESC);
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("STR_DIRECT_REQUEST > select")]
        public ActionResult get_req([DataSourceRequest] DataSourceRequest request, int DIPU_NO, string DRCT_DESC, string DIPU_YEAR)
        {
            var query = (from b in Db.STR_DIRECT_REQUEST
                         where (b.DRCT_DESC.Contains(DRCT_DESC) && b.DIPU_YEAR.Contains(DIPU_YEAR) && b.DIPU_NO == DIPU_NO)
                         orderby b.DIPU_NO descending
                         select new
                         {
                             b.DIPU_NO,
                             b.DRCT_DESC,
                             b.DIPU_YEAR
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PDF_TECH_DOC > select")]
        public ActionResult Get_PDF_TECH_DOC([DataSourceRequest] DataSourceRequest request, short? PlanId, short? ProjId)
        {
            // var query = from b in db.EXP_EXPI_DOC select new { b.CRET_DATE, b.FIRS_TIME };
            username = User.Identity.Name;

            var query = (from b in Db.PDF_TECH_DOC


                         orderby b.TCDC_CODE descending
                         select new
                         {
                             b.TCDC_CODE,
                             b.EQPD_DESC,
                             b.FINY_FINY_YEAR,
                             b.TCDC_LOCT,
                             b.RECM_PRICE,
                             b.TCDC_STAT,
                             b.CCNT_CNTR_NO

                         }).ToList();
            foreach (var item in query)
            {
                string stat = this.Ajax_flow_stat(item.TCDC_CODE, username, "FLW_TDOC.PFLW_TDOC");
                if (stat == "0")
                {
                    Db.Database.ExecuteSqlCommand(string.Format("update pdf_tech_doc set tcdc_stat='5' where tcdc_code='{0}'", item.TCDC_CODE));
                    Db.SaveChanges();
                }
            }

            var query2 = (from b in Db.PDF_TECH_DOC
                          where ((b.CPRO_CPLA_PLN_CODE == PlanId || PlanId == 0) && (b.CPRO_PRJ_CODE == ProjId || ProjId == 0))
                          orderby b.TCDC_CODE descending
                          select new
                          {
                              b.TCDC_CODE,
                              b.EQPD_DESC,
                              b.FINY_FINY_YEAR,
                              b.TCDC_LOCT,
                              b.RECM_PRICE,
                              b.TCDC_STAT,
                              b.CCNT_CNTR_NO

                          }).ToList();

            return Json(query2.ToDataSourceResult(request));

            // query = (from b in cntx.PDF_TECH_DOC orderby b.TCDC_CODE select b).Select(p => new
            //{
            //    p.TCDC_CODE,
            //    p.EQPD_DESC,

            //});
            //var d = new DataSourceResult
            //{

            //    Data = query

            //};
            //return Json(d);
        }

        [EntityAuthorize("TRH_ITEM > select")]
        public ActionResult Get_TRH_ITEM([DataSourceRequest] DataSourceRequest request, short? id)
        {
            var query = (from b in Db.TRH_ITEM
                         where b.CHAP_CHAP_ID == id
                         //&& b.CRET_BY==orclname
                         orderby b.ITEM_ID descending
                         select new
                         {
                             b.ITEM_ID,
                             b.ITEM_CODE,
                             b.ITEM_DESC,
                             b.ITEM_STAT,
                             b.ITEM_TYPE,
                             b.CHAP_CHAP_ID,
                             b.SUNM_UNIT_CODE
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("TRH_CHAPTER > select")]
        public ActionResult Get_TRH_CHAPTER([DataSourceRequest] DataSourceRequest request, short id)
        {
            var query = (from b in Db.TRH_CHAPTER
                         where (b.WORK_WORK_ID == id || id == null)
                         orderby b.CHAP_ID descending, b.CHAP_CODE descending
                         select new
                         {
                             b.CHAP_ID,
                             b.CHAP_CODE,
                             b.CHAP_DESC,
                             b.CHAP_STAT,
                             b.WORK_WORK_ID
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PRN_CUR_FACT_AMNT > select")]
        public ActionResult Get_fact([DataSourceRequest] DataSourceRequest request, int tcdc_code)
        {
            var query = (from p in Db.PRN_CUR_FACT_AMNT
                         where p.TCDC_TCDC_CODE == tcdc_code
                         orderby p.FACT_ROW descending
                         select new
                         {
                             p.FACT_ROW,
                             p.FINL_AMNT,
                             p.SCOP,
                             //WORK_DESC = p.TRH_WORK_TYPE.WORK_DESC,
                             //FACT_DESC = p.PRN_FACTORS.FACT_DESC,
                             //FACT_AMNT = p.PRN_FACTORS.FACT_AMNT,
                             p.WORK_WORK_ID
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PDF_DAILY_REPORT_TYPES > select")]
        public ActionResult Get_DRTY([DataSourceRequest] DataSourceRequest request, int? KPRJ_ROW)
        {
            var query = (from p in Db.PDF_DAILY_REPORT_TYPES
                         where (p.CKPR_KPRJ_ROW == KPRJ_ROW || KPRJ_ROW == null)
                         orderby p.DRPT_ROW descending
                         select new
                         {
                             p.DRPT_ROW,
                             p.DRPT_DESC,
                             p.SBTP_DESC,
                             p.TYPE_DESC,
                             p.DRPT_STAT,
                             p.CKPR_KPRJ_ROW,
                             KPRJ_DESC = p.CGT_KPRO.KPRJ_DESC
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PDF_CTRL_PRO > select")]
        public ActionResult Get_CTRL([DataSourceRequest] DataSourceRequest request, int? CTRL_ID)
        {
            //decimal sum=cntx.Database.SqlQuery<decimal>("select sum(a.CTRR_AMNT*b.WGHT)/100 from PDF_CTRL_PRO_ROW a,PDF_DAILY_REPORT_ITEMS b where CTRL_CTRL_ID=:p1 and a.PDRI_DPRI_ROW=b.DPRI_ROW",p1)
            var query = (from p in Db.PDF_CTRL_PRO
                         where (p.CTRL_CTRL_ID == CTRL_ID)
                         orderby p.CTRL_ID descending
                         select new
                         {
                             p.CTRL_ID,
                             p.CTRL_DESC,
                             p.CTRL_DATE,
                             PLN_DESC = p.CGT_PRO.CGT_PLAN.PLN_DESC,
                             PRJ_DESC = p.CGT_PRO.PRJ_DESC,
                             DRPT_DESC = p.PDF_DAILY_REPORT_TYPES.DRPT_DESC,
                             //PERCENT = getpercent(p.CTRL_ID)
                             //cntx.PDF_CTRL_PRO_ROW.Where(xx=>xx.CTRL_CTRL_ID==p.CTRL_ID).Select(xx=>xx.CTRR_AMNT *xx.PDF_DAILY_REPORT_ITEMS.WGHT).Sum()
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PDF_CTRL_PRO > select")]
        public ActionResult Get_CTRL2([DataSourceRequest] DataSourceRequest request, string code)
        {
            short? CPRO_PRJ_CODE = 0, CPRO_CPLA_PLN_CODE = 0;

            if (code != "," && !string.IsNullOrEmpty(code))
            {
                var val = code.Split(',');
                CPRO_CPLA_PLN_CODE = short.Parse(val[0].ToString());
                CPRO_PRJ_CODE = short.Parse(val[1].ToString());
            }
            // var q = cntx.Database.SqlQuery(string.Format("select "));

            var query = (from p in Db.PDF_CTRL_PRO
                             //  where (p.CPRO_PRJ_CODE == CPRO_PRJ_CODE && p.CPRO_CPLA_PLN_CODE == CPRO_CPLA_PLN_CODE && p.CTRL_CTRL_ID == null)
                         where p.CTRL_CTRL_ID == null
                         orderby p.CTRL_ID
                         select new
                         {
                             p.CTRL_ID,
                             p.CTRL_DESC,
                             PLN_DESC = p.CGT_PRO.CGT_PLAN.PLN_DESC,
                             PRJ_DESC = p.CGT_PRO.PRJ_DESC,
                             DRPT_DESC = p.PDF_DAILY_REPORT_TYPES.DRPT_DESC,
                             KPRJ_DESC = p.CGT_PRO.CGT_KPRO.KPRJ_DESC,
                             p.CTRL_TYPE,
                             p.CTRL_CODE,
                             CKPR_KPRJ_ROW = p.CGT_PRO.CKPR_KPRJ_ROW,
                             p.CPRO_PRJ_CODE,
                             p.CPRO_CPLA_PLN_CODE
                         }).ToList().Distinct();

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Get_CTRL3([DataSourceRequest] DataSourceRequest request, int? CKPR_KPRJ_ROW)
        {
            var query = (from r in Db.PDF_DAILY_REPORT_TYPES
                         join p in Db.PDF_CTRL_PRO on r.DRPT_ROW equals p.PDRT_DRPT_ROW
                         where (p.PDRT_DRPT_ROW == r.DRPT_ROW && r.CKPR_KPRJ_ROW == CKPR_KPRJ_ROW)
                         orderby p.CTRL_ID descending
                         select new
                         {
                             p.CTRL_ID,
                             p.CTRL_DESC,
                             PLN_DESC = p.CGT_PRO.CGT_PLAN.PLN_DESC,
                             PRJ_DESC = p.CGT_PRO.PRJ_DESC,
                             DRPT_DESC = p.PDF_DAILY_REPORT_TYPES.DRPT_DESC
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Get_CTRL_FILE([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from p in Db.PDF_CTRL_PRO
                             //join b in cntx.CGT_PRO on p.CPRO_PRJ_CODE equals b.PRJ_CODE
                             // join k in cntx.CGT_KPRO on p.CPRO_PRJ_CODE equals k.CGT_PRO.
                             // where (p.CPRO_PRJ_CODE==292 && k.CPRO_CPLA_PLN_CODE==1 ) 
                             // && b.CKPR_KPRJ_ROW == k.KPRJ_ROW 
                             // )
                         orderby p.CTRL_ID descending
                         select new
                         {
                             p.CTRL_ID,
                             p.CTRL_DESC,
                             PLN_DESC = p.CGT_PRO.CGT_PLAN.PLN_DESC,
                             PRJ_DESC = p.CGT_PRO.PRJ_DESC,
                             DRPT_DESC = p.PDF_DAILY_REPORT_TYPES.DRPT_DESC,
                             //  KPRJ_DESC= k.KPRJ_DESC,
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PDF_DAILY_REPORT_ITEMS > select")]
        public ActionResult Get_DRTY_ITEM([DataSourceRequest] DataSourceRequest request, short? id)
        {
            var query = (from p in Db.PDF_DAILY_REPORT_ITEMS
                         join a in Db.PDF_STANDARD_ACTIVITY on p.STND_ST_ID equals a.ST_ID
                         join b in Db.PDF_STANDARD_ACTIVITY on a.STND_ST_ID equals b.ST_ID
                         where p.PDRT_DRPT_ROW == id
                         //orderby p.PDF_STANDARD_ACTIVITY.STND_ST_ID descending
                         select new
                         {
                             p.DPRI_ROW,
                             p.ITEM_DESC,
                             p.WGHT,
                             p.STND_ST_ID,
                             // ST_DESC=p.PDF_STANDARD_ACTIVITY.PDF_STANDARD_ACTIVITY1.Where(xx=>xx.ST_ID==p.PDF_STANDARD_ACTIVITY.STND_ST_ID).Select(xx=>xx.STND_DESC).FirstOrDefault()
                             ST_DESC = b.STND_DESC
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("PDF_CTRL_PRO_ROW > select")]
        public ActionResult Get_CTRL_ROW([DataSourceRequest] DataSourceRequest request, short? id)
        {
            var query = (from p in Db.PDF_CTRL_PRO_ROW
                         where p.CTRL_CTRL_ID == id
                         orderby p.CTRR_ROW descending
                         select new
                         {
                             p.CTRR_ROW,
                             p.CTRR_AMNT,
                             ITEM_DESC = p.PDF_DAILY_REPORT_ITEMS.ITEM_DESC,
                             p.CTRL_CTRL_ID,
                             p.PDRI_DPRI_ROW,
                             WGHT = p.PDF_DAILY_REPORT_ITEMS.WGHT,
                             //LEVEL = p.PDF_DAILY_REPORT_ITEMS.PDF_STANDARD_ACTIVITY.STND_LEVEL
                             //,ST_ID = find_rec_f(Convert.ToString(p.PDF_DAILY_REPORT_ITEMS.STND_ST_ID))
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("TRH_PRICE > select")]
        public ActionResult Get_TRH_PRICE([DataSourceRequest] DataSourceRequest request, int chap_id, string year)
        {
            var query = (from b in Db.TRH_PRICE
                         where (b.TRH_ITEM.CHAP_CHAP_ID == chap_id && b.FINY_FINY_YEAR == year)
                         orderby b.PRCL_ID descending
                         select new
                         {
                             b.PRCL_ID,
                             b.ITEM_ITEM_ID,
                             b.ITEM_TYPE,
                             b.FINY_FINY_YEAR,
                             b.PRICE,
                             b.PRCL_STAT
                         }).ToList();
            return Json(query.ToDataSourceResult(request));
        }

        ////////////////////////////////////////////////////////////////////Update
        [EntityAuthorize("PRN_CONTRACTOR_SUPERVISION > update")]
        public ActionResult Update_PRN_CONTRACTOR_SUPERVISION([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PRN_CONTRACTOR_SUPERVISION> PRN_CONTRACTOR_SUPERVISION)
        {
            if (PRN_CONTRACTOR_SUPERVISION != null)
            {
                foreach (PRN_CONTRACTOR_SUPERVISION item in PRN_CONTRACTOR_SUPERVISION)
                {
                    item.CCOR_CNOR_ID = short.Parse(Request.Form["CCOR_CNOR_ID_2"]);
                    item.PJOB_JOB_ROW = Convert.ToInt16(Request.Form["PJOB_JOB_ROW2"]);
                    item.CSPR_STAT = Request.Form["CSPR_STAT2"];
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(PRN_CONTRACTOR_SUPERVISION.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("PRN_JOB > update")]
        public ActionResult Update_PRN_JOB([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PRN_JOB> PRN_JOB)
        {
            if (PRN_JOB != null)
            {
                foreach (PRN_JOB item in PRN_JOB)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(PRN_JOB.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("PRN_WAGES > update")]
        public ActionResult Update_PRN_WAGE([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PRN_WAGES> PRN_WAGES)
        {
            if (PRN_WAGES != null)
            {
                foreach (PRN_WAGES item in PRN_WAGES)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(PRN_WAGES.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("PRN_LETTER => update")]
        public ActionResult Update_PRN_LETTER([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PRN_LETTER> PRN_LETTER)
        {
            if (PRN_LETTER != null)
            {
                foreach (PRN_LETTER item in PRN_LETTER)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(PRN_LETTER.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("PRN_PRIVATE_FACTS > update")]
        public ActionResult Update_PRN_PRIVATE_FACT([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PRN_PRIVATE_FACTS> PRN_PRIVATE_FACT)
        {
            if (PRN_PRIVATE_FACT != null)
            {
                foreach (PRN_PRIVATE_FACTS item in PRN_PRIVATE_FACT)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(PRN_PRIVATE_FACT.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("TRH_WORK_TYPE > update")]
        public ActionResult Update_TRH_WORK_TYPE([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TRH_WORK_TYPE> WORK_TYPE)
        {
            if (WORK_TYPE != null)
            {
                foreach (TRH_WORK_TYPE item in WORK_TYPE)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(WORK_TYPE.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update_Get_cgt_letter([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<CGT_LETTER> CGT_LETTER)
        {
            if (CGT_LETTER != null)
            {
                foreach (CGT_LETTER item in CGT_LETTER)
                {
                    item.CRET_BY = orclname;
                    item.CRET_DATE = DateTime.Now;
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(CGT_LETTER.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update_minute_deliver([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TRH_MINUTE_DELIVER> TRH_MINUTE_DELIVER)
        {
            if (TRH_MINUTE_DELIVER != null)
            {
                foreach (TRH_MINUTE_DELIVER item in TRH_MINUTE_DELIVER)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(TRH_MINUTE_DELIVER.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update_minute_deliver_row([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TRH_MINUTE_DELIVER_ROW> TRH_MINUTE_DELIVER_ROW)
        {
            if (TRH_MINUTE_DELIVER_ROW != null)
            {
                foreach (TRH_MINUTE_DELIVER_ROW item in TRH_MINUTE_DELIVER_ROW)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(TRH_MINUTE_DELIVER_ROW.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("PRN_CATEGORIES > update")]
        public ActionResult Update_prn_category([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PRN_CATEGORIES> PRN_CATEGORIES)
        {
            if (PRN_CATEGORIES != null)
            {
                foreach (PRN_CATEGORIES item in PRN_CATEGORIES)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(PRN_CATEGORIES.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("PRN_FACTORS > update")]
        public ActionResult Update_PRN_FACTORS([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PRN_FACTORS> PRN_FACTORS)
        {
            if (PRN_FACTORS != null)
            {
                foreach (PRN_FACTORS item in PRN_FACTORS)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(PRN_FACTORS.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("TRH_CHAPTER > update")]
        public ActionResult Update_TRH_CHAPTER([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TRH_CHAPTER> TRH_CHAPTER)
        {
            if (TRH_CHAPTER != null)
            {
                foreach (TRH_CHAPTER item in TRH_CHAPTER)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(TRH_CHAPTER.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        [EntityAuthorize("PDF_TECH_DOC > select,update")]
        public string updte_confirm_tcdt(PDF_TECH_DOC objecttemp)
        {
            var query = from b in Db.PDF_TECH_DOC
                        where (b.TCDC_CODE == objecttemp.TCDC_CODE && b.CCNT_CNTR_NO != null)
                        select b;

            //var query2 = from b in cntx.PDF_TECH_DOC
            //             where (b.TCDC_CODE == objecttemp.TCDC_CODE && b.EQPD_TYPE.CompareTo("2") > 0 && b.EQPD_TYPE.CompareTo("6") < 0 && b.CCOR_CNOR_ID == null)
            //             select b;

            //string confirm = "0";

            //if (query.Any()) { confirm = "1"; }
            //else
            //    if (query2.Any()) { confirm = "2"; }
            //    else
            //        if (confirm == "0")
            //        {

            if (query.Any())
            {
                string sql = string.Format("update pdf_tech_doc set tcdc_stat='14' where tcdc_code={0}", objecttemp.TCDC_CODE);
                Db.Database.ExecuteSqlCommand(sql);
            }
            else
            {
                string sql = string.Format("update pdf_tech_doc set tcdc_stat='5' where tcdc_code={0}", objecttemp.TCDC_CODE);
                Db.Database.ExecuteSqlCommand(sql);
            }
            //                    }
            return ("2");
        }


        [EntityAuthorize("PDF_TECH_DOC > update")]
        public ActionResult Update_PDF_TECH_DOC(PDF_TECH_DOC objecttemp)
        {
            int tcdc_code = Convert.ToInt16(Request.Form["id"]);
            string sql = string.Format("update pdf_tech_doc set bl_stat='{0}',cntr_type='{1}',CPRO_CPLA_PLN_CODE='{2}',CPRO_PRJ_CODE='{3}',dur_day='{4}',dur_mont='{5}'" +
               ",dur_year='{6}',eqpd_desc='{7}',eqpd_type='{8}',FINY_FINY_YEAR='{9}',flor_fact='{10}',heit_fact='{11}',ORGA_MANA_CODE='{12}',ORGA_MANA_ASTA_CODE='{13}',ORGA_CODE='{14}',recm_price='{15}'" +
               ",regi_fact='{16}',TCDC_LOCT='{17}',TCDR_TGOB='{18}',CCOR_CNOR_ID='{19}',EQPD_DAY='{21}',EQPD_MONT='{22}',EQPD_YEAR='{23}',EQPD_NUM='{24}' where tcdc_code='{20}'",
               objecttemp.BL_STAT,
               objecttemp.CNTR_TYPE,
               objecttemp.CPRO_CPLA_PLN_CODE,
               objecttemp.CPRO_PRJ_CODE,
               objecttemp.DUR_DAY,
               objecttemp.DUR_MONT,
               objecttemp.DUR_YEAR,
               objecttemp.EQPD_DESC,
               objecttemp.EQPD_TYPE,
               objecttemp.FINY_FINY_YEAR,
               objecttemp.FLOR_FACT,
               objecttemp.HEIT_FACT,
               objecttemp.ORGA_MANA_CODE,
               objecttemp.ORGA_MANA_ASTA_CODE,
               objecttemp.ORGA_CODE,
               objecttemp.RECM_PRICE,
               objecttemp.REGI_FACT,
               objecttemp.TCDC_LOCT,
               objecttemp.TCDR_TGOB,
               objecttemp.CCOR_CNOR_ID,
               tcdc_code,
               objecttemp.EQPD_DAY,
               objecttemp.EQPD_MONT,
               objecttemp.EQPD_YEAR,
               objecttemp.EQPD_NUM
               );

            Db.Database.ExecuteSqlCommand(sql);
            Db.SaveChanges();
            return Json(new { Success = "True" }, JsonRequestBehavior.DenyGet);
        }

        [EntityAuthorize("TRH_ITEM > update")]
        public ActionResult Update_TRH_ITEM([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TRH_ITEM> TRH_ITEM)
        {
            if (TRH_ITEM != null)
            {
                foreach (TRH_ITEM item in TRH_ITEM)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(TRH_ITEM.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("TRH_INDEX > update")]
        public ActionResult Update_TRH_INDEX([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TRH_INDEX> TRH_INDEX)
        {
            if (TRH_INDEX != null)
            {
                foreach (TRH_INDEX item in TRH_INDEX)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(TRH_INDEX.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("TRH_PRICE > select,update")]
        public ActionResult Update_TRH_PRICE([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TRH_PRICE> TRH_PRICE)
        {
            if (TRH_PRICE != null)
            {
                foreach (TRH_PRICE item in TRH_PRICE)
                {
                    //bool doesExistAlready = Db.TRH_PRICE.Where(o => o.ITEM_ITEM_ID == item.ITEM_ITEM_ID)
                    //                                    .Where(o => o.FINY_FINY_YEAR == item.FINY_FINY_YEAR)
                    //                                    .Any(o => o.ITEM_ITEM_ID == item.ITEM_ITEM_ID);
                    //  if (!doesExistAlready)
                    //  {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                    //  }
                }
            }

            return Json(TRH_PRICE.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("TRH_STATEMENT > update")]
        public ActionResult Update_statement([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TRH_STATEMENT> TRH_STATEMENT)
        {
            if (TRH_STATEMENT != null)
            {
                foreach (TRH_STATEMENT item in TRH_STATEMENT)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(TRH_STATEMENT.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("TRH_STATEMENT_ROW > update")]
        public ActionResult Update_stmr_tcdr([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TRH_STATEMENT_ROW> TRH_STATEMENT_ROW)
        {
            if (TRH_STATEMENT_ROW != null)
            {
                foreach (TRH_STATEMENT_ROW item in TRH_STATEMENT_ROW)
                {
                    // item.STMR_PRICE=item.STMR_AMNT*item.
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(TRH_STATEMENT_ROW.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("PRN_INQUIRY_CONTRACTORS > update")]
        public ActionResult Update_inquiry_contractor([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PRN_INQUIRY_CONTRACTORS> PRN_INQUIRY_CONTRACTORS)
        {
            if (PRN_INQUIRY_CONTRACTORS != null)
            {
                foreach (PRN_INQUIRY_CONTRACTORS item in PRN_INQUIRY_CONTRACTORS)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            return Json(PRN_INQUIRY_CONTRACTORS.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("PRN_INQUIRY_RESPONDS > select,update | PRN_INQUIRY_CONTRACTORS > update")]
        public ActionResult Update_inquiry_responds([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PRN_INQUIRY_RESPONDS> PRN_INQUIRY_RESPONDS)
        {
            decimal? sum = 0, incr_id = 0;
            short? inqy_id = 0;

            PRN_INQUIRY_CONTRACTORS incr = new PRN_INQUIRY_CONTRACTORS();

            if (PRN_INQUIRY_RESPONDS != null)
            {
                foreach (PRN_INQUIRY_RESPONDS item in PRN_INQUIRY_RESPONDS)
                {
                    sum = Convert.ToInt32(item.INRS_PRICE) + sum;
                    incr_id = item.INCR_ID;
                    inqy_id = item.INQY_ID;
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }

                sum = Db.PRN_INQUIRY_RESPONDS.Where(oo => oo.INQY_ID == inqy_id)
                                             .Where(oo => oo.INCR_ID == incr_id)
                                             .Select(oo => oo.INRS_PRICE).Sum();

                string sql = string.Format("update prn_inquiry_contractors set INCR_PRIC='{0}' where ID='{1}' ", sum, incr_id);
                Db.Database.ExecuteSqlCommand(sql);
                Db.SaveChanges();
            }

            return Json(PRN_INQUIRY_RESPONDS.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("TRH_STATEMENT_ROW > select,update | TRH_STATEMENT > select | PDF_TECH_DOC_ROW > select")]
        public ActionResult Update_statement_row([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TRH_STATEMENT_ROW> TRH_STATEMENT_ROW)
        {
            int zarib = 0;

            if (TRH_STATEMENT_ROW != null)
            {
                foreach (TRH_STATEMENT_ROW item in TRH_STATEMENT_ROW)
                {
                    string cntr_no = Db.TRH_STATEMENT.Where(xx => xx.STMT_ID == item.STMT_STMT_ID).Select(xx => xx.CCNT_CNTR_NO).FirstOrDefault();
                    decimal end_amnt = Db.Database.SqlQuery<decimal>(string.Format("SELECT nvl(PRN_REMAIN_AMNT_U('{0}'),0) FROM DUAL", cntr_no)).FirstOrDefault();

                    // item.STMR_PRICE=item.STMR_AMNT*item.t
                    var query = from tdr in Db.PDF_TECH_DOC_ROW
                                where (tdr.TCDR_ROW == item.TCDR_TCDR_ROW && tdr.TCDC_TCDC_CODE == item.TCDR_TCDC_TCDC_CODE)
                                select new
                                {
                                    tdr.TCDR_FEE
                                };

                    zarib = 1;
                    decimal stmr_amnt = Convert.ToDecimal(item.STMR_AMNT);
                    // int tcdr_price=Convert.ToInt32(query.Select(x => x.TCDR_PRICE).FirstOrDefault());
                    decimal fee = Convert.ToDecimal(query.Select(xx => xx.TCDR_FEE).FirstOrDefault());
                    decimal? price = Db.TRH_STATEMENT_ROW.Where(xx => xx.STMT_STMT_ID == item.STMT_STMT_ID)
                                                         .Where(xx => xx.STMR_ID != item.STMR_ID)
                                                         .Select(xx => xx.STMR_PRICE).Sum();
                    if (price == null) { price = 0; };
                    if (item.PRCL_PRCL_ID == null)
                    {
                        item.STMR_PRICE = Convert.ToDecimal(stmr_amnt * fee * zarib);
                    }
                    else
                    {
                        item.STMR_PRICE = Convert.ToDecimal(stmr_amnt *
                                                            zarib *
                                                            Db.TRH_PRICE.Where(xx => xx.PRCL_ID == item.PRCL_PRCL_ID).Select(xx => xx.PRICE).FirstOrDefault()
                                                            );
                    }
                    //   if (item.STMR_PRICE <= end_amnt)
                    //  {
                    if (price + item.STMR_PRICE <= end_amnt)
                    {
                        Db.Entry(item).State = EntityState.Modified;
                        Db.SaveChanges();
                    }
                    //   }
                }
            }

            return Json(TRH_STATEMENT_ROW.ToDataSourceResult(request, ModelState));
        }

        [EntityAuthorize("TRH_PRICE > select | TRH_STATEMENT_ROW > select | PDF_TECH_DOC_ROW > update")]
        public ActionResult Update_tech_doc_row([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PDF_TECH_DOC_ROW> PDF_TECH_DOC_ROW)
        {
            if (PDF_TECH_DOC_ROW != null)
            {
                foreach (PDF_TECH_DOC_ROW item in PDF_TECH_DOC_ROW)
                {
                    int price = Convert.ToInt32(Db.TRH_PRICE.
                    Where(x => x.ITEM_ITEM_ID == item.ITEM_ITEM_ID).Select(x => x.PRICE).FirstOrDefault());
                    item.TCDR_PRICE = item.TCDR_AMNT * price;

                    var query = from b in Db.TRH_STATEMENT_ROW where (b.TCDR_TCDR_ROW == item.TCDR_ROW && b.TCDR_TCDC_TCDC_CODE == item.TCDC_TCDC_CODE) select b;
                    if (!query.Any())
                    {
                        Db.Entry(item).State = EntityState.Modified;
                        Db.SaveChanges();
                    }
                }
            }

            return Json(PDF_TECH_DOC_ROW.ToDataSourceResult(request, ModelState));
        }

        ////////////////////////////////////////////////////////////////////DropDown

        [EntityAuthorize("TRH_WORK_TYPE > select")]
        public ActionResult get_work_type_DP()
        {
            var RetVal = (from b in Db.TRH_WORK_TYPE where (b.WORK_STAT == "1") orderby b.WORK_DESC select new { b.WORK_ID, b.WORK_DESC }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("TRH_WORK_TYPE > select")]
        public ActionResult get_work_price_DP(string type)
        {
            var RetVal = (from b in Db.TRH_WORK_TYPE
                          join c in Db.TRH_CHAPTER on b.WORK_ID equals c.WORK_WORK_ID
                          where (b.WORK_TYPE != "3" && c.WORK_WORK_ID == b.WORK_ID)
                          orderby b.WORK_DESC
                          select new { b.WORK_ID, b.WORK_DESC }).ToList().Distinct();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("CGT_KPRO > select")]
        public ActionResult get_kproj_DP()
        {
            var RetVal = (from b in Db.CGT_KPRO orderby b.KPRJ_DESC select new { b.KPRJ_ROW, b.KPRJ_DESC }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("CGT_KPRO > select")]
        public ActionResult get_kproj_DP2(string prj_code)
        {
            string plan_code = "0";
            short sprj_code = 0, splan_code = 0;
            if (prj_code != null)
            {
                var val = prj_code.Split(',');
                prj_code = val[0].ToString();
                plan_code = val[1].ToString();
            }

            if (prj_code != null)
            {
                sprj_code = short.Parse(prj_code);
            }

            if (plan_code != null)
            {

                splan_code = short.Parse(plan_code);
            }

            var RetVal = (from b in Db.CGT_KPRO
                          join p in Db.CGT_PRO on b.KPRJ_ROW equals p.CKPR_KPRJ_ROW
                          where (b.KPRJ_ROW == p.CKPR_KPRJ_ROW && p.PRJ_CODE == sprj_code && p.CPLA_PLN_CODE == splan_code)
                          orderby b.KPRJ_DESC
                          select new { b.KPRJ_ROW, b.KPRJ_DESC }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("STR_UNIT_MEASURMENT > select")]
        public ActionResult get_STR_UNIT_MEASURMENT_DP()
        {
            var RetVal = (from b in Db.STR_UNIT_MEASURMENT orderby b.UNIT_DESC select new { b.UNIT_CODE, b.UNIT_DESC }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("STR_UNIT_MEASURMENT > select | TRH_ITEM > select")]
        public ActionResult get_STR_UNIT_MEASURMENT_DP2(long item_id)
        {
            var RetVal = (from b in Db.STR_UNIT_MEASURMENT
                          join i in Db.TRH_ITEM on b.UNIT_CODE equals i.SUNM_UNIT_CODE
                          where (i.ITEM_ID == item_id && i.SUNM_UNIT_CODE == b.UNIT_CODE)
                          orderby b.UNIT_DESC
                          select new { b.UNIT_CODE, b.UNIT_DESC }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("TRH_CHAPTER > select")]
        public ActionResult get_chapter_DP(Int16? work_id)
        {
            var RetVal = (from b in Db.TRH_CHAPTER
                          join i in Db.TRH_ITEM on b.CHAP_ID equals i.CHAP_CHAP_ID
                          where ((b.WORK_WORK_ID == work_id || work_id == null) && b.CHAP_STAT == "1" && b.CHAP_ID == i.CHAP_CHAP_ID)
                          orderby b.CHAP_DESC
                          select new { b.CHAP_ID, b.CHAP_DESC }).ToList().Distinct();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLocation_DP()
        {
            var RetVal = (from b in Db.BKP_GEOGH_LOC


                          orderby b.G_DESC
                          select new { b.G_CODE, b.G_DESC }).ToList().Distinct();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDetailed_DP()
        {
            var RetVal = (from b in Db.BKP_DETAILED


                          orderby b.DET_DESC
                          select new { b.DET_CODE, b.DET_DESC }).ToList().Distinct();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }
        [EntityAuthorize("TRH_CHAPTER > select")]
        public ActionResult get_chapter_DP2(Int16? work_id)
        {
            var RetVal = (from b in Db.TRH_CHAPTER
                          where ((b.WORK_WORK_ID == work_id || work_id == null) && b.CHAP_STAT == "1")
                          orderby b.CHAP_DESC
                          select new { b.CHAP_ID, b.CHAP_DESC }).ToList().Distinct();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("PRN_FACTORS > select")]
        public ActionResult get_prn_fact_DP()
        {
            var RetVal = (from b in Db.PRN_FACTORS orderby b.FACT_CODE select new { b.FACT_CODE, b.FACT_DESC }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("PDF_STANDARD_ACTIVITY > select")]
        public ActionResult get_stnd_DP(int id)
        {
            //var RetVal = from b in cntx.PDF_STANDARD_ACTIVITY
            //               where b.STND_LEVEL == (from c in cntx.PDF_STANDARD_ACTIVITY where c.CKPR_KPRJ_ROW == b.CKPR_KPRJ_ROW select c.STND_LEVEL.Max())
            //             orderby b.ST_ID
            //             select new { b.ST_ID, b.STND_DESC };
            short? q = Db.PDF_DAILY_REPORT_TYPES.Where(xx => xx.DRPT_ROW == id).Select(xx => xx.CKPR_KPRJ_ROW).FirstOrDefault();
            var RetVal = Db.Database.SqlQuery<MyClass>(string.Format("select a.ST_ID,a.STND_DESC from PDF_STANDARD_ACTIVITY  a where STND_LEVEL =  (SELECT max(STND_LEVEL)FROM PDF_STANDARD_ACTIVITY L_STND where L_STND.CKPR_KPRJ_ROW = a.CKPR_KPRJ_ROW  and L_STND.CKPR_KPRJ_ROW='{0}')", q));
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("PRN_WAGES > select")]
        public ActionResult get_wage_DP(string code)
        {
            int letr_id = 0, PJOB_JOB_ROW = 0;
            if (code != null)
            {
                var val = code.Split(',');
                letr_id = int.Parse(val[0].ToString());
                PJOB_JOB_ROW = int.Parse(val[1].ToString());
            }

            var RetVal = (from b in Db.PRN_WAGES
                          where b.PLET_LETR_ID == letr_id && b.PJOB_JOB_ROW == PJOB_JOB_ROW
                          orderby b.WAGE_ROW
                          select new { b.AMNT, b.WAGE_ROW, b.AQNT }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("PRN_WAGES > select")]
        public ActionResult get_wage_amnt_DP(int wage_row)
        {
            var RetVal = (from b in Db.PRN_WAGES where b.WAGE_ROW == wage_row orderby b.WAGE_ROW select new { b.AMNT }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("PRN_LETTER > select")]
        public ActionResult get_letter_DP()
        {
            var RetVal = (from b in Db.PRN_LETTER orderby b.LETR_ID select new { b.LETR_ID, b.LETTER }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Get_Purch_DP(string YEAR)
        {
            var RetVal = Db.Database.SqlQuery<STR_PURCHASE_REQUEST>("select * from STR_PURCHASE_REQUEST where PURE_YEAR=:YEAR" +
                                                               " and PRCH_DESC is not null  and (PURE_NO,PURE_YEAR) NOT IN (SELECT SPUR_PURE_NO,SPUR_PURE_YEAR  FROM CNT_PUR_CONTRACT )", YEAR)
                                                               .Select(x => new
                                                               {
                                                                   x.PURE_NO,
                                                                   x.PRCH_DESC,
                                                                   NO = x.PURE_NO + "^" + x.PURE_YEAR,
                                                               }).ToList();
            //var RetVal = (from b in Db.STR_PURCHASE_REQUEST orderby b.PRCH_DESC where b.PURE_YEAR==YEAR select new { b.PURE_NO, b.PRCH_DESC }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_FndWork_DP(string YEAR)
        {
            var RetVal = Db.Database.SqlQuery<FND_WORK_RQ>("select * from FND_WORK_RQ where YEAR=:YEAR" +
                                                               " and (year,wq_seq) not in (select twrq_year,twrq_wq_seq from cnt_work_contract )  and rqtp in (1,2) and fndt = 1 ", YEAR)
                                                               .Select(x => new
                                                               {
                                                                   x.WQ_SEQ,
                                                                   x.YEAR,
                                                                   SEQ = x.WQ_SEQ + "^" + x.YEAR,
                                                                   x.WQ_DESC
                                                               }).ToList();
            //var RetVal = (from b in Db.STR_PURCHASE_REQUEST orderby b.PRCH_DESC where b.PURE_YEAR==YEAR select new { b.PURE_NO, b.PRCH_DESC }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }


        [EntityAuthorize("PRN_FACTORS > select")]
        public ActionResult get_fact_amnt_DP(int? fact_code)
        {
            var RetVal = (from b in Db.PRN_FACTORS where (b.FACT_CODE == fact_code) select new { b.FACT_AMNT }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("PRN_CATEGORIES > select")]
        public ActionResult get_Category_DP()
        {
            var RetVal = (from b in Db.PRN_CATEGORIES orderby b.CARG_DESC select new { b.ID, b.CATG_NAME }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("PRN_PREF_V > select")]
        public ActionResult get_tran_DP(string cntr_no, string tran_year)
        {
            if (string.IsNullOrEmpty(cntr_no)) { cntr_no = "0"; };
            if (string.IsNullOrEmpty(tran_year)) { tran_year = "0"; };

            var RetVal = (from b in Db.PRN_PREF_V
                          where (b.TRAN_YEAR == tran_year && b.CCNT_CNTR_NO == cntr_no)
                          orderby b.TRAN_DESC
                          select new
                          {
                              DESC = b.TRAN_CODE + " " + b.STOR_DESC + " تاریخ:  " + b.TRAN_YEAR + "/" + b.TRAN_MONT + "/" + b.TRAN_DAY,
                              b.TRAN_YEAR,
                              b.TRAN_TYPE,
                              b.TRAN_MONT,
                              b.TRAN_DAY,
                              b.TRAN_CODE,
                              b.TRAN_NO
                          }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }


        [EntityAuthorize("TRH_PRICE_V > select | TRH_ITEM > select")]
        public ActionResult get_item_DP(int chap_id, string cntr_no, int TCDC_CODE)
        {
            if (string.IsNullOrEmpty(cntr_no)) { cntr_no = "0"; };
            var query = Db.TRH_PRICE_V.Where(x => x.CCNT_CNTR_NO == cntr_no).Select(x => x.ITEM_ITEM_ID);

            if (TCDC_CODE != null)
            {
                query = Db.TRH_PRICE_V.Where(x => x.TCDC_CODE == TCDC_CODE).Select(x => x.ITEM_ITEM_ID);
            }
            var query1 = (from b in Db.TRH_ITEM
                          join c in Db.TRH_CHAPTER on b.CHAP_CHAP_ID equals c.CHAP_ID

                          where (query.Contains(b.ITEM_ID) && b.CHAP_CHAP_ID == chap_id)
                          orderby b.ITEM_DESC

                          select new { b.ITEM_ID, b.ITEM_DESC }).ToList().Distinct();
            //var ret = query.Any(query);

            return Json(query1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_item_DP2(int chap_id)
        {
            var query1 = (from b in Db.TRH_ITEM
                          where b.CHAP_CHAP_ID == chap_id
                          orderby b.ITEM_DESC
                          select new { b.ITEM_ID, b.ITEM_DESC }).ToList();
            //var ret = query.Any(query);

            return Json(query1, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("TRH_ITEM > select")]
        public ActionResult get_item_stmr_DP(int chap_id)
        {
            var query1 = (from b in Db.TRH_ITEM
                          where b.CHAP_CHAP_ID == chap_id
                          orderby b.ITEM_DESC
                          select new { b.ITEM_ID, b.ITEM_DESC }).ToList();
            //var ret = query.Any(query);

            return Json(query1, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("TRH_ITEM > select | TRH_PRICE > select")]
        public ActionResult get_item_price_DP(int chap_id, string year)
        {
            if (string.IsNullOrEmpty(year)) { year = "0"; };
            var query1 = (from b in Db.TRH_ITEM
                          join p in Db.TRH_PRICE on b.ITEM_ID equals p.ITEM_ITEM_ID
                          where (b.CHAP_CHAP_ID == chap_id && b.ITEM_ID == p.ITEM_ITEM_ID && p.FINY_FINY_YEAR == year)
                          orderby b.ITEM_DESC
                          select new { b.ITEM_ID, b.ITEM_DESC }).ToList();
            //var ret = query.Any(query);

            return Json(query1, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("WORK_ORDER_CONTRACT_V > select")]
        public ActionResult get_work_ord_DP(string cntr_no)
        {
            if (string.IsNullOrEmpty(cntr_no)) { cntr_no = "0"; };
            var query = (from b in Db.WORK_ORDER_CONTRACT_V
                         where b.CCNT_CNTR_NO == cntr_no
                         select new { b.DES, b.WR_SEQN }).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("CGT_PRO > select | PDF_DAILY_REPORT_TYPES > select")]
        public ActionResult get_drty_DP(string prj_code)
        {
            string plan_code = "0";
            short sprj_code = 0, splan_code = 0;
            if (prj_code != null)
            {
                var val = prj_code.Split(',');
                prj_code = val[0].ToString();
                plan_code = val[1].ToString();
            }

            if (!string.IsNullOrEmpty(prj_code))
            {
                sprj_code = short.Parse(prj_code);
            }

            if (!string.IsNullOrEmpty(plan_code))
            {
                splan_code = short.Parse(plan_code);
            }

            var q = (from b in Db.CGT_PRO where b.PRJ_CODE == sprj_code && b.CPLA_PLN_CODE == splan_code select b).ToList();

            short? CKPR_KPRJ_ROW = q.Select(xx => xx.CKPR_KPRJ_ROW).FirstOrDefault();
            var query = (from b in Db.PDF_DAILY_REPORT_TYPES
                         where b.CKPR_KPRJ_ROW == CKPR_KPRJ_ROW
                         select new { b.DRPT_ROW, b.DRPT_DESC }).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_drty2_DP(short? CKPR_KPRJ_ROW)
        {
            var query = (from b in Db.PDF_DAILY_REPORT_TYPES
                         where b.CKPR_KPRJ_ROW == CKPR_KPRJ_ROW
                         select new { b.DRPT_ROW, b.DRPT_DESC }).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("PRN_PDRF_WORK > select | WORK_ORDER_CONTRACT_V > select")]
        public ActionResult get_pdrf_work_DP(long? let_code)
        {
            var query = (from b in Db.PRN_PDRF_WORK
                         join c in Db.WORK_ORDER_CONTRACT_V on b.TWRO_WR_SEQN equals c.WR_SEQN
                         where (b.TWRO_WR_SEQN == c.WR_SEQN && b.TWRO_W_YEAR == c.W_YEAR && b.CLET_LET_CODE == let_code)
                         orderby b.ID
                         select new { c.DES, c.WR_DESC }).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("PRN_PDRF_WORK > select | WORK_ORDER_CONTRACT_V > select")]
        public ActionResult get_cntr_work_DP(string cntr_no)
        {
            if (string.IsNullOrEmpty(cntr_no)) { cntr_no = "0"; };
            var query = (from b in Db.WORK_ORDER_CONTRACT_V
                         where (b.CCNT_CNTR_NO == cntr_no)
                         orderby b.DES
                         select new { b.DES, b.WR_DESC }).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("PRN_JOB > select")]
        public ActionResult get_job_DP()
        {
            var query = (from b in Db.PRN_JOB
                         orderby b.JOB_DESC
                         select new { b.JOB_DESC, b.JOB_ROW }).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("PCT_OPERATION > select | FND_WORK_OPER > select | FND_WORK_ORD > select")]
        public ActionResult get_oper_DP(string code)
        {
            int wr_seqn = 0;
            string w_year = "";
            if (!string.IsNullOrEmpty(code))
            {
                var val = code.Split('-');
                wr_seqn = Convert.ToInt32(val[0]);
                w_year = val[1].ToString();
            }

            var query = (from o in Db.PCT_OPERATION
                         join w in Db.CGT_WRTO_V on o.OPRN_CODE equals w.OPRN_CODE
                         where (o.OPRN_CODE == w.OPRN_CODE && w.WR_SEQN == wr_seqn && w.W_YEAR == w_year)
                         orderby o.OPRN_DESC
                         select new { o.OPRN_DESC, o.OPRN_CODE }).Distinct().ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("TRH_PRICE > select")]
        public ActionResult get_price_DP(int item_id, string year)
        {
            var RetVal = (from b in Db.TRH_PRICE
                          where (b.ITEM_ITEM_ID == item_id && b.PRCL_STAT == 1 && b.FINY_FINY_YEAR == year)
                          orderby b.PRCL_ID
                          select new { b.PRCL_ID, b.PRICE }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [EntityAuthorize("CNT_CONTRACT > select | CNT_CONTRACTOR > select")]
        public ActionResult get_contract_DP(short cnor_id)
        {
            var RetVal = (from b in Db.CNT_CONTRACT
                          join c in Db.CNT_CONT_CONTOR on b.CNTR_NO equals c.CCNT_CNTR_NO
                          join cc in Db.CNT_CONTRACTOR on c.CCOR_CNOR_ID equals cc.CNOR_ID
                          where ((b.CNTR_NO == c.CCNT_CNTR_NO) && (c.CCOR_CNOR_ID == cc.CNOR_ID) && (cc.CNOR_ID == cnor_id) || cnor_id == null)
                          orderby b.TITL
                          select new
                          {
                              b.CNTR_NO,
                              b.TITL
                          }).ToList();

            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        ///////////////////////////////////////////////////////AJAX

        [HttpPost]
        [EntityAuthorize("TRH_STATEMENT > select")]
        public string Ajax_get_statement_type(TRH_STATEMENT objecttemp)
        {
            TRH_STATEMENT stmt = new TRH_STATEMENT();
            var maxValue = Db.TRH_STATEMENT.Where(x => x.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Max(x => x.STMT_NO);
            var maxtype = Db.TRH_STATEMENT.Where(x => x.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO)
                                          .Where(x => x.STMT_NO == maxValue)
                                          .Select(x => x.STMT_TYPE).FirstOrDefault();
            if (maxtype == null)
                maxtype = "0";

            return (maxtype.ToString());
        }

        public bool flow_stat(int id, string item_key, string name)
        {
            //string item_key = "FLW_LEPA.PFLW_LEPA", name = "m_fayyazi";
            string sql = string.Format("SELECT stat  FROM WF_NOTE_V where item_key='{0}' and RECIPIENT_ROLE='{1}' order by not_id desc ", item_key + '^' + id.ToString(), name);
            string stat = Db.Database.SqlQuery<string>(sql).FirstOrDefault();

            if (stat == "OPEN")
                return true;
            else
                return false;
        }


        [HttpPost]
        public string Ajax_flow_stat(int id, string name, string item_key)
        {
            //string item_key = "FLW_LEPA.PFLW_LEPA";//, name = "m_fayyazi";
            string sql = string.Format("SELECT stat  FROM WF_NOTE_V where item_key='{0}' and RECIPIENT_ROLE='{1}' order by not_id desc ", item_key + '^' + id.ToString(), name);
            string stat = "OPEN";// cntx.Database.SqlQuery<string>(sql).FirstOrDefault();
            if (stat == "OPEN" || string.IsNullOrEmpty(stat))
                return "1";
            else
                return "0";
        }
        public JsonResult Ajax_Check_Attache(string ENTI_NAME, string ENTI_VALU)
        {

            string sql = string.Format("SELECT ID  FROM scn_attachinter where ENTI_NAME='{0}' and ENTI_VALU='{1}' ", ENTI_NAME, ENTI_VALU);
            if (Db.Database.SqlQuery<int>(sql).Any())

                return new ServerMessages(ServerOprationType.Success) { Message = "True" }.ToJson();
            else
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "False" }.ToJson();
        }

        [HttpPost]
        public int Ajax_Inc_Confirm(int id)
        {
            var q = (from b in Db.CGT_LETTER where b.LET_CODE == id select b).FirstOrDefault();
            string sql = string.Format("INSERT INTO  CNT_CONT_INCDEC(LETR_NO, LETR_YEAR, LETR_MNTH, LETR_DAY, INDC_TYPE, CN_STAT, LICN_NO) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", q.LET_NUMBER, q.LET_YEAR, q.LET_MNT, q.LET_DAY, '1', '1', q.LET_NUMBER);
            Db.Database.ExecuteSqlCommand(sql);
            sql = string.Format("SELECT nvl(max(AMNT_ID),0) + 1 FROM CNT_CONT_AMOUNT WHERE  nvl(CCNT_CNTR_NO, '1') = '{0}'", q.CCNT_CNTR_NO);

            int AMNT_ID = Db.Database.SqlQuery<int>(sql).FirstOrDefault();
            int type = 1;
            //if (q.LET_AMNT < 0) { type = 2; } else { type = 1; }
            sql = string.Format("INSERT INTO CNT_CONT_AMOUNT(CCNT_CNTR_NO,AMNT_ID,AMNT,INDC_TYPE,CN_STAT) VALUES ('{0}',{1},{2},{3},'1')", q.CCNT_CNTR_NO, AMNT_ID, q.LET_AMNT, type);
            Db.Database.ExecuteSqlCommand(sql);
            sql = string.Format("update cgt_letter set let_stat=11 where let_code={0}", id);
            Db.Database.ExecuteSqlCommand(sql);
            return AMNT_ID;
        }

        [HttpPost]
        public int Ajax_noteid(int id, string name, string item_key)
        {
            //string item_key = "FLW_LEPA.PFLW_LEPA";//, name = "m_fayyazi";
            string sql = string.Format("SELECT not_id  FROM WF_NOTE_V where item_key='{0}' and RECIPIENT_ROLE='{1}' order by not_id desc ", item_key + '^' + id.ToString(), name);
            int not_id = Db.Database.SqlQuery<int>(sql).FirstOrDefault();
            return not_id;
        }

        [HttpPost]
        public decimal Ajax_get_oper_remain(string code)
        {
            string w_year = "", year = "", wr_seqn = "", oper_code = "", subu_type = "";
            if (!string.IsNullOrEmpty(code))
            {
                var val = code.Split('-');
                wr_seqn = val[0].ToString();
                w_year = val[1].ToString();
                year = val[5].ToString();
                oper_code = val[6].ToString();
                subu_type = val[7].ToString();
            }

            int seqn = int.Parse(wr_seqn);
            string q = string.Format("SELECT prn_get_remain_u('{0}','{1}','{2}','{3}','{4}') remain FROM DUAL",
                wr_seqn,
                w_year,
                year,
                oper_code,
                subu_type
                );

            var query = Db.Database.SqlQuery<decimal>(q).FirstOrDefault();
            return query;
        }

        [HttpPost]
        [EntityAuthorize("TRH_STATEMENT > select")]
        public string Ajax_get_end_cnt(TRH_STATEMENT objecttemp)
        {
            var end_lett = Db.CGT_LETTER.Where(x => x.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Where(x => x.LET_TYPE == "17").
            Select(x => x.LET_CODE).FirstOrDefault();
            if (end_lett != 0) end_lett = 1;
            return (end_lett.ToString());
        }
        public bool Ajax_GetTrans(string FIN_LETT_NO)
        {
            bool Check = false;
            if (Db.HSB_TRAN_PDRFS.Where(x => x.PDRF_FIN_LETT_NO == FIN_LETT_NO).Any())
            {
                Check = true;
            }


            return Check;
        }

        public bool Ajax_GetRelation(int ID)
        {
            bool Check = false;
            if (Db.PRN_PDRF_OPER.Where(x => x.ID == ID && x.PRN_PAY_DRAFT.DCMT_DOC_SEQ != null).Any())
            {
                Check = true;
            }


            return Check;
        }

        [HttpPost]
        [EntityAuthorize("PDF_STANDARD_ACTIVITY > select")]
        public string Ajax_get_stnd_wght(PDF_STANDARD_ACTIVITY objecttemp)
        {
            var wght = Db.PDF_STANDARD_ACTIVITY.Where(x => x.ST_ID == objecttemp.ST_ID).
            Select(x => x.WGHT).FirstOrDefault();

            return (wght.ToString());
        }

        [HttpPost]
        [EntityAuthorize("PDF_CTRL_PRO > select")]
        public string Ajax_prj_file(PDF_CTRL_PRO objecttemp)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            // startInfo.WorkingDirectory = "C:\\Program Files\\Microsoft Office\\Office11";
            startInfo.FileName = "winproj.exe";
            startInfo.Arguments = "<mis47;uid=bandar;pwd=bandarbandar>\\" + objecttemp.CTRL_ID + "Project.mpp";
            System.Diagnostics.Process.Start(startInfo);
            return ("1");
        }

        [HttpPost]
        [EntityAuthorize("TRH_TCDR_V > select")]
        public string Ajax_get_tcdr_fact(TRH_TCDR_V objecttemp)
        {
            decimal? tcdr_fact = Db.Database.SqlQuery<decimal?>(string.Format("SELECT SUM(TCDR_PRICE*TRH_TCDR_FACT_U(TCDC_CODE,TCDR_ROW))   FROM TRH_TCDR_V  WHERE TCDC_CODE = '{0}'", objecttemp.TCDC_CODE)).FirstOrDefault();
            if (tcdr_fact == null)
                tcdr_fact = 0;
            return (tcdr_fact.ToString());
        }

        [HttpPost]
        [EntityAuthorize("TRH_TCDR_V > select")]
        public string Ajax_get_stmr_fact(TRH_STMR_V objecttemp)
        {
            decimal? tcdr_fact = Db.Database.SqlQuery<decimal?>(string.Format("SELECT SUM(STMR_PRICE*TRH_STMR_FACT_U(STMT_ID,STMR_ID))  FROM TRH_STMR_V  WHERE STMT_ID = '{0}'", objecttemp.STMT_ID)).FirstOrDefault();
            if (tcdr_fact == null)
                tcdr_fact = 0;
            return (tcdr_fact.ToString());
        }

        [HttpPost]
        [EntityAuthorize("TRH_STATEMENT > select")]
        public string Ajax_get_statement_stat(TRH_STATEMENT objecttemp)
        {
            TRH_STATEMENT stmt = new TRH_STATEMENT();
            var maxValue = Db.TRH_STATEMENT.Where(x => x.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Max(x => x.STMT_NO);
            string maxstat = "0";
            var draft = Db.PRN_PAY_DRAFT.Where(xx => xx.STMT_STMT_ID == Db.TRH_STATEMENT.Where(x => x.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO)
                                                                                        .Where(x => x.STMT_NO == maxValue)
                                                                                        .Select(x => x.STMT_ID).FirstOrDefault()
                                         ).Select(xx => xx.FIN_LETT_NO).FirstOrDefault();

            if (draft == null) maxstat = "1";
            return (maxstat.ToString());
        }

        [HttpPost]
        [EntityAuthorize("TRH_STATEMENT > select")]
        public string Ajax_get_statement_no(TRH_STATEMENT objecttemp)
        {
            TRH_STATEMENT stmt = new TRH_STATEMENT();
            var maxValue = Db.TRH_STATEMENT.Where(x => x.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Max(x => x.STMT_NO);
            return (maxValue.ToString());
        }

        [HttpPost]
        [EntityAuthorize("TRH_STATEMENT_ROW > select")]
        public string Ajax_get_statement_row(TRH_STATEMENT_ROW objecttemp)
        {
            var query = (from b in Db.TRH_STATEMENT_ROW
                         where (b.PRCL_PRCL_ID == objecttemp.PRCL_PRCL_ID && b.STMT_STMT_ID == objecttemp.STMT_STMT_ID)
                         select b).Count();

            return (query.ToString());
        }

        [EntityAuthorize("TRH_STATEMENT_ROW > select")]
        public string Ajax_get_statement_draft(TRH_STATEMENT_ROW objecttemp)
        {
            string data = "0";
            var query = from b in Db.PRN_PDRF_WORK
                        join c in Db.PRN_PDRF_OPER on b.ID equals c.PDRW_ID
                        where (b.STMT_STMT_ID == objecttemp.STMT_STMT_ID && b.ID == c.PDRW_ID)
                        select new { c.AMNT };

            decimal? price = Db.Database.SqlQuery<decimal>(string.Format("SELECT prn_current_stmr_u('{0}') FROM DUAL", objecttemp.STMT_STMT_ID)).FirstOrDefault();
            if (price == query.Select(xx => xx.AMNT).Sum())
            {
                data = "1";
            }

            return (data);
        }

        [EntityAuthorize("CGT_LETTER > select")]
        public string Ajax_get_statement_draft2(CGT_LETTER objecttemp)
        {
            string data = "0";
            var query = from b in Db.PRN_PDRF_WORK
                        join c in Db.PRN_PDRF_OPER on b.ID equals c.PDRW_ID
                        where (b.CLET_LET_CODE == objecttemp.LET_CODE && b.ID == c.PDRW_ID)
                        select new { c.AMNT };

            long? let_amnt = Db.CGT_LETTER.Where(xx => xx.LET_CODE == objecttemp.LET_CODE).Select(xx => xx.LET_AMNT).FirstOrDefault();
            if (Convert.ToDecimal(let_amnt) == query.Select(xx => xx.AMNT).Sum())
            {
                data = "1";
            }

            return (data);
        }
        public string Ajax_get_statement_draft_current(CGT_LETTER objecttemp)
        {
            string data = "0";
            var query = from c in Db.PRN_PDRF_OPER
                        where c.CLET_LET_CODE == objecttemp.LET_CODE
                        select new { c.AMNT };

            long? let_amnt = Db.CGT_LETTER.Where(xx => xx.LET_CODE == objecttemp.LET_CODE).Select(xx => xx.LET_AMNT).FirstOrDefault();
            if (Convert.ToDecimal(let_amnt) == query.Select(xx => xx.AMNT).Sum())
            {
                data = "1";
            }

            return (data);
        }
        [EntityAuthorize("TRH_STATEMENT> select")]
        public string Ajax_get_statement_id(TRH_STATEMENT objecttemp)
        {
            TRH_STATEMENT stmt = new TRH_STATEMENT();
            var maxid = Db.TRH_STATEMENT.Where(x => x.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Max(x => x.STMT_ID);
            return (maxid.ToString());
        }

        [HttpPost]
        [EntityAuthorize("CGT_LETTER > select")]
        public string Ajax_get_end_letter(CGT_LETTER objecttemp)
        {
            var cnt = Db.CGT_LETTER.Where(x => x.CCNT_CNTR_NO == objecttemp.CCNT_CNTR_NO).Where(x => x.LET_TYPE == "17").Count();
            if (cnt == null)
                cnt = 0;
            return (cnt.ToString());
        }

        [HttpPost]
        [EntityAuthorize("PRN_PDRF_WORK > select")]
        public string Ajax_PDRF_work(CGT_LETTER objecttemp)
        {
            var v_count = Db.PRN_PDRF_WORK.Where(xx => xx.CLET_LET_CODE == objecttemp.LET_CODE).Select(xx => xx.SUBU_TYPE).Distinct().Count();
            if (v_count > 1)
                v_count = 0;
            return (v_count.ToString());
        }

        //////////////////////////////////////////////////////////Report
        public ActionResult WORK_REPORT()
        {
            return View("TRH_WORK_TYPE_R");
        }

        public ActionResult CHAPTER_REPORT()
        {
            return View("TRH_CHAPTER_R");
        }

        public ActionResult INDEX_REPORT()
        {
            return View("TRH_INDEX_R");
        }

        public ActionResult ITEM_REPORT()
        {
            return View("TRH_ITEM_R");
        }

        public ActionResult PRICE_REPORT()
        {
            return View("TRH_PRICE_R");
        }

        public ActionResult STATEMENT_REPORT()
        {
            return View("TRH_STATEMENT_R");
        }

        //[EntityAuthorize("CGT_LETTER > select")]
        //public ActionResult ShowLetter(int id, int notId)
        //{
        //    BandarEntities dataContext = new BandarEntities();
        //    var model = dataContext.CGT_LETTER.Find(id);
        //    return View(model);
        //}

        //~TarhController()
        //        {
        //            //این دستور کانکشن را دیسکانکت میکند
        //            Db.Dispose();
        //        }

    }

    public class MyClass
    {
        public short ST_ID { get; set; }
        public string STND_DESC { get; set; }
    }

}

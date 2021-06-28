using System.Linq;
using System.Web.Mvc;
using Asr.Base;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace Equipment.Controllers.Report
{
    [Authorize]
    [Developer("H.Hamidi")]
    public partial class ReportController : Controller
    {

        //BandarEntities cntx;
        //public ReportController()
        //{
        //    cntx = this.DB();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        cntx.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        ////****************************************************************************************

        public ActionResult Report_BarTransEnteghal() // گزارش بار ترانس انتقال  
        {
            return View("Bar_Trans_Enteghal");
        }

        public ActionResult Report_DasteBandi_TedadKhoruji_2() // گزارش دسته بندی تعداد خروجی - فرم شماره 2
        {
            return View("DasteBandi_TedadKhoruji_2");
        }

        public ActionResult Report_Shenasname_PostAndTajhiz() // گزارش شناسنامه کامل تجهیزات و پست
        {
            return View("Shenasname_PostAndTajhiz");
        }

        public ActionResult Report_Post_List() // گزارش لیست پست ها
        {
            return View();
            //return Redirect(Url.Action("Show", "Report", new { Area = "Reporting" }) + "?rep=203&param=&vars=&options=null");            
        }

        public ActionResult Report_PublicPostInfo() // گزارش اطلاعات عمومی پست بر اساس نوع پست 
        {
            return View();
            //return Redirect(Url.Action("Show", "Report", new { Area = "Reporting" }) + "?rep=508&param=&vars=&options=null");
        }

        public ActionResult Report_PostInstrument() // گزارش تجهیزات پست
        {
            return View();
            //return Redirect(Url.Action("Show", "Report", new { Area = "Reporting" }) + "?rep=507&param=&vars=&options=null");
        }
        //
        public ActionResult Report_Attribute_List() // گزارش لیست صفات
        {
            //return Redirect(Url.Action("Show", "Report", new { Area = "Reporting" }) + "?rep=490&param=&vars=&options=null");
            //return Redirect(url);
            return View();
        }

        public ActionResult Report_Tip_List() // گزارش لیست تیپ ها
        {
            return View();
            //return Redirect(Url.Action("Show", "Report", new { Area = "Reporting" }) + "?rep=493&param=&vars=&options=null");
        }

        public ActionResult Report_PostBay() // گزارش اطلاعات پست بر اساس بی ها
        {
            return View();
            //return Redirect(Url.Action("Show", "Report", new { Area = "Reporting" }) + "?rep=509&param=&vars=&options=null");
        }

        public ActionResult Report_Instrument_List() // گزارش لیست تجهیزات
        {
            return View();
            //return Redirect(Url.Action("Show", "Report", new { Area = "Reporting" }) + "?rep=494&param=&vars=&options=null");
        }

        public ActionResult Report_LineList() // گزارش لیست خطوط
        {
            return View();
            //return Redirect(Url.Action("Show", "Report", new { Area = "Reporting" }) + "?rep=670&param=&vars=&options=null");
        }

        public ActionResult Report_LineListByLenght() // گزارش لیست خطوط بر اساس طول مدار
        {            
            return View();
            //return Redirect(Url.Action("Show", "Report", new { Area = "Reporting" }) + "?rep=671&param=&vars=&options=null");
        }

        //public ActionResult Report_LineAmar()
        //{
        //    //return Redirect(Url.Action("Show", "Report", new { Area = "Reporting" }) + "?rep=668&param=&vars=&options=null");
        //}

        //*************
        //public ActionResult Report_InstrumentByFactory() // گزارش تجهیزات بر اساس کارخانه
        //{
        //    return View();
        //}

        //public ActionResult Report_TipGroup_Instrument() // گزارش تیپ گروه تجهیز انتخاب شده
        //{
        //    return View();
        //}

        //public ActionResult Report_AttributeGroup_Instrument() // گزارش صفات گروه تجهیز انتخاب شده
        //{
        //    return View();
        //}

        //public ActionResult Report_InstrumentParameter() // گزارش تجهیزات پارامتری
        //{
        //    return View();
        //}

        public ActionResult Report_PostDetail_Bahrebardari(string state) // گزارش کلی پست - مشخصات تعداد و ظرفیت پست های بهره برداری شده
        {
            ViewBag.fromStatistics = string.IsNullOrEmpty(state) ? "0" : "1";
            return View();
        }

        public ActionResult Report_AmpedanceLine() // گزارش امپدانس خط
        {
            return View();
        }

        public ActionResult Report_CompletePost_S1() // گزارش اطلاعات تجهيزات پست ها - S1
        {
            return View();
        }

        public ActionResult Report_CompletePost_S2() // گزارش اطلاعات تجهيزات پست ها - S2
        {
            return View();
        }

        public ActionResult Report_CompletePost_S3() // گزارش اطلاعات تجهيزات پست ها - S3
        {
            return View();
        }

        public ActionResult Report_CompleteLine_L1() // گزارش کلی خط L1
        {
            return View();
        }

        public ActionResult Report_CompleteLine_L2(string state) // گزارش کلی خط L2
        {
            ViewBag.fromStatistics = string.IsNullOrEmpty(state) ? "0" : "1";
            return View();
        }

        public ActionResult Report_CompleteLine_L3() // گزارش کلی خط L3
        {
            return View();
        }

        public ActionResult Report_PostDetailFilter() // گزارش اطلاعات پست ها به همراه فیلتر های جستجو
        {
            return View();
        }

        public ActionResult Report_LineDetailFilter() // گزارش اطلاعات خط ها به همراه فیلتر های جستجو
        {
            return View();
        }

        public ActionResult Report_PostFullDetailFilter() // گزارش اطلاعات پست ها به همراه جزئیات با فیلتر های جستجو
        {
            return View();
        }

        public ActionResult Report_LineFullDetailFilter(string state) // گزارش اطلاعات خط ها به همراه جزئیات با فیلتر های جستجو
        {
            ViewBag.fromStatistics = string.IsNullOrEmpty(state) ? "0" : "1";
            return View();
        }

        public ActionResult Report_FiderDetail() // گزارش اطلاعات خطوط 20 کیلوولت - فیدر
        {            
            return View();
        }

        public ActionResult Report_InstrumentListInPost() // گزارش آماری و گزارش جزئیات تجهیزات مربوط در پست _ 2 گزارش جداگانه
        {
            return View();
        }

        public ActionResult Report_TipListAndInstrument() // گزارش اطلاعات تجهیزات بر اساس تیپ و کارخانه سازنده آنها به همراه فیلترهای جستجو
        {
            return View();
        }

        public ActionResult Report_RelayDetailList() // گزارش اطلاعات رله ها به همراه فیلترهای جستجو
        {
            return View();
        }

        public ActionResult Report_CountOfRelay() // گزارش آماری تعداد رله ها
        {
            return View();
        }

        public ActionResult Report_PostDetail_Amari(string state) // گزارش آماری پست - تعداد و ظرفیت پست ها
        {
            ViewBag.fromStatistics = string.IsNullOrEmpty(state) ? "0" : "1";
            return View();
        }

        public ActionResult Report_LineDetail_Amari(string state) // گزارش آماری خط - تعداد ، طول و جریان مجاز خطوط
        {
            ViewBag.fromStatistics = string.IsNullOrEmpty(state) ? "0" : "1";
            return View();
        }

        public ActionResult Report_Request_Amari() // گزارش آماری درخواست انجام کار - تعداد درخواست های گروه و افراد گروه
        {
            return View();
        }

        public ActionResult Report_Request_Amari_Details() // گزارش آماري درخواست هاي ايجاد شده فعال روي گروه هاي اجرايي
        {
            return View();
        }

        public ActionResult Report_RequestConfirm_Amari() // گزارش آماری درخواست های تایید شده/نشده
        {
            return View();
        }

        public ActionResult Report_RequestDetails() // گزارش ریز جزئیات درخواست انجام کار
        {
            return View();
        }

        public ActionResult Report_RequestShortDetails() // گزارش بخشی از جزئیات درخواست انجام کار
        {
            return View();
        }

        public ActionResult Report_RequestCancel() // گزارش درخواست های کنسل شده
        { 
            return View();
        }

        public ActionResult Report_RequestPost_Amari() // گزارش آماری و جزئیات درخواست انجام کارهای پست ها
        {
            return View();
        }

        public ActionResult Report_Request_OrganAndPostManager_Amari() // گزارش آماری درخواست های متوقف شده در کارتابل رئیس ناحیه و مسئول پست
        {
            return View();
        }

        public ActionResult Report_TimeProgramming() // گزارش برنامه زمانبندی به همراه فیلترهای جستجو
        {
            return View();
        }

        public ActionResult Report_RunTimeProgramming() // گزارش برنامه زمانبندی اجرا شده / نشده (مربوط به درخواست انجام کار) همراه فیلترهای جستجو
        {
            return View();
        }

        public ActionResult Report_Instrument_PriceList() // گزارش فهرست بهای تجهیزات
        {
            return View();
        }

        public ActionResult Report_Capacity_Transformers(string state) // گزارش تعداد و ظرفيت ترانسفورماتورها به تفکيک ولتاژ
        {
            ViewBag.fromStatistics = string.IsNullOrEmpty(state) ? "0" : "1";
            return View();
        }

        public ActionResult Report_InstrumentDetails() // گزارش مشخصه های تجهیزات
        {
            return View();
        }

        public ActionResult Report_CountOfSpecialInstrument() // گزارش آماری تعداد تجهیزات خاص
        {
            return View();
        }

        public ActionResult Report_InstruLimit() // گزارش محدودیت تجهیزات
        {
            return View();
        }

        public ActionResult Report_InstruLimit_Amari() // گزارش آماری محدودیت تجهیزات
        {
            return View();
        }

        public ActionResult Report_NoteBookInfo_Amari() // گزارش آماری و جزئیات شناسنامه عملیاتی پست ها
        {
            return View();
        }

        public ActionResult Report_Post_Grade() // گزارش رتبه بندی پست ها
        {
            return View();
        }

        //
        //************************* Start Function *************************
        // Function
        
        public System.Collections.Generic.IEnumerable<int> GetYearsToNow(int startYear)
        {
            System.Globalization.PersianCalendar pCalendar = new System.Globalization.PersianCalendar();
            int nowYear = pCalendar.GetYear(System.DateTime.Now);
            while (startYear < nowYear)
                yield return ++startYear;
        }
        //************************* End Function ****************************
        //



        //
        //********************** Start Function View ***********************
        // Function View

        public ActionResult GetPostListView()
        {
            var query = (from b in cntx.EXP_POST_V
                         where b.EXP_POST_LINE_EPOL_TYPE == "0"
                         select new
                         {
                             b.EXP_POST_LINE_EPOL_ID,
                             b.EXP_POST_LINE_EPOL_NAME,
                             b.EXP_UNIT_LEVEL_EUNL_NUM
                         }).Distinct().OrderByDescending(x=>x.EXP_UNIT_LEVEL_EUNL_NUM).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPostListView_WithOrgan(string OrganCode)
        {
            var query = (from b in cntx.EXP_POST_V
                         where b.EXP_POST_LINE_EPOL_TYPE == "0" && b.EXP_POST_LINE_ORGA_CODE == OrganCode
                         select new
                         {
                             b.EXP_POST_LINE_EPOL_ID,
                             b.EXP_POST_LINE_EPOL_NAME,
                             b.EXP_UNIT_LEVEL_EUNL_NUM
                         }).Distinct().OrderByDescending(x=>x.EXP_UNIT_LEVEL_EUNL_NUM).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrganListView()
        {
            var query = (from b in cntx.PAY_ORGAN
                         where b.MANA_ASTA_CODE == "7" && b.MANA_CODE == "6" && b.ORGA_STAT == "2" && b.CODE != "5"
                         select new
                         {
                             b.CODE,
                             b.ORGN_DESC
                         }).Distinct().ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrganByAnalyzorListView()
        {
            int[] array = { 11, 12, 13, 14, 15, 16 };
            var query = (from b in cntx.EXP_ANALYZOR_EVENT
                         where array.Contains(b.EANA_ROW)
                         select new
                         {
                             EANA_ROW = b.EANA_ROW,
                             EANA_DESC = b.EANA_DESC.Substring(7)
                         }).Distinct().ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomVoltListView()
        {
            int[] array = { 1, 62, 63, 161, 201 };
            var query = (from b in cntx.EXP_UNIT_LEVEL
                         where array.Contains(b.EUNL_ID)
                         orderby b.EUNL_NUM descending
                         select new
                         {
                             b.EUNL_ID,
                             b.EUNL_DESC
                         }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetVoltListView()
        {
            int[] array = { 1, 62, 63, 161, 201, 341 };
            var query = (from b in cntx.EXP_UNIT_LEVEL
                         where array.Contains(b.EUNL_ID)
                         orderby b.EUNL_NUM descending
                         select new 
                         {
                             b.EUNL_ID,
                             b.EUNL_DESC 
                         }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllVoltListView()
        {
            var query = (from b in cntx.EXP_UNIT_LEVEL
                         select new
                         {
                             b.EUNL_ID,
                             b.EUNL_DESC
                         }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult GetInstrumentListView()
        {
            var query = (from b in cntx.EXP_INSTRUMENT
                         select new
                         {
                             b.EINS_ID,
                             b.EINS_DESC
                         }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInstrumentFilterListView(bool withRele = false)
        {
            int?[] array;
            if (withRele)
            {
                array = new int?[] { 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 13, 23, 24, 30, 259, 62, 281, 282, 367 };
            }
            else
            {
                array = new int?[] { 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 13, 23, 24, 30, 259, 62, 281, 282 };
            }
            var query = (from b in cntx.EXP_INSTRUMENT
                         where array.Contains(b.EINS_ID)
                         select new
                         {
                             b.EINS_ID,
                             b.EINS_DESC
                         }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);            
        }

        public ActionResult GetSpecialInstrumentListView()
        {
            int?[] array;
            array = new int?[] { 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 13, 23, 24, 30, 56, 57, 62, 169, 232, 259, 281, 282, 327 };
            var query = (from b in cntx.EXP_INSTRUMENT
                         where array.Contains(b.EINS_ID)
                         select new
                         {
                             b.EINS_ID,
                             b.EINS_DESC
                         }).ToList();

            query.Insert(query.Count, new
            {
                EINS_ID = 0,
                EINS_DESC = "پست"
            });

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInstrumentPriceListView()
        {
            int?[] array;
            array = new int?[] { 2, 3, 7, 8, 9, 10, 11, 12, 13, 24, 62, 230, 259, 281, 282 };
            var query = (from b in cntx.EXP_INSTRUMENT
                         where array.Contains(b.EINS_ID)
                         select new
                         {
                             b.EINS_ID,
                             b.EINS_DESC
                         }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet); 
        }

        public ActionResult GetInstrumentCustomListView()
        {
            int?[] array;
            array = new int?[] { 2, 3, 4, 7, 8, 9, 10, 11, 12, 13, 24, 62, 259 };
            var query = (from b in cntx.EXP_INSTRUMENT
                         where array.Contains(b.EINS_ID)
                         select new
                         {
                             b.EINS_ID,
                             b.EINS_DESC
                         }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInstrumentWithAttribListView()
        {
            var query = (from b in cntx.EXP_INSTRUMENT
                         join c in cntx.EXP_ATTRIBUTE on b.EINS_ID equals c.EINS_EINS_ID
                         select new
                         {
                             b.EINS_ID,
                             b.EINS_DESC
                         }).Distinct().OrderBy(x => x.EINS_ID).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFactoryListView()
        {
            var query = (from b in cntx.EXP_FACTORY
                        select new
                        {
                            b.EFAC_ID,
                            b.EFAC_DESC
                        }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTipListView()
        {
            var query = (from b in cntx.EXP_TYPE_EQUIP
                         where b.ETEX_TYPE == "1"
                         select new
                         {
                             b.ETEX_ID,
                             b.ETEX_DESC
                         }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetYears()
        {
            var query = (from b in GetYearsToNow(1385)
                         select new
                         {
                             Text = b,
                             Value = b
                         }).OrderByDescending(x => x.Text).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetYears_TimeProgramming()
        {
            var query = (from b in cntx.EXP_EXPI_DOC
                         where b.ETDO_ETDO_ID == 101
                         select new { b.EEDO_YEAR }).OrderByDescending(x => x.EEDO_YEAR).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOwnerListView()
        {
            var query = (from b in cntx.EXP_OWENER_COMPANY
                         select new
                         {
                             b.EOCO_ID,
                             b.EOCO_DESC
                         }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_AOC_RDC()
        {
            var query = (from b in cntx.EXP_AOC_RDC
                         orderby b.EARD_NAME
                         select new
                         {
                             b.EARD_ID,
                             b.EARD_NAME
                         }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOwnerTypeListView()
        {
            var query = (from b in cntx.EXP_OWENER_TYPE
                         orderby b.EOTY_ID
                         select new
                         {
                             b.EOTY_ID,
                             b.EOTY_DESC
                         }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet); 
        }

        public ActionResult GetGroupExecutiveListView()
        {           
            var query = (from b in cntx.EXP_ANALYZOR_EVENT
                         where b.EANA_TYPE == 1 && b.EANA_ROW != 10
                         orderby b.EANA_DESC
                         select new
                         {
                             b.EANA_ROW,
                             b.EANA_DESC
                         }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet); 
        }

        public ActionResult GetAllPrograms(int? id)
        {
            var query = (from b in cntx.EXP_PROGRAM
                         where b.ETDO_ETDO_ID == id.Value
                         select new
                         {
                             b.EPRO_ID,
                             b.EPRO_DESC
                         }).OrderBy(x=> x.EPRO_DESC).ToList();
            return Json(query, JsonRequestBehavior.AllowGet); 
        }

        public ActionResult GetAllFunction(int? id)
        {
            var query = (from b in cntx.EXP_PFUNCTION
                         where b.EPRO_EPRO_ID == id.Value
                         select new
                         {
                             b.EFUN_ID,
                             b.EFUN_DESC
                         }).OrderBy(x => x.EFUN_DESC).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllTypeBayListView()
        {
            var query = (from b in cntx.EXP_TYPE_BAY                        
                         select new
                         {
                             b.ETBY_ID,
                             b.ETBY_DESC
                         }).OrderBy(x => x.ETBY_ID).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllGroupInstru()
        {
            var query = (from b in cntx.EXP_TYPE_INSTRU
                         select new
                         {
                             b.ETYI_ID,
                             b.ETYI_DESC
                         }).OrderBy(x => x.ETYI_ID).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllStateInstru()
        {
            var query = (from b in cntx.EXP_STATE_INSTRU
                         select new
                         {
                             b.ESIN_ID,
                             b.ESIN_DESC
                         }).OrderBy(x => x.ESIN_ID).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllReasonOfCancel()
        {
            var query = (from b in cntx.EXP_CANCEL_RESN
                         select new
                         {
                             b.ECRE_ID,
                             b.ECRE_DESC
                         }).OrderBy(x => x.ECRE_ID).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPostByPostGroup([DataSourceRequest] DataSourceRequest request,short? groupId)
        {
            var query = (from b in cntx.EXP_POST_LINE
                         join c in cntx.EXP_POST_GROUP on b.EPOL_ID equals c.EPOL_EPOL_ID
                         join d in cntx.EXP_UNIT_LEVEL on b.EUNL_EUNL_ID equals d.EUNL_ID
                         orderby d.EUNL_NUM descending
                         where b.EPOL_TYPE == "0" && b.EPOL_STAT == "1" && c.GROP_GROP_ID == groupId
                         select new
                         {
                             b.EPOL_ID,
                             b.EPOL_NAME,
                             b.CODE_DISP,
                             postVolt = d.EUNL_DESC
                         }).ToList();
            return Json(query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);            
        }

        //public ActionResult GetAllPostList()
        //{            
        //    var query = (from b in cntx.EXP_POST_LINE.AsEnumerable()
        //                 where (System.Convert.ToInt32(b.EPOL_TYPE) == 0)
        //                 select new
        //                 {
        //                     b.EPOL_ID,
        //                     b.EPOL_NAME
        //                 }).ToList();
        //    return Json(query, JsonRequestBehavior.AllowGet);
        //}

        //******************** End Function View *********************
    }
}

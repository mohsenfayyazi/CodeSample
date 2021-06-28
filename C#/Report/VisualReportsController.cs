using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Asr.Base;

namespace Equipment.Controllers.Report
{
    // ********** Need Field For Chart ************
    public class ChartItemInfo
    {
        public string DisplayName { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public int Order { get; set; }
        /// <summary>
        /// Data:
        /// 0 = طرح و توسعه ,
        /// 1 = بهره برداری ,
        /// 2 = برنامه ریزی ,
        /// 3 = منابع انسانی ,
        /// 4 = مالی ,
        /// 5 = بازرگانی
        /// </summary>
        public int Type { get; set; }
        public string DisplayComment { get; set; }
        public string ReportId { get; set; }
        public string ReportUrl { get; set; }
    }
    //**********************************************
    //*********** Insert Data For Chart ************

    [Authorize]
    [Developer("H.Hamidi")]
    public partial class VisualReportsController : DbController
    {

        List<ChartItemInfo> charts = new List<ChartItemInfo>();

        public VisualReportsController()
        {
            charts.AddRange(new ChartItemInfo[] 
            {
                new ChartItemInfo{DisplayName="1چارت" , ActionName="Actn1" , ControllerName="VisualReports", Order=1 , Type=0 , DisplayComment="" , ReportId="200" , ReportUrl="rep=2194&param=&vars=&options="},
                new ChartItemInfo{DisplayName="چارت2" , ActionName="Actn2" , ControllerName="VisualReports", Order=2 , Type=0 , DisplayComment="" , ReportId="300" , ReportUrl="rep=2194&param=&vars=&options="},
                new ChartItemInfo{DisplayName="چارت3" , ActionName="Actn3" , ControllerName="VisualReports", Order=3 , Type=2 , DisplayComment="" , ReportId="400" , ReportUrl="rep=2194&param=&vars=&options="},
                new ChartItemInfo{DisplayName="تستی" , ActionName="Test" , ControllerName="VisualReports", Order=3 , Type=0 , DisplayComment="" , ReportId="500" , ReportUrl="rep=2194&param=&vars=&options="}
            });
        }

        // *****************************
        // ******* test ********

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult Test2()
        {
            return View();
        }

        public ActionResult TestSavePic()
        {
            return View();
        }

        public ActionResult Export()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);

        }

        //*********************************
        // ******* Start Action ********

        public ActionResult Index()
        {
            return View();
        }

        public void FillViewBag(string type)
        {
            switch (type)
            {
                case "0":
                    ViewBag.SubSystemName = "بخش طرح و توسعه";
                    //ViewBag.IconSubSystem = "glyphicon glyphicon-signal";
                    ViewBag.IconSubSystem = "glyphicon glyphicon-transfer";
                    break;
                case "1":
                    ViewBag.SubSystemName = "بخش بهره برداری";
                    //ViewBag.IconSubSystem = "glyphicon glyphicon-th-large";
                    ViewBag.IconSubSystem = "glyphicon glyphicon-indent-left";
                    break;
                case "2":
                    ViewBag.SubSystemName = "بخش برنامه ریزی";
                    ViewBag.IconSubSystem = "glyphicon glyphicon-calendar";
                    break;
                case "3":
                    ViewBag.SubSystemName = "بخش منابع انسانی";
                    ViewBag.IconSubSystem = "glyphicon glyphicon-user";
                    break;
                case "4":
                    ViewBag.SubSystemName = "بخش مالی";
                    ViewBag.IconSubSystem = "glyphicon glyphicon-usd";
                    break;
                case "5":
                    ViewBag.SubSystemName = "بخش بازرگانی";
                    //ViewBag.IconSubSystem = "glyphicon glyphicon-folder-open";
                    ViewBag.IconSubSystem = "glyphicon glyphicon-level-up";
                    break;
                default:
                    ViewBag.SubSystemName = "همه زیر سیستم ها";
                    //ViewBag.IconSubSystem = "glyphicon glyphicon-option-horizontal";
                    ViewBag.IconSubSystem = "glyphicon glyphicon-stats";
                    // ViewBag.Details = charts.OrderBy(x => x.Order);
                    break;
            }
        }

        public ActionResult ListOfChart(string idSubSystem)
        {
            List<string> valisKeys = new List<string>() { "0", "1", "2", "3", "4", "5", "" };
            bool noFilter = string.IsNullOrEmpty(idSubSystem) || !valisKeys.Contains(idSubSystem);
            int sbstm = noFilter ? 0 : Convert.ToInt32(idSubSystem);
            idSubSystem = idSubSystem.Trim();
            FillViewBag(idSubSystem);
            var q = charts.AsEnumerable()
                .Where(x =>
                (x.Type == sbstm) || noFilter
                ).ToList().OrderBy(x => x.Order);
            ViewBag.Details = q;
            ViewBag.CountReport = q.Count();

            //var query = (from b in cntx.EXAMPLE_REPORT_VISUAL
            //             where b.SUBSYS_TYPE == idSubSystem
            //             orderby b.ORDER_CODE
            //             select new
            //             {
            //                 b.RPTVS_ID,
            //                 b.RPTVS_NAME,
            //                 b.RPTVS_DESC,
            //                 b.CNTRL_NAME,
            //                 b.ACTN_NAME
            //             }).ToList();

            //ViewBag.Details = query;
            //ViewBag.Details = cntx.EXAMPLE_REPORT_VISUAL.OrderBy(x => x.ORDER_CODE).ToList();
            return View();
        }


        public ActionResult Actn1()
        {
            return PartialView();
        }

        public ActionResult Actn2()
        {
            return PartialView();
        }

        public ActionResult Actn3()
        {
            return PartialView();
        }

        public ActionResult Actn4()
        {
            return PartialView();
        }

        public ActionResult Actn5()
        {
            return PartialView();
        }

        public ActionResult Actn6()
        {
            return PartialView();
        }

        public ActionResult Actn7()
        {
            return PartialView();
        }

        public ActionResult Actn8()
        {
            return PartialView();
        }

        //***********************************
        //********* Insert Form Test ***********

        public ActionResult InsertChartDetail()
        {
            return View();
        }

        public ActionResult ChartPieAction()
        {
            return View();
        }

        public ActionResult ChartLineAction()
        {
            return View();
        }

        public ActionResult ChartBarAction()
        {
            return View();
        }

        public ActionResult show()
        {
            var q1 = (from b in Db.EXP_POST_LINE
                      where b.EPOL_TYPE == "0"
                      select b).Count();
            var q2 = (from b in Db.EXP_POST_LINE
                      where b.EPOL_TYPE == "1"
                      select b).Count();
            var q3 = (from b in Db.EXP_POST_LINE
                      where b.EPOL_TYPE == "2"
                      select b).Count();

            ViewBag.q1 = q1;
            ViewBag.q2 = q2;
            ViewBag.q3 = q3;

            return View();
        }

        public ActionResult DataLineChart()
        {
            var query = (from b in Db.EXP_POST_LINE
                         group b by b.EPOL_TYPE into x
                         //select new
                         //{
                         //    type = x.Key,
                         //    count = x.Count
                         //};            
                         select x).ToList();

            //IEnumerable<EXP_POST_LINE> query = SomeRepository.GetData();

            return Json(query);
        }

    }

}

using Equipment.DAL;
using Equipment.Reporting;
using Kendo.Mvc.UI;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Planning.Earth
{
    [Authorize]
    public partial class EarthController : Controller
    {
        /// <summary>
        /// Return Report(Print) Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Print(string id)
        {
            id = id.Replace('-', '/');
            object model = id;
            return View("Print", model);
        }

        /// <summary>
        /// EAR_EARTH PRINT DATA
        /// </summary>
        /// <returns></returns>
        public ActionResult GetReportSnapshot()
        {
            try
            {
                DataSourceRequest request = (DataSourceRequest)Session["Earth-Grid-Filters"];
                StiReport myreport = DatabeseReport.GetReport(41);
                using (var cntx = PublicRepository.GetNewDatabaseContext)
                {
                    myreport.RegBusinessObject("Equipment", "Earth", cntx.EAR_EARTH.ApplyEarthFiltering(request.Filters).ApplyEarthSorting(request.Groups, request.Sorts).ToList());
                    //myreport.Dictionary.SynchronizeBusinessObjects(2);
                }
                return StiMvcViewer.GetReportSnapshotResult(HttpContext, myreport);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// Earth check list Tamplate Print
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetReportChecklistTemplate(int id)
        {
            try
            {
                StiReport myreport = DatabeseReport.GetReport(48);
                //using (var cntx = this.DB())
                //{
                //var ccc = new BandarEntities();

                myreport.RegBusinessObject("Equipment", "EAR_EARTH_CHK_TMP", (from b in cntx.EAR_EARTH_CHK_TMP
                                                                              where b.ECHT_ID == id
                                                                              select new
                                                                                   {
                                                                                       b.ECHT_ID,
                                                                                       b.ECHT_DESC
                                                                                   }).ToList());

                myreport.RegBusinessObject("Equipment", "EAR_CHECK_LIST_ROW_TMP", (from b in cntx.EAR_CHECK_LIST_ROW_TMP
                                                                                   where b.ECHT_ECHT_ID == id
                                                                                   select b).AsEnumerable().Select(b => new
                                                                                   {
                                                                                       b.CHTR_ID,
                                                                                       b.CHTR_ROW,
                                                                                       b.CHTR_DESC,
                                                                                       b.CHTR_WEIGHT,
                                                                                       b.CHTR_VALUE,
                                                                                       //PRSN = string.Format("{0} {1}\n({2})", b.PAY_PERSONEL.FIRS_NAME, b.PAY_PERSONEL.FAML_NAME, b.PAY_ORGAN.ORGA_DESC)
                                                                                   }).OrderBy(z => z.CHTR_ROW).ToList());
                //}
                return StiMvcViewer.GetReportSnapshotResult(HttpContext, myreport);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// Earth check list Print
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetReportChecklist(int id)
        {
            try
            {
                StiReport myreport = DatabeseReport.GetReport(49);
                //using (var cntx = this.DB())
                //{
                myreport.RegBusinessObject("Equipment", "EAR_EARTH_CHECK_LIST", (from b in cntx.EAR_EARTH_CHECK_LIST
                                                                                 where b.ECHL_ID == id
                                                                                 select new
                                                                                 {
                                                                                     b.ECHL_ID,
                                                                                     b.ECHL_DESC
                                                                                 }).ToList());

                myreport.RegBusinessObject("Equipment", "EAR_CHECK_LIST_ROW", (from b in cntx.EAR_CHECK_LIST_ROW
                                                                               where
                                                                                    b.ECHL_ECHL_ID == id
                                                                               select b).AsEnumerable().Select(b => new
                                                                               {
                                                                                   b.CHLR_ID,
                                                                                   b.CHLR_ROW,
                                                                                   b.CHLR_DESC,
                                                                                   b.CHLR_WEIGHT,
                                                                                   b.CHLR_VALUE,
                                                                                   PRSN = string.Format("{0} {1}",
                                                                                       b.PAY_PERSONEL.FIRS_NAME,
                                                                                       b.PAY_PERSONEL.FAML_NAME)
                                                                               }).OrderBy(z => z.CHLR_ROW).ToList());
                //}
                return StiMvcViewer.GetReportSnapshotResult(HttpContext, myreport);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult ViewerEvent()
        {
            return StiMvcViewer.ViewerEventResult(this.HttpContext);
        }

        public ActionResult PrintReport()
        {
            return StiMvcViewer.PrintReportResult(this.HttpContext);
        }

        public ActionResult ViewerExportReport()
        {
            return StiMvcViewerFx.ExportReportResult(this.Request);
        }

        public ActionResult Interaction()
        {
            return StiMvcViewer.InteractionResult(this.HttpContext);
        }

    }

}
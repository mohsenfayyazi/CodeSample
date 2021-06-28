using Equipment.DAL;
using Equipment.Reporting;
using Kendo.Mvc.UI;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Planning.Estimate
{
    [Authorize]
    public partial class EstimateController : Controller
    {
        public ActionResult PrintEstimate(string id)
        {
            switch (id.Trim())
            {
                //مراکز مصرف
                case "EstimatePrint":
                    {
                        object model = "EstimatePrint";
                        return View("PrintEstimate", model);
                    }
                case "OperationPrint":
                    {
                        object model = "OperationPrint";
                        return View("PrintEstimate", model);
                    }
            }
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult EstimatePrint()
        {
            try
            {
                DataSourceRequest request = (DataSourceRequest)Session["EstEstimate-Grid-Filters"];
                StiReport myreport = Reporting.DatabeseReport.GetReport(46);
                using (var cntx = PublicRepository.GetNewDatabaseContext)
                {
                    myreport.RegBusinessObject("Equipment", "EST_ESTIMATE", cntx.EST_ESTIMATE
                                                                                .ApplyEstEstimateFiltering(request.Filters)
                                                                                .ApplyEstEstimateSorting(request.Groups, request.Sorts)
                                                                                .AsEnumerable()
                                                                                .Select(s => new
                                                                                {
                                                                                    s.ESMT_ID,
                                                                                    EOCO = s.EXP_OWENER_COMPANY.EOCO_DESC,
                                                                                    s.FINY_FINY_YEAR,
                                                                                    s.ESMT_DIMAND
                                                                                })
                                                                                .ToList());
                }
                return StiMvcViewer.GetReportSnapshotResult(HttpContext, myreport);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult OperationPrint()
        {
            try
            {
                DataSourceRequest request = (DataSourceRequest)Session["EstOperation-Grid-Filters"];
                StiReport myreport = DatabeseReport.GetReport(47);
                using (var cntx = PublicRepository.GetNewDatabaseContext)
                {
                    myreport.RegBusinessObject("Equipment", "EST_OPERATION", cntx.EST_OPERATION
                                                                                 .ApplyEstOperationFiltering(request.Filters)
                                                                                 .ApplyEstOperationSorting(request.Groups, request.Sorts)
                                                                                 .AsEnumerable()
                                                                                 .Select(p => new
                                                                                 {
                                                                                     p.OPRN_ID,
                                                                                     EOCO = p.EXP_OWENER_COMPANY.EOCO_DESC,
                                                                                     p.FINY_FINY_YEAR,
                                                                                     p.OPRN_PIC,
                                                                                     p.OPRN_PRIOD,
                                                                                     p.OPRN_ENERGI,
                                                                                     p.OPRN_TYPE
                                                                                 })
                                                                                 .ToList());
                }
                return StiMvcViewer.GetReportSnapshotResult(HttpContext, myreport);
            }
            catch (Exception ex)
            {
                HttpContext.AddError(ex);
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
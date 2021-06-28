using Equipment.DAL;
using Equipment.Reporting;
using Kendo.Mvc.UI;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Planning.Appraisal
{
    [Authorize]
    public partial class AppraisalController : Controller
    {
        public ActionResult PrintAppraisal(string id)
        {
            switch (id.Trim())
            {
                //مراکز مصرف
                case "BkpGeoLoc":
                    {
                        object model = "GetReportBkpGeoghLoc";
                        return View("PrintAppraisal", model);
                    }
                case "CustomerPrint":
                    {
                        object model = "CustomerPrint";
                        return View("PrintAppraisal", model);
                    }
            }
            return View();
        }

        /// <summary>
        /// مراکز مصرف
        /// </summary>
        /// <returns></returns>
        public ActionResult GetReportBkpGeoghLoc()
        {
            try
            {
                DataSourceRequest request = (DataSourceRequest)Session["BkpGeoghLoc-Grid-Filters"];
                StiReport myreport = DatabeseReport.GetReport(44);
                using (var cntx = PublicRepository.GetNewDatabaseContext)
                {
                    myreport.RegBusinessObject("Equipment", "BKP_GEOGH_LOC", cntx.BKP_GEOGH_LOC
                                                                                 .ApplyBkpGeoghLocFiltering(request.Filters)
                                                                                 .ApplyBkpGeoghLocSorting(request.Groups, request.Sorts)
                                                                                 .AsEnumerable()
                                                                                 .Select(s => new
                                                                                 {
                                                                                     s.G_CODE,
                                                                                     s.G_DESC,
                                                                                     //CustomersCount = s.EXP_OWENER_COMPANY.Count
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
        /// مشترکین
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerPrint()
        {
            try
            {
                DataSourceRequest request = (DataSourceRequest)Session["ExpOwenerCompany-Grid-Filters"];
                StiReport myreport = DatabeseReport.GetReport(45);
                using (var cntx = PublicRepository.GetNewDatabaseContext)
                {
                    myreport.RegBusinessObject("Equipment", "EXP_OWNER_COMPANY", cntx.EXP_OWENER_COMPANY
                                                                                     .ApplyExpOwenerCompanyFiltering(request.Filters)
                                                                                     .ApplyExpOwenerCompanySorting(request.Groups, request.Sorts)
                                                                                     .AsEnumerable()
                                                                                     .Select(p => new
                                                                                     {
                                                                                         p.EOCO_ID,
                                                                                         p.EOCO_DESC,
                                                                                         EOTY_EOTY = p.EXP_OWENER_TYPE.EOTY_DESC,
                                                                                         p.ACTV_TYPE,
                                                                                         //GEOL_G_CODE = p.BKP_GEOGH_LOC.G_DESC
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
using Asr.Base;
using Asr.Report;
using Stimulsoft.Report;
using System.IO;
using System.Web.Mvc;

namespace Equipment.Controllers.Report
{
    [Developer("H.Hamidi")]
    public class ReportFileDownloadController : Controller
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            App_Start.AsrReportingConfig.RegisterConnectionString();
            var path = Server.MapPath("~/Content/Reports/Localization/fa.xml");
            StiOptions.Localization.Load(path);
        }

        private void RenderReport(AsrReport rep, string condition, string variables)
        {
            if (rep.ReportClass.REPT_TYPE.ToUpper().Trim() != "F")
            {
                if (string.IsNullOrEmpty(condition) || condition.ToLower().Equals("null"))
                {
                    rep.LoadDataSource(true);
                }
                else
                {
                    rep.LoadDataSource(condition.Replace("^^^", "%"));
                }
            }
            else
            {
                rep.LoadDataSource(condition);
            }

            if (!string.IsNullOrEmpty(variables))
            {
                rep.SetVariableValues(variables);
            }
            rep.StiTosave.Render();
        }

        [HttpGet, ReportFileDownload]
        public ActionResult DownloadReport(short rep, string param, string options, string vars)
        {
            return GetReport(rep, param, options, vars);
        }

        private ActionResult GetReport(short repId, string param, string options, string vars)
        {
            AsrReport report = new AsrReport(repId);
            Session["ReportForExport"] = repId;
            Session["ExportType"] = "xls";
            //return  RedirectToAction("ViewReportPdf", "Report", new { rep = repId, param = param, options = options, vars = vars });

            FileStreamResult result;
            RenderReport(report, param, vars);
            MemoryStream stream = new MemoryStream();
            StiExportFormat f;
            string ft = string.Empty;
            f = StiExportFormat.Excel2007;
            ft = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            report.GetStimulReport().ExportDocument(f, stream);
            stream.Flush(); //Always catches me out
            stream.Position = 0;
            result = File(stream, ft, string.Format("{0}.xlsx", report.ReportClass.REPT_NAME));
            return result;
            //return File("~/2.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("Report55.xlsx"));
        }

    }

}

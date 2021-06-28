using Asr.Report;
using Stimulsoft.Report;
using System;
using System.IO;
using System.Web.Mvc;

namespace Equipment.Controllers.Report
{
    public partial class ReportController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="param"></param>
        /// <param name="options"></param>
        /// <param name="vars"></param>
        /// <returns></returns>
        public ActionResult ViewReportPdf(short rep, string param, string options, string vars)
        {
            //     int tryCount = 0;
            // l:
            //     try
            //     {

            string typ = Convert.ToString(Session["ExportType"]);
            //   var user = new AsrUser("WEB_USER", "web_user", 21);
            //  ReportingCore.Initialize("DATA SOURCE=tfmis;PERSIST SECURITY INFO=True;USER ID=REPORTING_AGENT;password=rpt_agent;", user);
            //LocalizationConfig.LoadLocalization("fa.xml");
            //            short repId = short.Parse(Request.QueryString[0].DecryptFromAes());

            //DateTime dt = (DateTime)Request.QueryString[2].DecryptFromAes().ToObject(typeof(DateTime));

            //StiWebViewerFx1
            //if (dt > DateTime.Now)
            // {
            FileStreamResult result;
            AsrReport report = new AsrReport(rep);
            if (!report.Equals(null))
            {
                RenderReport(report, param, vars);
                MemoryStream stream = new MemoryStream();
                StiExportFormat f;
                string ft = string.Empty;

                switch (typ)
                {
                    case "docx":
                        {
                            f = StiExportFormat.Word2007;
                            ft = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                            break;
                        }
                    case "xls":
                        {
                            f = StiExportFormat.Excel2007;
                            ft = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            break;
                        }
                    case "chart":
                        {
                            f = StiExportFormat.ImageJpeg;
                            ft = "image/jpeg";
                            break;
                        }

                    case "pdf":
                    default:
                        {
                            f = StiExportFormat.Pdf;
                            ft = "application/pdf";
                            break;
                        }

                }
                report.GetStimulReport().ExportDocument(f, stream);
                stream.Flush(); //Always catches me out

                stream.Position = 0;

                switch (typ)
                {
                    case "docx":
                        {
                            result = File(stream, ft, string.Format("{0}.docx", report.ReportClass.REPT_NAME));
                            break;
                        }
                    case "xls":
                        {
                            result = File(stream, ft, string.Format("{0}.xlsx", report.ReportClass.REPT_NAME));
                            break;
                        }
                    case "chart":
                        {
                            result = base.File(stream, ft);
                            break;
                        }

                    case "pdf":
                    default:
                        {
                            result = File(stream, ft);
                            break;
                        }
                }

                return result;
            }
            else
            {
                Response.Write("Report Not Found!!");
                return null;
            }

            //  }
            //  catch (Exception)
            //  {
            //      if (tryCount < 3)
            //      {
            //          tryCount++;
            //          goto l;
            //      }
            //      else
            //      {
            //          return View();
            //      }
            //  }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="condition"></param>
        /// <param name="variables"></param>
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

    }

}

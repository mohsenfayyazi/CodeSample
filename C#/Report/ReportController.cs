using Asr.Report;
using Asr.Report.Models;
using Asr.Report.Security;
using Asr.Text;
using Equipment.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Report
{
    [Authorize]
    public partial class ReportController : Controller
    {
        BandarEntities cntx = new BandarEntities();
        //
        // GET: /Report/
        public ActionResult Index()
        {
            return View();
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            cntx = new BandarEntities(GlobalConst.GetSystemAgent());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                cntx.Dispose();
            }
            base.Dispose(disposing);
        }

        public string Condition(RPT_REPORT_PARAM param, AsrOperator op, string val, string val1)
        {
            AsrDataType paramType = (AsrDataType)param.RPT_VIEW_FIELD.RPDTTP_ROW_NO;
            string conditionPattern = string.Empty;
            string stringContinerChar = (paramType == AsrDataType.String || paramType == AsrDataType.DateTime || paramType == AsrDataType.Time) ? "'" : "";

            #region

            switch (op)
            {
                case AsrOperator.Between:
                    {
                        conditionPattern = String.Format("@P1 BETWEEN {0}@P2{0} AND {0}@P3{0}", stringContinerChar);
                        break;
                    }
                case AsrOperator.EndWith:
                    {
                        conditionPattern = "@P1 LIKE '%@P2'";
                        break;
                    }
                case AsrOperator.Equal:
                    {
                        conditionPattern = String.Format("@P1 = {0}@P2{0}", stringContinerChar);
                        break;
                    }
                case AsrOperator.In:
                    {
                        conditionPattern = "@P1 IN (@P2)";
                        break;
                    }
                case AsrOperator.LargerAndEqualThan:
                    {
                        conditionPattern = String.Format("@P1 >= {0}@P2{0}", stringContinerChar);
                        break;
                    }
                case AsrOperator.LargerThan:
                    {
                        conditionPattern = String.Format("@P1 > {0}@P2{0}", stringContinerChar);
                        break;
                    }
                case AsrOperator.LessAndEqualThan:
                    {
                        conditionPattern = String.Format("@P1 <= {0}@P2{0}", stringContinerChar);
                        break;
                    }
                case AsrOperator.LessThan:
                    {
                        conditionPattern = String.Format("@P1 < {0}@P2{0}", stringContinerChar);
                        break;
                    }
                case AsrOperator.Like:
                    {
                        conditionPattern = "@P1 LIKE '%@P2%'";
                        break;
                    }
                case AsrOperator.NotEqual:
                    {
                        conditionPattern = String.Format("@P1 <> {0}@P2{0}", stringContinerChar);
                        break;
                    }
                case AsrOperator.StartWith:
                    {
                        conditionPattern = "@P1 LIKE '@P2%'";
                        break;
                    }
            }

            #endregion

            conditionPattern = conditionPattern.Replace("@P1", "{0}").Replace("@P2", "{1}").Replace("@P3", "{2}");

            string fieldName = param.RPT_VIEW_FIELD.GetFullName();

            #region

            string condition = "";

            switch (paramType)
            {
                case AsrDataType.Boolean:
                    {
                        switch (op)
                        {
                            case AsrOperator.Equal:
                                {
                                    condition = string.Format(conditionPattern, fieldName, val);
                                    break;
                                }
                        }
                        break;
                    }

                case AsrDataType.Byte:
                case AsrDataType.Decimal:
                case AsrDataType.Double:
                case AsrDataType.Int16:
                case AsrDataType.Int32:
                case AsrDataType.Int64:
                case AsrDataType.Sbyte:
                case AsrDataType.Single:
                    {
                        switch (op)
                        {
                            case AsrOperator.Equal:
                            case AsrOperator.LargerThan:
                            case AsrOperator.LargerAndEqualThan:
                            case AsrOperator.LessThan:
                            case AsrOperator.LessAndEqualThan:
                            case AsrOperator.NotEqual:
                                {
                                    condition = string.Format(conditionPattern, fieldName, val);
                                    break;
                                }
                            case AsrOperator.Between:
                                {
                                    condition = string.Format(conditionPattern, fieldName, val, val1);
                                    break;
                                }
                            case AsrOperator.In:
                                {
                                    condition = string.Format(conditionPattern, fieldName, val);
                                    break;
                                }
                        }
                        break;
                    }
                case AsrDataType.DateTime:
                    {
                        switch (op)
                        {
                            case AsrOperator.Equal:
                            case AsrOperator.LargerThan:
                            case AsrOperator.LargerAndEqualThan:
                            case AsrOperator.LessThan:
                            case AsrOperator.LessAndEqualThan:
                            case AsrOperator.NotEqual:
                                {
                                    condition = string.Format(conditionPattern, fieldName, val);
                                    break;
                                }
                            case AsrOperator.Between:
                                {
                                    condition = string.Format(conditionPattern, fieldName, val, val1);
                                    break;
                                }
                            case AsrOperator.In:
                                {
                                    condition = string.Format(conditionPattern, fieldName, val);
                                    break;
                                }
                        }
                        break;
                    }
                case AsrDataType.String:
                    {
                        switch (op)
                        {
                            case AsrOperator.Equal:
                            case AsrOperator.LargerThan:
                            case AsrOperator.LargerAndEqualThan:
                            case AsrOperator.LessThan:
                            case AsrOperator.LessAndEqualThan:
                            case AsrOperator.NotEqual:
                            case AsrOperator.Like:
                            case AsrOperator.StartWith:
                            case AsrOperator.EndWith:
                            case AsrOperator.In:
                                {
                                    condition = string.Format(conditionPattern, fieldName, val.ToArabicUtf8());
                                    break;
                                }
                        }
                        break;
                    }
            }

            #endregion

            return condition;
        }

        public ActionResult Index(int? id)
        {
            return View();
        }

        public ActionResult ReportList()
        {
            return View();
        }

        public ActionResult ReportsRead([DataSourceRequest] DataSourceRequest request)
        {
            var user = new AsrUser("REPORTING_AGENT", "rpt_agent", 21);
            ReportingCore.Initialize("DATA SOURCE=tfmis;PERSIST SECURITY INFO=True;USER ID=REPORTING_AGENT;password=rpt_agent;", user);
            using (var cntx = new Entities())
            {
                var data = cntx.RPT_REPORT
                               .AsEnumerable()
                               .Select(b => new
                               {
                                   b.ROW_NO,
                                   b.REPT_NAME
                                   //ShowLink=string.Format("/reportviewer/showreport/{0}",b.ROW_NO)
                               })
                               .ToDataSourceResult(request);

                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }

        //public ActionResult ShowReportWithParam()
        //{
        //    short id = short.Parse(Request.Form["repId"]);
        //    var user = new AsrUser("REPORTING_AGENT", "rpt_agent", 21);
        //    ReportingCore.Initialize("DATA SOURCE=tfmis;PERSIST SECURITY INFO=True;USER ID=REPORTING_AGENT;password=rpt_agent;", user);
        //    var cntx = new Entities();
        //    var model = cntx.RPT_REPORT.Find(id);
        //    StringBuilder sb = new StringBuilder();
        //    foreach (var p in model.RPT_REPORT_PAGE.FirstOrDefault().RPT_REPORT_PARAM)
        //    {
        //        string pName = String.Format("ParamVal_{0}", p.ROW_NO);
        //        string pName1 = String.Format("ParamVal1_{0}", p.ROW_NO);
        //        string pOperator = String.Format("ParamOp_{0}", p.ROW_NO);
        //        string cn = Condition(p, GetOperator(Request.Form[pOperator]), Request.Form[pName], Request.Form[pName1]);
        //        sb.Append(string.Format("{0} AND ", cn));
        //    }
        //    string condition = sb.ToString(0, sb.Length - 4);
        //    DateTime now = DateTime.Now.AddMinutes(1);
        //    //string dtExpire = now.ToXmlSerilized();
        //  //  var retUrl = string.Format("/ViewReport.aspx?rep={0}&param={1}&options={2}", id.ToString()/*.EncryptToAes()*/, condition, dtExpire.EncryptToAes());
        //  //  return new ServerMessages(ServerOprationType.Success) { Message = "Operation Success", CoustomData = retUrl }.ToJson();
        //}        

    }
}

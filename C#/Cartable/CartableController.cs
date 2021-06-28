using Asr.Base;
using Asr.Cartable;
using Asr.Cartable.Models;
using Asr.Text;
using Equipment.Codes.Globalization;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Equipment.Controllers.Cartable
{
    [Authorize]
    [Developer("A.Saffari")]
    public class CartableController : DbController
    {
        //BandarEntities Db;
        //public CartableController()
        //{
        //    //cntx = GlobalConst.DB();
        //    Db = this.DB();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //        Db.Dispose();
        //    base.Dispose(disposing);
        //}


        // GET: /Cartable/
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult GetTreeMessages(string typeId)
        {
            MessageCateguryType cat = (MessageCateguryType)int.Parse(typeId);
            var dataList = AsrCartable.MessageCategories(User.Identity.Name, cat)
                                      .Select(b => new
                                      {
                                          id = b.MESSAGE_TYPE,
                                          name = b.DISPLAY_NAME.ConvertNumbersToPersian(),
                                          cnt = b.CNT.ToString().ConvertNumbersToPersian(),
                                          isMsgGroup = (b.MESSAGE_TYPE == "FLW_MSG") ? "Yes" : "No"
                                      });
            return Json(new { Success = true, data = dataList }, JsonRequestBehavior.AllowGet);
        }

        public string GetPostName(string sID, string MESSAGE_TYPE)
        {
            string epol_name = "";
            if (Regex.IsMatch(sID, @"^\d+$"))
            {
                decimal ID = decimal.Parse(sID);

                switch (MESSAGE_TYPE)
                {
                    case ("FLW_RESN"):
                        epol_name = Db.EXP_EDOC_INSTRU.Where(xx => xx.EDIN_ID == ID).Select(xx => xx.EXP_EXPI_DOC.EXP_POST_LINE.EPOL_NAME).FirstOrDefault();
                        break;
                    default:
                        epol_name = Db.EXP_EXPI_DOC.Where(xx => xx.EEDO_ID == ID).Select(xx => xx.EXP_POST_LINE.EPOL_NAME).FirstOrDefault();
                        break;
                }

                return epol_name;
            }
            else
                return "";
        }

        public ActionResult GetMessages([DataSourceRequest] DataSourceRequest request, string msgType)
        {
            PersianDateTime pdt = new PersianDateTime();
            var data = AsrCartable.Messages(User.Identity.Name, msgType).OrderByDescending(x => x.BEGIN_DATE).Select(x => new
            {
                x.NOT_ID,
                x.RECIPIENT_ROLE,
                x.FROM_USER,
                x.SUBJECT,
                x.MESSAGE_BODY,
                x.MSG_TITEL,
                persianTdTime = pdt.GetShortDate(x.BEGIN_DATE).ConvertNumbersToPersian(),
                DOC_NUMB = x.ITEM_KEY.Split('^')[1].ToString(),
                isMsg = msgType == "FLW_MSG" ? "Yes" : "No",
                EPOL_NAME = GetPostName(x.ITEM_KEY.Split('^')[1], x.MESSAGE_TYPE),

                //Db.EXP_EXPI_DOC.Where(xx => xx.EEDO_ID == x.ITEM_KEY.Split('^')[1]).Select(xx => xx.EXP_POST_LINE.EPOL_NAME).FirstOrDefault(),
                //CODE_NAME=cntx.EXP_EXPI_DOC.Where(xx=>xx.EEDO_ID==x.DOC_NUMB).Select(xx=>xx.EXP_EDOC_INSTRU.Select(yy=>yy.EPIU_EPIU_ID).FirstOrDefault()).FirstOrDefault(),
                //CODE_NAME = cntx.EXP_POST_LINE_INSTRU.Where(z => z.EPIU_ID == cntx.EXP_EXPI_DOC.Where(xx => xx.EEDO_ID == x.DOC_NUMB).Select(xx => xx.EXP_EDOC_INSTRU.Select(yy => yy.EPIU_EPIU_ID).FirstOrDefault()).FirstOrDefault()).Select(z => z.CODE_NAME).FirstOrDefault()
                //from b in cntx.EXP_EXPI_DOC where b.EEDO_ID ==x.DOC_NUMB select b.EXP_POST_LINE.EPOL_NAME
                //cntx.EXP_EXPI_DOC.Where(xx => xx.DOC_NUMB == x.ITEM_KEY).Select(xx => xx.EPOL_EPOL_ID).FirstOrDefault()
            });

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNextPersonels(int notId)
        {
            //   try
            //   {
            if (notId == 0)
                throw new ArgumentException("Notification Id Can not be null or empty", "notId");

            AsrWorkFlowProcess process = new AsrWorkFlowProcess(notId);
            if (!process.IsOpen)
                throw new Exception("This Process Archived Or not Open");

            var persons = process.GetNextStepUsers();

            return this.Json(new { Success = true, data = persons }, JsonRequestBehavior.AllowGet);
            //  }
            //  catch (Exception ex)
            //  {
            //      return this.Json(new { Success = true, data = new List<Asr.Security.SEC_USERS>()}, JsonRequestBehavior.AllowGet);
            //      return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();
            //  }
        }

        public JsonResult GetPrePersonels(int notId)
        {
            // try
            // {
            if (notId == 0)
                throw new ArgumentException("Notification Id Can not be null or empty", "notId");

            AsrWorkFlowProcess process = new AsrWorkFlowProcess(notId);
            if (!process.IsOpen)
                throw new Exception("This Process Archived Or not Open");

            var persons = process.GetPreStepUsers();

            return this.Json(new { Success = true, data = persons }, JsonRequestBehavior.AllowGet);
            // }
            // catch (Exception ex)
            // {
            //     return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();
            // }
        }

        [ValidateInput(false)]
        public JsonResult Approve(int notId, string userName, string msgbody)
        {
            try
            {
                AsrWorkFlowProcess fp = new AsrWorkFlowProcess(notId);
                if (fp.IsOpen)
                {
                    if (string.IsNullOrEmpty(msgbody))
                    {
                        fp.Approve(new string[] { userName });
                    }
                    else
                    {
                        fp.Approve(new string[] { userName }, msgbody);
                    }
                    NotificationHub hub = new NotificationHub();
                    hub.SendNotify(HttpContext.User.Identity.Name, userName, fp.MsgTitle);
                }





                return new ServerMessages(ServerOprationType.Success) { Message = "Approve" + Convert.ToString(notId) + Ajax_Send_Sms(notId, userName) }.ToJson();

            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "Approve Failed =>" + Convert.ToString(notId) }.ToJson();
            }
        }
        public string Ajax_Send_Sms(int? notId, string userName)
        {
            string ReturnVal = "";
            AsrWorkFlowProcess p1 = new AsrWorkFlowProcess(Convert.ToInt32(notId));
            SetIranWebReference.SendReceive Obj = new SetIranWebReference.SendReceive();


            string Mobile = Db.SEC_USERS.Where(xx => xx.ORCL_NAME.ToUpper() == userName.ToUpper()).Select(xx => xx.PAY_PERSONEL.MOBIL).FirstOrDefault();
            var ObjCtrl = new Message.Message.MessageController();

            int SMSTemplateId = Db.SMS_TEMPLATE.Where(xx => xx.TITL == Db.EXP_WFEX_V.Where(yy => yy.NOT_ID == notId).Select(yy => yy.MESSAGE_TYPE).FirstOrDefault()
            + "_" +
            Db.EXP_WFEX_V.Where(yy => yy.NOT_ID == notId).Select(yy => yy.MESSAGE_NAME).FirstOrDefault()).Select(xx => xx.ID).FirstOrDefault();
            if (SMSTemplateId == 0)
            {
                SMSTemplateId = Db.SMS_TEMPLATE.Where(xx => xx.TITL == p1.FlowName
            + "_" +
            p1.CurrentMessageName).Select(xx => xx.ID).FirstOrDefault();


            }

            decimal EEDO_ID = Db.EXP_WFEX_V.Where(yy => yy.NOT_ID == notId).Select(yy => yy.EEDO_ID).FirstOrDefault();

            int n = 0;
            bool isNumeric = int.TryParse(p1.ItemKey.Split('^')[1], out n);
            if (p1.ItemKey.Contains("-"))
            {
                isNumeric = int.TryParse(p1.ItemKey.Split('^')[1].Split('-')[0], out n);


            }
            if (EEDO_ID == 0 && isNumeric && p1.ItemKey.Contains("-"))
            {
                EEDO_ID = decimal.Parse(p1.ItemKey.Split('^')[1].Split('-')[0]);
            }
            else
            {
                EEDO_ID = decimal.Parse(p1.ItemKey.Split('^')[1]);
            }
            string SMS_Text = ""; int? ETDO_ID; short? ISQUERY;
            string Body = Db.SMS_TEMPLATE.Where(xx => xx.ID == SMSTemplateId).Select(xx => xx.BODY).FirstOrDefault();
            ETDO_ID = Db.SMS_TEMPLATE.Where(xx => xx.ID == SMSTemplateId).Select(xx => xx.ETDO_ETDO_ID).FirstOrDefault();
            ISQUERY = Db.SMS_TEMPLATE.Where(xx => xx.ID == SMSTemplateId).Select(xx => xx.ISQUERY).FirstOrDefault();
            using (var model = new BandarEntities())
            {
                if (!string.IsNullOrEmpty(Body) && SMSTemplateId != 0 && ETDO_ID != 303 && (ISQUERY == 1 || ISQUERY == 2) &&
                    Db.SMS_TEMPLATE.Where(xx => xx.ID == SMSTemplateId).Select(xx => xx.STAT).FirstOrDefault() == 1)
                {
                    DataTable ResultTable = new DataTable();
                    string query = string.Format(Body, EEDO_ID);
                    var cmd = (OracleCommand)model.Database.Connection.CreateCommand();
                    cmd.CommandText = query;
                    using (var da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(ResultTable);
                        if (ResultTable != null)
                        {
                            SMS_Text = ResultTable.Rows[0]["Text"].ToString();
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(SMS_Text))
            {
                int? r = Obj.SendSingle("asr", "Asr@ndishe123", 0, "30007291", Mobile, SMS_Text);
                if (r < 0)
                {

                    ReturnVal = " خطای '" + Db.CHK_DOMAIN.Where(xx => xx.DMAN_DMAN_ID == 921 && xx.DMAN_DMAN_ID_LIST == r).Select(xx => xx.DMAN_TITL).FirstOrDefault() + "'در ارسال پیامک به  " + userName;
                }
                else
                {
                    ReturnVal = " پیامک با موفقیت به " + userName + " ارسال شد ";
                }
            }
            return ReturnVal;

        }
        public JsonResult GetCurrentStat(int notId)
        {
            try
            {
                string currentStat = string.Empty;
                AsrWorkFlowProcess fp = new AsrWorkFlowProcess(notId);
                if (fp.IsOpen)
                {
                    currentStat = fp.CurrentStat + "-" + fp.CurrentMessageName;
                }
                return new ServerMessages(ServerOprationType.Success) { Message = currentStat }.ToJson();
            }
            catch (Exception)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "Approve Failed =>" + Convert.ToString(notId) }.ToJson();
            }
        }

        public JsonResult Reject(int notId, string userName, string msgbody)
        {
            try
            {
                AsrWorkFlowProcess fp = new AsrWorkFlowProcess(notId);
                if (fp.IsOpen)
                {
                    if (string.IsNullOrEmpty(msgbody))
                    {
                        fp.Reject(userName);
                    }
                    else
                    {
                        fp.Reject(userName, msgbody);
                        // fp.Approve(new string[] { userName }, msgbody);
                    }
                    // Notificat
                    //fp.Reject(userName);
                    NotificationHub hub = new NotificationHub();
                    hub.SendNotify(HttpContext.User.Identity.Name, userName, fp.MsgTitle, NotificationType.Warning);
                }
                return new ServerMessages(ServerOprationType.Success) { Message = "Reject" + Convert.ToString(notId) }.ToJson();
            }
            catch (Exception)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "Reject Failed =>" + Convert.ToString(notId) }.ToJson();
            }
        }

        public ActionResult DisplayForm(int id)
        {
            Dictionary<string, string> dic;

            try
            {
                AsrWorkFlowProcess fp = new AsrWorkFlowProcess(id);
                if (fp.IsOpen)
                {
                    dic = fp.GetActionInfoForPreviewEntity();
                    return RedirectToAction(dic["Action"], dic["Controller"], new { id = dic["WhereClous"], notId = dic["NotId"] });
                }
                return View();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() + " => " + Convert.ToString(id) }.ToJson();
            }
        }

        public PartialViewResult ShowMessage(string notId)
        {
            AsrWorkFlowMessage msg = new AsrWorkFlowMessage(Convert.ToInt32(notId));
            ViewBag.MessageContent = msg.MsgBody;
            ViewBag.Sender = msg.Sender;
            msg.SetReadStatus(AsrWorkFlowMessage.AsrMessageReadStatus.Readed);
            ViewData["notId"] = notId;
            return PartialView("ShowMessage");
        }


        public JsonResult GetWindowSize(int notId)
        {
            AsrWorkFlowProcess wp = new AsrWorkFlowProcess(notId);
            return new ServerMessages(ServerOprationType.Success) { CoustomData = new { w = wp.WindowWidth, h = wp.WindowHeight } }.ToJson();
        }

        [ValidateInput(false)]
        public JsonResult SendMessage(string performer, string messageSubject, string messageBody)
        {
            string sender = this.UserInfo().Username;
            try
            {
                AsrWorkFlowMessage msg = new AsrWorkFlowMessage(sender, performer.Split(',').ToArray(), messageSubject, messageBody);
                msg.Send();
                NotificationHub hub = new NotificationHub();
                hub.SendNotify(HttpContext.User.Identity.Name, performer.Split(',').FirstOrDefault(), messageSubject);
                return new ServerMessages(ServerOprationType.Success) { Message = "پیام جدید ارسا شد" }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();
            }
        }

        public JsonResult GetPersonel(string text)
        {
            //using (BandarEntities Db = new BandarEntities())
            //{
            var query = from p in Db.PAY_PERSONEL.AsEnumerable()
                        join s in Db.SEC_USERS.AsEnumerable() on p.EMP_NUMB equals s.PRSN_EMP_NUMB
                        select new { PersonelFullName = string.Format("{1} {0}", p.FIRS_NAME, p.FAML_NAME), PersonelUserName = s.ORCL_NAME };

            return Json(query.ToList(), JsonRequestBehavior.AllowGet);
            //}
        }

        public JsonResult RemoveProcess(int notId)
        {
            try
            {
                AsrWorkFlowProcess fp = new AsrWorkFlowProcess(notId);
                if (fp.IsOpen)
                {
                    fp.RemoveProccess();
                }
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("پروسه حذف شد: {0}", fp.NoteId) }.ToJson();
            }
            catch (Exception)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "remove Failed =>" + Convert.ToString(notId) }.ToJson();
            }
        }

        [AllowAnonymous]
        public ActionResult SignalrNotifyServer()
        {
            return View();
        }

        public ActionResult DeleteAllMsg(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                userName = User.Identity.Name;
            try
            {
                string deleteCommand = string.Format("delete from WF_NOTIFICATIONS where message_type = 'FLW_MSG' and LOWER(RECIPIENT_ROLE) = '{0}'", userName.ToLower());
                Db.Database.ExecuteSqlCommand(deleteCommand);
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("همه پیام های کارتابل شما حذف گردید") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();
            }
        }

        //-------------------------------New Cartble Actions------------------------------
        public PartialViewResult NotificationInfo(decimal notificationId)
        {
            using (AsrWorkflowNotification notify = new AsrWorkflowNotification(notificationId))
            {
                var msgInfo = notify.GetMessageInfo();
                ViewBag.NotifyMessage = msgInfo;
                ViewBag.NotifyMessageDetails = msgInfo.WF_MESSAGES_TL.ToList();
                return PartialView("PName", notify);
            }
        }

        //-------------------------------/New Cartble Actions------------------------------

    }

}
using Asr.Cartable;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Message.Message
{
    public class MessageController : DbController
    {
        //BandarEntities Db;

        PersianCalendar pc = new PersianCalendar();
        DateTime thisDate = DateTime.Now;
        int? userid = 0; string username = string.Empty;
        public MessageController()
            : base()
        {
            // Db = this.DB();
            username = this.UserInfo().Username;
        }

        public string Ajax_GetRecipient(int? EMRG_ID)
        {
            var query = Db.EXP_MESSAGE_GROUP_USER.Where(xx => xx.EMRG_EMRG_ID == EMRG_ID).Select(xx => xx.SEC_USERS.ORCL_NAME).ToList();
            string date = string.Join(",", query);
            //date = "'" + date + "'";
            return (date);
        }

        public string Ajax_GetRecipient_Responsible(int? id)
        {
            string user_name = this.HttpContext.User.Identity.Name;

            var query = Db.SEC_USER_TYPE_POST.Where(xx => xx.ETDO_ETDO_ID == 343 && xx.EURP_ACTV == 1).Select(xx => xx.SEC_USERS.ORCL_NAME).Distinct().ToList();
            if (id == 2 && (user_name == "s-khademi" || user_name == "AFROOSHE" || user_name == "A-KHOSHSIRAT"))
            {
                query = Db.SEC_USER_TYPE_POST.Where(xx => xx.ETDO_ETDO_ID == 463 && xx.EURP_TYPE == 2).Select(xx => xx.SEC_USERS.ORCL_NAME).Distinct().ToList();

            }
            string date = string.Join(",", query);
            //date = "'" + date + "'";
            return (date);
        }

        public string Ajax_Sms_Text(int? Id, int? ENTY_ID, string Date, string LastYear, string Year)
        {
            //var MainQuery = from b in Db.EXP_EDOC_INSTRU
            //                join c in Db.CHK_DOMAIN on b.DMAN_DMAN_ID equals c.DMAN_ID
            //                where b.EDIN_ID == Id
            //                select new { b.EDIN_ID, CODE_NAME = b.EXP_POST_LINE_INSTRU.CODE_NAME, EPOL_NAME = b.EXP_POST_LINE.EPOL_NAME, b.OFF_DATE, b.OFF_TIME, b.EDIN_MW, c.DMAN_TITL };
            //string SMS_Text = "تجهیز" + MainQuery.Select(xx => xx.CODE_NAME).FirstOrDefault() + " پست " + MainQuery.Select(xx => xx.EPOL_NAME).FirstOrDefault() + " مورخه " +
            //    MainQuery.Select(xx => xx.OFF_DATE).FirstOrDefault() + " در ساعت " + MainQuery.Select(xx => xx.OFF_TIME).FirstOrDefault() + " بصورت " +
            //    MainQuery.Select(xx => xx.DMAN_TITL).FirstOrDefault() + " با ایجاد خاموشی " + MainQuery.Select(xx => xx.EDIN_MW).FirstOrDefault() + " از مدار خارج شد.اکیپ جهت بررسی اعزام گردید. ";
            string SMS_Text = ""; int? ETDO_ID; short? ISQUERY;
            try
            {
                string Body = Db.SMS_TEMPLATE.Where(xx => xx.ID == Id).Select(xx => xx.BODY).FirstOrDefault();
                ETDO_ID = Db.SMS_TEMPLATE.Where(xx => xx.ID == Id).Select(xx => xx.ETDO_ETDO_ID).FirstOrDefault();
                ISQUERY = Db.SMS_TEMPLATE.Where(xx => xx.ID == Id).Select(xx => xx.ISQUERY).FirstOrDefault();
                using (var model = new BandarEntities())
                {
                    if (!string.IsNullOrEmpty(Body) && ENTY_ID != 0 && ISQUERY == 1 && Db.SMS_TEMPLATE.Where(xx => xx.ID == Id).Select(xx => xx.STAT).FirstOrDefault() != 0)
                    {
                        DataTable ResultTable = new DataTable();
                        string query = string.Format(Body, ENTY_ID, Date, LastYear, Year);
                        //= string.Format(@"select ' تجهیز '||code_name|| ' پست ' || epol_name||' مورخه '||OFF_DATE|| ' ساعت ' ||OFF_time ||' بصورت '|| DMAN_TITL|| ' با ایجاد خاموشی '|| EDIN_MW || ' MW از مدار خارج شدند.اکیپ جهت بررسی ارسال شد.' Text from exp_edoc_instru a,exp_post_line_instru b,exp_post_line c,chk_domain d
                        //                              where edin_id ={0}  and a.epiu_epiu_id=b.epiu_id and b.epol_epol_id_inslin=c.epol_id
                        //                                    and d.dman_id=a.dman_dman_id ", Id);

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
                if (ISQUERY == 0)
                {
                    SMS_Text = Body;
                };
            }
            catch (Exception ex)
            {
                SMS_Text = ex.Message;
            }
            return (SMS_Text);
        }

        public string Ajax_GetRecipient_Analyzer(int? EANA_ROW)
        {
            var query = Db.SEC_USER_TYPE_POST.Where(xx => xx.EANA_EANA_ROW == EANA_ROW && xx.EURP_ACTV == 1).Select(xx => xx.SEC_USERS.ORCL_NAME).ToList();
            string date = string.Join(",", query);
            //date = "'" + date + "'";
            return (date);
        }
        public string Ajax_GetRecipient_Template_Analyzer(int? Id, int? ESHH_ESHH_ID)
        {
            string Data = "";
            var TemplateQuery = from a in Db.SMS_RECEIVER_GROUP
                                where a.SMTE_ID == Id
                                select new { a.EANA_EANA_ROW, a.STAT };
            foreach (var Row in TemplateQuery)
            {
                var query = Db.SEC_USER_TYPE_POST.Where(xx => xx.EANA_EANA_ROW == Row.EANA_EANA_ROW).Select(xx => xx.SEC_USERS.ORCL_NAME).ToList();

                Data = Data + string.Join(",", query);
                if (Row.STAT == 2)
                {
                    var Query = Db.EXP_SHIFT_PERS.Where(xx => xx.ESHH_ESHH_ID == ESHH_ESHH_ID && !query.Contains(xx.SEC_USERS.ORCL_NAME)).Select(xx => xx.SEC_USERS.ORCL_NAME).ToList();
                    Data = Data + "," + string.Join(",", Query);
                }
            }


            //date = "'" + date + "'";
            return (Data);
        }

        public string Ajax_GetRecipient_Operator(short? GROP_ID)
        {
            var query = (from a in Db.EXP_POST_GROUP
                         join b in Db.SEC_USER_TYPE_POST on a.EPOL_EPOL_ID equals b.EPOL_EPOL_ID
                         where a.GROP_GROP_ID == GROP_ID && b.EURP_TYPE == 2 && b.ETDO_ETDO_ID == 463
                         select b.SEC_USERS.ORCL_NAME).Distinct().ToList();

            //var query = Db.SEC_USER_TYPE_POST.Where(xx => xx.ETDO_ETDO_ID == 343 && xx.EURP_ACTV == 1).Select(xx => xx.SEC_USERS.ORCL_NAME).Distinct().ToList();
            string date = string.Join(",", query);
            //date = "'" + date + "'";
            return (date);
        }

        public ActionResult EXP_MESSAGE()
        {
            return View();
        }
        public ActionResult Add_Group(int? id)
        {
            ViewData["SMTE_ID"] = id;
            return View();
        }
        public ActionResult Sms_Index()
        {
            return View();
        }
        public ActionResult Sms_Index_Admin()
        {
            return View();
        }
        public ActionResult SMS()
        {
            return View();
        }
        public ActionResult SMS_Abstract()
        {
            return View();
        }
        public ActionResult EXP_LEARN_PRSN()
        {
            return View();
        }

        public ActionResult EXP_RECIPIENT(int? id)
        {
            ViewData["EXME_ID"] = id;
            return View();
        }

        public ActionResult EXP_MESSAGE_GROUPS()
        {
            return View();
        }

        public ActionResult EXP_MESSAGE_GROUPS_PRSN(int? id)
        {
            ViewData["EMRG_ID"] = id;
            return View();
        }

        public ActionResult ShowMessage(string notId)
        {
            ViewData["notId"] = notId;
            return View();
        }

        public ActionResult Getpost(int? grop_id)
        {
            var query = (from b in Db.EXP_POST_LINE
                         join c in Db.EXP_POST_GROUP on b.EPOL_ID equals c.EPOL_EPOL_ID
                         orderby b.EPOL_NAME
                         where b.EPOL_TYPE == "0" && b.EPOL_STAT == "1" && c.GROP_GROP_ID == grop_id
                         select new { b.EPOL_ID, EPOL_NAME = b.EPOL_NAME }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllPost()
        {
            var query = (from b in Db.EXP_POST_LINE
                         orderby b.EPOL_NAME
                         where b.EPOL_TYPE == "0" && b.EPOL_STAT == "1"
                         select new { b.EPOL_ID, EPOL_NAME = b.EPOL_NAME }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_Group([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from p in Db.EXP_MESSAGE_RECIPIENT_GROUP
                         where p.CRET_BY == username
                         orderby p.EMRG_TITLE
                         select new
                         {
                             p.EMRG_ID,
                             p.EMRG_TITLE,
                             p.EMRG_STAT,
                             COUNT = Db.EXP_MESSAGE_GROUP_USER.Where(xx => xx.EMRG_EMRG_ID == p.EMRG_ID).Count()
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Get_SMS_RECEIVER_GROUP([DataSourceRequest] DataSourceRequest request, short? ID)
        {
            var query = (from p in Db.SMS_RECEIVER_GROUP
                         join c in Db.EXP_ANALYZOR_EVENT on p.EANA_EANA_ROW equals c.EANA_ROW

                         where p.SMTE_ID == ID || ID == null
                         select new
                         {
                             p.ID,
                             p.STAT,
                             EANA_DESC = c.EANA_DESC

                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }
        public ActionResult insert_subgroup(SMS_RECEIVER_GROUP objecttemp)
        {

            Db.SMS_RECEIVER_GROUP.Add(objecttemp);
            Db.SaveChanges();

            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
        }

        public ActionResult Insert_SMS_Template(SMS_TEMPLATE objecttemp)
        {
            try
            {
                Db.SMS_TEMPLATE.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { Message = string.Format("خطا در ثبت اطلاعات ") }.ToJson();

            }


        }
        public ActionResult Get_SMS_Template([DataSourceRequest] DataSourceRequest request, short? ISQUERY)
        {
            var query = (from p in Db.SMS_TEMPLATE
                         where (p.CRET_BY == username || username == "S-KHADEMI") && (p.ISQUERY == ISQUERY || ISQUERY == null)
                         orderby p.ID
                         select new
                         {
                             p.ID,
                             p.TITL,
                             p.BODY,
                             p.STAT,
                             p.ISQUERY,
                             p.ETDO_ETDO_ID
                         }).ToList();
            var ResultQuery = query.Select(a => new
            {

                a.BODY,
                a.TITL,
                a.ISQUERY,
                ETDO_DESC = Db.EXP_TYPE_DOC.Where(xx => xx.ETDO_ID == a.ETDO_ETDO_ID).Select(xx => xx.ETDO_DESC).FirstOrDefault(),
                a.STAT,
                a.ID
            }
            );

            return Json(ResultQuery.ToDataSourceResult(request));
        }
        public ActionResult Update_SMS([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SMS_TEMPLATE> SMS_TEMPLATE)
        {
            if (SMS_TEMPLATE != null)
            {
                foreach (SMS_TEMPLATE item in SMS_TEMPLATE)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }
            var query = (from p in Db.SMS_TEMPLATE
                         where (p.CRET_BY == username || username == "S-KHADEMI")
                         orderby p.ID
                         select new
                         {
                             p.ID,
                             p.TITL,
                             p.BODY,
                             p.STAT
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }
        public ActionResult Get_Recipient_grid([DataSourceRequest] DataSourceRequest request, int exme_id)
        {
            var query = (from p in Db.EXP_MESSAGE_RECIPIENT
                         join c in Db.SEC_USERS on p.SCSU_ROW_NO equals c.ROW_NO
                         where p.EXME_EXME_ID == exme_id
                         select new
                         {
                             USER = p.CRET_BY,
                             FIRS_NAME = c.FIRS_NAME != null ? c.FIRS_NAME : c.PAY_PERSONEL.FIRS_NAME,
                             FAML_NAME = c.FAML_NAME != null ? c.FAML_NAME : c.PAY_PERSONEL.FAML_NAME,
                             p.EXMR_VDATE
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult insert_groups(EXP_MESSAGE_RECIPIENT_GROUP objecttemp)
        {
            try
            {
                Db.EXP_MESSAGE_RECIPIENT_GROUP.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات  ") }.ToJson();
            }

        }

        public ActionResult insert_exp_message_groups_prsn(EXP_MESSAGE_GROUP_USER objecttemp)
        {
            try
            {
                Db.EXP_MESSAGE_GROUP_USER.Add(objecttemp);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات  ") }.ToJson();
            }

        }

        public ActionResult Send_Sms(EXP_MESSAGE objecttemp)
        {
            string Mobile = "";
            List<string> Messages = new List<string>();
            List<string> ErrorMessages = new List<string>();
            try
            {
                EXP_MESSAGE_RECIPIENT objectrecipie = new EXP_MESSAGE_RECIPIENT();
                AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
                Db.EXP_MESSAGE.Add(objecttemp);
                Db.SaveChanges();

                string first_name, last_name;
                first_name = Db.SEC_USERS.Where(xx => xx.ORCL_NAME == this.HttpContext.User.Identity.Name.ToUpper()).Select(xx => xx.PAY_PERSONEL.FIRS_NAME).FirstOrDefault() == null ?
                    Db.SEC_USERS.Where(xx => xx.ORCL_NAME == this.HttpContext.User.Identity.Name.ToUpper()).Select(xx => xx.FIRS_NAME).FirstOrDefault()
                    : Db.SEC_USERS.Where(xx => xx.ORCL_NAME == this.HttpContext.User.Identity.Name.ToUpper()).Select(xx => xx.PAY_PERSONEL.FIRS_NAME).FirstOrDefault();
                last_name = Db.SEC_USERS.Where(xx => xx.ORCL_NAME == this.HttpContext.User.Identity.Name.ToUpper()).Select(xx => xx.PAY_PERSONEL.FAML_NAME).FirstOrDefault() == null ?
                    Db.SEC_USERS.Where(xx => xx.ORCL_NAME == this.HttpContext.User.Identity.Name.ToUpper()).Select(xx => xx.FAML_NAME).FirstOrDefault()
                    : Db.SEC_USERS.Where(xx => xx.ORCL_NAME == this.HttpContext.User.Identity.Name.ToUpper()).Select(xx => xx.PAY_PERSONEL.FAML_NAME).FirstOrDefault();

                string usera = Request.Form["Recipient"];
                var users = usera.Split(',').ToArray();
                SetIranWebReference.SendReceive Obj = new SetIranWebReference.SendReceive();
                //SetIranWebReference.ServiceDestination[] Destinations = new SetIranWebReference.ServiceDestination[users.Count()];
                //for (int i = 0; i < Destinations.Length; ++i)
                //{
                //    Destinations[i] = new SetIranWebReference.ServiceDestination();
                //}
                //SetIranWebReference.ServiceOutbox OutBox = new SetIranWebReference.ServiceOutbox();


                //OutBox.ServiceNumber = "30007291";
                //OutBox.MessageText = "TES";
                //    Destinations[0].Destination = "09309100817";
                //    OutBox.Destinations = Destinations;
                //    //var r = Ob.SendSMS("asr", "Asr@ndishe123", 0, OutBox);







                foreach (var user in users)
                {
                    objectrecipie.EXME_EXME_ID = objecttemp.EXME_ID;
                    objectrecipie.SCSU_ROW_NO = Db.SEC_USERS.Where(xx => xx.ORCL_NAME == user).Select(xx => xx.ROW_NO).FirstOrDefault();
                    Db.EXP_MESSAGE_RECIPIENT.Add(objectrecipie);
                    Db.SaveChanges();
                    first_name = Db.SEC_USERS.Where(xx => xx.ROW_NO == objectrecipie.SCSU_ROW_NO).Select(xx => xx.PAY_PERSONEL.FIRS_NAME).FirstOrDefault() == null ?
                   Db.SEC_USERS.Where(xx => xx.ROW_NO == objectrecipie.SCSU_ROW_NO).Select(xx => xx.FIRS_NAME).FirstOrDefault()
                   : Db.SEC_USERS.Where(xx => xx.ROW_NO == objectrecipie.SCSU_ROW_NO).Select(xx => xx.PAY_PERSONEL.FIRS_NAME).FirstOrDefault();
                    last_name = Db.SEC_USERS.Where(xx => xx.ROW_NO == objectrecipie.SCSU_ROW_NO).Select(xx => xx.PAY_PERSONEL.FAML_NAME).FirstOrDefault() == null ?
                        Db.SEC_USERS.Where(xx => xx.ROW_NO == objectrecipie.SCSU_ROW_NO).Select(xx => xx.FAML_NAME).FirstOrDefault()
                        : Db.SEC_USERS.Where(xx => xx.ROW_NO == objectrecipie.SCSU_ROW_NO).Select(xx => xx.PAY_PERSONEL.FAML_NAME).FirstOrDefault();

                    int? r = 0;
                    Mobile = Db.SEC_USERS.Where(xx => xx.ROW_NO == objectrecipie.SCSU_ROW_NO).Select(xx => xx.PAY_PERSONEL.MOBIL).FirstOrDefault();
                    if (!string.IsNullOrEmpty(Mobile))
                    {
                        r = Obj.SendSingle("asr", "Asr@ndishe123", 0, "30007291", Mobile, objecttemp.EXME_BODY);
                    }
                    else
                    {
                        Messages.Add(" خطای  شماره موبایل در ارسال پیامک به  " + first_name + " " + last_name);

                    }
                    if (r < 0)
                    {

                        Messages.Add(" خطای '" + Db.CHK_DOMAIN.Where(xx => xx.DMAN_DMAN_ID == 921 && xx.DMAN_DMAN_ID_LIST == r).Select(xx => xx.DMAN_TITL).FirstOrDefault() + "'در ارسال پیامک به  " + first_name + " " + last_name);
                    }
                    else if (r != 0)
                    {
                        Messages.Add(" پیامک با موفقیت به " + first_name + " " + last_name + " ارسال شد ");
                    }




                }
            }
            catch (Exception ex)
            {
                //string ErrorMessage = string.Join("<br />", Messages.ToArray());
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.Message }.ToJson();
            }
            string successMessages = string.Join("<br />", Messages.ToArray());
            return new ServerMessages(ServerOprationType.Success) { Message = successMessages }.ToJson();
            // return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
        }

        public int Send_Simple_SMS(string Mobile, string SMS_Text)
        {

            SetIranWebReference.SendReceive Obj = new SetIranWebReference.SendReceive();
            int r = Obj.SendSingle("asr", "Asr@ndishe123", 0, "30007291", Mobile, SMS_Text);
            return r;
        }
        public ActionResult insert_message(EXP_MESSAGE objecttemp)
        {
            try
            {
                EXP_MESSAGE_RECIPIENT objectrecipie = new EXP_MESSAGE_RECIPIENT();
                AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
                Db.EXP_MESSAGE.Add(objecttemp);
                Db.SaveChanges();
                var message = new Asr.Cartable.AsrWorkFlowMessage();
                // wp.StartProcess(this.HttpContext.User.Identity.Name, new string[] { this.HttpContext.User.Identity.Name }, objecttemp.EXME_TITLE, objecttemp.EXME_BODY, 82, objecttemp.EXME_ID);
                // wp = new AsrWorkFlowProcess(NotId);
                // 
                string first_name, last_name;
                first_name = Db.SEC_USERS.Where(xx => xx.ORCL_NAME == this.HttpContext.User.Identity.Name.ToUpper()).Select(xx => xx.PAY_PERSONEL.FIRS_NAME).FirstOrDefault() == null ?
                    Db.SEC_USERS.Where(xx => xx.ORCL_NAME == this.HttpContext.User.Identity.Name.ToUpper()).Select(xx => xx.FIRS_NAME).FirstOrDefault()
                    : Db.SEC_USERS.Where(xx => xx.ORCL_NAME == this.HttpContext.User.Identity.Name.ToUpper()).Select(xx => xx.PAY_PERSONEL.FIRS_NAME).FirstOrDefault();
                last_name = Db.SEC_USERS.Where(xx => xx.ORCL_NAME == this.HttpContext.User.Identity.Name.ToUpper()).Select(xx => xx.PAY_PERSONEL.FAML_NAME).FirstOrDefault() == null ?
                    Db.SEC_USERS.Where(xx => xx.ORCL_NAME == this.HttpContext.User.Identity.Name.ToUpper()).Select(xx => xx.FAML_NAME).FirstOrDefault()
                    : Db.SEC_USERS.Where(xx => xx.ORCL_NAME == this.HttpContext.User.Identity.Name.ToUpper()).Select(xx => xx.PAY_PERSONEL.FAML_NAME).FirstOrDefault();

                string usera = Request.Form["Recipient"];
                var users = usera.Split(',').ToArray();
                //wp.Approve(users, objecttemp.EXME_TITLE);
                foreach (var user in users)
                {
                    objectrecipie.EXME_EXME_ID = objecttemp.EXME_ID;
                    objectrecipie.SCSU_ROW_NO = Db.SEC_USERS.Where(xx => xx.ORCL_NAME == user).Select(xx => xx.ROW_NO).FirstOrDefault();

                    Db.EXP_MESSAGE_RECIPIENT.Add(objectrecipie);
                    Db.SaveChanges();
                    // wp.StartMsgProcess(user, new string[] { this.HttpContext.User.Identity.Name }, objecttemp.EXME_TITLE, objecttemp.EXME_BODY, 82,objecttemp.EXME_ID.ToString()+"-");
                    wp.StartProcess(user, new string[] { user }, objecttemp.EXME_TITLE, objecttemp.EXME_BODY, 82, Db.SEC_USERS.Where(xx => xx.ORCL_NAME == user).Select(xx => xx.ROW_NO).FirstOrDefault().ToString() + "-" + objecttemp.EXME_ID.ToString());
                    int NotId = Db.Database.SqlQuery<int>(string.Format("select NOT_ID from wf_note_v where RECIPIENT_ROLE='{0}' and MESSAGE_TYPE='FLW_EMSG' and stat='OPEN'" +
                     "and MESSAGE_NAME='RECIPIENT' and ITEM_KEY='FLW_EMSG.PFLW_EMSG^{1}' and SUBJECT='{2}' ", user, Db.SEC_USERS.Where(xx => xx.ORCL_NAME == user).Select(xx => xx.ROW_NO).FirstOrDefault() + "-" + objecttemp.EXME_ID, objecttemp.EXME_TITLE)).FirstOrDefault();
                    string sql = string.Format("update WF_NOTIFICATIONS set FROM_USER='{0}' where NOTIFICATION_ID={1}", first_name + " " + last_name, NotId);
                    Db.Database.ExecuteSqlCommand(sql);
                    //  wp.StartProcess(this.HttpContext.User.Identity.Name, new string[] { this.HttpContext.User.Identity.Name }, objecttemp.EXME_TITLE, objecttemp.EXME_BODY, 83, objecttemp.EXME_ID);
                    // wp.SetKeyValue("USER_NAMES", user);
                    // wp.Approve(new string[] { user });
                    //  message = new  Asr.Cartable.AsrWorkFlowMessage(this.HttpContext.User.Identity.Name, users, objecttemp.EXME_TITLE, objecttemp.EXME_BODY);
                    //message.Send();
                    // message.Dispose();
                    // wp.StartMsgProcess(performer, new string[] { }, base.MsgTitle, base.MsgBody, 83);
                    // wp.StartMsgProcess(user, new string[] { this.HttpContext.User.Identity.Name }, objecttemp.EXME_TITLE, objecttemp.EXME_BODY, 83);
                    //wp.SetKeyValue("SENDER", this.Sender);
                    //p.SetKeyValue("SEND_DATE", DateTime.Now.ToString());
                    // wp.Approve(user, objecttemp.EXME_TITLE, objecttemp.EXME_BODY, 83, objecttemp.EXME_ID);
                    //   wp.Approve(new string[] { user }, objecttemp.EXME_TITLE);
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات" }.ToJson();
            }

            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
        }

        public ActionResult Get_Message_Group([DataSourceRequest] DataSourceRequest request, int? EMRG_ID)
        {
            var query = (from p in Db.EXP_MESSAGE_RECIPIENT_GROUP
                         join x in Db.EXP_MESSAGE_GROUP_USER on p.EMRG_ID equals x.EMRG_EMRG_ID
                         join q in Db.SEC_USERS on x.SCSU_ROW_NO equals q.ROW_NO
                         where (x.EMRG_EMRG_ID == EMRG_ID)
                         orderby q.FAML_NAME
                         select new
                         {
                             x.EMGU_ID,
                             FAML_NAME = (q.FIRS_NAME != null ? q.FIRS_NAME : q.PAY_PERSONEL.FIRS_NAME) + " " + (q.FAML_NAME != null ? q.FAML_NAME : q.PAY_PERSONEL.FAML_NAME),
                             EMRG_TITLE = x.EXP_MESSAGE_RECIPIENT_GROUP.EMRG_TITLE,
                             USER_NAME = q.ORCL_NAME
                         }).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Update_Group([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<EXP_MESSAGE_RECIPIENT_GROUP> EXP_MESSAGE_RECIPIENT_GROUP)
        {
            if (EXP_MESSAGE_RECIPIENT_GROUP != null)
            {
                foreach (EXP_MESSAGE_RECIPIENT_GROUP item in EXP_MESSAGE_RECIPIENT_GROUP)
                {
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }

            var query = (from p in Db.EXP_MESSAGE_RECIPIENT_GROUP
                         orderby p.EMRG_TITLE
                         select new
                         {
                             p.EMRG_ID,
                             p.EMRG_TITLE,
                             p.EMRG_STAT
                         }).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_Recipient(string text, int? EMRG_ID, int? EANA_ROW)
        {
            var query = (from a in Db.PAY_PERSONEL
                         join b in Db.SEC_USERS on a.EMP_NUMB equals b.PRSN_EMP_NUMB
                         where b.USER_STATE == "1" && (a.ASTA_CODE == "7" || a.ASTA_CODE == "1" || a.ASTA_CODE == "5" || a.ASTA_CODE == "4" || a.ASTA_CODE == null) && b.USER_NAME != null && (a.FAML_NAME.ToUpper().Contains(text) || a.FIRS_NAME.ToUpper().Contains(text) || text == null)
                         select new
                         {
                             b.ROW_NO,
                             b.ORCL_NAME,
                             FAML_NAME = (b.FIRS_NAME != null ? b.FIRS_NAME : b.PAY_PERSONEL.FIRS_NAME) + " " + (b.FAML_NAME != null ? b.FAML_NAME : b.PAY_PERSONEL.FAML_NAME) + "-" + b.ORCL_NAME,
                         })
                         .Union(
                         from b in Db.SEC_USERS
                         where b.USER_STATE == "1" && b.PRSN_EMP_NUMB == null && (b.FAML_NAME != null) && (b.FAML_NAME.ToUpper().Contains(text) || b.FIRS_NAME.ToUpper().Contains(text) || text == null)
                         orderby b.FAML_NAME
                         select new
                         {
                             b.ROW_NO,
                             b.ORCL_NAME,
                             FAML_NAME = (b.FIRS_NAME) + " " + (b.FAML_NAME) + "-" + b.ORCL_NAME,
                         }).ToList();

            //     if (EMRG_ID != null)
            //  {
            //
            //       query = (from b in Db.SEC_USERS
            //                   join a in Db.SEC_USER_TYPE_POST on b.ROW_NO equals a.SCSU_ROW_NO
            //
            //                  // where (a.EANA_EANA_ROW == EANA_ROW || EANA_ROW == null)
            //                   select new
            //                   {
            //                       b.ORCL_NAME,
            //                       FAML_NAME = (b.FIRS_NAME != null ? b.FIRS_NAME : b.PAY_PERSONEL.FIRS_NAME) + " " + (b.FAML_NAME != null ? b.FAML_NAME : b.PAY_PERSONEL.FAML_NAME) + "-" + b.ORCL_NAME,
            //                   })
            //                   .Union
            //                   (
            //                   from a in Db.EXP_MESSAGE_GROUP_USER
            //                   join b in Db.SEC_USERS on a.SCSU_ROW_NO equals b.ROW_NO
            //
            //                   where (a.EMRG_EMRG_ID == EMRG_ID || EMRG_ID == null)
            //                   select new
            //                   {
            //                       b.ORCL_NAME,
            //                       FAML_NAME = (b.FIRS_NAME != null ? b.FIRS_NAME : b.PAY_PERSONEL.FIRS_NAME) + " " + (b.FAML_NAME != null ? b.FAML_NAME : b.PAY_PERSONEL.FAML_NAME) + "-" + b.ORCL_NAME,
            //
            //
            //                   }).ToList()
            //                   ;
            //  }
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_Recipient_Sms(string text, int? EMRG_ID, int? EANA_ROW)
        {
            var query = (from a in Db.PAY_PERSONEL
                         join b in Db.SEC_USERS on a.EMP_NUMB equals b.PRSN_EMP_NUMB
                         where b.USER_STATE == "1"
                         //&& (a.ASTA_CODE == "7" || a.ASTA_CODE == "1" || a.ASTA_CODE == "5" || a.ASTA_CODE == "4" || a.ASTA_CODE == null)
                         && b.USER_NAME != null && (a.FAML_NAME.ToUpper().Contains(text) || a.FIRS_NAME.ToUpper().Contains(text) || text == null)
                         select new
                         {
                             b.ROW_NO,
                             b.ORCL_NAME,
                             FAML_NAME = (b.FIRS_NAME != null ? b.FIRS_NAME : b.PAY_PERSONEL.FIRS_NAME) + " " + (b.FAML_NAME != null ? b.FAML_NAME : b.PAY_PERSONEL.FAML_NAME) + "-" + a.MOBIL,
                         })
                         .Union(
                         from b in Db.SEC_USERS
                         where b.USER_STATE == "1" && b.PRSN_EMP_NUMB == null && (b.FAML_NAME != null) && (b.FAML_NAME.ToUpper().Contains(text) || b.FIRS_NAME.ToUpper().Contains(text) || text == null)
                         orderby b.FAML_NAME
                         select new
                         {
                             b.ROW_NO,
                             b.ORCL_NAME,
                             FAML_NAME = (b.FIRS_NAME) + " " + (b.FAML_NAME) + "-" + b.ORCL_NAME,
                         }).ToList();


            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_groups()
        {
            var RetVal = (from b in Db.EXP_MESSAGE_RECIPIENT_GROUP
                          where b.CRET_BY == username
                          orderby b.EMRG_TITLE
                          select new { b.EMRG_ID, b.EMRG_TITLE }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Get_Type_Doc()
        {
            var RetVal = (from b in Db.EXP_TYPE_DOC

                          orderby b.ETDO_DESC
                          select new { b.ETDO_ID, b.ETDO_DESC }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Get_Template(int? ETDO_ETDO_ID)
        {
            var RetVal = (from b in Db.SMS_TEMPLATE
                          where b.STAT == 1 && (b.ISQUERY == 1 || b.ISQUERY == 0) && (b.ETDO_ETDO_ID == ETDO_ETDO_ID || ETDO_ETDO_ID == null)
                          orderby b.BODY
                          select new { b.ID, b.BODY, b.TITL, b.ETDO_ETDO_ID, b.STAT }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_groups_operator()
        {
            var RetVal = (from b in Db.EXP_GROUPS
                          where b.GROP_CODE == 88
                          orderby b.GROP_DESC
                          select new { b.GROP_ID, b.GROP_DESC }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_groups_analyzer()
        {
            var RetVal = (from b in Db.EXP_ANALYZOR_EVENT
                          orderby b.EANA_DESC
                          where b.ACTV_TYPE == "1"
                          select new { b.EANA_ROW, b.EANA_DESC }).ToList();
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_Message([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from p in Db.EXP_MESSAGE
                         where p.CRET_BY == username && p.EXME_STAT != 3
                         orderby p.EXME_SEND_DATE
                         select new
                         {
                             p.EXME_ID,
                             p.EXME_TITLE,
                             p.EXME_BODY,
                             p.EXME_SEND_DATE,
                             p.EXME_STAT
                         }).OrderByDescending(xx => xx.EXME_SEND_DATE).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Get_Send_SMS([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from p in Db.EXP_MESSAGE
                         where p.CRET_BY == username && p.EXME_STAT == 3
                         orderby p.EXME_SEND_DATE
                         select new
                         {
                             p.EXME_ID,
                             p.EXME_TITLE,
                             p.EXME_BODY,
                             p.EXME_SEND_DATE,
                             p.EXME_STAT
                         }).OrderByDescending(xx => xx.EXME_ID).ToList();

            return Json(query.ToDataSourceResult(request));
        }

        //~ShiftController()
        //{
        //    //این دستور کانکشن را دیسکانکت میکند
        //    Db.Dispose();
        //}
    }

}

using Asr.Base;
using Equipment.Codes.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Asr.Text;
using Asr.Security;
using Equipment.Codes.Globalization;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Equipment.Codes;
using System.IO;
using Equipment.App_LocalResources.Support;
using Equipment.App_GlobalResources;

namespace Equipment.Controllers.WebSite
{
    //[Authorize]
    [GroupAuthorize("Administrators")]
    [Developer("Amirhossein.S")]
    public class SupportController : DbController
    {
        ////
        //// GET: /Support/
        //Equipment.Models.BandarEntities Db;

        //public SupportController()
        //{

        //    Db = this.DB();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        Db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        public ActionResult Index()
        {
            //Equipment.Codes.AsrDownloadManager dmg = new Codes.AsrDownloadManager();
            // dmg.Downloads = new List<Codes.AsrDownloadPackage>();
            //Codes.AsrDownloadPackage pkg=new Codes.AsrDownloadPackage();
            //pkg.Categury="WebBrowsers";
            //pkg.Id=1;
            //pkg.PackageName="Google Chrome";
            //pkg.Files = new List<Codes.AsrPackageFile>();
            //pkg.Files.Add(new Codes.AsrPackageFile { Id = 1, Label = "Google Chrome V43 x64", Path = "downloads/sadd.zip", Platform = Codes.AsrPackageFilePlatform.x64, Version = new Version("48.0.2564.103").ToString() });
            //pkg.Files.Add(new Codes.AsrPackageFile { Id = 2, Label = "Google Chrome V43 32bit", Path = "downloads/sadd32.zip", Platform = Codes.AsrPackageFilePlatform.x86, Version = new Version("48.0.2564.103").ToString() });
            //dmg.Downloads.Add(pkg);
            //dmg.SaveChanges();
            string fullName = string.Empty;
            string username = User.Identity.Name;
            AsrLoginInfo lginfo = new AsrLoginInfo();
            if (!string.IsNullOrEmpty(username))
            {
                using (AsrMembershipProvider mp = new AsrMembershipProvider(username))
                {
                    var usr = mp.AllUsers.First(x => x.USER_NAME.ToUpper().Trim() == username.ToUpper().Trim());
                    if (usr != null)
                    {
                        fullName = string.Format("{0} {1}", usr.FIRS_NAME, usr.FAML_NAME);
                        lginfo = mp.GetLastLoginInfo(GlobalConst.CurrentSubSystemId);
                    }
                }
            }

            ViewBag.UserFullName = fullName;
            ViewBag.UserIp = lginfo.Ip.ConvertNumbersToPersian();
            ViewBag.UserBrowserInfo = lginfo.BrowserInfo.ConvertNumbersToPersian();
            ViewBag.LoginDateTime = lginfo.LoginDateTime.ConvertNumbersToPersian();
            return View();
        }


        public PartialViewResult Dashboard()
        {
            ViewBag.CountOfUnReviewedRequetes = Db.PUB_USER_REQUEST.Count(x => x.URRQ_STAT == "U_CREATE");
            ViewBag.CountOfReviewingRequetes = Db.PUB_USER_REQUEST.Count(x => x.URRQ_STAT == "D_REVIEW");
            ViewBag.CountOfCompletedRequetes = Db.PUB_USER_REQUEST.Count(x => x.URRQ_STAT == "COMPLETE");
            return PartialView("DashboardPartial");
        }

        public PartialViewResult Requestes()
        {
            return PartialView("RequestesPartial");
        }

        public PartialViewResult Errors()
        {
            return PartialView("ErrorsPartial");
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult OnlineUsers()
        {
            NotificationHub hb = new NotificationHub();
            return PartialView("OnlineUsersPartial", hb.GetConnectedUsers().ToList());
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult ManageDownload()
        {
            //NotificationHub hb = new NotificationHub();
            //return PartialView("OnlineUsersPartial", hb.GetConnectedUsers().ToList());
            Codes.AsrDownloadManager dmgr = new Codes.AsrDownloadManager();
            return PartialView("ManageDownloadPartial", dmgr);
        }

        //ManageDownload
        public JsonResult ExceptionGridRead(int current, string searchPhrase, int rowCount)
        {
            using (SecurityEntities secdb = new SecurityEntities(true))
            {
                PersianDateTime pdt = new PersianDateTime();
                pdt.GetShortDate();
                int errIdtoSearch = 0;
                int.TryParse(searchPhrase, out errIdtoSearch);
                var data = string.IsNullOrEmpty(searchPhrase) ? secdb.LOG_ERROR.OrderByDescending(x => x.LGER_ID) : secdb.LOG_ERROR.Where(x => x.LGER_ID == errIdtoSearch).OrderByDescending(x => x.LGER_ID);
                int total = data.Count();
                return Json(new
                {
                    current = current,
                    rowCount = rowCount,
                    total = total,
                    rows = data.Skip((current - 1) * rowCount)
                               .Take(rowCount)
                               .AsEnumerable()
                               .Select(x => new
                               {
                                   x.LGER_ID,
                                   x.LGER_TEXT,
                                   x.LGER_IP,
                                   x.LGER_CLIE,
                                   x.LGER_BROW,
                                   CRET_DATE = pdt.GetShortDate(x.CRET_DATE) + " " + x.CRET_DATE.ToShortTimeString(),
                                   USRN = GetUserNameById(x.SCSU_ROW_NO)
                                   //CRET_DATE = PersianDateTime.GetPrettyDate(x.CRET_DATE)
                               })
                               .ToList()
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private string GetUserNameById(int? userId, bool showFullName = false)
        {
            if (!userId.HasValue)
                return "--";
            var user = Db.SEC_USERS.Find(userId.GetValueOrDefault());

            return user != null ? (user.USER_NAME) : "--";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <param name="searchPhrase"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public JsonResult RequestesGridRead(int current, string searchPhrase, int rowCount)
        {
            PersianDateTime pdt = new PersianDateTime();
            pdt.GetShortDate();
            int errIdtoSearch = 0;
            int.TryParse(searchPhrase, out errIdtoSearch);
            var data = string.IsNullOrEmpty(searchPhrase) ? Db.PUB_USER_REQUEST.OrderByDescending(x => x.URRQ_ID) : Db.PUB_USER_REQUEST.Where(x => x.URRQ_ID == errIdtoSearch).OrderByDescending(x => x.URRQ_ID);
            int total = data.Count();
            return Json(new
            {
                current = current,
                rowCount = rowCount,
                total = total,
                rows = data.Skip((current - 1) * rowCount)
                           .Take(rowCount)
                           .AsEnumerable()
                           .Select(x => new
                           {
                               x.URRQ_ID,
                               x.URRQ_SUBJ,
                               x.URRQ_STAT,
                               x.CRET_BY,
                               USR = GetUserNameById(x.SCSU_ROW_NO),
                               //CRET_DATE = pdt.GetShortDate(x.CRET_DATE) + " " + x.CRET_DATE.ToShortTimeString()
                               CRET_DATE = string.Format(PersianDateTime.GetPrettyDate(x.CRET_DATE).ConvertNumbersToPersian().Replace("AM", "صبح").Replace("PM", "ب.ظ"))
                           })
                           .ToList()
            }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetExceptionCategury()
        {
            var data = Db.Database.SqlQuery<ExceptionSummery>("select lger_type as EXCEPTION_TYPE ,Count(*) as EXCEPTION_COUNT from log_error group by lger_type order by 2 desc");
            //  return Json(data.ToList(), JsonRequestBehavior.AllowGet);
            return Json(data.Select(x => new { label = x.EXCEPTION_TYPE, value = x.EXCEPTION_COUNT }).ToList(), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult GetErrorPartial(int id)
        {
            using (Asr.Security.SecurityEntities secDb = new Asr.Security.SecurityEntities(true))
            {
                Asr.Security.LOG_ERROR err = secDb.LOG_ERROR.Find(id);
                return PartialView("ErrViewerPartial", err);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult GetRequestPartial(int id)
        {
            var request = Db.PUB_USER_REQUEST.Find(id);
            using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider(request.SCSU_ROW_NO.GetValueOrDefault()))
            {
                ViewBag.UserName = string.Format("{0} {1} ({2})", mp.User.FIRS_NAME, mp.User.FAML_NAME, mp.User.USER_NAME);
            }
            return PartialView("ReqViewerPartial", request);
        }


        public PartialViewResult GetRequestTimeLinePartial(int id)
        {
            var messages = Db.PUB_USER_REQUEST_MSG.Where(x => x.URRQ_URRQ_ID == id);
            return PartialView("RequestTimeLinePartial", messages.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult SuccessRequest(int id)
        {
            var requestToStartReviewing = Db.PUB_USER_REQUEST.Find(id);
            if (requestToStartReviewing != null)
            {
                if (requestToStartReviewing.URRQ_STAT != "U_CREATE")
                {
                    //COMPLETE
                    if (requestToStartReviewing.URRQ_STAT == "D_REJECT")
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = SupportResx.MsgRequestRejected }.ToJson();
                    if (requestToStartReviewing.URRQ_STAT == "COMPLETE")
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = SupportResx.MsgRequestCompletedAgo }.ToJson();
                }
                requestToStartReviewing.URRQ_STAT = "COMPLETE";
                requestToStartReviewing.URRQ_DVLP = User.Identity.Name;
                var usrReViewer = this.UserInfo();
                TimeLineItem timeLineMsg = new TimeLineItem()
                {
                    Inverted = true,
                    HasBadge = true,
                    BadgeIconCssClass = "glyphicon glyphicon-thumbs-up",
                    BadgeTypeCssClass = "success",
                    MessageHeader = SupportResx.MsgUserRequestSuccess,
                    MessageBody = string.Format(SupportResx.MsgRequestAgentReviewer, usrReViewer.GetFullName(), User.Identity.Name)
                };
                var msg = new PUB_USER_REQUEST_MSG
                {
                    URMG_TEXT = Newtonsoft.Json.JsonConvert.SerializeObject(timeLineMsg),
                    URMG_TYPE = "SYSTEM_COMPLETE_REQUEST_NOTIFY"
                };
                requestToStartReviewing.PUB_USER_REQUEST_MSG.Add(msg);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = SupportResx.MsgRequestStatusChanged }.ToJson();
            }
            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = GlobalResurces.MsgErrorInOperation }.ToJson();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult StartReviewingRequest(int id)
        {
            var requestToStartReviewing = Db.PUB_USER_REQUEST.Find(id);
            if (requestToStartReviewing != null)
            {
                if (requestToStartReviewing.URRQ_STAT == "D_REVIEW")
                {
                    //if (requestToStartReviewing.URRQ_STAT == "D_REJECT")
                    //   return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "این درخواست رد شده است" }.ToJson();
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = SupportResx.MsgRequestReviewing }.ToJson();
                }

                string messageHeader = SupportResx.MsgStartRequestReviewing;
                if (requestToStartReviewing.URRQ_STAT == "D_REJECT")
                    messageHeader = SupportResx.MsgReviewingAfterReject;
                else if (requestToStartReviewing.URRQ_STAT == "COMPLETE")
                    messageHeader = SupportResx.MsgReviewingAfterComplete;

                requestToStartReviewing.URRQ_STAT = "D_REVIEW";
                requestToStartReviewing.URRQ_DVLP = User.Identity.Name;
                var usrReViewer = this.UserInfo();
                TimeLineItem timeLineMsg = new TimeLineItem()
                {
                    Inverted = true,
                    HasBadge = true,
                    BadgeIconCssClass = "glyphicon glyphicon-wrench",
                    BadgeTypeCssClass = "primary",
                    MessageHeader = messageHeader,
                    MessageBody = string.Format(SupportResx.MsgRequestAgentReviewer, usrReViewer.GetFullName(), User.Identity.Name)
                };
                var msg = new PUB_USER_REQUEST_MSG
                {
                    URMG_TEXT = Newtonsoft.Json.JsonConvert.SerializeObject(timeLineMsg),
                    URMG_TYPE = "SYSTEM_START_REVIEWING_NOTIFY"
                };
                requestToStartReviewing.PUB_USER_REQUEST_MSG.Add(msg);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = SupportResx.MsgRequestStatusChanged }.ToJson();
            }
            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = GlobalResurces.MsgErrorInOperation }.ToJson();
        }

        public JsonResult RejectRequest(int id)
        {
            var requestToStartReviewing = Db.PUB_USER_REQUEST.Find(id);
            if (requestToStartReviewing != null)
            {
                if (requestToStartReviewing.URRQ_STAT == "D_REJECT")
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = SupportResx.MsgRequestRejected }.ToJson();
                else if (requestToStartReviewing.URRQ_STAT == "COMPLETE")
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = SupportResx.MsgCanNotRejectBecauseComplete }.ToJson();

                requestToStartReviewing.URRQ_STAT = "D_REJECT";
                requestToStartReviewing.URRQ_DVLP = User.Identity.Name;
                var usrReViewer = this.UserInfo();
                TimeLineItem timeLineMsg = new TimeLineItem()
                {
                    Inverted = true,
                    HasBadge = true,
                    BadgeIconCssClass = "glyphicon glyphicon-remove",
                    BadgeTypeCssClass = "danger",
                    MessageHeader = "رد درخواست",
                    MessageBody = string.Format(SupportResx.MsgRequestAgentReviewer, usrReViewer.GetFullName(), User.Identity.Name)
                };
                var msg = new PUB_USER_REQUEST_MSG
                {
                    URMG_TEXT = Newtonsoft.Json.JsonConvert.SerializeObject(timeLineMsg),
                    URMG_TYPE = "SYSTEM_REJECT_NOTIFY"
                };
                requestToStartReviewing.PUB_USER_REQUEST_MSG.Add(msg);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = SupportResx.MsgRequestStatusChanged }.ToJson();
            }
            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = GlobalResurces.MsgErrorInOperation }.ToJson();
        }

        [AllowAnonymousAttribute()]
        public JsonResult SendMessage(int requestId, int reciver, string messageText, string senderPanel)
        {
            var requestToAddMessage = Db.PUB_USER_REQUEST.Find(requestId);
            if (requestToAddMessage != null)
            {
                requestToAddMessage.URRQ_STAT = "D_ANSWER";
                var usrReViewer = this.UserInfo();
                TimeLineItem timeLineMsg = new TimeLineItem()
                {
                    Inverted = (senderPanel == "AdminPanel"),
                    HasBadge = true,
                    BadgeIconCssClass = "glyphicon glyphicon-envelope",
                    BadgeTypeCssClass = "info",
                    MessageHeader = SupportResx.MsgMessageSupportToUser,
                    MessageBody = string.Format(SupportResx.MsgMessageSender, usrReViewer.GetFullName(), User.Identity.Name, messageText)
                };
                var msg = new PUB_USER_REQUEST_MSG
                {
                    URMG_TEXT = Newtonsoft.Json.JsonConvert.SerializeObject(timeLineMsg),
                    URMG_TYPE = "D_MSG"
                };
                requestToAddMessage.PUB_USER_REQUEST_MSG.Add(msg);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = SupportResx.MsgMessageSent }.ToJson();
            }
            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = GlobalResurces.MsgErrorInOperation }.ToJson();
        }

        public JsonResult DisconnectUser(string id)
        {
            NotificationHub hp = new NotificationHub();
            hp.SendMessageToUser(User.Identity.Name, id, "", NotificationType.UserLogOut);
            return new ServerMessages(ServerOprationType.Success) { Message = SupportResx.MsgCommandSent }.ToJson();
        }


        public JsonResult SendMessageToUser(string userId, string messageTitle, string messageContent, string messageType)
        {
            NotificationHub hp = new NotificationHub();
            hp.SendMessageToUser(User.Identity.Name, userId, new
            {
                Title = messageTitle,
                Message = messageContent,
                MessageType = messageType
            }, NotificationType.UserShowMessage);
            return new ServerMessages(ServerOprationType.Success) { Message = SupportResx.MsgMessageSent }.ToJson();
        }

        public PartialViewResult GetUserMenuPartial(string id)
        {
            using (Asr.Security.AsrMembershipProvider mp = new AsrMembershipProvider(id))
            {
                ViewData["userId"] = mp.User.ROW_NO;
                ViewData["MasterMenu"] = "no";
                return PartialView("_MenuPartial");
            }
        }

        public PartialViewResult GetDownloadFilesListPartial(string id, string filter)
        {
            AsrDownloadManager dm = new AsrDownloadManager();
            if (string.IsNullOrEmpty(filter))
            {
                if (id == "ALL")
                {
                    var model = dm.Downloads;
                    return PartialView("DownloadFilesListPartial", model);
                }
                else if (string.IsNullOrEmpty(id))
                {
                    ViewBag.Filter = "دسته بندی نشده";
                    var model = dm.Downloads.Where(x => string.IsNullOrEmpty(x.Category));
                    return PartialView("DownloadFilesListPartial", model);
                }
                else
                {
                    ViewBag.Filter = id;
                    var model = dm.Downloads.Where(x => id == x.Category);
                    return PartialView("DownloadFilesListPartial", model);
                }
            }
            else
            {
                ViewBag.Filter = filter;
                var model = dm.Downloads.Where(x => x.Category.ToLower().Contains(filter.ToLower()) || x.PackageName.ToLower().Contains(filter.ToLower()));
                return PartialView("DownloadFilesListPartial", model);
            }
        }

        public JsonResult OpenTabForUser(string userId, string url, string tabheader, string tabOptions)
        {
            //Url, content.TabTitle
            NotificationHub hp = new NotificationHub();
            hp.SendMessageToUser(User.Identity.Name, userId, new
            {
                Url = url,
                TabTitle = tabheader,
                Options = tabOptions
            }, NotificationType.UserOpenTab);
            return new ServerMessages(ServerOprationType.Success) { Message = SupportResx.MsgMessageSent }.ToJson();
        }

        [HttpPost]
        public ActionResult AddNewPackage(IEnumerable<HttpPostedFileBase> files)
        {
            var form = Request.Form;
            var fm = new AsrDownloadManager();
            var package = new AsrDownloadPackage();

            package.PackageName = form["PackageName"];
            package.Category = form["PackageCategory"];
            int packId = 1;
            if (fm.Downloads.Where(x => x.Category == package.Category).Any())
            {
                var maxId = fm.Downloads.Where(x => x.Category == package.Category).Max(x => x.Id);
                packId = maxId + 1;
            }
            package.Id = packId;

            bool isSavedSuccessfully = true;
            string fName = "";
            int counter = 0;
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        var packFile = new AsrPackageFile();
                        counter++;
                        packFile.Id = counter;
                        packFile.Label = form[string.Format("fileName{0}", counter)];
                        packFile.Version = form[string.Format("fileVersion{0}", counter)];
                        try
                        {
                            packFile.Platform = (AsrPackageFilePlatform)Enum.Parse(typeof(AsrPackageFilePlatform), form[string.Format("filePlatform{0}", counter)]);
                        }
                        catch
                        {
                            packFile.Platform = AsrPackageFilePlatform.Any;
                        }

                        var originalDirectory = new DirectoryInfo(string.Format("{0}Content\\downloads\\", Server.MapPath(@"\")));

                        packFile.FileSize = file.ContentLength.ToString();
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), package.Category, package.PackageName);

                        var fileName1 = Path.GetFileName(file.FileName);
                        string linkPath = string.Format("~/Content/downloads/{0}/{1}/{2}", package.Category, package.PackageName, fileName1);
                        packFile.Path = linkPath;
                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = Path.Combine(pathString, fileName1);
                        file.SaveAs(path);
                        package.Files.Add(packFile);
                    }
                }

                fm.Downloads.Add(package);
                fm.SaveChanges();
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }

            if (isSavedSuccessfully)
                return Json(new { Message = "فایل ها با موفقیت آپلود شدند" });
            else
                return Json(new { Message = "Error in saving file" });
        }

        #region manage workflow codes

        public PartialViewResult ManageWfInstances()
        {
            using (Asr.Cartable.Models.OraOwfEngineConnStr cntx = new Asr.Cartable.Models.OraOwfEngineConnStr())
            {
                ViewBag.ItemTypes = cntx.WF_ITEM_TYPES_TL.OrderBy(x => x.DISPLAY_NAME).ToList();
                return PartialView("ManageWfInstancesPartial");
            }
        }

        //WfItemsGridRead
        public JsonResult WfItemsGridRead(int current, string searchPhrase, int rowCount, string itemType)
        {
            if (string.IsNullOrEmpty(itemType))
                return Json(new
              {
                  current = current,
                  rowCount = rowCount,
                  total = 0,
                  rows = ""
              }, JsonRequestBehavior.AllowGet);

            using (Asr.Cartable.Models.OraOwfEngineConnStr cntx = new Asr.Cartable.Models.OraOwfEngineConnStr())
            {
                // PersianDateTime pdt = new PersianDateTime();
                // pdt.GetShortDate();
                var data = string.IsNullOrEmpty(searchPhrase) ? cntx.WF_ITEMS.Where(x => x.ITEM_TYPE == itemType).OrderByDescending(x => x.BEGIN_DATE) : cntx.WF_ITEMS.Where(x => x.ITEM_TYPE == itemType && x.ITEM_KEY.Contains(searchPhrase)).OrderByDescending(x => x.BEGIN_DATE);
                int total = data.Count();
                return Json(new
                {
                    current = current,
                    rowCount = rowCount,
                    total = total,
                    rows = data.Skip((current - 1) * rowCount)
                               .Take(rowCount)
                               .AsEnumerable()
                               .Select(x => new
                               {
                                   x.ITEM_TYPE,
                                   x.ITEM_KEY,
                                   x.ROOT_ACTIVITY,
                                   x.ROOT_ACTIVITY_VERSION,
                                   x.BEGIN_DATE,
                                   x.END_DATE
                                   //CRET_DATE = pdt.GetShortDate(x.CRET_DATE) + " " + x.CRET_DATE.ToShortTimeString()
                                   //CRET_DATE = string.Format(PersianDateTime.GetPrettyDate(x.CRET_DATE).ConvertNumbersToPersian().Replace("AM", "صبح").Replace("PM", "ب.ظ"))
                               })
                               .ToList()
                }, JsonRequestBehavior.AllowGet);

            }

        }

        #endregion

    }

    public class TimeLineItem
    {
        public TimeLineItem()
        {

        }
        public int Id { get; set; }
        public string MessageHeader { get; set; }
        public string MessageBody { get; set; }
        public bool Inverted { get; set; }
        public bool HasBadge { get; set; }
        public string BadgeTypeCssClass { get; set; }
        public string BadgeIconCssClass { get; set; }
    }

    public class ExceptionSummery
    {
        public string EXCEPTION_TYPE { get; set; }
        public int EXCEPTION_COUNT { get; set; }
    }

}

using Asr.Text;
using Equipment.Models.CoustomModel;
using System;
using System.Linq;
using System.Web.Mvc;
//using Equipment.Hubs;

namespace Equipment.Controllers.WebSite
{
    [Authorize]
    public class HomeController : DbController
    {

        //[MenuAuthorize]
        public ActionResult Index(string returnUrl)
        {
            string fullName = string.Empty;
            string username = User.Identity.Name;
            Asr.Security.AsrLoginInfo lginfo = new Asr.Security.AsrLoginInfo();
            System.Collections.Generic.List<Asr.Security.SEC_TAFIEZ> successions = new System.Collections.Generic.List<Asr.Security.SEC_TAFIEZ>();
            if (!string.IsNullOrEmpty(username))
            {
                using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider(username))
                {
                    var usr = mp.AllUsers.First(x => x.USER_NAME.ToUpper().Trim() == username.ToUpper().Trim());
                    if (usr != null)
                    {
                        fullName = string.Format("{0} {1}", usr.FIRS_NAME, usr.FAML_NAME);
                    }
                    lginfo = mp.GetLastLoginInfo(GlobalConst.CurrentSubSystemId);
                    ViewBag.UserActivities = mp.GetJobs(false).ToList();
                    successions = mp.GetSuccession(DateTime.Now).ToList();
                    //ViewBag.UserJobs = mp.GetJobs(true).ToList();
                    ViewBag.UserGroups = mp.JoinedGroups.ToList();
                    ViewBag.ShowMap = mp.CheckPermission("PUB_DISPLAY_MAP");
                }
            }
            ViewBag.UserSuccessions = successions;
            ViewBag.UserFullName = fullName;
            ViewBag.UserIp = lginfo.Ip.ConvertNumbersToPersian();
            ViewBag.UserBrowserInfo = lginfo.BrowserInfo.ConvertNumbersToPersian();
            ViewBag.LoginDateTime = lginfo.LoginDateTime.ConvertNumbersToPersian();
            ViewBag.UserInfo = this.UserInfo().UserId;
            ViewBag.PersianDate = Db.Database.SqlQuery<string>(string.Format("SELECT farsi_date_u(to_date('{0}','mm/dd/yyyy')) FROM DUAL", DateTime.Now.ToShortDateString())).FirstOrDefault().ToString();
            //ViewBag.SecUsersLOV = Db.Database.SqlQuery<Equipment.Models.PUB_SYS_LOV>("SELECT * FROM PUB_SYS_LOV WHERE NAME = 'SEC_USERS_LOV'").FirstOrDefault();
            ViewBag.SecUsersLOV = Db.PUB_SYS_LOV.Find("SEC_USERS_LOV");

            return View("Index4");
        }

        public ActionResult Body()
        {
            //HttpContext.User.Identity
            return View();
        }

        //,
        public ActionResult Header()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult tree()
        {
            return View("tree");
        }

        public ActionResult insert()
        {
            return View("insert");
        }

        public ActionResult index2()
        {
            return View("index2");
        }
        public ActionResult dashboard()
        {
            return View("dashboard");
        }
        public ActionResult map()
        {
            return View("map");
        }
        public ActionResult index3()
        {
            return View("index3");
        }


        public ActionResult index4(string returnUrl)
        {
            return RedirectToAction("Index", new { returnUrl = returnUrl });
        }


        public ActionResult DefultTab()
        {
            return PartialView("DefultTab1");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="repeatPassword"></param>
        /// <returns></returns>
        public JsonResult ChangeUserPassword(string currentPassword, string newPassword, string repeatPassword, string pass)
        {
            bool retval = false;
            int userId = 0;
            if (Asr.Security.AsrMembershipProvider.Login(User.Identity.Name, currentPassword, pass, out userId))
            {
                using (var mp = new Asr.Security.AsrMembershipProvider(userId))
                {
                    retval = mp.ChangePassword("", newPassword, "");
                }
            }

            if (retval)
                return new ServerMessages(ServerOprationType.Success) { Message = "کلمه عبور کاربر تغییر کرد" }.ToJson();
            else
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "کلمه عبور کاربر تغییر نکرد" }.ToJson();
        }



        public JsonResult CreateSuccession(int userIdToSussession, DateTime startDate, DateTime endDate, bool activeStatus, decimal[] jdtyids)
        {
            try
            {
                jdtyids = jdtyids ?? new decimal[0];

                using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider(User.Identity.Name))
                {
                    mp.SetSuccession(userIdToSussession, startDate, endDate, activeStatus, jdtyids);
                }
                return new ServerMessages(ServerOprationType.Success) { Message = "جانشینی برای کاربر مورد نظر ایجاد شد" }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();

            }
        }

        /// <summary>
        /// change succession status
        /// </summary>
        /// <param name="tafzId"></param>
        /// <returns></returns>
        public JsonResult ToggleSuccessionStatus(int tafzId)
        {
            try
            {
                if (tafzId <= 0)
                    return new ServerMessages(ServerOprationType.Failure).ToJson();
                using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider(User.Identity.Name))
                {
                    if (mp.ToggleSuccession(tafzId))
                    {
                        return new ServerMessages(ServerOprationType.Success).ToJson();
                    }
                    else
                    {
                        return new ServerMessages(ServerOprationType.Failure).ToJson();
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure).ToJson();
            }
        }


        public ActionResult TafhizUserPartial(Asr.Security.SEC_TAFIEZ item)
        {
            if (item.SCSU_ROW_NO_R.HasValue)
            {
                var usr = Db.SEC_USERS.FirstOrDefault(x => x.ROW_NO == item.SCSU_ROW_NO_R);
                if (User != null)
                {

                    ViewBag.TAFZ_ID = item.TAFZ_ID;
                    ViewBag.TAFZ_STATUS = item.TAFZ_STATUS;
                    return PartialView(usr);
                }
            }
            return Content("");
        }

        public ActionResult UserJobItemPartial(Asr.Security.SEC_JOB_TYPE_DOC item)
        {
            if (item.ETDO_ETDO_ID.HasValue)
            {
                var doc = Db.EXP_TYPE_DOC.FirstOrDefault(x => x.ETDO_ID == item.ETDO_ETDO_ID);
                if (User != null)
                {
                    ViewBag.ACTIV_FNAM = item.ACTIV_FNAM;
                    return PartialView(doc);
                }
            }
            return Content("");
        }

    }

}

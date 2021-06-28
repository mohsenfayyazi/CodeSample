using Asr.Text.Cryptography;
using Equipment.App_LocalResources.Account;
using Equipment.Codes.Security;
using Equipment.Models.CoustomModel;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Equipment.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            //So that the user can be referred back to where they were when they click logon
            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }
            ViewBag.DateTimeNow = DateTime.Now.ToShortDateString();
            return View("Login2");
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return RedirectToAction("Index", new { returnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginUser(string username, string password,string pswUnEncripted, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || (username.ToUpper() == "SHIRAZ" && pswUnEncripted.ToUpper() == "SHIRAZ"))
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = AccountResx.MsgWrongUsernameAndPassword }.ToJson();

                    int userId = 0;
                    if (Asr.Security.AsrMembershipProvider.Login(username, password, pswUnEncripted, out userId))
                    {
                        using (Asr.Security.AsrMembershipProvider usr = new Asr.Security.AsrMembershipProvider(userId))
                        {
                            if (usr.AccountEnabled())
                            {
                                FormsAuthentication.SetAuthCookie(usr.User.USER_NAME/*.EncryptToAes("dslihgfkwjhg52398842309478@345987rtdsklfjhsd")*/, false);
                                Session["UserPassword"] = usr.User.ORCL_PASS.EncryptToAes("dslihgfkwjhg52398842309478@345987rtdsklfjhsd");
                                HttpCookie cookie = new HttpCookie("ASRFINYYEAR");

                                string[] computerName;
                                Session["CurrentUserId"] = usr.User.ROW_NO;
                                try
                                {
                                    computerName = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.Split('.');
                                }
                                catch
                                {
                                    computerName = new string[] { "ComputerName Dosent Detected" };
                                }

                                HttpBrowserCapabilitiesBase browser = Request.Browser;
                                string browserinfo = string.Format("{0} v{1}", browser.Browser, browser.Version);
                                string operationSystemName = string.Format("{0}/{1}", SecurityExtentions.GetOperationSystemName(Request.UserAgent), computerName[0]);
                                string clientIpAddress = "";
                                try
                                {
                                    clientIpAddress = Request.ServerVariables["REMOTE_ADDR"];
                                }
                                catch
                                {
                                    clientIpAddress = "not detected";
                                }
                                usr.LogLogin(clientIpAddress, browserinfo, operationSystemName, GlobalConst.CurrentSubSystemId);
                                AddUserToOnlineList(string.Format("{0} - {1}", usr.User.USER_NAME, clientIpAddress));
                            }
                            else
                            {
                                string msg = string.Format("<strong>{0}</strong><br>{3}<br><br><u><small>{1}:</small></u><br><div style=\"direction:ltr;\">{2}</div><br><small>{4}</small><br><div style=\"direction:ltr;\">{5}</div",
                                    AccountResx.MsgAccountDeactivated,
                                    App_GlobalResources.GlobalResurces.CreatorOfThisSoftware,
                                    App_GlobalResources.GlobalResurces.SupportContactInfo,
                                    App_GlobalResources.GlobalResurces.MsgContactToSupportTeamForCheckProblem,
                                    App_GlobalResources.GlobalResurces.InternalPhoneNo,
                                    App_GlobalResources.GlobalResurces.SupportContactInfoInternal);
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = msg, CoustomData = "ishtml" }.ToJson();
                            }
                        }
                        //Equipment.App_Start.AsrReportingConfig.RegisterConnectionString();

                        return new ServerMessages(ServerOprationType.Success)
                        {
                            Message = AccountResx.MsgLoginSuccessRedirecting,
                            CoustomData = string.IsNullOrEmpty(returnUrl) ? "Account/RedirectToHome" : returnUrl
                        }.ToJson();

                    }
                    else
                    {
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = AccountResx.MsgWrongUsernameAndPassword }.ToJson();
                    }
                }
                else
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = AccountResx.MsgWrongUsernameAndPassword }.ToJson();
            }
            catch (Exception ex)
            {
                long errId = 0;
                this.LogError(ex, out errId);

                return new ServerMessages(ServerOprationType.Failure)
                {
                    ExceptionMessage = ex.ToString()
                }.ToJson();
            }
        }


        private void AddUserToOnlineList(string userinfo)
        {
            if (HttpRuntime.Cache["LoggedInUsers"] != null) //if the list exists, add this user to it
            {
                //get the list of logged in users from the cache
                List<string> loggedInUsers = (List<string>)HttpRuntime.Cache["LoggedInUsers"];
                //add this user to the list
                loggedInUsers.Add(userinfo);
                //add the list back into the cache
                HttpRuntime.Cache["LoggedInUsers"] = loggedInUsers;
            }
            else //the list does not exist so create it
            {
                //create a new list
                List<string> loggedInUsers = new List<string>();
                //add this user to the list
                loggedInUsers.Add(userinfo);
                //add the list into the cache
                HttpRuntime.Cache["LoggedInUsers"] = loggedInUsers;
            }
        }
        private void RemoveUserFromOnlineList(string userinfo)
        {
            if (HttpRuntime.Cache["LoggedInUsers"] != null)//check if the list has been created
            {
                //the list is not null so we retrieve it from the cache
                List<string> loggedInUsers = (List<string>)HttpRuntime.Cache["LoggedInUsers"];
                if (loggedInUsers.Contains(userinfo))//if the user is in the list
                {
                    //then remove them
                    loggedInUsers.Remove(userinfo);
                }
                // else do nothing
            }
        }

        [AllowAnonymous]
        public ActionResult SignOut()
        {
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
                {
                    using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider(HttpContext.User.Identity.Name))
                    {
                        if (mp.User != null)
                        {
                            string clientIpAddress = "";
                            try
                            {
                                clientIpAddress = Request.ServerVariables["REMOTE_ADDR"];
                            }
                            catch
                            {
                                clientIpAddress = "not detected";
                            }
                            RemoveUserFromOnlineList(string.Format("{0} - {1}", HttpContext.User.Identity.Name, clientIpAddress));
                            mp.LogSignOut("0");
                        }
                    }
                }
            }
            catch 
            {
                throw;
            }

            FormsAuthentication.SignOut();
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);
            FormsAuthentication.RedirectToLoginPage();
            return RedirectToAction("index4", "home");
        }

        
        public ActionResult RedirectToHome()
        {
            return RedirectToAction("index4", "home");
        }
    }
}
using Equipment.Models;
using Equipment.Models.CoustomModel;
using System.Web.Mvc;

namespace Equipment.Controllers.WebSite
{
    public class ProblemsController : DbController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowError(string errorTitle, string errorDescription)
        {
            ViewBag.ErrorTitle = errorTitle;
            ViewBag.ErrorDescription = errorDescription;
            return View();
        }

        public ActionResult NotFound(string msg)
        {
            ViewBag.Msg = msg;
            return View();
        }

        public static ProblemsController Create(System.Web.Routing.RequestContext requestContext)
        {
            return ControllerBuilder.Current.GetControllerFactory().CreateController(requestContext, typeof(ProblemsController).Name) as ProblemsController;
        }

        /// <summary>
        /// ایجاد درخواست
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult MakeRequest(PUB_USER_REQUEST model)
        {
            //using (BandarEntities Db=this.DB())
            //{
            model.URRQ_STAT = "U_CREATE";
            model.SCSU_ROW_NO = this.UserInfo().UserId;
            Db.PUB_USER_REQUEST.Add(model);
            Db.SaveChanges();
            NotificationHub hub = new NotificationHub();
            var usInfo = this.UserInfo();
            hub.SendMessageToGroup(usInfo.GetFullName(), "Administrators", "یک درخواست جدید ثبت شد", NotificationType.AdminShowMessage);
            return new ServerMessages(ServerOprationType.Success) { Message = "درخواست شما با موفقیت به تیم پشتیبانی ارسال شد." }.ToJson();
            //}
        }
    }
}

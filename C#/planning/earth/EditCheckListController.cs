using Equipment.DAL;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Planning.Earth
{
    [Authorize]
    public partial class EarthController : Controller
    {
        public ActionResult EditCheckList()
        {
            return View();
        }

       

        public ActionResult EAR_EARTH_CHECK_LIST_Update(int ECHL_ID = 0, short CPRO_CPLA_PLN_CODE = 0, short CPRO_PRJ_CODE = 0, string ECHL_DESC = "", int ERTH_ERTH_ID = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(ECHL_DESC) || ERTH_ERTH_ID == 0 || CPRO_CPLA_PLN_CODE == 0 || CPRO_PRJ_CODE == 0)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات را کامل وارد کنید." }.ToJson();
                }
                if (PublicRepository.ExistModel("EAR_EARTH_CHECK_LIST", "ECHL_DESC='{0}' AND CPRO_CPLA_PLN_CODE={1} AND CPRO_PRJ_CODE={2} AND ERTH_ERTH_ID={3} AND ECHL_ID<>{4}", ECHL_DESC, CPRO_CPLA_PLN_CODE, CPRO_PRJ_CODE, ERTH_ERTH_ID, ECHL_ID))
                {
                    string msg = "از این قالب برای زمین [{0}] و پروژه [{1}]\n مربوط به طرح [{2}]\n چک لیستی با این عنوان ایجاد شده است.";
                    var pln = (from b in PublicRepository.cntx.CGT_PLAN where b.PLN_CODE == CPRO_CPLA_PLN_CODE select b).FirstOrDefault().PLN_DESC;
                    var prj = (from b in PublicRepository.cntx.CGT_PRO where b.PRJ_CODE == CPRO_PRJ_CODE && b.CPLA_PLN_CODE == CPRO_CPLA_PLN_CODE select b).FirstOrDefault().PRJ_DESC;
                    var ear = (from b in EarthRepository.Get_EAR_EARTH() where b.ERTH_ID == ERTH_ERTH_ID select b).FirstOrDefault().ERTH_NAME;
                    msg = string.Format(msg, ear, prj, pln);
                    //string.Format("[{0}] قبلا ثبت شده است.", newInstanceName)
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = msg }.ToJson();
                }

                //using (var cntx = this.DB())
                //{
                var mdl = cntx.EAR_EARTH_CHECK_LIST.Find(ECHL_ID);
                if (mdl == null)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "چک لیست پیدا نشد." }.ToJson();
                }
                else
                {
                    mdl.CPRO_CPLA_PLN_CODE = CPRO_CPLA_PLN_CODE;
                    mdl.CPRO_PRJ_CODE = CPRO_PRJ_CODE;
                    mdl.ECHL_DESC = ECHL_DESC;
                    mdl.ERTH_ERTH_ID = ERTH_ERTH_ID;
                    cntx.SaveChanges();
                }
                //}

                return new ServerMessages(ServerOprationType.Success) { Message = "چک لیست ویرایش شد." }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }
        }

        public ActionResult CheckListDetail(int? id)
        {
            var obj = (from b in EarthRepository.Get_EAR_EARTH_CHECK_LIST() where b.ECHL_ID == id select b).FirstOrDefault();
            ViewBag.chklst = obj;
            return View();
        }

    }

}
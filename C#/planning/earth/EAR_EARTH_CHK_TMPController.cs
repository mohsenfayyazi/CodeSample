using Equipment.DAL;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using System;
using System.Web.Mvc;

namespace Equipment.Controllers.Planning.Earth
{
    [Authorize]
    public partial class EarthController : Controller
    {
        //
        // GET: /EAR_EARTH_CHK_TMP/
        public ActionResult EAR_EARTH_CHK_TMP()
        {           
            return View();
        }

        public ActionResult Add_EAR_EARTH_CHK_TMP(EAR_EARTH_CHK_TMP newModel)
        {
            try
            {
                if (PublicRepository.ExistModel("EAR_EARTH_CHK_TMP", "ECHT_DESC ='{0}'", newModel.ECHT_DESC))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}] تکراری است", newModel.ECHT_DESC) }.ToJson();
                }
                newModel.SaveToDataBase();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", newModel.ECHT_DESC) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }
        }
    }
}

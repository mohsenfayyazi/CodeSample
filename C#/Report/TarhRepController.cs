using Equipment.Codes.Security;
using Kendo.Mvc.UI;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Report
{
    [Authorize]
    public partial class ReportController : Controller
    {

        private void FillViewBag()
        {
            #region Fill Needed Data To ViewBag(Expand To See Content)

            var ownerTypesQuery = from itm in cntx.CGT_KPRO
                                  select new
                                  {
                                      itm.KPRJ_ROW,
                                      itm.KPRJ_DESC
                                  };
            ViewBag.OwnerTypeLST = ownerTypesQuery.ToList();


            #endregion
        }

        //
        // GET: /TarhRep/
        //BandarEntities cntx = new BandarEntities();
        [MenuAuthorize]
        public ActionResult WORK_TYPE()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult CHAPTER()
        {
            return View();
        }

        [EntityAuthorize("TRH_WORK_TYPE > select")]
        public ActionResult GetWorkType()
        {
            var RetVal = from b in cntx.TRH_WORK_TYPE.AsEnumerable() where (b.WORK_STAT == "1") orderby b.WORK_DESC select new { WORK_CODE = string.Format("'{0}'", b.WORK_CODE), b.WORK_DESC };
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [MenuAuthorize]
        public ActionResult Item()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ItemBook()
        {
            return View();
        }

        [EntityAuthorize("BKP_FINANCIAL_YEAR > select")]
        public ActionResult GetFinyYear()
        {
            var RetVal = from b in cntx.BKP_FINANCIAL_YEAR orderby b.FINY_YEAR descending select new { b.FINY_YEAR };
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [MenuAuthorize]
        public ActionResult TechDoc()
        {
            return View();
        }

        [EntityAuthorize("PDF_TECH_DOC > select")]
        public ActionResult GetTechDoc()
        {
            var RetVal = from b in cntx.PDF_TECH_DOC orderby b.TCDC_CODE descending select new { b.TCDC_CODE };
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [MenuAuthorize]
        public ActionResult MinuteDel()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult SignificLet()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult PayDraft()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult InfoCnt()
        {
            return View();
        }

        [EntityAuthorize("CGT_KPRO > select")]
        public ActionResult GetKPRO()
        {
            var RetVal = from b in cntx.CGT_KPRO orderby b.CKPR_KPRJ_ROW descending select new { b.KPRJ_ROW, b.KPRJ_DESC };
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [MenuAuthorize]
        public ActionResult PrjAmal()
        {
            FillViewBag();
            return View();
        }

        [MenuAuthorize]
        public ActionResult StandardActiv()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult StandardActivFirst()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult ItemGroup()
        {
            return View();
        }

        [EntityAuthorize("BKP_BOOK_TYPE > select")]
        public ActionResult GetBookCode()
        {
            var RetVal = from b in cntx.BKP_BOOK_TYPE
                         where (b.BK_CODE != "1")
                         orderby b.BK_CODE
                         select new
                             {
                                 b.BK_CODE,
                                 b.BK_DESC
                             };
            return Json(RetVal, JsonRequestBehavior.AllowGet);
        }

        [MenuAuthorize]
        public ActionResult AmalWorkOrd()
        {
            return View();
        }

        public ActionResult CgtproRead([DataSourceRequest] DataSourceRequest request, string plncode, string Kprjrow)
        {
            //using (BandarEntities cntx = new BandarEntities())
            //{
            int filter = System.Convert.ToInt32(plncode);
            int filter1 = System.Convert.ToInt32(Kprjrow);
            if (filter1 != 0)
            {
                var query = (from b in cntx.CGT_PRO where b.CPLA_PLN_CODE == filter && b.CKPR_KPRJ_ROW == filter1 orderby b.CKPR_KPRJ_ROW, b.PRJ_CODE select b)
                .AsEnumerable().Select(p => new
                     {
                         p.CPLA_PLN_CODE,
                         p.PRJ_CODE,
                         p.PRJ_DESC,
                         p.CKPR_KPRJ_ROW
                     });
                var data = new DataSourceResult
                {
                    Data = query.ToList()
                };
                return Json(data);
            }
            else
            {
                var query = (from b in cntx.CGT_PRO where b.CPLA_PLN_CODE == filter orderby b.CKPR_KPRJ_ROW, b.PRJ_CODE select b)
                    .AsEnumerable().Select(p => new
                      {
                          p.CPLA_PLN_CODE,
                          p.PRJ_CODE,
                          p.PRJ_DESC,
                          p.CKPR_KPRJ_ROW
                      });
                var data = new DataSourceResult
                {
                    Data = query.ToList()
                };
                return Json(data);
            }
            //}
        }

        [MenuAuthorize]
        public ActionResult MadrakWorkord()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult NezaratP()
        {
            return View();
        }

        [MenuAuthorize]
        public ActionResult IncDecr()
        {
            return View();
        }

        // [MenuAuthorize]
        public ActionResult Tamdid()
        {
            return View();
        }

        public ActionResult AsnadM()
        {
            return View();
        }

        public ActionResult Store()
        {
            FillViewBag();
            return View();
        }

    }

}

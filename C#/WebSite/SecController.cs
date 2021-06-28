using Asr.Base;
using Asr.Text;
using Equipment.Codes.Security;
using Equipment.DAL;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Equipment.Controllers.WebSite
{
    //[Authorize]
    [GroupAuthorize("Administrators,SecurityOperator")]
    [Developer("Amirhossein.S")]

    ///<summery>
    ///Security Controller
    ///</summery>
    public class SecController : DbController
    {
        //private BandarEntities Db;

        ///// <summary>
        ///// ctor of security Controller
        ///// </summary>
        //public SecController()
        //{
        //    Db = this.DB();

        //}

        ///// <summary>
        ///// DISPOSE CONTROLLER RESOURSES
        ///// </summary>
        ///// <param name="disposing"></param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        Db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        /// <summary>
        /// main view
        /// </summary>
        /// <returns>Main View Of Security Page</returns>
        public ActionResult Index()
        {
            ViewBag.Doctypes = Db.EXP_TYPE_DOC.OrderBy(x => x.ETDO_DESC);
            return View();
        }
        public ActionResult Add_Mobile(int? id)
        {
            ViewData["Row_no"] = id;

            return View();
        }
        public ActionResult JobTypeDocRead([DataSourceRequest] DataSourceRequest request, int? etdoId)
        {
            if (!etdoId.HasValue)
                return Json(null);

            var query = Db.SEC_JOB_TYPE_DOC
                                 .Where(x => x.ETDO_ETDO_ID == etdoId)
                                 .AsEnumerable()
                                 .Select(p => new
                                 {
                                     p.JDTY_ID,
                                     p.ETDO_ETDO_ID,
                                     p.ACTIV_NAME,
                                     p.ACTIV_FNAM
                                 });
            if (etdoId.HasValue)
                request.Page = 1;
            return Json(query.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTypeDocCreate([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SEC_JOB_TYPE_DOC> models)
        {
            var results = new List<SEC_JOB_TYPE_DOC>();
            foreach (SEC_JOB_TYPE_DOC modelItem in models)
            {
                results.Add(modelItem);

                Db.SEC_JOB_TYPE_DOC.Add(modelItem);
                try
                {
                    Db.SaveChanges();
                }
                catch (Exception ex)
                {
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTypeDocEdit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SEC_JOB_TYPE_DOC> models)
        {
            var results = new List<SEC_JOB_TYPE_DOC>();
            if (models != null && ModelState.IsValid)
            {
                foreach (SEC_JOB_TYPE_DOC modelItem in models)
                {
                    Db.Entry(modelItem).State = EntityState.Modified;
                    try
                    {
                        Db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTypeDocDestory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SEC_JOB_TYPE_DOC> models)
        {
            if (models.Any())
            {
                foreach (SEC_JOB_TYPE_DOC modelItem in models)
                {
                    SEC_JOB_TYPE_DOC original = Db.SEC_JOB_TYPE_DOC.Where(x => x.JDTY_ID == modelItem.JDTY_ID).FirstOrDefault();
                    Db.SEC_JOB_TYPE_DOC.Remove(original);
                    try
                    {
                        Db.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        ex = ex;
                    }
                    catch (System.Data.Entity.Validation.DbUnexpectedValidationException ex)
                    {
                        ex = ex;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return Json(models.ToDataSourceResult(request, ModelState));
        }

        public ActionResult GetJobTypeDocEntities(decimal activityId)
        {
            return Json(Db.SEC_JOB_ENTITY.Include("SEC_ENTITY").Where(x => x.JDTY_JDTY_ID == activityId).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobFormSetting(decimal activityId)
        {
            using (Asr.Security.AsrJobProvider jp = new Asr.Security.AsrJobProvider(activityId))
            {
                return Json(new
                {
                    Success = true,
                    controllerName = string.IsNullOrEmpty(jp.Job.CNTR_NAME) ? "" : jp.Job.CNTR_NAME,
                    actionName = string.IsNullOrEmpty(jp.Job.ACTN_NAME) ? "" : jp.Job.ACTN_NAME,
                    wHeight = jp.Job.WIND_HIGH.HasValue ? jp.Job.WIND_HIGH : 0,
                    wWidth = jp.Job.WIND_WIDE.HasValue ? jp.Job.WIND_WIDE : 0
                }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult UpdateJobFormSetting(decimal activityId, string actName, string cntrlName, int wndWidth, int wndHeight)
        {
            try
            {
                using (Asr.Security.AsrJobProvider jp = new Asr.Security.AsrJobProvider(activityId))
                {
                    jp.Job.ACTN_NAME = actName;
                    jp.Job.CNTR_NAME = cntrlName;
                    jp.Job.WIND_WIDE = wndWidth;
                    jp.Job.WIND_HIGH = wndHeight;
                    jp.Update();
                    return new ServerMessages(ServerOprationType.Success).ToJson();
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public JsonResult GetMenu(string id, decimal? jdtyId)
        {
            try
            {
                List<int> s = new List<int>();
                s.AddRange(Db.SEC_JOB_MENU.AsEnumerable().Where(x => x.JDTY_JDTY_ID == jdtyId).Select(x => Convert.ToInt32(x.MENU_MENU_ID)).ToList());
                if (string.IsNullOrEmpty(id))
                {
                    var menus = Db.SEC_MENU
                                         .Where(e => e.MENU_MENU_ID == null && e.MENU_CATE != 2)
                                         .OrderBy(x => x.MENU_ORDR)
                                         .AsEnumerable()
                                         .Select(e => new
                                         {
                                             id = e.MENU_ID,
                                             Name = string.Format("{0} ( [{1}] - [{2}] )", e.MENU_TITL, e.MENU_CNTR, e.MENU_ACTN),
                                             hasChildren = e.SEC_MENU1.Count > 0 ? true : false,
                                             parentIdStr = id,
                                             @checked = s.Contains(e.MENU_ID),
                                             controller = e.MENU_CNTR,
                                             action = e.MENU_ACTN
                                         })
                                         .ToList();
                    return Json(menus, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int parentId = Convert.ToInt32(id);
                    var menus = Db.SEC_MENU
                                         .Where(e => e.MENU_MENU_ID == parentId && e.MENU_CATE != 2)
                                         .OrderBy(x => x.MENU_ORDR)
                                         .AsEnumerable()
                                         .Select(e => new
                                         {
                                             id = e.MENU_ID,
                                             Name = string.Format("{0} ( [{1}] - [{2}] )", e.MENU_TITL, e.MENU_CNTR, e.MENU_ACTN),
                                             hasChildren = e.SEC_MENU1.Count > 0 ? true : false,
                                             parentIdStr = id,
                                             @checked = s.Contains(e.MENU_ID),
                                             controller = e.MENU_CNTR,
                                             action = e.MENU_ACTN
                                         })
                                         .ToList();
                    return Json(menus, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(null);
                throw;
            }
        }

        public class MenuItemInfo
        {
            public int Id { get; set; }
            public short Index { get; set; }
        }

        public JsonResult UpdateMenuOrder(List<MenuItemInfo> items)
        {
            try
            {
                foreach (var item in items)
                {
                    var targetMenu = Db.SEC_MENU.Find(item.Id);
                    targetMenu.MENU_ORDR = item.Index;
                }
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success).ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public JsonResult UpdateMobile(PAY_PERSONEL ObjectTemp)
        {
            try
            {
                string sql = string.Format("update pay_personel set mobil='{0}' where EMP_NUMB={1}", ObjectTemp.MOBIL, ObjectTemp.EMP_NUMB);
                Db.Database.ExecuteSqlCommand(sql);
                return new ServerMessages(ServerOprationType.Success).ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }
        public JsonResult UpdateMenuParent(int menuId, short? parentId)
        {
            try
            {
                var targetMenu = Db.SEC_MENU.Find(menuId);
                targetMenu.MENU_MENU_ID = parentId;
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success).ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="menuIdForEdit"></param>
        /// <returns></returns>
        public PartialViewResult GetMenuEditorView(string mode, string menuIdForEdit)
        {
            if (mode == "menu" || mode == "subMenu")
            {
                ViewBag.ActionName = "SaveNewMenuItem";
                var model = new SEC_MENU();
                if (mode == "subMenu")
                    model.MENU_MENU_ID = System.Convert.ToInt32(menuIdForEdit);

                return PartialView("MenuItemEditor", model);
            }
            else if (mode == "update")
            {
                ViewBag.ActionName = "UpdateMenuItem";
                int menuId = Convert.ToInt32(menuIdForEdit);
                var model = Db.SEC_MENU.Find(menuId);
                return PartialView("MenuItemEditor", model);
            }
            else
            {
                return PartialView("MenuItemEditor");
            }
        }

        public ActionResult SaveNewMenuItem(SEC_MENU model)
        {
            try
            {
                short? order = Db.SEC_MENU.Where(x => x.MENU_MENU_ID.HasValue == model.MENU_MENU_ID.HasValue & x.MENU_CATE == model.MENU_CATE).Max(m => m.MENU_ORDR);
                if (order.HasValue)
                    model.MENU_ORDR = 0;
                else
                    model.MENU_ORDR = (short)(order + 1);

                Db.SEC_MENU.Add(model);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = "منو ایجاد شد" }.ToJson();

            }
            catch (Exception)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات" }.ToJson();
            }
        }

        public JsonResult DeleteMenuItem(int menuId)
        {
            try
            {
                var model = Db.SEC_MENU.Find(menuId);
                if (model != null)
                {
                    Db.SEC_MENU.Remove(model);
                    Db.SaveChanges();
                    return new ServerMessages(ServerOprationType.Success) { Message = "منو با موفقیت حذف شد" }.ToJson();
                }
                else
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "ابتدا یک منو را بری حذف انتخاب نمایید" }.ToJson();
            }
            catch (Exception)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "امکان حذف این رکورد وجود ندارد" }.ToJson();
            }
        }

        public ActionResult UpdateMenuItem(SEC_MENU model)
        {
            try
            {
                var modelToEdit = Db.SEC_MENU.Find(model.MENU_ID);
                if (modelToEdit != null)
                {
                    modelToEdit.MENU_TITL = model.MENU_TITL;
                    modelToEdit.MENU_ACTN = model.MENU_ACTN;
                    modelToEdit.MENU_CNTR = model.MENU_CNTR;
                    modelToEdit.MENU_CATE = model.MENU_CATE;
                    Db.SaveChanges();
                    return new ServerMessages(ServerOprationType.Success) { Message = "اطلاعات ویرایش شد." }.ToJson();
                }
                else
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "منو پیدا نشد" }.ToJson();
                }
            }
            catch (Exception)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ویرایش اطلاعات" }.ToJson();
            }
        }

        public ActionResult SaveEntity(string entityName, string entityDesc)
        {
            try
            {
                if (string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(entityDesc))
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "نام و شرح جدول اجباری است" }.ToJson();

                entityName = entityName.ToUpper().Trim();
                if (entityName.Contains(" "))
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "درنام جدول از کارکتر غیر مجاز استفاده شده است" }.ToJson();

                entityName = entityName.ToUpper();
                if (PublicRepository.ExistModel("SEC_ENTITY", "ENTI_TABL_NAME ='{0}'", entityName))
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}] تکراری است", entityName) }.ToJson();

                SEC_ENTITY entity = new SEC_ENTITY()
                {
                    ENTI_TABL_NAME = entityName.ToUpper(),
                    ENTI_DESC = entityDesc
                };
                Db.SEC_ENTITY.Add(entity);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = "جدول ثبت شد" }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public ActionResult SecEntityRead([DataSourceRequest] DataSourceRequest request)
        {
            var query = (from b in Db.SEC_ENTITY
                         select b).AsEnumerable()
                                  .Select(p => new
                                  {
                                      p.ENTI_ID,
                                      p.ENTI_TABL_NAME,
                                      p.ENTI_DESC
                                  });

            return Json(query.ToDataSourceResult(request));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SecEntityEdit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SEC_ENTITY> models)
        {
            var results = new List<SEC_ENTITY>();
            if (models != null && ModelState.IsValid)
            {
                foreach (SEC_ENTITY modelItem in models)
                {
                    Db.Entry(modelItem).State = EntityState.Modified;
                    try
                    {
                        Db.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        ex = ex;
                    }
                    catch (System.Data.Entity.Validation.DbUnexpectedValidationException ex)
                    {
                        ex = ex;
                    }
                    catch (Exception ex)
                    {
                    }
                }

            }
            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SecEntityDestory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SEC_ENTITY> models)
        {
            if (models.Any())
            {
                foreach (SEC_ENTITY modelItem in models)
                {
                    SEC_ENTITY original = Db.SEC_ENTITY.Where(x => x.ENTI_ID == modelItem.ENTI_ID).FirstOrDefault();
                    Db.SEC_ENTITY.Remove(original);
                    try
                    {
                        Db.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        ex = ex;
                    }
                    catch (System.Data.Entity.Validation.DbUnexpectedValidationException ex)
                    {
                        ex = ex;
                    }
                    catch (Exception ex)
                    {
                    }
                }

            }
            return Json(models.ToDataSourceResult(request, ModelState));
        }

        public ActionResult SecJobEntityRead([DataSourceRequest] DataSourceRequest request, decimal? activityId)
        {
            var query = (from b in Db.SEC_JOB_ENTITY
                         where b.JDTY_JDTY_ID == activityId
                         select b).AsEnumerable()
                                  .Select(p => new
                                  {
                                      p.JOEN_ID,
                                      p.JOEN_INSR,
                                      p.JOEN_SELE,
                                      p.JOEN_UPDA,
                                      p.JOEN_DELE,
                                      p.ENTI_ENTI_ID
                                  });
            return Json(query.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SecJobEntityCreate([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SEC_JOB_ENTITY> models, decimal activityId)
        {
            var results = new List<SEC_JOB_ENTITY>();
            foreach (SEC_JOB_ENTITY modelItem in models)
            {
                modelItem.JDTY_JDTY_ID = activityId;
                results.Add(modelItem);
                Db.SEC_JOB_ENTITY.Add(modelItem);
                try
                {
                    Db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    ex = ex;
                }
                catch (System.Data.Entity.Validation.DbUnexpectedValidationException ex)
                {
                    ex = ex;
                }
                catch (Exception ex)
                {
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SecJobEntityEdit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SEC_JOB_ENTITY> models)
        {
            var results = new List<SEC_JOB_ENTITY>();
            if (models != null && ModelState.IsValid)
            {
                foreach (SEC_JOB_ENTITY modelItem in models)
                {
                    Db.Entry(modelItem).State = EntityState.Modified;
                    try
                    {
                        Db.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        ex = ex;
                    }
                    catch (System.Data.Entity.Validation.DbUnexpectedValidationException ex)
                    {
                        ex = ex;
                    }
                    catch (Exception ex)
                    {
                    }
                }

            }
            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SecJobEntityDestory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SEC_JOB_ENTITY> models)
        {
            if (models.Any())
            {
                foreach (SEC_JOB_ENTITY modelItem in models)
                {
                    SEC_JOB_ENTITY original = Db.SEC_JOB_ENTITY.Where(x => x.JOEN_ID == modelItem.JOEN_ID).FirstOrDefault();
                    Db.SEC_JOB_ENTITY.Remove(original);
                    try
                    {
                        Db.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        ex = ex;
                    }
                    catch (System.Data.Entity.Validation.DbUnexpectedValidationException ex)
                    {
                        ex = ex;
                    }
                    catch (Exception ex)
                    {
                    }
                }

            }
            return Json(models.ToDataSourceResult(request, ModelState));
        }

        public ActionResult SecJobEntity(int? id, decimal activityId)
        {
            try
            {
                if (id == null)
                {
                    return View(new SEC_JOB_ENTITY { JDTY_JDTY_ID = activityId });
                }
                else
                {
                    return View(Db.SEC_JOB_ENTITY.Find(id));
                }
            }
            catch (Exception ex)
            {
                HttpContext.AddError(ex);
                return View();
            }
        }

        public ActionResult UpdateSecJobEntity(SEC_JOB_ENTITY model)
        {
            try
            {
                if (!model.JDTY_JDTY_ID.HasValue)
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "هیچ وضیفه ای انتخاب نشده است تا دسترسی این  جدول به آن داده شود!" }.ToJson();
                if (!model.ENTI_ENTI_ID.HasValue)
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "جدول مورد نظر را انتخاب کنید!" }.ToJson();
                if (PublicRepository.ExistModel("SEC_JOB_ENTITY", "JDTY_JDTY_ID = {0} AND ENTI_ENTI_ID={1} AND JOEN_ID <> {2}", model.JDTY_JDTY_ID, model.ENTI_ENTI_ID, model.JOEN_ID))
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "دسترسی مربوط به این جدول برای وظیفه ی مورد نظر قبلا ثبت شده است!" }.ToJson();

                var modelToModify = Db.SEC_JOB_ENTITY.Find(model.JOEN_ID);
                modelToModify.JOEN_DELE = model.JOEN_DELE;
                modelToModify.JOEN_INSR = model.JOEN_INSR;
                modelToModify.JOEN_SELE = model.JOEN_SELE;
                modelToModify.JOEN_UPDA = model.JOEN_UPDA;
                modelToModify.ENTI_ENTI_ID = model.ENTI_ENTI_ID;
                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
            return new ServerMessages(ServerOprationType.Success).ToJson();
        }

        public ActionResult SaveNewSecJobEntity(SEC_JOB_ENTITY model)
        {
            try
            {
                if (!model.JDTY_JDTY_ID.HasValue)
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "هیچ وضیفه ای انتخاب نشده است تا دسترسی این  جدول به آن داده شود!" }.ToJson();
                if (!model.ENTI_ENTI_ID.HasValue)
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "جدول مورد نظر را انتخاب کنید!" }.ToJson();
                if (PublicRepository.ExistModel("SEC_JOB_ENTITY", "JDTY_JDTY_ID = {0} AND ENTI_ENTI_ID={1}", model.JDTY_JDTY_ID, model.ENTI_ENTI_ID))
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "دسترسی مربوط به این جدول برای وظیفه ی مورد نظر قبلا ثبت شده است!" }.ToJson();

                Db.SEC_JOB_ENTITY.Add(model);
                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
            return new ServerMessages(ServerOprationType.Success).ToJson();
        }

        public ActionResult UpdateMenu(string[] menusToAdd, string[] menusToRemove, string jdtyId)
        {
            try
            {
                menusToAdd = menusToAdd ?? new string[0];
                menusToRemove = menusToRemove ?? new string[0];
                using (Asr.Security.AsrJobProvider job = new Asr.Security.AsrJobProvider(Convert.ToInt32(jdtyId)))
                {
                    foreach (string item in menusToAdd)
                    {
                        job.AssignMenu(Convert.ToInt32(item));
                    }
                    foreach (string item in menusToRemove)
                    {
                        job.RemoveMenu(Convert.ToInt32(item));
                    }
                    return new ServerMessages(ServerOprationType.Success) { Message = "منو ها با موفقیت ویرایش شد" }.ToJson();
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public ActionResult SaveNewGroup(string groupName, string categuryId)
        {
            if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(categuryId))
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "نام گروه و دسته بندی اجباری است" }.ToJson();
            if (PublicRepository.ExistModel("SEC_POST_APP", "SPAP_DESC = '{0}' AND SPAP_CATE = '{1}'", groupName.ToArabicUtf8(), categuryId))
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "گروهی با این نام قبلا ثبت شده است." }.ToJson();

            try
            {
                Asr.Security.AsrGroupProvider.Create(groupName, categuryId);
                return new ServerMessages(ServerOprationType.Success) { Message = "گروه ثبت شد" }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }

        }

        public JsonResult GetGroups(string categuryId)
        {
            try
            {
                return Json(new
                {
                    Success = true,
                    data = string.IsNullOrEmpty(categuryId) ? null : Asr.Security.AsrGroupProvider.GetList.Where(x => x.SPAP_CATE == categuryId).Select(x => new Asr.Security.SEC_POST_APP
                    {
                        ID = x.ID,
                        SPAP_CATE = x.SPAP_CATE,
                        SPAP_DESC = x.SPAP_DESC
                    }).ToList()
                },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public JsonResult DeleteGroup(decimal groupId)
        {
            try
            {
                Asr.Security.AsrGroupProvider.Delete(groupId);
                return new ServerMessages(ServerOprationType.Success) { Message = "رکورد با موفقیت حذف شد" }.ToJson();
            }
            catch (Exception)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "امکان حذف گروه ها یی که دارای کاربر یا وظیفه هستند وجود ندارد." }.ToJson();
            }
        }


        public ActionResult GetGroupUsers([DataSourceRequest] DataSourceRequest request, decimal groupId)
        {
            using (Asr.Security.AsrGroupProvider gp = new Asr.Security.AsrGroupProvider(groupId))
            {
                var query = (from b in gp.Users
                             select b)
                                      .AsEnumerable()
                                      .Select(p => new
                                      {
                                          p.ROW_NO,
                                          p.FIRS_NAME,
                                          p.FAML_NAME,
                                          p.USER_NAME,
                                          p.ORCL_NAME
                                      });

                return Json(query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ChangeUserState(int userId, bool state)
        {
            using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider(userId))
            {
                mp.ChangeState(state ? Asr.Security.AsrMembershipProvider.AsrUserState.Active : Asr.Security.AsrMembershipProvider.AsrUserState.Deactive);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddUserToGroup(decimal groupId, int userId)
        {
            try
            {
                using (Asr.Security.AsrGroupProvider gp = new Asr.Security.AsrGroupProvider(groupId))
                {
                    gp.AddMember(userId);
                    return new ServerMessages(ServerOprationType.Success) { Message = "کاربر به گروه اضافه شد" }.ToJson();
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public JsonResult RemoveUserFromGroup(decimal groupId, int userId)
        {
            using (Asr.Security.AsrGroupProvider gp = new Asr.Security.AsrGroupProvider(groupId))
            {
                gp.RemoveMember(userId);
            }
            return new ServerMessages(ServerOprationType.Success) { Message = "کاربر از گروه حذف شد" }.ToJson();
        }

        public ActionResult ReadAllUser([DataSourceRequest] DataSourceRequest request, string filterText)
        {
            bool doFilter = !string.IsNullOrEmpty(filterText.Trim());
            if (doFilter)
                filterText = filterText.ToUpper().ToArabicUtf8();

            using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider())
            {
                var query = (from b in mp.AllUsers
                             where (!doFilter) ||
                                   (b.FIRS_NAME.ToUpper().Contains(filterText) ||
                                    b.FAML_NAME.ToUpper().Contains(filterText) ||
                                    b.ORCL_NAME.ToUpper().Contains(filterText) ||
                                    b.USER_NAME.ToUpper().Contains(filterText))
                             select b)
                                      .Select(p => new
                                      {
                                          p.ROW_NO,
                                          p.FIRS_NAME,
                                          p.FAML_NAME,
                                          p.USER_NAME,
                                          p.ORCL_NAME
                                      });
                if (doFilter)
                    request.Page = 1;
                return Json(query.ToDataSourceResult(request));
            }
        }

        public ActionResult ReadAllPersonel([DataSourceRequest] DataSourceRequest request, string filterText)
        {
            bool dontFilter = string.IsNullOrEmpty(filterText.Trim());
            if (!dontFilter)
                filterText = filterText.ToUpper().ToArabicUtf8();

            var query = (from b in Db.PAY_PERSONEL
                         where (dontFilter) ||
                               (b.FIRS_NAME.ToUpper().Contains(filterText) ||
                                b.FAML_NAME.ToUpper().Contains(filterText) ||
                                b.FATH_NAME.ToUpper().Contains(filterText))
                         select b)
                                  .AsEnumerable()
                                  .Select(p => new
                                  {
                                      p.EMP_NUMB,
                                      p.FIRS_NAME,
                                      p.FAML_NAME,
                                      p.FATH_NAME
                                  });
            if (!dontFilter)
                request.Page = 1;
            return Json(query.ToDataSourceResult(request));
        }

        public JsonResult GetGroupsActivitiesCateguries(decimal groupId)
        {
            try
            {
                using (Asr.Security.AsrGroupProvider gp = new Asr.Security.AsrGroupProvider(groupId))
                {
                    List<int> typeDocKeys = gp.Jobs.Select(x => (int)x.ETDO_ETDO_ID).ToList();
                    var dataList = Db.EXP_TYPE_DOC
                                            .Where(x => typeDocKeys.Contains(x.ETDO_ID))
                                            .AsEnumerable()
                                            .Select(x => new
                                            {
                                                code = x.ETDO_ID,
                                                desc = x.ETDO_DESC
                                            })
                                            .ToList();
                    return Json(
                        new
                        {
                            Success = true,
                            data = dataList
                        },
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public JsonResult GetGroupActivities(decimal groupId, int etdoId)
        {
            try
            {
                using (Asr.Security.AsrGroupProvider gp = new Asr.Security.AsrGroupProvider(groupId))
                {
                    return Json(
                        new
                        {
                            Success = true,
                            data = gp.Jobs.Where(x => x.ETDO_ETDO_ID == etdoId).Select(x => new { x.JDTY_ID, x.ACTIV_FNAM, x.ACTIV_NAME }).ToList()
                        },
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public JsonResult DeleteGroupActivity(decimal groupId, decimal activityId)
        {
            try
            {
                using (Asr.Security.AsrGroupProvider gp = new Asr.Security.AsrGroupProvider(groupId))
                {
                    gp.RemoveJob(activityId);
                }
                return new ServerMessages(ServerOprationType.Success) { Message = "وظیفه از گروه حذف شد" }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public JsonResult AssignActivityToGroup(decimal groupId, decimal activityId)
        {
            try
            {
                using (Asr.Security.AsrGroupProvider gp = new Asr.Security.AsrGroupProvider(groupId))
                {
                    gp.AssignJob(activityId);
                }
                return new ServerMessages(ServerOprationType.Success) { Message = "وظیفه به گروه اضافه شد" }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public ActionResult GetActivitiesUsers([DataSourceRequest] DataSourceRequest request, decimal activityId)
        {
            using (Asr.Security.AsrJobProvider gp = new Asr.Security.AsrJobProvider(activityId))
            {

                var query = (from b in gp.PrivateUsers
                             select b)
                                      .AsEnumerable()
                                      .Select(p => new
                                      {
                                          p.ROW_NO,
                                          p.FIRS_NAME,
                                          p.FAML_NAME,
                                          p.USER_NAME,
                                          p.ORCL_NAME
                                      });
                return Json(query.ToDataSourceResult(request));
            }
        }

        public JsonResult AddUserToActivity(decimal activityId, int userId)
        {
            try
            {
                using (Asr.Security.AsrJobProvider gp = new Asr.Security.AsrJobProvider(activityId))
                {
                    gp.AddMember(userId);
                }
                return new ServerMessages(ServerOprationType.Success) { Message = "کاربر به وظیفه اضافه شد" }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public JsonResult RemoveUserFromActivity(decimal activityId, int userId)
        {
            using (Asr.Security.AsrJobProvider gp = new Asr.Security.AsrJobProvider(activityId))
            {
                gp.RemoveMember(userId);
            }
            return new ServerMessages(ServerOprationType.Success) { Message = "کاربر از وظیفه حذف شد" }.ToJson();
        }

        public JsonResult GetMethodsNameOfClass(string cntrlName)
        {
            try
            {
                return Json(Codes.AssemblyHelper.GetMethodNamesOfClass(cntrlName.Trim()).Select(x => new { Text = x, Value = x }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CreateDocumentType(string documentName)
        {

            Db.EXP_TYPE_DOC.Add(new EXP_TYPE_DOC
            {
                ETDO_DESC = documentName,
                FLOW_TYPE = "NoBPM"
            });
            Db.SaveChanges();

            return new ServerMessages(ServerOprationType.Success).ToJson();
        }

        public JsonResult CreateNewActivity(string activityName, string activityDesc, int etdoId)
        {
            Asr.Security.AsrJobProvider.CreateNew(activityName, activityDesc, 2, etdoId);
            return new ServerMessages(ServerOprationType.Success).ToJson();
        }

        public JsonResult UpdateActivity(string activityName, string activityDesc, decimal jdtyId)
        {
            using (Asr.Security.AsrJobProvider jp = new Asr.Security.AsrJobProvider(jdtyId))
            {
                jp.Job.ACTIV_NAME = activityName;
                jp.Job.ACTIV_FNAM = activityDesc;
                jp.Update();
            }
            return new ServerMessages(ServerOprationType.Success).ToJson();
        }

        public JsonResult RemoveActivity(decimal jdtyId)
        {
            Asr.Security.AsrJobProvider.Remove(jdtyId);
            return new ServerMessages(ServerOprationType.Success).ToJson();
        }

        public ActionResult CreateNewUser(Asr.Security.SEC_USERS newItem, decimal[] defaultgroups)
        {
            if (newItem == null)
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات را به شکل کامل پر کنید" }.ToJson();
            if (string.IsNullOrEmpty(newItem.USER_NAME))
                return new ServerMessages(ServerOprationType.Success) { ExceptionMessage = "نام کاربری را وارد کنید" }.ToJson();

            newItem.ORCL_NAME = string.Format("ORA{0}", newItem.USER_NAME.ToUpper().Trim());
            newItem.ORCL_PASS = System.Web.Security.Membership.GeneratePassword(5, 1);
            try
            {
                using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider())
                {
                    if (mp.AllUsers.Any(x => x.USER_NAME.ToUpper().Trim() == newItem.USER_NAME.ToUpper().Trim()))
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "نام کاربری که وارد کرده اید قبلا ثبت شده است!" }.ToJson();

                    if (mp.AllUsers.Any(x => x.ORCL_NAME.ToUpper().Trim() == newItem.ORCL_NAME.ToUpper().Trim()))
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "نام کاربری که وارد کرده اید قبلا ثبت شده است!" }.ToJson();

                    if ((newItem.PRSN_EMP_NUMB.HasValue) && (newItem.PRSN_EMP_NUMB > 0))
                        if (mp.AllUsers.Any(x => x.PRSN_EMP_NUMB == newItem.PRSN_EMP_NUMB))
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "برای پرسنل مورد نظر قبلا یک حساب کاربری ایجاد شده است" }.ToJson();

                    if ((newItem.OUTP_OUTP_ID.HasValue) && (newItem.OUTP_OUTP_ID > 0))
                        if (mp.AllUsers.Any(x => x.OUTP_OUTP_ID == newItem.OUTP_OUTP_ID))
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "برای پرسنل بیرون سازمانی مورد نظر قبلا یک حساب کاربری ایجاد شده است" }.ToJson();

                    string result;
                    var userProvider = Asr.Security.AsrMembershipProvider.CreateUser(newItem, out result);
                    if (string.IsNullOrEmpty(result))
                    {
                        userProvider.Grant("CONNECT,MIS_PUBLIC,PRN,EXP");
                        if (defaultgroups != null)
                        {
                            foreach (decimal i in defaultgroups)
                            {
                                using (Asr.Security.AsrGroupProvider gp = new Asr.Security.AsrGroupProvider(i))
                                {
                                    gp.AddMember(userProvider.User.ROW_NO);
                                }
                            }
                        }
                        return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}]  کاربر ایجاد شد", newItem.USER_NAME) }.ToJson();
                    }
                    else
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = result }.ToJson();
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }
        }

        [HttpGet]
        public ActionResult GetAllGroups()
        {
            return Json(Db.SEC_POST_APP.AsEnumerable().Select(x => new { Text = x.SPAP_DESC, Value = x.ID }).OrderBy(x => x.Text), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserGrant(int userId, string operationType, string grant)
        {
            if (userId <= 0)
                return new ServerMessages(ServerOprationType.Failure).ToJson();
            if (string.IsNullOrEmpty(operationType))
                return new ServerMessages(ServerOprationType.Failure).ToJson();
            if (string.IsNullOrEmpty(grant))
                return new ServerMessages(ServerOprationType.Failure).ToJson();

            using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider(userId))
            {
                switch (operationType.ToLower())
                {
                    case "assign":
                        mp.Grant(grant);
                        return new ServerMessages(ServerOprationType.Success) { Message = "مجوز به کاربر تخصیص داده شد" }.ToJson();
                    case "drop":
                        mp.Revoke(grant);
                        return new ServerMessages(ServerOprationType.Success) { Message = "مجوز حذف شد" }.ToJson();
                    default:
                        return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در انجام عملیات" }.ToJson();
                }
            }
        }

        public JsonResult ChangeUserPassword(int userId, string currentPassword, string newPassword, string repeatPassword, string passType)
        {
            bool retval = false;
            using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider(userId))
            {
                switch (passType)
                {
                    case "pass":
                        retval = mp.ChangePassword("", newPassword, "");
                        break;
                    case "orclpass":
                        retval = mp.ChangeDbPassword(newPassword);
                        break;
                    default:
                        break;
                }

            }
            if (retval)
                return new ServerMessages(ServerOprationType.Success) { Message = (passType == "pass") ? "کلمه عبور کاربر تغییر کرد" : "کلمه عبور اراکلی تغییر کرد" }.ToJson();
            else
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "کلمه عبور کاربر تغییر نکرد" }.ToJson();
        }

        public ActionResult DisplayError(int id)
        {
            using (Asr.Security.SecurityEntities secDb = new Asr.Security.SecurityEntities(true))
            {
                Asr.Security.LOG_ERROR err = secDb.LOG_ERROR.Find(id);
                return View(err);
            }
        }

        public JsonResult GetUserJobsDocuments(int userId)
        {
            using (Asr.Security.AsrMembershipProvider user = new Asr.Security.AsrMembershipProvider(userId))
            {
                return Json(new
                {
                    Success = true,
                    data = user.JobsDocuments.OrderBy(o => o.ETDO_DESC).Select(x => new { code = x.ETDO_ID, desc = x.ETDO_DESC }).ToList()
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetUserActivities(int userId, int etdoId)
        {
            try
            {
                using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider(userId))
                {
                    return Json(
                        new
                        {
                            Success = true,
                            data = mp.Jobs.OrderBy(o => o.ACTIV_FNAM).Where(x => x.ETDO_ETDO_ID == etdoId).Select(x => new { x.JDTY_ID, x.ACTIV_FNAM, x.ACTIV_NAME }).ToList()
                        },
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public JsonResult GetUserJoinedGroups(int userId)
        {
            try
            {
                using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider(userId))
                {
                    return Json(
                        new
                        {
                            Success = true,
                            data = mp.JoinedGroups.OrderBy(o => o.SPAP_DESC).Select(x => new { code = x.ID, desc = x.SPAP_DESC }).ToList()
                        },
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public JsonResult UserTablesRead([DataSourceRequest] DataSourceRequest request, int userId)
        {
            using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider(userId))
            {
                return Json(mp.UserTables.OrderBy(x => x.EntityTableName).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult GetUserMenu(int userId)
        {
            ViewData["userId"] = userId;
            ViewData["MasterMenu"] = "no";
            return PartialView("_MenuPartial");
        }

        public JsonResult GetActivityGroups(decimal activityId)
        {
            using (Asr.Security.AsrJobProvider jp = new Asr.Security.AsrJobProvider(activityId))
            {
                return Json(
                    new
                    {
                        Success = true,
                        data = jp.Groups.OrderBy(x => x.SPAP_DESC).ToList()
                    }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetUserInfo(int userId)
        {
            using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider(userId))
            {
                Asr.Security.AsrLoginInfo lgnInfo = new Asr.Security.AsrLoginInfo();
                string organeFullName = string.Empty,
                computerName = string.Empty,
                operationSystem = string.Empty;
                try
                {
                    lgnInfo = mp.GetLastLoginInfo("0");
                    organeFullName = mp.GetFullOrganName();
                    string[] clientArryInfo = lgnInfo.OperationSystem.Split('/');
                    if (clientArryInfo.Length >= 2)
                    {
                        operationSystem = clientArryInfo[0];
                        computerName = clientArryInfo[1];
                    }
                    else if (clientArryInfo.Length == 1)
                    {
                        operationSystem = clientArryInfo[0];
                    }
                }
                catch (Exception ex)
                {
                    ex = ex;
                }
                return Json(new
                {
                    Success = true,
                    userInfo = mp.AllUsers.FirstOrDefault(x => x.ROW_NO == mp.User.ROW_NO),
                    loginInfo = lgnInfo,
                    organeFullName = organeFullName,
                    operationSystem = operationSystem,
                    computerName = computerName
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PartialViewResult GetUserAccessRptReportViewer(int userId)
        {
            var path = Server.MapPath("~/Content/Reports/Localization/fa.xml");
            StiOptions.Localization.Load(path);
            var viewerOptions = new StiMvcViewerOptions()
            {
                Server = new StiMvcViewerOptions.ServerOptions
                {
                    Controller = "Sec",
                    UseRelativeUrls = true,
                },
                Actions = new StiMvcViewerOptions.ActionOptions
                {
                    GetReport = string.Format("GetUserAccessReportSnapshot/{0}", userId),
                    ViewerEvent = "ViewerEvent",
                    PrintReport = "PrintReport",
                    ExportReport = "ViewerExportReport",
                    Interaction = "Interaction"
                },
                Appearance = new StiMvcViewerOptions.AppearanceOptions { ScrollbarsMode = false },
                Theme = StiViewerTheme.SimpleGray
            };
            return PartialView("ReportViewer", viewerOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetUserAccessReportSnapshot(int id)
        {
            StiReport userAccessReport = new StiReport();
            userAccessReport.Load(Server.MapPath("~/Content/Reports/Security/UserAcceses.mrt"));
            userAccessReport.Dictionary.Databases.Clear();
            userAccessReport.Dictionary.Databases.Add(new StiOleDbDatabase("DefultConnection", GlobalConst.ConnectionStringOleDb));
            userAccessReport.Compile();
            userAccessReport["UserId"] = id;
            userAccessReport["ReportCreator"] = this.UserInfo().GetFullName();
            userAccessReport["CompanyName"] = GlobalConst.OfficeName;
            return StiMvcViewer.GetReportSnapshotResult(HttpContext, userAccessReport);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public PartialViewResult GetGroupAccessRptReportViewer(string groupId)
        {
            var path = Server.MapPath("~/Content/Reports/Localization/fa.xml");
            StiOptions.Localization.Load(path);
            var height = new Unit(1000);
            var width = new Unit(850);
            var viewerOptions = new StiMvcViewerOptions()
            {
                Server = new StiMvcViewerOptions.ServerOptions
                {
                    Controller = "Sec",
                    UseRelativeUrls = true,
                },
                Actions = new StiMvcViewerOptions.ActionOptions
                {
                    GetReport = string.Format("GetGroupAccessReportSnapshot/{0}", groupId),
                    ViewerEvent = "ViewerEvent",
                    PrintReport = "PrintReport",
                    ExportReport = "ViewerExportReport",
                    Interaction = "Interaction"
                },
                Appearance = new StiMvcViewerOptions.AppearanceOptions { ScrollbarsMode = false },
                Theme = StiViewerTheme.SimpleGray,
                Height = height,
                Width = width
            };
            return PartialView("ReportViewer", viewerOptions);
        }

        /// <summary>
        /// modify user first or last name
        /// </summary>
        /// <param name="userId">sec users row_no</param>
        /// <param name="key">property name for ezample(firstName)</param>
        /// <param name="value">property value for example(Amirhossein)</param>
        /// <returns></returns>
        public ActionResult UpdateUser(int userId, string key, string value)
        {
            if (userId == 0)
                return new ServerMessages(ServerOprationType.Failure).ToJson();
            if (string.IsNullOrEmpty(key))
                return new ServerMessages(ServerOprationType.Failure).ToJson();
            if (string.IsNullOrEmpty(value))
                return new ServerMessages(ServerOprationType.Failure).ToJson();

            var user = Db.SEC_USERS.Find(userId);
            if (user != null)
            {
                switch (key)
                {
                    case "firstName":
                        user.FIRS_NAME = value;
                        if (user.PRSN_EMP_NUMB.HasValue || user.PRSN_EMP_NUMB > 0)
                        {
                            PAY_PERSONEL person = Db.PAY_PERSONEL.FirstOrDefault(x => x.EMP_NUMB == user.PRSN_EMP_NUMB);
                            if (person != null)
                            {
                                person.FIRS_NAME = value;
                            }
                        }
                        Db.SaveChanges();
                        return new ServerMessages(ServerOprationType.Success).ToJson();
                    case "lastName":
                        user.FAML_NAME = value;
                        if (user.PRSN_EMP_NUMB.HasValue || user.PRSN_EMP_NUMB > 0)
                        {
                            PAY_PERSONEL person = Db.PAY_PERSONEL.FirstOrDefault(x => x.EMP_NUMB == user.PRSN_EMP_NUMB);
                            if (person != null)
                            {
                                person.FAML_NAME = value;
                            }
                        }
                        Db.SaveChanges();
                        return new ServerMessages(ServerOprationType.Success).ToJson();
                    default:
                        return new ServerMessages(ServerOprationType.Failure).ToJson();
                }
            }
            else
                return new ServerMessages(ServerOprationType.Failure).ToJson();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetGroupAccessReportSnapshot(string id)
        {
            StiReport userAccessReport = new StiReport();
            userAccessReport.Load(Server.MapPath("~/Content/Reports/Security/GroupAccess.mrt"));
            userAccessReport.Dictionary.Databases.Clear();
            userAccessReport.Dictionary.Databases.Add(new StiOleDbDatabase("DefultConnection", GlobalConst.ConnectionStringOleDb));
            userAccessReport.Compile();
            userAccessReport["GroupId"] = id;
            userAccessReport["ReportCreator"] = this.UserInfo().GetFullName();
            userAccessReport["CompanyName"] = GlobalConst.OfficeName;
            return StiMvcViewer.GetReportSnapshotResult(HttpContext, userAccessReport);
        }

        /// <summary>
        /// List Of Subsystems
        /// </summary>
        /// <returns>Subsystems</returns>
        public ActionResult GetSubsystems()
        {
            return Json(Db.SEC_SUBSYSTEM.Select(x => new
            {
                x.ID,
                x.SUSY_DESC,
                x.SUSY_ACTN,
                x.SUSY_CTLR
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetTrhSetting()
        {
            return PartialView();
        }


        public PartialViewResult GetExpSetting()
        {
            //dbContext.EXP_PERSON_EXPLI
            return PartialView();
        }


        public ActionResult GetExpPersonels([DataSourceRequest] DataSourceRequest request)
        {
            var query = Db.EXP_PERSON_EXPLI
                                 .AsEnumerable()
                                 .Select(p => new
                                 {
                                     p.EPEX_ID,
                                     p.EPEX_NAME,
                                     p.ACTV_TYPE,
                                     p.PRSN_EMP_NUMB
                                 });
            return Json(query.ToDataSourceResult(request));
        }


        public JsonResult CreateExpPersonel(string persnonelId, string outPersonId, bool inPerson)
        {
            if (inPerson)
            {
                if (string.IsNullOrEmpty(persnonelId))
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "لطفا کاربر درون سازمانی مورد نظر را انتخاب نمایید و دوباره یعی کنید" }.ToJson();

                short empNumb = Convert.ToInt16(persnonelId);
                var emp = Db.PAY_PERSONEL.FirstOrDefault(x => x.EMP_NUMB == empNumb);
                if (emp == null)
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "لطفا کاربر درون سازمانی مورد نظر را انتخاب نمایید و دوباره یعی کنید" }.ToJson();

                //EXP_PERSON_EXPLI                
                string personName = string.Format("{0} {1}", emp.FIRS_NAME, emp.FAML_NAME);
                Db.EXP_PERSON_EXPLI.Add(new EXP_PERSON_EXPLI
                {
                    PRSN_EMP_NUMB = empNumb,
                    EPEX_NAME = personName
                });
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = "کاربر ثبت شد" }.ToJson();
            }
            else
            {
                if (string.IsNullOrEmpty(outPersonId))
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "لطفا کاربر بیرون از سازمان مورد نظر را انتخاب نمایید و دوباره یعی کنید" }.ToJson();

                int outPersId = Convert.ToInt32(outPersonId);
                var emp = Db.EXP_OUT_PERSONEL.FirstOrDefault(x => x.OUTP_ID == outPersId);
                if (emp == null)
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "لطفا کاربر بیرون از سازمان مورد نظر را انتخاب نمایید و دوباره یعی کنید" }.ToJson();

                //EXP_PERSON_EXPLI
                string personName = string.Format("{0} {1}", emp.OUTP_FNAME, emp.OUTP_LNAME);
                Db.EXP_PERSON_EXPLI.Add(new EXP_PERSON_EXPLI
                {
                    OUTP_OUTP_ID = outPersId,
                    EPEX_NAME = personName
                });
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = "کاربر ثبت شد" }.ToJson();
            }
        }

        /// <summary>
        /// return history of user login
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns>list of [Asr.Security.LOG_LOGIN]</returns>
        public ActionResult GetUserLoginHistory(int userId)
        {
            using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider(userId))
            {
                var data = mp.LoginHistory
                            .Where(x => x.LOGN_CATE == GlobalConst.CurrentSubSystemId)
                            .OrderByDescending(x => x.LOGN_ID)
                            .Take(100)
                            .ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SecFormsRead([DataSourceRequest] DataSourceRequest request, string filterText)
        {
            using (Asr.Security.SecurityEntities secContext = new Asr.Security.SecurityEntities(true))
            {
                bool doNotFilter = string.IsNullOrEmpty(filterText);
                if (!doNotFilter)
                    filterText = filterText.ToUpper().ToArabicUtf8();
                var query = (from b in secContext.SEC_FORMS.Where(x => doNotFilter || (x.FORS_NAME.Contains(filterText) || x.FORS_HELP.Contains(filterText)))
                             select b)
                                      .AsEnumerable()
                                      .Select(p => new
                                      {
                                          p.FORS_ID,
                                          p.FORS_NAME,
                                          p.FORS_HELP,
                                          p.FORS_TYPE
                                      });
                return Json(query.ToDataSourceResult(request));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SecFormsEdit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<Asr.Security.SEC_FORMS> models)
        {
            var results = new List<Asr.Security.SEC_FORMS>();
            if (models != null && ModelState.IsValid)
            {
                using (Asr.Security.SecurityEntities secContext = new Asr.Security.SecurityEntities(true))
                {
                    foreach (Asr.Security.SEC_FORMS modelItem in models)
                    {
                        secContext.Entry(modelItem).State = EntityState.Modified;
                        try
                        {
                            secContext.SaveChanges();
                        }
                        catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                        {
                            ex = ex;
                        }
                        catch (System.Data.Entity.Validation.DbUnexpectedValidationException ex)
                        {
                            ex = ex;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState));
        }

        public ActionResult SecFormsDestory(int permissionId)
        {
            using (Asr.Security.SecurityEntities secContext = new Asr.Security.SecurityEntities(true))
            {
                Asr.Security.SEC_FORMS original = secContext.SEC_FORMS.FirstOrDefault(x => x.FORS_ID == permissionId);
                secContext.SEC_FORMS.Remove(original);
                try
                {
                    secContext.SaveChanges();
                    return new ServerMessages(ServerOprationType.Success) { Message = "مجوز حذف شد" }.ToJson();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();
                }
                catch (System.Data.Entity.Validation.DbUnexpectedValidationException ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();
                }
                catch (Exception ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();
                }
            }

        }

        public JsonResult AddEditPermission(string permissionName, string permissionDescription, string permissionFormActionMode, int permissionId)
        {
            if (string.IsNullOrEmpty(permissionName))
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "نام مجوز را وارد کنید." }.ToJson();
            if (string.IsNullOrEmpty(permissionFormActionMode))
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا!!" }.ToJson();

            using (Asr.Security.SecurityEntities secContext = new Asr.Security.SecurityEntities(true))
            {
                permissionName = permissionName.Trim().ToUpper();
                switch (permissionFormActionMode)
                {
                    default:
                    case "add":
                        if (secContext.SEC_FORMS.Any(x => x.FORS_NAME.ToUpper().Trim() == permissionName))
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مجوزی با این نام قبلا ثبت شده است" }.ToJson();
                        Asr.Security.SEC_FORMS instance = new Asr.Security.SEC_FORMS
                        {
                            FORS_NAME = permissionName,
                            FORS_HELP = permissionDescription,
                            FORS_TYPE = Guid.NewGuid().ToString()
                        };
                        secContext.SEC_FORMS.Add(instance);
                        secContext.SaveChanges();
                        return new ServerMessages(ServerOprationType.Success) { Message = "مجوز جدید ثبت شد", CoustomData = instance.FORS_TYPE }.ToJson();
                    case "edit":
                        if (secContext.SEC_FORMS.Any(x => (x.FORS_NAME.ToUpper().Trim() == permissionName) && (x.FORS_ID != permissionId)))
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "مجوزی با این نام قبلا ثبت شده است" }.ToJson();
                        Asr.Security.SEC_FORMS modelToEdit = secContext.SEC_FORMS.Find(permissionId);
                        if (modelToEdit != null)
                        {
                            modelToEdit.FORS_NAME = permissionName;
                            modelToEdit.FORS_HELP = permissionDescription;
                            secContext.SaveChanges();
                            return new ServerMessages(ServerOprationType.Success) { Message = "مجوز ویرایش شد" }.ToJson();
                        }
                        else
                        {
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ویرایش اطلاعات" }.ToJson();
                        }
                }
            }
        }

        public JsonResult GranttPermissionGroup(decimal groupId, int permissionId)
        {
            try
            {
                using (Asr.Security.AsrGroupProvider gp = new Asr.Security.AsrGroupProvider(groupId))
                {
                    gp.Grantt(permissionId);
                    return new ServerMessages(ServerOprationType.Success).ToJson();
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public JsonResult RevokePermissionGroup(decimal groupId, int permissionId)
        {
            try
            {
                using (Asr.Security.AsrGroupProvider gp = new Asr.Security.AsrGroupProvider(groupId))
                {
                    gp.Revoke(permissionId);
                    return new ServerMessages(ServerOprationType.Success).ToJson();
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();

            }
        }

        public ActionResult GroupPermissionsGridRead([DataSourceRequest] DataSourceRequest request, string filterText, decimal groupId)
        {
            using (Asr.Security.AsrGroupProvider secContext = new Asr.Security.AsrGroupProvider(groupId))
            {
                bool doNotFilter = string.IsNullOrEmpty(filterText);
                if (!doNotFilter)
                    filterText = filterText.ToUpper().ToArabicUtf8();
                var query = (from b in secContext.Permissions.Where(x => doNotFilter || (x.FORS_NAME.Contains(filterText) || x.FORS_HELP.Contains(filterText)))
                             select b)
                             .Select(p => new
                             {
                                 p.FORS_ID,
                                 p.FORS_NAME,
                                 p.FORS_HELP,
                                 p.FORS_TYPE
                             });
                return Json(query.ToDataSourceResult(request));
            }
        }

        public ActionResult UserPermissionsGridRead([DataSourceRequest] DataSourceRequest request, string filterText, int userId)
        {
            using (Asr.Security.AsrMembershipProvider secContext = new Asr.Security.AsrMembershipProvider(userId))
            {
                bool doNotFilter = string.IsNullOrEmpty(filterText);
                if (!doNotFilter)
                    filterText = filterText.ToUpper().ToArabicUtf8();
                var query = (from b in secContext.Permissions.Where(x => doNotFilter || (x.FORS_NAME.Contains(filterText) || x.FORS_HELP.Contains(filterText)))
                             select b)
                             .Select(p => new
                             {
                                 p.FORS_ID,
                                 p.FORS_NAME,
                                 p.FORS_HELP,
                                 p.FORS_TYPE,
                                 p.FORS_VALUE
                             });
                return Json(query.ToDataSourceResult(request));
            }
        }

        public JsonResult GranttPermissionUser(int userId, int permissionId)
        {
            try
            {
                using (Asr.Security.AsrMembershipProvider gp = new Asr.Security.AsrMembershipProvider(userId))
                {
                    gp.GranttSystemPermission(permissionId);
                    return new ServerMessages(ServerOprationType.Success).ToJson();
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        public JsonResult RevokePermissionUser(int userId, int permissionId, string permissionRoute)
        {
            if (permissionRoute.Contains("FROM_GROUP"))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "این مجوز از طریق عضویت در گروه *" + permissionRoute.Replace("FROM_GROUP -", "") + " * به کاربر تعلق گرفته است و امکان حذف وجود ندارد!" }.ToJson();
            }
            try
            {
                using (Asr.Security.AsrMembershipProvider gp = new Asr.Security.AsrMembershipProvider(userId))
                {
                    gp.RevokeSystemPermission(permissionId);
                    return new ServerMessages(ServerOprationType.Success).ToJson();
                }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ex).ToJson();
            }
        }

        #region stimul report actions

        public ActionResult ViewerEvent()
        {

            return StiMvcViewer.ViewerEventResult(this.HttpContext);
        }

        public ActionResult PrintReport()
        {
            return StiMvcViewer.PrintReportResult(this.HttpContext);
        }

        public ActionResult ViewerExportReport()
        {
            return StiMvcViewerFx.ExportReportResult(this.Request);
        }

        public ActionResult Interaction()
        {
            return StiMvcViewer.InteractionResult(this.HttpContext);
        }

        #endregion

    }

}
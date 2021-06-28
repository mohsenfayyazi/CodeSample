using Equipment.Codes.Security;
using Equipment.Models;
using Equipment.ViewModels.PostMap;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Equipment.Controllers.PostMap
{
    public class PostMapController : DbController
    {
        const string MapEntity = "EXP_POST_LINE";
        const string MapKey = "EPOL_ID";
        const string MapValue = "0";


        // GET: ProvincialMap
        public ActionResult Index()
        {
            PostMapViewModel model = new PostMapViewModel()
            {
                Entity = MapEntity,
                Key = MapKey,
                Value = MapValue,
                MapAttachment = Db.SCN_ATTACHINTER.Where(b => b.ENTI_KEY == MapKey && b.ENTI_NAME == MapEntity).ToList()
            };

            using (Asr.Security.AsrMembershipProvider mp = new Asr.Security.AsrMembershipProvider(User.Identity.Name))
            {
                model.UploadMapBtn = mp.CheckPermission("PUB_POSTMAP_BTN_UPLOAD");
                model.DeleteMapGridBtn = mp.CheckPermission("PUB_POSTMAP_GRIDBTN_DELETE");
            }

            return View(model);
        }

        public ActionResult PartialUpload(string value)
        {
            return PartialView("_PartialUpload", value);
        }

        public ActionResult GetPostDropDown()
        {
            var query = (from b in Db.EXP_POST_LINE
                         where b.EPOL_TYPE == "0" && b.EPOL_STAT != "3"
                         orderby b.EUNL_EUNL_ID, b.EPOL_NAME
                         select new
                         {
                             b.EPOL_ID,
                             b.EPOL_NAME
                         }).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaps([DataSourceRequest] DataSourceRequest request, string entity, string value, string key)
        {
            if (string.IsNullOrEmpty(entity) || string.IsNullOrEmpty(value) || string.IsNullOrEmpty(key))
                return Json(null);

            var query = from b in Db.SCN_ATTACHINTER
                        where (b.ENTI_KEY == key && b.ENTI_VALU == value && b.ENTI_NAME == entity)
                        select new
                        {
                            b.ID,
                            b.TITLE,
                            b.ATCH_ID
                        };

            return Json(query.ToDataSourceResult(request));
        }

        [EntityAuthorize("SCN_ATTACHE > select,insert,update|SCN_ATTACHINTER > select,insert,update")]
        public ActionResult SaveMap(IEnumerable<HttpPostedFileBase> files, string value)
        {
            string entity = MapEntity;
            string key = MapKey;

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file.FileName);
                var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                var fileExtension = (Path.GetExtension(file.FileName.ToLower())).Substring(1);

                try
                {
                    byte[] buffer = new byte[file.ContentLength];
                    file.InputStream.Read(buffer, 0, file.ContentLength - 1);

                    SCN_ATTACHE attach = new SCN_ATTACHE();
                    attach.FILE_FRMT = fileExtension;
                    attach.FILE_SIZE = file.ContentLength;
                    attach.PDF_FILE = buffer;
                    attach.CATG_ID = 1;
                    Db.SCN_ATTACHE.Add(attach);
                    Db.SaveChanges();

                    SCN_ATTACHINTER attacher = new SCN_ATTACHINTER();
                    attacher.TITLE = file.FileName;
                    attacher.ATCH_ID = attach.ID;
                    attacher.ENTI_KEY = key;
                    attacher.ENTI_NAME = entity;
                    attacher.ENTI_VALU = value;
                    Db.SCN_ATTACHINTER.Add(attacher);
                    Db.SaveChanges();
                }

                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
            }
            return Content("");
        }
    }
}
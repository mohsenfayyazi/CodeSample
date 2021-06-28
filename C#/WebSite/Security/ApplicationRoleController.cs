using Equipment.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.WebSite.Security
{
    [Authorize]
    public class ApplicationRoleController : DbController
    {
        //private BandarEntities Db;
        //public ApplicationRoleController()
        //{
        //    Db = this.DB();
        //}

        //~ApplicationRoleController()
        //{
        //    Db.Dispose();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    Db.Dispose();
        //    base.Dispose(disposing);
        //}

        //
        // GET: /ApplicationRole/Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /ApplicationRole/
        public ActionResult GetApplicationRole([DataSourceRequest] DataSourceRequest request)
        {
            return Json(Db.SEC_APP_ROLE.AsEnumerable().ToList().ToDataSourceResult(request));
        }

        //
        // GET: /ApplicationRole/Details/5
        public ActionResult Details(int id = 0)
        {
            SEC_APP_ROLE sec_app_role = Db.SEC_APP_ROLE.Find(id);
            if (sec_app_role == null)
            {
                return HttpNotFound();
            }
            return View(sec_app_role);
        }

        //
        // GET: /ApplicationRole/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ApplicationRole/Create
        [HttpPost]
        public ActionResult Create(SEC_APP_ROLE sec_app_role)
        {
            if (ModelState.IsValid)
            {
                Db.SEC_APP_ROLE.Add(sec_app_role);
                Db.SaveChanges();
            }
            return View(sec_app_role);
        }

        //
        // GET: /ApplicationRole/Edit/5
        public ActionResult Edit(int id = 0)
        {
            SEC_APP_ROLE sec_app_role = Db.SEC_APP_ROLE.Find(id);
            if (sec_app_role == null)
            {
                return HttpNotFound();
            }
            return View(sec_app_role);
        }

        //
        // POST: /ApplicationRole/Edit/5
        [HttpPost]
        public ActionResult Edit(SEC_APP_ROLE sec_app_role)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(sec_app_role).State = EntityState.Modified;
                Db.SaveChanges();
            }
            return View(sec_app_role);
        }

        //
        // GET: /ApplicationRole/Delete/5
        public ActionResult Delete(int id = 0)
        {
            SEC_APP_ROLE sec_app_role = Db.SEC_APP_ROLE.Find(id);
            if (sec_app_role == null)
            {
                return HttpNotFound();
            }
            return View(sec_app_role);
        }

        //
        // POST: /ApplicationRole/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            SEC_APP_ROLE sec_app_role = Db.SEC_APP_ROLE.Find(id);
            Db.SEC_APP_ROLE.Remove(sec_app_role);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

    }

}
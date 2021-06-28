using Equipment.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.WebSite.Security
{
    [Authorize]
    public class JobController : DbController
    {
        //private BandarEntities Db;

        //public JobController()
        //{
        //    Db = this.DB();
        //}

        //~JobController()
        //{
        //    Db.Dispose();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    Db.Dispose();
        //    base.Dispose(disposing);
        //}

        //
        // GET: /Job/Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Job/
        public ActionResult GetJob([DataSourceRequest] DataSourceRequest request)
        {
            return Json(Db.SEC_JOBS.AsEnumerable().ToList().ToDataSourceResult(request));
        }

        //
        // GET: /Job/Details/5
        public ActionResult Details(int id = 0)
        {
            SEC_JOBS sec_job = Db.SEC_JOBS.Find(id);
            if (sec_job == null)
            {
                return HttpNotFound();
            }
            return View(sec_job);
        }

        //
        // GET: /Job/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Job/Create
        [HttpPost]
        public ActionResult Create(SEC_JOBS sec_job)
        {
            if (ModelState.IsValid)
            {
                Db.SEC_JOBS.Add(sec_job);
                Db.SaveChanges();
            }
            return View(sec_job);
        }

        //
        // GET: /Job/Edit/5
        public ActionResult Edit(int id = 0)
        {
            SEC_JOBS sec_job = Db.SEC_JOBS.Find(id);
            if (sec_job == null)
            {
                return HttpNotFound();
            }
            return View(sec_job);
        }

        //
        // POST: /Job/Edit/5
        [HttpPost]
        public ActionResult Edit(SEC_JOBS sec_job)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(sec_job).State = EntityState.Modified;
                Db.SaveChanges();
            }
            return View(sec_job);
        }

        //
        // GET: /Job/Delete/5
        public ActionResult Delete(int id = 0)
        {
            SEC_JOBS sec_job = Db.SEC_JOBS.Find(id);
            if (sec_job == null)
            {
                return HttpNotFound();
            }
            return View(sec_job);
        }

        //
        // POST: /Job/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            SEC_JOBS sec_job = Db.SEC_JOBS.Find(id);
            Db.SEC_JOBS.Remove(sec_job);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

    }

}
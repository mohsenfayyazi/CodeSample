using Equipment.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.WebSite.Security
{
    [Authorize]
    public class FormController : DbController
    {
        //private BandarEntities Db;

        //public FormController()
        //{
        //    Db = this.DB();
        //}

        //~FormController()
        //{
        //    Db.Dispose();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    Db.Dispose();
        //    base.Dispose(disposing);
        //}

        //
        // GET: /Form/Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Form/
        public ActionResult GetForm([DataSourceRequest] DataSourceRequest request)
        {
            return Json(Db.SEC_FORMS.AsEnumerable().ToList().ToDataSourceResult(request));
        }

        //
        // GET: /Form/Details/5
        public ActionResult Details(int id = 0)
        {
            SEC_FORMS sec_form = Db.SEC_FORMS.Find(id);
            if (sec_form == null)
            {
                return HttpNotFound();
            }
            return View(sec_form);
        }

        //
        // GET: /Form/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Form/Create
        [HttpPost]
        public ActionResult Create(SEC_FORMS sec_form)
        {
            if (ModelState.IsValid)
            {
                Db.SEC_FORMS.Add(sec_form);
                Db.SaveChanges();
            }
            return View(sec_form);
        }

        //
        // GET: /Form/Edit/5
        public ActionResult Edit(int id = 0)
        {
            SEC_FORMS sec_form = Db.SEC_FORMS.Find(id);
            if (sec_form == null)
            {
                return HttpNotFound();
            }
            return View(sec_form);
        }

        //
        // POST: /Form/Edit/5
        [HttpPost]
        public ActionResult Edit(SEC_FORMS sec_form)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(sec_form).State = EntityState.Modified;
                Db.SaveChanges();
            }
            return View(sec_form);
        }

        //
        // GET: /Form/Delete/5
        public ActionResult Delete(int id = 0)
        {
            SEC_FORMS sec_form = Db.SEC_FORMS.Find(id);
            if (sec_form == null)
            {
                return HttpNotFound();
            }
            return View(sec_form);
        }

        //
        // POST: /Form/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            SEC_FORMS sec_form = Db.SEC_FORMS.Find(id);
            Db.SEC_FORMS.Remove(sec_form);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

    }

}
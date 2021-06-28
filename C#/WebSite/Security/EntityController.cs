using Equipment.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.WebSite.Security
{
    [Authorize]
    public class EntityController : DbController
    {
        //private BandarEntities Db;

        //public EntityController()
        //{
        //    Db = this.DB();
        //}


        //~EntityController()
        //{            
        //    Db.Dispose();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    Db.Dispose();
        //    base.Dispose(disposing);
        //}


        //
        // GET: /Entity/Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Entity/
        public ActionResult GetEntity([DataSourceRequest] DataSourceRequest request)
        {
            return Json(Db.SEC_ENTITY.AsEnumerable().ToList().ToDataSourceResult(request));
        }

        //
        // GET: /Entity/Details/5
        public ActionResult Details(int id = 0)
        {
            SEC_ENTITY sec_entity = Db.SEC_ENTITY.Find(id);
            if (sec_entity == null)
            {
                return HttpNotFound();
            }
            return View(sec_entity);
        }

        //
        // GET: /Entity/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Entity/Create
        [HttpPost]
        public ActionResult Create(SEC_ENTITY sec_entity)
        {
            if (ModelState.IsValid)
            {
                Db.SEC_ENTITY.Add(sec_entity);
                Db.SaveChanges();
            }
            return View(sec_entity);
        }

        //
        // GET: /Entity/Edit/5
        public ActionResult Edit(int id = 0)
        {
            SEC_ENTITY sec_entity = Db.SEC_ENTITY.Find(id);
            if (sec_entity == null)
            {
                return HttpNotFound();
            }
            return View(sec_entity);
        }

        //
        // POST: /Entity/Edit/5
        [HttpPost]
        public ActionResult Edit(SEC_ENTITY sec_entity)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(sec_entity).State = EntityState.Modified;
                Db.SaveChanges();
            }
            return View(sec_entity);
        }

        //
        // GET: /Entity/Delete/5
        public ActionResult Delete(int id = 0)
        {
            SEC_ENTITY sec_entity = Db.SEC_ENTITY.Find(id);
            if (sec_entity == null)
            {
                return HttpNotFound();
            }
            return View(sec_entity);
        }

        //
        // POST: /Entity/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            SEC_ENTITY sec_entity = Db.SEC_ENTITY.Find(id);
            Db.SEC_ENTITY.Remove(sec_entity);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

    }

}
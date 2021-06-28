using Equipment.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.WebSite.Security
{
    [Authorize]
    public class MenuController : DbController
    {
        //private BandarEntities Db;

        //public MenuController()
        //{
        //    Db = this.DB();
        //}

        //~MenuController()
        //{
        //    Db.Dispose();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    Db.Dispose();
        //    base.Dispose(disposing);
        //}

        //
        // GET: /Menu/Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Menu/
        public ActionResult GetMenu([DataSourceRequest] DataSourceRequest request)
        {
            return Json(Db.SEC_MENU.AsEnumerable().ToList().ToDataSourceResult(request));
        }

        //
        // GET: /Menu/Details/5
        public ActionResult Details(int id = 0)
        {
            SEC_MENU sec_menu = Db.SEC_MENU.Find(id);
            if (sec_menu == null)
            {
                return HttpNotFound();
            }
            return View(sec_menu);
        }

        //
        // GET: /Menu/Create
        public ActionResult Create()
        {
            ViewBag.MENU_MENU_ID = new SelectList(Db.SEC_MENU, "MENU_ID", "MENU_TITL");
            return View();
        }

        //
        // POST: /Menu/Create
        [HttpPost]
        public ActionResult Create(SEC_MENU sec_menu)
        {
            if (ModelState.IsValid)
            {
                Db.SEC_MENU.Add(sec_menu);
                Db.SaveChanges();
            }
            ViewBag.MENU_MENU_ID = new SelectList(Db.SEC_MENU, "MENU_ID", "MENU_TITL", sec_menu.MENU_MENU_ID);
            return View(sec_menu);
        }

        //
        // GET: /Menu/Edit/5
        public ActionResult Edit(int id = 0)
        {
            SEC_MENU sec_menu = Db.SEC_MENU.Find(id);
            if (sec_menu == null)
            {
                return HttpNotFound();
            }
            ViewBag.MENU_MENU_ID = new SelectList(Db.SEC_MENU, "MENU_ID", "MENU_TITL", sec_menu.MENU_MENU_ID);
            return View(sec_menu);
        }

        //
        // POST: /Menu/Edit/5
        [HttpPost]
        public ActionResult Edit(SEC_MENU sec_menu)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(sec_menu).State = EntityState.Modified;
                Db.SaveChanges();
            }
            ViewBag.MENU_MENU_ID = new SelectList(Db.SEC_MENU, "MENU_ID", "MENU_TITL", sec_menu.MENU_MENU_ID);
            return View(sec_menu);
        }

        //
        // GET: /Menu/Delete/5
        public ActionResult Delete(int id = 0)
        {
            SEC_MENU sec_menu = Db.SEC_MENU.Find(id);
            if (sec_menu == null)
            {
                return HttpNotFound();
            }
            return View(sec_menu);
        }

        //
        // POST: /Menu/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            SEC_MENU sec_menu = Db.SEC_MENU.Find(id);
            Db.SEC_MENU.Remove(sec_menu);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

    }

}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GISRZ.Models;

namespace GISRZ.Controllers
{
    public class securityController : Controller
    {
        private gisportalEntities1 db = new gisportalEntities1();

        // GET: security
        public async Task<ActionResult> Index()
        {
            return View(await db.securities.ToListAsync());
        }

        // GET: security/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            security security = await db.securities.FindAsync(id);
            if (security == null)
            {
                return HttpNotFound();
            }
            return View(security);
        }

        // GET: security/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: security/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "page_id,url,name,description,disabled,disabled_date,disabled_user_id")] security security)
        {
            if (ModelState.IsValid)
            {
                db.securities.Add(security);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(security);
        }

        // GET: security/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            security security = await db.securities.FindAsync(id);
            if (security == null)
            {
                return HttpNotFound();
            }
            return View(security);
        }

        // POST: security/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "page_id,url,name,description,disabled,disabled_date,disabled_user_id")] security security)
        {
            if (ModelState.IsValid)
            {
                db.Entry(security).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(security);
        }

        // GET: security/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            security security = await db.securities.FindAsync(id);
            if (security == null)
            {
                return HttpNotFound();
            }
            return View(security);
        }

        // POST: security/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            security security = await db.securities.FindAsync(id);
            db.securities.Remove(security);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

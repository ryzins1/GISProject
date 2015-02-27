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
    public class security_rolesController : Controller
    {
        private gisportalEntities1 db = new gisportalEntities1();

        // GET: security_roles
        public async Task<ActionResult> Index()
        {
            var security_roles = db.security_roles.Include(s => s.role).Include(s => s.security);
            return View(await security_roles.ToListAsync());
        }

        // GET: security_roles/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            security_roles security_roles = await db.security_roles.FindAsync(id);
            if (security_roles == null)
            {
                return HttpNotFound();
            }
            return View(security_roles);
        }

        // GET: security_roles/Create
        public ActionResult Create()
        {
            ViewBag.role_id = new SelectList(db.roles, "role_id", "name");
            ViewBag.page_id = new SelectList(db.securities, "page_id", "url");
            return View();
        }

        // POST: security_roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "role_id,page_id,action_datetime,action_user,action_type")] security_roles security_roles)
        {
            if (ModelState.IsValid)
            {
                db.security_roles.Add(security_roles);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.role_id = new SelectList(db.roles, "role_id", "name", security_roles.role_id);
            ViewBag.page_id = new SelectList(db.securities, "page_id", "url", security_roles.page_id);
            return View(security_roles);
        }

        // GET: security_roles/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            security_roles security_roles = await db.security_roles.FindAsync(id);
            if (security_roles == null)
            {
                return HttpNotFound();
            }
            ViewBag.role_id = new SelectList(db.roles, "role_id", "name", security_roles.role_id);
            ViewBag.page_id = new SelectList(db.securities, "page_id", "url", security_roles.page_id);
            return View(security_roles);
        }

        // POST: security_roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "role_id,page_id,action_datetime,action_user,action_type")] security_roles security_roles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(security_roles).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.role_id = new SelectList(db.roles, "role_id", "name", security_roles.role_id);
            ViewBag.page_id = new SelectList(db.securities, "page_id", "url", security_roles.page_id);
            return View(security_roles);
        }

        // GET: security_roles/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            security_roles security_roles = await db.security_roles.FindAsync(id);
            if (security_roles == null)
            {
                return HttpNotFound();
            }
            return View(security_roles);
        }

        // POST: security_roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            security_roles security_roles = await db.security_roles.FindAsync(id);
            db.security_roles.Remove(security_roles);
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

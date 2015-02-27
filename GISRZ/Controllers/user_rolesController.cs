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
    public class user_rolesController : Controller
    {
        private gisportalEntities1 db = new gisportalEntities1();

        // GET: user_roles
        public async Task<ActionResult> Index()
        {
            var user_roles = db.user_roles.Include(u => u.role);
            return View(await user_roles.ToListAsync());
        }

        // GET: user_roles/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_roles user_roles = await db.user_roles.FindAsync(id);
            if (user_roles == null)
            {
                return HttpNotFound();
            }
            return View(user_roles);
        }

        // GET: user_roles/Create
        public ActionResult Create()
        {
            ViewBag.role_id = new SelectList(db.roles, "role_id", "name");
            return View();
        }

        // POST: user_roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "user_id,role_id,action_datetime,action_user,action_type")] user_roles user_roles)
        {
            if (ModelState.IsValid)
            {
                db.user_roles.Add(user_roles);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.role_id = new SelectList(db.roles, "role_id", "name", user_roles.role_id);
            return View(user_roles);
        }

        // GET: user_roles/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_roles user_roles = await db.user_roles.FindAsync(id);
            if (user_roles == null)
            {
                return HttpNotFound();
            }
            ViewBag.role_id = new SelectList(db.roles, "role_id", "name", user_roles.role_id);
            return View(user_roles);
        }

        // POST: user_roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "user_id,role_id,action_datetime,action_user,action_type")] user_roles user_roles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user_roles).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.role_id = new SelectList(db.roles, "role_id", "name", user_roles.role_id);
            return View(user_roles);
        }

        // GET: user_roles/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_roles user_roles = await db.user_roles.FindAsync(id);
            if (user_roles == null)
            {
                return HttpNotFound();
            }
            return View(user_roles);
        }

        // POST: user_roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            user_roles user_roles = await db.user_roles.FindAsync(id);
            db.user_roles.Remove(user_roles);
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

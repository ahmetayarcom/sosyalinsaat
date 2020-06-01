using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bitirme.Entity;

namespace bitirme.Controllers
{
    [Authorize(Roles = "A")]
    public class taseronController : Controller
    {
        private webinsaatEntities db = new webinsaatEntities();

        // GET: taseron
        public ActionResult Index()
        {
            var taseron = db.taseron.Include(t => t.hesap);
            return View(taseron.ToList());
        }

        [OverrideAuthorization()]
        [Authorize(Roles = "A,M2,T2,U2")]
        public ActionResult TProfil(int? id)
        {
            if (id == null && User.IsInRole("T2"))
            {
                var taserondb = db.taseron.Where(i => i.hesap.mail == User.Identity.Name).FirstOrDefault();
                id = taserondb.ID;
            }
            else if (!User.IsInRole("M2") && !User.IsInRole("T2") && !User.IsInRole("U2") && !User.IsInRole("A"))
            {
                return RedirectToAction("Index", "Home");
            }
            var ustadb = db.usta.Where(m => m.taseron.ID == id);
            var taserondb2 = db.taseron.Where(m => m.ID == id).FirstOrDefault();
            var teklifdb = db.teklif.Where(m => (m.hesapID == taserondb2.hesapID) &&(m.durum==4));
            var teklifdb2 = db.teklif.Where(m => (m.hesapID == taserondb2.hesapID) && (m.durum == 2));
            if (taserondb2!=null)
            {
                ViewBag.thesapID = taserondb2.hesapID;
            }
            var tuple = new Tuple<IEnumerable<usta>, taseron, IEnumerable<teklif>, IEnumerable<teklif>>(ustadb, taserondb2, teklifdb, teklifdb2);


            taserondb2.isSayisi = teklifdb.Count() + teklifdb2.Count();
            db.SaveChanges();

            return View(tuple);
        }


        // GET: taseron/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            taseron taseron = db.taseron.Find(id);
            if (taseron == null)
            {
                return HttpNotFound();
            }
            return View(taseron);
        }

        // GET: taseron/Create
        [OverrideAuthorization()]
        [Authorize(Roles = "T")]
        public ActionResult Create()
        {
            ViewBag.hesapID = new SelectList(db.hesap, "ID", "mail");
            return View();
        }

        // POST: taseron/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [OverrideAuthorization()]
        [Authorize(Roles = "T")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ad,adres,telno,webadresi")] taseron taseron)
        {
            if (ModelState.IsValid)
            {
                var hesapdb = db.hesap.Where(i => i.mail == User.Identity.Name).FirstOrDefault();
                var id = hesapdb.ID;
                taseron.hesapID = id;
                taseron.calisanSayisi = 0;
                db.taseron.Add(taseron);
                hesapdb.rol = "T2";
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.hesapID = new SelectList(db.hesap, "ID", "mail", taseron.hesapID);
            return View(taseron);
        }

        // GET: taseron/Edit/5
        [OverrideAuthorization()]
        [Authorize(Roles = "T2,A")]
        public ActionResult Edit(int? id)
        {
            if (User.IsInRole("T2"))
            {
                var taserondb = db.taseron.Where(i => i.hesap.mail == User.Identity.Name).FirstOrDefault();
                var userid = taserondb.ID;
                if (id == null || id == userid)
                {
                    return View(taserondb);
                }
            }
            if (User.IsInRole("A"))
            {
                taseron taseron = db.taseron.Find(id);
                ViewBag.hesapID = new SelectList(db.hesap, "ID", "mail", taseron.hesapID);
                return View(taseron);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: taseron/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [OverrideAuthorization()]
        [Authorize(Roles = "T2,A")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,hesapID,ad,adres,telno,webadresi")] taseron taseron)
        {
            if (ModelState.IsValid)
            {
                db.Entry(taseron).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("TProfil");
            }
            ViewBag.hesapID = new SelectList(db.hesap, "ID", "mail", taseron.hesapID);
            return View(taseron);
        }

        // GET: taseron/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            taseron taseron = db.taseron.Find(id);
            if (taseron == null)
            {
                return HttpNotFound();
            }
            return View(taseron);
        }

        // POST: taseron/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            taseron taseron = db.taseron.Find(id);
            db.taseron.Remove(taseron);
            db.SaveChanges();
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

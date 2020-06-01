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
    [Authorize(Roles = "A,T2")]
    public class ustaController : Controller
    {
        private webinsaatEntities db = new webinsaatEntities();

        // GET: usta
        public ActionResult Index()
        {
            if (User.IsInRole("A"))
            {
                var usta = db.usta.Include(u => u.hesap).Include(u => u.taseron);
                return View(usta.ToList());
            }
            else if (User.IsInRole("T2"))
            {
                var taserondb = db.taseron.Where(x => x.hesap.mail == User.Identity.Name).FirstOrDefault();
                var usta2 = db.usta.Where(u => u.taseron.ID == taserondb.ID);
                return View(usta2.ToList());
            }
            return RedirectToAction("Index", "Home");
        }

        [OverrideAuthorization()]
        [Authorize(Roles = "A,M2,T2,U2")]
        public ActionResult UProfil(int? id)
        {
            if (id == null && User.IsInRole("U2"))
            {
                var ustadb = db.usta.Where(i => i.hesap.mail == User.Identity.Name).FirstOrDefault();
                id = ustadb.ID;
            }
            else if (!User.IsInRole("M2") && !User.IsInRole("T2") && !User.IsInRole("U2") && !User.IsInRole("A"))
            {
                return RedirectToAction("Index", "Home");
            }
            var ustadb2 = db.usta.Where(m => m.ID == id).FirstOrDefault();
            var teklifdb = db.teklif.Where(m => (m.hesapID == ustadb2.hesapID) && (m.durum == 4));
            var teklifdb2 = db.teklif.Where(m => (m.hesapID == ustadb2.hesapID) && (m.durum == 2));
            ViewBag.uhesapID = ustadb2.hesapID;
            var tuple = new Tuple<usta, IEnumerable<teklif>, IEnumerable<teklif>>(ustadb2, teklifdb, teklifdb2);

            ustadb2.isSayisi = teklifdb.Count()+teklifdb2.Count();
            db.SaveChanges();
            return View(tuple);
        }


        // GET: usta/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            usta usta = db.usta.Find(id);
            if (usta == null)
            {
                return HttpNotFound();
            }
            return View(usta);
        }

        // GET: usta/Create
        [OverrideAuthorization()]
        [Authorize(Roles = "T2,U")]
        public ActionResult Create()
        {
            ViewBag.hesapID = new SelectList(db.hesap, "ID", "mail");
            ViewBag.taseronID = new SelectList(db.taseron, "ID", "ad");
            return View();
        }

        // POST: usta/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [OverrideAuthorization()]
        [Authorize(Roles = "T2,U")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(usta usta, hesap hesap)
        {
            //usta.hesapID = hesap.ID;
            //if (ModelState.IsValid)    bunu yapınca olmadı
            //{
            if (User.IsInRole("T2"))
            {
                var hesapdb2 = db.hesap.Where(i => i.ID == hesap.ID).FirstOrDefault();
                var taserondb = db.taseron.Where(i => i.hesap.mail == User.Identity.Name).FirstOrDefault();
                usta.taseronID = taserondb.ID;
                usta.hesapID = hesapdb2.ID;
                hesapdb2.rol = "U2";
                db.usta.Add(usta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var hesapdb = db.hesap.Where(i => i.mail == User.Identity.Name).FirstOrDefault();
            var id = hesapdb.ID;
            usta.hesapID = id;
            db.usta.Add(usta);
            hesapdb.rol = "U2";
            db.SaveChanges();
            return RedirectToAction("Index");
            //}

            ViewBag.hesapID = new SelectList(db.hesap, "ID", "mail", usta.hesapID);
            ViewBag.taseronID = new SelectList(db.taseron, "ID", "ad", usta.taseronID);
            return View(usta);
        }

        // GET: usta/Edit/5
        [OverrideAuthorization()]
        [Authorize(Roles = "T2,U2,A")]
        public ActionResult Edit(int? id)
        {
            if (User.IsInRole("U2"))
            {
                var ustadb = db.usta.Where(i => i.hesap.mail == User.Identity.Name).FirstOrDefault();
                var userid = ustadb.ID;
                if (id == null || id == userid)
                {
                    return View(ustadb);
                }
            }
            if (User.IsInRole("A") || User.IsInRole("T2"))
            {
                if (User.IsInRole("T2"))
                {
                    var taserondb = db.taseron.Where(x => x.hesap.mail == User.Identity.Name).FirstOrDefault();
                    var tasid = taserondb.ID;
                    usta ustadb = db.usta.Find(id);
                    if (ustadb.taseronID==tasid)
                    {
                        return View(ustadb);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                usta usta = db.usta.Find(id);
                ViewBag.hesapID = new SelectList(db.hesap, "ID", "mail", usta.hesapID);
                ViewBag.taseronID = new SelectList(db.taseron, "ID", "ad", usta.taseronID);
                return View(usta);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: usta/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [OverrideAuthorization()]
        [Authorize(Roles = "T2,U2,A")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,hesapID,ad,soyad,taseronID")] usta usta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.hesapID = new SelectList(db.hesap, "ID", "mail", usta.hesapID);
            ViewBag.taseronID = new SelectList(db.taseron, "ID", "ad", usta.taseronID);
            return View(usta);
        }

        // GET: usta/Delete/5
        [OverrideAuthorization()]
        [Authorize(Roles = "A,U")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            usta usta = db.usta.Find(id);
            if (usta == null)
            {
                return HttpNotFound();
            }
            return View(usta);
        }


        // POST: usta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            usta usta = db.usta.Find(id);
            db.usta.Remove(usta);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [OverrideAuthorization()]
        [Authorize(Roles = "T2")]
        public ActionResult TUSil(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            usta usta = db.usta.Find(id);
            if (usta == null)
            {
                return HttpNotFound();
            }
            var taserondb = db.taseron.Where(x => x.hesap.mail == User.Identity.Name).FirstOrDefault();
            var tasid = taserondb.ID;
            if (usta.taseronID == tasid)
            {
                return View(usta);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost, ActionName("TUSil")]
        [ValidateAntiForgeryToken]
        public ActionResult TUSilOnay(int id)
        {
            usta usta = db.usta.Find(id);
            if (usta == null)
            {
                return HttpNotFound();
            }
            usta.taseronID = 1;
            db.SaveChanges();
            return RedirectToAction("Index", "usta");
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

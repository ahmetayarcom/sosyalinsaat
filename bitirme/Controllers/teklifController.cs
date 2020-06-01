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
    [Authorize(Roles = "A,M2,T2,U2")]
    public class teklifController : Controller
    {
        private webinsaatEntities db = new webinsaatEntities();

        // GET: teklif
        public ActionResult Index()
        {
            if (User.IsInRole("U2")|| User.IsInRole("T2"))
            {
                var hesapdb = db.hesap.Where(x => x.mail == User.Identity.Name).FirstOrDefault();
                var teklif = db.teklif.Where(x => x.hesapID == hesapdb.ID);
                return View(teklif.ToList());
            }
            if (User.IsInRole("M2"))
            {
                var hesapdb = db.hesap.Where(x => x.mail == User.Identity.Name).FirstOrDefault();
                var teklif = db.teklif.Where(x => x.ilan.proje.miteahhitID == hesapdb.ID);
                var gteklif = teklif.Where(x => x.durum == 1 || x.durum == 2);
                return View(gteklif.ToList());
            }
            if (User.IsInRole("A"))
            {
                var teklif = db.teklif.Include(t => t.hesap).Include(t => t.ilan).Include(t => t.usta);
                return View(teklif.ToList());
            }
            return RedirectToAction("Index","home");
        }

        // GET: teklif/Details/5
        [OverrideAuthorization()]
        [Authorize(Roles = "M2")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            teklif teklif = db.teklif.Find(id);
            if (teklif == null)
            {
                return HttpNotFound();
            }
            return View(teklif);
        }

        // GET: teklif/Create
        [OverrideAuthorization()]
        [Authorize(Roles = "T2,U2")]
        public ActionResult Create(int id)
        {
            if (User.IsInRole("T2"))
            {
                var taserondb = db.taseron.Where(x => x.hesap.mail == User.Identity.Name).FirstOrDefault();
                //var ustadb = db.usta.Where(x => x.taseronID == taserondb.ID);
                var teklifdb = db.teklif.Where(x => x.ilanID == id);
                var teklifdb2 = teklifdb.Where(x => x.hesapID == taserondb.hesapID).FirstOrDefault();

                if (teklifdb2 != null)
                {
                    return RedirectToAction("Index","Home");
                }

                ViewBag.hesapID = taserondb.hesapID;
                ViewBag.ilanID = id;
                ViewBag.ekipID = new SelectList(db.usta.Where(x => x.taseronID == taserondb.ID), "ID", "ad");
                ViewBag.durum = 1;
                return View();
            }
            if (User.IsInRole("U2"))
            {
                var ustadb = db.usta.Where(x => x.hesap.mail == User.Identity.Name).FirstOrDefault();
                //var ustadb = db.usta.Where(x => x.taseronID == taserondb.ID);
                var teklifdb = db.teklif.Where(x => x.ilanID == id).FirstOrDefault();

                if (teklifdb != null)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.hesapID = ustadb.hesapID;
                ViewBag.ilanID = id;
                ViewBag.ekipID = ustadb.ID;
                ViewBag.durum = 1;
                return View();
            }
            return View();
        }

        // POST: teklif/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [OverrideAuthorization()]
        [Authorize(Roles = "T2,U2")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(/*[Bind(Include = "ID,hesapID,fiyat,ilanID,durum,ekipID")] */teklif teklif)
        {
            if (ModelState.IsValid)
            {
                db.teklif.Add(teklif);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.hesapID = new SelectList(db.hesap, "ID", "mail", teklif.hesapID);
            ViewBag.ilanID = new SelectList(db.ilan, "ID", "ad", teklif.ilanID);
            ViewBag.ekipID = new SelectList(db.usta, "ID", "ad", teklif.ekipID);
            return View(teklif);
        }

        // GET: teklif/Edit/5
        [OverrideAuthorization()]
        [Authorize(Roles = "T2,U2")]
        public ActionResult Edit(int id)
        {
            if (User.IsInRole("U2"))
            {
                var ustadb = db.usta.Where(i => i.hesap.mail == User.Identity.Name).FirstOrDefault();
                var teklifdb = db.teklif.Find(id);
                if (teklifdb == null)
                {
                    return HttpNotFound();
                }
                if (teklifdb.hesapID==ustadb.hesapID)
                {
                    ViewBag.hesapID = ustadb.hesapID;
                    ViewBag.ilanID = teklifdb.ilanID;
                    ViewBag.ekipID = ustadb.ID;
                    ViewBag.durum = teklifdb.durum;
                    return View(teklifdb);
                }
            }
            if (User.IsInRole("T2"))
            {
                var taserondb = db.taseron.Where(i => i.hesap.mail == User.Identity.Name).FirstOrDefault();
                var teklifdb = db.teklif.Find(id);
                if (teklifdb == null)
                {
                    return HttpNotFound();
                }
                if (teklifdb.hesapID == taserondb.hesapID)
                {
                    ViewBag.hesapID = taserondb.hesapID;
                    ViewBag.ilanID = id;
                    ViewBag.ekipID = new SelectList(db.usta.Where(x => x.taseronID == taserondb.ID), "ID", "ad");
                    ViewBag.durum = teklifdb.durum;
                    return View(teklifdb);
                }
            }
            return RedirectToAction("Index","Home");
        }

        // POST: teklif/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [OverrideAuthorization()]
        [Authorize(Roles = "T2,U2")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,hesapID,fiyat,ilanID,durum,ekipID")] teklif teklif)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teklif).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.hesapID = new SelectList(db.hesap, "ID", "mail", teklif.hesapID);
            ViewBag.ilanID = new SelectList(db.ilan, "ID", "ad", teklif.ilanID);
            ViewBag.ekipID = new SelectList(db.usta, "ID", "ad", teklif.ekipID);
            return View(teklif);
        }

        // GET: teklif/Delete/5
        [OverrideAuthorization()]
        [Authorize(Roles = "T2,U2")]
        public ActionResult Delete(int? id)
        {
            if (User.IsInRole("U2"))
            {
                var ustadb = db.usta.Where(i => i.hesap.mail == User.Identity.Name).FirstOrDefault();
                var teklifdb = db.teklif.Find(id);
                if (teklifdb == null)
                {
                    return HttpNotFound();
                }
                if (teklifdb.hesapID == ustadb.hesapID)
                {
                    //ViewBag.hesapID = ustadb.hesapID;
                    //ViewBag.ilanID = teklifdb.ilanID;
                    //ViewBag.ekipID = ustadb.ID;
                    //ViewBag.durum = teklifdb.durum;
                    return View(teklifdb);
                }
            }
            if (User.IsInRole("T2"))
            {
                var taserondb = db.taseron.Where(i => i.hesap.mail == User.Identity.Name).FirstOrDefault();
                var teklifdb = db.teklif.Find(id);
                if (teklifdb == null)
                {
                    return HttpNotFound();
                }
                if (teklifdb.hesapID == taserondb.hesapID)
                {
                    //ViewBag.hesapID = taserondb.hesapID;
                    //ViewBag.ilanID = id;
                    //ViewBag.ekipID = new SelectList(db.usta.Where(x => x.taseronID == taserondb.ID), "ID", "ad");
                    //ViewBag.durum = teklifdb.durum;
                    return View(teklifdb);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: teklif/Delete/5
        [OverrideAuthorization()]
        [Authorize(Roles = "T2,U2")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            teklif teklif = db.teklif.Find(id);
            db.teklif.Remove(teklif);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [OverrideAuthorization()]
        [Authorize(Roles = "M2")]
        public ActionResult Onay(int id)
        {
            var teklifdb = db.teklif.Find(id);
            var siltek = db.teklif.Where(x => x.ilanID == teklifdb.ilanID);

            foreach (var item in siltek)
            {
                item.durum = 3;
            }

            teklifdb.durum = 2;
            teklifdb.ilan.durum = 2;


            db.SaveChanges();
            return RedirectToAction("Index","teklif");
        }

        [OverrideAuthorization()]
        [Authorize(Roles = "M2")]
        public ActionResult Bitti(int id)
        {
            var teklifdb = db.teklif.Find(id);

            teklifdb.durum = 4;
            teklifdb.ilan.durum = 3;

            db.SaveChanges();
            return RedirectToAction("Index", "teklif");
        }

        [OverrideAuthorization()]
        [Authorize(Roles = "M2")]
        public ActionResult Red(int id)
        {
            var teklifdb = db.teklif.Find(id);

            teklifdb.durum = 3;

            db.SaveChanges();
            return RedirectToAction("Index", "teklif");
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

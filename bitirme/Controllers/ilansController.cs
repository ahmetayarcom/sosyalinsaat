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
    [Authorize(Roles = "A,M2")]
    public class ilansController : Controller
    {
        private webinsaatEntities db = new webinsaatEntities();

        // GET: ilans
        public ActionResult Index()
        {
            if (User.IsInRole("A"))
            {
                var ilandb = db.ilan.Include(x => x.teklif);
                return View(ilandb.ToList());
            }
            var muteahhitdb = db.muteahhit.Where(x => x.hesap.mail == User.Identity.Name).FirstOrDefault();
            var id = muteahhitdb.ID;
            var ilan = db.ilan.Where(i => i.proje.miteahhitID == id);
            return View(ilan.ToList());
        }

        [OverrideAuthorization()]
        [Authorize(Roles = "A,M2,T2,U2")]
        public ActionResult IDetay(int id)
        {

            var ilan = db.ilan.Where(m => m.ID == id).FirstOrDefault();
            //var muteahhit = db.muteahhit.Where(m => m.proje == id).FirstOrDefault();
            //var proje = db.proje.Where(m => m.ID == id).FirstOrDefault();
            //var tuple = new Tuple<IEnumerable<ilan>, proje>(ilan, proje);
            return View(ilan);
        }

        // GET: ilans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ilan ilan = db.ilan.Find(id);
            if (ilan == null)
            {
                return HttpNotFound();
            }
            return View(ilan);
        }

        // GET: ilans/Create
        public ActionResult Create()
        {
            ViewBag.typeID = new SelectList(db.ilanType, "ID", "type");
            ViewBag.projeID = new SelectList(db.proje.Where(x=>x.muteahhit.hesap.mail==User.Identity.Name), "ID", "ad");
            return View();
        }

        // POST: ilans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ad,foto,baslamaTarih,bitisTarih,boyut,projeID,typeID")] ilan ilan)
        {
            if (ModelState.IsValid)
            {
                ilan.durum = 1;
                db.ilan.Add(ilan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.typeID = new SelectList(db.ilanType, "ID", "type", ilan.typeID);
            ViewBag.projeID = new SelectList(db.proje, "ID", "ad", ilan.projeID);
            return View(ilan);
        }

        // GET: ilans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ilan ilan = db.ilan.Find(id);
            if (ilan == null)
            {
                return HttpNotFound();
            }
            

            ViewBag.typeID = new SelectList(db.ilanType, "ID", "type", ilan.typeID);
            ViewBag.projeID = new SelectList(db.proje, "ID", "ad", ilan.projeID);
            return View(ilan);
        }

        // POST: ilans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ad,foto,baslamaTarih,bitisTarih,birimFiyat,boyut,durum,projeID,typeID")] ilan ilan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ilan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.typeID = new SelectList(db.ilanType, "ID", "type", ilan.typeID);
            ViewBag.projeID = new SelectList(db.proje, "ID", "ad", ilan.projeID);
            return View(ilan);
        }

        // GET: ilans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ilan ilan = db.ilan.Find(id);
            if (ilan == null)
            {
                return HttpNotFound();
            }
            return View(ilan);
        }

        // POST: ilans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ilan ilan = db.ilan.Find(id);
            db.ilan.Remove(ilan);
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

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
    public class projeController : Controller
    {
        private webinsaatEntities db = new webinsaatEntities();

        // GET: proje
        public ActionResult Index()
        {
            if (User.IsInRole("A"))
            {
                var projedb = db.proje.Include(x=>x.ilan);
                return View(projedb.ToList());
            }
            var muteahhitdb = db.muteahhit.Where(x => x.hesap.mail == User.Identity.Name).FirstOrDefault();
            var id = muteahhitdb.ID;
            var proje = db.proje.Where(p => p.muteahhit.ID==id);
            return View(proje.ToList());
        }

        [OverrideAuthorization()]
        [Authorize(Roles = "A,M2,T2,U2")]
        public ActionResult PDetay(int id)
        {

            var ilan = db.ilan.Where(m => m.proje.ID == id);
            //var muteahhit = db.muteahhit.Where(m => m.proje == id).FirstOrDefault();
            var projedb = db.proje.Where(m => m.ID == id).FirstOrDefault();

            var ilandb = ilan.Where(x => x.durum == 1);
            var dilandb = ilan.Where(x => x.durum == 2);
            var bilandb = ilan.Where(x => x.durum == 3);


            var tuple = new Tuple<IEnumerable<ilan>,proje, IEnumerable<ilan>, IEnumerable<ilan>>(ilandb, projedb, dilandb,bilandb);

            var pilansay = ilan.Count(x => x.projeID == projedb.ID);
            projedb.isSayisi = pilansay;
            db.SaveChanges();

            return View(tuple);
        }


        // GET: proje/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            proje proje = db.proje.Find(id);
            if (proje == null)
            {
                return HttpNotFound();
            }
            return View(proje);
        }

        // GET: proje/Create
        public ActionResult Create()
        {
            ViewBag.miteahhitID = new SelectList(db.muteahhit, "ID", "ad");
            return View();
        }

        // POST: proje/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,miteahhitID,ad,adres,isSayisi,foto")] proje proje)
        {
            if (User.IsInRole("M2"))
            {
                var muteahhitdb = db.muteahhit.Where(x => x.hesap.mail == User.Identity.Name).FirstOrDefault();
                proje.miteahhitID = muteahhitdb.ID;
            }
            if (ModelState.IsValid)
            {
                db.proje.Add(proje);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.miteahhitID = new SelectList(db.muteahhit, "ID", "ad", proje.miteahhitID);
            return View(proje);
        }

        // GET: proje/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            proje proje = db.proje.Find(id);
            if (proje == null)
            {
                return HttpNotFound();
            }
            ViewBag.miteahhitID = new SelectList(db.muteahhit, "ID", "ad", proje.miteahhitID);
            return View(proje);
        }

        // POST: proje/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,miteahhitID,ad,adres,isSayisi,foto")] proje proje)
        {
            if (ModelState.IsValid)
            {
                db.Entry(proje).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.miteahhitID = new SelectList(db.muteahhit, "ID", "ad", proje.miteahhitID);
            return View(proje);
        }

        // GET: proje/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            proje proje = db.proje.Find(id);
            if (proje == null)
            {
                return HttpNotFound();
            }
            return View(proje);
        }

        // POST: proje/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            proje proje = db.proje.Find(id);
            db.proje.Remove(proje);
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

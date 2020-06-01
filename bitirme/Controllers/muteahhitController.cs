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
    public class muteahhitController : Controller
    {
        private webinsaatEntities db = new webinsaatEntities();

        // GET: muteahhit
        public ActionResult Index()
        {
            var muteahhit = db.muteahhit.Include(m => m.hesap);
            return View(muteahhit.ToList());
        }


        [OverrideAuthorization()]
        [Authorize(Roles = "A,M2,T2,U2")]
        public ActionResult MProfil(int? id)
        {
            if (id==null&&User.IsInRole("M2"))
            {
                var muteahhitdb = db.muteahhit.Where(i => i.hesap.mail == User.Identity.Name).FirstOrDefault();
                id = muteahhitdb.ID;
            }
            else if (!User.IsInRole("M2")&& !User.IsInRole("T2") && !User.IsInRole("U2") && !User.IsInRole("A"))
            {
                return RedirectToAction("Index", "Home");
            }
            var ilan = db.ilan.Where(m => m.proje.muteahhit.ID == id);
            var hesapdb = db.hesap.Find(id);
            var muteahhit = db.muteahhit.Where(m => m.hesapID == hesapdb.ID).FirstOrDefault();
            var proje = db.proje.Where(m => m.miteahhitID == id);
            ViewBag.mhesapID = muteahhit.hesapID;
            var tuple= new Tuple<IEnumerable<ilan>, muteahhit, IEnumerable<proje>>(ilan, muteahhit,proje);

            var ilansay = ilan.Count();
            muteahhit.issayisi = ilansay;
            db.SaveChanges();

            return View(tuple);
        }

        // GET: muteahhit/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                var muteahhitdb = db.muteahhit.Where(i => i.hesap.mail == User.Identity.Name).FirstOrDefault();
                id = muteahhitdb.hesapID;
                return View(muteahhitdb);
            }
            muteahhit muteahhit = db.muteahhit.Find(id);
            if (User.IsInRole("A"))
            {
                muteahhit = db.muteahhit.Find(muteahhit.hesapID);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View(muteahhit);
        }

        [OverrideAuthorization()]
        [Authorize(Roles = "M")]
        // GET: muteahhit/Create
        public ActionResult Create()
        {
            ViewBag.hesapID = new SelectList(db.hesap, "ID", "mail");
            return View();
        }

        // POST: muteahhit/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [OverrideAuthorization()]
        [Authorize(Roles = "M")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ad,adres,telno,webadresi")] muteahhit muteahhit)
        {
            if (ModelState.IsValid)
            {
                var hesapdb = db.hesap.Where(i => i.mail == User.Identity.Name).FirstOrDefault();
                var id = hesapdb.ID;
                muteahhit.hesapID = id;
                db.muteahhit.Add(muteahhit);
                hesapdb.rol = "M2";
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.hesapID = new SelectList(db.hesap, "ID", "mail", muteahhit.hesapID);
            return View(muteahhit);
        }

        [OverrideAuthorization()]
        [Authorize(Roles = "M2,A")]
        // GET: muteahhit/Edit/5
        public ActionResult Edit(int? id)
        {
            if (User.IsInRole("M2"))
            {
                var muteahhitdb = db.muteahhit.Where(i => i.hesap.mail == User.Identity.Name).FirstOrDefault();
                var userid = muteahhitdb.ID;
                if (id == null || id == userid)
                {
                    return View(muteahhitdb);
                }
            }
            
            if (User.IsInRole("A"))
            {
                muteahhit muteahhit = db.muteahhit.Find(id);
                //muteahhit = db.muteahhit.Find(muteahhit.hesapID);
                ViewBag.hesapID = new SelectList(db.hesap, "ID", "mail", muteahhit.hesapID);
                return View(muteahhit);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [OverrideAuthorization()]
        [Authorize(Roles = "M2,A")]
        // POST: muteahhit/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,hesapID,ad,adres,telno,webadresi")] muteahhit muteahhit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(muteahhit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MProfil");
            }
            ViewBag.hesapID = new SelectList(db.hesap, "ID", "mail", muteahhit.hesapID);
            return View(muteahhit);
        }

        // GET: muteahhit/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            muteahhit muteahhit = db.muteahhit.Find(id);
            if (muteahhit == null)
            {
                return HttpNotFound();
            }
            return View(muteahhit);
        }

        // POST: muteahhit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            muteahhit muteahhit = db.muteahhit.Find(id);
            db.muteahhit.Remove(muteahhit);
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

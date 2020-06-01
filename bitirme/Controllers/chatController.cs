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
    public class chatController : Controller
    {
        private webinsaatEntities db = new webinsaatEntities();

        // GET: chat
        public ActionResult Index()
        {
            var chat = db.chat.Include(c => c.hesap).Include(c => c.hesap1);
            return View(chat.ToList());
        }

        public ActionResult MPage(int? id, int? cid)
        {
            if (id == null && cid == null)
            {
                var hesapdb2 = db.hesap.Where(x => x.mail == User.Identity.Name).FirstOrDefault();
                var chatdb2 = db.chat.Where(x => x.taraf1ID == hesapdb2.ID || x.taraf2ID == hesapdb2.ID);
                var mesajdb2 = db.mesaj.Where(x => x.chat.hesap1.ID == hesapdb2.ID);
                var tuple2 = new Tuple<IEnumerable<chat>, IEnumerable<mesaj>>(chatdb2, mesajdb2);
                string style1= "style=display:none";
                ViewBag.dpone = style1;
                return View(tuple2);
            }
            if (cid!=null)
            {
                var hesapdb2 = db.hesap.Where(x => x.mail == User.Identity.Name).FirstOrDefault();
                var chatdb3 = db.chat.Where(x => x.taraf1ID == hesapdb2.ID || x.taraf2ID == hesapdb2.ID);
                var chardbi = db.chat.Find(cid);
                var mesajdbc = db.mesaj.Where(x => x.chatID == cid);
                ViewBag.gonderenID = hesapdb2.ID;
                if (hesapdb2.ID == chardbi.taraf1ID)
                {
                    ViewBag.alanID = chardbi.taraf2ID;
                }
                else
                {
                    ViewBag.alanID = chardbi.taraf1ID;
                }
                ViewBag.chatID = chardbi.ID;
                ViewBag.anaMesajID = 0;
                var tuple3 = new Tuple<IEnumerable<chat>, IEnumerable<mesaj>>(chatdb3, mesajdbc);
                return View(tuple3);
            }
            //var mesaj = db.mesaj.Include(m => m.hesap).Include(m => m.hesap1);
            var hesapdb = db.hesap.Where(x => x.mail == User.Identity.Name).FirstOrDefault();
            if (hesapdb.ID==id)
            {
                return RedirectToAction("Index", "home");
            }
            var chatdb1 = db.chat.Where(x => x.taraf1ID == hesapdb.ID || x.taraf2ID == hesapdb.ID);

            var chatdb = db.chat.Where(x => (x.taraf1ID == hesapdb.ID && x.taraf2ID == id) || (x.taraf1ID == id && x.taraf2ID == hesapdb.ID)).FirstOrDefault();

            if (chatdb == null)
            {
                ViewBag.gonderenID = hesapdb.ID;
                ViewBag.alanID = id;
                return RedirectToAction("Create", "chat", new { id = id });
            }
            //var chatdb = db.chat.Where(x => (x.taraf1ID == chatk.taraf1ID && x.taraf2ID == chatk.taraf2ID) || (x.taraf1ID == chatk.taraf2ID && x.taraf2ID == chatk.taraf1ID)).FirstOrDefault();
            var mesajdb = db.mesaj.Where(x => x.chatID == chatdb.ID);

            ViewBag.gonderenID = hesapdb.ID;
            if (hesapdb.ID == chatdb.taraf1ID)
            {
                ViewBag.alanID = chatdb.taraf2ID;
            }
            else
            {
                ViewBag.alanID = chatdb.taraf1ID;
            }
            ViewBag.chatID = chatdb.ID;
            ViewBag.anaMesajID = 0;
            //ViewBag.ekipID = new SelectList(db.usta.Where(x => x.taseronID == taserondb.ID), "ID", "ad");

            var tuple = new Tuple<IEnumerable<chat>, IEnumerable<mesaj>>(chatdb1, mesajdb);
            return View(tuple);

        }

        // POST: mesaj/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MPage(mesaj mesaj)
        {
            mesaj.sendTime = System.DateTime.Now;
            mesaj.baslik = " ";

            if (ModelState.IsValid)
            {
                db.mesaj.Add(mesaj);
                db.SaveChanges();
                return RedirectToAction("MPage", "chat", new { cid = mesaj.chatID });
            }

            //ViewBag.alanHesapID = new SelectList(db.hesap, "ID", "mail", mesaj.alanHesapID);
            //ViewBag.gonderenHesapID = new SelectList(db.hesap, "ID", "mail", mesaj.gonderenHesapID);
            return View(mesaj);
        }


        // GET: chat/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            chat chat = db.chat.Find(id);
            if (chat == null)
            {
                return HttpNotFound();
            }
            return View(chat);
        }

        // GET: chat/Create
        public ActionResult Create(int id)
        {
            var hesapdb = db.hesap.Where(x => x.mail == User.Identity.Name).FirstOrDefault();
            var chatdb = db.chat.Where(x => (x.taraf1ID == id && x.taraf2ID == hesapdb.ID) || (x.taraf1ID == hesapdb.ID && x.taraf2ID == id)).FirstOrDefault();
            if (chatdb != null)
            {
                return RedirectToAction("MPage", "chat", new { id = id });
            }
            chat chat = new chat
            {
                taraf1ID = hesapdb.ID,
                taraf2ID = id
            };

            db.chat.Add(chat);
            db.SaveChanges();

            return RedirectToAction("MPage", "chat", new { id = id });
        }

        // POST: chat/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,gonderenID,alanID")] chat chat)
        {
            if (ModelState.IsValid)
            {
                db.chat.Add(chat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.taraf1ID = new SelectList(db.hesap, "ID", "mail", chat.taraf1ID);
            //ViewBag.ID = new SelectList(db.hesap, "ID", "mail", chat.ID);
            return View(chat);
        }

        // GET: chat/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            chat chat = db.chat.Find(id);
            if (chat == null)
            {
                return HttpNotFound();
            }
            //ViewBag.alanID = new SelectList(db.hesap, "ID", "mail", chat.alanID);
            //ViewBag.ID = new SelectList(db.hesap, "ID", "mail", chat.ID);
            return View(chat);
        }

        // POST: chat/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,gonderenID,alanID")] chat chat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.alanID = new SelectList(db.hesap, "ID", "mail", chat.alanID);
            //ViewBag.ID = new SelectList(db.hesap, "ID", "mail", chat.ID);
            return View(chat);
        }

        // GET: chat/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            chat chat = db.chat.Find(id);
            if (chat == null)
            {
                return HttpNotFound();
            }
            return View(chat);
        }

        // POST: chat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            chat chat = db.chat.Find(id);
            db.chat.Remove(chat);
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

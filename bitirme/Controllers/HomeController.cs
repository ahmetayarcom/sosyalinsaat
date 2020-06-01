using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bitirme.Entity;
using System.Data.Entity;

namespace bitirme.Controllers
{
    public class HomeController : Controller
    {
        private webinsaatEntities db = new webinsaatEntities();

        // GET: Home
        public ActionResult Index()
        {
            int ustsay = db.usta.Count();
            int tassay = db.taseron.Count();
            int bitilansay = db.ilan.Where(x => x.durum == 3).Count();
            int topilan = db.ilan.Count();
            int mutsay = db.muteahhit.Count();
            int hveren = ustsay + tassay;

            ViewBag.hveren = hveren;
            ViewBag.bitilansay = bitilansay;
            ViewBag.topilan = topilan;
            ViewBag.mutsay = mutsay;

            var ustadb = db.usta.Include(x=>x.hesap);
            var tasedb = db.taseron.Include(x => x.hesap);
            var ilandb = db.ilan.Where(x => x.durum == 1);

            var tuple = new Tuple<IEnumerable<usta>, IEnumerable<taseron>, IEnumerable<ilan>>(ustadb, tasedb, ilandb);
            return View(tuple);
        }

        public ActionResult Mesaj()
        {

            return View("Mesaj");
        }

        public ActionResult Giris()
        {
            return View("Giris");
        }

        public ActionResult Hakkimizda()
        {
            int ustsay = db.usta.Count();
            int tassay = db.taseron.Count();
            int bitilansay = db.ilan.Where(x=>x.durum==3).Count();
            int topilan = db.ilan.Count();
            int mutsay = db.muteahhit.Count();
            int hveren = ustsay + tassay;

            ViewBag.hveren = hveren;
            ViewBag.bitilansay = bitilansay;
            ViewBag.topilan = topilan;
            ViewBag.mutsay = mutsay;
            
            return View("Hakkimizda");
        }

        public ActionResult HVeren()
        {
            return View("HVeren");
        }

        public ActionResult Ustalar()
        {
            var ustalar = db.usta.Include(i=>i.hesap);
            return View(ustalar.ToList());
        }

        public ActionResult Taseronlar()
        {
            var taseronlar = db.taseron.Include(i => i.hesap);
            return View(taseronlar.ToList());
        }

        //[Authorize]
        public ActionResult Ilan()
        {
            var ilan = db.ilan.Where(x=>x.durum==1);
            return View(ilan.ToList());
        }

        public ActionResult Iletisim()
        {
            return View("Iletisim");
        }

        public ActionResult Kayit()
        {
            return View("Kayit");
        }

        public ActionResult Taseron()
        {
            return View("Taseron");
        }

        public ActionResult Usta()
        {
            return View("Usta");
        }
    }
}
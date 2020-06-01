using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using bitirme.Entity;

namespace bitirme.Controllers
{
    [Authorize(Roles = "A")]
    public class hesapController : Controller
    {
        private webinsaatEntities db = new webinsaatEntities();

        // GET: hesap
        public ActionResult Index()
        {
            var hesap = db.hesap.Include(h => h.hesapType);
            return View(hesap.ToList());
        }
        
        public ActionResult APanel()
        {
            var admindb = db.hesap.Where(x => x.mail == User.Identity.Name).FirstOrDefault();
            return View(admindb);
        }

        //public ActionResult Details()
        //{
        //    var id = User.Identity.GetUserID();
        //    hesap hesap = db.hesap.Find(id);
        //    return View(hesap);
        //}
        [AllowAnonymous]
        public ActionResult PP(int? id)
        {
            if (id!=null)
            {
                var hesapdb = db.hesap.Find(id);
                if (hesapdb.rol=="U2")
                {
                    var id2=hesapdb.usta.First().ID;
                    return RedirectToAction("UProfil", "usta",new { id= id2 });
                }
                if (hesapdb.rol == "T2")
                {
                    var id3 = hesapdb.taseron.First().ID;
                    return RedirectToAction("TProfil", "taseron", new { id = id3 });
                }

            }
            if (User.IsInRole("M2"))
            {
                return RedirectToAction("MProfil", "muteahhit");
            }
            else if (User.IsInRole("T2"))
            {
                return RedirectToAction("TProfil", "taseron");
            }
            else if (User.IsInRole("U2"))
            {
                return RedirectToAction("UProfil", "usta");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        
        // GET: hesap/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                var hesapdb = db.hesap.Where(i => i.mail == User.Identity.Name).FirstOrDefault();
                id = hesapdb.ID;
                return View(hesapdb);
            }
            hesap hesap = new hesap();
            if (User.IsInRole("A"))
            {
                hesap = db.hesap.Find(id);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View(hesap);
        }

        [AllowAnonymous]
        // GET: hesap/Create
        public ActionResult Create()
        {
            if (User.IsInRole("U"))
            {
                return RedirectToAction("Index", "home");
            }
            else if (User.IsInRole("M"))
            {
                return RedirectToAction("Index", "home");
            }
            else if (User.IsInRole("T"))
            {
                return RedirectToAction("Index", "home");
            }
            else if (User.IsInRole("M2"))
            {
                return RedirectToAction("Index", "home");
            }
            else if (User.IsInRole("T2"))
            {
                ViewBag.typeID = new SelectList(db.hesapType, "ID", "type");
                return View();
            }
            else if (User.IsInRole("U2"))
            {
                return RedirectToAction("Index", "home");
            }
            ViewBag.typeID = new SelectList(db.hesapType, "ID", "type");
            if (TempData["Mesaj"] != null)
            {
                ViewBag.Mesaj = "Girdiğiniz bilgiler hatalıdır!!";
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(hesap hesap)
        {
            if (User.IsInRole("T2"))
            {
                hesap.typeID = 1;
            }
            //if (ModelState.IsValid)
            //{
                if (hesap.typeID == 3)
                {
                    hesap.rol = "M";
                }
                else if (hesap.typeID == 2)
                {
                    hesap.rol = "T";
                }
                else if(hesap.typeID==1)
                {
                    hesap.rol = "U";
                }
                hesap hesap1 = db.hesap.Where(i => i.mail == hesap.mail).FirstOrDefault();
                if (hesap1 == null)
                {
                    db.hesap.Add(hesap);
                    try
                    {
                        db.SaveChanges();
                        if (!User.IsInRole("A"))
                        {
                            var hesapdb = db.hesap.FirstOrDefault(x => x.mail == hesap.mail && x.sifre == hesap.sifre);
                            if (hesapdb != null)
                            {
                                if (!User.IsInRole("T2"))
                                {
                                    FormsAuthentication.SetAuthCookie(hesap.mail, false);
                                }
                                if (hesap.typeID == 3)
                                {
                                    return RedirectToAction("Create", "muteahhit");
                                }
                                else if (hesap.typeID == 2)
                                {
                                    return RedirectToAction("Create", "taseron");
                                }
                                else if (hesap.typeID == 1)
                                {
                                    return RedirectToAction("Create", "usta",hesap);
                                }
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else if (User.IsInRole("T2"))
                        {
                            var hesapdb = db.hesap.FirstOrDefault(x => x.mail == hesap.mail && x.sifre == hesap.sifre);
                            if (hesapdb != null)
                            {
                                FormsAuthentication.SetAuthCookie(hesap.mail, false);
                                    return RedirectToAction("Create", "usta");
                            }

                        }
                        return RedirectToAction("Index", "hesap");
                    }

                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                            }
                        }
                    }
                }
                TempData["Mesaj"] = "Girdiğiniz bilgiler hatalıdır!!";
                return RedirectToAction("Create", "hesap");
            //}

            //ViewBag.typeID = new SelectList(db.hesapType, "ID", "type", hesap.typeID);
            //return View(hesap);
        }

        // GET: hesap/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hesap hesap = db.hesap.Find(id);
            if (hesap == null)
            {
                return HttpNotFound();
            }
            ViewBag.typeID = new SelectList(db.hesapType, "ID", "type", hesap.typeID);
            return View(hesap);
        }

        // POST: hesap/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,mail,sifre,fotograf,typeID,rol")] hesap hesap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hesap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.typeID = new SelectList(db.hesapType, "ID", "type", hesap.typeID);
            return View(hesap);
        }

        // GET: hesap/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hesap hesap = db.hesap.Find(id);
            if (hesap == null)
            {
                return HttpNotFound();
            }
            return View(hesap);
        }

        // POST: hesap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            hesap hesap = db.hesap.Find(id);
            db.hesap.Remove(hesap);
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

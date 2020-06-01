using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using bitirme.Entity;

namespace bitirme.Controllers
{
    public class SecurityController : Controller
    {
        private webinsaatEntities db = new webinsaatEntities();

        // GET: Security
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index","home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(hesap hesap,string ReturnUrl)
        {
            var hesapdb = db.hesap.FirstOrDefault(x => x.mail == hesap.mail && x.sifre == hesap.sifre);
            if (hesapdb!=null)
            {
                FormsAuthentication.SetAuthCookie(hesap.mail, false);
                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }

                return RedirectToAction("Index","Home");
            }
            else
            {
                ViewBag.Mesaj = "Girdiğiniz bilgiler hatalıdır!!";
                return View();
            }
            
        }


        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "home");
            }
            return View();
        }
        


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
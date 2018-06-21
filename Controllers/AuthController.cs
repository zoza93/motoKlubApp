using MotoKlubASP.Models;
using MotoKlubASP.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using wcfmotoklub;

namespace MotoKlubASP.Controllers
{
    //[Authorize(Users="")] //odredjuje ko moze da udje, izvlaci name iz cookie
    [AllowAnonymous]
    public class AuthController : Controller
    {
       // Service1Client wcf = new Service1Client();
        // GET: Auth
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Service1Client wcf = new Service1Client())
            {
                tblMember m;
                var logins = wcf.GetLogin(model.KORISNIK, model.ŠIFRA);
                if (logins != null)
                {
                    m = wcf.GetOneMemberWithLoginID(logins.LoginID);

                    Session["MemberSession"] = m;
                    Session["Admin"] = false;
                    if (m.PrivilageID == 5)
                    {
                        Session["Admin"] = true;
                    }

                    var identity = new ClaimsIdentity(new[]{
                            new Claim(ClaimTypes.Name,m.Name),
                            new Claim(ClaimTypes.Surname, m.Surname),
                            new Claim(ClaimTypes.Country, m.City)
                        },
                        "ApplicationCookie");

                    var ctx = Request.GetOwinContext();
                    var authManager = ctx.Authentication;
                    authManager.SignIn(identity);
                    return RedirectToAction("Index", "Home", new { MemberID = m.MemberID });

                }
                else
                {
                    TempData["Success"] = "Neispravan unos";
                }
            }

            ModelState.AddModelError("", "Neispravno korisnicko ime ili šifra");
            return View(model);
        }


        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login", "Auth");
        }
    }
}
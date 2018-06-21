using MotoKlubASP.Models;
using MotoKlubASP.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wcfmotoklub;

namespace MotoKlubASP.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Edit(int? LoginID)
        {
            tblLogin tbllogin;
            if (LoginID == null)
            {
                return HttpNotFound();
            }
            else
            {
                using (Service1Client wcf = new Service1Client())
                {
                    tbllogin = wcf.GetOneLogin(LoginID);
                }
            }
            return View(tbllogin);
        }
        // POST: Smer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LoginID,Username,Password")] tblLogin tbllogin)
        {
            if (ModelState.IsValid)
            {
                using (Service1Client wcf = new Service1Client())
                {
                    wcf.AddLogin(tbllogin);
                }
                return RedirectToAction("Index", "Home");
            }
            return View(tbllogin);
        }

        public ActionResult Add()
        {
            //dodaj nalog
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "Username, Password")] tblLogin tbllogin)
        {
            tbllogin.LoginID = 0;
            using (Service1Client wcf = new Service1Client())
            {
                wcf.AddLogin(tbllogin);
            }
            return RedirectToAction("Add2", "Login");
        }

        public ActionResult Add2()
        {
            //dodeli
            List<tblMember> allMembers = new List<tblMember>();
            List<tblMember> MembersWithoutLogin = new List<tblMember>();
            List<tblLogin> freeLogins = new List<tblLogin>();
            using (Service1Client wcf = new Service1Client())
            {
                allMembers = wcf.GetMemberList().ToList();
                freeLogins = wcf.GetLoginList().ToList();
                List<tblLogin> l = wcf.GetFreeLogin().ToList();
                freeLogins.RemoveAll(k => l.Any(y => y.LoginID == k.LoginID));
            }
            if (freeLogins.Count == 0)
            {
                TempData["Success"] = "Nema slobodnih LOGIN naloga. Kreirajte novi!";
                return RedirectToAction("Add", "Login");
            }
            else
            {
                foreach (tblMember m in allMembers)
                {
                    if (m.LoginID == null)
                    {
                        MembersWithoutLogin.Add(m);
                    }
                }
                ViewBag.LoginID = freeLogins.Select(x => new SelectListItem { Text = x.Username + " " + x.Password, Value = x.LoginID.ToString() }).ToList();
                ViewBag.MemberID = MembersWithoutLogin.Select(x => new SelectListItem { Text = x.Name + " " + x.Surname, Value = x.MemberID.ToString() }).ToList();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add2([Bind(Include = "MemberID, LoginID")] tblMember tblmember)
        {
            tblMember mem;
            using (Service1Client wcf = new Service1Client())
            {
                mem = wcf.GetOneMember(tblmember.MemberID);
                mem.LoginID = tblmember.LoginID;
                wcf.AddMemebr(mem);
            }
            return RedirectToAction("Index", "MemberList");
        }

        public ActionResult ShowAll(string search)
        {
            List<tblLogin> list;
            List<LoginWithOwner> list2 = new List<LoginWithOwner>();
            using (Service1Client wcf = new Service1Client())
            {
                list = wcf.GetLoginList().ToList();
            }
            LoginWithOwner lwo;
            tblMember mem;
            foreach (tblLogin l in list)
            {
                lwo = new LoginWithOwner();
                lwo.LoginID = l.LoginID;
                lwo.Username = l.Username;
                lwo.Password = l.Password;
                using (Service1Client wcf = new Service1Client())
                {
                    mem = new tblMember();
                    mem = wcf.GetOneMemberWithLoginID(l.LoginID);
                    if (mem != null)
                    {
                        lwo.Name = mem.Name;
                        lwo.Surname = mem.Surname;
                    }
                    else
                    {
                        lwo.Name = "";
                        lwo.Surname = "";
                    }
                }
                list2.Add(lwo);
            }
            if (search != null)
            {
                return View(list2.Where(x => (x.Username).ToLower().Contains(search.ToLower()) || (x.Name).ToLower().Contains(search.ToLower()) || (x.Surname).ToLower().Contains(search.ToLower())).ToList());
            }
            else
            {
                return View(list2);
            }
        }

        public ActionResult Delete(int LoginID)
        {
            tblLogin login = new tblLogin();
            using (Service1Client wcf = new Service1Client())
            {
                login = wcf.GetOneLogin(LoginID);
            }

            return View();
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int LoginID)
        {
            
            using (Service1Client wcf = new Service1Client())
            {
                wcf.DeleteLogin(LoginID);
            }
            return RedirectToAction("ShowAll", "Login");
        }
    }
}
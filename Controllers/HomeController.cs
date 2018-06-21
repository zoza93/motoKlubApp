using MotoKlubASP.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wcfmotoklub;

namespace MotoKlubASP.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {

            var x = Session["MemberSession"] as tblMember;
            List<tblMember> my = new List<tblMember>();
            my.Add(x);
            string privilage;
            string status;
            using (Service1Client wcf = new Service1Client())
            {
                var p = wcf.GetOnePrivilage(x.PrivilageID);
                if (p != null)
                {
                    privilage = p.TypeOfPrivilage;
                    ViewBag.PrivilageID = privilage;
                }
                else
                    ViewBag.PrivilageID = "NEMA";
                var s = wcf.GetOneStatus(x.StatusID);
                if (s != null)
                {
                    status = s.StatusType;
                    ViewBag.StatusID = status;
                }
                else
                    ViewBag.StatusID = "NEMA";
                if (!wcf.HaveBike(x.MemberID))
                    ViewBag.Bike = "NE";
                else
                    ViewBag.Bike = "DA";
            }
            return View(my);
        }

        public ActionResult Edit()
        {
            var x = Session["MemberSession"] as tblMember;
            return View(x);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MemberID, Name, Surname, Nickname, DateBirth, JMBG, Phone, Address, City, Number, Email, StatusID, LoginID, PrivilageID")] tblMember tblMember)
        {
            if (ModelState.IsValid)
            {
                tblMember memOld = Session["MemberSession"] as tblMember;
                tblMember.Name = memOld.Name;
                tblMember.Number = memOld.Number;
                using (Service1Client wcf = new Service1Client())
                {
                    wcf.AddMemebr(tblMember);
                }

                Session["MemberSession"] = tblMember;
                return RedirectToAction("Index");
            }
            return View(tblMember);
        }
    }
}
using MotoKlubASP.Models;
using MotoKlubASP.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using wcfmotoklub;

namespace MotoKlubASP.Controllers
{
    public class BikeController : Controller
    {
        // GET: Bike
        //ispis svih motora u bazi
        public ActionResult Index(string search)
        {
            List<tblBike> allBikes = new List<tblBike>();

            using (Service1Client wcf = new Service1Client())
            {
                allBikes = wcf.GetBikeList().ToList();
            }
            if (allBikes == null)
            {
                TempData["Success"] = "U bazi podataka ne postoji ni jedan motor, mozete ga dodati";
                return RedirectToAction("Create", "Bike");
            }
            else
            {
                List<BikesAndMembers> list = new List<BikesAndMembers>();
                BikesAndMembers bandm;
                foreach (tblBike b in allBikes)
                {
                    bandm = new BikesAndMembers();
                    bandm.BikeID = b.BikeID;
                    if (b.Brand == null)
                        bandm.Brand = "";
                    else
                        bandm.Brand = b.Brand.ToUpper();
                    if (b.Model == null)
                        bandm.Model = "";
                    else
                        bandm.Model = b.Model.ToUpper();
                    bandm.Year = b.Year;
                    bandm.MemberID = b.MemberID;
                    using (Service1Client wcf = new Service1Client())
                    {
                        if (b.MemberID != null)
                        {
                            var member = wcf.GetOneMember((int)b.MemberID);
                            bandm.Name = member.Name.ToUpper();
                            bandm.Surname = member.Surname.ToUpper();
                            ViewBag.Member = bandm.Name + " " + bandm.Surname;
                        }
                        else
                        {
                            TempData["Success"] = "Motor nema vlasnika";
                        }
                    }
                    list.Add(bandm);

                }
                if (search != null)
                {
                    return View(list.Where(x => (x.Brand.ToUpper()).Contains(search.ToUpper()) ||
                                            (x.Model.ToUpper()).Contains(search.ToUpper()) || 
                                            (x.Name.ToUpper()).Contains(search.ToUpper()) || 
                                            (x.Surname.ToUpper()).Contains(search.ToUpper()) ||
                                            (x.Year.ToString()).Contains(search.ToUpper())).ToList());
                }
                else
                {
                    return View(list);
                }
            }

        }

        //ispis samo jednog motora
        public ActionResult Index2()
        {
            var x = Session["MemberSession"] as tblMember;
            List<tblBike> list = new List<tblBike>();
            using (Service1Client wcf = new Service1Client())
            {
                list = wcf.GetMyBikes(x.MemberID).ToList();
            }
            if (list.Count == 0)
            {
                TempData["Success"] = "U bazi podataka ne postoji vas motor, mozete ga dodati";
                return RedirectToAction("Create2", "Bike");
            }
            else
            {
                ViewBag.Member = x.Name + " " + x.Surname;

                return View(list);
            }
        }

        //kreiranje
        public ActionResult Create()
        {
            using (Service1Client wcf = new Service1Client())
            {
                ViewBag.MemberID = wcf.GetMemberList().Select(x => new SelectListItem { Text = x.Name + " " + x.Surname, Value = x.MemberID.ToString() }).ToList();
            }
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BikeID, Brand, Model, Year, MemberID")] tblBike tblbike)
        {
            using (Service1Client wcf = new Service1Client())
            {
                wcf.AddBike(tblbike);
            }
            return RedirectToAction("Index", "Bike");
        }

        //kreiraj motor kao obican korisnik
        public ActionResult Create2()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2([Bind(Include = "BikeID, Brand, Model, Year, MemberID")] tblBike tblbike)
        {
            var x = Session["MemberSession"] as tblMember;
            tblbike.MemberID = x.MemberID;
            using (Service1Client wcf = new Service1Client())
            {
                wcf.AddBike(tblbike);
            }
            return RedirectToAction("Index2", "Bike");
        }

        //edit
        public ActionResult Edit(int BikeID)
        {
            tblBike tblbike;
            using (Service1Client wcf = new Service1Client())
            {
                tblbike = wcf.GetOneBike(BikeID);
            }
            return View(tblbike);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BikeID, Brand, Model, Year, MemberID")] tblBike tblbike)
        {
            if (ModelState.IsValid)
            {
                using (Service1Client wcf = new Service1Client())
                {
                    wcf.AddBike(tblbike);
                }
                return RedirectToAction("Index", "Bike");
            }
            return View(tblbike);
        }

        //delete
        public ActionResult Delete(int BikeID)
        {
            tblBike tblbike;
            using (Service1Client wcf = new Service1Client())
            {
                tblbike = wcf.GetOneBike(BikeID);
                var x = wcf.GetOneMember((int)tblbike.MemberID);
                ViewBag.MemberID = x.Name + " " + x.Surname;
            }

            if (tblbike == null)
            {
                return HttpNotFound();
            }

            return View(tblbike);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int BikeID)
        {
            using (Service1Client wcf = new Service1Client())
            {
                wcf.DeleteBike(BikeID);
            }
            return RedirectToAction("Index", "Bike");
        }

    }
}
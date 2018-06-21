using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using MotoKlubASP.Models;
using MotoKlubASP.ServiceReference1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wcfmotoklub;

namespace MotoKlubASP.Controllers
{
    public class MemberListController : Controller
    {
        // GET: MemberList
        public ActionResult Index(string search)
        {
            List<tblMember> listOfMember = new List<tblMember>();
            using (Service1Client wcf = new Service1Client())
            {
                listOfMember = wcf.GetMemberList().ToList();
            }
            foreach (tblMember m in listOfMember)
            {
                if (m.Nickname != null)
                    m.Nickname = m.Nickname.ToUpper();
                else
                    m.Nickname = "";
                if (m.Number == null)
                    m.Number = "";
                if (m.Phone == null)
                    m.Phone = "";
                if (m.Name == null)
                    m.Name = "";
                if (m.Surname == null)
                    m.Surname = "";
            }
            if (search != null)
            {
                return View(listOfMember.Where(x => (x.Nickname).Contains(search.ToUpper()) ||
                                            (x.Number.ToUpper()).Contains(search.ToUpper()) || 
                                            (x.Name.ToUpper()).Contains(search.ToUpper()) || 
                                            (x.Surname.ToUpper()).Contains(search.ToUpper()) || 
                                            (x.Phone).Contains(search.ToUpper())).ToList());
            }
            else
            {
                return View(listOfMember);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string GridHtml)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "Grid.pdf");
            }
        }

        public ActionResult Details(int id)
        {
            bool haveBike;
            Members member = new Members();
            tblBike bike = null;
            using (Service1Client wcf = new Service1Client())
            {
                tblMember tblMember = wcf.GetOneMember(id);
                // List<tblPrivilage> list = new List<tblPrivilage>();
                var listPrivilage = wcf.GetOnePrivilage(tblMember.PrivilageID);
                var listStatus = wcf.GetOneStatus(tblMember.StatusID);
                member.MemberID = tblMember.MemberID;
                member.Name = tblMember.Name;
                member.Surname = tblMember.Surname;
                member.Nickname = tblMember.Nickname;
                member.DateBirth = tblMember.DateBirth;
                member.JMBG = tblMember.JMBG;
                member.Phone = tblMember.Phone;
                member.Address = tblMember.Address;
                member.City = tblMember.City;
                member.Number = tblMember.Number;
                member.Email = tblMember.Email;
                member.StatusID = tblMember.StatusID;
                member.LoginID = tblMember.LoginID;
                member.PrivilageID = tblMember.PrivilageID;
                if (listPrivilage == null)
                {
                    member.TypeOfPrivilage = "";
                }
                else
                {
                    member.TypeOfPrivilage = listPrivilage.TypeOfPrivilage;
                }
                if (listStatus == null)
                {
                    member.StatusType = "";
                }
                else
                {
                    member.StatusType = listStatus.StatusType;
                }
                if (wcf.GetMyBikes(id).Count() == 0)
                    haveBike = false;
                else
                {
                    bike = wcf.GetMyBikes(id).FirstOrDefault();
                    haveBike = true;
                }
            }

            if (member == null)
            {
                return HttpNotFound();
            }

            if (haveBike)
                ViewBag.Bike = bike.Brand + " - " + bike.Model;
            else
                ViewBag.Bike = "NEMA";

            return View(member);
        }

        public ActionResult Edit(int id)
        {
            tblMember tblmember;
            using (Service1Client wcf = new Service1Client())
            {
                tblmember = wcf.GetOneMember(id);
                List<tblPrivilage> listP = new List<tblPrivilage>();
                var priv = wcf.GetPrivilageList();
                tblPrivilage pri = wcf.GetOnePrivilage(tblmember.PrivilageID);
                listP.Add(pri); //dodaje za pocetnu vrednost korisnikovu trenutnu privilegiju
                if (pri != null) //ukoliko trenutna nije null dodaje i opciju null
                    listP.Add(null);
                foreach (tblPrivilage p in priv)
                {
                    if (pri != null)
                    {
                        if (p.PrivilageID != pri.PrivilageID) //dodaje sve ostale
                            listP.Add(p);
                    }
                    else
                        listP.Add(p);

                }
                ViewBag.PrivilageID = new SelectList(listP, "PrivilageID", "TypeOfPrivilage");

                tblStatu statusNow = wcf.GetOneStatus(tblmember.StatusID);
                List<tblStatu> statuss = new List<tblStatu>();
                statuss.Add(statusNow);
                var allStatus = wcf.GetStatusList();
                foreach (tblStatu s in allStatus)
                {
                    if (statusNow != null)
                    {
                        if (s.StatusID != statusNow.StatusID)
                            statuss.Add(s);
                    }
                    else
                        statuss.Add(s);
                }
                ViewBag.StatusID = new SelectList(statuss, "StatusID", "StatusType");
            }
            if (tblmember == null)
            {
                return HttpNotFound();
            }

            return View(tblmember);
        }
        // POST: Smer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MemberID, Name, Surname, Nickname, DateBirth, JMBG, Phone, Address, City, Number, Email, StatusID, LoginID, PrivilageID")] tblMember tblmember)
        {
            if (ModelState.IsValid)
            {
                tblMember memOld = tblmember;
                using (Service1Client wcf = new Service1Client())
                {
                    wcf.AddMemebr(tblmember);
                }
                return RedirectToAction("Index", "MemberList");
            }
            return View(tblmember);
        }

        //create memeber (ADMIN)
        public ActionResult Create()
        {
            List<tblStatu> statuss = new List<tblStatu>();
            List<tblPrivilage> allPrivilage = new List<tblPrivilage>();
            allPrivilage.Add(null);
            using (Service1Client wcf = new Service1Client())
            {
                statuss = wcf.GetStatusList().ToList();
                var priv = wcf.GetPrivilageList();
                foreach (tblPrivilage p in priv)
                {
                    allPrivilage.Add(p);
                }
            }
            ViewBag.StatusID = new SelectList(statuss, "StatusID", "StatusType");
            ViewBag.PrivilageID = new SelectList(allPrivilage, "PrivilageID", "TypeOfPrivilage");

            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MemberID, Name, Surname, Nickname, DateBirth, JMBG, Phone, Address, City, Number, Email, StatusID, LoginID, PrivilageID")] tblMember tblmember)
        {
            using (Service1Client wcf = new Service1Client())
            {
                wcf.AddMemebr(tblmember);
            }
            return RedirectToAction("Index", "MemberList");
        }

    }
}
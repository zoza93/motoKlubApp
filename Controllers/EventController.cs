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
    public class EventController : Controller
    {
        // GET: Event
        //all events
        public ActionResult Index(string search)
        {
            List<tblMember> memberList = new List<tblMember>();
            using (Service1Client wcf = new Service1Client())
            {
                memberList = wcf.GetMemberList().ToList();
            }
            if (search != null)
            {
                return View(memberList.Where(x => (x.Name.ToUpper()).Contains(search.ToUpper()) ||
                                            (x.Surname.ToUpper()).Contains(search.ToUpper()) ||
                                            (x.Number.ToUpper()).Contains(search.ToUpper())).ToList());
            }
            else
            {
                return View(memberList);
            }
            
        }

        public ActionResult GetEvent(int MemberID)
        {
            List<EventWithType> events = new List<EventWithType>();
            EventWithType ewtype;
            using (Service1Client wcf = new Service1Client())
            {
                var myEvents = wcf.GetMyEventList(MemberID);

                foreach (tblEvent e in myEvents)
                {
                    ewtype = new EventWithType();
                    ewtype.EventID = e.EventID;
                    ewtype.Date = e.Date;
                    ewtype.EventName = e.EventName;
                    ewtype.TypeOfEventID = e.TypeOfEventID;
                    ewtype.EventType = wcf.GetOneTypeOfEvent((int)e.TypeOfEventID).EventType;
                    events.Add(ewtype);
                }
            }
            Session["AddEventForMember"] = MemberID;
            if (events.Count == 0)
            {
                if ((bool)Session["Admin"])
                {

                    return RedirectToAction("Create", "Event");

                }
                TempData["Success"] = "Za ovu osobu nema podataka u bazi";
                return RedirectToAction("Index", "Event");
            }
            return View(events);
        }

        //mylist
        public ActionResult Index2()
        {
            var x = Session["MemberSession"] as tblMember;
            List<EventWithType> events = new List<EventWithType>();
            EventWithType ewtype;
            using (Service1Client wcf = new Service1Client())
            {
                var myEvents = wcf.GetMyEventList(x.MemberID);

                foreach (tblEvent e in myEvents)
                {
                    ewtype = new EventWithType();
                    ewtype.EventID = e.EventID;
                    ewtype.Date = e.Date;
                    ewtype.EventName = e.EventName;
                    ewtype.TypeOfEventID = e.TypeOfEventID;
                    ewtype.EventType = wcf.GetOneTypeOfEvent((int)e.TypeOfEventID).EventType;
                    events.Add(ewtype);
                }
            }
            if (events.Count == 0)
            {
                TempData["Success"] = "U bazi podataka ne postoje dogadjaji na kojima ste prisustvovali";
                return RedirectToAction("Create2", "Event");
            }
            return View(events);
        }
        public ActionResult DeleteMemEve(int EventID)
        {
            if (EventID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblMemberEvent tblmemberevent;
            tblEvent tblevent;
            var x = Session["MemberSession"] as tblMember;
            using (Service1Client wcf = new Service1Client())
            {
                tblmemberevent = wcf.GetOneMemberEvent(EventID, x.MemberID);
                tblevent = wcf.GetOneEvent(EventID);
            }

            if (tblmemberevent == null)
            {
                return HttpNotFound();
            }

            ViewBag.EventName = tblevent.EventName;
            ViewBag.EventDate = tblevent.Date.ToString();
            ViewBag.MemberName = x.Name + " " + x.Surname;
            return View(tblmemberevent);
        }
        [HttpPost, ActionName("DeleteMemEve")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMemEveConfirmed(int EventID)
        {
            var x = Session["MemberSession"] as tblMember;
            using (Service1Client wcf = new Service1Client())
            {
                wcf.DeleteEventMember(EventID, x.MemberID);
            }
            return RedirectToAction("Index2", "Event");
        }

        public ActionResult Create()
        {
            int MemberID = (int)Session["AddEventForMember"];
            tblMember member;
            List<tblEvent> listevent;
            using (Service1Client wcf = new Service1Client())
            {
                member = wcf.GetOneMember(MemberID);
                listevent = new List<tblEvent>();
                listevent = wcf.GetEventList().ToList();
                List<tblEvent> my = wcf.GetMyEventList(MemberID).ToList();
                listevent.RemoveAll(k => my.Any(y => y.EventID == k.EventID));

            }
            ViewBag.MemberID = member.Name + " " + member.Surname;
            ViewBag.EventID = new SelectList(listevent, "EventID", "EventName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID")] tblMemberEvent tblmemeve)
        {
            tblmemeve.MemberID = (int)Session["AddEventForMember"];
            tblmemeve.Comment = null;
            using (Service1Client wcf = new Service1Client())
            {
                wcf.AddMemberEvent(tblmemeve);
            }
            Session["AddEventForMember"] = null;
            return RedirectToAction("Index", "Event");
        }

        public ActionResult Create2()
        {
            var x = Session["MemberSession"] as tblMember;
            ViewBag.MemberID = x.Name + " " + x.Surname;
            List<tblEvent> listEvent;
            using (Service1Client wcf = new Service1Client())
            {
                listEvent = new List<tblEvent>();
                listEvent = wcf.GetEventList().ToList();
                List<tblEvent> my = wcf.GetMyEventList(x.MemberID).ToList();
                listEvent.RemoveAll(k => my.Any(y => y.EventID == k.EventID));

            }
            ViewBag.EventID = new SelectList(listEvent, "EventID", "EventName");
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2([Bind(Include = "EventID")] tblMemberEvent tblmemeve)
        {
            var x = Session["MemberSession"] as tblMember;
            tblmemeve.MemberID = x.MemberID;
            tblmemeve.Comment = null;
            using (Service1Client wcf = new Service1Client())
            {
                wcf.AddMemberEvent(tblmemeve);
            }
            return RedirectToAction("Index2", "Event");
        }

        public ActionResult Maps()
        {
            //getAllCordination i proslediti u view
            List<Coordinate> list = new List<Coordinate>();
            Coordinate coo;
            string[] str = new string[3];
            List<string> list2 = new List<string>();
            using (Service1Client wcf = new Service1Client())
            {
                list2 = wcf.GetCoordinateList().ToList();
            }
            foreach (string s in list2)
            {
                coo = new Coordinate();
                str = s.Split('-');
                try
                {
                    coo.Latitude = float.Parse(str[0]);
                    coo.Longitude = float.Parse(str[1]);
                    coo.Name = str[2];
                    list.Add(coo);
                }
                catch (Exception ex)
                {
                    
                }
                
            }
            return View(list);
        }
    }
}
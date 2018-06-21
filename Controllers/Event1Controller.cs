using MotoKlubASP.Models;
using MotoKlubASP.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wcfmotoklub;

namespace MotoKlubASP.Controllers
{
    public class Event1Controller : Controller
    {
        // GET: Event1
        public ActionResult Index(string search)
        {
            List<EventWithType> events = new List<EventWithType>();
            EventWithType ewtype;
            using (Service1Client wcf = new Service1Client())
            {
                var myEvents = wcf.GetEventList();

                foreach (tblEvent e in myEvents)
                {
                    ewtype = new EventWithType();
                    ewtype.EventID = e.EventID;
                    ewtype.Date = e.Date;
                    ewtype.EventName = e.EventName;
                    ewtype.Location = e.Location;
                    ewtype.TypeOfEventID = e.TypeOfEventID;
                    if (e.TypeOfEventID != 0)
                        ewtype.EventType = wcf.GetOneTypeOfEvent((int)e.TypeOfEventID).EventType;
                    else
                        ewtype.EventType = "Nedefinisano";
                    ewtype.NumberOfMember = wcf.GetNumberOfMemberOnSpecificEvent(e.EventID);
                    events.Add(ewtype);
                }
            }
            if (events.Count == 0)
            {
                TempData["Success"] = "U bazi podataka ne postoje dogadjaji";
                return RedirectToAction("Create", "Event1");
            }
            foreach (EventWithType e in events)
            {
                if (e.EventName != null)
                    e.EventName = e.EventName.ToUpper();
                else
                    e.EventName = "";
                if (e.Location != null)
                    e.Location = e.Location.ToUpper();
                else
                    e.Location = "";
                if (e.EventType != null)
                    e.EventType = e.EventType.ToUpper();
                else
                    e.EventType = "";
            }
            if (search != null)
            {
                return View(events.Where(x => (x.EventName).Contains(search.ToUpper()) ||
                                            (x.Location).Contains(search.ToUpper()) ||
                                            (x.EventType).Contains(search.ToUpper()) ||
                                            (x.NumberOfMember.ToString()).Contains(search.ToUpper())||
                                            (x.Date.ToString()).Contains(search.ToUpper())).ToList());
            }
            else
            {
                return View(events);
            }
        }

        public ActionResult Create()
        {
            List<tblTypeOfEvent> listtypeofevent;
            using (Service1Client wcf = new Service1Client())
            {
                listtypeofevent = new List<tblTypeOfEvent>();
                listtypeofevent = wcf.GetTypeList().ToList();
            }
            ViewBag.TypeOfEventID = new SelectList(listtypeofevent, "TypeOfEventID", "EventType");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventName, Date, TypeOfEventID, Location, Latitude, Longitude")] tblEvent tblEvent)
        {

            using (Service1Client wcf = new Service1Client())
            {
                wcf.AddEvent(tblEvent);
            }
            return RedirectToAction("Index", "Event1");
        }

        //edit
        public ActionResult Edit(int EventID)
        {
            tblEvent tblevent;
            List<tblTypeOfEvent> listtypeofevent = new List<tblTypeOfEvent>();
            using (Service1Client wcf = new Service1Client())
            {
                tblevent = wcf.GetOneEvent(EventID);
                listtypeofevent.Add(wcf.GetOneTypeOfEvent((int)tblevent.TypeOfEventID));
                var x = wcf.GetTypeList().ToList();
                foreach (tblTypeOfEvent t in x)
                {
                    if (t.TypeOfEventID != tblevent.TypeOfEventID)
                        listtypeofevent.Add(t);
                }
            }

            ViewBag.TypeOfEventID = new SelectList(listtypeofevent, "TypeOfEventID", "EventType");
            return View(tblevent);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID, EventName, Date, TypeOfEventID, Location, Latitude, Longitude")] tblEvent tblevent)
        {
            if (ModelState.IsValid)
            {
                using (Service1Client wcf = new Service1Client())
                {
                    wcf.AddEvent(tblevent);
                }
                return RedirectToAction("Index", "Event1");
            }
            return View(tblevent);
        }

        //delete
        public ActionResult Delete(int EventID)
        {
            tblEvent tblevent;
            using (Service1Client wcf = new Service1Client())
            {
                tblevent = wcf.GetOneEvent(EventID);
                var x = wcf.GetOneTypeOfEvent((int)tblevent.TypeOfEventID);
                ViewBag.EventType = x.EventType;
            }

            if (tblevent == null)
            {
                return HttpNotFound();
            }

            return View(tblevent);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int EventID)
        {
            using (Service1Client wcf = new Service1Client())
            {
                wcf.DeleteEvent(EventID);
            }
            return RedirectToAction("Index", "Event1");
        }

        public ActionResult Participants(int EventID)
        {
            List<tblMember> list;
            using (Service1Client wcf = new Service1Client())
            {
                list = new List<tblMember>();
                list = wcf.GetMemberOnEvent(EventID).ToList();
            }
            return View(list);
        }
    }
}
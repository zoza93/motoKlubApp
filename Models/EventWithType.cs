using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MotoKlubASP.Models
{
    public class EventWithType
    {
        public int EventID { get; set; }
        public DateTime? Date { get; set; }
        public string EventName { get; set; }
        public int? TypeOfEventID { get; set; }
        public string Location { get; set; }
        public string EventType { get; set; }
        public int? NumberOfMember { get; set; }
    }
}
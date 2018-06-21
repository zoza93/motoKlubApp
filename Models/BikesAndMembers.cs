using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MotoKlubASP.Models
{
    public class BikesAndMembers
    {
        public int BikeID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int? Year { get; set; }
        public int? MemberID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MotoKlubASP.Models
{
    public class Members
    {
        public int MemberID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Nickname { get; set; }
        public DateTime? DateBirth { get; set; }
        public string JMBG { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Number { get; set; }
        public string Email { get; set; }
        public int? StatusID { get; set; }
        public string StatusType { get; set; }
        public int? LoginID { get; set; }
        public int? PrivilageID { get; set; }
        public string TypeOfPrivilage { get; set; }
    }
}
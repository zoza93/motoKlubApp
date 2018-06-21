using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MotoKlubASP.Models
{
    public class LoginWithOwner
    {
        public int LoginID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
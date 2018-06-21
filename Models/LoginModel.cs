using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MotoKlubASP.Models
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string KORISNIK { get; set; }
 
        [Required]
        [DataType(DataType.Password)]
        public string ŠIFRA { get; set; }
 
        //[HiddenInput(DisplayValue=false)]
        //public string ReturnUrl { get; set; }

    }
}
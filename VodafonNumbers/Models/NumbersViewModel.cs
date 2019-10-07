using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafon.Models
{
    public class NumbersViewModel
    {
        public int ID { get; set; }
        public string No { get; set; }
        public bool IsChoosen { get; set; }
        public bool IsPrivate { get; set; }
        public string UserID { get; set; }
    }
}
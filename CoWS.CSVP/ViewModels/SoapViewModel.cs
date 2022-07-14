using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoWS.PayrollVouchers.ViewModels
{
    public class SoapViewModel
    {
        public string Request { get; set; }
        public string Response { get; set; }
        public DateTime DatePosted { get; set; }
        public string FileDescription { get; set; }
        public DateTime FileDate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoWS.PayrollVouchers.ViewModels
{
    public class WebServiceSettingsViewModel
    {
        [Key]
        public int ID { get; set; }
        public string FMSUser { get; set; }
        public string FMSPassword1 { get; set; }
        public string FMSPassword2 { get; set; }
        public string FMSPassword3 { get; set; }
        public string Ledger { get; set; }
        public string OSUser { get; set; }
        public string OSPassword { get; set; }
        public string OutputDevice { get; set; }
        public string WebServiceURL { get; set; }
        public DateTime DateModified { get; set; }
        public string WhoModified { get; set; }

    }
}
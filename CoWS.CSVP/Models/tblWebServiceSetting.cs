//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CoWS.PayrollVouchers.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblWebServiceSetting
    {
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
        public Nullable<System.DateTime> DateModified { get; set; }
        public string WhoModified { get; set; }
    }
}

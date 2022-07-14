using System;
using System.ComponentModel.DataAnnotations;

namespace CoWS.PayrollVouchers.ViewModels
{
    public class GeneralSettingsViewModel
    {
        [Key]
        public int ID { get; set; }
        public string BankNumber { get; set; }
        public string ChildSupportDesc { get; set; }
        public string FilePath { get; set; }
        public string UserCode { get; set; }
        public string VouchersPayableDesc { get; set; }
        public string TempPassword { get; set; }
        public DateTime DateModified { get; set; }
        public string WhoModified { get; set; }
    }
}
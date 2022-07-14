using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoWS.PayrollVouchers.ViewModels
{
    public class GLTransactionDetailViewModel
    {
        [Key]
        [Display(Name = "Transaction ID")]
        public int TransactionID { get; set; }
        [Display(Name = "Transaction Amount")]
        public decimal TransactionAmount { get; set; }
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }
        [Display(Name = "Comments")]
        public string Comments { get; set; }
        [Display(Name = "Fund")]
        public string Fund { get; set; }
        [Display(Name = "Resp Code")]
        public string ResponsibilityCode { get; set; }
        [Display(Name = "Obj Code")]
        public string ObjectCode { get; set; }
        [Display(Name = "Prog Code")]
        public string ProgramCode { get; set; }
        public int GLID { get; set; }
        public DateTime DateModified { get; set; }
        public string WhoModified { get; set; }
    }
}
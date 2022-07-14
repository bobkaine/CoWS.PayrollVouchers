using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoWS.PayrollVouchers.ViewModels
{
    public class GLTDViewModel
    {
        [Key]
        [Display(Name = "ID")]
        public int GLID { get; set; }
        [Display(Name = "GL Type")]
        public string GLType { get; set; }
        [Display(Name = "Batch #")]
        public string BatchNumber { get; set; }
        [Display(Name = "Batch Type")]
        public string BatchType { get; set; }
        [Display(Name = "Batch Total")]
        public decimal BatchTotal { get; set; }
        [Display(Name = "Comments")]
        public string GLComments { get; set; }
        [Display(Name = "Journal Comments")]
        public string JournalComments { get; set; }
        public string Ledger { get; set; }
        [Display(Name = "Fiscal Period")]
        public string Period { get; set; }
        [Display(Name = "Fiscal Year")]
        public string Year { get; set; }
        [Display(Name = "User Code")]
        public string UserCode { get; set; }

        [Display(Name = "Transaction ID")]
        public int TransactionID { get; set; }
        [Display(Name = "Transaction Amount")]
        public decimal TransactionAmount { get; set; }
        [Display(Name = "Transaction Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
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
        public DateTime DateModified { get; set; }
        public string WhoModified { get; set; }

    }
}
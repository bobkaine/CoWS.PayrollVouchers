using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoWS.PayrollVouchers.ViewModels
{
    public class GeneralLedgerViewModel
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
        public DateTime DateModified { get; set; }
        public string WhoModified { get; set; }

        public GeneralLedgerViewModel()
        {
            this.TransactionDetails = new List<GLTransactionDetailViewModel>();
        }

        public List<GLTransactionDetailViewModel> TransactionDetails { get; set; }
    }
}
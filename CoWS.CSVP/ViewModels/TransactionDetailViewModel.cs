using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoWS.PayrollVouchers.ViewModels
{
    public class TransactionDetailViewModel
    {
        [Display(Name = "Transaction ID")]
        public int TransactionID { get; set; }
        [Display(Name = "Trans Code")]
        public string TransCode { get; set; }
        [Display(Name = "Check Create Date")]
        public string CreationDate { get; set; }
        [Display(Name = "Check Amount")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal CheckAmount { get; set; }
        [Display(Name = "Resp Code")]
        public string ResponsibilityCode { get; set; }
        [Display(Name = "Obj Code")]
        public string ObjectCode { get; set; }
        [Display(Name = "Prog Code")]
        public string ProgramCode { get; set; }
        [Display(Name = "Line Item Description")]
        public string LineItemDescription { get; set; }
        [Display(Name = "Bank Number")]
        public string BankNumber { get; set; }
        public int DocumentID { get; set; }
        public DateTime DateModified { get; set; }
        public string WhoModified { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoWS.PayrollVouchers.ViewModels
{
    public class BatchViewModel
    {
        [Key]
        [Display(Name = "Batch Header ID")]
        public int BatchHeaderID { get; set; }
        [Display(Name = "Batch ID")]
        public string BatchID { get; set; }
        [Display(Name = "Batch Type")]
        public string BatchType { get; set; }
        [Display(Name = "Batch Total")]
        public decimal BatchTotal { get; set; }
        [Display(Name = "File Description")]
        public string FileDescription1 { get; set; }
        [Display(Name = "File Description")]
        public string FileDescription2 { get; set; }
        [Display(Name = "User Code")]
        public string UserCode { get; set; }
        [Display(Name = "Bank Number")]
        public string BankNumber { get; set; }
        public string Ledger { get; set; }
        [Display(Name = "Fiscal Period")]
        public string Period { get; set; }
        [Display(Name = "Fiscal Year")]
        public string Year { get; set; }
        public DateTime DateModified { get; set; }
        public string WhoModified { get; set; }
        public string APApprover { get; set; }
        public string GLApprover { get; set; }
        public string FinalApprover { get; set; }
        public BatchViewModel()
        {
            this.DocumentNumbers = new List<DocumentNumberViewModel>();
        }

        public List<DocumentNumberViewModel> DocumentNumbers { get; set; }


    }
}
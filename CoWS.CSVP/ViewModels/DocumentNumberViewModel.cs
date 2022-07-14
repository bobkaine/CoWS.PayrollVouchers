using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoWS.PayrollVouchers.ViewModels
{
    public class DocumentNumberViewModel
    {
        [Display(Name = "Doc ID")]
        public int DocumentID { get; set; }
        [Display(Name = "Doc Type")]
        public string DocumentType { get; set; }
        [Display(Name = "Pay Period End Date")]
        public string PayPeriodEndingDate { get; set; }
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }
        [Display(Name = "Vendor Number")]
        public string VendorNumber { get; set; }
        [Display(Name = "User Code")]
        public string UserCode { get; set; }
        [Display(Name = "Doc Create Date")]
        public string DocumentCreationDate { get; set; }
        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; }
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; }
        [Display(Name = "Check Description")]
        public string CheckDescription { get; set; }
        [Display(Name = "Bank Number")]
        public string BankNumber { get; set; }
        public int BatchHeaderID { get; set; }
        public DateTime DateModified { get; set; }
        public string WhoModified { get; set; }
        public DocumentNumberViewModel()
        {
            this.TransactionDetails = new List<TransactionDetailViewModel>();
        }

        public List<TransactionDetailViewModel> TransactionDetails { get; set; }
    }
}
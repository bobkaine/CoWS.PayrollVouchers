using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoWS.PayrollVouchers.ViewModels
{
    public class BatchDocTransViewModel
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
        public string Ledger { get; set; }
        [Display(Name = "Fiscal Period")]
        public string Period { get; set; }
        [Display(Name = "Fiscal Year")]
        public string Year { get; set; }

        // Document Number
        [Display(Name = "Doc ID")]
        public int DocumentID { get; set; }
        [Display(Name = "Doc Type")]
        [Required]
        public string DocumentType { get; set; }
        [Display(Name = "Pay Period End Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Required]
        public DateTime PayPeriodEndingDate { get; set; }
        [Display(Name = "Vendor Name")]
        [Required]
        public string VendorName { get; set; }
        [Display(Name = "Vendor #")]
        [Required]
        public string VendorNumber { get; set; }
        [Display(Name = "Doc Create Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Required]
        public DateTime DocumentCreationDate { get; set; }
        [Display(Name = "Pmt Type")]
        [Required]
        public string PaymentType { get; set; }
        [Display(Name = "Pmt Status")]
        [Required]
        public string PaymentStatus { get; set; }
        [Display(Name = "Check Description")]
        [Required]
        public string CheckDescription { get; set; }

        // Transaction Detail
        [Display(Name = "Transaction ID")]
        public int TransactionID { get; set; }
        [Required]
        [Display(Name = "Trans Code")]
        public string TransCode { get; set; }
        [Display(Name = "Check Create Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Required]
        public DateTime CheckCreationDate { get; set; }
        [Display(Name = "Check Amount")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        [Required]
        public decimal CheckAmount { get; set; }
        [Display(Name = "Resp Code")]
        [Required]
        public string ResponsibilityCode { get; set; }
        [Display(Name = "Obj Code")]
        [Required]
        public string ObjectCode { get; set; }
        [Display(Name = "Prog Code")]
        [Required]
        public string ProgramCode { get; set; }
        [Display(Name = "Line Item Description")]
        [Required]
        public string LineItemDescription { get; set; }

        // Common Properties
        [Display(Name = "Bank Number")]
        [Required]
        public string BankNumber { get; set; }

        [Display(Name = "User Code")]
        [Required]
        public string UserCode { get; set; }

        public DateTime DateModified { get; set; }

        public string WhoModified { get; set; }
        public string APApprover { get; set; }
        public string GLApprover { get; set; }
        public string FinalApprover { get; set; }

    }
}
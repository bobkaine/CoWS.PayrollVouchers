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
    
    public partial class tblTransactionDetailAudit
    {
        public int AuditID { get; set; }
        public int TransactionID { get; set; }
        public string TransCode { get; set; }
        public System.DateTime CreationDate { get; set; }
        public decimal CheckAmount { get; set; }
        public string ResponsibilityCode { get; set; }
        public string ObjectCode { get; set; }
        public string ProgramCode { get; set; }
        public string LineItemDescription { get; set; }
        public string BankNumber { get; set; }
        public int DocumentID { get; set; }
        public System.DateTime DateModified { get; set; }
        public string WhoModified { get; set; }
    }
}
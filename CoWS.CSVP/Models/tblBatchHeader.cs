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
    
    public partial class tblBatchHeader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblBatchHeader()
        {
            this.tblDocumentNumbers = new HashSet<tblDocumentNumber>();
        }
    
        public int BatchHeaderID { get; set; }
        public string BatchID { get; set; }
        public string BatchType { get; set; }
        public decimal BatchTotal { get; set; }
        public string FileDescription1 { get; set; }
        public string FileDescription2 { get; set; }
        public string UserCode { get; set; }
        public string BankNumber { get; set; }
        public System.DateTime DateModified { get; set; }
        public string WhoModified { get; set; }
        public string APApprover { get; set; }
        public string GLApprover { get; set; }
        public string FinalApprover { get; set; }
        public string Ledger { get; set; }
        public string Period { get; set; }
        public string Year { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDocumentNumber> tblDocumentNumbers { get; set; }
    }
}

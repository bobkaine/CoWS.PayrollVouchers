using CoWS.PayrollVouchers.FMSWebService;
using CoWS.PayrollVouchers.Models;
using CoWS.PayrollVouchers.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CoWS.Web.Logging;

namespace CoWS.PayrollVouchers.Controllers
{
    [Authorize(Roles = "AP Processor, GL Processor, Reviewer")]
    public class VouchersPayableController : Controller
    {
        private CoWSEventSource _evtSource = new CoWSEventSource();
        private CommonController _cc = new CommonController();
        
        // GET: Vouchers Payable
        //public async Task<ActionResult> Index()
        public ActionResult Index()
        {
            ViewBag.Message = "VouchersPayable";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> VouchersPayable_Create(BatchDocTransViewModel bdt)
        {
            if (!ModelState.IsValid)
            {
                return Json(bdt);
            }

            try
            {
                using (var db = new CSVPEntities())
                {
                    var whoModified = _cc.GetUserID(User.Identity.Name);
                    var dateModified = DateTime.Now;

                    var dn = new tblDocumentNumber
                    {
                        BankNumber = bdt.BankNumber,
                        BatchHeaderID = bdt.BatchHeaderID,
                        CheckDescription = bdt.CheckDescription,
                        DocumentType = bdt.DocumentType,
                        DocumentCreationDate = bdt.DocumentCreationDate,
                        PaymentStatus = bdt.PaymentStatus,
                        PaymentType = bdt.PaymentType,
                        PayPeriodEndingDate = bdt.PayPeriodEndingDate,
                        UserCode = bdt.UserCode,
                        VendorName = bdt.VendorName,
                        VendorNumber = bdt.VendorNumber,
                        WhoModified = whoModified,
                        DateModified = dateModified
                    };
                    db.tblDocumentNumbers.Add(dn);

                    var td = new tblTransactionDetail
                    {
                        DocumentID = dn.DocumentID,
                        BankNumber = dn.BankNumber,
                        CheckAmount = bdt.CheckAmount,
                        CreationDate = bdt.CheckCreationDate,
                        LineItemDescription = bdt.LineItemDescription,
                        ObjectCode = bdt.ObjectCode,
                        ProgramCode = bdt.ProgramCode,
                        ResponsibilityCode = bdt.ResponsibilityCode,
                        TransCode = bdt.TransCode,
                        WhoModified = whoModified,
                        DateModified = dateModified
                    };
                    db.tblTransactionDetails.Add(td);
                    await db.SaveChangesAsync();

                    var summedChecks = (from d in db.tblDocumentNumbers
                                        join t in db.tblTransactionDetails
                                        on d.DocumentID equals t.DocumentID
                                        where d.BatchHeaderID == d.BatchHeaderID
                                        select t.CheckAmount).Sum();

                    if (bdt.BatchTotal != summedChecks)
                    {
                        var bh = db.tblBatchHeaders.Where(w => w.BatchHeaderID == bdt.BatchHeaderID).FirstOrDefault();
                        if (bh != null)
                        {
                            bh.BatchTotal = summedChecks;
                            bh.WhoModified = whoModified;
                            bh.DateModified = dateModified;
                            db.Entry(bh).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                    }
                };
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(42, ex, CoWSEventSource.AppSection.VouchersPayable);
                return Json(ex.Message);
            }
        }

        public ActionResult VouchersPayable_Read([DataSourceRequest]DataSourceRequest request, string DocumentCreationDate)
        {
            try
            {
                var isDate = DateTime.TryParse(DocumentCreationDate, out DateTime documentCreationDate);

                if (!isDate) return Json(null);

                using (var db = new CSVPEntities())
                {
                    var bdt = (from bh in db.tblBatchHeaders
                               join dn in db.tblDocumentNumbers
                               on bh.BatchHeaderID equals dn.BatchHeaderID
                               join td in db.tblTransactionDetails
                               on dn.DocumentID equals td.DocumentID
                               where dn.DocumentType == "PV"
                               where dn.DocumentCreationDate == documentCreationDate
                               select new BatchDocTransViewModel()
                               {
                                   BankNumber = bh.BankNumber,
                                   BatchHeaderID = bh.BatchHeaderID,
                                   BatchID = bh.BatchID,
                                   BatchTotal = bh.BatchTotal,
                                   BatchType = bh.BatchType,
                                   CheckAmount = td.CheckAmount,
                                   CheckCreationDate = td.CreationDate,
                                   CheckDescription = dn.CheckDescription,
                                   DocumentCreationDate = dn.DocumentCreationDate,
                                   DocumentID = dn.DocumentID,
                                   DocumentType = dn.DocumentType,
                                   FileDescription1 = bh.FileDescription1,
                                   FileDescription2 = bh.FileDescription2,
                                   LineItemDescription = td.LineItemDescription,
                                   ObjectCode = td.ObjectCode,
                                   PaymentStatus = dn.PaymentStatus,
                                   PaymentType = dn.PaymentType,
                                   PayPeriodEndingDate = dn.PayPeriodEndingDate,
                                   Period = bh.Period,
                                   ProgramCode = td.ProgramCode,
                                   ResponsibilityCode = td.ResponsibilityCode,
                                   TransactionID = td.TransactionID,
                                   TransCode = td.TransCode,
                                   UserCode = bh.UserCode,
                                   VendorName = dn.VendorName,
                                   VendorNumber = dn.VendorNumber,
                                   Year = bh.Year,
                                   APApprover = bh.APApprover,
                                   GLApprover = bh.GLApprover,
                                   FinalApprover = bh.FinalApprover,
                                   DateModified = td.DateModified
                               }).OrderByDescending(o => o.DateModified).ToList();

                    DataSourceResult result = bdt.ToDataSourceResult(request);
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(43, ex, CoWSEventSource.AppSection.VouchersPayable);
                return Json(null);
            }
        }

        [HttpPost]
        public async Task<ActionResult> VouchersPayable_Update(BatchDocTransViewModel bdt)
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    var batch = db.tblBatchHeaders.Where(w => w.BatchID == bdt.BatchID).FirstOrDefault();
                    var document = db.tblDocumentNumbers.Where(w => w.DocumentID == bdt.DocumentID).FirstOrDefault();
                    var transaction = db.tblTransactionDetails.Where(w => w.TransactionID == bdt.TransactionID).FirstOrDefault();

                    var dateModified = DateTime.Now;
                    var whoModified = _cc.GetUserID(User.Identity.Name);

                    if (document != null)
                    {
                        document.PayPeriodEndingDate = bdt.PayPeriodEndingDate;
                        document.DateModified = dateModified;
                        document.WhoModified = whoModified;
                        db.Entry(document).State = EntityState.Modified;

                        if (transaction != null)
                        {
                            transaction.CheckAmount = bdt.CheckAmount;
                            transaction.CreationDate = bdt.CheckCreationDate;
                            db.Entry(transaction).State = EntityState.Modified;
                        }
                    }
                    await db.SaveChangesAsync();

                    var summedChecks = (from d in db.tblDocumentNumbers
                                        join t in db.tblTransactionDetails
                                        on d.DocumentID equals t.DocumentID
                                        where d.BatchHeaderID == bdt.BatchHeaderID
                                        select t.CheckAmount).Sum();

                    if (bdt.BatchTotal != summedChecks)
                    {
                        var bh = db.tblBatchHeaders.Where(w => w.BatchHeaderID == bdt.BatchHeaderID).FirstOrDefault();
                        if (bh != null)
                        {
                            bh.BatchTotal = summedChecks;
                            bh.WhoModified = whoModified;
                            bh.DateModified = dateModified;
                            db.Entry(bh).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                    }
                    return RedirectToAction("Index");
                }                
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(44, ex, CoWSEventSource.AppSection.VouchersPayable);
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> VouchersPayable_Destroy(BatchDocTransViewModel bdt)
        {
            using (var db = new CSVPEntities())
            {
                try
                {
                    var whoModified = _cc.GetUserID(User.Identity.Name);
                    var dateModified = DateTime.Now;

                    var document = db.tblDocumentNumbers.Where(w => w.DocumentID == bdt.DocumentID).FirstOrDefault();
                    var transaction = db.tblTransactionDetails.Where(w => w.TransactionID == bdt.TransactionID).FirstOrDefault();

                    if (transaction != null && document != null)
                    {
                        db.Entry(transaction).State = EntityState.Deleted;
                        db.Entry(document).State = EntityState.Deleted;
                    }
                    await db.SaveChangesAsync();

                    var summedChecks = (from d in db.tblDocumentNumbers
                                        join t in db.tblTransactionDetails
                                        on d.DocumentID equals t.DocumentID
                                        where d.BatchHeaderID == bdt.BatchHeaderID
                                        select t.CheckAmount).Sum();

                    if (bdt.BatchTotal != summedChecks)
                    {
                        var bh = db.tblBatchHeaders.Where(w => w.BatchHeaderID == bdt.BatchHeaderID).FirstOrDefault();
                        if (bh != null)
                        {
                            bh.BatchTotal = summedChecks;
                            bh.WhoModified = whoModified;
                            bh.DateModified = dateModified;
                            db.Entry(bh).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _evtSource.HandleException(61, ex, CoWSEventSource.AppSection.ChildSupport);
                    return Json(ex.Message);
                }
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_evtSource != null)
                {
                    _evtSource.Dispose();
                    _evtSource = null;
                }

                if (_cc != null)
                {
                    _cc.Dispose();
                    _cc = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}

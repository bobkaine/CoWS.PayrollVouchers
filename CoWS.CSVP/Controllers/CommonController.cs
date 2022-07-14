using CoWS.PayrollVouchers.FMSWebServices;
using CoWS.PayrollVouchers.Models;
using CoWS.PayrollVouchers.ViewModels;
using CoWS.Web.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CoWS.PayrollVouchers.Controllers
{
    [Authorize]
    public class CommonController : Controller
    {
        private CoWSEventSource _evtSource = new CoWSEventSource();
        DateTime glbDateModified;
        String glbWhoModified;

        public bool DocumentNumberExists(string DocumentType, string DocumentCreationDate)
        {
            var blnExists = true;
            using (var db = new CSVPEntities())
            {
                try
                {
                    var isDate = DateTime.TryParse(DocumentCreationDate, out DateTime documentCreationDate);

                    blnExists = db.tblDocumentNumbers.Where(w => w.DocumentCreationDate == documentCreationDate)
                                                     .Where(w => w.DocumentType == DocumentType).Any();
                }
                catch (Exception ex)
                {
                    _evtSource.HandleException(11, ex, CoWSEventSource.AppSection.Common);
                }

            }
            return blnExists;
        }

        public JsonResult ProcessCSVPFile(string FileType, string DocumentCreationDate)
        {
            try
            {
                glbDateModified = DateTime.Now;

                using (var db = new CSVPEntities())
                {
                    glbWhoModified = GetUserID(User.Identity.Name);
                }

                BatchViewModel bvm = ParseCSVPFile(FileType, DocumentCreationDate);
                if (bvm == null)
                {
                    return Json(null);
                }
                else if (bvm.BatchType == "ZZ")
                {
                    return Json(bvm, JsonRequestBehavior.AllowGet);
                }

                CreateBatchHeader(ref bvm);
                CreateDocumentNumbersAndTransactionDetails(ref bvm);
                return Json(bvm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(12, ex, CoWSEventSource.AppSection.Common);
                return Json(null);
            }
        }

        public JsonResult GetCurrentFileDocumentCreationDate(string DocumentType)
        {
            try
            {
                var filePath = "";
                using (var db = new CSVPEntities())
                {
                    filePath = db.tblGeneralSettings.Select(s => s.FilePath).FirstOrDefault();
                }

                if (!filePath.EndsWith("\\"))
                {
                    filePath += "\\";
                }

                switch (DocumentType)
                {
                    case "CS":
                        filePath += "paycs.txt";
                        break;
                    case "PV":
                        filePath += "paypv.txt";
                        break;
                }

                if (!System.IO.File.Exists(filePath))
                {
                    return Json(DateTime.MinValue, JsonRequestBehavior.AllowGet);
                }

                StreamReader _reader = System.IO.File.OpenText(filePath);
                string line;
                DateTime fileDate = DateTime.MinValue;

                while ((line = _reader.ReadLine()) != null)
                {
                    var recordType = line.Substring(0, 2);
                    if (recordType == "DN")
                    {
                        fileDate = new DateTime(Convert.ToInt32(line.Substring(79, 4).Trim()),
                                                Convert.ToInt32(line.Substring(83, 2).Trim()),
                                                Convert.ToInt32(line.Substring(85, 2).Trim()));
                        break;
                    }
                }
                _reader.Dispose();
                return Json(fileDate, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(13, ex, CoWSEventSource.AppSection.Common);
                return Json(null);
            }
        }

        public BatchViewModel ParseCSVPFile(string fileType, string documentCreationDate)
        {
            try
            {
                var filePath = "";
                using (var db = new CSVPEntities())
                {
                    filePath = db.tblGeneralSettings.Select(s => s.FilePath).FirstOrDefault();
                }

                if (!filePath.EndsWith("\\"))
                {
                    filePath += "\\";
                }

                switch (fileType)
                {
                    case "CS":
                        filePath += "paycs.txt";
                        break;
                    case "PV":
                        filePath += "paypv.txt";
                        break;
                }

                if (!System.IO.File.Exists(filePath))
                {
                    return null;
                }

                StreamReader _reader = System.IO.File.OpenText(filePath);
                string line;

                BatchViewModel bvm = new BatchViewModel();
                DocumentNumberViewModel dnvm = new DocumentNumberViewModel();
                TransactionDetailViewModel tdvm;
                while ((line = _reader.ReadLine()) != null)
                {
                    var recordType = line.Substring(0, 2);
                    switch (recordType)
                    {
                        case "BH":
                            bvm = ParseBatchHeader(line, fileType);
                            break;
                        case "DN":
                            dnvm = ParseDocumentNumber(line);
                            var isValid = VerifyDocumentCreationDate(ref dnvm, documentCreationDate);
                            if (!isValid)
                            {
                                DocumentNumberViewModel emptyDNVM = new DocumentNumberViewModel()
                                {
                                    DocumentCreationDate = dnvm.DocumentCreationDate
                                };

                                List<DocumentNumberViewModel> listDocumentNumbers = new List<DocumentNumberViewModel>
                                {
                                    emptyDNVM
                                };

                                BatchViewModel emptyBVM = new BatchViewModel()
                                {
                                    BatchType = "ZZ",
                                    DocumentNumbers = listDocumentNumbers
                                };
                                return emptyBVM;
                            }
                            else
                            {
                                if (bvm.Period == null )
                                {
                                    var fileYear = CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(Convert.ToInt32(dnvm.PayPeriodEndingDate.Substring(4, 2).Trim()));
                                    var filePeriod = Convert.ToInt32(dnvm.PayPeriodEndingDate.Substring(0, 2).Trim());

                                    var fiscalYear = "";
                                    var fiscalPeriod = "";

                                    if (filePeriod > 6)
                                    {
                                        fiscalYear = (fileYear + 1).ToString();
                                        fiscalPeriod = (filePeriod - 6).ToString("00");
                                    }
                                    else
                                    {
                                        fiscalYear = fileYear.ToString();
                                        fiscalPeriod = (filePeriod + 6).ToString("00");
                                    }
                                    bvm.Period = fiscalPeriod;
                                    bvm.Year = fiscalYear;
                                }
                            }
                            break;
                        case "TD":
                            tdvm = ParseTransactionDetail(line);
                            dnvm.TransactionDetails.Add(tdvm);
                            bvm.DocumentNumbers.Add(dnvm);
                            break;
                    }
                }
                _reader.Dispose();
                return bvm;
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(14, ex, CoWSEventSource.AppSection.Common);
                return null;
            }
        }

        private int CreateBatchHeader(ref BatchViewModel bvm)
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    var bhEntity = new tblBatchHeader
                    {
                        BatchType = bvm.BatchType,
                        BatchTotal = bvm.BatchTotal,
                        FileDescription1 = bvm.FileDescription1,
                        FileDescription2 = bvm.FileDescription2,
                        UserCode = bvm.UserCode,
                        BankNumber = bvm.BankNumber,
                        Ledger = bvm.Ledger,
                        Period = bvm.Period,
                        Year = bvm.Year,
                        DateModified = bvm.DateModified,
                        WhoModified = bvm.WhoModified
                    };
                    db.tblBatchHeaders.Add(bhEntity);
                    db.SaveChanges();
                    bvm.BatchHeaderID = bhEntity.BatchHeaderID;
                }                    
                return bvm.BatchHeaderID;
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(15, ex, CoWSEventSource.AppSection.Common);
                return -1;
            }

        }

        public int CreateDocumentNumbersAndTransactionDetails(ref BatchViewModel bvm)
        {            
            try
            {
                using (var db = new CSVPEntities())
                {
                    foreach (var item in bvm.DocumentNumbers)
                    {
                        var isCreateDate = DateTime.TryParse(item.DocumentCreationDate.Substring(0, 4) + "/" +
                                                                item.DocumentCreationDate.Substring(4, 2) + "/" +
                                                                item.DocumentCreationDate.Substring(6, 2), out DateTime documentCreationDate);
                        var isPayDate = DateTime.TryParse(item.PayPeriodEndingDate.Substring(0, 2) + "/" +
                                                            item.PayPeriodEndingDate.Substring(2, 2) + "/" +
                                                            item.PayPeriodEndingDate.Substring(4, 2), out DateTime payPeriodEndingDate);

                        if (isCreateDate && isPayDate)
                        {
                            var dnEntity = new tblDocumentNumber
                            {
                                BatchHeaderID = bvm.BatchHeaderID,
                                BankNumber = item.BankNumber,
                                CheckDescription = item.CheckDescription,
                                DocumentCreationDate = documentCreationDate,
                                DocumentType = item.DocumentType,
                                PaymentStatus = item.PaymentStatus,
                                PaymentType = item.PaymentType,
                                PayPeriodEndingDate = payPeriodEndingDate,
                                UserCode = item.UserCode,
                                VendorName = item.VendorName,
                                VendorNumber = item.VendorNumber,
                                DateModified = item.DateModified,
                                WhoModified = item.WhoModified
                            };
                            db.tblDocumentNumbers.Add(dnEntity);
                            db.SaveChanges();

                            item.DocumentID = dnEntity.DocumentID;

                            CreateTransactionDetail(ref bvm, item.DocumentID);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _evtSource.HandleException(16, ex, CoWSEventSource.AppSection.Common);
                return -1;
            }
            return 0;
        }

        public int CreateTransactionDetail(ref BatchViewModel bvm, int documentID)
        {            
            try
            {
                using (var db = new CSVPEntities())
                {
                    var tdList = bvm.DocumentNumbers.Where(w => w.DocumentID == documentID).Select(s => s.TransactionDetails).FirstOrDefault();

                    foreach (var td in tdList)
                    {
                        var isDate = DateTime.TryParse(td.CreationDate.Substring(0, 4) + "/" +
                                                        td.CreationDate.Substring(4, 2) + "/" +
                                                        td.CreationDate.Substring(6, 2), out DateTime creationDate);

                        if (isDate)
                        {
                            var tdEntity = new tblTransactionDetail
                            {
                                DocumentID = documentID,
                                BankNumber = td.BankNumber,
                                CheckAmount = td.CheckAmount,
                                CreationDate = creationDate,
                                LineItemDescription = td.LineItemDescription,
                                ObjectCode = td.ObjectCode,
                                ProgramCode = td.ProgramCode,
                                ResponsibilityCode = td.ResponsibilityCode,
                                TransCode = td.TransCode,
                                DateModified = td.DateModified,
                                WhoModified = td.WhoModified
                            };
                            db.tblTransactionDetails.Add(tdEntity);
                            db.SaveChanges();
                            td.TransactionID = tdEntity.TransactionID;
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(17, ex, CoWSEventSource.AppSection.Common);
                return -1;
            }
        }

        private bool VerifyDocumentCreationDate(ref DocumentNumberViewModel dnvm, string passedDocumentCreationDate)
        {
            var fileDate = new DateTime(Convert.ToInt32(dnvm.DocumentCreationDate.Substring(0, 4)),
                                        Convert.ToInt32(dnvm.DocumentCreationDate.Substring(4, 2)),
                                        Convert.ToInt32(dnvm.DocumentCreationDate.Substring(6, 2)));
            DateTime.TryParse(passedDocumentCreationDate, out DateTime parsedDate);

            if (fileDate != parsedDate)
            {
                dnvm.DocumentCreationDate = fileDate.ToShortDateString();
                return false;
            }
            return true;
        }

        public BatchViewModel ParseBatchHeader(string line, string batchType)
        {
            BatchViewModel bvm = new BatchViewModel()
            {
                BatchType = batchType,
                BatchTotal = Convert.ToDecimal(line.Substring(14, 17)),
                FileDescription1 = line.Substring(43, 50).Trim(),
                FileDescription2 = line.Substring(93, 50).Trim(),
                UserCode = line.Substring(143, 3).Trim(),
                BankNumber = line.Substring(160, 4).Trim(),
                Ledger = "FMSAP",
                DateModified = glbDateModified,
                WhoModified = glbWhoModified
            };
            return bvm;
        }

        public DocumentNumberViewModel ParseDocumentNumber(string line)
        {
            DocumentNumberViewModel dnvm = new DocumentNumberViewModel()
            {
                DocumentType = line.Substring(28, 2).Trim(),
                PayPeriodEndingDate = line.Substring(30, 6).Trim(),
                VendorName = line.Substring(37, 23).Trim(),
                VendorNumber = line.Substring(60, 16).Trim(),
                UserCode = line.Substring(76, 3).Trim(),
                DocumentCreationDate = line.Substring(79, 8).Trim(),
                PaymentType = line.Substring(87, 4).Trim(),
                PaymentStatus = line.Substring(99, 2).Trim(),
                CheckDescription = line.Substring(102, 28).Trim(),
                BankNumber = line.Substring(130, 4).Trim(),
                DateModified = glbDateModified,
                WhoModified = glbWhoModified
            };
            return dnvm;
        }

        public TransactionDetailViewModel ParseTransactionDetail(string line)
        {
            TransactionDetailViewModel tdvm = new TransactionDetailViewModel()
            {
                TransCode = line.Substring(2, 3).Trim(),
                CreationDate = line.Substring(5, 8).Trim(),
                CheckAmount = Convert.ToDecimal(line.Substring(13, 17)),
                ResponsibilityCode = line.Substring(30, 6).Trim(),
                ObjectCode = line.Substring(36, 6).Trim(),
                ProgramCode = line.Substring(42, 6).Trim(),
                LineItemDescription = line.Substring(114, 13).Trim(),
                BankNumber = line.Substring(142, 4).Trim(),
                DateModified = glbDateModified,
                WhoModified = glbWhoModified
            };
            return tdvm;
        }

        public ActionResult SubmitAPBatch(string FileType, DateTime DocumentCreationDate)
        {
            try
            {
                var batchRecords = GetBatchRecords(FileType, DocumentCreationDate);
                var bvm = PopulateBatchViewModel(batchRecords);
                var apBatch = CreateAPBatch(bvm);
                var response = CallCreateAPBatch(apBatch, bvm.BatchHeaderID);
                
                foreach (var item in response)
                {
                    if (item.Key == "BatchID")
                    {
                        UpdateBatch(item.Value, bvm.BatchHeaderID);
                    }
                }
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                _evtSource.HandleException(18, ex, CoWSEventSource.AppSection.Common);
                return Json(ex.Message);
            }            
        }

        public ActionResult GetLatestBatch(string BatchType)
        {
            try
            {
                var latestBatchDate = DateTime.MinValue;
                using (var db = new CSVPEntities())
                {
                    latestBatchDate = (from bh in db.tblBatchHeaders
                                      join dn in db.tblDocumentNumbers
                                      on bh.BatchHeaderID equals dn.BatchHeaderID
                                      where bh.BatchType == BatchType 
                                      select dn.DocumentCreationDate).FirstOrDefault();
                    if (latestBatchDate != null)
                    {
                        return Json(latestBatchDate);
                    }
                }
                return (Json(latestBatchDate));
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(19, ex, CoWSEventSource.AppSection.Common);
                return Json(ex.Message);
            }
        }

        private List<BatchDocTransViewModel> GetBatchRecords(string FileType, DateTime DocumentCreationDate)
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    List<BatchDocTransViewModel> batchRecords = (from bh in db.tblBatchHeaders
                                                                 join dn in db.tblDocumentNumbers
                                                                 on bh.BatchHeaderID equals dn.BatchHeaderID
                                                                 join td in db.tblTransactionDetails
                                                                 on dn.DocumentID equals td.DocumentID
                                                                 where dn.DocumentType == FileType
                                                                 where dn.DocumentCreationDate == DocumentCreationDate
                                                                 select new BatchDocTransViewModel()
                                                                 {
                                                                     BatchHeaderID = bh.BatchHeaderID,
                                                                     BatchID = bh.BatchID,
                                                                     BatchType = bh.BatchType,
                                                                     BatchTotal = bh.BatchTotal,
                                                                     FileDescription1 = bh.FileDescription1,
                                                                     FileDescription2 = bh.FileDescription2,
                                                                     UserCode = bh.UserCode,
                                                                     BankNumber = bh.BankNumber,
                                                                     Ledger = bh.Ledger,
                                                                     Period = bh.Period,
                                                                     Year = bh.Year,
                                                                     DocumentType = dn.DocumentType,
                                                                     PayPeriodEndingDate = dn.PayPeriodEndingDate,
                                                                     VendorName = dn.VendorName,
                                                                     VendorNumber = dn.VendorNumber,
                                                                     DocumentCreationDate = dn.DocumentCreationDate,
                                                                     PaymentType = dn.PaymentType,
                                                                     PaymentStatus = dn.PaymentStatus,
                                                                     CheckDescription = dn.CheckDescription,
                                                                     TransCode = td.TransCode,
                                                                     CheckCreationDate = td.CreationDate,
                                                                     CheckAmount = td.CheckAmount,
                                                                     ResponsibilityCode = td.ResponsibilityCode,
                                                                     ObjectCode = td.ObjectCode,
                                                                     ProgramCode = td.ProgramCode,
                                                                     LineItemDescription = td.LineItemDescription
                                                                 }).ToList();
                    return batchRecords;
                }
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(20, ex, CoWSEventSource.AppSection.Common);
                return null;
            }            
        }

        private BatchViewModel PopulateBatchViewModel(List<BatchDocTransViewModel> batchRecords)
        {
            try
            {
                BatchViewModel bvm = new BatchViewModel()
                {
                    BatchHeaderID = batchRecords[0].BatchHeaderID,
                    BankNumber = batchRecords[0].BankNumber,
                    BatchType = batchRecords[0].BatchType,
                    BatchTotal = batchRecords[0].BatchTotal,
                    FileDescription1 = batchRecords[0].FileDescription1,
                    FileDescription2 = batchRecords[0].FileDescription2,
                    UserCode = batchRecords[0].UserCode,
                    Ledger = batchRecords[0].Ledger,
                    Period = batchRecords[0].Period,
                    Year = batchRecords[0].Year
                };

                foreach (var br in batchRecords)
                {
                    TransactionDetailViewModel tdvm = new TransactionDetailViewModel()
                    {
                        TransCode = br.TransCode,
                        CreationDate = br.CheckCreationDate.ToShortDateString(),
                        CheckAmount = br.CheckAmount,
                        ResponsibilityCode = br.ResponsibilityCode,
                        ObjectCode = br.ObjectCode,
                        ProgramCode = br.ProgramCode,
                        LineItemDescription = br.LineItemDescription,
                        BankNumber = br.BankNumber
                    };

                    DocumentNumberViewModel dnvm = new DocumentNumberViewModel()
                    {
                        DocumentType = br.DocumentType,
                        PayPeriodEndingDate = br.PayPeriodEndingDate.ToShortDateString(),
                        VendorName = br.VendorName,
                        VendorNumber = br.VendorNumber,
                        UserCode = br.UserCode,
                        DocumentCreationDate = br.DocumentCreationDate.ToShortDateString(),
                        PaymentType = br.PaymentType,
                        PaymentStatus = br.PaymentStatus,
                        CheckDescription = br.CheckDescription,
                        BankNumber = br.BankNumber,
                    };
                    dnvm.TransactionDetails.Add(tdvm);
                    bvm.DocumentNumbers.Add(dnvm);
                }
                return bvm;
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(21, ex, CoWSEventSource.AppSection.Common);
                return null;
            }
        }
                
        private APBatch CreateAPBatch(BatchViewModel bvm)
        {
            try
            {
                var documentCount = bvm.DocumentNumbers.Count;
                var transactionCount = 0;

                // create an array of Documents
                APBatchNewDocument[] aPBatchNewDocuments = new APBatchNewDocument[documentCount];
                var i = 0;

                // iterate through the documents within the batch
                foreach (var dn in bvm.DocumentNumbers)
                {
                    var tCount = dn.TransactionDetails.Count;

                    // create an array of transactions
                    APBatchTransaction[] aPBatchTransactions = new APBatchTransaction[tCount];
                    var j = 0;

                    // iterate through the transactions found within the current document
                    foreach (var td in dn.TransactionDetails)
                    {
                        var creationDate = DateTime.Parse(td.CreationDate).ToString("yyyy-MM-dd");

                        // create a new transaction from the Batch.DocumentNumber.TransactionDetail
                        APBatchTransaction aPBatchTransaction = new APBatchTransaction()
                        {
                            TranCode = td.TransCode,
                            Amount = td.CheckAmount,
                            RefDate = creationDate,
                            Account = td.ResponsibilityCode.Substring(0,3) + td.ResponsibilityCode + td.ObjectCode + td.ProgramCode,
                            Comment = td.LineItemDescription, 
                            Bank = td.BankNumber,
                            Price = td.CheckAmount,
                            FCOverride = "N"
                        };
                        aPBatchTransactions[j] = aPBatchTransaction;
                        j++;
                    }
                    transactionCount = j;

                    var documentCreationDate = DateTime.Parse(dn.DocumentCreationDate).ToString("yyyy-MM-dd");
                    var payPeriodEndingDate = DateTime.Parse(dn.PayPeriodEndingDate).ToString("yyyy-MM-dd");

                    // create a new document from the Batch.DocumentNumber
                    APBatchNewDocument aPBatchNewDocument = new APBatchNewDocument()
                    {
                        DocType = dn.DocumentType,
                        DocNumber = "",
                        Vendor = dn.VendorNumber,
                        User = dn.UserCode,
                        DocDate = documentCreationDate,
                        Terms = dn.PaymentType,
                        PayDate = payPeriodEndingDate,
                        Desc = dn.CheckDescription,
                        Bank = dn.BankNumber,
                        SendDocID = "Y",
                        UseSepCheck = "Y",
                        SepCheck = true,
                        Transactions = aPBatchTransactions
                    };

                    aPBatchNewDocuments[i] = aPBatchNewDocument;
                    i++;
                }

                // create a new Batch record to include the array of DocumentNumbers and TransactionDetails from above
                APBatch inputRec = new APBatch()
                {
                    BatchType = bvm.BatchType,
                    BatchNo = 0,
                    Period = bvm.Period, 
                    Year = bvm.Year, 
                    FinancialCtl = bvm.BatchTotal,
                    DocumentCtl = documentCount,
                    TransactionCtl = transactionCount,
                    Desc1 = bvm.FileDescription1,
                    Desc2 = bvm.FileDescription2,
                    Bank = bvm.BankNumber,
                    Gencon = "APDE",
                    User = bvm.UserCode,
                    UnconditionalUpdate = true,
                    NewDocuments = aPBatchNewDocuments
                };
                return inputRec;
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(22, ex, CoWSEventSource.AppSection.Common);
                return null;
            }
        }

        internal List<KeyValuePair<string,string>> CallCreateAPBatch(APBatch apBatch, int BatchHeaderID)
        {
            KeyValuePair<string, string> _keyValuePair;
            List<KeyValuePair<string, string>> _status = new List<KeyValuePair<string, string>>();
            try
            {
                APBatchStatus statusRec = new APBatchStatus();
                var _request = apBatch.ToXML();
                
                using (var db = new CSVPEntities())
                {
                    var wsSettings = db.tblWebServiceSettings.FirstOrDefault();
                    var result = 0;
                    using (FMSWebServicesSoapClient apClient = new FMSWebServicesSoapClient("FMSWebService", wsSettings.WebServiceURL))
                    {
                        result = apClient.CreateAPBatch(wsSettings.FMSUser, wsSettings.FMSPassword1, wsSettings.FMSPassword2, wsSettings.FMSPassword3, wsSettings.Ledger, wsSettings.OutputDevice, wsSettings.OSUser, wsSettings.OSPassword, apBatch, out statusRec);
                    }

                    if (result > 0)
                    {
                        _keyValuePair = GetErrorResultMessage(result);
                        var _response = statusRec.ToXML();
                        SaveSoapXML(apBatch.BatchType,BatchHeaderID, _request, _response);
                        _status.Add(_keyValuePair);
                        return _status;
                    }
                }
                if (statusRec.Overall == "N")
                {
                    var _response = statusRec.ToXML();
                    SaveSoapXML(apBatch.BatchType, BatchHeaderID, _request, _response);
                    _status.Add(new KeyValuePair<string, string>("ErrorMessage", "Post was not completely successful. See Soap Details."));
                }

                if (statusRec.BatchId != "")
                {
                    _status.Add(new KeyValuePair<string, string>("BatchID", statusRec.BatchId));
                }
                return _status;
            }

            catch (Exception ex)
            {
                _evtSource.HandleException(23, ex, CoWSEventSource.AppSection.Common);
                _status.Add(new KeyValuePair<string, string>("ErrorMessage", ex.Message));
                return _status;
            }                
        }
                
        private void UpdateBatch(string BatchID, int BatchHeaderID)
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    var entity = db.tblBatchHeaders.Where(w => w.BatchHeaderID == BatchHeaderID).FirstOrDefault();

                    if (entity != null)
                    {
                        var userID = GetUserID(User.Identity.Name);
                        entity.BatchID = BatchID;
                        entity.DateModified = DateTime.Now;
                        entity.WhoModified = userID;
                        entity.APApprover = userID;
                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(24, ex, CoWSEventSource.AppSection.Common);
                throw ex;
            }
        }

        internal KeyValuePair<string,string> GetErrorResultMessage(int result)
        {
            var errMsg = "";
            switch (result)
            {
                case 1:
                    errMsg = "Failure due to invalid FMS user/password.";
                    break;
                case 2:
                    errMsg = "Failure due to invalid FMS server logon user/password.";
                    break;
                case 3:
                    errMsg = "Security failure because the FMS user does not have the capability to use the requested Web Service.";
                    break;
                case 4:
                    errMsg = "Failure due to inability to connect to the MHNet connection listener. Probable reasons would be that the listener service is not running or the required configuration section is missing or invalid in the Web Service configuration file.";
                    break;
                case 5:
                    errMsg = "Failure trying to connect to FMS. Contact MH&Co.";
                    break;
                case 6:
                    errMsg = "Failure in the MHNet server process. Contact MH&Co.";
                    break;
                case 7:
                    errMsg = "The specified output device is not valid. The device must exist in the $DESTINATIONS-OBJECT table.";
                    break;
                default:
                    break;
            }
            return new KeyValuePair<string, string>("ErrorMessage", errMsg); 
        }

        public string GetUserID(string UserName)
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    return db.AspNetUsers.Where(w => w.UserName == UserName).Select(s => s.Id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(25, ex, CoWSEventSource.AppSection.Common);
                return null;
            }            
        }

        public JsonResult RemoveApprovals(string BatchID)
        {
            try
            {
                using (var db = new CSVPEntities())
                {                    
                    var entity = db.tblBatchHeaders.Where(w => w.BatchID == BatchID).FirstOrDefault();

                    if (entity != null)
                    {
                        entity.BatchID = null;
                        entity.APApprover = null;
                        entity.GLApprover = null;
                        entity.FinalApprover = null;
                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return Json("Success", JsonRequestBehavior.AllowGet);
                    }
                    return Json(String.Format("BatchID: {0} not found.", BatchID), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(26, ex, CoWSEventSource.AppSection.Common);
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateGLProcessorApproval(int BatchHeaderID, bool approved)
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    var entity = db.tblBatchHeaders.Where(w => w.BatchHeaderID == BatchHeaderID).FirstOrDefault();
                    if (entity != null)
                    {
                        if (approved)
                        {
                            CommonController cc = new CommonController();
                            entity.GLApprover = cc.GetUserID(User.Identity.Name);
                        }
                        else
                        {
                            entity.GLApprover = null;
                        }
                        db.Entry(entity).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
                return Json("Success");
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(27, ex, CoWSEventSource.AppSection.Common);
                return Json(ex.Message);
            }
        }

        public async Task<ActionResult> UpdateReviewerApproval(int BatchHeaderID, bool approved)
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    var entity = db.tblBatchHeaders.Where(w => w.BatchHeaderID == BatchHeaderID).FirstOrDefault();
                    if (entity != null)
                    {
                        if (approved)
                        {
                            CommonController cc = new CommonController();
                            entity.FinalApprover = cc.GetUserID(User.Identity.Name);
                        }
                        else
                        {
                            entity.FinalApprover = null;
                        }
                        db.Entry(entity).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
                return Json("Success");
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(28, ex, CoWSEventSource.AppSection.Common);
                return Json(ex.Message);
            }
        }

        public void SaveSoapXML(string BatchType, int ID, string Request, string Response)
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    var entity = db.tblSoapXMLs.Where(w => w.BatchType == BatchType).FirstOrDefault();
                    if (entity != null)
                    {
                        // update
                        entity.ID = ID;
                        entity.Request = Request;
                        entity.Response = Response;
                        entity.DatePosted = DateTime.Now;
                        db.Entry(entity).State = EntityState.Modified;
                    }
                    else
                    {
                        //create
                        var sx = new tblSoapXML
                        {
                            BatchType = BatchType,
                            ID = ID,
                            Request = Request,
                            Response = Response,
                            DatePosted = DateTime.Now
                        };
                        db.tblSoapXMLs.Add(sx);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(58, ex, CoWSEventSource.AppSection.Common);
            }
        }

        public JsonResult ReadSoapXML(string BatchType)
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    SoapViewModel svm;
                    if (BatchType == "CS" || BatchType == "PV")
                    {
                        svm = (from x in db.tblSoapXMLs
                                      join bh in db.tblBatchHeaders
                                      on x.ID equals bh.BatchHeaderID
                                      join dn in db.tblDocumentNumbers
                                      on bh.BatchHeaderID equals dn.BatchHeaderID
                                      where x.BatchType == BatchType
                                      select new SoapViewModel
                                      {
                                          Request = x.Request,
                                          Response = x.Response,
                                          DatePosted = x.DatePosted,
                                          FileDescription = bh.FileDescription1,
                                          FileDate = dn.DocumentCreationDate
                                      }).FirstOrDefault();
                    }
                    else
                    {
                        svm = (from x in db.tblSoapXMLs
                               join gl in db.tblGeneralLedgers
                               on x.ID equals gl.GLID
                               join td in db.tblGLTransactionDetails
                               on gl.GLID equals td.GLID
                               where x.BatchType == BatchType
                               select new SoapViewModel
                               {
                                   Request = x.Request,
                                   Response = x.Response,
                                   DatePosted = x.DatePosted,
                                   FileDescription = gl.GLComments,
                                   FileDate = td.TransactionDate
                               }).FirstOrDefault();

                    }
                    if (svm == null)
                    {
                        svm = new SoapViewModel()
                        {
                            Request = "",
                            Response = "",
                            FileDescription = "No data found."
                        };
                    }
                    return Json(svm, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(59, ex, CoWSEventSource.AppSection.Common);
                return Json(null);
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
            }

            base.Dispose(disposing);
        }
    }
}

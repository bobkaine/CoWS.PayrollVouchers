using CoWS.PayrollVouchers.FMSWebServices;
using CoWS.PayrollVouchers.Models;
using CoWS.PayrollVouchers.ViewModels;
using CoWS.Web.Logging;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
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
    [Authorize(Roles = "GL Processor")]
    public class PayrollController : Controller
    {
        private CoWSEventSource _evtSource = new CoWSEventSource();
        private CommonController _cc = new CommonController();
        DateTime glbDateModified;
        String glbWhoModified;

        // GET: Payroll
        public ActionResult Index()
        {
            ViewBag.Message = "Payroll Review";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Payroll_Create(GLTDViewModel gltd)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new CSVPEntities())
                    {
                        var whoModified = _cc.GetUserID(User.Identity.Name);
                        var dateModified = DateTime.Now;

                        var td = new tblGLTransactionDetail
                        {
                            GLID = gltd.GLID,
                            TransactionAmount = gltd.TransactionAmount,
                            TransactionDate = gltd.TransactionDate,
                            Comments = gltd.Comments,
                            Fund = gltd.Fund,
                            ResponsibilityCode = gltd.ResponsibilityCode,
                            ObjectCode = gltd.ObjectCode,
                            ProgramCode = gltd.ProgramCode,
                            WhoModified = whoModified,
                            DateModified = dateModified
                        };
                        db.tblGLTransactionDetails.Add(td);
                        await db.SaveChangesAsync();

                        var summedChecks = (from td2 in db.tblGLTransactionDetails
                                            where td2.GLID == gltd.GLID
                                            select td2.TransactionAmount).Sum();

                        if (gltd.BatchTotal != summedChecks)
                        {
                            var gl = db.tblGeneralLedgers.Where(w => w.GLID == gltd.GLID).FirstOrDefault();
                            if (gl != null)
                            {
                                gl.BatchTotal = summedChecks;
                                gl.WhoModified = whoModified;
                                gl.DateModified = dateModified;
                                db.Entry(gl).State = EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                    };
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _evtSource.HandleException(42, ex, CoWSEventSource.AppSection.Payroll);
                    return Json(ex.Message);
                }
            }
            return Json(gltd);
        }

        [HttpPost]
        public ActionResult Payroll_Read([DataSourceRequest]DataSourceRequest request, string TransactionDate)
        {
            try
            {
                var isDate = DateTime.TryParse(TransactionDate, out DateTime transactionDate);

                if (!isDate) return Json(null);

                using (var db = new CSVPEntities())
                {
                    var gltd = (from gl in db.tblGeneralLedgers
                              join td in db.tblGLTransactionDetails
                              on gl.GLID equals td.GLID
                              where td.TransactionDate == transactionDate
                              select new GLTDViewModel()
                              {
                                  GLID = gl.GLID,
                                  GLType = gl.GLType,
                                  BatchNumber = gl.BatchNumber,
                                  BatchTotal = gl.BatchTotal,
                                  BatchType = gl.BatchType,
                                  GLComments = gl.GLComments,
                                  JournalComments = gl.JournalComments,
                                  Ledger = gl.Ledger,
                                  Period = gl.Period,
                                  Year = gl.Year,
                                  UserCode = gl.UserCode,
                                  DateModified = gl.DateModified,
                                  WhoModified = gl.WhoModified,
                                  TransactionID = td.TransactionID,
                                  TransactionDate = td.TransactionDate,
                                  TransactionAmount = td.TransactionAmount,
                                  Comments = td.Comments,
                                  Fund = td.Fund,
                                  ResponsibilityCode = td.ResponsibilityCode,
                                  ObjectCode = td.ObjectCode,
                                  ProgramCode = td.ProgramCode
                            }).OrderByDescending(o => o.DateModified).ToList();

                    DataSourceResult result = gltd.ToDataSourceResult(request);
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(43, ex, CoWSEventSource.AppSection.Payroll);
                return Json(null);
            }

        }

        [HttpPost]
        public async Task<ActionResult> Payroll_Update(GLTDViewModel gltd)
        {
            using (var db = new CSVPEntities())
            {
                try
                {
                    var gl = db.tblGeneralLedgers.Where(w => w.GLID == gltd.GLID).FirstOrDefault();
                    var transaction = db.tblGLTransactionDetails.Where(w => w.TransactionID == gltd.TransactionID).FirstOrDefault();

                    var dateModified = DateTime.Now;
                    var whoModified = _cc.GetUserID(User.Identity.Name);

                    if (transaction != null)
                    {
                        transaction.TransactionAmount = gltd.TransactionAmount;
                        transaction.TransactionDate = gltd.TransactionDate;
                        transaction.Fund = gltd.Fund;
                        transaction.ResponsibilityCode = gltd.ResponsibilityCode;
                        transaction.ObjectCode = gltd.ObjectCode;
                        transaction.ProgramCode = gltd.ProgramCode;
                        transaction.DateModified = dateModified;
                        transaction.WhoModified = whoModified;
                        db.Entry(transaction).State = EntityState.Modified;
                    }
                    await db.SaveChangesAsync();

                    var summedChecks = (from td in db.tblGLTransactionDetails
                                        where td.GLID == gltd.GLID
                                        select td.TransactionAmount).Sum();

                    if (gltd.BatchTotal != summedChecks)
                    {
                        if (gl != null)
                        {
                            gl.BatchTotal = summedChecks;
                            gl.WhoModified = whoModified;
                            gl.DateModified = dateModified;
                            db.Entry(gl).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _evtSource.HandleException(44, ex, CoWSEventSource.AppSection.Payroll);
                    return Json(ex.Message);
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Payroll_Destroy(GLTDViewModel gltd)
        {
            using (var db = new CSVPEntities())
            {
                try
                {
                    var whoModified = _cc.GetUserID(User.Identity.Name);
                    var dateModified = DateTime.Now;

                    var transaction = db.tblGLTransactionDetails.Where(w => w.TransactionID == gltd.TransactionID).FirstOrDefault();

                    if (transaction != null)
                    {
                        db.Entry(transaction).State = EntityState.Deleted;
                    }
                    await db.SaveChangesAsync();

                    var summedChecks = (from td in db.tblGLTransactionDetails
                                        where td.GLID == gltd.GLID
                                        select td.TransactionAmount).Sum();

                    if (gltd.BatchTotal != summedChecks)
                    {
                        var gl = db.tblGeneralLedgers.Where(w => w.GLID == gltd.GLID).FirstOrDefault();
                        if (gl != null)
                        {
                            gl.BatchTotal = summedChecks;
                            gl.WhoModified = whoModified;
                            gl.DateModified = dateModified;
                            db.Entry(gl).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _evtSource.HandleException(45, ex, CoWSEventSource.AppSection.Payroll);
                    return Json(ex.Message);
                }
            }
        }

        public JsonResult GetCurrentFileTransactionDate(string FileType)
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

                switch (FileType)
                {
                    case "Regular":
                        filePath += "paygl.txt";
                        break;
                    case "Pension":
                        filePath += "payglPEN.txt";
                        break;
                    case "Longevity":
                        filePath += "payglLONG.txt";
                        break;
                    case "N99":
                        filePath += "payglN99.txt";
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
                    if (recordType == "TD")
                    {
                        fileDate = new DateTime(CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(Convert.ToInt32(line.Substring(110, 2).Trim())),
                                                Convert.ToInt32(line.Substring(106, 2).Trim()),
                                                Convert.ToInt32(line.Substring(108, 2).Trim()));
                        break;
                    }
                }
                _reader.Dispose();
                return Json(fileDate, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(46, ex, CoWSEventSource.AppSection.Payroll);
                return Json(null);
            }
        }

        public bool TransactionDetailExists(string GLType, string TransactionDate)
        {
            var blnExists = true;
            using (var db = new CSVPEntities())
            {
                try
                {
                    var isDate = DateTime.TryParse(TransactionDate, out DateTime transactionDate);

                    blnExists = (from td in db.tblGLTransactionDetails
                                 join gl in db.tblGeneralLedgers
                                 on td.GLID equals gl.GLID
                                 where td.TransactionDate == transactionDate
                                 where gl.GLType == GLType
                                 select gl.GLID).Any();
                    //db.tblGLTransactionDetails.Where(w => w.TransactionDate == transactionDate).Any();
                }
                catch (Exception ex)
                {
                    _evtSource.HandleException(47, ex, CoWSEventSource.AppSection.Payroll);
                }

            }
            return blnExists;
        }

        public JsonResult ProcessGeneralLedgerFile(string FileType, string TransactionDate)
        {
            try
            {
                glbDateModified = DateTime.Now;

                using (var db = new CSVPEntities())
                {
                    glbWhoModified = _cc.GetUserID(User.Identity.Name);
                }

                GeneralLedgerViewModel gl = ParseGeneralLedgerFile(FileType, TransactionDate);
                if (gl == null)
                {
                    return Json("");
                }
                else if (gl.BatchType == "ZZ")
                {
                    return Json(gl, JsonRequestBehavior.AllowGet);
                }

                CreateGeneralLedger(ref gl);
                CreateTransactionDetails(ref gl);
                return Json(gl, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(48, ex, CoWSEventSource.AppSection.Payroll);
                return Json(null);
            }
        }

        public GeneralLedgerViewModel ParseGeneralLedgerFile(string FileType, string TransactionDate)
        {
            try
            {
                var filePath = "";
                var glType = "";

                using (var db = new CSVPEntities())
                {
                    filePath = db.tblGeneralSettings.Select(s => s.FilePath).FirstOrDefault();
                }

                if (!filePath.EndsWith("\\"))
                {
                    filePath += "\\";
                }

                switch (FileType)
                {
                    case "Regular":
                        filePath += "paygl.txt";
                        glType = "R";
                        break;
                    case "Pension":
                        filePath += "payglPEN.txt";
                        glType = "P";
                        break;
                    case "Longevity":
                        filePath += "payglLONG.txt";
                        glType = "L";
                        break;
                    case "N99":
                        filePath += "payglN99.txt";
                        glType = "N";
                        break;
                }

                if (!System.IO.File.Exists(filePath))
                {
                    return null;
                }

                StreamReader _reader = System.IO.File.OpenText(filePath);
                string line;

                GeneralLedgerViewModel gl = new GeneralLedgerViewModel();
                GLTransactionDetailViewModel td;
                while ((line = _reader.ReadLine()) != null)
                {
                    var recordType = line.Substring(0, 2);
                    switch (recordType)
                    {
                        case "BH":
                        case "JH":
                            ParseGeneralLedger(line, glType, ref gl);
                            break;
                        case "TD":
                            td = ParseTransactionDetail(line, TransactionDate);
                            var isValid = VerifyTransactionDate(ref td, TransactionDate);
                            if (!isValid)
                            {
                                List<GLTransactionDetailViewModel> listTransactionDetails = new List<GLTransactionDetailViewModel>()
                                {
                                    td
                                };

                                GeneralLedgerViewModel emptyGL = new GeneralLedgerViewModel()
                                {
                                    BatchType = "ZZ",
                                    TransactionDetails = listTransactionDetails
                                };
                                return emptyGL;
                            }
                            gl.TransactionDetails.Add(td);
                            break;
                    }
                }
                _reader.Dispose();
                return gl;
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(49, ex, CoWSEventSource.AppSection.Payroll);
                return null;
            }
        }

        public int CreateGeneralLedger(ref GeneralLedgerViewModel gl)
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    var entity = new tblGeneralLedger
                    {
                        GLType = gl.GLType,
                        BatchType = gl.BatchType,
                        BatchTotal = gl.BatchTotal,
                        GLComments = gl.GLComments,
                        JournalComments = gl.JournalComments,
                        Ledger = gl.Ledger,
                        Period = gl.Period,
                        Year = gl.Year,
                        UserCode = gl.UserCode,
                        DateModified = gl.DateModified,
                        WhoModified = gl.WhoModified
                    };
                    db.tblGeneralLedgers.Add(entity);
                    db.SaveChanges();
                    gl.GLID = entity.GLID;
                }
                return gl.GLID;
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(50, ex, CoWSEventSource.AppSection.Payroll);
                return -1;
            }
        }

        public int CreateTransactionDetails(ref GeneralLedgerViewModel gl)
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    foreach (var td in gl.TransactionDetails) 
                    {
                        var entity = new tblGLTransactionDetail
                        {
                            GLID = gl.GLID,
                            TransactionAmount = td.TransactionAmount,
                            TransactionDate = td.TransactionDate,
                            Comments = td.Comments,
                            Fund = td.Fund,
                            ResponsibilityCode = td.ResponsibilityCode,
                            ObjectCode = td.ObjectCode,
                            ProgramCode = td.ProgramCode,
                            DateModified = td.DateModified,
                            WhoModified = td.WhoModified
                        };
                        db.tblGLTransactionDetails.Add(entity);
                        db.SaveChanges();
                        td.TransactionID = entity.TransactionID;                        
                    }                    
                }
                return 0;
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(51, ex, CoWSEventSource.AppSection.Payroll);
                return -1;
            }
        }

        public ActionResult SubmitActualsBatch(string GeneralLedgerType, DateTime TransactionDate)
        {
            try
            {
                var glRecords = GetGeneralLedgerRecords(GeneralLedgerType, TransactionDate);
                var gl = PopulateGeneralLedgerViewModel(glRecords);
                var actualsBatch = CreateActualsBatch(gl);
                var response = CallCreateActualsBatch(actualsBatch, gl.Ledger, gl.GLID);

                foreach (var item in response)
                {
                    if (item.Key == "BatchID")
                    {
                        UpdateGeneralLedger(item.Value, gl.GLID);
                    }
                }
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(52, ex, CoWSEventSource.AppSection.Payroll);
                return Json(ex.Message);
            }
        }

        private List<GLTDViewModel> GetGeneralLedgerRecords(string GeneralLedgerType, DateTime TransactionDate)
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    List<GLTDViewModel> glRecords = (from gl in db.tblGeneralLedgers
                                                     join td in db.tblGLTransactionDetails
                                                     on gl.GLID equals td.GLID
                                                     where gl.GLType == GeneralLedgerType
                                                     where td.TransactionDate == TransactionDate
                                                     select new GLTDViewModel()
                                                     {
                                                        BatchNumber = gl.BatchNumber,
                                                        BatchTotal = gl.BatchTotal,
                                                        BatchType = gl.BatchType,
                                                        Comments = td.Comments,
                                                        DateModified = gl.DateModified,
                                                        Fund = td.Fund,
                                                        GLComments = gl.GLComments,
                                                        GLID = gl.GLID,
                                                        GLType = gl.GLType,
                                                        JournalComments = gl.JournalComments,
                                                        Ledger = gl.Ledger,
                                                        ObjectCode = td.ObjectCode,
                                                        Period = gl.Period,
                                                        ProgramCode = td.ProgramCode,
                                                        ResponsibilityCode = td.ResponsibilityCode,
                                                        TransactionAmount = td.TransactionAmount,
                                                        TransactionDate = td.TransactionDate,
                                                        TransactionID = td.TransactionID,
                                                        UserCode = gl.UserCode,
                                                        WhoModified = gl.WhoModified,
                                                        Year = gl.Year                                                        
                                                     }).ToList();
                    return glRecords;
                }
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(53, ex, CoWSEventSource.AppSection.Payroll);
                return null;
            }
        }

        private GeneralLedgerViewModel PopulateGeneralLedgerViewModel(List<GLTDViewModel> glRecords)
        {
            try
            {
                GeneralLedgerViewModel glvm = new GeneralLedgerViewModel()
                {
                    GLID = glRecords[0].GLID,
                    BatchType = glRecords[0].BatchType,
                    BatchTotal = glRecords[0].BatchTotal,
                    GLComments = glRecords[0].GLComments,
                    JournalComments = glRecords[0].JournalComments,
                    Ledger = glRecords[0].Ledger,
                    Period = glRecords[0].Period,
                    Year = glRecords[0].Year,
                    UserCode = glRecords[0].UserCode,
                    DateModified = glRecords[0].DateModified,
                    WhoModified = glRecords[0].WhoModified
                };

                foreach (var glr in glRecords)
                {
                    GLTransactionDetailViewModel td = new GLTransactionDetailViewModel()
                    {
                        TransactionID = glr.TransactionID,
                        TransactionAmount = glr.TransactionAmount,
                        TransactionDate = glr.TransactionDate,
                        Comments = glr.Comments,
                        Fund = glr.Fund,
                        ResponsibilityCode = glr.ResponsibilityCode,
                        ObjectCode = glr.ObjectCode,
                        ProgramCode = glr.ProgramCode,
                        GLID = glr.GLID,
                        DateModified = glr.DateModified,
                        WhoModified = glr.WhoModified
                    };

                    glvm.TransactionDetails.Add(td);
                }
                return glvm;
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(54, ex, CoWSEventSource.AppSection.Payroll);
                return null;
            }
        }

        private ActualsBatch CreateActualsBatch(GeneralLedgerViewModel glvm)
        {
            try
            {
                var txCount = glvm.TransactionDetails.Count();

                // create an array of transactions
                ActualsTransaction[] actualsTransactions = new ActualsTransaction[txCount];
                var j = 0;

                // iterate through the transactions found within the current document
                foreach (var td in glvm.TransactionDetails)
                {
                    var transactionDate = td.TransactionDate.ToString("yyyy-MM-dd");

                    // create a new transaction from the Batch.DocumentNumber.TransactionDetail
                    ActualsTransaction actualsTransaction = new ActualsTransaction()
                    {
                        Account = td.Fund + td.ResponsibilityCode + td.ObjectCode + td.ProgramCode,
                        DataType = "FFF",
                        Amount = td.TransactionAmount,
                        RefDate = td.TransactionDate.ToString("yyyy-MM-dd"),
                        Comment = td.Comments
                    };
                    actualsTransactions[j] = actualsTransaction;
                    j++;
                }
                txCount = j;

                ActualsJournal actualsJournal = new ActualsJournal()
                {
                    JournalId = "1",
                    Desc1 = glvm.JournalComments,
                    Reverse = false,
                    Td = actualsTransactions
                };

                ActualsJournal[] actualsJournals = new ActualsJournal[1];
                actualsJournals[0] = actualsJournal;

                // create a new GL record to include the array of TransactionDetails from above
                ActualsBatch inputRec = new ActualsBatch()
                {
                    BatchType = glvm.BatchType,
                    BatchNo = 0,
                    User = glvm.UserCode,
                    Period = glvm.Period,
                    Year = glvm.Year,
                    FinCtl = glvm.BatchTotal,
                    DebitCtl = 0,
                    StatCtl = 0,
                    HashCtl = 0,
                    JournalCtl = 1,
                    TxCtl = txCount,
                    Gencon = "GLDE",
                    OutputDevice = "DISK",
                    UnconditionalUpdate = true,
                    RptHdr1 = "",
                    RptHdr2 = glvm.GLComments,
                    Journal = actualsJournals
                };
                return inputRec;
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(55, ex, CoWSEventSource.AppSection.Payroll);
                return null;
            }
        }

        private List<KeyValuePair<string, string>> CallCreateActualsBatch(ActualsBatch actualsBatch, string ledger, int GLID)
        {
            KeyValuePair<string, string> _keyValuePair;
            List<KeyValuePair<string, string>> _status = new List<KeyValuePair<string, string>>();
            try
            {
                ActualsBatchStatus statusRec = new ActualsBatchStatus();
                var _request = actualsBatch.ToXML();

                using (var db = new CSVPEntities())
                {
                    var wsSettings = db.tblWebServiceSettings.FirstOrDefault();
                    var result = 0;
                    using (FMSWebServicesSoapClient glClient = new FMSWebServicesSoapClient("FMSWebService", wsSettings.WebServiceURL))
                    {
                        result = glClient.CreateActualsBatch(wsSettings.FMSUser, wsSettings.FMSPassword1, wsSettings.FMSPassword2, wsSettings.FMSPassword3, ledger, wsSettings.OSUser, wsSettings.OSPassword, actualsBatch, out statusRec);
                    }

                    if (result > 0)
                    {
                        _keyValuePair = _cc.GetErrorResultMessage(result);
                        var _response = statusRec.ToXML();
                        _cc.SaveSoapXML("GL", GLID, _request, _response);
                        _status.Add(_keyValuePair);
                        return _status;
                    }
                }
                if (statusRec.Overall == "N")
                {
                    var _response = statusRec.ToXML();
                    _cc.SaveSoapXML("GL", GLID, _request, _response);
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
                _evtSource.HandleException(56, ex, CoWSEventSource.AppSection.Payroll);
                _status.Add(new KeyValuePair<string, string>("ErrorMessage", ex.Message));
                return _status;
            }
        }
        
        private void UpdateGeneralLedger(string BatchNumber, int GLID)
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    var entity = db.tblGeneralLedgers.Where(w => w.GLID == GLID).FirstOrDefault();

                    if (entity != null)
                    {
                        var userID = _cc.GetUserID(User.Identity.Name);
                        entity.BatchNumber = BatchNumber;
                        entity.DateModified = DateTime.Now;
                        entity.WhoModified = userID;
                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(57, ex, CoWSEventSource.AppSection.Payroll);
                throw ex;
            }
        }

        public void ParseGeneralLedger(string line, string glType, ref GeneralLedgerViewModel gl)
        {
            if (line.Substring(0, 2).Trim() == "BH") {
                gl.GLType = glType;
                gl.BatchType = line.Substring(2, 2).Trim();
                gl.Ledger = line.Substring(10, 5).Trim();
                gl.Period = line.Substring(16, 2).Trim();
                gl.Year = line.Substring(18, 4).Trim();
                gl.UserCode = line.Substring(22, 3).Trim();
                gl.BatchTotal = Convert.ToDecimal(line.Substring(66, 19));
                gl.GLComments = line.Substring(116, 35).Trim();
                gl.DateModified = glbDateModified;
                gl.WhoModified = glbWhoModified;
            }
            else
            {
                gl.JournalComments = line.Substring(11, 35).Trim();
            }
        }

        public GLTransactionDetailViewModel ParseTransactionDetail(string line, string passedTransactionDate)
        {
            
            GLTransactionDetailViewModel td = new GLTransactionDetailViewModel();
            td.TransactionDate = new DateTime(CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(Convert.ToInt32(line.Substring(110, 2).Trim())),
                                                                                                 Convert.ToInt32(line.Substring(106, 2).Trim()),
                                                                                                 Convert.ToInt32(line.Substring(108, 2).Trim()));
            DateTime.TryParse(passedTransactionDate, out DateTime transactionDate);

            if (td.TransactionDate != transactionDate)
            {
                return td;
            }

            if (line.Substring(85, 1) == "-")
            {
                td.TransactionAmount = Decimal.Negate(Convert.ToDecimal(line.Substring(66, 19)));
            }
            else
            {
                td.TransactionAmount = Convert.ToDecimal(line.Substring(66, 19));
            }
            td.TransactionDate = new DateTime(CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(Convert.ToInt32(line.Substring(110, 2).Trim())),
                                                                                                  Convert.ToInt32(line.Substring(106, 2).Trim()),
                                                                                                  Convert.ToInt32(line.Substring(108, 2).Trim()));
            td.Fund = line.Substring(2, 3).Trim();
            td.ResponsibilityCode = line.Substring(2, 6).Trim();
            td.ObjectCode = line.Substring(8, 6).Trim();
            td.ProgramCode = line.Substring(14, 6).Trim();
            td.Comments = line.Substring(112, 35).Trim();
            td.DateModified = glbDateModified;
            td.WhoModified = glbWhoModified;
            
            return td;
        }

        private bool VerifyTransactionDate(ref GLTransactionDetailViewModel td, string passedTransactionDate)
        {
            DateTime.TryParse(passedTransactionDate, out DateTime parsedDate);

            if (td.TransactionDate != parsedDate)
            {
                return false;
            }
            return true;
        }

        public JsonResult UpdateBatchComment(int GLID, string Comments)
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    var gl = db.tblGeneralLedgers.Where(w => w.GLID == GLID).FirstOrDefault();
                    if (gl != null)
                    {
                        gl.GLComments = Comments;
                        gl.JournalComments = Comments;
                    }
                    db.Entry(gl).State = EntityState.Modified;

                    var tdList = db.tblGLTransactionDetails.Where(w => w.GLID == GLID).ToList();

                    foreach (var td in tdList)
                    {
                        td.Comments = Comments;
                        db.Entry(td).State = EntityState.Modified;                        
                    }
                    db.SaveChanges();
                }
                return Json("Success");
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(60, ex, CoWSEventSource.AppSection.Payroll);
                return Json(ex.Message);
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
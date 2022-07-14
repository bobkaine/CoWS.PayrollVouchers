using CoWS.PayrollVouchers.Models;
using CoWS.PayrollVouchers.ViewModels;
using CoWS.Web.Logging;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace CoWS.PayrollVouchers.Controllers
{
    [Authorize(Roles = "App Admin")]
    public class AdminController : Controller
    {
        private CoWSEventSource _evtSource = new CoWSEventSource();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetWebServiceSettings()
        {
            using (var db = new CSVPEntities())
            {
                try
                {
                    var wsSettings = db.tblWebServiceSettings.FirstOrDefault();
                    return Json(wsSettings, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    _evtSource.HandleException(2, ex, CoWSEventSource.AppSection.Admin);
                    return Json(ex.Message);
                }
            }
        }

        public JsonResult GetGeneralSettings()
        {
            using (var db = new CSVPEntities())
            {
                try
                {
                    var gSettings = db.tblGeneralSettings.FirstOrDefault();
                    return Json(gSettings, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    _evtSource.HandleException(3, ex, CoWSEventSource.AppSection.Admin);
                    return Json(ex.Message);
                }
            }
        }

        public JsonResult SaveWebServiceSettings(WebServiceSettingsViewModel wsSettings)
        {
            CommonController cc = new CommonController();
            using (var db = new CSVPEntities())
            {
                try
                {
                    var entity = db.tblWebServiceSettings.Where(w => w.ID == wsSettings.ID).FirstOrDefault();

                    if (entity != null) //Update
                    {
                        if (entity.FMSUser != wsSettings.FMSUser) // if value has changed
                        {
                            entity.FMSUser = wsSettings.FMSUser;
                        }

                        if (entity.FMSPassword1 != wsSettings.FMSPassword1)
                        {
                            entity.FMSPassword1 = wsSettings.FMSPassword1;
                        }
                                               
                        if (entity.FMSPassword2 != wsSettings.FMSPassword2)
                        {
                            entity.FMSPassword2 = wsSettings.FMSPassword2;
                        }

                        if (entity.FMSPassword3 != wsSettings.FMSPassword3)
                        {
                            entity.FMSPassword3 = wsSettings.FMSPassword3;
                        }

                        if (entity.Ledger != wsSettings.Ledger)
                        {
                            entity.Ledger = wsSettings.Ledger;
                        }

                        if (entity.OSUser != wsSettings.OSUser)
                        {
                            entity.OSUser = wsSettings.OSUser;
                        }

                        if (entity.OSPassword != wsSettings.OSPassword)
                        {
                            entity.OSPassword = wsSettings.OSPassword;
                        }

                        if (entity.OutputDevice != wsSettings.OutputDevice)
                        {
                            entity.OutputDevice = wsSettings.OutputDevice;
                        }

                        if (entity.WebServiceURL != wsSettings.WebServiceURL)
                        {
                            entity.WebServiceURL = wsSettings.WebServiceURL;
                        }

                        entity.DateModified = DateTime.Now;
                        entity.WhoModified = cc.GetUserID(User.Identity.Name);

                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();                        
                    }
                    else // New Record
                    {
                        var newEntity = new tblWebServiceSetting
                        {
                            FMSUser = wsSettings.FMSUser,
                            FMSPassword1 = wsSettings.FMSPassword1,
                            FMSPassword2 = wsSettings.FMSPassword2,
                            FMSPassword3 = wsSettings.FMSPassword3,
                            Ledger = wsSettings.Ledger,
                            OSUser = wsSettings.OSUser,
                            OSPassword = wsSettings.OSPassword,
                            OutputDevice = wsSettings.OutputDevice,
                            WebServiceURL = wsSettings.WebServiceURL,
                            DateModified = DateTime.Now,
                            WhoModified = cc.GetUserID(User.Identity.Name)
                        };
                        db.tblWebServiceSettings.Add(newEntity);
                        db.SaveChanges();
                    }
                    return Json(0);
                }
                catch (Exception ex)
                {
                    _evtSource.HandleException(4, ex, CoWSEventSource.AppSection.Admin);
                    return Json(-1);                    
                }
                finally
                {
                    cc.Dispose();
                }
            }
        }

        public JsonResult SaveGeneralSettings(GeneralSettingsViewModel gSettings)
        {
            CommonController cc = new CommonController();
            using (var db = new CSVPEntities())
            {
                try
                {
                    var entity = db.tblGeneralSettings.Where(w => w.ID == gSettings.ID).FirstOrDefault();

                    if (entity != null) //Update
                    {
                        if (entity.BankNumber != gSettings.BankNumber) // if value has changed
                        {
                            entity.BankNumber = gSettings.BankNumber;
                        }

                        if (entity.ChildSupportDesc != gSettings.ChildSupportDesc)
                        {
                            entity.ChildSupportDesc = gSettings.ChildSupportDesc;
                        }

                        if (entity.FilePath != gSettings.FilePath)
                        {
                            entity.FilePath = gSettings.FilePath;
                        }

                        if (entity.UserCode != gSettings.UserCode)
                        {
                            entity.UserCode = gSettings.UserCode;
                        }

                        if (entity.VouchersPayableDesc != gSettings.VouchersPayableDesc)
                        {
                            entity.VouchersPayableDesc = gSettings.VouchersPayableDesc;
                        }

                        if (entity.TempPassword != gSettings.TempPassword)
                        {
                            entity.TempPassword = gSettings.TempPassword;
                        }

                        entity.DateModified = DateTime.Now;
                        entity.WhoModified = cc.GetUserID(User.Identity.Name);

                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else // New Record
                    {
                        var newEntity = new tblGeneralSetting
                        {
                            BankNumber = gSettings.BankNumber,
                            ChildSupportDesc = gSettings.ChildSupportDesc,
                            FilePath = gSettings.FilePath,
                            UserCode = gSettings.UserCode,
                            VouchersPayableDesc = gSettings.VouchersPayableDesc,
                            TempPassword = gSettings.TempPassword,
                            DateModified = DateTime.Now,
                            WhoModified = cc.GetUserID(User.Identity.Name)
                        };
                        db.tblGeneralSettings.Add(newEntity);
                        db.SaveChanges();
                    }
                    return Json(0);
                }
                catch (Exception ex)
                {
                    _evtSource.HandleException(5, ex, CoWSEventSource.AppSection.Admin);
                    return Json(-1);
                }
                finally
                {
                    cc.Dispose();
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
            }

            base.Dispose(disposing);
        }
    }
}
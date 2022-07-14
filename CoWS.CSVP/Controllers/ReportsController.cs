using CoWS.PayrollVouchers.Models;
using CoWS.PayrollVouchers.Reporting;
using CoWS.Web.Logging;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace CoWS.PayrollVouchers.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private CoWSEventSource _evtSource = new CoWSEventSource();
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public void GenerateReport(string ReportName, string ReportParams, string ExportFormat)
        {
            ReportDocument rd = new ReportDocument();
            try
            {
                var response = HttpContext.ApplicationInstance.Response;
                response.Clear();
                response.Buffer = false;
                response.ClearContent();
                response.ClearHeaders();
                response.Cache.SetCacheability(HttpCacheability.Public);
                response.ContentType = "application/pdf";

                var reportParams = JsonConvert.DeserializeObject<List<ReportParameter>>(ReportParams);
                var reportPath = Path.Combine(Server.MapPath("~/Reporting/Reports"), ReportName + ".rpt");

                var db = new CSVPEntities();
                var server = db.Database.Connection.DataSource;
                var database = db.Database.Connection.Database;

                rd.Load(reportPath);
                rd.DataSourceConnections[0].SetConnection(server, database, true);

                foreach (ReportParameter param in reportParams)
                {
                    rd.SetParameterValue(param.Name, param.Value);
                }

                switch (ExportFormat)
                {
                    case "5":
                        rd.ExportToHttpResponse(ExportFormatType.PortableDocFormat, response, true, ReportName);
                        break;
                    case "4":
                        rd.ExportToHttpResponse(ExportFormatType.Excel, response, true, ReportName);
                        break;
                    case "15":
                        rd.ExportToHttpResponse(ExportFormatType.ExcelWorkbook, response, true, ReportName);
                        break;
                    case "3":
                        rd.ExportToHttpResponse(ExportFormatType.WordForWindows, response, true, ReportName);
                        break;
                    case "11":
                        rd.ExportToHttpResponse(ExportFormatType.TabSeperatedText, response, true, ReportName);
                        break;
                    default:
                        rd.ExportToHttpResponse(ExportFormatType.PortableDocFormat, response, true, ReportName);
                        break;
                }
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(30, ex, CoWSEventSource.AppSection.Reports);
            }
            finally
            {
                rd.Close();
                rd.Dispose();
            }
        }

        public JsonResult GetReports(int? ReportType)
        {
            List<ReportItem> reports = new List<ReportItem>();

            try
            {
                if (ReportType != null)
                {
                    switch (ReportType)
                    {
                        case 1: // Child Support
                            reports.Add(new ReportItem { Text = "Preliminary Summary", Value = "1" });
                            reports.Add(new ReportItem { Text = "Post Summary", Value = "2" });
                            reports.Add(new ReportItem { Text = "Voucher Pages", Value = "3" });
                            break;
                        case 2: // Vouchers Payable
                            reports.Add(new ReportItem { Text = "Preliminary Summary", Value = "1" });
                            reports.Add(new ReportItem { Text = "Post Summary", Value = "2" });
                            reports.Add(new ReportItem { Text = "Voucher Pages", Value = "3" });
                            break;
                        case 3: // Payroll
                            reports.Add(new ReportItem { Text = "Post Summary", Value = "1" });
                            break;
                        default:
                            break;
                    }
                }
                return Json(reports, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(31, ex, CoWSEventSource.AppSection.Reports);
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

    public class ReportItem
    {
        public string Text;
        public string Value;
    }
}
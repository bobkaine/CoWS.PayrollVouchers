using System.Web.Mvc;

namespace CoWS.PayrollVouchers.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Payroll Vouchers";

            return View();
        }
    }
}
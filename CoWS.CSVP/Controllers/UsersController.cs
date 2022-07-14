using CoWS.PayrollVouchers.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoWS.Web.Logging;

namespace CoWS.PayrollVouchers.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private CoWSEventSource _evtSource = new CoWSEventSource();

        // GET: Users
        public Boolean isSecurityAdmin()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity;
                    ApplicationDbContext context = new ApplicationDbContext();
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                    var s = UserManager.GetRoles(user.GetUserId());

                    for (int i = 0; i < s.Count; i++)
                    {
                        if (s[i].ToString() == "Security Admin")
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(40, ex, CoWSEventSource.AppSection.Users);
                return false;
            }
        }

        public Boolean isAppAdmin()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity;
                    ApplicationDbContext context = new ApplicationDbContext();
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                    var s = UserManager.GetRoles(user.GetUserId());

                    for (int i = 0; i < s.Count; i++)
                    {
                        if (s[i].ToString() == "App Admin")
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(41, ex, CoWSEventSource.AppSection.Users);
                return false;
            }
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;
                ViewBag.displayMenu = "No";

                if (isSecurityAdmin())
                {
                    ViewBag.displayMenu = "Yes";
                }
                return View();
            }
            else
            {
                ViewBag.Name = "Not Logged IN";
            }

            return View();
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
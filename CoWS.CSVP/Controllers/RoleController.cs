using CoWS.PayrollVouchers.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CoWS.PayrollVouchers.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        ApplicationDbContext _context;

        public RoleController()
        {
            _context = new ApplicationDbContext();
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            if (User.Identity.IsAuthenticated)
            {


                if (!isSecurityAdmin())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            var Roles = _context.Roles.ToList();
            return View(Roles);

        }
        public Boolean isSecurityAdmin()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
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
        /// <summary>
        /// Create  a New role
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {


                if (!isSecurityAdmin())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            var Role = new IdentityRole();
            return View(Role);
        }

        /// <summary>
        /// Create a New Role
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(IdentityRole Role)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!isSecurityAdmin())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            _context.Roles.Add(Role);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
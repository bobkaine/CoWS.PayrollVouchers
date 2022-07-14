using CoWS.PayrollVouchers.Models;
using CoWS.PayrollVouchers.ViewModels;
using CoWS.Web.Logging;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CoWS.PayrollVouchers.Controllers
{
    [Authorize(Roles = "Security Admin")]
    public class SecurityController : Controller
    {
        private CoWSEventSource _evtSource = new CoWSEventSource();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationDbContext context;

        public SecurityController()
        {
            context = new ApplicationDbContext();
        }

        public SecurityController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult CreateNewUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateNewUser(UserViewModel uvm)
        {
            if (!ModelState.IsValid)
            {
                return View(uvm);
            }

            try
            {
                var user = new ApplicationUser { UserName = uvm.FirstName + "." + uvm.LastName, Email = uvm.Email, FirstName = uvm.FirstName, LastName = uvm.LastName, Active = true };
                var tempPassword = GetTempPassword();
                var result = await UserManager.CreateAsync(user, tempPassword);
                if (result.Succeeded)
                {
                    //Assign Roles to user Here 
                    foreach (var userRole in uvm.UserRoles)
                    {
                        await this.UserManager.AddToRoleAsync(user.Id, userRole.ToString());
                    }
                    ViewBag.SuccessMessage = "User successfully created.";
                    return View();
                }
                AddErrors(result);
                ViewBag.ErrorMessage = "User create failed.";
                return View(uvm);
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(32, ex, CoWSEventSource.AppSection.Security);
                ModelState.AddModelError("", ex.Message);
                ViewBag.ErrorMessage = "User create failed.";
                return View(uvm); 
            }
        }

        public ActionResult ModifyUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ModifyUser(string userID)
        {
            try
            {
                var user = UserManager.FindById(userID);
                if (user != null)
                {
                    UserViewModel model = new UserViewModel()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        UserName = user.UserName,
                        UserID = user.Id,
                        Active = user.Active

                    };

                    var userRoles = new List<string>();
                    foreach (var userRole in user.Roles)
                    {
                        userRoles.Add(GetRole(userRole.RoleId));
                    }
                    model.UserRoles = userRoles;
                    return View(model);
                }
                return View();
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(33, ex, CoWSEventSource.AppSection.Security);
                return View();
            }
        }

        public ActionResult UpdateUser()
        {
            return View("ModifyUser");
        }

        [HttpPost]
        public async Task<ActionResult>UpdateUser(UserViewModel uvm)
        {
            if (!ModelState.IsValid)
            {
                return View("ModifyUser", uvm);
            }

            try
            {
                var user = UserManager.FindById(uvm.UserID);

                if (user.FirstName != uvm.FirstName || user.LastName != uvm.LastName)
                {
                    user.FirstName = uvm.FirstName;
                    user.LastName = uvm.LastName;
                    user.UserName = uvm.FirstName + "." + uvm.LastName;
                    uvm.UserName = user.UserName;
                }
                user.Email = uvm.Email;
                user.Active = uvm.Active;

                var roles = await UserManager.GetRolesAsync(user.Id);
                await UserManager.RemoveFromRolesAsync(user.Id, roles.ToArray());

                //Assign New Roles to user Here 
                foreach (var userRole in uvm.UserRoles)
                {
                    await UserManager.AddToRoleAsync(user.Id, userRole.ToString());
                }

                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    ViewBag.SuccessMessage = "User successfully updated.";
                    return View("ModifyUser", uvm);
                }
                AddErrors(result);
                ViewBag.ErrorMessage = "User update failed.";
                return View("ModifyUser",uvm);
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(34, ex, CoWSEventSource.AppSection.Security);
                ModelState.AddModelError("", ex.Message);
                ViewBag.ErrorMessage = "User update failed.";
                return View("ModifyUser", uvm);
            }
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(string userID)
        {
            try
            {
                var token = await UserManager.GeneratePasswordResetTokenAsync(userID);
                var tempPassword = GetTempPassword();
                var result = await UserManager.ResetPasswordAsync(userID, token, tempPassword);
                if (result.Succeeded)
                {
                    ViewBag.SuccessMessage = "Password Reset Successful.";
                    return Json("Success");
                }
                AddErrors(result);
                ViewBag.SuccessMessage = "Password Reset Failed.";
                return Json(null);
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(35, ex, CoWSEventSource.AppSection.Security);
                ViewBag.SuccessMessage = "Password Reset Failed.";
                return Json(null);
            }
        }

        private string GetTempPassword()
        {
            try
            {
                using (var db = new CSVPEntities())
                {
                    var tempPassword = db.tblGeneralSettings.Select(s => s.TempPassword).FirstOrDefault();
                    return tempPassword;
                }
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(36, ex, CoWSEventSource.AppSection.Security);
                return null;
            }
        }

        public JsonResult GetUserRoles()
        {
            try
            {
                return Json(new ApplicationDbContext().Roles.Select(s => new { Name = s.Name }).ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(37, ex, CoWSEventSource.AppSection.Security);
                return Json(null);
            }
        }

        public JsonResult GetUsers()
        {
            try
            {
                var users = context.Users.Select(s => new { FullName = s.LastName + ", " + s.FirstName, UserID = s.Id }).OrderBy(o => o.FullName).ToList();
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(38, ex, CoWSEventSource.AppSection.Security);
                return Json(ex.Message);
            }
        }

        private string GetRole(string roleID)
        {
            try
            {
                return context.Roles.Where(w => w.Id == roleID).Select(s => s.Name).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _evtSource.HandleException(39, ex, CoWSEventSource.AppSection.Security);
                return null;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }

                if(_evtSource != null)
                {
                    _evtSource.Dispose();
                    _evtSource = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
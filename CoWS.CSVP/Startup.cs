using CoWS.PayrollVouchers.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartupAttribute(typeof(CoWS.PayrollVouchers.Startup))]
namespace CoWS.PayrollVouchers
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesandUsers();
        }

        // In this method we will create default User roles and Admin user for login
        private void CreateRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // In Startup Create an Security Admin role, Admin role, and then an Super User with both roles
            if (!roleManager.RoleExists("Security Admin"))
            {
                // First we create Security Admin Role
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Security Admin";
                roleManager.Create(role);

                // Create the App Admin role 
                if (!roleManager.RoleExists("App Admin"))
                {
                    role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                    role.Name = "App Admin";
                    roleManager.Create(role);
                }

                // Then we create a Super User			
                var user = new ApplicationUser();
                user.UserName = "SuperUser";
                user.Email = "bob@kaine-enterprises.com";

                string userPWD = "Password!1";

                var chkUser = userManager.Create(user, userPWD);

                //Add SuperUser to Security Admin and App Admin roles
                if (chkUser.Succeeded)
                {
                    var result1 = userManager.AddToRole(user.Id, "Security Admin");
                    var result2 = userManager.AddToRole(user.Id, "App Admin");
                }
            }
            
            // Create the AP Processor role 
            if (!roleManager.RoleExists("AP Processor"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "AP Processor";
                roleManager.Create(role);
            }

            // Create the GL Processor role 
            if (!roleManager.RoleExists("GL Processor"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "GL Processor";
                roleManager.Create(role);
            }

            // Create the Reviewer role 
            if (!roleManager.RoleExists("Reviewer"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Reviewer";
                roleManager.Create(role);
            }
        }
    }
}
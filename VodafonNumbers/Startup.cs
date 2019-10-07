using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Configuration;
using VodafonNumbers.Models;

[assembly: OwinStartupAttribute(typeof(VodafonNumbers.Startup))]
namespace VodafonNumbers
{
    public partial class Startup
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //CreateDefualtRoleAndUser();
            app.MapSignalR();
        }

        public void CreateDefualtRoleAndUser()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            IdentityRole role = new IdentityRole();
            if (!roleManager.RoleExists("Admins"))
            {
                role.Name = "Admins";
                roleManager.Create(role);
                ApplicationUser user = new ApplicationUser();
                user.UserName = "Hendy";
                user.Email = "hendy@gmail.com";
                var check = userManager.Create(user, "123456");
                if (check.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admins");
                }
            }
        }


    }
}

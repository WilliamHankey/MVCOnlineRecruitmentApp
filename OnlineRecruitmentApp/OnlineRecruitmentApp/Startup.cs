using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using OnlineRecruitmentApp.Models;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineRecruitmentApp.Startup))]
namespace OnlineRecruitmentApp
{
    public partial class Startup
    {
        public async void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            await createRolesandUsersAsync();
        }
        // In this method we will create default User roles and Admin user for login   
        private async System.Threading.Tasks.Task createRolesandUsersAsync()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));


            // In Startup i am creating first Admin Role and creating a default Admin User
            if (!roleManager.RoleExists("Admin"))
            {
                // first we create Admin role   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                // Here we create a user and grant him/her the Admin role.
                // The default behaviour when creating users is a bit weird.
                // When users register, only their emails and passwords are required,
                // but these emails are stored in the database as both emails and usernames.
                // During login, the emails are again requested, but then compared to the
                // username in the DB. This behaviour can be changed, but for now we will
                // just go with the flow. Since we are directly creating both username and
                // email here, we ensure that they are both the same.
                string myEmail = "myself@gmail.com";
                string myPassword = "A@b123456";
                var user = new ApplicationUser { UserName = myEmail, Email = myEmail };
                var result = await UserManager.CreateAsync(user, myPassword);
                if (result.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }
            }

            // Creating Manager role    
            if (!roleManager.RoleExists("Manager"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Manager";
                roleManager.Create(role);
            }

        }
    }
}

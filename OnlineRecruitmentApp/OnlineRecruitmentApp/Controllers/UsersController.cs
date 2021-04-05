using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlineRecruitmentApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnlineRecruitmentApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        ApplicationDbContext db;

        // Constructor
        public UsersController() // if this line gives error – check controller name capital letters
        {
            db = new ApplicationDbContext();
        }

        // GET: Users
        public ActionResult Index()
        {
            // Get the users from the identity table
            var Users = db.Users.ToList();
            return View(Users);
        }

        // GET:
        public ActionResult EditUserRoles(string userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the user
            Microsoft.AspNet.Identity.EntityFramework.IdentityUser user = db.Users.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Get a list of all the available roles - to display in the dropdown
            var roles = db.Roles.ToList();

            // We want to highlight the user's existing roles (if any) in the dropdown
            // Get all the names of the user's roles using the received user's id
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            List<string> userRolesNamesList = UserManager.GetRoles(userId).ToList();

            // Use the names to get the complete role objects (including their IDs) of the users roles
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            List<IdentityRole> userRolesList = RoleManager.Roles.Where(r => userRolesNamesList.Contains(r.Name)).ToList();

            // Create an array containing the user's existing roles' IDs (if any)
            string[] userRolesIDsArray = new string[userRolesList.Count];
            int index = 0;
            foreach (IdentityRole role in userRolesList)
            {
                userRolesIDsArray[index] = role.Id; // This is the role id - not the user id we used earlier
                index++;
            }

            // Add the roles to the viewbag, since it is treated as "additional data" 
            // (it is not the user's data normally associated with a controller action
            // like this)
            ViewBag.Roles = new MultiSelectList(roles, "Id", "Name", userRolesIDsArray);

            return View(user);
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUserRoles(FormCollection collection)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            // Because the form contains data from two different models (user
            // and roles), we use a FormCollection to extract the data manually.
            // Alternatively, we could have created a special viewmodel just for
            // this purpose - which would also enable easier validation.

            // Extract the user's id
            string userId = collection["id"];
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the user
            Microsoft.AspNet.Identity.EntityFramework.IdentityUser user = db.Users.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Extract the Role IDs from the collection (in a single 
            // text string - separated by commas)
            string RoleIdString = collection["RoleId"];
            if (RoleIdString != null)
            {
                // Remove all of the user's existing Roles
                var userRoles = await UserManager.GetRolesAsync(userId);
                await UserManager.RemoveFromRolesAsync(userId, userRoles.ToArray());

                // Add the user's new Roles
                // Extract the roles from the text string into an array (by commas)
                string[] RoleIdArray = RoleIdString.Split(',');
                foreach (string roleId in RoleIdArray)
                {
                    // Retrieve the role, because you need its Name below
                    IdentityRole role = RoleManager.FindById(roleId);
                    // Add the user to the role
                    UserManager.AddToRole(userId, role.Name);
                }
            }

            /* Repeat the processes used in the "GET" EditUserRoles action
               to display the user's new Roles */
            // Get a list of all the available roles - to display in the dropdown
            var roles = db.Roles.ToList();

            // We want to highlight the user's existing roles (if any) in the dropdown
            // Get all the names of the user's roles using the received user's id
            List<string> userRolesNamesList = UserManager.GetRoles(userId).ToList();

            // Use the names to get the complete role objects (including their IDs) of the users roles
            List<IdentityRole> userRolesList = RoleManager.Roles.Where(r => userRolesNamesList.Contains(r.Name)).ToList();

            // Create an array containing the user's existing roles' IDs (if any)
            string[] userRolesIDsArray = new string[userRolesList.Count];
            int index = 0;
            foreach (IdentityRole role in userRolesList)
            {
                userRolesIDsArray[index] = role.Id; // This is the role id - not the user id we used earlier
                index++;
            }

            // Add the roles to the viewbag, since it is treated as "additional data" 
            // (it is not the user's data normally associated with a controller action
            // like this)
            ViewBag.Roles = new MultiSelectList(roles, "Id", "Name", userRolesIDsArray);

            return View(user);
        }
    }

}
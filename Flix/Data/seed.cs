using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flix.Data
{
    public class UserRolesSeeder
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider, IConfiguration Configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            string[] roleNames = { "Administrators", "Users", "Uploaders" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public static void SeedUsers(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            try
            {
                if (!context.Users.Any(u => u.Email == "admin@bickel.solutions"))
                {
                    var userStore = new UserStore<IdentityUser>(context);
                    //var manager = new UserManager<IdentityUser>(userStore);
                    var user = new IdentityUser() { UserName = "admin@bickel.solutions", Email = "admin@bickel.solutions", EmailConfirmed = true };
                    IdentityResult result = userManager.CreateAsync(user, "B1ckelS0lutions!!").Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Administrators").Wait();
                    }
                }

            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry("Application", ex.StackTrace,
                //                       System.Diagnostics.EventLogEntryType.Error);
                return;
            }
        }
    }
}

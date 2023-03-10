using Flix.Data;
using Flix.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Flix.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ApplicationDbContext _context;
        public AdministrationController(RoleManager<IdentityRole> roleMgr, UserManager<IdentityUser> userMgr, ApplicationDbContext context)
        {
            roleManager = roleMgr;
            userManager = userMgr;
            _context = context;
        }

        #region Users Management Page
        [Authorize(Roles = "Administrators")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roles.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {

            var role = await roleManager.FindByIdAsync(roleId);
            //var role = await roleManager.FindByNameAsync("Administrators");//roleId);
            var rolenum = role.Id;
            ViewBag.roleId = roleId;
            ViewBag.roleName = role.Name;
            

            if (rolenum == null)
            {
                //ViewBag.ErrorMessage = $"Role with Id={rolenum} cannot be found";
                //return View("Error");
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            var model = new List<UserRolesView>();

            foreach(var user in userManager.Users)
            {
                var UserRolesView = new UserRolesView
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    UserRolesView.IsSelected = true;
                }
                else
                {
                    UserRolesView.IsSelected = false;

                }
                model.Add(UserRolesView);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrators")]
        public async Task<IActionResult> EditUsersInRole(List<UserRolesView> model, string roleId)
        {
            
            var role = await roleManager.FindByIdAsync(roleId);
            //var role = await roleManager.FindByNameAsync("Administrators");
            var rolenum = role.Id;

            if (rolenum == null)
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            for(int i =0; i < model.Count(); i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else { continue; }
                if (result.Succeeded)
                {
                    if (i < model.Count())
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditUsersInRole", new { roleId = role.Id });
                        
                    }
                }
            }
            return RedirectToAction("EditUsersInRole", new { roleId = role.Id });
        }
        #endregion
        #region General Settings Page
        [HttpGet]
        public IActionResult General()
        {
            if (_context.AdminSettings.ToList().Count() != 0 ) { return View(_context.AdminSettings.ToList().Last()); }
            else { return View(); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrators")]
        public async Task<IActionResult> General([Bind("ID,MaintenanceDate,Scheduled,MaintenanceTime,useExistingDirectory,directoryScannerInterval,directoryPath")] AdminSettings admin)
        {
            if (ModelState.IsValid)
            {
                if (_context.AdminSettings.ToList().Count() != 0)
                {
                    foreach(var item in _context.AdminSettings.ToList()) 
                    {
                        _context.Remove(_context.AdminSettings.Single(m => m.ID == item.ID));
                        await _context.SaveChangesAsync();
                    }
                    
                }

                await _context.AddAsync(admin);
                await _context.SaveChangesAsync();

                //Allows the maintenance bar to show up to the user:
                AdminSettings.MaintenaceInfo = admin.MaintenanceDate;
                AdminSettings.ScheduledBool = admin.Scheduled;
                AdminSettings.MaintenaceTimeInfo = admin.MaintenanceTime;

                return RedirectToAction("General");
            }
            return View(admin);
        }
        
        //Modify the way settings are cleared. This should only clear the maintenance times not the directory.
        public async Task<IActionResult> Clear()
        {
            if (_context.AdminSettings.ToList().Count() != 0)
            {
                
                foreach (var item in _context.AdminSettings.ToList())
                {
                    AdminSettings adminSettings = new AdminSettings
                    {
                        MaintenanceDate = "",
                        MaintenanceTime = "",
                        useExistingDirectory = item.useExistingDirectory,
                        directoryPath= item.directoryPath,
                        directoryScannerInterval=item.directoryScannerInterval,
                    };
                    _context.Update(adminSettings);
                    //_context.Remove(_context.AdminSettings.Single(m => m.ID == item.ID));
                    await _context.SaveChangesAsync();
                }
                AdminSettings.MaintenaceInfo = null;
                AdminSettings.ScheduledBool = false;
            }
            return RedirectToAction("General");
        }
        #endregion
    }
}
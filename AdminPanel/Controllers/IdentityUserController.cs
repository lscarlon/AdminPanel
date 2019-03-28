using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using AdminPanel.Attributes;
using AdminPanel.Models;
using AdminPanel.Identity;
using AdminPanel.Common;



namespace AdminPanel.Controllers
{
    [DisplayOrder(2)]
    [DisplayImage("fa fa-dashboard")]
    [TreeView("i", "fa fa-angle-left pull-right", "")]
    public class IdentityUserController : CustomController
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly AppDbContext db;

        public IdentityUserController(UserManager<User> userManager, RoleManager<Role> roleManager, AppDbContext db)  
        {  
            this.userManager = userManager;  
            this.roleManager = roleManager;
            this.db = db;
        }  

        [HttpGet]
        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("/js/IdentityUser-Index.js")]
        [CommandAuthorize("User List")]
        public IActionResult Index(string filterRole, bool partial = false)
        {
            List<IdentityUserListViewModel> model = new List<IdentityUserListViewModel>();
            model = userManager.Users.Select(u => new IdentityUserListViewModel
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                RoleName = db.Roles.FirstOrDefault(r => r.Id == db.UserRoles.FirstOrDefault(ur => ur.UserId == u.Id).RoleId).Name
            }).ToList();
            if (!(filterRole is null)) {
                model = model.Where(u => u.RoleName == db.Roles.FirstOrDefault(r => r.Id == filterRole).Name).ToList();
            }
            if (partial)
                return PartialView(model);
            else
                return View(model);
        }
         
        [HttpPost, ActionName("Index")]
        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("/js/IdentityUser-Index.js")]
        [CommandAuthorize("User List")]
        public IActionResult IndexPost(string filterRole, bool partial = false)
        {
            List<IdentityUserListViewModel> model = new List<IdentityUserListViewModel>();
            model = userManager.Users.Select(u => new IdentityUserListViewModel
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                RoleName = db.Roles.FirstOrDefault(r => r.Id == db.UserRoles.FirstOrDefault(ur => ur.UserId == u.Id).RoleId).Name
            }).ToList();
            if (!(filterRole is null))
            {
                model = model.Where(u => u.RoleName ==  db.Roles.FirstOrDefault(r => r.Id==filterRole).Name).ToList();
            }
            //ViewData["DebugMessage"] = filterRole;
            if (partial)
                return PartialView(model);
            else
                return View(model);
        }

        [HttpGet]
        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("")]
        [CommandAuthorize("User Add")]
        public IActionResult AddUser()
        {
            IdentityUserViewModel model = new IdentityUserViewModel
            {
                ApplicationRoles = roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id
                }).ToList()
            };
            return PartialView("_AddUser", model);
        }

        [HttpPost]
        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("")]
        [CommandAuthorize("User Add")]
        public async Task<IActionResult> AddUser(IdentityUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Name = model.Name,
                    UserName = model.UserName,
                    Email = model.Email
                };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    Role applicationRole = await roleManager.FindByIdAsync(model.ApplicationRoleId);
                    if (applicationRole != null)
                    {
                        IdentityResult roleResult = await userManager.AddToRoleAsync(user, applicationRole.Name);
                        if (roleResult.Succeeded)
                        {
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("")]
        [CommandAuthorize("User Edit")]
        public async Task<IActionResult> EditUser(string id)
        {
            IdentityUserEditModel model = new IdentityUserEditModel
            {
                ApplicationRoles = roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id
                }).ToList()
            };

            if (!String.IsNullOrEmpty(id))
            {
                User user = await userManager.FindByIdAsync(id);
                if (user != null)
                {
                    model.Name = user.Name;
                    model.Email = user.Email;
                    string existingRole = userManager.GetRolesAsync(user).Result.FirstOrDefault();
                    if (existingRole != null)
                    {
                        model.ApplicationRoleId = roleManager.Roles.Single(r => r.Name == existingRole).Id;
                    }
                    else {
                        model.ApplicationRoleId = null;
                    }
                }
            }
            return PartialView("_EditUser", model);
        }

        [HttpPost]
        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("")]
        [CommandAuthorize("User Edit")]
        public async Task<IActionResult> EditUser(string id, IdentityUserEditModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.Name = model.Name;
                    user.Email = model.Email;
                    string existingRole = userManager.GetRolesAsync(user).Result.FirstOrDefault();
                    string existingRoleId = null;
                    if (existingRole != null) {existingRoleId = roleManager.Roles.Single(r => r.Name == existingRole).Id;}
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        if (existingRoleId != model.ApplicationRoleId)
                        {
                            bool roleResultSuccess = true;
                            if (existingRoleId != null) {
                                IdentityResult roleResult = await userManager.RemoveFromRoleAsync(user, existingRole);
                                if (!roleResult.Succeeded) { roleResultSuccess = false; }
                            }
                            if (roleResultSuccess)
                            {
                                Role applicationRole = await roleManager.FindByIdAsync(model.ApplicationRoleId);
                                if (applicationRole != null)
                                {
                                    IdentityResult newRoleResult = await userManager.AddToRoleAsync(user, applicationRole.Name);
                                    if (newRoleResult.Succeeded)
                                    {
                                        return RedirectToAction("Index");
                                    }
                                }
                            }
                        }
                        return RedirectToAction("Index");
                    }
                }
            }
            return PartialView("_EditUser", model);
        }

        [HttpGet]
        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("")]
        [CommandAuthorize("User Delete")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            string name = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                User applicationUser = await userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    name = applicationUser.Name;
                }
            }
            return PartialView("_DeleteUser", name);
        }

        [HttpPost]
        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("")]
        [CommandAuthorize("User Delete")]
        public async Task<IActionResult> DeleteUser(string id, IFormCollection form)
        {
            if (!String.IsNullOrEmpty(id))
            {
                User applicationUser = await userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    IdentityResult result = await userManager.DeleteAsync(applicationUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }
    }
}

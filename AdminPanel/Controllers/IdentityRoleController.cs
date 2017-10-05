using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using AdminPanel.Attributes;
using AdminPanel.Models;
using AdminPanel.Identity;
using AdminPanel.Common;

namespace AdminPanel.Controllers
{
    [DisplayOrder(1)]
    [DisplayImage("glyphicon glyphicon-log-in")]
    [TreeView("i", "fa fa-angle-left pull-right", "")]
    public class IdentityRoleController : CustomController
    {
        private readonly RoleManager<Role> roleManager;
        private readonly AppDbContext db;

        public IdentityRoleController(RoleManager<Role> roleManager, AppDbContext db)
        {
            this.roleManager = roleManager;
            this.db = db;
        }

        [HttpGet]
        [DisplayActionMenu]
        [DisplayImage("glyphicon glyphicon-user")]
        [ScriptAfterPartialView("")]
        [CommandName("Index")]
        public IActionResult Index(bool partial = false)
        {
            List<IdentityRoleListViewModel> model = new List<IdentityRoleListViewModel>();
            model = roleManager.Roles.Select(r => new IdentityRoleListViewModel
            {
                RoleName = r.Name,
                Id = r.Id,
                Description = r.Description,
                NumberOfUsers = db.UserRoles.Where(ur => ur.RoleId == r.Id).Count()
            }).ToList();
            return View(model);
        }

        [HttpGet]
        [ScriptAfterPartialView("")]
        public async Task<IActionResult> AddEditApplicationRole(string id)
        {
            IdentityRoleViewModel model = new IdentityRoleViewModel();
            if (!String.IsNullOrEmpty(id))
            {
                Role applicationRole = await roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    model.Id = applicationRole.Id;
                    model.RoleName = applicationRole.Name;
                    model.Description = applicationRole.Description;
                }
            }
            return PartialView("_AddEditRole", model);
        }

        [HttpPost]
        [ScriptAfterPartialView("")]
        public async Task<IActionResult> AddEditApplicationRole(string id, IdentityRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isExist = !String.IsNullOrEmpty(id);
                Role applicationRole = isExist ? await roleManager.FindByIdAsync(id) :
               new Role
               {
                   CreatedDate = DateTime.UtcNow
               };
                applicationRole.Name = model.RoleName;
                applicationRole.Description = model.Description;
                applicationRole.IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                IdentityResult roleRuslt = isExist ? await roleManager.UpdateAsync(applicationRole)
                                                    : await roleManager.CreateAsync(applicationRole);
                if (roleRuslt.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpGet]
        [ScriptAfterPartialView("")]
        public async Task<IActionResult> DeleteApplicationRole(string id)
        {
            string name = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                Role applicationRole = await roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    name = applicationRole.Name;
                }
            }
            return PartialView("_DeleteRole", name);
        }

        [HttpPost]
        [ScriptAfterPartialView("")]
        public async Task<IActionResult> DeleteApplicationRole(string id, IFormCollection form)
        {
            if (!String.IsNullOrEmpty(id))
            {
                Role applicationRole = await roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    IdentityResult roleRuslt = roleManager.DeleteAsync(applicationRole).Result;
                    if (roleRuslt.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }
    }
}

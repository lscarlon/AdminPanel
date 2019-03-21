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
using Z.EntityFramework.Plus;

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
        [ScriptAfterPartialView("/js/IdentityRole-Index.js")]
        [CommandAuthorize("Role List")]
        public IActionResult Index(string filterUser, bool partial = false)
        {
            List<IdentityRoleListViewModel> model = new List<IdentityRoleListViewModel>();
            model = roleManager.Roles.Select(r => new IdentityRoleListViewModel
            {
                RoleName = r.Name,
                Id = r.Id,
                Description = r.Description,
                NumberOfUsers = db.UserRoles.Where(ur => ur.RoleId == r.Id).Count()
            }).ToList();
            if (!(filterUser is null))
            { }
                
            if (partial)
                return PartialView(model);
            else
                return View(model);
        }

        [HttpGet]
        [ScriptAfterPartialView("")]
        [CommandAuthorize("Role Edit")]
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
        [CommandAuthorize("Role Edit")]
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
        [CommandAuthorize("Role Delete")]
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
        [CommandAuthorize("Role Delete")]
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

        [HttpGet]
        [ScriptAfterPartialView("")]
        [CommandAuthorize("Role Permission")]
        public async Task<IActionResult> EditRoleClaim(string id)
        {
            List<IdentityRoleClaimViewModel> model = new List<IdentityRoleClaimViewModel>();
        
            if (!String.IsNullOrEmpty(id))
            {
                Role applicationRole = await roleManager.FindByIdAsync(id);
                if (applicationRole == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["Role"] =applicationRole;
                    model = db.Commands
                            .GroupJoin(
                                db.Menus
                                .Where(m => m.Controller != null && m.DisplayOrder >= 0)
                                .GroupBy(m => m.Controller)
                                .Select(m => new
                                {
                                    Controller = m.Key,
                                    DisplayOrder = m.Min(mm => mm.DisplayOrder)
                                })
                                , c => c.Controller, m => m.Controller,
                                (c, m) => new { c.Controller, c.CommandName, DisplayOrder = (int?)m.FirstOrDefault().DisplayOrder }
                            )
                            .GroupJoin(
                                db.RoleClaims
                                .Where(c => c.ClaimType == "CommandAuthorize" && c.RoleId == applicationRole.Id)
                                .Select(c => new
                                {
                                    c.ClaimValue,
                                    c.RoleId
                                })
                                , c => c.CommandName, cl => cl.ClaimValue,
                                (c, cl) => new IdentityRoleClaimViewModel
                                {
                                    Controller = c.Controller,
                                    CommandName = c.CommandName,
                                    DisplayOrder = c.DisplayOrder.HasValue ? c.DisplayOrder : int.MaxValue,
                                    Checked = cl.FirstOrDefault().RoleId != null ? true : false
                                }
                            )
                            .OrderBy(c => c.DisplayOrder)
                            .ThenBy(c => c.Controller)
                            .ToList();
                }
            }
            return PartialView("_EditRoleClaim", model);
        }

        [HttpPost]
        [ScriptAfterPartialView("")]
        [CommandAuthorize("Role Permission")]
        public async Task<IActionResult> EditRoleClaim(string id, List<IdentityRoleClaimViewModel> model)
        {
            if (ModelState.IsValid && !String.IsNullOrEmpty(id))
            {
                Role applicationRole = await roleManager.FindByIdAsync(id);
                db.RoleClaims.Where(rc =>
                                         rc.ClaimType == "CommandAuthorize"
                                         && rc.RoleId == id)
                                    .Delete();
                db.RoleClaims.AddRange(  model.
                                         Where(rc => rc.Checked == true).
                                         Select(rc => new IdentityRoleClaim<string>
                                         {
                                             RoleId = id,
                                             ClaimType = "CommandAuthorize",
                                             ClaimValue = rc.CommandName
                                         }).ToList()
                                     );
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}

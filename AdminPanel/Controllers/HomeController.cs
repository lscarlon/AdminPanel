using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AdminPanel.Attributes;
using AdminPanel.Identity;
using AdminPanel.Common;
using Microsoft.Extensions.DependencyInjection;
using AdminPanel.Models;

namespace AdminPanel.Controllers
{
    [DisplayOrder(0)]
    [DisplayImage("fa fa-dashboard")]
    [TreeView("i", "fa fa-angle-left pull-right", "")]
    public class HomeController : CustomController
    {
        private readonly UserManager<User> userManager;
        private readonly AppDbContext db;

        public HomeController(UserManager<User> userManager, AppDbContext db)
        {
            this.userManager = userManager;
            this.db = db;
        }

        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("")]
        //[CommandAuthorize("Homepage")]
        public IActionResult Default(bool partial = false)
        {
            ViewBag.RolesCount = User.Claims.Where(c => c.Type == ClaimTypes.Role).Count();

            if (partial)
                return PartialView(User.Claims.ToList());
            else
                return View(User.Claims.ToList());
        }


        public IActionResult TestInvioEmail([FromServices] IEmailService smtpClient, bool partial = false)
        {
            string response;

            EmailMessage emailMessage = new EmailMessage
            {
                FromAddress = new EmailAddress { Name = "AdminPanel", Address = "adminpanel@l.carlone.it" },
                Subject = "Oggetto",
                Content = "Messaggio di prova"
            };
            emailMessage.ToAddresses.Add(new EmailAddress { Name = "Luigi Carlone", Address = "l.carlone@dpssrl.com" });

            if (!smtpClient.Send(emailMessage, out response)) ViewBag.Response = response;
            return RedirectToAction("Default");
        }

        public IActionResult Test(bool partial = false)
        {
            List<IdentityUserListViewModel> model = new List<IdentityUserListViewModel>();
            model = userManager.Users.Select(u => new IdentityUserListViewModel
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                RoleName = db.Roles.First(r => r.Id == db.UserRoles.First(ur => ur.UserId == u.Id).RoleId).Name
            }).ToList();
            return RedirectToAction("Default");

        }
    }
}

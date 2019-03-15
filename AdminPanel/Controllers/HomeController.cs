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

namespace AdminPanel.Controllers
{
    [DisplayOrder(0)]
    [DisplayImage("fa fa-dashboard")]
    [TreeView("i", "fa fa-angle-left pull-right", "")]
    public class HomeController : CustomController
    {
        private readonly UserManager<User> userManager;

        public HomeController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("")]
        [CommandAuthorize("Homepage")]
        public IActionResult Default(bool partial = false)
        {
            //foreach (Claim cl in User.Claims.ToList())
            //{
            //    ViewBag
            //}

            if (partial)
                return PartialView();
            else
                return View(User.Claims.ToList());
        }

        public IActionResult Test( [FromServices] IEmailService smtpClient, bool partial = false)
        {
            string response;
            EmailMessage emailMessage = new EmailMessage { 
                FromAddress = new EmailAddress { Name = "AdminPanel", Address = "adminpanel@l.carlone.it" },    
                Subject="Oggetto",
                Content="Messaggio di prova"
            };
            emailMessage.ToAddresses.Add(new EmailAddress { Name = "Luigi Carlone", Address = "l.carlone@dpssrl.com" });

            if (!smtpClient.Send(emailMessage, out response)) ViewBag.Response = response;
            return RedirectToAction("Default");
        }

    }
}

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
        [CommandName("Homepage")]
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

    }
}

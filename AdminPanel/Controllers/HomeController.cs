using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AdminPanel.Attributes;
using AdminPanel.Identity;
using AdminPanel.Common;

using System.Reflection;

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
            

            if (partial)
                return PartialView();
            else
                return View();
        }

    }
}

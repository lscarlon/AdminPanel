﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AdminPanel.Attributes;
using AdminPanel.Identity;

namespace AdminPanel.Controllers
{
    [Authorize]
    [DisplayOrder(0)]
    [DisplayImage("fa fa-dashboard")]
    [TreeView("i", "fa fa-angle-left pull-right", "")]
    public class HomeController : Controller
    {
        private readonly UserManager<User> userManager;

        public HomeController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [Authorize(Roles = "SuperAdmin")]
        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("")]
        public IActionResult Default(bool partial = false)
        {
            string userName = userManager.GetUserName(User);
            ViewData["userName"] = userName;
            if (partial)
                return PartialView();
            else
                return View();
        }

    }
}

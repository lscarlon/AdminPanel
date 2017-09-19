using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AdminPanel.Attributes;

namespace AdminPanel.Controllers
{
    [DisplayOrder(1)]
    [DisplayImage("fa fa-dashboard")]
    [TreeView("i", "fa fa-angle-left pull-right", "")]
    public class CustomErrorsController : Controller
    {
        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("")]
        public IActionResult E404(bool partial = false)
        {
            if (partial)
                return PartialView();
            else
                return View();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AdminLTE.NETCore.Attributes;

namespace AdminLTE.NETCore.Controllers
{
    [DisplayOrder(1)]
    [DisplayImage("fa fa-files-o")]
    //[TreeView("span", "label label-primary pull-right", "4")]
    [TreeViewSettings("span|label label-primary pull-right|4")]
    public class LayoutOptionsController : Controller
    {
        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("")]
        public IActionResult TopNavigation(bool partial = false)
        {
            if (partial)
                return PartialView();
            else
                return View();
        }

        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("")]
        public IActionResult Boxed(bool partial = false)
        {
            if (partial)
                return PartialView();
            else
                return View();
        }

        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("")]
        public IActionResult Fixed(bool partial = false)
        {
            if (partial)
                return PartialView();
            else
                return View();
        }

        [DisplayActionMenu]
        [DisplayImage("fa fa-circle-o")]
        [ScriptAfterPartialView("")]
        public IActionResult CollapsedSidebar(bool partial = false)
        {
            if (partial)
                return PartialView();
            else
                return View();
        }
    }
}

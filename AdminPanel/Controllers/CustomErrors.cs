using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AdminPanel.Attributes;

namespace AdminPanel.Controllers
{
    [DisplayOrder(-1)]
    public class CustomErrorsController : Controller
    {

        public IActionResult E404(bool partial = false)
        {
            if (partial)
                return PartialView();
            else
                return View();
        }

    }
}

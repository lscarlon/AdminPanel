using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using AdminPanel.Attributes;
using AdminPanel.Common;


namespace AdminPanel.Controllers
{
    [DisplayOrder(-1)]
    public class ErrorController : CustomController
    {
        //[AllowAnonymous]
        //public IActionResult E404(bool partial = false)
        //{
        //    if (partial)
        //        return PartialView();
        //    else
        //        return View();
        //}

        [AllowAnonymous]
        [Route("Error/E500")]
        public IActionResult E500()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ViewBag.statusCode = "500";
            if (exceptionFeature != null)
            {
                ViewBag.ErrorMessage = exceptionFeature.Error.Message;
                ViewBag.RouteOfException = exceptionFeature.Path;
            }
            return View("HandleErrorCode");
        }

        [AllowAnonymous]
        [Route("Error/{statusCode}")]
        public IActionResult HandleErrorCode(int statusCode)
        {
            var statusCodeData = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            ViewBag.statusCode = statusCode;

            switch (statusCode)
            {
                case 401:
                    ViewBag.ErrorMessage = "Sorry you have to authenticate to access this page";
                    return RedirectToAction("Login", "Login", new { returnUrl = statusCodeData.OriginalPath });
                case 403:
                    ViewBag.ErrorMessage = "Sorry you have not permission to access this page";
                    ViewBag.RouteOfException = HttpContext.Request.Query["ReturnUrl"];
                    break;
                case 404:
                    ViewBag.ErrorMessage = "Sorry the page you requested could not be found";
                    ViewBag.RouteOfException = statusCodeData.OriginalPath;
                    break;
                case 500:
                    ViewBag.ErrorMessage = "Sorry something went wrong on the server";
                    ViewBag.RouteOfException = statusCodeData.OriginalPath;
                    break;
            }

            return View();
        }
    }
}

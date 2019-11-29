using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;




namespace Locust.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISettingsService _settingsService;

        public HomeController(ISettingsService settingsService)
        {
            _settingsService = settingsService;

        }

        public ActionResult Index()
        {

            if (!UserHelper.HasSomeAccessSite(User.Identity.Name).HasPermission)
            {
                Response.Redirect("Home/Access");
            }

            Setting u = _settingsService.GetByName(new Setting() { UserName = UserHelper.GetCurrentUser(User.Identity.Name) });
            if (u == null)
            {
                ViewBag.template = "Blue";
            }
            else
            {
                ViewBag.template = u.UIMode;
            }

            return View();
        }

        protected override void HandleUnknownAction(string actionName)
        {
            try
            {
                this.View(actionName).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                Response.Redirect("Error404");
            }
        }
    }
}
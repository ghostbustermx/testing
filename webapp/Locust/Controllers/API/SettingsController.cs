using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class SettingsController : ApiController
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;

        }

        [ActionName("Get")]
        [HttpGet]
        public Setting GetSetting()
        {
            Setting setting = new Setting()
            {
                UserName = UserHelper.GetCurrentUser(User.Identity.Name)
            };
            return _settingsService.GetByName(setting);
        }

        [ActionName("Save")]
        [HttpPost]
        public Setting SaveSetting(Setting setting)
        {
            setting.UserName = UserHelper.GetCurrentUser(User.Identity.Name);
            return _settingsService.Save(setting);
        }

        [ActionName("Update")]
        [HttpPut]
        public Setting UpdateSetting(Setting setting)
        {

            setting.UserName = UserHelper.GetCurrentUser(User.Identity.Name);
            return _settingsService.Update(setting);
        }
    }
}

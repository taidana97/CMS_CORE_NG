﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookieService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ModelService;
using UserService;
using WritableOptionsService;
using Microsoft.AspNetCore.DataProtection;
using AttributeService;

namespace CMS_CORE_NG.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class SiteSettingsController : Controller
    {
        private readonly IUserSvc _userSvc;
        private readonly ICookieSvc _cookieSvc;
        private readonly IServiceProvider _provider;
        private readonly DataProtectionKeys _dataProtectionKeys;
        private readonly AppSettings _appSettings;
        private readonly IWritableSvc<SiteWideSettings> _writableSiteWideSettings;
        private AdminBaseViewModel _adminBaseViewModel;

        public SiteSettingsController(
            IUserSvc userSvc,
            ICookieSvc cookieSvc,
            IServiceProvider provider,
            IOptions<DataProtectionKeys> dataProtectionKeys,
            IOptions<AppSettings> appSettings,
            IWritableSvc<SiteWideSettings> writableSiteWideSettings)
        {
            _userSvc = userSvc;
            _cookieSvc = cookieSvc;
            _provider = provider;
            _dataProtectionKeys = dataProtectionKeys.Value;
            _appSettings = appSettings.Value;
            _writableSiteWideSettings = writableSiteWideSettings;
        }

        public async Task<IActionResult> Index()
        {
            var protectorProvider = _provider.GetService<IDataProtectionProvider>();
            var protector = protectorProvider.CreateProtector(_dataProtectionKeys.ApplicationUserKey);
            var userProfile = await _userSvc.GetUserProfileByIdAsync(protector.Unprotect(_cookieSvc.Get("user_id")));
            var addUserModel = new AddUserModel();

            _adminBaseViewModel = new AdminBaseViewModel
            {
                Profile = userProfile,
                AddUser = addUserModel,
                AppSetting = _appSettings,
                SiteWideSetting = _writableSiteWideSettings.Value
            };

            return View("Index", _adminBaseViewModel);
        }

        [AjaxOnly]
        [HttpPost]
        public async Task<IActionResult> UpdateSettings([FromBody] SiteWideSettings options)
        {
            await Task.Delay(0);

            var resultError = _writableSiteWideSettings.Update((opt) =>
            {
                opt.WebsiteName = options.WebsiteName;
                opt.WebsiteTitle = options.WebsiteTitle;
                opt.WebsiteDescription = options.WebsiteDescription;
                opt.WebsiteKeywords = options.WebsiteKeywords;
                opt.WebsiteAuthor = options.WebsiteAuthor;
                opt.WebsiteFooter = options.WebsiteFooter;
                opt.WebsiteStatus = options.WebsiteStatus;
                opt.WebsiteRegistration = options.WebsiteRegistration;
                opt.AgeVerification = options.AgeVerification;
            });

            if (!resultError)
            {
                return Ok(new { success = true });
            }

            return BadRequest(new { success = false });
        }
    }
}

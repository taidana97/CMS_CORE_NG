using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookieService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ModelService;
using Microsoft.Extensions.DependencyInjection;
using UserService;
using WritableOptionsService;
using Microsoft.AspNetCore.DataProtection;
using DashboardService;

namespace CMS_CORE_NG.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class HomeController : Controller
    {
        private readonly ICookieSvc _cookieSvc;
        private readonly IDashboardSvc _dashboardSvc;
        private readonly IServiceProvider _provider;
        private readonly DataProtectionKeys _dataProtectionKeys;
        private readonly AppSettings _appSettings;
        private readonly IUserSvc _userSvc;
        private readonly IWritableSvc<SiteWideSettings> _writableSiteWideSettings;
        private static AdminBaseViewModel _adminBaseViewModel;

        public HomeController(
            IUserSvc userSvc,
            ICookieSvc cookieSvc,
            IDashboardSvc dashboardSvc,
            IServiceProvider provider,
            IOptions<DataProtectionKeys> dataProtectionKeys,
            IOptions<AppSettings> appSettings,
            IWritableSvc<SiteWideSettings> writableSiteWideSettings
            )
        {
            _userSvc = userSvc;
            _cookieSvc = cookieSvc;
            _dashboardSvc = dashboardSvc;
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
            var dashboard = await _dashboardSvc.GetDashboard();

            _adminBaseViewModel = new AdminBaseViewModel
            {
                Profile = userProfile,
                AddUser = addUserModel,
                AppSetting = _appSettings,
                Dashboard = dashboard,
                SiteWideSetting = _writableSiteWideSettings.Value
            };

            return View("Index", _adminBaseViewModel);
        }
    }
}

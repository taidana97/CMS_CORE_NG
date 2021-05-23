using ActivityService;
using AuthService;
using BackendService;
using CMS_CORE_NG.Extensions;
using CookieService;
using CountryService;
using DashboardService;
using DataService;
using EmailService;
using FiltersService;
using FunctionalService;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ModelService;
using RoleService;
using System;
using System.Linq;
using System.Text;
using UserService;

namespace CMS_CORE_NG
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            // DB CONNECTION OPTIONS
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("CmsCoreNg_DEV"),
                    x => x.MigrationsAssembly("CMS_CORE_NG")));

            services.AddDbContext<DataProtectionKeysContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DataProtectionKeysContext"),
                    x => x.MigrationsAssembly("CMS_CORE_NG")));

            // FUNCTIONAL SERVICE
            services.AddTransient<IFunctionalSvc, FunctionalSvc>();
            services.Configure<AdminUserOptions>(Configuration.GetSection("AdminUserOptions"));
            services.Configure<AppUserOptions>(Configuration.GetSection("AppUserOptions"));

            // Writable SERVICE
            var sideWideSettings = Configuration.GetSection("SiteWideSettings");
            var sendGridOptionsSection = Configuration.GetSection("SendGridOptions");
            var smtpOptionsSection = Configuration.GetSection("SmtpOptions");
            services.ConfigureWritable<SiteWideSettings>(sideWideSettings, "appsettings.json");
            services.ConfigureWritable<SendGridOptions>(sendGridOptionsSection, "appsettings.json");
            services.ConfigureWritable<SmtpOptions>(smtpOptionsSection, "appsettings.json");

            // DEFAULT IDENTITY OPTIONS
            var identityDefaultOptionsConfiguration = Configuration.GetSection("IdentityDefaultOptions");
            services.Configure<IdentityDefaultOptions>(identityDefaultOptionsConfiguration);
            var identityDefaultOptions = identityDefaultOptionsConfiguration.Get<IdentityDefaultOptions>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = identityDefaultOptions.PasswordRequireDigit;
                options.Password.RequiredLength = identityDefaultOptions.PasswordRequiredLength;
                options.Password.RequireNonAlphanumeric = identityDefaultOptions.PasswordRequireNonAlphanumeric;
                options.Password.RequireUppercase = identityDefaultOptions.PasswordRequireUppercase;
                options.Password.RequireLowercase = identityDefaultOptions.PasswordRequireLowercase;
                options.Password.RequiredUniqueChars = identityDefaultOptions.PasswordRequiredUniqueChars;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(identityDefaultOptions.LockoutDefaultLockoutTimeSpanInMinutes);
                options.Lockout.MaxFailedAccessAttempts = identityDefaultOptions.LockoutMaxFailedAccessAttempts;
                options.Lockout.AllowedForNewUsers = identityDefaultOptions.LockoutAllowedForNewUsers;

                // User settings
                options.User.RequireUniqueEmail = identityDefaultOptions.UserRequireUniqueEmail;

                // email confirmation require
                options.SignIn.RequireConfirmedEmail = identityDefaultOptions.SignInRequireConfirmedEmail;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            // DATA PROTECTION SERVICE
            var dataProtectionService = Configuration.GetSection("DataProtectionKeys");
            services.Configure<DataProtectionKeys>(dataProtectionService);
            services.AddDataProtection().PersistKeysToDbContext<DataProtectionKeysContext>();

            // USER HELPER SERVICE
            services.AddTransient<IUserSvc, UserSvc>();

            // User role service
            services.AddTransient<IRoleSvc, RoleSvc>();

            // APPSETTINGS SERVICE
            var appSettingSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingSection);

            // JWT AUTHENTICATION SERVICE
            var appSettings = appSettingSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(o =>
            {
                // Microsoft.AspNetCore.Authentication.JwtBearer 3.1.10'
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = appSettings.ValidateIssuerSigningKey,
                    ValidateIssuer = appSettings.ValidateIssuer,
                    ValidateAudience = appSettings.ValidateAudience,
                    ValidIssuer = appSettings.Site,
                    ValidAudience = appSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // EMAIL SERVICE
            services.Configure<SendGridOptions>(Configuration.GetSection("SendGridOptions"));
            services.Configure<SmtpOptions>(Configuration.GetSection("SmtpOptions"));
            services.AddTransient<IEmailSvc, EmailSvc>();

            // AUTH SERViCE
            services.AddTransient<IAuthSvc, AuthSvc>();

            // ADMIN SERVICE
            services.AddTransient<IAdminSvc, AdminSvc>();

            // ACTIVITY SERVICE
            services.AddTransient<IActivitySvc, ActivitySvc>();

            // Cookie Helper SERVICE
            services.AddHttpContextAccessor();
            services.AddTransient<CookieOptions>();
            services.AddTransient<ICookieSvc, CookieSvc>();

            // Country SERVICE
            services.AddTransient<ICountrySvc, CountrySvc>();

            // Dashboard SERVICE
            services.AddTransient<IDashboardSvc, DashboardSvc>();

            // AuthenticationSchemes SERVICE
            services.AddAuthentication("Administrator")
                .AddScheme<AdminAuthenticationOptions, AdminAuthenticationHandler>("Admin", null);

            // ENABLE CORS
            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build();
                });
            });

            // ENABLE API Versioning
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });


            /*---------------------------------------------------------------------------------------------------*/
            /*                                 Razor Pages Runtime SERVICE                                       */
            /* Add Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation NuGet package to the project                */
            /* Surprised that refreshing a view while the app is running did not work                            */
            /*---------------------------------------------------------------------------------------------------*/
            services.AddMvc()
                .AddControllersAsServices()
                .AddRazorRuntimeCompilation()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            // [ValidateAntiForgeryToken]
            // [AutoValidateAntiforgeryToken]
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCors("EnableCORS");

            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            // [Authorize]
            app.UseAuthorization();

            // [ValidateAntiForgeryToken]
            // [AutoValidateAntiforgeryToken]
            app.Use(nextDelegate => context =>
            {
                string path = context.Request.Path.Value.ToLower();
                string[] directUrls =
                {
                    "/admin",
                    "/store",
                    "/cart",
                    "checkout",
                    "/login"
                };
                if (path.StartsWith("/swagger")
                    || path.StartsWith("/api")
                    || string.Equals("/", path)
                    || directUrls.Any(url => path.StartsWith(url)))
                { 
                    AntiforgeryTokenSet tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append(
                        "XSRF-TOKEN",
                        tokens.RequestToken,
                        new CookieOptions()
                        {
                            HttpOnly = false,
                            Secure = false,
                            IsEssential = true,
                            SameSite = SameSiteMode.Strict
                        }
                    );
                }
                return nextDelegate(context);
            });

            app.UseEndpoints(endpoints =>
            {
                // * Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation 3.1.5
                endpoints.MapControllerRoute(
                   name: "areas",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}

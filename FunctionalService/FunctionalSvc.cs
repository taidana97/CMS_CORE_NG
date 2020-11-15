﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ModelService;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalService
{
    public class FunctionalSvc : IFunctionalSvc
    {
        private readonly AdminUserOptions _adminUserOptions;
        private readonly AppUserOptions _appUserOptions;
        private readonly UserManager<ApplicationUser> _userManager;


        public FunctionalSvc(
            IOptions<AppUserOptions> appUserOptions,
            IOptions<AdminUserOptions> adminUserOptions,
            UserManager<ApplicationUser> userManager
            )
        {
            _appUserOptions = appUserOptions.Value;
            _adminUserOptions = adminUserOptions.Value;
            _userManager = userManager;
        }

        public async Task CreateDefaultAdminUser()
        {
            try
            {
                var adminUser = new ApplicationUser
                {
                    Email = _adminUserOptions.Email,
                    UserName = _adminUserOptions.Username,
                    EmailConfirmed = true,
                    ProfilePic = "asdasd",
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true,
                    Firstname = _adminUserOptions.FirstName,
                    Lastname = _adminUserOptions.LastName,
                    UserRole = "Administrator",
                    IsActive = true,
                    UserAddresses = new List<AddressModel>
                    {
                        new AddressModel{Country = _adminUserOptions.Country, Type="Billing"},
                        new AddressModel{Country = _adminUserOptions.Country, Type="Shpping"},
                    }
                };

                var result = await _userManager.CreateAsync(adminUser, _adminUserOptions.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Administrator");
                    Log.Information("Admin User Created {UserName}", adminUser.UserName);
                }
                else
                {
                    var errorString = string.Join(",", result.Errors);
                    Log.Error("Error while creating user {Error}", errorString);
                }

            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                 ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
        }

        private Task<string> GetDefaultProfilePic()
        {
            throw new NotImplementedException();
        }

        public async Task CreateDefaultUser()
        {
            try
            {
                var appUser = new ApplicationUser
                {
                    Email = _appUserOptions.Email,
                    UserName = _appUserOptions.Username,
                    EmailConfirmed = true,
                    ProfilePic = "aasdasd",
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true,
                    Firstname = _appUserOptions.FirstName,
                    Lastname = _appUserOptions.LastName,
                    UserRole = "Customer",
                    IsActive = true,
                    UserAddresses = new List<AddressModel>
                    {
                        new AddressModel {Country = _appUserOptions.Country, Type = "Billing"},
                        new AddressModel {Country = _appUserOptions.Country, Type = "Shipping"}
                    }
                };

                var result = await _userManager.CreateAsync(appUser, _appUserOptions.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, "Customer");
                    Log.Information("App User Created {UserName}", appUser.UserName);
                }
                else
                {
                    var errorString = string.Join(",", result.Errors);
                    Log.Error("Error while creating user {Error}", errorString);
                }
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                 ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
        }

    }
}

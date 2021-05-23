using ActivityService;
using CookieService;
using DataService;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModelService;
using Serilog;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService
{
    public class AuthSvc : IAuthSvc
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _db;
        private readonly ICookieSvc _cookieSvc;
        private readonly IServiceProvider _provider;
        private readonly DataProtectionKeys _dataProtectionKeys;
        private readonly IActivitySvc _activitySvc;
        //private IDataProtector _protector;
        private string[] UserRoles = new[] { "Administrator", "Customer" };
        //private TokenValidationParameters validationParameters;
        //private JwtSecurityTokenHandler handler;
        //private string unProtectedToken;
        //private ClaimsPrincipal validateToken;

        public AuthSvc(
            UserManager<ApplicationUser> userManager,
            IOptions<AppSettings> appSettings,
            IOptions<DataProtectionKeys> dataProtectionKeys,
            ApplicationDbContext db,
            ICookieSvc cookieSvc,
            IActivitySvc activitySvc,
            IServiceProvider provider
            )
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _dataProtectionKeys = dataProtectionKeys.Value;
            _db = db;
            _cookieSvc = cookieSvc;
            _provider = provider;
            _activitySvc = activitySvc;
        }

        // Will be used for authenticating the Admin
        public async Task<TokenResponseModel> Auth(LoginViewModel model)
        {
            ActivityModel activityModel = new ActivityModel();
            activityModel.Date = DateTime.Now;
            activityModel.IpAddress = _cookieSvc.GetUserIP();
            activityModel.Location = _cookieSvc.GetUserCountry();
            activityModel.OperatingSystem = _cookieSvc.GetUserOS();

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                    return CreateErrorResponseToken("Request Not Supported", HttpStatusCode.Unauthorized);

                var roles = await _userManager.GetRolesAsync(user);

                if (roles.FirstOrDefault() != "Administrator")
                {
                    activityModel.UserId = user.Id;
                    activityModel.Type = "Un-Authorized Login attempt";
                    activityModel.Icon = "fas fa-user-secret";
                    activityModel.Color = "danger";
                    await _activitySvc.AddUserActivity(activityModel);

                    Log.Error("Error: Role Not Admin");
                    return CreateErrorResponseToken("Request Not Supported", HttpStatusCode.Unauthorized);
                }

                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    activityModel.UserId = user.Id;
                    activityModel.Type = "Login attempt failed";
                    activityModel.Icon = "far fa-times-circle";
                    activityModel.Color = "danger";
                    await _activitySvc.AddUserActivity(activityModel);

                    Log.Error("Error: Invalid Password for Admin");
                    return CreateErrorResponseToken("Request Not Supported", HttpStatusCode.Unauthorized);
                }

                // Then Check If email is confirmed
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    activityModel.UserId = user.Id;
                    activityModel.Type = "Login attempt Success - Email Not Verified";
                    activityModel.Icon = "far fa-envelope";
                    activityModel.Color = "warning";
                    await _activitySvc.AddUserActivity(activityModel);

                    Log.Error("Error: Email Not Confirmed for {user}", user.UserName);
                    return CreateErrorResponseToken("Email Not Confirmed", HttpStatusCode.Unauthorized);
                }                

                activityModel.UserId = user.Id;
                activityModel.Type = "Login attempt successful";
                activityModel.Icon = "fas fa-thumbs-up";
                activityModel.Color = "success";
                await _activitySvc.AddUserActivity(activityModel);

                var authToken = await GenerateNewToken(user, model);

                return authToken;
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            return CreateErrorResponseToken("Request Not Supported", HttpStatusCode.Unauthorized);
        }

        private async Task<TokenResponseModel> GenerateNewToken(ApplicationUser user, LoginViewModel model)
        {
            // Create a key to encrypt the JWT 
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));

            // Get user role => check if user is admin
            var roles = await _userManager.GetRolesAsync(user);

            // Creating JWT token
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim("LoggedOn", DateTime.Now.ToString(CultureInfo.InvariantCulture)),
                }),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _appSettings.Site,
                Audience = _appSettings.Audience,
                Expires = (string.Equals(roles.FirstOrDefault(), "Administrator", StringComparison.CurrentCultureIgnoreCase)) ?
                    DateTime.UtcNow.AddMinutes(60) :
                    DateTime.UtcNow.AddMinutes(Convert.ToDouble(_appSettings.ExpireTime))
            };

            /* Create the unique encryption key for token - 2nd layer protection */
            var encryptionKeyRt = Guid.NewGuid().ToString();
            var encryptionKeyJwt = Guid.NewGuid().ToString();

            /* Get the Data protection service instance */
            var protectorProvider = _provider.GetService<IDataProtectionProvider>();

            /* Create a protector instance */
            var protectorJwt = protectorProvider.CreateProtector(encryptionKeyJwt);

            /* Generate Token and Protect the user token */
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptedToken = protectorJwt.Protect(tokenHandler.WriteToken(token));

            /* Create and update the token table */
            TokenModel newRToken = new TokenModel();

            /* Create refresh token instance */
            newRToken = CreateRefreshToken(_appSettings.ClientId, user.Id, Convert.ToInt32(_appSettings.RtExpireTime));

            /* assign the tne JWT encryption key */
            newRToken.EncryptionKeyJwt = encryptionKeyJwt;
            newRToken.EncryptionKeyRt = encryptionKeyRt;

            /* Add Refresh Token with Encryption Key for JWT to DB */
            try
            {
                // First we need to check if the user has already logged in and has tokens in DB
                var rt = _db.Tokens.FirstOrDefault(t => t.UserId == user.Id);

                if (rt != null)
                {
                    // invalidate the old refresh token (by deleting it)
                    _db.Tokens.Remove(rt);

                    // add the new refresh token
                    _db.Tokens.Add(newRToken);
                }
                else
                {
                    await _db.Tokens.AddAsync(newRToken);
                }

                // persist changes in the DB
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            // Return Response containing encrypted token
            var protectorRt = protectorProvider.CreateProtector(encryptionKeyRt);
            var layerOneProtector = protectorProvider.CreateProtector(_dataProtectionKeys.ApplicationUserKey);

            var encAuthToken = new TokenResponseModel
            {
                Token = layerOneProtector.Protect(encryptedToken),
                Expiration = token.ValidTo,
                RefreshToken = protectorRt.Protect(newRToken.Value),
                Role = roles.FirstOrDefault(),
                Username = user.UserName,
                UserId = layerOneProtector.Protect(user.Id),
                ResponseInfo = CreateResponse("Auth Token Created", HttpStatusCode.OK)
            };

            return encAuthToken;
        }

        // Private Methods
        private static TokenResponseModel CreateErrorResponseToken(string errorMessage, HttpStatusCode statusCode)
        {
            var errorToken = new TokenResponseModel
            {
                Token = null,
                Username = null,
                Role = null,
                RefreshTokenExpiration = DateTime.Now,
                RefreshToken = null,
                Expiration = DateTime.Now,
                ResponseInfo = CreateResponse(errorMessage, statusCode),
            };

            return errorToken;
        }

        private static ResponseStatusInfoModel CreateResponse(string errorMessage, HttpStatusCode statusCode)
        {
            var responseStatusInfo = new ResponseStatusInfoModel
            {
                Message = errorMessage,
                StatusCode = statusCode,
            };

            return responseStatusInfo;
        }

        private static TokenModel CreateRefreshToken(string clientId, string userId, int expireTime)
        {
            return new TokenModel()
            {
                ClientId = clientId,
                UserId = userId,
                Value = Guid.NewGuid().ToString("N"),
                CreatedDate = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMinutes(expireTime),
                EncryptionKeyRt = "",
                EncryptionKeyJwt = ""
            };
        }
    }
}

//---NUGET PACKAGES
//Microsoft.IdentityModel.Tokens
//System.IdentityModel.Tokens.Jwt
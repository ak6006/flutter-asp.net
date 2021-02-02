using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using API;
using API.Models;
using API.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using API.Results;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private Entities db = new Entities();
        private ApplicationDBContext AuthDB = new ApplicationDBContext();
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;


        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

       

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

       

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }


        // POST api/Account/PasswordCode
        [AllowAnonymous]
        [Route("PasswordCode")]
        public IHttpActionResult PasswordCode(string PhoneNumber)
        {
            var UserExist = AuthDB.Users.Where(u => u.PhoneNumber == PhoneNumber).FirstOrDefault();
            if(UserExist == null)
            {
                return BadRequest("invalid Phone Number");
            }
            var oldCode = db.PasswordCodes.Where(c => c.Phone == PhoneNumber);
            if(oldCode != null)
            {
                foreach (var item in oldCode)
                {
                    db.PasswordCodes.Remove(item);
                }
            }
            var deviceId = UserExist.DeviceToken;
            Random _rdm = new Random();
            var Code = _rdm.Next(1000, 9999);
            var input = new NotificationViewModel()
            {
                Title = "يرجى استخدام كود التفعيل",
                Msg = Code.ToString()
            };
            try
            {
                var PasswordCode = new PasswordCode()
                {
                    Code = Code.ToString(),
                    Phone = PhoneNumber,
                    Time = DateTime.Now
                };
                db.PasswordCodes.Add(PasswordCode);
                db.SaveChanges();
                PushNotification p = new PushNotification(input, deviceId);
                return Ok(Code);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        // POST api/Account/ChangePassword
        [AllowAnonymous]
        [Route("ChangePassword")]
        public IHttpActionResult ChangePassword(ChangePasswordModel model)
        {
            var UserExist = AuthDB.Users.Where(u => u.PhoneNumber == model.Phone ).FirstOrDefault();
            if (UserExist == null)
            {
                return BadRequest("Invalid Phone number");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            PasswordCode passwordCode = db.PasswordCodes.Where(p => p.Code == model.Code && p.Phone == model.Phone).FirstOrDefault();
            if (passwordCode == null)
            {
                return BadRequest("Invalid Code");
            }
            TimeSpan t = DateTime.Now - passwordCode.Time;
            if(t.TotalMinutes>30)
            {
                return BadRequest("This code is expired");
            }
            var user = AuthDB.Users.Where(u => u.PhoneNumber == model.Phone).FirstOrDefault();
            try
            {

                UserStore<ApplicationUser> store =
                            new UserStore<ApplicationUser>(new ApplicationDBContext());
                UserManager<ApplicationUser> manager =
                   new UserManager<ApplicationUser>(store);
                var newPasswordHash = manager.PasswordHasher.HashPassword(model.Password);

                store.SetPasswordHashAsync(user, newPasswordHash);
                //UserExist.FirstOrDefault().PasswordHash = newPasswordHash;
                AuthDB.SaveChanges();
                return Ok("Done");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            var UserExist = AuthDB.Users.Where(u => u.PhoneNumber == model.PhoneNumber).FirstOrDefault();
            if(UserExist!=null)
            {
                return BadRequest("Phone number has been used before");
            }
            var PhoneExist = db.addresses.Where(p => p.phone == model.PhoneNumber).ToList();
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
   
                var user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.UserName + "@gmail.com"
                };



                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (PhoneExist.Count != 0)
                    {
                        UserManager.AddToRole(user.Id, "Customer");
                    }
                    //string locat = Url.Link("DefaultApi", new { })
                    return Created("", "register Sucess " + user.UserName);
                }
                else
                    return BadRequest((result.Errors.ToList())[0]);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

       

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using API.Models;
using API.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(API.Startup1))]

namespace API
{
    public class Startup1
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }
        private ApplicationDBContext AuthDB = new ApplicationDBContext();
        public void CreateRoles()
        {
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(AuthDB));
            IdentityRole role;
            if (!roleManager.RoleExists("Admin"))
            {
                role = new IdentityRole
                {
                    Name = "Admin"
                };
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Customer"))
            {
                role = new IdentityRole
                {
                    Name = "Customer"
                };
                roleManager.Create(role);
            }

        }
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDBContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);


            PublicClientId = "self";

            CreateRoles();
            app.UseCors(CorsOptions.AllowAll);
            //use middle creat token cookie oauth
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions()
            {
                //URl http how expirestion
                TokenEndpointPath = new PathString("/login"),//http-https
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(3),
                AllowInsecureHttp = true,
                //how to create token (fields)==>
                Provider = new TokenCreate()
            });
            //Check token valid 
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                "DefaultApi", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            app.UseWebApi(config);
        }
    }
    //inherit class ==>override
    internal class TokenCreate : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //
            context.Validated();//any clientid Valid
        }
        //Check User Password ==>login {"":,""} ==>create Token
        public override async Task GrantResourceOwnerCredentials
            (OAuthGrantResourceOwnerCredentialsContext context)//Method//Request Body /login
        {
            //OWin Cors
            context.OwinContext.Response.Headers.Add(" Access - Control - Allow - Origin ", new[] { "*" });
            //Check
            UserStore<ApplicationUser> store =
                    new UserStore<ApplicationUser>(new ApplicationDBContext());

            UserManager<ApplicationUser> manager =
                new UserManager<ApplicationUser>(store);

            ApplicationUser user = await manager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("grant_error", "username & password Not Valid");//invalid
            }
            else
            {
                //create token
                ClaimsIdentity claims = new ClaimsIdentity(context.Options.AuthenticationType);//token beare -jwt cookie
                                                                                               //fields 


                ClaimsIdentity oAuthIdentity = await manager.CreateIdentityAsync(user,
               context.Options.AuthenticationType);

                ClaimsIdentity cookiesIdentity = await manager.CreateIdentityAsync(user,
                    CookieAuthenticationDefaults.AuthenticationType);
                List<Claim> roles = oAuthIdentity.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
                AuthenticationProperties properties = CreateProperties(user.UserName, Newtonsoft.Json.JsonConvert.SerializeObject(roles.Select(x => x.Value)));

                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(cookiesIdentity);
                //claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                //claims.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                //if (manager.IsInRole(user.Id, "Customer"))
                //    claims.AddClaim(new Claim(ClaimTypes.Role, "Customer"));

                //context.Validated(claims);//card field NAme,image,...role

            }
            //fields Token

        }

        public static AuthenticationProperties CreateProperties(string userName, string Roles)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
        {
            { "userName", userName },
            {"roles",Roles}
        };
            return new AuthenticationProperties(data);
        }


        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}

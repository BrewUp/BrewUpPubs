using System.Security.Claims;
using BrewUpPubs.Shared.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace BrewUpPubs.Modules
{
    public class AuthenticationModule : IModule
    {
        public bool IsEnabled => true;
        public int Order => 0;

        public IServiceCollection RegisterModule(WebApplicationBuilder builder)
        {
            var tokenParameters = new TokenParameters();
            builder.Configuration.GetSection("BrewUp:TokenParameters").Bind(tokenParameters);

            builder.Services.AddAuthentication(HandleAuthentication)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, "keycloak",
                    options => HandleOpenIdConnect(options, tokenParameters));

            builder.Services.AddAuthorization();
            
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Configure Identity to use the same JWT claims as OpenIddict instead
                // of the legacy WS-Federation claims it uses by default (ClaimTypes),
                // which saves you from doing the mapping in your authorization controller.
                // options.ClaimsIdentity.UserNameClaimType = Claims.Name;
                // options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
                // options.ClaimsIdentity.RoleClaimType = Claims.Role;
                // options.ClaimsIdentity.EmailClaimType = Claims.Email;

                // Note: to require account confirmation before login,
                // register an email sender service (IEmailSender) and
                // set options.SignIn.RequireConfirmedAccount to true.
                //
                // For more information, visit https://aka.ms/aspaccountconf.
                options.SignIn.RequireConfirmedAccount = false;
            });

            return builder.Services;
        }

        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            return endpoints;
        }

        #region Helpers
        private static void HandleAuthentication(AuthenticationOptions options)
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;            
        }
        
        private static void HandleOpenIdConnect(OpenIdConnectOptions options, TokenParameters tokenParameters)
        {
            options.SignInScheme = "Identity.External";
            options.Authority = tokenParameters.ServerRealm;
            options.ClientId = tokenParameters.ClientId;
            options.ClientSecret = tokenParameters.ClientSecret;
            options.MetadataAddress = tokenParameters.Metadata;
            //Require keycloak to use SSL
                
            options.GetClaimsFromUserInfoEndpoint = true;
            
            options.CallbackPath = new PathString("/Callback");
            
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            
            options.SaveTokens = true;
            
            options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
            options.NonceCookie.SameSite = SameSiteMode.None;
            options.CorrelationCookie.SameSite = SameSiteMode.None;
            
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.RequireHttpsMetadata = false; //dev

            options.TokenValidationParameters = new TokenValidationParameters
            {
                // NameClaimType = "name",
                // RoleClaimType = ClaimTypes.Role,
                ValidateIssuer = true,
                ValidateLifetime = true
            };
        }
        #endregion
    }
}
using System.Security.Claims;
using BrewUpPubs.Shared.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
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
                .AddCookie(HandleCookies)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, "keycloak",
                    options => HandleOpenIdConnect(options, tokenParameters));

            //builder.Services.AddAuthorization(HandleAuthorizations);
            builder.Services.AddAuthorization();

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

        private static void HandleCookies(CookieAuthenticationOptions options)
        {
            options.Cookie.Name = "keycloak.cookie";
            options.Cookie.MaxAge = TimeSpan.FromMinutes(60);
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.SlidingExpiration = true;

            options.Events = new CookieAuthenticationEvents
            {
                OnValidatePrincipal = async cookieCtx =>
                {
                    var now = DateTimeOffset.UtcNow;
                    var expiresAt = cookieCtx.Properties.GetTokenValue("expires_at");
                    var accessTokenExpiration = expiresAt is not null
                        ? DateTimeOffset.Parse(expiresAt)
                        : DateTimeOffset.MinValue;
                    var timeRemaining = accessTokenExpiration.Subtract(now);

                    if (timeRemaining.TotalSeconds < 0)
                    {
                        cookieCtx.RejectPrincipal();
                        await cookieCtx.HttpContext.SignOutAsync();
                    }
                }
            };
        }
        
        private static void HandleOpenIdConnect(OpenIdConnectOptions options, TokenParameters tokenParameters)
        {
            options.RequireHttpsMetadata = false;

            //Use default signin scheme
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //Keycloak server
            options.Authority = tokenParameters.ServerRealm;
            //Keycloak client ID
            options.ClientId = tokenParameters.ClientId;
            //Keycloak client secret
            options.ClientSecret = tokenParameters.ClientSecret;
            //Keycloak .wellknown config origin to fetch config
            options.MetadataAddress = tokenParameters.Metadata;
            
            //Require keycloak to use SSL
            //options.RequireHttpsMetadata = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            
            //Save the token
            options.SaveTokens = true;
            //Token response type, will sometimes need to be changed to IdToken, depending on config.
            options.ResponseType = OpenIdConnectResponseType.Code;
            //SameSite is needed for Chrome/Firefox, as they will give http error 500 back, if not set to unspecified.
            options.NonceCookie.SameSite = SameSiteMode.Unspecified;
            options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;
                
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = ClaimTypes.Role,
                ValidateIssuer = true
            };
        }

        private static void HandleAuthorizations(AuthorizationOptions options)
        {
            //Create policy with more than one claim
            options.AddPolicy("users", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Value == "user" || c.Value == "admin")));
            
            //Create policy with only one claim
            options.AddPolicy("Tempo", policy =>
                policy.RequireClaim(ClaimTypes.Role, "Tempo"));
            
            //Create a policy with a claim that doesn't exist or you are unauthorized to
            options.AddPolicy("noaccess", policy =>
                policy.RequireClaim(ClaimTypes.Role, "noaccess"));
        }
        #endregion
    }
}
using BrewUpPubs.Shared.Configuration;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
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

            builder.Services.AddAuthentication(AzureADDefaults.JwtBearerAuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

            builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Events.OnTokenValidated = async context =>
                {
                    options.MetadataAddress = tokenParameters.MetadataUri;

                    options.TokenValidationParameters.NameClaimType = "PubsApi";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuers = new List<string>(),
                        ValidAudiences = new List<string>(),
                        // The signing key must match!
                        ValidateIssuerSigningKey = false,
                        // Validate the token expiry
                        ValidateLifetime = true,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = HandleTokenValidated,
                        OnChallenge = HandleChallenge
                    };
                };
            });

            builder.Services.AddAuthorization();

            return builder.Services;
        }

        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            return endpoints;
        }

        #region Helpers
        private static void SetJwtBearerOptions(JwtBearerOptions options, TokenParameters tokenParameters)
        {
            // For check signing key
            options.RequireHttpsMetadata = true;
            options.MetadataAddress = tokenParameters.MetadataUri;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                // Validate the token expiry
                ValidateLifetime = true,
            };
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = HandleTokenValidated,
                OnChallenge = HandleChallenge
            };
        }

        private static Task HandleTokenValidated(TokenValidatedContext context)
        {
            context.HttpContext.Items["Token"] = context.SecurityToken;
            return Task.CompletedTask;
        }

        private static Task HandleChallenge(JwtBearerChallengeContext context)
        {
            // Skip the default logic.
            context.HandleResponse();

            var ex = new Exception(context.ErrorDescription ?? "Unknown error");

            switch (context.AuthenticateFailure)
            {
                case null when string.IsNullOrWhiteSpace(context.Request.Headers.Authorization):
                    ex = new Exception("The token is required");
                    break;
                case SecurityTokenExpiredException:
                    context.Response.Headers.Add("Token-Expired", "true");
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            return context.Response.WriteAsync("An error occurred while checking JWT Token");
        }
        #endregion
    }
}
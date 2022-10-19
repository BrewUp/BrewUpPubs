using System.Security.Claims;
using System.Text.Json;
using BrewUpPubs.Shared.Configuration;
using BrewUpPubs.Transformers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Logging;
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

            builder.Services.AddTransient<IClaimsTransformation, ClaimsTransformers>();

            IdentityModelEventSource.ShowPII = true;
            builder.Services.AddAuthentication(HandleJwtAuthenticationOptions)
                .AddJwtBearer(options => SetJwtBearerOptions(options, tokenParameters));

            builder.Services.AddAuthorization(HandleAuthorizations);
            builder.Services.AddAuthorization();

            return builder.Services;
        }

        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            return endpoints;
        }

        #region Helpers
        private static void HandleJwtAuthenticationOptions(AuthenticationOptions options)
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
        
        private static void SetJwtBearerOptions(JwtBearerOptions options, TokenParameters tokenParameters)
        {
            // For check signing key
            options.RequireHttpsMetadata = true;
            options.MetadataAddress = tokenParameters.Metadata;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                //IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenParameters.ClientSecret)),
                
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = tokenParameters.ServerRealm,
                // // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudiences = new List<string>
                {
                    "account"
                },
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

            var error = new
            {
                Title = "BrewUp Security",
                StatusCodes = StatusCodes.Status401Unauthorized,
                Code = "Unauthorized",
                Details = ex.Message
            };
            var jsonString = JsonSerializer.Serialize(error);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            return context.Response.WriteAsync(jsonString);
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
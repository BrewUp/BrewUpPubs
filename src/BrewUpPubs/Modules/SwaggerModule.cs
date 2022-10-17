using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BrewUpPubs.Modules;


public sealed class SwaggerModule : IModule
{
    public bool IsEnabled => true;
    public int Order => 0;

    public const string BearerAuthId = "Bearer";

    public IServiceCollection RegisterModule(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(SetSwaggerGenOptions);

        return builder.Services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        return endpoints;
    }

    private void SetSwaggerGenOptions(SwaggerGenOptions options)
    {
        options.OperationFilter<SecurityRequirementsOperationFilter>();
        options.SwaggerDoc("v2", new OpenApiInfo
        {
            Title = "BrewUp Pubs Api",
            Version = "v1"
        });
        options.AddSecurityDefinition(BearerAuthId, new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            In = ParameterLocation.Header,
            Name = "Authorization",
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
            Description = "Please enter a valid token"
        });

        ConfigureXmlComments(options);
    }

    private void ConfigureXmlComments(SwaggerGenOptions options)
    {
        // Tells swagger to pick up the output XML document file
        options.IncludeXmlComments(Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            $"{GetType().Assembly.GetName().Name}.xml"
        ));

        // Collect all referenced projects output XML document file paths
        var currentAssembly = Assembly.GetExecutingAssembly();
        var xmlDocs = currentAssembly.GetReferencedAssemblies()
            .Union(new[] { currentAssembly.GetName() })
            .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location)!, $"{a.Name}.xml"))
            .Where(File.Exists).ToArray();

        Array.ForEach(xmlDocs, (d) => { options.IncludeXmlComments(d); });
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
internal class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var requiredScopes = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Select(attr => attr.AuthenticationSchemes)
            .Distinct()
            .ToArray();

        var requireAuth = false;
        var id = string.Empty;

        if (requiredScopes.Contains(JwtBearerDefaults.AuthenticationScheme))
        {
            requireAuth = true;
            id = SwaggerModule.BearerAuthId;
        }

        if (!requireAuth || string.IsNullOrEmpty(id)) return;

        operation.Responses.Add("401", new OpenApiResponse
        {
            Description = "Unauthorized",
        });
        operation.Responses.Add("403", new OpenApiResponse
        {
            Description = "Forbidden",
        });

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = id
                        }
                    },
                    Array.Empty<string>()
                }
            }
        };
    }
}

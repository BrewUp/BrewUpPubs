using Microsoft.OpenApi.Models;

namespace BrewUpPubs.Modules
{
    public sealed class SwaggerModule : IModule
    {
        public bool IsEnabled => true;
        public int Order => 0;

        public IServiceCollection RegisterModule(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(setup => setup.SwaggerDoc("v1", new OpenApiInfo()
            {
                Description = "BrewUp Pubs API",
                Title = "BrewUp Pubs Api",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "BrewUp.Api"
                }
            }));

            return builder.Services;
        }

        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            return endpoints;
        }
    }
}
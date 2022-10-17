namespace BrewUpPubs.Modules
{
    public sealed class CorsModule : IModule
    {
        public bool IsEnabled => true;
        public int Order => 0;

        public IServiceCollection RegisterModule(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", corsBuilder =>
                    corsBuilder.AllowAnyMethod()
                        .WithOrigins("http://127.0.0.1:8043", "http://127.0.0.1:8043")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowCredentials()
                        .AllowAnyHeader());
            });

            return builder.Services;
        }

        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            return endpoints;
        }
    }
}
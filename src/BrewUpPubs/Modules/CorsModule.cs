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
                        .WithOrigins("http://localhost:8080", "http://localhost:8080")
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
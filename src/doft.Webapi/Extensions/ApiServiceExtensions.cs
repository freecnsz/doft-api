using Microsoft.OpenApi.Models;

namespace doft.Webapi.Extensions
{
    public static class ApiServiceExtensions
    {
        public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "doft.Webapi", Version = "v1" });
            });
            return services;
        }

        
    }
}

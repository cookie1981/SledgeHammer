using MicroMachines.Common.Core.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RiskCaptureCorsOptions = profiling.Configuration.RiskCaptureCorsOptions;
using WipRisk = profiling.Models.WipRisk;

namespace profiling.ServiceCollectionExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCorsPolicies(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddCors()
                .AddConfiguration<CorsConfiguration>(configuration.GetSection("Cors"))
                .AddSingleton<IConfigureOptions<CorsOptions>, RiskCaptureCorsOptions>();

            return services;
        }

        public static IServiceCollection AddMongoHealthCheck(this IServiceCollection services)
        {
            services.AddHealthCheck<profiling.HealthChecks.MongoServiceHealthCheck<WipRisk>>();
            return services;
        }
    }
}
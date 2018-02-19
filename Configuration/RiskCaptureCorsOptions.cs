using MicroMachines.Common.Core.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace profiling.Configuration
{
    public class RiskCaptureCorsOptions : IConfigureOptions<CorsOptions>
    {
        private readonly CorsConfiguration _configuration;

        public RiskCaptureCorsOptions(CorsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(CorsOptions options)
        {
            options.AddPolicy("GetPolicy", x =>
            {
                x.AddDefaultOptions(_configuration)
                    .WithMethods("GET");
            });

            options.AddPolicy("PostPolicy", x =>
            {
                x.AddDefaultOptions(_configuration)
                    .WithMethods("POST")
                    .WithExposedHeaders("Location");
            });

            options.AddPolicy("PutPolicy", x =>
            {
                x.AddDefaultOptions(_configuration)
                    .WithMethods("PUT")
                    .WithExposedHeaders("Location");
            });
        }
    }
}
using System;
using MicroMachines.Common.Interfaces;

namespace profiling.Configuration
{
    public class RiskValidationConfiguration : IHttpClientConfiguration
    {
        public Uri Uri { get; set; }
        public Uri Ping { get; set; } = new Uri("private/ping", UriKind.Relative);
        public int? Timeout { get; set; }
        public string ServiceName { get; } = "RiskValidation";
    }
}

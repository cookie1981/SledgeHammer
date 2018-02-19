using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MicroMachines.Common.Interfaces;
using MicroMachines.Common.Models;
using Newtonsoft.Json.Linq;
using RiskValidationConfiguration = profiling.Configuration.RiskValidationConfiguration;
using RiskValidationResult = profiling.Models.RiskValidationResult;

namespace profiling.Services
{
    public class RiskValidationServiceWrapper : IRiskValidationServiceWrapper
    {
        private readonly HttpClient _riskValidationServiceClient;

        public RiskValidationServiceWrapper(IHttpClientProvider<RiskValidationConfiguration> httpClientProvider)
        {
            _riskValidationServiceClient = httpClientProvider.HttpClient;
        }

        private const string ApplicationJsonMediaType = "application/json";

        public async Task<RiskValidationResult> Validate(ProductVersion productVersion, string risk)
        {
            var riskValidationResult = new RiskValidationResult();
            try
            {
                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"/validate/product/{productVersion.Product.ToString().ToLowerInvariant()}/version/{productVersion.Version}"))
                {
                    using (httpRequestMessage.Content = new StringContent(risk, Encoding.UTF8, ApplicationJsonMediaType))
                    using (var result = await _riskValidationServiceClient.SendAsync(httpRequestMessage))
                    {
                        riskValidationResult.StatusCode = result.StatusCode;
                        riskValidationResult.Result = await TryParseContent(result.Content);
                    }
                }

                return riskValidationResult;
            }
            catch (Exception ex)
            {
                throw new RiskValidationFailedException("Catastrophic failure calling Risk Validation Service.", ex);
            }
        }

        private static async Task<JObject> TryParseContent(HttpContent content)
        {
            JObject result = null;
            try
            {
                if (content != null)
                {
                    result = await content.ReadAsJsonAsync<JObject>();
                }
            }
            catch
            {
                //do nothing!
            }

            return result;
        }
    }
}
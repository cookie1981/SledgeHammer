using System.Net;
using System.Threading.Tasks;
using MicroMachines.Common.Models;
using Newtonsoft.Json.Linq;
using profiling.Models;

//using RiskCapture.Models;
//using RiskCapture.Services;

namespace profiling.RequestValidation
{
    public class StubRiskValidationServiceWrapper : Services.IRiskValidationServiceWrapper
    {
        public Task<RiskValidationResult> Validate(ProductVersion productVersion, string risk)
        {
            var fakeResult = new RiskValidationResult()
            {
                StatusCode = HttpStatusCodeToReturn,
                Result = Result
            };

            return Task.FromResult(fakeResult);
        }

        public HttpStatusCode HttpStatusCodeToReturn { get; set; } = HttpStatusCode.Accepted;

        public JObject Result { get; set; }
    }
}

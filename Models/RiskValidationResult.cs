using System.Net;
using Newtonsoft.Json.Linq;

namespace profiling.Models
{
    public class RiskValidationResult
    { 
        public JObject Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
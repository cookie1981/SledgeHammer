using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MicroMachines.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;

namespace profiling.RequestValidation
{
    public interface IRequestValidator
    {
        Task<(HttpStatusCode StatusCode, JObject Result, List<string> Errors)> ValidateAsync(ModelStateDictionary modelState, 
            ProductVersion productVersion, JObject wip);
    }
}
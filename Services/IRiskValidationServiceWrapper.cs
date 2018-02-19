using System.Threading.Tasks;
using MicroMachines.Common.Models;
using RiskValidationResult = profiling.Models.RiskValidationResult;

namespace profiling.Services
{
    public interface IRiskValidationServiceWrapper
    {
        Task<RiskValidationResult> Validate(ProductVersion productVersion, string risk);
    }
}
using System.Threading.Tasks;
using MicroMachines.Common.Models;
using Newtonsoft.Json.Linq;

namespace profiling.Storage
{
    public interface IStorageService
    {
        Task<string> Save(ProductVersion productVersion, JObject wip);
        Task<JObject> Fetch(ProductVersion productVersion, string wipId);
        Task<JObject> Update(ProductVersion productVersion, JObject wip, string wipId);
    }
}
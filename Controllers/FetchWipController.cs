using System.Threading.Tasks;
using MicroMachines.Common;
using MicroMachines.Common.Core.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Risk.Schema.Validator;
//using RiskCapture.Controllers;
//using RiskCapture.Storage;
//using RiskCapture.Storage.Mongo;
using DataAccessException = profiling.Storage.Mongo.DataAccessException;
using IStorageService = profiling.Storage.IStorageService;
using SessionMismatchException = profiling.Storage.SessionMismatchException;

namespace profiling.Controllers
{
    [Route("capture/product/{product}/version/{version}")]
    public class FetchWipController : RiskCaptureControllerBase
    {
        public FetchWipController(IStorageService storageService, ISchemaValidator schemaValidator)
            : base(storageService, schemaValidator)
        {
        }

        [EnableCors("GetPolicy")]
        [HttpGet("wip/{wipId}")]
        [RequiredHeaders(CTMHeaders.SessionId, CTMHeaders.VisitorId)]
        public async Task<IActionResult> Fetch([FromRoute] string product, [FromRoute] string version, [FromRoute] string wipId)
        {
            if (!ModelState.IsValid)
            {
                return Forbidden();
            }

            try
            {
                var wip = await StorageService.Fetch(ProductVersion(product, version), wipId);

                if (wip == null)
                {
                    return NotFound();
                }

                return Ok(wip);
            }
            catch (DataAccessException ex)
            {
                 return BadGateway(ex.Message);
            }
            catch (SessionMismatchException)
            {
                return Forbidden();
            }
        }
    }
}
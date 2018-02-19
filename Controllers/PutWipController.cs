using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MicroMachines.Common;
using MicroMachines.Common.Core.Helpers;
using MicroMachines.Common.Interfaces;
using MicroMachines.Common.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Risk.Schema.Validator;
using IRequestValidator = profiling.RequestValidation.IRequestValidator;
using IStorageService = profiling.Storage.IStorageService;
using RiskValidationFailedException = profiling.Services.RiskValidationFailedException;
using SessionMismatchException = profiling.Storage.SessionMismatchException;

namespace profiling.Controllers
{
    [Route("capture/product/{product}/version/{version}")]
    public class PutWipController : RiskCaptureControllerBase
    {
        public PutWipController(IGuidGenerator guidGenerator, 
            IStorageService storageService, 
            ISchemaValidator schemaValidator,
            IRequestValidator requestValidator) : 
            base(storageService, schemaValidator, requestValidator)
        {
        }

        [EnableCors("PutPolicy")]
        [RequiredHeaders(CTMHeaders.SessionId, CTMHeaders.VisitorId)]
        [HttpPut("wip/{wipId}")]
        public async Task<IActionResult> Put([FromRoute] string product, [FromRoute] string version, [FromRoute] string wipId, [FromBody] JObject wip)
        {
            return await PUTIT(product, version, wipId, wip);
        }

        public async Task<IActionResult> PUTIT(string product, string version, string wipId, JObject wip)
        {
            try
            {
                var productVersion = ProductVersion(product, version);
                var validationResult = await RequestValidator.ValidateAsync(ModelState, productVersion, wip);

                if (validationResult.StatusCode != HttpStatusCode.OK)
                {
                    return validationResult.Errors.Any()
                        ? StatusCode((int) validationResult.StatusCode, validationResult.Errors)
                        : StatusCode((int) validationResult.StatusCode, validationResult.Result);
                }

                var result = await StorageService.Update(productVersion, wip, wipId);

                if (result == null)
                {
                    return NotFound();
                }

                AddLocationHeaderToResponse(productVersion, wipId);

                if (validationResult.Result == null)
                {
                    return NoContent();
                }
                return Ok(validationResult.Result);
            }
            catch (RiskValidationFailedException ex)
            {
                return BadGateway(ex.Message);
            }
            catch (SessionMismatchException)
            {
                return Forbidden();
            }
        }

        private void AddLocationHeaderToResponse(ProductVersion productVersion, string wipId)
        {
//            var locationUri = CreateUriOfNewResource(productVersion, wipId);

//            HttpContext.Response.Headers.Add("Location", locationUri.ToString());
        }
    }
}
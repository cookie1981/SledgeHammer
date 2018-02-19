using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MicroMachines.Common;
using MicroMachines.Common.Core.Helpers;
using MicroMachines.Common.Exceptions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Risk.Schema.Validator;
//using RiskCapture.RequestValidation;
//using RiskCapture.Services;
//using RiskCapture.Storage;
//using RiskCapture.Storage.Mongo;
using DataAccessException = profiling.Storage.Mongo.DataAccessException;
using IRequestValidator = profiling.RequestValidation.IRequestValidator;
using IStorageService = profiling.Storage.IStorageService;
using RiskValidationFailedException = profiling.Services.RiskValidationFailedException;

namespace profiling.Controllers
{
    [Route("capture/product/{product}/version/{version}")]
    public class SaveWipController : profiling.Controllers.RiskCaptureControllerBase
    {
        public SaveWipController(IStorageService storageService, 
                                    ISchemaValidator schemaValidator, 
                                    IRequestValidator requestValidator) 
            : base(storageService, schemaValidator, requestValidator)
        {
        }

        [EnableCors("PostPolicy")]
        [HttpPost]
        [RequiredHeaders(CTMHeaders.SessionId, CTMHeaders.VisitorId)]
        public async Task<IActionResult> Save([FromRoute] string product, [FromRoute] string version,
            [FromBody] JObject wip)
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

                var wipId = await StorageService.Save(productVersion, wip);
                return Created(CreateUriOfNewResource(productVersion, wipId), validationResult.Result);
            }
            catch (HttpRequestTimeoutException)
            {
                return GatewayTimeout();
            }
            catch (HttpBadResponseException)
            {
                return BadGateway();
            }
            catch (DataAccessException ex)
            {
                return BadGateway(ex.Message);
            }
            catch (RiskValidationFailedException ex)
            {
                return BadGateway(ex.Message);
            }
        }
    }
}
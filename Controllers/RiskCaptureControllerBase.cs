using System;
using System.Net;
using MicroMachines.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Risk.Schema.Validator;
//using RiskCapture.HttpResponses;
//using RiskCapture.RequestValidation;
//using RiskCapture.Storage;
using BadGatewayResult = profiling.HttpResponses.BadGatewayResult;
using ForbiddenResult = profiling.HttpResponses.ForbiddenResult;
using GatewayTimeoutResult = profiling.HttpResponses.GatewayTimeoutResult;
using IRequestValidator = profiling.RequestValidation.IRequestValidator;
using IStorageService = profiling.Storage.IStorageService;

namespace profiling.Controllers
{
    public class RiskCaptureControllerBase : Controller
    {
        protected readonly IStorageService StorageService;
        protected readonly ISchemaValidator SchemaValidator;
        protected readonly IRequestValidator RequestValidator;

        protected RiskCaptureControllerBase(IStorageService storageService,
                                            ISchemaValidator schemaValidator,
                                            IRequestValidator requestValidator)
        {
            StorageService = storageService;
            SchemaValidator = schemaValidator;
            RequestValidator = requestValidator;
        }

        protected RiskCaptureControllerBase(IStorageService storageService, ISchemaValidator schemaValidator)
        {
            StorageService = storageService;
            SchemaValidator = schemaValidator;
        }

        protected Uri CreateUriOfNewResource(ProductVersion productVersion, string wipId)
        {
            return new Uri($"{Request.Scheme}://{Request.Host}/capture/product/{productVersion.Product.ToString().ToLowerInvariant()}/version/{productVersion.Version}/wip/{wipId}");
        }

        protected ProductVersion ProductVersion(string product, string version) => MicroMachines.Common.Models.ProductVersion.Parse(product + "/" + version);

        [NonAction]
        public virtual BadGatewayResult BadGateway() => new BadGatewayResult();

        [NonAction]
        public virtual GatewayTimeoutResult GatewayTimeout() => new GatewayTimeoutResult();

        [NonAction]
        public virtual ForbiddenResult Forbidden() => new ForbiddenResult();

        protected ObjectResult BadGateway(string message)
        {
            return StatusCode((int)HttpStatusCode.BadGateway, message);
        }
    }
}
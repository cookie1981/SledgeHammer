using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MicroMachines.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using profiling.Services;
using Risk.Schema.Validator;

namespace profiling.RequestValidation
{
    public class RiskCaptureRequestValidator : IRequestValidator
    {
        private readonly ISchemaValidator _schemaValidator;
        private readonly IRiskValidationServiceWrapper _riskValidationService;
        private readonly ILogger<RiskCaptureRequestValidator> _logger;

        public RiskCaptureRequestValidator(ISchemaValidator schemaValidator, IRiskValidationServiceWrapper riskValidationService, ILogger<RiskCaptureRequestValidator> logger)
        {
            _schemaValidator = schemaValidator;
            _riskValidationService = riskValidationService;
            _logger = logger;
        }

        public async Task<(HttpStatusCode StatusCode, JObject Result, List<string> Errors)> ValidateAsync(ModelStateDictionary modelState, ProductVersion productVersion, JObject wip)
        {
            var errorsToReturn = new List<string>();

            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }

            if (!modelState.IsValid)
            {
                return (HttpStatusCode.BadRequest, null, errorsToReturn);
            }

            if (wip == null)
            {
                errorsToReturn.Add("wip is empty");
                return (HttpStatusCode.BadRequest, null, errorsToReturn);
            }

            var (containsRequiredElements, errors) = WipContainsRequiredElements(wip, productVersion);

            if (!containsRequiredElements)
            {
                errorsToReturn.AddRange(errors);
                return (HttpStatusCode.BadRequest, null, errorsToReturn);
            }

            if (_schemaValidator.DoesNotContainSchema(productVersion.Product.ToString().ToLowerInvariant()))
            {
                var errorMessage = $"Product schema does not exist for product:{productVersion.Product.ToString().ToLowerInvariant()}";
                _logger.LogError(errorMessage);
                errorsToReturn.Add(errorMessage);
                return (HttpStatusCode.BadRequest, null, errorsToReturn);
            }

            var validationResult = await _riskValidationService.Validate(productVersion, ExtractRiskJsonFromPayload(wip));

            if (validationResult.StatusCode == HttpStatusCode.BadRequest)
            {
                return (HttpStatusCode.BadRequest, validationResult.Result, errorsToReturn);
            }

            if (!IsRiskValidForCapture(validationResult.StatusCode))
            {
                const string errorMessage = "Unexpected response from the risk validation.";
                _logger.LogError($"{errorMessage}. StatusCode: {validationResult.StatusCode}");
                errorsToReturn.Add(errorMessage);

                return (HttpStatusCode.BadGateway, null, errorsToReturn);
            }

            return (HttpStatusCode.OK, validationResult.Result, errorsToReturn);
        }

        private static IEnumerable<string> BuildRequiredElements(ProductVersion productVersion)
        {
            if (productVersion.Product.ToString().ToLower() == "bike" && productVersion.Version.Major <= 2 &&
                (productVersion.Version.Major == 1 || productVersion.Version.Minor < 1))
            {
                return new[] {ExpectedWipElements.Risk};
            }

            return new[] { ExpectedWipElements.Metadata, ExpectedWipElements.Risk };
        }

        private static (bool ContainsRequiredElements, List<string> Errors) WipContainsRequiredElements(JObject wip, ProductVersion productVersion)
        {
            var requiredElements = BuildRequiredElements(productVersion);

            var errors = (from element in requiredElements
                          where wip[element] == null
                          select $"Wip is missing required element: {element}").ToList();

            return (!errors.Any(), errors);
        }
        
        public static bool IsRiskValidForCapture(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.NoContent || statusCode == HttpStatusCode.Accepted;
        }

        private static string ExtractRiskJsonFromPayload(JObject wip)
        {
            if (wip != null && wip.TryGetValue(ExpectedWipElements.Risk, StringComparison.CurrentCultureIgnoreCase, out var extractedRisk))
            {
                return extractedRisk.ToString();
            }

            throw new MissingElementFromWipException("Payload is missing");
        }
    }
}
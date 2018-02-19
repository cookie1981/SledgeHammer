using System.Threading.Tasks;
using MicroMachines.Common.Testing;
using Newtonsoft.Json.Linq;
using Risk.Schema.Validator;
using RiskCapture.Configuration;
using RiskCapture.Models;
using RiskCapture.RequestValidation;
using RiskCapture.Storage;
using RiskCapture.Storage.Mongo;
using RiskCapture.Tests.Bson;
using RiskCapture.Tests.DateTimeProvider;
using RiskCapture.Tests.RequestValidation;

namespace Perf2
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var stubGuidGenerator = new StubGuidGenerator();
            var dateTimeProvider = new FakeDateTimeProvider();
            var fakeBsonIdProvider = new FakeBsonIdProvider();
            var riskCaptureMongoConfiguration = new RiskCaptureMongoConfiguration
            {
                MongoConnection = "mongodb://localhost:27017"
            };

            var mongoClientBuilder = new MongoClientBuilder();
            var mongoDataRepository = new MongoDataRepository<WipRisk>(new MongoCollectionProvider<WipRisk>(riskCaptureMongoConfiguration, mongoClientBuilder));
            var stubTrackingInfo = new StubTrackingInfo();
            var stubStorageService = new StubLogger<StorageService>();
            var slogger = new StubLogger<RiskCaptureRequestValidator>();

            var storageService = new StorageService(mongoDataRepository, stubTrackingInfo, stubStorageService, dateTimeProvider, fakeBsonIdProvider);
            var schemaValidator = new SchemaValidator();
            //            var riskValidationConfiguration = new RiskValidationConfiguration()
            //            {
            //                Uri = new Uri("https://risk-validation-product.shadow.ctmers.io/")
            //            };
            //            var builder = new HttpClientBuilder();

            //            var httpClientFactory = new HttpClientFactory();
            //            var httpClientProvider = new HttpClientProvider<RiskValidationConfiguration>(httpClientFactory, riskValidationConfiguration);
            //            var riskValidationServiceWrapper = new RiskValidationServiceWrapper(httpClientProvider);
            var stubRiskValidationServiceWrapper = new StubRiskValidationServiceWrapper();
            var riskCaptureRequestValidator = new RiskCaptureRequestValidator(schemaValidator, stubRiskValidationServiceWrapper, slogger);
            var controller = new RiskCapture.Controllers.PutWipController(stubGuidGenerator, storageService, schemaValidator, riskCaptureRequestValidator);

            var wip = JObject.Parse("");

            for (var x = 0; x < 10000; x++)
            {
                var response = await controller.PUTIT("Car", "2.0", "5a6081d1260725212482594c", wip);
            }
        }
    }
}

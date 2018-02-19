using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicroMachines.Common;
using MicroMachines.Common.Net;
using MicroMachines.Common.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RiskCapture.Tests.Controllers;

namespace profiling
{
    class Program
    {
        private static HttpClient Client = new HttpClient();


        public static async Task Main()
        {
            //await OldProfilingCode();
            var random = new Random(0);
            GuidGenerator = new StubGuidGenerator();
            const string uri = "http://localhost:9010/capture/product/bike/version/2.0/wip/5a7a2367593b3254c8ba1930";
            
            var content = JObject.Parse(json);

            //for (var x = 0; x < 100; x++)
            while (true)
            {
                var waitTimeMs = random.Next(100, 3000);
                Thread.Sleep(waitTimeMs);
                var request = CreateRequest(uri, content, HttpMethod.Put);
                try
                {
                    HttpResponseMessage responseMessage;
                    responseMessage = await Client.SendAsync(request);
                    Console.WriteLine(responseMessage?.StatusCode);
                }
                catch (Exception)
                {
                }
            }
        }

//        public static TestBase.Wip BuildWip()
//        {
//            var testRisk = new TestRisk { Property = "value" };
//            var testMetaData = new TestMetaData { Outboundme = false };
//
//            return new TestBase.Wip { risk = testRisk, metadata = testMetaData };
//        }
//
//        internal class TestRisk
//        {
//            public string Property;
//        }
//
//        internal class TestInvalidRisk : TestRisk
//        {
//            public string OtherProperty = "flange";
//        }
//
//        internal class TestMetaData
//        {
//            public bool Outboundme;
//        }

        protected static  StubGuidGenerator GuidGenerator { get; private set; }

        protected static HttpRequestMessage CreateRequestWithHeaders(string uri, HttpMethod method)
        {
            var request = new HttpRequestMessage(method, uri)
            {
                Headers =
                {
                    {CTMHeaders.SessionId, "1e640fd2-debc-644c-adfc-69902c949c22"},
                    {CTMHeaders.CausationId, GuidGenerator.Generate().ToString()},
                    {CTMHeaders.CorrelationId, GuidGenerator.Generate().ToString()},
                    {CTMHeaders.VisitorId, "1e640fd2-debc-644c-adfc-69902c949c22"},
                    {"Origin", "http://localhost"}
                }
            };
            return request;
        }

        protected static HttpRequestMessage CreateRequest(string uri, Object wip, HttpMethod method, bool notAWip = false)
        {
            var request = CreateRequestWithHeaders(uri, method);

            if (method == HttpMethod.Post || method == HttpMethod.Put)
            {
                if (wip != null)
                {
                    request.Content = new JsonContent(wip, new JsonSerializerSettings());
                }

                if (wip == null)
                {
                    if (!notAWip)
                    {
                        request.Content = new ByteArrayContent(new byte[0])
                        {
                            Headers = { { "Content-Type", "application/json" } }
                        };
                    }
                    if (notAWip)
                    {
                        request.Content = new StringContent("{yo", Encoding.UTF8, "application/json");
                    }
                }
            }

            return request;
        }

//        private static async Task OldProfilingCode()
//        {
//            var stubGuidGenerator = new StubGuidGenerator();
//            var dateTimeProvider = new FakeDateTimeProvider();
//            dateTimeProvider.Now = DateTime.UtcNow;
//            var fakeBsonIdProvider = new FakeBsonIdProvider();
//            var riskCaptureMongoConfiguration = new RiskCaptureMongoConfiguration
//            {
//                MongoConnection = "mongodb://localhost:27017"
//            };
//
//            var mongoClientBuilder = new MongoClientBuilder();
//            var mongoDataRepository =
//                new MongoDataRepository<WipRisk>(
//                    new MongoCollectionProvider<WipRisk>(riskCaptureMongoConfiguration, mongoClientBuilder));
//            var stubTrackingInfo = new StubTrackingInfo();
//            var stubStorageService = new StubLogger<StorageService>();
//            var slogger = new StubLogger<RiskCaptureRequestValidator>();
//
//            var storageService = new StorageService(mongoDataRepository,
//                stubTrackingInfo, stubStorageService,
//                dateTimeProvider, fakeBsonIdProvider);
//            var schemaValidator = new SchemaValidator();
//            //            var riskValidationConfiguration = new RiskValidationConfiguration()
//            //            {
//            //                Uri = new Uri("https://risk-validation-product.shadow.ctmers.io/")
//            //            };
//            //            var builder = new HttpClientBuilder();
//
//            //            var httpClientFactory = new HttpClientFactory();
//            //            var httpClientProvider = new HttpClientProvider<RiskValidationConfiguration>(httpClientFactory, riskValidationConfiguration);
//            //            var riskValidationServiceWrapper = new RiskValidationServiceWrapper(httpClientProvider);
//            var stubRiskValidationServiceWrapper = new StubRiskValidationServiceWrapper();
//            var riskCaptureRequestValidator =
//                new RiskCaptureRequestValidator(schemaValidator, stubRiskValidationServiceWrapper, slogger);
//            var controller =
//                new PutWipController(stubGuidGenerator, storageService, schemaValidator, riskCaptureRequestValidator);
//
//            var wip = JObject.Parse(json);
//
//            for (var x = 0; x < 100; x++)
//            {
//                Console.WriteLine($"calling controller for nth time:{x}");
//                var response = await controller.PUTIT("Car", "2.0", "5a6b58fc85cd5055ec6a7494", wip);
//            }
//
//            Console.ReadLine();
//        }
//
        private const string json = "{\n    \"risk\": {\n        \"VoluntaryExcess\": {\n            \"dataCode\": \"100\",\n            \"displayText\": \"£100\"\n        },\n        \"ContactDetails\": {\n            \"outBounding\": false,\n            \"EmailAddress\": \"david.cook@bglgroup.co.uk\",\n            \"MainTelephoneNumber\": \"\"\n        },\n        \"ProposerId\": \"880ccd88-55b1-47a3-b709-5916f3386e00\",\n        \"People\": [\n            {\n                \"MemberOfBikingOrganisation\": {\n                    \"dataCode\": \"NO\",\n                    \"displayText\": \"None\"\n                },\n                \"AdditionalMotorQualification\": {\n                    \"dataCode\": \"DQN\",\n                    \"displayText\": \"No\"\n                },\n                \"HasLivedInUKSinceBirth\": true,\n                \"PersonId\": \"880ccd88-55b1-47a3-b709-5916f3386e00\",\n                \"LivedInUKFromDate\": \"1981-02\",\n                \"DateOfBirth\": \"1981-02-17\",\n                \"Claims\": [],\n                \"Convictions\": [],\n                \"Title\": {\n                    \"dataCode\": \"MR\",\n                    \"displayText\": \"Mr\"\n                },\n                \"FirstName\": \"dave\",\n                \"LastName\": \"cook\",\n                \"Address\": {\n                    \"formattedAddress\": {\n                        \"line1\": \"47 Main Street\",\n                        \"postcode\": \"PE8 6PR\",\n                        \"line2\": \"Yarwell\",\n                        \"line3\": \"Peterborough\",\n                        \"line4\": \"\",\n                        \"line5\": \"\"\n                    },\n                    \"postOfficeAddress\": {\n                        \"organisationName\": \"\",\n                        \"department\": \"\",\n                        \"subBuilding\": \"\",\n                        \"building\": \"\",\n                        \"number\": \"47\",\n                        \"dependentThoroughfare\": \"\",\n                        \"thoroughfare\": \"Main Street\",\n                        \"doubleDependentLocality\": \"\",\n                        \"dependentLocality\": \"Yarwell\",\n                        \"town\": \"Peterborough\",\n                        \"postalCounty\": \"\",\n                        \"postcode\": \"PE8 6PR\",\n                        \"abbreviatedPostalCounty\": \"\",\n                        \"optionalCounty\": \"Cambridgeshire\",\n                        \"abbreviatedOptionalCounty\": \"Cambs\",\n                        \"traditionalCounty\": \"Northamptonshire\",\n                        \"administrativeCounty\": \"Northamptonshire\",\n                        \"dps\": \"3U\"\n                    }\n                },\n                \"MaritalStatus\": {\n                    \"dataCode\": \"M\",\n                    \"displayText\": \"Married\"\n                },\n                \"PrimaryOccupation\": {\n                    \"dataCode\": \"U03\",\n                    \"displayText\": \"Unemployed\"\n                },\n                \"PrimaryOccupationBusinessType\": {\n                    \"dataCode\": \"186\",\n                    \"displayText\": \"Not In Employment\"\n                },\n                \"EmploymentStatus\": {\n                    \"dataCode\": \"U\",\n                    \"displayText\": \"Unemployed\"\n                },\n                \"HasPassedCBT\": true,\n                \"DrivingLicenceType\": {\n                    \"dataCode\": \"FM\",\n                    \"displayText\": \"Full UK Motorcycle\"\n                },\n                \"NumberOfYearsLicenceHeld\": 10,\n                \"UseOfAnyOtherVehicle\": {\n                    \"dataCode\": \"UVE\",\n                    \"displayText\": \"Have use of another car\"\n                },\n                \"OwnsHome\": true,\n                \"HasSameAddressAsProposer\": true,\n                \"HasChildrenUnderSixteen\": null,\n                \"HasNonMotorConvictions\": false\n            }\n        ],\n        \"Vehicle\": {\n            \"AlarmCode\": {\n                \"dataCode\": \"NONE\",\n                \"displayText\": \"None\"\n            },\n            \"SecondaryAlarmCode\": {\n                \"dataCode\": \"NONE\",\n                \"displayText\": \"None\"\n            },\n            \"Imported\": false,\n            \"Modifications\": [],\n            \"VehicleNotPurchasedYet\": true\n        },\n        \"VehicleUsage\": {\n            \"ValueOfAccessories\": 0\n        },\n        \"YearsContinuouslyInsured\": 10,\n        \"AdditionalRiderIds\": [],\n        \"PolicyId\": \"123456\"\n    },\n    \"metadata\": {\n        \"CreatedAt\": \"2018-01-18T11:15:29.0858844Z\",\n        \"ModifiedAt\": \"2018-01-18T14:28:30.0576739Z\"\n    },\n    \"enquiryFeatures\": [\n        {\n            \"code\": \"Breakdown\",\n            \"value\": true\n        },\n        {\n            \"code\": \"other\",\n            \"value\": true\n        }\n    ]\n}";
        
    }
}

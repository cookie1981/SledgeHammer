using MicroMachines.Common.Mongo.Models;
using MongoDB.Driver;

namespace profiling.Configuration
{
    public class RiskCaptureMongoConfiguration : IMongoClientConfiguration
    {
        public string MongoConnection { get; set; }
        public int? Timeout { get; set; } = 3000;
        public string ServiceName { get; } = "Mongo DB";
        public MongoUrl MongoUrl => new MongoUrl(MongoConnection);
        public string DatabaseName { get; } = "RiskCapture";
        public string CollectionName { get; } = "WIPRisks";
    }
}
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
//using RiskCapture.Storage.Mongo;
using IHaveAUniqueId = profiling.Storage.Mongo.IHaveAUniqueId;

namespace profiling.Models
{
    public class WipRisk : IHaveAUniqueId
    {
        private string _product;
        private string _version;

        [BsonId]
        public string Id { get; set; }

        public string Product
        {
            get => _product;
            set => _product = value.ToLowerInvariant();
        }

        public string Version
        {
            get => _version;
            set => _version = value.ToLowerInvariant();
        }

        public Guid SessionId { get; set; }

        public Guid VisitorId { get; set; }

        public BsonDocument Payload { get; set; }

        public BsonDateTime ModifiedAt { get; set; }

        public int WipVersion { get; set; }
    }
}
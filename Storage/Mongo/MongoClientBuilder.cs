using System;
using MicroMachines.Common.Mongo.Models;
using MongoDB.Driver;

namespace profiling.Storage.Mongo
{
    public class MongoClientBuilder : profiling.Storage.Mongo.IMongoClientBuilder
    {
        private readonly TimeSpan _defaultTimeout = TimeSpan.FromMilliseconds(5000);

        public IMongoClient Build(IMongoClientConfiguration mongoConfig)
        {
            var mongoTimeout = DetermineTimeout(mongoConfig);

            var settings = MongoClientSettings.FromUrl(mongoConfig.MongoUrl);
            settings.ConnectTimeout = mongoTimeout;
            settings.SocketTimeout = mongoTimeout;
            settings.ServerSelectionTimeout = mongoTimeout;

            return new MongoClient(settings);
        }

        private TimeSpan DetermineTimeout(IMongoClientConfiguration configuration)
        {
            return configuration.Timeout == null ? _defaultTimeout : TimeSpan.FromMilliseconds(configuration.Timeout.Value);
        }
    }
}
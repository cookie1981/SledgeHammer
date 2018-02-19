using MongoDB.Driver;
using profiling.Configuration;

namespace profiling.Storage.Mongo
{
    public class MongoCollectionProvider<T> : IMongoCollectionProvider<T>
    {
        private RiskCaptureMongoConfiguration _mongoConfig;
        private IMongoDatabase _database;

        public MongoCollectionProvider(RiskCaptureMongoConfiguration mongoConfig, IMongoClientBuilder mongoClientBuilder)
        {
            _mongoConfig = mongoConfig;
            var client = mongoClientBuilder.Build(_mongoConfig);
            _database = client.GetDatabase(_mongoConfig.DatabaseName);
        }

        ~MongoCollectionProvider()
        {
            _database = null;
            _mongoConfig = null;
        }

        public IMongoCollection<T> GetCollection()
        {
            return _database.GetCollection<T>(_mongoConfig.CollectionName);
        }

        public IMongoDatabase Database => _database;
    }
}
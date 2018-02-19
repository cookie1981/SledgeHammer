using MicroMachines.Common.Mongo.Models;
using MongoDB.Driver;

namespace profiling.Storage.Mongo
{
    public interface IMongoClientBuilder
    {
        IMongoClient Build(IMongoClientConfiguration mongoConfig);
    }
}
using MongoDB.Driver;

namespace profiling.Storage.Mongo
{
    public interface IMongoCollectionProvider<T>
    {
        IMongoCollection<T> GetCollection();
        IMongoDatabase Database { get; }
    }
}
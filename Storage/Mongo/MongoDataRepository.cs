using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
//using RiskCapture.Storage.Mongo;

namespace profiling.Storage.Mongo
{
    public class MongoDataRepository<T> : IDataRepository<T> where T : class, IHaveAUniqueId
    {
        private IMongoCollection<T> _collection;

        public MongoDataRepository(IMongoCollectionProvider<T> mongoCollectionProvider)
        {
            _collection = mongoCollectionProvider.GetCollection();
        }

        ~MongoDataRepository() => _collection = null;

        public async Task SaveAsync(T objectToSave)
        {
            await _collection.InsertOneAsync(objectToSave);
        }

        public async Task<T> FetchAsync(string wipId)
        {
            T result;

            using (var response = await _collection.FindAsync(x => x.Id == wipId))
            {
                result = await response.FirstOrDefaultAsync();
            }

            return result;
        }

        public async Task<T> UpdateAsync(T objectToUpdate)
        {
            return await _collection.FindOneAndReplaceAsync(x => x.Id == objectToUpdate.Id, objectToUpdate);
        }
    }

    public interface IHaveAUniqueId
    {
        string Id { get; }
    }
} 
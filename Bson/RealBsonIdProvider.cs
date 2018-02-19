using MongoDB.Bson;

namespace profiling.Bson
{
    public class RealBsonIdProvider : profiling.Bson.IBsonIdGenerator
    {
        public string GenerateId()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}
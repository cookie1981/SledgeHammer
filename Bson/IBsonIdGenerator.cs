namespace profiling.Bson
{
    public interface IBsonIdGenerator
    {
        string GenerateId();
    }

    public class FakeBsonIdProvider : IBsonIdGenerator
    {
        public string GenerateId()
        {
            return "NotExactlyUniqueId";
        }
    }
}
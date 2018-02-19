using System;

namespace profiling.Storage.Mongo
{
    public class DataAccessException : Exception
    {
        public DataAccessException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
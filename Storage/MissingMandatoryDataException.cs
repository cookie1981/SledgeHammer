using System;

namespace profiling.Storage
{
    public class MissingMandatoryDataException : Exception
    {
        public MissingMandatoryDataException(string message ) : base(message)
        {
        }
    }
}
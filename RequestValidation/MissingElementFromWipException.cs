using System;

namespace profiling.RequestValidation
{
    public class MissingElementFromWipException : Exception
    {
        public MissingElementFromWipException(string message) : base(message)
        {
        }
    }
}
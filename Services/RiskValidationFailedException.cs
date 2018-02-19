using System;

namespace profiling.Services
{
    public class RiskValidationFailedException : Exception
    {
        public RiskValidationFailedException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
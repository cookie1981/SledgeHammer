using System;
//using RiskCapture.DateTimeProvider;

namespace profiling.DateTimeProvider
{
    public class RealDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
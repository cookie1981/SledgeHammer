using System;

namespace profiling.DateTimeProvider
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }

    public class FakeDateTimeProvider : IDateTimeProvider
    {
        private DateTime now;

        public DateTime Now
        {
            get
            {
                if (now == DateTime.MinValue)
                {
                    throw new Exception("You didn't set the Now Date on the FakeDateTimeProvider.");
                }

                return this.now;
            }
            set { this.now = value; }
        }

        public FakeDateTimeProvider()
        {
        }

        public FakeDateTimeProvider(DateTime now)
        {
            Now = now;
        }
    }
}
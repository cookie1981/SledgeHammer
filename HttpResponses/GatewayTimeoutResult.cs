using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace profiling.HttpResponses
{
    public class GatewayTimeoutResult : StatusCodeResult
    {
        public GatewayTimeoutResult()
            : base(StatusCodes.Status504GatewayTimeout)
        {
        }
    }
}
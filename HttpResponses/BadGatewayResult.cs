using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace profiling.HttpResponses
{
    public class BadGatewayResult : StatusCodeResult
    {
        public BadGatewayResult()
            : base(StatusCodes.Status502BadGateway)
        {
        }
    }
}
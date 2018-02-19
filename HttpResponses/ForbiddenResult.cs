using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace profiling.HttpResponses
{
    public class ForbiddenResult : StatusCodeResult
    {
        public ForbiddenResult()
            : base(StatusCodes.Status403Forbidden)
        {
        }
    }
}
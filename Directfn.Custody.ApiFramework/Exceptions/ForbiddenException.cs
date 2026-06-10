using Microsoft.AspNetCore.Http;

namespace Directfn.Custody.ApiFramework.Exceptions
{
    public sealed class ForbiddenException : AppException
    {
        public ForbiddenException(string errorCode, string message) : base(errorCode, message, StatusCodes.Status403Forbidden)
        {
        }
    }
}

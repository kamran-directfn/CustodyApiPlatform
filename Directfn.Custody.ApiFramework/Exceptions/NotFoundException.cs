using Microsoft.AspNetCore.Http;

namespace Directfn.Custody.ApiFramework.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string errorCode, string message) : base(errorCode, message, StatusCodes.Status404NotFound)
        {
        }
    }
}
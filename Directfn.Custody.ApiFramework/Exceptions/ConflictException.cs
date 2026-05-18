using Microsoft.AspNetCore.Http;

namespace Directfn.Custody.ApiFramework.Exceptions;

public sealed class ConflictException : AppException
{
    public ConflictException(string errorCode, string message)
        : base(errorCode, message, StatusCodes.Status409Conflict)
    {
    }
}
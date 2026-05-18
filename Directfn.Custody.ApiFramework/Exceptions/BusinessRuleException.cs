using Microsoft.AspNetCore.Http;

namespace Directfn.Custody.ApiFramework.Exceptions;

public sealed class BusinessRuleException : AppException
{
    public BusinessRuleException(string errorCode, string message)
        : base(errorCode, message, StatusCodes.Status422UnprocessableEntity)
    {
    }
}
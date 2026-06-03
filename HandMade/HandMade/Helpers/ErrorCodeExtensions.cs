using HandMade.Application.Shared;
using HandMade.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HandMade.Helpers
{
    public static class ErrorCodeExtensions
    {
        public static int ToStatusCode(this ErrorCode errorCode)
        {
            return errorCode switch
            {
                ErrorCode.UserNotFound => StatusCodes.Status404NotFound,
                ErrorCode.NoRolesFound => StatusCodes.Status404NotFound,
                ErrorCode.EmailAlreadyExists => StatusCodes.Status409Conflict,
                ErrorCode.UserNameAlreadyExists => StatusCodes.Status409Conflict,
                ErrorCode.ThisUserAlreadyHasThisRole => StatusCodes.Status409Conflict,
                _ => MapByRange(errorCode)
            };
        }

        private static int MapByRange(ErrorCode errorCode)
        {
            int code = (int)errorCode;
            if (code is >= 1001 and <= 1999) return StatusCodes.Status401Unauthorized;
            if (code is >= 2001 and <= 2999) return StatusCodes.Status400BadRequest;
            if (code is >= 3001 and <= 3999) return StatusCodes.Status409Conflict;
            if (code is >= 4001 and <= 4999) return StatusCodes.Status400BadRequest;
            if (code is >= 5001 and <= 5999) return StatusCodes.Status401Unauthorized;
            return StatusCodes.Status400BadRequest;
        }

        public static ObjectResult ToProblem(this ErrorCode errorCode,string detail,string? instance = null)
        {
            var problem = new ProblemDetails
            {
                Title = errorCode.ToString(),
                Detail = detail,
                Status = errorCode.ToStatusCode(),
                Instance = instance
            };
            problem.Extensions["errorCode"] = (int)errorCode;
            return new ObjectResult(problem) { StatusCode = problem.Status };
        }
    }
}

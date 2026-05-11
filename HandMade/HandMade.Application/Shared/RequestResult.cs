using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Shared
{
    public record RequestResult<TResult>(TResult Data, bool IsSuccess, ErrorCode ErrorCode)
    {
        public static RequestResult<TResult> Success(TResult data)
        {
            return new RequestResult<TResult>(data, true, ErrorCode.None);
        }

        public static RequestResult<TResult> Faild(ErrorCode errorCode)
        {
            return new RequestResult<TResult>(default, false, errorCode);
        }
    }
}

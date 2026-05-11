using HandMade.Application.Shared;

namespace HandMade.ViewModels
{
    public class ResponseViewModel<T>
    {
        public T? Data { get; set; }
        public ErrorCode ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }

        public static ResponseViewModel<T> Success(T? data)
        {
            return new ResponseViewModel<T>
            {
                Data = data,
                ErrorMessage = null,
                ErrorCode = ErrorCode.None
            };
        }

        public static ResponseViewModel<T> Faild(ErrorCode errorCode, string? errorMessage = null)
        {
            return new ResponseViewModel<T>
            {
                Data = default,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode
            };
        }
    }
}
